#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace LegendsOfRus.Editor
{
    public static class Step20ProductionAssetWave03Step94ReferenceAuthorityIntake
    {
        public const string ApprovedRoot = "Assets/LegendsOfRus/Art/ReferenceOnly/ApprovedScreens";
        public const string IntakeJson = "ProjectDocs/V1_20_STEP09_4/ReferenceAuthority/WAVE03_REFERENCE_AUTHORITY_INTAKE.json";

        static readonly AuthorityDefinition[] Definitions =
        {
            new AuthorityDefinition(ScreenId.Events, "EventsHub", "Events.png"),
            new AuthorityDefinition(ScreenId.EventDetail, "EventDetail", "EventDetail.png"),
            new AuthorityDefinition(ScreenId.IndividualRanking, "IndividualRanking", "IndividualRanking.png"),
            new AuthorityDefinition(ScreenId.TeamHub, "TeamHub", "TeamHub.png"),
            new AuthorityDefinition(ScreenId.TeamRanking, "TeamRanking", "TeamRanking.png"),
            new AuthorityDefinition(ScreenId.TeamChat, "TeamChat", "TeamChat.png"),
            new AuthorityDefinition(ScreenId.FriendsList, "FriendsList", "FriendsList.png"),
            new AuthorityDefinition(ScreenId.Mail, "Mail", "Mail.png")
        };

        [MenuItem("Legends of Rus/Production UI/v1.20 Step 9.4/Intake Approved Wave 3 Reference Authority")]
        public static void RunMenu()
        {
            List<string> failures;
            List<IntakeEntry> entries = WriteIntake(out failures);
            if (failures.Count > 0) throw new InvalidOperationException(string.Join(Environment.NewLine, failures.ToArray()));
            string message = "Step 9.4 reference-authority intake complete: " + entries.Count + "/8 direct approved Wave 3 references found. DesignScreens were not promoted.";
            Debug.Log(message);
            if (!Application.isBatchMode) EditorUtility.DisplayDialog("Legends of Rus", message, "OK");
        }

        public static List<IntakeEntry> WriteIntake(out List<string> failures)
        {
            failures = new List<string>();
            var entries = new List<IntakeEntry>();
            foreach (AuthorityDefinition definition in Definitions)
            {
                string projectPath = ApprovedRoot + "/" + definition.fileName;
                string abs = Absolute(projectPath);
                if (!File.Exists(abs)) continue;
                Vector2Int size;
                try { size = ReadPngSize(abs); }
                catch (Exception ex) { failures.Add(definition.pageKey + ": cannot decode approved reference: " + ex.Message); continue; }
                if (size.x != 941 || size.y != 1672)
                {
                    failures.Add(definition.pageKey + ": approved reference must be exactly 941x1672, found " + size.x + "x" + size.y + ".");
                    continue;
                }
                entries.Add(new IntakeEntry(definition.id, definition.pageKey, projectPath, Sha256(abs), size.x, size.y));
            }

            string absJson = Absolute(IntakeJson);
            Directory.CreateDirectory(Path.GetDirectoryName(absJson));
            var sb = new StringBuilder();
            sb.AppendLine("{");
            sb.AppendLine("  \"schemaVersion\": 1,");
            sb.AppendLine("  \"milestone\": \"v1.20-step09.4-wave03-reference-authority-intake\",");
            sb.AppendLine("  \"authorityRoot\": \"" + Escape(ApprovedRoot) + "\",");
            sb.AppendLine("  \"canonicalPageCount\": 8,");
            sb.AppendLine("  \"directApprovedReferenceCount\": " + entries.Count + ",");
            sb.AppendLine("  \"demoDesignScreensPromoted\": false,");
            sb.AppendLine("  \"requiredReferenceDimensions\": \"941x1672\",");
            sb.AppendLine("  \"entries\": [");
            for (int i = 0; i < entries.Count; i++)
            {
                IntakeEntry e = entries[i];
                sb.Append("    {\"pageKey\":\"").Append(Escape(e.pageKey)).Append("\",\"screenId\":\"").Append(e.id)
                    .Append("\",\"path\":\"").Append(Escape(e.projectPath)).Append("\",\"sha256\":\"").Append(e.sha256)
                    .Append("\",\"width\":").Append(e.width).Append(",\"height\":").Append(e.height).Append("}");
                if (i + 1 < entries.Count) sb.Append(',');
                sb.AppendLine();
            }
            sb.AppendLine("  ],");
            sb.AppendLine("  \"failureCount\": " + failures.Count);
            sb.AppendLine("}");
            File.WriteAllText(absJson, sb.ToString());
            AssetDatabase.Refresh();
            return entries;
        }

        public static List<IntakeEntry> CollectWithoutWriting(out List<string> failures)
        {
            failures = new List<string>();
            var entries = new List<IntakeEntry>();
            foreach (AuthorityDefinition definition in Definitions)
            {
                string projectPath = ApprovedRoot + "/" + definition.fileName;
                string abs = Absolute(projectPath);
                if (!File.Exists(abs)) continue;
                try
                {
                    Vector2Int size = ReadPngSize(abs);
                    if (size.x != 941 || size.y != 1672) { failures.Add(definition.pageKey + ": invalid approved reference dimensions."); continue; }
                    entries.Add(new IntakeEntry(definition.id, definition.pageKey, projectPath, Sha256(abs), size.x, size.y));
                }
                catch (Exception ex) { failures.Add(definition.pageKey + ": " + ex.Message); }
            }
            return entries;
        }

        static Vector2Int ReadPngSize(string path)
        {
            Texture2D texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            try
            {
                if (!texture.LoadImage(File.ReadAllBytes(path), false)) throw new InvalidOperationException("PNG decode failed.");
                return new Vector2Int(texture.width, texture.height);
            }
            finally { UnityEngine.Object.DestroyImmediate(texture); }
        }

        static string Sha256(string path)
        {
            using (SHA256 sha = SHA256.Create())
            using (FileStream stream = File.OpenRead(path))
                return BitConverter.ToString(sha.ComputeHash(stream)).Replace("-", string.Empty).ToLowerInvariant();
        }
        static string Absolute(string projectPath) { return Path.Combine(Directory.GetCurrentDirectory(), projectPath.Replace('/', Path.DirectorySeparatorChar)); }
        static string Escape(string value) { return (value ?? string.Empty).Replace("\\", "\\\\").Replace("\"", "\\\""); }

        readonly struct AuthorityDefinition
        {
            public readonly ScreenId id; public readonly string pageKey; public readonly string fileName;
            public AuthorityDefinition(ScreenId id, string pageKey, string fileName) { this.id = id; this.pageKey = pageKey; this.fileName = fileName; }
        }

        public readonly struct IntakeEntry
        {
            public readonly ScreenId id; public readonly string pageKey; public readonly string projectPath; public readonly string sha256; public readonly int width; public readonly int height;
            public IntakeEntry(ScreenId id, string pageKey, string projectPath, string sha256, int width, int height)
            { this.id = id; this.pageKey = pageKey; this.projectPath = projectPath; this.sha256 = sha256; this.width = width; this.height = height; }
        }
    }
}
#endif
