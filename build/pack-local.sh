#!/usr/bin/env bash
#
# Packs all packable gip.* projects of iPlusMES into ./local-packages
# (the local NuGet feed used by consumer solutions, see nuget.config there).
#
# Usage:  ./build/pack-local.sh [Debug|Release] [ProjectName ...]
#         ./build/pack-local.sh Release gip.mes.datamodel gip.bso.manufacturing
#
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
CONFIG="${1:-Release}"
shift || true
OUT="$ROOT/local-packages"
mkdir -p "$OUT"

# Tooling / executables that must not be packed (gip.bso.test IS packed intentionally)
EXCLUDE="gip.mes.cmdlet tcat.mes.processapplication"

FAILED=0
cd "$ROOT"

pack_project() {
	local csproj="$1"
	local name
	name="$(basename "$csproj" .csproj)"
	case "$name" in *Backup*) return 0 ;; esac
	# skip legacy (non-SDK) projects - they cannot be packed
	grep -q "<Project Sdk" "$csproj" || { echo "==> Skipping $name (non-SDK project)"; return 0; }
	for ex in $EXCLUDE; do
		[ "$name" = "$ex" ] && return 0
	done
	echo "==> Packing $name ($CONFIG)"
	# Packages are consumed via the iPlus.* NuGet packages (iPlus.Avalonia,
	# iPlus.Xaml.Behaviors.*), so they must be built with UseAvaloniaFork=false.
	# Otherwise compiled type references would bind to the fork's merged
	# Xaml.Behaviors.dll / fork Avalonia assemblies, which do not exist in a
	# NuGet deployment.
	if ! dotnet pack "$csproj" -c "$CONFIG" -o "$OUT" -p:UseAvaloniaFork=false; then
		echo "==> FAILED: $name"
		return 1
	fi
}

if [ "$#" -gt 0 ]; then
	for name in "$@"; do
		pack_project "$ROOT/$name/$name.csproj" || FAILED=1
	done
else
	for csproj in $(find "$ROOT" -maxdepth 3 -name "*.csproj" -not -path "*/bin/*" -not -path "*/obj/*" -not -path "*/local-packages/*"); do
		[ -e "$csproj" ] || continue
		pack_project "$csproj" || FAILED=1
	done
fi

if [ "$FAILED" != "0" ]; then echo "Some projects FAILED to pack"; exit 1; fi

# Safety net: remove empty symbol packages (projects without PDBs).
# nuget.org rejects empty .snupkg files with 400, which aborts
# 'dotnet nuget push local-packages/*.nupkg'.
for snupkg in "$OUT"/*.snupkg; do
	[ -e "$snupkg" ] || continue
	if ! unzip -l "$snupkg" 2>/dev/null | grep -q '\.pdb'; then
		echo "==> Removing empty symbol package: $(basename "$snupkg")"
		rm -f "$snupkg"
	fi
done

echo "Done. Packages written to $OUT"
