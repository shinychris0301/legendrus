from pathlib import Path
import shutil
root=Path(__file__).resolve().parents[5];base=Path(__file__).resolve().parent.parent
changes={}
def edit(rel,old,new):
 p=root/rel;s=changes.get(p,p.read_text());assert old in s,rel;changes[p]=s.replace(old,new,1)
f='Assets/LegendsOfRus/Scripts/BalanceReviewCore/BalanceReviewModels.cs'
edit(f,'? (float)TotalWinningMovesRemaining / (TotalWinningMoveLimit / (float)Wins)','? (float)TotalWinningMovesRemaining / TotalWinningMoveLimit')
f='Assets/LegendsOfRus/Tests/BalanceReviewEditMode/BalanceTelemetryTutorialFunnelEditModeTests.cs'
edit(f,'[Test]\n        public void AggregateComputesWinningMoveMargin()','[TestCase(1)]\n        [TestCase(3)]\n        [TestCase(6)]\n        public void AggregateComputesWinningMoveMargin(int wins)')
edit(f,'for (int attempt = 1; attempt <= 3; attempt++)','for (int attempt = 1; attempt <= wins; attempt++)')
f='Assets/LegendsOfRus/Scripts/CommercialCore/Analytics/AnalyticsSchema.cs'
edit(f,'"out_of_energy",\n            "chapter_complete"','''"out_of_energy",
            "chapter_complete",
            "splash_title",
            "event_detail",
            "individual_ranking",
            "team_hub",
            "team_ranking",
            "team_chat",
            "friends_list",
            "inventory",
            "special_offer",
            "language_select",
            "login",
            "tutorial_overlay",
            "offline_state",
            "maintenance_state",
            "empty_error_state"''')
f='Assets/LegendsOfRus/Tests/BalanceReviewPlayMode/BalanceTelemetryTutorialFunnelPlayModeTests.cs'
edit(f,'using System.Collections;','using System.Collections;\nusing System.Reflection;')
edit(f,'        BalanceTelemetryReviewService service;','''        BalanceTelemetryReviewService service;
        BalanceObservationBuffer previousObservations;
        ScreenRouter previousRouter;
        GameState previousState;
        static readonly FieldInfo Observations = typeof(BalanceTelemetryReviewService)
            .GetField("observations", BindingFlags.Instance | BindingFlags.NonPublic);''')
edit(f,'''            if (BalanceTelemetryReviewService.Instance != null)
                Object.DestroyImmediate(BalanceTelemetryReviewService.Instance.gameObject);
            host = new GameObject("BalanceReviewTestHost");
            service = host.AddComponent<BalanceTelemetryReviewService>();''','''            previousRouter = ScreenRouter.Instance;
            previousState = GameState.Instance;
            service = BalanceTelemetryReviewService.Instance;
            if (service == null)
            {
                host = new GameObject("BalanceReviewTestHost");
                service = host.AddComponent<BalanceTelemetryReviewService>();
            }
            else
            {
                // Keep the shared root and every other service alive. Swap only this
                // fixture's transient buffer and restore the exact original object.
                Assert.That(Observations, Is.Not.Null);
                previousObservations = (BalanceObservationBuffer)Observations.GetValue(service);
                Observations.SetValue(service, new BalanceObservationBuffer());
            }''')
edit(f,'''            if (host != null)
                Object.DestroyImmediate(host);''','''            if (host != null)
            {
                Object.DestroyImmediate(host);
                host = null;
            }
            else if (service != null)
                Observations.SetValue(service, previousObservations);
            Assert.That(ScreenRouter.Instance, Is.SameAs(previousRouter));
            Assert.That(GameState.Instance, Is.SameAs(previousState));
            service = null;
            previousObservations = null;''')
for p,s in changes.items():
 target=base/'Sources/Before'/p.relative_to(root);target.parent.mkdir(parents=True,exist_ok=True)
 if not target.exists():shutil.copy2(p,target)
 p.write_text(s)
print('Fixed move-margin calculation, route allowlist and test-root isolation')
