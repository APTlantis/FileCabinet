# FileCabinet — Why Determinism Matters

Determinism matters because preservation is a long-term promise.

When an artifact is retained for future operators, the system should be able to explain what it did, where it stored the file, how it identified the content, and what changed afterward. A vault that cannot explain itself becomes another mystery folder.

## Deterministic Retention

FileCabinet uses deterministic retention to keep decisions inspectable:

- retained paths are organized under the vault
- catalog entries record source and storage context
- hashes identify file content
- generated assets can be rebuilt from retained files
- health reports are derived from catalog and filesystem state
- CLI output is stable enough for scripts

The goal is not to make every workflow automatic. The goal is to make every important state transition explainable.

## Why Not Pure Search

Search helps find things, but search alone does not preserve context.

A file can be searchable and still be untrusted:

- the original source may be unknown
- the file may have changed since it was captured
- the folder may no longer explain why it mattered
- duplicate copies may be indistinguishable
- generated metadata may hide uncertainty

FileCabinet treats search as one part of recall, not as the foundation of trust.

## Why Not Automatic Inference

Automatic inference can be useful, but it should not become the source of truth for retained artifacts.

FileCabinet favors operator-authored context because the operator often knows why a file matters:

- the driver that fixed a device
- the contract version actually signed
- the firmware image used in a deployment
- the PDF that documented a vendor promise
- the build artifact shipped to a customer

Generated metadata can assist recall. It should not quietly rewrite the story.

## Determinism In Practice

The practical test is simple:

An operator should be able to open the vault later and answer:

- What is this?
- Where did it come from?
- Why was it retained?
- Has it changed?
- What generated assets support it?
- What repairs or recoveries have been performed?
- How can it be exported or restored?

When those answers are available without guesswork, the vault is doing its job.

