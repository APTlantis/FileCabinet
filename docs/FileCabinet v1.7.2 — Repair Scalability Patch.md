# FileCabinet v1.7.2 - Repair Scalability and Analyze/Fix Patch

FileCabinet v1.7.2 is a maintenance patch focused on making repair and verification usable for vaults that contain very large retained files, while making vault analysis and repair decisions clearer after a vault move.

## Repair And Health Analysis

- The Health tab is now a comprehensive Analyze & Fix workspace with summary counts, vault/catalog paths, status, findings, repair candidates, filters, selection controls, and repair history in one place.
- Analyze now separates total findings, repairable candidates, review-only findings, and selected repairs so the operator can tell what is broken and what Apply will actually touch.
- Repair candidates can be filtered by finding type and by repair state: all, repairable, review-only, selected, or unselected.
- Safe repair selection now defaults to catalog-only path rebinds and generated thumbnail recovery, while hash recomputation and text extraction remain explicit operator choices.
- Bulk path rebinds are supported for vault moves: when old absolute paths are missing but vault-relative files resolve under the active vault root, selected `RebindPath` candidates can be applied in one batch.
- Analyze, Apply Selected, and Rescan now return focus to the Health tab instead of opening Settings.
- Health analysis now defers automatic hash mismatch verification for retained files larger than 16 GB instead of reading those files end to end during every Analyze, Verify, or Repair Preview run.
- Deferred large-file checks are reported as `Hash verification deferred` findings so the operator can still see which artifacts were intentionally skipped.
- Missing-hash findings no longer trigger a full-file hash read during analysis when there are no existing active catalog hashes to compare.
- Manual Hash Check and selected hash repair still perform explicit retained-file hashing when the operator chooses to run them.

## Hashing Performance

- Active cryptographic hashes now share a single sequential read pass when possible.
- The default active set, SHA-256, BLAKE3, and KangarooTwelve, no longer requires three separate full-file reads during ingest or explicit repair.
- Murmur3-32 and xxHash64 compatibility hashes now stream file contents instead of loading the entire retained file into memory.
- Hash reads now use a larger sequential buffer for better behavior on large archives, disk images, and installer payloads.

## Release Consistency

- Application and CLI versions were updated to 1.7.2.
- CLI version output now comes from the assembly version instead of a stale hard-coded string.
- The installer build script now defaults to 1.7.2.0.
- README and manifest release metadata were updated for the v1.7.2 patch.
- Release verification now covers the Analyze & Fix panel regression tests, including safe default repair selection and bulk moved-vault path rebind.

## Verification

- `dotnet test FileCabinet.Tests\FileCabinet.Tests.vbproj` - 102 passed
- `installer\build-installer.ps1 -Version 1.7.2.0`
- `Get-FileHash -Algorithm SHA256 artifacts\installer\FileCabinet-1.7.2.0-win-x64.msi`

## Artifact

- File: `artifacts/installer/FileCabinet-1.7.2.0-win-x64.msi`
- Runtime: `win-x64`
- Size: `127,905,792` bytes
- SHA-256: `04572DF20C96A1E618338DD614FC3E627FA59D6386A6FD2867EE692A7EEF888F`
