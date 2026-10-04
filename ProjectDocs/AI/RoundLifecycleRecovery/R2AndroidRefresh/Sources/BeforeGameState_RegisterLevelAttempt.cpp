IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t GameState_RegisterLevelAttempt_m31F23CCF55EB0C2270E2E0376980F8F61C4F9504 (GameState_t5E590D2FB317152A4011A3CA69E3F311E0A7393A* __this, LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* ___0_levelData, const RuntimeMethod* method) 
{
	LevelProgressData_tF4CF8BE6A46189E730098D0C3DEE22CAA57E0C9E* V_0 = NULL;
	RetainedRoundData_t3EA1BA932DFF2408098F1FB87EC5A5A24130D7A1* V_1 = NULL;
	{
		LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* L_0 = ___0_levelData;
		if (L_0)
		{
			goto IL_0005;
		}
	}
	{
		return 0;
	}

IL_0005:
	{
		bool L_1;
		L_1 = GameState_ContinuationBlocksNewAttempt_m908136F30BB6F308311486AF5F487FAD53014484(__this, NULL);
		if (!L_1)
		{
			goto IL_0018;
		}
	}
	{
		InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB* L_2 = (InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB*)il2cpp_codegen_object_new(((RuntimeClass*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB_il2cpp_TypeInfo_var)));
		InvalidOperationException__ctor_mE4CB6F4712AB6D99A2358FBAE2E052B3EE976162(L_2, ((String_t*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&_stringLiteral5E7774E9CC83E43BB29365F9F0CC10B783406B7C)), NULL);
		IL2CPP_RAISE_MANAGED_EXCEPTION(L_2, ((RuntimeMethod*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&GameState_RegisterLevelAttempt_m31F23CCF55EB0C2270E2E0376980F8F61C4F9504_RuntimeMethod_var)));
	}

IL_0018:
	{
		bool L_3;
		L_3 = GameState_TryGetRetainedRound_m2295FF12D42D33C9E7E1FF1D8B33B8E3859793A5(__this, (&V_1), NULL);
		if (!L_3)
		{
			goto IL_002d;
		}
	}
	{
		InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB* L_4 = (InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB*)il2cpp_codegen_object_new(((RuntimeClass*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB_il2cpp_TypeInfo_var)));
		InvalidOperationException__ctor_mE4CB6F4712AB6D99A2358FBAE2E052B3EE976162(L_4, ((String_t*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&_stringLiteral166980D7EB4EE9E05FE89E514C01D64A754D86C0)), NULL);
		IL2CPP_RAISE_MANAGED_EXCEPTION(L_4, ((RuntimeMethod*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&GameState_RegisterLevelAttempt_m31F23CCF55EB0C2270E2E0376980F8F61C4F9504_RuntimeMethod_var)));
	}

IL_002d:
	{
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_5 = __this->___Save;
		NullCheck(L_5);
		L_5->___retainedRound = (RetainedRoundData_t3EA1BA932DFF2408098F1FB87EC5A5A24130D7A1*)NULL;
		Il2CppCodeGenWriteBarrier((void**)(&L_5->___retainedRound), (void*)(RetainedRoundData_t3EA1BA932DFF2408098F1FB87EC5A5A24130D7A1*)NULL);
		__this->___unsavedRetainedRound = (RetainedRoundData_t3EA1BA932DFF2408098F1FB87EC5A5A24130D7A1*)NULL;
		Il2CppCodeGenWriteBarrier((void**)(&__this->___unsavedRetainedRound), (void*)(RetainedRoundData_t3EA1BA932DFF2408098F1FB87EC5A5A24130D7A1*)NULL);
		LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* L_6 = ___0_levelData;
		NullCheck(L_6);
		int32_t L_7 = L_6->___chapterId;
		LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* L_8 = ___0_levelData;
		NullCheck(L_8);
		int32_t L_9 = L_8->___levelId;
		bool L_10;
		L_10 = GameState_IsLevelUnlocked_mA15470234C68329950931198C078B05B900F0689(__this, L_7, L_9, NULL);
		if (L_10)
		{
			goto IL_0056;
		}
	}
	{
		return 0;
	}

IL_0056:
	{
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_11 = __this->___Save;
		LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* L_12 = ___0_levelData;
		NullCheck(L_12);
		int32_t L_13 = L_12->___chapterId;
		NullCheck(L_11);
		L_11->___selectedChapter = L_13;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_14 = __this->___Save;
		LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* L_15 = ___0_levelData;
		NullCheck(L_15);
		int32_t L_16 = L_15->___levelId;
		NullCheck(L_14);
		L_14->___selectedLevel = L_16;
		LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* L_17 = ___0_levelData;
		NullCheck(L_17);
		int32_t L_18 = L_17->___chapterId;
		LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* L_19 = ___0_levelData;
		NullCheck(L_19);
		int32_t L_20 = L_19->___levelId;
		LevelProgressData_tF4CF8BE6A46189E730098D0C3DEE22CAA57E0C9E* L_21;
		L_21 = GameState_GetLevelProgress_mA5B0F87F59C027AFD2DB0B2523B2B811233EF4B1(__this, L_18, L_20, (bool)1, NULL);
		V_0 = L_21;
		LevelProgressData_tF4CF8BE6A46189E730098D0C3DEE22CAA57E0C9E* L_22 = V_0;
		LevelProgressData_tF4CF8BE6A46189E730098D0C3DEE22CAA57E0C9E* L_23 = L_22;
		NullCheck(L_23);
		int32_t L_24 = L_23->___attempts;
		NullCheck(L_23);
		L_23->___attempts = ((int32_t)il2cpp_codegen_add(L_24, 1));
		LevelProgressData_tF4CF8BE6A46189E730098D0C3DEE22CAA57E0C9E* L_25 = V_0;
		NullCheck(L_25);
		int32_t L_26 = L_25->___attempts;
		if ((((int32_t)L_26) <= ((int32_t)1)))
		{
			goto IL_00b1;
		}
	}
	{
		LevelProgressData_tF4CF8BE6A46189E730098D0C3DEE22CAA57E0C9E* L_27 = V_0;
		LevelProgressData_tF4CF8BE6A46189E730098D0C3DEE22CAA57E0C9E* L_28 = L_27;
		NullCheck(L_28);
		int32_t L_29 = L_28->___retries;
		NullCheck(L_28);
		L_28->___retries = ((int32_t)il2cpp_codegen_add(L_29, 1));
	}

IL_00b1:
	{
		GameState_Persist_m0261476FB722D9A1BAB0BB601B749EFC7E307C8D(__this, NULL);
		LevelProgressData_tF4CF8BE6A46189E730098D0C3DEE22CAA57E0C9E* L_30 = V_0;
		NullCheck(L_30);
		int32_t L_31 = L_30->___attempts;
		return L_31;
	}
}
