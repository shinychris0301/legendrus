IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool GameState_TryCompleteLevel_m1A62C5C8B0955D4911B88F1A105CAD96B09AEBEC (GameState_t5E590D2FB317152A4011A3CA69E3F311E0A7393A* __this, LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* ___0_levelData, int32_t ___1_score, LevelCompletionResult_t4C786293AEB16558E39DC3DCEEE1542A5487F03E** ___2_completion, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&GameState_TryCompleteLevel_m1A62C5C8B0955D4911B88F1A105CAD96B09AEBEC_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&JsonUtility_FromJson_TisPlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30_m5FA532D489BA59FC0DD187FA87D56E71F1F63B04_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&LevelCompletionResult_t4C786293AEB16558E39DC3DCEEE1542A5487F03E_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&LevelProgressData_tF4CF8BE6A46189E730098D0C3DEE22CAA57E0C9E_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&LevelRewardData_tB1785C653344D16E0780E094B3936973F9DB0B85_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&List_1_Add_mE786EABF7EB1669974F9EA4BD9F5E5377F09396A_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&List_1_Find_m71D79F9D5466D30CB1CBED42C60069703317EF3D_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&List_1__ctor_m2011DE22B9D007A29C2E04C6E21189D5168CBEBC_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&List_1_tE17596A66614F9E1802150B437325DB3004F8681_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Predicate_1_tB25A8FDB6257D222F05FE4E1071EADB7EEB0C814_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&U3CU3Ec__DisplayClass74_0_U3CTryCompleteLevelU3Eb__0_mF93C07A27D5D2B3E4CC7EA0137407D4C320A6B2C_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&U3CU3Ec__DisplayClass74_0_tB1C6F79892902A4EEE2282BCA5ECF650D4F7955C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	U3CU3Ec__DisplayClass74_0_tB1C6F79892902A4EEE2282BCA5ECF650D4F7955C* V_0 = NULL;
	PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* V_1 = NULL;
	LevelProgressData_tF4CF8BE6A46189E730098D0C3DEE22CAA57E0C9E* V_2 = NULL;
	int32_t V_3 = 0;
	LevelRewardData_tB1785C653344D16E0780E094B3936973F9DB0B85* V_4 = NULL;
	bool V_5 = false;
	int32_t V_6 = 0;
	int32_t V_7 = 0;
	LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* V_8 = NULL;
	LevelCompletionResult_t4C786293AEB16558E39DC3DCEEE1542A5487F03E* V_9 = NULL;
	bool V_10 = false;
	PlayerSaveWriteResult_tD3521C4F0AD4F171529F5475A2BD7B28C0CB1D9C* V_11 = NULL;
	bool V_12 = false;
	LevelCompletionResult_t4C786293AEB16558E39DC3DCEEE1542A5487F03E* V_13 = NULL;
	il2cpp::utils::ExceptionSupportStack<RuntimeObject*, 1> __active_exceptions;
	LevelRewardData_tB1785C653344D16E0780E094B3936973F9DB0B85* G_B17_0 = NULL;
	LevelRewardData_tB1785C653344D16E0780E094B3936973F9DB0B85* G_B16_0 = NULL;
	int32_t G_B20_0 = 0;
	int32_t G_B23_0 = 0;
	PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* G_B30_0 = NULL;
	PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* G_B29_0 = NULL;
	int32_t G_B31_0 = 0;
	PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* G_B31_1 = NULL;
	Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* G_B39_0 = NULL;
	Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* G_B38_0 = NULL;
	{
		U3CU3Ec__DisplayClass74_0_tB1C6F79892902A4EEE2282BCA5ECF650D4F7955C* L_0 = (U3CU3Ec__DisplayClass74_0_tB1C6F79892902A4EEE2282BCA5ECF650D4F7955C*)il2cpp_codegen_object_new(U3CU3Ec__DisplayClass74_0_tB1C6F79892902A4EEE2282BCA5ECF650D4F7955C_il2cpp_TypeInfo_var);
		U3CU3Ec__DisplayClass74_0__ctor_mC2D4274D389E20B1680D02DE97E2B48BCE495DAE(L_0, NULL);
		V_0 = L_0;
		U3CU3Ec__DisplayClass74_0_tB1C6F79892902A4EEE2282BCA5ECF650D4F7955C* L_1 = V_0;
		LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* L_2 = ___0_levelData;
		NullCheck(L_1);
		L_1->___levelData = L_2;
		Il2CppCodeGenWriteBarrier((void**)(&L_1->___levelData), (void*)L_2);
		LevelCompletionResult_t4C786293AEB16558E39DC3DCEEE1542A5487F03E** L_3 = ___2_completion;
		il2cpp_codegen_stind<RuntimeObject*>((RuntimeObject**)L_3, (RuntimeObject*)NULL);
		Il2CppCodeGenWriteBarrier((void**)(RuntimeObject**)L_3, (void*)(RuntimeObject*)NULL);
		bool L_4;
		L_4 = GameState_ContinuationBlocksNewAttempt_m908136F30BB6F308311486AF5F487FAD53014484(__this, NULL);
		if (!L_4)
		{
			goto IL_0023;
		}
	}
	{
		InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB* L_5 = (InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB*)il2cpp_codegen_object_new(((RuntimeClass*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB_il2cpp_TypeInfo_var)));
		InvalidOperationException__ctor_mE4CB6F4712AB6D99A2358FBAE2E052B3EE976162(L_5, ((String_t*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&_stringLiteralB427EA8D0025E3A9E1FA52E07B616EA69EBB6A5C)), NULL);
		IL2CPP_RAISE_MANAGED_EXCEPTION(L_5, ((RuntimeMethod*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&GameState_TryCompleteLevel_m1A62C5C8B0955D4911B88F1A105CAD96B09AEBEC_RuntimeMethod_var)));
	}

IL_0023:
	{
		U3CU3Ec__DisplayClass74_0_tB1C6F79892902A4EEE2282BCA5ECF650D4F7955C* L_6 = V_0;
		NullCheck(L_6);
		LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* L_7 = L_6->___levelData;
		if (L_7)
		{
			goto IL_0036;
		}
	}
	{
		ArgumentNullException_t327031E412FAB2351B0022DD5DAD47E67E597129* L_8 = (ArgumentNullException_t327031E412FAB2351B0022DD5DAD47E67E597129*)il2cpp_codegen_object_new(((RuntimeClass*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&ArgumentNullException_t327031E412FAB2351B0022DD5DAD47E67E597129_il2cpp_TypeInfo_var)));
		ArgumentNullException__ctor_m444AE141157E333844FC1A9500224C2F9FD24F4B(L_8, ((String_t*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&_stringLiteral93547B0D0ACC775E6401080B6E728559614917FB)), NULL);
		IL2CPP_RAISE_MANAGED_EXCEPTION(L_8, ((RuntimeMethod*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&GameState_TryCompleteLevel_m1A62C5C8B0955D4911B88F1A105CAD96B09AEBEC_RuntimeMethod_var)));
	}

IL_0036:
	{
		U3CU3Ec__DisplayClass74_0_tB1C6F79892902A4EEE2282BCA5ECF650D4F7955C* L_9 = V_0;
		NullCheck(L_9);
		LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* L_10 = L_9->___levelData;
		NullCheck(L_10);
		int32_t L_11 = L_10->___chapterId;
		U3CU3Ec__DisplayClass74_0_tB1C6F79892902A4EEE2282BCA5ECF650D4F7955C* L_12 = V_0;
		NullCheck(L_12);
		LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* L_13 = L_12->___levelData;
		NullCheck(L_13);
		int32_t L_14 = L_13->___levelId;
		bool L_15;
		L_15 = GameState_IsLevelUnlocked_mA15470234C68329950931198C078B05B900F0689(__this, L_11, L_14, NULL);
		if (L_15)
		{
			goto IL_0084;
		}
	}
	{
		U3CU3Ec__DisplayClass74_0_tB1C6F79892902A4EEE2282BCA5ECF650D4F7955C* L_16 = V_0;
		NullCheck(L_16);
		LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* L_17 = L_16->___levelData;
		NullCheck(L_17);
		int32_t L_18 = L_17->___chapterId;
		int32_t L_19 = L_18;
		RuntimeObject* L_20 = Box(il2cpp_defaults.int32_class, &L_19);
		U3CU3Ec__DisplayClass74_0_tB1C6F79892902A4EEE2282BCA5ECF650D4F7955C* L_21 = V_0;
		NullCheck(L_21);
		LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* L_22 = L_21->___levelData;
		NullCheck(L_22);
		int32_t L_23 = L_22->___levelId;
		int32_t L_24 = L_23;
		RuntimeObject* L_25 = Box(il2cpp_defaults.int32_class, &L_24);
		String_t* L_26;
		L_26 = String_Format_mFB7DA489BD99F4670881FF50EC017BFB0A5C0987(((String_t*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&_stringLiteral25D4AC2FBD67B76C7C7A73B6BDBE2D91795F86CC)), L_20, L_25, NULL);
		InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB* L_27 = (InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB*)il2cpp_codegen_object_new(((RuntimeClass*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB_il2cpp_TypeInfo_var)));
		InvalidOperationException__ctor_mE4CB6F4712AB6D99A2358FBAE2E052B3EE976162(L_27, L_26, NULL);
		IL2CPP_RAISE_MANAGED_EXCEPTION(L_27, ((RuntimeMethod*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&GameState_TryCompleteLevel_m1A62C5C8B0955D4911B88F1A105CAD96B09AEBEC_RuntimeMethod_var)));
	}

IL_0084:
	{
		bool L_28 = __this->___canPersist;
		if (!L_28)
		{
			goto IL_0094;
		}
	}
	{
		bool L_29;
		L_29 = GameState_get_HasPendingBoosterCommand_m3E1BA9F545A38895AD411E75F5DE3C9F67C76C15(__this, NULL);
		if (!L_29)
		{
			goto IL_0096;
		}
	}

IL_0094:
	{
		return (bool)0;
	}

IL_0096:
	{
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_30 = __this->___Save;
		String_t* L_31;
		L_31 = JsonUtility_ToJson_m28CC6843B9D3723D88AD13EA3829B71FDE7826BA(L_30, NULL);
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_32;
		L_32 = JsonUtility_FromJson_TisPlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30_m5FA532D489BA59FC0DD187FA87D56E71F1F63B04(L_31, JsonUtility_FromJson_TisPlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30_m5FA532D489BA59FC0DD187FA87D56E71F1F63B04_RuntimeMethod_var);
		V_1 = L_32;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_33 = V_1;
		NullCheck(L_33);
		List_1_tE17596A66614F9E1802150B437325DB3004F8681* L_34 = L_33->___levelProgress;
		if (L_34)
		{
			goto IL_00ba;
		}
	}
	{
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_35 = V_1;
		List_1_tE17596A66614F9E1802150B437325DB3004F8681* L_36 = (List_1_tE17596A66614F9E1802150B437325DB3004F8681*)il2cpp_codegen_object_new(List_1_tE17596A66614F9E1802150B437325DB3004F8681_il2cpp_TypeInfo_var);
		List_1__ctor_m2011DE22B9D007A29C2E04C6E21189D5168CBEBC(L_36, List_1__ctor_m2011DE22B9D007A29C2E04C6E21189D5168CBEBC_RuntimeMethod_var);
		NullCheck(L_35);
		L_35->___levelProgress = L_36;
		Il2CppCodeGenWriteBarrier((void**)(&L_35->___levelProgress), (void*)L_36);
	}

IL_00ba:
	{
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_37 = V_1;
		NullCheck(L_37);
		List_1_tE17596A66614F9E1802150B437325DB3004F8681* L_38 = L_37->___levelProgress;
		U3CU3Ec__DisplayClass74_0_tB1C6F79892902A4EEE2282BCA5ECF650D4F7955C* L_39 = V_0;
		Predicate_1_tB25A8FDB6257D222F05FE4E1071EADB7EEB0C814* L_40 = (Predicate_1_tB25A8FDB6257D222F05FE4E1071EADB7EEB0C814*)il2cpp_codegen_object_new(Predicate_1_tB25A8FDB6257D222F05FE4E1071EADB7EEB0C814_il2cpp_TypeInfo_var);
		Predicate_1__ctor_m2F993E85A2F022710AAEA1BC28A29428117AF0FF(L_40, L_39, (intptr_t)((void*)U3CU3Ec__DisplayClass74_0_U3CTryCompleteLevelU3Eb__0_mF93C07A27D5D2B3E4CC7EA0137407D4C320A6B2C_RuntimeMethod_var), NULL);
		NullCheck(L_38);
		LevelProgressData_tF4CF8BE6A46189E730098D0C3DEE22CAA57E0C9E* L_41;
		L_41 = List_1_Find_m71D79F9D5466D30CB1CBED42C60069703317EF3D(L_38, L_40, List_1_Find_m71D79F9D5466D30CB1CBED42C60069703317EF3D_RuntimeMethod_var);
		V_2 = L_41;
		LevelProgressData_tF4CF8BE6A46189E730098D0C3DEE22CAA57E0C9E* L_42 = V_2;
		if (L_42)
		{
			goto IL_0109;
		}
	}
	{
		LevelProgressData_tF4CF8BE6A46189E730098D0C3DEE22CAA57E0C9E* L_43 = (LevelProgressData_tF4CF8BE6A46189E730098D0C3DEE22CAA57E0C9E*)il2cpp_codegen_object_new(LevelProgressData_tF4CF8BE6A46189E730098D0C3DEE22CAA57E0C9E_il2cpp_TypeInfo_var);
		LevelProgressData__ctor_m402EF4C42282EEA365E976D2E459FEDFE1BBC16B(L_43, NULL);
		LevelProgressData_tF4CF8BE6A46189E730098D0C3DEE22CAA57E0C9E* L_44 = L_43;
		U3CU3Ec__DisplayClass74_0_tB1C6F79892902A4EEE2282BCA5ECF650D4F7955C* L_45 = V_0;
		NullCheck(L_45);
		LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* L_46 = L_45->___levelData;
		NullCheck(L_46);
		int32_t L_47 = L_46->___chapterId;
		NullCheck(L_44);
		L_44->___chapterId = L_47;
		LevelProgressData_tF4CF8BE6A46189E730098D0C3DEE22CAA57E0C9E* L_48 = L_44;
		U3CU3Ec__DisplayClass74_0_tB1C6F79892902A4EEE2282BCA5ECF650D4F7955C* L_49 = V_0;
		NullCheck(L_49);
		LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* L_50 = L_49->___levelData;
		NullCheck(L_50);
		int32_t L_51 = L_50->___levelId;
		NullCheck(L_48);
		L_48->___levelId = L_51;
		V_2 = L_48;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_52 = V_1;
		NullCheck(L_52);
		List_1_tE17596A66614F9E1802150B437325DB3004F8681* L_53 = L_52->___levelProgress;
		LevelProgressData_tF4CF8BE6A46189E730098D0C3DEE22CAA57E0C9E* L_54 = V_2;
		NullCheck(L_53);
		List_1_Add_mE786EABF7EB1669974F9EA4BD9F5E5377F09396A_inline(L_53, L_54, List_1_Add_mE786EABF7EB1669974F9EA4BD9F5E5377F09396A_RuntimeMethod_var);
	}

IL_0109:
	{
		LevelProgressData_tF4CF8BE6A46189E730098D0C3DEE22CAA57E0C9E* L_55 = V_2;
		NullCheck(L_55);
		int32_t L_56 = L_55->___attempts;
		if (L_56)
		{
			goto IL_0118;
		}
	}
	{
		LevelProgressData_tF4CF8BE6A46189E730098D0C3DEE22CAA57E0C9E* L_57 = V_2;
		NullCheck(L_57);
		L_57->___attempts = 1;
	}

IL_0118:
	{
		U3CU3Ec__DisplayClass74_0_tB1C6F79892902A4EEE2282BCA5ECF650D4F7955C* L_58 = V_0;
		NullCheck(L_58);
		LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* L_59 = L_58->___levelData;
		int32_t L_60 = ___1_score;
		NullCheck(L_59);
		int32_t L_61;
		L_61 = LevelData_EvaluateStars_m1D59963D0CB9D2C9DD64A88349F0B4BF688FA889(L_59, L_60, NULL);
		V_3 = L_61;
		LevelProgressData_tF4CF8BE6A46189E730098D0C3DEE22CAA57E0C9E* L_62 = V_2;
		LevelProgressData_tF4CF8BE6A46189E730098D0C3DEE22CAA57E0C9E* L_63 = V_2;
		NullCheck(L_63);
		int32_t L_64 = L_63->___bestScore;
		int32_t L_65 = ___1_score;
		int32_t L_66;
		L_66 = Mathf_Max_m7FA442918DE37E3A00106D1F2E789D65829792B8_inline(L_64, L_65, NULL);
		NullCheck(L_62);
		L_62->___bestScore = L_66;
		LevelProgressData_tF4CF8BE6A46189E730098D0C3DEE22CAA57E0C9E* L_67 = V_2;
		LevelProgressData_tF4CF8BE6A46189E730098D0C3DEE22CAA57E0C9E* L_68 = V_2;
		NullCheck(L_68);
		int32_t L_69 = L_68->___bestStars;
		int32_t L_70 = V_3;
		int32_t L_71;
		L_71 = Mathf_Max_m7FA442918DE37E3A00106D1F2E789D65829792B8_inline(L_69, L_70, NULL);
		NullCheck(L_67);
		L_67->___bestStars = L_71;
		U3CU3Ec__DisplayClass74_0_tB1C6F79892902A4EEE2282BCA5ECF650D4F7955C* L_72 = V_0;
		NullCheck(L_72);
		LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* L_73 = L_72->___levelData;
		NullCheck(L_73);
		LevelRewardData_tB1785C653344D16E0780E094B3936973F9DB0B85* L_74 = L_73->___rewards;
		LevelRewardData_tB1785C653344D16E0780E094B3936973F9DB0B85* L_75 = L_74;
		if (L_75)
		{
			G_B17_0 = L_75;
			goto IL_015d;
		}
		G_B16_0 = L_75;
	}
	{
		LevelRewardData_tB1785C653344D16E0780E094B3936973F9DB0B85* L_76 = (LevelRewardData_tB1785C653344D16E0780E094B3936973F9DB0B85*)il2cpp_codegen_object_new(LevelRewardData_tB1785C653344D16E0780E094B3936973F9DB0B85_il2cpp_TypeInfo_var);
		LevelRewardData__ctor_m53EFF22F2A588393935819D89E126745968881C7(L_76, NULL);
		G_B17_0 = L_76;
	}

IL_015d:
	{
		V_4 = G_B17_0;
		LevelProgressData_tF4CF8BE6A46189E730098D0C3DEE22CAA57E0C9E* L_77 = V_2;
		NullCheck(L_77);
		bool L_78 = L_77->___rewardClaimed;
		V_5 = (bool)((((int32_t)L_78) == ((int32_t)0))? 1 : 0);
		bool L_79 = V_5;
		if (L_79)
		{
			goto IL_0171;
		}
	}
	{
		G_B20_0 = 0;
		goto IL_017e;
	}

IL_0171:
	{
		LevelRewardData_tB1785C653344D16E0780E094B3936973F9DB0B85* L_80 = V_4;
		NullCheck(L_80);
		int32_t L_81 = L_80->___gold;
		int32_t L_82;
		L_82 = Mathf_Max_m7FA442918DE37E3A00106D1F2E789D65829792B8_inline(0, L_81, NULL);
		G_B20_0 = L_82;
	}

IL_017e:
	{
		V_6 = G_B20_0;
		bool L_83 = V_5;
		if (L_83)
		{
			goto IL_0187;
		}
	}
	{
		G_B23_0 = 0;
		goto IL_0194;
	}

IL_0187:
	{
		LevelRewardData_tB1785C653344D16E0780E094B3936973F9DB0B85* L_84 = V_4;
		NullCheck(L_84);
		int32_t L_85 = L_84->___gems;
		int32_t L_86;
		L_86 = Mathf_Max_m7FA442918DE37E3A00106D1F2E789D65829792B8_inline(0, L_85, NULL);
		G_B23_0 = L_86;
	}

IL_0194:
	{
		V_7 = G_B23_0;
	}
	try
	{
		{
			LevelProgressData_tF4CF8BE6A46189E730098D0C3DEE22CAA57E0C9E* L_87 = V_2;
			LevelProgressData_tF4CF8BE6A46189E730098D0C3DEE22CAA57E0C9E* L_88 = V_2;
			NullCheck(L_88);
			int32_t L_89 = L_88->___completions;
			if (((int64_t)L_89 + (int64_t)1 < (int64_t)kIl2CppInt32Min) || ((int64_t)L_89 + (int64_t)1 > (int64_t)kIl2CppInt32Max))
				IL2CPP_RAISE_MANAGED_EXCEPTION(il2cpp_codegen_get_overflow_exception(), GameState_TryCompleteLevel_m1A62C5C8B0955D4911B88F1A105CAD96B09AEBEC_RuntimeMethod_var);
			NullCheck(L_87);
			L_87->___completions = ((int32_t)il2cpp_codegen_add(L_89, 1));
			bool L_90 = V_5;
			if (!L_90)
			{
				goto IL_0238_1;
			}
		}
		{
			PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_91 = V_1;
			PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_92 = V_1;
			NullCheck(L_92);
			int32_t L_93 = L_92->___gold;
			int32_t L_94 = V_6;
			if (((int64_t)L_93 + (int64_t)L_94 < (int64_t)kIl2CppInt32Min) || ((int64_t)L_93 + (int64_t)L_94 > (int64_t)kIl2CppInt32Max))
				IL2CPP_RAISE_MANAGED_EXCEPTION(il2cpp_codegen_get_overflow_exception(), GameState_TryCompleteLevel_m1A62C5C8B0955D4911B88F1A105CAD96B09AEBEC_RuntimeMethod_var);
			NullCheck(L_91);
			L_91->___gold = ((int32_t)il2cpp_codegen_add(L_93, L_94));
			PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_95 = V_1;
			PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_96 = V_1;
			NullCheck(L_96);
			int32_t L_97 = L_96->___gems;
			int32_t L_98 = V_7;
			if (((int64_t)L_97 + (int64_t)L_98 < (int64_t)kIl2CppInt32Min) || ((int64_t)L_97 + (int64_t)L_98 > (int64_t)kIl2CppInt32Max))
				IL2CPP_RAISE_MANAGED_EXCEPTION(il2cpp_codegen_get_overflow_exception(), GameState_TryCompleteLevel_m1A62C5C8B0955D4911B88F1A105CAD96B09AEBEC_RuntimeMethod_var);
			NullCheck(L_95);
			L_95->___gems = ((int32_t)il2cpp_codegen_add(L_97, L_98));
			PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_99 = V_1;
			PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_100 = V_1;
			NullCheck(L_100);
			int32_t L_101 = L_100->___boosterMace;
			LevelRewardData_tB1785C653344D16E0780E094B3936973F9DB0B85* L_102 = V_4;
			NullCheck(L_102);
			int32_t L_103 = L_102->___boosterMace;
			int32_t L_104;
			L_104 = Mathf_Max_m7FA442918DE37E3A00106D1F2E789D65829792B8_inline(0, L_103, NULL);
			if (((int64_t)L_101 + (int64_t)L_104 < (int64_t)kIl2CppInt32Min) || ((int64_t)L_101 + (int64_t)L_104 > (int64_t)kIl2CppInt32Max))
				IL2CPP_RAISE_MANAGED_EXCEPTION(il2cpp_codegen_get_overflow_exception(), GameState_TryCompleteLevel_m1A62C5C8B0955D4911B88F1A105CAD96B09AEBEC_RuntimeMethod_var);
			NullCheck(L_99);
			L_99->___boosterMace = ((int32_t)il2cpp_codegen_add(L_101, L_104));
			PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_105 = V_1;
			PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_106 = V_1;
			NullCheck(L_106);
			int32_t L_107 = L_106->___boosterFirebird;
			LevelRewardData_tB1785C653344D16E0780E094B3936973F9DB0B85* L_108 = V_4;
			NullCheck(L_108);
			int32_t L_109 = L_108->___boosterFirebird;
			int32_t L_110;
			L_110 = Mathf_Max_m7FA442918DE37E3A00106D1F2E789D65829792B8_inline(0, L_109, NULL);
			if (((int64_t)L_107 + (int64_t)L_110 < (int64_t)kIl2CppInt32Min) || ((int64_t)L_107 + (int64_t)L_110 > (int64_t)kIl2CppInt32Max))
				IL2CPP_RAISE_MANAGED_EXCEPTION(il2cpp_codegen_get_overflow_exception(), GameState_TryCompleteLevel_m1A62C5C8B0955D4911B88F1A105CAD96B09AEBEC_RuntimeMethod_var);
			NullCheck(L_105);
			L_105->___boosterFirebird = ((int32_t)il2cpp_codegen_add(L_107, L_110));
			PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_111 = V_1;
			PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_112 = V_1;
			NullCheck(L_112);
			int32_t L_113 = L_112->___boosterLivingLight;
			LevelRewardData_tB1785C653344D16E0780E094B3936973F9DB0B85* L_114 = V_4;
			NullCheck(L_114);
			int32_t L_115 = L_114->___boosterLivingLight;
			int32_t L_116;
			L_116 = Mathf_Max_m7FA442918DE37E3A00106D1F2E789D65829792B8_inline(0, L_115, NULL);
			if (((int64_t)L_113 + (int64_t)L_116 < (int64_t)kIl2CppInt32Min) || ((int64_t)L_113 + (int64_t)L_116 > (int64_t)kIl2CppInt32Max))
				IL2CPP_RAISE_MANAGED_EXCEPTION(il2cpp_codegen_get_overflow_exception(), GameState_TryCompleteLevel_m1A62C5C8B0955D4911B88F1A105CAD96B09AEBEC_RuntimeMethod_var);
			NullCheck(L_111);
			L_111->___boosterLivingLight = ((int32_t)il2cpp_codegen_add(L_113, L_116));
			PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_117 = V_1;
			PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_118 = V_1;
			NullCheck(L_118);
			int32_t L_119 = L_118->___boosterSeaPearl;
			LevelRewardData_tB1785C653344D16E0780E094B3936973F9DB0B85* L_120 = V_4;
			NullCheck(L_120);
			int32_t L_121 = L_120->___boosterSeaPearl;
			int32_t L_122;
			L_122 = Mathf_Max_m7FA442918DE37E3A00106D1F2E789D65829792B8_inline(0, L_121, NULL);
			if (((int64_t)L_119 + (int64_t)L_122 < (int64_t)kIl2CppInt32Min) || ((int64_t)L_119 + (int64_t)L_122 > (int64_t)kIl2CppInt32Max))
				IL2CPP_RAISE_MANAGED_EXCEPTION(il2cpp_codegen_get_overflow_exception(), GameState_TryCompleteLevel_m1A62C5C8B0955D4911B88F1A105CAD96B09AEBEC_RuntimeMethod_var);
			NullCheck(L_117);
			L_117->___boosterSeaPearl = ((int32_t)il2cpp_codegen_add(L_119, L_122));
			LevelProgressData_tF4CF8BE6A46189E730098D0C3DEE22CAA57E0C9E* L_123 = V_2;
			NullCheck(L_123);
			L_123->___rewardClaimed = (bool)1;
		}

IL_0238_1:
		{
			goto IL_0243;
		}
	}
	catch(Il2CppExceptionWrapper& e)
	{
		if(il2cpp_codegen_class_is_assignable_from (((RuntimeClass*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&OverflowException_t6F6AD8CACE20C37F701C05B373A215C4802FAB0C_il2cpp_TypeInfo_var)), il2cpp_codegen_object_class(e.ex)))
		{
			IL2CPP_PUSH_ACTIVE_EXCEPTION(e.ex);
			goto CATCH_023a;
		}
		throw e;
	}

CATCH_023a:
	{
		OverflowException_t6F6AD8CACE20C37F701C05B373A215C4802FAB0C* L_124 = ((OverflowException_t6F6AD8CACE20C37F701C05B373A215C4802FAB0C*)IL2CPP_GET_ACTIVE_EXCEPTION(OverflowException_t6F6AD8CACE20C37F701C05B373A215C4802FAB0C*));;
		V_12 = (bool)0;
		IL2CPP_POP_ACTIVE_EXCEPTION(Exception_t*);
		goto IL_03bf;
	}

IL_0243:
	{
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_125 = V_1;
		U3CU3Ec__DisplayClass74_0_tB1C6F79892902A4EEE2282BCA5ECF650D4F7955C* L_126 = V_0;
		NullCheck(L_126);
		LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* L_127 = L_126->___levelData;
		NullCheck(L_127);
		int32_t L_128 = L_127->___chapterId;
		NullCheck(L_125);
		L_125->___lastCompletedChapter = L_128;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_129 = V_1;
		U3CU3Ec__DisplayClass74_0_tB1C6F79892902A4EEE2282BCA5ECF650D4F7955C* L_130 = V_0;
		NullCheck(L_130);
		LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* L_131 = L_130->___levelData;
		NullCheck(L_131);
		int32_t L_132 = L_131->___levelId;
		NullCheck(L_129);
		L_129->___lastCompletedLevel = L_132;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_133 = V_1;
		int32_t L_134 = V_6;
		NullCheck(L_133);
		L_133->___lastRewardGold = L_134;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_135 = V_1;
		int32_t L_136 = V_7;
		NullCheck(L_135);
		L_135->___lastRewardGems = L_136;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_137 = V_1;
		bool L_138 = V_5;
		if (!L_138)
		{
			G_B30_0 = L_137;
			goto IL_0281;
		}
		G_B29_0 = L_137;
	}
	{
		int32_t L_139 = V_6;
		G_B31_0 = ((((int32_t)L_139) > ((int32_t)0))? 1 : 0);
		G_B31_1 = G_B29_0;
		goto IL_0282;
	}

IL_0281:
	{
		G_B31_0 = 0;
		G_B31_1 = G_B30_0;
	}

IL_0282:
	{
		NullCheck(G_B31_1);
		G_B31_1->___lastRewardEligibleForDouble = (bool)G_B31_0;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_140 = V_1;
		NullCheck(L_140);
		L_140->___lastRewardDoubled = (bool)0;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_141 = V_1;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_142 = V_1;
		NullCheck(L_142);
		int32_t L_143 = L_142->___storyStage;
		U3CU3Ec__DisplayClass74_0_tB1C6F79892902A4EEE2282BCA5ECF650D4F7955C* L_144 = V_0;
		NullCheck(L_144);
		LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* L_145 = L_144->___levelData;
		NullCheck(L_145);
		int32_t L_146 = L_145->___levelId;
		int32_t L_147;
		L_147 = Mathf_Max_m7FA442918DE37E3A00106D1F2E789D65829792B8_inline(L_143, L_146, NULL);
		NullCheck(L_141);
		L_141->___storyStage = L_147;
		U3CU3Ec__DisplayClass74_0_tB1C6F79892902A4EEE2282BCA5ECF650D4F7955C* L_148 = V_0;
		NullCheck(L_148);
		LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* L_149 = L_148->___levelData;
		LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* L_150;
		L_150 = ChapterLevelCatalog_Next_m32F35B3A065D88170B3BCD8325F49157B3CF18F6(L_149, NULL);
		V_8 = L_150;
		LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* L_151 = V_8;
		if (!L_151)
		{
			goto IL_02ef;
		}
	}
	{
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_152 = V_1;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_153 = V_1;
		NullCheck(L_153);
		int32_t L_154 = L_153->___highestUnlockedLevel;
		LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* L_155 = V_8;
		NullCheck(L_155);
		int32_t L_156 = L_155->___levelId;
		int32_t L_157;
		L_157 = Mathf_Max_m7FA442918DE37E3A00106D1F2E789D65829792B8_inline(L_154, L_156, NULL);
		NullCheck(L_152);
		L_152->___highestUnlockedLevel = L_157;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_158 = V_1;
		LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* L_159 = V_8;
		NullCheck(L_159);
		int32_t L_160 = L_159->___chapterId;
		NullCheck(L_158);
		L_158->___selectedChapter = L_160;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_161 = V_1;
		LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* L_162 = V_8;
		NullCheck(L_162);
		int32_t L_163 = L_162->___levelId;
		NullCheck(L_161);
		L_161->___selectedLevel = L_163;
		goto IL_032d;
	}

IL_02ef:
	{
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_164 = V_1;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_165 = V_1;
		NullCheck(L_165);
		int32_t L_166 = L_165->___highestUnlockedLevel;
		U3CU3Ec__DisplayClass74_0_tB1C6F79892902A4EEE2282BCA5ECF650D4F7955C* L_167 = V_0;
		NullCheck(L_167);
		LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* L_168 = L_167->___levelData;
		NullCheck(L_168);
		int32_t L_169 = L_168->___levelId;
		int32_t L_170;
		L_170 = Mathf_Max_m7FA442918DE37E3A00106D1F2E789D65829792B8_inline(L_166, L_169, NULL);
		NullCheck(L_164);
		L_164->___highestUnlockedLevel = L_170;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_171 = V_1;
		U3CU3Ec__DisplayClass74_0_tB1C6F79892902A4EEE2282BCA5ECF650D4F7955C* L_172 = V_0;
		NullCheck(L_172);
		LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* L_173 = L_172->___levelData;
		NullCheck(L_173);
		int32_t L_174 = L_173->___chapterId;
		NullCheck(L_171);
		L_171->___selectedChapter = L_174;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_175 = V_1;
		U3CU3Ec__DisplayClass74_0_tB1C6F79892902A4EEE2282BCA5ECF650D4F7955C* L_176 = V_0;
		NullCheck(L_176);
		LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* L_177 = L_176->___levelData;
		NullCheck(L_177);
		int32_t L_178 = L_177->___levelId;
		NullCheck(L_175);
		L_175->___selectedLevel = L_178;
	}

IL_032d:
	{
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_179 = V_1;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_180 = V_1;
		NullCheck(L_180);
		int32_t L_181 = L_180->___selectedChapter;
		NullCheck(L_179);
		L_179->___chapter = L_181;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_182 = V_1;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_183 = V_1;
		NullCheck(L_183);
		int32_t L_184 = L_183->___highestUnlockedLevel;
		NullCheck(L_182);
		L_182->___level = L_184;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_185 = V_1;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_186 = V_1;
		int32_t L_187;
		L_187 = GameState_CalculateTotalStars_m59DF79CAF1120C86E571DFED1EBC6F615FC41FC5(L_186, NULL);
		NullCheck(L_185);
		L_185->___stars = L_187;
		U3CU3Ec__DisplayClass74_0_tB1C6F79892902A4EEE2282BCA5ECF650D4F7955C* L_188 = V_0;
		NullCheck(L_188);
		LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* L_189 = L_188->___levelData;
		int32_t L_190 = V_3;
		int32_t L_191 = V_6;
		int32_t L_192 = V_7;
		bool L_193 = V_5;
		int32_t L_194 = ___1_score;
		LevelCompletionResult_t4C786293AEB16558E39DC3DCEEE1542A5487F03E* L_195 = (LevelCompletionResult_t4C786293AEB16558E39DC3DCEEE1542A5487F03E*)il2cpp_codegen_object_new(LevelCompletionResult_t4C786293AEB16558E39DC3DCEEE1542A5487F03E_il2cpp_TypeInfo_var);
		LevelCompletionResult__ctor_m11F0272596ADC6663FD9DDF196CD5FA0F977AC78(L_195, L_189, L_190, L_191, L_192, L_193, L_194, NULL);
		V_9 = L_195;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_196 = V_1;
		bool L_197;
		L_197 = SaveService_TrySave_m11A043A57FCBE0607BDAAA6C34B4E969064615B0(L_196, (&V_11), NULL);
		V_10 = L_197;
		PlayerSaveWriteResult_tD3521C4F0AD4F171529F5475A2BD7B28C0CB1D9C* L_198 = V_11;
		GameState_set_LastWriteResult_mE42274830424D045544D465431B5C582BC50A567_inline(__this, L_198, NULL);
		bool L_199 = V_10;
		if (L_199)
		{
			goto IL_038e;
		}
	}
	{
		PlayerSaveWriteResult_tD3521C4F0AD4F171529F5475A2BD7B28C0CB1D9C* L_200 = V_11;
		NullCheck(L_200);
		bool L_201;
		L_201 = PlayerSaveWriteResult_get_PrimaryDurablyCommitted_m0D28E9CF94BC730C6E592F0CC6D4638D8961D000_inline(L_200, NULL);
		if (L_201)
		{
			goto IL_038e;
		}
	}
	{
		__this->___canPersist = (bool)0;
		return (bool)0;
	}

IL_038e:
	{
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_202 = V_1;
		__this->___Save = L_202;
		Il2CppCodeGenWriteBarrier((void**)(&__this->___Save), (void*)L_202);
		bool L_203 = V_10;
		__this->___canPersist = L_203;
		LevelCompletionResult_t4C786293AEB16558E39DC3DCEEE1542A5487F03E** L_204 = ___2_completion;
		LevelCompletionResult_t4C786293AEB16558E39DC3DCEEE1542A5487F03E* L_205 = V_9;
		LevelCompletionResult_t4C786293AEB16558E39DC3DCEEE1542A5487F03E* L_206 = L_205;
		V_13 = L_206;
		GameState_set_LastCompletionResult_mBC96C630298B2EA9E1657205DA637CCA1BB8E82F_inline(__this, L_206, NULL);
		LevelCompletionResult_t4C786293AEB16558E39DC3DCEEE1542A5487F03E* L_207 = V_13;
		il2cpp_codegen_stind<RuntimeObject*>((RuntimeObject**)L_204, (RuntimeObject*)L_207);
		Il2CppCodeGenWriteBarrier((void**)(RuntimeObject**)L_204, (void*)(RuntimeObject*)L_207);
		Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* L_208 = __this->___Changed;
		Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* L_209 = L_208;
		if (L_209)
		{
			G_B39_0 = L_209;
			goto IL_03b8;
		}
		G_B38_0 = L_209;
	}
	{
		goto IL_03bd;
	}

IL_03b8:
	{
		NullCheck(G_B39_0);
		Action_Invoke_m7126A54DACA72B845424072887B5F3A51FC3808E_inline(G_B39_0, NULL);
	}

IL_03bd:
	{
		return (bool)1;
	}

IL_03bf:
	{
		bool L_210 = V_12;
		return L_210;
	}
}
