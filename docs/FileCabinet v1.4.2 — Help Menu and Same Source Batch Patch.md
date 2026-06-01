# FileCabinet v1.4.2 — Help Menu and Same Source Batch Patch

FileCabinet v1.4.2 is a focused usability and responsiveness patch.

This release exposes the growing preservation documentation directly from the app, fixes the Same Source Batch discovery view lockup, and packages the docs alongside the installed application.

## Help Menu

The custom title bar now includes a compact Help surface.

The Help menu opens key Markdown documents with the system default application:

- User Guide
- The Deliberate Retention Tradeoff
- Trust and Verification Model
- Why SHA-256 and BLAKE3
- Repair and Recovery Guide
- Deterministic Vault Roadmap

It also includes:

- Open Docs Folder
- About FileCabinet

The existing Settings buttons remain in place, and the title bar now includes a Settings entry that opens the same settings summary.

## Documentation Packaging

The WPF app now copies `README.md` and `docs\*.md` into build and publish output.

This makes the Help menu useful from installed builds instead of only from the development checkout.

## Same Source Batch Responsiveness

The Same Source Batch discovery scope no longer performs repeated catalog-wide pairwise scans inside the WPF table filter.

The UI now computes same-source-batch membership once for the active selection and checks precomputed artifact keys while filtering.

This preserves the existing behavior:

- with a selected artifact, show artifacts from the same original source directory and ingest session
- without a selected artifact, show artifacts that belong to any same-source batch group

Selection changes while the Same Source Batch scope is active are guarded to avoid recursive filter refresh churn.

## Tests

This release adds coverage for:

- selected-artifact Same Source Batch membership
- unselected Same Source Batch group membership
- large catalog Same Source Batch scope-key generation
- Help documentation path resolution
- missing Help documentation fallback behavior

## Version

Application, CLI, installer default, and project manifest metadata have been updated to `1.4.2`.

Size: 117878784 bytes
SHA-256: 7183B3F8887905B5D344139278FFE80255A4B442622E03F454E88EF45CE97C74