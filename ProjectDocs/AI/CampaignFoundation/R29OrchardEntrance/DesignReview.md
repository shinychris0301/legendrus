# R29 orchard entrance — implementation design

Implements the ten concrete151–160 briefs from [R28](../R28CheckpointAudit/NextSectorPlan.md). Existing1–150 and all prior story/localization entries remain unchanged. No new mechanics or artwork; effects remain deferred.

|Rounds|Purpose|
|---|---|
|151/156|Recovery after150/155; modest collection and scattered single ice|
|152|Offset crate entrances with open central columns|
|153|Two unequal feather lanes, stronger lower locks and separate ice|
|154|Open six-color blue/purple collection|
|155|Three separated feather routes with layered lower crates|
|157|Stronger inner/lower locks contrasted with single outer locks|
|158|18 consumed moves with six two-stage ice cells; no timer|
|159|Asymmetric crate delivery plus off-route chains|
|160|Three locked delivery routes and side crates; local orchard checkpoint|

Boards8×7, seeds16151–16160, recovery34moves/5colors; normal29moves except survival28;155hard28/6colors,160hard30/6colors. Goal quantities count destroyed cells rather than layers. Delivery starts and bottom exits are open. Existing obstacles, feather delivery and special-creation rules retain their actual meaning. Difficulty is provisional until human play; no solver-only tightening.

The150 scene promises orchard/field access. After160 `campaign.chapter01.orchard_entrance` states the entrance is open and directs the next work toward paths between trees and then fields. RU/EN scene, ten names and twenty distinct outcome reactions are installed. No harvest, helper, production, restoration or departure system is implied. Returns ChapterMap; plannedChapter1 finale500 remains unavailable.

First-clear rewards151–160 total1840gold,6gems,1SeaPearl. No separate milestone grant. Candidate uses existing CampaignActive loader and save/navigation services.161 and Chapter2 remain unavailable.
