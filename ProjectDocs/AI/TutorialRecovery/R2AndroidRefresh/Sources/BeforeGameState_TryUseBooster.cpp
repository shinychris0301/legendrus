IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool GameState_TryUseBooster_m19110E094D19701126B89AC8583412890372C299 (GameState_t5E590D2FB317152A4011A3CA69E3F311E0A7393A* __this, int32_t ___0_type, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&JsonUtility_FromJson_TisPlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30_m5FA532D489BA59FC0DD187FA87D56E71F1F63B04_RuntimeMethod_var);
		s_Il2CppMethodInitialized = true;
	}
	PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* V_0 = NULL;
	{
		bool L_0 = __this->___canPersist;
		if (!L_0)
		{
			goto IL_001a;
		}
	}
	{
		int32_t L_1 = ___0_type;
		bool L_2;
		L_2 = CampaignBoosterTransactions_Known_mB4D91A0E09F3B6EC152EA6591E4BAA3905B6FA89(L_1, NULL);
		if (!L_2)
		{
			goto IL_001a;
		}
	}
	{
		int32_t L_3 = ___0_type;
		int32_t L_4;
		L_4 = GameState_BoosterCount_mCE82517DEC995B50BD88A948B1C2F7FF82EAD191(__this, L_3, NULL);
		if ((((int32_t)L_4) >= ((int32_t)1)))
		{
			goto IL_001c;
		}
	}

IL_001a:
	{
		return (bool)0;
	}

IL_001c:
	{
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_5 = __this->___Save;
		String_t* L_6;
		L_6 = JsonUtility_ToJson_m28CC6843B9D3723D88AD13EA3829B71FDE7826BA(L_5, NULL);
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_7;
		L_7 = JsonUtility_FromJson_TisPlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30_m5FA532D489BA59FC0DD187FA87D56E71F1F63B04(L_6, JsonUtility_FromJson_TisPlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30_m5FA532D489BA59FC0DD187FA87D56E71F1F63B04_RuntimeMethod_var);
		V_0 = L_7;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_8 = V_0;
		int32_t L_9 = ___0_type;
		CampaignBoosterTransactions_Add_m2E76DA0F9FC77EA8CAE42488806D10B97A720D9D(L_8, L_9, (-1), NULL);
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_10 = V_0;
		bool L_11;
		L_11 = GameState_TryCommitBalance_mD9DA4B9AE5D934AD30A0F070F9C9AE01F165CCD7(__this, L_10, NULL);
		return L_11;
	}
}
