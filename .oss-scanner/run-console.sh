#!/usr/bin/env bash
#
# OSS Scanner helper: run the headless MES console against a local SQL Server.
#
#   ./.oss-scanner/run-console.sh [seconds]
#
# Steps:
#   1. starts a local SQL Server (mssql-server is pre-installed in the image),
#   2. restores every SQL Server backup found in Database/*.7z
#      (IPCDrivesV5.bak and iPlusMESV5.bak; skips databases that already exist),
#   3. writes vbiplus.config into the console build output so the console
#      connects to localhost. Like the committed ConnectionStrings.config of
#      the MES client it points BOTH entity connection strings (iPlusV5_Entities
#      / iPlusMESV5_Entities) at the iPlusMESV5 catalog,
#   4. applies pending DbSyncer scripts (the console skips the built-in
#      DbSyncer), and
#   5. runs gip.mes.console (/U00 /P00) for the given number of seconds
#      (default: 60). The pipe keeps stdin open (the console otherwise exits
#      immediately when stdin is closed); SIGTERM shuts it down.
#
# Requires ~2 GB of free RAM for sqlservr.
set -euo pipefail

REPO_ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
BIN_DIR="${IPLUS_CONSOLE_BIN:-$REPO_ROOT/bin/Release/net10.0}"
RUN_SECONDS="${1:-60}"
export ACCEPT_EULA=Y
export MSSQL_SA_PASSWORD='Netspirit!2026'
# catalog that holds @DbSyncerInfo and the sample data for this product
DEFAULT_CATALOG=iPlusMESV5

DB_DATA_DIR=/tmp/mssql-data
mkdir -p "$DB_DATA_DIR" /tmp/mssql-db

log() { echo "[run-console] $*"; }

# --- 1. start SQL Server -------------------------------------------------------
if ! (echo > /dev/tcp/127.0.0.1/1433) 2>/dev/null; then
    log "starting local SQL Server ..."
    /opt/mssql/bin/sqlservr > /tmp/sqlserver.log 2>&1 &
    for i in $(seq 1 60); do
        if sqlcmd -S localhost -U sa -P "$MSSQL_SA_PASSWORD" -C -b -o /dev/null -Q "SELECT 1" 2>/dev/null; then
            log "SQL Server is up"
            break
        fi
        [ "$i" -eq 60 ] && { log "SQL Server did not come up:"; tail -50 /tmp/sqlserver.log; exit 1; }
        sleep 2
    done
else
    log "SQL Server already running"
fi

