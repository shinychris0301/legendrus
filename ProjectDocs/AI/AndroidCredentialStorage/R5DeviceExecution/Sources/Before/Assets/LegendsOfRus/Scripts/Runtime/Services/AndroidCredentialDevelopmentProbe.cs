#if UNITY_EDITOR || (UNITY_ANDROID && DEVELOPMENT_BUILD)
using System.Collections;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace LegendsOfRus
{
    internal static class AndroidCredentialDevelopmentProbe
    {
        const string Extra = "lor_credential_probe";
        static bool running;
        static AndroidCredentialProbeHost interruptedHost;
        internal static bool BlocksAccounts
        {
            get
            {
#if UNITY_ANDROID && DEVELOPMENT_BUILD && !UNITY_EDITOR
                if (running) return true;
                try
                {
                    using (var context = Context())
                    using (var file = context.Call<AndroidJavaObject>("getNoBackupFilesDir"))
                        return Directory.Exists(Path.Combine(file.Call<string>("getAbsolutePath"), AndroidCredentialProbeHost.DirectoryName));
                }
                catch { return true; }
#else
                return false;
#endif
            }
        }
        static AndroidJavaObject Context()
        {
            using (var unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            using (var activity = unity.GetStatic<AndroidJavaObject>("currentActivity"))
                return activity.Call<AndroidJavaObject>("getApplicationContext");
        }
        internal static IEnumerator RunRequested(BackendAccountService service)
        {
#if UNITY_ANDROID && DEVELOPMENT_BUILD && !UNITY_EDITOR
            string command = null;
            try
            {
                using (var unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                using (var activity = unity.GetStatic<AndroidJavaObject>("currentActivity"))
                using (var intent = activity.Call<AndroidJavaObject>("getIntent"))
                {
                    command = intent.Call<string>("getStringExtra", Extra);
                    intent.Call<AndroidJavaObject>("removeExtra", Extra)?.Dispose();
                }
            }
            catch { yield break; }
            if (!CredentialProbeProtocol.KnownCommand(command) || running) yield break;
            bool configured = service == null || service.IsConfigured;
            var context = Context();
            running = true;
            var task = Task.Run(() =>
            {
                AndroidJNI.AttachCurrentThread();
                AndroidCredentialProbeHost host = null;
                try
                {
                    host = new AndroidCredentialProbeHost(context, configured);
                    var report = CredentialProbeProtocol.Run(command, host);
                    host.WriteReport(report);
                    if (report.status == "awaiting_force_stop") interruptedHost = host;
                    return report.status;
                }
                catch { return "failed_retained"; }
                finally
                {
                    if (host != interruptedHost) { if (host != null) host.Dispose(); else context.Dispose(); }
                    AndroidJNI.DetachCurrentThread();
                }
            });
            while (!task.IsCompleted) yield return null;
            running = false;
            // Fixed status only: no exception, fixture, account, ciphertext or token output.
            Debug.Log("LOR_CREDENTIAL_PROBE " + (task.IsFaulted ? "failed_retained" : task.Result));
#else
            yield break;
#endif
        }
    }
}
#endif
