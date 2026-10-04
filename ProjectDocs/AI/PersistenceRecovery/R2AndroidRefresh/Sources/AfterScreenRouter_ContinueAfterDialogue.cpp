IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void ScreenRouter_ContinueAfterDialogue_mF1C4512958024BF0A6D98A90E5A6BD89BE09322C (ScreenRouter_t9EC367B45B1296F4CF0CBDEA57AEF86CC5340C00* __this, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&GameObject_GetComponentInChildren_TisProductionDialogueView_t9612D941765F8A0545DD0212DE1D76B0BDD0F8C5_mDF4D077D3B3846055C205509871A44F3EEDC0659_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ScreenRouter_ContinueAfterDialogue_mF1C4512958024BF0A6D98A90E5A6BD89BE09322C_RuntimeMethod_var);
		s_Il2CppMethodInitialized = true;
	}
	ProductionDialogueView_t9612D941765F8A0545DD0212DE1D76B0BDD0F8C5* V_0 = NULL;
	int32_t V_1 = 0;
	{
		int32_t L_0;
		L_0 = StoryService_AdvanceDialogue_m607394AACFC68F30AD59B70975E3C764C1CCAAB0(NULL);
		V_1 = L_0;
		int32_t L_1 = V_1;
		switch (((int32_t)il2cpp_codegen_subtract((int32_t)L_1, 1)))
		{
			case 0:
			{
				goto IL_0054;
			}
			case 1:
			{
				goto IL_005c;
			}
			case 2:
			{
				goto IL_006d;
			}
			case 3:
			{
				goto IL_0064;
			}
			case 4:
			{
				goto IL_0024;
			}
		}
	}
	{
		goto IL_0075;
	}

IL_0024:
	{
		GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_2 = __this->___contentRoot;
		NullCheck(L_2);
		ProductionDialogueView_t9612D941765F8A0545DD0212DE1D76B0BDD0F8C5* L_3;
		L_3 = GameObject_GetComponentInChildren_TisProductionDialogueView_t9612D941765F8A0545DD0212DE1D76B0BDD0F8C5_mDF4D077D3B3846055C205509871A44F3EEDC0659(L_2, GameObject_GetComponentInChildren_TisProductionDialogueView_t9612D941765F8A0545DD0212DE1D76B0BDD0F8C5_mDF4D077D3B3846055C205509871A44F3EEDC0659_RuntimeMethod_var);
		V_0 = L_3;
		ProductionDialogueView_t9612D941765F8A0545DD0212DE1D76B0BDD0F8C5* L_4 = V_0;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_5;
		L_5 = Object_op_Inequality_mD0BE578448EAA61948F25C32F8DD55AB1F778602(L_4, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (!L_5)
		{
			goto IL_004b;
		}
	}
	{
		ProductionDialogueView_t9612D941765F8A0545DD0212DE1D76B0BDD0F8C5* L_6 = V_0;
		Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* L_7 = (Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07*)il2cpp_codegen_object_new(Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07_il2cpp_TypeInfo_var);
		Action__ctor_mBDC7B0B4A3F583B64C2896F01BDED360772F67DC(L_7, __this, (intptr_t)((void*)ScreenRouter_ContinueAfterDialogue_mF1C4512958024BF0A6D98A90E5A6BD89BE09322C_RuntimeMethod_var), NULL);
		NullCheck(L_6);
		ProductionDialogueView_Bind_m620E9F9D67FC1E7E52C0BC3DC18059732472DB9C(L_6, L_7, NULL);
	}

IL_004b:
	{
		bool L_8;
		L_8 = ScreenRouter_ShowErrorDialog_m54E0B54656DDD07A0CC9A5294216CC8A992C4823(__this, (Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07*)NULL, NULL);
		return;
	}

IL_0054:
	{
		ScreenRouter_Show_mB289A1408F5F71A5043EFE930648595D5DCE3734(__this, 6, NULL);
		return;
	}

IL_005c:
	{
		ScreenRouter_Show_mB289A1408F5F71A5043EFE930648595D5DCE3734(__this, 4, NULL);
		return;
	}

IL_0064:
	{
		ScreenRouter_Show_mB289A1408F5F71A5043EFE930648595D5DCE3734(__this, ((int32_t)21), NULL);
		return;
	}

IL_006d:
	{
		ScreenRouter_Show_mB289A1408F5F71A5043EFE930648595D5DCE3734(__this, 3, NULL);
		return;
	}

IL_0075:
	{
		ScreenRouter_Show_mB289A1408F5F71A5043EFE930648595D5DCE3734(__this, 3, NULL);
		return;
	}
}
