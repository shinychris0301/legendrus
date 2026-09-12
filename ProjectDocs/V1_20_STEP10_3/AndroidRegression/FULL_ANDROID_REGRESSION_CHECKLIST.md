# v1.20 Step 10.3 — Full Android Regression Checklist

This checklist is a physical-device acceptance gate. Record PASS in Unity only after every applicable check succeeds on the **current integration fingerprint**. Any later cutover or rollback invalidates the locks.

## Required flow

1. Cold boot → Splash/Loading/Home.
2. Home navigation, World Map, Chapter Map, visible Back and Android system Back.
3. Pre-Level → Gameplay; 4-way single-drag swipe; boosters/obstacles; pause/resume/restart/exit.
4. Victory/Fail/Chapter Complete; save/load; energy; progression.
5. Shop/Special Offer/rewarded flows fail closed without verified authority.
6. Events/rankings/team/chat/friends/mail preserve their accepted authority boundaries.
7. Settings/language/guest login/tutorial/system states work without activating missing backends.
8. Safe area/cutout, live Cyrillic text, touch targets, background/resume, and responsive layout pass on physical Android hardware.
9. No unauthorized currency, energy, entitlement, reward, mail, social, or leaderboard mutation occurs.
10. No new runtime exceptions or functional regressions are observed.

Use the Step 10.3 Unity menu commands to record Functional Regression PASS, then Physical Android Regression PASS, then evaluate Final Android Regression Acceptance.
