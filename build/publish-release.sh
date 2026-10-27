#!/usr/bin/env bash
#
# Publishes the ready-to-run iPlus-MES binaries for every platform into
# ./artifacts/release and writes SHA-256 checksums. The output is meant to be
# attached to a GitHub Release (see .github/workflows/release.yml).
#
# This is the *binary* channel. The *source/developer* channel is
# build/pack-local.sh (NuGet packages). One artifact must not serve both
# audiences.
#
# iPlus-MES has no separate service host: gip.mes.console IS the server
# application (it hosts the WCF relay, the data access layer and the
# application manager), so the "service" platform publishes that project.
#
# Usage:
#   ./build/publish-release.sh [options]
#
# Options:
#   -v, --version <ver>     Suite version stamped into the artifact names
#                           (default: IPlusMESPackageVersion from build/iPlus.Package.props)
#   -o, --output <dir>      Output directory (default: ./artifacts/release)
#   -p, --platform <list>   Comma separated platforms to build:
#                           desktop-win,desktop-linux,browser,android,service,
#                           console,setup,all
#                           (default: desktop-win,desktop-linux,browser,service)
#                           "all" builds every platform and additionally produces
#                           the combined iplus-mes-<ver>-all.zip bundle.
#   -c, --configuration     Build configuration (default: Release)
#       --self-contained    Publish self-contained (default)
#       --no-self-contained Publish framework-dependent
#       --skip-checksums    Do not write SHA256SUMS
#       --keep-intermediate Keep the raw publish folders next to the archives
#   -h, --help              Show this help
#
# Environment:
#   IPLUS_USE_AVALONIA_FORK   true|false (default: false)
#                             false = consume the iPlus.* Avalonia packages from nuget.org
#                             (required for CI and for anyone without the fork checkout)
#   IPLUS_BUILD_ANDROID_TFM   true|false (default: false)
#                             gip.core.autocomponent adds an -android TFM on Linux; that
#                             breaks non-Android publishes, so it is disabled here.
#                             The android platform enables it automatically.
#   ANDROID_KEYSTORE_BASE64   Base64 encoded .jks/.keystore. When set, the APK is signed
#                             with that key instead of the per-machine debug key.
#   ANDROID_KEYSTORE_PASSWORD Password of the keystore.
#   ANDROID_KEY_ALIAS         Key alias inside the keystore.
#   ANDROID_KEY_PASSWORD      Password of the key (defaults to the keystore password).
#
# Requirements:
#   - .NET 10 SDK
#   - workloads: wasm-tools (browser), android (android)
#   - the iPlus repository cloned side-by-side (../iPlus): the MES projects
#     reference the framework projects directly.
#
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
CONFIG="Release"
OUT="$ROOT/artifacts/release"
PLATFORMS="desktop-win,desktop-linux,browser,service"
ALL_PLATFORMS="desktop-win,desktop-linux,browser,service,console,setup"
SELF_CONTAINED="true"
SKIP_CHECKSUMS="false"
KEEP_INTERMEDIATE="false"
VERSION=""

USE_AVALONIA_FORK="${IPLUS_USE_AVALONIA_FORK:-false}"
BUILD_ANDROID_TFM="${IPLUS_BUILD_ANDROID_TFM:-false}"

usage() {
	awk 'NR > 1 { if ($0 !~ /^#/) exit; sub(/^# ?/, ""); print }' "${BASH_SOURCE[0]}"
}

while [ "$#" -gt 0 ]; do
	case "$1" in
		-v|--version) VERSION="${2:?missing value for $1}"; shift 2 ;;
		-o|--output) OUT="${2:?missing value for $1}"; shift 2 ;;
		-p|--platform) PLATFORMS="${2:?missing value for $1}"; shift 2 ;;
		-c|--configuration) CONFIG="${2:?missing value for $1}"; shift 2 ;;
		--self-contained) SELF_CONTAINED="true"; shift ;;
		--no-self-contained) SELF_CONTAINED="false"; shift ;;
		--skip-checksums) SKIP_CHECKSUMS="true"; shift ;;
		--keep-intermediate) KEEP_INTERMEDIATE="true"; shift ;;
		-h|--help) usage; exit 0 ;;
		*) echo "Unknown option: $1" >&2; usage >&2; exit 1 ;;
	esac
done

