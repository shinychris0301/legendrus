# Intermediate evidence — not the final delivery

`PlayMode2/Home/` retains two actual Unity diagnostic PNGs and their original capture JSON files as exact byte copies. That run passed18/21 Play Mode cases and failed three navigation-glyph checks. These captures precede fixes for the tall cloak cutoff, caption fit, Start text/shape and reward inset.

`RetentionManifest.json` records the original paths, retained paths, file sizes, hashes and byte-equality checks. No source pixels or copied JSON contents were edited.

`PostImport1SourceAudit.json` preserves the initial source audit before the additional `ProductionHomeGoldText` implementation. It must not be presented as the final source audit.
