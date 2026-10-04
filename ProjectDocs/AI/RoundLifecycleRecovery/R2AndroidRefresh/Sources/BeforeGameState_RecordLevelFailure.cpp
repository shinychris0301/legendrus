IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void GameState_RecordLevelFailure_m03EA2F1F11FB60F1D69DFBFE12EB680248756450 (GameState_t5E590D2FB317152A4011A3CA69E3F311E0A7393A* __this, LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* ___0_levelData, int32_t ___1_score, RuntimeObject* ___2_goalProgress, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&LevelFailureResult_tEAFB9DEC27BA6439583A47C07A1D65D0B79D12E0_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	LevelProgressData_tF4CF8BE6A46189E730098D0C3DEE22CAA57E0C9E* V_0 = NULL;
	{
		bool L_0;
		L_0 = GameState_ContinuationBlocksNewAttempt_m908136F30BB6F308311486AF5F487FAD53014484(__this, NULL);
		if (!L_0)
		{
			goto IL_0013;
		}
	}
	{
		InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB* L_1 = (InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB*)il2cpp_codegen_object_new(((RuntimeClass*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB_il2cpp_TypeInfo_var)));
		InvalidOperationException__ctor_mE4CB6F4712AB6D99A2358FBAE2E052B3EE976162(L_1, ((String_t*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&_stringLiteral0D386D5B4A13C8148A7E5E6982213110317835BA)), NULL);
		IL2CPP_RAISE_MANAGED_EXCEPTION(L_1, ((RuntimeMethod*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&GameState_RecordLevelFailure_m03EA2F1F11FB60F1D69DFBFE12EB680248756450_RuntimeMethod_var)));
	}

IL_0013:
	{
		LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* L_2 = ___0_levelData;
		if (L_2)
		{
			goto IL_0017;
		}
	}
	{
		return;
	}

IL_0017:
	{
		LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* L_3 = ___0_levelData;
		NullCheck(L_3);
		int32_t L_4 = L_3->___chapterId;
		LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* L_5 = ___0_levelData;
		NullCheck(L_5);
		int32_t L_6 = L_5->___levelId;
		bool L_7;
		L_7 = GameState_IsLevelUnlocked_mA15470234C68329950931198C078B05B900F0689(__this, L_4, L_6, NULL);
		if (L_7)
		{
			goto IL_002c;
		}
	}
	{
		return;
	}

IL_002c:
	{
		LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* L_8 = ___0_levelData;
		NullCheck(L_8);
		int32_t L_9 = L_8->___chapterId;
		LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* L_10 = ___0_levelData;
		NullCheck(L_10);
		int32_t L_11 = L_10->___levelId;
		LevelProgressData_tF4CF8BE6A46189E730098D0C3DEE22CAA57E0C9E* L_12;
		L_12 = GameState_GetLevelProgress_mA5B0F87F59C027AFD2DB0B2523B2B811233EF4B1(__this, L_9, L_11, (bool)1, NULL);
		V_0 = L_12;
		LevelProgressData_tF4CF8BE6A46189E730098D0C3DEE22CAA57E0C9E* L_13 = V_0;
		NullCheck(L_13);
		int32_t L_14 = L_13->___attempts;
		if (L_14)
		{
			goto IL_004f;
		}
	}
	{
		LevelProgressData_tF4CF8BE6A46189E730098D0C3DEE22CAA57E0C9E* L_15 = V_0;
		NullCheck(L_15);
		L_15->___attempts = 1;
	}

IL_004f:
	{
		LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* L_16 = ___0_levelData;
		int32_t L_17 = ___1_score;
		RuntimeObject* L_18 = ___2_goalProgress;
		LevelFailureResult_tEAFB9DEC27BA6439583A47C07A1D65D0B79D12E0* L_19 = (LevelFailureResult_tEAFB9DEC27BA6439583A47C07A1D65D0B79D12E0*)il2cpp_codegen_object_new(LevelFailureResult_tEAFB9DEC27BA6439583A47C07A1D65D0B79D12E0_il2cpp_TypeInfo_var);
		LevelFailureResult__ctor_m64326183EAA332B58AAEC8FF65AC41CA53B07559(L_19, L_16, L_17, L_18, NULL);
		GameState_set_LastFailureResult_m93C05E4ECCB370EAD4C1691FEC10E4F59FCC4DCF_inline(__this, L_19, NULL);
		LevelProgressData_tF4CF8BE6A46189E730098D0C3DEE22CAA57E0C9E* L_20 = V_0;
		LevelProgressData_tF4CF8BE6A46189E730098D0C3DEE22CAA57E0C9E* L_21 = L_20;
		NullCheck(L_21);
		int32_t L_22 = L_21->___failures;
		NullCheck(L_21);
		L_21->___failures = ((int32_t)il2cpp_codegen_add(L_22, 1));
		LevelProgressData_tF4CF8BE6A46189E730098D0C3DEE22CAA57E0C9E* L_23 = V_0;
		LevelProgressData_tF4CF8BE6A46189E730098D0C3DEE22CAA57E0C9E* L_24 = V_0;
		NullCheck(L_24);
		int32_t L_25 = L_24->___bestScore;
		int32_t L_26 = ___1_score;
		int32_t L_27;
		L_27 = Mathf_Max_m7FA442918DE37E3A00106D1F2E789D65829792B8_inline(L_25, L_26, NULL);
		NullCheck(L_23);
		L_23->___bestScore = L_27;
		GameState_Persist_m0261476FB722D9A1BAB0BB601B749EFC7E307C8D(__this, NULL);
		return;
	}
}
