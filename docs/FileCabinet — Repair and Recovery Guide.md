# FileCabinet — Repair and Recovery Guide

FileCabinet repair is designed around review before mutation.

Health analysis tells the operator what appears wrong. Repair and recovery tools then provide explicit paths to make the vault more trustworthy without hiding risk.

## Analyze First

Start with Analyze or the CLI verify command:

```powershell
FileCabinet.Cli.exe verify --fail-on medium
```

Review findings before applying changes. Pay special attention to high-risk findings such as missing retained files, hash mismatches, files outside the vault, and unexpected duplicates.

## Common Findings

### Missing retained file

The catalog points to a retained file that is not present.

Recommended response:

- check whether the vault path is mounted
- restore the file from backup if available
- avoid deleting the catalog entry until the loss is understood

### Hash mismatch

The retained file exists, but its current hash does not match the catalog.

Recommended response:

- treat the file as changed or suspect
- compare against backup or source material
- do not auto-accept the new hash unless the change is intentional

### Missing thumbnail

A generated preview asset is missing.

Recommended response:

- rebuild thumbnails from the retained file
- this is usually lower risk because thumbnails are derived state

### Missing or stale extracted text

The retained file is present, but extracted text is missing or out of date.

Recommended response:

- re-extract text from the retained file
- verify search results after repair

### Orphan retained file

A file exists under the vault's retained items folder but has no catalog entry.

Recommended response:

- preview rescan results
- adopt the file only when it appears to be a legitimate retained artifact

## WPF Recovery Tools

The desktop app provides operator-facing repair and recovery tools:

- Analyze
- Rescan
- Apply Selected Repair Candidates
- Hash Check
- Restore Copy
- Quarantine
- Delete Forever

Long-running vault maintenance runs asynchronously so the UI remains responsive during hashing, orphan scans, repair preparation, and generated asset work.

## CLI Recovery Tools

The CLI provides scriptable operations:

```powershell
FileCabinet.Cli.exe repair-preview --json
FileCabinet.Cli.exe repair --apply --yes
FileCabinet.Cli.exe rescan --apply --yes
FileCabinet.Cli.exe rebuild-thumbnails --apply --yes
```

Mutating commands require both `--apply` and `--yes`.

## Recovery Principle

When there is doubt, preserve evidence first.

Use reports, exports, packages, and quarantine before destructive cleanup. The vault should help an operator understand what happened, not rush to make the warning disappear.

