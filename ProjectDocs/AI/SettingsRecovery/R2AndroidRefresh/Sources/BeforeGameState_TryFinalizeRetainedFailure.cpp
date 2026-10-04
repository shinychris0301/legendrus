IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool GameState_TryFinalizeRetainedFailure_m52557FDA715F0077DDFF25D05316B96D0FC98B9B (GameState_t5E590D2FB317152A4011A3CA69E3F311E0A7393A* __this, String_t* ___0_attemptId, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Enumerable_Single_TisLevelProgressData_tF4CF8BE6A46189E730098D0C3DEE22CAA57E0C9E_mC574EAC8A65109C096F8B5396D4C5B06E8C6BD30_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Func_2_tDBE5F93C653E4571C7B739C2AFC1D6D08AAC90D5_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&GameState_TryFinalizeRetainedFailure_m52557FDA715F0077DDFF25D05316B96D0FC98B9B_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&JsonUtility_FromJson_TisPlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30_m5FA532D489BA59FC0DD187FA87D56E71F1F63B04_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Math_tEB65DE7CA8B083C412C969C92981C030865486CE_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&U3CU3Ec__DisplayClass13_0_U3CTryFinalizeRetainedFailureU3Eb__0_mF17B5C4A7DEB31BA568A0F72396C63EE781F9617_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&U3CU3Ec__DisplayClass13_0_tA63187BE5D9361F2DA220EA66778C89525A0E5E3_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	U3CU3Ec__DisplayClass13_0_tA63187BE5D9361F2DA220EA66778C89525A0E5E3* V_0 = NULL;
	PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* V_1 = NULL;
	LevelProgressData_tF4CF8BE6A46189E730098D0C3DEE22CAA57E0C9E* V_2 = NULL;
	bool V_3 = false;
	il2cpp::utils::ExceptionSupportStack<RuntimeObject*, 1> __active_exceptions;
	BalanceTelemetryReviewService_tCEB958C4014C08412D40F113D01EC97650E86BE8* G_B12_0 = NULL;
	BalanceTelemetryReviewService_tCEB958C4014C08412D40F113D01EC97650E86BE8* G_B11_0 = NULL;
	{
		U3CU3Ec__DisplayClass13_0_tA63187BE5D9361F2DA220EA66778C89525A0E5E3* L_0 = (U3CU3Ec__DisplayClass13_0_tA63187BE5D9361F2DA220EA66778C89525A0E5E3*)il2cpp_codegen_object_new(U3CU3Ec__DisplayClass13_0_tA63187BE5D9361F2DA220EA66778C89525A0E5E3_il2cpp_TypeInfo_var);
		U3CU3Ec__DisplayClass13_0__ctor_m1A60D158C0B2DC3E45D2CC323DE079AAAC5F4DBC(L_0, NULL);
		V_0 = L_0;
		U3CU3Ec__DisplayClass13_0_tA63187BE5D9361F2DA220EA66778C89525A0E5E3* L_1 = V_0;
		NullCheck(L_1);
		RetainedRoundData_t3EA1BA932DFF2408098F1FB87EC5A5A24130D7A1** L_2 = (RetainedRoundData_t3EA1BA932DFF2408098F1FB87EC5A5A24130D7A1**)(&L_1->___data);
		bool L_3;
		L_3 = GameState_TryGetRetainedRound_m2295FF12D42D33C9E7E1FF1D8B33B8E3859793A5(__this, L_2, NULL);
		if (!L_3)
		{
			goto IL_0027;
		}
	}
	{
		U3CU3Ec__DisplayClass13_0_tA63187BE5D9361F2DA220EA66778C89525A0E5E3* L_4 = V_0;
		NullCheck(L_4);
		RetainedRoundData_t3EA1BA932DFF2408098F1FB87EC5A5A24130D7A1* L_5 = L_4->___data;
		NullCheck(L_5);
		String_t* L_6 = L_5->___attemptId;
		String_t* L_7 = ___0_attemptId;
		bool L_8;
		L_8 = String_op_Inequality_m8C940F3CFC42866709D7CA931B3D77B4BE94BCB6(L_6, L_7, NULL);
		if (!L_8)
		{
			goto IL_0029;
		}
	}

IL_0027:
	{
		return (bool)0;
	}

IL_0029:
	{
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_9 = __this->___Save;
		String_t* L_10;
		L_10 = JsonUtility_ToJson_m28CC6843B9D3723D88AD13EA3829B71FDE7826BA(L_9, NULL);
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_11;
		L_11 = JsonUtility_FromJson_TisPlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30_m5FA532D489BA59FC0DD187FA87D56E71F1F63B04(L_10, JsonUtility_FromJson_TisPlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30_m5FA532D489BA59FC0DD187FA87D56E71F1F63B04_RuntimeMethod_var);
		V_1 = L_11;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_12 = V_1;
		U3CU3Ec__DisplayClass13_0_tA63187BE5D9361F2DA220EA66778C89525A0E5E3* L_13 = V_0;
		NullCheck(L_13);
		RetainedRoundData_t3EA1BA932DFF2408098F1FB87EC5A5A24130D7A1* L_14 = L_13->___data;
		bool L_15;
		L_15 = GameState_CancelUnusedContinuation_mBA4328B6049F1A508F9B3DBC222E06FB70710CA6(__this, L_12, L_14, NULL);
		if (L_15)
		{
			goto IL_004b;
		}
	}
	{
		return (bool)0;
	}

IL_004b:
	{
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_16 = V_1;
		NullCheck(L_16);
		List_1_tE17596A66614F9E1802150B437325DB3004F8681* L_17 = L_16->___levelProgress;
		U3CU3Ec__DisplayClass13_0_tA63187BE5D9361F2DA220EA66778C89525A0E5E3* L_18 = V_0;
		Func_2_tDBE5F93C653E4571C7B739C2AFC1D6D08AAC90D5* L_19 = (Func_2_tDBE5F93C653E4571C7B739C2AFC1D6D08AAC90D5*)il2cpp_codegen_object_new(Func_2_tDBE5F93C653E4571C7B739C2AFC1D6D08AAC90D5_il2cpp_TypeInfo_var);
		Func_2__ctor_mEDFF83A6E1ACCE9AACE0971C069EBAB4262B11A8(L_19, L_18, (intptr_t)((void*)U3CU3Ec__DisplayClass13_0_U3CTryFinalizeRetainedFailureU3Eb__0_mF17B5C4A7DEB31BA568A0F72396C63EE781F9617_RuntimeMethod_var), NULL);
		LevelProgressData_tF4CF8BE6A46189E730098D0C3DEE22CAA57E0C9E* L_20;
		L_20 = Enumerable_Single_TisLevelProgressData_tF4CF8BE6A46189E730098D0C3DEE22CAA57E0C9E_mC574EAC8A65109C096F8B5396D4C5B06E8C6BD30(L_17, L_19, Enumerable_Single_TisLevelProgressData_tF4CF8BE6A46189E730098D0C3DEE22CAA57E0C9E_mC574EAC8A65109C096F8B5396D4C5B06E8C6BD30_RuntimeMethod_var);
		V_2 = L_20;
	}
	try
	{
		LevelProgressData_tF4CF8BE6A46189E730098D0C3DEE22CAA57E0C9E* L_21 = V_2;
		LevelProgressData_tF4CF8BE6A46189E730098D0C3DEE22CAA57E0C9E* L_22 = V_2;
		NullCheck(L_22);
		int32_t L_23 = L_22->___failures;
		if (((int64_t)L_23 + (int64_t)1 < (int64_t)kIl2CppInt32Min) || ((int64_t)L_23 + (int64_t)1 > (int64_t)kIl2CppInt32Max))
			IL2CPP_RAISE_MANAGED_EXCEPTION(il2cpp_codegen_get_overflow_exception(), GameState_TryFinalizeRetainedFailure_m52557FDA715F0077DDFF25D05316B96D0FC98B9B_RuntimeMethod_var);
		NullCheck(L_21);
		L_21->___failures = ((int32_t)il2cpp_codegen_add(L_23, 1));
		goto IL_007b;
	}
	catch(Il2CppExceptionWrapper& e)
	{
		if(il2cpp_codegen_class_is_assignable_from (((RuntimeClass*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&OverflowException_t6F6AD8CACE20C37F701C05B373A215C4802FAB0C_il2cpp_TypeInfo_var)), il2cpp_codegen_object_class(e.ex)))
		{
			IL2CPP_PUSH_ACTIVE_EXCEPTION(e.ex);
			goto CATCH_0073;
		}
		throw e;
	}

CATCH_0073:
	{
		OverflowException_t6F6AD8CACE20C37F701C05B373A215C4802FAB0C* L_24 = ((OverflowException_t6F6AD8CACE20C37F701C05B373A215C4802FAB0C*)IL2CPP_GET_ACTIVE_EXCEPTION(OverflowException_t6F6AD8CACE20C37F701C05B373A215C4802FAB0C*));;
		V_3 = (bool)0;
		IL2CPP_POP_ACTIVE_EXCEPTION(Exception_t*);
		goto IL_0130;
	}

IL_007b:
	{
		LevelProgressData_tF4CF8BE6A46189E730098D0C3DEE22CAA57E0C9E* L_25 = V_2;
		LevelProgressData_tF4CF8BE6A46189E730098D0C3DEE22CAA57E0C9E* L_26 = V_2;
		NullCheck(L_26);
		int32_t L_27 = L_26->___bestScore;
		U3CU3Ec__DisplayClass13_0_tA63187BE5D9361F2DA220EA66778C89525A0E5E3* L_28 = V_0;
		NullCheck(L_28);
		RetainedRoundData_t3EA1BA932DFF2408098F1FB87EC5A5A24130D7A1* L_29 = L_28->___data;
		NullCheck(L_29);
		int32_t L_30 = L_29->___score;
		il2cpp_codegen_runtime_class_init_inline(Math_tEB65DE7CA8B083C412C969C92981C030865486CE_il2cpp_TypeInfo_var);
		int32_t L_31;
		L_31 = Math_Max_m530EBA549AFD98CFC2BD29FE86C6376E67DF11CF(L_27, L_30, NULL);
		NullCheck(L_25);
		L_25->___bestScore = L_31;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_32 = V_1;
		NullCheck(L_32);
		L_32->___retainedRound = (RetainedRoundData_t3EA1BA932DFF2408098F1FB87EC5A5A24130D7A1*)NULL;
		Il2CppCodeGenWriteBarrier((void**)(&L_32->___retainedRound), (void*)(RetainedRoundData_t3EA1BA932DFF2408098F1FB87EC5A5A24130D7A1*)NULL);
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_33 = V_1;
		bool L_34;
		L_34 = GameState_CommitRetained_m193E5EA8E28AA28A9D6FCA892B9720F47849AFE2(__this, L_33, NULL);
		if (L_34)
		{
			goto IL_00a9;
		}
	}
	{
		return (bool)0;
	}

IL_00a9:
	{
		__this->___unsavedRetainedRound = (RetainedRoundData_t3EA1BA932DFF2408098F1FB87EC5A5A24130D7A1*)NULL;
		Il2CppCodeGenWriteBarrier((void**)(&__this->___unsavedRetainedRound), (void*)(RetainedRoundData_t3EA1BA932DFF2408098F1FB87EC5A5A24130D7A1*)NULL);
		U3CU3Ec__DisplayClass13_0_tA63187BE5D9361F2DA220EA66778C89525A0E5E3* L_35 = V_0;
		NullCheck(L_35);
		RetainedRoundData_t3EA1BA932DFF2408098F1FB87EC5A5A24130D7A1* L_36 = L_35->___data;
		NullCheck(L_36);
		int32_t L_37 = L_36->___chapter;
		U3CU3Ec__DisplayClass13_0_tA63187BE5D9361F2DA220EA66778C89525A0E5E3* L_38 = V_0;
		NullCheck(L_38);
		RetainedRoundData_t3EA1BA932DFF2408098F1FB87EC5A5A24130D7A1* L_39 = L_38->___data;
		NullCheck(L_39);
		int32_t L_40 = L_39->___level;
		LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* L_41;
		L_41 = ChapterLevelCatalog_Find_mB0A3275A64F35FB511F90A6049719C9B1971FDC6(L_37, L_40, NULL);
		NullCheck(L_41);
		String_t* L_42 = L_41->___id;
		U3CU3Ec__DisplayClass13_0_tA63187BE5D9361F2DA220EA66778C89525A0E5E3* L_43 = V_0;
		NullCheck(L_43);
		RetainedRoundData_t3EA1BA932DFF2408098F1FB87EC5A5A24130D7A1* L_44 = L_43->___data;
		NullCheck(L_44);
		int32_t L_45 = L_44->___attempt;
		U3CU3Ec__DisplayClass13_0_tA63187BE5D9361F2DA220EA66778C89525A0E5E3* L_46 = V_0;
		NullCheck(L_46);
		RetainedRoundData_t3EA1BA932DFF2408098F1FB87EC5A5A24130D7A1* L_47 = L_46->___data;
		NullCheck(L_47);
		int32_t L_48 = L_47->___score;
		AnalyticsRecordResult_tEBE87AEB03C6D60620CF177295FC8B80E2EF350A* L_49;
		L_49 = PrivacyAwareAnalytics_TryLevelFail_m5A52AB25563F7417D09C72E18B62912667005E74(L_42, L_45, L_48, NULL);
		BalanceTelemetryReviewService_tCEB958C4014C08412D40F113D01EC97650E86BE8* L_50;
		L_50 = BalanceTelemetryReviewService_get_Instance_m8BE222FB4502374A70B342B1003DFCB437801EA4_inline(NULL);
		BalanceTelemetryReviewService_tCEB958C4014C08412D40F113D01EC97650E86BE8* L_51 = L_50;
		if (L_51)
		{
			G_B12_0 = L_51;
			goto IL_00f7;
		}
		G_B11_0 = L_51;
	}
	{
		goto IL_012e;
	}

IL_00f7:
	{
		U3CU3Ec__DisplayClass13_0_tA63187BE5D9361F2DA220EA66778C89525A0E5E3* L_52 = V_0;
		NullCheck(L_52);
		RetainedRoundData_t3EA1BA932DFF2408098F1FB87EC5A5A24130D7A1* L_53 = L_52->___data;
		NullCheck(L_53);
		int32_t L_54 = L_53->___chapter;
		U3CU3Ec__DisplayClass13_0_tA63187BE5D9361F2DA220EA66778C89525A0E5E3* L_55 = V_0;
		NullCheck(L_55);
		RetainedRoundData_t3EA1BA932DFF2408098F1FB87EC5A5A24130D7A1* L_56 = L_55->___data;
		NullCheck(L_56);
		int32_t L_57 = L_56->___level;
		LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* L_58;
		L_58 = ChapterLevelCatalog_Find_mB0A3275A64F35FB511F90A6049719C9B1971FDC6(L_54, L_57, NULL);
		U3CU3Ec__DisplayClass13_0_tA63187BE5D9361F2DA220EA66778C89525A0E5E3* L_59 = V_0;
		NullCheck(L_59);
		RetainedRoundData_t3EA1BA932DFF2408098F1FB87EC5A5A24130D7A1* L_60 = L_59->___data;
		NullCheck(L_60);
		int32_t L_61 = L_60->___attempt;
		U3CU3Ec__DisplayClass13_0_tA63187BE5D9361F2DA220EA66778C89525A0E5E3* L_62 = V_0;
		NullCheck(L_62);
		RetainedRoundData_t3EA1BA932DFF2408098F1FB87EC5A5A24130D7A1* L_63 = L_62->___data;
		NullCheck(L_63);
		int32_t L_64 = L_63->___score;
		NullCheck(G_B12_0);
		bool L_65;
		L_65 = BalanceTelemetryReviewService_TryRecordLevelFail_m6A9517A4678F2BBCFDACE1A75E2D78BA42B9C03B(G_B12_0, L_58, L_61, L_64, NULL);
	}

IL_012e:
	{
		return (bool)1;
	}

IL_0130:
	{
		bool L_66 = V_3;
		return L_66;
	}
}
