# Legacy Home Firebird usage audit R8 — 2026-10-01

Completed the scheduled read-only audit. **`HomeAligned/home_firebird_generated.png` has no identified current consumer and is a bounded archive candidate.** It is still bundled in the R7 development APK. No resource, importer, prefab or runtime source was changed in this task.

## Asset and ownership

- Source: `Assets/LegendsOfRus/Resources/ProductionUI/HomeAligned/home_firebird_generated.png`.
- GUID: `072000aa439cc61468feba1c5d67b108`.
- Actual PNG examined: 1024×1536, 2,416,121 bytes; Unity imports RGBA32 with 11 mip levels. Android override remains RGBA32/max4096. [Source/import inventory](QA/Asset.json), [Unity import observation](QA/EditorAudit/Imported.json).
- R7 packed inventory contains three object rows totaling **8,389,220 bytes**: texture data8,388,604, texture metadata160 and sprite456. This is not compressed APK savings or measured resident memory.
- No byte-identical other PNG exists among the task-start Assets/Packages/ProjectSettings files. The recommendation is archival of this exact unused resource, not duplicate substitution or removal of similarly named art.

Source scan covered **5,617 text files** in Assets, Packages and ProjectSettings. The only candidate-name/GUID match is its own metadata GUID. There are no serialized reference lines, resource-key literals or asset-name mentions in production code/configuration. [References](QA/References.json).

Unity's recursive dependency inspection covered **99 serialized asset roots** (prefabs, scene, assets, materials and other supported serialized types); **zero depend on the candidate**. The exact root list is retained in [Dependencies.json](QA/EditorAudit/Dependencies.json). This supplements, rather than replaces, the source/load-path review.

Reviewed Home's strict legacy `TextureNames`, measured preparation keys, fresh constructor, old/current migrations and `HasRequiredBindings`. The current contract explicitly requires the retained `Firebird` Image to have a null sprite and an inactive GameObject. Fresh construction and legacy migration already set that state. Current visible Home art uses `HomeReferenceAligned/source_hero.png`; the legacy intermediate `home_hero_firebird` is a different resource. Splash/Loading prefab dependencies are also independent of this candidate. [Owner trace](QA/OwnerTrace.txt).

Reviewed HomeAligned loader wrappers and their literal/array arguments in Gameplay, Victory, Shop, DailyReward, SpecialOffer and navigation styling. Their keys select portraits, frames, currencies and controls, not this file. The runtime bulk Sprite loader is scoped to GameplayAligned sprite sheets. Home's current editor importer iterates the current preparation list, while `LockedDesignImporter` applies settings by directory without creating runtime consumers. [Loader sites](QA/LoaderSites.json), [arguments](QA/LoaderArguments.txt).

## Actual verification

**Five Edit Mode cases passed, zero failed/skipped**, Unity6000.5.9f1, batch/nographics, isolated configuration. [Run summary](QA/EditorAudit/summary.json), [NUnit XML](QA/EditorAudit/results.xml).

1. Candidate loads today and its GUID/import is verified; all99 serialized dependency roots exclude it.
2. Fresh Home builds through the real delegate loader:24requests, none for the candidate; complete bindings and empty/inactive old Firebird retained.
3. Legacy Home migration (source-composition reference cleared on a disposable copy):24requests, none for the candidate; complete current composition restored.
4. Current Home migration:0requests through the legacy delegate; complete bindings and the same unused Firebird state.
5. Installed Home/Splash/Loading exclude the candidate from their dependency graphs; current Home passes its full binding contract.

The audit fixture only mutated disposable instances; no scene/prefab saves or asset moves. Temporary source/meta removed automatically. Fixture and runner are retained under Sources for reproduction. The inspected PNG was the source asset, not a new Unity page capture or a newly accepted visual.

No Play Mode, new image comparison, Android build/install, broad regression, commit or deployment ran. These checks cannot prove arbitrary future or externally supplied dynamic resource keys; they establish the current local ownership boundary. R7 runtime/build results remain historical, not rerun acceptance.

## Scope and next implementation

[Final hashes](QA/Scope.json): **all7,779 baseline Assets/Packages/ProjectSettings files unchanged**, zero changed/added/deleted. Current APK unchanged at835,948,002bytes, SHA256`2b7caabb7612d1c762a55bf7190d757f33a9d1a10f8e03e120979d9fd76f0aac`. Only audit scripts/evidence and project handoff documents were added/updated; generated caches are outside the authored changes. No new size saving is claimed.

Next implement a single-resource archival: move this PNG and its `.meta` together to `Assets/LegendsOfRus/Art/ReferenceOnly/RetiredHome/`, preserve both files byte-for-byte and retain its GUID, and add a narrowly scoped importer rule using the existing Home import contract. A naive move would otherwise fall through to different default import settings. Existing prefabs/builders need no reference replacement because they already require no candidate sprite.

Add an archive/dependency regression, verify the fresh/legacy/current Home paths and native Home/Splash/Loading flow with the Resources key absent, then use the guarded development build. Compare packed GUID membership, signing/identity/permissions/alignment and actual APK size against R7. Keep the approved Splash artwork, Home's current hero, coins and other Firebird/booster resources intact. No blanket compression or new art. Phone execution/hosting deferred; effects last. Human round31–500 pacing, font provenance and Android lifecycle/performance/readability remain open.
