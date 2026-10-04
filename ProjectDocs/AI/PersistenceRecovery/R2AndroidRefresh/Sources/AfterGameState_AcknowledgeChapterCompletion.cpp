IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool GameState_AcknowledgeChapterCompletion_mE5126E9523CE8E3D09521BBBE31037EC693A12C1 (GameState_t5E590D2FB317152A4011A3CA69E3F311E0A7393A* __this, int32_t ___0_chapterId, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&JsonUtility_FromJson_TisPlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30_m5FA532D489BA59FC0DD187FA87D56E71F1F63B04_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&List_1_Add_m0248A96C5334E9A93E6994B7780478BCD994EA3D_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&List_1_Contains_m4FD96E89F15844C90032C7386BAB528817F1FF5B_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&List_1_Sort_m6DA897B8E2A3434A84E3529FE9850B2773D88B65_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&List_1__ctor_m17F501B5A5C289ECE1B4F3D6EBF05DFA421433F8_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&List_1_t05915E9237850A58106982B7FE4BC5DA4E872E73_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* V_0 = NULL;
	{
		bool L_0 = __this->___canPersist;
		if (!L_0)
		{
			goto IL_0012;
		}
	}
	{
		int32_t L_1 = ___0_chapterId;
		int32_t L_2;
		L_2 = GameState_GetChapterCompletionPhase_m695050CE6D412B9524950538CB58B8B1074F222F(__this, L_1, NULL);
		if ((((int32_t)L_2) == ((int32_t)2)))
		{
			goto IL_0014;
		}
	}

IL_0012:
	{
		return (bool)0;
	}

IL_0014:
	{
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_3 = __this->___Save;
		String_t* L_4;
		L_4 = JsonUtility_ToJson_m28CC6843B9D3723D88AD13EA3829B71FDE7826BA(L_3, NULL);
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_5;
		L_5 = JsonUtility_FromJson_TisPlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30_m5FA532D489BA59FC0DD187FA87D56E71F1F63B04(L_4, JsonUtility_FromJson_TisPlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30_m5FA532D489BA59FC0DD187FA87D56E71F1F63B04_RuntimeMethod_var);
		V_0 = L_5;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_6 = V_0;
		NullCheck(L_6);
		List_1_t05915E9237850A58106982B7FE4BC5DA4E872E73* L_7 = L_6->___acknowledgedChapterCompletions;
		if (L_7)
		{
			goto IL_0038;
		}
	}
	{
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_8 = V_0;
		List_1_t05915E9237850A58106982B7FE4BC5DA4E872E73* L_9 = (List_1_t05915E9237850A58106982B7FE4BC5DA4E872E73*)il2cpp_codegen_object_new(List_1_t05915E9237850A58106982B7FE4BC5DA4E872E73_il2cpp_TypeInfo_var);
		List_1__ctor_m17F501B5A5C289ECE1B4F3D6EBF05DFA421433F8(L_9, List_1__ctor_m17F501B5A5C289ECE1B4F3D6EBF05DFA421433F8_RuntimeMethod_var);
		NullCheck(L_8);
		L_8->___acknowledgedChapterCompletions = L_9;
		Il2CppCodeGenWriteBarrier((void**)(&L_8->___acknowledgedChapterCompletions), (void*)L_9);
	}

IL_0038:
	{
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_10 = V_0;
		NullCheck(L_10);
		List_1_t05915E9237850A58106982B7FE4BC5DA4E872E73* L_11 = L_10->___acknowledgedChapterCompletions;
		int32_t L_12 = ___0_chapterId;
		NullCheck(L_11);
		bool L_13;
		L_13 = List_1_Contains_m4FD96E89F15844C90032C7386BAB528817F1FF5B(L_11, L_12, List_1_Contains_m4FD96E89F15844C90032C7386BAB528817F1FF5B_RuntimeMethod_var);
		if (!L_13)
		{
			goto IL_0048;
		}
	}
	{
		return (bool)0;
	}

IL_0048:
	{
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_14 = V_0;
		NullCheck(L_14);
		List_1_t05915E9237850A58106982B7FE4BC5DA4E872E73* L_15 = L_14->___acknowledgedChapterCompletions;
		int32_t L_16 = ___0_chapterId;
		NullCheck(L_15);
		List_1_Add_m0248A96C5334E9A93E6994B7780478BCD994EA3D_inline(L_15, L_16, List_1_Add_m0248A96C5334E9A93E6994B7780478BCD994EA3D_RuntimeMethod_var);
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_17 = V_0;
		NullCheck(L_17);
		List_1_t05915E9237850A58106982B7FE4BC5DA4E872E73* L_18 = L_17->___acknowledgedChapterCompletions;
		NullCheck(L_18);
		List_1_Sort_m6DA897B8E2A3434A84E3529FE9850B2773D88B65(L_18, List_1_Sort_m6DA897B8E2A3434A84E3529FE9850B2773D88B65_RuntimeMethod_var);
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_19 = V_0;
		bool L_20;
		L_20 = GameState_TryCommitBalance_mD9DA4B9AE5D934AD30A0F070F9C9AE01F165CCD7(__this, L_19, NULL);
		return L_20;
	}
}
