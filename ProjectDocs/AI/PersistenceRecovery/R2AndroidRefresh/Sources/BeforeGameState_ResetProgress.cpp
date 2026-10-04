IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void GameState_ResetProgress_mB7390FF8F24287D24C745C1DA3B9C8D4320ED479 (GameState_t5E590D2FB317152A4011A3CA69E3F311E0A7393A* __this, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&DateTime_t66193957C73913903DDAD89FEDC46139BCA5802D_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Debug_t8394C7EEAECA3689C2C9B9DE9C7166D73596276F_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&_stringLiteral032AEBD64BE298939AAE41C02EF00DF153AB1540);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&_stringLiteralD9E859F5FB588608A69E648DE74444EB5CC5FA63);
		s_Il2CppMethodInitialized = true;
	}
	PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* V_0 = NULL;
	PlayerSaveWriteResult_tD3521C4F0AD4F171529F5475A2BD7B28C0CB1D9C* V_1 = NULL;
	bool V_2 = false;
	DateTime_t66193957C73913903DDAD89FEDC46139BCA5802D V_3;
	memset((&V_3), 0, sizeof(V_3));
	PlayerSaveWriteResult_tD3521C4F0AD4F171529F5475A2BD7B28C0CB1D9C* G_B4_0 = NULL;
	String_t* G_B4_1 = NULL;
	PlayerSaveWriteResult_tD3521C4F0AD4F171529F5475A2BD7B28C0CB1D9C* G_B3_0 = NULL;
	String_t* G_B3_1 = NULL;
	String_t* G_B5_0 = NULL;
	String_t* G_B5_1 = NULL;
	PlayerSaveWriteResult_tD3521C4F0AD4F171529F5475A2BD7B28C0CB1D9C* G_B9_0 = NULL;
	String_t* G_B9_1 = NULL;
	PlayerSaveWriteResult_tD3521C4F0AD4F171529F5475A2BD7B28C0CB1D9C* G_B8_0 = NULL;
	String_t* G_B8_1 = NULL;
	String_t* G_B10_0 = NULL;
	String_t* G_B10_1 = NULL;
	Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* G_B13_0 = NULL;
	Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* G_B12_0 = NULL;
	{
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_0 = (PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30*)il2cpp_codegen_object_new(PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30_il2cpp_TypeInfo_var);
		PlayerSaveData__ctor_m7A7E9DB622A56147969520D66AF2CD47097003D5(L_0, NULL);
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_1 = L_0;
		il2cpp_codegen_runtime_class_init_inline(DateTime_t66193957C73913903DDAD89FEDC46139BCA5802D_il2cpp_TypeInfo_var);
		DateTime_t66193957C73913903DDAD89FEDC46139BCA5802D L_2;
		L_2 = DateTime_get_UtcNow_m06B6E9995FE16846A0F71EC9DB23E90BE2C5F9FA(NULL);
		V_3 = L_2;
		int64_t L_3;
		L_3 = DateTime_get_Ticks_mC2CF04ED0EAB425C72C2532FFC5743777F3C93A6((&V_3), NULL);
		NullCheck(L_1);
		L_1->___lastEnergyUtcTicks = L_3;
		V_0 = L_1;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_4 = V_0;
		bool L_5;
		L_5 = SaveService_TryReplaceHistory_mB350F210F6DEF187B4CD71B5CC18C176E42A5BE4(L_4, (&V_1), NULL);
		V_2 = L_5;
		PlayerSaveWriteResult_tD3521C4F0AD4F171529F5475A2BD7B28C0CB1D9C* L_6 = V_1;
		GameState_set_LastWriteResult_mE42274830424D045544D465431B5C582BC50A567_inline(__this, L_6, NULL);
		bool L_7 = V_2;
		if (L_7)
		{
			goto IL_0058;
		}
	}
	{
		PlayerSaveWriteResult_tD3521C4F0AD4F171529F5475A2BD7B28C0CB1D9C* L_8 = V_1;
		NullCheck(L_8);
		bool L_9;
		L_9 = PlayerSaveWriteResult_get_PrimaryDurablyCommitted_m0D28E9CF94BC730C6E592F0CC6D4638D8961D000_inline(L_8, NULL);
		if (L_9)
		{
			goto IL_0058;
		}
	}
	{
		__this->___canPersist = (bool)0;
		PlayerSaveWriteResult_tD3521C4F0AD4F171529F5475A2BD7B28C0CB1D9C* L_10 = V_1;
		PlayerSaveWriteResult_tD3521C4F0AD4F171529F5475A2BD7B28C0CB1D9C* L_11 = L_10;
		if (L_11)
		{
			G_B4_0 = L_11;
			G_B4_1 = _stringLiteral032AEBD64BE298939AAE41C02EF00DF153AB1540;
			goto IL_0048;
		}
		G_B3_0 = L_11;
		G_B3_1 = _stringLiteral032AEBD64BE298939AAE41C02EF00DF153AB1540;
	}
	{
		G_B5_0 = ((String_t*)(NULL));
		G_B5_1 = G_B3_1;
		goto IL_004d;
	}

