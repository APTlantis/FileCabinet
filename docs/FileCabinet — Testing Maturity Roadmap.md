# FileCabinet — Testing Maturity Roadmap

This document is a companion roadmap for the full FileCabinet roadmap sequence.

The main roadmap describes what FileCabinet should become. The testing roadmap describes how the project proves those promises are true. For FileCabinet, tests are not only implementation safety. They are part of the preservation model: the vault should be able to prove what it kept, why it kept it, whether it changed, and whether it can be restored.

The central direction remains:

* deterministic
* fixture-driven
* regression-resistant
* script-friendly
* preservation-oriented
* operator-trust-focused

Testing should mature alongside the product. Each preservation feature should gain enough automated evidence that future changes can improve the vault without weakening its core promises.

---

# Priority T1 — Core Determinism Tests

## Principle

The behaviors that identify, order, serialize, and verify artifacts must not drift accidentally.

FileCabinet should have focused tests for every deterministic contract that later features depend on.

---

## T1.1 Deterministic Identity and Hashing

### Goals

* Prove that content identity remains stable
* Prevent accidental changes to hash behavior
* Keep duplicate and relation signals predictable
* Make generated identifiers and names testable where determinism is expected

### Suggested Projects

* Add stable hash calculation tests for SHA-256 and BLAKE3
* Verify duplicate detection behavior across repeated catalog loads
* Test generated asset naming for thumbnails, extracted text, reports, and packages
* Test path normalization for vault-relative and moved-root scenarios
* Add regression tests for relation signals that depend on hashes or shared path context

### Constraints

Tests should distinguish deterministic product contracts from intentionally unique values such as new GUIDs.

When randomness or timestamps are required, tests should inject or normalize those values rather than accepting unstable output.

---

## T1.2 Stable Serialization and Ordering

### Goals

* Keep catalog and report output predictable
* Protect script and golden-file workflows from accidental ordering drift
* Make exported preservation evidence easier to diff
* Support long-term compatibility checks

### Suggested Projects

* Test catalog serialization round trips
* Verify deterministic report ordering
* Normalize timestamps and machine-specific paths in snapshot tests
* Add JSON schema or shape assertions for report and package metadata
* Test stable ordering for duplicate groups, health findings, repair candidates, relations, and search results

### Long-Term Value

Stable output makes FileCabinet easier to automate, inspect, compare, and preserve.

---

# Priority T2 — Vault Fixture Library

## Principle

FileCabinet needs small, strange, reusable vaults that make important edge cases easy to test.

Fixtures should become the backbone for new features, repairs, migrations, reports, and package validation.

---

## T2.1 Canonical Test Vaults

### Goals

* Make common vault states reproducible
* Reduce duplicated setup across tests
* Preserve known edge cases as reusable assets
* Help future contributors understand expected vault shape

### Suggested Projects

* Build an empty vault fixture
* Build a tiny normal vault fixture
* Build duplicate-file, missing-file, and missing-generated-asset fixtures
* Build corrupted-catalog and incomplete-metadata fixtures
* Build image, document, media, archive-heavy, and installer-heavy fixtures

### Constraints

Fixtures should stay small enough for fast local test runs.

Large or slow fixtures should be opt-in integration fixtures, not required for every unit test.

---

## T2.2 Migration and Package Fixtures

### Goals

* Reproduce moved-drive and moved-folder cases
* Support package, export, restore, and rehydration tests
* Make compatibility behavior explicit
* Keep future package-format changes testable

### Suggested Projects

* Create moved-drive and moved-root vault fixtures
* Create partial-copy and orphan-file fixtures
* Create package/export fixtures with catalog, files, thumbnails, extracted text, and repair logs
* Add helper APIs for copying fixtures into temporary test roots
* Add fixture documentation that explains what each vault is meant to prove

### Long-Term Value

A fixture library lets FileCabinet test preservation behavior with realistic vault states without relying on a developer's personal archive.

---

# Priority T3 — Intake & Workflow Tests

## Principle

Daily-use features should be tested as workflows, not only as isolated helpers.

FileCabinet's intake and review flows must remain predictable because they are where preservation context enters the vault.

---

## T3.1 Intake Path Coverage

### Goals

* Prove that files enter the vault consistently
* Preserve copy/move semantics
* Keep batch behavior observable
* Protect shell and drag/drop intake from regressions

### Suggested Projects

* Test copy and move intake behavior
* Test batch intake and recursive folder intake
* Test shell request parsing for Copy to FileCabinet and Move to FileCabinet
* Test duplicate intake rename behavior
* Add tests for intake status, activity entries, generated hashes, extracted text, and thumbnail state

### Constraints

Tests should avoid depending on the live Windows shell.

Shell and drag/drop behavior should be covered through command/request parsing and service-level workflows where possible.

---

## T3.2 Review and Bulk Workflow Tests

### Goals

* Support the daily-use roadmap safely
* Prove that bulk actions preserve operator control
* Ensure review queues remain explainable
* Keep audit history attached to meaningful actions

### Suggested Projects

* Test bulk tag, category, trust, and archive-status changes
* Test preview and confirmation models for bulk operations
* Test queue assignment from intake profiles
* Test saved views and operational dashboard counts
* Test audit trail entries after bulk operations

### Constraints

Bulk actions should have tests for preview, confirmation, and audit history before they become routine UI workflows.

---

# Priority T4 — Preservation Integrity Tests

## Principle

The vault's trust model needs automated proof.

