IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void ScreenRouter_OpenSelectedLevel_m5D09F1E395BAFA1FA8463BD96E5EE28254CFA62E (ScreenRouter_t9EC367B45B1296F4CF0CBDEA57AEF86CC5340C00* __this, const RuntimeMethod* method) 
{
	{
		GameState_t5E590D2FB317152A4011A3CA69E3F311E0A7393A* L_0;
		L_0 = GameState_get_Instance_m3202061BC5E0A39B53697C35D0BEE15692173D74_inline(NULL);
		NullCheck(L_0);
		GameState_EnsureSelectedLevelIsUnlocked_mE9340A0362BC59847C851690ECA88155190F656E(L_0, NULL);
		bool L_1;
		L_1 = StoryService_TryBeginChapterIntro_m88F2D80ECA493ABA02681B5E3DB36F05E7B2CEE6(NULL);
		if (!L_1)
		{
			goto IL_0019;
		}
	}
	{
		ScreenRouter_Show_mB289A1408F5F71A5043EFE930648595D5DCE3734(__this, 6, NULL);
		return;
	}

IL_0019:
	{
		bool L_2;
		L_2 = ScreenRouter_TryShowPendingChapterFlow_mCCA27879A2C00BC2F85FBFE796AD7C1903E64E1E(__this, NULL);
		if (!L_2)
		{
			goto IL_0022;
		}
	}
	{
		return;
	}

IL_0022:
	{
		ScreenRouter_Show_mB289A1408F5F71A5043EFE930648595D5DCE3734(__this, 4, NULL);
		return;
	}
}
