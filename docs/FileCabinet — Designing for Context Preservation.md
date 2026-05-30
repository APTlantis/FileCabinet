# FileCabinet — Designing for Context Preservation

FileCabinet exists because important files often outlive the context that made them important.

A retained artifact is not only bytes. It is also source, purpose, risk, verification state, and recovery history. FileCabinet's design should preserve that surrounding context as carefully as it preserves the file.

## Context Worth Preserving

Useful retained context includes:

- where the file came from
- when it was captured
- whether it was copied or moved
- what source batch it belonged to
- what project, vendor, device, customer, or incident it relates to
- why it was retained
- what hashes identify its content
- what repair actions have happened since capture

This context helps future operators decide whether the artifact is still trustworthy.

## Operator Authorship

FileCabinet should make it easy for operators to add meaningful context:

- notes
- tags
- categories
- custom metadata
- starred state
- relationship review

Generated metadata can assist, but the operator's explanation is often the most valuable part of the record.

## UI Design Implications

The desktop UI should stay focused on repeatable vault work:

- clear artifact lists
- stable preview panes
- searchable metadata
- visible integrity state
- explicit repair actions
- concise status text during long operations

The app should avoid burying operational tools behind decorative or marketing-style screens. It is a working vault, not a showcase page.

## CLI Design Implications

The CLI should be stable, predictable, and script-friendly:

- real stdout and stderr
- meaningful exit codes
- deterministic text and JSON output
- no hidden UI dependency
- no mutation unless the command clearly asks for it

This allows scheduled verification, cold-storage packaging, and scripted searches without changing the desktop workflow.

## Boundary Clarity

FileCabinet becomes stronger when it remains clear about what it is not:

- not cloud storage
- not an enterprise document management system
- not a filesystem replacement
- not an automatic inference workspace
- not a generalized productivity suite

The positioning is intentionally narrow:

> FileCabinet is a deterministic local-first vault for preserving high-signal technical artifacts and the operational context surrounding them.

