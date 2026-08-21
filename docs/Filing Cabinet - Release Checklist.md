# Filing Cabinet - Release Checklist

## v0.1.1

Release status: `microsoft-store-package-accepted`

Current DRS status:

- [x] Store candidate MSIX exists: `Aptlantis.FilingCabinet_0.1.1.0_x64.msix`.
- [x] ARHS-compatible Release Hasher manifest exists for the MSIX: `FilingCabinet-0.1.1.0.hashmanifest.toml`.
- [x] Detached PGP signature exists for the hash manifest: `FilingCabinet-0.1.1.0.hashmanifest.toml.asc`.
- [x] Detached SLH-DSA signature exists for the hash manifest: `FilingCabinet-0.1.1.0.hashmanifest.toml.sphincs`.
- [x] Microsoft Store product identity reserved as `Filing Cabinet` / `Aptlantis.FilingCabinet`.
- [x] Store-aligned `Package.appxmanifest` exists with `Package/Identity/Name = Aptlantis.FilingCabinet`, `Package/Identity/Publisher = CN=81D6747D-F84F-4EFF-ACAA-9635D91ACCD0`, `DisplayName = Filing Cabinet`, `PublisherDisplayName = Aptlantis`, and `uap:VisualElements DisplayName = Filing Cabinet`.
- [ ] Rebuild and rehash the Store MSIX after the packaged Explorer context-menu extension is included.
- [x] Store candidate package accepted by Partner Center package validation.
- [x] Product naming and documentation cleanup completed before final release candidate.
- [x] Final accepted package hashed with Release Hasher after Partner Center accepts the package.
- [ ] Public Store certification completed through Partner Center.
- [ ] Microsoft Store-signed public package retrieved or verified.
- [ ] MSIX install, launch, update, uninstall, and shell-integration behavior independently verified by Codex.
- [ ] Store release notes, screenshots, privacy/category declarations, and package identity reviewed.

Store candidate MSIX evidence:

* Path: `Aptlantis.FilingCabinet_0.1.1.0_x64.msix`
* Package version: `0.1.1.0`
* Architecture: X64
* Device family: Windows.Desktop min version `10.0.18362.0`
* Language: `en-us`
* Capabilities: `runFullTrust`, `Microsoft.storeFilter.core.notSupported_8wekyb3d8bbwe`
* Local size: `9900554` bytes
* Store-reported size: `9.4 MB`
* Hash manifest: `FilingCabinet-0.1.1.0.hashmanifest.toml`
* SHA-256: `0ec8e3bdfbd391da466945f3acc09836989fe43ff9f31852f33b73568c592694`
* BLAKE3-256: `adb67602c9b0faa0566bbf5ddeb84de3ddb409bdaf981290a6bdeb4dbdc33b1d`
* Signing authority: Microsoft Store signs the distributed package after certification/publication.
* Detached manifest signatures: `FilingCabinet-0.1.1.0.hashmanifest.toml.asc`, `FilingCabinet-0.1.1.0.hashmanifest.toml.sphincs`

Reserved Store identity:

* Product name: `Filing Cabinet`
* Package identity name: `Aptlantis.FilingCabinet`
* Publisher: `CN=81D6747D-F84F-4EFF-ACAA-9635D91ACCD0`
* Publisher display name: `Aptlantis`
* Package family name: `Aptlantis.FilingCabinet_jfrcsngvdwx7g`
* Package SID: `S-1-15-2-1860814627-1124911970-2441074946-1662115893-3861747353-1868345528-3456606545`
* Store ID: `9N29X9KR70R3`
* First submission method: Partner Center website upload
* Package validation result: Partner Center accepted `Aptlantis.FilingCabinet_0.1.1.0_x64.msix`.
* Current blocker: public release still requires any remaining certification/publication steps, verification of the Microsoft Store-signed distributed package, and install/launch/update/uninstall checks.

Historical/local MSI evidence:

- [x] Version number matches project file, CLI project, manifest, and installer package version.
- [x] MIT license recorded in the project manifest, JSON manifest, top-level `LICENSE`, README, and source docs set.
- [x] Source release note written: `docs/Filing Cabinet v0.1.1 - Compliance Alignment Patch.md`.
- [x] Installer documentation payload rebuilt from the current canonical `docs/` directory.
- [x] Source build completed through `installer/build-installer.ps1 -Version 0.1.1.0`.
- [x] Tests passed: `dotnet test FilingCabinet.Tests/FilingCabinet.Tests.vbproj --configuration Release --no-restore`.
- [x] Installer build completed: `installer/build-installer.ps1 -Version 0.1.1.0`.
- [x] Release Hasher manifest recorded SHA-256, BLAKE3-256, and KT128 for the final installer artifact.
- [x] Source release note, packaged docs, checklist, project manifest, JSON manifest, and hash manifest synchronized.
- [x] Signing status recorded as unsigned / `NotSigned`.

