IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool GameState_TryRegisterLevelAttempt_mAF97A5BE4A922ECAE59CE0DCD3DCD6649AFFC57E (GameState_t5E590D2FB317152A4011A3CA69E3F311E0A7393A* __this, LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* ___0_levelData, int32_t* ___1_attempt, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&JsonUtility_FromJson_TisPlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30_m5FA532D489BA59FC0DD187FA87D56E71F1F63B04_RuntimeMethod_var);
		s_Il2CppMethodInitialized = true;
	}
	PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* V_0 = NULL;
	int32_t V_1 = 0;
	{
		int32_t* L_0 = ___1_attempt;
		il2cpp_codegen_stind<int32_t>((int32_t*)L_0, (int32_t)0);
		LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* L_1 = ___0_levelData;
		bool L_2;
		L_2 = GameState_CanRegisterLevelAttempt_m509E53F6E5D8D3EC951AD4C656B4C83BE02A853A(__this, L_1, NULL);
		if (L_2)
		{
			goto IL_000e;
		}
	}
	{
		return (bool)0;
	}

IL_000e:
	{
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_3 = __this->___Save;
		String_t* L_4;
		L_4 = JsonUtility_ToJson_m28CC6843B9D3723D88AD13EA3829B71FDE7826BA(L_3, NULL);
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_5;
		L_5 = JsonUtility_FromJson_TisPlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30_m5FA532D489BA59FC0DD187FA87D56E71F1F63B04(L_4, JsonUtility_FromJson_TisPlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30_m5FA532D489BA59FC0DD187FA87D56E71F1F63B04_RuntimeMethod_var);
		V_0 = L_5;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_6 = V_0;
		LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* L_7 = ___0_levelData;
		bool L_8;
		L_8 = GameState_TryPrepareLevelAttempt_m1B62820945E8D99C9C474070E9DC25AE1ADDFB50(L_6, L_7, (&V_1), NULL);
		if (!L_8)
		{
			goto IL_0035;
		}
	}
	{
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_9 = V_0;
		bool L_10;
		L_10 = GameState_TryCommitRoundCandidate_mF567FE151D88B0B07E7F938F7534DF807FD76CE9(__this, L_9, (LevelFailureResult_tEAFB9DEC27BA6439583A47C07A1D65D0B79D12E0*)NULL, (bool)1, NULL);
		if (L_10)
		{
			goto IL_0037;
		}
	}

IL_0035:
	{
		return (bool)0;
	}

IL_0037:
	{
		int32_t* L_11 = ___1_attempt;
		int32_t L_12 = V_1;
		il2cpp_codegen_stind<int32_t>((int32_t*)L_11, (int32_t)L_12);
		return (bool)1;
	}
}
