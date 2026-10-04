IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void GameState_AddEnergy_m3774D9130904C361E9039A8CBE6EE009801D2A17 (GameState_t5E590D2FB317152A4011A3CA69E3F311E0A7393A* __this, int32_t ___0_amount, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&DateTime_t66193957C73913903DDAD89FEDC46139BCA5802D_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	DateTime_t66193957C73913903DDAD89FEDC46139BCA5802D V_0;
	memset((&V_0), 0, sizeof(V_0));
	{
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_0 = __this->___Save;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_1 = __this->___Save;
		NullCheck(L_1);
		int32_t L_2 = L_1->___energy;
		int32_t L_3 = ___0_amount;
		int32_t L_4;
		L_4 = Mathf_Max_m7FA442918DE37E3A00106D1F2E789D65829792B8_inline(0, L_3, NULL);
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_5 = __this->___Save;
		NullCheck(L_5);
		int32_t L_6 = L_5->___energyMax;
		int32_t L_7;
		L_7 = Mathf_Clamp_m4DC36EEFDBE5F07C16249DA568023C5ECCFF0E7B_inline(((int32_t)il2cpp_codegen_add(L_2, L_4)), 0, L_6, NULL);
		NullCheck(L_0);
		L_0->___energy = L_7;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_8 = __this->___Save;
		NullCheck(L_8);
		int32_t L_9 = L_8->___energy;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_10 = __this->___Save;
		NullCheck(L_10);
		int32_t L_11 = L_10->___energyMax;
		if ((((int32_t)L_9) < ((int32_t)L_11)))
		{
			goto IL_005f;
		}
	}
	{
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_12 = __this->___Save;
		il2cpp_codegen_runtime_class_init_inline(DateTime_t66193957C73913903DDAD89FEDC46139BCA5802D_il2cpp_TypeInfo_var);
		DateTime_t66193957C73913903DDAD89FEDC46139BCA5802D L_13;
		L_13 = DateTime_get_UtcNow_m06B6E9995FE16846A0F71EC9DB23E90BE2C5F9FA(NULL);
		V_0 = L_13;
		int64_t L_14;
		L_14 = DateTime_get_Ticks_mC2CF04ED0EAB425C72C2532FFC5743777F3C93A6((&V_0), NULL);
		NullCheck(L_12);
		L_12->___lastEnergyUtcTicks = L_14;
	}

IL_005f:
	{
		GameState_Persist_m0261476FB722D9A1BAB0BB601B749EFC7E307C8D(__this, NULL);
		return;
	}
}
