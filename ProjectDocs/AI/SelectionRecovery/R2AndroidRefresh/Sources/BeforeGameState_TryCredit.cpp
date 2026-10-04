IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool GameState_TryCredit_m51CCF6EC7D6B5E28CE92A3A710757EACEF1E0D0D (GameState_t5E590D2FB317152A4011A3CA69E3F311E0A7393A* __this, int32_t ___0_amount, Action_2_t0274C7E415D90647FD97CB90D7D7417FDE17D2BA* ___1_apply, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&JsonUtility_FromJson_TisPlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30_m5FA532D489BA59FC0DD187FA87D56E71F1F63B04_RuntimeMethod_var);
		s_Il2CppMethodInitialized = true;
	}
	PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* V_0 = NULL;
	bool V_1 = false;
	il2cpp::utils::ExceptionSupportStack<RuntimeObject*, 1> __active_exceptions;
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
	}
	try
	{
		Action_2_t0274C7E415D90647FD97CB90D7D7417FDE17D2BA* L_4 = ___1_apply;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_5 = V_0;
		int32_t L_6 = ___0_amount;
		int32_t L_7;
		L_7 = Mathf_Max_m7FA442918DE37E3A00106D1F2E789D65829792B8_inline(0, L_6, NULL);
		NullCheck(L_4);
		Action_2_Invoke_m07A953FC53DC33AA40AC2D6056DD7CB7CAC9E6FE_inline(L_4, L_5, L_7, NULL);
		goto IL_0030;
	}
	catch(Il2CppExceptionWrapper& e)
	{
		if(il2cpp_codegen_class_is_assignable_from (((RuntimeClass*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&OverflowException_t6F6AD8CACE20C37F701C05B373A215C4802FAB0C_il2cpp_TypeInfo_var)), il2cpp_codegen_object_class(e.ex)))
		{
			IL2CPP_PUSH_ACTIVE_EXCEPTION(e.ex);
			goto CATCH_002b;
		}
		throw e;
	}

CATCH_002b:
	{
		OverflowException_t6F6AD8CACE20C37F701C05B373A215C4802FAB0C* L_8 = ((OverflowException_t6F6AD8CACE20C37F701C05B373A215C4802FAB0C*)IL2CPP_GET_ACTIVE_EXCEPTION(OverflowException_t6F6AD8CACE20C37F701C05B373A215C4802FAB0C*));;
		V_1 = (bool)0;
		IL2CPP_POP_ACTIVE_EXCEPTION(Exception_t*);
		goto IL_0038;
	}

IL_0030:
	{
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_9 = V_0;
		bool L_10;
		L_10 = GameState_TryCommitBalance_mD9DA4B9AE5D934AD30A0F070F9C9AE01F165CCD7(__this, L_9, NULL);
		return L_10;
	}

IL_0038:
	{
		bool L_11 = V_1;
		return L_11;
	}
}
