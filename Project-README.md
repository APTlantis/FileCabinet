# FileCabinet

## Purpose

FileCabinet is a local-first Windows desktop vault for retaining, cataloging, previewing, verifying, and recovering technical artifacts. The project includes a VB.NET/WPF application, CLI, tests, WiX installer workflow, documentation, integrity metadata, and repair/recovery facilities.

## Governance

- [FileCabinet.manifest.toml](FileCabinet.manifest.toml)
- [AGENTS.md](AGENTS.md)
- [User and operator README](README.md)
- [Desktop Application Release Standard](D:/.library/aptlantis_core/DRS/README.md)

## Current state

Version `0.1.0` is the active governed stable release line. The prior v1.x installer train is retained as pre-standard historical evidence and no longer acts as current release authority.

The v0.1.0 local release was verified on 2026-08-04 with source build, 104 passing tests, WiX MSI packaging, SHA-256 hashing, unsigned Authenticode status, and launch verification from the published executable. Full MSI install/uninstall was not executed during this local verification pass.

## Architecture and workflows

- WPF desktop application and VB.NET domain/services.
- Separate CLI and MSTest projects.
- Local JSON catalog and user-selected portable vault storage.
- Deterministic ingest, preview, relation, health, repair, export, and integrity workflows.
- PowerShell/WiX installer pipeline under `installer`.

## Verification entry points

Follow `README.md` and `installer/build-installer.ps1`. A release verification pass must cover source build, tests, installer creation, SHA-256, installation or launch verification, data-safety notes, and documentation/manifests aligned to the resulting artifact.
