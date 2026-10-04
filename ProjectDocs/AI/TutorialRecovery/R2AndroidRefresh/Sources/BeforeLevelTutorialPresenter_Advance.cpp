IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void LevelTutorialPresenter_Advance_mC656636F0183A1F3C9E0E27E5CC8E46ED39AC479 (LevelTutorialPresenter_t294D830AC137EAA461F72B2C73979D74C2FF1254* __this, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	GameState_t5E590D2FB317152A4011A3CA69E3F311E0A7393A* V_0 = NULL;
	int32_t G_B5_0 = 0;
	BalanceTelemetryReviewService_tCEB958C4014C08412D40F113D01EC97650E86BE8* G_B8_0 = NULL;
	BalanceTelemetryReviewService_tCEB958C4014C08412D40F113D01EC97650E86BE8* G_B7_0 = NULL;
	{
		LevelTutorialStepData_t5F0CFFC053BB14B0C2874F016AE8AEC5516B66FD* L_0;
		L_0 = LevelTutorialPresenter_get_CurrentStep_m2FBFC9333C208B8C3A2DBDC7541C7EEC908E4799_inline(__this, NULL);
		if (!L_0)
		{
			goto IL_0083;
		}
	}
	{
		Match3Board_tE4CAA58AF06191DC578D60DE5FA7659B8C8ACD69* L_1 = __this->___board;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_2;
		L_2 = Object_op_Inequality_mD0BE578448EAA61948F25C32F8DD55AB1F778602(L_1, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (!L_2)
		{
			goto IL_0083;
		}
	}
	{
		GameState_t5E590D2FB317152A4011A3CA69E3F311E0A7393A* L_3;
		L_3 = GameState_get_Instance_m3202061BC5E0A39B53697C35D0BEE15692173D74_inline(NULL);
		V_0 = L_3;
		GameState_t5E590D2FB317152A4011A3CA69E3F311E0A7393A* L_4 = V_0;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_5;
		L_5 = Object_op_Inequality_mD0BE578448EAA61948F25C32F8DD55AB1F778602(L_4, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (!L_5)
		{
			goto IL_0038;
		}
	}
	{
		GameState_t5E590D2FB317152A4011A3CA69E3F311E0A7393A* L_6 = V_0;
		LevelTutorialStepData_t5F0CFFC053BB14B0C2874F016AE8AEC5516B66FD* L_7;
		L_7 = LevelTutorialPresenter_get_CurrentStep_m2FBFC9333C208B8C3A2DBDC7541C7EEC908E4799_inline(__this, NULL);
		NullCheck(L_7);
		String_t* L_8 = L_7->___id;
		NullCheck(L_6);
		bool L_9;
		L_9 = GameState_CompleteTutorial_m7DA3AB3239AD74E6FA5DF85723FEABB8ABF9F185(L_6, L_8, NULL);
		G_B5_0 = ((int32_t)(L_9));
		goto IL_0039;
	}

IL_0038:
	{
		G_B5_0 = 0;
	}

IL_0039:
	{
		if (!G_B5_0)
		{
			goto IL_0083;
		}
	}
	{
		Match3Board_tE4CAA58AF06191DC578D60DE5FA7659B8C8ACD69* L_10 = __this->___board;
		NullCheck(L_10);
		LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* L_11;
		L_11 = Match3Board_get_CurrentLevel_m0E75CF21C6F1053FCB8357B94B7EE1DEA6715420_inline(L_10, NULL);
		NullCheck(L_11);
		String_t* L_12 = L_11->___id;
		LevelTutorialStepData_t5F0CFFC053BB14B0C2874F016AE8AEC5516B66FD* L_13;
		L_13 = LevelTutorialPresenter_get_CurrentStep_m2FBFC9333C208B8C3A2DBDC7541C7EEC908E4799_inline(__this, NULL);
		NullCheck(L_13);
		String_t* L_14 = L_13->___id;
		AnalyticsRecordResult_tEBE87AEB03C6D60620CF177295FC8B80E2EF350A* L_15;
		L_15 = PrivacyAwareAnalytics_TryTutorialStepComplete_m6486BCF70976D474388E1065D168D2A35DDF4622(L_12, L_14, NULL);
		BalanceTelemetryReviewService_tCEB958C4014C08412D40F113D01EC97650E86BE8* L_16;
		L_16 = BalanceTelemetryReviewService_get_Instance_m8BE222FB4502374A70B342B1003DFCB437801EA4_inline(NULL);
		BalanceTelemetryReviewService_tCEB958C4014C08412D40F113D01EC97650E86BE8* L_17 = L_16;
		if (L_17)
		{
			G_B8_0 = L_17;
			goto IL_0067;
		}
		G_B7_0 = L_17;
	}
	{
		goto IL_0083;
	}

IL_0067:
	{
		Match3Board_tE4CAA58AF06191DC578D60DE5FA7659B8C8ACD69* L_18 = __this->___board;
		NullCheck(L_18);
		LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* L_19;
		L_19 = Match3Board_get_CurrentLevel_m0E75CF21C6F1053FCB8357B94B7EE1DEA6715420_inline(L_18, NULL);
		LevelTutorialStepData_t5F0CFFC053BB14B0C2874F016AE8AEC5516B66FD* L_20;
		L_20 = LevelTutorialPresenter_get_CurrentStep_m2FBFC9333C208B8C3A2DBDC7541C7EEC908E4799_inline(__this, NULL);
		NullCheck(L_20);
		String_t* L_21 = L_20->___id;
		NullCheck(G_B8_0);
		bool L_22;
		L_22 = BalanceTelemetryReviewService_TryRecordTutorialCompleted_m00E55E63C0185613B203982C7CDBE965D0661911(G_B8_0, L_19, L_21, NULL);
	}

IL_0083:
	{
		LevelTutorialPresenter_set_CurrentStep_m7932D3DF24C9BF5ACC391D289E7BEF7D4651FFCC_inline(__this, (LevelTutorialStepData_t5F0CFFC053BB14B0C2874F016AE8AEC5516B66FD*)NULL, NULL);
		GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_23 = __this->___overlay;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_24;
		L_24 = Object_op_Inequality_mD0BE578448EAA61948F25C32F8DD55AB1F778602(L_23, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (!L_24)
		{
			goto IL_00a3;
		}
	}
	{
		GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_25 = __this->___overlay;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		Object_Destroy_mE97D0A766419A81296E8D4E5C23D01D3FE91ACBB(L_25, NULL);
	}

IL_00a3:
	{
		__this->___overlay = (GameObject_t76FEDD663AB33C991A9C9A23129337651094216F*)NULL;
		Il2CppCodeGenWriteBarrier((void**)(&__this->___overlay), (void*)(GameObject_t76FEDD663AB33C991A9C9A23129337651094216F*)NULL);
		LevelTutorialSequence_t0A0AF6211DAB5C148DD031D531C50820E6057ADC* L_26 = __this->___sequence;
		if (!L_26)
		{
			goto IL_00c6;
		}
	}
	{
		LevelTutorialSequence_t0A0AF6211DAB5C148DD031D531C50820E6057ADC* L_27 = __this->___sequence;
		NullCheck(L_27);
		bool L_28;
		L_28 = LevelTutorialSequence_get_HasPending_m63C49A7795D8DD304E58C325C3D7B7ABE32DCBA8(L_27, NULL);
		if (!L_28)
		{
			goto IL_00c6;
		}
	}
	{
		LevelTutorialPresenter_TryPresentNext_m323B983B6D8F57B4CC3A5B14207C489A632E725E(__this, NULL);
		return;
	}

IL_00c6:
	{
		LevelTutorialPresenter_ResumeBoard_mF5B24F588CACEC9ABD026D7139FA83CEE1BC2436(__this, NULL);
		return;
	}
}