# --- 2. restore the iPlusMESV5 sample database ------------------------------------
# MES is scanned standalone: the framework DB (iPlusV5 in /iPlus/Database) is a
# git-lfs pointer file in the shallow GitHub clone and is not used - all entity
# connections go to the iPlusMESV5 catalog, see vbiplus.config below.
for archive in "$REPO_ROOT"/Database/*.7z; do
    [ -e "$archive" ] || continue
    7z e -y -o/tmp/mssql-db "$archive" >/dev/null
done

for bak in /tmp/mssql-db/*.bak; do
    [ -e "$bak" ] || break
    dbname="$(basename "$bak" .bak)"
    exists="$(sqlcmd -S localhost -U sa -P "$MSSQL_SA_PASSWORD" -C -h -1 -b -W -Q \
        "SET NOCOUNT ON; SELECT COUNT(*) FROM sys.databases WHERE name = '$dbname'" 2>/dev/null | tr -d '[:space:]')"
    exists="${exists:-0}"
    if [ "$exists" != "0" ]; then
        log "database $dbname already exists — skipping restore"
        continue
    fi
    log "restoring $bak as $dbname ..."
    # Build MOVE clauses from the backup's logical file names (paths from the
    # original machine usually do not exist in the container). RESTORE
    # FILELISTONLY columns: 1=LogicalName, 2=PhysicalName, 3=Type, ...
    moves=""
    while IFS='|' read -r logical physical; do
        [ -z "$logical" ] && continue
        case "${physical##*.}" in
            mdf|ldf|ndf) ext="${physical##*.}" ;;
            *) ext="mdf" ;;
        esac
        moves="$moves MOVE '$logical' TO '$DB_DATA_DIR/$dbname.$ext',"
    done < <(sqlcmd -S localhost -U sa -P "$MSSQL_SA_PASSWORD" -C -h -1 -W -s '|' -Q \
        "SET NOCOUNT ON; RESTORE FILELISTONLY FROM DISK='$bak'" 2>/dev/null \
        | grep -v '^$' | awk -F'|' '{ print $1 "|" $2 }' | sort -u)
    moves="${moves%,}"
    # shellcheck disable=SC2086
    sqlcmd -S localhost -U sa -P "$MSSQL_SA_PASSWORD" -C -b -Q \
        "RESTORE DATABASE [$dbname] FROM DISK='$bak' WITH RECOVERY, $moves" \
        || log "WARNING: restore of $dbname failed"
    log "database $dbname restored"
done

# --- 3. write vbiplus.config ------------------------------------------------------
# vbiplus.config in the working directory is preferred by CommandLineHelper over
# the exe's own config, so the console connects to localhost.
cat > "$BIN_DIR/vbiplus.config" <<EOF
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <connectionStrings>
    <add name="iPlusV5_Entities"
         connectionString="Integrated Security=True;
         Encrypt=False;
         data source=localhost;
         initial catalog=iPlusMESV5;
         Trusted_Connection=False;
         persist security info=True;
         user id=sa;
         password=$MSSQL_SA_PASSWORD;
         multipleactiveresultsets=True;
         application name=iPlus"/>
    <add name="iPlusMESV5_Entities"
         connectionString="Integrated Security=True;
         Encrypt=False;
         data source=localhost;
         initial catalog=iPlusMESV5;
         Trusted_Connection=False;
         persist security info=True;
         user id=sa;
         password=$MSSQL_SA_PASSWORD;
         multipleactiveresultsets=True;
         application name=iPlus"/>
  </connectionStrings>
</configuration>
EOF

# --- 4. apply pending DbSyncer scripts ---------------------------------------------
# gip.mes.console starts with registerACObjects=false, so the built-in DbSyncer
# (gip.core.dbsyncer SyncTask.DoSync) never runs and a sample database restored
# from Database/*.7z lacks schema changes introduced after the backup was taken
# (e.g. the VBUserInstance TLS columns added 2026-10-15 in the iPlus context).
# This reproduces the DbSyncer behavior: for every script context run each
# dbsync_*.sql whose ScriptDate (parsed from the file name, format
# yyyy-MM-dd_HH-mm_author) is newer than the context's max @DbSyncerInfo.ScriptDate,
# executing it with sqlcmd (which handles the GO batch separator like the
# syncer's Regex splitter) and registering it in @DbSyncerInfo.
DB_CATALOG="$DEFAULT_CATALOG"
for context_dir in "$BIN_DIR"/DbScripts/*/; do
    [ -d "$context_dir" ] || continue
    context="$(basename "$context_dir")"
    max_date="$(sqlcmd -S localhost -U sa -P "$MSSQL_SA_PASSWORD" -C -b -h -1 -W \
        -d "$DB_CATALOG" -Q "SET NOCOUNT ON;
            SELECT CONVERT(varchar(19), MAX(ScriptDate), 120)
            FROM [@DbSyncerInfo]
            WHERE RTRIM(DbSyncerInfoContextID) = '$context'" 2>/dev/null \
        | tr -d '[:space:]' || true)"
    for script in "$context_dir"dbsync_*.sql; do
        [ -e "$script" ] || break
        base="$(basename "$script" .sql)"
        IFS='_' read -r _ script_date script_time author <<<"$base"
        script_dt="${script_date} ${script_time/-/:}"
        if [ -n "$max_date" ] && [[ ! "$script_dt" > "$max_date" ]]; then
            continue
        fi
        log "applying $context/$base (db max: ${max_date:-none}) ..."
        if sqlcmd -S localhost -U sa -P "$MSSQL_SA_PASSWORD" -C -b \
            -d "$DB_CATALOG" -i "$script" > /tmp/dbsync-apply.log 2>&1; then
            now="$(date '+%Y-%m-%d %H:%M:%S')"
            sqlcmd -S localhost -U sa -P "$MSSQL_SA_PASSWORD" -C -b -d "$DB_CATALOG" -Q \
                "SET NOCOUNT ON; INSERT INTO [@DbSyncerInfo]
                 ([DbSyncerInfoContextID], [ScriptDate], [UpdateDate], [UpdateAuthor])
                 VALUES ('$context', convert(datetime, '$script_dt', 120),
                         convert(datetime, '$now', 120), '$author')" \
                > /dev/null 2>&1 || log "WARNING: could not record $base in @DbSyncerInfo"
        else
            log "WARNING: $base failed:"; cat /tmp/dbsync-apply.log
        fi
    done
done

# --- 5. run the console -----------------------------------------------------------
log "running $BIN_DIR/gip.mes.console for ${RUN_SECONDS}s ..."
cd "$BIN_DIR"
# Keep stdin open: gip.mes.console waits for Console.ReadLine() after startup and
# would exit immediately with a closed stdin.
(tail -f /dev/null | ./gip.mes.console "/U00" "/P00" > /tmp/gip-console.out 2>&1) &
pipeline=$!
sleep "$RUN_SECONDS"
consolepid="$(pgrep -f 'gip\.mes\.console' | head -1 || true)"
if [ -n "$consolepid" ]; then
    log "console is running (pid $consolepid) — stopping"
    kill -TERM "$consolepid" 2>/dev/null || true
    sleep 5
    kill -KILL "$consolepid" 2>/dev/null || true
fi
kill "$pipeline" 2>/dev/null || true
log "console output (last 60 lines):"
tail -60 /tmp/gip-console.out
