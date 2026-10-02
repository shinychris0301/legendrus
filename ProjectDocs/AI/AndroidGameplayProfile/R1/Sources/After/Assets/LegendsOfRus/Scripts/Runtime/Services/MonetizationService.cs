using System;
using UnityEngine;

namespace LegendsOfRus
{
    public enum RuntimeMonetizationActionState
    {
        Unavailable = 0,
        CandidateOnly = 1,
        VerifiedAndDurablyCommitted = 2
    }

    /// <summary>
    /// Result crossing the Runtime assembly boundary. Only the final state may
    /// allow navigation that assumes a reward exists. Step 2 never returns that
    /// state because no approved atomic commercial store exists for save v6.
    /// </summary>
    public sealed class RuntimeMonetizationActionResult
    {
        public RuntimeMonetizationActionState State { get; private set; }
        public string ReasonCode { get; private set; }

        public bool IsVerifiedAndDurablyCommitted
        {
            get
            {
                return State ==
                    RuntimeMonetizationActionState.VerifiedAndDurablyCommitted;
            }
        }

        internal RuntimeMonetizationActionResult(
            RuntimeMonetizationActionState state,
            string reasonCode
        )
        {
            State = state;
            ReasonCode = reasonCode ?? string.Empty;
        }

        public static RuntimeMonetizationActionResult Unavailable(
            string reasonCode = "MONETIZATION_ADAPTER_DISABLED"
        )
        {
            return new RuntimeMonetizationActionResult(
                RuntimeMonetizationActionState.Unavailable,
                reasonCode
            );
        }

        internal static RuntimeMonetizationActionResult CandidateOnly(
            string reasonCode = "COMMERCIAL_FULFILLMENT_ATOMIC_STORE_UNAVAILABLE"
        )
        {
            return new RuntimeMonetizationActionResult(
                RuntimeMonetizationActionState.CandidateOnly,
                reasonCode
            );
        }

        internal static RuntimeMonetizationActionResult Committed()
        {
            return new RuntimeMonetizationActionResult(
                RuntimeMonetizationActionState.VerifiedAndDurablyCommitted,
                string.Empty
            );
        }
    }

    /// <summary>
    /// Trusted composition port. The Runtime assembly owns no store, ad SDK,
    /// verifier, receipt, reward recipe, or persistence implementation.
    /// </summary>
    internal interface IRuntimeMonetizationPort
    {
        RuntimeMonetizationActionResult TryPurchase(string offerId);
        RuntimeMonetizationActionResult TryRewardedEnergy();
        RuntimeMonetizationActionResult TryRewardedDouble();
        RuntimeMonetizationActionResult TryRewardedContinue();
    }

    internal sealed class DisabledRuntimeMonetizationPort :
        IRuntimeMonetizationPort
    {
        public RuntimeMonetizationActionResult TryPurchase(string offerId)
        {
            return RuntimeMonetizationActionResult.Unavailable();
        }

        public RuntimeMonetizationActionResult TryRewardedEnergy()
        {
            return RuntimeMonetizationActionResult.Unavailable();
        }

        public RuntimeMonetizationActionResult TryRewardedDouble()
        {
            return RuntimeMonetizationActionResult.Unavailable();
        }

        public RuntimeMonetizationActionResult TryRewardedContinue()
        {
            return RuntimeMonetizationActionResult.Unavailable(
                "COMMERCIAL_FULFILLMENT_RECIPE_UNAVAILABLE"
            );
        }
    }

    /// <summary>
    /// Fail-closed facade retained for existing UI wiring. The inherited mock
    /// methods are compatibility wrappers only and can no longer grant currency,
    /// energy, boosters, or reward doubling.
    /// </summary>
    public sealed class MonetizationService : MonoBehaviour
    {
        static readonly IRuntimeMonetizationPort DisabledPort =
            new DisabledRuntimeMonetizationPort();

        readonly object syncRoot = new object();
        IRuntimeMonetizationPort runtimePort = DisabledPort;

        public static MonetizationService Instance { get; private set; }

        public bool HasBoundRuntimePort
        {
            get
            {
                lock (syncRoot)
                    return !ReferenceEquals(runtimePort, DisabledPort);
            }
        }

