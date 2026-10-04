IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool GameState_TryPrepareLevelAttempt_m1B62820945E8D99C9C474070E9DC25AE1ADDFB50 (PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* ___0_candidate, LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* ___1_levelData, int32_t* ___2_attempt, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&GameState_TryPrepareLevelAttempt_m1B62820945E8D99C9C474070E9DC25AE1ADDFB50_RuntimeMethod_var);
		s_Il2CppMethodInitialized = true;
	}
	LevelProgressData_tF4CF8BE6A46189E730098D0C3DEE22CAA57E0C9E* V_0 = NULL;
	bool V_1 = false;
	il2cpp::utils::ExceptionSupportStack<RuntimeObject*, 1> __active_exceptions;
	{
		int32_t* L_0 = ___2_attempt;
		il2cpp_codegen_stind<int32_t>((int32_t*)L_0, (int32_t)0);
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_1 = ___0_candidate;
		NullCheck(L_1);
		L_1->___retainedRound = (RetainedRoundData_t3EA1BA932DFF2408098F1FB87EC5A5A24130D7A1*)NULL;
		Il2CppCodeGenWriteBarrier((void**)(&L_1->___retainedRound), (void*)(RetainedRoundData_t3EA1BA932DFF2408098F1FB87EC5A5A24130D7A1*)NULL);
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_2 = ___0_candidate;
		LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* L_3 = ___1_levelData;
		NullCheck(L_3);
		int32_t L_4 = L_3->___chapterId;
		NullCheck(L_2);
		L_2->___selectedChapter = L_4;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_5 = ___0_candidate;
		LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* L_6 = ___1_levelData;
		NullCheck(L_6);
		int32_t L_7 = L_6->___levelId;
		NullCheck(L_5);
		L_5->___selectedLevel = L_7;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_8 = ___0_candidate;
		LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* L_9 = ___1_levelData;
		LevelProgressData_tF4CF8BE6A46189E730098D0C3DEE22CAA57E0C9E* L_10;
		L_10 = GameState_CandidateLevelProgress_m9EFEA76AC5DFD65128554F397E4B8D4161544E2D(L_8, L_9, NULL);
		V_0 = L_10;
	}
	try
	{
		{
			LevelProgressData_tF4CF8BE6A46189E730098D0C3DEE22CAA57E0C9E* L_11 = V_0;
			LevelProgressData_tF4CF8BE6A46189E730098D0C3DEE22CAA57E0C9E* L_12 = V_0;
			NullCheck(L_12);
			int32_t L_13 = L_12->___attempts;
			if (((int64_t)L_13 + (int64_t)1 < (int64_t)kIl2CppInt32Min) || ((int64_t)L_13 + (int64_t)1 > (int64_t)kIl2CppInt32Max))
				IL2CPP_RAISE_MANAGED_EXCEPTION(il2cpp_codegen_get_overflow_exception(), GameState_TryPrepareLevelAttempt_m1B62820945E8D99C9C474070E9DC25AE1ADDFB50_RuntimeMethod_var);
			NullCheck(L_11);
			L_11->___attempts = ((int32_t)il2cpp_codegen_add(L_13, 1));
			LevelProgressData_tF4CF8BE6A46189E730098D0C3DEE22CAA57E0C9E* L_14 = V_0;
			NullCheck(L_14);
			int32_t L_15 = L_14->___attempts;
			if ((((int32_t)L_15) <= ((int32_t)1)))
			{
				goto IL_004f_1;
			}
		}
		{
			LevelProgressData_tF4CF8BE6A46189E730098D0C3DEE22CAA57E0C9E* L_16 = V_0;
			LevelProgressData_tF4CF8BE6A46189E730098D0C3DEE22CAA57E0C9E* L_17 = V_0;
			NullCheck(L_17);
			int32_t L_18 = L_17->___retries;
			if (((int64_t)L_18 + (int64_t)1 < (int64_t)kIl2CppInt32Min) || ((int64_t)L_18 + (int64_t)1 > (int64_t)kIl2CppInt32Max))
				IL2CPP_RAISE_MANAGED_EXCEPTION(il2cpp_codegen_get_overflow_exception(), GameState_TryPrepareLevelAttempt_m1B62820945E8D99C9C474070E9DC25AE1ADDFB50_RuntimeMethod_var);
			NullCheck(L_16);
			L_16->___retries = ((int32_t)il2cpp_codegen_add(L_18, 1));
		}

IL_004f_1:
		{
			goto IL_0056;
		}
	}
	catch(Il2CppExceptionWrapper& e)
	{
		if(il2cpp_codegen_class_is_assignable_from (((RuntimeClass*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&OverflowException_t6F6AD8CACE20C37F701C05B373A215C4802FAB0C_il2cpp_TypeInfo_var)), il2cpp_codegen_object_class(e.ex)))
		{
			IL2CPP_PUSH_ACTIVE_EXCEPTION(e.ex);
			goto CATCH_0051;
		}
		throw e;
	}

CATCH_0051:
	{
		OverflowException_t6F6AD8CACE20C37F701C05B373A215C4802FAB0C* L_19 = ((OverflowException_t6F6AD8CACE20C37F701C05B373A215C4802FAB0C*)IL2CPP_GET_ACTIVE_EXCEPTION(OverflowException_t6F6AD8CACE20C37F701C05B373A215C4802FAB0C*));;
		V_1 = (bool)0;
		IL2CPP_POP_ACTIVE_EXCEPTION(Exception_t*);
		goto IL_0064;
	}

IL_0056:
	{
		int32_t* L_20 = ___2_attempt;
		LevelProgressData_tF4CF8BE6A46189E730098D0C3DEE22CAA57E0C9E* L_21 = V_0;
		NullCheck(L_21);
		int32_t L_22 = L_21->___attempts;
		il2cpp_codegen_stind<int32_t>((int32_t*)L_20, (int32_t)L_22);
		int32_t* L_23 = ___2_attempt;
		int32_t L_24 = il2cpp_codegen_ldind<int32_t, int32_t>(L_23);
		return (bool)((((int32_t)L_24) > ((int32_t)0))? 1 : 0);
	}

IL_0064:
	{
		bool L_25 = V_1;
		return L_25;
	}
}
