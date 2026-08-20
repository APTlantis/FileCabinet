# FilingCabinet — Repair and Recovery Guide

FilingCabinet repair is designed around review before mutation.

Health analysis tells the operator what appears wrong. Repair and recovery tools then provide explicit paths to make the vault more trustworthy without hiding risk.

## Analyze First

Start with Analyze or the CLI verify command:

```powershell
FilingCabinet.Cli.exe verify --fail-on medium
```

Review findings before applying changes. Pay special attention to high-risk findings such as missing retained files, hash mismatches, files outside the vault, and unexpected duplicates.

Default analysis is metadata-first: FilingCabinet does not automatically compute or verify retained-file hashes just to produce a health report. "Verify small hashes" only checks retained files up to 1 GB; hash mismatch verification for files larger than 16 GB is deferred and reported as deferred rather than read end-to-end. Explicit hash checks remain available whenever the operator wants full verification.

## Common Findings

### Missing retained file

The catalog points to a retained file that is not present.

Recommended response:

- check whether the vault path is mounted
- restore the file from backup if available
- avoid deleting the catalog entry until the loss is understood

### Hash mismatch

The retained file exists, but its current hash does not match the catalog.

Recommended response:

- treat the file as changed or suspect
- compare against backup or source material
- do not auto-accept the new hash unless the change is intentional

### Missing thumbnail

A generated preview asset is missing.

Recommended response:

- rebuild thumbnails from the retained file
- this is usually lower risk because thumbnails are derived state

### Missing or stale extracted text

The retained file is present, but extracted text is missing or out of date.

Recommended response:

- re-extract text from the retained file
- verify search results after repair

### Orphan retained file

A file exists under the vault's retained items folder but has no catalog entry.

Recommended response:

- preview rescan results
- adopt the file only when it appears to be a legitimate retained artifact

## WPF Recovery Tools

The desktop app provides operator-facing repair and recovery tools:

- Analyze
- Rescan
- Apply Selected Repair Candidates
- Hash Check
- Restore Copy
- Quarantine
- Delete Forever

Long-running vault maintenance runs asynchronously so the UI remains responsive during hashing, orphan scans, repair preparation, and generated asset work.

## CLI Recovery Tools

The CLI provides scriptable operations:

```powershell
FilingCabinet.Cli.exe repair-preview --json
FilingCabinet.Cli.exe repair --apply --yes
FilingCabinet.Cli.exe rescan --apply --yes
FilingCabinet.Cli.exe rebuild-thumbnails --apply --yes
```

Mutating commands require both `--apply` and `--yes`.

## Integrity Matrix

FilingCabinet validates, explains, and recovers vault health without relying on automatic inference. Every recovery path should be deterministic, inspectable, and safe for an operator to approve.

| Scenario | Detection | Expected Report | Safe Repair Action | Risk Level |
|---|---|---|---|---|
| Stored file missing | Catalog artifact path is empty or file does not exist | Missing file count and sample artifact names | Mark as missing; do not remove catalog row automatically | Medium |
| Duplicate retained file | Two or more artifacts share SHA-256 | Duplicate hash group count and samples | Report candidates; operator decides whether to keep both | Low |
| Orphan file under `items` | File exists in vault `items` but no catalog artifact points to it | Orphan count and sample filenames | Rescan adopts as cataloged artifact | Low |
| Missing generated thumbnail | Artifact says thumbnail was generated but file is absent | Missing thumbnail count and sample artifact names | Regenerate thumbnail from retained source file | Low |
| Failed thumbnail generation | Image thumbnail generation throws or source cannot decode | Thumbnail status is `Generation failed` | Leave retained artifact intact; retry during repair if appropriate | Low |
| Orphan thumbnail | Thumbnail file exists but no artifact references it | Orphan thumbnail count and samples | Report first; cleanup should require approval | Low |
| Stale extracted-text index | Extracted text file exists but no artifact references it | Stale index count and samples | Report first; cleanup should require approval | Low |
| Missing extracted-text index | Artifact references extracted text path but file is absent | Missing index count and sample artifact names | Re-extract if source format is supported | Medium |
| Hash missing | Artifact has no active hash values | Unverified or incomplete hash count | Recompute hashes from retained source file | Low |
| Hash mismatch | Recomputed hash differs from catalog hash | Mismatch count and affected artifact names | Mark as mismatch; do not overwrite trusted hash automatically | High |
| Relative path broken after vault move | Artifact absolute path missing but relative path exists under current vault root | Rebind candidate count | Rebind path to current vault root after operator approval | Medium |
| File outside vault | Catalog artifact path points outside selected vault | External path count and sample names | Report; optionally restore/copy into vault with approval | Medium |
| Interrupted ingest | File copied but catalog entry missing, or catalog entry exists with missing metadata | Orphan/adoption or incomplete metadata finding | Adopt file or complete metadata deterministically | Medium |
| Catalog backup roundtrip | Exported catalog cannot load or differs after deserialize/serialize | Backup validation failure | Refuse restore and keep current catalog | High |
| Vault portability roundtrip | Vault moved to another root and catalog can resolve retained files through relative paths | Rebind candidate count | Rebind catalog paths through Apply Selected; avoid destructive edits | High |

> FilingCabinet should make vault state understandable before it makes vault state different.

## Recovery Principle

When there is doubt, preserve evidence first.

Use reports, exports, packages, and quarantine before destructive cleanup. The vault should help an operator understand what happened, not rush to make the warning disappear.


