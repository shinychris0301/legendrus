IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool GameState_CompleteStoryScene_m74C9B3BB2CF98FEBFC15B4F3D9CA78568814A005 (GameState_t5E590D2FB317152A4011A3CA69E3F311E0A7393A* __this, String_t* ___0_sceneId, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&List_1__ctor_mCA8DD57EAC70C2B5923DBB9D5A77CEAC22E7068E_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&List_1_tF470A3BE5C1B5B68E1325EF3F109D172E60BD7CD_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_0 = __this->___Save;
		NullCheck(L_0);
		List_1_tF470A3BE5C1B5B68E1325EF3F109D172E60BD7CD* L_1 = L_0->___completedStoryScenes;
		if (L_1)
		{
			goto IL_001d;
		}
	}
	{
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_2 = __this->___Save;
		List_1_tF470A3BE5C1B5B68E1325EF3F109D172E60BD7CD* L_3 = (List_1_tF470A3BE5C1B5B68E1325EF3F109D172E60BD7CD*)il2cpp_codegen_object_new(List_1_tF470A3BE5C1B5B68E1325EF3F109D172E60BD7CD_il2cpp_TypeInfo_var);
		List_1__ctor_mCA8DD57EAC70C2B5923DBB9D5A77CEAC22E7068E(L_3, List_1__ctor_mCA8DD57EAC70C2B5923DBB9D5A77CEAC22E7068E_RuntimeMethod_var);
		NullCheck(L_2);
		L_2->___completedStoryScenes = L_3;
		Il2CppCodeGenWriteBarrier((void**)(&L_2->___completedStoryScenes), (void*)L_3);
	}

IL_001d:
	{
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_4 = __this->___Save;
		NullCheck(L_4);
		List_1_tF470A3BE5C1B5B68E1325EF3F109D172E60BD7CD* L_5 = L_4->___completedStoryScenes;
		String_t* L_6 = ___0_sceneId;
		bool L_7;
		L_7 = StoryProgress_Complete_mF4B20F290FB75F001C54C5F543475D8619D96F69(L_5, L_6, NULL);
		if (L_7)
		{
			goto IL_0032;
		}
	}
	{
		return (bool)0;
	}

IL_0032:
	{
		GameState_Persist_m0261476FB722D9A1BAB0BB601B749EFC7E307C8D(__this, NULL);
		return (bool)1;
	}
}
