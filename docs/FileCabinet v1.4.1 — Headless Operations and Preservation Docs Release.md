# FileCabinet v1.4.1 — Headless Operations and Preservation Docs Release

FileCabinet v1.4.1 completes the Priority 4 CLI/headless operations work and the Priority 5 documentation pass.

This release strengthens FileCabinet as both a desktop vault and a scriptable local-first preservation system.

## Headless Operations

This release adds the separate `FileCabinet.Cli.exe` automation surface.

The CLI supports:

- ingest
- verify
- search
- export
- report
- repair-preview
- repair
- rescan
- rebuild-thumbnails
- package

CLI commands write real stdout and stderr, return script-friendly exit codes, and support stable JSON output where appropriate.

Mutating repair, rescan, and thumbnail rebuild operations require explicit approval with both `--apply` and `--yes`.

## Vault Packaging

The new package command writes a deterministic vault export for cold storage or migration.

Packages include:

- catalog JSON
- catalog JSONL
- retained vault items
- thumbnails
- extracted text
- repair logs
- vault health report

The package can be written as a folder or zipped archive.

## UI Hardening

Vault maintenance operations now run asynchronously so long-running scans, hash checks, repair preparation, thumbnail regeneration, and rescan work do not freeze the WPF UI.

This includes hardening around:

- Analyze
- Rescan
- Apply Selected Repair Candidates
- Hash Check
- preview loading
- extracted-text search
- quarantine count refresh

## Documentation and Philosophy

Priority 5 adds preservation-oriented documentation:

- The Art of Deliberate Retention
- Vault Lifecycle
- Trust and Verification Model
- Why Determinism Matters
- Local-First Artifact Preservation
- Repair and Recovery Guide
- Designing for Context Preservation
- Why VB.NET and WPF

These documents clarify FileCabinet's position as a deterministic local-first vault for preserving high-signal technical artifacts and the operational context surrounding them.

## Version

Application, CLI, installer default, and project manifest metadata have been updated to `1.4.1`.
