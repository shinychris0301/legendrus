IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool GameState_ClaimDailyReward_m58913FA12C1C4EFB0077B14E802668EA7711A393 (GameState_t5E590D2FB317152A4011A3CA69E3F311E0A7393A* __this, DailyRewardGrant_t3E6F1ABD071C4E274E3257577773D20AC5C5DD69* ___0_granted, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&DateTime_t66193957C73913903DDAD89FEDC46139BCA5802D_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&JsonUtility_FromJson_TisPlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30_m5FA532D489BA59FC0DD187FA87D56E71F1F63B04_RuntimeMethod_var);
		s_Il2CppMethodInitialized = true;
	}
	DateTime_t66193957C73913903DDAD89FEDC46139BCA5802D V_0;
	memset((&V_0), 0, sizeof(V_0));
	PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* V_1 = NULL;
	PlayerSaveWriteResult_tD3521C4F0AD4F171529F5475A2BD7B28C0CB1D9C* V_2 = NULL;
	Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* G_B9_0 = NULL;
	Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* G_B8_0 = NULL;
	{
		DailyRewardGrant_t3E6F1ABD071C4E274E3257577773D20AC5C5DD69* L_0 = ___0_granted;
		il2cpp_codegen_initobj(L_0, sizeof(DailyRewardGrant_t3E6F1ABD071C4E274E3257577773D20AC5C5DD69));
		il2cpp_codegen_runtime_class_init_inline(DateTime_t66193957C73913903DDAD89FEDC46139BCA5802D_il2cpp_TypeInfo_var);
		DateTime_t66193957C73913903DDAD89FEDC46139BCA5802D L_1;
		L_1 = DateTime_get_UtcNow_m06B6E9995FE16846A0F71EC9DB23E90BE2C5F9FA(NULL);
		V_0 = L_1;
		bool L_2 = __this->___canPersist;
		if (!L_2)
		{
			goto IL_0023;
		}
	}
	{
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_3 = __this->___Save;
		DateTime_t66193957C73913903DDAD89FEDC46139BCA5802D L_4 = V_0;
		bool L_5;
		L_5 = DailyRewardSchedule_CanClaim_mF9F976A98717EDA80D54038BAB255A84B8863C1B(L_3, L_4, NULL);
		if (L_5)
		{
			goto IL_0025;
		}
	}

IL_0023:
	{
		return (bool)0;
	}

IL_0025:
	{
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_6 = __this->___Save;
		String_t* L_7;
		L_7 = JsonUtility_ToJson_m28CC6843B9D3723D88AD13EA3829B71FDE7826BA(L_6, NULL);
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_8;
		L_8 = JsonUtility_FromJson_TisPlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30_m5FA532D489BA59FC0DD187FA87D56E71F1F63B04(L_7, JsonUtility_FromJson_TisPlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30_m5FA532D489BA59FC0DD187FA87D56E71F1F63B04_RuntimeMethod_var);
		V_1 = L_8;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_9 = V_1;
		DateTime_t66193957C73913903DDAD89FEDC46139BCA5802D L_10 = V_0;
		bool L_11;
		L_11 = DailyRewardSchedule_TryApply_mF65B3D7F66337A25BE9E31E6716F93E9BA33D878(L_9, L_10, NULL);
		if (L_11)
		{
			goto IL_0041;
		}
	}
	{
		return (bool)0;
	}

IL_0041:
	{
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_12 = V_1;
		bool L_13;
		L_13 = SaveService_TrySave_m11A043A57FCBE0607BDAAA6C34B4E969064615B0(L_12, (&V_2), NULL);
		PlayerSaveWriteResult_tD3521C4F0AD4F171529F5475A2BD7B28C0CB1D9C* L_14 = V_2;
		GameState_set_LastWriteResult_mE42274830424D045544D465431B5C582BC50A567_inline(__this, L_14, NULL);
		if (L_13)
		{
			goto IL_005b;
		}
	}
	{
		__this->___canPersist = (bool)0;
		return (bool)0;
	}

IL_005b:
	{
		DailyRewardGrant_t3E6F1ABD071C4E274E3257577773D20AC5C5DD69* L_15 = ___0_granted;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_16 = V_1;
		NullCheck(L_16);
		int32_t L_17 = L_16->___gold;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_18 = __this->___Save;
		NullCheck(L_18);
		int32_t L_19 = L_18->___gold;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_20 = V_1;
		NullCheck(L_20);
		int32_t L_21 = L_20->___gems;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_22 = __this->___Save;
		NullCheck(L_22);
		int32_t L_23 = L_22->___gems;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_24 = V_1;
		NullCheck(L_24);
		int32_t L_25 = L_24->___energy;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_26 = __this->___Save;
		NullCheck(L_26);
		int32_t L_27 = L_26->___energy;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_28 = V_1;
		NullCheck(L_28);
		int32_t L_29 = L_28->___boosterMace;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_30 = __this->___Save;
		NullCheck(L_30);
		int32_t L_31 = L_30->___boosterMace;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_32 = V_1;
		NullCheck(L_32);
		int32_t L_33 = L_32->___boosterFirebird;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_34 = __this->___Save;
		NullCheck(L_34);
		int32_t L_35 = L_34->___boosterFirebird;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_36 = V_1;
		NullCheck(L_36);
		int32_t L_37 = L_36->___boosterLivingLight;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_38 = __this->___Save;
		NullCheck(L_38);
		int32_t L_39 = L_38->___boosterLivingLight;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_40 = V_1;
		NullCheck(L_40);
		int32_t L_41 = L_40->___boosterSeaPearl;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_42 = __this->___Save;
		NullCheck(L_42);
		int32_t L_43 = L_42->___boosterSeaPearl;
		DailyRewardGrant_t3E6F1ABD071C4E274E3257577773D20AC5C5DD69 L_44;
		memset((&L_44), 0, sizeof(L_44));
		DailyRewardGrant__ctor_mD5DD4A96DE9B478E9216D0A0281D2D62C63AE78B((&L_44), ((int32_t)il2cpp_codegen_subtract(L_17, L_19)), ((int32_t)il2cpp_codegen_subtract(L_21, L_23)), ((int32_t)il2cpp_codegen_subtract(L_25, L_27)), ((int32_t)il2cpp_codegen_subtract(L_29, L_31)), ((int32_t)il2cpp_codegen_subtract(L_33, L_35)), ((int32_t)il2cpp_codegen_subtract(L_37, L_39)), ((int32_t)il2cpp_codegen_subtract(L_41, L_43)), NULL);
		*(DailyRewardGrant_t3E6F1ABD071C4E274E3257577773D20AC5C5DD69*)L_15 = L_44;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_45 = V_1;
		__this->___Save = L_45;
		Il2CppCodeGenWriteBarrier((void**)(&__this->___Save), (void*)L_45);
		Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* L_46 = __this->___Changed;
		Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* L_47 = L_46;
		if (L_47)
		{
			G_B9_0 = L_47;
			goto IL_00f7;
		}
		G_B8_0 = L_47;
	}
	{
		goto IL_00fc;
	}

IL_00f7:
	{
		NullCheck(G_B9_0);
		Action_Invoke_m7126A54DACA72B845424072887B5F3A51FC3808E_inline(G_B9_0, NULL);
	}

IL_00fc:
	{
		return (bool)1;
	}
}
