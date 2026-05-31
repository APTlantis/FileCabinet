# FileCabinet — Stewardship & Preservation Maturity Roadmap

This document continues the FileCabinet roadmap after Priority 15.

The next phase should strengthen FileCabinet as a stewardship system. The vault already preserves files, hashes, generated assets, repair evidence, and operator metadata. The next maturity layer is preserving meaning: why artifacts matter, how collections are reported, how vaults remain portable, and how long-term health is reviewed over time.

The central direction remains:

* local-first
* deterministic
* inspectable
* repairable
* operator-focused
* preservation-oriented

FileCabinet should help an operator care for a vault across months and years without becoming a generic records-management system. The goal is practical preservation discipline for technical artifacts and the context around them.

---

# Priority 16 — Knowledge Continuity & Context Preservation

## Principle

A file can survive while its meaning disappears.

FileCabinet should make it easier to retain the human and operational context that explains why an artifact was saved, how it relates to other work, and what a future operator needs to know before trusting it.

---

## 33. Artifact Narratives

### Goals

* Preserve the story behind important artifacts
* Distinguish quick notes from long-term preservation context
* Help future operators understand why a file mattered
* Keep narrative fields operator-authored and inspectable

### Suggested Projects

* Add an artifact narrative field for important retained files
* Provide a focused editor for longer context notes
* Include narrative text in search and preservation reports
* Add filters for artifacts with missing or incomplete narrative context
* Preserve narratives through export, package, and restore workflows

### Constraints

Narratives should not be generated automatically.

Generated summaries may assist review in the future, but the preservation record should clearly distinguish operator-authored context from derived text.

---

## 34. Project Associations and Context Journals

### Goals

* Connect artifacts to projects, incidents, releases, clients, devices, or research efforts
* Preserve chronology around why groups of artifacts were retained
* Support later recall without forcing a rigid folder hierarchy
* Make contextual groupings portable with the vault

### Suggested Projects

* Add lightweight project or context association fields
* Add vault-local context journal entries
* Link journal entries to artifacts, saved views, packages, or reports
* Add filters for project/context associations
* Include context journals in preservation packages

### Long-Term Value

Context journals let the vault preserve more than isolated objects.

They help answer:

> What was happening when these artifacts were collected?

---

# Priority 17 — Preservation Reports & Evidence Packages

## Principle

A preserved vault should be able to explain itself outside the running app.

Reports and evidence packages should make vault state, artifact identity, verification status, and operator context portable enough to inspect later.

---

## 35. Vault and Collection Reports

### Goals

* Generate human-readable summaries of vault state
* Support collection-level reporting for selected artifacts
* Preserve counts, categories, trust state, review queues, and integrity status
* Make reports useful both for daily operation and long-term archive review

### Suggested Projects

* Add vault summary reports in Markdown and JSON
* Add collection reports for saved views, filters, or selected artifacts
* Include artifact counts, storage size, trust classifications, retention priorities, and verification status
* Include report generation metadata and FileCabinet version
* Add deterministic ordering for report output

### Constraints

Reports should be derived from catalog and vault state.

They should not become a second source of truth or silently rewrite catalog metadata.

---

## 36. Evidence Packages and Preservation Records

### Goals

* Package selected artifacts with the context needed to understand them later
* Include hashes, metadata, extracted text, thumbnails, repair history, and relevant notes
* Support handoff, cold storage, and incident/research preservation workflows
* Make package contents inspectable without requiring the desktop UI

### Suggested Projects

* Add evidence package generation for selected artifacts or saved views
* Include a package manifest with artifact identities and hash summaries
* Include preservation records in Markdown or JSON alongside retained files
* Add package verification commands
* Add tests for package completeness and deterministic layout

### Constraints

Evidence packages should preserve context without implying a legal chain-of-custody standard unless the operator explicitly documents that use.

---

# Priority 18 — Vault Interchange & Long-Term Portability

## Principle

A vault should remain understandable when moved between machines, drives, and future versions.

Portability is not just copying files. It means preserving identity, catalog meaning, generated assets, repair evidence, and compatibility expectations.

---

