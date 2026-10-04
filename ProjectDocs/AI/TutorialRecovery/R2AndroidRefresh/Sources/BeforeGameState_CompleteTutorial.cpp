IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool GameState_CompleteTutorial_m7DA3AB3239AD74E6FA5DF85723FEABB8ABF9F185 (GameState_t5E590D2FB317152A4011A3CA69E3F311E0A7393A* __this, String_t* ___0_tutorialId, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&List_1_Add_mF10DB1D3CBB0B14215F0E4F8AB4934A1955E5351_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&List_1_Contains_m359254483BE42CAD4DCA8FBAFB87473FB4CF00E1_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&List_1__ctor_mCA8DD57EAC70C2B5923DBB9D5A77CEAC22E7068E_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&List_1_tF470A3BE5C1B5B68E1325EF3F109D172E60BD7CD_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		String_t* L_0 = ___0_tutorialId;
		bool L_1;
		L_1 = String_IsNullOrWhiteSpace_m42E1F3B2C358068D645E46F01CF1834DC77A5A10(L_0, NULL);
		if (!L_1)
		{
			goto IL_000a;
		}
	}
	{
		return (bool)0;
	}

IL_000a:
	{
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_2 = __this->___Save;
		NullCheck(L_2);
		List_1_tF470A3BE5C1B5B68E1325EF3F109D172E60BD7CD* L_3 = L_2->___completedTutorialSteps;
		if (L_3)
		{
			goto IL_0027;
		}
	}
	{
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_4 = __this->___Save;
		List_1_tF470A3BE5C1B5B68E1325EF3F109D172E60BD7CD* L_5 = (List_1_tF470A3BE5C1B5B68E1325EF3F109D172E60BD7CD*)il2cpp_codegen_object_new(List_1_tF470A3BE5C1B5B68E1325EF3F109D172E60BD7CD_il2cpp_TypeInfo_var);
		List_1__ctor_mCA8DD57EAC70C2B5923DBB9D5A77CEAC22E7068E(L_5, List_1__ctor_mCA8DD57EAC70C2B5923DBB9D5A77CEAC22E7068E_RuntimeMethod_var);
		NullCheck(L_4);
		L_4->___completedTutorialSteps = L_5;
		Il2CppCodeGenWriteBarrier((void**)(&L_4->___completedTutorialSteps), (void*)L_5);
	}

IL_0027:
	{
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_6 = __this->___Save;
		NullCheck(L_6);
		List_1_tF470A3BE5C1B5B68E1325EF3F109D172E60BD7CD* L_7 = L_6->___completedTutorialSteps;
		String_t* L_8 = ___0_tutorialId;
		NullCheck(L_7);
		bool L_9;
		L_9 = List_1_Contains_m359254483BE42CAD4DCA8FBAFB87473FB4CF00E1(L_7, L_8, List_1_Contains_m359254483BE42CAD4DCA8FBAFB87473FB4CF00E1_RuntimeMethod_var);
		if (!L_9)
		{
			goto IL_003c;
		}
	}
	{
		return (bool)0;
	}

IL_003c:
	{
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_10 = __this->___Save;
		NullCheck(L_10);
		List_1_tF470A3BE5C1B5B68E1325EF3F109D172E60BD7CD* L_11 = L_10->___completedTutorialSteps;
		String_t* L_12 = ___0_tutorialId;
		NullCheck(L_11);
		List_1_Add_mF10DB1D3CBB0B14215F0E4F8AB4934A1955E5351_inline(L_11, L_12, List_1_Add_mF10DB1D3CBB0B14215F0E4F8AB4934A1955E5351_RuntimeMethod_var);
		GameState_Persist_m0261476FB722D9A1BAB0BB601B749EFC7E307C8D(__this, NULL);
		return (bool)1;
	}
}
