IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Match3Board_RecoverActiveCommand_m7B67E8173409E5F4F2927D99EBBF7EBDFF2E4B67 (Match3Board_tE4CAA58AF06191DC578D60DE5FA7659B8C8ACD69* __this, String_t* ___0_reason, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Debug_t8394C7EEAECA3689C2C9B9DE9C7166D73596276F_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Match3Board_SafeInvoke_TisMatch3Board_tE4CAA58AF06191DC578D60DE5FA7659B8C8ACD69_m94B924C26D384CC6D0F5219D7E5EFC8F0DE96750_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&_stringLiteral1168E92C164109D6220480DEDA987085B2A21155);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&_stringLiteral3128911948D9784A0895ED6AF83D88434BA902D4);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&_stringLiteral83E13C3BCE04533E097617114A91007CDB2F5458);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&_stringLiteralAB2D97924FF679FBF2660FF488E1ED83263078B1);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&_stringLiteralAF95DB0C574EF7DCA44A6C0A6F8A60C7B25B55B5);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&_stringLiteralB3E77C1635BBBAD917F20480EB9B12C87D9C1D1C);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&_stringLiteralBD23895BED333CA613D5D5F5328C46CEE809E315);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&_stringLiteralED0EE957BFBF8265ED3CF3F60E554838D0A77FDB);
		s_Il2CppMethodInitialized = true;
	}
	RuntimeCommandTransaction_tF69C17DAF81E9F0EAAEEF603E293B533B97AC945* V_0 = NULL;
	int32_t V_1 = 0;
	bool V_2 = false;
	bool V_3 = false;
	int32_t G_B13_0 = 0;
	GameplayStateMachine_t01C75AC2E3E92D6A53E4B80363DB5C26D85DA0CA* G_B15_0 = NULL;
	GameplayStateMachine_t01C75AC2E3E92D6A53E4B80363DB5C26D85DA0CA* G_B14_0 = NULL;
	int32_t G_B16_0 = 0;
	GameplayStateMachine_t01C75AC2E3E92D6A53E4B80363DB5C26D85DA0CA* G_B16_1 = NULL;
	int32_t G_B20_0 = 0;
	StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248* G_B20_1 = NULL;
	StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248* G_B20_2 = NULL;
	int32_t G_B19_0 = 0;
	StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248* G_B19_1 = NULL;
	StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248* G_B19_2 = NULL;
	String_t* G_B21_0 = NULL;
	int32_t G_B21_1 = 0;
	StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248* G_B21_2 = NULL;
	StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248* G_B21_3 = NULL;
	{
		bool L_0 = __this->___recoveringCommand;
		if (L_0)
		{
			goto IL_0018;
		}
	}
	{
		bool L_1 = __this->___refundPending;
		if (L_1)
		{
			goto IL_0018;
		}
	}
	{
		RuntimeCommandTransaction_tF69C17DAF81E9F0EAAEEF603E293B533B97AC945* L_2 = __this->___activeCommand;
		if (L_2)
		{
			goto IL_0019;
		}
	}

IL_0018:
	{
		return;
	}

