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
	bool V_3 = false;
	il2cpp::utils::ExceptionSupportStack<RuntimeObject*, 1> __active_exceptions;
	Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* G_B12_0 = NULL;
	Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* G_B11_0 = NULL;
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
	}
	try
	{
		{
			PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_9 = V_1;
			DateTime_t66193957C73913903DDAD89FEDC46139BCA5802D L_10 = V_0;
			bool L_11;
			L_11 = DailyRewardSchedule_TryApply_mF65B3D7F66337A25BE9E31E6716F93E9BA33D878(L_9, L_10, NULL);
			if (L_11)
			{
				goto IL_0046_1;
			}
		}
		{
			V_3 = (bool)0;
			goto IL_010d;
		}

IL_0046_1:
		{
			goto IL_0050;
		}
	}
	catch(Il2CppExceptionWrapper& e)
	{
		if(il2cpp_codegen_class_is_assignable_from (((RuntimeClass*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&OverflowException_t6F6AD8CACE20C37F701C05B373A215C4802FAB0C_il2cpp_TypeInfo_var)), il2cpp_codegen_object_class(e.ex)))
		{
			IL2CPP_PUSH_ACTIVE_EXCEPTION(e.ex);
			goto CATCH_0048;
		}
		throw e;
	}

CATCH_0048:
	{
		OverflowException_t6F6AD8CACE20C37F701C05B373A215C4802FAB0C* L_12 = ((OverflowException_t6F6AD8CACE20C37F701C05B373A215C4802FAB0C*)IL2CPP_GET_ACTIVE_EXCEPTION(OverflowException_t6F6AD8CACE20C37F701C05B373A215C4802FAB0C*));;
		V_3 = (bool)0;
		IL2CPP_POP_ACTIVE_EXCEPTION(Exception_t*);
		goto IL_010d;
	}

IL_0050:
	{
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_13 = V_1;
		bool L_14;
		L_14 = SaveService_TrySave_m11A043A57FCBE0607BDAAA6C34B4E969064615B0(L_13, (&V_2), NULL);
		PlayerSaveWriteResult_tD3521C4F0AD4F171529F5475A2BD7B28C0CB1D9C* L_15 = V_2;
		GameState_set_LastWriteResult_mE42274830424D045544D465431B5C582BC50A567_inline(__this, L_15, NULL);
		if (L_14)
		{
			goto IL_006a;
		}
	}
	{
		__this->___canPersist = (bool)0;
		return (bool)0;
	}

IL_006a:
	{
		DailyRewardGrant_t3E6F1ABD071C4E274E3257577773D20AC5C5DD69* L_16 = ___0_granted;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_17 = V_1;
		NullCheck(L_17);
		int32_t L_18 = L_17->___gold;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_19 = __this->___Save;
		NullCheck(L_19);
		int32_t L_20 = L_19->___gold;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_21 = V_1;
		NullCheck(L_21);
		int32_t L_22 = L_21->___gems;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_23 = __this->___Save;
		NullCheck(L_23);
		int32_t L_24 = L_23->___gems;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_25 = V_1;
		NullCheck(L_25);
		int32_t L_26 = L_25->___energy;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_27 = __this->___Save;
		NullCheck(L_27);
		int32_t L_28 = L_27->___energy;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_29 = V_1;
		NullCheck(L_29);
		int32_t L_30 = L_29->___boosterMace;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_31 = __this->___Save;
		NullCheck(L_31);
		int32_t L_32 = L_31->___boosterMace;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_33 = V_1;
		NullCheck(L_33);
		int32_t L_34 = L_33->___boosterFirebird;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_35 = __this->___Save;
		NullCheck(L_35);
		int32_t L_36 = L_35->___boosterFirebird;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_37 = V_1;
		NullCheck(L_37);
		int32_t L_38 = L_37->___boosterLivingLight;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_39 = __this->___Save;
		NullCheck(L_39);
		int32_t L_40 = L_39->___boosterLivingLight;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_41 = V_1;
		NullCheck(L_41);
		int32_t L_42 = L_41->___boosterSeaPearl;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_43 = __this->___Save;
		NullCheck(L_43);
		int32_t L_44 = L_43->___boosterSeaPearl;
		DailyRewardGrant_t3E6F1ABD071C4E274E3257577773D20AC5C5DD69 L_45;
		memset((&L_45), 0, sizeof(L_45));
		DailyRewardGrant__ctor_mD5DD4A96DE9B478E9216D0A0281D2D62C63AE78B((&L_45), ((int32_t)il2cpp_codegen_subtract(L_18, L_20)), ((int32_t)il2cpp_codegen_subtract(L_22, L_24)), ((int32_t)il2cpp_codegen_subtract(L_26, L_28)), ((int32_t)il2cpp_codegen_subtract(L_30, L_32)), ((int32_t)il2cpp_codegen_subtract(L_34, L_36)), ((int32_t)il2cpp_codegen_subtract(L_38, L_40)), ((int32_t)il2cpp_codegen_subtract(L_42, L_44)), NULL);
		*(DailyRewardGrant_t3E6F1ABD071C4E274E3257577773D20AC5C5DD69*)L_16 = L_45;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_46 = V_1;
		__this->___Save = L_46;
		Il2CppCodeGenWriteBarrier((void**)(&__this->___Save), (void*)L_46);
		Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* L_47 = __this->___Changed;
		Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* L_48 = L_47;
		if (L_48)
		{
			G_B12_0 = L_48;
			goto IL_0106;
		}
		G_B11_0 = L_48;
	}
	{
		goto IL_010b;
	}

IL_0106:
	{
		NullCheck(G_B12_0);
		Action_Invoke_m7126A54DACA72B845424072887B5F3A51FC3808E_inline(G_B12_0, NULL);
	}

IL_010b:
	{
		return (bool)1;
	}

IL_010d:
	{
		bool L_49 = V_3;
		return L_49;
	}
}
