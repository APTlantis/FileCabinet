# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Read first

Before working on any release-related task, read in this order:
1. `FileCabinet.manifest.toml` — authoritative version, lifecycle state, and verification records
2. `Project-README.md` — current state summary and verification entry points
3. `README.md` — user/operator guide and developer notes

## Project overview

FileCabinet is a local-first Windows desktop vault for retaining, cataloging, previewing, verifying, and recovering high-signal technical artifacts. It targets `.NET 10.0-windows` and is written entirely in VB.NET with WPF.

Current version: **1.7.3**. Lifecycle state: **paused** (the documented MSI is absent from this checkout; release readiness requires a full rebuild and verification pass).

## Solution structure

```
FileCabinet.slnx              — solution (two projects only)
FileCabinet.vbproj            — WPF desktop app (WinExe, net10.0-windows)
FileCabinet.Cli\              — headless CLI (console exe, references the main project)
FileCabinet.Tests\            — MSTest test project (references both above)
installer\build-installer.ps1 — WiX MSI build script
Themes\NeonInk.xaml           — Neon Ink ResourceDictionary (merged in Application.xaml)
docs\                         — release notes and preservation documentation
artifacts\                    — publish output and installer output (not committed)
```

## Build and test commands

```powershell
# Restore (run once or after package changes)
dotnet restore FileCabinet.slnx
dotnet restore FileCabinet.Tests\FileCabinet.Tests.vbproj

# Build (solution or individual project)
dotnet build FileCabinet.slnx --no-restore
dotnet build FileCabinet.vbproj --no-restore

# Build into a temp path when the app is already running (avoids file locks)
dotnet build FileCabinet.vbproj --no-restore -p:OutputPath=.verify-build\ -p:UseAppHost=false

# Run all tests
dotnet test FileCabinet.Tests\FileCabinet.Tests.vbproj --no-restore

# Run tests with coverage (matches CI)
dotnet test FileCabinet.Tests\FileCabinet.Tests.vbproj --configuration Release --no-restore /p:CollectCoverage=true /p:CoverletOutput=TestResults\ /p:CoverletOutputFormat=opencover

# Run a single test class by name
dotnet test FileCabinet.Tests\FileCabinet.Tests.vbproj --no-restore --filter "ClassName=FileCabinet.Tests.HashServiceTests"

# Build the MSI installer (publishes self-contained win-x64 first, then creates the MSI)
.\installer\build-installer.ps1 -Version 1.7.3.0

# Hash the built MSI
Get-FileHash -Algorithm SHA256 artifacts\installer\FileCabinet-1.7.3.0-win-x64.msi
```

## Architecture

### Data layer

- **`CatalogData`** (`Models.vb`) is the single serialized root — it holds vault list, artifact list, tags, categories, activities, stats, settings (active hashes, ingest mode, UI prefs), and UI state. It lives at `%APPDATA%\FileCabinet\catalog.json`.
- **`CatalogService`** (`CatalogService.vb`) owns all load/save/backup/export operations on `catalog.json`. Backup writes to the vault's `exports\` folder and round-trips through validation.
- **`ArtifactModel`** (`Models.vb`) is the core domain record. Beyond file facts (path, size, hashes, dates) it holds operator-authored metadata: `RetentionReason`, `WhyThisMatters`, `SourceProvenance`, `AcquisitionMethod`, `TrustClassification`, `RetentionPriority`, `ArchiveStatus`. Hash values have two storage layers: well-known first-class properties (`Sha256`, `Blake3`, …) and an extensible `Hashes` dictionary for the 20+ optional compatibility checksums added in v1.7.0.

### Service layer

Each service is stateless and instantiated by `MainViewModel`:

