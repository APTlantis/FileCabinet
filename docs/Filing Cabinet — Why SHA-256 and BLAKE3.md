# FileCabinet — Why SHA-256 and BLAKE3

FileCabinet records both SHA-256 and BLAKE3 hashes intentionally.

That choice can look redundant at first. For a local-first preservation vault, redundancy is the point. A retained artifact should be identifiable today, verifiable years from now, and practical to check across large batches of files. SHA-256 and BLAKE3 serve different strengths while reinforcing the same trust model.

## SHA-256 Is The Compatibility Anchor

SHA-256 is the long-standing industry standard.

It is widely supported by operating systems, programming languages, package managers, release tooling, forensic tools, backup workflows, and public software distribution systems. When a vendor publishes a checksum, when a release ledger records installer identity, or when an operator compares a retained artifact against an external source, SHA-256 is often the common language.

That matters for FileCabinet because preservation is not only about what the app can verify internally. It is also about whether an artifact can be compared against outside evidence later:

- a vendor checksum
- a release note
- a software bill of materials
- a package registry record
- a backup manifest
- a historical archive

SHA-256 gives FileCabinet an interoperability anchor that should remain understandable for a long time.

## BLAKE3 Is The Operational Workhorse

BLAKE3 is modern, fast, and designed for efficient hashing on current machines.

It is especially useful for workflows FileCabinet naturally grows into:

- large retained artifacts
- batch ingest
- vault verification
- package verification
- restore drills
- scheduled command-line checks
- repeated repair and health scans

BLAKE3 is parallel-friendly, which means it can take better advantage of multi-core systems than older serial hash designs. That matters when an operator is checking a large vault, rebuilding trust after a restore, or scripting verification across cold-storage media.

The goal is not novelty. The goal is practical speed without giving up cryptographic content identity.

## Why Store Both

FileCabinet is not trying to pick a favorite hash. It is building a stronger fingerprint.

SHA-256 provides compatibility, maturity, and long-term external recognition. BLAKE3 provides fast local verification and strong modern performance for large or repeated operations. Storing both makes the catalog more useful in more situations.

It also improves confidence over time. A collision in one strong cryptographic hash is already expected to be extraordinarily unlikely for normal preservation work. Requiring the same retained content to collide under two different hash families raises the bar much further. An accidental or adversarial match would need to fool both recorded identities, not just one.

That does not make FileCabinet magical. It does make the evidence stronger.

## How FileCabinet Uses Them

During ingest and verification, FileCabinet computes both hashes for the retained file and stores them in the catalog.

The hashes support:

- integrity checks
- duplicate detection
- repair findings
- relation hints
- package and export confidence
- future restore verification

SHA-256 remains the best outward-facing comparison value. BLAKE3 remains valuable for fast local verification and future automation. Together they support FileCabinet's larger promise: a vault should be able to explain whether a retained artifact is still the same artifact.

## The Tradeoff

Computing two hashes costs more than computing one.

For FileCabinet, that cost is acceptable. Ingest and verification are trust-building moments, and the extra work produces a catalog that is more portable, more inspectable, and more resilient as tools and expectations change.

The design is deliberately conservative: use the standard everyone recognizes, add the modern hash that makes large local workflows practical, and preserve both results in plain catalog metadata.
