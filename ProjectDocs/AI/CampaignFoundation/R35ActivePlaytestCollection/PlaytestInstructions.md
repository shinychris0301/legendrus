# Installed campaign — recorded playtest

1. Open the project in Unity 6000.5.9f1 and enter Play Mode. Reach **Home**, with no modal or other development preview open.
2. Open **Legends of Rus → Development → Campaign Active Playtest (Recorded)**. This opens the existing Campaign Playtest Monitor with a block selector.
3. Choose **Rounds 31–40** first, then use a separate recording for **41–50**. All installed ten-round blocks through **191–200** are selectable. Click **Start installed-campaign recording**.
4. Play without inventory boosters for the first pass. The disposable fixture starts with **30 energy and zero boosters**; rewards earned during your actual rounds can subsequently add stock. Earlier rounds are fixture progress, not recorded play. Normal player progress is untouched.
5. After Victory or Fail, before Continue or Retry, rate **Difficulty**, **Goal clarity** and **Special opportunity** in the monitor. Include losses and retries. Do not rate an automated run as human evidence.
6. At the end of the block, finish its story scene. The completion screen offers Save, Exit and optional replay of that block's final round. It does not start the next block. Replay is a separate recorded attempt.
7. Use **Show recording** or **Copy recording path** in the monitor. Stop Play Mode when finished. An unfinished attempt is saved as interrupted; its board is not resumed next time. Reopening the selector creates a fresh disposable session and a separate export.

Exports remain under `Application.persistentDataPath/CampaignPlaytests/Sessions/manual_campaign/`. The content version starts with `CampaignActive-` and hashes the installed definition plus all four content/localization files. Each export includes selected start/end rounds, fixture-completed-through and collection context. Each attempt includes the rule version, level fingerprint, run/attempt, outcomes, moves, goals, inventory usage and ratings. No upload is automatic.

If local saving fails, use **Save now** after resolving the folder problem; the final-screen Exit remains guarded by a successful save. Older R9/R12/R13/R14 menu entries remain available for historical comparisons; they are different content sources.

This is an Editor development preview using the existing campaign session and native gameplay board. It is not an Android device test or acceptance of the full production navigation journey. Human difficulty remains unverified until the new manual exports are reviewed. Effects and animations remain deferred.
