# R35 — installed campaign playtest collection

The recorded menus previously stopped at the R14 rounds 31–40 candidate. The new Editor menu selects a ten-round block from installed CampaignActive content. It retains historical menus and the existing preview, journal, board, recorder and local-export architecture.

`CampaignActivePlaytestSetup` is compiled only under `UNITY_EDITOR`. It validates the actual installed definition and four JSON files without legacy fallback, validates the range, and creates a canonical campaign fixture in a privately owned in-memory journal. The fixture contains acknowledged prior progress, 30 energy and zero boosters. No artificial starts/wins are emitted into the recorder; runSequence starts at zero. At round 1 the original intro remains unread. At later blocks only earlier eligible fixture scenes are acknowledged. Normal GameState and PlayerPrefs are not used by the setup.

Recording metadata adds optional `campaignStartRound`, `campaignEndRound`, `fixtureCompletedThrough` and `collectionContext` fields. Historical schema-1 sessions still deserialize with default values. The existing per-attempt rule version and level fingerprint remain authoritative. The full content hash includes the definition, levels, story and both locales. Session exports retain separate manual/automated source labels.

The existing board host calls InitializeCampaign, which uses the same CurrentGameplayRules and current board artwork as normal gameplay. No matching, budgets, layouts, rewards, artwork or effect changes are included.

The block-end screen still waits for the final round's story acknowledgement. Its text now states the recording boundary rather than incorrectly calling the following installed round unavailable. Replay explicitly selects EndRound: when a next round exists, the result acknowledgement has already advanced selection. Without this correction a block-ending replay could have started the next round. Selection alone neither spends energy nor grants rewards; pressing Play starts a separate replay attempt.

Only development UI text/control composition changes; no production page prefab/layout regeneration is required. Runtime additions to the generic recorder are backward-compatible metadata. New helper and tests have stable .meta files. Test fixture progress and service-simulated setup are always distinguished from actual board commands and human evidence.
