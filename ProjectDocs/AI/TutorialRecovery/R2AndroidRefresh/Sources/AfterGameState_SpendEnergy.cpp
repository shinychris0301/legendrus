IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool GameState_SpendEnergy_mD8D9F45B107F9A46577C939F1784260B2D097D5D (GameState_t5E590D2FB317152A4011A3CA69E3F311E0A7393A* __this, int32_t ___0_amount, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&DateTime_t66193957C73913903DDAD89FEDC46139BCA5802D_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&JsonUtility_FromJson_TisPlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30_m5FA532D489BA59FC0DD187FA87D56E71F1F63B04_RuntimeMethod_var);
		s_Il2CppMethodInitialized = true;
	}
	PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* V_0 = NULL;
	int64_t V_1 = 0;
	DateTime_t66193957C73913903DDAD89FEDC46139BCA5802D V_2;
	memset((&V_2), 0, sizeof(V_2));
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
		int64_t L_8 = V_1;
		bool L_9;
		L_9 = GameState_AdvanceEnergyClock_mAEF469217E6DAA91EEF60CBAF2437534EDDA45F6(L_7, L_8, NULL);
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_10 = V_0;
		NullCheck(L_10);
		int32_t L_11 = L_10->___energy;
		int32_t L_12 = ___0_amount;
		if ((((int32_t)L_11) >= ((int32_t)L_12)))
		{
			goto IL_0040;
		}
	}
	{
		return (bool)0;
	}

IL_0040:
	{
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_13 = V_0;
		NullCheck(L_13);
		int32_t L_14 = L_13->___energy;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_15 = V_0;
		NullCheck(L_15);
		int32_t L_16 = L_15->___energyMax;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_17 = V_0;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_18 = L_17;
		NullCheck(L_18);
		int32_t L_19 = L_18->___energy;
		int32_t L_20 = ___0_amount;
		NullCheck(L_18);
		L_18->___energy = ((int32_t)il2cpp_codegen_subtract(L_19, L_20));
		if (!((((int32_t)((((int32_t)L_14) < ((int32_t)L_16))? 1 : 0)) == ((int32_t)0))? 1 : 0))
		{
			goto IL_0068;
		}
	}
	{
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_21 = V_0;
		int64_t L_22 = V_1;
		NullCheck(L_21);
		L_21->___lastEnergyUtcTicks = L_22;
	}

IL_0068:
	{
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_23 = V_0;
		bool L_24;
		L_24 = GameState_TryCommitBalance_mD9DA4B9AE5D934AD30A0F070F9C9AE01F165CCD7(__this, L_23, NULL);
		return L_24;
	}
}
