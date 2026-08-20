# FileCabinet - Release Checklist

## v0.1.1

Release status: `local-verified`

- [x] Version number matches project file, CLI project, manifest, and installer package version.
- [x] MIT license recorded in the project manifest, JSON manifest, top-level `LICENSE`, README, and source docs set.
- [x] Source release note written: `docs/FileCabinet v0.1.1 - Compliance Alignment Patch.md`.
- [x] Installer documentation payload rebuilt from the current canonical `docs/` directory.
- [x] Source build completed through `installer/build-installer.ps1 -Version 0.1.1.0`.
- [x] Tests passed: `dotnet test FileCabinet.Tests/FileCabinet.Tests.vbproj --configuration Release --no-restore`.
- [x] Installer build completed: `installer/build-installer.ps1 -Version 0.1.1.0`.
- [x] Release Hasher manifest recorded SHA-256, BLAKE3-256, and KT128 for the final installer artifact.
- [x] Source release note, packaged docs, checklist, project manifest, JSON manifest, and hash manifest synchronized.
- [x] Signing status recorded as unsigned / `NotSigned`.

Verification evidence:

* Tests: `dotnet test FileCabinet.Tests/FileCabinet.Tests.vbproj --configuration Release --no-restore`
* Installer build: `installer/build-installer.ps1 -Version 0.1.1.0`
* Release Hasher: `artifacts/installer/FileCabinet_msi-0.1.1.0.hashmanifest.toml`
* Authenticode: unsigned / `NotSigned`

Final artifact:

* Path: `artifacts/installer/FileCabinet-0.1.1.0-win-x64.msi`
* Runtime: `win-x64`
* Package version: `0.1.1.0`
* Size: recorded in the hash manifest
* SHA-256: recorded in the hash manifest
* BLAKE3-256: recorded in the hash manifest
* KT128: recorded in the hash manifest
* Hash manifest: `artifacts/installer/FileCabinet_msi-0.1.1.0.hashmanifest.toml`
* License: `MIT`
* Build date: `2026-08-20`

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
- [x] Release Hasher manifest recorded SHA-256, BLAKE3-256, and KT128 for the final installer artifact.
- [x] MIT license recorded in the project manifest, top-level `LICENSE`, README, and source docs set.
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
* BLAKE3-256: `f482b18f30ea6eb8881b947b8cb2caed0bb54e204d8d6785e7477c648ffaae6e`
* KT128: `2r3I4PpAdkMPh3D5o6AtzUaZc3yOKhCfVmwC9x5UCX8Nybu1uNXOiw6Yiwgwh9VqrWgIAe+BO7Z+0hLBIeauxKv0CcBRloMFIyqpJJIexd7iNaf/tIEAlAet60nEYhY8s9zjldgATQGix4GuYBK7hDTh72cN6uV5G8CB6Y8RDao=`
* Hash manifest: `artifacts/installer/FileCabinet_msi-0.1.0.0.hashmanifest.toml`
* License: `MIT`
* Build date: `2026-08-04`
