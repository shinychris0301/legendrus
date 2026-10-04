using System;
using System.Globalization;
using LegendsOfRus.SaveCore;
using UnityEngine;

namespace LegendsOfRus
{
    internal sealed class PlayerSavePayloadCodec : ILegacySavePayloadInspector
    {
        internal const int MaximumPayloadCharacters =
            SaveEnvelopeCodec.MaximumPayloadCharacters;

        enum PlayerSaveField
        {
            SaveVersion,
            Gold,
            Gems,
            Energy,
            EnergyMax,
            LastEnergyUtcTicks,
            Chapter,
            Level,
            Stars,
            StoryStage,
            SelectedChapter,
            SelectedLevel,
            HighestUnlockedLevel,
            LastCompletedChapter,
            LastCompletedLevel,
            LastRewardGold,
            LastRewardGems,
            LastRewardEligibleForDouble,
            LastRewardDoubled,
            LevelProgress,
            CompletedTutorialSteps,
            CompletedStoryScenes,
            AcknowledgedChapterCompletions,
            BoosterMace,
            BoosterFirebird,
            BoosterLivingLight,
            BoosterSeaPearl,
            LastDailyRewardUtc,
            Music,
            Sfx,
            Vibration,
            Notifications
        }

        const ulong CurrentV6RequiredFields =
            (1UL << ((int)PlayerSaveField.Notifications + 1)) - 1UL;

        public LegacyPayloadInspection Inspect(string serializedPayload)
        {
            int declaredVersion;
            string code;
            if (!TryInspectVersion(
                    serializedPayload,
                    true,
                    out declaredVersion,
                    out code))
            {
                return LegacyPayloadInspection.Invalid();
            }

            // Future-version evidence must remain authoritative even when its
            // schema is unknown to this client. The journal will classify it
            // as unsupported and block both backup downgrade and writes.
            if (declaredVersion
                > PlayerSaveMigrationRegistry.CurrentPayloadVersion)
            {
                return LegacyPayloadInspection.Valid(declaredVersion);
            }

            PlayerSaveData data;
            if (!TryDeserialize(
                    serializedPayload,
                    0,
                    true,
                    out data,
                    out code)
                || data == null)
            {
                return LegacyPayloadInspection.Invalid();
            }
            return LegacyPayloadInspection.Valid(data.saveVersion);
        }

        internal static bool TryDeserialize(
            string serializedPayload,
            int expectedVersion,
            bool allowMissingVersion,
            out PlayerSaveData data,
            out string diagnosticCode)
        {
            data = null;
            int declaredVersion;
            if (!TryInspectVersion(
                    serializedPayload,
                    allowMissingVersion,
                    out declaredVersion,
                    out diagnosticCode))
            {
                return false;
            }

            if (expectedVersion > 0 && declaredVersion != expectedVersion)
            {
                diagnosticCode = "SAVE_PAYLOAD_VERSION_MISMATCH";
                return false;
            }

            try
            {
                data = JsonUtility.FromJson<PlayerSaveData>(serializedPayload);
            }
            catch (ArgumentException)
            {
                diagnosticCode = "SAVE_PAYLOAD_JSON_INVALID";
                return false;
            }
            catch (Exception)
            {
                diagnosticCode = "SAVE_PAYLOAD_DESERIALIZATION_FAILED";
                return false;
            }

            if (data == null)
            {
                diagnosticCode = "SAVE_PAYLOAD_DESERIALIZED_NULL";
                return false;
            }

            // An absent extension keeps legacy v6 saves valid; malformed receipts cannot grant stock.
            if (data.boosterCommand != null && !data.boosterCommand.IsValid)
            {
                data = null;
                diagnosticCode = "SAVE_BOOSTER_COMMAND_INVALID";
                return false;
            }

            // JsonUtility may retain field initializers for an omitted field.
            // The inspected legacy version is authoritative.
            data.saveVersion = declaredVersion;
            diagnosticCode = "SAVE_PAYLOAD_VALID";
            return true;
        }

