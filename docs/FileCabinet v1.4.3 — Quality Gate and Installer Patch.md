# FileCabinet v1.4.3 — Quality Gate and Installer Patch

FileCabinet v1.4.3 is a release-discipline patch focused on SonarCloud quality gate cleanup, coverage reporting, and refreshed installer metadata.

## Quality Gate Cleanup

This release resolves the new-code quality gate findings from the v1.4.2 patch cycle.

The CLI parser and output helpers now live under the `FileCabinet.Cli` namespace, and parser fallback paths explicitly report unexpected value options instead of silently ignoring them.

Regular expression release marker extraction now uses a bounded timeout, limiting regex execution time during relation scoring.

Recoverable catalog loading, directory expansion, and headless rescan adoption failures now write debug diagnostics instead of using empty catch blocks.

## Coverage Reporting

The GitHub Actions SonarCloud workflow now emits OpenCover coverage through Coverlet and passes the report using the VB.NET-specific scanner property.

This lets SonarCloud read coverage for new VB.NET lines instead of reporting `0.0%` coverage when tests have actually run.

## Test Coverage

The test suite adds focused coverage for:

- CLI switch parsing and defensive parser branches
- CLI text and JSON output helpers
- corrupt catalog recovery
- locked orphan-file rescan behavior
- preview title fallback behavior
- release and analyzer cleanup paths

The release was verified with 79 passing tests.

## Installer

The Windows x64 MSI was rebuilt with package version `1.4.3.0`.

Installer:

- `FileCabinet-1.4.3.0-win-x64.msi`

The release manifest and GitHub release asset record the final MSI size and SHA-256 checksum.

## Version

Application, CLI, installer default, and project manifest metadata have been updated to `1.4.3`.
