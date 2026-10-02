using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace LegendsOfRus.CommercialCore.Analytics
{
    public static class AnalyticsEventIds
    {
        public const string SessionStart = "session_start";
        public const string ScreenView = "screen_view";
        public const string LevelStart = "level_start";
        public const string LevelComplete = "level_complete";
        public const string LevelFail = "level_fail";
        public const string EnergyGateBlocked = "energy_gate_blocked";
        public const string TutorialStepView = "tutorial_step_view";
        public const string TutorialStepComplete = "tutorial_step_complete";
        public const string StorySceneStart = "story_scene_start";
        public const string StorySceneComplete = "story_scene_complete";
        public const string ChapterComplete = "chapter_complete";
        public const string ChapterCompleteAcknowledged =
            "chapter_complete_acknowledged";
        public const string SandboxEntitlementGrant = "sandbox_entitlement_grant";
        public const string PurchaseOfferView = "purchase_offer_view";
        public const string PurchaseStart = "purchase_start";
        public const string PurchaseResult = "purchase_result";
        public const string RewardedAdOfferView = "rewarded_ad_offer_view";
        public const string RewardedAdStart = "rewarded_ad_start";
        public const string RewardedAdResult = "rewarded_ad_result";
    }

    public static class AnalyticsAttributeKeys
    {
        public const string ScreenId = "screen_id";
        public const string LevelId = "level_id";
        public const string AttemptNumber = "attempt_number";
        public const string IsRetry = "is_retry";
        public const string Score = "score";
        public const string Stars = "stars";
        public const string FirstClear = "first_clear";
        public const string RequiredEnergy = "required_energy";
        public const string AvailableEnergy = "available_energy";
        public const string TutorialId = "tutorial_id";
        public const string TriggerId = "trigger_id";
        public const string SceneId = "scene_id";
        public const string ChapterId = "chapter_id";
        public const string SourceId = "source_id";
        public const string EntitlementId = "entitlement_id";
        public const string ProductId = "product_id";
        public const string PlacementId = "placement_id";
        public const string OutcomeId = "outcome_id";
        public const string RewardId = "reward_id";
        public const string RewardGranted = "reward_granted";
    }

    public sealed class AnalyticsAttributeSchema
    {
        readonly string[] allowedStringValues;
        readonly ReadOnlyCollection<string> readOnlyAllowedStringValues;

        public string Key { get; }
        public AnalyticsValueKind Kind { get; }
        public bool Required { get; }
        public int MaximumStringLength { get; }
        public long MinimumInteger { get; }
        public long MaximumInteger { get; }
        public IReadOnlyList<string> AllowedStringValues =>
            readOnlyAllowedStringValues;

        internal AnalyticsAttributeSchema(
            string key,
            AnalyticsValueKind kind,
            bool required,
            int maximumStringLength,
            long minimumInteger,
            long maximumInteger,
            string[] allowedStringValues
        )
        {
            Key = key;
            Kind = kind;
            Required = required;
            MaximumStringLength = maximumStringLength;
            MinimumInteger = minimumInteger;
            MaximumInteger = maximumInteger;
            this.allowedStringValues = allowedStringValues == null
                ? new string[0]
                : (string[])allowedStringValues.Clone();
            readOnlyAllowedStringValues = Array.AsReadOnly(this.allowedStringValues);
        }

        internal bool IsAllowedStringValue(string value)
        {
            if (allowedStringValues.Length == 0)
                return true;

            for (int i = 0; i < allowedStringValues.Length; i++)
            {
                if (string.Equals(
                    allowedStringValues[i],
                    value,
                    StringComparison.Ordinal
                ))
                {
                    return true;
                }
            }

            return false;
        }
    }

    public sealed class AnalyticsEventSchema
    {
        readonly AnalyticsAttributeSchema[] attributes;
        readonly ReadOnlyCollection<AnalyticsAttributeSchema> readOnlyAttributes;

        public string EventId { get; }
        public IReadOnlyList<AnalyticsAttributeSchema> Attributes =>
            readOnlyAttributes;

        internal AnalyticsEventSchema(
            string eventId,
            params AnalyticsAttributeSchema[] attributes
        )
        {
            EventId = eventId;
            this.attributes = attributes == null
                ? new AnalyticsAttributeSchema[0]
                : (AnalyticsAttributeSchema[])attributes.Clone();
            readOnlyAttributes = Array.AsReadOnly(this.attributes);
        }

        public bool TryGetAttribute(
            string key,
            out AnalyticsAttributeSchema attribute
        )
        {
            for (int i = 0; i < attributes.Length; i++)
            {
                if (string.Equals(attributes[i].Key, key, StringComparison.Ordinal))
                {
                    attribute = attributes[i];
                    return true;
                }
            }

            attribute = null;
            return false;
        }
    }

    /// <summary>
    /// Compiled schema v1. Data and remote configuration cannot silently add
    /// event names or free-form fields at runtime.
    /// </summary>
    public static class AnalyticsSchemaRegistry
    {
        public const int CurrentSchemaVersion = 1;
        public const int MaximumEventIdLength = 64;
        public const int MaximumAttributeKeyLength = 48;
        public const int MaximumAttributesPerEvent = 16;
        public const int MaximumSessionIdLength = 64;

        static readonly string[] ScreenIds =
        {
            "loading",
            "home",
            "world_map",
            "chapter_map",
            "pre_level",
            "gameplay",
            "dialogue",
            "victory",
            "fail",
            "shop",
            "chapter_15",
            "daily_reward",
            "reward_claim",
            "booster_select",
            "events",
            "profile",
            "settings",
            "pause",
            "characters",
            "mail",
            "out_of_energy",
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
            "empty_error_state"
        };

        static readonly string[] PurchaseOutcomes =
        {
            "verified",
            "cancelled",
            "unavailable",
            "rejected",
            "failed"
        };

        static readonly string[] RewardedAdOutcomes =
        {
            "completed",
            "skipped",
            "unavailable",
            "failed"
        };

        static readonly AnalyticsEventSchema[] Schemas =
        {
            Event(AnalyticsEventIds.SessionStart),
            Event(
                AnalyticsEventIds.ScreenView,
                Id(AnalyticsAttributeKeys.ScreenId, 32, ScreenIds)
            ),
            Event(
                AnalyticsEventIds.LevelStart,
                Id(AnalyticsAttributeKeys.LevelId),
                Integer(AnalyticsAttributeKeys.AttemptNumber, 1L, int.MaxValue),
                Boolean(AnalyticsAttributeKeys.IsRetry)
            ),
            Event(
                AnalyticsEventIds.LevelComplete,
                Id(AnalyticsAttributeKeys.LevelId),
                Integer(AnalyticsAttributeKeys.AttemptNumber, 1L, int.MaxValue),
                Integer(AnalyticsAttributeKeys.Score, 0L, int.MaxValue),
                Integer(AnalyticsAttributeKeys.Stars, 1L, 3L),
                Boolean(AnalyticsAttributeKeys.FirstClear)
            ),
            Event(
                AnalyticsEventIds.LevelFail,
                Id(AnalyticsAttributeKeys.LevelId),
                Integer(AnalyticsAttributeKeys.AttemptNumber, 1L, int.MaxValue),
                Integer(AnalyticsAttributeKeys.Score, 0L, int.MaxValue)
            ),
            Event(
                AnalyticsEventIds.EnergyGateBlocked,
                Id(AnalyticsAttributeKeys.LevelId),
                Integer(AnalyticsAttributeKeys.RequiredEnergy, 1L, int.MaxValue),
                Integer(AnalyticsAttributeKeys.AvailableEnergy, 0L, int.MaxValue)
            ),
            Event(
                AnalyticsEventIds.TutorialStepView,
                Id(AnalyticsAttributeKeys.LevelId),
                Id(AnalyticsAttributeKeys.TutorialId),
                Id(AnalyticsAttributeKeys.TriggerId)
            ),
            Event(
                AnalyticsEventIds.TutorialStepComplete,
                Id(AnalyticsAttributeKeys.LevelId),
                Id(AnalyticsAttributeKeys.TutorialId)
            ),
            Event(
                AnalyticsEventIds.StorySceneStart,
                Id(AnalyticsAttributeKeys.SceneId)
            ),
            Event(
                AnalyticsEventIds.StorySceneComplete,
                Id(AnalyticsAttributeKeys.SceneId)
            ),
            Event(
                AnalyticsEventIds.ChapterComplete,
                Integer(AnalyticsAttributeKeys.ChapterId, 1L, 20L)
            ),
            Event(
                AnalyticsEventIds.ChapterCompleteAcknowledged,
                Integer(AnalyticsAttributeKeys.ChapterId, 1L, 20L)
            ),
            Event(
                AnalyticsEventIds.SandboxEntitlementGrant,
                Id(AnalyticsAttributeKeys.SourceId),
                Id(AnalyticsAttributeKeys.EntitlementId)
            ),
            Event(
                AnalyticsEventIds.PurchaseOfferView,
                Id(AnalyticsAttributeKeys.ProductId),
                Id(AnalyticsAttributeKeys.PlacementId)
            ),
            Event(
                AnalyticsEventIds.PurchaseStart,
                Id(AnalyticsAttributeKeys.ProductId),
                Id(AnalyticsAttributeKeys.PlacementId)
            ),
            Event(
                AnalyticsEventIds.PurchaseResult,
                Id(AnalyticsAttributeKeys.ProductId),
                Id(AnalyticsAttributeKeys.PlacementId),
                Id(AnalyticsAttributeKeys.OutcomeId, 32, PurchaseOutcomes)
            ),
            Event(
                AnalyticsEventIds.RewardedAdOfferView,
                Id(AnalyticsAttributeKeys.PlacementId),
                Id(AnalyticsAttributeKeys.RewardId)
            ),
            Event(
                AnalyticsEventIds.RewardedAdStart,
                Id(AnalyticsAttributeKeys.PlacementId),
                Id(AnalyticsAttributeKeys.RewardId)
            ),
            Event(
                AnalyticsEventIds.RewardedAdResult,
                Id(AnalyticsAttributeKeys.PlacementId),
                Id(AnalyticsAttributeKeys.RewardId),
                Id(AnalyticsAttributeKeys.OutcomeId, 32, RewardedAdOutcomes),
                Boolean(AnalyticsAttributeKeys.RewardGranted)
            )
        };

        static readonly ReadOnlyCollection<AnalyticsEventSchema> ReadOnlySchemas =
            Array.AsReadOnly(Schemas);

        public static IReadOnlyList<AnalyticsEventSchema> All => ReadOnlySchemas;

        public static bool TryGet(string eventId, out AnalyticsEventSchema schema)
        {
            for (int i = 0; i < Schemas.Length; i++)
            {
                if (string.Equals(Schemas[i].EventId, eventId, StringComparison.Ordinal))
                {
                    schema = Schemas[i];
                    return true;
                }
            }

            schema = null;
            return false;
        }

        static AnalyticsEventSchema Event(
            string eventId,
            params AnalyticsAttributeSchema[] attributes
        )
        {
            return new AnalyticsEventSchema(eventId, attributes);
        }

        static AnalyticsAttributeSchema Id(
            string key,
            int maximumLength = 64,
            string[] allowedValues = null
        )
        {
            return new AnalyticsAttributeSchema(
                key,
                AnalyticsValueKind.String,
                true,
                maximumLength,
                0L,
                0L,
                allowedValues
            );
        }

        static AnalyticsAttributeSchema Integer(
            string key,
            long minimum,
            long maximum
        )
        {
            return new AnalyticsAttributeSchema(
                key,
                AnalyticsValueKind.Integer,
                true,
                0,
                minimum,
                maximum,
                null
            );
        }

        static AnalyticsAttributeSchema Boolean(string key)
        {
            return new AnalyticsAttributeSchema(
                key,
                AnalyticsValueKind.Boolean,
                true,
                0,
                0L,
                0L,
                null
            );
        }
    }
}
