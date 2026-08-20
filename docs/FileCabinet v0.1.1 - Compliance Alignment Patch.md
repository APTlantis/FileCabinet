# FileCabinet v0.1.1 - Compliance Alignment Patch

FileCabinet v0.1.1 is a small governance and release-evidence patch. It does not introduce a new vault workflow. Its purpose is to make the project easier to adopt, verify, and redistribute under the current DRS process.

---

## What Changed

### MIT License Published

The project now has a top-level `LICENSE` file using the MIT License. The README, project manifest, release checklist, and source documentation set all identify MIT as the project license.

The installer build script also includes the root `LICENSE` file in the MSI payload for rebuilt installers.

### Release Hash Manifest Published

The installer release record now includes a Release Hasher TOML manifest. The manifest records the DRS-compatible hash suite for the MSI:

* SHA-256 for broad compatibility.
* BLAKE3-256 for fast modern integrity checks.
* KT128 for ARHS-style release evidence.

### Documentation Payload Rebuilt From Canonical Docs

The MSI documentation payload is rebuilt from the current canonical `docs/` directory. The payload now includes the license pointer, installer hash manifest note, release checklist, and this release note alongside the standing preservation and technical rationale documents.

### Release Records Synchronized

The project manifest, JSON manifest, README, project README, source release note, release checklist, packaged docs, installer artifact path, and hash manifest path now agree on the `0.1.1` application version and `0.1.1.0` Windows package version.

---

## Release Artifact

Expected installer:

* `FileCabinet-0.1.1.0-win-x64.msi`

Hash manifest:

* `artifacts/installer/FileCabinet_msi-0.1.1.0.hashmanifest.toml`
* `docs/FileCabinet - Installer Hash Manifest.md`

SHA-256:

* Recorded in `artifacts/installer/FileCabinet_msi-0.1.1.0.hashmanifest.toml`.

BLAKE3-256:

* Recorded in `artifacts/installer/FileCabinet_msi-0.1.1.0.hashmanifest.toml`.

KT128:

* Recorded in `artifacts/installer/FileCabinet_msi-0.1.1.0.hashmanifest.toml`.

Signing status: unsigned unless a separate signing pass is performed.

License: MIT. See `LICENSE` and `docs/FileCabinet - License.md`.

---

## Verification Scope

This patch should be verified with:

* Release build and tests.
* MSI rebuild at package version `0.1.1.0`.
* Release Hasher output for the final MSI.
* Manifest, release note, checklist, packaged docs, and installer hash synchronization.

This patch does not replace a full public distribution certification pass. Installer lifecycle verification remains a separate gate when a fresh install/uninstall certification is required.
