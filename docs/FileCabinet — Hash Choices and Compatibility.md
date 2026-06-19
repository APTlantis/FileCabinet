# FileCabinet - Hash Choices and Compatibility

FileCabinet starts new catalogs with SHA-256 only. That keeps first-run behavior familiar and avoids spending time on extra fingerprints until the operator asks for them.

Additional hashes are still useful. They help verify older downloads, release notes, vendor manifests, archives, firmware packages, and files from projects that published a non-SHA-256 digest. Enabling a hash means FileCabinet will compute it during ingest, health repair, and explicit hash checks.

## Recommended Default

### SHA-256

Best use: general integrity verification and broad interoperability.

SHA-256 is part of the SHA-2 family standardized by NIST in FIPS 180-4. It is the safest default for normal FileCabinet use because many vendors, package managers, operating systems, and release workflows publish SHA-256 digests.

Reference: [NIST FIPS 180-4, Secure Hash Standard](https://csrc.nist.gov/pubs/fips/180-4/upd1/final)

## Modern Strong Hashes

### BLAKE3

Best use: fast local verification, especially for large retained files.

BLAKE3 is a modern cryptographic hash designed for high performance and parallelism. It is useful when you want a second strong fingerprint that is fast on large vaults.

Reference: [BLAKE3 specifications](https://github.com/BLAKE3-team/BLAKE3-specs)

### KangarooTwelve

Best use: ARHS-style release verification and Keccak-family evidence.

KangarooTwelve is a Keccak-team extendable-output function. FileCabinet records a 32-byte digest for catalog compatibility. It is useful when matching ARHS release records or other K12-based workflows.

Reference: [Keccak Team KangarooTwelve](https://keccak.team/kangarootwelve.html)

### SHA3-256

Best use: NIST SHA-3 family verification.

SHA3-256 is standardized in NIST FIPS 202. It is a good alternate cryptographic fingerprint when a source publishes SHA-3 values or when you want evidence from a different design family than SHA-2.

Reference: [NIST FIPS 202, SHA-3 Standard](https://csrc.nist.gov/pubs/fips/202/final)

### Skein-512

Best use: matching existing Skein digests.

Skein was a SHA-3 competition finalist. FileCabinet includes it for compatibility with older manifests or projects that published Skein values.

Reference: [Skein hash function site](https://www.schneier.com/academic/skein/)

## Legacy Cryptographic Hashes

### MD5

Best use: legacy software verification only.

MD5 is vulnerable to practical collision attacks and should not be used for new trust decisions. It remains useful when a historical vendor or archived project only published an MD5 checksum and you need to compare against that exact record.

Reference: [RFC 1321, The MD5 Message-Digest Algorithm](https://www.rfc-editor.org/rfc/rfc1321)

### Whirlpool

Best use: matching older published Whirlpool digests.

Whirlpool is included for compatibility. It is not the default because SHA-256 is more widely published and better understood by most current release pipelines.

Reference: [NESSIE Whirlpool portfolio entry](https://www.cosic.esat.kuleuven.be/nessie/portfolio/)

## Compatibility Checksums

These are not cryptographic integrity hashes. Use them to match old tools, archive records, embedded device manifests, or transport checksums.

- `cksum (POSIX)` - for POSIX `cksum` output. Reference: [POSIX cksum](https://pubs.opengroup.org/onlinepubs/9699919799/utilities/cksum.html)
- `CRC-8/SMBus` - for small-device and SMBus-style checksum records.
- `CRC-16/ARC` - for older archive, serial, and device checksum records.
- `CRC-32/IEEE` - for ZIP, Ethernet, and many common checksum records.
- `CRC-64/ECMA` - for some large-file and archival checksum records.
- `Adler-32` - for zlib-style checksum records. Reference: [RFC 1950, ZLIB Compressed Data Format](https://www.rfc-editor.org/rfc/rfc1950)
- `BSD sum16` - for legacy Unix `sum` output.
- `SYSV sum16` - for legacy System V `sum` output.
- `Internet checksum` - for RFC 1071-style checksums. Reference: [RFC 1071](https://www.rfc-editor.org/rfc/rfc1071)
- `sum8`, `sum24`, `sum32` - simple additive checksums for compatibility only.
- `Fletcher-8`, `Fletcher-16`, `Fletcher-32` - older error-detection checksums.
- `xor8` - simple XOR checksum for legacy records.

## Compatibility Non-Cryptographic Hashes

These are fast general-purpose hashes, not security checks. Enable them only when matching existing records.

- `FNV-1 32`
- `FNV-1a 32`
- `FNV-1a 64`
- `Jenkins one-at-a-time`
- `djb2 32`
- `SDBM 32`
- `Murmur3 32`
- `xxHash64`

Reference: [IETF draft for FNV hash](https://datatracker.ietf.org/doc/html/draft-eastlake-fnv-25) and [xxHash project](https://xxhash.com/)
