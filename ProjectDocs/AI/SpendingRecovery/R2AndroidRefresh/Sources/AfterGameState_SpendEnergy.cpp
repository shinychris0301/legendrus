IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool GameState_SpendEnergy_mD8D9F45B107F9A46577C939F1784260B2D097D5D (GameState_t5E590D2FB317152A4011A3CA69E3F311E0A7393A* __this, int32_t ___0_amount, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&DateTime_t66193957C73913903DDAD89FEDC46139BCA5802D_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&JsonUtility_FromJson_TisPlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30_m5FA532D489BA59FC0DD187FA87D56E71F1F63B04_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Math_tEB65DE7CA8B083C412C969C92981C030865486CE_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&TimeSpan_t8195C5B013A2C532FEBDF0B64B6911982E750F5A_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* V_0 = NULL;
	int64_t V_1 = 0;
	DateTime_t66193957C73913903DDAD89FEDC46139BCA5802D V_2;
	memset((&V_2), 0, sizeof(V_2));
	int64_t V_3 = 0;
	int32_t V_4 = 0;
	TimeSpan_t8195C5B013A2C532FEBDF0B64B6911982E750F5A V_5;
	memset((&V_5), 0, sizeof(V_5));
	PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* G_B8_0 = NULL;
	PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* G_B7_0 = NULL;
	int64_t G_B9_0 = 0;
	PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* G_B9_1 = NULL;
	{
		bool L_0 = __this->___canPersist;
		if (!L_0)
		{
			goto IL_000c;
		}
	}
	{
		int32_t L_1 = ___0_amount;
		if ((((int32_t)L_1) >= ((int32_t)0)))
		{
			goto IL_000e;
		}
	}

IL_000c:
	{
		return (bool)0;
	}

IL_000e:
	{
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_2 = __this->___Save;
		String_t* L_3;
		L_3 = JsonUtility_ToJson_m28CC6843B9D3723D88AD13EA3829B71FDE7826BA(L_2, NULL);
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_4;
		L_4 = JsonUtility_FromJson_TisPlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30_m5FA532D489BA59FC0DD187FA87D56E71F1F63B04(L_3, JsonUtility_FromJson_TisPlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30_m5FA532D489BA59FC0DD187FA87D56E71F1F63B04_RuntimeMethod_var);
		V_0 = L_4;
		il2cpp_codegen_runtime_class_init_inline(DateTime_t66193957C73913903DDAD89FEDC46139BCA5802D_il2cpp_TypeInfo_var);
		DateTime_t66193957C73913903DDAD89FEDC46139BCA5802D L_5;
		L_5 = DateTime_get_UtcNow_m06B6E9995FE16846A0F71EC9DB23E90BE2C5F9FA(NULL);
		V_2 = L_5;
		int64_t L_6;
		L_6 = DateTime_get_Ticks_mC2CF04ED0EAB425C72C2532FFC5743777F3C93A6((&V_2), NULL);
		V_1 = L_6;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_7 = V_0;
		NullCheck(L_7);
		int32_t L_8 = L_7->___energy;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_9 = V_0;
		NullCheck(L_9);
		int32_t L_10 = L_9->___energyMax;
		if ((((int32_t)L_8) < ((int32_t)L_10)))
		{
			goto IL_0050;
		}
	}
	{
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_11 = V_0;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_12 = V_0;
		NullCheck(L_12);
		int32_t L_13 = L_12->___energyMax;
		NullCheck(L_11);
		L_11->___energy = L_13;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_14 = V_0;
		int64_t L_15 = V_1;
		NullCheck(L_14);
		L_14->___lastEnergyUtcTicks = L_15;
		goto IL_00be;
	}

IL_0050:
	{
		il2cpp_codegen_runtime_class_init_inline(TimeSpan_t8195C5B013A2C532FEBDF0B64B6911982E750F5A_il2cpp_TypeInfo_var);
		TimeSpan_t8195C5B013A2C532FEBDF0B64B6911982E750F5A L_16;
		L_16 = TimeSpan_FromMinutes_m5748E4C33327BC758C73B6DAE1BDD6BF44711CB5((30.0), NULL);
		V_5 = L_16;
		int64_t L_17;
		L_17 = TimeSpan_get_Ticks_mC50131E57621F29FACC53B3241432ABB874FA1B5_inline((&V_5), NULL);
		V_3 = L_17;
		int64_t L_18 = (il2cpp_codegen_conv<int64_t,int32_t,int32_t,false,false>(0,NULL));
		int64_t L_19 = V_1;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_20 = V_0;
		NullCheck(L_20);
		int64_t L_21 = L_20->___lastEnergyUtcTicks;
		il2cpp_codegen_runtime_class_init_inline(Math_tEB65DE7CA8B083C412C969C92981C030865486CE_il2cpp_TypeInfo_var);
		int64_t L_22;
		L_22 = Math_Max_mD37411571F0547F04F280D5A8D1F044819AFA597(L_18, ((int64_t)il2cpp_codegen_subtract(L_19, L_21)), NULL);
		int64_t L_23 = V_3;
		int32_t L_24 = (il2cpp_codegen_conv<int32_t,int64_t,int64_t,false,false>(((int64_t)(L_22/L_23)),NULL));
		V_4 = L_24;
		int32_t L_25 = V_4;
		if ((((int32_t)L_25) <= ((int32_t)0)))
		{
			goto IL_00be;
		}
	}
	{
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_26 = V_0;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_27 = V_0;
		NullCheck(L_27);
		int32_t L_28 = L_27->___energyMax;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_29 = V_0;
		NullCheck(L_29);
		int32_t L_30 = L_29->___energy;
		int32_t L_31 = V_4;
		int32_t L_32;
		L_32 = Mathf_Min_m888083F74FF5655778F0403BB5E9608BEFDEA8CB_inline(L_28, ((int32_t)il2cpp_codegen_add(L_30, L_31)), NULL);
		NullCheck(L_26);
		L_26->___energy = L_32;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_33 = V_0;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_34 = V_0;
		NullCheck(L_34);
		int32_t L_35 = L_34->___energy;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_36 = V_0;
		NullCheck(L_36);
		int32_t L_37 = L_36->___energyMax;
		if ((((int32_t)L_35) >= ((int32_t)L_37)))
		{
			G_B8_0 = L_33;
			goto IL_00b8;
		}
		G_B7_0 = L_33;
	}
	{
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_38 = V_0;
		NullCheck(L_38);
		int64_t L_39 = L_38->___lastEnergyUtcTicks;
		int32_t L_40 = V_4;
		int64_t L_41 = (il2cpp_codegen_conv<int64_t,int32_t,int32_t,false,false>(L_40,NULL));
		int64_t L_42 = V_3;
		G_B9_0 = ((int64_t)il2cpp_codegen_add(L_39, ((int64_t)il2cpp_codegen_multiply(L_41, L_42))));
		G_B9_1 = G_B7_0;
		goto IL_00b9;
	}

IL_00b8:
	{
		int64_t L_43 = V_1;
		G_B9_0 = L_43;
		G_B9_1 = G_B8_0;
	}

IL_00b9:
	{
		NullCheck(G_B9_1);
		G_B9_1->___lastEnergyUtcTicks = G_B9_0;
	}

IL_00be:
	{
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_44 = V_0;
		NullCheck(L_44);
		int32_t L_45 = L_44->___energy;
		int32_t L_46 = ___0_amount;
		if ((((int32_t)L_45) >= ((int32_t)L_46)))
		{
			goto IL_00c9;
		}
	}
	{
		return (bool)0;
	}

IL_00c9:
	{
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_47 = V_0;
		NullCheck(L_47);
		int32_t L_48 = L_47->___energy;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_49 = V_0;
		NullCheck(L_49);
		int32_t L_50 = L_49->___energyMax;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_51 = V_0;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_52 = L_51;
		NullCheck(L_52);
		int32_t L_53 = L_52->___energy;
		int32_t L_54 = ___0_amount;
		NullCheck(L_52);
		L_52->___energy = ((int32_t)il2cpp_codegen_subtract(L_53, L_54));
		if (!((((int32_t)((((int32_t)L_48) < ((int32_t)L_50))? 1 : 0)) == ((int32_t)0))? 1 : 0))
		{
			goto IL_00f1;
		}
	}
	{
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_55 = V_0;
		int64_t L_56 = V_1;
		NullCheck(L_55);
		L_55->___lastEnergyUtcTicks = L_56;
	}

IL_00f1:
	{
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_57 = V_0;
		bool L_58;
		L_58 = GameState_TryCommitBalance_mD9DA4B9AE5D934AD30A0F070F9C9AE01F165CCD7(__this, L_57, NULL);
		return L_58;
	}
}
