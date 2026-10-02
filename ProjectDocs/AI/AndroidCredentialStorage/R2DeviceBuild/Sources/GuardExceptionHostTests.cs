using System;
using System.IO;
using LegendsOfRus.AndroidBuild;
class GuardExceptionHostTests
{
    static int passed;
    static void Check(bool condition, string name)
    {
        if (!condition) throw new Exception("FAIL: " + name);
        Console.WriteLine("PASS: " + name); passed++;
    }
    static void Main(string[] args)
    {
        string root = Path.Combine(Path.GetTempPath(), "lor-credential-guard-" + Guid.NewGuid());
        string relative = "Assets/LegendsOfRus/Plugins/AndroidCredentials/";
        string[] names = { "CredentialVault.java", "CredentialVault.java.meta", "AndroidCredentialBridge.java", "AndroidCredentialBridge.java.meta" };
        Directory.CreateDirectory(Path.Combine(root, relative));
        try
        {
            foreach (string name in names)
                File.Copy(Path.Combine(args[0], relative + name), Path.Combine(root, relative + name));
            string vault = Path.Combine(root, relative + names[0]);
            string bridge = Path.Combine(root, relative + names[2]);
            Check(ReviewedAndroidCredentialPlugins.IsAllowed(root, vault), "exact vault accepted");
            Check(ReviewedAndroidCredentialPlugins.IsAllowed(root, bridge), "exact bridge accepted");
            foreach (string name in names)
            {
                string path = Path.Combine(root, relative + name);
                byte[] original = File.ReadAllBytes(path);
                File.AppendAllText(path, "\n// altered\n");
                Check(!ReviewedAndroidCredentialPlugins.IsAllowed(root, vault), name + " mutation rejects vault");
                Check(!ReviewedAndroidCredentialPlugins.IsAllowed(root, bridge), name + " mutation rejects bridge");
                File.Delete(path);
                Check(!ReviewedAndroidCredentialPlugins.IsAllowed(root, vault), name + " omission rejects inventory");
                File.WriteAllBytes(path, original);
            }
            string copy = Path.Combine(root, "CopiedCredentialVault.java");
            File.Copy(vault, copy);
            Check(!ReviewedAndroidCredentialPlugins.IsAllowed(root, copy), "identical bytes outside approved path rejected");
            foreach (string name in new[] { "Additional.java", "Additional.kt", "Additional.jar", "Additional.aar", "Additional.so", "Additional.keystore" })
                Check(!ReviewedAndroidCredentialPlugins.IsAllowed(root, Path.Combine(root, relative + name)), "unlisted " + name + " rejected");
            Check(!ReviewedAndroidCredentialPlugins.IsAllowed(root, Path.Combine(root, relative + names[1])), "metadata not source exemption");
            Check(!ReviewedAndroidCredentialPlugins.IsAllowed(null, vault), "null root rejected");
            Check(!ReviewedAndroidCredentialPlugins.IsAllowed(root, null), "null candidate rejected");
            Check(!ReviewedAndroidCredentialPlugins.IsAllowed(root + "-missing", vault), "wrong root rejected");
            Check(ReviewedAndroidCredentialPlugins.IsAllowed(root, vault) && ReviewedAndroidCredentialPlugins.IsAllowed(root, bridge), "restored exact inventory accepted");
            Console.WriteLine("TOTAL: " + passed + " passed; 0 failed");
        }
        finally { Directory.Delete(root, true); }
    }
}
