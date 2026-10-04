IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Match3Board_InitializeCore_m135C11F55EA1098D3F637709C35C16A00AD954A2 (Match3Board_tE4CAA58AF06191DC578D60DE5FA7659B8C8ACD69* __this, LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* ___0_levelData, Action_1_tD69A6DC9FBE94131E52F5A73B2A9D4AB51EEC404* ___1_onWin, Action_1_tD69A6DC9FBE94131E52F5A73B2A9D4AB51EEC404* ___2_onFail, Nullable_1_tCF32C56A2641879C053C86F273C0C6EC1B40BC28 ___3_committedAttempt, RetainedRoundData_t3EA1BA932DFF2408098F1FB87EC5A5A24130D7A1* ___4_retained, const RuntimeMethod* method) 
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
	LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* G_B2_0 = NULL;
	Match3Board_tE4CAA58AF06191DC578D60DE5FA7659B8C8ACD69* G_B2_1 = NULL;
	LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* G_B1_0 = NULL;
	Match3Board_tE4CAA58AF06191DC578D60DE5FA7659B8C8ACD69* G_B1_1 = NULL;
	int32_t G_B9_0 = 0;
	Match3Board_tE4CAA58AF06191DC578D60DE5FA7659B8C8ACD69* G_B11_0 = NULL;
	Match3Board_tE4CAA58AF06191DC578D60DE5FA7659B8C8ACD69* G_B10_0 = NULL;
	String_t* G_B12_0 = NULL;
	Match3Board_tE4CAA58AF06191DC578D60DE5FA7659B8C8ACD69* G_B12_1 = NULL;
	String_t* G_B14_0 = NULL;
	Match3Board_tE4CAA58AF06191DC578D60DE5FA7659B8C8ACD69* G_B14_1 = NULL;
	String_t* G_B13_0 = NULL;
	Match3Board_tE4CAA58AF06191DC578D60DE5FA7659B8C8ACD69* G_B13_1 = NULL;
	Match3Board_tE4CAA58AF06191DC578D60DE5FA7659B8C8ACD69* G_B21_0 = NULL;
	Match3Board_tE4CAA58AF06191DC578D60DE5FA7659B8C8ACD69* G_B20_0 = NULL;
	Sprite_tAFF74BC83CD68037494CB0B4F28CBDF8971CAB99* G_B22_0 = NULL;
	Match3Board_tE4CAA58AF06191DC578D60DE5FA7659B8C8ACD69* G_B22_1 = NULL;
	Match3Board_tE4CAA58AF06191DC578D60DE5FA7659B8C8ACD69* G_B24_0 = NULL;
	Match3Board_tE4CAA58AF06191DC578D60DE5FA7659B8C8ACD69* G_B23_0 = NULL;
	Sprite_tAFF74BC83CD68037494CB0B4F28CBDF8971CAB99* G_B25_0 = NULL;
	Match3Board_tE4CAA58AF06191DC578D60DE5FA7659B8C8ACD69* G_B25_1 = NULL;
	{
		bool L_0;
		L_0 = Nullable_1_get_HasValue_mCF2FD8B3055FA87FC9C504F2122B3B0FAEDE3EC9_inline((&___3_committedAttempt), Nullable_1_get_HasValue_mCF2FD8B3055FA87FC9C504F2122B3B0FAEDE3EC9_RuntimeMethod_var);
		__this->___campaignManaged = L_0;
		Action_1_tD69A6DC9FBE94131E52F5A73B2A9D4AB51EEC404* L_1 = ___1_onWin;
		__this->___onWin = L_1;
		Il2CppCodeGenWriteBarrier((void**)(&__this->___onWin), (void*)L_1);
		Action_1_tD69A6DC9FBE94131E52F5A73B2A9D4AB51EEC404* L_2 = ___2_onFail;
		__this->___onFail = L_2;
		Il2CppCodeGenWriteBarrier((void**)(&__this->___onFail), (void*)L_2);
		LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* L_3 = ___0_levelData;
		LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* L_4 = L_3;
		if (L_4)
		{
			G_B2_0 = L_4;
			G_B2_1 = __this;
			goto IL_0027;
		}
		G_B1_0 = L_4;
		G_B1_1 = __this;
	}
	{
		LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* L_5;
		L_5 = ChapterLevelCatalog_ResolveSelected_m31E802BDED4683B59A35F355FA271E8165513D93((PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30*)NULL, NULL);
		G_B2_0 = L_5;
		G_B2_1 = G_B1_1;
	}

