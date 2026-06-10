# FileCabinet v1.6.0 - Health and Settings Release

FileCabinet v1.6.0 focuses on settings trust, real dynamic hashing, and actionable vault health repair workflows.

## Highlights

- Replaced the KangarooTwelve SHA3 placeholder with real KangarooTwelve hashing through the StreamHash runtime.
- Centralized hash option metadata so Settings, ingest, verification, health checks, and repairs use the same canonical hash IDs.
- Hardened Settings so at least one active hash must remain enabled and legacy hashes are clearly labeled as compatibility options.
- Added typed vault maintenance progress with stage, item, count, total, percent, and detail fields.
- Improved moved-vault recovery: path rebind findings are automatically selected as safe repair candidates when vault-relative paths resolve under the active vault root.
- Updated hash repair so every active hash field is recomputed together, not just SHA256 and BLAKE3.
- Disabled review-only repair candidate selection in the UI so unavailable actions do not look applyable.
- Replaced inert auto-process checkboxes with live summaries for active hashes and ingest behavior.

## Health and Repair

Health analysis remains non-mutating by default. Repairs still require explicit operator action, and the repair log records skipped, failed, and applied actions.

The primary recovery workflow for this release is moving a vault from one drive/root to another, such as `K:` to `P:`:

1. Analyze vault health.
2. Review preselected `RebindPath` candidates.
3. Apply selected repairs.
4. Re-run health analysis.
5. Regenerate missing generated thumbnails when needed.

## Hashing and Release Verification

FileCabinet now aligns runtime hashing with ARHS defaults:

- SHA256
- BLAKE3
- KangarooTwelve

Release artifact hashing and detached signatures are handled by the existing local ARHS and ReleaseHasher toolchain:

- ARHS documentation: `D:\010-CITY-HALL\ARHS`
- ReleaseHasher project: `D:\200-CTS\230-HASHING\ReleaseHasher`
- ReleaseHasher supports ARHS hashing plus detached PGP and SPHINCS+/post-quantum signing.

FileCabinet does not copy signing key material into this repository.

## Release Artifact

Installer:

- File: `artifacts/installer/FileCabinet-1.6.0.0-win-x64.msi`
- Size: `127868928` bytes
- SHA-256: `e2efe9935378a557432f0c2443d476a0b8ba548572db9656ef2d437b63e2d2af`
- BLAKE3: `6ffa5c250f07d5b86d3e6c0d76218f4732f842fba8b757089e402d5095cd3d63`
- KangarooTwelve: `4f934e3bb6a3615b8abce7e0112c90131985496d126409bddda86dc28aaec13b`

Post-quantum detached signature:

- File: `artifacts/installer/FileCabinet-1.6.0.0-win-x64.pq.sig`
- Size: `17088` bytes
- SHA-256: `4e17345a485d32e4cc0c7a560e87f111f31d0539f7f96eeb9c17f1a7ec875dae`
- BLAKE3: `610c5e1f97fe49af51eb0a307fe319ace4ff970335b54421d471d37a0082016d`
- KangarooTwelve: `8c090a9892a5abb22b13b74d87cbd2c4176324a709564336083b18efbcfbd652`

PGP detached signing was attempted with ReleaseHasher and the available local PGP key. It is blocked until the ReleaseHasher PGP path accepts the key; the command returned `No signing key found in certificate`.

## Verification

Validated on 2026-06-10:

```powershell
dotnet test FileCabinet.Tests\FileCabinet.Tests.vbproj --no-restore
powershell -ExecutionPolicy Bypass -File installer\build-installer.ps1 -Version 1.6.0.0
D:\200-CTS\230-HASHING\ReleaseHasher\target\debug\release-hasher.exe --json hash artifacts\installer\FileCabinet-1.6.0.0-win-x64.msi
D:\200-CTS\230-HASHING\ReleaseHasher\target\debug\release-hasher.exe --json sign-pq --key D:\200-CTS\230-HASHING\ReleaseHasher\mykey.sec artifacts\installer\FileCabinet-1.6.0.0-win-x64.msi
```

Result: 87 tests passed.
