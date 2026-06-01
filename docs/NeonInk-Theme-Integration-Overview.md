# Neon Ink Theme Integration Overview

**Target:** FileCabinet / Aptlantis Studio UI  
**Theme:** Aptlantis Studio - Neon Ink  
**Version:** 0.1.0  
**Mode:** Dark  
**Platform:** .NET 10 WPF / VB.NET desktop app  

---

## 1. Purpose

Neon Ink should be added as a first-class dark theme for FileCabinet.

The goal is not only to repaint the interface. Neon Ink gives the app a consistent semantic language for vault state, artifact type, navigation, validation, warnings, relationships, and preview/detail panels.

The theme should make FileCabinet feel like a durable artifact manager: dense, technical, readable, archive-aware, and alive with state where state matters.

Core principle:

> Color communicates meaning before decoration.

---

## 2. Current UI Baseline

FileCabinet is a local-first Windows desktop vault built with .NET 10, WPF, VB.NET, and XAML.

The current UI already points in the Neon Ink direction:

- Dark layered application shell.
- Left navigation with vault, category, tag, and status groupings.
- Dense dashboard tiles for totals, size, indexed count, large objects, and quarantine.
- Artifact table with metadata columns.
- Right inspector with preview, details, relations, and vault health panels.
- Status color already used for quarantine, starred, indexed, ingest, and repair activity.

Most current styling lives directly in `MainWindow.xaml` as window resources, styles, and hard-coded color literals. `Application.xaml` currently has an empty `Application.Resources` block. This makes Neon Ink a good opportunity to extract color decisions into a reusable WPF theme dictionary without changing the application layout.

---

## 3. Theme Identity

```json
{
  "id": "neon-ink",
  "name": "Aptlantis Studio - Neon Ink",
  "version": "0.1.0",
  "mode": "dark"
}
```

Neon Ink is a dark, semantic, artifact-first theme. It uses near-black layered surfaces, cool cyan navigation, violet process states, green validation, yellow attention, red risk, orange build/file-operation energy, and magenta discovery or featured emphasis.

Bright color should be reserved for meaning: state, category, focus, warning, action, or relationship. The theme should not use neon accents as background decoration.

---

## 4. Token Groups

### 4.1 Background Tokens

| Token | Hex | UI Use |
|---|---:|---|
| `void` | `#050816` | Window backdrop, deepest shell |
| `base` | `#0B0F1A` | Main application background |
| `panel` | `#111827` | Sidebars, inspector panels, cards |
| `raised` | `#162033` | Selected rows, active tabs, elevated controls |
| `soft` | `#0F172A` | Secondary panels, table rows, inactive controls |

### 4.2 Text Tokens

| Token | Hex | UI Use |
|---|---:|---|
| `primary` | `#E5E7EB` | Primary labels, file names, stat values |
| `secondary` | `#CBD5E1` | Supporting labels, tab labels |
| `muted` | `#94A3B8` | Metadata, timestamps, paths |
| `faint` | `#64748B` | Disabled or low-priority text |
| `inverse` | `#050816` | Text on bright badges |

### 4.3 Semantic Tokens

| Role | Hex | FileCabinet Meaning |
|---|---:|---|
| `info` | `#22D3EE` | General information, active focus, drop zones |
| `structure` | `#06B6D4` | Categories, hierarchy, relations |
| `navigation` | `#38BDF8` | Selected nav, tabs, route markers |
| `success` | `#34D399` | Healthy, indexed, validated |
| `verified` | `#22C55E` | Verified files or trusted metadata |
| `reproducible` | `#2DD4BF` | Stable provenance or repeatable ingest |
| `important` | `#FACC15` | Starred, notes, attention markers |
| `caution` | `#FBBF24` | Repair needed, review suggested |
| `critical` | `#F43F5E` | Quarantine, blocked, destructive state |
| `error` | `#EF4444` | Failed operation |
| `process` | `#A78BFA` | Ingest, scanning, transformation |
| `pipeline` | `#C084FC` | Multi-step operations |
| `code_heat` | `#F97316` | Build artifacts, commands, generated outputs |
| `build` | `#FB923C` | Installer/package/build operations |
| `featured` | `#F472B6` | Featured files, spotlighted assets |
| `creative` | `#EC4899` | Creative/media artifacts |
| `experimental` | `#818CF8` | Preview, prototype, unstable features |
| `archive` | `#94A3B8` | Historical, retained, inactive |
| `unknown` | `#CBD5E1` | Uncategorized or unresolved state |

