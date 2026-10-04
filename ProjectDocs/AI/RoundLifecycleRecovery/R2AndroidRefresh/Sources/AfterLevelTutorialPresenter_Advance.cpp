IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void LevelTutorialPresenter_Advance_mC656636F0183A1F3C9E0E27E5CC8E46ED39AC479 (LevelTutorialPresenter_t294D830AC137EAA461F72B2C73979D74C2FF1254* __this, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	GameState_t5E590D2FB317152A4011A3CA69E3F311E0A7393A* V_0 = NULL;
	bool V_1 = false;
	int32_t G_B11_0 = 0;
	Action_1_t551F28E28931976BE5EDB871536352FE6A56B8E6* G_B16_0 = NULL;
	Action_1_t551F28E28931976BE5EDB871536352FE6A56B8E6* G_B15_0 = NULL;
	BalanceTelemetryReviewService_tCEB958C4014C08412D40F113D01EC97650E86BE8* G_B20_0 = NULL;
	BalanceTelemetryReviewService_tCEB958C4014C08412D40F113D01EC97650E86BE8* G_B19_0 = NULL;
	{
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_0;
		L_0 = Object_op_Equality_mB6120F782D83091EF56A198FCEBCF066DB4A9605(__this, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (L_0)
		{
			goto IL_002f;
		}
	}
	{
		bool L_1;
		L_1 = Behaviour_get_isActiveAndEnabled_mEB4ECCE9761A7016BC619557CEFEA1A30D3BF28A(__this, NULL);
		if (!L_1)
		{
			goto IL_002f;
		}
	}
	{
		LevelTutorialStepData_t5F0CFFC053BB14B0C2874F016AE8AEC5516B66FD* L_2;
		L_2 = LevelTutorialPresenter_get_CurrentStep_m2FBFC9333C208B8C3A2DBDC7541C7EEC908E4799_inline(__this, NULL);
		if (!L_2)
		{
			goto IL_002f;
		}
	}
	{
		Match3Board_tE4CAA58AF06191DC578D60DE5FA7659B8C8ACD69* L_3 = __this->___board;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_4;
		L_4 = Object_op_Equality_mB6120F782D83091EF56A198FCEBCF066DB4A9605(L_3, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (L_4)
		{
			goto IL_002f;
		}
	}
	{
		bool L_5;
		L_5 = LevelTutorialPresenter_get_IsPresenting_m2B8F83446C06F46591BCCC35F74AE1F38CE84A1C(__this, NULL);
		if (L_5)
		{
			goto IL_0030;
		}
	}

IL_002f:
	{
		return;
	}

IL_0030:
	{
		LevelTutorialStepData_t5F0CFFC053BB14B0C2874F016AE8AEC5516B66FD* L_6;
		L_6 = LevelTutorialPresenter_get_CurrentStep_m2FBFC9333C208B8C3A2DBDC7541C7EEC908E4799_inline(__this, NULL);
		if (!L_6)
		{
			goto IL_00ec;
		}
	}
	{
		Match3Board_tE4CAA58AF06191DC578D60DE5FA7659B8C8ACD69* L_7 = __this->___board;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_8;
		L_8 = Object_op_Inequality_mD0BE578448EAA61948F25C32F8DD55AB1F778602(L_7, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (!L_8)
		{
			goto IL_00ec;
		}
	}
	{
		GameState_t5E590D2FB317152A4011A3CA69E3F311E0A7393A* L_9;
		L_9 = GameState_get_Instance_m3202061BC5E0A39B53697C35D0BEE15692173D74_inline(NULL);
		V_0 = L_9;
		GameState_t5E590D2FB317152A4011A3CA69E3F311E0A7393A* L_10 = V_0;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_11;
		L_11 = Object_op_Inequality_mD0BE578448EAA61948F25C32F8DD55AB1F778602(L_10, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (!L_11)
		{
			goto IL_006e;
		}
	}
	{
		GameState_t5E590D2FB317152A4011A3CA69E3F311E0A7393A* L_12 = V_0;
		LevelTutorialStepData_t5F0CFFC053BB14B0C2874F016AE8AEC5516B66FD* L_13;
		L_13 = LevelTutorialPresenter_get_CurrentStep_m2FBFC9333C208B8C3A2DBDC7541C7EEC908E4799_inline(__this, NULL);
		NullCheck(L_13);
		String_t* L_14 = L_13->___id;
		NullCheck(L_12);
		bool L_15;
		L_15 = GameState_CompleteTutorial_m7DA3AB3239AD74E6FA5DF85723FEABB8ABF9F185(L_12, L_14, NULL);
		G_B11_0 = ((int32_t)(L_15));
		goto IL_006f;
	}

IL_006e:
	{
		G_B11_0 = 0;
	}

IL_006f:
	{
		V_1 = (bool)G_B11_0;
		bool L_16 = V_1;
		if (L_16)
		{
			goto IL_00a1;
		}
	}
	{
		GameState_t5E590D2FB317152A4011A3CA69E3F311E0A7393A* L_17 = V_0;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_18;
		L_18 = Object_op_Equality_mB6120F782D83091EF56A198FCEBCF066DB4A9605(L_17, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (L_18)
		{
			goto IL_008f;
		}
	}
	{
		GameState_t5E590D2FB317152A4011A3CA69E3F311E0A7393A* L_19 = V_0;
		LevelTutorialStepData_t5F0CFFC053BB14B0C2874F016AE8AEC5516B66FD* L_20;
		L_20 = LevelTutorialPresenter_get_CurrentStep_m2FBFC9333C208B8C3A2DBDC7541C7EEC908E4799_inline(__this, NULL);
		NullCheck(L_20);
		String_t* L_21 = L_20->___id;
		NullCheck(L_19);
		bool L_22;
		L_22 = GameState_HasCompletedTutorial_mFB9AF9E1D9899BD4AB5781D1013292E5BB78870D(L_19, L_21, NULL);
		if (L_22)
		{
			goto IL_00a1;
		}
	}

IL_008f:
	{
		Action_1_t551F28E28931976BE5EDB871536352FE6A56B8E6* L_23 = __this->___AcknowledgementRejected;
		Action_1_t551F28E28931976BE5EDB871536352FE6A56B8E6* L_24 = L_23;
		if (L_24)
		{
			G_B16_0 = L_24;
			goto IL_009a;
		}
		G_B15_0 = L_24;
	}
	{
		return;
	}

IL_009a:
	{
		NullCheck(G_B16_0);
		Action_1_Invoke_m05D950C4601087D8297123F9F97296B647DD7447_inline(G_B16_0, __this, NULL);
		return;
	}

IL_00a1:
	{
		bool L_25 = V_1;
		if (!L_25)
		{
			goto IL_00ec;
		}
	}
	{
		Match3Board_tE4CAA58AF06191DC578D60DE5FA7659B8C8ACD69* L_26 = __this->___board;
		NullCheck(L_26);
		LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* L_27;
		L_27 = Match3Board_get_CurrentLevel_m0E75CF21C6F1053FCB8357B94B7EE1DEA6715420_inline(L_26, NULL);
		NullCheck(L_27);
		String_t* L_28 = L_27->___id;
		LevelTutorialStepData_t5F0CFFC053BB14B0C2874F016AE8AEC5516B66FD* L_29;
		L_29 = LevelTutorialPresenter_get_CurrentStep_m2FBFC9333C208B8C3A2DBDC7541C7EEC908E4799_inline(__this, NULL);
		NullCheck(L_29);
		String_t* L_30 = L_29->___id;
		AnalyticsRecordResult_tEBE87AEB03C6D60620CF177295FC8B80E2EF350A* L_31;
		L_31 = PrivacyAwareAnalytics_TryTutorialStepComplete_m6486BCF70976D474388E1065D168D2A35DDF4622(L_28, L_30, NULL);
		BalanceTelemetryReviewService_tCEB958C4014C08412D40F113D01EC97650E86BE8* L_32;
		L_32 = BalanceTelemetryReviewService_get_Instance_m8BE222FB4502374A70B342B1003DFCB437801EA4_inline(NULL);
		BalanceTelemetryReviewService_tCEB958C4014C08412D40F113D01EC97650E86BE8* L_33 = L_32;
		if (L_33)
		{
			G_B20_0 = L_33;
			goto IL_00d0;
		}
		G_B19_0 = L_33;
	}
	{
		goto IL_00ec;
	}

IL_00d0:
	{
		Match3Board_tE4CAA58AF06191DC578D60DE5FA7659B8C8ACD69* L_34 = __this->___board;
		NullCheck(L_34);
		LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* L_35;
		L_35 = Match3Board_get_CurrentLevel_m0E75CF21C6F1053FCB8357B94B7EE1DEA6715420_inline(L_34, NULL);
		LevelTutorialStepData_t5F0CFFC053BB14B0C2874F016AE8AEC5516B66FD* L_36;
		L_36 = LevelTutorialPresenter_get_CurrentStep_m2FBFC9333C208B8C3A2DBDC7541C7EEC908E4799_inline(__this, NULL);
		NullCheck(L_36);
		String_t* L_37 = L_36->___id;
		NullCheck(G_B20_0);
		bool L_38;
		L_38 = BalanceTelemetryReviewService_TryRecordTutorialCompleted_m00E55E63C0185613B203982C7CDBE965D0661911(G_B20_0, L_35, L_37, NULL);
	}

IL_00ec:
	{
		LevelTutorialPresenter_set_CurrentStep_m7932D3DF24C9BF5ACC391D289E7BEF7D4651FFCC_inline(__this, (LevelTutorialStepData_t5F0CFFC053BB14B0C2874F016AE8AEC5516B66FD*)NULL, NULL);
		GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_39 = __this->___overlay;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_40;
		L_40 = Object_op_Inequality_mD0BE578448EAA61948F25C32F8DD55AB1F778602(L_39, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (!L_40)
		{
			goto IL_0118;
		}
	}
	{
		GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_41 = __this->___overlay;
		NullCheck(L_41);
		GameObject_SetActive_m638E92E1E75E519E5B24CF150B08CA8E0CDFAB92(L_41, (bool)0, NULL);
		GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_42 = __this->___overlay;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		Object_Destroy_mE97D0A766419A81296E8D4E5C23D01D3FE91ACBB(L_42, NULL);
	}

IL_0118:
	{
		__this->___overlay = (GameObject_t76FEDD663AB33C991A9C9A23129337651094216F*)NULL;
		Il2CppCodeGenWriteBarrier((void**)(&__this->___overlay), (void*)(GameObject_t76FEDD663AB33C991A9C9A23129337651094216F*)NULL);
		LevelTutorialSequence_t0A0AF6211DAB5C148DD031D531C50820E6057ADC* L_43 = __this->___sequence;
		if (!L_43)
		{
			goto IL_013b;
		}
	}
	{
		LevelTutorialSequence_t0A0AF6211DAB5C148DD031D531C50820E6057ADC* L_44 = __this->___sequence;
		NullCheck(L_44);
		bool L_45;
		L_45 = LevelTutorialSequence_get_HasPending_m63C49A7795D8DD304E58C325C3D7B7ABE32DCBA8(L_44, NULL);
		if (!L_45)
		{
			goto IL_013b;
		}
	}
	{
		LevelTutorialPresenter_TryPresentNext_m323B983B6D8F57B4CC3A5B14207C489A632E725E(__this, NULL);
		return;
	}

IL_013b:
	{
		LevelTutorialPresenter_ResumeBoard_mF5B24F588CACEC9ABD026D7139FA83CEE1BC2436(__this, NULL);
		return;
	}
}
