using System;
using System.Collections;
using System.IO;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
namespace LegendsOfRus.Tests.PlayMode
{
    public sealed class LanguagePreferenceRestartPlayModeTests
    {
        [UnityTest] public IEnumerator DevicePreferenceSurvivesFreshUnityProcess()
        {
            string isolated=Environment.GetEnvironmentVariable("LOR_LOADING_TEST_CONFIG_ROOT");
            string phase=Environment.GetEnvironmentVariable("LOR_LANGUAGE_RESTART_PHASE");
            if(string.IsNullOrEmpty(isolated)||string.IsNullOrEmpty(phase))Assert.Ignore("Requires dedicated isolated two-process check.");
            Assert.That(Environment.GetEnvironmentVariable("XDG_CONFIG_HOME"),Is.EqualTo(isolated));
            Assert.That(Path.GetFullPath(Application.persistentDataPath).StartsWith(Path.GetFullPath(isolated)+Path.DirectorySeparatorChar),Is.True);
            if(phase=="write")
            {
                PlayerPrefs.DeleteKey(LanguagePreference.Key);DemoFeatureState.ResetSession();
                Assert.That(DemoFeatureState.LanguageCode,Is.EqualTo("ru"));
                DemoFeatureState.SelectLanguage("en");Assert.That(PlayerPrefs.GetString(LanguagePreference.Key),Is.EqualTo("en"));
            }
            else
            {
                Assert.That(phase,Is.EqualTo("read"));
                Assert.That(LanguagePreference.Load(),Is.EqualTo("en"));
                Assert.That(DemoFeatureState.LanguageCode,Is.EqualTo("en"),"Fresh process reads saved language before any user selection.");
                ScreenRouter.Instance.Show(ScreenId.SplashTitle);
                for(int i=0;i<24;i++)ScreenRouter.Instance.MotionPresenter.TickForTesting(MetaUiMotionProfile.MaximumTickSeconds);
                yield return null;
                var v=ScreenRouter.Instance.Viewport.GetComponentInChildren<SplashTitleView>();
                Assert.That(v.LanguageText.text,Is.EqualTo("English"));
            }
            yield return null;
        }
    }
}
