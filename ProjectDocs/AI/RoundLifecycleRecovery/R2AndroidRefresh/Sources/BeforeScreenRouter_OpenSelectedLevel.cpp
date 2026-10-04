IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool ScreenRouter_OpenSelectedLevel_m77909CC24AC5AEF4490139F276116EF060A4D0FD (ScreenRouter_t9EC367B45B1296F4CF0CBDEA57AEF86CC5340C00* __this, const RuntimeMethod* method) 
{
	{
		bool L_0;
		L_0 = ScreenRouter_get_HasActiveModalBlocker_m158669A3E3F1B8C69362266E5D59A400C78793B0(__this, NULL);
		if (L_0)
		{
			goto IL_0010;
		}
	}
	{
		bool L_1;
		L_1 = ScreenRouter_get_IsScreenTransitioning_m3E031B320BE4B0E736E819DAFD52624DBED47015(__this, NULL);
		if (!L_1)
		{
			goto IL_0012;
		}
	}

IL_0010:
	{
		return (bool)0;
	}

IL_0012:
	{
		GameState_t5E590D2FB317152A4011A3CA69E3F311E0A7393A* L_2;
		L_2 = GameState_get_Instance_m3202061BC5E0A39B53697C35D0BEE15692173D74_inline(NULL);
		NullCheck(L_2);
		bool L_3;
		L_3 = GameState_TryEnsureSelectedLevelIsUnlocked_mB10422023D3F25FEEAFAC46AF1BC583D4F220D90(L_2, NULL);
		if (L_3)
		{
			goto IL_0028;
		}
	}
	{
		bool L_4;
		L_4 = ScreenRouter_ShowErrorDialog_m54E0B54656DDD07A0CC9A5294216CC8A992C4823(__this, (Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07*)NULL, NULL);
		return (bool)0;
	}

IL_0028:
	{
		bool L_5;
		L_5 = StoryService_TryBeginChapterIntro_m88F2D80ECA493ABA02681B5E3DB36F05E7B2CEE6(NULL);
		if (!L_5)
		{
			goto IL_0038;
		}
	}
	{
		ScreenRouter_Show_mB289A1408F5F71A5043EFE930648595D5DCE3734(__this, 6, NULL);
		goto IL_0049;
	}

IL_0038:
	{
		bool L_6;
		L_6 = ScreenRouter_TryShowPendingChapterFlow_mCCA27879A2C00BC2F85FBFE796AD7C1903E64E1E(__this, NULL);
		if (!L_6)
		{
			goto IL_0042;
		}
	}
	{
		return (bool)1;
	}

IL_0042:
	{
		ScreenRouter_Show_mB289A1408F5F71A5043EFE930648595D5DCE3734(__this, 4, NULL);
	}

IL_0049:
	{
		int32_t L_7 = __this->___current;
		return (bool)((((int32_t)((((int32_t)L_7) == ((int32_t)3))? 1 : 0)) == ((int32_t)0))? 1 : 0);
	}
}
