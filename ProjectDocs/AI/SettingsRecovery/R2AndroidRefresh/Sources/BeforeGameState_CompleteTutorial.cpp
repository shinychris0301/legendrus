IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool GameState_CompleteTutorial_m7DA3AB3239AD74E6FA5DF85723FEABB8ABF9F185 (GameState_t5E590D2FB317152A4011A3CA69E3F311E0A7393A* __this, String_t* ___0_tutorialId, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&JsonUtility_FromJson_TisPlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30_m5FA532D489BA59FC0DD187FA87D56E71F1F63B04_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&List_1_Add_mF10DB1D3CBB0B14215F0E4F8AB4934A1955E5351_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&List_1__ctor_mCA8DD57EAC70C2B5923DBB9D5A77CEAC22E7068E_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&List_1_tF470A3BE5C1B5B68E1325EF3F109D172E60BD7CD_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* V_0 = NULL;
	{
		bool L_0 = __this->___canPersist;
		if (!L_0)
		{
			goto IL_0019;
		}
	}
	{
		String_t* L_1 = ___0_tutorialId;
		bool L_2;
		L_2 = String_IsNullOrWhiteSpace_m42E1F3B2C358068D645E46F01CF1834DC77A5A10(L_1, NULL);
		if (L_2)
		{
			goto IL_0019;
		}
	}
	{
		String_t* L_3 = ___0_tutorialId;
		bool L_4;
		L_4 = GameState_HasCompletedTutorial_mFB9AF9E1D9899BD4AB5781D1013292E5BB78870D(__this, L_3, NULL);
		if (!L_4)
		{
			goto IL_001b;
		}
	}

IL_0019:
	{
		return (bool)0;
	}

IL_001b:
	{
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_5 = __this->___Save;
		String_t* L_6;
		L_6 = JsonUtility_ToJson_m28CC6843B9D3723D88AD13EA3829B71FDE7826BA(L_5, NULL);
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_7;
		L_7 = JsonUtility_FromJson_TisPlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30_m5FA532D489BA59FC0DD187FA87D56E71F1F63B04(L_6, JsonUtility_FromJson_TisPlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30_m5FA532D489BA59FC0DD187FA87D56E71F1F63B04_RuntimeMethod_var);
		V_0 = L_7;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_8 = V_0;
		NullCheck(L_8);
		List_1_tF470A3BE5C1B5B68E1325EF3F109D172E60BD7CD* L_9 = L_8->___completedTutorialSteps;
		if (L_9)
		{
			goto IL_003f;
		}
	}
	{
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_10 = V_0;
		List_1_tF470A3BE5C1B5B68E1325EF3F109D172E60BD7CD* L_11 = (List_1_tF470A3BE5C1B5B68E1325EF3F109D172E60BD7CD*)il2cpp_codegen_object_new(List_1_tF470A3BE5C1B5B68E1325EF3F109D172E60BD7CD_il2cpp_TypeInfo_var);
		List_1__ctor_mCA8DD57EAC70C2B5923DBB9D5A77CEAC22E7068E(L_11, List_1__ctor_mCA8DD57EAC70C2B5923DBB9D5A77CEAC22E7068E_RuntimeMethod_var);
		NullCheck(L_10);
		L_10->___completedTutorialSteps = L_11;
		Il2CppCodeGenWriteBarrier((void**)(&L_10->___completedTutorialSteps), (void*)L_11);
	}

IL_003f:
	{
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_12 = V_0;
		NullCheck(L_12);
		List_1_tF470A3BE5C1B5B68E1325EF3F109D172E60BD7CD* L_13 = L_12->___completedTutorialSteps;
		String_t* L_14 = ___0_tutorialId;
		NullCheck(L_13);
		List_1_Add_mF10DB1D3CBB0B14215F0E4F8AB4934A1955E5351_inline(L_13, L_14, List_1_Add_mF10DB1D3CBB0B14215F0E4F8AB4934A1955E5351_RuntimeMethod_var);
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_15 = V_0;
		bool L_16;
		L_16 = GameState_TryCommitBalance_mD9DA4B9AE5D934AD30A0F070F9C9AE01F165CCD7(__this, L_15, NULL);
		return L_16;
	}
}
