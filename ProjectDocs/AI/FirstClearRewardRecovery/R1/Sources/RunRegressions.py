"""Run existing fixture groups sequentially in their required isolated stores."""
from pathlib import Path
import subprocess
import sys

BASE = Path(__file__).resolve().parent.parent
run = BASE / 'Sources/Run.py'
groups = [
    ('Retained', 'LegendsOfRus.Tests.PlayMode.ContinuationTransactionPlayModeTests;LegendsOfRus.Tests.PlayMode.RetainedRoundPlayModeTests', None),
    ('Error', 'LegendsOfRus.Tests.PlayMode.ErrorDialogAlignmentPlayModeTests', None),
    ('Journeys', 'LegendsOfRus.Tests.PlayMode.FunctionalDemoNavigationPlayModeTests.NativeJourneyWinsAndReturnsHome;LegendsOfRus.Tests.PlayMode.FunctionalDemoNavigationPlayModeTests.NativeJourneyFailsRetriesAndExitsHome', None),
    ('Recap', ';'.join('LegendsOfRus.Tests.PlayMode.StoryRecapPlayModeTests.' + name for name in (
        'EmptyHistoryIsTruthfulAndCannotOpenReader', 'NativeReaderBrowsesAndReturnsWithoutChangingStoryOrJournal',
        'IntroRepeatedNativeTapAdvancesOneLineAndBackCloses', 'FinaleFinishAndRepeatedReentryNeverDispatchChapterComplete',
        'DepartureRetiresReaderWithoutStaleCallbacks')), None),
    ('RecapQA', 'LegendsOfRus.Tests.PlayMode.StoryRecapPlayModeTests.DisposableRecapPreservesSyntheticNormalJournal', BASE / 'QA/OptionsQA.json'),
]
for name, test_filter, options in groups:
    command = [sys.executable, str(run), name, 'PlayMode', test_filter]
    if options:
        command.append(str(options))
    subprocess.run(command, check=True)
