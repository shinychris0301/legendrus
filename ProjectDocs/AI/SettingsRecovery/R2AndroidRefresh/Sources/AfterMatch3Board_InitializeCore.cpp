IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Match3Board_InitializeCore_m9AF2370FD9D71DA21A8A460DA6C8392CA8E1E7E9 (Match3Board_tE4CAA58AF06191DC578D60DE5FA7659B8C8ACD69* __this, LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* ___0_levelData, Action_1_tD69A6DC9FBE94131E52F5A73B2A9D4AB51EEC404* ___1_onWin, Action_1_tD69A6DC9FBE94131E52F5A73B2A9D4AB51EEC404* ___2_onFail, Nullable_1_tCF32C56A2641879C053C86F273C0C6EC1B40BC28 ___3_committedAttempt, RetainedRoundData_t3EA1BA932DFF2408098F1FB87EC5A5A24130D7A1* ___4_retained, bool ___5_preRegisteredOrdinaryAttempt, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Action_2_t7463B4187C533C6A6D08AE282D19A0E2E2C43F59_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&BoardResolutionPlanner_t406B06C1345DBB98BCEEC09079B44161EC11584E_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&CanvasGroupU5BU2CU5D_tEB84059F65BF2D11FC2E171C31AE365EE14F4E92_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Component_GetComponent_TisCoreLoopVfxPresenter_tE910CE8C54823137C9B728BDF5D4AF1F974B2629_m549830C808CE5EEBF5BBCFA22829828568B956C4_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Debug_t8394C7EEAECA3689C2C9B9DE9C7166D73596276F_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&GameObject_AddComponent_TisCoreLoopVfxPresenter_tE910CE8C54823137C9B728BDF5D4AF1F974B2629_m7B3528003C74D82B4D9697BDB5BAF50E81C8C945_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&GameplayCellMechanicsU5BU2CU5D_t03CE458703BE0CA287D70CB098DC3D5A91CE5A1C_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&GemShatterView_t80F06E6F29BAD82083095696D19D791F890B3DF6_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&HashSet_1_Clear_m0181AAD373CCA0B6C26FE43A802121B3C0DE5F90_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ImageU5BU2CU5D_tE1BBCA9E0E3B201C19F136F7C3ECA0C2C52AC909_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&LevelGoalRuntimeSnapshot_t24F2DAA1BB402B7D65674674F9EB6E4C816F76EE_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&LevelGoalRuntime_tE6D112739A0BF1C24963E7792D1AFDBD02897B9B_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Match3Board_OnGameplayPhaseChanged_m55DD322CA8568007E040C4E1839C0A2392F65407_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Match3CellInputU5BU2CU5D_tE910E9FB76E9A7CE95A991B085545D951472CC68_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&MaterialBreakView_tDAFC40328458688CED885B9CD3829D721AAB4B32_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Nullable_1_GetValueOrDefault_m8D130DB7F2A1E694736B449176F9C26DB456597B_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Nullable_1_get_HasValue_mCF2FD8B3055FA87FC9C504F2122B3B0FAEDE3EC9_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ProductionGameplayArt_tAC6E14B3903B3A1CA5CB5F45C61BC57B8D200E75_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&PrototypeImpactView_tE25D72CAD44A6824CF1D457FD47AA30A1F2EF60D_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&RectTransformU5BU2CU5D_t5CC9267DFBB3DD103D05387E721CC3496529D72D_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&SpriteU5BU5D_tCEE379E10CAD9DBFA770B331480592548ED0EA1B_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&_stringLiteral22795E9DF10F963C56196B72FCB81B192C3EA4D1);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&_stringLiteral589A74837A2634FEA2C0BAECCD5E79EA1BBE5386);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&_stringLiteral58EC50A9B0BB5302C846D8955B5ECDFBF7EC33CB);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&_stringLiteral6E4FCD87E88F2704AB463530C70DBCB3D116A875);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&_stringLiteralD2F90B879E9C875DD5A61CDA1C1FDE5C0B9C1550);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&_stringLiteralD8224EB348914F18B459158B0498D27694A7CD12);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&_stringLiteralDCC83F93DF4DF808F41773846742D1857B5FA601);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&_stringLiteralDD0F04E0A3F0BA1F904E0E27720F367F4228C69D);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&_stringLiteralE42E8BB820D4F7550A0F04619F4E15FDC56943B9);
		s_Il2CppMethodInitialized = true;
	}
	int32_t V_0 = 0;
	int32_t V_1 = 0;
	Nullable_1_tCF32C56A2641879C053C86F273C0C6EC1B40BC28 V_2;
	memset((&V_2), 0, sizeof(V_2));
	Guid_t V_3;
	memset((&V_3), 0, sizeof(V_3));
	int32_t V_4 = 0;
	Match3Board_tE4CAA58AF06191DC578D60DE5FA7659B8C8ACD69* G_B2_0 = NULL;
	Match3Board_tE4CAA58AF06191DC578D60DE5FA7659B8C8ACD69* G_B1_0 = NULL;
	int32_t G_B3_0 = 0;
	Match3Board_tE4CAA58AF06191DC578D60DE5FA7659B8C8ACD69* G_B3_1 = NULL;
	LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* G_B5_0 = NULL;
	Match3Board_tE4CAA58AF06191DC578D60DE5FA7659B8C8ACD69* G_B5_1 = NULL;
	LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* G_B4_0 = NULL;
	Match3Board_tE4CAA58AF06191DC578D60DE5FA7659B8C8ACD69* G_B4_1 = NULL;
	int32_t G_B12_0 = 0;
	Match3Board_tE4CAA58AF06191DC578D60DE5FA7659B8C8ACD69* G_B16_0 = NULL;
	Match3Board_tE4CAA58AF06191DC578D60DE5FA7659B8C8ACD69* G_B15_0 = NULL;
	String_t* G_B17_0 = NULL;
	Match3Board_tE4CAA58AF06191DC578D60DE5FA7659B8C8ACD69* G_B17_1 = NULL;
	String_t* G_B19_0 = NULL;
	Match3Board_tE4CAA58AF06191DC578D60DE5FA7659B8C8ACD69* G_B19_1 = NULL;
	String_t* G_B18_0 = NULL;
	Match3Board_tE4CAA58AF06191DC578D60DE5FA7659B8C8ACD69* G_B18_1 = NULL;
	Match3Board_tE4CAA58AF06191DC578D60DE5FA7659B8C8ACD69* G_B26_0 = NULL;
	Match3Board_tE4CAA58AF06191DC578D60DE5FA7659B8C8ACD69* G_B25_0 = NULL;
	Sprite_tAFF74BC83CD68037494CB0B4F28CBDF8971CAB99* G_B27_0 = NULL;
	Match3Board_tE4CAA58AF06191DC578D60DE5FA7659B8C8ACD69* G_B27_1 = NULL;
	Match3Board_tE4CAA58AF06191DC578D60DE5FA7659B8C8ACD69* G_B29_0 = NULL;
	Match3Board_tE4CAA58AF06191DC578D60DE5FA7659B8C8ACD69* G_B28_0 = NULL;
	Sprite_tAFF74BC83CD68037494CB0B4F28CBDF8971CAB99* G_B30_0 = NULL;
	Match3Board_tE4CAA58AF06191DC578D60DE5FA7659B8C8ACD69* G_B30_1 = NULL;
	{
		bool L_0;
		L_0 = Nullable_1_get_HasValue_mCF2FD8B3055FA87FC9C504F2122B3B0FAEDE3EC9_inline((&___3_committedAttempt), Nullable_1_get_HasValue_mCF2FD8B3055FA87FC9C504F2122B3B0FAEDE3EC9_RuntimeMethod_var);
		if (!L_0)
		{
			G_B2_0 = __this;
			goto IL_0011;
		}
		G_B1_0 = __this;
	}
	{
		bool L_1 = ___5_preRegisteredOrdinaryAttempt;
		G_B3_0 = ((((int32_t)L_1) == ((int32_t)0))? 1 : 0);
		G_B3_1 = G_B1_0;
		goto IL_0012;
	}

