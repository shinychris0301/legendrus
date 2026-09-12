# PrefabFix07 — apply after ColorFix06

Fixes the shipped revision-4 prefab being rejected by ColorFix06 revision-5 code.
Includes the corrected layered prefab with the ColorFix06 layout already serialized.

Stop Play Mode, close Unity, back up, then merge Assets into the current ColorFix06 project.
Reopen and wait for compilation/imports. Run:
Legends of Rus > Production UI > Splash R3 > Validate PrefabFix07
Then play Assets/Scenes/Bootstrap.unity.

No PNG, font configuration, package, Player setting, or scene is replaced by this patch.
All supplied existing .meta files retain their original contents.
No Unity execution has been performed here; validation and package evidence are in:
ProjectDocs/SplashTitle_R3_PrefabFix07/
