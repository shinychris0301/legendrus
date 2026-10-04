IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void ScreenRouter_ShowGameplayTutorialError_mF47851D20D999688992EB7A8BA5F40EB6CF4AE2D (ScreenRouter_t9EC367B45B1296F4CF0CBDEA57AEF86CC5340C00* __this, Match3Board_tE4CAA58AF06191DC578D60DE5FA7659B8C8ACD69* ___0_board, LevelTutorialPresenter_t294D830AC137EAA461F72B2C73979D74C2FF1254* ___1_presenter, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&GameObject_GetComponentInChildren_TisLevelTutorialPresenter_t294D830AC137EAA461F72B2C73979D74C2FF1254_m688A30A97649A9F60C572AE5948DCD06DD3BB1B2_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&GameObject_GetComponentInChildren_TisProductionErrorDialogView_t605F616332AD9FF774C08DCF1375B6428DC58664_mD004649A234FD6B09D50B0DFA124DAE5A7FD197D_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ScreenRouter_CloseModal_mDA5B2C06AE67DE089FA24083FA5781E5976D607B_RuntimeMethod_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		Match3Board_tE4CAA58AF06191DC578D60DE5FA7659B8C8ACD69* L_0 = ___0_board;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_1;
		L_1 = Object_op_Equality_mB6120F782D83091EF56A198FCEBCF066DB4A9605(L_0, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (L_1)
		{
			goto IL_0074;
		}
	}
	{
		Match3Board_tE4CAA58AF06191DC578D60DE5FA7659B8C8ACD69* L_2 = ___0_board;
		Match3Board_tE4CAA58AF06191DC578D60DE5FA7659B8C8ACD69* L_3 = __this->___activeBoard;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_4;
		L_4 = Object_op_Inequality_mD0BE578448EAA61948F25C32F8DD55AB1F778602(L_2, L_3, NULL);
		if (L_4)
		{
			goto IL_0074;
		}
	}
	{
		int32_t L_5 = __this->___current;
		if ((!(((uint32_t)L_5) == ((uint32_t)5))))
		{
			goto IL_0074;
		}
	}
	{
		Match3Board_tE4CAA58AF06191DC578D60DE5FA7659B8C8ACD69* L_6 = ___0_board;
		NullCheck(L_6);
		int32_t L_7;
		L_7 = Match3Board_get_Phase_m581BB8F28FB57A952649B79CDE465D807B8ABEAD(L_6, NULL);
		if ((!(((uint32_t)L_7) == ((uint32_t)((int32_t)10)))))
		{
			goto IL_0074;
		}
	}
	{
		LevelTutorialPresenter_t294D830AC137EAA461F72B2C73979D74C2FF1254* L_8 = ___1_presenter;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_9;
		L_9 = Object_op_Equality_mB6120F782D83091EF56A198FCEBCF066DB4A9605(L_8, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (L_9)
		{
			goto IL_0074;
		}
	}
	{
		LevelTutorialPresenter_t294D830AC137EAA461F72B2C73979D74C2FF1254* L_10 = ___1_presenter;
		NullCheck(L_10);
		bool L_11;
		L_11 = LevelTutorialPresenter_get_IsPresenting_m2B8F83446C06F46591BCCC35F74AE1F38CE84A1C(L_10, NULL);
		if (!L_11)
		{
			goto IL_0074;
		}
	}
	{
		LevelTutorialPresenter_t294D830AC137EAA461F72B2C73979D74C2FF1254* L_12 = ___1_presenter;
		NullCheck(L_12);
		LevelTutorialStepData_t5F0CFFC053BB14B0C2874F016AE8AEC5516B66FD* L_13;
		L_13 = LevelTutorialPresenter_get_CurrentStep_m2FBFC9333C208B8C3A2DBDC7541C7EEC908E4799_inline(L_12, NULL);
		if (!L_13)
		{
			goto IL_0074;
		}
	}
	{
		GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_14 = __this->___contentRoot;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_15;
		L_15 = Object_op_Equality_mB6120F782D83091EF56A198FCEBCF066DB4A9605(L_14, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (L_15)
		{
			goto IL_0074;
		}
	}
	{
		GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_16 = __this->___contentRoot;
		NullCheck(L_16);
		LevelTutorialPresenter_t294D830AC137EAA461F72B2C73979D74C2FF1254* L_17;
		L_17 = GameObject_GetComponentInChildren_TisLevelTutorialPresenter_t294D830AC137EAA461F72B2C73979D74C2FF1254_m688A30A97649A9F60C572AE5948DCD06DD3BB1B2(L_16, GameObject_GetComponentInChildren_TisLevelTutorialPresenter_t294D830AC137EAA461F72B2C73979D74C2FF1254_m688A30A97649A9F60C572AE5948DCD06DD3BB1B2_RuntimeMethod_var);
		LevelTutorialPresenter_t294D830AC137EAA461F72B2C73979D74C2FF1254* L_18 = ___1_presenter;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_19;
		L_19 = Object_op_Inequality_mD0BE578448EAA61948F25C32F8DD55AB1F778602(L_17, L_18, NULL);
		if (L_19)
		{
			goto IL_0074;
		}
	}
	{
		int32_t L_20 = __this->___modalMotionState;
		if (L_20)
		{
			goto IL_0074;
		}
	}
	{
		bool L_21;
		L_21 = ScreenRouter_get_IsScreenTransitioning_m3E031B320BE4B0E736E819DAFD52624DBED47015(__this, NULL);
		if (!L_21)
		{
			goto IL_0075;
		}
	}

IL_0074:
	{
		return;
	}

IL_0075:
	{
		bool L_22;
		L_22 = ScreenRouter_ShowModal_m195E7C54FEA3162B7A49F9BF61E3CA321B3AE5D9(__this, ((int32_t)36), NULL);
		if (L_22)
		{
			goto IL_0080;
		}
	}
	{
		return;
	}

IL_0080:
	{
		LevelTutorialPresenter_t294D830AC137EAA461F72B2C73979D74C2FF1254* L_23 = ___1_presenter;
		__this->___tutorialErrorOwner = L_23;
		Il2CppCodeGenWriteBarrier((void**)(&__this->___tutorialErrorOwner), (void*)L_23);
		GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_24 = __this->___modalVisualRoot;
		NullCheck(L_24);
		ProductionErrorDialogView_t605F616332AD9FF774C08DCF1375B6428DC58664* L_25;
		L_25 = GameObject_GetComponentInChildren_TisProductionErrorDialogView_t605F616332AD9FF774C08DCF1375B6428DC58664_mD004649A234FD6B09D50B0DFA124DAE5A7FD197D(L_24, GameObject_GetComponentInChildren_TisProductionErrorDialogView_t605F616332AD9FF774C08DCF1375B6428DC58664_mD004649A234FD6B09D50B0DFA124DAE5A7FD197D_RuntimeMethod_var);
		Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* L_26 = (Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07*)il2cpp_codegen_object_new(Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07_il2cpp_TypeInfo_var);
		Action__ctor_mBDC7B0B4A3F583B64C2896F01BDED360772F67DC(L_26, __this, (intptr_t)((void*)ScreenRouter_CloseModal_mDA5B2C06AE67DE089FA24083FA5781E5976D607B_RuntimeMethod_var), NULL);
		NullCheck(L_25);
		ProductionErrorDialogView_Bind_m87E611C0B45763106348C7A6376AC838943DA2C5(L_25, L_26, (Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07*)NULL, NULL);
		return;
	}
}
