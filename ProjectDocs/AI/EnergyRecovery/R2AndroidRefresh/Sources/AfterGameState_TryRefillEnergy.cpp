IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool GameState_TryRefillEnergy_mCA72E86354348135DFFF044962CEFA86DA2211F9 (GameState_t5E590D2FB317152A4011A3CA69E3F311E0A7393A* __this, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&DateTime_t66193957C73913903DDAD89FEDC46139BCA5802D_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&JsonUtility_FromJson_TisPlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30_m5FA532D489BA59FC0DD187FA87D56E71F1F63B04_RuntimeMethod_var);
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
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_5 = V_0;
		NullCheck(L_5);
		int32_t L_6 = L_5->___energyMax;
		NullCheck(L_4);
		L_4->___energy = L_6;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_7 = V_0;
		il2cpp_codegen_runtime_class_init_inline(DateTime_t66193957C73913903DDAD89FEDC46139BCA5802D_il2cpp_TypeInfo_var);
		DateTime_t66193957C73913903DDAD89FEDC46139BCA5802D L_8;
		L_8 = DateTime_get_UtcNow_m06B6E9995FE16846A0F71EC9DB23E90BE2C5F9FA(NULL);
		V_1 = L_8;
		int64_t L_9;
		L_9 = DateTime_get_Ticks_mC2CF04ED0EAB425C72C2532FFC5743777F3C93A6((&V_1), NULL);
		NullCheck(L_7);
		L_7->___lastEnergyUtcTicks = L_9;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_10 = V_0;
		bool L_11;
		L_11 = GameState_TryCommitBalance_mD9DA4B9AE5D934AD30A0F070F9C9AE01F165CCD7(__this, L_10, NULL);
		return L_11;
	}
}
