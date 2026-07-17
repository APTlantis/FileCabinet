# FileCabinet

## Purpose

FileCabinet is a local-first Windows desktop vault for retaining, cataloging, previewing, verifying, and recovering technical artifacts. The project includes a VB.NET/WPF application, CLI, tests, WiX installer workflow, documentation, integrity metadata, and repair/recovery facilities.

## Governance

- [FileCabinet.manifest.toml](FileCabinet.manifest.toml)
- [AGENTS.md](AGENTS.md)
- [User and operator README](README.md)
- [Desktop Application Release Standard](D:/.library/aptlantis_core/DRS/README.md)

## Current state

Version `1.7.3` is supported by the project file, README, release notes, and historical verification records. The checkout is clean, but the documented MSI is not present under `artifacts/installer`; therefore the project is classified as `paused`, not release-ready. Current build, tests, installer, hash, installation, and launch must be rerun before another readiness claim.

## Architecture and workflows

- WPF desktop application and VB.NET domain/services.
- Separate CLI and MSTest projects.
- Local JSON catalog and user-selected portable vault storage.
- Deterministic ingest, preview, relation, health, repair, export, and integrity workflows.
- PowerShell/WiX installer pipeline under `installer`.

## Verification entry points

Follow `README.md` and `installer/build-installer.ps1`. A release verification pass must cover source build, tests, installer creation, SHA-256, installation, launch, and documentation/manifests aligned to the resulting artifact.