if [ -z "$VERSION" ]; then
	VERSION="$(sed -n 's/.*<IPlusMESPackageVersion Condition=.*>\([^<]*\)<\/IPlusMESPackageVersion>.*/\1/p' \
		"$ROOT/../iPlus/build/iPlus.Package.props" | head -1)"
	VERSION="${VERSION:-5.0.0}"
fi

mkdir -p "$OUT"
WORK="$OUT/.work"
rm -rf "$WORK"
mkdir -p "$WORK"

log() { echo "==> $*"; }

# Creates a zip archive of a publish folder and removes the folder afterwards
# (unless --keep-intermediate was given).
archive() {
	local source="$1"
	local name="$2"
	local target="$OUT/$name.zip"
	log "Archiving $name"
	rm -f "$target"
	(cd "$(dirname "$source")" && zip -qr "$target" "$(basename "$source")")
	if [ "$KEEP_INTERMEDIATE" != "true" ]; then
		rm -rf "$source"
	fi
}

# Publishes a project. Extra arguments are appended to the dotnet publish call.
publish() {
	local project="$1"
	local dir="$2"
	shift 2
	dotnet publish "$ROOT/$project" \
		-c "$CONFIG" \
		-p:UseAvaloniaFork="$USE_AVALONIA_FORK" \
		-p:BuildAndroidTargetFramework="$BUILD_ANDROID_TFM" \
		-p:Version="$VERSION" \
		-p:ContinuousIntegrationBuild=true \
		-p:ErrorOnDuplicatePublishOutputFiles=false \
		-o "$dir" \
		"$@"
}

publish_desktop() {
	local rid="$1"
	local name="iplus-mes-$VERSION-desktop-$rid"
	local dir="$WORK/$name"
	log "Publishing desktop ($rid)"
	publish "gip.mes.client.avui.Desktop/gip.mes.client.avui.Desktop.csproj" "$dir" \
		-r "$rid" --self-contained "$SELF_CONTAINED" -p:PublishSingleFile=false
	archive "$dir" "$name"
}

publish_browser() {
	local name="iplus-mes-$VERSION-browser"
	local dir="$WORK/$name"
	log "Publishing browser (wasm)"
	publish "gip.mes.client.avui.Browser/gip.mes.client.avui.Browser.csproj" "$dir"
	archive "$dir" "$name"
}

publish_android() {
	local name="iplus-mes-$VERSION-android"
	local dir="$WORK/$name"
	log "Publishing android (apk)"

	# Optional stable signing. Without it the build uses the per-machine debug key,
	# which is fine for sideloading but changes on every CI run.
	local signing_args=()
	if [ -n "${ANDROID_KEYSTORE_BASE64:-}" ]; then
		local keystore="$WORK/android-signing.keystore"
		echo "$ANDROID_KEYSTORE_BASE64" | base64 -d > "$keystore"
		signing_args=(
			-p:AndroidKeyStore=true
			-p:AndroidSigningKeyStore="$keystore"
			-p:AndroidSigningStorePass="${ANDROID_KEYSTORE_PASSWORD:-}"
			-p:AndroidSigningKeyAlias="${ANDROID_KEY_ALIAS:-}"
			-p:AndroidSigningKeyPass="${ANDROID_KEY_PASSWORD:-${ANDROID_KEYSTORE_PASSWORD:-}}"
		)
		log "Signing APK with the configured keystore (alias: ${ANDROID_KEY_ALIAS:-<unset>})"
	else
		log "No ANDROID_KEYSTORE_BASE64 set — using the debug key (fine for sideloading)"
	fi

	# The Android client needs the -android TFM of gip.core.autocomponent.
	BUILD_ANDROID_TFM="true" publish \
		"gip.mes.client.avui.Android/gip.mes.client.avui.Android.csproj" "$dir" \
		-p:AndroidPackageFormats=apk "${signing_args[@]}"
	# The APK is the only artifact end users need.
	local apk
	apk="$(find "$dir" -name '*.apk' -type f | head -1)"
	if [ -z "$apk" ]; then
		echo "ERROR: no .apk produced for android" >&2
		return 1
	fi
	cp "$apk" "$OUT/$name.apk"
	if [ "$KEEP_INTERMEDIATE" != "true" ]; then
		rm -rf "$dir"
	fi
}

