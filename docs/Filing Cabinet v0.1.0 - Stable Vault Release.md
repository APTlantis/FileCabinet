# FileCabinet v0.1.0 - Stable Vault Release

FileCabinet v0.1.0 is the first intentionally governed stable release of the local-first vault application under the current DRS release process. The historical v1.x installer train is preserved as pre-standard evidence, while this release restarts the active version line around the verified vault, catalog, preview, integrity, and repair workflows currently used in Herb's production environment.

---

## Highlights

### Stable Vault Baseline

This release establishes FileCabinet as a maintenance-ready desktop vault for retaining, cataloging, previewing, verifying, and recovering high-signal technical artifacts. The app, CLI, installer script, manifest, and release records now use the governed `0.1.0` application version and `0.1.0.0` Windows package version.

### Vault Health Workspace

Vault Health remains a dedicated workspace with overview, repair, finding, and history sections. Default health analysis is metadata-first, expensive file hash work stays explicit, and bulk repair selection separates safe automatic work from expensive or review-only findings.

### Release Evidence Reset

The old v1.x installer artifacts have been moved out of the active installer path and preserved under `artifacts/historical/pre-standard-v1x`. The current release authority is the v0.1.0 manifest, release checklist, source release note, and final artifact hash.

### Documentation Reset

`docs/` has been reduced from the full v1.x release/patch/roadmap history down to the curated set that ships with the application: this release note, the release checklist, the Blue Slate theme overview, and ten standing conceptual docs covering vault lifecycle, trust and verification, retention philosophy, repair and recovery, context preservation, determinism, hash choices, and the VB.NET/WPF platform choice. Durable decisions from the retired release notes (deliberate exclusion of PDF rendering/OCR/shell thumbnails, the always-on-hash and inactive-hash-preservation rules, and the metadata-first health analysis thresholds) were folded into the relevant standing doc before the old files were removed; changelog-style entries and forward-looking roadmap proposals were not carried forward.

---

## Design Boundaries

FileCabinet v0.1.0 intentionally does not:

* Claim that the old v1.x installer train is the active governed release line.
* Automatically restore or delete retained vault files during health analysis.
* Resolve hash mismatches without explicit operator action.
* Depend on cloud sync, accounts, or hosted storage for normal operation.

---

## Built With

* VB.NET and WPF
* .NET 10 Windows desktop runtime
* WiX Toolset 6.0.2
* MSTest
* Blake3, StreamHash, System.IO.Hashing, and BouncyCastle.Cryptography

---

## Release Artifact

Expected installer:

* `FileCabinet-0.1.0.0-win-x64.msi`

Hash manifest:

* `artifacts/installer/FileCabinet_msi-0.1.0.0.hashmanifest.toml`
* `docs/FileCabinet - Installer Hash Manifest.md`

SHA-256:

* `112794C704CBAD0ABF2696E5BA962E0039059EFCC1AFB1CE218D95A0D6A764B7`

BLAKE3-256:

* `f482b18f30ea6eb8881b947b8cb2caed0bb54e204d8d6785e7477c648ffaae6e`

KT128:

* `2r3I4PpAdkMPh3D5o6AtzUaZc3yOKhCfVmwC9x5UCX8Nybu1uNXOiw6Yiwgwh9VqrWgIAe+BO7Z+0hLBIeauxKv0CcBRloMFIyqpJJIexd7iNaf/tIEAlAet60nEYhY8s9zjldgATQGix4GuYBK7hDTh72cN6uV5G8CB6Y8RDao=`

The packaged copy of this release note shows an external-hash placeholder because release documentation is bundled inside the installer. The canonical post-build hash is recorded in the source release note, `File Cabinet.manifest.toml`, `docs/FileCabinet - Release Checklist.md`, and `artifacts/installer/FileCabinet_msi-0.1.0.0.hashmanifest.toml`.

Signing status: unsigned; `Get-AuthenticodeSignature` reported `NotSigned` on 2026-08-04.

License: MIT. See `LICENSE` and `docs/FileCabinet - License.md`.

## MSI Lifecycle Verification

The built MSI was lifecycle-verified on 2026-08-17 with quiet install and uninstall logs under `artifacts/verification`.

Verified installed state:

* Installed payload and bundled docs were present under `C:\Program Files (x86)\FileCabinet`.
* `FileCabinet.Cli.exe --version` returned `FileCabinet.Cli 0.1.0`.
* Windows Explorer shell verbs `Copy to FileCabinet` and `Move to FileCabinet` were present and pointed to the installed executable.
* Start Menu and public desktop shortcuts were present.
* Installed `FileCabinet.exe` launched with window title `FileCabinet - Personal Vault & Artifact Manager`.

Verified uninstall state:

* MSI removal completed successfully.
* Install folder, Start Menu shortcut folder, public desktop shortcut, shell verb registry keys, and uninstall registry entry were absent after uninstall.

Packaging note: the MSI currently installs the win-x64 single-file payload under `C:\Program Files (x86)\FileCabinet` and registers the uninstall entry in the WOW6432 registry view. That behavior is verified for v0.1.0 compatibility, but it should be reviewed before a future public distribution pass.
