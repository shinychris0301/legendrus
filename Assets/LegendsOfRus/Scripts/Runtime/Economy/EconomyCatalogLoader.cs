using System;
using LegendsOfRus.EconomyCore;
using UnityEngine;

namespace LegendsOfRus
{
    /// <summary>
    /// Read-only Resources boundary for the v1.15 Step 1 proposal. Loading and
    /// validation never register the catalog as runtime authority and never
    /// touch GameState, PlayerPrefs, or a player save.
    /// </summary>
    public static class EconomyCatalogLoader
    {
        public const string ResourcePath = "Config/economy";

        public static EconomyCatalogLoadResult LoadProposal()
        {
            TextAsset asset = Resources.Load<TextAsset>(ResourcePath);
            if (asset == null)
            {
                return EconomyCatalogLoadResult.Failed(
                    "ECON_RESOURCE_MISSING",
                    "The proposal resource Config/economy.json is missing."
                );
            }

            EconomyCatalogData catalog;
            try
            {
                catalog = JsonUtility.FromJson<EconomyCatalogData>(asset.text);
            }
            catch (Exception exception)
            {
                return EconomyCatalogLoadResult.Failed(
                    "ECON_JSON_INVALID",
                    exception.GetType().Name + ": " + exception.Message
                );
            }

            if (catalog == null)
            {
                return EconomyCatalogLoadResult.Failed(
                    "ECON_JSON_EMPTY",
                    "The proposal JSON produced no catalog."
                );
            }

            EconomyValidationReport validation =
                EconomyCatalogValidator.Validate(catalog);
            bool valid = validation.verdict == EconomyVerdict.Pass;
            return new EconomyCatalogLoadResult(
                catalog,
                validation,
                asset.text,
                valid ? string.Empty : EconomyViolationCodes.InvalidCatalog,
                valid
                    ? string.Empty
                    : "The proposal catalog failed validation with " +
                        validation.violations.Length + " violation(s)."
            );
        }
    }

    public sealed class EconomyCatalogLoadResult
    {
        public EconomyCatalogData Catalog { get; }
        public EconomyValidationReport Validation { get; }
        public string SourceJson { get; }
        public string ErrorCode { get; }
        public string Error { get; }

        public bool Succeeded
        {
            get
            {
                if (
                    Catalog == null ||
                    Validation == null ||
                    Validation.verdict != EconomyVerdict.Pass ||
                    !string.IsNullOrEmpty(ErrorCode)
                )
                {
                    return false;
                }

                // The DTO is mutable for JsonUtility compatibility. Revalidate
                // at the boundary so a caller cannot mutate a successful load
                // into runtime-authoritative or otherwise invalid data while
                // leaving this result marked successful.
                return EconomyCatalogValidator.Validate(Catalog).verdict ==
                    EconomyVerdict.Pass;
            }
        }

        public EconomyCatalogLoadResult(
            EconomyCatalogData catalog,
            EconomyValidationReport validation,
            string sourceJson,
            string errorCode,
            string error
        )
        {
            Catalog = catalog;
            Validation = validation;
            SourceJson = sourceJson ?? string.Empty;
            ErrorCode = errorCode ?? string.Empty;
            Error = error ?? string.Empty;
        }

        public static EconomyCatalogLoadResult Failed(string code, string error)
        {
            return new EconomyCatalogLoadResult(
                null,
                new EconomyValidationReport
                {
                    verdict = EconomyVerdict.Fail,
                    violations = new[]
                    {
                        new EconomyViolationData(
                            code,
                            EconomyViolationSeverity.Error,
                            "$",
                            error
                        )
                    }
                },
                string.Empty,
                code,
                error
            );
        }
    }
}
