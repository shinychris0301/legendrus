using System;

namespace LegendsOfRus
{
    /// <summary>
    /// Detached approval record for one immutable economy proposal. This DTO is
    /// mutable only so Unity's JsonUtility can deserialize the Resources asset;
    /// the activation gate never returns it as runtime policy.
    /// </summary>
    [Serializable]
    public sealed class RuntimeEconomyActivationManifestData
    {
        // Required fields default to invalid sentinels so omitted JSON fails closed.
        public int schemaVersion;
        public string manifestId = string.Empty;
        public int catalogSchemaVersion;
        public string catalogId = string.Empty;
        public string catalogSourceSha256 = string.Empty;
        public string unityVersion = string.Empty;
        public bool productApproval;
        public bool compilationVerified;
        public bool editorSelfTestsPassed;
        public bool editModeTestsPassed;
        public bool playModeTestsPassed;
        public bool runtimeActivationApproved;
    }
}
