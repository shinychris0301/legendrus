IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void ProductionSettingsView_TogglePreference_mBB0C26D45BDE34CC63CB95D3ED4CC556956B4287 (ProductionSettingsView_t8B601F105D46E705808BBD9FA8B26DD7877703BC* __this, int32_t ___0_index, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		int32_t L_0 = ___0_index;
		if ((((int32_t)L_0) < ((int32_t)0)))
		{
			goto IL_0008;
		}
	}
	{
		int32_t L_1 = ___0_index;
		if ((((int32_t)L_1) <= ((int32_t)3)))
		{
			goto IL_0013;
		}
	}

IL_0008:
	{
		ArgumentOutOfRangeException_tEA2822DAF62B10EEED00E0E3A341D4BAF78CF85F* L_2 = (ArgumentOutOfRangeException_tEA2822DAF62B10EEED00E0E3A341D4BAF78CF85F*)il2cpp_codegen_object_new(((RuntimeClass*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&ArgumentOutOfRangeException_tEA2822DAF62B10EEED00E0E3A341D4BAF78CF85F_il2cpp_TypeInfo_var)));
		ArgumentOutOfRangeException__ctor_mBC1D5DEEA1BA41DE77228CB27D6BAFEB6DCCBF4A(L_2, ((String_t*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&_stringLiteral2B6D6F48C27C60C3B55391AB377D9DC8F5639AA1)), NULL);
		IL2CPP_RAISE_MANAGED_EXCEPTION(L_2, ((RuntimeMethod*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&ProductionSettingsView_TogglePreference_mBB0C26D45BDE34CC63CB95D3ED4CC556956B4287_RuntimeMethod_var)));
	}

IL_0013:
	{
		bool L_3;
		L_3 = ProductionSettingsView_CanAct_mC2A737BC95396907ED8F4E0BDF6D4BE540B20052(__this, NULL);
		if (!L_3)
		{
			goto IL_0028;
		}
	}
	{
		GameState_t5E590D2FB317152A4011A3CA69E3F311E0A7393A* L_4;
		L_4 = GameState_get_Instance_m3202061BC5E0A39B53697C35D0BEE15692173D74_inline(NULL);
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_5;
		L_5 = Object_op_Equality_mB6120F782D83091EF56A198FCEBCF066DB4A9605(L_4, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (!L_5)
		{
			goto IL_0029;
		}
	}

IL_0028:
	{
		return;
	}

IL_0029:
	{
		int32_t L_6 = ___0_index;
		switch (L_6)
		{
			case 0:
			{
				goto IL_0041;
			}
			case 1:
			{
				goto IL_004d;
			}
			case 2:
			{
				goto IL_0059;
			}
			case 3:
			{
				goto IL_0065;
			}
		}
	}
	{
		goto IL_006f;
	}

IL_0041:
	{
		GameState_t5E590D2FB317152A4011A3CA69E3F311E0A7393A* L_7;
		L_7 = GameState_get_Instance_m3202061BC5E0A39B53697C35D0BEE15692173D74_inline(NULL);
		NullCheck(L_7);
		GameState_ToggleMusic_m7ADD62D41ACF2A3D0E2FD0FA59650B80BF127E38(L_7, NULL);
		goto IL_006f;
	}

IL_004d:
	{
		GameState_t5E590D2FB317152A4011A3CA69E3F311E0A7393A* L_8;
		L_8 = GameState_get_Instance_m3202061BC5E0A39B53697C35D0BEE15692173D74_inline(NULL);
		NullCheck(L_8);
		GameState_ToggleSfx_m98568415ADC57929E125CD18B0D1A71FEA81C14F(L_8, NULL);
		goto IL_006f;
	}

IL_0059:
	{
		GameState_t5E590D2FB317152A4011A3CA69E3F311E0A7393A* L_9;
		L_9 = GameState_get_Instance_m3202061BC5E0A39B53697C35D0BEE15692173D74_inline(NULL);
		NullCheck(L_9);
		GameState_ToggleVibration_m9BE48FD832DF460717C0643D2BA044B383EE1F5D(L_9, NULL);
		goto IL_006f;
	}

IL_0065:
	{
		GameState_t5E590D2FB317152A4011A3CA69E3F311E0A7393A* L_10;
		L_10 = GameState_get_Instance_m3202061BC5E0A39B53697C35D0BEE15692173D74_inline(NULL);
		NullCheck(L_10);
		GameState_ToggleNotifications_m7EAA28673AD1EDB7D212D76214B16D07BA340B05(L_10, NULL);
	}

IL_006f:
	{
		ProductionSettingsView_RefreshState_m6D0B60E1D33A8775F54328EFE4BE309958DE1648(__this, NULL);
		return;
	}
}
