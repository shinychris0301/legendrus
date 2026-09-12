# Legends of Rus — v1.15 Step 1

Static source verification confirms a versioned, proposal-only economy catalog
and deterministic offline validation and simulation foundation are present. The
catalog covers currencies, energy, booster activation inventory costs and
purchase-price availability, continue options, and reward sources. It is
deliberately non-authoritative: loading, validating, or simulating it cannot
change `PlayerSaveData`, existing balances, rewards, inventory, progression, or
UI behavior.

The comparison workflow distinguishes three kinds of prototype evidence:

- `declared_unwired_config` for values present in the inherited economy JSON;
- `runtime_hardcode` for values currently enforced by runtime C#; and
- `authored_level_data` for Chapter 1 first-clear rewards.

This provenance matters because the inherited economy JSON is not the source
used by the current runtime. Step 1 compares the proposal with that prototype
without silently making either set of values authoritative.

Booster purchase prices are not defined or inferred. Booster purchase
affordability is therefore reported as `INCOMPLETE`, while activation inventory
requirements remain eligible for deterministic simulation.

Open this folder directly in Unity 6000.5.9f1, then run:

1. `Legends of Rus > Validate Commercial Vertical Slice`
2. `Legends of Rus > Tests > Run v1.15 Step 1 Self-Tests`
3. EditMode and PlayMode tests from Unity Test Runner

Step 1 does not integrate a store, rewarded-ad SDK, analytics provider,
backend, or remote-config service. It does not redesign economy screens,
migrate saves, apply proposed values to existing players, or change Chapter 1
level rewards. Those runtime decisions require a separately approved milestone.

Unity compilation, Editor self-tests, and Test Runner execution remain pending
until the project is opened in Unity 6000.5.9f1.

Implementation details are in
`Assets/LegendsOfRus/Documentation/V1_15_STEP01_ECONOMY_CONFIGURATION_SIMULATION_FOUNDATION.md`.