IL_0019:
	{
		__this->___recoveringCommand = (bool)1;
		RuntimeCommandTransaction_tF69C17DAF81E9F0EAAEEF603E293B533B97AC945* L_3 = __this->___activeCommand;
		V_0 = L_3;
		RuntimeCommandTransaction_tF69C17DAF81E9F0EAAEEF603E293B533B97AC945* L_4 = V_0;
		NullCheck(L_4);
		String_t* L_5;
		L_5 = RuntimeCommandTransaction_get_Label_mAA9C72AEE5F924A8C5DDF7378CEAF55E1EAE8DB4_inline(L_4, NULL);
		RuntimeCommandTransaction_tF69C17DAF81E9F0EAAEEF603E293B533B97AC945* L_6 = V_0;
		NullCheck(L_6);
		Nullable_1_t9CE92CE712BC5CC12E66AF5FBC540402BF567C73 L_7;
		L_7 = RuntimeCommandTransaction_get_ConsumedBooster_mF36C3846051C768498BE76E7C59A49C2AABC0B2A_inline(L_6, NULL);
		int32_t L_8;
		L_8 = Match3Board_ToGameplayBooster_m7ECF703BBE8B09C51471F096EF460C8F9745E659(L_5, L_7, NULL);
		V_1 = L_8;
		MonoBehaviour_StopAllCoroutines_m872033451D42013A99867D09337490017E9ED318(__this, NULL);
		Match3Board_ClearTransientEffects_m228B13DB121B99025E2625D2A1E56CC9C6F7CD90(__this, NULL);
		Match3Board_ResetPointerOwnership_mE164A708FE82EBDF87F682C9E186D6C39669D9A9(__this, NULL);
		BoardCommandWatchdog_tFB774DA752FC7B2B47E51FCF907C3ABDC7C326A7* L_9 = __this->___commandWatchdog;
		NullCheck(L_9);
		BoardCommandWatchdog_Cancel_m3582C83DE397F3F2413E5806D8DDECE313010AB0(L_9, NULL);
		BoardModel_tCD704F6305D85C7E64A4F76AF732A1FE2BC63601* L_10 = __this->___model;
		RuntimeCommandTransaction_tF69C17DAF81E9F0EAAEEF603E293B533B97AC945* L_11 = V_0;
		NullCheck(L_11);
		BoardSnapshot_t689B87BDB1373020E2758049B806473BB7F89308* L_12;
		L_12 = RuntimeCommandTransaction_get_BeforeSnapshot_mEA8BA7C718E395DD05FC6C8B48DEA4C2139C7FBE_inline(L_11, NULL);
		NullCheck(L_10);
		BoardModel_RestoreSnapshot_m717E15E29830990BF0E8DAFB34CFD6E0992C67A7(L_10, L_12, NULL);
		RuntimeCommandTransaction_tF69C17DAF81E9F0EAAEEF603E293B533B97AC945* L_13 = V_0;
		NullCheck(L_13);
		int32_t L_14;
		L_14 = RuntimeCommandTransaction_get_MovesBefore_m9879BEB74536ADCA906DC0A2BFFFD83A68160F68_inline(L_13, NULL);
		__this->___moves = L_14;
		RuntimeCommandTransaction_tF69C17DAF81E9F0EAAEEF603E293B533B97AC945* L_15 = V_0;
		NullCheck(L_15);
		int32_t L_16;
		L_16 = RuntimeCommandTransaction_get_ScoreBefore_mB4C9CC8105C31B00349690785C1BF8F03774C3A4_inline(L_15, NULL);
		__this->___score = L_16;
		LevelGoalRuntime_tE6D112739A0BF1C24963E7792D1AFDBD02897B9B* L_17 = __this->___goalRuntime;
		if (!L_17)
		{
			goto IL_00a0;
		}
	}
	{
		RuntimeCommandTransaction_tF69C17DAF81E9F0EAAEEF603E293B533B97AC945* L_18 = V_0;
		NullCheck(L_18);
		LevelGoalRuntimeSnapshot_t24F2DAA1BB402B7D65674674F9EB6E4C816F76EE* L_19;
		L_19 = RuntimeCommandTransaction_get_GoalsBefore_mAC8EB05A47447ADB1F44CD434BFF00CD12F32723_inline(L_18, NULL);
		if (!L_19)
		{
			goto IL_00a0;
		}
	}
	{
		LevelGoalRuntime_tE6D112739A0BF1C24963E7792D1AFDBD02897B9B* L_20 = __this->___goalRuntime;
		RuntimeCommandTransaction_tF69C17DAF81E9F0EAAEEF603E293B533B97AC945* L_21 = V_0;
		NullCheck(L_21);
		LevelGoalRuntimeSnapshot_t24F2DAA1BB402B7D65674674F9EB6E4C816F76EE* L_22;
		L_22 = RuntimeCommandTransaction_get_GoalsBefore_mAC8EB05A47447ADB1F44CD434BFF00CD12F32723_inline(L_21, NULL);
		NullCheck(L_20);
		LevelGoalRuntime_RestoreSnapshot_mE9E8A0BFBC6F9629CD501FD48AD5F9EF77B62CFE(L_20, L_22, NULL);
	}

IL_00a0:
	{
		RuntimeCommandTransaction_tF69C17DAF81E9F0EAAEEF603E293B533B97AC945* L_23 = V_0;
		bool L_24;
		L_24 = Match3Board_RefundCommand_mDC69071AE976F66ECC89D7949220F397689B1039(__this, L_23, NULL);
		V_2 = L_24;
		bool L_25 = V_2;
		__this->___refundPending = (bool)((((int32_t)L_25) == ((int32_t)0))? 1 : 0);
		Nullable_1_t9CE92CE712BC5CC12E66AF5FBC540402BF567C73* L_26 = (Nullable_1_t9CE92CE712BC5CC12E66AF5FBC540402BF567C73*)(&__this->___armedBooster);
		il2cpp_codegen_initobj(L_26, sizeof(Nullable_1_t9CE92CE712BC5CC12E66AF5FBC540402BF567C73));
		Vector2Int_t69B2886EBAB732D9B880565E18E7568F3DE0CE6A L_27;
		memset((&L_27), 0, sizeof(L_27));
		Vector2Int__ctor_mC20D1312133EB8CB63EC11067088B043660F11CE_inline((&L_27), (-1), (-1), NULL);
		__this->___selected = L_27;
		bool L_28 = V_2;
		if (!L_28)
		{
			goto IL_00d5;
		}
	}
	{
		__this->___activeCommand = (RuntimeCommandTransaction_tF69C17DAF81E9F0EAAEEF603E293B533B97AC945*)NULL;
		Il2CppCodeGenWriteBarrier((void**)(&__this->___activeCommand), (void*)(RuntimeCommandTransaction_tF69C17DAF81E9F0EAAEEF603E293B533B97AC945*)NULL);
	}

