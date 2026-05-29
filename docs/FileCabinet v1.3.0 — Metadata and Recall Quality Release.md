# FileCabinet v1.3.0 — Metadata and Recall Quality Release

This release strengthens FileCabinet as a deterministic recall system. It adds structured operator metadata, broader explainable relations, and built-in discovery scopes without introducing automatic inference or opaque ranking.

## Structured Operator Metadata

Artifact records now preserve more of the human context around deliberate retention:

- retention reason
- why this matters
- source provenance
- acquisition method
- trust classification
- retention priority
- archive status

These fields are additive catalog properties. Older catalogs load with safe defaults, and free-form notes remain available for longer context.

## Explainable Relations

Relations continue to show exactly why artifacts are related. Existing signals remain, including duplicate hashes, shared tags, category/type family, same folder, date batch, and filename tokens.

New deterministic signals include:

- same ingest session
- shared extension family
- shared provenance token
- shared release marker
- shared hash prefix
- shared extracted-text keyword

Every relation reason is rendered in the Relations tab so the operator can inspect the basis of the match.

## Discovery Scopes

The navigation panel now includes built-in recall views:

- Unverified
- Missing Preview
- Repair Needed
- Duplicate Candidates
- Same Source Batch
- Large Artifacts

These scopes are saved as normal view state and can be combined with search, tag, and category filters.
