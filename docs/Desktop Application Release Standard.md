# Desktop Application Release Standard

This document defines the release model used across local-first Windows desktop applications in this organization. It was derived from practices established in FileCabinet and refined through the Aegis project.

The goal is a release process that is honest, inspectable, and reproducible — one that leaves a clear record of what shipped, how it was verified, and what it deliberately does not do.

---

## 1. Core Principles

These principles are non-negotiable. Every release decision should trace back to at least one of them.

**Ship understanding, not just binaries.**
A release is not complete until someone who did not build it can understand what changed, why it changed, and how to verify the artifact they received.

**Every release has a theme.**
Before writing a line of code, name the release. The name is a commitment. If the work does not match the name, either the name was wrong or the scope drifted. Both problems should be caught before the release note is written, not after.

**Design boundaries are part of the release.**
Stating what a release intentionally does not include is as important as stating what it includes. Boundaries prevent scope creep, manage user expectations, and make future releases easier to plan.

**Detect before mutating.**
Any system that changes state — installer, upgrade, repair workflow — should make the change visible before it happens. Irreversible actions require explicit operator approval.

**The artifact hash is the release.**
Publishing an installer without a hash is not a release. It is a file drop. SHA-256 is the minimum. Record it in the release document, not just a manifest.

**Documentation ships with the build.**
Release notes, trust documents, and core docs should be part of the publish output. An operator who has only the installed application should be able to find the documentation that describes it.

---

## 2. Versioning

Use semantic versioning: **MAJOR.MINOR.PATCH**

For Windows installers and package metadata, use the four-part Windows version: **MAJOR.MINOR.PATCH.0**

The fourth component is always `0` unless a hotfix requires an emergency re-release of the same patch version without a version bump.

| Increment | Trigger |
|---|---|
| MAJOR | Breaking vault format, storage structure, or protocol change. A user upgrading needs to be warned explicitly. |
| MINOR | New features, new workflows, new capabilities. Backward-compatible with existing data. |
| PATCH | Bug fixes, UI corrections, dependency refreshes, documentation corrections. No new capability added. |

**Patch policy:** A patch is appropriate when a fix is isolated, the change surface is small, and no new feature is introduced. A patch should be releasable in a single session: identify the problem, fix it, verify it, document it, ship it. If the fix requires a design decision or touches multiple systems, it is a minor release.

**Version source of truth:** The project manifest (`*.manifest.json` or `*.manifest.toml`) is the canonical version record. The installer build script reads the version from the manifest or receives it as a parameter. The application assembly version must match.

---

## 3. Release Document

Every release — including patches — requires a release document.

The release document is the written promise of what the release contains. It is the artifact the operator reads to decide whether to install. It should be written before or simultaneously with the final build, not after.

### Required Structure

```
# [AppName] v[X.Y.Z] — [Release Theme Name]

[One paragraph: what this release is about and why it exists.
The tone is honest about scope. Do not oversell a patch as a feature release.]

---

## Highlights

### [Feature or Change Area Name]
[Plain description. What changed, why it matters, what the operator can now do.]

[Repeat for each meaningful change. A patch may have only one section.]

---

## What This Release Improves

[Optional for patches. For minor releases: a short paragraph connecting the
highlights to the broader direction of the project.]

---

## Design Boundaries

[AppName] v[X.Y.Z] intentionally does not:

* [Thing that was considered but scoped out]
* [Behavior that operators might expect but is deliberately absent]
* [Complexity that will be addressed in a future release]

---

## Built With

* [Framework]
* [Language]
* [Runtime]
* [Installer toolchain]
* [Other notable dependencies]

---

## Release Artifact

Expected installer:

* `[AppName]-[X.Y.Z.0]-win-x64.[ext]`

SHA-256:

* `[UPPERCASE HEX HASH]`

[Optional BLAKE3 hash if applicable.]

[One sentence about signing status.]
```

### Required Fields Checklist

- [ ] Version number matches project manifest and assembly
- [ ] Theme name is present and reflects the actual scope
- [ ] At least one Highlights section with plain-language description
- [ ] Design Boundaries section with at least two entries
- [ ] Built With section lists all notable dependencies
- [ ] Release Artifact section with exact filename and SHA-256
- [ ] Signing status is stated (even if unsigned or self-signed)

### Prohibited Content

- Do not claim production readiness in a release note without a security review on record.
- Do not omit Design Boundaries because "there are no limits." There are always limits.
- Do not write a release note as a commit log. Group by meaning, not by change.

---

## 4. Artifact Naming Convention

Installer and release artifacts follow this naming pattern:

```
[AppName]-[MAJOR.MINOR.PATCH.0]-[platform].[ext]
```

Examples:
```
FileCabinet-1.3.1.0-win-x64.msi
Aegis-0.1.2.0-win-x64.msix
```

Rules:
- **AppName** is a single PascalCase word matching the project identifier in the manifest.
- **Version** is the four-part Windows version including the trailing `.0`.
- **Platform** is `win-x64` for all standard Windows x64 releases. Add `win-arm64` when ARM64 is validated.
- **Extension** reflects the actual installer format (`msi`, `msix`, `exe`).
- File names are case-sensitive for hashing purposes. Use the exact name in the release document.

