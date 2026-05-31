# FileCabinet — Archival Maturity Roadmap

This document continues the FileCabinet roadmap after Priority 5.

The next phase should strengthen FileCabinet as a long-term preservation system: not just a vault that can retain artifacts today, but a vault that can prove, restore, migrate, and explain those artifacts years later.

The central direction remains:

* local-first
* deterministic
* inspectable
* repairable
* operator-focused
* preservation-oriented

FileCabinet should continue to avoid becoming cloud storage, an enterprise document management system, a filesystem replacement, or an automatic inference workspace. Its strength is deliberate retention with context.

---

# Priority 6 — Archival Assurance & Restore Drills

## Principle

An archive is only trustworthy if it can be restored and verified.

FileCabinet should not merely create backups, packages, or exports. It should make it practical to prove that retained artifacts, catalog context, generated assets, and repair history can survive a restore cycle.

---

## 13. Restore Drill Framework

### Goals

* Create repeatable restore exercises for real vaults and test vaults
* Validate that a vault can be restored into a clean location
* Confirm catalog, retained files, thumbnails, extracted text, and repair logs remain usable
* Produce a clear pass/fail restore drill report

### Suggested Projects

* Add a restore drill command that copies or rehydrates a vault package into a temporary location
* Run health verification against the restored copy
* Compare restored artifact counts, hashes, generated asset counts, and repair log presence
* Record restore drill results as a deterministic report
* Add tests using small packaged vault fixtures

### Constraints

Restore drills should not mutate the original vault.

The operator should always know whether the drill is testing a live vault, an exported package, or a copied restore target.

---

## 14. Package Verification & Rehydration

### Goals

* Verify deterministic vault packages before trusting them
* Rehydrate packages into usable vault folders
* Detect incomplete or corrupted package contents
* Preserve original package identity and verification results

### Suggested Projects

* Add package verification for manifest, catalog, retained files, generated assets, and repair logs
* Add package rehydration into a selected folder
* Validate that package catalog paths can be rebound safely after extraction
* Generate a package verification report in text and JSON
* Support CLI-first package verification for scheduled cold-storage checks

### Constraints

Package verification should be read-only.

Rehydration should require an explicit output folder and should never overwrite an existing live vault without operator confirmation.

---

# Priority 7 — Vault Interchange & Migration Safety

## Principle

A preserved vault should be portable without becoming ambiguous.

Migration should preserve identity, context, and trust signals while making path changes and environment differences visible.

---

## 15. Vault Package Manifest Specification

### Goals

* Formalize the deterministic vault package structure
* Define required and optional package files
* Make package validation independent of the desktop UI
* Support future compatibility checks

### Suggested Projects

* Document package manifest fields and versioning rules
* Include artifact count, hash summary, generated asset counts, export timestamp, and source vault identity
* Add manifest validation tests
* Add compatibility warnings for unknown package versions
* Record the FileCabinet app and CLI versions used to create a package

### Constraints

The manifest should describe the package, not become a second catalog.

The catalog remains the primary metadata record.

---

## 16. Vault Migration and Rebind Workflows

### Goals

* Improve recovery after drive-letter changes, folder moves, and external disk migration
* Make path rebinding explicit and reviewable
* Detect when a moved vault is intact versus partially missing
* Support clean operator approval for rebind actions

### Suggested Projects

* Add a migration analysis report
* Detect candidate vault roots based on expected folder structure and retained file hashes
* Preview path rebind changes before applying them
* Log rebind actions in the repair history
* Add tests for drive-letter and folder-root changes

### Constraints

Rebind should never silently accept a new vault root.

Hash evidence should be preferred over path similarity when deciding whether a moved retained file is trustworthy.

---

# Priority 8 — Retention Policy & Evidence Quality

## Principle

The reason an artifact was retained is part of the artifact.

FileCabinet should make it easy to preserve human intent, trust context, and evidence quality without turning into a generic records-management system.

---

## 17. Retention Reason and Trust Tier Model

### Goals

* Capture why an artifact deserves long-term retention
* Distinguish routine retained files from high-value evidence
* Support review queues based on retention priority and trust state
* Make trust state operator-authored and inspectable

