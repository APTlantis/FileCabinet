# FileCabinet v1.2.0 — Vault Reliability and Trust Release

FileCabinet v1.2.0 strengthens the vault as a trustworthy local retention system.

Where v1.1.0 improved preview, details, and relations, this release focuses on making vault state inspectable before changing it. The result is a more accountable repair workflow: findings are visible, safe repairs require confirmation, review-only situations stay review-only, and repair actions leave a local audit trail.

---

## Highlights

### Vault Health Report

The Vault Health panel now exposes Analyze results as a real report instead of compressing repair state into a status string.

Analyze can report:

* missing retained files
* missing hashes
* hash mismatches
* duplicate hashes
* missing generated thumbnails
* orphan thumbnails
* missing extracted-text indexes
* stale extracted-text indexes
* files outside the active vault
* path rebind candidates after vault moves
* incomplete metadata from interrupted or partial ingest states

The report separates findings from actions and shows risk, proposed action, catalog mutation, retained-file impact, and approval state.

### Repair Preview And Controlled Execution

Repair candidates are now explicit and selectable.

Safe repairs can be applied only through an operator confirmation boundary. v1.2.0 treats these as safe automatic actions:

* regenerate missing thumbnails
* recompute missing hashes
* re-extract missing text indexes
* rebind stale absolute paths when the vault-relative file exists under the active vault root

Review-only findings remain visible but are skipped by Apply Selected. This includes hash mismatches, files outside the vault, incomplete metadata, duplicate hashes, and orphan generated assets.

### Repair Logging And History

Every selected repair candidate now receives a vault-local repair log entry.

Repair history is stored at:

* `catalog\repair-log.jsonl`

The Vault Health panel also shows recent repair history so operators can see what changed, what failed, and what was skipped without opening the log file manually.

### Backup Validation

Catalog backup is now validated immediately after export.

FileCabinet writes the backup, reopens it, deserializes it, and confirms that required catalog collections are present. A backup is reported as validated only when it can be read back as usable catalog data.

### Portability And Trust Findings

v1.2.0 adds health findings that make portable vault scenarios safer:

* **Path rebind candidate** when an old absolute path is missing but the vault-relative path resolves under the active vault root.
* **File outside vault** when a catalog entry points outside the selected vault.
* **Incomplete metadata** when a retained file exists but the catalog row appears partially built.

These findings are intentionally review-only. FileCabinet reports the situation, explains it, and leaves the operator in control.

---

## What This Release Improves

v1.2.0 makes FileCabinet more dependable after real use.

The app now does a better job answering:

* Is this vault internally consistent?
* Which findings are safe to repair?
* Which findings need operator judgment?
* Was a repair applied, failed, or skipped?
* Is a catalog backup actually readable?
* Did a vault move leave paths that can be rebound?

This release keeps the project centered on deliberate curation, deterministic verification, and local trust infrastructure.

---

## Design Boundaries

FileCabinet v1.2.0 does not add restore automation, destructive cleanup of orphan generated assets, or automatic resolution of hash mismatches.

The rule remains:

> FileCabinet should make vault state understandable before it makes vault state different.

---

## Built With

* WPF
* VB.NET
* .NET 10
* WiX Toolset
* Local-first storage
* Deterministic health, repair, and backup validation workflows

---

## Release Artifact

Expected installer:

* `FileCabinet-1.2.0.0-win-x64.msi`

SHA-256:

* `282E1B6DF7BE31754F4B907C8A3DBAB70AECDE09B116A2A6FF06DA6FD109DFA0`

This installer contains the self-contained Windows x64 desktop build for FileCabinet v1.2.0.