---

## 5. WPF Theme Integration

Neon Ink should be introduced as a WPF ResourceDictionary before adding runtime theme switching.

Recommended first step:

```text
Themes/
  NeonInk.xaml
```

`Themes/NeonInk.xaml` should define the core colors and brushes currently embedded in `MainWindow.xaml`. `Application.xaml` should merge that dictionary so resources are available app-wide:

```xml
<Application.Resources>
    <ResourceDictionary>
        <ResourceDictionary.MergedDictionaries>
            <ResourceDictionary Source="Themes/NeonInk.xaml" />
        </ResourceDictionary.MergedDictionaries>
    </ResourceDictionary>
</Application.Resources>
```

The first integration should preserve existing resource keys where practical. That keeps the change small and lowers the chance of layout or binding regressions.

### 5.1 Current Resource Mapping

| Current Resource or Literal | Neon Ink Target |
|---|---|
| `WindowBgBrush` | `void` or `base` |
| `PanelBgBrush` | `panel` |
| `CardBgBrush` | `panel` |
| `CardAltBrush` | `soft` |
| `BorderDimBrush` | low-opacity `archive` or `navigation` |
| `AccentBrush` | `navigation` |
| `PrimaryTextBrush` | `primary` |
| `SecondaryTextBrush` | `secondary` |
| `MutedTextBrush` | `muted` |
| `GreenBrush` | `success` |
| `#FFC234` starred/rating literals | `important` |
| `#F24F5F` quarantine/delete literals | `critical` |
| current blue selected-state literals | `navigation` / `raised` |

### 5.2 Suggested Resource Shape

Define both colors and brushes so future styles can choose the right abstraction:

```xml
<Color x:Key="NeonInk.VoidColor">#050816</Color>
<SolidColorBrush x:Key="WindowBgBrush" Color="{StaticResource NeonInk.VoidColor}" />

<Color x:Key="NeonInk.NavigationColor">#38BDF8</Color>
<SolidColorBrush x:Key="AccentBrush" Color="{StaticResource NeonInk.NavigationColor}" />
```

Use explicit semantic names for new resources, but keep the current public brush keys until the UI is fully migrated. For example, `AccentBrush` can point to the Neon Ink navigation color while newer styles can use `NeonInk.NavigationBrush`.

---

## 6. UI Mapping

### 6.1 App Shell

Use `void` for the outer window and deepest chrome. Use `base` for the main content area. Sidebars and inspectors should use `panel`, with borders derived from subdued `archive` or low-opacity `navigation`.

The title bar should stay restrained. Neon accent should appear as a thin focus edge, icon color, or selected route marker rather than a heavy banner.

### 6.2 Left Navigation

Navigation should use:

- `navigation` for selected vault and active route.
- `soft` for inactive nav buttons.
- `raised` for hover and keyboard focus.
- `muted` for group headings.
- Semantic badges for status counts.

Recommended badge mapping:

| Badge | Token |
|---|---|
| Inbox | `navigation` |
| Starred | `important` |
| Quarantine | `critical` |
| Unverified | `caution` |
| Missing Preview | `experimental` |
| Repair Needed | `caution` |
| Duplicate Candidates | `structure` |
| Same Source Batch | `reproducible` |
| Large Artifacts | `build` |

### 6.3 Dashboard Tiles

Dashboard tiles should act as artifact status panels. Each tile gets one dominant accent:

| Tile | Accent |
|---|---|
| Total Items | `navigation` |
| Vault Size | `process` |
| Indexed | `success` |
| Large Objects | `build` |
| In Quarantine | `critical` |

Glows should be subtle and state-driven. Idle tiles should use border and icon color without strong glow.

### 6.4 Drop Zone

The ingest drop zone is a primary interaction surface.

Use:

- `info` border for idle ready state.
- `process` glow for active drag-over.
- `success` for accepted files.
- `critical` for rejected files.
- `code_heat` or `build` when ingest creates generated outputs.

### 6.5 Activity Feed

Activity should be colored by event type:

| Event | Token |
|---|---|
| Ingested | `process` |
| Indexed | `success` |
| Moved | `navigation` |
| Deleted | `critical` |
| Repaired | `caution` to `success` |
| Preview Generated | `experimental` |
| Backup Created | `archive` or `reproducible` |

### 6.6 File Table

The table should prioritize scanability.

Use:

- `base` or near-void rows.
- `soft` alternating rows only if needed.
- `raised` with `navigation` accent for selected rows.
- `muted` metadata columns.
- Semantic tags in the Tags column.
- `important` star icons.

