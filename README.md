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

For images, FileCabinet renders an image preview. For text-like files, it renders a text preview. For unsupported binary formats such as archives, installers, disk images, and other retained files, it keeps the file as a first-class artifact and shows a clear fallback message.

The details area shows editable metadata and file facts:

- name
- category
- tags
- rating
- stored path
- type
- created and modified timestamps
- BLAKE3 and SHA-256 hashes
- hash verification status
- extracted text status and index path
- original source path
- notes

Use **Save** to persist metadata edits. Use **Revert** to reload the selected artifact's current catalog values.

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

## Related Items

The **Relations** section in the right panel shows a first-pass related-artifacts list. Related items are ranked by:

- duplicate SHA-256 hash
- shared category
- shared tags

This is deterministic local matching, not AI. It is meant to help you quickly spot nearby artifacts such as a manifest and archive, related images, or files with matching tags.

## Repair, Rescan, And Backups

FileCabinet includes a few recovery-oriented tools.

**Repair** checks catalog health. It reports missing stored files, duplicate hash groups, orphan files under `items`, and adopted file counts.

**Rescan** looks for files under the vault's `items` folder that are not yet in the catalog and adopts them as cataloged artifacts.

**Back up catalog** writes a portable catalog snapshot into the vault's `exports` folder. This is useful before manual vault work, experimentation, or moving data between machines.

The settings/status strip at the bottom right summarizes vault path, catalog path, last backup, intake mode, duplicate behavior, repair status, and AI status.

## What FileCabinet Does Not Do Yet

Some UI labels are intentionally present but still deferred.

Local AI is not active yet. Embeddings, AI classification, semantic search, summarization, and assistant-style recall are planned after the core vault workflows are stable.

OCR is not implemented yet. Text extraction currently handles text-like files, not image text or scanned PDFs.

PDF preview is currently a retained-file fallback rather than full document rendering.

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
