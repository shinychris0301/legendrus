using System;

namespace LegendsOfRus.QA
{
    [Serializable]
    public struct DeviceMatrixProfile
    {
        public string Id;
        public DeviceProfileInput Input;
        public DeviceCapabilityBand ExpectedCapability;
        public DeviceAspectBucket ExpectedAspect;

        public DeviceMatrixProfile(
            string id,
            DeviceProfileInput input,
            DeviceCapabilityBand expectedCapability,
            DeviceAspectBucket expectedAspect)
        {
            Id = id ?? string.Empty;
            Input = input;
            ExpectedCapability = expectedCapability;
            ExpectedAspect = expectedAspect;
        }
    }

    public static class DeviceMatrixCatalog
    {
        static readonly DeviceMatrixProfile[] RequiredProfiles =
        {
            new DeviceMatrixProfile(
                "android_low_16_9_720p",
                new DeviceProfileInput(3072, 512, 4, 4096, 720, 1280),
                DeviceCapabilityBand.Low,
                DeviceAspectBucket.Standard16By9),
            new DeviceMatrixProfile(
                "android_mid_18_9_1080p",
                new DeviceProfileInput(4096, 768, 6, 4096, 1080, 2160),
                DeviceCapabilityBand.Mid,
                DeviceAspectBucket.Tall18By9),
            new DeviceMatrixProfile(
                "android_mid_19_5_9_1080p",
                new DeviceProfileInput(4096, 1024, 6, 4096, 1080, 2340),
                DeviceCapabilityBand.Mid,
                DeviceAspectBucket.Tall19_5By9),
            new DeviceMatrixProfile(
                "android_high_20_9_1080p",
                new DeviceProfileInput(6144, 1024, 8, 8192, 1080, 2400),
                DeviceCapabilityBand.High,
                DeviceAspectBucket.Tall20By9),
            new DeviceMatrixProfile(
                "android_high_20_9_1440p",
                new DeviceProfileInput(8192, 2048, 8, 8192, 1440, 3200),
                DeviceCapabilityBand.High,
                DeviceAspectBucket.Tall20By9)
        };

        public static int RequiredProfileCount => RequiredProfiles.Length;

        public static DeviceMatrixProfile[] CopyRequiredProfiles()
        {
            var copy = new DeviceMatrixProfile[RequiredProfiles.Length];
            Array.Copy(RequiredProfiles, copy, RequiredProfiles.Length);
            return copy;
        }

        public static bool Validate(out string failure)
        {
            failure = string.Empty;
            if (RequiredProfiles.Length < 5)
            {
                failure = "QA_MATRIX_TOO_SMALL";
                return false;
            }

            bool has16 = false;
            bool has18 = false;
            bool has195 = false;
            bool has20 = false;
            bool hasLow = false;
            bool hasMid = false;
            bool hasHigh = false;

            for (int i = 0; i < RequiredProfiles.Length; i++)
            {
                DeviceMatrixProfile profile = RequiredProfiles[i];
                if (string.IsNullOrEmpty(profile.Id))
                {
                    failure = "QA_MATRIX_ID_MISSING";
                    return false;
                }
                DeviceProfileSnapshot actual =
                    DeviceProfileClassifier.Classify(profile.Input);
                if (actual.CapabilityBand != profile.ExpectedCapability ||
                    actual.AspectBucket != profile.ExpectedAspect)
                {
                    failure = "QA_MATRIX_CLASSIFICATION_MISMATCH";
                    return false;
                }

                has16 |= actual.AspectBucket == DeviceAspectBucket.Standard16By9;
                has18 |= actual.AspectBucket == DeviceAspectBucket.Tall18By9;
                has195 |= actual.AspectBucket == DeviceAspectBucket.Tall19_5By9;
                has20 |= actual.AspectBucket == DeviceAspectBucket.Tall20By9;
                hasLow |= actual.CapabilityBand == DeviceCapabilityBand.Low;
                hasMid |= actual.CapabilityBand == DeviceCapabilityBand.Mid;
                hasHigh |= actual.CapabilityBand == DeviceCapabilityBand.High;
            }

            if (!has16 || !has18 || !has195 || !has20 ||
                !hasLow || !hasMid || !hasHigh)
            {
                failure = "QA_MATRIX_REQUIRED_BUCKET_MISSING";
                return false;
            }
            return true;
        }
    }
}
