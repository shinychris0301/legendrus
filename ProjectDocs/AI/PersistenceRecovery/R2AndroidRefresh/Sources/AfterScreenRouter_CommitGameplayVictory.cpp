IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void ScreenRouter_CommitGameplayVictory_m2C85B5B25AFF53AB58021B96BD2EFB444FACF798 (ScreenRouter_t9EC367B45B1296F4CF0CBDEA57AEF86CC5340C00* __this, Match3Board_tE4CAA58AF06191DC578D60DE5FA7659B8C8ACD69* ___0_board, int32_t ___1_score, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&_stringLiteral836E1BF3FCEE83FCCE50B1BE1D0795D7AE1DA758);
		s_Il2CppMethodInitialized = true;
	}
	LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* V_0 = NULL;
	LevelCompletionResult_t4C786293AEB16558E39DC3DCEEE1542A5487F03E* V_1 = NULL;
	BalanceTelemetryReviewService_tCEB958C4014C08412D40F113D01EC97650E86BE8* G_B6_0 = NULL;
	BalanceTelemetryReviewService_tCEB958C4014C08412D40F113D01EC97650E86BE8* G_B5_0 = NULL;
	{
		Match3Board_tE4CAA58AF06191DC578D60DE5FA7659B8C8ACD69* L_0 = ___0_board;
		bool L_1;
		L_1 = ScreenRouter_IsPendingGameplayVictory_m5F15794588383298DAEC0C365A0B18E172F84F3F(__this, L_0, NULL);
		if (L_1)
		{
			goto IL_000a;
		}
	}
	{
		return;
	}

IL_000a:
	{
		Match3Board_tE4CAA58AF06191DC578D60DE5FA7659B8C8ACD69* L_2 = ___0_board;
		NullCheck(L_2);
		LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* L_3;
		L_3 = Match3Board_get_CurrentLevel_m0E75CF21C6F1053FCB8357B94B7EE1DEA6715420_inline(L_2, NULL);
		V_0 = L_3;
		GameState_t5E590D2FB317152A4011A3CA69E3F311E0A7393A* L_4;
		L_4 = GameState_get_Instance_m3202061BC5E0A39B53697C35D0BEE15692173D74_inline(NULL);
		LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* L_5 = V_0;
		int32_t L_6 = ___1_score;
		NullCheck(L_4);
		bool L_7;
		L_7 = GameState_TryCompleteLevel_m1A62C5C8B0955D4911B88F1A105CAD96B09AEBEC(L_4, L_5, L_6, (&V_1), NULL);
		if (L_7)
		{
			goto IL_0031;
		}
	}
	{
		Match3Board_tE4CAA58AF06191DC578D60DE5FA7659B8C8ACD69* L_8 = ___0_board;
		int32_t L_9 = ___1_score;
		RuntimeObject* L_10;
		L_10 = ScreenRouter_ShowGameplayCompletionError_m6E241E82AD181B5A43D41B82CCB80AAE6C446A67(__this, L_8, L_9, NULL);
		Coroutine_t85EA685566A254C23F3FD77AB5BDFFFF8799596B* L_11;
		L_11 = MonoBehaviour_StartCoroutine_m4CAFF732AA28CD3BDC5363B44A863575530EC812(__this, L_10, NULL);
		return;
	}

IL_0031:
	{
		LevelCompletionResult_t4C786293AEB16558E39DC3DCEEE1542A5487F03E* L_12 = V_1;
		NullCheck(L_12);
		LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* L_13;
		L_13 = LevelCompletionResult_get_Level_m4D6ECBFFE3B38A0EC0FE315BD792F2330C836A62_inline(L_12, NULL);
		NullCheck(L_13);
		String_t* L_14 = L_13->___id;
		int32_t L_15 = __this->___activeAnalyticsAttemptNumber;
		int32_t L_16 = ___1_score;
		LevelCompletionResult_t4C786293AEB16558E39DC3DCEEE1542A5487F03E* L_17 = V_1;
		NullCheck(L_17);
		int32_t L_18;
		L_18 = LevelCompletionResult_get_Stars_mA07535FF05855D521584C54CCC1EDBB542F7F4D0_inline(L_17, NULL);
		LevelCompletionResult_t4C786293AEB16558E39DC3DCEEE1542A5487F03E* L_19 = V_1;
		NullCheck(L_19);
		bool L_20;
		L_20 = LevelCompletionResult_get_FirstClearRewardGranted_mDD0D2CDF571340F385068AA4087B983520B120E5_inline(L_19, NULL);
		AnalyticsRecordResult_tEBE87AEB03C6D60620CF177295FC8B80E2EF350A* L_21;
		L_21 = PrivacyAwareAnalytics_TryLevelComplete_m5E7E9836758F983D8BAA1F9E1460003F046955ED(L_14, L_15, L_16, L_18, L_20, NULL);
		BalanceTelemetryReviewService_tCEB958C4014C08412D40F113D01EC97650E86BE8* L_22;
		L_22 = BalanceTelemetryReviewService_get_Instance_m8BE222FB4502374A70B342B1003DFCB437801EA4_inline(NULL);
		BalanceTelemetryReviewService_tCEB958C4014C08412D40F113D01EC97650E86BE8* L_23 = L_22;
		if (L_23)
		{
			G_B6_0 = L_23;
			goto IL_0060;
		}
		G_B5_0 = L_23;
	}
	{
		goto IL_0074;
	}

IL_0060:
	{
		LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* L_24 = V_0;
		int32_t L_25 = __this->___activeAnalyticsAttemptNumber;
		Match3Board_tE4CAA58AF06191DC578D60DE5FA7659B8C8ACD69* L_26 = ___0_board;
		NullCheck(L_26);
		int32_t L_27;
		L_27 = Match3Board_get_Moves_mD40937FDE23D4EF8D932E7671AF981398FFF8803_inline(L_26, NULL);
		int32_t L_28 = ___1_score;
		NullCheck(G_B6_0);
		bool L_29;
		L_29 = BalanceTelemetryReviewService_TryRecordLevelWin_mCE76E5DAD515D0D06BF9A82D677A05536E997126(G_B6_0, L_24, L_25, L_27, L_28, NULL);
	}

IL_0074:
	{
		ScreenRouter_ClearActiveGameplayAnalytics_m4CF68DB20D999B94D81D051272CF94F5FE02480A(__this, NULL);
		LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* L_30 = V_0;
		ScreenRouter_ShowOutcome_m6317B72F2FCD29C49C03729CF30767467A7A55F0(__this, 7, L_30, _stringLiteral836E1BF3FCEE83FCCE50B1BE1D0795D7AE1DA758, NULL);
		return;
	}
}
