#if UNITY_EDITOR && UNITY_6000_0_OR_NEWER
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using UnityEditor;
using UnityEditor.Build;

namespace LegendsOfRus.AndroidBuild
{
    [Serializable]
    internal sealed class AndroidBuildOptionEnumEvidence
    {
        public string name;
        public string rawHex;
        public bool zeroValue;
        public bool singleBit;
        public bool obsolete;
    }

    internal sealed class AndroidBuildOptionDefinition
    {
        internal readonly string Name;
        internal readonly uint Bits;
        internal readonly bool Obsolete;

        internal AndroidBuildOptionDefinition(string name, uint bits, bool obsolete)
        {
            Name = name;
            Bits = bits;
            Obsolete = obsolete;
        }
    }

    /// <summary>
    /// Fix04: keep Development mandatory; accept standard compression/error-checking flags.
    /// Zero-valued or composite enum aliases are not reported as enabled switches.
    /// No flags are silently removed from a scheduled request.
    /// </summary>
    internal static class AndroidDevelopmentBuildOptions
    {
        internal const BuildOptions Allowed = BuildOptions.Development |
            BuildOptions.ShowBuiltPlayer | BuildOptions.CompressWithLz4 |
            BuildOptions.CompressWithLz4HC | BuildOptions.StrictMode;

        internal const BuildOptions BuildOnlyDefaults = BuildOptions.Development |
            BuildOptions.CompressWithLz4 | BuildOptions.StrictMode;

        private const string SnapshotKey = "LegendsOfRus.BuildFix04.ScheduledOptions";
        private const string SnapshotPresentKey = SnapshotKey + ".Present";

        internal static bool HasDevelopment(BuildOptions value)
        {
            return (value & BuildOptions.Development) != 0;
        }

        internal static BuildOptions Disallowed(BuildOptions value)
        {
            return value & ~Allowed;
        }

        internal static void Validate(BuildOptions value)
        {
            if (!HasDevelopment(value))
                throw Rejected(AndroidBuildReasonCodes.DevelopmentBuildRequired, value);
            if (Disallowed(value) != 0)
                throw Rejected(AndroidBuildReasonCodes.BuildOptionsUnsafe, value);
        }

        private static BuildFailedException Rejected(string reason, BuildOptions value)
        {
            return new BuildFailedException(AndroidBuildGuard.Denied(reason).Message +
                " " + Describe(value));
        }

        internal static string Describe(BuildOptions value)
        {
            return "RequestedBits=0x" + RawBits(value).ToString("X8", CultureInfo.InvariantCulture) +
                "; ActiveFlags=" + FormatActive(value) +
                "; Development=" + HasDevelopment(value) +
                "; AutoRunPlayer=" + ((value & BuildOptions.AutoRunPlayer) != 0) +
                "; DisallowedBits=0x" + RawBits(Disallowed(value)).ToString("X8", CultureInfo.InvariantCulture) +
                "; DisallowedFlags=" + FormatActive(Disallowed(value)) + ".";
        }

        // Unity BuildOptions is an Int32 enum. Unsigned conversion retains the high bit.
        internal static uint RawBits(BuildOptions value) { return unchecked((uint)value); }

        internal static string FormatActive(BuildOptions value)
        {
            return FormatBits(RawBits(value), ReadDefinitions());
        }

        // A pure formatting seam for testing zero aliases, duplicate aliases and unknown bits.
        internal static string FormatBits(uint bits, AndroidBuildOptionDefinition[] definitions)
        {
            if (bits == 0) return "None";
            var candidates = new List<AndroidBuildOptionDefinition>(definitions ??
                new AndroidBuildOptionDefinition[0]);
            candidates.Sort((left, right) =>
            {
                int comparison = left.Bits.CompareTo(right.Bits);
                if (comparison != 0) return comparison;
                comparison = left.Obsolete.CompareTo(right.Obsolete);
                return comparison != 0 ? comparison : string.CompareOrdinal(left.Name, right.Name);
            });
            uint described = 0;
            var names = new List<string>();
            foreach (var definition in candidates)
            {
                uint flag = definition.Bits;
                if (!IsSingleBit(flag) || string.IsNullOrEmpty(definition.Name)) continue;
                if ((bits & flag) != flag || (described & flag) != 0) continue;
                names.Add(definition.Name);
                described |= flag;
            }
            uint unknown = bits & ~described;
            if (unknown != 0)
                names.Add("UnknownBits(0x" + unknown.ToString("X8", CultureInfo.InvariantCulture) + ")");
            return string.Join(", ", names.ToArray());
        }

        internal static bool IsSingleBit(uint value)
        {
            return value != 0 && (value & (value - 1)) == 0;
        }

        private static AndroidBuildOptionDefinition[] ReadDefinitions()
        {
            var definitions = new List<AndroidBuildOptionDefinition>();
            foreach (FieldInfo field in typeof(BuildOptions).GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                if (!field.IsLiteral) continue;
                var value = (BuildOptions)field.GetValue(null);
                definitions.Add(new AndroidBuildOptionDefinition(field.Name, RawBits(value),
                    Attribute.IsDefined(field, typeof(ObsoleteAttribute))));
            }
            return definitions.ToArray();
        }

        internal static AndroidBuildOptionEnumEvidence[] ReadEnumEvidence()
        {
            var definitions = ReadDefinitions();
            Array.Sort(definitions, (left, right) => string.CompareOrdinal(left.Name, right.Name));
            var result = new AndroidBuildOptionEnumEvidence[definitions.Length];
            for (int index = 0; index < definitions.Length; index++)
            {
                var definition = definitions[index];
                result[index] = new AndroidBuildOptionEnumEvidence
                {
                    name = definition.Name,
                    rawHex = "0x" + definition.Bits.ToString("X8", CultureInfo.InvariantCulture),
                    zeroValue = definition.Bits == 0,
                    singleBit = IsSingleBit(definition.Bits),
                    obsolete = definition.Obsolete
                };
            }
            return result;
        }

        internal static void RememberScheduled(BuildOptions value)
        {
            SessionState.SetInt(SnapshotKey, unchecked((int)value));
            SessionState.SetBool(SnapshotPresentKey, true);
        }

        internal static bool HasScheduledSnapshot
        {
            get { return SessionState.GetBool(SnapshotPresentKey, false); }
        }

        internal static string ScheduledSnapshot()
        {
            return HasScheduledSnapshot
                ? Describe((BuildOptions)SessionState.GetInt(SnapshotKey, 0))
                : "No Android scheduled-options callback observed in this Editor session since Fix04.";
        }
    }
}
#endif
