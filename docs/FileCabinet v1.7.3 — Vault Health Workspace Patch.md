# FileCabinet v1.7.3 - Vault Health Workspace Patch

FileCabinet v1.7.3 is a health and repair usability patch focused on making vault maintenance faster, clearer, and less punishing for vaults that contain large retained files.

The main change is structural: Vault Health now has its own workspace instead of being squeezed into the right-side artifact panel and duplicated in the lower status card. Health actions are explicit, bulk repair selection is easier to understand, and default analysis avoids surprise full-file hash reads.

## Vault Health Workspace

- The file table toolbar now exposes a single `Health` button that opens Vault Health without starting a scan.
- Vault Health is now a main workspace with progress, summary cards, breakdown bars, repair candidates, findings, and history in one place.
- The previous cramped right-panel Health content, lower-right Vault Health card, and duplicated dashboard-style health controls were removed.
- Health navigation now supports focused `Overview`, `Repairs`, `Findings`, and `History` sections.
- Progress and scan status now stay at the top of the workspace instead of being buried under filters or dropdowns.

## Metadata-First Analysis

- `Analyze Health` now defaults to metadata-first analysis.
- Default analysis checks catalog paths, missing retained files, missing hashes, duplicate catalog hash values, thumbnails, extracted text references, orphan generated assets, and metadata completeness.
- Default analysis does not automatically compute or verify retained-file hashes.
- Large-file hash work is reported separately so the operator can see what was intentionally deferred.
- Hash verification after Apply stays metadata-first and does not silently deep-scan retained files again.

## Explicit Hash Work

- `Verify small hashes` verifies existing hashes only for retained files up to `1 GB`.
- Selected hash verification and selected hash recomputation remain explicit actions for cases where the operator really does want FileCabinet to read larger retained files.
- `RecomputeHash` is treated as automatic but expensive, so it is available for bulk operation without being part of the safe default selection.
- `Full Rescan & Orphan Recovery` now lives inside Vault Health with wording that makes it clear it refreshes/adopts vault files and then runs analysis.

## Bulk Repair Selection

- Repair candidates are split into automatic repairs, expensive automatic repairs, and review-only findings.
- `Select safe automatic` selects catalog-only or generated-asset repairs such as path rebinds, thumbnail regeneration, and extracted text recovery.
- `Select visible`, `Clear visible`, and per-action selection controls make large repair lists manageable.
- Review-only findings are excluded from repair selection instead of looking like work that Apply can fix.
- Bulk selection updates are batched so large candidate lists update the view once instead of feeling delayed row by row.
- Applying repairs summarizes the selected automatic work and then reruns metadata-first analysis.

## Release Consistency

- Application and CLI versions were updated to `1.7.3`.
- The installer build script now defaults to `1.7.3.0`.
- README and manifest release metadata were updated for the v1.7.3 patch.
- CLI version output was verified from the rebuilt publish output as `FileCabinet.Cli 1.7.3`.

## Verification

- `dotnet build FileCabinet.slnx --configuration Release --no-restore --no-incremental`
- `dotnet test FileCabinet.Tests\FileCabinet.Tests.vbproj --configuration Release --no-restore` - 104 passed
- `powershell -ExecutionPolicy Bypass -File installer\build-installer.ps1 -Version 1.7.3.0`
- `Get-FileHash -Algorithm SHA256 artifacts\installer\FileCabinet-1.7.3.0-win-x64.msi`

## Artifact

- File: `artifacts/installer/FileCabinet-1.7.3.0-win-x64.msi`
- Runtime: `win-x64`
- Size: `128,389,120` bytes
- SHA-256: `71FEAF41CE935AA0A3D7E43CC3E515FC965032FBA1AE6CF5B7FF429B2C74212F`
