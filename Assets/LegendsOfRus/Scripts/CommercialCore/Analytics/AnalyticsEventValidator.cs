using System;
using System.Collections.Generic;

namespace LegendsOfRus.CommercialCore.Analytics
{
    /// <summary>
    /// Strict allow-list validation. Invalid input is rejected as supplied;
    /// this type never trims, renames, coerces, or otherwise repairs events.
    /// </summary>
    public static class AnalyticsEventValidator
    {
        public static AnalyticsRecordResult ValidateRegistry()
        {
            var events = new HashSet<string>(StringComparer.Ordinal);
            IReadOnlyList<AnalyticsEventSchema> schemas = AnalyticsSchemaRegistry.All;
            for (int i = 0; i < schemas.Count; i++)
            {
                AnalyticsEventSchema schema = schemas[i];
                if (
                    schema == null ||
                    !IsCanonicalKey(
                        schema.EventId,
                        AnalyticsSchemaRegistry.MaximumEventIdLength
                    )
                )
                {
                    return AnalyticsRecordResult.Reject(
                        AnalyticsReasonCodes.EventIdInvalid
                    );
                }

                if (!events.Add(schema.EventId))
                {
                    return AnalyticsRecordResult.Reject(
                        AnalyticsReasonCodes.EventUnknown
                    );
                }

                if (
                    schema.Attributes == null ||
                    schema.Attributes.Count >
                        AnalyticsSchemaRegistry.MaximumAttributesPerEvent
                )
                {
                    return AnalyticsRecordResult.Reject(
                        AnalyticsReasonCodes.AttributeCountInvalid
                    );
                }

                var keys = new HashSet<string>(StringComparer.Ordinal);
                for (int j = 0; j < schema.Attributes.Count; j++)
                {
                    AnalyticsAttributeSchema attribute = schema.Attributes[j];
                    if (attribute == null)
                    {
                        return AnalyticsRecordResult.Reject(
                            AnalyticsReasonCodes.AttributeNull
                        );
                    }

                    if (
                        !IsCanonicalKey(
                            attribute.Key,
                            AnalyticsSchemaRegistry.MaximumAttributeKeyLength
                        )
                    )
                    {
                        return AnalyticsRecordResult.Reject(
                            AnalyticsReasonCodes.AttributeKeyInvalid
                        );
                    }

                    if (!keys.Add(attribute.Key))
                    {
                        return AnalyticsRecordResult.Reject(
                            AnalyticsReasonCodes.AttributeDuplicate
                        );
                    }

                    if (attribute.Kind == AnalyticsValueKind.String)
                    {
                        if (attribute.MaximumStringLength < 1)
                        {
                            return AnalyticsRecordResult.Reject(
                                AnalyticsReasonCodes.StringValueInvalid
                            );
                        }

                        var values = new HashSet<string>(StringComparer.Ordinal);
                        for (int k = 0; k < attribute.AllowedStringValues.Count; k++)
                        {
                            string value = attribute.AllowedStringValues[k];
                            if (
                                !IsCanonicalIdentifier(
                                    value,
                                    attribute.MaximumStringLength
                                ) ||
                                !values.Add(value)
                            )
                            {
                                return AnalyticsRecordResult.Reject(
                                    AnalyticsReasonCodes.StringValueInvalid
                                );
                            }
                        }
                    }
                    else if (attribute.Kind == AnalyticsValueKind.Integer)
                    {
                        if (attribute.MinimumInteger > attribute.MaximumInteger)
                        {
                            return AnalyticsRecordResult.Reject(
                                AnalyticsReasonCodes.IntegerValueOutOfRange
                            );
                        }
                    }
                    else if (attribute.Kind != AnalyticsValueKind.Boolean)
                    {
                        return AnalyticsRecordResult.Reject(
                            AnalyticsReasonCodes.AttributeTypeMismatch
                        );
                    }
                }
            }

            return AnalyticsRecordResult.Success();
        }

        public static AnalyticsRecordResult ValidateDraft(AnalyticsEventDraft draft)
        {
            AnalyticsEventSchema ignored;
            return ValidateDraft(draft, out ignored);
        }

