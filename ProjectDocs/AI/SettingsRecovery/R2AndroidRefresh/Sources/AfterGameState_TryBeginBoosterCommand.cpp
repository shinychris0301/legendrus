IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool GameState_TryBeginBoosterCommand_mBB45448CFDD6FDB85CA1743E013935412BE11F83 (GameState_t5E590D2FB317152A4011A3CA69E3F311E0A7393A* __this, int32_t ___0_type, String_t** ___1_commandId, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&BoosterCommandReceipt_tC1406EAC660F3A183904BE357F158495A694DE6C_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&JsonUtility_FromJson_TisPlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30_m5FA532D489BA59FC0DD187FA87D56E71F1F63B04_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&_stringLiteralE42E8BB820D4F7550A0F04619F4E15FDC56943B9);
		s_Il2CppMethodInitialized = true;
	}
	PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* V_0 = NULL;
	Guid_t V_1;
	memset((&V_1), 0, sizeof(V_1));
	{
		String_t** L_0 = ___1_commandId;
		il2cpp_codegen_stind<RuntimeObject*>((RuntimeObject**)L_0, (RuntimeObject*)NULL);
		Il2CppCodeGenWriteBarrier((void**)(RuntimeObject**)L_0, (void*)(RuntimeObject*)NULL);
		bool L_1 = __this->___canPersist;
		if (!L_1)
		{
			goto IL_0025;
		}
	}
	{
		bool L_2;
		L_2 = GameState_get_HasPendingBoosterCommand_m3E1BA9F545A38895AD411E75F5DE3C9F67C76C15(__this, NULL);
		if (L_2)
		{
			goto IL_0025;
		}
	}
	{
		int32_t L_3 = ___0_type;
		bool L_4;
		L_4 = CampaignBoosterTransactions_Known_mB4D91A0E09F3B6EC152EA6591E4BAA3905B6FA89(L_3, NULL);
		if (!L_4)
		{
			goto IL_0025;
		}
	}
	{
		int32_t L_5 = ___0_type;
		int32_t L_6;
		L_6 = GameState_BoosterCount_mCE82517DEC995B50BD88A948B1C2F7FF82EAD191(__this, L_5, NULL);
		if ((((int32_t)L_6) >= ((int32_t)1)))
		{
			goto IL_0027;
		}
	}

IL_0025:
	{
		return (bool)0;
	}

IL_0027:
	{
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_7 = __this->___Save;
		String_t* L_8;
		L_8 = JsonUtility_ToJson_m28CC6843B9D3723D88AD13EA3829B71FDE7826BA(L_7, NULL);
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_9;
		L_9 = JsonUtility_FromJson_TisPlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30_m5FA532D489BA59FC0DD187FA87D56E71F1F63B04(L_8, JsonUtility_FromJson_TisPlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30_m5FA532D489BA59FC0DD187FA87D56E71F1F63B04_RuntimeMethod_var);
		V_0 = L_9;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_10 = V_0;
		int32_t L_11 = ___0_type;
		CampaignBoosterTransactions_Add_m2E76DA0F9FC77EA8CAE42488806D10B97A720D9D(L_10, L_11, (-1), NULL);
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_12 = V_0;
		BoosterCommandReceipt_tC1406EAC660F3A183904BE357F158495A694DE6C* L_13 = (BoosterCommandReceipt_tC1406EAC660F3A183904BE357F158495A694DE6C*)il2cpp_codegen_object_new(BoosterCommandReceipt_tC1406EAC660F3A183904BE357F158495A694DE6C_il2cpp_TypeInfo_var);
		BoosterCommandReceipt__ctor_m1B7EAFA77CA1391AF64A86AE7A5BB9075FE11973(L_13, NULL);
		BoosterCommandReceipt_tC1406EAC660F3A183904BE357F158495A694DE6C* L_14 = L_13;
		Guid_t L_15;
		L_15 = Guid_NewGuid_m1F4894E8DC089811D6252148AD5858E58D43A7BD(NULL);
		V_1 = L_15;
		String_t* L_16;
		L_16 = Guid_ToString_mDAA91A4A993E3A7AD8339665E3F0CC35FE00E833((&V_1), _stringLiteralE42E8BB820D4F7550A0F04619F4E15FDC56943B9, NULL);
		NullCheck(L_14);
		L_14->___commandId = L_16;
		Il2CppCodeGenWriteBarrier((void**)(&L_14->___commandId), (void*)L_16);
		BoosterCommandReceipt_tC1406EAC660F3A183904BE357F158495A694DE6C* L_17 = L_14;
		int32_t L_18 = ___0_type;
		NullCheck(L_17);
		L_17->___type = L_18;
		BoosterCommandReceipt_tC1406EAC660F3A183904BE357F158495A694DE6C* L_19 = L_17;
		NullCheck(L_19);
		L_19->___state = 1;
		NullCheck(L_12);
		L_12->___boosterCommand = L_19;
		Il2CppCodeGenWriteBarrier((void**)(&L_12->___boosterCommand), (void*)L_19);
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_20 = V_0;
		bool L_21;
		L_21 = GameState_TryCommitBalance_mD9DA4B9AE5D934AD30A0F070F9C9AE01F165CCD7(__this, L_20, NULL);
		if (L_21)
		{
			goto IL_007c;
		}
	}
	{
		return (bool)0;
	}

IL_007c:
	{
		String_t** L_22 = ___1_commandId;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_23 = V_0;
		NullCheck(L_23);
		BoosterCommandReceipt_tC1406EAC660F3A183904BE357F158495A694DE6C* L_24 = L_23->___boosterCommand;
		NullCheck(L_24);
		String_t* L_25 = L_24->___commandId;
		il2cpp_codegen_stind<RuntimeObject*>((RuntimeObject**)L_22, (RuntimeObject*)L_25);
		Il2CppCodeGenWriteBarrier((void**)(RuntimeObject**)L_22, (void*)(RuntimeObject*)L_25);
		return (bool)1;
	}
}
