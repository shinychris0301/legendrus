# Team Hub Footer Touch R6 — visual and interaction review

The information icon keeps the approved source appearance. Only its native raycast area expands: 48×48 to 76×76 reference pixels. It remains separated from Friends and inside the safe viewport. No artwork was regenerated or recolored.

Native narrow Russian and relocalized English information-panel captures were inspected. Footer ornaments remain whole; the Friends control is intentionally a sliced panel. The information panel blocks underlying footer input while open and closes through its native handler.

Compared with Typography and Progress R5, six of eight matching captures are pixel-identical. Tall Russian differs by four pixels and narrow Russian by two, each with maximum channel delta one. See [comparison data](BaselineComparison.json). This establishes preservation of the approved baseline, not exact agreement with every original-reference detail.

Final verification: 11 Unity tests passed (6 EditMode + 5 PlayMode), 17 native captures, and 11 browser review views passed. Verify01's new test incorrectly treated Friends as a simple sprite; corrected the assertion without changing Friends artwork, then reran successfully in Verify02.

Existing reconstructed surfaces, uncertain exact source-font identity, and tall-screen scenery space remain. The information-panel texture's existing vertical stretch is unchanged. No physical-device/Android/backend/performance/full-suite validation was performed.

Next recommendation: Team Ranking responsive/footer audit.
