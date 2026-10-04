IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool GameState_TryRecordLevelFailure_m8C0D994176FC03D3AD8E5A9B60CD8B1B0E8A3DC6 (GameState_t5E590D2FB317152A4011A3CA69E3F311E0A7393A* __this, LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* ___0_levelData, int32_t ___1_score, RuntimeObject* ___2_goalProgress, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&GameState_TryRecordLevelFailure_m8C0D994176FC03D3AD8E5A9B60CD8B1B0E8A3DC6_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&JsonUtility_FromJson_TisPlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30_m5FA532D489BA59FC0DD187FA87D56E71F1F63B04_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&LevelFailureResult_tEAFB9DEC27BA6439583A47C07A1D65D0B79D12E0_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* V_0 = NULL;
	LevelProgressData_tF4CF8BE6A46189E730098D0C3DEE22CAA57E0C9E* V_1 = NULL;
	bool V_2 = false;
	il2cpp::utils::ExceptionSupportStack<RuntimeObject*, 1> __active_exceptions;
	{
		bool L_0 = __this->___canPersist;
		if (L_0)
		{
			goto IL_000a;
		}
	}
	{
		return (bool)0;
	}

IL_000a:
	{
		bool L_1;
		L_1 = GameState_ContinuationBlocksNewAttempt_m908136F30BB6F308311486AF5F487FAD53014484(__this, NULL);
		if (!L_1)
		{
			goto IL_001d;
		}
	}
	{
		InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB* L_2 = (InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB*)il2cpp_codegen_object_new(((RuntimeClass*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB_il2cpp_TypeInfo_var)));
		InvalidOperationException__ctor_mE4CB6F4712AB6D99A2358FBAE2E052B3EE976162(L_2, ((String_t*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&_stringLiteral0D386D5B4A13C8148A7E5E6982213110317835BA)), NULL);
		IL2CPP_RAISE_MANAGED_EXCEPTION(L_2, ((RuntimeMethod*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&GameState_TryRecordLevelFailure_m8C0D994176FC03D3AD8E5A9B60CD8B1B0E8A3DC6_RuntimeMethod_var)));
	}

IL_001d:
	{
		LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* L_3 = ___0_levelData;
		if (!L_3)
		{
			goto IL_0034;
		}
	}
	{
		LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* L_4 = ___0_levelData;
		NullCheck(L_4);
		int32_t L_5 = L_4->___chapterId;
		LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* L_6 = ___0_levelData;
		NullCheck(L_6);
		int32_t L_7 = L_6->___levelId;
		bool L_8;
		L_8 = GameState_IsLevelUnlocked_mA15470234C68329950931198C078B05B900F0689(__this, L_5, L_7, NULL);
		if (L_8)
		{
			goto IL_0036;
		}
	}

IL_0034:
	{
		return (bool)0;
	}

IL_0036:
	{
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_9 = __this->___Save;
		String_t* L_10;
		L_10 = JsonUtility_ToJson_m28CC6843B9D3723D88AD13EA3829B71FDE7826BA(L_9, NULL);
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_11;
		L_11 = JsonUtility_FromJson_TisPlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30_m5FA532D489BA59FC0DD187FA87D56E71F1F63B04(L_10, JsonUtility_FromJson_TisPlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30_m5FA532D489BA59FC0DD187FA87D56E71F1F63B04_RuntimeMethod_var);
		V_0 = L_11;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_12 = V_0;
		LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* L_13 = ___0_levelData;
		LevelProgressData_tF4CF8BE6A46189E730098D0C3DEE22CAA57E0C9E* L_14;
		L_14 = GameState_CandidateLevelProgress_m9EFEA76AC5DFD65128554F397E4B8D4161544E2D(L_12, L_13, NULL);
		V_1 = L_14;
		LevelProgressData_tF4CF8BE6A46189E730098D0C3DEE22CAA57E0C9E* L_15 = V_1;
		NullCheck(L_15);
		int32_t L_16 = L_15->___attempts;
		if (L_16)
		{
			goto IL_005e;
		}
	}
	{
		LevelProgressData_tF4CF8BE6A46189E730098D0C3DEE22CAA57E0C9E* L_17 = V_1;
		NullCheck(L_17);
		L_17->___attempts = 1;
	}

IL_005e:
	{
	}
	try
	{
		LevelProgressData_tF4CF8BE6A46189E730098D0C3DEE22CAA57E0C9E* L_18 = V_1;
		LevelProgressData_tF4CF8BE6A46189E730098D0C3DEE22CAA57E0C9E* L_19 = V_1;
		NullCheck(L_19);
		int32_t L_20 = L_19->___failures;
		if (((int64_t)L_20 + (int64_t)1 < (int64_t)kIl2CppInt32Min) || ((int64_t)L_20 + (int64_t)1 > (int64_t)kIl2CppInt32Max))
			IL2CPP_RAISE_MANAGED_EXCEPTION(il2cpp_codegen_get_overflow_exception(), GameState_TryRecordLevelFailure_m8C0D994176FC03D3AD8E5A9B60CD8B1B0E8A3DC6_RuntimeMethod_var);
		NullCheck(L_18);
		L_18->___failures = ((int32_t)il2cpp_codegen_add(L_20, 1));
		goto IL_0074;
	}
	catch(Il2CppExceptionWrapper& e)
	{
		if(il2cpp_codegen_class_is_assignable_from (((RuntimeClass*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&OverflowException_t6F6AD8CACE20C37F701C05B373A215C4802FAB0C_il2cpp_TypeInfo_var)), il2cpp_codegen_object_class(e.ex)))
		{
			IL2CPP_PUSH_ACTIVE_EXCEPTION(e.ex);
			goto CATCH_006f;
		}
		throw e;
	}

CATCH_006f:
	{
		OverflowException_t6F6AD8CACE20C37F701C05B373A215C4802FAB0C* L_21 = ((OverflowException_t6F6AD8CACE20C37F701C05B373A215C4802FAB0C*)IL2CPP_GET_ACTIVE_EXCEPTION(OverflowException_t6F6AD8CACE20C37F701C05B373A215C4802FAB0C*));;
		V_2 = (bool)0;
		IL2CPP_POP_ACTIVE_EXCEPTION(Exception_t*);
		goto IL_0097;
	}

IL_0074:
	{
		LevelProgressData_tF4CF8BE6A46189E730098D0C3DEE22CAA57E0C9E* L_22 = V_1;
		LevelProgressData_tF4CF8BE6A46189E730098D0C3DEE22CAA57E0C9E* L_23 = V_1;
		NullCheck(L_23);
		int32_t L_24 = L_23->___bestScore;
		int32_t L_25 = ___1_score;
		int32_t L_26;
		L_26 = Mathf_Max_m7FA442918DE37E3A00106D1F2E789D65829792B8_inline(L_24, L_25, NULL);
		NullCheck(L_22);
		L_22->___bestScore = L_26;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_27 = V_0;
		LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* L_28 = ___0_levelData;
		int32_t L_29 = ___1_score;
		RuntimeObject* L_30 = ___2_goalProgress;
		LevelFailureResult_tEAFB9DEC27BA6439583A47C07A1D65D0B79D12E0* L_31 = (LevelFailureResult_tEAFB9DEC27BA6439583A47C07A1D65D0B79D12E0*)il2cpp_codegen_object_new(LevelFailureResult_tEAFB9DEC27BA6439583A47C07A1D65D0B79D12E0_il2cpp_TypeInfo_var);
		LevelFailureResult__ctor_m64326183EAA332B58AAEC8FF65AC41CA53B07559(L_31, L_28, L_29, L_30, NULL);
		bool L_32;
		L_32 = GameState_TryCommitRoundCandidate_mF567FE151D88B0B07E7F938F7534DF807FD76CE9(__this, L_27, L_31, (bool)0, NULL);
		return L_32;
	}

IL_0097:
	{
		bool L_33 = V_2;
		return L_33;
	}
}
