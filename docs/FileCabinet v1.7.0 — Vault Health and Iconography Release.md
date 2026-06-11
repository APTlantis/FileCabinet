# FileCabinet v1.7.0 - Vault Health and Iconography Release

FileCabinet v1.7.0 is a larger usability and trust release focused on active hash correctness, a dedicated Vault Health workspace, richer artifact iconography, and a sharper Neon Ink desktop theme.

## Highlights

- Fixed active hash handling so ingest, hash check, health scan, repair, rescan, headless verify, and orphan adoption compute only the hashes enabled in Settings.
- Preserved inactive historical hash values in the catalog while no longer requiring or recomputing them when inactive.
- Replaced fixed BLAKE3/SHA-256 detail rows with dynamic integrity rows for every supported hash.
- Added a Vault Health right-panel tab beside Preview, Details, and Relations.
- Moved health summary, findings, repair candidates, repair history, and maintenance actions into the dedicated Health tab.
- Updated duplicate-hash health findings to include the hash family, such as `SHA256: ...`, so multi-hash suites remain clear.
- Added a curated Material Symbols icon map with WPF-safe glyph fallbacks for category-specific preview and artifact-list icons.
- Tuned Neon Ink theme colors with richer semantic dim/glow variants for active, verified, warning, and critical states.
- Fixed custom window maximization so the borderless WPF shell respects the Windows taskbar work area.

## Hash Behavior

The active hash set is now the single source of truth for runtime hashing. If Settings enables only `MD5`, FileCabinet computes and verifies `MD5` only. If Settings enables `SHA3-256` and `KangarooTwelve`, FileCabinet computes and verifies only those hashes.

Inactive catalog values are retained for audit continuity. They are shown as inactive in Details, but they are not treated as missing health findings and are not silently recomputed.

## Vault Health

Vault Health now lives in the right panel as a first-class tab. The tab keeps analysis non-mutating by default, keeps repair candidates visible, and still routes apply actions through the existing explicit confirmation dialog.

## Iconography

FileCabinet standardizes on Material Symbols as the source vocabulary without vendoring the full upstream repository. The curated mapping is stored at `Assets/MaterialSymbolsIconMap.md`; the desktop UI currently renders compatible Segoe MDL2 glyph fallbacks for reliable WPF display.

## Verification

Validated during development:

```powershell
dotnet test FileCabinet.Tests\FileCabinet.Tests.vbproj --no-restore
powershell -ExecutionPolicy Bypass -File installer\build-installer.ps1 -Version 1.7.0.0
D:\200-CTS\230-HASHING\ReleaseHasher\target\debug\release-hasher.exe --json hash artifacts\installer\FileCabinet-1.7.0.0-win-x64.msi
D:\200-CTS\230-HASHING\ReleaseHasher\target\debug\release-hasher.exe --json sign-pq --key D:\200-CTS\230-HASHING\ReleaseHasher\mykey.sec artifacts\installer\FileCabinet-1.7.0.0-win-x64.msi
```

Result: 93 tests passed.

## Release Artifact

Installer:

- File: `artifacts/installer/FileCabinet-1.7.0.0-win-x64.msi`
- Size: `127877120` bytes
- SHA-256: `b4959b990408d902ded07da35e3e4ea95fa211038ce5924fa0fdbb12b38eee82`
- BLAKE3: `264e282d39659bc0f43d9ae93dd36119196db7f566e68cfa67b99dda03f49c90`
- KangarooTwelve: `2685ac125ba94ffb1a7eab9643db00e0b956fbe63f4d7324ddb7fcee768cbdc1`

Post-quantum detached signature:

- File: `artifacts/installer/FileCabinet-1.7.0.0-win-x64.pq.sig`
- Size: `17088` bytes
- SHA-256: `7be43edb78ea10ef88ef81f5cb5d41939ceb98d894bbf49473409fe43c87f6ec`
- BLAKE3: `9ad3fd1d0b7de88bc0a3e294e511e232094ab679a6db7a8aea1892837ea8a632`
- KangarooTwelve: `4033afe0f56bd049aac06f1900b6ae3001d495bb49355d23356913e6d5331e40`