Integrity tests should cover retained files, generated assets, extracted text, repair logs, packages, and restore drills.

---

## T4.1 Vault Health and Repair Evidence

### Goals

* Verify retained files against catalog hashes
* Detect missing or stale generated assets
* Confirm repair findings are risk-classified correctly
* Preserve repair history through catalog and vault operations

### Suggested Projects

* Test missing files, hash mismatches, missing hashes, and duplicate hashes
* Test missing thumbnails and stale extracted-text indexes
* Test repair-log persistence and chronological ordering
* Test review-only findings versus automatically repairable findings
* Test that hash mismatches are never silently accepted as repaired

### Constraints

Integrity tests should protect cautious behavior.

High-risk findings must remain operator-reviewed unless a future roadmap explicitly changes that policy.

---

## T4.2 Package Verification and Restore Drills

### Goals

* Prove that packages contain required preservation evidence
* Verify package contents before trusting them
* Confirm restore drills produce clear pass/fail results
* Test rehydration into clean folders

### Suggested Projects

* Test package manifest contents
* Test package verification for catalog, retained files, generated assets, and repair logs
* Test restore drill reports against known-good and intentionally broken packages
* Test rehydration path rebinding after extraction
* Add package compatibility warning tests

### Long-Term Value

Restore and package tests turn FileCabinet's archival promise into something repeatable.

---

# Priority T5 — Migration & Rebind Tests

## Principle

Moved vaults should be recoverable without becoming ambiguous.

Migration tests should enforce the rule that hash evidence beats path similarity.

---

## T5.1 Moved Vault and Drive-Letter Scenarios

### Goals

* Validate drive-letter and folder-root changes
* Detect intact moved vaults
* Distinguish partial copies from complete migrations
* Keep rebind actions previewable

### Suggested Projects

* Test drive-letter changed scenarios
* Test vault folder moved scenarios
* Test retained files moved with matching hashes
* Test partial vault copy detection
* Test migration analysis report output

### Constraints

Tests should not require specific drive letters or physical removable media.

Drive and root changes should be simulated with temporary folders and normalized paths.

---

## T5.2 Rebind Safety and Auditability

### Goals

* Prevent unsafe path rebinding
* Ensure mismatches remain visible
* Preserve operator approval boundaries
* Record rebind evidence in repair history

### Suggested Projects

* Test path match with hash mismatch
* Test hash match with changed path
* Test rebind preview output
* Test approved rebind application
* Test rebind audit history and report entries

### Constraints

Rebind tests should fail if path similarity is trusted without supporting hash evidence for retained content.

---

# Priority T6 — Report & CLI Golden Tests

## Principle

The CLI and reports are preservation interfaces.

Their output should be stable enough for scripts, scheduled verification, cold-storage checks, and historical comparison.

---

## T6.1 CLI Command Golden Tests

### Goals

* Protect CLI stdout, stderr, exit codes, and JSON output
* Keep command behavior script-friendly
* Verify mutating commands require explicit approval
* Normalize machine-specific output for stable comparisons

### Suggested Projects

* Add golden tests for `verify`, `report`, `search`, `package`, and future `restore-drill`
* Test JSON output shape and required fields
* Test Markdown output contains expected sections
* Test exit code `2` for threshold-level verification findings
* Test mutating commands require `--apply` and `--yes`

### Constraints

Golden files should normalize dates, absolute paths, and environment-specific values.

Tests should make intentional output changes easy to review.

---

## T6.2 Report Schema and Rendering Tests

### Goals

* Verify reports remain useful as preservation artifacts
* Keep report ordering deterministic
* Support future HTML and Markdown report outputs
* Make report compatibility visible

### Suggested Projects

* Test vault summary, integrity, review queue, storage, and preservation status reports
* Add schema checks for JSON and JSONL report outputs
* Add section checks for Markdown and future HTML reports
* Test report generation from fixture vaults
* Test report inclusion in packages and evidence bundles

### Long-Term Value

Report tests make FileCabinet's external evidence trustworthy, not just readable.

---

# Priority T7 — Stewardship Context Tests

## Principle

Preserving meaning needs tests too.

Narratives, project associations, custody entries, evidence packages, review queues, and health scores should remain explainable across export, package, migration, and restore workflows.

---

## T7.1 Context Preservation Tests

### Goals

* Prove that operator-authored meaning survives long-term workflows
* Keep context fields searchable and exportable
* Protect chronology for custody and activity records
* Preserve the difference between authored and generated context

### Suggested Projects

* Test narratives survive export, package, restore, and import
* Test project associations and context journals through package workflows
* Test custody entries keep chronological order
* Test retention reason, provenance, trust, priority, and archive status in reports
* Test generated suggestions remain distinguishable from operator-authored context

### Constraints

Context tests should protect operator authorship.

Future generated summaries should never overwrite or blur authored preservation records.

---

## T7.2 Stewardship Queue and Health Score Tests

### Goals

* Keep review queues explainable
* Ensure health scores are derived from visible evidence
* Prevent opaque ranking from becoming the source of truth
* Support long-term preservation review cycles

### Suggested Projects

* Test why each item appears in a review queue
* Test health score inputs and summary output
* Test due-for-review and stale-verification queues
* Test preservation analytics snapshots
* Test archive estate reports across multiple fixture vaults

### Long-Term Value

These tests make stewardship features accountable.

They help prove that FileCabinet is preserving meaning, not merely storing files.

