IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR LevelCompletionResult_t4C786293AEB16558E39DC3DCEEE1542A5487F03E* GameState_CompleteLevel_m0E7DE13DCE834A788086A415FC333D78E6A51ADF (GameState_t5E590D2FB317152A4011A3CA69E3F311E0A7393A* __this, LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* ___0_levelData, int32_t ___1_score, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&LevelCompletionResult_t4C786293AEB16558E39DC3DCEEE1542A5487F03E_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&LevelRewardData_tB1785C653344D16E0780E094B3936973F9DB0B85_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	LevelProgressData_tF4CF8BE6A46189E730098D0C3DEE22CAA57E0C9E* V_0 = NULL;
	int32_t V_1 = 0;
	LevelRewardData_tB1785C653344D16E0780E094B3936973F9DB0B85* V_2 = NULL;
	bool V_3 = false;
	int32_t V_4 = 0;
	int32_t V_5 = 0;
	LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* V_6 = NULL;
	LevelRewardData_tB1785C653344D16E0780E094B3936973F9DB0B85* G_B10_0 = NULL;
	LevelRewardData_tB1785C653344D16E0780E094B3936973F9DB0B85* G_B9_0 = NULL;
	int32_t G_B13_0 = 0;
	int32_t G_B16_0 = 0;
	PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* G_B20_0 = NULL;
	PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* G_B19_0 = NULL;
	int32_t G_B21_0 = 0;
	PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* G_B21_1 = NULL;
	{
		bool L_0;
		L_0 = GameState_ContinuationBlocksNewAttempt_m908136F30BB6F308311486AF5F487FAD53014484(__this, NULL);
		if (!L_0)
		{
			goto IL_0013;
		}
	}
	{
		InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB* L_1 = (InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB*)il2cpp_codegen_object_new(((RuntimeClass*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB_il2cpp_TypeInfo_var)));
		InvalidOperationException__ctor_mE4CB6F4712AB6D99A2358FBAE2E052B3EE976162(L_1, ((String_t*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&_stringLiteralB427EA8D0025E3A9E1FA52E07B616EA69EBB6A5C)), NULL);
		IL2CPP_RAISE_MANAGED_EXCEPTION(L_1, ((RuntimeMethod*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&GameState_CompleteLevel_m0E7DE13DCE834A788086A415FC333D78E6A51ADF_RuntimeMethod_var)));
	}

IL_0013:
	{
		LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* L_2 = ___0_levelData;
		if (L_2)
		{
			goto IL_0021;
		}
	}
	{
		ArgumentNullException_t327031E412FAB2351B0022DD5DAD47E67E597129* L_3 = (ArgumentNullException_t327031E412FAB2351B0022DD5DAD47E67E597129*)il2cpp_codegen_object_new(((RuntimeClass*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&ArgumentNullException_t327031E412FAB2351B0022DD5DAD47E67E597129_il2cpp_TypeInfo_var)));
		ArgumentNullException__ctor_m444AE141157E333844FC1A9500224C2F9FD24F4B(L_3, ((String_t*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&_stringLiteral93547B0D0ACC775E6401080B6E728559614917FB)), NULL);
		IL2CPP_RAISE_MANAGED_EXCEPTION(L_3, ((RuntimeMethod*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&GameState_CompleteLevel_m0E7DE13DCE834A788086A415FC333D78E6A51ADF_RuntimeMethod_var)));
	}

IL_0021:
	{
		LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* L_4 = ___0_levelData;
		NullCheck(L_4);
		int32_t L_5 = L_4->___chapterId;
		LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* L_6 = ___0_levelData;
		NullCheck(L_6);
		int32_t L_7 = L_6->___levelId;
		bool L_8;
		L_8 = GameState_IsLevelUnlocked_mA15470234C68329950931198C078B05B900F0689(__this, L_5, L_7, NULL);
		if (L_8)
		{
			goto IL_005b;
		}
	}
	{
		LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* L_9 = ___0_levelData;
		NullCheck(L_9);
		int32_t L_10 = L_9->___chapterId;
		int32_t L_11 = L_10;
		RuntimeObject* L_12 = Box(il2cpp_defaults.int32_class, &L_11);
		LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* L_13 = ___0_levelData;
		NullCheck(L_13);
		int32_t L_14 = L_13->___levelId;
		int32_t L_15 = L_14;
		RuntimeObject* L_16 = Box(il2cpp_defaults.int32_class, &L_15);
		String_t* L_17;
		L_17 = String_Format_mFB7DA489BD99F4670881FF50EC017BFB0A5C0987(((String_t*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&_stringLiteral25D4AC2FBD67B76C7C7A73B6BDBE2D91795F86CC)), L_12, L_16, NULL);
		InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB* L_18 = (InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB*)il2cpp_codegen_object_new(((RuntimeClass*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB_il2cpp_TypeInfo_var)));
		InvalidOperationException__ctor_mE4CB6F4712AB6D99A2358FBAE2E052B3EE976162(L_18, L_17, NULL);
		IL2CPP_RAISE_MANAGED_EXCEPTION(L_18, ((RuntimeMethod*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&GameState_CompleteLevel_m0E7DE13DCE834A788086A415FC333D78E6A51ADF_RuntimeMethod_var)));
	}

IL_005b:
	{
		LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* L_19 = ___0_levelData;
		NullCheck(L_19);
		int32_t L_20 = L_19->___chapterId;
		LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* L_21 = ___0_levelData;
		NullCheck(L_21);
		int32_t L_22 = L_21->___levelId;
		LevelProgressData_tF4CF8BE6A46189E730098D0C3DEE22CAA57E0C9E* L_23;
		L_23 = GameState_GetLevelProgress_mA5B0F87F59C027AFD2DB0B2523B2B811233EF4B1(__this, L_20, L_22, (bool)1, NULL);
		V_0 = L_23;
		LevelProgressData_tF4CF8BE6A46189E730098D0C3DEE22CAA57E0C9E* L_24 = V_0;
		NullCheck(L_24);
		int32_t L_25 = L_24->___attempts;
		if (L_25)
		{
			goto IL_007e;
		}
	}
	{
		LevelProgressData_tF4CF8BE6A46189E730098D0C3DEE22CAA57E0C9E* L_26 = V_0;
		NullCheck(L_26);
		L_26->___attempts = 1;
	}

IL_007e:
	{
		LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* L_27 = ___0_levelData;
		int32_t L_28 = ___1_score;
		NullCheck(L_27);
		int32_t L_29;
		L_29 = LevelData_EvaluateStars_m1D59963D0CB9D2C9DD64A88349F0B4BF688FA889(L_27, L_28, NULL);
		V_1 = L_29;
		LevelProgressData_tF4CF8BE6A46189E730098D0C3DEE22CAA57E0C9E* L_30 = V_0;
		LevelProgressData_tF4CF8BE6A46189E730098D0C3DEE22CAA57E0C9E* L_31 = L_30;
		NullCheck(L_31);
		int32_t L_32 = L_31->___completions;
		NullCheck(L_31);
		L_31->___completions = ((int32_t)il2cpp_codegen_add(L_32, 1));
		LevelProgressData_tF4CF8BE6A46189E730098D0C3DEE22CAA57E0C9E* L_33 = V_0;
		LevelProgressData_tF4CF8BE6A46189E730098D0C3DEE22CAA57E0C9E* L_34 = V_0;
		NullCheck(L_34);
		int32_t L_35 = L_34->___bestScore;
		int32_t L_36 = ___1_score;
		int32_t L_37;
		L_37 = Mathf_Max_m7FA442918DE37E3A00106D1F2E789D65829792B8_inline(L_35, L_36, NULL);
		NullCheck(L_33);
		L_33->___bestScore = L_37;
		LevelProgressData_tF4CF8BE6A46189E730098D0C3DEE22CAA57E0C9E* L_38 = V_0;
		LevelProgressData_tF4CF8BE6A46189E730098D0C3DEE22CAA57E0C9E* L_39 = V_0;
		NullCheck(L_39);
		int32_t L_40 = L_39->___bestStars;
		int32_t L_41 = V_1;
		int32_t L_42;
		L_42 = Mathf_Max_m7FA442918DE37E3A00106D1F2E789D65829792B8_inline(L_40, L_41, NULL);
		NullCheck(L_38);
		L_38->___bestStars = L_42;
		LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* L_43 = ___0_levelData;
		NullCheck(L_43);
		LevelRewardData_tB1785C653344D16E0780E094B3936973F9DB0B85* L_44 = L_43->___rewards;
		LevelRewardData_tB1785C653344D16E0780E094B3936973F9DB0B85* L_45 = L_44;
		if (L_45)
		{
			G_B10_0 = L_45;
			goto IL_00c7;
		}
		G_B9_0 = L_45;
	}
	{
		LevelRewardData_tB1785C653344D16E0780E094B3936973F9DB0B85* L_46 = (LevelRewardData_tB1785C653344D16E0780E094B3936973F9DB0B85*)il2cpp_codegen_object_new(LevelRewardData_tB1785C653344D16E0780E094B3936973F9DB0B85_il2cpp_TypeInfo_var);
		LevelRewardData__ctor_m53EFF22F2A588393935819D89E126745968881C7(L_46, NULL);
		G_B10_0 = L_46;
	}

IL_00c7:
	{
		V_2 = G_B10_0;
		LevelProgressData_tF4CF8BE6A46189E730098D0C3DEE22CAA57E0C9E* L_47 = V_0;
		NullCheck(L_47);
		bool L_48 = L_47->___rewardClaimed;
		V_3 = (bool)((((int32_t)L_48) == ((int32_t)0))? 1 : 0);
		bool L_49 = V_3;
		if (L_49)
		{
			goto IL_00d8;
		}
	}
	{
		G_B13_0 = 0;
		goto IL_00e4;
	}

IL_00d8:
	{
		LevelRewardData_tB1785C653344D16E0780E094B3936973F9DB0B85* L_50 = V_2;
		NullCheck(L_50);
		int32_t L_51 = L_50->___gold;
		int32_t L_52;
		L_52 = Mathf_Max_m7FA442918DE37E3A00106D1F2E789D65829792B8_inline(0, L_51, NULL);
		G_B13_0 = L_52;
	}

IL_00e4:
	{
		V_4 = G_B13_0;
		bool L_53 = V_3;
		if (L_53)
		{
			goto IL_00ec;
		}
	}
	{
		G_B16_0 = 0;
		goto IL_00f8;
	}

IL_00ec:
	{
		LevelRewardData_tB1785C653344D16E0780E094B3936973F9DB0B85* L_54 = V_2;
		NullCheck(L_54);
		int32_t L_55 = L_54->___gems;
		int32_t L_56;
		L_56 = Mathf_Max_m7FA442918DE37E3A00106D1F2E789D65829792B8_inline(0, L_55, NULL);
		G_B16_0 = L_56;
	}

IL_00f8:
	{
		V_5 = G_B16_0;
		bool L_57 = V_3;
		if (!L_57)
		{
			goto IL_01a7;
		}
	}
	{
		LevelProgressData_tF4CF8BE6A46189E730098D0C3DEE22CAA57E0C9E* L_58 = V_0;
		NullCheck(L_58);
		L_58->___rewardClaimed = (bool)1;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_59 = __this->___Save;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_60 = L_59;
		NullCheck(L_60);
		int32_t L_61 = L_60->___gold;
		int32_t L_62 = V_4;
		NullCheck(L_60);
		L_60->___gold = ((int32_t)il2cpp_codegen_add(L_61, L_62));
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_63 = __this->___Save;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_64 = L_63;
		NullCheck(L_64);
		int32_t L_65 = L_64->___gems;
		int32_t L_66 = V_5;
		NullCheck(L_64);
		L_64->___gems = ((int32_t)il2cpp_codegen_add(L_65, L_66));
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_67 = __this->___Save;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_68 = L_67;
		NullCheck(L_68);
		int32_t L_69 = L_68->___boosterMace;
		LevelRewardData_tB1785C653344D16E0780E094B3936973F9DB0B85* L_70 = V_2;
		NullCheck(L_70);
		int32_t L_71 = L_70->___boosterMace;
		int32_t L_72;
		L_72 = Mathf_Max_m7FA442918DE37E3A00106D1F2E789D65829792B8_inline(0, L_71, NULL);
		NullCheck(L_68);
		L_68->___boosterMace = ((int32_t)il2cpp_codegen_add(L_69, L_72));
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_73 = __this->___Save;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_74 = L_73;
		NullCheck(L_74);
		int32_t L_75 = L_74->___boosterFirebird;
		LevelRewardData_tB1785C653344D16E0780E094B3936973F9DB0B85* L_76 = V_2;
		NullCheck(L_76);
		int32_t L_77 = L_76->___boosterFirebird;
		int32_t L_78;
		L_78 = Mathf_Max_m7FA442918DE37E3A00106D1F2E789D65829792B8_inline(0, L_77, NULL);
		NullCheck(L_74);
		L_74->___boosterFirebird = ((int32_t)il2cpp_codegen_add(L_75, L_78));
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_79 = __this->___Save;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_80 = L_79;
		NullCheck(L_80);
		int32_t L_81 = L_80->___boosterLivingLight;
		LevelRewardData_tB1785C653344D16E0780E094B3936973F9DB0B85* L_82 = V_2;
		NullCheck(L_82);
		int32_t L_83 = L_82->___boosterLivingLight;
		int32_t L_84;
		L_84 = Mathf_Max_m7FA442918DE37E3A00106D1F2E789D65829792B8_inline(0, L_83, NULL);
		NullCheck(L_80);
		L_80->___boosterLivingLight = ((int32_t)il2cpp_codegen_add(L_81, L_84));
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_85 = __this->___Save;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_86 = L_85;
		NullCheck(L_86);
		int32_t L_87 = L_86->___boosterSeaPearl;
		LevelRewardData_tB1785C653344D16E0780E094B3936973F9DB0B85* L_88 = V_2;
		NullCheck(L_88);
		int32_t L_89 = L_88->___boosterSeaPearl;
		int32_t L_90;
		L_90 = Mathf_Max_m7FA442918DE37E3A00106D1F2E789D65829792B8_inline(0, L_89, NULL);
		NullCheck(L_86);
		L_86->___boosterSeaPearl = ((int32_t)il2cpp_codegen_add(L_87, L_90));
	}

IL_01a7:
	{
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_91 = __this->___Save;
		LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* L_92 = ___0_levelData;
		NullCheck(L_92);
		int32_t L_93 = L_92->___chapterId;
		NullCheck(L_91);
		L_91->___lastCompletedChapter = L_93;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_94 = __this->___Save;
		LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* L_95 = ___0_levelData;
		NullCheck(L_95);
		int32_t L_96 = L_95->___levelId;
		NullCheck(L_94);
		L_94->___lastCompletedLevel = L_96;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_97 = __this->___Save;
		int32_t L_98 = V_4;
		NullCheck(L_97);
		L_97->___lastRewardGold = L_98;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_99 = __this->___Save;
		int32_t L_100 = V_5;
		NullCheck(L_99);
		L_99->___lastRewardGems = L_100;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_101 = __this->___Save;
		bool L_102 = V_3;
		if (!L_102)
		{
			G_B20_0 = L_101;
			goto IL_01f3;
		}
		G_B19_0 = L_101;
	}
	{
		int32_t L_103 = V_4;
		G_B21_0 = ((((int32_t)L_103) > ((int32_t)0))? 1 : 0);
		G_B21_1 = G_B19_0;
		goto IL_01f4;
	}

IL_01f3:
	{
		G_B21_0 = 0;
		G_B21_1 = G_B20_0;
	}

IL_01f4:
	{
		NullCheck(G_B21_1);
		G_B21_1->___lastRewardEligibleForDouble = (bool)G_B21_0;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_104 = __this->___Save;
		NullCheck(L_104);
		L_104->___lastRewardDoubled = (bool)0;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_105 = __this->___Save;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_106 = __this->___Save;
		NullCheck(L_106);
		int32_t L_107 = L_106->___storyStage;
		LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* L_108 = ___0_levelData;
		NullCheck(L_108);
		int32_t L_109 = L_108->___levelId;
		int32_t L_110;
		L_110 = Mathf_Max_m7FA442918DE37E3A00106D1F2E789D65829792B8_inline(L_107, L_109, NULL);
		NullCheck(L_105);
		L_105->___storyStage = L_110;
		LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* L_111 = ___0_levelData;
		LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* L_112;
		L_112 = ChapterLevelCatalog_Next_m32F35B3A065D88170B3BCD8325F49157B3CF18F6(L_111, NULL);
		V_6 = L_112;
		LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* L_113 = V_6;
		if (!L_113)
		{
			goto IL_027a;
		}
	}
	{
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_114 = __this->___Save;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_115 = __this->___Save;
		NullCheck(L_115);
		int32_t L_116 = L_115->___highestUnlockedLevel;
		LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* L_117 = V_6;
		NullCheck(L_117);
		int32_t L_118 = L_117->___levelId;
		int32_t L_119;
		L_119 = Mathf_Max_m7FA442918DE37E3A00106D1F2E789D65829792B8_inline(L_116, L_118, NULL);
		NullCheck(L_114);
		L_114->___highestUnlockedLevel = L_119;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_120 = __this->___Save;
		LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* L_121 = V_6;
		NullCheck(L_121);
		int32_t L_122 = L_121->___chapterId;
		NullCheck(L_120);
		L_120->___selectedChapter = L_122;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_123 = __this->___Save;
		LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* L_124 = V_6;
		NullCheck(L_124);
		int32_t L_125 = L_124->___levelId;
		NullCheck(L_123);
		L_123->___selectedLevel = L_125;
		goto IL_02bd;
	}

IL_027a:
	{
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_126 = __this->___Save;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_127 = __this->___Save;
		NullCheck(L_127);
		int32_t L_128 = L_127->___highestUnlockedLevel;
		LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* L_129 = ___0_levelData;
		NullCheck(L_129);
		int32_t L_130 = L_129->___levelId;
		int32_t L_131;
		L_131 = Mathf_Max_m7FA442918DE37E3A00106D1F2E789D65829792B8_inline(L_128, L_130, NULL);
		NullCheck(L_126);
		L_126->___highestUnlockedLevel = L_131;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_132 = __this->___Save;
		LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* L_133 = ___0_levelData;
		NullCheck(L_133);
		int32_t L_134 = L_133->___chapterId;
		NullCheck(L_132);
		L_132->___selectedChapter = L_134;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_135 = __this->___Save;
		LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* L_136 = ___0_levelData;
		NullCheck(L_136);
		int32_t L_137 = L_136->___levelId;
		NullCheck(L_135);
		L_135->___selectedLevel = L_137;
	}

IL_02bd:
	{
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_138 = __this->___Save;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_139 = __this->___Save;
		NullCheck(L_139);
		int32_t L_140 = L_139->___selectedChapter;
		NullCheck(L_138);
		L_138->___chapter = L_140;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_141 = __this->___Save;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_142 = __this->___Save;
		NullCheck(L_142);
		int32_t L_143 = L_142->___highestUnlockedLevel;
		NullCheck(L_141);
		L_141->___level = L_143;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_144 = __this->___Save;
		int32_t L_145;
		L_145 = GameState_CalculateTotalStars_m1F4D790674EC2976BCF67CD87BD441FCD6CE0A38(__this, NULL);
		NullCheck(L_144);
		L_144->___stars = L_145;
		LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* L_146 = ___0_levelData;
		int32_t L_147 = V_1;
		int32_t L_148 = V_4;
		int32_t L_149 = V_5;
		bool L_150 = V_3;
		int32_t L_151 = ___1_score;
		LevelCompletionResult_t4C786293AEB16558E39DC3DCEEE1542A5487F03E* L_152 = (LevelCompletionResult_t4C786293AEB16558E39DC3DCEEE1542A5487F03E*)il2cpp_codegen_object_new(LevelCompletionResult_t4C786293AEB16558E39DC3DCEEE1542A5487F03E_il2cpp_TypeInfo_var);
		LevelCompletionResult__ctor_m11F0272596ADC6663FD9DDF196CD5FA0F977AC78(L_152, L_146, L_147, L_148, L_149, L_150, L_151, NULL);
		GameState_set_LastCompletionResult_mBC96C630298B2EA9E1657205DA637CCA1BB8E82F_inline(__this, L_152, NULL);
		GameState_Persist_m0261476FB722D9A1BAB0BB601B749EFC7E307C8D(__this, NULL);
		LevelCompletionResult_t4C786293AEB16558E39DC3DCEEE1542A5487F03E* L_153;
		L_153 = GameState_get_LastCompletionResult_m50CFB78EDA50FD111F72F15688A5A56B59999459_inline(__this, NULL);
		return L_153;
	}
}
