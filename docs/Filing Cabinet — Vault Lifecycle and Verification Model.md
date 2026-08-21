# Filing Cabinet — Vault Lifecycle and Verification Model

Filing Cabinet treats a retained file as an artifact with a lifecycle, not as a loose item in a folder.

That lifecycle is deliberately conservative. Every stage should preserve bytes, context, operator intent, and trust evidence so the vault remains understandable years later even if the original project folder, download page, vendor portal, device, or working context is gone.

The lifecycle is:

```text
Capture
  ↓
Classification
  ↓
Enrichment
  ↓
Verification
  ↓
Recovery
  ↓
Packaging
```

Trust is maintained throughout that lifecycle by keeping vault state inspectable rather than hidden behind an opaque index or remote service.

## 1. Capture

Capture begins when an operator deliberately chooses to retain a file or folder.

Filing Cabinet supports both copy and move intake:

- **Copy intake** retains an independent vault copy while leaving the source in place.
- **Move intake** transfers ownership to the vault after the retained copy has been assembled.

The important distinction is intent.

A file is not merely indexed. It has been selected for preservation.

Capture should preserve enough information to explain where the artifact came from and how it entered the vault.

## 2. Classification

After capture, Filing Cabinet records deterministic metadata describing the artifact and its storage context.

That may include:

- original name
- original source path
- retained vault path
- category
- type family
- size
- timestamps
- content hashes
- source batch information
- operator notes
- tags
- custom metadata
- trust and retention fields

Automatic classification should remain modest.

Generated labels can assist organization, but operator-authored context remains more important than automatic inference.

The catalog should record facts and operator decisions without quietly inventing meaning that was never supplied.

## 3. Enrichment

Filing Cabinet may create local supporting assets when appropriate.

These can include:

- thumbnails for previewable image files
- fallback preview cards for retained binary families
- extracted text for text-like artifacts
- relation hints based on shared metadata, hashes, source batches, and naming

These assets improve recall and navigation.

They do not replace the retained source file.

Generated state should remain rebuildable whenever possible so a missing thumbnail or stale text index does not become equivalent to losing the retained artifact itself.

## 4. Verification

Verification compares catalog intent against vault reality.

Filing Cabinet does not ask the operator to trust an opaque index. It exposes discrepancies between structured catalog state and the local filesystem.

Verification can identify several forms of drift.

### Existence Drift

The catalog points to a retained file that is no longer present.

This may indicate:

- an unavailable or unmounted vault path
- accidental deletion
- incomplete migration
- interrupted recovery
- storage loss

Missing retained content is materially different from missing generated state and should receive correspondingly higher attention.

### Content Drift

A retained file exists, but its current content hash does not match the value stored in the catalog.

A mismatch means the retained content can no longer be assumed to be the same content that was originally recorded.

Filing Cabinet should report the mismatch rather than silently accepting a replacement hash.

### Preview Drift

Generated thumbnails or fallback preview assets are missing or stale.

Because these are derived assets, they can usually be rebuilt from the retained source file.

This is generally lower risk than missing or changed retained content.

### Text Drift

Extracted text is missing, stale, or disconnected from the retained artifact.

This can degrade search and recall without necessarily threatening the retained source itself.

When the source format remains supported, extracted text can usually be regenerated.

### Ownership Drift

A catalog entry points to a file outside the selected vault.

This weakens the vault's ownership boundary and can make future migration, packaging, backup, or recovery less reliable.

Filing Cabinet should report the condition and allow the operator to decide whether the artifact should be restored or copied into the vault.

### Catalog Drift

Required metadata is missing, incomplete, or internally inconsistent.

Catalog problems may affect:

- search
- repair
- portability
- relationship discovery
- verification
- future recovery

They should be reported explicitly rather than silently normalized when the correct intent is ambiguous.

### Filesystem Drift

Files exist under Filing Cabinet's retained storage but have no corresponding catalog entry.

These orphan files may result from interrupted ingest, manual filesystem changes, recovery work, or prior failures.

Rescan can identify them as possible adoption candidates, but Filing Cabinet should not assume that every unknown file belongs in the catalog.

## Trust Anchors

Filing Cabinet relies on a small set of understandable trust anchors:

- retained files live in the local vault filesystem
- catalog state is stored in structured local data
- content identity is represented by hashes
- generated assets are rebuildable
- repair actions are recorded
- destructive actions require explicit operator intent

These anchors allow the vault to remain understandable without:

- a service account
- a cloud API
- a remote metadata server
- a hidden database service

