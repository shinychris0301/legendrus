# Gameplay R5 — portrait and booster ornament restoration

The previous revision did not address all the reported defects. The source portrait's outer scrollwork had been cut by a geometric support mask, and rectangular wood repairs had painted over the booster caption curls. R5 repairs the artwork itself and fits live content around the restored details.

## Changes

- **Portrait:** restored original RGB and the earlier Scenario isolation alpha before the destructive ellipse support. The recovered outer curls include864 original gold pixels previously clipped away. Removed the attached sample-nameplate/header residues; retained the bottom jewel. Adjusted the portrait stencil to the actual oval opening. Existing Ivan artwork remains. The example-level badge stays omitted; the arc hidden behind it is reconstructed from the opposite side and is not claimed as exact source recovery.
- **Header:** the joined backing now starts at x115, extending115px behind the portrait, rather than ending at x230 beside it. Its original rail material continues behind the decorative band. Trimmed exterior dark padding to the metal contour; intentionally dark panel faces/recesses remain opaque. The portrait and live HUD values render above this backing.
- **Booster cards:** restored original caption-band curls instead of covering them with a rectangular wood patch.720 source-colored pixels were recovered from that earlier repair. Removed only the original sample word inside the central dark wooden face, using bounded local inpaint. Moved stock badges4px upward and fit live lettering into the clear aperture x40–148/y219–248, including its shadow. Longer names consequently render smaller; no name or function was changed.

Three new versioned sprites in `GameplayOrnamentsR5`; old resources remain untouched. No additional Scenario jobs or credits: this reuses existing Scenario isolation results and original pixels. [Reproducible asset preparation](Sources/PrepareAssets.py), [provenance](Sources/AssetProvenance.json), [restoration metrics](Sources/RestorationMetrics.json).

## Native implementation and checks

The active Gameplay prefab and owning builder agree at **revision16**. Six existing Assets files modified, seven added, zero deleted. All405 prior prefab records are retained. Only the Gameplay prefab changed; other screens, board palette, gems and gameplay rules remain unchanged. Exact file/hash audit: [ScopeAndVerification.json](QA/ScopeAndVerification.json).

- Unity6000.5.9f1 Build03: compiled, rebuilt and read back the canonical prefab successfully.
- Final Verify02: **9 Edit Mode +31 Play Mode =40 passed**, zero failed/skipped. Includes live control/binding identity, RU/EN captions and large values, two/three missions, small-phone junction visibility, input, boosters/VFX and tall simulated safe areas.
- Caption checks now use the aperture between the restored inward curls rather than the old broad rectangular label well. Initial runtime assertions exposed floating-point boundary error of0.000015 reference pixels; the final test uses0.01px tolerance. No visual behavior was relaxed to bypass a failure.
- Inspected alpha on black/white/magenta and actual Unity Russian/English941px,438px and tall576px views. Review images are actual Unity captures; specials/large values are labelled controlled fixtures.
- Browser:14 review options load at390px without JavaScript errors or horizontal overflow.

No Android build, physical-device verification or full-project suite ran. Shared font assignments still use the existing desktop development fallback; exact source-font identity and Android parity remain unresolved. Caption names longer than the reference's examples require smaller text to retain all ornamentation. Hidden portrait details and the left header extension are reconstructions; the whole screen is not claimed pixel-identical.

Next: visual review of the native R5 page, including long-caption readability on a phone.
