# FileCabinet — Trust and Verification Model

FileCabinet earns trust by making vault state inspectable.

The vault does not ask the operator to trust an opaque index. It keeps retained files on disk, records structured catalog metadata, and exposes health findings that explain when the catalog and filesystem disagree.

## Trust Anchors

FileCabinet relies on a few plain trust anchors:

- retained files live in the local vault filesystem
- catalog state is stored as JSON
- content identity is represented by hashes
- generated assets are rebuildable
- repair actions are logged
- destructive actions require explicit operator intent

This makes the vault understandable without a service account, cloud API, or hidden database server.

## Verification Surfaces

Verification checks several kinds of drift:

- **Existence drift:** the catalog points to a file that is no longer present.
- **Content drift:** a retained file exists but its hash no longer matches the catalog.
- **Preview drift:** generated thumbnails or fallback previews are missing or stale.
- **Text drift:** extracted text is missing, stale, or disconnected from the artifact.
- **Ownership drift:** a catalog entry points outside the vault.
- **Catalog drift:** required metadata is incomplete or internally inconsistent.
- **Filesystem drift:** retained files exist under the vault but are not cataloged.

Each finding should describe the subject, risk level, and likely next step.

## Risk Levels

Risk levels are meant to guide review:

- **Low:** generated or derived state can probably be rebuilt.
- **Medium:** recall, preview, or metadata quality is degraded.
- **High:** retained content may be missing, changed, duplicated unexpectedly, or outside the vault boundary.

High risk does not imply automatic repair. It means the operator should review before trusting or changing the vault.

## Repair Philosophy

Repair should be deterministic, cautious, and reversible where possible.

Automatic repair is appropriate for generated assets and missing derived metadata when the retained file is present. Operator review is required for ambiguous content problems such as duplicate files, mismatched hashes, or unexpected orphan files.

The repair log is part of the trust model. It records what changed, when it changed, and what outcome was reported.

## Headless Verification

The CLI exposes the same verification model for scripts:

```powershell
FileCabinet.Cli.exe verify --fail-on medium
FileCabinet.Cli.exe report --format json --output K:\FileCabinet\exports\health.json
```

Exit code `2` means verification found threshold-level issues. That allows scheduled checks to fail visibly without mutating the vault.

