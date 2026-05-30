# FileCabinet v1.3.1 — Explorer Context Menu Patch

This patch adds Windows Explorer intake shortcuts for faster vault capture:

- Copy to FileCabinet
- Move to FileCabinet

The installer registers both verbs for filesystem objects, so files and folders can be sent to FileCabinet from Explorer. Each verb launches FileCabinet with a one-time intake mode and the selected path.

The command-line intake mode does not change the user's saved default drop-zone intake preference.
