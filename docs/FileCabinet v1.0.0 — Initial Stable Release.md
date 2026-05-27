# FileCabinet v1.0.0 — Initial Stable Release

FileCabinet v1.0.0 marks the first stable release of a local-first vault built specifically for preserving high-signal technical artifacts and the operational context surrounding them.

This release establishes the core philosophy and foundation of the project:

* deterministic storage
* inspectable metadata
* integrity verification
* cautious ownership
* deliberate archival workflows

FileCabinet is not intended to replace the native filesystem, Windows Explorer, Everything Search, or cloud storage platforms. Instead, it acts as a curated retention layer for the small percentage of files that carry lasting operational value beyond their raw bytes.

These artifacts may include:

* known-good installers and drivers
* server ISOs and disk images
* configuration snapshots
* manifests and datasets
* PGP keys and security artifacts
* patched binaries
* recovery tools
* research materials
* irreplaceable workflow components

---

## Highlights

### Local-First Vault Architecture

Artifacts remain fully local and portable. Vaults can live on separate drives and maintain a self-contained storage structure independent of the application install location.

### Smart Intake System

FileCabinet supports multiple ingestion strategies:

* Move into vault
* Copy into vault
* Verified transfer workflows
* Safe duplicate handling

Artifacts are automatically organized into deterministic year/month storage structures to eliminate long-term folder hierarchy drift.

### Integrity Verification

Every ingested artifact is fingerprinted using:

* BLAKE3
* SHA-256

Hash verification tools allow long-term integrity checking against corruption, accidental modification, or storage degradation.

### Metadata & Context Preservation

FileCabinet preserves:

* original file paths
* categories
* tags
* timestamps
* relationships
* extracted text content
* operational context

The goal is to prevent “context decay” over time.

### Full-Text Extraction

Automatic indexing for text-based technical artifacts including:

* JSON
* TOML
* YAML
* XML
* Markdown
* CSV
* log files

Indexed content becomes globally searchable directly inside the vault.

### Deterministic Related Items

Related artifact discovery uses deterministic local matching rather than AI-driven inference:

* duplicate hashes
* shared categories
* shared tags

### Quarantine & Recovery Workflows

Artifacts can be safely isolated before permanent deletion using the built-in quarantine system.

Recovery tooling includes:

* Repair
* Rescan
* orphan recovery
* catalog reconciliation
* backup catalog export/import

---

## What FileCabinet Is Not

FileCabinet is intentionally **not**:

* a Windows Explorer replacement
* a generic document manager
* a cloud-sync storage platform
* an enterprise DMS
* an AI-first productivity suite

The native filesystem already handles large-scale storage extremely well.

FileCabinet exists to preserve:

* trust
* context
* discoverability
* operational continuity
* artifact integrity

for the files that matter enough to deliberately retain.

---

## Current Scope

v1.0.0 focuses on stabilizing:

* vault architecture
* metadata systems
* storage safety
* catalog integrity
* deterministic workflows
* repair/recovery tooling

AI-assisted categorization, embeddings, OCR expansion, and semantic workflows remain intentionally deferred until the core vault systems are fully mature and reliable.

---

## Built With

* WPF
* VB.NET
* .NET 10
* Local-first storage philosophy
* Deterministic metadata workflows

---

## Philosophy

FileCabinet was built from a simple operational reality:

Modern systems are excellent at storing infinite amounts of data, but terrible at preserving the meaning and trust associated with the files we depend on most.

FileCabinet exists to deliberately curate those artifacts before their context disappears.
