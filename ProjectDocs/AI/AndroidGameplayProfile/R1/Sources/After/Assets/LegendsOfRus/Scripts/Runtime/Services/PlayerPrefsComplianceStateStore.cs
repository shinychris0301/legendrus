using System;
using LegendsOfRus.Compliance;
using UnityEngine;

namespace LegendsOfRus
{
    [Serializable]
    sealed class ComplianceStateDto
    {
        public int schemaVersion;
        public string privacyVersion;
        public long privacyAcceptedUtcTicks;
        public string termsVersion;
        public long termsAcceptedUtcTicks;
        public int analyticsConsent;
        public int adsConsent;
        public long consentUpdatedUtcTicks;
        public string supportReference;
    }

    sealed class PlayerPrefsComplianceStateStore
    {
        internal const string Key = "LegendsOfRus_Compliance_v1";
        internal const int MaximumSerializedCharacters = 4096;

        internal bool TryLoad(out ComplianceStateSnapshot state)
        {
            state = null;
            if (!PlayerPrefs.HasKey(DevelopmentGameplayProfile.PreferenceKey(Key)))
                return false;

            string serialized = PlayerPrefs.GetString(DevelopmentGameplayProfile.PreferenceKey(Key), string.Empty);
            if (string.IsNullOrEmpty(serialized) ||
                serialized.Length > MaximumSerializedCharacters)
                return false;

            ComplianceStateDto dto;
            try
            {
                dto = JsonUtility.FromJson<ComplianceStateDto>(serialized);
            }
            catch
            {
                return false;
            }
            if (dto == null)
                return false;

            state = FromDto(dto);
            return CompliancePolicy.IsValidState(state);
        }

        internal bool TrySave(ComplianceStateSnapshot state)
        {
            if (!CompliancePolicy.IsValidState(state))
                return false;

            string serialized;
            try
            {
                serialized = JsonUtility.ToJson(ToDto(state));
            }
            catch
            {
                return false;
            }
            if (string.IsNullOrEmpty(serialized) ||
                serialized.Length > MaximumSerializedCharacters)
                return false;

            PlayerPrefs.SetString(DevelopmentGameplayProfile.PreferenceKey(Key), serialized);
            PlayerPrefs.Save();
            return true;
        }

        internal void Clear()
        {
            PlayerPrefs.DeleteKey(DevelopmentGameplayProfile.PreferenceKey(Key));
            PlayerPrefs.Save();
        }

        static ComplianceStateSnapshot FromDto(ComplianceStateDto dto)
        {
            return new ComplianceStateSnapshot(
                dto.schemaVersion,
                dto.privacyVersion,
                dto.privacyAcceptedUtcTicks,
                dto.termsVersion,
                dto.termsAcceptedUtcTicks,
                (ConsentDecision)dto.analyticsConsent,
                (ConsentDecision)dto.adsConsent,
                dto.consentUpdatedUtcTicks,
                dto.supportReference
            );
        }

        static ComplianceStateDto ToDto(ComplianceStateSnapshot state)
        {
            return new ComplianceStateDto
            {
                schemaVersion = state.SchemaVersion,
                privacyVersion = state.PrivacyVersion,
                privacyAcceptedUtcTicks = state.PrivacyAcceptedUtcTicks,
                termsVersion = state.TermsVersion,
                termsAcceptedUtcTicks = state.TermsAcceptedUtcTicks,
                analyticsConsent = (int)state.AnalyticsConsent,
                adsConsent = (int)state.AdsConsent,
                consentUpdatedUtcTicks = state.ConsentUpdatedUtcTicks,
                supportReference = state.SupportReference
            };
        }
    }
}
