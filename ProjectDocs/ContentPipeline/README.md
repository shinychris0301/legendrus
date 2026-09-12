# v1.20 Content Production Pipeline — Step 1

This folder is the non-runtime authoring and transition boundary for scalable chapter production.

## Step 1 rule

Chapter 1 remains authoritative in its existing Resources/Config files during this checkpoint. Step 1 defines and validates the future contract; it does not move, rewrite, duplicate, or replace the locked Chapter 1 runtime content.

## Canonical future namespaces

- Stable chapter IDs: `chapter01` … `chapter20`
- Address prefix: `lor.chapter.XX.*`
- Future canonical folder: `Assets/LegendsOfRus/Content/Chapters/CHXX`
- Display-name localization key: `chapter.XX.title`

The template uses Chapter 2 and 10 levels only as an authoring example. The pipeline supports 1–100 levels per chapter; the example does not declare that every chapter must contain 10 levels.

## Migration order

1. Validate existing Chapter 1 level, story, and RU localization catalogs without modifying them.
2. Review the 20-chapter manifest contract and stable address namespace.
3. Review content ownership and locked-art non-destructive rules.
4. Review template and localization conventions.
5. Review QA automation and migration plan.
6. Only a later explicitly authorized checkpoint may create canonical chapter folders or migrate runtime loading.

## Locked-art rule

Approved art/reference PNGs are immutable in Step 1. Automatic generation and overwrite remain disabled.