## 37. Package Specifications and Compatibility Validation

### Goals

* Define the expected structure of vault packages and evidence packages
* Make compatibility checks explicit
* Support future version transitions without ambiguity
* Help operators understand whether an older package can be trusted

### Suggested Projects

* Document required and optional package files
* Add package schema or manifest version fields
* Validate catalog, retained files, extracted text, thumbnails, and repair logs
* Warn about unknown or newer package versions
* Add CLI output suitable for scheduled compatibility checks

### Constraints

Compatibility validation should be read-only.

Warnings should explain risk without automatically altering package contents.

---

## 38. Migration Tools and Rebind Improvements

### Goals

* Improve recovery after drive-letter changes, folder moves, and archive extraction
* Make moved-vault detection explainable
* Prefer hash evidence over path similarity
* Let operators preview and approve rebind changes

### Suggested Projects

* Add migration analysis for suspected moved vaults
* Detect expected vault folder structure and retained-file hash matches
* Preview path updates before applying them
* Log migration and rebind actions in repair history
* Add tests for portable vault relocation scenarios

### Constraints

Rebind workflows should never silently accept a new vault root.

The operator should always see what evidence supports the proposed migration.

---

# Priority 19 — Preservation Analytics

## Principle

Long-term stewardship benefits from trends, not just snapshots.

FileCabinet should help operators understand vault growth, retention behavior, verification quality, and storage evolution over time while keeping analytics deterministic and local.

---

## 39. Vault Growth and Retention Trends

### Goals

* Show how the vault changes over time
* Help operators understand which categories, tags, and projects are growing
* Identify stale review areas and under-documented artifact sets
* Support archive planning without cloud telemetry

### Suggested Projects

* Record periodic vault summary snapshots
* Report growth by artifact count, storage size, category, tag, and trust state
* Track retention priority distribution over time
* Add trend reports for saved views and collections
* Include analytics snapshots in vault exports when requested

### Constraints

Analytics should be computed from local catalog and report data.

No external telemetry or service dependency should be introduced.

---

## 40. Verification Metrics and Storage Evolution

### Goals

* Track verification health across repeated checks
* Surface recurring hash, preview, text extraction, or repair findings
* Help operators identify storage media or workflow problems
* Support long-term backup and cold-storage review

### Suggested Projects

* Store verification run summaries
* Track finding counts by risk level and type
* Report time since last successful verification
* Compare current vault health to prior snapshots
* Add CLI-friendly verification trend output

### Long-Term Value

Verification metrics turn vault health into an observable practice.

They help answer:

> Is this archive becoming more trustworthy or more fragile?

---

# Priority 20 — Archive Health & Stewardship Workflows

## Principle

Preservation is a recurring practice.

FileCabinet should guide operators toward review cycles, stewardship queues, and visible risk identification without making automatic decisions about what matters.

---

## 41. Preservation Health Scores and Stewardship Queues

### Goals

* Summarize archive health in a way operators can act on
* Identify artifacts that need metadata, verification, review, or repair
* Keep every score explainable
* Avoid opaque ranking or automatic trust decisions

### Suggested Projects

* Add health indicators for missing hashes, failed verification, missing context, and stale generated assets
* Add stewardship queues for review cycles and preservation gaps
* Show reasons for each queue membership
* Add filters for high-priority or evidence-grade artifacts needing review
* Include stewardship queue counts in reports

### Constraints

Health scores should be composed from visible findings and metadata completeness.

The score should be a navigation aid, not a hidden judgment.

---

## 42. Review Cycles and Long-Term Risk Identification

### Goals

* Help operators revisit important artifacts on a schedule
* Surface long-unverified vaults and stale packages
* Identify risky preservation states before data is lost
* Support recurring review without requiring cloud services

### Suggested Projects

* Add review interval metadata for important artifacts or collections
* Add due-for-review saved views
* Add scheduled CLI examples for verification and reports
* Flag artifacts with old verification dates or repeated repair findings
* Add preservation checklist output for cold-storage review

### Constraints

Review cycles should remain operator-controlled.

FileCabinet can surface what needs attention, but the operator decides what action to take.
