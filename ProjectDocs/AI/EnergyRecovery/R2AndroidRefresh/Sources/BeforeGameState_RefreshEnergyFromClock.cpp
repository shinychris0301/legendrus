IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void GameState_RefreshEnergyFromClock_m62D3547232AF01EC9779ADF04DB1A023B9B8F3B9 (GameState_t5E590D2FB317152A4011A3CA69E3F311E0A7393A* __this, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&DateTime_t66193957C73913903DDAD89FEDC46139BCA5802D_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Math_tEB65DE7CA8B083C412C969C92981C030865486CE_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&TimeSpan_t8195C5B013A2C532FEBDF0B64B6911982E750F5A_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	int64_t V_0 = 0;
	int64_t V_1 = 0;
	int32_t V_2 = 0;
	DateTime_t66193957C73913903DDAD89FEDC46139BCA5802D V_3;
	memset((&V_3), 0, sizeof(V_3));
	TimeSpan_t8195C5B013A2C532FEBDF0B64B6911982E750F5A V_4;
	memset((&V_4), 0, sizeof(V_4));
	{
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_0 = __this->___Save;
		NullCheck(L_0);
		int32_t L_1 = L_0->___energy;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_2 = __this->___Save;
		NullCheck(L_2);
		int32_t L_3 = L_2->___energyMax;
		if ((((int32_t)L_1) < ((int32_t)L_3)))
		{
			goto IL_0047;
		}
	}
	{
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_4 = __this->___Save;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_5 = __this->___Save;
		NullCheck(L_5);
		int32_t L_6 = L_5->___energyMax;
		NullCheck(L_4);
		L_4->___energy = L_6;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_7 = __this->___Save;
		il2cpp_codegen_runtime_class_init_inline(DateTime_t66193957C73913903DDAD89FEDC46139BCA5802D_il2cpp_TypeInfo_var);
		DateTime_t66193957C73913903DDAD89FEDC46139BCA5802D L_8;
		L_8 = DateTime_get_UtcNow_m06B6E9995FE16846A0F71EC9DB23E90BE2C5F9FA(NULL);
		V_3 = L_8;
		int64_t L_9;
		L_9 = DateTime_get_Ticks_mC2CF04ED0EAB425C72C2532FFC5743777F3C93A6((&V_3), NULL);
		NullCheck(L_7);
		L_7->___lastEnergyUtcTicks = L_9;
		return;
	}

IL_0047:
	{
		il2cpp_codegen_runtime_class_init_inline(DateTime_t66193957C73913903DDAD89FEDC46139BCA5802D_il2cpp_TypeInfo_var);
		DateTime_t66193957C73913903DDAD89FEDC46139BCA5802D L_10;
		L_10 = DateTime_get_UtcNow_m06B6E9995FE16846A0F71EC9DB23E90BE2C5F9FA(NULL);
		V_3 = L_10;
		int64_t L_11;
		L_11 = DateTime_get_Ticks_mC2CF04ED0EAB425C72C2532FFC5743777F3C93A6((&V_3), NULL);
		V_0 = L_11;
		il2cpp_codegen_runtime_class_init_inline(TimeSpan_t8195C5B013A2C532FEBDF0B64B6911982E750F5A_il2cpp_TypeInfo_var);
		TimeSpan_t8195C5B013A2C532FEBDF0B64B6911982E750F5A L_12;
		L_12 = TimeSpan_FromMinutes_m5748E4C33327BC758C73B6DAE1BDD6BF44711CB5((30.0), NULL);
		V_4 = L_12;
		int64_t L_13;
		L_13 = TimeSpan_get_Ticks_mC50131E57621F29FACC53B3241432ABB874FA1B5_inline((&V_4), NULL);
		V_1 = L_13;
		int64_t L_14 = (il2cpp_codegen_conv<int64_t,int32_t,int32_t,false,false>(0,NULL));
		int64_t L_15 = V_0;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_16 = __this->___Save;
		NullCheck(L_16);
		int64_t L_17 = L_16->___lastEnergyUtcTicks;
		il2cpp_codegen_runtime_class_init_inline(Math_tEB65DE7CA8B083C412C969C92981C030865486CE_il2cpp_TypeInfo_var);
		int64_t L_18;
		L_18 = Math_Max_mD37411571F0547F04F280D5A8D1F044819AFA597(L_14, ((int64_t)il2cpp_codegen_subtract(L_15, L_17)), NULL);
		int64_t L_19 = V_1;
		int32_t L_20 = (il2cpp_codegen_conv<int32_t,int64_t,int64_t,false,false>(((int64_t)(L_18/L_19)),NULL));
		V_2 = L_20;
		int32_t L_21 = V_2;
		if ((((int32_t)L_21) > ((int32_t)0)))
		{
			goto IL_008a;
		}
	}
	{
		return;
	}

IL_008a:
	{
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_22 = __this->___Save;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_23 = __this->___Save;
		NullCheck(L_23);
		int32_t L_24 = L_23->___energyMax;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_25 = __this->___Save;
		NullCheck(L_25);
		int32_t L_26 = L_25->___energy;
		int32_t L_27 = V_2;
		int32_t L_28;
		L_28 = Mathf_Min_m888083F74FF5655778F0403BB5E9608BEFDEA8CB_inline(L_24, ((int32_t)il2cpp_codegen_add(L_26, L_27)), NULL);
		NullCheck(L_22);
		L_22->___energy = L_28;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_29 = __this->___Save;
		NullCheck(L_29);
		int32_t L_30 = L_29->___energy;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_31 = __this->___Save;
		NullCheck(L_31);
		int32_t L_32 = L_31->___energyMax;
		if ((((int32_t)L_30) < ((int32_t)L_32)))
		{
			goto IL_00d8;
		}
	}
	{
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_33 = __this->___Save;
		int64_t L_34 = V_0;
		NullCheck(L_33);
		L_33->___lastEnergyUtcTicks = L_34;
		goto IL_00ee;
	}

IL_00d8:
	{
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_35 = __this->___Save;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_36 = L_35;
		NullCheck(L_36);
		int64_t L_37 = L_36->___lastEnergyUtcTicks;
		int32_t L_38 = V_2;
		int64_t L_39 = (il2cpp_codegen_conv<int64_t,int32_t,int32_t,false,false>(L_38,NULL));
		int64_t L_40 = V_1;
		NullCheck(L_36);
		L_36->___lastEnergyUtcTicks = ((int64_t)il2cpp_codegen_add(L_37, ((int64_t)il2cpp_codegen_multiply(L_39, L_40))));
	}

IL_00ee:
	{
		GameState_Persist_m0261476FB722D9A1BAB0BB601B749EFC7E307C8D(__this, NULL);
		return;
	}
}
