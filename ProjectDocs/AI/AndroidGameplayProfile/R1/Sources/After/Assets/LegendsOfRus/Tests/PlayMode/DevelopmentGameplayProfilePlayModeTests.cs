using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using NUnit.Framework;
using UnityEngine;

namespace LegendsOfRus.Tests.PlayMode
{
    public sealed class DevelopmentGameplayProfilePlayModeTests
    {
        static readonly string[] NormalKeys = {
            "LegendsOfRus_Save_v1", "LegendsOfRus_Save_v1_Backup", "LegendsOfRus_Save_v1_Staging",
            LanguagePreference.Key, "LegendsOfRus_Compliance_v1"
        };
        [Serializable] sealed class Fingerprint { public string[] hashes; }

        [Test]
        public void ColdProcessProfileIsolationAndPersistence()
        {
            string phase = Environment.GetEnvironmentVariable("LOR_GAMEPLAY_QA_PHASE");
            if (string.IsNullOrEmpty(phase)) Assert.Ignore("Requires the isolated gameplay-profile phase runner.");
            string config = Environment.GetEnvironmentVariable("LOR_LOADING_TEST_CONFIG_ROOT");
            Assert.That(config, Does.StartWith("/tmp/lor-gameplay-profile-r1/"));
            Assert.That(Environment.GetEnvironmentVariable("XDG_CONFIG_HOME"), Is.EqualTo(config));
            Assert.That(Path.GetFullPath(Application.persistentDataPath), Does.StartWith(config + "/"));
            string checkpoint = Path.Combine(config, "normal-key-fingerprints.json");
            var state = GameState.Instance;
            Assert.That(state, Is.Not.Null);
            Assert.That(state.CanPersist, Is.True);

            if (phase == "normal-write")
            {
                Assert.That(DevelopmentGameplayProfile.Active, Is.False);
                state.ResetProgress();
                state.Save.gold = 13731;
                state.Save.gems = 217;
                state.Save.music = false;
                state.Persist();
                LanguagePreference.Save("en");
                typeof(PrivacySupportService).GetMethod("ClearLocalComplianceState")
                    .Invoke(PrivacySupportService.Instance, null);
                PlayerPrefs.Save();
                File.WriteAllText(checkpoint, JsonUtility.ToJson(Snapshot()));
                Assert.That(DevelopmentGameplayProfile.PreferenceKey(NormalKeys[0]), Is.EqualTo(NormalKeys[0]));
                return;
            }

            Assert.That(File.Exists(checkpoint), Is.True, "Run normal-write first in the same isolated product store.");
            var before = JsonUtility.FromJson<Fingerprint>(File.ReadAllText(checkpoint));
            Assert.That(Snapshot().hashes, Is.EqualTo(before.hashes), "Normal keys changed before this phase.");
            if (phase == "normal-read")
            {
                Assert.That(DevelopmentGameplayProfile.Active, Is.False);
                Assert.That(state.Save.gold, Is.EqualTo(13731));
                Assert.That(state.Save.gems, Is.EqualTo(217));
                Assert.That(state.Save.music, Is.False);
                Assert.That(LanguagePreference.Load(), Is.EqualTo("en"));
                return;
            }

            Assert.That(DevelopmentGameplayProfile.Active, Is.True);
            Assert.That(DevelopmentGameplayProfile.PreferenceKey(NormalKeys[0]),
                Is.EqualTo(DevelopmentGameplayProfile.Prefix + NormalKeys[0]));
            if (phase == "write")
            {
                AssertFresh(state, 5);
                Assert.That(state.SpendEnergy(), Is.True);
                Assert.That(state.TryUseBooster(BoosterType.BogatyrMace), Is.True);
                var level = ChapterLevelCatalog.Find(1, 1);
                var first = state.CompleteLevel(level, 10000);
                Assert.That(first.FirstClearRewardGranted, Is.True);
                var replay = state.CompleteLevel(level, 10000);
                Assert.That(replay.FirstClearRewardGranted, Is.False);
                Assert.That(state.Save.gold, Is.EqualTo(12540 + level.rewards.gold));
                state.ToggleMusic();
                LanguagePreference.Save("en");
                state.Persist();
                Assert.That(SaveService.Load().gold, Is.EqualTo(state.Save.gold));
            }
            else if (phase == "resume")
            {
                var level = ChapterLevelCatalog.Find(1, 1);
                Assert.That(state.Save.energy, Is.EqualTo(4));
                Assert.That(state.Save.boosterMace, Is.EqualTo(6));
                Assert.That(state.Save.gold, Is.EqualTo(12540 + level.rewards.gold));
                Assert.That(state.Save.music, Is.False);
                Assert.That(LanguagePreference.Load(), Is.EqualTo("en"));
                Assert.That(state.CompleteLevel(level, 10000).FirstClearRewardGranted, Is.False);
            }
            else if (phase == "corrupt")
            {
                string primary = DevelopmentGameplayProfile.PreferenceKey(NormalKeys[0]);
                string backup = DevelopmentGameplayProfile.PreferenceKey(NormalKeys[1]);
                string p = PlayerPrefs.GetString(primary), b = PlayerPrefs.GetString(backup);
                try
                {
                    PlayerPrefs.SetString(primary, "broken QA primary");
                    PlayerPrefs.SetString(backup, "broken QA backup");
                    var loaded = SaveService.LoadWithResult();
                    Assert.That(loaded.CanPersist, Is.False);
                    Assert.That(loaded.Status, Is.EqualTo(PlayerSaveLoadStatus.InvalidPayload));
                    Assert.That(loaded.Data.gold, Is.Not.EqualTo(13731), "Do not fall back to a normal save.");
                    Assert.That(SaveService.TrySave(new PlayerSaveData(), out _), Is.False);
                }
                finally
                {
                    PlayerPrefs.SetString(primary, p);
                    PlayerPrefs.SetString(backup, b);
                    PlayerPrefs.Save();
                }
            }
            else if (phase == "reset") AssertFresh(state, 5);
            else if (phase == "empty-energy")
            {
                AssertFresh(state, 0);
                Assert.That(state.SpendEnergy(), Is.False);
                Assert.That(state.Save.energy, Is.Zero);
            }
            else Assert.Fail("Unknown verification phase");

            // Exercise the actual compliance store's reset without granting optional consent.
            typeof(PrivacySupportService).GetMethod("ClearLocalComplianceState")
                .Invoke(PrivacySupportService.Instance, null);
            var account = BackendAccountService.Instance;
            Assert.That(account, Is.Not.Null);
            Assert.That(account.TryConfigureAndroid("https://example.invalid"), Is.False);
            Assert.Throws<InvalidOperationException>(() => account.Configure(null, null));
            Assert.That(account.IsConfigured, Is.False);
            Assert.That(MonetizationService.Instance.TryPurchase("starter").ReasonCode, Is.EqualTo("GAMEPLAY_QA_OFFLINE"));
            Assert.That(MonetizationService.Instance.TryBindRuntimePort(new DisabledRuntimeMonetizationPort()), Is.False);
            Assert.That(Snapshot().hashes, Is.EqualTo(before.hashes), "QA actions/reset touched normal player keys.");
            // Profile is process-locked even if an Editor launch variable changes after startup.
            string command = Environment.GetEnvironmentVariable("LOR_GAMEPLAY_QA");
            try
            {
                Environment.SetEnvironmentVariable("LOR_GAMEPLAY_QA", null);
                Assert.That(DevelopmentGameplayProfile.Active, Is.True);
            }
            finally { Environment.SetEnvironmentVariable("LOR_GAMEPLAY_QA", command); }
        }

        static void AssertFresh(GameState state, int energy)
        {
            Assert.That(state.Save.gold, Is.EqualTo(12540));
            Assert.That(state.Save.gems, Is.EqualTo(350));
            Assert.That(state.Save.energy, Is.EqualTo(energy));
            Assert.That(state.Save.boosterMace, Is.EqualTo(7));
            Assert.That(state.Save.music, Is.True);
            Assert.That(state.Save.highestUnlockedLevel, Is.EqualTo(1));
            Assert.That(state.Save.levelProgress, Is.Empty);
            Assert.That(LanguagePreference.Load(), Is.EqualTo("ru"));
        }

        static Fingerprint Snapshot()
        {
            var result = new Fingerprint { hashes = new string[NormalKeys.Length] };
            using (var hash = SHA256.Create())
                for (int i = 0; i < NormalKeys.Length; i++)
                    result.hashes[i] = PlayerPrefs.HasKey(NormalKeys[i])
                        ? Convert.ToBase64String(hash.ComputeHash(Encoding.UTF8.GetBytes(PlayerPrefs.GetString(NormalKeys[i]))))
                        : "absent";
            return result;
        }
    }
}
