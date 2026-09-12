# Chapter Pipeline Migration Plan

## Phase A — contract (this checkpoint)
- Define manifest, template, stable IDs, addresses, ownership, localization, and QA rules.
- Validate the current Chapter 1 catalogs in place.
- Do not change runtime routes or asset loading.

## Phase B — canonical Chapter 1 package
- Create `Content/Chapters/CH01` only after explicit migration approval.
- Copy/transform data through reviewed tooling, compare semantic hashes/validation results, and preserve a rollback path.
- Do not alter approved art bytes.

## Phase C — runtime loader cutover
- Add a chapter-aware catalog/loader behind the existing runtime API.
- Keep compatibility fallback until Chapter 1 regression tests and representative-device tests pass.

## Phase D — Chapter 2 production entry
- Author Chapter 2 with the reviewed template.
- Add chapter-scoped localization, story, level, environment, and map-art addresses.
- Validate content ownership and QA evidence before runtime activation.