Verification evidence:

* Tests: `dotnet test FilingCabinet.Tests/FilingCabinet.Tests.vbproj --configuration Release --no-restore`
* Installer build: `installer/build-installer.ps1 -Version 0.1.1.0`
* Release Hasher: `artifacts/installer/FilingCabinet_msi-0.1.1.0.hashmanifest.toml`
* Authenticode: unsigned / `NotSigned`

Historical MSI artifact:

* Path: `artifacts/installer/FilingCabinet-0.1.1.0-win-x64.msi`
* Runtime: `win-x64`
* Package version: `0.1.1.0`
* Size: recorded in the hash manifest
* SHA-256: recorded in the hash manifest
* BLAKE3-256: recorded in the hash manifest
* KT128: recorded in the hash manifest
* Hash manifest: `artifacts/installer/FilingCabinet_msi-0.1.1.0.hashmanifest.toml`
* License: `MIT`
* Build date: `2026-08-20`

## v0.1.0

Release status: `local-verified`

- [x] Version number matches project file, CLI project, manifest, and installer package version.
- [x] Historical v1.x installer artifacts moved out of the active release path.
- [x] `docs/` reduced to the curated core set (v0.1.0 reset); durable content from retired release notes folded into standing docs first.
- [x] Source build completed: `dotnet build FilingCabinet.slnx --configuration Release --no-incremental`.
- [x] Tests passed: `dotnet test FilingCabinet.Tests/FilingCabinet.Tests.vbproj --configuration Release` passed 104 tests.
- [x] Installer build completed: `installer/build-installer.ps1 -Version 0.1.0.0`.
- [x] CLI version verified from publish output as `FilingCabinet.Cli 0.1.0`.
- [x] Launch verified from publish output with window title `Filing Cabinet - Personal Vault & Artifact Manager`.
- [x] SHA-256 recorded for final installer artifact.
- [x] Release Hasher manifest recorded SHA-256, BLAKE3-256, and KT128 for the final installer artifact.
- [x] MIT license recorded in the project manifest, top-level `LICENSE`, README, and source docs set.
- [x] Signing status recorded as unsigned / `NotSigned`.
- [x] Full MSI install/uninstall lifecycle verified on 2026-08-17 using the built MSI artifact.
- [x] Installed shell integration verified: `Copy to Filing Cabinet` and `Move to Filing Cabinet` verbs pointed to the installed executable.
- [x] Installed CLI verified as `FilingCabinet.Cli 0.1.0`.
- [x] Installed app launch verified with window title `Filing Cabinet - Personal Vault & Artifact Manager`.
- [x] MSI uninstall removed the install folder, Start Menu shortcut folder, public desktop shortcut, shell verb registry keys, and uninstall registry entry.

MSI lifecycle evidence:

* Install log: `artifacts/verification/msi-install-0.1.0.0.log`
* Uninstall log: `artifacts/verification/msi-uninstall-0.1.0.0.log`
* Observed install path: `C:\Program Files (x86)\Filing Cabinet`
* Observed uninstall registry view before cleanup: `HKLM\Software\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall\{4BDF2858-B4B0-47CD-A40C-336EDFA472FE}`

Final artifact:

* Path: `artifacts/installer/FilingCabinet-0.1.0.0-win-x64.msi`
* Runtime: `win-x64`
* Package version: `0.1.0.0`
* Size: `130887680`
* SHA-256: `112794C704CBAD0ABF2696E5BA962E0039059EFCC1AFB1CE218D95A0D6A764B7`
* BLAKE3-256: `f482b18f30ea6eb8881b947b8cb2caed0bb54e204d8d6785e7477c648ffaae6e`
* KT128: `2r3I4PpAdkMPh3D5o6AtzUaZc3yOKhCfVmwC9x5UCX8Nybu1uNXOiw6Yiwgwh9VqrWgIAe+BO7Z+0hLBIeauxKv0CcBRloMFIyqpJJIexd7iNaf/tIEAlAet60nEYhY8s9zjldgATQGix4GuYBK7hDTh72cN6uV5G8CB6Y8RDao=`
* Hash manifest: `artifacts/installer/FilingCabinet_msi-0.1.0.0.hashmanifest.toml`
* License: `MIT`
* Build date: `2026-08-04`