The operator can inspect the actual retained files and the catalog state that describes them.

## Risk Levels

Verification findings use risk levels to help prioritize review.

### Low

Low-risk findings generally involve generated or derived state that can probably be rebuilt.

Examples include:

- missing thumbnails
- orphan thumbnails
- failed preview generation
- some missing derived metadata

Low risk does not mean irrelevant. It means the retained source itself is probably still intact.

### Medium

Medium-risk findings indicate degraded context, portability, recall, or ownership.

Examples can include:

- missing extracted text
- broken relative paths
- files outside the vault
- incomplete metadata
- interrupted ingest state

These findings deserve review because they may become more serious over time.

### High

High-risk findings indicate that retained content or core trust evidence may be compromised.

Examples include:

- hash mismatches
- failed catalog backup validation
- unresolved vault portability failures
- missing or materially changed retained content

High risk does not mean Filing Cabinet should repair automatically.

It means the operator should review the condition before trusting or mutating the affected artifact.

## Metadata-First Health Analysis

Routine health analysis should be useful without requiring every retained file to be read end-to-end.

Filing Cabinet therefore favors metadata-first analysis for ordinary reporting.

The system does not automatically compute or verify every retained-file hash simply to produce a health report.

Where configured, bounded verification can check smaller files while deferring more expensive reads.

Explicit hash verification remains available when the operator wants stronger evidence.

This keeps routine health analysis practical while preserving the ability to perform full verification deliberately.

## 5. Recovery

Recovery paths are intentionally explicit.

Filing Cabinet should make the current vault state understandable before it changes that state.

Available recovery operations include:

- **Restore Copy** — return a retained file to a chosen location without removing it from the vault.
- **Quarantine** — isolate questionable retained files without immediately deleting them.
- **Repair candidates** — describe possible fixes before application.
- **Rescan adoption** — bring legitimate orphan retained files back under catalog management.
- **Rebuild generated assets** — regenerate thumbnails or extracted text when source material is intact.
- **Delete Forever** — explicitly perform irreversible removal.

Ambiguous conditions should favor preservation and review over aggressive cleanup.

## Repair Philosophy

Repair should be deterministic, cautious, and reversible where possible.

Automatic repair is most appropriate for clearly derived state when the retained source is present.

Examples include:

- rebuilding thumbnails
- regenerating extracted text
- reconstructing missing derived metadata

Operator review is more important when content identity or ownership is uncertain.

Examples include:

- duplicate retained files
- unexpected orphan files
- hash mismatches
- missing source content
- external paths
- path rebinding after migration

Filing Cabinet should not make a warning disappear by rewriting trusted history.

The goal is to restore understandable state, not merely clean up a report.

## Repair Logging

Repair history is part of the trust model.

When vault state changes, Filing Cabinet should preserve enough information to explain:

- what changed
- when it changed
- what operation caused the change
- what artifact was affected
- what outcome was reported

A future operator should not have to guess whether a difference resulted from intentional repair, manual intervention, migration, or unexplained drift.

## 6. Packaging

For cold storage, migration, or recovery preparation, Filing Cabinet can produce a deterministic vault package.

A package may include:

- catalog JSON
- catalog JSONL
- retained items
- thumbnails
- extracted text
- repair logs
- vault health reports

Packaging preserves both artifact bytes and the surrounding operational context required to understand them later.

The purpose is not merely to compress a folder.

It is to create a portable preservation unit containing the evidence needed to reconstruct the vault's meaning.

## Headless Verification

The CLI exposes the same trust and verification model for scripts and scheduled workflows.

Examples include:

```powershell
FilingCabinet.Cli.exe verify --fail-on medium
FilingCabinet.Cli.exe report --format json --output K:\Filing Cabinet\exports\health.json
```

A threshold-level verification result can produce a non-zero exit code so automated checks fail visibly without mutating vault state.

Mutating CLI operations should remain explicit and should require deliberate confirmation.

This preserves a clear separation between:

```text
inspect
report
verify
```

and:

```text
repair
rescan
rebuild
delete
```

## Lifecycle Principle

Every stage of the artifact lifecycle should preserve enough evidence to answer:

- What is this?
- Where did it come from?
- Why is it here?
- Has the retained content changed?
- Is supporting generated state complete?
- Does the catalog still agree with the filesystem?
- What repairs or recoveries have occurred?
- Can the artifact be moved, restored, or packaged safely?

The governing principle is simple:

> Filing Cabinet should make vault state understandable before it makes vault state different.