IL_0048:
	{
		NullCheck(G_B4_0);
		String_t* L_12;
		L_12 = VirtualFuncInvoker0< String_t* >::Invoke(3, G_B4_0);
		G_B5_0 = L_12;
		G_B5_1 = G_B4_1;
	}

IL_004d:
	{
		String_t* L_13;
		L_13 = String_Concat_m9E3155FB84015C823606188F53B47CB44C444991(G_B5_1, G_B5_0, NULL);
		il2cpp_codegen_runtime_class_init_inline(Debug_t8394C7EEAECA3689C2C9B9DE9C7166D73596276F_il2cpp_TypeInfo_var);
		Debug_LogError_mB00B2B4468EF3CAF041B038D840820FB84C924B2(L_13, NULL);
		return;
	}

IL_0058:
	{
		StoryService_ResetRuntime_m987401C5A55C038199F4DC05B5B3F79D02DFC722(NULL);
		GameState_set_LastCompletionResult_mBC96C630298B2EA9E1657205DA637CCA1BB8E82F_inline(__this, (LevelCompletionResult_t4C786293AEB16558E39DC3DCEEE1542A5487F03E*)NULL, NULL);
		GameState_set_LastFailureResult_m93C05E4ECCB370EAD4C1691FEC10E4F59FCC4DCF_inline(__this, (LevelFailureResult_tEAFB9DEC27BA6439583A47C07A1D65D0B79D12E0*)NULL, NULL);
		__this->___unsavedRetainedRound = (RetainedRoundData_t3EA1BA932DFF2408098F1FB87EC5A5A24130D7A1*)NULL;
		Il2CppCodeGenWriteBarrier((void**)(&__this->___unsavedRetainedRound), (void*)(RetainedRoundData_t3EA1BA932DFF2408098F1FB87EC5A5A24130D7A1*)NULL);
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_14 = V_0;
		__this->___Save = L_14;
		Il2CppCodeGenWriteBarrier((void**)(&__this->___Save), (void*)L_14);
		bool L_15 = V_2;
		__this->___canPersist = L_15;
		bool L_16 = V_2;
		if (L_16)
		{
			goto IL_009f;
		}
	}
	{
		PlayerSaveWriteResult_tD3521C4F0AD4F171529F5475A2BD7B28C0CB1D9C* L_17 = V_1;
		PlayerSaveWriteResult_tD3521C4F0AD4F171529F5475A2BD7B28C0CB1D9C* L_18 = L_17;
		if (L_18)
		{
			G_B9_0 = L_18;
			G_B9_1 = _stringLiteralD9E859F5FB588608A69E648DE74444EB5CC5FA63;
			goto IL_0090;
		}
		G_B8_0 = L_18;
		G_B8_1 = _stringLiteralD9E859F5FB588608A69E648DE74444EB5CC5FA63;
	}
	{
		G_B10_0 = ((String_t*)(NULL));
		G_B10_1 = G_B8_1;
		goto IL_0095;
	}

IL_0090:
	{
		NullCheck(G_B9_0);
		String_t* L_19;
		L_19 = VirtualFuncInvoker0< String_t* >::Invoke(3, G_B9_0);
		G_B10_0 = L_19;
		G_B10_1 = G_B9_1;
	}

IL_0095:
	{
		String_t* L_20;
		L_20 = String_Concat_m9E3155FB84015C823606188F53B47CB44C444991(G_B10_1, G_B10_0, NULL);
		il2cpp_codegen_runtime_class_init_inline(Debug_t8394C7EEAECA3689C2C9B9DE9C7166D73596276F_il2cpp_TypeInfo_var);
		Debug_LogWarning_m33EF1B897E0C7C6FF538989610BFAFFEF4628CA9(L_20, NULL);
	}

IL_009f:
	{
		Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* L_21 = __this->___Changed;
		Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* L_22 = L_21;
		if (L_22)
		{
			G_B13_0 = L_22;
			goto IL_00aa;
		}
		G_B12_0 = L_22;
	}
	{
		return;
	}

IL_00aa:
	{
		NullCheck(G_B13_0);
		Action_Invoke_m7126A54DACA72B845424072887B5F3A51FC3808E_inline(G_B13_0, NULL);
		return;
	}
}
