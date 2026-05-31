# FileCabinet — Stronger Daily-Use Roadmap

This document continues the FileCabinet roadmap after Priority 10.

The next phase should make daily vault work faster, clearer, and more repeatable without weakening FileCabinet's core preservation model. As vaults grow, operators need better ways to review many artifacts, keep intake consistent, save working views, inspect retained files deeply, and automate routine reporting.

The central direction remains:

* local-first
* deterministic
* inspectable
* repairable
* operator-focused
* workflow-aware

FileCabinet should continue to make high-volume operations explicit and reviewable. Daily-use improvements should reduce friction, not hide decisions.

---

# Priority 11 — Bulk Operations & Workflow Tools

## Principle

As vault size grows, individual artifact management becomes inefficient.

FileCabinet should allow operators to perform common maintenance and organizational tasks across large artifact sets without sacrificing transparency.

---

## 23. Multi-Artifact Operations

### Goals

* Reduce repetitive management work
* Support large-scale metadata maintenance
* Preserve operator control

### Suggested Projects

* Multi-select artifact actions
* Bulk tag assignment
* Bulk category assignment
* Bulk trust-tier updates
* Bulk archive status updates
* Bulk note additions
* Bulk export actions

### Constraints

All bulk actions should provide:

* preview
* confirmation
* audit history

---

## 24. Workflow Queues

### Goals

Create intentional review pipelines.

### Suggested Queues

* Newly Ingested
* Needs Metadata
* Needs Review
* Unverified
* Preservation Candidate
* Repair Required

### Benefits

Operators always know:

> What needs attention next?

---

# Priority 12 — Intake Automation & Review Pipelines

## Principle

Artifact intake should be consistent, predictable, and reviewable.

---

## 25. Intake Profiles

### Goals

Apply deterministic rules during ingestion.

### Suggested Projects

* Downloads profile
* Dataset profile
* Source Code profile
* Documentation profile
* Media profile
* Custom profiles

### Example

Downloads Profile:

* Category = Review
* Trust Tier = Unreviewed
* Queue = Needs Review

---

## 26. Watch Folders & Batch Intake

### Goals

Reduce ingestion friction.

### Suggested Projects

* Watched directories
* Batch drop zones
* Recursive folder intake
* Duplicate intake warnings
* Intake preview reports

### Constraints

All automated intake remains reviewable and operator-visible.

---

# Priority 13 — Saved Views & Operational Dashboards

## Principle

Operators should be able to understand vault state at a glance.

---

## 27. Saved Views

### Goals

Create reusable working contexts.

### Suggested Views

* Recently Added
* Datasets
* Source Code
* Manuals
* Needs Review
* Critical Artifacts
* Repair Queue

### Benefits

Views become lightweight workspaces without altering storage structure.

---

## 28. Vault Dashboard

### Goals

Surface useful operational metrics.

### Suggested Widgets

* Recent Activity
* New Artifacts
* Storage Usage
* Top Categories
* Top Tags
* Review Queue Counts
* Integrity Status
* Repair Findings

### Long-Term Value

A "vault health overview" for daily use.

---

# Priority 14 — Metadata Expansion & Deep Inspection

## Principle

Artifacts become more valuable when they can be understood without opening them.

---

## 29. Metadata Enrichment

### Goals

Expose more useful artifact information.

### Suggested Projects

* Archive statistics
* Document statistics
* Image metadata
* Media metadata
* Software package metadata
* Manifest summaries
* Dependency summaries

### Constraints

Metadata remains deterministic and reproducible.

---

## 30. Archive-Aware Inspection

### Goals

Provide structural insight into retained artifacts.

### Examples

ZIP Archive:

* File count
* Folder count
* Top-level contents

Git Repository:

* Branch count
* Commit count
* Repository indicators

MSI Package:

* Product
* Publisher
* Version

### Benefits

Improves discovery without requiring extraction or execution.

---

# Priority 15 — CLI & Reporting Ecosystem

## Principle

Everything possible through the UI should eventually be available through automation.

---

## 31. First-Class CLI

### Goals

Enable automation and scripting.

### Suggested Commands

* ingest
* verify
* repair
* rescan
* search
* export
* report
* package
* restore-drill

### Benefits

* automation
* scheduled verification
* batch operations
* server-side workflows

---

## 32. Reporting Engine

### Goals

Generate portable vault intelligence.

### Suggested Reports

* Vault Summary
* Integrity Report
* Review Queue Report
* Storage Analysis
* Collection Report
* Preservation Status

### Output Formats

* JSON
* JSONL
* Markdown
* HTML

### Long-Term Value

Reports become both operational tools and preservation artifacts.
