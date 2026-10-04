IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Match3Board_RecoverActiveCommand_m7B67E8173409E5F4F2927D99EBBF7EBDFF2E4B67 (Match3Board_tE4CAA58AF06191DC578D60DE5FA7659B8C8ACD69* __this, String_t* ___0_reason, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Debug_t8394C7EEAECA3689C2C9B9DE9C7166D73596276F_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ICampaignBoardInventory_tE3818DEC91979E54420832962522A249095E28AF_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Nullable_1_get_HasValue_m4651E27BB4CB970BCB109D66BDD7B0B131AFFA92_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Nullable_1_get_Value_m6B6CD4D63B49EBA7D4F0F6C61E9566E75143F7A0_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&_stringLiteral3128911948D9784A0895ED6AF83D88434BA902D4);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&_stringLiteralAB2D97924FF679FBF2660FF488E1ED83263078B1);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&_stringLiteralB3E77C1635BBBAD917F20480EB9B12C87D9C1D1C);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&_stringLiteralE0C6DCA56CAAB38F06A2409492ED813C77CDFBBA);
		s_Il2CppMethodInitialized = true;
	}
	RuntimeCommandTransaction_tF69C17DAF81E9F0EAAEEF603E293B533B97AC945* V_0 = NULL;
	int32_t V_1 = 0;
	Nullable_1_t9CE92CE712BC5CC12E66AF5FBC540402BF567C73 V_2;
	memset((&V_2), 0, sizeof(V_2));
	RuntimeObject* G_B10_0 = NULL;
	RuntimeObject* G_B9_0 = NULL;
	{
		bool L_0 = __this->___recoveringCommand;
		if (L_0)
		{
			goto IL_0010;
		}
	}
	{
		RuntimeCommandTransaction_tF69C17DAF81E9F0EAAEEF603E293B533B97AC945* L_1 = __this->___activeCommand;
		if (L_1)
		{
			goto IL_0011;
		}
	}

IL_0010:
	{
		return;
	}

