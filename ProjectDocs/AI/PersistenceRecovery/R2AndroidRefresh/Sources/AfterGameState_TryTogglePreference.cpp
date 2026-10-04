IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool GameState_TryTogglePreference_mC0A015B29957F92A08221215BE81E5C9566727D4 (GameState_t5E590D2FB317152A4011A3CA69E3F311E0A7393A* __this, int32_t ___0_index, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&JsonUtility_FromJson_TisPlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30_m5FA532D489BA59FC0DD187FA87D56E71F1F63B04_RuntimeMethod_var);
		s_Il2CppMethodInitialized = true;
	}
	PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* V_0 = NULL;
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
		IL2CPP_RAISE_MANAGED_EXCEPTION(L_2, ((RuntimeMethod*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&GameState_TryTogglePreference_mC0A015B29957F92A08221215BE81E5C9566727D4_RuntimeMethod_var)));
	}

IL_0013:
	{
		bool L_3 = __this->___canPersist;
		if (L_3)
		{
			goto IL_001d;
		}
	}
	{
		return (bool)0;
	}

IL_001d:
	{
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_4 = __this->___Save;
		String_t* L_5;
		L_5 = JsonUtility_ToJson_m28CC6843B9D3723D88AD13EA3829B71FDE7826BA(L_4, NULL);
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_6;
		L_6 = JsonUtility_FromJson_TisPlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30_m5FA532D489BA59FC0DD187FA87D56E71F1F63B04(L_5, JsonUtility_FromJson_TisPlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30_m5FA532D489BA59FC0DD187FA87D56E71F1F63B04_RuntimeMethod_var);
		V_0 = L_6;
		int32_t L_7 = ___0_index;
		switch (L_7)
		{
			case 0:
			{
				goto IL_0046;
			}
			case 1:
			{
				goto IL_0057;
			}
			case 2:
			{
				goto IL_0068;
			}
			case 3:
			{
				goto IL_0079;
			}
		}
	}
	{
		goto IL_0088;
	}

IL_0046:
	{
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_8 = V_0;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_9 = V_0;
		NullCheck(L_9);
		bool L_10 = L_9->___music;
		NullCheck(L_8);
		L_8->___music = (bool)((((int32_t)L_10) == ((int32_t)0))? 1 : 0);
		goto IL_0088;
	}

IL_0057:
	{
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_11 = V_0;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_12 = V_0;
		NullCheck(L_12);
		bool L_13 = L_12->___sfx;
		NullCheck(L_11);
		L_11->___sfx = (bool)((((int32_t)L_13) == ((int32_t)0))? 1 : 0);
		goto IL_0088;
	}

IL_0068:
	{
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_14 = V_0;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_15 = V_0;
		NullCheck(L_15);
		bool L_16 = L_15->___vibration;
		NullCheck(L_14);
		L_14->___vibration = (bool)((((int32_t)L_16) == ((int32_t)0))? 1 : 0);
		goto IL_0088;
	}

IL_0079:
	{
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_17 = V_0;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_18 = V_0;
		NullCheck(L_18);
		bool L_19 = L_18->___notifications;
		NullCheck(L_17);
		L_17->___notifications = (bool)((((int32_t)L_19) == ((int32_t)0))? 1 : 0);
	}

IL_0088:
	{
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_20 = V_0;
		bool L_21;
		L_21 = GameState_TryCommitBalance_mD9DA4B9AE5D934AD30A0F070F9C9AE01F165CCD7(__this, L_20, NULL);
		return L_21;
	}
}
