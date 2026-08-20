# FileCabinet v0.1.1 - Compliance Alignment Patch

FileCabinet v0.1.1 is a small governance and release-evidence patch. It does not introduce a new vault workflow. Its purpose is to make the project easier to adopt, verify, and redistribute under the current DRS process. After the DRS release-policy update, the forward public Windows package path is MSIX submitted through the Microsoft Store; locally self-signed MSIX packages are development/sideload evidence. The first Store product identity is reserved as `Filing Cabinet` with package identity `Aptlantis.FilingCabinet`.

---

## What Changed

### MIT License Published

The project now has a top-level `LICENSE` file using the MIT License. The README, project manifest, release checklist, and source documentation set all identify MIT as the project license.

The installer build script also includes the root `LICENSE` file in the MSI payload for rebuilt installers.

### Release Hash Manifests Published

The package release records now include Release Hasher TOML manifests. The MSIX development package manifest records the ARHS release hash suite:

* SHA-256 for broad compatibility.
* BLAKE3-256 for fast modern integrity checks.
* KT128 for ARHS release evidence.

The historical/local MSI also keeps its Release Hasher manifest as direct-distribution evidence.

### Documentation Payload Rebuilt From Canonical Docs

The MSI documentation payload is rebuilt from the current canonical `docs/` directory. The payload now includes the license pointer, installer hash manifest note, release checklist, and this release note alongside the standing preservation and technical rationale documents. Future Store MSIX release records should carry the same source documentation alignment before submission.

### Release Records Synchronized

The project manifest, JSON manifest, README, project README, source release note, release checklist, package artifact path, and hash manifest path now agree on the `0.1.1` application version and `0.1.1.0` Windows package version.

---

## Release Artifact

Development MSIX:

* `file-cabinet_0.1.1.0_x64.msix`

MSIX hash manifest:

* `file-cabinet_0.1.1.0_x64.hashmanifest.toml`

Signing status: self-signed development certificate, for sideload/development use only. Public Windows distribution still requires Microsoft Store submission and Store signing.

Reserved Store identity:

* Product name: `Filing Cabinet`
* Package identity name: `Aptlantis.FilingCabinet`
* Publisher: `CN=81D6747D-F84F-4EFF-ACAA-9635D91ACCD0`
* Publisher display name: `Aptlantis`
* Package family name: `Aptlantis.FilingCabinet_jfrcsngvdwx7g`
* Store ID: `9N29X9KR70R3`

The Store candidate must be rebuilt with that identity before Partner Center upload.

First-gate Partner Center feedback established the manifest pattern:

* `Package/Identity/Name`: `Aptlantis.FilingCabinet`
* `Package/Identity/Publisher`: `CN=81D6747D-F84F-4EFF-ACAA-9635D91ACCD0`
* `Package/Properties/DisplayName`: `Filing Cabinet`
* `Package/Properties/PublisherDisplayName`: `Aptlantis`
* `uap:VisualElements DisplayName`: `Filing Cabinet`

The package acceptance/device-family availability gate is not the final release. The final candidate still needs naming/documentation cleanup, rebuild, package acceptance, release hashing, certification, publication, Store signing verification, and install/launch/update/uninstall checks.

Historical/local MSI:

* `FileCabinet-0.1.1.0-win-x64.msi`

MSI hash manifest:

* `artifacts/installer/FileCabinet_msi-0.1.1.0.hashmanifest.toml`
* `docs/FileCabinet - Installer Hash Manifest.md`

SHA-256:

* Recorded in `artifacts/installer/FileCabinet_msi-0.1.1.0.hashmanifest.toml`.

BLAKE3-256:

* Recorded in `artifacts/installer/FileCabinet_msi-0.1.1.0.hashmanifest.toml`.

KT128:

* Recorded in `artifacts/installer/FileCabinet_msi-0.1.1.0.hashmanifest.toml`.

MSI signing status: unsigned unless a separate signing pass is performed.

License: MIT. See `LICENSE` and `docs/FileCabinet - License.md`.

---

## Verification Scope

This patch should be verified with:

* Release build and tests.
* MSIX package at package version `0.1.1.0`.
* Release Hasher output for the final MSIX.
* Store submission/signing verification before any public Windows release claim.
* Historical MSI rebuild at package version `0.1.1.0` when MSI direct distribution remains relevant.
* Manifest, release note, checklist, packaged docs, and installer hash synchronization.

This patch does not replace a full public distribution certification pass. Installer lifecycle verification remains a separate gate when a fresh install/uninstall certification is required.
