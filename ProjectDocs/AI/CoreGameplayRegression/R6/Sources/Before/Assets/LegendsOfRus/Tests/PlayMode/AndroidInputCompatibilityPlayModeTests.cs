using System;
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.UI;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace LegendsOfRus.Tests.PlayMode
{
    public sealed class AndroidInputCompatibilityPlayModeTests
    {
        [Test]
        public void PlayerUsesOnlyNewInputBackend()
        {
#if !ENABLE_INPUT_SYSTEM
            Assert.Fail("The new Input System must be enabled.");
#endif
#if ENABLE_LEGACY_INPUT_MANAGER
            Assert.Fail("Legacy Input Manager must not be compiled as an active backend.");
#endif
            Assert.That(ScreenRouter.Instance, Is.Not.Null);
            Assert.That(UnityEngine.Object.FindObjectsByType<EventSystem>(FindObjectsSortMode.None).Length, Is.EqualTo(1));
            Assert.That(EventSystem.current.GetComponent<InputSystemUIInputModule>(), Is.Not.Null);
            Assert.That(EventSystem.current.GetComponent<StandaloneInputModule>(), Is.Null);
        }

        [UnityTest]
        public IEnumerator QueuedTouchMouseAndEscapeUseNativeUiWithoutLegacyInput()
        {
            string config = Environment.GetEnvironmentVariable("LOR_LOADING_TEST_CONFIG_ROOT");
            Assert.That(config, Does.StartWith("/tmp/lor-android-input-r1/"));
            Assert.That(Environment.GetEnvironmentVariable("XDG_CONFIG_HOME"), Is.EqualTo(config));
            Assert.That(Application.persistentDataPath, Does.StartWith(config + "/"));
            var router = ScreenRouter.Instance;
            Assert.That(router, Is.Not.Null);
            // Headless Editor verification has no focused Game view. Override only
            // the test session's settings; production focus policy is unchanged.
            var originalSettings = InputSystem.settings;
            var originalFlags = originalSettings.hideFlags;
            // InputManager destroys its temporary default on replacement unless retained.
            if (originalFlags == HideFlags.HideAndDontSave) originalSettings.hideFlags = HideFlags.DontSave;
            var testSettings = UnityEngine.Object.Instantiate(originalSettings);
            testSettings.hideFlags = HideFlags.HideAndDontSave;
            testSettings.backgroundBehavior = InputSettings.BackgroundBehavior.IgnoreFocus;
            testSettings.editorInputBehaviorInPlayMode = InputSettings.EditorInputBehaviorInPlayMode.AllDeviceInputAlwaysGoesToGameView;
            InputSystem.settings = testSettings;
            var touch = InputSystem.AddDevice<Touchscreen>();
            var mouse = InputSystem.AddDevice<Mouse>();
            var keyboard = InputSystem.AddDevice<Keyboard>();
            int energy = GameState.Instance.Save.energy, gold = GameState.Instance.Save.gold;
            try
            {
                router.Show(ScreenId.Home);
                yield return new WaitForSecondsRealtime(0.8f);
                Vector2 point = ButtonCenter(router);
                InputSystem.QueueStateEvent(touch, new TouchState { touchId = 1, position = point,
                    phase = UnityEngine.InputSystem.TouchPhase.Began, pressure = 1 });
                yield return new WaitForSecondsRealtime(0.1f);
                InputSystem.QueueStateEvent(touch, new TouchState { touchId = 1, position = point,
                    phase = UnityEngine.InputSystem.TouchPhase.Ended });
                yield return new WaitForSecondsRealtime(0.8f);
                Assert.That(router.CurrentScreen, Is.EqualTo(ScreenId.WorldMap), "Queued touch must hit the real Home map button.");

                InputSystem.QueueStateEvent(keyboard, new KeyboardState(Key.Escape));
                yield return new WaitForSecondsRealtime(0.1f);
                InputSystem.QueueStateEvent(keyboard, new KeyboardState());
                yield return new WaitForSecondsRealtime(0.8f);
                Assert.That(router.CurrentScreen, Is.EqualTo(ScreenId.Home), "Escape uses the existing Back route.");

                point = ButtonCenter(router);
                InputSystem.QueueStateEvent(mouse, new MouseState { position = point });
                yield return null;
                InputSystem.QueueStateEvent(mouse, new MouseState { position = point }.WithButton(MouseButton.Left));
                yield return new WaitForSecondsRealtime(0.1f);
                InputSystem.QueueStateEvent(mouse, new MouseState { position = point });
                yield return new WaitForSecondsRealtime(0.8f);
                Assert.That(router.CurrentScreen, Is.EqualTo(ScreenId.WorldMap), "Queued mouse must retain Editor navigation.");
                Assert.That(GameState.Instance.Save.energy, Is.EqualTo(energy));
                Assert.That(GameState.Instance.Save.gold, Is.EqualTo(gold));
            }
            finally
            {
                InputSystem.RemoveDevice(touch);
                InputSystem.RemoveDevice(mouse);
                InputSystem.RemoveDevice(keyboard);
                InputSystem.settings = originalSettings;
                originalSettings.hideFlags = originalFlags;
                if (testSettings != null) UnityEngine.Object.Destroy(testSettings);
                router.Show(ScreenId.Home);
            }
        }

        static Vector2 ButtonCenter(ScreenRouter router)
        {
            var view = router.Viewport.GetComponentInChildren<ProductionHomeView>();
            Assert.That(view, Is.Not.Null);
            Button button = view.WorldMapButton;
            Assert.That(button.IsInteractable(), Is.True);
            Canvas.ForceUpdateCanvases();
            var rect = button.GetComponent<RectTransform>();
            var canvas = router.GameCanvas;
            return RectTransformUtility.WorldToScreenPoint(
                canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
                rect.TransformPoint(rect.rect.center));
        }
    }
}