        internal static bool TryInspectVersion(
            string serializedPayload,
            bool allowMissingVersion,
            out int version,
            out string diagnosticCode)
        {
            version = 0;
            if (string.IsNullOrWhiteSpace(serializedPayload))
            {
                diagnosticCode = "SAVE_PAYLOAD_MISSING";
                return false;
            }
            if (serializedPayload.Length > MaximumPayloadCharacters)
            {
                diagnosticCode = "SAVE_PAYLOAD_TOO_LARGE";
                return false;
            }

            string trimmed = serializedPayload.Trim();
            if (trimmed.Length < 2
                || trimmed[0] != '{'
                || trimmed[trimmed.Length - 1] != '}'
                || trimmed.IndexOf('\0') >= 0
                || !HasBalancedJsonShape(trimmed))
            {
                diagnosticCode = "SAVE_PAYLOAD_JSON_SHAPE_INVALID";
                return false;
            }
            bool recognized;
            bool validVersionSyntax;
            int versionCount;
            ulong fieldMask;
            ulong duplicateFieldMask;
            bool fieldTypesValid;
            if (!TryReadTopLevelFields(
                    trimmed,
                    out recognized,
                    out validVersionSyntax,
                    out versionCount,
                    out version,
                    out fieldMask,
                    out duplicateFieldMask,
                    out fieldTypesValid))
            {
                diagnosticCode = "SAVE_PAYLOAD_JSON_SHAPE_INVALID";
                return false;
            }
            if (!recognized)
            {
                diagnosticCode = "SAVE_PAYLOAD_NOT_RECOGNIZED";
                return false;
            }

            if (versionCount == 0)
            {
                if (!allowMissingVersion)
                {
                    diagnosticCode = "SAVE_PAYLOAD_VERSION_MISSING";
                    return false;
                }
                version = 1;
                diagnosticCode = "SAVE_PAYLOAD_LEGACY_UNVERSIONED";
                return true;
            }
            if (versionCount != 1
                || !validVersionSyntax
                || version <= 0)
            {
                diagnosticCode = "SAVE_PAYLOAD_VERSION_INVALID";
                return false;
            }

            // JsonUtility fills omitted members from field initializers. A
            // syntactically valid fragment such as {"saveVersion":6} would
            // therefore look like a complete current save after deserialize.
            // Require the complete v6 DTO at the raw-import boundary so an
            // incomplete primary cannot outrank a valid committed backup.
            // Earlier payload schemas remain intentionally permissive because
            // their historical serialized members differ by version.
            if (version == PlayerSaveMigrationRegistry.CurrentPayloadVersion)
            {
                if (duplicateFieldMask != 0 || !fieldTypesValid)
                {
                    diagnosticCode = "SAVE_PAYLOAD_SCHEMA_INVALID";
                    return false;
                }
                if ((fieldMask & CurrentV6RequiredFields)
                    != CurrentV6RequiredFields)
                {
                    diagnosticCode = "SAVE_PAYLOAD_SCHEMA_INCOMPLETE";
                    return false;
                }
            }

            diagnosticCode = "SAVE_PAYLOAD_VERSION_VALID";
            return true;
        }

