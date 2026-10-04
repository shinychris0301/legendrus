IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void GameState_Persist_m0261476FB722D9A1BAB0BB601B749EFC7E307C8D (GameState_t5E590D2FB317152A4011A3CA69E3F311E0A7393A* __this, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Debug_t8394C7EEAECA3689C2C9B9DE9C7166D73596276F_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&_stringLiteral5792712343BCFEC04EF06B80701CDC08FD8DC924);
		s_Il2CppMethodInitialized = true;
	}
	PlayerSaveWriteResult_tD3521C4F0AD4F171529F5475A2BD7B28C0CB1D9C* G_B5_0 = NULL;
	String_t* G_B5_1 = NULL;
	PlayerSaveWriteResult_tD3521C4F0AD4F171529F5475A2BD7B28C0CB1D9C* G_B4_0 = NULL;
	String_t* G_B4_1 = NULL;
	String_t* G_B6_0 = NULL;
	String_t* G_B6_1 = NULL;
	{
		bool L_0 = __this->___persistenceInProgress;
		if (!L_0)
		{
			goto IL_0009;
		}
	}
	{
		return;
	}

IL_0009:
	{
		bool L_1;
		L_1 = GameState_TryPersist_mF3A65DB34A79CC303E1F44137C6BDF4C69B1E13C(__this, NULL);
		if (L_1)
		{
			goto IL_0032;
		}
	}
	{
		PlayerSaveWriteResult_tD3521C4F0AD4F171529F5475A2BD7B28C0CB1D9C* L_2;
		L_2 = GameState_get_LastWriteResult_mD44BB55E0B74A05D457AEB42926EFCF30BC84D9A_inline(__this, NULL);
		PlayerSaveWriteResult_tD3521C4F0AD4F171529F5475A2BD7B28C0CB1D9C* L_3 = L_2;
		if (L_3)
		{
			G_B5_0 = L_3;
			G_B5_1 = _stringLiteral5792712343BCFEC04EF06B80701CDC08FD8DC924;
			goto IL_0023;
		}
		G_B4_0 = L_3;
		G_B4_1 = _stringLiteral5792712343BCFEC04EF06B80701CDC08FD8DC924;
	}
	{
		G_B6_0 = ((String_t*)(NULL));
		G_B6_1 = G_B4_1;
		goto IL_0028;
	}

IL_0023:
	{
		NullCheck(G_B5_0);
		String_t* L_4;
		L_4 = VirtualFuncInvoker0< String_t* >::Invoke(3, G_B5_0);
		G_B6_0 = L_4;
		G_B6_1 = G_B5_1;
	}

IL_0028:
	{
		String_t* L_5;
		L_5 = String_Concat_m9E3155FB84015C823606188F53B47CB44C444991(G_B6_1, G_B6_0, NULL);
		il2cpp_codegen_runtime_class_init_inline(Debug_t8394C7EEAECA3689C2C9B9DE9C7166D73596276F_il2cpp_TypeInfo_var);
		Debug_LogError_mB00B2B4468EF3CAF041B038D840820FB84C924B2(L_5, NULL);
	}

IL_0032:
	{
		return;
	}
}
