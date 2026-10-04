IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool GameState_AcknowledgeChapterCompletion_mE5126E9523CE8E3D09521BBBE31037EC693A12C1 (GameState_t5E590D2FB317152A4011A3CA69E3F311E0A7393A* __this, int32_t ___0_chapterId, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&List_1_Add_m0248A96C5334E9A93E6994B7780478BCD994EA3D_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&List_1_Contains_m4FD96E89F15844C90032C7386BAB528817F1FF5B_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&List_1_Sort_m6DA897B8E2A3434A84E3529FE9850B2773D88B65_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&List_1__ctor_m17F501B5A5C289ECE1B4F3D6EBF05DFA421433F8_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&List_1_t05915E9237850A58106982B7FE4BC5DA4E872E73_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		int32_t L_0 = ___0_chapterId;
		int32_t L_1;
		L_1 = GameState_GetChapterCompletionPhase_m695050CE6D412B9524950538CB58B8B1074F222F(__this, L_0, NULL);
		if ((((int32_t)L_1) == ((int32_t)2)))
		{
			goto IL_000c;
		}
	}
	{
		return (bool)0;
	}

IL_000c:
	{
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_2 = __this->___Save;
		NullCheck(L_2);
		List_1_t05915E9237850A58106982B7FE4BC5DA4E872E73* L_3 = L_2->___acknowledgedChapterCompletions;
		if (L_3)
		{
			goto IL_0029;
		}
	}
	{
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_4 = __this->___Save;
		List_1_t05915E9237850A58106982B7FE4BC5DA4E872E73* L_5 = (List_1_t05915E9237850A58106982B7FE4BC5DA4E872E73*)il2cpp_codegen_object_new(List_1_t05915E9237850A58106982B7FE4BC5DA4E872E73_il2cpp_TypeInfo_var);
		List_1__ctor_m17F501B5A5C289ECE1B4F3D6EBF05DFA421433F8(L_5, List_1__ctor_m17F501B5A5C289ECE1B4F3D6EBF05DFA421433F8_RuntimeMethod_var);
		NullCheck(L_4);
		L_4->___acknowledgedChapterCompletions = L_5;
		Il2CppCodeGenWriteBarrier((void**)(&L_4->___acknowledgedChapterCompletions), (void*)L_5);
	}

IL_0029:
	{
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_6 = __this->___Save;
		NullCheck(L_6);
		List_1_t05915E9237850A58106982B7FE4BC5DA4E872E73* L_7 = L_6->___acknowledgedChapterCompletions;
		int32_t L_8 = ___0_chapterId;
		NullCheck(L_7);
		bool L_9;
		L_9 = List_1_Contains_m4FD96E89F15844C90032C7386BAB528817F1FF5B(L_7, L_8, List_1_Contains_m4FD96E89F15844C90032C7386BAB528817F1FF5B_RuntimeMethod_var);
		if (!L_9)
		{
			goto IL_003e;
		}
	}
	{
		return (bool)0;
	}

IL_003e:
	{
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_10 = __this->___Save;
		NullCheck(L_10);
		List_1_t05915E9237850A58106982B7FE4BC5DA4E872E73* L_11 = L_10->___acknowledgedChapterCompletions;
		int32_t L_12 = ___0_chapterId;
		NullCheck(L_11);
		List_1_Add_m0248A96C5334E9A93E6994B7780478BCD994EA3D_inline(L_11, L_12, List_1_Add_m0248A96C5334E9A93E6994B7780478BCD994EA3D_RuntimeMethod_var);
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_13 = __this->___Save;
		NullCheck(L_13);
		List_1_t05915E9237850A58106982B7FE4BC5DA4E872E73* L_14 = L_13->___acknowledgedChapterCompletions;
		NullCheck(L_14);
		List_1_Sort_m6DA897B8E2A3434A84E3529FE9850B2773D88B65(L_14, List_1_Sort_m6DA897B8E2A3434A84E3529FE9850B2773D88B65_RuntimeMethod_var);
		GameState_Persist_m0261476FB722D9A1BAB0BB601B749EFC7E307C8D(__this, NULL);
		return (bool)1;
	}
}
