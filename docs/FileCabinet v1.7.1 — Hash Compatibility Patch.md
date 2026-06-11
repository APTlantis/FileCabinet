# FileCabinet v1.7.1 — Hash Compatibility Patch

FileCabinet expands its optional integrity evidence with two staged increases of compatibility-oriented hashes and checksums for long-lived archives.

## Added Hash Families

### Increase 1: Archival Checksums

- POSIX `cksum`
- CRC-8/SMBus
- CRC-16/ARC
- CRC-32/IEEE
- CRC-64/ECMA
- Adler-32
- BSD sum16
- SYSV sum16
- Internet checksum
- sum8
- sum24
- sum32

### Increase 2: Legacy And Fast Hashes

- Fletcher-8
- Fletcher-16
- Fletcher-32
- xor8
- FNV-1 32
- FNV-1a 32
- FNV-1a 64
- Jenkins one-at-a-time
- djb2 32
- SDBM 32
- Murmur3 32
- xxHash64

## Compatibility Notes

Existing vaults remain readable. The original named catalog fields for SHA-256, BLAKE3, KangarooTwelve, SHA3-256, MD5, Whirlpool, and Skein are preserved, while newly added algorithms are stored in an extensible `hashes` dictionary keyed by stable algorithm ID.

The default active hash set remains `SHA256,BLAKE3,KangarooTwelve`.

CRC, checksum, and non-cryptographic hash options are included for corruption detection, cross-tool comparison, and legacy media compatibility. They are not substitutes for cryptographic authenticity checks.

## Verification

The patch adds regression vectors for all 24 new algorithms across empty, text, and binary fixtures, plus tests for mapped catalog persistence, ingest, health verification, and dynamic hash settings.
