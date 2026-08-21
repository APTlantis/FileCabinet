param(
    [string]$Configuration = "Release",
    [string]$Platform = "x64",
    [string]$ManifestPath = "Package.appxmanifest"
)

$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot
$shellProject = Join-Path $repoRoot "FilingCabinet.ShellExtension\FilingCabinet.ShellExtension.vcxproj"
$appProject = Join-Path $repoRoot "FilingCabinet.vbproj"
$resolvedManifest = Join-Path $repoRoot $ManifestPath
$msBuildCandidates = @(
    "C:\Program Files (x86)\Microsoft Visual Studio\2022\BuildTools\MSBuild\Current\Bin\amd64\MSBuild.exe",
    "C:\Program Files\Microsoft Visual Studio\2022\BuildTools\MSBuild\Current\Bin\amd64\MSBuild.exe",
    "C:\Program Files\Microsoft Visual Studio\18\Community\MSBuild\Current\Bin\amd64\MSBuild.exe"
)

$msBuildPath = $msBuildCandidates | Where-Object { Test-Path -LiteralPath $_ } | Select-Object -First 1
if ([string]::IsNullOrWhiteSpace($msBuildPath)) {
    throw "MSBuild.exe with Visual C++ targets was not found. Install Visual Studio Build Tools with the Desktop development with C++ workload."
}

& $msBuildPath $shellProject /p:Configuration=$Configuration /p:Platform=$Platform /m
dotnet build $appProject --configuration $Configuration --no-restore
winapp pack (Join-Path $repoRoot "bin\$Configuration\net10.0-windows") --manifest $resolvedManifest
