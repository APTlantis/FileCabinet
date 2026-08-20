param(
    [string]$Configuration = "Release",
    [string]$Runtime = "win-x64",
    [string]$Version = "0.1.1.0"
)

$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot
$projectPath = Join-Path $repoRoot "FilingCabinet.vbproj"
$cliProjectPath = Join-Path $repoRoot "FilingCabinet.Cli\FilingCabinet.Cli.vbproj"
$iconPath = Join-Path $repoRoot "Assets\FilingCabinet.ico"
$readmePath = Join-Path $repoRoot "README.md"
$licensePath = Join-Path $repoRoot "LICENSE"
$docsSourceDir = Join-Path $repoRoot "docs"
$publishDir = Join-Path $repoRoot "artifacts\publish\$Runtime"
$installerDir = Join-Path $repoRoot "artifacts\installer"
$wxsPath = Join-Path $installerDir "FilingCabinet.wxs"
$msiPath = Join-Path $installerDir "FilingCabinet-$Version-$Runtime.msi"

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

dotnet publish $cliProjectPath `
    -c $Configuration `
    -r $Runtime `
    --self-contained true `
    -p:PublishSingleFile=true `
    -p:IncludeNativeLibrariesForSelfExtract=true `
    -p:EnableCompressionInSingleFile=true `
    -p:DebugType=None `
    -p:DebugSymbols=false `
    -o $publishDir

$exePath = Join-Path $publishDir "FilingCabinet.exe"
if (-not (Test-Path $exePath)) {
    throw "Published executable was not found at $exePath"
}

$cliExePath = Join-Path $publishDir "FilingCabinet.Cli.exe"
if (-not (Test-Path $cliExePath)) {
    throw "Published CLI executable was not found at $cliExePath"
}

$escapedExePath = ConvertTo-XmlAttributeValue (Resolve-Path $exePath).Path
$escapedCliExePath = ConvertTo-XmlAttributeValue (Resolve-Path $cliExePath).Path
$escapedIconPath = ConvertTo-XmlAttributeValue (Resolve-Path $iconPath).Path
$escapedReadmePath = ConvertTo-XmlAttributeValue (Resolve-Path $readmePath).Path
$escapedLicensePath = ConvertTo-XmlAttributeValue (Resolve-Path $licensePath).Path
$docsIndex = 0
$docsFileElements = Get-ChildItem -LiteralPath $docsSourceDir -Filter "*.md" |
    Sort-Object Name |
    ForEach-Object {
        $docsIndex += 1
        $safeBase = ($_.BaseName -replace "[^A-Za-z0-9_]", "_")
        if ($safeBase.Length -gt 48) {
            $safeBase = $safeBase.Substring(0, 48)
        }

        $safeId = "Doc{0:D3}_{1}" -f $docsIndex, $safeBase
        $escapedPath = ConvertTo-XmlAttributeValue $_.FullName
        $escapedName = ConvertTo-XmlAttributeValue $_.Name
        $keyPath = if ($docsIndex -eq 1) { " KeyPath=""yes""" } else { "" }
        "          <File Id=""$safeId"" Source=""$escapedPath"" Name=""$escapedName""$keyPath />"
    }

$docsFilesXml = $docsFileElements -join [Environment]::NewLine

$wxs = @"
<Wix xmlns="http://wixtoolset.org/schemas/v4/wxs">
  <Package Name="FilingCabinet" Manufacturer="FilingCabinet" Version="$Version" UpgradeCode="{55C58397-E0A6-4DA8-A23B-BCBB1F0EC34D}" Scope="perMachine">
    <MajorUpgrade DowngradeErrorMessage="A newer version of FilingCabinet is already installed." />
    <MediaTemplate EmbedCab="yes" />
    <Icon Id="FilingCabinetIcon" SourceFile="$escapedIconPath" />
    <Property Id="ARPPRODUCTICON" Value="FilingCabinetIcon" />

    <StandardDirectory Id="ProgramFiles64Folder">
      <Directory Id="INSTALLFOLDER" Name="FilingCabinet">
        <Component Id="FilingCabinetExecutable" Guid="{1D8D59EE-11DD-4854-8E98-4EF2F77099F4}">
          <File Id="FilingCabinetShortcutIcon" Source="$escapedIconPath" Name="FilingCabinet.ico" />
          <File Id="FilingCabinetExe" Source="$escapedExePath" KeyPath="yes">
            <Shortcut Id="StartMenuShortcut" Directory="ApplicationProgramsFolder" Name="FilingCabinet" WorkingDirectory="INSTALLFOLDER" Icon="FilingCabinetIcon" IconIndex="0" Advertise="no" />
            <Shortcut Id="DesktopShortcut" Directory="DesktopFolder" Name="FilingCabinet" WorkingDirectory="INSTALLFOLDER" Icon="FilingCabinetIcon" IconIndex="0" Advertise="no" />
          </File>
          <File Id="FilingCabinetCliExe" Source="$escapedCliExePath" Name="FilingCabinet.Cli.exe" />
          <File Id="FilingCabinetReadme" Source="$escapedReadmePath" Name="README.md" />
          <File Id="FilingCabinetLicense" Source="$escapedLicensePath" Name="LICENSE" />
          <RegistryValue Root="HKCU" Key="Software\FilingCabinet" Name="InstallFolder" Type="string" Value="[INSTALLFOLDER]" />
          <RegistryValue Root="HKCR" Key="AllFilesystemObjects\shell\FilingCabinet.CopyToFilingCabinet" Name="MUIVerb" Type="string" Value="Copy to FilingCabinet" />
          <RegistryValue Root="HKCR" Key="AllFilesystemObjects\shell\FilingCabinet.CopyToFilingCabinet" Name="Icon" Type="string" Value="[#FilingCabinetExe]" />
          <RegistryValue Root="HKCR" Key="AllFilesystemObjects\shell\FilingCabinet.CopyToFilingCabinet\command" Type="string" Value="&quot;[#FilingCabinetExe]&quot; --copy &quot;%1&quot;" />
          <RegistryValue Root="HKCR" Key="AllFilesystemObjects\shell\FilingCabinet.MoveToFilingCabinet" Name="MUIVerb" Type="string" Value="Move to FilingCabinet" />
          <RegistryValue Root="HKCR" Key="AllFilesystemObjects\shell\FilingCabinet.MoveToFilingCabinet" Name="Icon" Type="string" Value="[#FilingCabinetExe]" />
          <RegistryValue Root="HKCR" Key="AllFilesystemObjects\shell\FilingCabinet.MoveToFilingCabinet\command" Type="string" Value="&quot;[#FilingCabinetExe]&quot; --move &quot;%1&quot;" />
        </Component>
        <Directory Id="DocsFolder" Name="docs">
          <Component Id="FilingCabinetDocs" Guid="{6896C636-BBE2-445F-8FA1-DBE3E47F7F26}">
$docsFilesXml
          </Component>
        </Directory>
      </Directory>
    </StandardDirectory>

    <StandardDirectory Id="ProgramMenuFolder">
      <Directory Id="ApplicationProgramsFolder" Name="FilingCabinet" />
    </StandardDirectory>
    <StandardDirectory Id="DesktopFolder" />

    <Feature Id="Main" Title="FilingCabinet" Level="1">
      <ComponentRef Id="FilingCabinetExecutable" />
      <ComponentRef Id="FilingCabinetDocs" />
    </Feature>
  </Package>
</Wix>
"@

Set-Content -Path $wxsPath -Value $wxs -Encoding UTF8
dotnet wix build $wxsPath -o $msiPath

Write-Host "Installer created: $msiPath"