---

## 5. Artifact Integrity

Every release artifact requires a SHA-256 hash recorded in the release document.

Minimum requirement: **SHA-256**, uppercase hex, no separators.

Preferred for security-sensitive releases: **SHA-256 + BLAKE3**.

The hash must be computed from the final artifact file, not from the publish directory before packaging. Re-running the installer build must reproduce the same binary hash if the inputs have not changed.

### Where to Record Hashes

1. **In the release document** — the primary and most visible location.
2. **In the project manifest** — under `release.installer.sha256`.
3. **In the release checklist** (if maintained separately) — under the per-version verification block.

Never publish only a hash without also naming the exact artifact file the hash covers.

---

## 6. Build Verification Record

Every release must have a verification record: a short, factual statement of what was tested and when.

The verification record belongs in:
- The project manifest (`release.verified`)
- The release checklist (per-version section)

### Minimum Verification Record

```
Build result:    [Project].vcxproj / dotnet build completed [Date]
Test result:     [TestRunner] passed [N] tests on [Date]
Install result:  Installed, launched ([window title]), uninstalled successfully on [Date]
Data safety:     [Data file] was not modified or deleted by install/uninstall on [Date]
Signing:         [Self-signed / code-signed / unsigned] on [Date]
```

### What Must Be Recorded Per Release

| Check | Minimum Evidence |
|---|---|
| Build completed | "Release x64 build completed on [date]" |
| Tests passed | "passed N tests" with test runner name |
| Install works | "installed and launched on [date]" with window title |
| Uninstall is safe | "uninstalled successfully; [data file] not deleted" |
| Upgrade is safe | "existing data preserved after upgrade" (if applicable) |
| Signing status | Explicit statement — never implicit |

### Test Count Is Meaningful

Record the test count in the verification block. If the test count drops between releases, it should be noticed and explained. If tests were removed for legitimate reasons, document why.

---

## 7. Project Manifest

Every project maintains a machine-readable project descriptor file.

Accepted formats: `[ProjectName].manifest.json` or `[ProjectName].manifest.toml`

The manifest is the single source of truth for:
- Current version
- Latest release artifact name and hash
- Last verified date and test result
- Build and deploy automation strings
- Documentation index
- External dependency list

The manifest must be updated as part of the release, not as an afterthought. It should never describe a version that has not been released.

### Minimum Manifest Fields

```json
{
  "project": {
    "id": "projectname",
    "version": "X.Y.Z"
  },
  "release": {
    "version": "X.Y.Z",
    "name": "AppName vX.Y.Z — Theme Name",
    "date": "YYYY-MM-DD",
    "installer": {
      "path": "artifacts/installer/[filename]",
      "runtime": "win-x64",
      "package_version": "X.Y.Z.0",
      "size_bytes": 0,
      "sha256": "HASH"
    },
    "verified": {
      "date": "YYYY-MM-DD",
      "tests": "[runner] passed N tests",
      "installer_build": "[script] completed"
    }
  },
  "documentation": {
    "release_notes": "docs/[release note filename]"
  }
}
```

---

## 8. Documentation Delivery

Release documentation must travel with the application.

**During build:** Copy the `docs/` folder into the publish output directory alongside the application binaries.

**In the installer:** Include the `docs/` folder in the installed package so an operator who has only the installed application can access release notes and core docs without the source repository.

**Minimum docs that ship:**
- The release note for the current version
- The trust or security model document (if the application handles sensitive data)
- Any integrity validation matrix or verification model

Documentation files are text files. They add negligible size and are the most durable part of the release artifact.

---

## 9. Supporting Document Types

A mature project accumulates a small set of supporting documents. These are not roadmap items — they are living references that describe what the project is, how it works, and why it makes the decisions it makes.

| Document Type | Purpose | When to Create |
|---|---|---|
| Release notes (`vX.Y.Z`) | What shipped, what it does, the artifact hash | Every release |
| Release checklist | Pre-release gate with per-version verification blocks | Before the first public release |
| Project manifest | Machine-readable project state and release record | Project start |
| Trust / security model | How the application handles sensitive data and who it trusts | Before any security-relevant release |
| Vault / data format notes | Storage schema, versioning policy, upgrade path | When the data format stabilizes |
| Integrity validation matrix | What "healthy" state looks like; how to detect and repair drift | When a repair or verification workflow exists |
| Threat model | Attack surface, trust boundaries, known limitations | Before any security claim |
| Dependency provenance | Exact versions, sources, build flags for each external dependency | Before any public release |
| Build reproducibility guide | Clean-clone build steps, required tools, exact versions | Before sharing with other developers |

Supporting documents do not need to be comprehensive at creation. They should be **honest about their current scope** and updated as the project evolves. An incomplete trust model that says what it covers is more useful than no trust model.

---

## 10. Release Checklist

The release checklist is a living gate document. It does not change between releases — the same sections apply every time. New per-version verification blocks are appended to it.

