from pathlib import Path
ROOT=Path(__file__).resolve().parents[5]
p=ROOT/'Assets/LegendsOfRus/Scripts/Runtime/UI/LevelTutorialPresenter.cs';s=p.read_text().replace('if (!isActiveAndEnabled || CurrentStep == null', 'if (this == null || !isActiveAndEnabled || CurrentStep == null',1);p.write_text(s)
p=ROOT/'Assets/LegendsOfRus/Scripts/Runtime/UI/ScreenRouter.cs';s=p.read_text().replace('        Match3Board activeBoard;', '        Match3Board activeBoard;\n        LevelTutorialPresenter tutorialErrorOwner;',1)
s=s.replace('                    if(IsGameplayInventoryError(activeBoard))CloseGameplayInventoryError(activeBoard);','                    if(tutorialErrorOwner != null)CloseModal();\n                    else if(IsGameplayInventoryError(activeBoard))CloseGameplayInventoryError(activeBoard);',1)
a=s.index('        void ShowGameplayTutorialError(');b=s.index('        bool IsGameplayInventoryError',a);part=s[a:b].replace('            if (!ShowModal(ScreenId.EmptyErrorState)) return;', '            if (!ShowModal(ScreenId.EmptyErrorState)) return;\n            tutorialErrorOwner = presenter;',1);s=s[:a]+part+s[b:]
s=s.replace('''        void RetireModalRootImmediately()
        {
            recapEntries=null;''','''        void RetireModalRootImmediately()
        {
            tutorialErrorOwner = null;
            recapEntries=null;''',1);p.write_text(s)
p=ROOT/'Assets/LegendsOfRus/Tests/PlayMode/TutorialAlignmentPlayModeTests.cs';s=p.read_text();s=s.replace(''' Settle();yield return null;Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.Home));Assert.That(router.ActiveModal,Is.Null);
 }
 [UnityTest] public IEnumerator Sections''',''' Settle();yield return null;Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.Home));Assert.That(router.ActiveModal,Is.Null);
 oldButton.onClick.Invoke();presenter.Advance();Assert.That(JsonUtility.ToJson(state.Save),Is.EqualTo(before));Assert.That(state.LastWriteResult,Is.SameAs(write));Assert.That(router.ActiveModal,Is.Null);
 }
 [UnityTest] public IEnumerator Sections''',1);p.write_text(s)
print('Tutorial modal owner precedes broad inventory lock handling on Back; owner cleared at retirement, destroyed callbacks ignored.')
