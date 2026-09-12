# Legends of Rus — v1.15 Step 2

v1.15 Step 2 implements the source foundation for runtime economy
transactions and energy integration without activating it in the released
prototype. The milestone adds an atomic multi-resource transaction engine, a
detached copy-on-write coordination boundary, an immutable runtime policy, and
a fail-closed approval gate bound to the reviewed Step 1 catalog and exactly
Unity 6000.5.9f1.

The detached coordinator rejects caller-spoofed free retries, detects clock
rollback even at full energy, requires completion evidence before a first-clear
grant, and takes first-clear amounts only from the immutable SHA-bound policy
rather than mutable level data.

Unity 6000.5 compatibility cleanup removes unsupported `NonParallelizable`
fixture annotations, uses the current unsorted `FindObjectsByType` overload,
and names hotspot action state without hiding `MonoBehaviour.IsInvoking()`.

The live `GameState`, `ScreenRouter`, `Match3Board`, and `RuntimeBootstrap`
paths remain unchanged. They do not register or consume the Step 2 coordinator
or activated policy. The catalog also remains `proposalOnly: true` and
`authoritative: false`. Consequently, existing entry, refill, reward, booster,
shop-mock, mail, and save behavior is still the inherited prototype behavior;
Step 2 does not claim that its known runtime hazards are fixed.

The shipped activation manifest records product approval for implementing the
Step 2 foundation, but deliberately records these values as `false`:

- `compilationVerified`
- `editorSelfTestsPassed`
- `editModeTestsPassed`
- `playModeTestsPassed`
- `runtimeActivationApproved`

Every missing verification or approval produces the frozen legacy policy. No
partially approved catalog policy is returned.

Open this folder directly in Unity 6000.5.9f1, then complete the activation
checkpoint in this order:

1. Allow Unity to import and compile the project with zero Console errors.
2. Run `Legends of Rus > Validate Commercial Vertical Slice`.
3. Run the v1.15 Step 2 Editor self-tests from the `Legends of Rus > Tests`
   menu.
4. Run all EditMode tests in Unity Test Runner.
5. Run all PlayMode tests in Unity Test Runner.
6. Record only the verification flags supported by retained passing evidence.
7. Review and separately approve the live wiring change before changing
   `runtimeActivationApproved` or connecting the coordinator to gameplay.
8. Re-run compilation, Editor self-tests, EditMode tests, and PlayMode tests
   after any activation wiring and before release approval.

Unity is unavailable in the source-packaging environment. Compilation, Editor
self-tests, Test Runner execution, and final runtime activation therefore
remain pending and are not claimed as passing.

Save compatibility is unchanged: `PlayerSaveData.saveVersion` remains `6`, the
PlayerPrefs key remains `LegendsOfRus_Save_v1`, and Step 2 adds no save field,
migration, balance rebase, or automatic grant. Store/IAP, rewarded-ad,
analytics, backend/cloud, and remote-config SDKs remain outside the milestone.

Implementation details and the activation boundary are documented in
`Assets/LegendsOfRus/Documentation/V1_15_STEP02_RUNTIME_ECONOMY_TRANSACTIONS_ENERGY_INTEGRATION.md`.
