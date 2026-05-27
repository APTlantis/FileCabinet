# FileCabinet — Vault Integrity Validation Matrix

This matrix defines how FileCabinet should validate, explain, and recover vault health without relying on automatic inference.

The goal is operational trust: every recovery path should be deterministic, inspectable, and safe for an operator to approve.

---

## Validation Principles

* Detect before mutating.
* Explain every finding in plain language.
* Prefer repairable catalog state over silent cleanup.
* Preserve retained files unless the operator explicitly deletes them.
* Treat generated assets as rebuildable, not authoritative.
* Keep vault-owned paths portable through relative metadata wherever possible.

---

## Integrity Matrix

| Scenario | Detection | Expected Report | Safe Repair Action | Risk Level | Test Coverage |
|---|---|---|---|---|---|
| Stored file missing | Catalog artifact path is empty or file does not exist | Missing file count and sample artifact names | Mark as missing; do not remove catalog row automatically | Medium | Existing repair report behavior |
| Duplicate retained file | Two or more artifacts share SHA-256 | Duplicate hash group count and samples | Report candidates; operator decides whether to keep both | Low | Existing duplicate hash tests should be expanded |
| Orphan file under `items` | File exists in vault `items` but no catalog artifact points to it | Orphan count and sample filenames | Rescan adopts as cataloged artifact | Low | Existing rescan/adoption tests |
| Missing generated thumbnail | Artifact says thumbnail was generated but file is absent | Missing thumbnail count and sample artifact names | Regenerate thumbnail from retained source file | Low | ThumbnailService missing-thumbnail test |
| Failed thumbnail generation | Image thumbnail generation throws or source cannot decode | Thumbnail status is `Generation failed` | Leave retained artifact intact; retry during repair if appropriate | Low | Add decode-failure fixture |
| Orphan thumbnail | Thumbnail file exists but no artifact references it | Orphan thumbnail count and samples | Report first; cleanup should require approval | Low | Needed |
| Stale extracted-text index | Extracted text file exists but no artifact references it | Stale index count and samples | Report first; cleanup should require approval | Low | Needed |
| Missing extracted-text index | Artifact references extracted text path but file is absent | Missing index count and sample artifact names | Re-extract if source format is supported | Medium | Needed |
| Hash missing | Artifact has no BLAKE3 or SHA-256 | Unverified or incomplete hash count | Recompute hashes from retained source file | Low | Hash-check tests can expand |
| Hash mismatch | Recomputed hash differs from catalog hash | Mismatch count and affected artifact names | Mark as mismatch; do not overwrite trusted hash automatically | High | Needed |
| Relative path broken after vault move | Artifact absolute path missing but relative path exists under current vault root | Rebind candidate count | Rebind path to current vault root after operator approval | Medium | Needed |
| File outside vault | Catalog artifact path points outside selected vault | External path count and sample names | Report; optionally restore/copy into vault with approval | Medium | Needed |
| Interrupted ingest | File copied but catalog entry missing, or catalog entry exists with missing metadata | Orphan/adoption or incomplete metadata finding | Adopt file or complete metadata deterministically | Medium | Needed |
| Catalog backup roundtrip | Exported catalog cannot load or differs after deserialize/serialize | Backup validation failure | Refuse restore and keep current catalog | High | Needed |
| Vault portability roundtrip | Vault moved to another root and catalog cannot rebind | Portability validation failure | Present rebind workflow; avoid destructive edits | High | Needed |

---

## Priority 1 Implementation Targets

### Vault Health Report

Create a richer health report that separates analysis from action.

The report should include:

* missing files
* duplicate hash groups
* orphan item files
* missing generated thumbnails
* orphan thumbnails
* missing extracted-text indexes
* stale extracted-text indexes
* missing hashes
* hash mismatches
* path rebind candidates
* files outside the active vault

### Repair Preview / Dry Run

Repair should gain an analysis-only mode before adding more automatic actions.

The operator should be able to see:

* finding type
* affected artifact or file
* proposed action
* whether the action mutates catalog metadata
* whether the action touches retained files
* risk level

### Backup / Restore Validation

Backup should be treated as a trust workflow, not just a file export.

Validation should check:

* catalog JSON can be parsed
* required catalog collections exist
* artifact references are internally consistent
* vault-relative paths can resolve
* generated assets can be missing and rebuilt
* retained files are never deleted during validation

---

## Acceptance Criteria

Priority 1 is considered stable when FileCabinet can:

* produce a deterministic vault health report without mutating files
* distinguish findings from repairs
* regenerate missing generated thumbnails
* adopt orphan item files through rescan
* identify missing/stale generated assets
* validate catalog backups before restore
* document every repair action in operator-readable language

The guiding rule is simple:

> FileCabinet should make vault state understandable before it makes vault state different.
