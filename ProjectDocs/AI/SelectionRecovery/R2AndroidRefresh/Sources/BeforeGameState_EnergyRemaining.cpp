IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR TimeSpan_t8195C5B013A2C532FEBDF0B64B6911982E750F5A GameState_EnergyRemaining_mFA11989BC7F2AF0C3406DDFF40C38E0576FA1070 (GameState_t5E590D2FB317152A4011A3CA69E3F311E0A7393A* __this, const RuntimeMethod* method) 
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
	int64_t V_2 = 0;
	int64_t V_3 = 0;
	TimeSpan_t8195C5B013A2C532FEBDF0B64B6911982E750F5A V_4;
	memset((&V_4), 0, sizeof(V_4));
	DateTime_t66193957C73913903DDAD89FEDC46139BCA5802D V_5;
	memset((&V_5), 0, sizeof(V_5));
	{
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_0 = __this->___Save;
		NullCheck(L_0);
		int32_t L_1 = L_0->___energy;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_2 = __this->___Save;
		NullCheck(L_2);
		int32_t L_3 = L_2->___energyMax;
		if ((((int32_t)L_1) < ((int32_t)L_3)))
		{
			goto IL_001e;
		}
	}
	{
		il2cpp_codegen_runtime_class_init_inline(TimeSpan_t8195C5B013A2C532FEBDF0B64B6911982E750F5A_il2cpp_TypeInfo_var);
		TimeSpan_t8195C5B013A2C532FEBDF0B64B6911982E750F5A L_4 = ((TimeSpan_t8195C5B013A2C532FEBDF0B64B6911982E750F5A_StaticFields*)il2cpp_codegen_static_fields_for(TimeSpan_t8195C5B013A2C532FEBDF0B64B6911982E750F5A_il2cpp_TypeInfo_var))->___Zero;
		return L_4;
	}

IL_001e:
	{
		il2cpp_codegen_runtime_class_init_inline(TimeSpan_t8195C5B013A2C532FEBDF0B64B6911982E750F5A_il2cpp_TypeInfo_var);
		TimeSpan_t8195C5B013A2C532FEBDF0B64B6911982E750F5A L_5;
		L_5 = TimeSpan_FromMinutes_m5748E4C33327BC758C73B6DAE1BDD6BF44711CB5((30.0), NULL);
		V_4 = L_5;
		int64_t L_6;
		L_6 = TimeSpan_get_Ticks_mC50131E57621F29FACC53B3241432ABB874FA1B5_inline((&V_4), NULL);
		V_0 = L_6;
		il2cpp_codegen_runtime_class_init_inline(DateTime_t66193957C73913903DDAD89FEDC46139BCA5802D_il2cpp_TypeInfo_var);
		DateTime_t66193957C73913903DDAD89FEDC46139BCA5802D L_7;
		L_7 = DateTime_get_UtcNow_m06B6E9995FE16846A0F71EC9DB23E90BE2C5F9FA(NULL);
		V_5 = L_7;
		int64_t L_8;
		L_8 = DateTime_get_Ticks_mC2CF04ED0EAB425C72C2532FFC5743777F3C93A6((&V_5), NULL);
		V_1 = L_8;
		int64_t L_9 = (il2cpp_codegen_conv<int64_t,int32_t,int32_t,false,false>(0,NULL));
		int64_t L_10 = V_1;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_11 = __this->___Save;
		NullCheck(L_11);
		int64_t L_12 = L_11->___lastEnergyUtcTicks;
		il2cpp_codegen_runtime_class_init_inline(Math_tEB65DE7CA8B083C412C969C92981C030865486CE_il2cpp_TypeInfo_var);
		int64_t L_13;
		L_13 = Math_Max_mD37411571F0547F04F280D5A8D1F044819AFA597(L_9, ((int64_t)il2cpp_codegen_subtract(L_10, L_12)), NULL);
		V_2 = L_13;
		int64_t L_14 = V_0;
		int64_t L_15 = V_2;
		int64_t L_16 = V_0;
		V_3 = ((int64_t)il2cpp_codegen_subtract(L_14, ((int64_t)(L_15%L_16))));
		int64_t L_17 = (il2cpp_codegen_conv<int64_t,int32_t,int32_t,false,false>(0,NULL));
		int64_t L_18 = V_3;
		int64_t L_19;
		L_19 = Math_Max_mD37411571F0547F04F280D5A8D1F044819AFA597(L_17, L_18, NULL);
		TimeSpan_t8195C5B013A2C532FEBDF0B64B6911982E750F5A L_20;
		L_20 = TimeSpan_FromTicks_m9C683B1D142312F22E8CC6C803E32EF6D42F9003(L_19, NULL);
		return L_20;
	}
}
