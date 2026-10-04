IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool GameState_TryStartLevel_mF8B8C9FE72F7FDB5CFDAFF42E0EA26303D283D30 (GameState_t5E590D2FB317152A4011A3CA69E3F311E0A7393A* __this, LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* ___0_levelData, int32_t* ___1_attempt, bool* ___2_insufficientEnergy, const RuntimeMethod* method) 
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
	int32_t V_2 = 0;
	DateTime_t66193957C73913903DDAD89FEDC46139BCA5802D V_3;
	memset((&V_3), 0, sizeof(V_3));
	{
		int32_t* L_0 = ___1_attempt;
		il2cpp_codegen_stind<int32_t>((int32_t*)L_0, (int32_t)0);
		bool* L_1 = ___2_insufficientEnergy;
		il2cpp_codegen_stind<int8_t>((int8_t*)L_1, (int8_t)0);
		LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* L_2 = ___0_levelData;
		bool L_3;
		L_3 = GameState_CanRegisterLevelAttempt_m509E53F6E5D8D3EC951AD4C656B4C83BE02A853A(__this, L_2, NULL);
		if (!L_3)
		{
			goto IL_0017;
		}
	}
	{
		bool L_4;
		L_4 = GameState_get_HasPendingBoosterCommand_m3E1BA9F545A38895AD411E75F5DE3C9F67C76C15(__this, NULL);
		if (!L_4)
		{
			goto IL_0019;
		}
	}

IL_0017:
	{
		return (bool)0;
	}

IL_0019:
	{
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_5 = __this->___Save;
		String_t* L_6;
		L_6 = JsonUtility_ToJson_m28CC6843B9D3723D88AD13EA3829B71FDE7826BA(L_5, NULL);
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_7;
		L_7 = JsonUtility_FromJson_TisPlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30_m5FA532D489BA59FC0DD187FA87D56E71F1F63B04(L_6, JsonUtility_FromJson_TisPlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30_m5FA532D489BA59FC0DD187FA87D56E71F1F63B04_RuntimeMethod_var);
		V_0 = L_7;
		il2cpp_codegen_runtime_class_init_inline(DateTime_t66193957C73913903DDAD89FEDC46139BCA5802D_il2cpp_TypeInfo_var);
		DateTime_t66193957C73913903DDAD89FEDC46139BCA5802D L_8;
		L_8 = DateTime_get_UtcNow_m06B6E9995FE16846A0F71EC9DB23E90BE2C5F9FA(NULL);
		V_3 = L_8;
		int64_t L_9;
		L_9 = DateTime_get_Ticks_mC2CF04ED0EAB425C72C2532FFC5743777F3C93A6((&V_3), NULL);
		V_1 = L_9;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_10 = V_0;
		int64_t L_11 = V_1;
		bool L_12;
		L_12 = GameState_AdvanceEnergyClock_mAEF469217E6DAA91EEF60CBAF2437534EDDA45F6(L_10, L_11, NULL);
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_13 = V_0;
		NullCheck(L_13);
		int32_t L_14 = L_13->___energy;
		if ((((int32_t)L_14) >= ((int32_t)1)))
		{
			goto IL_004e;
		}
	}
	{
		bool* L_15 = ___2_insufficientEnergy;
		il2cpp_codegen_stind<int8_t>((int8_t*)L_15, (int8_t)1);
		return (bool)0;
	}

IL_004e:
	{
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_16 = V_0;
		NullCheck(L_16);
		int32_t L_17 = L_16->___energy;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_18 = V_0;
		NullCheck(L_18);
		int32_t L_19 = L_18->___energyMax;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_20 = V_0;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_21 = L_20;
		NullCheck(L_21);
		int32_t L_22 = L_21->___energy;
		NullCheck(L_21);
		L_21->___energy = ((int32_t)il2cpp_codegen_subtract(L_22, 1));
		if (!((((int32_t)((((int32_t)L_17) < ((int32_t)L_19))? 1 : 0)) == ((int32_t)0))? 1 : 0))
		{
			goto IL_0076;
		}
	}
	{
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_23 = V_0;
		int64_t L_24 = V_1;
		NullCheck(L_23);
		L_23->___lastEnergyUtcTicks = L_24;
	}

IL_0076:
	{
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_25 = V_0;
		LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* L_26 = ___0_levelData;
		bool L_27;
		L_27 = GameState_TryPrepareLevelAttempt_m1B62820945E8D99C9C474070E9DC25AE1ADDFB50(L_25, L_26, (&V_2), NULL);
		if (!L_27)
		{
			goto IL_008c;
		}
	}
	{
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_28 = V_0;
		bool L_29;
		L_29 = GameState_TryCommitRoundCandidate_mF567FE151D88B0B07E7F938F7534DF807FD76CE9(__this, L_28, (LevelFailureResult_tEAFB9DEC27BA6439583A47C07A1D65D0B79D12E0*)NULL, (bool)1, NULL);
		if (L_29)
		{
			goto IL_008e;
		}
	}

IL_008c:
	{
		return (bool)0;
	}

IL_008e:
	{
		int32_t* L_30 = ___1_attempt;
		int32_t L_31 = V_2;
		il2cpp_codegen_stind<int32_t>((int32_t*)L_30, (int32_t)L_31);
		return (bool)1;
	}
}
