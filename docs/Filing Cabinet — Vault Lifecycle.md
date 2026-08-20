# FileCabinet — Vault Lifecycle

FileCabinet treats a retained file as an artifact with a lifecycle, not as a loose item in a folder.

The lifecycle is deliberately conservative: every transition should preserve bytes, context, and operator intent. The vault should be easy to inspect years later, even if the original project folder, download page, vendor portal, or working context is gone.

## 1. Capture

Capture begins when an operator chooses to retain a file or folder.

FileCabinet supports both copy and move intake:

- Copy intake retains an independent vault copy while leaving the source in place.
- Move intake transfers ownership to the vault after the retained copy is assembled.

The important distinction is intent. A file is not merely indexed. It is selected for preservation.

## 2. Classification

After capture, FileCabinet records deterministic metadata:

- original name and source path
- retained vault path
- category and type family
- size and timestamps
- content hashes
- source batch information
- operator notes, tags, and custom metadata

Automatic classification is intentionally modest. Operator-authored context remains more important than generated labels.

## 3. Enrichment

FileCabinet enriches artifacts with local generated assets when appropriate:

- thumbnails for previewable image files
- fallback preview cards for retained binary families
- extracted text for text-like artifacts
- relation hints based on shared metadata, hashes, source batches, and naming

These generated assets support recall, but they do not replace the retained file.

## 4. Verification

Verification compares catalog intent against vault reality.

Health analysis checks for conditions such as:

- missing retained files
- hash mismatches
- duplicate content hashes
- missing generated thumbnails
- stale or missing extracted text
- files outside the vault
- orphan retained files
- orphan generated assets

The result is a report the operator can inspect before deciding whether to repair, rescan, quarantine, or export.

## 5. Recovery

Recovery paths are intentionally explicit:

- Restore Copy returns a retained file to a chosen folder without removing it from the vault.
- Quarantine isolates questionable retained files without deleting them.
- Repair candidates describe possible fixes before applying them.
- Rescan adoption brings orphan retained files back under catalog management.
- Delete Forever is the explicit irreversible removal path.

FileCabinet should make the vault state understandable before it changes the vault state.

## 6. Packaging

For cold storage or migration, the CLI can produce a deterministic vault package containing:

- catalog JSON
- catalog JSONL
- retained items
- thumbnails
- extracted text
- repair logs
- vault health report

The package is meant to preserve both the files and the operational context needed to trust them later.

