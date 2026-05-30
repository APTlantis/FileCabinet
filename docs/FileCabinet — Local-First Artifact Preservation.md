# FileCabinet — Local-First Artifact Preservation

FileCabinet is local-first by design.

The vault is meant to remain useful without a subscription, remote service, account login, or external index. That does not prevent operators from backing up the vault with their own tools. It means FileCabinet's primary storage and catalog model should be understandable on the local machine.

## What Local-First Means

For FileCabinet, local-first means:

- retained files are stored in a user-selected local vault
- catalog data is local JSON
- generated assets are local files
- health analysis runs locally
- repair and rescan decisions are operator-approved
- CLI operations work without the WPF desktop UI

The vault can be copied, backed up, inspected, packaged, and restored with ordinary filesystem tools.

## What Local-First Does Not Mean

Local-first does not mean isolation from good backup practice.

Operators should still use their preferred backup strategy for vault folders, installer artifacts, and exported packages. FileCabinet focuses on retention structure and verification; it is not a replacement for redundant storage.

Local-first also does not mean every file belongs in the vault. FileCabinet is for high-signal retained artifacts, not every transient working file.

## Operational Benefits

Local-first preservation gives FileCabinet several important properties:

- **Portability:** a vault can move with its files and catalog.
- **Auditability:** state can be inspected without a vendor service.
- **Recoverability:** retained files remain ordinary files on disk.
- **Scriptability:** the CLI can verify, report, package, and search from scheduled jobs.
- **Cautious ownership:** operators decide when the vault owns a file.

## Backup Guidance

A strong FileCabinet backup should include:

- the vault root
- the AppData catalog if the catalog lives outside the vault
- exported catalog snapshots
- deterministic vault packages for cold storage
- installer artifacts needed to reinstall the same release

The CLI package command is intended for this boundary:

```powershell
FileCabinet.Cli.exe package --output K:\FileCabinet\exports\FileCabinetPackage --zip
```