IL_0027:
	{
		NullCheck(G_B2_1);
		G_B2_1->___currentLevel = G_B2_0;
		Il2CppCodeGenWriteBarrier((void**)(&G_B2_1->___currentLevel), (void*)G_B2_0);
		LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* L_6 = __this->___currentLevel;
		NullCheck(L_6);
		int32_t L_7 = L_6->___columns;
		int32_t L_8;
		L_8 = Mathf_Max_m7FA442918DE37E3A00106D1F2E789D65829792B8_inline(3, L_7, NULL);
		__this->___columns = L_8;
		LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* L_9 = __this->___currentLevel;
		NullCheck(L_9);
		int32_t L_10 = L_9->___rows;
		int32_t L_11;
		L_11 = Mathf_Max_m7FA442918DE37E3A00106D1F2E789D65829792B8_inline(3, L_10, NULL);
		__this->___rows = L_11;
		LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* L_12 = __this->___currentLevel;
		NullCheck(L_12);
		int32_t L_13 = L_12->___tileTypes;
		int32_t L_14;
		L_14 = Mathf_Clamp_m4DC36EEFDBE5F07C16249DA568023C5ECCFF0E7B_inline(L_13, 3, 6, NULL);
		__this->___tileTypes = L_14;
		int32_t L_15 = __this->___columns;
		int32_t L_16 = __this->___rows;
		il2cpp_array_size_t L_18[] = { (il2cpp_array_size_t)L_15, (il2cpp_array_size_t)L_16 };
		ImageU5BU2CU5D_tE1BBCA9E0E3B201C19F136F7C3ECA0C2C52AC909* L_17 = (ImageU5BU2CU5D_tE1BBCA9E0E3B201C19F136F7C3ECA0C2C52AC909*)GenArrayNew(ImageU5BU2CU5D_tE1BBCA9E0E3B201C19F136F7C3ECA0C2C52AC909_il2cpp_TypeInfo_var, L_18);
		__this->___cellBackgrounds = L_17;
		Il2CppCodeGenWriteBarrier((void**)(&__this->___cellBackgrounds), (void*)L_17);
		int32_t L_19 = __this->___columns;
		int32_t L_20 = __this->___rows;
		il2cpp_array_size_t L_22[] = { (il2cpp_array_size_t)L_19, (il2cpp_array_size_t)L_20 };
		GameplayCellMechanicsU5BU2CU5D_t03CE458703BE0CA287D70CB098DC3D5A91CE5A1C* L_21 = (GameplayCellMechanicsU5BU2CU5D_t03CE458703BE0CA287D70CB098DC3D5A91CE5A1C*)GenArrayNew(GameplayCellMechanicsU5BU2CU5D_t03CE458703BE0CA287D70CB098DC3D5A91CE5A1C_il2cpp_TypeInfo_var, L_22);
		__this->___cellMechanics = L_21;
		Il2CppCodeGenWriteBarrier((void**)(&__this->___cellMechanics), (void*)L_21);
		int32_t L_23 = __this->___columns;
		int32_t L_24 = __this->___rows;
		il2cpp_array_size_t L_26[] = { (il2cpp_array_size_t)L_23, (il2cpp_array_size_t)L_24 };
		Match3CellInputU5BU2CU5D_tE910E9FB76E9A7CE95A991B085545D951472CC68* L_25 = (Match3CellInputU5BU2CU5D_tE910E9FB76E9A7CE95A991B085545D951472CC68*)GenArrayNew(Match3CellInputU5BU2CU5D_tE910E9FB76E9A7CE95A991B085545D951472CC68_il2cpp_TypeInfo_var, L_26);
		__this->___cellInputs = L_25;
		Il2CppCodeGenWriteBarrier((void**)(&__this->___cellInputs), (void*)L_25);
		int32_t L_27 = __this->___columns;
		int32_t L_28 = __this->___rows;
		il2cpp_array_size_t L_30[] = { (il2cpp_array_size_t)L_27, (il2cpp_array_size_t)L_28 };
		ImageU5BU2CU5D_tE1BBCA9E0E3B201C19F136F7C3ECA0C2C52AC909* L_29 = (ImageU5BU2CU5D_tE1BBCA9E0E3B201C19F136F7C3ECA0C2C52AC909*)GenArrayNew(ImageU5BU2CU5D_tE1BBCA9E0E3B201C19F136F7C3ECA0C2C52AC909_il2cpp_TypeInfo_var, L_30);
		__this->___gemImages = L_29;
		Il2CppCodeGenWriteBarrier((void**)(&__this->___gemImages), (void*)L_29);
		int32_t L_31 = __this->___columns;
		int32_t L_32 = __this->___rows;
		il2cpp_array_size_t L_34[] = { (il2cpp_array_size_t)L_31, (il2cpp_array_size_t)L_32 };
		ImageU5BU2CU5D_tE1BBCA9E0E3B201C19F136F7C3ECA0C2C52AC909* L_33 = (ImageU5BU2CU5D_tE1BBCA9E0E3B201C19F136F7C3ECA0C2C52AC909*)GenArrayNew(ImageU5BU2CU5D_tE1BBCA9E0E3B201C19F136F7C3ECA0C2C52AC909_il2cpp_TypeInfo_var, L_34);
		__this->___specialImages = L_33;
		Il2CppCodeGenWriteBarrier((void**)(&__this->___specialImages), (void*)L_33);
		int32_t L_35 = __this->___columns;
		int32_t L_36 = __this->___rows;
		il2cpp_array_size_t L_38[] = { (il2cpp_array_size_t)L_35, (il2cpp_array_size_t)L_36 };
		RectTransformU5BU2CU5D_t5CC9267DFBB3DD103D05387E721CC3496529D72D* L_37 = (RectTransformU5BU2CU5D_t5CC9267DFBB3DD103D05387E721CC3496529D72D*)GenArrayNew(RectTransformU5BU2CU5D_t5CC9267DFBB3DD103D05387E721CC3496529D72D_il2cpp_TypeInfo_var, L_38);
		__this->___visualRoots = L_37;
		Il2CppCodeGenWriteBarrier((void**)(&__this->___visualRoots), (void*)L_37);
		int32_t L_39 = __this->___columns;
		int32_t L_40 = __this->___rows;
		il2cpp_array_size_t L_42[] = { (il2cpp_array_size_t)L_39, (il2cpp_array_size_t)L_40 };
		CanvasGroupU5BU2CU5D_tEB84059F65BF2D11FC2E171C31AE365EE14F4E92* L_41 = (CanvasGroupU5BU2CU5D_tEB84059F65BF2D11FC2E171C31AE365EE14F4E92*)GenArrayNew(CanvasGroupU5BU2CU5D_tEB84059F65BF2D11FC2E171C31AE365EE14F4E92_il2cpp_TypeInfo_var, L_42);
		__this->___visualGroups = L_41;
		Il2CppCodeGenWriteBarrier((void**)(&__this->___visualGroups), (void*)L_41);
		GameplayStateMachine_t01C75AC2E3E92D6A53E4B80363DB5C26D85DA0CA* L_43 = __this->___interaction;
		NullCheck(L_43);
		GameplayStateMachine_Force_m611EBC14C71224E77DCEC80729A25636364BD1AD(L_43, 0, NULL);
		GameplayStateMachine_t01C75AC2E3E92D6A53E4B80363DB5C26D85DA0CA* L_44 = __this->___interaction;
		Action_2_t7463B4187C533C6A6D08AE282D19A0E2E2C43F59* L_45 = (Action_2_t7463B4187C533C6A6D08AE282D19A0E2E2C43F59*)il2cpp_codegen_object_new(Action_2_t7463B4187C533C6A6D08AE282D19A0E2E2C43F59_il2cpp_TypeInfo_var);
		Action_2__ctor_m1C7BD94925E1664C72C698DC0E4B131DA7975C9D(L_45, __this, (intptr_t)((void*)Match3Board_OnGameplayPhaseChanged_m55DD322CA8568007E040C4E1839C0A2392F65407_RuntimeMethod_var), NULL);
		NullCheck(L_44);
		GameplayStateMachine_add_Changed_m4F917B2203EFC5CAC183018E06E6D8D328F30EDD(L_44, L_45, NULL);
		Match3Board_ResetMotionTimeline_m51D4471A32C0B924D5CD951B9E6D15F5ABF64E62(__this, NULL);
		HashSet_1_tEFC6605F7DE53F71946C33FD371E53C3100F2178* L_46 = __this->___warnedCallbackFailures;
		NullCheck(L_46);
		HashSet_1_Clear_m0181AAD373CCA0B6C26FE43A802121B3C0DE5F90(L_46, HashSet_1_Clear_m0181AAD373CCA0B6C26FE43A802121B3C0DE5F90_RuntimeMethod_var);
		__this->___activePointerId = ((int32_t)-2147483648LL);
		LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* L_47 = __this->___currentLevel;
		NullCheck(L_47);
		int32_t L_48 = L_47->___moveLimit;
		int32_t L_49;
		L_49 = Mathf_Max_m7FA442918DE37E3A00106D1F2E789D65829792B8_inline(1, L_48, NULL);
		__this->___moves = L_49;
		LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* L_50 = __this->___currentLevel;
		NullCheck(L_50);
		int32_t L_51 = L_50->___startingScore;
		int32_t L_52;
		L_52 = Mathf_Max_m7FA442918DE37E3A00106D1F2E789D65829792B8_inline(0, L_51, NULL);
		__this->___score = L_52;
		LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* L_53 = __this->___currentLevel;
		int32_t L_54 = __this->___score;
		LevelGoalRuntime_tE6D112739A0BF1C24963E7792D1AFDBD02897B9B* L_55 = (LevelGoalRuntime_tE6D112739A0BF1C24963E7792D1AFDBD02897B9B*)il2cpp_codegen_object_new(LevelGoalRuntime_tE6D112739A0BF1C24963E7792D1AFDBD02897B9B_il2cpp_TypeInfo_var);
		LevelGoalRuntime__ctor_mDF8759DADA63093B81270008ADCE3F90F512DB01(L_55, L_53, L_54, NULL);
		__this->___goalRuntime = L_55;
		Il2CppCodeGenWriteBarrier((void**)(&__this->___goalRuntime), (void*)L_55);
		RetainedRoundData_t3EA1BA932DFF2408098F1FB87EC5A5A24130D7A1* L_56 = ___4_retained;
		if (L_56)
		{
			goto IL_01d2;
		}
	}
	{
		Nullable_1_tCF32C56A2641879C053C86F273C0C6EC1B40BC28 L_57 = ___3_committedAttempt;
		V_2 = L_57;
		bool L_58;
		L_58 = Nullable_1_get_HasValue_mCF2FD8B3055FA87FC9C504F2122B3B0FAEDE3EC9_inline((&V_2), Nullable_1_get_HasValue_mCF2FD8B3055FA87FC9C504F2122B3B0FAEDE3EC9_RuntimeMethod_var);
		if (L_58)
		{
			goto IL_01c9;
		}
	}
	{
		GameState_t5E590D2FB317152A4011A3CA69E3F311E0A7393A* L_59;
		L_59 = GameState_get_Instance_m3202061BC5E0A39B53697C35D0BEE15692173D74_inline(NULL);
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_60;
		L_60 = Object_op_Inequality_mD0BE578448EAA61948F25C32F8DD55AB1F778602(L_59, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (L_60)
		{
			goto IL_01b7;
		}
	}
	{
		G_B9_0 = 1;
		goto IL_01d9;
	}

IL_01b7:
	{
		GameState_t5E590D2FB317152A4011A3CA69E3F311E0A7393A* L_61;
		L_61 = GameState_get_Instance_m3202061BC5E0A39B53697C35D0BEE15692173D74_inline(NULL);
		LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* L_62 = __this->___currentLevel;
		NullCheck(L_61);
		int32_t L_63;
		L_63 = GameState_RegisterLevelAttempt_m31F23CCF55EB0C2270E2E0376980F8F61C4F9504(L_61, L_62, NULL);
		G_B9_0 = L_63;
		goto IL_01d9;
	}

IL_01c9:
	{
		int32_t L_64;
		L_64 = Nullable_1_GetValueOrDefault_m8D130DB7F2A1E694736B449176F9C26DB456597B_inline((&V_2), Nullable_1_GetValueOrDefault_m8D130DB7F2A1E694736B449176F9C26DB456597B_RuntimeMethod_var);
		G_B9_0 = L_64;
		goto IL_01d9;
	}

IL_01d2:
	{
		RetainedRoundData_t3EA1BA932DFF2408098F1FB87EC5A5A24130D7A1* L_65 = ___4_retained;
		NullCheck(L_65);
		int32_t L_66 = L_65->___attempt;
		G_B9_0 = L_66;
	}

IL_01d9:
	{
		V_0 = G_B9_0;
		int32_t L_67 = V_0;
		__this->___retainedAttemptNumber = L_67;
		RetainedRoundData_t3EA1BA932DFF2408098F1FB87EC5A5A24130D7A1* L_68 = ___4_retained;
		if (L_68)
		{
			G_B11_0 = __this;
			goto IL_01e9;
		}
		G_B10_0 = __this;
	}
	{
		G_B12_0 = ((String_t*)(NULL));
		G_B12_1 = G_B10_0;
		goto IL_01f0;
	}

IL_01e9:
	{
		RetainedRoundData_t3EA1BA932DFF2408098F1FB87EC5A5A24130D7A1* L_69 = ___4_retained;
		NullCheck(L_69);
		String_t* L_70 = L_69->___attemptId;
		G_B12_0 = L_70;
		G_B12_1 = G_B11_0;
	}

IL_01f0:
	{
		String_t* L_71 = G_B12_0;
		if (L_71)
		{
			G_B14_0 = L_71;
			G_B14_1 = G_B12_1;
			goto IL_0206;
		}
		G_B13_0 = L_71;
		G_B13_1 = G_B12_1;
	}
	{
		Guid_t L_72;
		L_72 = Guid_NewGuid_m1F4894E8DC089811D6252148AD5858E58D43A7BD(NULL);
		V_3 = L_72;
		String_t* L_73;
		L_73 = Guid_ToString_mDAA91A4A993E3A7AD8339665E3F0CC35FE00E833((&V_3), _stringLiteralE42E8BB820D4F7550A0F04619F4E15FDC56943B9, NULL);
		G_B14_0 = L_73;
		G_B14_1 = G_B13_1;
	}

IL_0206:
	{
		NullCheck(G_B14_1);
		G_B14_1->___retainedAttemptId = G_B14_0;
		Il2CppCodeGenWriteBarrier((void**)(&G_B14_1->___retainedAttemptId), (void*)G_B14_0);
		LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* L_74 = __this->___currentLevel;
		NullCheck(L_74);
		int32_t L_75 = L_74->___chapterId;
		LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* L_76 = __this->___currentLevel;
		NullCheck(L_76);
		int32_t L_77 = L_76->___levelId;
		LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* L_78 = __this->___currentLevel;
		NullCheck(L_78);
		int32_t L_79 = L_78->___boardSeed;
		int32_t L_80 = V_0;
		int32_t L_81;
		L_81 = Mathf_Max_m7FA442918DE37E3A00106D1F2E789D65829792B8_inline(1, L_80, NULL);
		int32_t L_82;
		L_82 = BoardSeedUtility_CreateRuntimeSeed_mEAA3D5994206C5557C4F9B764007E6F31632C13E(L_75, L_77, ((int32_t)il2cpp_codegen_add(L_79, L_81)), NULL);
		V_1 = L_82;
		LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* L_83 = __this->___currentLevel;
		int32_t L_84 = V_1;
		int32_t L_85 = __this->___ruleVersion;
		BoardModel_tCD704F6305D85C7E64A4F76AF732A1FE2BC63601* L_86;
		L_86 = LevelBoardFactory_Create_m6160AAFC9EF6ECD0F333DE91C1C0B850A21AFE23(L_83, L_84, ((int32_t)128), L_85, NULL);
		__this->___model = L_86;
		Il2CppCodeGenWriteBarrier((void**)(&__this->___model), (void*)L_86);
		BoardModel_tCD704F6305D85C7E64A4F76AF732A1FE2BC63601* L_87 = __this->___model;
		int32_t L_88 = __this->___ruleVersion;
		NullCheck(L_87);
		BoardModel_set_SquareMatches_mC5F1CCACFBD8C8C0F9CCF3A93E3709FE6C9EF217_inline(L_87, (bool)((((int32_t)((((int32_t)L_88) < ((int32_t)3))? 1 : 0)) == ((int32_t)0))? 1 : 0), NULL);
		BoardModel_tCD704F6305D85C7E64A4F76AF732A1FE2BC63601* L_89 = __this->___model;
		int32_t L_90 = __this->___ruleVersion;
		NullCheck(L_89);
		BoardModel_set_StandaloneSpecials_mF044AB99BFFDAC701C8460B88BA4F2BE9EFABDA5_inline(L_89, (bool)((((int32_t)((((int32_t)L_90) < ((int32_t)5))? 1 : 0)) == ((int32_t)0))? 1 : 0), NULL);
		RetainedRoundData_t3EA1BA932DFF2408098F1FB87EC5A5A24130D7A1* L_91 = ___4_retained;
		if (!L_91)
		{
			goto IL_02df;
		}
	}
	{
		BoardModel_tCD704F6305D85C7E64A4F76AF732A1FE2BC63601* L_92 = __this->___model;
		RetainedRoundData_t3EA1BA932DFF2408098F1FB87EC5A5A24130D7A1* L_93 = ___4_retained;
		LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* L_94 = __this->___currentLevel;
		NullCheck(L_93);
		BoardSnapshot_t689B87BDB1373020E2758049B806473BB7F89308* L_95;
		L_95 = RetainedRoundData_Snapshot_m013655A1E1415902284871AB730C330930CBD8AD(L_93, L_94, NULL);
		NullCheck(L_92);
		BoardModel_RestoreSnapshot_m717E15E29830990BF0E8DAFB34CFD6E0992C67A7(L_92, L_95, NULL);
		RetainedRoundData_t3EA1BA932DFF2408098F1FB87EC5A5A24130D7A1* L_96 = ___4_retained;
		NullCheck(L_96);
		int32_t L_97 = L_96->___moves;
		__this->___moves = L_97;
		RetainedRoundData_t3EA1BA932DFF2408098F1FB87EC5A5A24130D7A1* L_98 = ___4_retained;
		NullCheck(L_98);
		int32_t L_99 = L_98->___score;
		__this->___score = L_99;
		LevelGoalRuntime_tE6D112739A0BF1C24963E7792D1AFDBD02897B9B* L_100 = __this->___goalRuntime;
		RetainedRoundData_t3EA1BA932DFF2408098F1FB87EC5A5A24130D7A1* L_101 = ___4_retained;
		NullCheck(L_101);
		Int32U5BU5D_t19C97395396A72ECAF310612F0760F165060314C* L_102 = L_101->___goals;
		LevelGoalRuntimeSnapshot_t24F2DAA1BB402B7D65674674F9EB6E4C816F76EE* L_103 = (LevelGoalRuntimeSnapshot_t24F2DAA1BB402B7D65674674F9EB6E4C816F76EE*)il2cpp_codegen_object_new(LevelGoalRuntimeSnapshot_t24F2DAA1BB402B7D65674674F9EB6E4C816F76EE_il2cpp_TypeInfo_var);
		LevelGoalRuntimeSnapshot__ctor_mB1C0E014AAA8E8F868A2613574420863E34CBE79(L_103, L_102, NULL);
		NullCheck(L_100);
		LevelGoalRuntime_RestoreSnapshot_mE9E8A0BFBC6F9629CD501FD48AD5F9EF77B62CFE(L_100, L_103, NULL);
		GameplayStateMachine_t01C75AC2E3E92D6A53E4B80363DB5C26D85DA0CA* L_104 = __this->___interaction;
		NullCheck(L_104);
		GameplayStateMachine_Force_m611EBC14C71224E77DCEC80729A25636364BD1AD(L_104, ((int32_t)12), NULL);
	}

IL_02df:
	{
		SpriteU5BU5D_tCEE379E10CAD9DBFA770B331480592548ED0EA1B* L_105 = (SpriteU5BU5D_tCEE379E10CAD9DBFA770B331480592548ED0EA1B*)(SpriteU5BU5D_tCEE379E10CAD9DBFA770B331480592548ED0EA1B*)SZArrayNew(SpriteU5BU5D_tCEE379E10CAD9DBFA770B331480592548ED0EA1B_il2cpp_TypeInfo_var, (uint32_t)6);
		__this->___gemSprites = L_105;
		Il2CppCodeGenWriteBarrier((void**)(&__this->___gemSprites), (void*)L_105);
		V_4 = 0;
		goto IL_0306;
	}

IL_02f0:
	{
		SpriteU5BU5D_tCEE379E10CAD9DBFA770B331480592548ED0EA1B* L_106 = __this->___gemSprites;
		int32_t L_107 = V_4;
		int32_t L_108 = V_4;
		il2cpp_codegen_runtime_class_init_inline(ProductionGameplayArt_tAC6E14B3903B3A1CA5CB5F45C61BC57B8D200E75_il2cpp_TypeInfo_var);
		Sprite_tAFF74BC83CD68037494CB0B4F28CBDF8971CAB99* L_109;
		L_109 = ProductionGameplayArt_Gem_m0F7FCC514E81A71AAFE067A4643CAB791B93322D(L_108, NULL);
		NullCheck(L_106);
		(L_106)->SetAt(static_cast<il2cpp_array_size_t>(L_107), (Sprite_tAFF74BC83CD68037494CB0B4F28CBDF8971CAB99*)L_109);
		int32_t L_110 = V_4;
		V_4 = ((int32_t)il2cpp_codegen_add(L_110, 1));
	}

IL_0306:
	{
		int32_t L_111 = V_4;
		SpriteU5BU5D_tCEE379E10CAD9DBFA770B331480592548ED0EA1B* L_112 = __this->___gemSprites;
		NullCheck(L_112);
		int32_t L_113 = (il2cpp_codegen_conv<int32_t,int64_t,int64_t,false,false>((((RuntimeArray*)L_112)->max_length),NULL));
		if ((((int32_t)L_111) < ((int32_t)L_113)))
		{
			goto IL_02f0;
		}
	}
	{
		il2cpp_codegen_runtime_class_init_inline(ProductionGameplayArt_tAC6E14B3903B3A1CA5CB5F45C61BC57B8D200E75_il2cpp_TypeInfo_var);
		Sprite_tAFF74BC83CD68037494CB0B4F28CBDF8971CAB99* L_114;
		L_114 = ProductionGameplayArt_Load_m59B222C07F5B485968D9DBC3A5A59136EE0AF1FC(_stringLiteralD2F90B879E9C875DD5A61CDA1C1FDE5C0B9C1550, NULL);
		__this->___rowSpecialSprite = L_114;
		Il2CppCodeGenWriteBarrier((void**)(&__this->___rowSpecialSprite), (void*)L_114);
		Sprite_tAFF74BC83CD68037494CB0B4F28CBDF8971CAB99* L_115;
		L_115 = ProductionGameplayArt_Load_m59B222C07F5B485968D9DBC3A5A59136EE0AF1FC(_stringLiteral589A74837A2634FEA2C0BAECCD5E79EA1BBE5386, NULL);
		__this->___columnSpecialSprite = L_115;
		Il2CppCodeGenWriteBarrier((void**)(&__this->___columnSpecialSprite), (void*)L_115);
		bool L_116;
		L_116 = Match3Board_get_UsesCurrentBoardArt_m18CE1C8EEE2834D494A8630E617E11C68F702508(__this, NULL);
		if (L_116)
		{
			G_B21_0 = __this;
			goto IL_0347;
		}
		G_B20_0 = __this;
	}
	{
		il2cpp_codegen_runtime_class_init_inline(ProductionGameplayArt_tAC6E14B3903B3A1CA5CB5F45C61BC57B8D200E75_il2cpp_TypeInfo_var);
		Sprite_tAFF74BC83CD68037494CB0B4F28CBDF8971CAB99* L_117;
		L_117 = ProductionGameplayArt_Load_m59B222C07F5B485968D9DBC3A5A59136EE0AF1FC(_stringLiteral58EC50A9B0BB5302C846D8955B5ECDFBF7EC33CB, NULL);
		G_B22_0 = L_117;
		G_B22_1 = G_B20_0;
		goto IL_0351;
	}

IL_0347:
	{
		Sprite_tAFF74BC83CD68037494CB0B4F28CBDF8971CAB99* L_118;
		L_118 = PrototypeBoardArt_Load_mA923719A88D5F1CA2892B8F351D0D1E1C6E67237(_stringLiteralD8224EB348914F18B459158B0498D27694A7CD12, NULL);
		G_B22_0 = L_118;
		G_B22_1 = G_B21_0;
	}

IL_0351:
	{
		NullCheck(G_B22_1);
		G_B22_1->___burstSpecialSprite = G_B22_0;
		Il2CppCodeGenWriteBarrier((void**)(&G_B22_1->___burstSpecialSprite), (void*)G_B22_0);
		bool L_119;
		L_119 = Match3Board_get_UsesCurrentBoardArt_m18CE1C8EEE2834D494A8630E617E11C68F702508(__this, NULL);
		if (L_119)
		{
			G_B24_0 = __this;
			goto IL_036b;
		}
		G_B23_0 = __this;
	}
	{
		il2cpp_codegen_runtime_class_init_inline(ProductionGameplayArt_tAC6E14B3903B3A1CA5CB5F45C61BC57B8D200E75_il2cpp_TypeInfo_var);
		Sprite_tAFF74BC83CD68037494CB0B4F28CBDF8971CAB99* L_120;
		L_120 = ProductionGameplayArt_Load_m59B222C07F5B485968D9DBC3A5A59136EE0AF1FC(_stringLiteralDD0F04E0A3F0BA1F904E0E27720F367F4228C69D, NULL);
		G_B25_0 = L_120;
		G_B25_1 = G_B23_0;
		goto IL_0375;
	}

IL_036b:
	{
		Sprite_tAFF74BC83CD68037494CB0B4F28CBDF8971CAB99* L_121;
		L_121 = PrototypeBoardArt_Load_mA923719A88D5F1CA2892B8F351D0D1E1C6E67237(_stringLiteralDD0F04E0A3F0BA1F904E0E27720F367F4228C69D, NULL);
		G_B25_0 = L_121;
		G_B25_1 = G_B24_0;
	}

IL_0375:
	{
		NullCheck(G_B25_1);
		G_B25_1->___colorSpecialSprite = G_B25_0;
		Il2CppCodeGenWriteBarrier((void**)(&G_B25_1->___colorSpecialSprite), (void*)G_B25_0);
		Match3Board_BuildGrid_mC6079479E2BBB5447C1DB91568A472303F10F396(__this, NULL);
		CoreLoopVfxPresenter_tE910CE8C54823137C9B728BDF5D4AF1F974B2629* L_122;
		L_122 = Component_GetComponent_TisCoreLoopVfxPresenter_tE910CE8C54823137C9B728BDF5D4AF1F974B2629_m549830C808CE5EEBF5BBCFA22829828568B956C4(__this, Component_GetComponent_TisCoreLoopVfxPresenter_tE910CE8C54823137C9B728BDF5D4AF1F974B2629_m549830C808CE5EEBF5BBCFA22829828568B956C4_RuntimeMethod_var);
		__this->___vfxPresenter = L_122;
		Il2CppCodeGenWriteBarrier((void**)(&__this->___vfxPresenter), (void*)L_122);
		CoreLoopVfxPresenter_tE910CE8C54823137C9B728BDF5D4AF1F974B2629* L_123 = __this->___vfxPresenter;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_124;
		L_124 = Object_op_Equality_mB6120F782D83091EF56A198FCEBCF066DB4A9605(L_123, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (!L_124)
		{
			goto IL_03ab;
		}
	}
	{
		GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_125;
		L_125 = Component_get_gameObject_m57AEFBB14DB39EC476F740BA000E170355DE691B(__this, NULL);
		NullCheck(L_125);
		CoreLoopVfxPresenter_tE910CE8C54823137C9B728BDF5D4AF1F974B2629* L_126;
		L_126 = GameObject_AddComponent_TisCoreLoopVfxPresenter_tE910CE8C54823137C9B728BDF5D4AF1F974B2629_m7B3528003C74D82B4D9697BDB5BAF50E81C8C945(L_125, GameObject_AddComponent_TisCoreLoopVfxPresenter_tE910CE8C54823137C9B728BDF5D4AF1F974B2629_m7B3528003C74D82B4D9697BDB5BAF50E81C8C945_RuntimeMethod_var);
		__this->___vfxPresenter = L_126;
		Il2CppCodeGenWriteBarrier((void**)(&__this->___vfxPresenter), (void*)L_126);
	}

IL_03ab:
	{
		CoreLoopVfxPresenter_tE910CE8C54823137C9B728BDF5D4AF1F974B2629* L_127 = __this->___vfxPresenter;
		NullCheck(L_127);
		CoreLoopVfxPresenter_Initialize_m5FD4D522DC7C3F0D438552B3BBD75A241E50DBF1(L_127, __this, NULL);
		BoardModel_tCD704F6305D85C7E64A4F76AF732A1FE2BC63601* L_128 = __this->___model;
		int32_t L_129 = __this->___ruleVersion;
		BoardResolutionPlanner_t406B06C1345DBB98BCEEC09079B44161EC11584E* L_130 = (BoardResolutionPlanner_t406B06C1345DBB98BCEEC09079B44161EC11584E*)il2cpp_codegen_object_new(BoardResolutionPlanner_t406B06C1345DBB98BCEEC09079B44161EC11584E_il2cpp_TypeInfo_var);
		BoardResolutionPlanner__ctor_mB912B3C51EA1C0CB897B4DAE52231FEB710AC9F1(L_130, L_128, (BoardBalanceProfile_t256D2E27898B041EF0DD0362EB0D091F1781A038*)NULL, L_129, NULL);
		__this->___resolutionPlanner = L_130;
		Il2CppCodeGenWriteBarrier((void**)(&__this->___resolutionPlanner), (void*)L_130);
		LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* L_131 = __this->___currentLevel;
		NullCheck(L_131);
		String_t* L_132 = L_131->___id;
		int32_t L_133 = V_0;
		int32_t L_134 = L_133;
		RuntimeObject* L_135 = Box(il2cpp_defaults.int32_class, &L_134);
		String_t* L_136;
		L_136 = String_Format_mFB7DA489BD99F4670881FF50EC017BFB0A5C0987(_stringLiteralDCC83F93DF4DF808F41773846742D1857B5FA601, L_132, L_135, NULL);
		int32_t L_137 = __this->___columns;
		int32_t L_138 = L_137;
		RuntimeObject* L_139 = Box(il2cpp_defaults.int32_class, &L_138);
		int32_t L_140 = __this->___rows;
		int32_t L_141 = L_140;
		RuntimeObject* L_142 = Box(il2cpp_defaults.int32_class, &L_141);
		int32_t L_143 = V_1;
		int32_t L_144 = L_143;
		RuntimeObject* L_145 = Box(il2cpp_defaults.int32_class, &L_144);
		String_t* L_146;
		L_146 = String_Format_mA0534D6E2AE4D67A6BD8D45B3321323930EB930C(_stringLiteral22795E9DF10F963C56196B72FCB81B192C3EA4D1, L_139, L_142, L_145, NULL);
		BoardModel_tCD704F6305D85C7E64A4F76AF732A1FE2BC63601* L_147 = __this->___model;
		NullCheck(L_147);
		String_t* L_148;
		L_148 = BoardModel_ComputeStateHash_m0C5A34304940F1672704BEEA01C59D508BA8E41B(L_147, NULL);
		String_t* L_149;
		L_149 = String_Concat_m093934F71A9B351911EE46311674ED463B180006(L_136, L_146, _stringLiteral6E4FCD87E88F2704AB463530C70DBCB3D116A875, L_148, NULL);
		il2cpp_codegen_runtime_class_init_inline(Debug_t8394C7EEAECA3689C2C9B9DE9C7166D73596276F_il2cpp_TypeInfo_var);
		Debug_Log_m87A9A3C761FF5C43ED8A53B16190A53D08F818BB(L_149, NULL);
		Match3Board_RefreshAll_m96F8E8431C0DD822FDB5F3B777748391CA8922B0(__this, NULL);
		int32_t L_150 = __this->___ruleVersion;
		if ((((int32_t)L_150) == ((int32_t)1)))
		{
			goto IL_0458;
		}
	}
	{
		il2cpp_codegen_runtime_class_init_inline(GemShatterView_t80F06E6F29BAD82083095696D19D791F890B3DF6_il2cpp_TypeInfo_var);
		GemShatterView_Attach_m81F36FAFC4ECDDC2174CB7AA5C0C5D822F56E385(__this, NULL);
		il2cpp_codegen_runtime_class_init_inline(PrototypeImpactView_tE25D72CAD44A6824CF1D457FD47AA30A1F2EF60D_il2cpp_TypeInfo_var);
		PrototypeImpactView_tE25D72CAD44A6824CF1D457FD47AA30A1F2EF60D* L_151;
		L_151 = PrototypeImpactView_Attach_m9C1DB598F7B8B94588B366082CFBD9552E9B3335(__this, NULL);
		PrototypeBoosterView_Attach_mA34F3068575858E5DD2E7A74B677DE1352A537A2(__this, NULL);
		PrototypeBoardAudio_t71A170AB22BE68893967F777162916AEDD966DC1* L_152;
		L_152 = PrototypeBoardAudio_Attach_mEA97DA6BDEF72549C9513AD8E1A0D8C58F7314B2(__this, NULL);
		__this->___mechanicsAudio = L_152;
		Il2CppCodeGenWriteBarrier((void**)(&__this->___mechanicsAudio), (void*)L_152);
	}

IL_0458:
	{
		int32_t L_153 = __this->___ruleVersion;
		if ((((int32_t)L_153) < ((int32_t)3)))
		{
			goto IL_0468;
		}
	}
	{
		SparkFlightView_t7B92AD22D3FD749058AD27F201E2F09CCCC43504* L_154;
		L_154 = SparkFlightView_Attach_mD04ED8B1A56F5A2507E5D01787AF4C0713BF6690(__this, NULL);
	}

IL_0468:
	{
		int32_t L_155 = __this->___ruleVersion;
		if ((((int32_t)L_155) < ((int32_t)5)))
		{
			goto IL_047e;
		}
	}
	{
		il2cpp_codegen_runtime_class_init_inline(MaterialBreakView_tDAFC40328458688CED885B9CD3829D721AAB4B32_il2cpp_TypeInfo_var);
		MaterialBreakView_tDAFC40328458688CED885B9CD3829D721AAB4B32* L_156;
		L_156 = MaterialBreakView_Attach_m465341320E640F99036193217EB8B452FA5099BB(__this, NULL);
		SpecialSequenceView_Attach_m2503CD61B3F02143E754C2ABB28EBBF395C527EC(__this, NULL);
	}

IL_047e:
	{
		Match3Board_PublishState_m888BDD9A14D75FA1915246672C1F7B94F83580FF(__this, NULL);
		return;
	}
}
