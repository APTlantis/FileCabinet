# Filecabinet — Personal Artifact Vault

Filecabinet is a local-first desktop application designed to act as a permanent retention space for important digital artifacts. Unlike a traditional file explorer or document manager, Filecabinet is intended to serve as a dedicated “vault” for files that should remain organized, searchable, and recoverable long-term regardless of file type.

The system is built around the idea that modern technical workflows produce a wide variety of outputs that do not fit neatly into conventional directory structures. Instead of forcing users to manually maintain complex folder hierarchies, Filecabinet combines structured storage, metadata indexing, and optional local AI assistance to create a centralized artifact registry.

## Core Philosophy

Filecabinet is not intended to replace normal filesystem organization. Most files remain where they naturally belong on the machine. The vault exists specifically for the smaller percentage of important files that benefit from permanent retention, contextual organization, and intelligent recall.

Examples include:

* ISO images
* installers
* PGP/SSH keys
* signed manifests
* TOML/JSON/YAML configurations
* archive snapshots
* torrent files
* exported datasets
* screenshots
* PDFs
* recovery documents
* generated assets
* research artifacts
* structured metadata

The goal is to provide:

> “A designated permanent retention zone for important digital artifacts.”

---

# Storage Architecture

The application itself remains lightweight and installed normally on the system drive, while vault contents are stored on a user-selected storage volume with large available capacity.

Example:

```text
C:\Program Files\Filecabinet\
```

Vault contents:

```text
A:\Filecabinet\
```

This allows the vault to grow independently from the operating system drive and supports very large retained object collections over time.

---

# Vault Structure

Each vault is self-contained and portable.

Example layout:

```text
A:\Filecabinet\
  catalog\
  items\
  manifests\
  inbox\
  quarantine\
  exports\
  thumbnails\
  extracted-text\
```

The vault itself acts as the primary artifact container while AppData only stores lightweight application settings.

---

# File Intake System

Filecabinet includes a dedicated drag-and-drop intake zone used for artifact ingestion.

When files are added, the application may:

* copy or move the file into the vault
* generate hashes
* identify file types
* extract metadata
* generate thumbnails/previews
* perform text extraction when possible
* assign suggested categories/tags
* create searchable indexes
* generate semantic embeddings
* associate related artifacts

Large objects such as ISO files or installers are preserved as first-class vault objects rather than treated as unsupported edge cases.

The first local preview pipeline generates cached thumbnails for image artifacts and records fallback-card status for retained files that are not directly rendered. PDF rendering, OCR, and shell-handler thumbnails remain deferred.

---

# Intelligent Recall

Filecabinet is designed to support contextual retrieval rather than simple filename search.

Instead of only searching paths or names, the system can answer queries such as:

* “Show files related to archive uploads.”
* “Find the ISO I used for WSL testing.”
* “What files are connected to AAMHS manifests?”
* “Show related signing keys and torrent files.”

This is accomplished through metadata indexing, embeddings, tagging, and optional local AI integration.

---

# Local AI Integration

The application may optionally bundle a small local embedding model or lightweight language model.

The AI component acts as an assistant layer responsible for:

* summarization
* categorization
* semantic search
* related-item discovery
* contextual recall
* metadata generation
* artifact explanation

The AI system is intended to augment the vault rather than replace direct browsing.

---

# Design Goals

* Local-first
* Offline-capable
* Portable vault structure
* Large object friendly
* Minimal UI complexity
* Long-term retention focused
* AI-assisted organization
* Metadata-driven recall
* Self-contained vault portability

---

# Intended UI Style

The interface is intentionally simple and utility-oriented:

* left navigation panels
* category lists
* artifact tables
* preview panes
* search/AI query bar
* drag-and-drop intake zone
* metadata panels
* notes and relationship views

The application prioritizes operational clarity over flashy UI design.
