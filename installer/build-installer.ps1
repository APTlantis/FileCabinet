param(
    [string]$Configuration = "Release",
    [string]$Runtime = "win-x64",
    [string]$Version = "0.1.0.0"
)

$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot
$projectPath = Join-Path $repoRoot "FileCabinet.vbproj"
$iconPath = Join-Path $repoRoot "Assets\FileCabinet.ico"
$publishDir = Join-Path $repoRoot "artifacts\publish\$Runtime"
$installerDir = Join-Path $repoRoot "artifacts\installer"
$wxsPath = Join-Path $installerDir "FileCabinet.wxs"
$msiPath = Join-Path $installerDir "FileCabinet-$Version-$Runtime.msi"

function ConvertTo-XmlAttributeValue {
    param([string]$Value)

    return $Value.
        Replace("&", "&amp;").
        Replace("""", "&quot;").
        Replace("<", "&lt;").
        Replace(">", "&gt;")
}

New-Item -ItemType Directory -Force -Path $publishDir, $installerDir | Out-Null

dotnet tool restore

dotnet publish $projectPath `
    -c $Configuration `
    -r $Runtime `
    --self-contained true `
    -p:PublishSingleFile=true `
    -p:IncludeNativeLibrariesForSelfExtract=true `
    -p:EnableCompressionInSingleFile=true `
    -p:DebugType=None `
    -p:DebugSymbols=false `
    -o $publishDir

$exePath = Join-Path $publishDir "FileCabinet.exe"
if (-not (Test-Path $exePath)) {
    throw "Published executable was not found at $exePath"
}

$escapedExePath = ConvertTo-XmlAttributeValue (Resolve-Path $exePath).Path
$escapedIconPath = ConvertTo-XmlAttributeValue (Resolve-Path $iconPath).Path

$wxs = @"
<Wix xmlns="http://wixtoolset.org/schemas/v4/wxs">
  <Package Name="FileCabinet" Manufacturer="FileCabinet" Version="$Version" UpgradeCode="{55C58397-E0A6-4DA8-A23B-BCBB1F0EC34D}" Scope="perMachine">
    <MajorUpgrade DowngradeErrorMessage="A newer version of FileCabinet is already installed." />
    <MediaTemplate EmbedCab="yes" />
    <Icon Id="FileCabinetIcon" SourceFile="$escapedIconPath" />
    <Property Id="ARPPRODUCTICON" Value="FileCabinetIcon" />

    <StandardDirectory Id="ProgramFiles64Folder">
      <Directory Id="INSTALLFOLDER" Name="FileCabinet">
        <Component Id="FileCabinetExecutable" Guid="{1D8D59EE-11DD-4854-8E98-4EF2F77099F4}">
          <File Id="FileCabinetShortcutIcon" Source="$escapedIconPath" Name="FileCabinet.ico" />
          <File Id="FileCabinetExe" Source="$escapedExePath" KeyPath="yes">
            <Shortcut Id="StartMenuShortcut" Directory="ApplicationProgramsFolder" Name="FileCabinet" WorkingDirectory="INSTALLFOLDER" Icon="FileCabinetIcon" IconIndex="0" Advertise="no" />
            <Shortcut Id="DesktopShortcut" Directory="DesktopFolder" Name="FileCabinet" WorkingDirectory="INSTALLFOLDER" Icon="FileCabinetIcon" IconIndex="0" Advertise="no" />
          </File>
          <RegistryValue Root="HKCU" Key="Software\FileCabinet" Name="InstallFolder" Type="string" Value="[INSTALLFOLDER]" />
        </Component>
      </Directory>
    </StandardDirectory>

    <StandardDirectory Id="ProgramMenuFolder">
      <Directory Id="ApplicationProgramsFolder" Name="FileCabinet" />
    </StandardDirectory>
    <StandardDirectory Id="DesktopFolder" />

    <Feature Id="Main" Title="FileCabinet" Level="1">
      <ComponentRef Id="FileCabinetExecutable" />
    </Feature>
  </Package>
</Wix>
"@

Set-Content -Path $wxsPath -Value $wxs -Encoding UTF8
dotnet wix build $wxsPath -o $msiPath

Write-Host "Installer created: $msiPath"
