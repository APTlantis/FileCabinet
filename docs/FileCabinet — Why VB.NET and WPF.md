# FileCabinet — Why VB.NET and WPF

FileCabinet is written in VB.NET and WPF intentionally.

That choice can look conservative from the outside. For this project, it is a strength. FileCabinet is a local-first Windows vault whose most important qualities are reliability, inspectability, filesystem integration, long-term maintainability, and careful operator control. VB.NET and WPF fit that shape well.

## A Native Windows Tool

FileCabinet is not trying to be a cross-platform web app.

It works closely with Windows concepts:

- local filesystem vaults
- AppData catalog storage
- Windows Explorer context-menu intake
- desktop drag and drop
- native file dialogs
- local thumbnails and previews
- MSI installation

WPF gives FileCabinet a mature desktop UI model for those workflows without forcing a browser runtime into the middle of the app.

## Intentional Code Over Framework Churn

The project favors explicit, readable code.

VB.NET supports that style well. Its syntax is direct, its event and property model is approachable, and its .NET runtime support is modern. The language lets FileCabinet express operational behavior in a way that is easy to revisit later:

- load a catalog
- ingest a file
- compute hashes
- generate a thumbnail
- build a health report
- apply a repair candidate
- save state cautiously

For preservation software, boring clarity is an asset.

## WPF Still Solves This Problem Well

WPF remains a strong fit for a dense Windows desktop tool.

FileCabinet needs panes, lists, commands, bindings, virtualization, previews, status fields, and operator workflows. WPF provides these directly:

- data binding for catalog-backed UI state
- commands for explicit operator actions
- templates for artifact rows and preview states
- virtualization for larger vault lists
- async-friendly UI updates
- mature styling without requiring a web stack

The result is a desktop app that can stay close to the operating system and close to the data it manages.

## Robust Does Not Mean Fashionable

FileCabinet's technical priorities are not novelty-driven.

The app should be:

- deterministic
- repairable
- local-first
- easy to inspect
- easy to build
- easy to reason about
- cautious with user data

VB.NET and WPF support those goals. They are stable, well-understood, and deeply integrated with the Windows desktop environment FileCabinet targets.

## The Tradeoff

This choice does have a boundary: FileCabinet is a Windows desktop application first.

That is acceptable because the vault model is local, filesystem-oriented, and operator-driven. The new CLI provides a headless automation surface for scripts and scheduled jobs, while the WPF app remains the primary human interface.

The architecture is not nostalgic. It is deliberately matched to the job.

