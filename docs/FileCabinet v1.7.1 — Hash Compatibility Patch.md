# FileCabinet v1.7.1 — Hash Compatibility Patch

FileCabinet v1.7.1 expands optional integrity evidence with compatibility-oriented hashes and checksums for long-lived archives. This patch keeps the default active hash set unchanged while giving operators more ways to compare retained files across old tools, transfer media, and future recovery contexts.

---

## Highlights

### Archival Checksums

The hash registry now includes POSIX `cksum`, CRC-8/SMBus, CRC-16/ARC, CRC-32/IEEE, CRC-64/ECMA, Adler-32, BSD sum16, SYSV sum16, Internet checksum, sum8, sum24, and sum32.

### Legacy And Fast Hashes

The registry also includes Fletcher-8, Fletcher-16, Fletcher-32, xor8, FNV-1 32, FNV-1a 32, FNV-1a 64, Jenkins one-at-a-time, djb2 32, SDBM 32, Murmur3 32, and xxHash64.

### Extensible Catalog Storage

Existing vaults remain readable. The original named catalog fields for SHA-256, BLAKE3, KangarooTwelve, SHA3-256, MD5, Whirlpool, and Skein are preserved, while newly added algorithms are stored in an extensible `hashes` dictionary keyed by stable algorithm ID.

### Dynamic Hash Settings

The Settings panel now renders hash choices from the registry, so future additions do not need one-off UI properties. At the time of this patch, the default active hash set was `SHA256,BLAKE3,KangarooTwelve`; newer builds start with SHA-256 only and let operators opt into additional hashes.

---

## Design Boundaries

FileCabinet v1.7.1 intentionally does not:

- Enable any newly added checksum or non-cryptographic hash by default.
- Treat CRCs, sums, or fast hashes as cryptographic authenticity checks.
- Generate the full ARHS, BLAKE3, KangarooTwelve, or post-quantum signature bundle for this patch artifact.

---

## Built With

- .NET 10
- VB.NET and WPF
- WiX Toolset 6.0.2
- Blake3 2.2.1
- BouncyCastle.Cryptography 2.3.1
- StreamHash 1.11.3
- System.IO.Hashing 10.0.5
- MSTest 4.0.2

---

## Release Artifact

Expected installer:

- `FileCabinet-1.7.1.0-win-x64.msi`

Package size:

- `127889408` bytes

SHA-256:

- `49308F97047C3778F7B78AAFDEECEC0535918B483AB59340815810424034A2FE`

Signing status:

- Not signed for this patch release. SHA-256 only was recorded.

---

## Verification

- Build result: `installer/build-installer.ps1 -Version 1.7.1.0` completed on 2026-06-11.
- Test result: `dotnet test FileCabinet.Tests\FileCabinet.Tests.vbproj --no-restore` passed 98 tests on 2026-06-11.
- Hash result: `Get-FileHash -Algorithm SHA256 artifacts\installer\FileCabinet-1.7.1.0-win-x64.msi` produced the SHA-256 above.

The patch adds regression vectors for all 24 new algorithms across empty, text, and binary fixtures, plus tests for mapped catalog persistence, ingest, health verification, and dynamic hash settings.
