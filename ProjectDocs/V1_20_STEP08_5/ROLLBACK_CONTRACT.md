# Step 8.5 rollback contract

The first explicit Apply creates `ProjectDocs/V1_20_STEP08_5/CutoverRollback/IMMUTABLE_BASELINE_SNAPSHOT.json`.

The snapshot records whether an active Shop prefab/meta existed before Step 8.5 and stores raw copies when present. It is never overwritten by Apply. The Shop cutover activation state is a separate Resources JSON artifact and is created only after the production prefab has imported and validated.

Rollback order:
1. Remove the activation-state resource.
2. Remove the Step 8.5 active Shop prefab.
3. Restore the original Shop prefab/meta if the snapshot says one existed; otherwise leave the active Shop slot absent.
4. Clear Step 8.5 applied/functional/Android acceptance state.
5. Preserve the immutable snapshot and write a rollback audit JSON.

The always-present Step 8.5 runtime registry code is dormant when the activation-state resource is absent, so rollback returns Shop rendering to the inherited DesignScreenRegistry path without rewriting the accepted core registry.
