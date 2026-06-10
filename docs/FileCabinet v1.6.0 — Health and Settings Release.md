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

## Verification

Validated on 2026-06-10:

```powershell
dotnet test FileCabinet.Tests\FileCabinet.Tests.vbproj --no-restore
```

Result: 87 tests passed.