        static bool TryReadTopLevelFields(
            string json,
            out bool recognized,
            out bool validVersionSyntax,
            out int versionCount,
            out int version,
            out ulong fieldMask,
            out ulong duplicateFieldMask,
            out bool fieldTypesValid)
        {
            recognized = false;
            validVersionSyntax = true;
            versionCount = 0;
            version = 0;
            fieldMask = 0;
            duplicateFieldMask = 0;
            fieldTypesValid = true;
            int objectDepth = 0;
            int arrayDepth = 0;

            for (int i = 0; i < json.Length; i++)
            {
                char value = json[i];
                if (value == '{')
                {
                    objectDepth++;
                    continue;
                }
                if (value == '}')
                {
                    objectDepth--;
                    if (objectDepth < 0)
                        return false;
                    continue;
                }
                if (value == '[')
                {
                    arrayDepth++;
                    continue;
                }
                if (value == ']')
                {
                    arrayDepth--;
                    if (arrayDepth < 0)
                        return false;
                    continue;
                }
                if (value != '"')
                    continue;

                int start = i + 1;
                bool escaped = false;
                bool containsEscape = false;
                i++;
                for (; i < json.Length; i++)
                {
                    char text = json[i];
                    if (escaped)
                    {
                        escaped = false;
                        containsEscape = true;
                        continue;
                    }
                    if (text == '\\')
                    {
                        escaped = true;
                        continue;
                    }
                    if (text == '"')
                        break;
                    if (text < ' ')
                        return false;
                }
                if (i >= json.Length || escaped)
                    return false;
                if (objectDepth != 1 || arrayDepth != 0 || containsEscape)
                    continue;

                int afterKey = i + 1;
                while (afterKey < json.Length
                    && char.IsWhiteSpace(json[afterKey]))
                {
                    afterKey++;
                }
                if (afterKey >= json.Length || json[afterKey] != ':')
                    continue;

                string key = json.Substring(start, i - start);
                PlayerSaveField field;
                if (!TryGetField(key, out field))
                    continue;

                if (IsLegacyRecognitionAnchor(field))
                    recognized = true;
                ulong bit = 1UL << (int)field;
                if ((fieldMask & bit) != 0)
                    duplicateFieldMask |= bit;
                fieldMask |= bit;

                int valueStart = afterKey + 1;
                while (valueStart < json.Length
                    && char.IsWhiteSpace(json[valueStart]))
                {
                    valueStart++;
                }
                if (!HasExpectedValueStart(field, json, valueStart))
                    fieldTypesValid = false;

                if (field == PlayerSaveField.SaveVersion)
                {
                    recognized = true;
                    versionCount++;
                    int numberStart = valueStart;
                    int numberEnd = numberStart;
                    if (numberEnd < json.Length && json[numberEnd] == '-')
                        numberEnd++;
                    int digitStart = numberEnd;
                    while (numberEnd < json.Length
                        && json[numberEnd] >= '0'
                        && json[numberEnd] <= '9')
                    {
                        numberEnd++;
                    }
                    if (digitStart == numberEnd
                        || (numberEnd < json.Length
                            && !char.IsWhiteSpace(json[numberEnd])
                            && json[numberEnd] != ','
                            && json[numberEnd] != '}'))
                    {
                        validVersionSyntax = false;
                        continue;
                    }

                    int parsed;
                    if (!int.TryParse(
                            json.Substring(
                                numberStart,
                                numberEnd - numberStart),
                            NumberStyles.AllowLeadingSign,
                            CultureInfo.InvariantCulture,
                            out parsed))
                    {
                        validVersionSyntax = false;
                        continue;
                    }
                    version = parsed;
                }
            }

            return objectDepth == 0 && arrayDepth == 0;
        }

        static bool HasExpectedValueStart(
            PlayerSaveField field,
            string json,
            int valueStart)
        {
            if (valueStart >= json.Length)
                return false;

            char value = json[valueStart];
            switch (field)
            {
                case PlayerSaveField.LastRewardEligibleForDouble:
                case PlayerSaveField.LastRewardDoubled:
                case PlayerSaveField.Music:
                case PlayerSaveField.Sfx:
                case PlayerSaveField.Vibration:
                case PlayerSaveField.Notifications:
                    return value == 't' || value == 'f';

                case PlayerSaveField.LevelProgress:
                case PlayerSaveField.CompletedTutorialSteps:
                case PlayerSaveField.CompletedStoryScenes:
                case PlayerSaveField.AcknowledgedChapterCompletions:
                    return value == '[';

                case PlayerSaveField.LastDailyRewardUtc:
                    return value == '"';

                default:
                    return value == '-'
                        || (value >= '0' && value <= '9');
            }
        }

        static bool IsLegacyRecognitionAnchor(PlayerSaveField field)
        {
            switch (field)
            {
                case PlayerSaveField.SaveVersion:
                case PlayerSaveField.Gold:
                case PlayerSaveField.Gems:
                case PlayerSaveField.Energy:
                case PlayerSaveField.Chapter:
                case PlayerSaveField.Level:
                case PlayerSaveField.Stars:
                case PlayerSaveField.StoryStage:
                    return true;
                default:
                    return false;
            }
        }

