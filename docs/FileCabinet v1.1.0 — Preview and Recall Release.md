# FileCabinet v1.1.0 — Preview and Recall Release

FileCabinet v1.1.0 builds directly on the initial stable release by making retained artifacts easier to recognize, inspect, and connect.

The core vault workflows remain the foundation:

* local-first storage
* deterministic metadata
* integrity verification
* cautious intake
* repairable catalog state

This release focuses on the right side of the application: richer previews, clearer details, and more explainable relationships between retained files.

---

## Highlights

### Preview Pipeline V1

FileCabinet now has a first local preview pipeline.

Image artifacts generate cached thumbnails under the vault's `thumbnails\yyyy\MM\` structure during ingest and rescan adoption. These generated thumbnails are stored as vault-relative catalog metadata and reused by the Preview tab.

Non-renderable artifacts such as installers, archives, torrents, and disk images receive explicit fallback preview status rather than pretending to be unsupported.

Repair checks now include missing generated thumbnails and can regenerate them when the retained source file is present.

### Format-Aware Preview Cards

Unsupported binary and document-like artifacts now get clearer preview cards instead of a generic placeholder.

FileCabinet distinguishes common retained artifact families including:

* ISO and disk images
* installers
* archives
* torrents
* PDFs
* documents
* spreadsheets
* presentations
* security files
* audio and video
* manifests and configuration files

These cards show meaningful titles, badges, colors, and action hints so the vault remains useful even when a file cannot be rendered inline.

### Real Right-Panel Tabs

The right side now has actual tabs:

* Preview
* Details
* Relations

Preview focuses on visual or textual inspection. Details holds editable metadata, file facts, notes, hashes, extraction status, and thumbnail status. Relations now has dedicated space for related-artifact discovery.

### Explainable Relations

Related item discovery is still deterministic and local, but it now explains why items are related.

Relations can be ranked and described by signals such as:

* duplicate SHA-256 hashes
* shared tags
* shared categories
* shared type families
* same original folder
* same date batch
* matching filename tokens

The Relations tab now shows relation scores and human-readable reasons, making nearby artifacts easier to trust and act on.

### Installer and Branding Polish

The Windows installer and application metadata now use FileCabinet version `1.1.0`.

The installer defaults to producing a `v1.1.0` MSI, and the application icon/shortcut branding has been tightened so installed builds better reflect the FileCabinet identity.

---

## What This Release Improves

v1.1.0 makes the vault feel more inspectable.

Where v1.0.0 established durable storage and catalog safety, v1.1.0 improves the experience of selecting an artifact and quickly understanding:

* what it is
* whether it has a preview
* how it relates to nearby files
* what metadata can be edited
* whether generated preview assets are healthy

The result is a stronger artifact review loop without introducing cloud services, automatic inference, or platform-dependent shell thumbnail behavior.

---

## Design Boundaries

FileCabinet v1.1.0 intentionally keeps the preview and recall model deterministic. It does not add:

* PDF page rendering
* image-text extraction for screenshots or scanned documents
* Windows shell thumbnail extraction

The application continues to emphasize deliberate curation, operator-authored context, inspectable metadata, and repairable local vault state.

---

## Built With

* WPF
* VB.NET
* .NET 10
* WiX Toolset
* Local-first storage
* Deterministic metadata and preview workflows

---

## Release Artifact

Expected installer:

* `FileCabinet-1.1.0.0-win-x64.msi`

This installer contains the self-contained Windows x64 desktop build for FileCabinet v1.1.0.
