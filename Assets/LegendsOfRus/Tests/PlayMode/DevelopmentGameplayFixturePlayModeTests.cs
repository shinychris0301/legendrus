// Cold-launch preset verification against the independently rehearsed service fixtures.
using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace LegendsOfRus.Tests.PlayMode
{
    public sealed class DevelopmentGameplayFixturePlayModeTests
    {
        static readonly string[] Keys = { "LegendsOfRus_Save_v1", "LegendsOfRus_Save_v1_Backup",
            "LegendsOfRus_Save_v1_Staging", LanguagePreference.Key, "LegendsOfRus_Compliance_v1" };
        static readonly string[] Cases = { "stock-999", "stock-1000", "stock-9999", "stock-max", "story-early", "story-finale" };
        [Serializable] sealed class Fingerprint { public string[] hashes; }
        [Serializable] sealed class Evidence
        {
            public string fixture, stage;
            public int completedRounds, acknowledgedScenes, recapScenes, selectedRound, unlockedRound, stars;
            public int gold, gems, mace, firebird, light, pearl, energy;
            public string chapterPhase;
            public bool normalFingerprintsPreserved, restoredPayloadExactly, nativeRecapPreservesJournal, replayGrantRejected;
        }

        [UnityTest]
        public IEnumerator PresetColdLaunchAndResumePreserveNormalData()
        {
            string stage = Environment.GetEnvironmentVariable("LOR_QA_FIXTURE_STAGE");
            if (string.IsNullOrEmpty(stage)) Assert.Ignore("Requires the explicit isolated fixture writer/reader process.");
            string name = Environment.GetEnvironmentVariable("LOR_QA_FIXTURE_CASE");
            string config = Environment.GetEnvironmentVariable("LOR_LOADING_TEST_CONFIG_ROOT");
            string output = Environment.GetEnvironmentVariable("LOR_QA_FIXTURE_OUTPUT");
            Assert.That(new[] { "write", "read" }.Contains(stage), Is.True);
            Assert.That(Cases.Contains(name), Is.True);
            Assert.That(config, Does.StartWith("/tmp/lor-gameplay-profile-r1/fixture-r4/"));
            Assert.That(Environment.GetEnvironmentVariable("XDG_CONFIG_HOME"), Is.EqualTo(config));
            Assert.That(Environment.GetEnvironmentVariable("LOR_GAMEPLAY_QA"), Is.EqualTo(stage == "write" ? name : "resume"));
            Assert.That(output, Does.EndWith("/AndroidGameplayProfile/R4FixedPresets/QA/Fixtures"));
            Assert.That(DevelopmentGameplayProfile.Active, Is.True);
            Assert.That(Path.GetFullPath(Application.persistentDataPath), Does.StartWith(config + "/"));
            var baseline = JsonUtility.FromJson<Fingerprint>(File.ReadAllText(Path.Combine(config, "normal-key-fingerprints.json")));
            Assert.That(Snapshot().hashes, Is.EqualTo(baseline.hashes));
            yield return SceneManager.LoadSceneAsync("Bootstrap", LoadSceneMode.Single);
            var state = GameState.Instance;
            Assert.That(state.CanPersist, Is.True);
            Directory.CreateDirectory(output);
            string payloadPath = Path.Combine(output, name + ".player.json");
            if (stage == "write")
            {
                // No test seeding/reset: the cold-launch production selector must
                // have committed this exact bounded state before any GameState read.
                string reference = Path.Combine(Application.dataPath,
                    "../ProjectDocs/AI/AndroidGameplayProfile/R3FixturePreparation/QA/Fixtures", name + ".player.json");
                var expected = JsonUtility.FromJson<PlayerSaveData>(File.ReadAllText(reference));
                expected.lastEnergyUtcTicks = state.Save.lastEnergyUtcTicks;
                Assert.That(JsonUtility.ToJson(state.Save), Is.EqualTo(JsonUtility.ToJson(expected)));
                File.WriteAllText(payloadPath, JsonUtility.ToJson(state.Save));
            }
            else
                Assert.That(JsonUtility.ToJson(state.Save), Is.EqualTo(File.ReadAllText(payloadPath)), "Cold-process restoration changed the prepared payload.");

            Assert.That(JsonUtility.ToJson(SaveService.Load()), Is.EqualTo(JsonUtility.ToJson(state.Save)));
            var evidence = Inspect(name, stage, state);
            if (stage == "read" && name.StartsWith("story-", StringComparison.Ordinal))
            {
                var router = ScreenRouter.Instance;
                router.Show(ScreenId.Profile); Settle(router); yield return null;
                var profile = router.Viewport.GetComponentInChildren<ProductionProfileView>();
                profile.SelectSection(ProfileSection.History); profile.ChangeHistoryPage(1);
                Assert.That(profile.HistoryPage, Is.EqualTo(1));
                string save = JsonUtility.ToJson(state.Save), journal = QaJournal();
                profile.DynamicRoot.Find("StoryRecap").GetComponent<Button>().onClick.Invoke(); Settle(router); yield return null;
                var reader = router.Viewport.GetComponentInChildren<ProductionStoryRecapView>();
                Assert.That(reader, Is.Not.Null);
                Assert.That(reader.SceneIndex, Is.EqualTo(name == "story-early" ? 2 : 57));
                if (name == "story-early") reader.Back.onClick.Invoke();
                else
                {
                    int lines = StoryCatalog.FindScene(StoryCatalog.LoadChapter1().completion.finaleSceneId).lines.Length;
                    for (int i = 0; i < lines; i++)
                    { reader.Dialogue.Continue.onClick.Invoke(); Settle(router); yield return null; }
                }
                Settle(router); yield return null;
                Assert.That(router.ActiveModal, Is.Null);
                Assert.That(router.CurrentScreen, Is.EqualTo(ScreenId.Profile));
                Assert.That(profile.HistoryPage, Is.EqualTo(1));
                Assert.That(JsonUtility.ToJson(state.Save), Is.EqualTo(save));
                Assert.That(QaJournal(), Is.EqualTo(journal));
                evidence.nativeRecapPreservesJournal = true;
                int gold = state.Save.gold, gems = state.Save.gems;
                var stock = new[] { state.Save.boosterMace, state.Save.boosterFirebird, state.Save.boosterLivingLight, state.Save.boosterSeaPearl };
                Assert.That(state.CompleteLevel(ChapterLevelCatalog.Find(1, 1), int.MaxValue).FirstClearRewardGranted, Is.False);
                Assert.That(state.Save.gold, Is.EqualTo(gold)); Assert.That(state.Save.gems, Is.EqualTo(gems));
                Assert.That(new[] { state.Save.boosterMace, state.Save.boosterFirebird, state.Save.boosterLivingLight, state.Save.boosterSeaPearl }, Is.EqualTo(stock));
                evidence.replayGrantRejected = true;
            }
            Assert.That(Snapshot().hashes, Is.EqualTo(baseline.hashes));
            evidence.normalFingerprintsPreserved = true;
            File.WriteAllText(Path.Combine(output, name + "." + stage + ".json"), JsonUtility.ToJson(evidence, true));
            yield return null;
        }

        static Evidence Inspect(string name, string stage, GameState state)
        {
            var p = state.Save; var recap = StoryRecap.Read(p);
            bool stock = name.StartsWith("stock-", StringComparison.Ordinal);
            int completed = stock ? 0 : name == "story-early" ? 8 : 500;
            int scenes = stock ? 0 : name == "story-early" ? 3 : 58;
            Assert.That(p.levelProgress.Count, Is.EqualTo(completed));
            Assert.That(p.levelProgress.All(r => r.attempts == 1 && r.completions == 1 && r.rewardClaimed && r.bestStars == 3), Is.True);
            Assert.That(p.completedStoryScenes.Count, Is.EqualTo(scenes)); Assert.That(recap.Count, Is.EqualTo(scenes));
            Assert.That(p.selectedChapter, Is.EqualTo(1)); Assert.That(p.selectedLevel, Is.EqualTo(Math.Min(500, completed + 1)));
            Assert.That(p.highestUnlockedLevel, Is.EqualTo(Math.Min(500, completed + 1)));
            Assert.That(p.stars, Is.EqualTo(completed * 3)); Assert.That(p.energy, Is.EqualTo(4));
            Assert.That(ChapterLevelCatalog.Find(2, 1), Is.Null);
            if (stock)
            {
                int count = name == "stock-max" ? int.MaxValue : int.Parse(name.Substring(6));
                Assert.That(new[] { p.boosterMace, p.boosterFirebird, p.boosterLivingLight, p.boosterSeaPearl }, Is.All.EqualTo(count));
                Assert.That(p.gold, Is.EqualTo(12540)); Assert.That(p.gems, Is.EqualTo(350));
            }
            else if (name == "story-finale")
            {
                Assert.That(state.GetChapterCompletionPhase(1), Is.EqualTo(ChapterCompletionPhase.Acknowledged));
                Assert.That(p.acknowledgedChapterCompletions, Is.EqualTo(new[] { 1 }));
                Assert.That(recap.Last().Scene.id, Is.EqualTo(StoryCatalog.LoadChapter1().completion.finaleSceneId));
            }
            else Assert.That(p.acknowledgedChapterCompletions, Is.Empty);
            return new Evidence { fixture = name, stage = stage, completedRounds = completed, acknowledgedScenes = scenes,
                recapScenes = recap.Count, selectedRound = p.selectedLevel, unlockedRound = p.highestUnlockedLevel,
                stars = p.stars, gold = p.gold, gems = p.gems, mace = p.boosterMace, firebird = p.boosterFirebird,
                light = p.boosterLivingLight, pearl = p.boosterSeaPearl, energy = p.energy,
                chapterPhase = state.GetChapterCompletionPhase(1).ToString(), restoredPayloadExactly = stage == "read" };
        }
        static void FinishNormalStory()
        {
            int remaining = 100;
            while (StoryService.AdvanceDialogue() == StoryAdvanceResult.MoreLines)
                Assert.That(--remaining, Is.GreaterThan(0));
        }
        static void Settle(ScreenRouter router)
        { for (int i = 0; i < 24; i++) router.MotionPresenter.TickForTesting(MetaUiMotionProfile.MaximumTickSeconds); Canvas.ForceUpdateCanvases(); }
        static string QaJournal() => string.Join("|", Keys.Take(3).Select(key => PlayerPrefs.GetString(DevelopmentGameplayProfile.Prefix + key, "absent")));
        static Fingerprint Snapshot()
        {
            using (var hash = SHA256.Create()) return new Fingerprint { hashes = Keys.Select(key => PlayerPrefs.HasKey(key)
                ? Convert.ToBase64String(hash.ComputeHash(Encoding.UTF8.GetBytes(PlayerPrefs.GetString(key)))) : "absent").ToArray() };
        }
    }
}
