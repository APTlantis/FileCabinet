# Filing Cabinet — Hashing and Compatibility

Filing Cabinet uses content hashes to identify retained artifacts, verify integrity, compare against external evidence, and support long-term recovery.

The hashing model is intentionally configurable.

New catalogs begin with **SHA-256 only**. This keeps first-run behavior familiar, interoperable, and reasonably fast while allowing operators to enable additional hashes when their preservation or compatibility needs justify them.

At least one hash must always remain active.

The active hash set is the source of truth for runtime hashing. If an algorithm is later disabled, historical values already stored on artifacts remain in the catalog for audit continuity. Disabled hashes are not treated as missing findings and are not silently recomputed.

## Why SHA-256 Is the Default

SHA-256 is Filing Cabinet's primary compatibility anchor.

It is part of the SHA-2 family standardized by NIST and is widely supported across:

- operating systems
- package managers
- software release systems
- forensic tools
- backup workflows
- programming languages
- public software archives
- vendor checksum pages

That broad interoperability matters because Filing Cabinet does not verify artifacts only against its own catalog.

A retained artifact may later need to be compared against:

- a vendor checksum
- a release note
- a software bill of materials
- a package registry
- a backup manifest
- an archived download page
- another release-integrity system

SHA-256 is often the common language between those systems.

For that reason, SHA-256 is the safest default for general Filing Cabinet use.