IL_0011:
	{
		__this->___recoveringCommand = (bool)1;
		RuntimeCommandTransaction_tF69C17DAF81E9F0EAAEEF603E293B533B97AC945* L_2 = __this->___activeCommand;
		V_0 = L_2;
		RuntimeCommandTransaction_tF69C17DAF81E9F0EAAEEF603E293B533B97AC945* L_3 = V_0;
		NullCheck(L_3);
		String_t* L_4;
		L_4 = RuntimeCommandTransaction_get_Label_mAA9C72AEE5F924A8C5DDF7378CEAF55E1EAE8DB4_inline(L_3, NULL);
		RuntimeCommandTransaction_tF69C17DAF81E9F0EAAEEF603E293B533B97AC945* L_5 = V_0;
		NullCheck(L_5);
		Nullable_1_t9CE92CE712BC5CC12E66AF5FBC540402BF567C73 L_6;
		L_6 = RuntimeCommandTransaction_get_ConsumedBooster_mF36C3846051C768498BE76E7C59A49C2AABC0B2A_inline(L_5, NULL);
		int32_t L_7;
		L_7 = Match3Board_ToGameplayBooster_m7ECF703BBE8B09C51471F096EF460C8F9745E659(L_4, L_6, NULL);
		V_1 = L_7;
		MonoBehaviour_StopAllCoroutines_m872033451D42013A99867D09337490017E9ED318(__this, NULL);
		Match3Board_ClearTransientEffects_m228B13DB121B99025E2625D2A1E56CC9C6F7CD90(__this, NULL);
		Match3Board_ResetPointerOwnership_mE164A708FE82EBDF87F682C9E186D6C39669D9A9(__this, NULL);
		BoardCommandWatchdog_tFB774DA752FC7B2B47E51FCF907C3ABDC7C326A7* L_8 = __this->___commandWatchdog;
		NullCheck(L_8);
		BoardCommandWatchdog_Cancel_m3582C83DE397F3F2413E5806D8DDECE313010AB0(L_8, NULL);
		BoardModel_tCD704F6305D85C7E64A4F76AF732A1FE2BC63601* L_9 = __this->___model;
		RuntimeCommandTransaction_tF69C17DAF81E9F0EAAEEF603E293B533B97AC945* L_10 = V_0;
		NullCheck(L_10);
		BoardSnapshot_t689B87BDB1373020E2758049B806473BB7F89308* L_11;
		L_11 = RuntimeCommandTransaction_get_BeforeSnapshot_mEA8BA7C718E395DD05FC6C8B48DEA4C2139C7FBE_inline(L_10, NULL);
		NullCheck(L_9);
		BoardModel_RestoreSnapshot_m717E15E29830990BF0E8DAFB34CFD6E0992C67A7(L_9, L_11, NULL);
		RuntimeCommandTransaction_tF69C17DAF81E9F0EAAEEF603E293B533B97AC945* L_12 = V_0;
		NullCheck(L_12);
		int32_t L_13;
		L_13 = RuntimeCommandTransaction_get_MovesBefore_m9879BEB74536ADCA906DC0A2BFFFD83A68160F68_inline(L_12, NULL);
		__this->___moves = L_13;
		RuntimeCommandTransaction_tF69C17DAF81E9F0EAAEEF603E293B533B97AC945* L_14 = V_0;
		NullCheck(L_14);
		int32_t L_15;
		L_15 = RuntimeCommandTransaction_get_ScoreBefore_mB4C9CC8105C31B00349690785C1BF8F03774C3A4_inline(L_14, NULL);
		__this->___score = L_15;
		LevelGoalRuntime_tE6D112739A0BF1C24963E7792D1AFDBD02897B9B* L_16 = __this->___goalRuntime;
		if (!L_16)
		{
			goto IL_0098;
		}
	}
	{
		RuntimeCommandTransaction_tF69C17DAF81E9F0EAAEEF603E293B533B97AC945* L_17 = V_0;
		NullCheck(L_17);
		LevelGoalRuntimeSnapshot_t24F2DAA1BB402B7D65674674F9EB6E4C816F76EE* L_18;
		L_18 = RuntimeCommandTransaction_get_GoalsBefore_mAC8EB05A47447ADB1F44CD434BFF00CD12F32723_inline(L_17, NULL);
		if (!L_18)
		{
			goto IL_0098;
		}
	}
	{
		LevelGoalRuntime_tE6D112739A0BF1C24963E7792D1AFDBD02897B9B* L_19 = __this->___goalRuntime;
		RuntimeCommandTransaction_tF69C17DAF81E9F0EAAEEF603E293B533B97AC945* L_20 = V_0;
		NullCheck(L_20);
		LevelGoalRuntimeSnapshot_t24F2DAA1BB402B7D65674674F9EB6E4C816F76EE* L_21;
		L_21 = RuntimeCommandTransaction_get_GoalsBefore_mAC8EB05A47447ADB1F44CD434BFF00CD12F32723_inline(L_20, NULL);
		NullCheck(L_19);
		LevelGoalRuntime_RestoreSnapshot_mE9E8A0BFBC6F9629CD501FD48AD5F9EF77B62CFE(L_19, L_21, NULL);
	}

IL_0098:
	{
		bool L_22 = __this->___campaignManaged;
		if (!L_22)
		{
			goto IL_00c2;
		}
	}
	{
		RuntimeCommandTransaction_tF69C17DAF81E9F0EAAEEF603E293B533B97AC945* L_23 = V_0;
		NullCheck(L_23);
		Nullable_1_t9CE92CE712BC5CC12E66AF5FBC540402BF567C73 L_24;
		L_24 = RuntimeCommandTransaction_get_ConsumedBooster_mF36C3846051C768498BE76E7C59A49C2AABC0B2A_inline(L_23, NULL);
		V_2 = L_24;
		bool L_25;
		L_25 = Nullable_1_get_HasValue_m4651E27BB4CB970BCB109D66BDD7B0B131AFFA92_inline((&V_2), Nullable_1_get_HasValue_m4651E27BB4CB970BCB109D66BDD7B0B131AFFA92_RuntimeMethod_var);
		if (!L_25)
		{
			goto IL_00c2;
		}
	}
	{
		RuntimeObject* L_26 = __this->___campaignInventory;
		RuntimeObject* L_27 = L_26;
		if (L_27)
		{
			G_B10_0 = L_27;
			goto IL_00bc;
		}
		G_B9_0 = L_27;
	}
	{
		goto IL_00c2;
	}