        static bool TryGetField(string key, out PlayerSaveField field)
        {
            switch (key)
            {
                case "saveVersion":
                    field = PlayerSaveField.SaveVersion;
                    return true;
                case "gold":
                    field = PlayerSaveField.Gold;
                    return true;
                case "gems":
                    field = PlayerSaveField.Gems;
                    return true;
                case "energy":
                    field = PlayerSaveField.Energy;
                    return true;
                case "energyMax":
                    field = PlayerSaveField.EnergyMax;
                    return true;
                case "lastEnergyUtcTicks":
                    field = PlayerSaveField.LastEnergyUtcTicks;
                    return true;
                case "chapter":
                    field = PlayerSaveField.Chapter;
                    return true;
                case "level":
                    field = PlayerSaveField.Level;
                    return true;
                case "stars":
                    field = PlayerSaveField.Stars;
                    return true;
                case "storyStage":
                    field = PlayerSaveField.StoryStage;
                    return true;
                case "selectedChapter":
                    field = PlayerSaveField.SelectedChapter;
                    return true;
                case "selectedLevel":
                    field = PlayerSaveField.SelectedLevel;
                    return true;
                case "highestUnlockedLevel":
                    field = PlayerSaveField.HighestUnlockedLevel;
                    return true;
                case "lastCompletedChapter":
                    field = PlayerSaveField.LastCompletedChapter;
                    return true;
                case "lastCompletedLevel":
                    field = PlayerSaveField.LastCompletedLevel;
                    return true;
                case "lastRewardGold":
                    field = PlayerSaveField.LastRewardGold;
                    return true;
                case "lastRewardGems":
                    field = PlayerSaveField.LastRewardGems;
                    return true;
                case "lastRewardEligibleForDouble":
                    field = PlayerSaveField.LastRewardEligibleForDouble;
                    return true;
                case "lastRewardDoubled":
                    field = PlayerSaveField.LastRewardDoubled;
                    return true;
                case "levelProgress":
                    field = PlayerSaveField.LevelProgress;
                    return true;
                case "completedTutorialSteps":
                    field = PlayerSaveField.CompletedTutorialSteps;
                    return true;
                case "completedStoryScenes":
                    field = PlayerSaveField.CompletedStoryScenes;
                    return true;
                case "acknowledgedChapterCompletions":
                    field = PlayerSaveField.AcknowledgedChapterCompletions;
                    return true;
                case "boosterMace":
                    field = PlayerSaveField.BoosterMace;
                    return true;
                case "boosterFirebird":
                    field = PlayerSaveField.BoosterFirebird;
                    return true;
                case "boosterLivingLight":
                    field = PlayerSaveField.BoosterLivingLight;
                    return true;
                case "boosterSeaPearl":
                    field = PlayerSaveField.BoosterSeaPearl;
                    return true;
                case "lastDailyRewardUtc":
                    field = PlayerSaveField.LastDailyRewardUtc;
                    return true;
                case "music":
                    field = PlayerSaveField.Music;
                    return true;
                case "sfx":
                    field = PlayerSaveField.Sfx;
                    return true;
                case "vibration":
                    field = PlayerSaveField.Vibration;
                    return true;
                case "notifications":
                    field = PlayerSaveField.Notifications;
                    return true;
                default:
                    field = PlayerSaveField.SaveVersion;
                    return false;
            }
        }

        static bool HasBalancedJsonShape(string json)
        {
            int objectDepth = 0;
            int arrayDepth = 0;
            bool inString = false;
            bool escaped = false;

            for (int i = 0; i < json.Length; i++)
            {
                char value = json[i];
                if (inString)
                {
                    if (escaped)
                    {
                        escaped = false;
                        continue;
                    }
                    if (value == '\\')
                    {
                        escaped = true;
                        continue;
                    }
                    if (value == '"')
                        inString = false;
                    else if (value < ' ')
                        return false;
                    continue;
                }

                if (value == '"')
                    inString = true;
                else if (value == '{')
                    objectDepth++;
                else if (value == '}')
                {
                    objectDepth--;
                    if (objectDepth < 0)
                        return false;
                }
                else if (value == '[')
                    arrayDepth++;
                else if (value == ']')
                {
                    arrayDepth--;
                    if (arrayDepth < 0)
                        return false;
                }
            }

            return !inString
                && !escaped
                && objectDepth == 0
                && arrayDepth == 0;
        }
    }
}
