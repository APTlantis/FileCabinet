param(
    [string]$Configuration = "Release",
    [string]$Runtime = "win-x64",
    [string]$Version = "1.4.1.0"
)

$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot
$projectPath = Join-Path $repoRoot "FileCabinet.vbproj"
$cliProjectPath = Join-Path $repoRoot "FileCabinet.Cli\FileCabinet.Cli.vbproj"
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

$exePath = Join-Path $publishDir "FileCabinet.exe"
if (-not (Test-Path $exePath)) {
    throw "Published executable was not found at $exePath"
}

$cliExePath = Join-Path $publishDir "FileCabinet.Cli.exe"
if (-not (Test-Path $cliExePath)) {
    throw "Published CLI executable was not found at $cliExePath"
}

$escapedExePath = ConvertTo-XmlAttributeValue (Resolve-Path $exePath).Path
$escapedCliExePath = ConvertTo-XmlAttributeValue (Resolve-Path $cliExePath).Path
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
          <File Id="FileCabinetCliExe" Source="$escapedCliExePath" Name="FileCabinet.Cli.exe" />
          <RegistryValue Root="HKCU" Key="Software\FileCabinet" Name="InstallFolder" Type="string" Value="[INSTALLFOLDER]" />
          <RegistryValue Root="HKCR" Key="AllFilesystemObjects\shell\FileCabinet.CopyToFileCabinet" Name="MUIVerb" Type="string" Value="Copy to FileCabinet" />
          <RegistryValue Root="HKCR" Key="AllFilesystemObjects\shell\FileCabinet.CopyToFileCabinet" Name="Icon" Type="string" Value="[#FileCabinetExe]" />
          <RegistryValue Root="HKCR" Key="AllFilesystemObjects\shell\FileCabinet.CopyToFileCabinet\command" Type="string" Value="&quot;[#FileCabinetExe]&quot; --copy &quot;%1&quot;" />
          <RegistryValue Root="HKCR" Key="AllFilesystemObjects\shell\FileCabinet.MoveToFileCabinet" Name="MUIVerb" Type="string" Value="Move to FileCabinet" />
          <RegistryValue Root="HKCR" Key="AllFilesystemObjects\shell\FileCabinet.MoveToFileCabinet" Name="Icon" Type="string" Value="[#FileCabinetExe]" />
          <RegistryValue Root="HKCR" Key="AllFilesystemObjects\shell\FileCabinet.MoveToFileCabinet\command" Type="string" Value="&quot;[#FileCabinetExe]&quot; --move &quot;%1&quot;" />
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
