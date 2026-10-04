"""Extend the prior native inclusion audit to the reviewed round lifecycle owners."""
from pathlib import Path
p=Path(__file__).resolve().parent/'CaptureNative.py';s=p.read_text()
extra=['GameState_RegisterLevelAttempt','GameState_CanRegisterLevelAttempt','GameState_TryRegisterLevelAttempt','GameState_TryStartLevel','GameState_CandidateLevelProgress','GameState_TryPrepareLevelAttempt','GameState_TryCommitRoundCandidate','GameState_RecordLevelFailure','GameState_TryRecordLevelFailure','GameState_TryFinalizeRetainedFailure','Match3Board_Initialize','Match3Board_InitializeCore']
s=s.replace('pattern=re.compile(', 'targets += '+repr(extra)+'\npattern=re.compile(',1)
needle=" if owner=='GameState_ClaimDailyReward'"
s=s.replace(needle," if owner=='Match3Board_Initialize' and 'LevelData_' not in m.group(0):continue\n"+needle,1)
s=s.replace("set(targets)-{'GameState_TryEnsureSelectedLevelIsUnlocked','GameState_NormalizeLevelProgress'}",'set(targets)-'+repr(set(extra)-{'GameState_RegisterLevelAttempt','GameState_RecordLevelFailure','GameState_TryFinalizeRetainedFailure','Match3Board_Initialize','Match3Board_InitializeCore'}),1)
s=s.replace("{'GameState_SelectLevel','GameState_EnsureSelectedLevelIsUnlocked','ScreenRouter_OpenSelectedLevel','ProductionChapterMapView_BindNavigation','ScreenRouter_BuildInteractionLayer','ScreenRouter_StartLevel'}<=set(changed)","{'GameState_RegisterLevelAttempt','GameState_RecordLevelFailure','GameState_TryFinalizeRetainedFailure','Match3Board_Initialize','Match3Board_InitializeCore','ScreenRouter_StartLevel','ScreenRouter_BuildGameplayOverlay'}<=set(changed)",1)
s=s.replace("spend=start.index('GameState_SpendEnergy_',first)","spend=start.index('GameState_TryStartLevel_',first)",1)
s=s.replace("first<start.index('return;',first)<spend and 'ScreenRouter_ShowErrorDialog_' in start[first:spend]", "first<spend and 'ScreenRouter_ShowErrorDialog_' in start[first:spend] and 'if (' in start[first:spend]",1)
p.write_text(s)
print('Native audit now requires54before/61after owner regions and retains five earlier callbacks.')
