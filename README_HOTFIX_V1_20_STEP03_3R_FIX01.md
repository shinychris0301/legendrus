# v1.20 Step 3.3R.3 → Step 3.3R-FIX01 Hotfix

Overlay this archive on the exact v1.20 Step 3.3R.3 project. It adds the missing serialized runtime page delivery and FIX01 standalone runtime assets, patches the production page loader for direct asset-based fallback, and guards the older page builders from overwriting FIX01. No screenshot/reference file is deleted.

The hotfix intentionally supplies deterministic `.meta` GUIDs for the production page marker scripts because the shipped prefabs reference those scripts. The Step 3.3R.3 release ZIP contained no serialized page prefabs, so there are no shipped prefab references that depend on a prior generated marker GUID.

After overlay, open Unity 6000.5.9f1 and allow a full refresh/import. Then run the FIX01 validation and self-test menus described in `README_V1_20_STEP03_3R_FIX01.md`.
