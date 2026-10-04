IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Match3Board_Initialize_m7DAF63C6BE4269C5A966EEA17F428FD81E92CFF8 (Match3Board_tE4CAA58AF06191DC578D60DE5FA7659B8C8ACD69* __this, LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* ___0_levelData, Action_1_tD69A6DC9FBE94131E52F5A73B2A9D4AB51EEC404* ___1_onWin, Action_1_tD69A6DC9FBE94131E52F5A73B2A9D4AB51EEC404* ___2_onFail, Nullable_1_tCF32C56A2641879C053C86F273C0C6EC1B40BC28 ___3_registeredAttempt, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Nullable_1_GetValueOrDefault_m8D130DB7F2A1E694736B449176F9C26DB456597B_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Nullable_1__ctor_m141FA88563AC0B5179132FB929EABD02C47FF703_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Nullable_1_get_HasValue_mCF2FD8B3055FA87FC9C504F2122B3B0FAEDE3EC9_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Nullable_1_get_Value_m0DF3B826A745419D26A168689AEB4BE6F8698ECA_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	Nullable_1_tCF32C56A2641879C053C86F273C0C6EC1B40BC28 V_0;
	memset((&V_0), 0, sizeof(V_0));
	int32_t V_1 = 0;
	Nullable_1_tCF32C56A2641879C053C86F273C0C6EC1B40BC28 V_2;
	memset((&V_2), 0, sizeof(V_2));
	LevelProgressData_tF4CF8BE6A46189E730098D0C3DEE22CAA57E0C9E* G_B8_0 = NULL;
	LevelProgressData_tF4CF8BE6A46189E730098D0C3DEE22CAA57E0C9E* G_B7_0 = NULL;
	Nullable_1_tCF32C56A2641879C053C86F273C0C6EC1B40BC28 G_B9_0;
	memset((&G_B9_0), 0, sizeof(G_B9_0));
	{
		__this->___ruleVersion = 5;
		__this->___useCurrentBoardArt = (bool)1;
		__this->___campaignInventory = (RuntimeObject*)NULL;
		Il2CppCodeGenWriteBarrier((void**)(&__this->___campaignInventory), (void*)(RuntimeObject*)NULL);
		bool L_0;
		L_0 = Nullable_1_get_HasValue_mCF2FD8B3055FA87FC9C504F2122B3B0FAEDE3EC9_inline((&___3_registeredAttempt), Nullable_1_get_HasValue_mCF2FD8B3055FA87FC9C504F2122B3B0FAEDE3EC9_RuntimeMethod_var);
		if (!L_0)
		{
			goto IL_0033;
		}
	}
	{
		int32_t L_1;
		L_1 = Nullable_1_get_Value_m0DF3B826A745419D26A168689AEB4BE6F8698ECA((&___3_registeredAttempt), Nullable_1_get_Value_m0DF3B826A745419D26A168689AEB4BE6F8698ECA_RuntimeMethod_var);
		if ((((int32_t)L_1) >= ((int32_t)1)))
		{
			goto IL_0033;
		}
	}
	{
		ArgumentException_tAD90411542A20A9C72D5CDA3A84181D8B947A263* L_2 = (ArgumentException_tAD90411542A20A9C72D5CDA3A84181D8B947A263*)il2cpp_codegen_object_new(((RuntimeClass*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&ArgumentException_tAD90411542A20A9C72D5CDA3A84181D8B947A263_il2cpp_TypeInfo_var)));
		ArgumentException__ctor_m026938A67AF9D36BB7ED27F80425D7194B514465(L_2, ((String_t*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&_stringLiteralA7520982369E4F6031FB37878DB79D47335384C7)), NULL);
		IL2CPP_RAISE_MANAGED_EXCEPTION(L_2, ((RuntimeMethod*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&Match3Board_Initialize_m7DAF63C6BE4269C5A966EEA17F428FD81E92CFF8_RuntimeMethod_var)));
	}

IL_0033:
	{
		bool L_3;
		L_3 = Nullable_1_get_HasValue_mCF2FD8B3055FA87FC9C504F2122B3B0FAEDE3EC9_inline((&___3_registeredAttempt), Nullable_1_get_HasValue_mCF2FD8B3055FA87FC9C504F2122B3B0FAEDE3EC9_RuntimeMethod_var);
		if (!L_3)
		{
			goto IL_00a4;
		}
	}
	{
		LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* L_4 = ___0_levelData;
		if (!L_4)
		{
			goto IL_0099;
		}
	}
	{
		GameState_t5E590D2FB317152A4011A3CA69E3F311E0A7393A* L_5;
		L_5 = GameState_get_Instance_m3202061BC5E0A39B53697C35D0BEE15692173D74_inline(NULL);
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_6;
		L_6 = Object_op_Equality_mB6120F782D83091EF56A198FCEBCF066DB4A9605(L_5, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (L_6)
		{
			goto IL_0099;
		}
	}
	{
		GameState_t5E590D2FB317152A4011A3CA69E3F311E0A7393A* L_7;
		L_7 = GameState_get_Instance_m3202061BC5E0A39B53697C35D0BEE15692173D74_inline(NULL);
		LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* L_8 = ___0_levelData;
		NullCheck(L_8);
		int32_t L_9 = L_8->___chapterId;
		LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* L_10 = ___0_levelData;
		NullCheck(L_10);
		int32_t L_11 = L_10->___levelId;
		NullCheck(L_7);
		LevelProgressData_tF4CF8BE6A46189E730098D0C3DEE22CAA57E0C9E* L_12;
		L_12 = GameState_GetLevelProgress_mA5B0F87F59C027AFD2DB0B2523B2B811233EF4B1(L_7, L_9, L_11, (bool)0, NULL);
		LevelProgressData_tF4CF8BE6A46189E730098D0C3DEE22CAA57E0C9E* L_13 = L_12;
		if (L_13)
		{
			G_B8_0 = L_13;
			goto IL_0072;
		}
		G_B7_0 = L_13;
	}
	{
		il2cpp_codegen_initobj((&V_2), sizeof(Nullable_1_tCF32C56A2641879C053C86F273C0C6EC1B40BC28));
		Nullable_1_tCF32C56A2641879C053C86F273C0C6EC1B40BC28 L_14 = V_2;
		G_B9_0 = L_14;
		goto IL_007c;
	}

IL_0072:
	{
		NullCheck(G_B8_0);
		int32_t L_15 = G_B8_0->___attempts;
		Nullable_1_tCF32C56A2641879C053C86F273C0C6EC1B40BC28 L_16;
		memset((&L_16), 0, sizeof(L_16));
		Nullable_1__ctor_m141FA88563AC0B5179132FB929EABD02C47FF703((&L_16), L_15, Nullable_1__ctor_m141FA88563AC0B5179132FB929EABD02C47FF703_RuntimeMethod_var);
		G_B9_0 = L_16;
	}

IL_007c:
	{
		V_0 = G_B9_0;
		int32_t L_17;
		L_17 = Nullable_1_get_Value_m0DF3B826A745419D26A168689AEB4BE6F8698ECA((&___3_registeredAttempt), Nullable_1_get_Value_m0DF3B826A745419D26A168689AEB4BE6F8698ECA_RuntimeMethod_var);
		V_1 = L_17;
		int32_t L_18;
		L_18 = Nullable_1_GetValueOrDefault_m8D130DB7F2A1E694736B449176F9C26DB456597B_inline((&V_0), Nullable_1_GetValueOrDefault_m8D130DB7F2A1E694736B449176F9C26DB456597B_RuntimeMethod_var);
		int32_t L_19 = V_1;
		bool L_20;
		L_20 = Nullable_1_get_HasValue_mCF2FD8B3055FA87FC9C504F2122B3B0FAEDE3EC9_inline((&V_0), Nullable_1_get_HasValue_mCF2FD8B3055FA87FC9C504F2122B3B0FAEDE3EC9_RuntimeMethod_var);
		if (((int32_t)(((((int32_t)L_18) == ((int32_t)L_19))? 1 : 0)&(int32_t)L_20)))
		{
			goto IL_00a4;
		}
	}

IL_0099:
	{
		ArgumentException_tAD90411542A20A9C72D5CDA3A84181D8B947A263* L_21 = (ArgumentException_tAD90411542A20A9C72D5CDA3A84181D8B947A263*)il2cpp_codegen_object_new(((RuntimeClass*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&ArgumentException_tAD90411542A20A9C72D5CDA3A84181D8B947A263_il2cpp_TypeInfo_var)));
		ArgumentException__ctor_m026938A67AF9D36BB7ED27F80425D7194B514465(L_21, ((String_t*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&_stringLiteral0E2FC461A856C0F631B70F7CD928F3B50EC2A060)), NULL);
		IL2CPP_RAISE_MANAGED_EXCEPTION(L_21, ((RuntimeMethod*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&Match3Board_Initialize_m7DAF63C6BE4269C5A966EEA17F428FD81E92CFF8_RuntimeMethod_var)));
	}

IL_00a4:
	{
		LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* L_22 = ___0_levelData;
		Action_1_tD69A6DC9FBE94131E52F5A73B2A9D4AB51EEC404* L_23 = ___1_onWin;
		Action_1_tD69A6DC9FBE94131E52F5A73B2A9D4AB51EEC404* L_24 = ___2_onFail;
		Nullable_1_tCF32C56A2641879C053C86F273C0C6EC1B40BC28 L_25 = ___3_registeredAttempt;
		bool L_26;
		L_26 = Nullable_1_get_HasValue_mCF2FD8B3055FA87FC9C504F2122B3B0FAEDE3EC9_inline((&___3_registeredAttempt), Nullable_1_get_HasValue_mCF2FD8B3055FA87FC9C504F2122B3B0FAEDE3EC9_RuntimeMethod_var);
		Match3Board_InitializeCore_m9AF2370FD9D71DA21A8A460DA6C8392CA8E1E7E9(__this, L_22, L_23, L_24, L_25, (RetainedRoundData_t3EA1BA932DFF2408098F1FB87EC5A5A24130D7A1*)NULL, L_26, NULL);
		return;
	}
}
