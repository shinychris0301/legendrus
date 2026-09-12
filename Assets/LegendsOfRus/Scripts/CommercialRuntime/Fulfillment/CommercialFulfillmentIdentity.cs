using System.Security.Cryptography;
using System.Text;
using LegendsOfRus.CommercialCore.Monetization;
using UnityEngine;

namespace LegendsOfRus.CommercialRuntime.Fulfillment
{
    internal static class CommercialFulfillmentIdentity
    {
        const string MarkerDomain =
            "LegendsOfRus.CommercialFulfillment/v1\0";

        public static bool TryFingerprint(
            PlayerSaveData source,
            out string fingerprint
        )
        {
            fingerprint = string.Empty;
            if (source == null)
                return false;

            try
            {
                string json = JsonUtility.ToJson(source);
                if (string.IsNullOrEmpty(json))
                    return false;
                fingerprint = ComputeSha256(json);
                return !string.IsNullOrEmpty(fingerprint);
            }
            catch
            {
                fingerprint = string.Empty;
                return false;
            }
        }

        public static PlayerSaveData CloneSave(PlayerSaveData source)
        {
            if (source == null)
                return null;

            try
            {
                return JsonUtility.FromJson<PlayerSaveData>(
                    JsonUtility.ToJson(source)
                );
            }
            catch
            {
                return null;
            }
        }

        public static bool TryBuildMarker(
            EntitlementReservation reservation,
            out string marker
        )
        {
            marker = string.Empty;
            if (
                reservation == null ||
                string.IsNullOrWhiteSpace(reservation.FulfillmentKey)
            )
            {
                return false;
            }

            string digest = ComputeSha256(
                MarkerDomain + reservation.FulfillmentKey
            );
            if (string.IsNullOrEmpty(digest))
                return false;

            marker = "cf1-" + digest;
            return true;
        }

        public static string TransactionKey(string marker)
        {
            return string.IsNullOrEmpty(marker)
                ? string.Empty
                : "commercial.fulfillment." + marker;
        }

        static string ComputeSha256(string value)
        {
            byte[] bytes = new UTF8Encoding(false).GetBytes(
                value ?? string.Empty
            );
            byte[] digest;
            using (SHA256 sha256 = SHA256.Create())
                digest = sha256.ComputeHash(bytes);

            var result = new StringBuilder(digest.Length * 2);
            for (int i = 0; i < digest.Length; i++)
                result.Append(digest[i].ToString("x2"));
            return result.ToString();
        }
    }
}