| Service | File | Role |
|---|---|---|
| `CatalogService` | `CatalogService.vb` | JSON load/save/backup/export |
| `IngestionService` | `IngestionService.vb` | File copy/move into vault, artifact creation, hash computation, text extraction, thumbnail generation |
| `HashService` | `HashService.vb` | All hash computation; `HashRegistry` defines the 30+ supported algorithms, their categories, and which are default-enabled |
| `PreviewService` | `PreviewService.vb` | Image and text preview generation; returns `ArtifactPreview` with a `Kind` enum |
| `ThumbnailService` | `ThumbnailService.vb` | Deterministic thumbnail generation and caching under `thumbnails\yyyy\MM\` |
| `VaultSearchService` | `VaultSearchService.vb` | Filters artifact lists by query, scope, category, and tag |
| `VaultHeadlessService` | `VaultHeadlessService.vb` | CLI-facing operations (ingest, verify, export, repair, rescan, package) with typed result objects |
| `RepairLogService` | `RepairLogService.vb` | Append-only JSONL repair history at `catalog\repair-log.jsonl` |

### ViewModel and UI

- **`MainViewModel`** (`MainViewModel.vb`) is the single ViewModel for the entire WPF window. It owns `ICollectionView` filtering, all observable properties, edit state, command implementations, and calls into the service layer.
- **`MainWindow.xaml`** / **`MainWindow.xaml.vb`** is the single-window WPF shell. The layout is: left sidebar (vault list, discovery scopes, categories, tags) → center workspace (drop zone, search, artifact table, status strip) → right panel (Preview & Relations tab + Details tab + action grid). Vault Health is a dedicated workspace that replaces the main artifact area.
- **`BindingProxy.vb`** provides a `DataContext` relay for bindings inside `DataTemplate` and similar contexts where direct `DataContext` is not available.
- **`RelayCommand.vb`** / `AsyncRelayCommand` are the standard `ICommand` implementations used throughout.

### CLI

- `FileCabinet.Cli` references `FileCabinet.vbproj` directly. `CliParser.vb` dispatches verbs to `VaultHeadlessService`. `CliTextOutput.vb` and `CliJsonOutput.vb` handle the two output modes. Exit codes: `0` success, `1` failure, `2` verify threshold met, `3` partial ingest/repair.

### Hash architecture

`HashRegistry` (`HashService.vb`) is the single source of truth for all supported algorithms. Hash options have an `Id` (e.g. `"SHA256"`, `"BLAKE3"`), a `CatalogPropertyName` (mapping to `ArtifactModel` or `FileHashes`), a `Category` (`"Recommended"`, `"Modern strong hashes"`, `"Legacy cryptographic hashes"`, `"Compatibility checksums"`, `"Compatibility non-crypto hashes"`), and an `IsDefaultEnabled` flag. New catalogs default to SHA-256 only. All cryptographic hashes share a single sequential file read per ingest/repair pass.

## Theme system

All colors live in `Themes/NeonInk.xaml`, merged in `Application.xaml`. Color semantics are strict — do not use semantic accent colors for decoration:

- **Cyan/teal** — navigation, selection, focus, structure
- **Violet/purple** — ingest, preview, process, pipeline states
- **Pink/magenta** — featured, creative, metadata emphasis
- **Green** — healthy, indexed, verified, validated
- **Yellow** — starred, attention
- **Red** — quarantine, failure, delete, destructive
- **Orange** — large objects, build outputs, installers, package-like artifacts

## Design constraints

- Recall and relations are **deterministic and inspectable** — no ML, no opaque scoring. Relation reasons must be catalog signals (shared tags, same source folder, shared hash prefix, etc.).
- Repair actions are **safe-by-default**: catalog-mutating and file-touching repairs require explicit operator selection. Review-only findings are never applied automatically.
- Hash mismatch verification for files larger than 16 GB is deferred. Auto hash verification is limited to files ≤ 1 GB.
- Shell thumbnails are intentionally not used. All preview and thumbnail generation is local and deterministic.
- `OptionStrict On` is enforced in the main project.

## Vault folder layout

```
<vault-root>\
  items\yyyy\MM\        — retained files (ingested here by date)
  catalog\              — repair-log.jsonl
  quarantine\           — quarantined files
  exports\              — catalog backups and package exports
  thumbnails\yyyy\MM\   — generated thumbnails
  extracted-text\yyyy\MM\ — extracted text indexes
```

## Release verification checklist

A release is only complete when all of these pass:
1. `dotnet build FileCabinet.slnx --configuration Release --no-restore --no-incremental`
2. `dotnet test FileCabinet.Tests\FileCabinet.Tests.vbproj --configuration Release --no-restore` (all tests pass)
3. `.\installer\build-installer.ps1 -Version <version>` completes
4. `Get-FileHash -Algorithm SHA256 artifacts\installer\FileCabinet-<version>-win-x64.msi` recorded
5. README, `FileCabinet.manifest.toml`, project file version, and release notes all agree
6. MSI launches and the app starts correctly on Windows x64
