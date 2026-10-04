IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool ProductionSettingsView_InputReady_m8DC89E168BFF7EB9C914784F9A7003508FA74353 (ProductionSettingsView_t8B601F105D46E705808BBD9FA8B26DD7877703BC* __this, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Nullable_1_GetValueOrDefault_m9E93E2CEA72E2689103A641157336A4F674A67A3_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	ScreenRouter_t9EC367B45B1296F4CF0CBDEA57AEF86CC5340C00* V_0 = NULL;
	Nullable_1_t4FCFD27B7FC762CFC2F64096CD6201A0B0AF4894 V_1;
	memset((&V_1), 0, sizeof(V_1));
	{
		bool L_0;
		L_0 = ProductionSettingsView_get_ActionsBound_m14238CA2FBFFCEFCD92391BF75105EF9B3931593_inline(__this, NULL);
		if (!L_0)
		{
			goto IL_0018;
		}
	}
	{
		bool L_1 = __this->___committed;
		if (L_1)
		{
			goto IL_0018;
		}
	}
	{
		bool L_2;
		L_2 = Behaviour_get_isActiveAndEnabled_mEB4ECCE9761A7016BC619557CEFEA1A30D3BF28A(__this, NULL);
		if (L_2)
		{
			goto IL_001a;
		}
	}

IL_0018:
	{
		return (bool)0;
	}

IL_001a:
	{
		ScreenRouter_t9EC367B45B1296F4CF0CBDEA57AEF86CC5340C00* L_3;
		L_3 = ScreenRouter_get_Instance_m8BF278B1C796119090BB172271DD3E34450961E1_inline(NULL);
		V_0 = L_3;
		ScreenRouter_t9EC367B45B1296F4CF0CBDEA57AEF86CC5340C00* L_4 = V_0;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_5;
		L_5 = Object_op_Equality_mB6120F782D83091EF56A198FCEBCF066DB4A9605(L_4, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (!L_5)
		{
			goto IL_002b;
		}
	}
	{
		return (bool)1;
	}

IL_002b:
	{
		ScreenRouter_t9EC367B45B1296F4CF0CBDEA57AEF86CC5340C00* L_6 = V_0;
		NullCheck(L_6);
		Nullable_1_t4FCFD27B7FC762CFC2F64096CD6201A0B0AF4894 L_7;
		L_7 = ScreenRouter_get_ActiveModal_m9EA5BC60DA723655448B472A191332E1E102D79F_inline(L_6, NULL);
		V_1 = L_7;
		int32_t L_8;
		L_8 = Nullable_1_GetValueOrDefault_m9E93E2CEA72E2689103A641157336A4F674A67A3_inline((&V_1), Nullable_1_GetValueOrDefault_m9E93E2CEA72E2689103A641157336A4F674A67A3_RuntimeMethod_var);
		if ((!(((uint32_t)L_8) == ((uint32_t)((int32_t)16)))))
		{
			goto IL_0047;
		}
	}
	{
		ScreenRouter_t9EC367B45B1296F4CF0CBDEA57AEF86CC5340C00* L_9 = V_0;
		NullCheck(L_9);
		bool L_10;
		L_10 = ScreenRouter_get_IsModalTransitioning_mA7EE64426C2CB00227FEC49004C2775C8016F40A(L_9, NULL);
		return (bool)((((int32_t)L_10) == ((int32_t)0))? 1 : 0);
	}

IL_0047:
	{
		ScreenRouter_t9EC367B45B1296F4CF0CBDEA57AEF86CC5340C00* L_11 = V_0;
		NullCheck(L_11);
		bool L_12;
		L_12 = ScreenRouter_get_HasActiveModalBlocker_m158669A3E3F1B8C69362266E5D59A400C78793B0(L_11, NULL);
		if (L_12)
		{
			goto IL_0059;
		}
	}
	{
		ScreenRouter_t9EC367B45B1296F4CF0CBDEA57AEF86CC5340C00* L_13 = V_0;
		NullCheck(L_13);
		bool L_14;
		L_14 = ScreenRouter_get_IsScreenTransitioning_m3E031B320BE4B0E736E819DAFD52624DBED47015(L_13, NULL);
		return (bool)((((int32_t)L_14) == ((int32_t)0))? 1 : 0);
	}

IL_0059:
	{
		return (bool)0;
	}
}
