IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void GameState_Persist_m0261476FB722D9A1BAB0BB601B749EFC7E307C8D (GameState_t5E590D2FB317152A4011A3CA69E3F311E0A7393A* __this, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Debug_t8394C7EEAECA3689C2C9B9DE9C7166D73596276F_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&_stringLiteral5792712343BCFEC04EF06B80701CDC08FD8DC924);
		s_Il2CppMethodInitialized = true;
	}
	PlayerSaveWriteResult_tD3521C4F0AD4F171529F5475A2BD7B28C0CB1D9C* V_0 = NULL;
	PlayerSaveWriteResult_tD3521C4F0AD4F171529F5475A2BD7B28C0CB1D9C* G_B3_0 = NULL;
	String_t* G_B3_1 = NULL;
	PlayerSaveWriteResult_tD3521C4F0AD4F171529F5475A2BD7B28C0CB1D9C* G_B2_0 = NULL;
	String_t* G_B2_1 = NULL;
	String_t* G_B4_0 = NULL;
	String_t* G_B4_1 = NULL;
	Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* G_B7_0 = NULL;
	Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* G_B6_0 = NULL;
	{
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_0 = __this->___Save;
		bool L_1;
		L_1 = SaveService_TrySave_m11A043A57FCBE0607BDAAA6C34B4E969064615B0(L_0, (&V_0), NULL);
		if (L_1)
		{
			goto IL_003a;
		}
	}
	{
		PlayerSaveWriteResult_tD3521C4F0AD4F171529F5475A2BD7B28C0CB1D9C* L_2 = V_0;
		GameState_set_LastWriteResult_mE42274830424D045544D465431B5C582BC50A567_inline(__this, L_2, NULL);
		__this->___canPersist = (bool)0;
		PlayerSaveWriteResult_tD3521C4F0AD4F171529F5475A2BD7B28C0CB1D9C* L_3 = V_0;
		PlayerSaveWriteResult_tD3521C4F0AD4F171529F5475A2BD7B28C0CB1D9C* L_4 = L_3;
		if (L_4)
		{
			G_B3_0 = L_4;
			G_B3_1 = _stringLiteral5792712343BCFEC04EF06B80701CDC08FD8DC924;
			goto IL_002a;
		}
		G_B2_0 = L_4;
		G_B2_1 = _stringLiteral5792712343BCFEC04EF06B80701CDC08FD8DC924;
	}
	{
		G_B4_0 = ((String_t*)(NULL));
		G_B4_1 = G_B2_1;
		goto IL_002f;
	}

IL_002a:
	{
		NullCheck(G_B3_0);
		String_t* L_5;
		L_5 = VirtualFuncInvoker0< String_t* >::Invoke(3, G_B3_0);
		G_B4_0 = L_5;
		G_B4_1 = G_B3_1;
	}

IL_002f:
	{
		String_t* L_6;
		L_6 = String_Concat_m9E3155FB84015C823606188F53B47CB44C444991(G_B4_1, G_B4_0, NULL);
		il2cpp_codegen_runtime_class_init_inline(Debug_t8394C7EEAECA3689C2C9B9DE9C7166D73596276F_il2cpp_TypeInfo_var);
		Debug_LogError_mB00B2B4468EF3CAF041B038D840820FB84C924B2(L_6, NULL);
		return;
	}

IL_003a:
	{
		PlayerSaveWriteResult_tD3521C4F0AD4F171529F5475A2BD7B28C0CB1D9C* L_7 = V_0;
		GameState_set_LastWriteResult_mE42274830424D045544D465431B5C582BC50A567_inline(__this, L_7, NULL);
		__this->___canPersist = (bool)1;
		Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* L_8 = __this->___Changed;
		Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* L_9 = L_8;
		if (L_9)
		{
			G_B7_0 = L_9;
			goto IL_0053;
		}
		G_B6_0 = L_9;
	}
	{
		return;
	}

IL_0053:
	{
		NullCheck(G_B7_0);
		Action_Invoke_m7126A54DACA72B845424072887B5F3A51FC3808E_inline(G_B7_0, NULL);
		return;
	}
}
