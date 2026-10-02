#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using LegendsOfRus.ContentPipeline;
using LegendsOfRus.SaveCore;

namespace LegendsOfRus
{
    // Editor-only, opt-in fixture. Owns only an in-memory journal; never imports a user's save.
    public sealed class CampaignActivePlaytestSetup
    {
        sealed class MemoryStore : ISaveJournalStore
        {
            readonly Dictionary<string, string> values = new Dictionary<string, string>();
            Dictionary<string, string> durable = new Dictionary<string, string>();
            public bool TryRead(string key, out string value) => values.TryGetValue(key, out value);
            public void Write(string key, string value) => values[key] = value;
            public void Delete(string key) => values.Remove(key);
            public void Flush() => durable = new Dictionary<string, string>(values);
            public void Reload() { values.Clear(); foreach (var pair in durable) values.Add(pair.Key, pair.Value); }
        }

        readonly MemoryStore store = new MemoryStore();
        readonly CampaignDefinition definition;
        readonly ValidatedCampaignCatalog catalog;
        public CampaignSaveSession Session { get; private set; }
        public int StartRound { get; }
        public int EndRound { get; }
        public string Fingerprint { get; }
        public const string Context = "CampaignActive; disposable earlier-progress fixture; 30 energy; zero starting boosters";

        static string Read(string name)
        {
            var asset = Resources.Load<TextAsset>(ActiveCampaignContent.ResourceRoot + name);
            if (!asset) throw new InvalidOperationException("Missing installed campaign resource: " + name);
            return asset.text;
        }

        public static int[] AvailableBlocks()
        {
            var levels = ActiveCampaignContent.Load().LoadChapter(1).levels;
            return levels.Where(l => (l.levelId - 1) % 10 == 0).Select(l => l.levelId).ToArray();
        }

        public static CampaignActivePlaytestSetup Create(int firstRound) => new CampaignActivePlaytestSetup(
            firstRound, Read("definition"), Read("levels"), Read("story"), Read("story_ru"), Read("story_en"));

        // Explicit inputs also permit validation tests without mutating installed resources.
        public CampaignActivePlaytestSetup(int firstRound, string definitionJson, string levelsJson,
            string storyJson, string russianJson, string englishJson)
        {
            definition = JsonUtility.FromJson<CampaignDefinition>(definitionJson);
            catalog = new ValidatedCampaignCatalog(definition);
            if (!catalog.TryAdd(new CampaignPackageInput { ChapterId = 1, ContentRevision = definition.ContentRevision,
                LevelsJson = levelsJson, StoryJson = storyJson, RussianJson = russianJson, EnglishJson = englishJson }, out var errors))
                throw new ArgumentException("Invalid installed campaign: " + string.Join("; ", errors));
            var levels = catalog.LoadChapter(1).levels;
            int last = levels.Max(l => l.levelId);
            if (firstRound < 1 || (firstRound - 1) % 10 != 0 || firstRound > last)
                throw new ArgumentOutOfRangeException(nameof(firstRound));
            StartRound = firstRound; EndRound = Math.Min(last, firstRound + 9);
            for (int n = 1; n <= EndRound; n++)
                if (!levels.Any(l => l.levelId == n)) throw new ArgumentException("Missing campaign round " + n);
            Fingerprint = "CampaignActive-" + CampaignPlaytestRecorder.Fingerprint(
                definitionJson + "\n" + levelsJson + "\n" + storyJson + "\n" + russianJson + "\n" + englishJson);

            var initial = new PlayerSaveData { energy = 30, energyMax = 30,
                boosterMace = 0, boosterFirebird = 0, boosterLivingLight = 0, boosterSeaPearl = 0,
                lastEnergyUtcTicks = DateTime.UtcNow.Ticks };
            var fixture = CampaignPlayerSaveCodec.FromLegacy(JsonUtility.ToJson(initial), definition);
            fixture.player.selectedChapter = 1; fixture.player.selectedLevel = firstRound;
            fixture.player.levelProgress = Enumerable.Range(1, firstRound - 1).Select(n => new LevelProgressData {
                chapterId = 1, levelId = n, attempts = 1, completions = 1, bestStars = 1, rewardClaimed = true
            }).ToList();
            CampaignPlayerSaveCodec.RefreshDerivedProgress(fixture.player);
            var journal = new SaveJournalRepository(store, new SaveEnvelopeCodec(), new PlayerSavePayloadCodec(),
                CampaignSaveSession.DevelopmentKeys(), CampaignPlayerSaveCodec.PayloadVersion);
            var written = journal.TrySave(CampaignPlayerSaveCodec.Encode(fixture, definition), CampaignPlayerSaveCodec.PayloadVersion);
            if (!written.Success) throw new InvalidOperationException(written.DiagnosticCode);
            Session = Reopen();
            // No simulated starts/wins, rewards or recorder events during fixture setup.
            // At round 1 retain the intro; later blocks acknowledge only prior fixture scenes.
            if (firstRound > 1)
            {
                while (true)
                {
                    if (!Session.TryGetPendingStory(out var pending, out var error)) throw new InvalidOperationException(error);
                    if (pending == null) break;
                    var acknowledged = Session.AcknowledgeStory(pending.Key);
                    if (!acknowledged.Committed) throw new InvalidOperationException(acknowledged.Diagnostic);
                }
            }
        }

        public CampaignSaveSession Reopen()
        {
            store.Reload();
            return Session = new CampaignSaveSession(store, definition, catalog);
        }

        public CampaignDevelopmentUI Open(RectTransform parent, Action onExit, string recordingDirectory = null, bool automated = false)
        {
            var ui = CampaignDevelopmentUI.Open(parent, Session, Reopen, onExit);
            CampaignPlaytestRecorder.Attach(ui, Session, Fingerprint, recordingDirectory, automated, EndRound, StartRound,
                Context, StartRound - 1);
            return ui;
        }
    }
}
#endif