IL_0011:
	{
		G_B3_0 = 0;
		G_B3_1 = G_B2_0;
	}

IL_0012:
	{
		NullCheck(G_B3_1);
		G_B3_1->___campaignManaged = (bool)G_B3_0;
		Action_1_tD69A6DC9FBE94131E52F5A73B2A9D4AB51EEC404* L_2 = ___1_onWin;
		__this->___onWin = L_2;
		Il2CppCodeGenWriteBarrier((void**)(&__this->___onWin), (void*)L_2);
		Action_1_tD69A6DC9FBE94131E52F5A73B2A9D4AB51EEC404* L_3 = ___2_onFail;
		__this->___onFail = L_3;
		Il2CppCodeGenWriteBarrier((void**)(&__this->___onFail), (void*)L_3);
		LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* L_4 = ___0_levelData;
		LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* L_5 = L_4;
		if (L_5)
		{
			G_B5_0 = L_5;
			G_B5_1 = __this;
			goto IL_0031;
		}
		G_B4_0 = L_5;
		G_B4_1 = __this;
	}
	{
		LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* L_6;
		L_6 = ChapterLevelCatalog_ResolveSelected_m31E802BDED4683B59A35F355FA271E8165513D93((PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30*)NULL, NULL);
		G_B5_0 = L_6;
		G_B5_1 = G_B4_1;
	}

