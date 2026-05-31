# FileCabinet — Deterministic Vault Roadmap

## Strategic Direction

The central direction remains:

* local-first
* deterministic
* inspectable
* repairable
* operator-focused

artifact retention system.

The project already demonstrates strong operational maturity through:

* structured vault storage
* metadata preservation
* integrity verification
* preview pipelines
* deterministic relations
* repair/rescan workflows
* cautious intake behavior

The next phase should focus on strengthening reliability, portability, documentation clarity, and long-term archival trust rather than introducing automatic inference.

---

# Priority 1 — Vault Reliability & Trust

## Principle

The vault should be able to prove that retained files, catalog records, generated assets, and repair evidence still agree.

Reliability work should make recovery safer without hiding uncertainty from the operator.

---

## 1. Backup / Restore Validation

The backup and recovery workflows now matter enough to formally validate.

### Goals

* Verify full catalog export/import roundtrips
* Test recovery from partial corruption
* Validate orphan recovery behavior
* Ensure vault portability between systems and drives

### Suggested Tasks

* Build repeatable recovery test scenarios
* Add “vault health” verification report
* Validate relative-path resilience
* Test missing thumbnail recovery
* Test missing source file handling
* Test interrupted ingest recovery

### Deliverable

A documented “vault integrity validation matrix.”

---

## 2. Repair & Rescan Hardening

Repair/rescan is becoming core infrastructure.

### Goals

* Make all recovery operations deterministic and explainable
* Ensure repair actions are observable before execution

### Suggested Tasks

* Dry-run repair mode
* Repair preview report
* Detect catalog/file mismatches
* Detect missing generated assets
* Detect orphan thumbnails
* Detect stale extracted-text indexes
* Add repair action logging/history

### Important Direction

Repair operations should remain:

* transparent
* reversible where possible
* explicitly operator-approved

---

## 3. Vault Portability Testing

The app philosophy strongly benefits from portable vaults.

### Goals

* Ensure vaults survive:

  * drive letter changes
  * external storage migration
  * archive extraction
  * backup restoration

### Suggested Tasks

* Test vault relocation scenarios
* Support relative vault metadata where possible
* Add “rebind missing vault” workflow
* Validate portable backup archives

### Future Possibility

Portable “cold archive vaults”:

* offline drives
* removable archive media
* historical snapshots

---

# Priority 2 — Metadata & Recall Quality

## Principle

Recall should improve through deterministic context, not opaque ranking.

Metadata, notes, relations, and discovery scopes should help the operator understand why artifacts matter and why they appear together.

---

## 4. Relation Engine Expansion

The deterministic relations system is already one of the strongest features.

### Goals

Improve contextual recall while keeping every signal inspectable and reproducible.

### Suggested Signals

* filename token overlap
* same ingest session
* shared extension family
* shared manifest/project origin
* nearby ingest timestamps
* shared hash family metadata
* same extracted keywords
* same release/version markers

### Important Constraint

All relations should remain:

* explainable
* inspectable
* reproducible

The user should always understand:

> “why are these related?”

---

## 5. Metadata Editing & Notes

The details panel is evolving into the memory layer.

### Suggested Additions

* operator notes
* retention reason
* “why this matters”
* source/provenance fields
* acquisition method
* trust classification
* retention priority
* archive status

### Important Philosophy

The metadata system should preserve:

* operational context
* human intent
* artifact significance

not generic document-management metadata.

---

## 6. Search & Discovery Refinement

Search should reinforce deliberate retention.

### Suggested Improvements

* combined tag/category filters
* “recently ingested”
* “unverified”
* “missing preview”
* “repair needed”
* “duplicate candidates”
* “same source batch”
* “large artifacts”
* saved searches/views

### Possible Addition

Operator “collections”:

* temporary logical groupings
* non-destructive organization
* investigation/research sets

---

# Priority 3 — Preview & Inspection

## Principle

Inspection should make retained artifacts understandable without pretending every format can be rendered the same way.

Previews, extracted text, and generated assets should remain local, deterministic, cached, and rebuildable.

---

## 7. Preview Pipeline Maturity

Preview support is already good because unsupported formats are handled honestly instead of pretending everything is renderable.

### Goals

Improve inspection quality without becoming bloated.

### Suggested Expansions

* text preview for manifests/configs
* syntax-highlighted technical files
* metadata-first preview cards
* PDF first-page thumbnails
* archive content summaries
* installer metadata extraction
* EXIF/media metadata display

### Important Direction

Preview systems should remain:

* local
* deterministic
* cached
* rebuildable

---

## 8. Extraction Pipeline Refinement

Text extraction is becoming foundational.

### Suggested Improvements

* extraction health status
* re-extract action
* extraction logs
* supported-format registry
* partial extraction warnings
* extraction size limits
* encoding detection improvements

### Long-Term Value

This strengthens:

* recall
* search quality
* operational discoverability

without requiring opaque ranking or generated classifications.

---

# Priority 4 — Ecosystem & Operator Tooling

## Principle

Operator workflows should be scriptable, reportable, and portable while preserving the same cautious semantics as the desktop app.

Automation should expose vault state clearly instead of bypassing review.

---

## 9. CLI / Headless Operations

This feels like a natural evolution point.

### Possible Commands

* ingest
* verify
* repair
* rescan
* export
* search
* generate-report
* rebuild-thumbnails

### Benefits

* scripting
* automation
* scheduled verification
* batch ingest
* server-side archive workflows

This aligns extremely well with your overall Aptlantis ecosystem.

---

## 10. Standardized Export Format

A huge long-term value add.

### Proposed Structure

* catalog JSON/JSONL
* extracted text
* thumbnails
* manifests
* integrity reports
* repair logs

### Benefits

* long-term preservation
* interoperability
* cold storage export
* migration safety
* archival packaging

This could become:

> a deterministic vault interchange format

---

# Priority 5 — Documentation & Philosophy

## Principle

FileCabinet's design philosophy should be explicit enough that future work can preserve the product's boundaries.

Documentation should explain not only what the vault does, but why it avoids cloud dependence, automatic inference, and hidden state.

---

## 11. Preservation-Oriented Documentation

This project already has unusually strong philosophy.

That should become a first-class strength.

### Recommended Docs

* The Art of Deliberate Retention
* Vault Lifecycle
* Trust & Verification Model
* Why Determinism Matters
* Local-First Artifact Preservation
* Repair & Recovery Guide
* Designing for Context Preservation

---

## 12. Positioning Clarity

The app becomes stronger the more clearly it avoids becoming:

* cloud storage
* enterprise DMS
* filesystem replacement
* automatic inference workspace
* generalized productivity suite

The clearest positioning appears to be:

> FileCabinet is a deterministic local-first vault for preserving high-signal technical artifacts and the operational context surrounding them.

