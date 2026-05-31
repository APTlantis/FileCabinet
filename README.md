# FileCabinet User Guide

FileCabinet is a local-first desktop vault for digital artifacts you want to keep, find, verify, and recover later. It is not meant to replace your normal folders. Think of it as a deliberate retention space for files that deserve more structure than "somewhere in Downloads" but do not fit neatly into one project folder.

Good candidates include installers, disk images, manifests, configuration files, keys, screenshots, datasets, archives, torrents, generated assets, recovery documents, and research artifacts.

## The Core Idea

FileCabinet stores important files inside a selected vault folder and keeps a catalog of what each file is, where it came from, how it was classified, what hashes identify it, and what searchable text was extracted from it.

The app is local-first. Vault files live on your machine, under the vault path you choose. The lightweight application catalog lives in AppData, and the vault itself contains portable subfolders for retained items, exports, quarantine, thumbnails, and extracted text.

Typical vault layout:

```text
K:\FileCabinet\
  items\
  catalog\
  quarantine\
  exports\
  thumbnails\
  extracted-text\
```

## Vaults

The vault list in the left sidebar shows the available vault entries. A vault entry points to a folder such as `K:\` or `K:\FileCabinet`.

Use the folder button beside **VAULTS** to set the selected vault folder. If a stale vault points to a drive you do not have anymore, select it and use the remove button beside the folder button. Removing a vault entry removes it from FileCabinet's list; it does not delete files from disk.

The current vault title appears at the top of the main area. Storage and item counts are derived from the catalog and vault state.

## Adding Files

The drop zone is the main intake path. You can drag files or folders onto it, or click it to browse for files.

FileCabinet supports two intake modes:

- **Move into vault** removes the original after the file is safely copied into the vault. This makes FileCabinet the owner of that retained artifact.
- **Copy into vault** keeps the original where it is and stores a retained copy in the vault.

The active mode is shown directly inside the drop zone as **INTAKE MODE** so you can check it before dropping a batch of files. The mode button under **Ingest Options** toggles between move and copy.

The Windows Explorer context menu also includes **Copy to FileCabinet** and **Move to FileCabinet** after installing the MSI. These commands open FileCabinet and ingest the selected file or folder using that one-time intake mode without changing the app's default drop-zone setting.

When a file is ingested, FileCabinet:

- places it under `items\yyyy\MM\`
- avoids filename collisions by renaming safely
- records the original path
- computes BLAKE3 and SHA-256 hashes
- infers type, category, and starter tags
- extracts searchable text for text-like files
- updates activity, stats, categories, and tags

## Finding Files

The search box at the top searches the catalog and extracted text. It can match names, types, categories, paths, original paths, notes, tags, hashes, and extracted text content.

The left sidebar gives narrower ways to browse:

- **All Items** shows the full catalog.
- **Recent** and **Inbox** focus on recently ingested or recently modified items.
- **Starred** shows items you marked as important.
- **Quarantine** shows items moved into the quarantine category/folder.
- **Categories** filter by inferred or edited category.
- **Tags** filter by tag. The tag search field narrows the visible tag cloud.

Filters can be combined. For example, you can view recent items in a category, or search within a selected tag.

## The Artifact Table

The table at the bottom is the main catalog view. Selecting a row updates the right panel.

The table toolbar includes:

- sort by name
- sort by modified date
- compact/comfortable row density
- repair check
- rescan

The density toggle is only a display preference. It does not change catalog data.

## Preview And Details

The right panel shows the selected artifact.

For images, FileCabinet generates a cached thumbnail under the vault's `thumbnails` folder and uses it for preview. For text-like files, it renders a text preview. For unsupported binary formats such as archives, installers, disk images, and other retained files, it keeps the file as a first-class artifact and shows a format-aware fallback card.

The details area shows editable metadata and file facts:

- name
- category
- tags
- rating
- retention reason
- why this matters
- source provenance
- acquisition method
- trust classification
- retention priority
- archive status
- stored path
- type
- created and modified timestamps
- BLAKE3 and SHA-256 hashes
- hash verification status
- extracted text status and index path
- thumbnail/preview generation status
- original source path
- notes

Use **Save** to persist metadata edits. Use **Revert** to reload the selected artifact's current catalog values.

Structured operator metadata is separate from free-form notes. Use it to preserve the human reason an artifact was retained, where it came from, how it was acquired, how much you trust it, and whether it is active, archived, quarantined, or waiting for review.

## Metadata And Recall

The **Relations** tab uses deterministic catalog signals only. Relation reasons remain inspectable, such as duplicate hashes, shared tags, same original folder, same ingest session, matching filename tokens, shared extension family, shared provenance tokens, shared release markers, shared hash prefixes, and shared extracted-text keywords.

The navigation panel includes built-in discovery scopes for common recall and cleanup tasks:

- **Unverified** shows artifacts with unverified or questionable trust, or hashes that are not verified.
- **Missing Preview** shows generated previews that are referenced but missing.
- **Repair Needed** shows artifacts with vault health findings.
- **Duplicate Candidates** shows artifacts that share a SHA-256 hash with another catalog item.
- **Same Source Batch** shows artifacts near the selected artifact's source folder and ingest session; without a selected item it shows batch clusters.
- **Large Artifacts** shows artifacts at or above 1 GB.

These views combine with text search, category filters, and tag filters.

## Artifact Actions

The action grid in the right panel contains the operational file actions.

**Open Location** opens File Explorer with the stored vault file selected.

**Open File** opens the stored file with the system default application.

**Restore Copy** lets you choose a destination folder and copies the selected vault file back out. This keeps the vault copy intact.

**Toggle Star** marks or unmarks an artifact as important.

**Add Tags** adds a starter `review` tag to the edit box. Save the metadata to persist it.

**Hash Check** recomputes hashes for the stored file and updates the hash status. If a stored hash does not match, FileCabinet reports the mismatch.

**Quarantine** moves the stored file into the vault's `quarantine` folder and updates the artifact category. This is safer than deleting when you are unsure.

**Delete Forever** permanently deletes the stored file and removes the catalog entry after confirmation. This is intended for files you are sure you no longer need.

## Text Extraction

FileCabinet extracts text from text-like files during ingest and rescan adoption. Extracted text is stored under `extracted-text\yyyy\MM\` and linked from the artifact record.

This makes retained config files, manifests, scripts, markdown, JSON, TOML, YAML, XML, logs, CSV files, and similar artifacts searchable by content.

Binary files are marked **Not extractable**. Failed extraction is recorded as **Extraction failed** rather than silently pretending the file was indexed.

## Thumbnail Generation

FileCabinet generates deterministic local thumbnails for image files during ingest and rescan adoption. Thumbnail files are stored under `thumbnails\yyyy\MM\` inside the vault and referenced by the catalog.

Non-renderable retained artifacts such as installers, archives, torrents, and disk images use format-aware fallback cards instead of shell thumbnails. Repair checks report missing generated thumbnails and attempt to regenerate them when the original vault file is present.

## Related Items

The **Relations** section in the right panel shows a first-pass related-artifacts list. Related items are ranked by:

- duplicate SHA-256 hash
- shared category
- shared tags

This is deterministic local matching with explainable reasons. It is meant to help you quickly spot nearby artifacts such as a manifest and archive, related images, or files with matching tags.

## Repair, Rescan, And Backups

FileCabinet includes a few recovery-oriented tools.

**Analyze** checks vault health without mutating retained files. It reports missing stored files, duplicate hash groups, missing hashes, hash mismatches, missing thumbnails, orphan thumbnails, missing extracted-text indexes, stale extracted-text indexes, path rebind candidates, files outside the active vault, and incomplete metadata.

**Apply Selected** runs only safe repair candidates after confirmation, such as recomputing missing hashes, regenerating missing thumbnails, re-extracting missing text indexes, and rebinding stale absolute paths when the vault-relative file exists under the active vault. Review-only findings remain visible but are skipped by controlled execution.

Repair activity is written to the vault-local `catalog\repair-log.jsonl` history and summarized in the Vault Health panel.

**Rescan** looks for files under the vault's `items` folder that are not yet in the catalog and adopts them as cataloged artifacts.

**Back up catalog** writes a portable catalog snapshot into the vault's `exports` folder and validates that the exported JSON can be read back as a usable catalog. This is useful before manual vault work, experimentation, or moving data between machines.

The settings/status strip at the bottom right summarizes vault path, catalog path, last backup, intake mode, duplicate behavior, repair status, and deterministic recall status.

## CLI And Headless Operations

FileCabinet also includes a separate console executable for scripting and scheduled operations:

```powershell
FileCabinet.Cli.exe --help
FileCabinet.Cli.exe ingest --copy --vault K:\FileCabinet C:\Downloads\artifact.zip
FileCabinet.Cli.exe verify --fail-on medium --json
FileCabinet.Cli.exe search "firmware manifest" --scope all
FileCabinet.Cli.exe export --output K:\FileCabinet\exports
FileCabinet.Cli.exe report --format json --output K:\FileCabinet\exports\health.json
FileCabinet.Cli.exe repair-preview --json
FileCabinet.Cli.exe repair --apply --yes
FileCabinet.Cli.exe rescan --apply --yes
FileCabinet.Cli.exe rebuild-thumbnails --apply --yes
FileCabinet.Cli.exe package --output K:\FileCabinet\exports\FileCabinetPackage --zip
```

The CLI writes real stdout/stderr and returns script-friendly exit codes: `0` for success, `1` for command/runtime failure, `2` when verification findings meet the requested threshold, and `3` for partial ingest or partial repair/rebuild. Mutating repair, rescan, and thumbnail rebuild commands require both `--apply` and `--yes`; without `--apply`, they report what would happen.

The `package` command writes a deterministic vault export containing catalog JSON, catalog JSONL, retained items, extracted text, thumbnails, repair logs, and a vault-health report. Use `--zip` for a single cold-storage archive.

## Preservation Docs

FileCabinet's preservation model is documented in:

### Preservation Model

- [The Art of Deliberate Retention](docs/TheArtofDeliberateRetention.md)
- [Vault Lifecycle](docs/FileCabinet%20%E2%80%94%20Vault%20Lifecycle.md)
- [Trust and Verification Model](docs/FileCabinet%20%E2%80%94%20Trust%20and%20Verification%20Model.md)
- [Local-First Artifact Preservation](docs/FileCabinet%20%E2%80%94%20Local-First%20Artifact%20Preservation.md)
- [Repair and Recovery Guide](docs/FileCabinet%20%E2%80%94%20Repair%20and%20Recovery%20Guide.md)
- [Designing for Context Preservation](docs/FileCabinet%20%E2%80%94%20Designing%20for%20Context%20Preservation.md)

### Technical Rationale

- [Why Determinism Matters](docs/FileCabinet%20%E2%80%94%20Why%20Determinism%20Matters.md)
- [Why SHA-256 and BLAKE3](docs/FileCabinet%20%E2%80%94%20Why%20SHA-256%20and%20BLAKE3.md)
- [Why VB.NET and WPF](docs/FileCabinet%20%E2%80%94%20Why%20VB.NET%20and%20WPF.md)

### Roadmaps

- [Deterministic Vault Roadmap](docs/FileCabinet%20%E2%80%94%20Deterministic%20Vault%20Roadmap.md)
- [Archival Maturity Roadmap](docs/FileCabinet%20%E2%80%94%20Archival%20Maturity%20Roadmap.md)
- [Stronger Daily-Use Roadmap](docs/FileCabinet%20%E2%80%94%20Stronger%20Daily-Use%20Roadmap.md)
- [Stewardship & Preservation Maturity Roadmap](docs/FileCabinet%20%E2%80%94%20Stewardship%20%26%20Preservation%20Maturity%20Roadmap.md)
- [Legacy & Federation Roadmap](docs/FileCabinet%20%E2%80%94%20Legacy%20%26%20Federation%20Roadmap.md)

## Design Boundaries

FileCabinet is intentionally focused on deliberate curation rather than automatic inference.

Text extraction currently handles text-like files, not image text or scanned PDFs.

PDF preview is currently a retained-file fallback rather than full document rendering.

Windows shell thumbnails are not used yet. Preview generation is intentionally local and deterministic.

## Operational Notes

FileCabinet is designed around cautious ownership:

- Move mode is for files you want the vault to own.
- Copy mode is for files you want retained without disturbing the original.
- Restore Copy gets a file back out without removing it from the vault.
- Quarantine isolates questionable files without deleting them.
- Delete Forever is the explicit irreversible removal path.

When in doubt, use Copy mode or Quarantine first. Use Delete Forever only when you are certain the retained file and catalog entry should be removed.

## Developer Notes

The app is a WPF/VB project targeting `.NET 10.0-windows`.

Useful commands:

```powershell
dotnet build FileCabinet.vbproj --no-restore
dotnet test FileCabinet.Tests\FileCabinet.Tests.vbproj --no-restore
```

If the app is currently running, Windows may lock `bin\Debug\net10.0-windows\FileCabinet.exe`. For verification while the app is open, build without an app host into a temporary output path:

```powershell
dotnet build FileCabinet.vbproj --no-restore -p:OutputPath=.verify-build\ -p:UseAppHost=false
```