publish_service() {
	local rid="$1"
	local name="iplus-mes-$VERSION-service-$rid"
	local dir="$WORK/$name"
	log "Publishing service ($rid)"
	# gip.mes.console is the MES server host (there is no gip.mes.service).
	publish "gip.mes.console/gip.mes.console.csproj" "$dir" \
		-r "$rid" --self-contained "$SELF_CONTAINED"
	archive "$dir" "$name"
}

publish_console() {
	local rid="$1"
	local name="iplus-mes-$VERSION-console-$rid"
	local dir="$WORK/$name"
	log "Publishing console ($rid)"
	publish "gip.mes.console/gip.mes.console.csproj" "$dir" \
		-r "$rid" --self-contained "$SELF_CONTAINED"
	archive "$dir" "$name"
}

# Publishes the cross-platform installer/updater. It lives in the iPlus
# repository (the repositories are cloned side by side) and knows both product
# kinds, so the MES release ships the same tool.
publish_setup() {
	local rid="$1"
	local name="iplus-mes-$VERSION-setup-$rid"
	local dir="$WORK/$name"
	log "Publishing setup tool ($rid)"
	publish "../iPlus/gip.tool.installerAndUpdater/gip.tool.installerAndUpdater.csproj" "$dir" \
		-r "$rid" --self-contained "$SELF_CONTAINED" -p:PublishSingleFile=false
	archive "$dir" "$name"
}

# Unpacks every per-platform archive into one folder and zips it again, so a
# customer can download a single file instead of picking the right platform.
# The resulting zip has one folder per platform at its root (no extra wrapper).
publish_all() {
	local name="iplus-mes-$VERSION-all"
	local stage="$WORK/$name"
	log "Assembling combined bundle ($name)"
	rm -rf "$stage"
	mkdir -p "$stage"
	local found=0
	for zip in "$OUT"/iplus-mes-"$VERSION"-*.zip; do
		[ -e "$zip" ] || continue
		unzip -q "$zip" -d "$stage"
		found=1
	done
	if [ "$found" -eq 0 ]; then
		echo "ERROR: no per-platform archives to combine" >&2
		return 1
	fi
	local target="$OUT/$name.zip"
	rm -f "$target"
	(cd "$stage" && zip -qr "$target" .)
	if [ "$KEEP_INTERMEDIATE" != "true" ]; then
		rm -rf "$stage"
	fi
}

IFS=',' read -r -a REQUESTED <<<"$PLATFORMS"
WANT_ALL="false"
for platform in "${REQUESTED[@]}"; do
	[ "$platform" = "all" ] && WANT_ALL="true"
done
if [ "$WANT_ALL" = "true" ]; then
	PLATFORMS="$ALL_PLATFORMS"
	IFS=',' read -r -a REQUESTED <<<"$PLATFORMS"
fi

for platform in "${REQUESTED[@]}"; do
	case "$platform" in
		desktop-win) publish_desktop win-x64 ;;
		desktop-linux) publish_desktop linux-x64 ;;
		browser) publish_browser ;;
		android) publish_android ;;
		service) publish_service linux-x64 ;;
		console) publish_console linux-x64; publish_console win-x64 ;;
		setup) publish_setup linux-x64; publish_setup win-x64 ;;
		all) ;; # handled below
		*) echo "Unknown platform: $platform" >&2; exit 1 ;;
	esac
done

if [ "$WANT_ALL" = "true" ]; then
	publish_all
fi

if [ "$SKIP_CHECKSUMS" != "true" ]; then
	log "Writing SHA256SUMS"
	(
		cd "$OUT"
		# Collect the artifacts explicitly so that a missing platform (e.g. no .apk
		# when android was not built) does not make the checksum command fail.
		files=()
		for f in ./*.zip ./*.apk; do
			[ -e "$f" ] && files+=("$f")
		done
		if [ "${#files[@]}" -eq 0 ]; then
			echo "ERROR: no artifacts were produced" >&2
			exit 1
		fi
		# Portable checksum generation (macOS has shasum, not sha256sum).
		if command -v sha256sum >/dev/null 2>&1; then
			sha256sum "${files[@]}" | sed 's| \./| |'
		else
			shasum -a 256 "${files[@]}" | sed 's| \./| |'
		fi
	) > "$OUT/SHA256SUMS"
fi

if [ "$KEEP_INTERMEDIATE" != "true" ]; then
	rm -rf "$WORK"
fi

log "Done. Artifacts in $OUT"
ls -1 "$OUT"
