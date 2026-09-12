#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace LegendsOfRus.Tests.ProductionCoreLoopUi
{
    public sealed class SplashTitleBuildWiringEditModeTests
    {
        const string PrefabPath = "Assets/LegendsOfRus/Resources/ProductionUI/Pages/SplashTitle.prefab";
        GameObject fixtureRoot;

        [TearDown]
        public void RemoveIsolatedClone()
        {
            if (fixtureRoot != null) Object.DestroyImmediate(fixtureRoot);
        }

        [Test]
        public void SavedCanonicalPrefabPassesBuildWiringValidation()
        {
            GameObject prefab = LoadCanonicalPrefab();
            Assert.That(EditorUtility.IsPersistent(prefab), Is.True);
            Assert.That(Validate(Type.EmptyTypes, new object[0]), Is.Empty);
        }

        [Test]
        public void InactiveCloneRetainsFiveValidButtonArtworkTargets()
        {
            GameObject clone = CreateInactiveClone();
            Button[] buttons = clone.GetComponentsInChildren<Button>(true);
            Assert.That(buttons.Length, Is.EqualTo(5));
            foreach (Button button in buttons)
            {
                Assert.That(button.targetGraphic, Is.Not.Null);
                Assert.That(button.targetGraphic.transform.IsChildOf(button.transform), Is.True);
                Assert.That(button.targetGraphic.raycastTarget, Is.True);
                Assert.That(button.gameObject.activeInHierarchy, Is.False);
            }
            Assert.That(ValidateClone(clone), Is.Empty);
        }

        [Test]
        public void InactiveCloneStillRejectsRaycastDecorationOutsideButtons()
        {
            GameObject clone = CreateInactiveClone();
            Image source = clone.GetComponentInChildren<Image>(true);
            Assert.That(source, Is.Not.Null);
            Assert.That(source.sprite, Is.Not.Null);
            var decoration = new GameObject("StrayDecoration", typeof(RectTransform),
                typeof(CanvasRenderer), typeof(Image));
            decoration.transform.SetParent(clone.transform, false);
            Image image = decoration.GetComponent<Image>();
            image.sprite = source.sprite;
            image.preserveAspect = true;
            image.raycastTarget = true;

            Assert.That(ValidateClone(clone), Is.EquivalentTo(new[]
            {
                "Decorative graphic intercepts input: StrayDecoration"
            }));
        }

        GameObject CreateInactiveClone()
        {
            // Instantiate beneath an inactive parent so no runtime OnEnable callback runs.
            fixtureRoot = new GameObject("SplashWiringTestRoot");
            fixtureRoot.SetActive(false);
            GameObject clone = Object.Instantiate(LoadCanonicalPrefab(), fixtureRoot.transform, false);
            clone.SetActive(false);
            Assert.That(clone.activeInHierarchy, Is.False);
            return clone;
        }

        static GameObject LoadCanonicalPrefab()
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);
            Assert.That(prefab, Is.Not.Null, "The accepted canonical prefab must already exist.");
            return prefab;
        }

        static List<string> ValidateClone(GameObject clone)
        {
            return Validate(new[] { typeof(GameObject) }, new object[] { clone });
        }

        static List<string> Validate(Type[] argumentTypes, object[] arguments)
        {
            // An asmdef test assembly cannot directly reference the predefined Editor assembly.
            Type validator = Type.GetType(
                "LegendsOfRus.Editor.SplashTitleR3Validation, Assembly-CSharp-Editor", true);
            MethodInfo method = validator.GetMethod("ValidateWiring", BindingFlags.Public | BindingFlags.Static,
                null, argumentTypes, null);
            Assert.That(method, Is.Not.Null);
            return (List<string>)method.Invoke(null, arguments);
        }
    }
}
#endif
