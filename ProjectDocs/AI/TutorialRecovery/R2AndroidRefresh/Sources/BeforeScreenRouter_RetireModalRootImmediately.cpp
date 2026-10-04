IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void ScreenRouter_RetireModalRootImmediately_mCD65D4BE17BC2250E3CC0E2FF60A880A86C46A9B (ScreenRouter_t9EC367B45B1296F4CF0CBDEA57AEF86CC5340C00* __this, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Nullable_1_GetValueOrDefault_m9E93E2CEA72E2689103A641157336A4F674A67A3_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* V_0 = NULL;
	{
		__this->___recapEntries = (RuntimeObject*)NULL;
		Il2CppCodeGenWriteBarrier((void**)(&__this->___recapEntries), (void*)(RuntimeObject*)NULL);
		Nullable_1_t4FCFD27B7FC762CFC2F64096CD6201A0B0AF4894* L_0 = (Nullable_1_t4FCFD27B7FC762CFC2F64096CD6201A0B0AF4894*)(&__this->___activeModalId);
		int32_t L_1;
		L_1 = Nullable_1_GetValueOrDefault_m9E93E2CEA72E2689103A641157336A4F674A67A3_inline(L_0, Nullable_1_GetValueOrDefault_m9E93E2CEA72E2689103A641157336A4F674A67A3_RuntimeMethod_var);
		if ((!(((uint32_t)L_1) == ((uint32_t)((int32_t)12)))))
		{
			goto IL_0029;
		}
	}
	{
		Nullable_1_tFF46807EF2622F15A62B26F542EC38258BA8254E* L_2 = (Nullable_1_tFF46807EF2622F15A62B26F542EC38258BA8254E*)(&__this->___pendingRewardReceipt);
		il2cpp_codegen_initobj(L_2, sizeof(Nullable_1_tFF46807EF2622F15A62B26F542EC38258BA8254E));
		__this->___pendingRewardDay = 0;
	}

IL_0029:
	{
		GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_3 = __this->___modalRoot;
		V_0 = L_3;
		__this->___modalRoot = (GameObject_t76FEDD663AB33C991A9C9A23129337651094216F*)NULL;
		Il2CppCodeGenWriteBarrier((void**)(&__this->___modalRoot), (void*)(GameObject_t76FEDD663AB33C991A9C9A23129337651094216F*)NULL);
		__this->___modalVisualRoot = (GameObject_t76FEDD663AB33C991A9C9A23129337651094216F*)NULL;
		Il2CppCodeGenWriteBarrier((void**)(&__this->___modalVisualRoot), (void*)(GameObject_t76FEDD663AB33C991A9C9A23129337651094216F*)NULL);
		__this->___modalTransition = (MetaUiTransition_tF27436B0392F808114BF15D3FBF6336D609C4C78*)NULL;
		Il2CppCodeGenWriteBarrier((void**)(&__this->___modalTransition), (void*)(MetaUiTransition_tF27436B0392F808114BF15D3FBF6336D609C4C78*)NULL);
		Nullable_1_t4FCFD27B7FC762CFC2F64096CD6201A0B0AF4894* L_4 = (Nullable_1_t4FCFD27B7FC762CFC2F64096CD6201A0B0AF4894*)(&__this->___activeModalId);
		il2cpp_codegen_initobj(L_4, sizeof(Nullable_1_t4FCFD27B7FC762CFC2F64096CD6201A0B0AF4894));
		Nullable_1_t4FCFD27B7FC762CFC2F64096CD6201A0B0AF4894* L_5 = (Nullable_1_t4FCFD27B7FC762CFC2F64096CD6201A0B0AF4894*)(&__this->___pendingModalId);
		il2cpp_codegen_initobj(L_5, sizeof(Nullable_1_t4FCFD27B7FC762CFC2F64096CD6201A0B0AF4894));
		__this->___modalAfterClose = (Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07*)NULL;
		Il2CppCodeGenWriteBarrier((void**)(&__this->___modalAfterClose), (void*)(Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07*)NULL);
		__this->___modalContinuationGeneration = 0;
		__this->___modalMotionState = 0;
		GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_6 = V_0;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_7;
		L_7 = Object_op_Inequality_mD0BE578448EAA61948F25C32F8DD55AB1F778602(L_6, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (!L_7)
		{
			goto IL_0088;
		}
	}
	{
		GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_8 = V_0;
		NullCheck(L_8);
		GameObject_SetActive_m638E92E1E75E519E5B24CF150B08CA8E0CDFAB92(L_8, (bool)0, NULL);
		GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_9 = V_0;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		Object_Destroy_mE97D0A766419A81296E8D4E5C23D01D3FE91ACBB(L_9, NULL);
	}

IL_0088:
	{
		return;
	}
}
