# File Cabinet

## Purpose

File Cabinet is the governed project for FileCabinet, a local-first Windows desktop vault for retaining, cataloging, previewing, verifying, and recovering technical artifacts. The project includes a VB.NET/WPF application, CLI, tests, Windows MSIX packaging, a historical WiX installer workflow, documentation, integrity metadata, and repair/recovery facilities.

## Governance

- [File Cabinet.manifest.toml](File%20Cabinet.manifest.toml)
- [AGENTS.md](AGENTS.md)
- [User and operator README](README.md)
- [Desktop Application Release Standard](D:/.library/aptlantis_core/DRS/README.md)
- [Windows GUI MSIX And Microsoft Store Workflow](../Windows-GUI-MSIX-Store-Workflow.md)

## Current state

Version `0.1.1` is the active governed stable release line. The prior v1.x installer train is retained as pre-standard historical evidence and no longer acts as current release authority.

The forward public Windows release path is MSIX submitted through the Microsoft Store, where the Store signs the public package. The first Store product identity is reserved as **Filing Cabinet**:

- Package identity name: `Aptlantis.FilingCabinet`
- Publisher: `CN=81D6747D-F84F-4EFF-ACAA-9635D91ACCD0`
- Publisher display name: `Aptlantis`
- Package family name: `Aptlantis.FilingCabinet_jfrcsngvdwx7g`
- Store ID: `9N29X9KR70R3`

A trial package reached Partner Center package acceptance/device-family availability after `Package.appxmanifest` used `Aptlantis.FilingCabinet` for package identity, `CN=81D6747D-F84F-4EFF-ACAA-9635D91ACCD0` for publisher, `Filing Cabinet` for visible app display names, and `Aptlantis` for publisher display name. That first gate is useful feedback, but it is not certification, publication, Store signing, or release readiness.

A prior self-signed development MSIX was recorded with ARHS hash evidence in `file-cabinet_0.1.1.0_x64.hashmanifest.toml`; that package is sideload/development evidence, not a public Store release. The root `.msix` package is not currently present on disk, so the final Store candidate must be rebuilt after naming/documentation cleanup and only then hashed for release.

The v0.1.0 local release was verified on 2026-08-04 with source build, 104 passing tests, WiX MSI packaging, SHA-256 hashing, unsigned Authenticode status, and launch verification from the published executable. The MSI lifecycle was later verified on 2026-08-17 with quiet install, shell integration checks, installed CLI version, installed WPF launch, installed documentation payload, and quiet uninstall cleanup. That MSI record remains historical/local direct-distribution evidence.

The project is licensed under the MIT License. The current MSIX development package has a dedicated hash manifest at `file-cabinet_0.1.1.0_x64.hashmanifest.toml`. The historical MSI artifact has a separate hash manifest at `artifacts/installer/FileCabinet_msi-0.1.1.0.hashmanifest.toml`, with a source documentation copy at `docs/FileCabinet - Installer Hash Manifest.md`.

Governance records use the current directory name, **File Cabinet**. Product-facing compatibility names remain **FileCabinet** for the executable, CLI, AppData catalog path, MSI product, registry keys, shell verbs, artifact filenames, and repository links.

## Architecture and workflows

- WPF desktop application and VB.NET domain/services.
- Separate CLI and MSTest projects.
- Local JSON catalog and user-selected portable vault storage.
- Deterministic ingest, preview, relation, health, repair, export, and integrity workflows.
- `winapp` MSIX workflow using `Package.appxmanifest` and generated assets under `Assets`.
- PowerShell/WiX installer pipeline under `installer` retained as local/direct-distribution evidence.

## Verification entry points

Follow `README.md`, the Store-aligned `Package.appxmanifest`, and the DRS MSIX workflow. A public release verification pass must cover source build, tests, MSIX/MSIXUPLOAD creation, Store identity alignment, Partner Center package acceptance, certification status, Store signing authority, ARHS hash evidence for the final accepted package, installation or launch verification, data-safety notes, license inclusion, and documentation/manifests aligned to the resulting artifact.
