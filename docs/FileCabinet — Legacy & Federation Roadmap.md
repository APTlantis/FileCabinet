# FileCabinet — Legacy & Federation Roadmap

This document continues the FileCabinet roadmap after Priority 20.

The next phase should consider what happens when FileCabinet is no longer managing a single active vault on one machine. Mature preservation eventually involves multiple vaults, cold-storage copies, historical snapshots, recovery kits, and archives that should remain understandable even if the original application is unavailable.

The central direction remains:

* local-first
* deterministic
* inspectable
* repairable
* operator-focused
* ecosystem-aware

FileCabinet should preserve entire archive ecosystems without becoming cloud storage or an enterprise records platform. This feels very Aptlantis: practical, self-describing, and built for long-term continuity.

---

# Priority 21 — Multi-Vault Awareness

## Principle

Operators may eventually care for more than one vault.

FileCabinet should support awareness across active vaults, restored vaults, cold-storage vaults, and external archive media while keeping each vault independently understandable.

---

## 43. Vault Registry and Discovery

### Goals

* Track known vaults without centralizing their data
* Distinguish active, offline, restored, archived, and test vaults
* Help operators find vaults across local drives and external media
* Preserve each vault's identity and last-known health state

### Suggested Projects

* Add a local vault registry with vault identity, path, status, and last-seen timestamp
* Detect likely vault roots from expected folder structure and manifest files
* Show whether a vault is online, missing, moved, or read-only
* Record last verification summary per known vault
* Add registry export for disaster recovery

### Constraints

The registry should reference vaults, not absorb them.

Each vault should remain portable and usable without the registry.

---

## 44. Cross-Vault Search and Statistics

### Goals

* Search across known online vaults
* Identify duplicate or related artifacts across vault boundaries
* Show aggregate storage, category, tag, trust, and verification summaries
* Support archive review without merging vaults

### Suggested Projects

* Add read-only cross-vault search over selected online vaults
* Include vault identity in each result
* Add cross-vault duplicate detection using SHA-256 and BLAKE3
* Add aggregate statistics for selected vault sets
* Add CLI reporting for multi-vault summaries

### Constraints

Cross-vault operations should be read-only by default.

Actions that alter a vault should happen inside that vault's normal repair or metadata workflow.

---

# Priority 22 — Federated Reporting

## Principle

An archive estate needs rollups without losing local evidence.

Federated reports should summarize multiple vaults while preserving links back to each vault's own catalog, reports, and verification evidence.

---

## 45. Multi-Vault Reports and Collection Rollups

### Goals

* Generate reports across selected vaults
* Summarize collections that span more than one vault
* Preserve per-vault counts, health, and identity
* Support periodic archive estate review

### Suggested Projects

* Add multi-vault summary reports in Markdown and JSON
* Include per-vault health, artifact counts, storage totals, and verification age
* Add collection rollups by project, tag, category, or trust tier
* Link report sections back to source vault/package identifiers
* Add deterministic ordering by vault identity and artifact key

### Constraints

Rollups should not flatten away source context.

Every aggregate number should be traceable back to its contributing vaults.

---

## 46. Archive Estate Dashboards and Global Integrity Reports

### Goals

* Provide an at-a-glance view of archive estate health
* Identify offline, stale, unverified, or high-risk vaults
* Support scheduled reporting for long-term archive maintenance
* Keep dashboard state local and explainable

### Suggested Projects

* Add an archive estate dashboard for known vaults
* Show last verification date, risk counts, storage usage, and review status
* Add global integrity reports for selected vault groups
* Add CLI commands for scheduled multi-vault health checks
* Add exportable dashboard snapshots for cold-storage records

### Constraints

Dashboards should summarize recorded evidence.

They should not imply that offline vaults are healthy merely because they were healthy the last time they were seen.

---

# Priority 23 — Vault Lineage & Historical Snapshots

## Principle

Long-term archives change, and those changes should be understandable.

FileCabinet should help operators compare vault states over time, track package lineage, and understand how an archive evolved.

---

## 47. Snapshot Tracking and Historical Comparisons

### Goals

* Record stable summaries of vault state at important moments
* Compare current vault state to prior snapshots
* Support restore, migration, package, and release review
* Make archive evolution visible without diffing raw JSON manually

