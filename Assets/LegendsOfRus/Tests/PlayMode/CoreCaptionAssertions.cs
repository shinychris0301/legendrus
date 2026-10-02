using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

namespace LegendsOfRus.Tests.PlayMode
{
    internal static class CoreCaptionAssertions
    {
        [Serializable] internal sealed class Row
        {
            public string name, text;
            public float fontPixels, meshScale;
            public int lines, visibleCharacters;
            public Rect labelBounds, glyphBounds;
        }
        [Serializable] internal sealed class Record { public Row[] captions; }
        internal static Row Check(Text text, Camera camera, float minimumPixels)
        {
            Assert.That(text.enabled, Is.True, text.name);
            Assert.That(text.text, Is.Not.Empty, text.name);
            Rect label = Bounds(text.rectTransform, camera);
            float font = text.resizeTextForBestFit
                ? text.cachedTextGenerator.fontSizeUsedForBestFit / text.pixelsPerUnit : text.fontSize;
            float pixels = font * label.height / text.rectTransform.rect.height;
            Assert.That(pixels, Is.GreaterThanOrEqualTo(minimumPixels), text.text);
            Assert.That(text.cachedTextGenerator.characterCountVisible,
                Is.GreaterThanOrEqualTo(text.text.Replace("\n", "").Length), text.text + " lost characters");
            Assert.That(text.preferredHeight, Is.LessThanOrEqualTo(text.rectTransform.rect.height + 1), text.text);
            var mesh = text.canvasRenderer.GetMesh();
            Assert.That(mesh.vertexCount, Is.GreaterThan(0), text.text);
            var points = mesh.vertices.Select(vertex => RectTransformUtility.WorldToScreenPoint(
                camera, text.rectTransform.TransformPoint(vertex))).ToArray();
            Rect glyph = Rect.MinMaxRect(points.Min(point => point.x), points.Min(point => point.y),
                points.Max(point => point.x), points.Max(point => point.y));
            Inside(glyph, label, text.text);
            var fit = text.GetComponent<ProductionGameplayCaptionFit>();
            if (fit != null) Assert.That(fit.LastScale, Is.GreaterThanOrEqualTo(.98f), text.text + " silently shrank");
            Assert.That(pixels * (fit != null ? fit.LastScale : 1), Is.GreaterThanOrEqualTo(minimumPixels),
                text.text + " effective glyph scale fell below the measured target");
            return new Row { name = text.name, text = text.text, fontPixels = pixels,
                meshScale = fit != null ? fit.LastScale : 1, lines = text.cachedTextGenerator.lineCount,
                visibleCharacters = text.cachedTextGenerator.characterCountVisible, labelBounds = label, glyphBounds = glyph };
        }
        internal static Rect Bounds(RectTransform rect, Camera camera)
        {
            var corners = new Vector3[4];
            rect.GetWorldCorners(corners);
            var low = RectTransformUtility.WorldToScreenPoint(camera, corners[0]);
            var high = RectTransformUtility.WorldToScreenPoint(camera, corners[2]);
            return Rect.MinMaxRect(low.x, low.y, high.x, high.y);
        }
        internal static void Inside(Rect inner, Rect outer, string message)
        {
            Assert.That(inner.xMin, Is.GreaterThanOrEqualTo(outer.xMin - 1), message);
            Assert.That(inner.yMin, Is.GreaterThanOrEqualTo(outer.yMin - 1), message);
            Assert.That(inner.xMax, Is.LessThanOrEqualTo(outer.xMax + 1), message);
            Assert.That(inner.yMax, Is.LessThanOrEqualTo(outer.yMax + 1), message);
        }
        internal static void Save(string name, IEnumerable<Row> rows)
        {
            string folder = Environment.GetEnvironmentVariable("LOR_CORE_OUTCOME_READABILITY_DIR");
            if (string.IsNullOrEmpty(folder)) return;
            File.WriteAllText(Path.Combine(folder, "CaptionMeasurements_" + name + ".json"),
                JsonUtility.ToJson(new Record { captions = rows.ToArray() }, true));
        }
    }
}