Avoid coloring whole rows by file type. File type semantics should appear as small icons, tags, rails, or compact labels.

### 6.7 Right Inspector

The inspector is an artifact panel stack.

Tabs:

- Active tab uses `navigation`.
- Preview-specific signals may use `experimental`.
- Relations use `structure`.
- Details use `info`.

Panel mapping:

| Inspector Area | Token |
|---|---|
| Preview | `experimental` or `info` |
| Details | `info` |
| Relations | `structure` |
| Vault Health | `success`, `caution`, or `critical` by result |

Relations should use `structure` for normal related-item links, `success` for strong verified matches, and `caution` for weak or inferred matches.

---

## 7. Migration Strategy

Neon Ink should be integrated in two phases.

### Phase 1: Extract and Normalize

Move the existing inline brush and color resources from `MainWindow.xaml` into `Themes/NeonInk.xaml`. Keep the existing style keys and layout intact.

Replace hard-coded color literals gradually:

1. Background and panel colors.
2. Text colors.
3. Navigation and selected states.
4. Badge, warning, success, quarantine, and destructive states.
5. Preview, relation, ingest, and vault health accents.

This phase should not introduce user-facing theme switching. The goal is to make the current dark UI explicitly Neon Ink.

### Phase 2: Make Theme Selection Explicit

After the theme dictionary is stable, add a named theme record such as `neon-ink`. Theme switching can then load the selected dictionary at startup or through settings.

Only add runtime theme switching after resources are centralized and all major screens use semantic brushes. That keeps the first pass focused and testable.

---

## 8. Implementation Steps

1. Create `Themes/NeonInk.xaml`.
2. Move the current shared colors and brushes from `MainWindow.xaml` into the theme dictionary.
3. Merge `Themes/NeonInk.xaml` from `Application.xaml`.
4. Preserve current brush keys such as `WindowBgBrush`, `PanelBgBrush`, `CardBgBrush`, `CardAltBrush`, `BorderDimBrush`, `AccentBrush`, `PrimaryTextBrush`, `SecondaryTextBrush`, `MutedTextBrush`, and `GreenBrush`.
5. Add new Neon Ink semantic brush keys for `navigation`, `success`, `important`, `caution`, `critical`, `process`, `structure`, `experimental`, `archive`, and `build`.
6. Replace hard-coded blue, yellow, green, and red literals in `MainWindow.xaml` with semantic resources.
7. Update dashboard tiles, badges, selected rows, right-panel tabs, relation indicators, and vault health states to use semantic tokens.
8. Build the app to catch missing WPF resources.
9. Visually inspect the dashboard, navigation, table, preview, details, relations, and vault health panels.

---

## 9. Acceptance Checklist

Neon Ink can be considered integrated when:

- The app builds without XAML resource errors.
- `Themes/NeonInk.xaml` is merged through `Application.xaml`.
- No `StaticResource` reference is missing at runtime.
- Existing layout, commands, bindings, and view model behavior remain unchanged.
- The shell uses `void`, `base`, `panel`, `raised`, and `soft` consistently.
- Primary labels, metadata, disabled text, and inverse badge text use the text token ladder.
- Navigation, selected rows, tabs, and focus states use `navigation`.
- Quarantine and destructive states use `critical` or `error`.
- Indexed, verified, and healthy states use `success` or `verified`.
- Starred, rating, and attention states use `important`.
- Ingest, scan, and process states use `process`.
- File-operation/build states use `code_heat` or `build`.
- Relationship surfaces use `structure`, with stronger verified relations using `success`.
- No component uses bright neon color without semantic purpose.
- Dense table and inspector layouts remain readable.
- Preview, Details, Relations, and Vault Health all feel like part of the same theme.

---

## 10. Verification Notes

Recommended checks after implementation:

```powershell
dotnet build
dotnet test FileCabinet.Tests/FileCabinet.Tests.vbproj
```

The test run is most important if the implementation touches code-behind, view models, packaging behavior, or anything beyond XAML resources. For a pure documentation update, confirm that `FileCabinet.vbproj` still includes `docs\*.md` as content so this overview is packaged with the app documentation.

---

## 11. Summary

Adding Neon Ink as a theme turns FileCabinet from a dark utility UI into a semantically themed artifact manager.

The current app is already structurally close. The integration work is mainly about extracting inline WPF colors into a named theme dictionary, replacing ad hoc styling with semantic resources, and ensuring every bright accent communicates state, category, action, or relationship.
