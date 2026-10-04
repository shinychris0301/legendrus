IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool GameState_CanRegisterLevelAttempt_m509E53F6E5D8D3EC951AD4C656B4C83BE02A853A (GameState_t5E590D2FB317152A4011A3CA69E3F311E0A7393A* __this, LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* ___0_levelData, const RuntimeMethod* method) 
{
	RetainedRoundData_t3EA1BA932DFF2408098F1FB87EC5A5A24130D7A1* V_0 = NULL;
	{
		LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* L_0 = ___0_levelData;
		if (!L_0)
		{
			goto IL_000b;
		}
	}
	{
		bool L_1 = __this->___canPersist;
		if (L_1)
		{
			goto IL_000d;
		}
	}

IL_000b:
	{
		return (bool)0;
	}

IL_000d:
	{
		bool L_2;
		L_2 = GameState_ContinuationBlocksNewAttempt_m908136F30BB6F308311486AF5F487FAD53014484(__this, NULL);
		if (!L_2)
		{
			goto IL_0020;
		}
	}
	{
		InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB* L_3 = (InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB*)il2cpp_codegen_object_new(((RuntimeClass*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB_il2cpp_TypeInfo_var)));
		InvalidOperationException__ctor_mE4CB6F4712AB6D99A2358FBAE2E052B3EE976162(L_3, ((String_t*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&_stringLiteral5E7774E9CC83E43BB29365F9F0CC10B783406B7C)), NULL);
		IL2CPP_RAISE_MANAGED_EXCEPTION(L_3, ((RuntimeMethod*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&GameState_CanRegisterLevelAttempt_m509E53F6E5D8D3EC951AD4C656B4C83BE02A853A_RuntimeMethod_var)));
	}

IL_0020:
	{
		bool L_4;
		L_4 = GameState_TryGetRetainedRound_m2295FF12D42D33C9E7E1FF1D8B33B8E3859793A5(__this, (&V_0), NULL);
		if (!L_4)
		{
			goto IL_0035;
		}
	}
	{
		InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB* L_5 = (InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB*)il2cpp_codegen_object_new(((RuntimeClass*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB_il2cpp_TypeInfo_var)));
		InvalidOperationException__ctor_mE4CB6F4712AB6D99A2358FBAE2E052B3EE976162(L_5, ((String_t*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&_stringLiteral166980D7EB4EE9E05FE89E514C01D64A754D86C0)), NULL);
		IL2CPP_RAISE_MANAGED_EXCEPTION(L_5, ((RuntimeMethod*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&GameState_CanRegisterLevelAttempt_m509E53F6E5D8D3EC951AD4C656B4C83BE02A853A_RuntimeMethod_var)));
	}

IL_0035:
	{
		LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* L_6 = ___0_levelData;
		NullCheck(L_6);
		int32_t L_7 = L_6->___chapterId;
		LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* L_8 = ___0_levelData;
		NullCheck(L_8);
		int32_t L_9 = L_8->___levelId;
		bool L_10;
		L_10 = GameState_IsLevelUnlocked_mA15470234C68329950931198C078B05B900F0689(__this, L_7, L_9, NULL);
		return L_10;
	}
}
