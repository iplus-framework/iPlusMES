# pack-local.ps1
#
# Packs all packable gip.* projects of iPlusMES into .\local-packages
# (the local NuGet feed used by consumer solutions, see nuget.config there).
#
# Usage:  .\build\pack-local.ps1 [-Configuration Debug|Release] [-Projects ProjectName ...]
#         .\build\pack-local.ps1 -Configuration Release -Projects gip.mes.datamodel gip.bso.manufacturing
#
# Windows equivalent of pack-local.sh.

param(
    [ValidateSet("Debug", "Release")]
    [string]$Configuration = "Release",

    # Optional: pack only the named projects (without .csproj extension).
    [Parameter(ValueFromRemainingArguments = $true)]
    [string[]]$Projects
)

$ErrorActionPreference = "Continue"

$Root = Split-Path -Parent $PSScriptRoot
$Out = Join-Path $Root "local-packages"
New-Item -ItemType Directory -Force -Path $Out | Out-Null

# Tooling / executables that must not be packed (gip.bso.test IS packed intentionally)
$Exclude = @(
    "gip.mes.cmdlet", "tcat.mes.processapplication"
)

$Failed = 0

function Pack-Project {
    param([string]$Csproj)

    $name = [System.IO.Path]::GetFileNameWithoutExtension($Csproj)
    if ($name -like "*Backup*") { return }
    # skip legacy (non-SDK) projects - they cannot be packed
    if (-not (Select-String -Path $Csproj -Pattern "<Project Sdk" -Quiet)) {
        Write-Host "==> Skipping $name (non-SDK project)"
        return
    }
    if ($Exclude -contains $name) { return }

    Write-Host "==> Packing $name ($Configuration)"
    # Packages are consumed via the iPlus.* NuGet packages (iPlus.Avalonia,
    # iPlus.Xaml.Behaviors.*), so they must be built with UseAvaloniaFork=false.
    # Otherwise compiled type references would bind to the fork's merged
    # Xaml.Behaviors.dll / fork Avalonia assemblies, which do not exist in a
    # NuGet deployment.
    dotnet pack $Csproj -c $Configuration -o $Out -p:UseAvaloniaFork=false
    if ($LASTEXITCODE -ne 0) {
        Write-Host "==> FAILED: $name" -ForegroundColor Red
        $script:Failed = 1
    }
}

Set-Location $Root

if ($Projects) {
    foreach ($name in $Projects) {
        Pack-Project (Join-Path $Root "$name/$name.csproj")
    }
}
else {
    $csprojs = Get-ChildItem -Path $Root -Recurse -Depth 3 -Filter *.csproj |
        Where-Object { $_.FullName -notmatch '\\(bin|obj|local-packages)\\' }
    foreach ($item in $csprojs) {
        Pack-Project $item.FullName
    }
}

if ($Failed -ne 0) {
    Write-Host "Some projects FAILED to pack" -ForegroundColor Red
    exit 1
}
Write-Host "Done. Packages written to $Out" -ForegroundColor Green