IL_0031:
	{
		NullCheck(G_B5_1);
		G_B5_1->___currentLevel = G_B5_0;
		Il2CppCodeGenWriteBarrier((void**)(&G_B5_1->___currentLevel), (void*)G_B5_0);
		LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* L_7 = __this->___currentLevel;
		NullCheck(L_7);
		int32_t L_8 = L_7->___columns;
		int32_t L_9;
		L_9 = Mathf_Max_m7FA442918DE37E3A00106D1F2E789D65829792B8_inline(3, L_8, NULL);
		__this->___columns = L_9;
		LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* L_10 = __this->___currentLevel;
		NullCheck(L_10);
		int32_t L_11 = L_10->___rows;
		int32_t L_12;
		L_12 = Mathf_Max_m7FA442918DE37E3A00106D1F2E789D65829792B8_inline(3, L_11, NULL);
		__this->___rows = L_12;
		LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* L_13 = __this->___currentLevel;
		NullCheck(L_13);
		int32_t L_14 = L_13->___tileTypes;
		int32_t L_15;
		L_15 = Mathf_Clamp_m4DC36EEFDBE5F07C16249DA568023C5ECCFF0E7B_inline(L_14, 3, 6, NULL);
		__this->___tileTypes = L_15;
		int32_t L_16 = __this->___columns;
		int32_t L_17 = __this->___rows;
		il2cpp_array_size_t L_19[] = { (il2cpp_array_size_t)L_16, (il2cpp_array_size_t)L_17 };
		ImageU5BU2CU5D_tE1BBCA9E0E3B201C19F136F7C3ECA0C2C52AC909* L_18 = (ImageU5BU2CU5D_tE1BBCA9E0E3B201C19F136F7C3ECA0C2C52AC909*)GenArrayNew(ImageU5BU2CU5D_tE1BBCA9E0E3B201C19F136F7C3ECA0C2C52AC909_il2cpp_TypeInfo_var, L_19);
		__this->___cellBackgrounds = L_18;
		Il2CppCodeGenWriteBarrier((void**)(&__this->___cellBackgrounds), (void*)L_18);
		int32_t L_20 = __this->___columns;
		int32_t L_21 = __this->___rows;
		il2cpp_array_size_t L_23[] = { (il2cpp_array_size_t)L_20, (il2cpp_array_size_t)L_21 };
		GameplayCellMechanicsU5BU2CU5D_t03CE458703BE0CA287D70CB098DC3D5A91CE5A1C* L_22 = (GameplayCellMechanicsU5BU2CU5D_t03CE458703BE0CA287D70CB098DC3D5A91CE5A1C*)GenArrayNew(GameplayCellMechanicsU5BU2CU5D_t03CE458703BE0CA287D70CB098DC3D5A91CE5A1C_il2cpp_TypeInfo_var, L_23);
		__this->___cellMechanics = L_22;
		Il2CppCodeGenWriteBarrier((void**)(&__this->___cellMechanics), (void*)L_22);
		int32_t L_24 = __this->___columns;
		int32_t L_25 = __this->___rows;
		il2cpp_array_size_t L_27[] = { (il2cpp_array_size_t)L_24, (il2cpp_array_size_t)L_25 };
		Match3CellInputU5BU2CU5D_tE910E9FB76E9A7CE95A991B085545D951472CC68* L_26 = (Match3CellInputU5BU2CU5D_tE910E9FB76E9A7CE95A991B085545D951472CC68*)GenArrayNew(Match3CellInputU5BU2CU5D_tE910E9FB76E9A7CE95A991B085545D951472CC68_il2cpp_TypeInfo_var, L_27);
		__this->___cellInputs = L_26;
		Il2CppCodeGenWriteBarrier((void**)(&__this->___cellInputs), (void*)L_26);
		int32_t L_28 = __this->___columns;
		int32_t L_29 = __this->___rows;
		il2cpp_array_size_t L_31[] = { (il2cpp_array_size_t)L_28, (il2cpp_array_size_t)L_29 };
		ImageU5BU2CU5D_tE1BBCA9E0E3B201C19F136F7C3ECA0C2C52AC909* L_30 = (ImageU5BU2CU5D_tE1BBCA9E0E3B201C19F136F7C3ECA0C2C52AC909*)GenArrayNew(ImageU5BU2CU5D_tE1BBCA9E0E3B201C19F136F7C3ECA0C2C52AC909_il2cpp_TypeInfo_var, L_31);
		__this->___gemImages = L_30;
		Il2CppCodeGenWriteBarrier((void**)(&__this->___gemImages), (void*)L_30);
		int32_t L_32 = __this->___columns;
		int32_t L_33 = __this->___rows;
		il2cpp_array_size_t L_35[] = { (il2cpp_array_size_t)L_32, (il2cpp_array_size_t)L_33 };
		ImageU5BU2CU5D_tE1BBCA9E0E3B201C19F136F7C3ECA0C2C52AC909* L_34 = (ImageU5BU2CU5D_tE1BBCA9E0E3B201C19F136F7C3ECA0C2C52AC909*)GenArrayNew(ImageU5BU2CU5D_tE1BBCA9E0E3B201C19F136F7C3ECA0C2C52AC909_il2cpp_TypeInfo_var, L_35);
		__this->___specialImages = L_34;
		Il2CppCodeGenWriteBarrier((void**)(&__this->___specialImages), (void*)L_34);
		int32_t L_36 = __this->___columns;
		int32_t L_37 = __this->___rows;
		il2cpp_array_size_t L_39[] = { (il2cpp_array_size_t)L_36, (il2cpp_array_size_t)L_37 };
		RectTransformU5BU2CU5D_t5CC9267DFBB3DD103D05387E721CC3496529D72D* L_38 = (RectTransformU5BU2CU5D_t5CC9267DFBB3DD103D05387E721CC3496529D72D*)GenArrayNew(RectTransformU5BU2CU5D_t5CC9267DFBB3DD103D05387E721CC3496529D72D_il2cpp_TypeInfo_var, L_39);
		__this->___visualRoots = L_38;
		Il2CppCodeGenWriteBarrier((void**)(&__this->___visualRoots), (void*)L_38);
		int32_t L_40 = __this->___columns;
		int32_t L_41 = __this->___rows;
		il2cpp_array_size_t L_43[] = { (il2cpp_array_size_t)L_40, (il2cpp_array_size_t)L_41 };
		CanvasGroupU5BU2CU5D_tEB84059F65BF2D11FC2E171C31AE365EE14F4E92* L_42 = (CanvasGroupU5BU2CU5D_tEB84059F65BF2D11FC2E171C31AE365EE14F4E92*)GenArrayNew(CanvasGroupU5BU2CU5D_tEB84059F65BF2D11FC2E171C31AE365EE14F4E92_il2cpp_TypeInfo_var, L_43);
		__this->___visualGroups = L_42;
		Il2CppCodeGenWriteBarrier((void**)(&__this->___visualGroups), (void*)L_42);
		GameplayStateMachine_t01C75AC2E3E92D6A53E4B80363DB5C26D85DA0CA* L_44 = __this->___interaction;
		NullCheck(L_44);
		GameplayStateMachine_Force_m611EBC14C71224E77DCEC80729A25636364BD1AD(L_44, 0, NULL);
		GameplayStateMachine_t01C75AC2E3E92D6A53E4B80363DB5C26D85DA0CA* L_45 = __this->___interaction;
		Action_2_t7463B4187C533C6A6D08AE282D19A0E2E2C43F59* L_46 = (Action_2_t7463B4187C533C6A6D08AE282D19A0E2E2C43F59*)il2cpp_codegen_object_new(Action_2_t7463B4187C533C6A6D08AE282D19A0E2E2C43F59_il2cpp_TypeInfo_var);
		Action_2__ctor_m1C7BD94925E1664C72C698DC0E4B131DA7975C9D(L_46, __this, (intptr_t)((void*)Match3Board_OnGameplayPhaseChanged_m55DD322CA8568007E040C4E1839C0A2392F65407_RuntimeMethod_var), NULL);
		NullCheck(L_45);
		GameplayStateMachine_add_Changed_m4F917B2203EFC5CAC183018E06E6D8D328F30EDD(L_45, L_46, NULL);
		Match3Board_ResetMotionTimeline_m51D4471A32C0B924D5CD951B9E6D15F5ABF64E62(__this, NULL);
		HashSet_1_tEFC6605F7DE53F71946C33FD371E53C3100F2178* L_47 = __this->___warnedCallbackFailures;
		NullCheck(L_47);
		HashSet_1_Clear_m0181AAD373CCA0B6C26FE43A802121B3C0DE5F90(L_47, HashSet_1_Clear_m0181AAD373CCA0B6C26FE43A802121B3C0DE5F90_RuntimeMethod_var);
		__this->___activePointerId = ((int32_t)-2147483648LL);
		LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* L_48 = __this->___currentLevel;
		NullCheck(L_48);
		int32_t L_49 = L_48->___moveLimit;
		int32_t L_50;
		L_50 = Mathf_Max_m7FA442918DE37E3A00106D1F2E789D65829792B8_inline(1, L_49, NULL);
		__this->___moves = L_50;
		LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* L_51 = __this->___currentLevel;
		NullCheck(L_51);
		int32_t L_52 = L_51->___startingScore;
		int32_t L_53;
		L_53 = Mathf_Max_m7FA442918DE37E3A00106D1F2E789D65829792B8_inline(0, L_52, NULL);
		__this->___score = L_53;
		LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* L_54 = __this->___currentLevel;
		int32_t L_55 = __this->___score;
		LevelGoalRuntime_tE6D112739A0BF1C24963E7792D1AFDBD02897B9B* L_56 = (LevelGoalRuntime_tE6D112739A0BF1C24963E7792D1AFDBD02897B9B*)il2cpp_codegen_object_new(LevelGoalRuntime_tE6D112739A0BF1C24963E7792D1AFDBD02897B9B_il2cpp_TypeInfo_var);
		LevelGoalRuntime__ctor_mDF8759DADA63093B81270008ADCE3F90F512DB01(L_56, L_54, L_55, NULL);
		__this->___goalRuntime = L_56;
		Il2CppCodeGenWriteBarrier((void**)(&__this->___goalRuntime), (void*)L_56);
		RetainedRoundData_t3EA1BA932DFF2408098F1FB87EC5A5A24130D7A1* L_57 = ___4_retained;
		if (L_57)
		{
			goto IL_01dc;
		}
	}
	{
		Nullable_1_tCF32C56A2641879C053C86F273C0C6EC1B40BC28 L_58 = ___3_committedAttempt;
		V_2 = L_58;
		bool L_59;
		L_59 = Nullable_1_get_HasValue_mCF2FD8B3055FA87FC9C504F2122B3B0FAEDE3EC9_inline((&V_2), Nullable_1_get_HasValue_mCF2FD8B3055FA87FC9C504F2122B3B0FAEDE3EC9_RuntimeMethod_var);
		if (L_59)
		{
			goto IL_01d3;
		}
	}
	{
		GameState_t5E590D2FB317152A4011A3CA69E3F311E0A7393A* L_60;
		L_60 = GameState_get_Instance_m3202061BC5E0A39B53697C35D0BEE15692173D74_inline(NULL);
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_61;
		L_61 = Object_op_Inequality_mD0BE578448EAA61948F25C32F8DD55AB1F778602(L_60, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (L_61)
		{
			goto IL_01c1;
		}
	}
	{
		G_B12_0 = 1;
		goto IL_01e3;
	}

IL_01c1:
	{
		GameState_t5E590D2FB317152A4011A3CA69E3F311E0A7393A* L_62;
		L_62 = GameState_get_Instance_m3202061BC5E0A39B53697C35D0BEE15692173D74_inline(NULL);
		LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* L_63 = __this->___currentLevel;
		NullCheck(L_62);
		int32_t L_64;
		L_64 = GameState_RegisterLevelAttempt_m31F23CCF55EB0C2270E2E0376980F8F61C4F9504(L_62, L_63, NULL);
		G_B12_0 = L_64;
		goto IL_01e3;
	}

IL_01d3:
	{
		int32_t L_65;
		L_65 = Nullable_1_GetValueOrDefault_m8D130DB7F2A1E694736B449176F9C26DB456597B_inline((&V_2), Nullable_1_GetValueOrDefault_m8D130DB7F2A1E694736B449176F9C26DB456597B_RuntimeMethod_var);
		G_B12_0 = L_65;
		goto IL_01e3;
	}

IL_01dc:
	{
		RetainedRoundData_t3EA1BA932DFF2408098F1FB87EC5A5A24130D7A1* L_66 = ___4_retained;
		NullCheck(L_66);
		int32_t L_67 = L_66->___attempt;
		G_B12_0 = L_67;
	}

IL_01e3:
	{
		V_0 = G_B12_0;
		int32_t L_68 = V_0;
		if ((((int32_t)L_68) >= ((int32_t)1)))
		{
			goto IL_01f3;
		}
	}
	{
		InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB* L_69 = (InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB*)il2cpp_codegen_object_new(((RuntimeClass*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB_il2cpp_TypeInfo_var)));
		InvalidOperationException__ctor_mE4CB6F4712AB6D99A2358FBAE2E052B3EE976162(L_69, ((String_t*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&_stringLiteral9E9EB871C5BD8E7D5CDC15FCEF357508E273F33C)), NULL);
		IL2CPP_RAISE_MANAGED_EXCEPTION(L_69, ((RuntimeMethod*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&Match3Board_InitializeCore_m9AF2370FD9D71DA21A8A460DA6C8392CA8E1E7E9_RuntimeMethod_var)));
	}

IL_01f3:
	{
		int32_t L_70 = V_0;
		__this->___retainedAttemptNumber = L_70;
		RetainedRoundData_t3EA1BA932DFF2408098F1FB87EC5A5A24130D7A1* L_71 = ___4_retained;
		if (L_71)
		{
			G_B16_0 = __this;
			goto IL_0202;
		}
		G_B15_0 = __this;
	}
	{
		G_B17_0 = ((String_t*)(NULL));
		G_B17_1 = G_B15_0;
		goto IL_0209;
	}

IL_0202:
	{
		RetainedRoundData_t3EA1BA932DFF2408098F1FB87EC5A5A24130D7A1* L_72 = ___4_retained;
		NullCheck(L_72);
		String_t* L_73 = L_72->___attemptId;
		G_B17_0 = L_73;
		G_B17_1 = G_B16_0;
	}

IL_0209:
	{
		String_t* L_74 = G_B17_0;
		if (L_74)
		{
			G_B19_0 = L_74;
			G_B19_1 = G_B17_1;
			goto IL_021f;
		}
		G_B18_0 = L_74;
		G_B18_1 = G_B17_1;
	}
	{
		Guid_t L_75;
		L_75 = Guid_NewGuid_m1F4894E8DC089811D6252148AD5858E58D43A7BD(NULL);
		V_3 = L_75;
		String_t* L_76;
		L_76 = Guid_ToString_mDAA91A4A993E3A7AD8339665E3F0CC35FE00E833((&V_3), _stringLiteralE42E8BB820D4F7550A0F04619F4E15FDC56943B9, NULL);
		G_B19_0 = L_76;
		G_B19_1 = G_B18_1;
	}

IL_021f:
	{
		NullCheck(G_B19_1);
		G_B19_1->___retainedAttemptId = G_B19_0;
		Il2CppCodeGenWriteBarrier((void**)(&G_B19_1->___retainedAttemptId), (void*)G_B19_0);
		LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* L_77 = __this->___currentLevel;
		NullCheck(L_77);
		int32_t L_78 = L_77->___chapterId;
		LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* L_79 = __this->___currentLevel;
		NullCheck(L_79);
		int32_t L_80 = L_79->___levelId;
		LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* L_81 = __this->___currentLevel;
		NullCheck(L_81);
		int32_t L_82 = L_81->___boardSeed;
		int32_t L_83 = V_0;
		int32_t L_84;
		L_84 = Mathf_Max_m7FA442918DE37E3A00106D1F2E789D65829792B8_inline(1, L_83, NULL);
		int32_t L_85;
		L_85 = BoardSeedUtility_CreateRuntimeSeed_mEAA3D5994206C5557C4F9B764007E6F31632C13E(L_78, L_80, ((int32_t)il2cpp_codegen_add(L_82, L_84)), NULL);
		V_1 = L_85;
		LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* L_86 = __this->___currentLevel;
		int32_t L_87 = V_1;
		int32_t L_88 = __this->___ruleVersion;
		BoardModel_tCD704F6305D85C7E64A4F76AF732A1FE2BC63601* L_89;
		L_89 = LevelBoardFactory_Create_m6160AAFC9EF6ECD0F333DE91C1C0B850A21AFE23(L_86, L_87, ((int32_t)128), L_88, NULL);
		__this->___model = L_89;
		Il2CppCodeGenWriteBarrier((void**)(&__this->___model), (void*)L_89);
		BoardModel_tCD704F6305D85C7E64A4F76AF732A1FE2BC63601* L_90 = __this->___model;
		int32_t L_91 = __this->___ruleVersion;
		NullCheck(L_90);
		BoardModel_set_SquareMatches_mC5F1CCACFBD8C8C0F9CCF3A93E3709FE6C9EF217_inline(L_90, (bool)((((int32_t)((((int32_t)L_91) < ((int32_t)3))? 1 : 0)) == ((int32_t)0))? 1 : 0), NULL);
		BoardModel_tCD704F6305D85C7E64A4F76AF732A1FE2BC63601* L_92 = __this->___model;
		int32_t L_93 = __this->___ruleVersion;
		NullCheck(L_92);
		BoardModel_set_StandaloneSpecials_mF044AB99BFFDAC701C8460B88BA4F2BE9EFABDA5_inline(L_92, (bool)((((int32_t)((((int32_t)L_93) < ((int32_t)5))? 1 : 0)) == ((int32_t)0))? 1 : 0), NULL);
		RetainedRoundData_t3EA1BA932DFF2408098F1FB87EC5A5A24130D7A1* L_94 = ___4_retained;
		if (!L_94)
		{
			goto IL_02f8;
		}
	}
	{
		BoardModel_tCD704F6305D85C7E64A4F76AF732A1FE2BC63601* L_95 = __this->___model;
		RetainedRoundData_t3EA1BA932DFF2408098F1FB87EC5A5A24130D7A1* L_96 = ___4_retained;
		LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* L_97 = __this->___currentLevel;
		NullCheck(L_96);
		BoardSnapshot_t689B87BDB1373020E2758049B806473BB7F89308* L_98;
		L_98 = RetainedRoundData_Snapshot_m013655A1E1415902284871AB730C330930CBD8AD(L_96, L_97, NULL);
		NullCheck(L_95);
		BoardModel_RestoreSnapshot_m717E15E29830990BF0E8DAFB34CFD6E0992C67A7(L_95, L_98, NULL);
		RetainedRoundData_t3EA1BA932DFF2408098F1FB87EC5A5A24130D7A1* L_99 = ___4_retained;
		NullCheck(L_99);
		int32_t L_100 = L_99->___moves;
		__this->___moves = L_100;
		RetainedRoundData_t3EA1BA932DFF2408098F1FB87EC5A5A24130D7A1* L_101 = ___4_retained;
		NullCheck(L_101);
		int32_t L_102 = L_101->___score;
		__this->___score = L_102;
		LevelGoalRuntime_tE6D112739A0BF1C24963E7792D1AFDBD02897B9B* L_103 = __this->___goalRuntime;
		RetainedRoundData_t3EA1BA932DFF2408098F1FB87EC5A5A24130D7A1* L_104 = ___4_retained;
		NullCheck(L_104);
		Int32U5BU5D_t19C97395396A72ECAF310612F0760F165060314C* L_105 = L_104->___goals;
		LevelGoalRuntimeSnapshot_t24F2DAA1BB402B7D65674674F9EB6E4C816F76EE* L_106 = (LevelGoalRuntimeSnapshot_t24F2DAA1BB402B7D65674674F9EB6E4C816F76EE*)il2cpp_codegen_object_new(LevelGoalRuntimeSnapshot_t24F2DAA1BB402B7D65674674F9EB6E4C816F76EE_il2cpp_TypeInfo_var);
		LevelGoalRuntimeSnapshot__ctor_mB1C0E014AAA8E8F868A2613574420863E34CBE79(L_106, L_105, NULL);
		NullCheck(L_103);
		LevelGoalRuntime_RestoreSnapshot_mE9E8A0BFBC6F9629CD501FD48AD5F9EF77B62CFE(L_103, L_106, NULL);
		GameplayStateMachine_t01C75AC2E3E92D6A53E4B80363DB5C26D85DA0CA* L_107 = __this->___interaction;
		NullCheck(L_107);
		GameplayStateMachine_Force_m611EBC14C71224E77DCEC80729A25636364BD1AD(L_107, ((int32_t)12), NULL);
	}

IL_02f8:
	{
		SpriteU5BU5D_tCEE379E10CAD9DBFA770B331480592548ED0EA1B* L_108 = (SpriteU5BU5D_tCEE379E10CAD9DBFA770B331480592548ED0EA1B*)(SpriteU5BU5D_tCEE379E10CAD9DBFA770B331480592548ED0EA1B*)SZArrayNew(SpriteU5BU5D_tCEE379E10CAD9DBFA770B331480592548ED0EA1B_il2cpp_TypeInfo_var, (uint32_t)6);
		__this->___gemSprites = L_108;
		Il2CppCodeGenWriteBarrier((void**)(&__this->___gemSprites), (void*)L_108);
		V_4 = 0;
		goto IL_031f;
	}

IL_0309:
	{
		SpriteU5BU5D_tCEE379E10CAD9DBFA770B331480592548ED0EA1B* L_109 = __this->___gemSprites;
		int32_t L_110 = V_4;
		int32_t L_111 = V_4;
		il2cpp_codegen_runtime_class_init_inline(ProductionGameplayArt_tAC6E14B3903B3A1CA5CB5F45C61BC57B8D200E75_il2cpp_TypeInfo_var);
		Sprite_tAFF74BC83CD68037494CB0B4F28CBDF8971CAB99* L_112;
		L_112 = ProductionGameplayArt_Gem_m0F7FCC514E81A71AAFE067A4643CAB791B93322D(L_111, NULL);
		NullCheck(L_109);
		(L_109)->SetAt(static_cast<il2cpp_array_size_t>(L_110), (Sprite_tAFF74BC83CD68037494CB0B4F28CBDF8971CAB99*)L_112);
		int32_t L_113 = V_4;
		V_4 = ((int32_t)il2cpp_codegen_add(L_113, 1));
	}

IL_031f:
	{
		int32_t L_114 = V_4;
		SpriteU5BU5D_tCEE379E10CAD9DBFA770B331480592548ED0EA1B* L_115 = __this->___gemSprites;
		NullCheck(L_115);
		int32_t L_116 = (il2cpp_codegen_conv<int32_t,int64_t,int64_t,false,false>((((RuntimeArray*)L_115)->max_length),NULL));
		if ((((int32_t)L_114) < ((int32_t)L_116)))
		{
			goto IL_0309;
		}
	}
	{
		il2cpp_codegen_runtime_class_init_inline(ProductionGameplayArt_tAC6E14B3903B3A1CA5CB5F45C61BC57B8D200E75_il2cpp_TypeInfo_var);
		Sprite_tAFF74BC83CD68037494CB0B4F28CBDF8971CAB99* L_117;
		L_117 = ProductionGameplayArt_Load_m59B222C07F5B485968D9DBC3A5A59136EE0AF1FC(_stringLiteralD2F90B879E9C875DD5A61CDA1C1FDE5C0B9C1550, NULL);
		__this->___rowSpecialSprite = L_117;
		Il2CppCodeGenWriteBarrier((void**)(&__this->___rowSpecialSprite), (void*)L_117);
		Sprite_tAFF74BC83CD68037494CB0B4F28CBDF8971CAB99* L_118;
		L_118 = ProductionGameplayArt_Load_m59B222C07F5B485968D9DBC3A5A59136EE0AF1FC(_stringLiteral589A74837A2634FEA2C0BAECCD5E79EA1BBE5386, NULL);
		__this->___columnSpecialSprite = L_118;
		Il2CppCodeGenWriteBarrier((void**)(&__this->___columnSpecialSprite), (void*)L_118);
		bool L_119;
		L_119 = Match3Board_get_UsesCurrentBoardArt_m18CE1C8EEE2834D494A8630E617E11C68F702508(__this, NULL);
		if (L_119)
		{
			G_B26_0 = __this;
			goto IL_0360;
		}
		G_B25_0 = __this;
	}
	{
		il2cpp_codegen_runtime_class_init_inline(ProductionGameplayArt_tAC6E14B3903B3A1CA5CB5F45C61BC57B8D200E75_il2cpp_TypeInfo_var);
		Sprite_tAFF74BC83CD68037494CB0B4F28CBDF8971CAB99* L_120;
		L_120 = ProductionGameplayArt_Load_m59B222C07F5B485968D9DBC3A5A59136EE0AF1FC(_stringLiteral58EC50A9B0BB5302C846D8955B5ECDFBF7EC33CB, NULL);
		G_B27_0 = L_120;
		G_B27_1 = G_B25_0;
		goto IL_036a;
	}

IL_0360:
	{
		Sprite_tAFF74BC83CD68037494CB0B4F28CBDF8971CAB99* L_121;
		L_121 = PrototypeBoardArt_Load_mA923719A88D5F1CA2892B8F351D0D1E1C6E67237(_stringLiteralD8224EB348914F18B459158B0498D27694A7CD12, NULL);
		G_B27_0 = L_121;
		G_B27_1 = G_B26_0;
	}

IL_036a:
	{
		NullCheck(G_B27_1);
		G_B27_1->___burstSpecialSprite = G_B27_0;
		Il2CppCodeGenWriteBarrier((void**)(&G_B27_1->___burstSpecialSprite), (void*)G_B27_0);
		bool L_122;
		L_122 = Match3Board_get_UsesCurrentBoardArt_m18CE1C8EEE2834D494A8630E617E11C68F702508(__this, NULL);
		if (L_122)
		{
			G_B29_0 = __this;
			goto IL_0384;
		}
		G_B28_0 = __this;
	}
	{
		il2cpp_codegen_runtime_class_init_inline(ProductionGameplayArt_tAC6E14B3903B3A1CA5CB5F45C61BC57B8D200E75_il2cpp_TypeInfo_var);
		Sprite_tAFF74BC83CD68037494CB0B4F28CBDF8971CAB99* L_123;
		L_123 = ProductionGameplayArt_Load_m59B222C07F5B485968D9DBC3A5A59136EE0AF1FC(_stringLiteralDD0F04E0A3F0BA1F904E0E27720F367F4228C69D, NULL);
		G_B30_0 = L_123;
		G_B30_1 = G_B28_0;
		goto IL_038e;
	}

IL_0384:
	{
		Sprite_tAFF74BC83CD68037494CB0B4F28CBDF8971CAB99* L_124;
		L_124 = PrototypeBoardArt_Load_mA923719A88D5F1CA2892B8F351D0D1E1C6E67237(_stringLiteralDD0F04E0A3F0BA1F904E0E27720F367F4228C69D, NULL);
		G_B30_0 = L_124;
		G_B30_1 = G_B29_0;
	}

IL_038e:
	{
		NullCheck(G_B30_1);
		G_B30_1->___colorSpecialSprite = G_B30_0;
		Il2CppCodeGenWriteBarrier((void**)(&G_B30_1->___colorSpecialSprite), (void*)G_B30_0);
		Match3Board_BuildGrid_mC6079479E2BBB5447C1DB91568A472303F10F396(__this, NULL);
		CoreLoopVfxPresenter_tE910CE8C54823137C9B728BDF5D4AF1F974B2629* L_125;
		L_125 = Component_GetComponent_TisCoreLoopVfxPresenter_tE910CE8C54823137C9B728BDF5D4AF1F974B2629_m549830C808CE5EEBF5BBCFA22829828568B956C4(__this, Component_GetComponent_TisCoreLoopVfxPresenter_tE910CE8C54823137C9B728BDF5D4AF1F974B2629_m549830C808CE5EEBF5BBCFA22829828568B956C4_RuntimeMethod_var);
		__this->___vfxPresenter = L_125;
		Il2CppCodeGenWriteBarrier((void**)(&__this->___vfxPresenter), (void*)L_125);
		CoreLoopVfxPresenter_tE910CE8C54823137C9B728BDF5D4AF1F974B2629* L_126 = __this->___vfxPresenter;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_127;
		L_127 = Object_op_Equality_mB6120F782D83091EF56A198FCEBCF066DB4A9605(L_126, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (!L_127)
		{
			goto IL_03c4;
		}
	}
	{
		GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_128;
		L_128 = Component_get_gameObject_m57AEFBB14DB39EC476F740BA000E170355DE691B(__this, NULL);
		NullCheck(L_128);
		CoreLoopVfxPresenter_tE910CE8C54823137C9B728BDF5D4AF1F974B2629* L_129;
		L_129 = GameObject_AddComponent_TisCoreLoopVfxPresenter_tE910CE8C54823137C9B728BDF5D4AF1F974B2629_m7B3528003C74D82B4D9697BDB5BAF50E81C8C945(L_128, GameObject_AddComponent_TisCoreLoopVfxPresenter_tE910CE8C54823137C9B728BDF5D4AF1F974B2629_m7B3528003C74D82B4D9697BDB5BAF50E81C8C945_RuntimeMethod_var);
		__this->___vfxPresenter = L_129;
		Il2CppCodeGenWriteBarrier((void**)(&__this->___vfxPresenter), (void*)L_129);
	}

IL_03c4:
	{
		CoreLoopVfxPresenter_tE910CE8C54823137C9B728BDF5D4AF1F974B2629* L_130 = __this->___vfxPresenter;
		NullCheck(L_130);
		CoreLoopVfxPresenter_Initialize_m5FD4D522DC7C3F0D438552B3BBD75A241E50DBF1(L_130, __this, NULL);
		BoardModel_tCD704F6305D85C7E64A4F76AF732A1FE2BC63601* L_131 = __this->___model;
		int32_t L_132 = __this->___ruleVersion;
		BoardResolutionPlanner_t406B06C1345DBB98BCEEC09079B44161EC11584E* L_133 = (BoardResolutionPlanner_t406B06C1345DBB98BCEEC09079B44161EC11584E*)il2cpp_codegen_object_new(BoardResolutionPlanner_t406B06C1345DBB98BCEEC09079B44161EC11584E_il2cpp_TypeInfo_var);
		BoardResolutionPlanner__ctor_mB912B3C51EA1C0CB897B4DAE52231FEB710AC9F1(L_133, L_131, (BoardBalanceProfile_t256D2E27898B041EF0DD0362EB0D091F1781A038*)NULL, L_132, NULL);
		__this->___resolutionPlanner = L_133;
		Il2CppCodeGenWriteBarrier((void**)(&__this->___resolutionPlanner), (void*)L_133);
		LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* L_134 = __this->___currentLevel;
		NullCheck(L_134);
		String_t* L_135 = L_134->___id;
		int32_t L_136 = V_0;
		int32_t L_137 = L_136;
		RuntimeObject* L_138 = Box(il2cpp_defaults.int32_class, &L_137);
		String_t* L_139;
		L_139 = String_Format_mFB7DA489BD99F4670881FF50EC017BFB0A5C0987(_stringLiteralDCC83F93DF4DF808F41773846742D1857B5FA601, L_135, L_138, NULL);
		int32_t L_140 = __this->___columns;
		int32_t L_141 = L_140;
		RuntimeObject* L_142 = Box(il2cpp_defaults.int32_class, &L_141);
		int32_t L_143 = __this->___rows;
		int32_t L_144 = L_143;
		RuntimeObject* L_145 = Box(il2cpp_defaults.int32_class, &L_144);
		int32_t L_146 = V_1;
		int32_t L_147 = L_146;
		RuntimeObject* L_148 = Box(il2cpp_defaults.int32_class, &L_147);
		String_t* L_149;
		L_149 = String_Format_mA0534D6E2AE4D67A6BD8D45B3321323930EB930C(_stringLiteral22795E9DF10F963C56196B72FCB81B192C3EA4D1, L_142, L_145, L_148, NULL);
		BoardModel_tCD704F6305D85C7E64A4F76AF732A1FE2BC63601* L_150 = __this->___model;
		NullCheck(L_150);
		String_t* L_151;
		L_151 = BoardModel_ComputeStateHash_m0C5A34304940F1672704BEEA01C59D508BA8E41B(L_150, NULL);
		String_t* L_152;
		L_152 = String_Concat_m093934F71A9B351911EE46311674ED463B180006(L_139, L_149, _stringLiteral6E4FCD87E88F2704AB463530C70DBCB3D116A875, L_151, NULL);
		il2cpp_codegen_runtime_class_init_inline(Debug_t8394C7EEAECA3689C2C9B9DE9C7166D73596276F_il2cpp_TypeInfo_var);
		Debug_Log_m87A9A3C761FF5C43ED8A53B16190A53D08F818BB(L_152, NULL);
		Match3Board_RefreshAll_m96F8E8431C0DD822FDB5F3B777748391CA8922B0(__this, NULL);
		int32_t L_153 = __this->___ruleVersion;
		if ((((int32_t)L_153) == ((int32_t)1)))
		{
			goto IL_0471;
		}
	}
	{
		il2cpp_codegen_runtime_class_init_inline(GemShatterView_t80F06E6F29BAD82083095696D19D791F890B3DF6_il2cpp_TypeInfo_var);
		GemShatterView_Attach_m81F36FAFC4ECDDC2174CB7AA5C0C5D822F56E385(__this, NULL);
		il2cpp_codegen_runtime_class_init_inline(PrototypeImpactView_tE25D72CAD44A6824CF1D457FD47AA30A1F2EF60D_il2cpp_TypeInfo_var);
		PrototypeImpactView_tE25D72CAD44A6824CF1D457FD47AA30A1F2EF60D* L_154;
		L_154 = PrototypeImpactView_Attach_m9C1DB598F7B8B94588B366082CFBD9552E9B3335(__this, NULL);
		PrototypeBoosterView_Attach_mA34F3068575858E5DD2E7A74B677DE1352A537A2(__this, NULL);
		PrototypeBoardAudio_t71A170AB22BE68893967F777162916AEDD966DC1* L_155;
		L_155 = PrototypeBoardAudio_Attach_mEA97DA6BDEF72549C9513AD8E1A0D8C58F7314B2(__this, NULL);
		__this->___mechanicsAudio = L_155;
		Il2CppCodeGenWriteBarrier((void**)(&__this->___mechanicsAudio), (void*)L_155);
	}

IL_0471:
	{
		int32_t L_156 = __this->___ruleVersion;
		if ((((int32_t)L_156) < ((int32_t)3)))
		{
			goto IL_0481;
		}
	}
	{
		SparkFlightView_t7B92AD22D3FD749058AD27F201E2F09CCCC43504* L_157;
		L_157 = SparkFlightView_Attach_mD04ED8B1A56F5A2507E5D01787AF4C0713BF6690(__this, NULL);
	}

IL_0481:
	{
		int32_t L_158 = __this->___ruleVersion;
		if ((((int32_t)L_158) < ((int32_t)5)))
		{
			goto IL_0497;
		}
	}
	{
		il2cpp_codegen_runtime_class_init_inline(MaterialBreakView_tDAFC40328458688CED885B9CD3829D721AAB4B32_il2cpp_TypeInfo_var);
		MaterialBreakView_tDAFC40328458688CED885B9CD3829D721AAB4B32* L_159;
		L_159 = MaterialBreakView_Attach_m465341320E640F99036193217EB8B452FA5099BB(__this, NULL);
		SpecialSequenceView_Attach_m2503CD61B3F02143E754C2ABB28EBBF395C527EC(__this, NULL);
	}

IL_0497:
	{
		Match3Board_PublishState_m888BDD9A14D75FA1915246672C1F7B94F83580FF(__this, NULL);
		return;
	}
}