        internal static AnalyticsRecordResult ValidateDraft(
            AnalyticsEventDraft draft,
            out AnalyticsEventSchema schema
        )
        {
            schema = null;
            if (draft == null)
                return AnalyticsRecordResult.Reject(AnalyticsReasonCodes.DraftNull);

            if (
                !IsCanonicalKey(
                    draft.EventId,
                    AnalyticsSchemaRegistry.MaximumEventIdLength
                )
            )
            {
                return AnalyticsRecordResult.Reject(
                    AnalyticsReasonCodes.EventIdInvalid
                );
            }

            if (!AnalyticsSchemaRegistry.TryGet(draft.EventId, out schema))
                return AnalyticsRecordResult.Reject(AnalyticsReasonCodes.EventUnknown);

            if (
                draft.Attributes == null ||
                draft.Attributes.Count >
                    AnalyticsSchemaRegistry.MaximumAttributesPerEvent
            )
            {
                return AnalyticsRecordResult.Reject(
                    AnalyticsReasonCodes.AttributeCountInvalid
                );
            }

            var seen = new HashSet<string>(StringComparer.Ordinal);
            for (int i = 0; i < draft.Attributes.Count; i++)
            {
                AnalyticsAttribute attribute = draft.Attributes[i];
                if (attribute == null)
                    return AnalyticsRecordResult.Reject(
                        AnalyticsReasonCodes.AttributeNull
                    );

                if (
                    !IsCanonicalKey(
                        attribute.Key,
                        AnalyticsSchemaRegistry.MaximumAttributeKeyLength
                    )
                )
                {
                    return AnalyticsRecordResult.Reject(
                        AnalyticsReasonCodes.AttributeKeyInvalid
                    );
                }

                if (!seen.Add(attribute.Key))
                    return AnalyticsRecordResult.Reject(
                        AnalyticsReasonCodes.AttributeDuplicate
                    );

                AnalyticsAttributeSchema attributeSchema;
                if (!schema.TryGetAttribute(attribute.Key, out attributeSchema))
                    return AnalyticsRecordResult.Reject(
                        AnalyticsReasonCodes.AttributeUnexpected
                    );

                if (attribute.Kind != attributeSchema.Kind)
                    return AnalyticsRecordResult.Reject(
                        AnalyticsReasonCodes.AttributeTypeMismatch
                    );

                AnalyticsRecordResult valueResult = ValidateValue(
                    attribute,
                    attributeSchema
                );
                if (!valueResult.Accepted)
                    return valueResult;
            }

            for (int i = 0; i < schema.Attributes.Count; i++)
            {
                AnalyticsAttributeSchema required = schema.Attributes[i];
                if (required.Required && !seen.Contains(required.Key))
                    return AnalyticsRecordResult.Reject(
                        AnalyticsReasonCodes.AttributeMissing
                    );
            }

            return AnalyticsRecordResult.Success();
        }

        public static AnalyticsRecordResult ValidateEnvelope(
            AnalyticsEventEnvelope envelope
        )
        {
            if (envelope == null)
                return AnalyticsRecordResult.Reject(
                    AnalyticsReasonCodes.EnvelopeNull
                );

            if (envelope.SchemaVersion != AnalyticsSchemaRegistry.CurrentSchemaVersion)
            {
                return AnalyticsRecordResult.Reject(
                    AnalyticsReasonCodes.SchemaVersionUnsupported
                );
            }

            if (
                !IsCanonicalIdentifier(
                    envelope.SessionId,
                    AnalyticsSchemaRegistry.MaximumSessionIdLength
                )
            )
            {
                return AnalyticsRecordResult.Reject(
                    AnalyticsReasonCodes.SessionIdInvalid
                );
            }

            if (envelope.Sequence < 1L)
                return AnalyticsRecordResult.Reject(
                    AnalyticsReasonCodes.SequenceInvalid
                );
            if (envelope.UtcTicks < 1L)
                return AnalyticsRecordResult.Reject(
                    AnalyticsReasonCodes.TimestampInvalid
                );

            return ValidateDraft(
                new AnalyticsEventDraft(
                    envelope.EventId,
                    envelope.DetachedAttributes()
                )
            );
        }

        internal static bool IsCanonicalIdentifier(string value, int maximumLength)
        {
            if (
                string.IsNullOrEmpty(value) ||
                maximumLength < 1 ||
                value.Length > maximumLength ||
                !IsAsciiLetterOrDigit(value[0]) ||
                !IsAsciiLetterOrDigit(value[value.Length - 1])
            )
            {
                return false;
            }

            for (int i = 1; i < value.Length - 1; i++)
            {
                char character = value[i];
                if (
                    !IsAsciiLetterOrDigit(character) &&
                    character != '_' &&
                    character != '-' &&
                    character != '.'
                )
                {
                    return false;
                }
            }

            return true;
        }

        internal static bool IsCanonicalKey(string value, int maximumLength)
        {
            if (
                string.IsNullOrEmpty(value) ||
                maximumLength < 1 ||
                value.Length > maximumLength ||
                value[0] < 'a' ||
                value[0] > 'z'
            )
            {
                return false;
            }

            for (int i = 1; i < value.Length; i++)
            {
                char character = value[i];
                if (
                    (character < 'a' || character > 'z') &&
                    (character < '0' || character > '9') &&
                    character != '_'
                )
                {
                    return false;
                }
            }

            return true;
        }

        static AnalyticsRecordResult ValidateValue(
            AnalyticsAttribute attribute,
            AnalyticsAttributeSchema schema
        )
        {
            switch (attribute.Kind)
            {
                case AnalyticsValueKind.String:
                    if (
                        !IsCanonicalIdentifier(
                            attribute.StringValue,
                            schema.MaximumStringLength
                        )
                    )
                    {
                        return AnalyticsRecordResult.Reject(
                            AnalyticsReasonCodes.StringValueInvalid
                        );
                    }

                    if (!schema.IsAllowedStringValue(attribute.StringValue))
                    {
                        return AnalyticsRecordResult.Reject(
                            AnalyticsReasonCodes.StringValueNotAllowed
                        );
                    }
                    break;
                case AnalyticsValueKind.Integer:
                    if (
                        attribute.IntegerValue < schema.MinimumInteger ||
                        attribute.IntegerValue > schema.MaximumInteger
                    )
                    {
                        return AnalyticsRecordResult.Reject(
                            AnalyticsReasonCodes.IntegerValueOutOfRange
                        );
                    }
                    break;
                case AnalyticsValueKind.Boolean:
                    break;
                default:
                    return AnalyticsRecordResult.Reject(
                        AnalyticsReasonCodes.AttributeTypeMismatch
                    );
            }

            return AnalyticsRecordResult.Success();
        }

        static bool IsAsciiLetterOrDigit(char value)
        {
            return
                (value >= 'a' && value <= 'z') ||
                (value >= '0' && value <= '9');
        }
    }
}