IL_00d5:
	{
		Match3Board_ResetVisualTransforms_m1C81B135CA6B9E83BF69F548502AFC95535F22F2(__this, NULL);
		Match3Board_RefreshAll_m96F8E8431C0DD822FDB5F3B777748391CA8922B0(__this, NULL);
		bool L_29 = V_2;
		if (!L_29)
		{
			goto IL_00f7;
		}
	}
	{
		bool L_30;
		L_30 = Match3Board_get_HasInventoryRecoveryError_m2F77ADD473EBFDA4F38F30D9CAB956220AC2A846(__this, NULL);
		if (L_30)
		{
			goto IL_00f7;
		}
	}
	{
		bool L_31;
		L_31 = Match3Board_get_CampaignInventoryBlocked_mBD9371C16DFC74E4E255168688003BBFE0D0B563(__this, NULL);
		G_B13_0 = ((((int32_t)L_31) == ((int32_t)0))? 1 : 0);
		goto IL_00f8;
	}

IL_00f7:
	{
		G_B13_0 = 0;
	}

IL_00f8:
	{
		V_3 = (bool)G_B13_0;
		GameplayStateMachine_t01C75AC2E3E92D6A53E4B80363DB5C26D85DA0CA* L_32 = __this->___interaction;
		bool L_33 = V_3;
		if (L_33)
		{
			G_B15_0 = L_32;
			goto IL_0106;
		}
		G_B14_0 = L_32;
	}
	{
		G_B16_0 = ((int32_t)10);
		G_B16_1 = G_B14_0;
		goto IL_0107;
	}

IL_0106:
	{
		G_B16_0 = 0;
		G_B16_1 = G_B15_0;
	}

IL_0107:
	{
		NullCheck(G_B16_1);
		GameplayStateMachine_Force_m611EBC14C71224E77DCEC80729A25636364BD1AD(G_B16_1, G_B16_0, NULL);
		Match3Board_PublishState_m888BDD9A14D75FA1915246672C1F7B94F83580FF(__this, NULL);
		__this->___lastWaveVerificationPassed = (bool)0;
		bool L_34 = V_2;
		if (!L_34)
		{
			goto IL_013f;
		}
	}
	{
		Vector2Int_t69B2886EBAB732D9B880565E18E7568F3DE0CE6A L_35;
		memset((&L_35), 0, sizeof(L_35));
		Vector2Int__ctor_mC20D1312133EB8CB63EC11067088B043660F11CE_inline((&L_35), 0, 0, NULL);
		Vector2Int_t69B2886EBAB732D9B880565E18E7568F3DE0CE6A L_36;
		memset((&L_36), 0, sizeof(L_36));
		Vector2Int__ctor_mC20D1312133EB8CB63EC11067088B043660F11CE_inline((&L_36), 0, 0, NULL);
		String_t* L_37 = __this->___lastResolutionPlanHash;
		int32_t L_38 = V_1;
		Match3Board_PublishMotion_mCD47F32AAFA9F0EB01223DEF130ABB1FA2387D16(__this, ((int32_t)14), L_35, L_36, 0, 0, 0, L_37, 0, L_38, (RuntimeObject*)NULL, 0, NULL);
	}

IL_013f:
	{
		StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248* L_39 = (StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248*)(StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248*)SZArrayNew(StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248_il2cpp_TypeInfo_var, (uint32_t)8);
		StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248* L_40 = L_39;
		NullCheck(L_40);
		(L_40)->SetAt(static_cast<il2cpp_array_size_t>(0), (String_t*)_stringLiteralAF95DB0C574EF7DCA44A6C0A6F8A60C7B25B55B5);
		StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248* L_41 = L_40;
		bool L_42 = V_2;
		if (L_42)
		{
			G_B20_0 = 1;
			G_B20_1 = L_41;
			G_B20_2 = L_41;
			goto IL_0159;
		}
		G_B19_0 = 1;
		G_B19_1 = L_41;
		G_B19_2 = L_41;
	}
	{
		G_B21_0 = _stringLiteralBD23895BED333CA613D5D5F5328C46CEE809E315;
		G_B21_1 = G_B19_0;
		G_B21_2 = G_B19_1;
		G_B21_3 = G_B19_2;
		goto IL_015e;
	}

