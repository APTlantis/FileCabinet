# Blue Slate Theme Integration Overview

**Target:** FileCabinet / Aptlantis Studio UI  
**Theme:** Aptlantis Blue Slate  
**Mode:** Dark  
**Platform:** .NET 10 WPF / VB.NET desktop app  

## Purpose

FileCabinet uses Blue Slate as its native WPF theme foundation. The migration keeps the existing three-zone operational shell intact while moving the palette, shared brushes, default control styling, and model-provided accent strings onto Blue Slate semantics.

The goal is a dense local-first vault interface that feels dark, technical, archival, precise, and evidence-driven without turning bright accent color into decoration.

## WPF Resource Shape

The app loads `Themes/BlueSlate.xaml` from `Application.xaml`. That dictionary provides:

- canonical `Atl.Color.*` raw palette resources from the Blue Slate token source
- semantic `Atl.Brush.*` resources for background, panels, borders, text, action, focus, warning, attention, taxonomy, archive, success, and verified states
- FileCabinet compatibility brush keys such as `WindowBgBrush`, `PanelBgBrush`, `CardBgBrush`, `CardAltBrush`, `BorderDimBrush`, `AccentBrush`, `PrimaryTextBrush`, `SecondaryTextBrush`, `MutedTextBrush`, and `GreenBrush`
- WPF-native default styles for common controls including buttons, text boxes, combo boxes, check boxes, radio buttons, list boxes, data grids, tab controls, progress bars, and menus

The compatibility keys keep the existing XAML layout stable while allowing newer WPF apps to consume the reusable `Atl.*` resource names.

## Local Extensions

Blue Slate does not define a dedicated destructive red in its base token set. FileCabinet adds a local `Atl.Brush.Danger` / `BlueSlatePalette.Danger` extension for quarantine, delete, missing-file, and security-sensitive states. This prevents destructive flows from being expressed as ordinary warning amber.

FileCabinet also keeps a local build/package extension for archives, large objects, installers, and package-like artifacts. This is mapped close to Blue Slate warning/attention colors but remains named separately in code so build/file-operation semantics do not drift into generic warning treatment.

## VB Model Accents

Some FileCabinet visuals are supplied by view models as string brush values rather than XAML resources. `BlueSlatePalette.vb` centralizes those values so artifact icons, preview fallback cards, health breakdown bars, stat cards, hash badges, and activity entries use the same semantic palette as the WPF dictionary.

## Acceptance Notes

This migration is visual and structural only. It does not make a release-ready claim, rebuild the installer, or update release hashes. A release pass still needs the normal DRS build, test, installer, launch, SHA-256, and documentation verification gates.
