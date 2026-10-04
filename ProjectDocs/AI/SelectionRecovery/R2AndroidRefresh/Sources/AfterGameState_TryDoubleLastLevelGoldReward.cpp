IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool GameState_TryDoubleLastLevelGoldReward_m1F4365DDE5E82F099DB4EB8A97D7AC871E0D23F3 (GameState_t5E590D2FB317152A4011A3CA69E3F311E0A7393A* __this, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&GameState_TryDoubleLastLevelGoldReward_m1F4365DDE5E82F099DB4EB8A97D7AC871E0D23F3_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&JsonUtility_FromJson_TisPlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30_m5FA532D489BA59FC0DD187FA87D56E71F1F63B04_RuntimeMethod_var);
		s_Il2CppMethodInitialized = true;
	}
	PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* V_0 = NULL;
	PlayerSaveWriteResult_tD3521C4F0AD4F171529F5475A2BD7B28C0CB1D9C* V_1 = NULL;
	bool V_2 = false;
	il2cpp::utils::ExceptionSupportStack<RuntimeObject*, 1> __active_exceptions;
	Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* G_B12_0 = NULL;
	Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* G_B11_0 = NULL;
	{
		bool L_0 = __this->___canPersist;
		if (!L_0)
		{
			goto IL_0030;
		}
	}
	{
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_1 = __this->___Save;
		NullCheck(L_1);
		bool L_2 = L_1->___lastRewardEligibleForDouble;
		if (!L_2)
		{
			goto IL_0030;
		}
	}
	{
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_3 = __this->___Save;
		NullCheck(L_3);
		bool L_4 = L_3->___lastRewardDoubled;
		if (L_4)
		{
			goto IL_0030;
		}
	}
	{
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_5 = __this->___Save;
		NullCheck(L_5);
		int32_t L_6 = L_5->___lastRewardGold;
		if ((((int32_t)L_6) > ((int32_t)0)))
		{
			goto IL_0032;
		}
	}

IL_0030:
	{
		return (bool)0;
	}

IL_0032:
	{
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_7 = __this->___Save;
		String_t* L_8;
		L_8 = JsonUtility_ToJson_m28CC6843B9D3723D88AD13EA3829B71FDE7826BA(L_7, NULL);
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_9;
		L_9 = JsonUtility_FromJson_TisPlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30_m5FA532D489BA59FC0DD187FA87D56E71F1F63B04(L_8, JsonUtility_FromJson_TisPlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30_m5FA532D489BA59FC0DD187FA87D56E71F1F63B04_RuntimeMethod_var);
		V_0 = L_9;
	}
	try
	{
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_10 = V_0;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_11 = V_0;
		NullCheck(L_11);
		int32_t L_12 = L_11->___gold;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_13 = V_0;
		NullCheck(L_13);
		int32_t L_14 = L_13->___lastRewardGold;
		if (((int64_t)L_12 + (int64_t)L_14 < (int64_t)kIl2CppInt32Min) || ((int64_t)L_12 + (int64_t)L_14 > (int64_t)kIl2CppInt32Max))
			IL2CPP_RAISE_MANAGED_EXCEPTION(il2cpp_codegen_get_overflow_exception(), GameState_TryDoubleLastLevelGoldReward_m1F4365DDE5E82F099DB4EB8A97D7AC871E0D23F3_RuntimeMethod_var);
		NullCheck(L_10);
		L_10->___gold = ((int32_t)il2cpp_codegen_add(L_12, L_14));
		goto IL_005d;
	}
	catch(Il2CppExceptionWrapper& e)
	{
		if(il2cpp_codegen_class_is_assignable_from (((RuntimeClass*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&OverflowException_t6F6AD8CACE20C37F701C05B373A215C4802FAB0C_il2cpp_TypeInfo_var)), il2cpp_codegen_object_class(e.ex)))
		{
			IL2CPP_PUSH_ACTIVE_EXCEPTION(e.ex);
			goto CATCH_0058;
		}
		throw e;
	}

CATCH_0058:
	{
		OverflowException_t6F6AD8CACE20C37F701C05B373A215C4802FAB0C* L_15 = ((OverflowException_t6F6AD8CACE20C37F701C05B373A215C4802FAB0C*)IL2CPP_GET_ACTIVE_EXCEPTION(OverflowException_t6F6AD8CACE20C37F701C05B373A215C4802FAB0C*));;
		V_2 = (bool)0;
		IL2CPP_POP_ACTIVE_EXCEPTION(Exception_t*);
		goto IL_0098;
	}

IL_005d:
	{
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_16 = V_0;
		NullCheck(L_16);
		L_16->___lastRewardDoubled = (bool)1;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_17 = V_0;
		bool L_18;
		L_18 = SaveService_TrySave_m11A043A57FCBE0607BDAAA6C34B4E969064615B0(L_17, (&V_1), NULL);
		PlayerSaveWriteResult_tD3521C4F0AD4F171529F5475A2BD7B28C0CB1D9C* L_19 = V_1;
		GameState_set_LastWriteResult_mE42274830424D045544D465431B5C582BC50A567_inline(__this, L_19, NULL);
		if (L_18)
		{
			goto IL_007e;
		}
	}
	{
		__this->___canPersist = (bool)0;
		return (bool)0;
	}

IL_007e:
	{
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_20 = V_0;
		__this->___Save = L_20;
		Il2CppCodeGenWriteBarrier((void**)(&__this->___Save), (void*)L_20);
		Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* L_21 = __this->___Changed;
		Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* L_22 = L_21;
		if (L_22)
		{
			G_B12_0 = L_22;
			goto IL_0091;
		}
		G_B11_0 = L_22;
	}
	{
		goto IL_0096;
	}

IL_0091:
	{
		NullCheck(G_B12_0);
		Action_Invoke_m7126A54DACA72B845424072887B5F3A51FC3808E_inline(G_B12_0, NULL);
	}

IL_0096:
	{
		return (bool)1;
	}

IL_0098:
	{
		bool L_23 = V_2;
		return L_23;
	}
}
