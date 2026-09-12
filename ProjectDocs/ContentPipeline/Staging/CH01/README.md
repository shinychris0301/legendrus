# Chapter 1 canonical package staging — v1.20 Step 2

This folder is **non-runtime staging evidence**. The three legacy Chapter 1 payloads are copied byte-for-byte from `Assets/LegendsOfRus/Resources/...` and their SHA-256 hashes are pinned in `chapter_package_manifest.json`.

- Runtime still reads the legacy Resources files.
- No Addressables entries are created or moved.
- No locked artwork is copied, regenerated, or overwritten.
- No level balance, reward, progression, or localization content is changed.
- `EvidenceReviewed` remains false until a human reviews the generated package.