        void Awake()
        {
            Instance = this;
        }

        void OnDestroy()
        {
            lock (syncRoot)
                runtimePort = DisabledPort;
            if (Instance == this)
                Instance = null;
        }

        internal bool TryBindRuntimePort(IRuntimeMonetizationPort port)
        {
            if (DevelopmentGameplayProfile.Active || port == null)
                return false;

            lock (syncRoot)
            {
                if (port is DisabledRuntimeMonetizationPort)
                {
                    runtimePort = DisabledPort;
                    return true;
                }
                if (!ReferenceEquals(runtimePort, DisabledPort))
                    return ReferenceEquals(runtimePort, port);
                runtimePort = port;
                return true;
            }
        }

        public RuntimeMonetizationActionResult TryPurchase(string offerId)
        {
            if (!IsKnownOffer(offerId))
            {
                return RuntimeMonetizationActionResult.Unavailable(
                    "MONETIZATION_UNKNOWN_OFFER"
                );
            }

            return Invoke(port => port.TryPurchase(offerId));
        }

        public RuntimeMonetizationActionResult TryRewardedEnergy()
        {
            if (!OptionalAdsAllowed())
                return RuntimeMonetizationActionResult.Unavailable(
                    "MONETIZATION_AD_CONSENT_REQUIRED");
            return Invoke(port => port.TryRewardedEnergy());
        }

        public RuntimeMonetizationActionResult TryRewardedDouble()
        {
            if (!OptionalAdsAllowed())
                return RuntimeMonetizationActionResult.Unavailable(
                    "MONETIZATION_AD_CONSENT_REQUIRED");
            return Invoke(port => port.TryRewardedDouble());
        }

        public RuntimeMonetizationActionResult TryRewardedContinue()
        {
            if (!OptionalAdsAllowed())
                return RuntimeMonetizationActionResult.Unavailable(
                    "MONETIZATION_AD_CONSENT_REQUIRED");
            return Invoke(port => port.TryRewardedContinue());
        }

        // Compatibility wrappers retained so inherited scenes or test code do
        // not regain the old direct-grant behavior.
        public bool MockPurchase(string packId)
        {
            return TryPurchase(packId).IsVerifiedAndDurablyCommitted;
        }

        public bool MockRewardedEnergy()
        {
            return TryRewardedEnergy().IsVerifiedAndDurablyCommitted;
        }

        public bool MockRewardedContinue()
        {
            return TryRewardedContinue().IsVerifiedAndDurablyCommitted;
        }

        RuntimeMonetizationActionResult Invoke(
            Func<IRuntimeMonetizationPort, RuntimeMonetizationActionResult> action
        )
        {
            if (DevelopmentGameplayProfile.Active)
                return RuntimeMonetizationActionResult.Unavailable("GAMEPLAY_QA_OFFLINE");
            IRuntimeMonetizationPort port;
            lock (syncRoot)
                port = runtimePort ?? DisabledPort;

            RuntimeMonetizationActionResult result;
            try
            {
                result = action != null ? action(port) : null;
            }
            catch
            {
                return RuntimeMonetizationActionResult.Unavailable(
                    "MONETIZATION_ADAPTER_UNAVAILABLE"
                );
            }

            if (result == null)
            {
                return RuntimeMonetizationActionResult.Unavailable(
                    "MONETIZATION_ADAPTER_UNAVAILABLE"
                );
            }

            // A committed outcome is internally inconsistent if it also carries
            // a failure reason. Treat it as unavailable rather than trusting it.
            if (
                result.IsVerifiedAndDurablyCommitted &&
                !string.IsNullOrEmpty(result.ReasonCode)
            )
            {
                return RuntimeMonetizationActionResult.Unavailable(
                    "MONETIZATION_INCONSISTENT_RESULT"
                );
            }

            return result;
        }

        static bool OptionalAdsAllowed()
        {
            return PrivacySupportService.Instance != null &&
                PrivacySupportService.Instance.MayUseAds;
        }

        static bool IsKnownOffer(string offerId)
        {
            switch (offerId)
            {
                case "starter":
                case "hero":
                case "firebird":
                case "energy":
                    return true;
                default:
                    return false;
            }
        }
    }
}