Reference: [NIST FIPS 180-4, Secure Hash Standard](https://csrc.nist.gov/pubs/fips/180-4/upd1/final)

## Why Additional Hashes Exist

A single widely supported hash is enough for many vaults.

Additional hashes become useful when an operator needs:

- compatibility with an existing manifest
- comparison against historical vendor checksums
- faster repeated verification of large files
- evidence from a different cryptographic design family
- interoperability with an existing archive or release-integrity workflow

Filing Cabinet therefore separates the question:

> Which hash should every new vault understand?

from:

> Which additional fingerprints are useful for this operator or artifact?

SHA-256 answers the first question.

The optional hash set answers the second.

## BLAKE3

**Best use:** fast local verification, especially for large retained files and repeated vault checks.

BLAKE3 is a modern cryptographic hash designed for high performance and parallel execution.

Its value to Filing Cabinet is operational rather than novelty-driven.

It is particularly useful for:

- large retained artifacts
- batch ingest
- vault verification
- restore drills
- package verification
- repair workflows
- scheduled CLI checks
- repeated integrity scans

Where SHA-256 provides broad external recognition, BLAKE3 can serve as a high-performance local verification fingerprint.

An operator who wants both interoperability and fast repeated verification may therefore enable both SHA-256 and BLAKE3.

The two serve different purposes:

- **SHA-256:** interoperability anchor
- **BLAKE3:** modern local verification workhorse

Reference: [BLAKE3 specifications](https://github.com/BLAKE3-team/BLAKE3-specs)

## KangarooTwelve

**Best use:** ARHS-style release verification and Keccak-family evidence.

KangarooTwelve is a Keccak-team extendable-output function.

Filing Cabinet records a 32-byte digest for catalog compatibility.

It is useful when retained artifacts need to be compared against release records or other workflows that publish KangarooTwelve values.

Reference: [Keccak Team KangarooTwelve](https://keccak.team/kangarootwelve.html)

## SHA3-256

**Best use:** NIST SHA-3 family verification.

SHA3-256 provides a standardized cryptographic fingerprint from a design family distinct from SHA-2.

It is useful when:

- a source publishes SHA-3 values
- an operator wants an additional cryptographic family represented
- retained evidence already includes SHA3-256

Reference: [NIST FIPS 202, SHA-3 Standard](https://csrc.nist.gov/pubs/fips/202/final)

## Skein-512

**Best use:** matching existing Skein digests.

Skein was a SHA-3 competition finalist.

Filing Cabinet includes it primarily for compatibility with older manifests or projects that published Skein values.

It is not part of the default hashing configuration.

Reference: [Skein hash function](https://www.schneier.com/academic/skein/)

## Legacy Cryptographic Hashes

Some retained artifacts predate modern hashing practices.

Filing Cabinet may therefore support older cryptographic hashes when comparison against historical records matters.

Their presence does not mean they should be used for new trust decisions.

### MD5

**Best use:** legacy software verification only.

MD5 is vulnerable to practical collision attacks and should not be used as the sole basis for a new integrity decision.

It remains useful when a historical vendor, archive, firmware package, or old release record publishes only an MD5 checksum and the operator needs to compare against that exact value.

Reference: [RFC 1321, The MD5 Message-Digest Algorithm](https://www.rfc-editor.org/rfc/rfc1321)

### Whirlpool

**Best use:** matching older published Whirlpool digests.

Whirlpool remains available for compatibility with existing evidence.

It is not a default because SHA-256 is more widely published and better supported by contemporary release tooling.

Reference: [NESSIE Whirlpool portfolio entry](https://www.cosic.esat.kuleuven.be/nessie/portfolio/)

## Compatibility Checksums

Filing Cabinet also supports checksums that are not cryptographic integrity hashes.

These should not be treated as equivalent to SHA-256, BLAKE3, SHA3-256, or other cryptographic fingerprints.

Their role is compatibility.

They can be useful when comparing retained artifacts against:

- old Unix tools
- archive formats
- transport checksums
- embedded-device manifests
- firmware records
- historical software releases

Supported compatibility checksums include:

- `cksum (POSIX)`
- `CRC-8/SMBus`
- `CRC-16/ARC`
- `CRC-32/IEEE`
- `CRC-64/ECMA`
- `Adler-32`
- `BSD sum16`
- `SYSV sum16`
- `Internet checksum`
- `sum8`
- `sum24`
- `sum32`
- `Fletcher-8`
- `Fletcher-16`
- `Fletcher-32`
- `xor8`

Useful references include:

- [POSIX cksum](https://pubs.opengroup.org/onlinepubs/9699919799/utilities/cksum.html)
- [RFC 1950, ZLIB Compressed Data Format](https://www.rfc-editor.org/rfc/rfc1950)
- [RFC 1071, Computing the Internet Checksum](https://www.rfc-editor.org/rfc/rfc1071)

## Compatibility Non-Cryptographic Hashes

Filing Cabinet also supports several fast general-purpose hashes for matching existing records.

These are not security hashes and should not be used as primary evidence that retained content has not been maliciously altered.

Supported algorithms include:

- `FNV-1 32`
- `FNV-1a 32`
- `FNV-1a 64`
- `Jenkins one-at-a-time`
- `djb2 32`
- `SDBM 32`
- `Murmur3 32`
- `xxHash64`

They should be enabled only when an existing system, manifest, or historical record makes them useful.

References:

- [IETF FNV draft](https://datatracker.ietf.org/doc/html/draft-eastlake-fnv-25)
- [xxHash project](https://xxhash.com/)

## Historical Hash Preservation

Hash configuration can change over the lifetime of a vault.

That should not erase history.

If an artifact was previously hashed with an algorithm that is later disabled:

- the stored value remains in the catalog
- the value remains available as historical evidence
- Filing Cabinet does not treat its absence from the active set as an error
- Filing Cabinet does not silently recompute the disabled algorithm
- future ingest and routine verification follow the currently active hash set

This distinction keeps configuration changes from rewriting the historical record.

## Hashing During Vault Operations

The active hash set governs operations that explicitly require content fingerprints, including:

- ingest
- health repair where hashes are missing
- explicit hash checks
- integrity verification where requested

Health analysis remains conservative about expensive reads.

Filing Cabinet does not need to read every retained file end-to-end simply to produce a metadata-first health report.

Explicit verification remains available when stronger evidence is required.

## Choosing a Hash Configuration

For most operators:

**SHA-256 alone is sufficient.**

It provides strong cryptographic content identity and excellent compatibility with external tools and published checksums.

A useful expanded configuration may be:

```text
SHA-256
BLAKE3
```

This combines broad interoperability with fast modern local verification.

More specialized configurations may add:

```text
KangarooTwelve
SHA3-256
```

when those algorithms are already used by related release or archival workflows.

Legacy hashes, checksums, and non-cryptographic hashes should generally be enabled only when there is an existing value that needs to be matched.

## Design Principle

Filing Cabinet does not collect hashes simply because more hashes look more secure.

Every enabled algorithm should have a purpose.

That purpose may be:

- interoperability
- performance
- historical comparison
- release-integrity compatibility
- independent evidence from another cryptographic family

The default remains deliberately conservative:

> Start with the broadly understood standard, preserve the ability to add stronger or more specialized evidence when it is useful, and never erase historical fingerprints merely because the active configuration changes.