IL_0159:
	{
		G_B21_0 = _stringLiteral83E13C3BCE04533E097617114A91007CDB2F5458;
		G_B21_1 = G_B20_0;
		G_B21_2 = G_B20_1;
		G_B21_3 = G_B20_2;
	}

IL_015e:
	{
		NullCheck(G_B21_2);
		(G_B21_2)->SetAt(static_cast<il2cpp_array_size_t>(G_B21_1), (String_t*)G_B21_0);
		StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248* L_43 = G_B21_3;
		NullCheck(L_43);
		(L_43)->SetAt(static_cast<il2cpp_array_size_t>(2), (String_t*)_stringLiteral1168E92C164109D6220480DEDA987085B2A21155);
		StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248* L_44 = L_43;
		RuntimeCommandTransaction_tF69C17DAF81E9F0EAAEEF603E293B533B97AC945* L_45 = V_0;
		NullCheck(L_45);
		String_t* L_46;
		L_46 = RuntimeCommandTransaction_get_Label_mAA9C72AEE5F924A8C5DDF7378CEAF55E1EAE8DB4_inline(L_45, NULL);
		NullCheck(L_44);
		(L_44)->SetAt(static_cast<il2cpp_array_size_t>(3), (String_t*)L_46);
		StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248* L_47 = L_44;
		NullCheck(L_47);
		(L_47)->SetAt(static_cast<il2cpp_array_size_t>(4), (String_t*)_stringLiteralB3E77C1635BBBAD917F20480EB9B12C87D9C1D1C);
		StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248* L_48 = L_47;
		String_t* L_49 = ___0_reason;
		NullCheck(L_48);
		(L_48)->SetAt(static_cast<il2cpp_array_size_t>(5), (String_t*)L_49);
		StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248* L_50 = L_48;
		NullCheck(L_50);
		(L_50)->SetAt(static_cast<il2cpp_array_size_t>(6), (String_t*)_stringLiteral3128911948D9784A0895ED6AF83D88434BA902D4);
		StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248* L_51 = L_50;
		BoardModel_tCD704F6305D85C7E64A4F76AF732A1FE2BC63601* L_52 = __this->___model;
		NullCheck(L_52);
		String_t* L_53;
		L_53 = BoardModel_ComputeStateHash_m0C5A34304940F1672704BEEA01C59D508BA8E41B(L_52, NULL);
		NullCheck(L_51);
		(L_51)->SetAt(static_cast<il2cpp_array_size_t>(7), (String_t*)L_53);
		String_t* L_54;
		L_54 = String_Concat_m647EBF831F54B6DF7D5AFA5FD012CF4EE7571B6A(L_51, NULL);
		il2cpp_codegen_runtime_class_init_inline(Debug_t8394C7EEAECA3689C2C9B9DE9C7166D73596276F_il2cpp_TypeInfo_var);
		Debug_LogError_mB00B2B4468EF3CAF041B038D840820FB84C924B2(L_54, NULL);
		bool L_55 = V_2;
		if (!L_55)
		{
			goto IL_01aa;
		}
	}
	{
		Match3Board_ShowLegacyToast_m05CDA16010AB018CD8760C2CA7E214C526ADFFEF(__this, _stringLiteralAB2D97924FF679FBF2660FF488E1ED83263078B1, NULL);
	}

IL_01aa:
	{
		bool L_56 = V_3;
		if (L_56)
		{
			goto IL_01bf;
		}
	}
	{
		Action_1_tA4479854036FBAC90E8CB938E1B8E34D745CAD74* L_57 = __this->___InventoryRecoveryRequired;
		Match3Board_SafeInvoke_TisMatch3Board_tE4CAA58AF06191DC578D60DE5FA7659B8C8ACD69_m94B924C26D384CC6D0F5219D7E5EFC8F0DE96750(__this, L_57, __this, _stringLiteralED0EE957BFBF8265ED3CF3F60E554838D0A77FDB, Match3Board_SafeInvoke_TisMatch3Board_tE4CAA58AF06191DC578D60DE5FA7659B8C8ACD69_m94B924C26D384CC6D0F5219D7E5EFC8F0DE96750_RuntimeMethod_var);
	}

IL_01bf:
	{
		__this->___recoveringCommand = (bool)0;
		return;
	}
}
