# FileCabinet v1.4.4 — Neon Ink Theme Patch

FileCabinet v1.4.4 is a focused visual polish patch that introduces the Neon Ink theme foundation and refreshes the desktop UI palette.

## Neon Ink Theme

The WPF application now loads a dedicated `Themes/NeonInk.xaml` resource dictionary from `Application.xaml`.

The theme defines explicit background, text, navigation, process, featured, success, caution, critical, archive, and preview-oriented brushes. `MainWindow.xaml` now uses those shared resources instead of keeping the core palette inline in the window.

## Visual Refresh

The UI has been rebalanced around Neon Ink semantics:

- cyan and teal for navigation, selection, and structural cues
- violet and purple for ingest, preview, process, and panel-tab states
- pink and magenta for featured, creative, and metadata emphasis
- green for healthy/indexed/validated states
- yellow for starred and attention states
- red for quarantine, delete, and destructive states
- orange for large/build-style artifacts

Selected rows and navigation fills now use a darker navigation surface so text remains readable while the cyan accent still feels active.

## Preview and Activity Accents

Generic file previews now use category-aware Neon Ink accents. Installers, archives, manifests, presentations, audio/video, PDFs, torrents, and security files each map to a semantic color instead of defaulting to blue.

New activity and stat entries also use the refreshed palette.

## Documentation

The Neon Ink integration overview has been revised to describe the WPF ResourceDictionary approach and the intended migration path for theme support.

## Tests

This release was verified with the FileCabinet unit test suite and a release installer build.

## Installer

The Windows x64 MSI was rebuilt with package version `1.4.4.0`.

Installer:

- `FileCabinet-1.4.4.0-win-x64.msi`
- Size: `117886976` bytes
- SHA-256: `C60E0B594751B5B6C818C5782E8E3928912DA607CF83BDCB03C8AA3F29AF1449`

The release manifest records the final MSI size and SHA-256 checksum.

## Version

Application, CLI, installer default, and project manifest metadata have been updated to `1.4.4`.
