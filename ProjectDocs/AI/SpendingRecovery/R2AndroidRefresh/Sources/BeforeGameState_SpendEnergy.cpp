IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool GameState_SpendEnergy_mD8D9F45B107F9A46577C939F1784260B2D097D5D (GameState_t5E590D2FB317152A4011A3CA69E3F311E0A7393A* __this, int32_t ___0_amount, const RuntimeMethod* method) 
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
		GameState_RefreshEnergyFromClock_m62D3547232AF01EC9779ADF04DB1A023B9B8F3B9(__this, NULL);
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_0 = __this->___Save;
		NullCheck(L_0);
		int32_t L_1 = L_0->___energy;
		int32_t L_2 = ___0_amount;
		if ((((int32_t)L_1) >= ((int32_t)L_2)))
		{
			goto IL_0016;
		}
	}
	{
		return (bool)0;
	}

IL_0016:
	{
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_3 = __this->___Save;
		NullCheck(L_3);
		int32_t L_4 = L_3->___energy;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_5 = __this->___Save;
		NullCheck(L_5);
		int32_t L_6 = L_5->___energyMax;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_7 = __this->___Save;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_8 = L_7;
		NullCheck(L_8);
		int32_t L_9 = L_8->___energy;
		int32_t L_10 = ___0_amount;
		NullCheck(L_8);
		L_8->___energy = ((int32_t)il2cpp_codegen_subtract(L_9, L_10));
		if (!((((int32_t)((((int32_t)L_4) < ((int32_t)L_6))? 1 : 0)) == ((int32_t)0))? 1 : 0))
		{
			goto IL_005e;
		}
	}
	{
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_11 = __this->___Save;
		il2cpp_codegen_runtime_class_init_inline(DateTime_t66193957C73913903DDAD89FEDC46139BCA5802D_il2cpp_TypeInfo_var);
		DateTime_t66193957C73913903DDAD89FEDC46139BCA5802D L_12;
		L_12 = DateTime_get_UtcNow_m06B6E9995FE16846A0F71EC9DB23E90BE2C5F9FA(NULL);
		V_0 = L_12;
		int64_t L_13;
		L_13 = DateTime_get_Ticks_mC2CF04ED0EAB425C72C2532FFC5743777F3C93A6((&V_0), NULL);
		NullCheck(L_11);
		L_11->___lastEnergyUtcTicks = L_13;
	}

IL_005e:
	{
		GameState_Persist_m0261476FB722D9A1BAB0BB601B749EFC7E307C8D(__this, NULL);
		return (bool)1;
	}
}