### Pre-Release Gates (Apply Every Release)

**Build**
- [ ] Clean clone builds successfully
- [ ] All package dependencies restore without manual intervention
- [ ] Required tool versions are documented
- [ ] Release and Debug configurations are both validated
- [ ] Target platform(s) are explicitly listed (x64 / ARM64 / both)

**Tests**
- [ ] Full test suite passes
- [ ] Test count is recorded
- [ ] No tests were removed without explanation
- [ ] If crypto or vault operations exist: primitive, fixture, and negative path tests pass
- [ ] Manual UI smoke test is performed and recorded

**Data Safety**
- [ ] First-run behavior creates expected data structures
- [ ] Upgrade from previous version preserves existing data
- [ ] Uninstall does not silently delete user data
- [ ] If a vault or data format version check exists: version enforcement is tested

**Security / Trust**
- [ ] README and release note state audit/review status accurately
- [ ] Security-relevant behavior changes are documented
- [ ] Dependency changes (especially cryptographic) are documented
- [ ] Known gaps and limitations are updated in the manifest or threat model

**Artifacts**
- [ ] Installer or package artifact is produced with expected filename
- [ ] SHA-256 hash is recorded in the release document
- [ ] Signing status is documented
- [ ] Release document is in `docs/` and included in the build output

### Per-Version Verification Block

Append one of these blocks to the checklist for each release:

```
## v[X.Y.Z] [Platform] Verification

* Package target: `[path/to/installer]`
* Package size: `[N]` bytes
* SHA-256: `[HASH]`
* Signing: [self-signed CN=... / code-signed by ... / unsigned]
* Build result: Release [platform] build completed on [date]
* Test result: [runner] passed [N] tests on [date]
* Install result: installed, launched with title `[window title]`, uninstalled successfully on [date]
* Data safety: `[data path]` not deleted by uninstall on [date]
* Public release: [planned / not planned for this version]
```

---

## 11. Release Blockers

A release is blocked — regardless of feature completeness — if any of the following are true:

- A test that was passing in the previous release now fails without a recorded explanation
- The installer artifact hash in the release document does not match the artifact file
- The installer deletes or overwrites user data without explicit operator approval
- The release note claims production readiness without a security review on record
- A breaking data format change is not documented and does not include an upgrade path
- An external cryptographic dependency was changed without updating the dependency provenance record
- The manifest version does not match the installer version

---

## 12. Release Cadence Principles

There is no fixed release schedule. Releases are driven by scope completion, not calendars.

**A release is ready when its theme is fully implemented.**
If the release was named "Vault Reliability and Trust Release," it should not ship until the trust and reliability work is complete. Partial themes produce confusing releases.

**Patches may be released at any time.**
If a patch is isolated and verified, waiting for a feature release is not required and not preferred. Ship the patch.

**Minor releases absorb related work.**
If two feature areas are closely enough related that they belong in the same release theme, include them. If they are independent, consider two releases.

**A release that takes more than a few sessions to develop should have at least a draft release note written before coding begins.**
Writing the release note first forces the scope to be explicit before any time is spent building.

**Documentation lag is a release blocker.**
If the release note, manifest, and checklist are not ready, the release is not ready. These are not optional paperwork — they are part of what makes a release a release.

---

## 13. Reference: FileCabinet Lineage

This standard was derived from the FileCabinet release history. The following practices were established in FileCabinet and should be considered validated:

| Practice | First Established | Evidence |
|---|---|---|
| Release theme naming | v1.0.0 | "Initial Stable Release", "Preview and Recall Release", etc. |
| Design Boundaries as a required section | v1.1.0 | Explicit list of intentional exclusions in every release note |
| SHA-256 in release document | v1.1.0 | `FileCabinet-1.1.0.0-win-x64.msi` hash in v1.1.0 note |
| `docs/` folder in publish output | v1.0.0 | Docs folder present in `artifacts/publish/win-x64/docs/` |
| Machine-readable manifest | v1.4.x | `FileCabinet.manifest.json` with full release and verification record |
| Test count in verification record | v1.0.0 | "passed N tests" recorded per version |
| Repair log as trust artifact | v1.2.0 | `catalog/repair-log.jsonl` logged and surfaced in UI |
| Headless CLI verification | v1.2.0+ | `FileCabinet.Cli.exe verify --fail-on medium` |
| Trust and verification model document | v1.2.0 | `FileCabinet — Trust and Verification Model.md` |
| Integrity validation matrix | v1.2.0 | `FileCabinet — Vault Integrity Validation Matrix.md` |

The Aegis project extended this model with:

| Practice | Evidence |
|---|---|
| Per-version verification blocks in release checklist | `Aegis - Release Checklist.md` v0.1.0 through v0.1.2 |
| Explicit release blockers section | Aegis release checklist Blockers section |
| Test path override in services | `SetKeyringPathForTesting`, `SetSettingsPathForTesting` |
| 3:12 test ratio enforcement | `.agents/rules.md` |

---

*This document is a working standard. It should be updated when a new practice is validated in a real release.*
