# FileCabinet - Release Checklist

## v0.1.0

Release status: `local-verified`

- [x] Version number matches project file, CLI project, manifest, and installer package version.
- [x] Historical v1.x installer artifacts moved out of the active release path.
- [x] `docs/` reduced to the curated core set (v0.1.0 reset); durable content from retired release notes folded into standing docs first.
- [x] Source build completed: `dotnet build FileCabinet.slnx --configuration Release --no-incremental`.
- [x] Tests passed: `dotnet test FileCabinet.Tests/FileCabinet.Tests.vbproj --configuration Release` passed 104 tests.
- [x] Installer build completed: `installer/build-installer.ps1 -Version 0.1.0.0`.
- [x] CLI version verified from publish output as `FileCabinet.Cli 0.1.0`.
- [x] Launch verified from publish output with window title `FileCabinet - Personal Vault & Artifact Manager`.
- [x] SHA-256 recorded for final installer artifact.
- [x] Signing status recorded as unsigned / `NotSigned`.
- [x] Full MSI install/uninstall lifecycle verified on 2026-08-17 using the built MSI artifact.
- [x] Installed shell integration verified: `Copy to FileCabinet` and `Move to FileCabinet` verbs pointed to the installed executable.
- [x] Installed CLI verified as `FileCabinet.Cli 0.1.0`.
- [x] Installed app launch verified with window title `FileCabinet - Personal Vault & Artifact Manager`.
- [x] MSI uninstall removed the install folder, Start Menu shortcut folder, public desktop shortcut, shell verb registry keys, and uninstall registry entry.

MSI lifecycle evidence:

* Install log: `artifacts/verification/msi-install-0.1.0.0.log`
* Uninstall log: `artifacts/verification/msi-uninstall-0.1.0.0.log`
* Observed install path: `C:\Program Files (x86)\FileCabinet`
* Observed uninstall registry view before cleanup: `HKLM\Software\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall\{4BDF2858-B4B0-47CD-A40C-336EDFA472FE}`

Final artifact:

* Path: `artifacts/installer/FileCabinet-0.1.0.0-win-x64.msi`
* Runtime: `win-x64`
* Package version: `0.1.0.0`
* Size: `130887680`
* SHA-256: `112794C704CBAD0ABF2696E5BA962E0039059EFCC1AFB1CE218D95A0D6A764B7`
* Build date: `2026-08-04`
