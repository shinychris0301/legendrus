IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void ProductionSettingsView_TogglePreference_mBB0C26D45BDE34CC63CB95D3ED4CC556956B4287 (ProductionSettingsView_t8B601F105D46E705808BBD9FA8B26DD7877703BC* __this, int32_t ___0_index, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	bool V_0 = false;
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
		__this->___preferenceChangeInProgress = (bool)1;
	}
	{
		auto __finallyBlock = il2cpp::utils::Finally([&]
		{

FINALLY_003e:
			{
				__this->___preferenceChangeInProgress = (bool)0;
				return;
			}
		});
		try
		{
			GameState_t5E590D2FB317152A4011A3CA69E3F311E0A7393A* L_6;
			L_6 = GameState_get_Instance_m3202061BC5E0A39B53697C35D0BEE15692173D74_inline(NULL);
			int32_t L_7 = ___0_index;
			NullCheck(L_6);
			bool L_8;
			L_8 = GameState_TryTogglePreference_mC0A015B29957F92A08221215BE81E5C9566727D4(L_6, L_7, NULL);
			V_0 = L_8;
			goto IL_0046;
		}
		catch(Il2CppNativeThreadAbortException&)
		{
			__finallyBlock.SetNativeThreadAbortOccurred();
		}
		catch(Il2CppExceptionWrapper& e)
		{
			__finallyBlock.StoreException(e.ex);
		}
	}

IL_0046:
	{
		ProductionSettingsView_RefreshState_m6D0B60E1D33A8775F54328EFE4BE309958DE1648(__this, NULL);
		bool L_9 = V_0;
		if (L_9)
		{
			goto IL_0056;
		}
	}
	{
		ProductionSettingsView_ShowInfo_m0DD48DFEF7E979B0F7F2F789CE7C7477A2C2A78A(__this, 7, NULL);
	}

IL_0056:
	{
		return;
	}
}