IL_00bc:
	{
		NullCheck(G_B10_0);
		bool L_28;
		L_28 = InterfaceFuncInvoker0< bool >::Invoke(3, ICampaignBoardInventory_tE3818DEC91979E54420832962522A249095E28AF_il2cpp_TypeInfo_var, G_B10_0);
	}

IL_00c2:
	{
		bool L_29 = __this->___campaignManaged;
		if (L_29)
		{
			goto IL_0100;
		}
	}
	{
		RuntimeCommandTransaction_tF69C17DAF81E9F0EAAEEF603E293B533B97AC945* L_30 = V_0;
		NullCheck(L_30);
		Nullable_1_t9CE92CE712BC5CC12E66AF5FBC540402BF567C73 L_31;
		L_31 = RuntimeCommandTransaction_get_ConsumedBooster_mF36C3846051C768498BE76E7C59A49C2AABC0B2A_inline(L_30, NULL);
		V_2 = L_31;
		bool L_32;
		L_32 = Nullable_1_get_HasValue_m4651E27BB4CB970BCB109D66BDD7B0B131AFFA92_inline((&V_2), Nullable_1_get_HasValue_m4651E27BB4CB970BCB109D66BDD7B0B131AFFA92_RuntimeMethod_var);
		if (!L_32)
		{
			goto IL_0100;
		}
	}
	{
		GameState_t5E590D2FB317152A4011A3CA69E3F311E0A7393A* L_33;
		L_33 = GameState_get_Instance_m3202061BC5E0A39B53697C35D0BEE15692173D74_inline(NULL);
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_34;
		L_34 = Object_op_Inequality_mD0BE578448EAA61948F25C32F8DD55AB1F778602(L_33, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (!L_34)
		{
			goto IL_0100;
		}
	}
	{
		GameState_t5E590D2FB317152A4011A3CA69E3F311E0A7393A* L_35;
		L_35 = GameState_get_Instance_m3202061BC5E0A39B53697C35D0BEE15692173D74_inline(NULL);
		RuntimeCommandTransaction_tF69C17DAF81E9F0EAAEEF603E293B533B97AC945* L_36 = V_0;
		NullCheck(L_36);
		Nullable_1_t9CE92CE712BC5CC12E66AF5FBC540402BF567C73 L_37;
		L_37 = RuntimeCommandTransaction_get_ConsumedBooster_mF36C3846051C768498BE76E7C59A49C2AABC0B2A_inline(L_36, NULL);
		V_2 = L_37;
		int32_t L_38;
		L_38 = Nullable_1_get_Value_m6B6CD4D63B49EBA7D4F0F6C61E9566E75143F7A0((&V_2), Nullable_1_get_Value_m6B6CD4D63B49EBA7D4F0F6C61E9566E75143F7A0_RuntimeMethod_var);
		NullCheck(L_35);
		GameState_AddBooster_mB7C8861557F0E49F47BE2CF8A1B4DABE78497034(L_35, L_38, 1, NULL);
	}

IL_0100:
	{
		Nullable_1_t9CE92CE712BC5CC12E66AF5FBC540402BF567C73* L_39 = (Nullable_1_t9CE92CE712BC5CC12E66AF5FBC540402BF567C73*)(&__this->___armedBooster);
		il2cpp_codegen_initobj(L_39, sizeof(Nullable_1_t9CE92CE712BC5CC12E66AF5FBC540402BF567C73));
		Vector2Int_t69B2886EBAB732D9B880565E18E7568F3DE0CE6A L_40;
		memset((&L_40), 0, sizeof(L_40));
		Vector2Int__ctor_mC20D1312133EB8CB63EC11067088B043660F11CE_inline((&L_40), (-1), (-1), NULL);
		__this->___selected = L_40;
		__this->___activeCommand = (RuntimeCommandTransaction_tF69C17DAF81E9F0EAAEEF603E293B533B97AC945*)NULL;
		Il2CppCodeGenWriteBarrier((void**)(&__this->___activeCommand), (void*)(RuntimeCommandTransaction_tF69C17DAF81E9F0EAAEEF603E293B533B97AC945*)NULL);
		Match3Board_ResetVisualTransforms_m1C81B135CA6B9E83BF69F548502AFC95535F22F2(__this, NULL);
		Match3Board_RefreshAll_m96F8E8431C0DD822FDB5F3B777748391CA8922B0(__this, NULL);
		GameplayStateMachine_t01C75AC2E3E92D6A53E4B80363DB5C26D85DA0CA* L_41 = __this->___interaction;
		NullCheck(L_41);
		GameplayStateMachine_Force_m611EBC14C71224E77DCEC80729A25636364BD1AD(L_41, 0, NULL);
		Match3Board_PublishState_m888BDD9A14D75FA1915246672C1F7B94F83580FF(__this, NULL);
		__this->___lastWaveVerificationPassed = (bool)0;
		Vector2Int_t69B2886EBAB732D9B880565E18E7568F3DE0CE6A L_42;
		memset((&L_42), 0, sizeof(L_42));
		Vector2Int__ctor_mC20D1312133EB8CB63EC11067088B043660F11CE_inline((&L_42), 0, 0, NULL);
		Vector2Int_t69B2886EBAB732D9B880565E18E7568F3DE0CE6A L_43;
		memset((&L_43), 0, sizeof(L_43));
		Vector2Int__ctor_mC20D1312133EB8CB63EC11067088B043660F11CE_inline((&L_43), 0, 0, NULL);
		String_t* L_44 = __this->___lastResolutionPlanHash;
		int32_t L_45 = V_1;
		Match3Board_PublishMotion_mCD47F32AAFA9F0EB01223DEF130ABB1FA2387D16(__this, ((int32_t)14), L_42, L_43, 0, 0, 0, L_44, 0, L_45, (RuntimeObject*)NULL, 0, NULL);
		StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248* L_46 = (StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248*)(StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248*)SZArrayNew(StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248_il2cpp_TypeInfo_var, (uint32_t)6);
		StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248* L_47 = L_46;
		NullCheck(L_47);
		(L_47)->SetAt(static_cast<il2cpp_array_size_t>(0), (String_t*)_stringLiteralE0C6DCA56CAAB38F06A2409492ED813C77CDFBBA);
		StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248* L_48 = L_47;
		RuntimeCommandTransaction_tF69C17DAF81E9F0EAAEEF603E293B533B97AC945* L_49 = V_0;
		NullCheck(L_49);
		String_t* L_50;
		L_50 = RuntimeCommandTransaction_get_Label_mAA9C72AEE5F924A8C5DDF7378CEAF55E1EAE8DB4_inline(L_49, NULL);
		NullCheck(L_48);
		(L_48)->SetAt(static_cast<il2cpp_array_size_t>(1), (String_t*)L_50);
		StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248* L_51 = L_48;
		NullCheck(L_51);
		(L_51)->SetAt(static_cast<il2cpp_array_size_t>(2), (String_t*)_stringLiteralB3E77C1635BBBAD917F20480EB9B12C87D9C1D1C);
		StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248* L_52 = L_51;
		String_t* L_53 = ___0_reason;
		NullCheck(L_52);
		(L_52)->SetAt(static_cast<il2cpp_array_size_t>(3), (String_t*)L_53);
		StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248* L_54 = L_52;
		NullCheck(L_54);
		(L_54)->SetAt(static_cast<il2cpp_array_size_t>(4), (String_t*)_stringLiteral3128911948D9784A0895ED6AF83D88434BA902D4);
		StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248* L_55 = L_54;
		BoardModel_tCD704F6305D85C7E64A4F76AF732A1FE2BC63601* L_56 = __this->___model;
		NullCheck(L_56);
		String_t* L_57;
		L_57 = BoardModel_ComputeStateHash_m0C5A34304940F1672704BEEA01C59D508BA8E41B(L_56, NULL);
		NullCheck(L_55);
		(L_55)->SetAt(static_cast<il2cpp_array_size_t>(5), (String_t*)L_57);
		String_t* L_58;
		L_58 = String_Concat_m647EBF831F54B6DF7D5AFA5FD012CF4EE7571B6A(L_55, NULL);
		il2cpp_codegen_runtime_class_init_inline(Debug_t8394C7EEAECA3689C2C9B9DE9C7166D73596276F_il2cpp_TypeInfo_var);
		Debug_LogError_mB00B2B4468EF3CAF041B038D840820FB84C924B2(L_58, NULL);
		Match3Board_ShowLegacyToast_m05CDA16010AB018CD8760C2CA7E214C526ADFFEF(__this, _stringLiteralAB2D97924FF679FBF2660FF488E1ED83263078B1, NULL);
		__this->___recoveringCommand = (bool)0;
		return;
	}
}
