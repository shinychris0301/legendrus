IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool GameState_TryAddEnergy_m0BE90248ECA62CBBA70CDCA36894BBD89E2464B3 (GameState_t5E590D2FB317152A4011A3CA69E3F311E0A7393A* __this, int32_t ___0_amount, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&DateTime_t66193957C73913903DDAD89FEDC46139BCA5802D_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&JsonUtility_FromJson_TisPlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30_m5FA532D489BA59FC0DD187FA87D56E71F1F63B04_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Math_tEB65DE7CA8B083C412C969C92981C030865486CE_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* V_0 = NULL;
	DateTime_t66193957C73913903DDAD89FEDC46139BCA5802D V_1;
	memset((&V_1), 0, sizeof(V_1));
	{
		bool L_0 = __this->___canPersist;
		if (L_0)
		{
			goto IL_000a;
		}
	}
	{
		return (bool)0;
	}

IL_000a:
	{
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_1 = __this->___Save;
		String_t* L_2;
		L_2 = JsonUtility_ToJson_m28CC6843B9D3723D88AD13EA3829B71FDE7826BA(L_1, NULL);
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_3;
		L_3 = JsonUtility_FromJson_TisPlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30_m5FA532D489BA59FC0DD187FA87D56E71F1F63B04(L_2, JsonUtility_FromJson_TisPlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30_m5FA532D489BA59FC0DD187FA87D56E71F1F63B04_RuntimeMethod_var);
		V_0 = L_3;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_4 = V_0;
		int64_t L_5 = (il2cpp_codegen_conv<int64_t,int32_t,int32_t,false,false>(0,NULL));
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_6 = V_0;
		NullCheck(L_6);
		int32_t L_7 = L_6->___energyMax;
		int64_t L_8 = (il2cpp_codegen_conv<int64_t,int32_t,int32_t,false,false>(L_7,NULL));
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_9 = V_0;
		NullCheck(L_9);
		int32_t L_10 = L_9->___energy;
		int64_t L_11 = (il2cpp_codegen_conv<int64_t,int32_t,int32_t,false,false>(L_10,NULL));
		int32_t L_12 = ___0_amount;
		int32_t L_13;
		L_13 = Mathf_Max_m7FA442918DE37E3A00106D1F2E789D65829792B8_inline(0, L_12, NULL);
		int64_t L_14 = (il2cpp_codegen_conv<int64_t,int32_t,int32_t,false,false>(L_13,NULL));
		il2cpp_codegen_runtime_class_init_inline(Math_tEB65DE7CA8B083C412C969C92981C030865486CE_il2cpp_TypeInfo_var);
		int64_t L_15;
		L_15 = Math_Min_mD731E8A02F13C67C1EAC7C1E7F81909FE466F079(L_8, ((int64_t)il2cpp_codegen_add(L_11, L_14)), NULL);
		int64_t L_16;
		L_16 = Math_Max_mD37411571F0547F04F280D5A8D1F044819AFA597(L_5, L_15, NULL);
		int32_t L_17 = (il2cpp_codegen_conv<int32_t,int64_t,int64_t,false,false>(L_16,NULL));
		NullCheck(L_4);
		L_4->___energy = L_17;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_18 = V_0;
		NullCheck(L_18);
		int32_t L_19 = L_18->___energy;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_20 = V_0;
		NullCheck(L_20);
		int32_t L_21 = L_20->___energyMax;
		if ((((int32_t)L_19) < ((int32_t)L_21)))
		{
			goto IL_0066;
		}
	}
	{
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_22 = V_0;
		il2cpp_codegen_runtime_class_init_inline(DateTime_t66193957C73913903DDAD89FEDC46139BCA5802D_il2cpp_TypeInfo_var);
		DateTime_t66193957C73913903DDAD89FEDC46139BCA5802D L_23;
		L_23 = DateTime_get_UtcNow_m06B6E9995FE16846A0F71EC9DB23E90BE2C5F9FA(NULL);
		V_1 = L_23;
		int64_t L_24;
		L_24 = DateTime_get_Ticks_mC2CF04ED0EAB425C72C2532FFC5743777F3C93A6((&V_1), NULL);
		NullCheck(L_22);
		L_22->___lastEnergyUtcTicks = L_24;
	}

IL_0066:
	{
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_25 = V_0;
		bool L_26;
		L_26 = GameState_TryCommitBalance_mD9DA4B9AE5D934AD30A0F070F9C9AE01F165CCD7(__this, L_25, NULL);
		return L_26;
	}
}
