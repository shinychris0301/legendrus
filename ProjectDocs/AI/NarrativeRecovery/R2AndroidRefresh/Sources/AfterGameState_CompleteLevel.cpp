IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR LevelCompletionResult_t4C786293AEB16558E39DC3DCEEE1542A5487F03E* GameState_CompleteLevel_m0E7DE13DCE834A788086A415FC333D78E6A51ADF (GameState_t5E590D2FB317152A4011A3CA69E3F311E0A7393A* __this, LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* ___0_levelData, int32_t ___1_score, const RuntimeMethod* method) 
{
	LevelCompletionResult_t4C786293AEB16558E39DC3DCEEE1542A5487F03E* V_0 = NULL;
	{
		LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* L_0 = ___0_levelData;
		int32_t L_1 = ___1_score;
		bool L_2;
		L_2 = GameState_TryCompleteLevel_m1A62C5C8B0955D4911B88F1A105CAD96B09AEBEC(__this, L_0, L_1, (&V_0), NULL);
		if (L_2)
		{
			goto IL_0017;
		}
	}
	{
		InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB* L_3 = (InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB*)il2cpp_codegen_object_new(((RuntimeClass*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB_il2cpp_TypeInfo_var)));
		InvalidOperationException__ctor_mE4CB6F4712AB6D99A2358FBAE2E052B3EE976162(L_3, ((String_t*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&_stringLiteral68C010EEE34CC895C8A6F13391F8F9A7204E1E43)), NULL);
		IL2CPP_RAISE_MANAGED_EXCEPTION(L_3, ((RuntimeMethod*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&GameState_CompleteLevel_m0E7DE13DCE834A788086A415FC333D78E6A51ADF_RuntimeMethod_var)));
	}

IL_0017:
	{
		LevelCompletionResult_t4C786293AEB16558E39DC3DCEEE1542A5487F03E* L_4 = V_0;
		return L_4;
	}
}
