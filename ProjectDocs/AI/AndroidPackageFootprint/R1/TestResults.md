# Android package footprint R1 — audit results

**The first reduction to test is packaging overhead, before changing artwork.** The current 1,388,384,859-byte APK contains **550,165,625 bytes (39.63%) of unindexed local ZIP records**. No smaller APK was produced in this audit.

## Exact APK accounting

Decimal MB below; this is compressed on-disk APK content, not runtime memory.

| Component | Bytes | MB |
|---|---:|---:|
| Unity `data.unity3d` | 801,241,165 | 801.24 |
| Native libraries | 29,571,462 | 29.57 |
| DEX | 3,097,776 | 3.10 |
| Other indexed payload | 4,233,388 | 4.23 |
| Indexed local headers/extra fields | 32,733 | 0.03 |
| Unindexed local records | **550,165,625** | **550.17** |
| Trailing signing block/central directory | 42,710 | 0.04 |
| **Total APK** | **1,388,384,859** | **1,388.38** |

All 403 central-directory entries passed ZIP CRC verification. All inter-entry gaps were walked structurally: 8,407 unnamed, zero-payload local records account for the entire 550.17 MB. The largest gap is 538.52 MB. These records are outside the indexed game payload, not evidence that the artwork itself is corrupt. Their structure is consistent with incremental packager free-space records; confirming that fresh packaging removes them is the next experiment.

With identical compressed payload, avoiding that overhead would put the package near **838 MB**, before any changed alignment/signing/compression overhead. This is an estimate, not a measured new build or guaranteed saving. Do not manually strip bytes from the signed APK; rebuild through the existing guarded Unity pipeline and compare the resulting signature, contents and alignment.

## Asset findings

The last build reused player data and contains no new per-asset report. The prior full build's archived APK was verified against its recorded SHA256. Its `data.unity3d` and the current APK's entry are **byte-identical by SHA256**: `4045f968b83e3f96d779c83acb8e96012ae059555be8cc509da6a384a7622827`. Therefore the earlier full-build asset inventory is relevant evidence for this unchanged payload. It reports **textures at 99.1%** of user asset data.

Parsed 2,365 reported asset rows. The following folder totals sum rounded Unity per-asset uncompressed sizes; they are approximate serialized asset data, **not separate compressed APK savings and not measured resident memory**.

| Folder | Approximate uncompressed MiB |
|---|---:|
| GameplayAligned | 273.21 |
| HomeAligned | 212.53 |
| FailAligned | 105.43 |
| PreLevelAligned | 64.00 |
| SplashTitleReconstructed | 60.00 |
| VictoryAligned | 51.19 |
| IndividualRankingAligned | 40.08 |
| WorldMapAligned | 38.00 |

426 reported PNG assets currently have explicit Android RGBA32 overrides; their reported uncompressed sizes sum to about 999 MiB. `LockedDesignImporter` deliberately preserves source pixels, alpha and, for minified controls, mipmaps. Existing UI acceptance tests also assert these formats. A blanket compression/size change would conflict with the approved fidelity baseline, especially extracted lettering and fine ornament edges. The approved Splash remains excluded from redesign or opportunistic import changes.

Examples worth evaluating only after packaging is corrected:

- Home's coin icon source is 1,254×1,254, while a Home resource-icon placement is 53×61 reference pixels. It is shared by other pages, so a source reduction requires checking every use.
- Home's progress-fill source is 2,172×724; Gameplay uses it in thin progress bars, while Home uses a larger presentation. Do not crop or resize from one usage alone.
- Individual Ranking's 1,814×1,942 frame contributes about 13.4 MiB of reported data. Its frame/border and lettering readability need native visual comparison before format changes.

Ten groups of byte-identical Resources PNG sources were found, including shared booster rims, gem copies, headers and backing panels. Excess source PNG bytes total only 1.37 MB; that is **not** a deduced APK saving. All ten groups have multiple paths in the build inventory. Different GUIDs/import settings and dynamically constructed Resources keys must be reconciled before any consolidation. No deletion is authorized by this inventory alone. No `ReferenceOnly` or `/Editor/` asset paths appear in the parsed report; historical revision suffixes do not prove an asset is unused.

## Checks and preservation

- Exact APK byte accounting, ZIP CRC, structural gap parsing and current/prior Unity payload hash comparison: passed.
- Archived full-build APK provenance hash: passed.
- **7,774 project files unchanged** across Assets, Packages and ProjectSettings; 0 changed/added/deleted there.
- Current APK SHA256 unchanged: `456c9d9475461f4bb7bad8d155cbf629cac34ac25a27ff4a0b5a20b8986a1f19`.
- No Unity run, new compilation/runtime tests, build, repack, signature modification, source/importer/prefab edit, phone/ADB action or deployment. Existing signing verification remains evidence from the previous task, not a newly executed check here.

[Measured summary](QA/Summary.json) · [all APK entries](QA/ApkEntries.json) · [unindexed records](QA/UnindexedLocalRecords.json) · [rounded asset inventory](QA/PackedAssetsRounded.json) · [folder totals](QA/PackedFoldersRounded.json) · [identical-source candidates](QA/IdenticalSourceCandidates.json) · [scope](QA/Scope.json) · [reproducible audit script](Sources/Audit.py).

## Revised next steps

1. **Fresh guarded packaging:** archive the current APK and isolate the generated launcher packaging outputs/intermediates so the packager creates a fresh archive. Preserve Unity content, source assets, import settings, package versions, identity, signing and guards. Do not remove player data or source files, bypass guards, or strip/re-sign the APK manually.
2. Verify actual size change and gap inventory; compare every indexed entry, Unity asset payload hash, runtime metadata, permissions, signing certificate and 16 KiB ZIP/ELF alignment. Record any new payload difference instead of assuming it is harmless.
3. Only then evaluate a bounded texture/import optimization outside Splash, beginning with shared high-resolution controls. Capture before/after in their largest real usage, inspect alpha/lettering and update owner/importer and meaningful acceptance checks together. No blanket lossy compression or Resources migration.
4. Keep physical Android execution deferred as requested. Human Chapter 1 pacing, font provenance and device lifecycle/performance/readability remain open; hosting/live services deferred and gameplay effects last.
