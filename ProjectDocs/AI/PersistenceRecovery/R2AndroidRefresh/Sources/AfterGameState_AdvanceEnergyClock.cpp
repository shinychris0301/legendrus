IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool GameState_AdvanceEnergyClock_mAEF469217E6DAA91EEF60CBAF2437534EDDA45F6 (PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* ___0_candidate, int64_t ___1_now, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Math_tEB65DE7CA8B083C412C969C92981C030865486CE_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&TimeSpan_t8195C5B013A2C532FEBDF0B64B6911982E750F5A_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	int64_t V_0 = 0;
	int64_t V_1 = 0;
	TimeSpan_t8195C5B013A2C532FEBDF0B64B6911982E750F5A V_2;
	memset((&V_2), 0, sizeof(V_2));
	PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* G_B8_0 = NULL;
	PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* G_B7_0 = NULL;
	int64_t G_B9_0 = 0;
	PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* G_B9_1 = NULL;
	{
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_0 = ___0_candidate;
		NullCheck(L_0);
		int32_t L_1 = L_0->___energy;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_2 = ___0_candidate;
		NullCheck(L_2);
		int32_t L_3 = L_2->___energyMax;
		if ((((int32_t)L_1) < ((int32_t)L_3)))
		{
			goto IL_0033;
		}
	}
	{
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_4 = ___0_candidate;
		NullCheck(L_4);
		int32_t L_5 = L_4->___energy;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_6 = ___0_candidate;
		NullCheck(L_6);
		int32_t L_7 = L_6->___energyMax;
		if ((!(((uint32_t)L_5) == ((uint32_t)L_7))))
		{
			goto IL_001e;
		}
	}
	{
		return (bool)0;
	}

IL_001e:
	{
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_8 = ___0_candidate;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_9 = ___0_candidate;
		NullCheck(L_9);
		int32_t L_10 = L_9->___energyMax;
		NullCheck(L_8);
		L_8->___energy = L_10;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_11 = ___0_candidate;
		int64_t L_12 = ___1_now;
		NullCheck(L_11);
		L_11->___lastEnergyUtcTicks = L_12;
		return (bool)1;
	}

IL_0033:
	{
		il2cpp_codegen_runtime_class_init_inline(TimeSpan_t8195C5B013A2C532FEBDF0B64B6911982E750F5A_il2cpp_TypeInfo_var);
		TimeSpan_t8195C5B013A2C532FEBDF0B64B6911982E750F5A L_13;
		L_13 = TimeSpan_FromMinutes_m5748E4C33327BC758C73B6DAE1BDD6BF44711CB5((30.0), NULL);
		V_2 = L_13;
		int64_t L_14;
		L_14 = TimeSpan_get_Ticks_mC50131E57621F29FACC53B3241432ABB874FA1B5_inline((&V_2), NULL);
		V_0 = L_14;
		int64_t L_15 = (il2cpp_codegen_conv<int64_t,int32_t,int32_t,false,false>(0,NULL));
		int64_t L_16 = ___1_now;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_17 = ___0_candidate;
		NullCheck(L_17);
		int64_t L_18 = L_17->___lastEnergyUtcTicks;
		il2cpp_codegen_runtime_class_init_inline(Math_tEB65DE7CA8B083C412C969C92981C030865486CE_il2cpp_TypeInfo_var);
		int64_t L_19;
		L_19 = Math_Max_mD37411571F0547F04F280D5A8D1F044819AFA597(L_15, ((int64_t)il2cpp_codegen_subtract(L_16, L_18)), NULL);
		int64_t L_20 = V_0;
		V_1 = ((int64_t)(L_19/L_20));
		int64_t L_21 = V_1;
		int64_t L_22 = (il2cpp_codegen_conv<int64_t,int32_t,int32_t,false,false>(0,NULL));
		if ((((int64_t)L_21) > ((int64_t)L_22)))
		{
			goto IL_0063;
		}
	}
	{
		return (bool)0;
	}

IL_0063:
	{
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_23 = ___0_candidate;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_24 = ___0_candidate;
		NullCheck(L_24);
		int32_t L_25 = L_24->___energyMax;
		int64_t L_26 = (il2cpp_codegen_conv<int64_t,int32_t,int32_t,false,false>(L_25,NULL));
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_27 = ___0_candidate;
		NullCheck(L_27);
		int32_t L_28 = L_27->___energy;
		int64_t L_29 = (il2cpp_codegen_conv<int64_t,int32_t,int32_t,false,false>(L_28,NULL));
		int64_t L_30 = V_1;
		il2cpp_codegen_runtime_class_init_inline(Math_tEB65DE7CA8B083C412C969C92981C030865486CE_il2cpp_TypeInfo_var);
		int64_t L_31;
		L_31 = Math_Min_mD731E8A02F13C67C1EAC7C1E7F81909FE466F079(L_26, ((int64_t)il2cpp_codegen_add(L_29, L_30)), NULL);
		int32_t L_32 = (il2cpp_codegen_conv<int32_t,int64_t,int64_t,false,false>(L_31,NULL));
		NullCheck(L_23);
		L_23->___energy = L_32;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_33 = ___0_candidate;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_34 = ___0_candidate;
		NullCheck(L_34);
		int32_t L_35 = L_34->___energy;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_36 = ___0_candidate;
		NullCheck(L_36);
		int32_t L_37 = L_36->___energyMax;
		if ((((int32_t)L_35) >= ((int32_t)L_37)))
		{
			G_B8_0 = L_33;
			goto IL_009a;
		}
		G_B7_0 = L_33;
	}
	{
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_38 = ___0_candidate;
		NullCheck(L_38);
		int64_t L_39 = L_38->___lastEnergyUtcTicks;
		int64_t L_40 = V_1;
		int64_t L_41 = V_0;
		G_B9_0 = ((int64_t)il2cpp_codegen_add(L_39, ((int64_t)il2cpp_codegen_multiply(L_40, L_41))));
		G_B9_1 = G_B7_0;
		goto IL_009b;
	}

IL_009a:
	{
		int64_t L_42 = ___1_now;
		G_B9_0 = L_42;
		G_B9_1 = G_B8_0;
	}

IL_009b:
	{
		NullCheck(G_B9_1);
		G_B9_1->___lastEnergyUtcTicks = G_B9_0;
		return (bool)1;
	}
}
