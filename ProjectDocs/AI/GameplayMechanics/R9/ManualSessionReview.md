# Manual session review — R9

Source: the user-supplied immutable export, copied to Sources/ManualSession.json. One manual_sandbox session, MechanicsRoundsR4, schema 1; 20 closed attempts, no active or dropped attempts. The provided export is the analysis source; later checkpoint changes are not silently substituted.

| Round | Wins | Losses | Interrupted | Findings |
| --- | ---: | ---: | ---: | --- |
| 1 — Ледяная тропа | 0 | 3 | 12 | All three losses retained both ice goals. All 12 interruptions spent no moves; do not count these as difficulty failures. Difficulty labels varied from easy to hard. |
| 2 — Под ледяной коркой | 1 | 1 | 0 | First loss cleared the crate but left both ice goals. Marked hard, goals unclear, effects readable. Retry won with 2 moves left. |
| 3 — Полет искры | 1 | 0 | 0 | Won with 2 moves left, but marked hard, goals unclear and effects busy. Winning does not establish clarity. |
| 4 — Рунные врата | 1 | 0 | 0 | Won with 3 moves left; marked fair, goals unclear and effects readable. |
| 5 — Испытание мастера | 0 | 1 | 0 | Lost with one ice and two seals remaining; crates cleared. Marked fair and goals unclear; effects unrated. |

## What the evidence supports

The eight completed attempts contain three wins and five losses. This is a single player with repeated attempts, not a population win-rate estimate. The strongest consistent observation is goal confusion across rounds 2–5. Round 3 also has an explicit busy-effects rating. The user’s accompanying message supplies a separate qualitative observation: impact feels weak and booster designs are poor. The JSON has no direct intensity rating, input trace, video, audio or frame-time data. It cannot identify the exact move or rendering defect behind those opinions.

The repeated zero-move restarts are consistent with retry navigation, but do not establish an input bug. Foreground duration excludes post-result time and is not an FPS metric. No balancing change should be inferred solely from those durations.

## Verified implementation issues and response

- Most prototype hits were a thin ring plus six tiny flat diamonds with a shared 0.42-second lifetime. Replace with a brief charge, expanding illuminated wave, bright local core and faceted fragments; keep ordinary clears smaller than specials.
- Non-Spark waves published the gem-shatter cue before the special activation cue. Present special activation first, then clear fragments, while preserving deterministic board resolution.
- Spark traveled in 0.36 seconds with no persistent target marker. Use an eased 0.52-second arc with a restrained trail and a target ring; impact follows arrival.
- Line specials used elongated old gem artwork and Burst used an older atlas region. Generate original directional sapphire/gold and navy/gold bomb sprites; preserve ordinary gameplay gems.
- The disabled booster tint multiplied illustration brightness by 0.4 with alpha 0.8. Keep sandbox zero-stock icons recognizable at 0.8 brightness/opaque, with disabled interaction and zero counts intact.
- Inventory boosters had no prototype-specific illustrated windup. Add a bounded, non-interactive mace swing / Firebird dive / magical-tool reveal using approved shared sprites.

## Scope and remaining work

This is a sandbox presentation pass. Campaign rule migration, round budgets/layouts, reward amounts and shared production booster assets are not changed. Goal teaching copy should explicitly describe obstacle counts/layers. Recheck clarity and visual impact in a second manual pass; audio, haptics and physical-phone performance remain separate unverified areas.
