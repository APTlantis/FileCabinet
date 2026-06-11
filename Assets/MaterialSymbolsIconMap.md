# FileCabinet Material Symbols Icon Map

FileCabinet v1.7.0 uses Material Symbols as the source vocabulary for file and category iconography, with WPF-safe Segoe MDL2 glyph fallbacks in the current desktop UI.

Source:

- Google Material Symbols / Material Icons: https://github.com/google/material-design-icons
- License: Apache 2.0

| FileCabinet category | Material Symbol source name | WPF fallback role |
| --- | --- | --- |
| Images | `image` | image preview/file |
| Documents | `description` | document |
| PDF documents | `picture_as_pdf` | document with critical accent |
| Spreadsheets | `table` | tabular data |
| Presentations | `present_to_all` | slide deck |
| Manifests / Config | `settings` | configuration |
| Audio | `audio_file` | audio |
| Video | `video_file` | video |
| Archives | `folder_zip` | archive |
| Software / Installers | `deployed_code` | installer/package |
| ISOs / Disk Images | `album` | disk image |
| Keys / Security | `key` | security material |
| Torrents | `hub` | distributed source |
| Quarantine | `dangerous` | quarantined item |
| Unknown | `draft` | generic file |

The full Material Symbols repository is intentionally not vendored into FileCabinet. This curated map keeps the app source small while preserving the selected icon vocabulary and license trail.
