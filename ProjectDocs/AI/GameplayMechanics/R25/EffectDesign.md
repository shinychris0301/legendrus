# R25 — Gem scatter design

Use the approved gem itself as the material source. Every ordinary gem hit has eight irregular triangular shards and sixteen small faceted chips; no circular halo. Line items emit these at each actual cell-contact event, not simultaneously across a row. This also enriches ordinary matches and other effects that shatter ordinary gems.

| Layer | Timing after contact | Visual behavior |
| --- | --- | --- |
| Impact light | 0–0.13 s | Short tapered colored flare with a compact bright glint |
| Tiny chips | 0–0.38/0.435/0.49/0.545 s | Varied radial trajectories, fast initial scattering, drag and falling motion; sizes about 6–10% of a cell |
| Chip sparkle | 0–0.55 s | Eight light accents follow the same chip paths; small tapered trails and fading glints |
| Larger shards | 0–0.62 s | Eight irregular source-textured wedges, outward travel, tumble, gravity and fade |

Cell coordinates seed trajectory variation without consuming gameplay random values. Keep the existing contact schedule, input release and resolution plan. Six reusable texture batches; at most sixteen active gem breaks per color, each at most 88 vertices. The existing bounded additive mesh supplies light. Clip both layers to the board; clear them on recovery/disable. This is a Canvas-based particle implementation suited to the existing UI board, with no per-particle GameObjects or physics bodies.

Review full-size normal-speed Unity captures of row, column and combined clear, alongside R24. Code limits are not Android performance evidence. This pass changes the shatter layer, not all item choreography.
