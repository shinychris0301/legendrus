IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void U3CU3Ec__DisplayClass29_0_U3CBuildOverlayU3Eb__0_mD9524AF715C252949B5708108FB6F75BD0C2FB27 (U3CU3Ec__DisplayClass29_0_t0711079A39BF4E1A22175E1A52F36B2A305A96BD* __this, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		LevelTutorialPresenter_t294D830AC137EAA461F72B2C73979D74C2FF1254* L_0 = __this->___U3CU3E4__this;
		NullCheck(L_0);
		GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_1 = L_0->___overlay;
		GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_2 = __this->___presentedOverlay;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_3;
		L_3 = Object_op_Equality_mB6120F782D83091EF56A198FCEBCF066DB4A9605(L_1, L_2, NULL);
		if (!L_3)
		{
			goto IL_0036;
		}
	}
	{
		LevelTutorialPresenter_t294D830AC137EAA461F72B2C73979D74C2FF1254* L_4 = __this->___U3CU3E4__this;
		NullCheck(L_4);
		LevelTutorialStepData_t5F0CFFC053BB14B0C2874F016AE8AEC5516B66FD* L_5;
		L_5 = LevelTutorialPresenter_get_CurrentStep_m2FBFC9333C208B8C3A2DBDC7541C7EEC908E4799_inline(L_4, NULL);
		LevelTutorialStepData_t5F0CFFC053BB14B0C2874F016AE8AEC5516B66FD* L_6 = __this->___step;
		if ((!(((RuntimeObject*)(LevelTutorialStepData_t5F0CFFC053BB14B0C2874F016AE8AEC5516B66FD*)L_5) == ((RuntimeObject*)(LevelTutorialStepData_t5F0CFFC053BB14B0C2874F016AE8AEC5516B66FD*)L_6))))
		{
			goto IL_0036;
		}
	}
	{
		LevelTutorialPresenter_t294D830AC137EAA461F72B2C73979D74C2FF1254* L_7 = __this->___U3CU3E4__this;
		NullCheck(L_7);
		LevelTutorialPresenter_Advance_mC656636F0183A1F3C9E0E27E5CC8E46ED39AC479(L_7, NULL);
	}

IL_0036:
	{
		return;
	}
}
