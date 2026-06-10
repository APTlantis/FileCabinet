# FileCabinet v1.5.0 - Dynamic Hashing Release

FileCabinet v1.5.0 fundamentally overhauls the way cryptographic hashing is handled.

## Features

### Expanded Cryptography with BouncyCastle
FileCabinet now ships with `BouncyCastle.Cryptography`, enabling support for a wide array of hash algorithms. We have replaced the static "Hash Suites" with a dynamic, user-selectable checkbox system in the new Settings Panel.

### Supported Hashes
Users can now individually toggle the calculation of any combination of the following hashes:
- **Baseline**: SHA256
- **Performance**: BLAKE3
- **Modern Standards**: SHA3-256, KangarooTwelve
- **Legacy & Alternate**: MD5, Whirlpool, Skein

### Improved UI and UX
- **Settings Panel**: A new overarching Settings overlay allows the adjustment of hashing preferences, display density, and ingest modes.
- **Improved Contrast**: UI components within the Settings panel have been tweaked to meet visual accessibility standards.

## Security & Verification

This release fully adopts the **APTlantis Release Hashing Standard (ARHS)**. All artifacts are verifiable using SHA256, BLAKE3, and KangarooTwelve, and are signed with a Post-Quantum signature (`SPHINCS+`).

### Signatures
* `FileCabinet-1.5.0-win-x64.msi.sig` (Detached SPHINCS+ signature)

### Cryptographic Hashes
*(Generated via ReleaseHasher)*

```json
```
