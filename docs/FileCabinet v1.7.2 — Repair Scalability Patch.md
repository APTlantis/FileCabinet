# FileCabinet v1.7.2 - Repair Scalability Patch

FileCabinet v1.7.2 is a maintenance patch focused on making repair and verification usable for vaults that contain very large retained files.

## Repair And Health Analysis

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

## Verification

- `dotnet test FileCabinet.Tests\FileCabinet.Tests.vbproj`
- `installer\build-installer.ps1 -Version 1.7.2.0`
- `Get-FileHash -Algorithm SHA256 artifacts\installer\FileCabinet-1.7.2.0-win-x64.msi`

## Artifact

- File: `artifacts/installer/FileCabinet-1.7.2.0-win-x64.msi`
- Runtime: `win-x64`
- Size: `127,893,504` bytes
- SHA-256: `9C14866B2CAB42C1B92DAEC08C12BF3C6E7F9E7617A4DE41C1D6AFAF66C7F13D`