### Suggested Projects

* Add structured retention reason fields
* Add trust tiers such as Unreviewed, Useful, Important, Critical, and Evidence
* Add filters for missing retention reason and unreviewed trust tier
* Include retention reason and trust tier in search
* Include these fields in export and package outputs

### Constraints

Trust tiers should not be assigned automatically.

Generated signals may suggest review, but the operator should author the final trust classification.

---

## 18. Provenance and Chain-of-Custody Notes

### Goals

* Preserve source and custody context for important artifacts
* Track when an artifact was acquired, restored, exported, repaired, or rehydrated
* Make evidence history readable without parsing raw JSON
* Support high-confidence retained records for operational and technical work

### Suggested Projects

* Add provenance notes for acquisition source, operator, method, and related incident or project
* Add chain-of-custody timeline entries for major artifact events
* Surface custody history in the details panel
* Include custody history in reports and packages
* Add tests that custody entries remain stable through export/import

### Constraints

Chain-of-custody should describe FileCabinet operations and operator-authored notes.

It should not imply legal evidence handling unless the operator explicitly documents that context.

---

# Priority 9 — Operational Review Queues & Auditability

## Principle

Long-term vault health depends on recurring human review.

FileCabinet should help operators see what needs attention without automatically deciding what matters.

---

## 19. Scheduled Verification and Risk Review Queues

### Goals

* Make recurring vault verification practical
* Surface stale, risky, incomplete, or unreviewed artifacts
* Support CLI-driven scheduled checks
* Help operators prioritize review work

### Suggested Projects

* Add saved verification reports
* Add review queues for high-risk findings, missing retention reasons, unverified hashes, stale extracted text, and missing previews
* Add CLI output designed for scheduled task logs
* Add optional report folders under vault exports
* Add tests for queue membership and deterministic ordering

### Constraints

Scheduled verification should not mutate vault state.

Review queues should explain why each artifact appears in the queue.

---

## 20. Human-Readable Activity and Repair Timeline

### Goals

* Make vault history understandable from the UI and exported reports
* Combine artifact activity, repair logs, hash checks, restores, exports, and package events
* Help operators answer what changed and when
* Preserve auditability without hiding raw records

### Suggested Projects

* Add a unified timeline view for artifact and vault events
* Include repair logs and CLI operations in timeline reports
* Add filters for event type and risk level
* Include timeline excerpts in package and health reports
* Add tests for chronological ordering and stable event formatting

### Constraints

The timeline should be derived from recorded events.

It should not invent explanations that are not present in catalog, repair log, or operator-authored metadata.

---

# Priority 10 — Preservation Ecosystem & Release Discipline

## Principle

The software that preserves artifacts should preserve its own release context.

FileCabinet releases, installers, manifests, documentation, and verification evidence should be managed with the same care as retained vault artifacts.

---

## 21. Release Artifact Ledger

### Goals

* Track release artifacts with hashes, sizes, versions, dates, and notes
* Preserve which installer produced which vault package or catalog version
* Make release evidence easy to publish and inspect
* Improve confidence when reinstalling older versions

### Suggested Projects

* Add a release ledger document or JSONL record
* Record MSI path, SHA-256, size, version, build date, and release notes path
* Include CLI version and package format version
* Add a script that verifies release artifacts against the ledger
* Add release checklist entries for tests, build, installer, smoke checks, and manifest updates

### Constraints

The ledger should be append-only in spirit.

Correcting a release record should preserve the reason for the correction.

---

## 22. Cold Storage and Backup Guidance

### Goals

* Provide practical guidance for long-term vault storage
* Explain what must be backed up together
* Define recommended verification intervals
* Help operators choose between live vaults, exports, packages, and installer archives

### Suggested Projects

* Add a cold-storage guide
* Document recommended backup sets
* Provide CLI examples for verify, report, package, and package verification
* Add restore drill examples
* Include a printable release-and-vault preservation checklist

### Constraints

FileCabinet should not pretend to replace backup software.

The guidance should focus on preserving artifact context, not prescribing one storage vendor or medium.

