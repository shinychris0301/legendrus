IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool GameState_TrySettleBoosterCommand_mAB9DD0D7B65D3E9D40665FCA710B1E678166C27E (GameState_t5E590D2FB317152A4011A3CA69E3F311E0A7393A* __this, String_t* ___0_commandId, bool ___1_applied, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&JsonUtility_FromJson_TisPlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30_m5FA532D489BA59FC0DD187FA87D56E71F1F63B04_RuntimeMethod_var);
		s_Il2CppMethodInitialized = true;
	}
	BoosterCommandReceipt_tC1406EAC660F3A183904BE357F158495A694DE6C* V_0 = NULL;
	int32_t V_1 = 0;
	PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* V_2 = NULL;
	bool V_3 = false;
	il2cpp::utils::ExceptionSupportStack<RuntimeObject*, 1> __active_exceptions;
	int32_t G_B8_0 = 0;
	{
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_0 = __this->___Save;
		NullCheck(L_0);
		BoosterCommandReceipt_tC1406EAC660F3A183904BE357F158495A694DE6C* L_1 = L_0->___boosterCommand;
		V_0 = L_1;
		BoosterCommandReceipt_tC1406EAC660F3A183904BE357F158495A694DE6C* L_2 = V_0;
		if (!L_2)
		{
			goto IL_002d;
		}
	}
	{
		BoosterCommandReceipt_tC1406EAC660F3A183904BE357F158495A694DE6C* L_3 = V_0;
		NullCheck(L_3);
		bool L_4;
		L_4 = BoosterCommandReceipt_get_IsValid_m22BFE8F106E27579F6D394C1882C220EBDBDB118(L_3, NULL);
		if (!L_4)
		{
			goto IL_002d;
		}
	}
	{
		String_t* L_5 = ___0_commandId;
		bool L_6;
		L_6 = String_IsNullOrEmpty_mEA9E3FB005AC28FE02E69FCF95A7B8456192B478(L_5, NULL);
		if (L_6)
		{
			goto IL_002d;
		}
	}
	{
		BoosterCommandReceipt_tC1406EAC660F3A183904BE357F158495A694DE6C* L_7 = V_0;
		NullCheck(L_7);
		String_t* L_8 = L_7->___commandId;
		String_t* L_9 = ___0_commandId;
		bool L_10;
		L_10 = String_op_Inequality_m8C940F3CFC42866709D7CA931B3D77B4BE94BCB6(L_8, L_9, NULL);
		if (!L_10)
		{
			goto IL_002f;
		}
	}

IL_002d:
	{
		return (bool)0;
	}

IL_002f:
	{
		bool L_11 = ___1_applied;
		if (L_11)
		{
			goto IL_0035;
		}
	}
	{
		G_B8_0 = 3;
		goto IL_0036;
	}

IL_0035:
	{
		G_B8_0 = 2;
	}

IL_0036:
	{
		V_1 = G_B8_0;
		BoosterCommandReceipt_tC1406EAC660F3A183904BE357F158495A694DE6C* L_12 = V_0;
		NullCheck(L_12);
		int32_t L_13 = L_12->___state;
		int32_t L_14 = V_1;
		if ((!(((uint32_t)L_13) == ((uint32_t)L_14))))
		{
			goto IL_0042;
		}
	}
	{
		return (bool)1;
	}

IL_0042:
	{
		bool L_15 = __this->___canPersist;
		if (!L_15)
		{
			goto IL_0053;
		}
	}
	{
		BoosterCommandReceipt_tC1406EAC660F3A183904BE357F158495A694DE6C* L_16 = V_0;
		NullCheck(L_16);
		int32_t L_17 = L_16->___state;
		if ((((int32_t)L_17) == ((int32_t)1)))
		{
			goto IL_0055;
		}
	}

IL_0053:
	{
		return (bool)0;
	}

IL_0055:
	{
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_18 = __this->___Save;
		String_t* L_19;
		L_19 = JsonUtility_ToJson_m28CC6843B9D3723D88AD13EA3829B71FDE7826BA(L_18, NULL);
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_20;
		L_20 = JsonUtility_FromJson_TisPlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30_m5FA532D489BA59FC0DD187FA87D56E71F1F63B04(L_19, JsonUtility_FromJson_TisPlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30_m5FA532D489BA59FC0DD187FA87D56E71F1F63B04_RuntimeMethod_var);
		V_2 = L_20;
	}
	try
	{
		{
			bool L_21 = ___1_applied;
			if (L_21)
			{
				goto IL_0076_1;
			}
		}
		{
			PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_22 = V_2;
			BoosterCommandReceipt_tC1406EAC660F3A183904BE357F158495A694DE6C* L_23 = V_0;
			NullCheck(L_23);
			int32_t L_24 = L_23->___type;
			CampaignBoosterTransactions_Add_m2E76DA0F9FC77EA8CAE42488806D10B97A720D9D(L_22, L_24, 1, NULL);
		}

IL_0076_1:
		{
			goto IL_007d;
		}
	}
	catch(Il2CppExceptionWrapper& e)
	{
		if(il2cpp_codegen_class_is_assignable_from (((RuntimeClass*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&OverflowException_t6F6AD8CACE20C37F701C05B373A215C4802FAB0C_il2cpp_TypeInfo_var)), il2cpp_codegen_object_class(e.ex)))
		{
			IL2CPP_PUSH_ACTIVE_EXCEPTION(e.ex);
			goto CATCH_0078;
		}
		throw e;
	}

CATCH_0078:
	{
		OverflowException_t6F6AD8CACE20C37F701C05B373A215C4802FAB0C* L_25 = ((OverflowException_t6F6AD8CACE20C37F701C05B373A215C4802FAB0C*)IL2CPP_GET_ACTIVE_EXCEPTION(OverflowException_t6F6AD8CACE20C37F701C05B373A215C4802FAB0C*));;
		V_3 = (bool)0;
		IL2CPP_POP_ACTIVE_EXCEPTION(Exception_t*);
		goto IL_0091;
	}

IL_007d:
	{
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_26 = V_2;
		NullCheck(L_26);
		BoosterCommandReceipt_tC1406EAC660F3A183904BE357F158495A694DE6C* L_27 = L_26->___boosterCommand;
		int32_t L_28 = V_1;
		NullCheck(L_27);
		L_27->___state = L_28;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_29 = V_2;
		bool L_30;
		L_30 = GameState_TryCommitBalance_mD9DA4B9AE5D934AD30A0F070F9C9AE01F165CCD7(__this, L_29, NULL);
		return L_30;
	}

IL_0091:
	{
		bool L_31 = V_3;
		return L_31;
	}
}