### Suggested Projects

* Add snapshot records for exports, packages, verification runs, and restore drills
* Compare artifact counts, hash sets, storage totals, generated asset counts, and finding counts
* Report added, removed, moved, or changed artifacts
* Link snapshots to package manifests and repair logs
* Add tests for deterministic snapshot diff output

### Constraints

Snapshots should be summaries and evidence pointers.

They should not duplicate the entire catalog unless explicitly exported as part of a package.

---

## 48. Timeline Diff Reports and Vault Evolution Analysis

### Goals

* Help operators understand what changed between two points in time
* Surface unexpected artifact churn or verification degradation
* Support archive audits and restore validation
* Preserve human-readable history alongside machine-readable reports

### Suggested Projects

* Generate timeline diff reports between snapshots
* Highlight new artifacts, missing artifacts, hash changes, metadata changes, and repair events
* Add risk-focused summaries for changes that need review
* Include relevant operator notes and context journal entries
* Add CLI support for comparing two snapshots or packages

### Long-Term Value

Vault lineage helps answer:

> How did this archive become what it is today?

---

# Priority 24 — Preservation Kits & Self-Describing Archives

## Principle

A vault should remain understandable even if FileCabinet disappears.

Preservation kits should carry enough documentation, manifests, checksums, and human-readable guidance that a future operator can inspect retained artifacts without relying on institutional memory.

---

## 49. Recovery Kits and Human-Readable Specifications

### Goals

* Package the instructions needed to understand and restore a vault
* Document vault layout, catalog format, hash fields, generated assets, and repair logs
* Support cold-storage handoff and disaster recovery
* Make archive contents inspectable with ordinary tools

### Suggested Projects

* Generate a recovery kit with README, package manifest, checksums, and restore instructions
* Include a human-readable catalog field guide
* Include CLI examples for verify, report, package, and rehydrate workflows
* Add release/app version information and compatibility notes
* Add printable preservation checklist output

### Constraints

Recovery kits should explain the vault; they should not become another mutable control plane.

The retained files and catalog remain the source records.

---

## 50. Catalog Documentation and Portable Preservation Packages

### Goals

* Make catalog data understandable outside the app
* Preserve package structure in a stable, inspectable form
* Support future migration by humans or tools
* Improve confidence that archived vaults can outlive the current UI

### Suggested Projects

* Document catalog fields and their preservation meaning
* Add package-level checksum manifests
* Include sample queries or inspection recipes for JSON/JSONL catalog files
* Add package validation summaries in plain text and JSON
* Add tests that package docs match actual package contents

### Long-Term Goal

A vault remains understandable even if FileCabinet disappears.

---

# Priority 25 — Archive Estate Management

## Principle

Long-term preservation is an estate-management problem.

Operators need to know which vaults are active, which are cold, which media need verification, and which preservation rotations are overdue.

---

## 51. Active, Cold Storage, and External Archive Media Tracking

### Goals

* Distinguish live working vaults from cold-storage copies and external media
* Track where preservation copies exist
* Record last verification and restore-drill evidence per copy
* Help operators avoid losing track of offline archives

### Suggested Projects

* Add archive media records for external disks, network shares, and package locations
* Track media label, vault identity, package identity, last seen date, and verification date
* Add reports for offline vault inventory
* Add prompts or saved views for overdue verification
* Include media records in estate reports

### Constraints

Media tracking should be descriptive.

FileCabinet should not assume ownership of devices or storage systems outside the selected vault workflows.

---

## 52. Preservation Rotation and Backup Verification Across Vaults

### Goals

* Support recurring verification across multiple vaults and media
* Identify stale packages, unverified cold copies, and missing restore drills
* Produce practical maintenance checklists
* Help operators prove that archive copies remain usable

### Suggested Projects

* Add rotation schedules for vault verification and package checks
* Add cross-vault backup verification reports
* Add restore-drill status per active or cold-storage vault
* Add CLI examples for scheduled estate verification
* Add exportable maintenance checklists for archive review

### Constraints

Preservation rotation should remain operator-driven.

FileCabinet should surface due work and recorded evidence, while the operator chooses the storage policy and schedule.
