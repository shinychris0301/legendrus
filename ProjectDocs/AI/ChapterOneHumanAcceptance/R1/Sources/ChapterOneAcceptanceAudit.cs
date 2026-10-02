#if UNITY_EDITOR
using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace LegendsOfRus.Editor
{
    // Temporary read-only Editor audit. No scenes, normal saves or gameplay are opened.
    public static class ChapterOneAcceptanceAudit
    {
        [Serializable] public sealed class RoundRecord
        {
            public int round;
            public string fingerprint, canonicalJson;
        }
        [Serializable] public sealed class Report
        {
            public string editorVersion, applicationVersion, contentFingerprint, persistentDataPath;
            public int ruleVersion, validatedBlocks;
            public int[] availableBlocks;
            public RoundRecord[] rounds;
        }
        static void Require(bool condition, string message)
        {
            if (!condition) throw new InvalidOperationException(message);
        }
        public static void Run()
        {
            try
            {
                string output = Environment.GetEnvironmentVariable("LOR_ACCEPTANCE_AUDIT_OUTPUT");
                string config = Environment.GetEnvironmentVariable("LOR_LOADING_TEST_CONFIG_ROOT");
                Require(!string.IsNullOrEmpty(output) && !string.IsNullOrEmpty(config), "Explicit audit paths required");
                Require(Path.GetFullPath(Application.persistentDataPath).StartsWith(Path.GetFullPath(config) + Path.DirectorySeparatorChar), "Product storage must be isolated");
                var blocks = CampaignActivePlaytestSetup.AvailableBlocks();
                Require(blocks.SequenceEqual(Enumerable.Range(0, 50).Select(i => 1 + i * 10)), "Expected all 50 installed blocks");
                string fingerprint = null;
                foreach (int start in blocks)
                {
                    var fixture = CampaignActivePlaytestSetup.Create(start);
                    if (fingerprint == null) fingerprint = fixture.Fingerprint;
                    Require(fixture.Fingerprint == fingerprint, "All fixtures must use the same installed content");
                    Require(fixture.EndRound == start + 9, "Exact ten-round block required");
                    Require(fixture.Session.TryLoad(out var save, out _), "Fixture save must load");
                    Require(save.player.energy == 30 && save.player.energyMax == 30, "Thirty fixture energy required");
                    Require(save.player.boosterMace == 0 && save.player.boosterFirebird == 0 && save.player.boosterLivingLight == 0 && save.player.boosterSeaPearl == 0, "Zero starting inventory boosters required");
                    Require(save.player.levelProgress.Count == start - 1 && save.runSequence == 0, "Earlier progress is fixture data, never observed runs");
                    Require(fixture.Session.ResolveSelectedRound().levelId == start, "Fixture selection mismatch");
                    Require(fixture.Reopen().TryLoad(out var reopened, out _), "In-memory fixture must reopen");
                    Require(JsonUtility.ToJson(save) == JsonUtility.ToJson(reopened), "Reopened fixture changed");
                }
                var levels = ActiveCampaignContent.Load().LoadChapter(1).levels.OrderBy(l => l.levelId).ToArray();
                Require(levels.Select(l => l.levelId).SequenceEqual(Enumerable.Range(1, 500)), "Installed rounds must be continuous 1–500");
                var report = new Report
                {
                    editorVersion = Application.unityVersion, applicationVersion = Application.version,
                    contentFingerprint = fingerprint, persistentDataPath = Application.persistentDataPath,
                    ruleVersion = (int)Match3Board.CurrentGameplayRules,
                    validatedBlocks = blocks.Length, availableBlocks = blocks,
                    rounds = levels.Select(l => new RoundRecord
                    {
                        round = l.levelId, canonicalJson = JsonUtility.ToJson(l),
                        fingerprint = CampaignPlaytestRecorder.Fingerprint(JsonUtility.ToJson(l))
                    }).ToArray()
                };
                Directory.CreateDirectory(Path.GetDirectoryName(output));
                File.WriteAllText(output, JsonUtility.ToJson(report, true));
                Debug.Log("Chapter One acceptance audit: 50 exact fixtures and 500 native round fingerprints exported; no measured play.");
                EditorApplication.Exit(0);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                EditorApplication.Exit(1);
            }
        }
    }
}
#endif
