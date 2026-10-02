using System;
using System.Collections;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LegendsOfRus
{
    internal sealed class BackendHttpTransport : IBackendTransport
    {
        readonly HttpClient client;
        readonly CancellationTokenSource lifetime = new CancellationTokenSource();
        public string Origin { get; }

        internal BackendHttpTransport(string origin, bool editorLoopback = false)
        {
            if (!Uri.TryCreate(origin, UriKind.Absolute, out var uri) || uri.UserInfo.Length != 0 ||
                uri.Query.Length != 0 || uri.Fragment.Length != 0 || uri.AbsolutePath != "/")
                throw new ArgumentException("Invalid backend origin");
            bool permitted = uri.Scheme == "https";
#if UNITY_EDITOR
            // Explicit Editor verification only. Android never permits plaintext credentials.
            permitted |= editorLoopback && uri.Scheme == "http" && (uri.Host == "127.0.0.1" || uri.Host == "[::1]");
#endif
            if (!permitted) throw new ArgumentException("HTTPS backend required");
            Origin = uri.GetLeftPart(UriPartial.Authority);
            client = new HttpClient(new HttpClientHandler { AllowAutoRedirect = false, UseCookies = false }) { Timeout = TimeSpan.FromSeconds(10) };
        }

        public IEnumerator Send(string method, string path, string body, string bearer, Action<BackendHttpResult> completed)
        {
            if (path != "/v1/auth/bootstrap" && path != "/v1/auth/refresh" && path != "/v1/player")
                throw new ArgumentException("Unsupported backend route");
            var pending = SendAsync(method, path, body, bearer);
            while (!pending.IsCompleted) yield return null;
            completed(pending.GetAwaiter().GetResult());
        }

        async Task<BackendHttpResult> SendAsync(string method, string path, string body, string bearer)
        {
            try
            {
                using (var request = new HttpRequestMessage(new HttpMethod(method), Origin + path))
                using (var timeout = CancellationTokenSource.CreateLinkedTokenSource(lifetime.Token))
                {
                    timeout.CancelAfter(TimeSpan.FromSeconds(10));
                    if (body != null) request.Content = new StringContent(body, Encoding.UTF8, "application/json");
                    if (bearer != null) request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", bearer);
                    using (var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, timeout.Token).ConfigureAwait(false))
                    {
                        if ((int)response.StatusCode != 200) return new BackendHttpResult((int)response.StatusCode, null);
                        if (response.Content.Headers.ContentLength > 65536) return new BackendHttpResult(0, null);
                        using (var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
                        using (var bytes = new MemoryStream())
                        {
                            var buffer = new byte[4096]; int count;
                            while ((count = await stream.ReadAsync(buffer, 0, buffer.Length, timeout.Token).ConfigureAwait(false)) > 0)
                            {
                                if (bytes.Length + count > 65536) return new BackendHttpResult(0, null);
                                bytes.Write(buffer, 0, count);
                            }
                            return new BackendHttpResult(200, new UTF8Encoding(false, true).GetString(bytes.ToArray()));
                        }
                    }
                }
            }
            catch (Exception) { return new BackendHttpResult(0, null); } // Never surface credential-bearing request data.
        }

        public void Dispose() { lifetime.Cancel(); client.Dispose(); }
    }
}
