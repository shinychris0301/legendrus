IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void ProductionSettingsView_Unbind_m9D0D11CD8C211DC152C1A372AD71A4E25B05F198 (ProductionSettingsView_t8B601F105D46E705808BBD9FA8B26DD7877703BC* __this, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&List_1_Clear_m0444A69F2E16C517FEDBEB99B58E0F8D853E4073_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&List_1_Clear_mA44D7453AD861432A068BDB8B927D1639AA9F2AF_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&List_1_get_Count_m2817F5F47727C2C1FB87CE382D77F92D2658EE78_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&List_1_get_Item_m28EDEA4B6A9DF285910079B34B540331FFE727F6_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&List_1_get_Item_m5BABE1644AE4925547F413C692E38B1E1CE23DA3_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&RuntimeObject_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	int32_t V_0 = 0;
	{
		RuntimeObject* L_0 = (RuntimeObject*)il2cpp_codegen_object_new(RuntimeObject_il2cpp_TypeInfo_var);
		Object__ctor_mE837C6B9FA8C6D5D109F4B2EC885D79919AC0EA2(L_0, NULL);
		__this->___bindingOwner = L_0;
		Il2CppCodeGenWriteBarrier((void**)(&__this->___bindingOwner), (void*)L_0);
		V_0 = 0;
		goto IL_0048;
	}

IL_000f:
	{
		List_1_tCB5B3B0C70F6DFB6B059BB2C65DC77B101849444* L_1 = __this->___buttons;
		int32_t L_2 = V_0;
		NullCheck(L_1);
		Button_t6786514A57F7AFDEE5431112FEA0CAB24F5AE098* L_3;
		L_3 = List_1_get_Item_m5BABE1644AE4925547F413C692E38B1E1CE23DA3(L_1, L_2, List_1_get_Item_m5BABE1644AE4925547F413C692E38B1E1CE23DA3_RuntimeMethod_var);
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_4;
		L_4 = Object_op_Implicit_m93896EF7D68FA113C42D3FE2BC6F661FC7EF514A(L_3, NULL);
		if (!L_4)
		{
			goto IL_0044;
		}
	}
	{
		List_1_tCB5B3B0C70F6DFB6B059BB2C65DC77B101849444* L_5 = __this->___buttons;
		int32_t L_6 = V_0;
		NullCheck(L_5);
		Button_t6786514A57F7AFDEE5431112FEA0CAB24F5AE098* L_7;
		L_7 = List_1_get_Item_m5BABE1644AE4925547F413C692E38B1E1CE23DA3(L_5, L_6, List_1_get_Item_m5BABE1644AE4925547F413C692E38B1E1CE23DA3_RuntimeMethod_var);
		NullCheck(L_7);
		ButtonClickedEvent_t8EA72E90B3BD1392FB3B3EF167D5121C23569E4C* L_8;
		L_8 = Button_get_onClick_m701712A7F7F000CC80D517C4510697E15722C35C_inline(L_7, NULL);
		List_1_t81DD6D8E3F2D498C5E128E9488F7CC05E1881C4D* L_9 = __this->___listeners;
		int32_t L_10 = V_0;
		NullCheck(L_9);
		UnityAction_t11A1F3B953B365C072A5DCC32677EE1796A962A7* L_11;
		L_11 = List_1_get_Item_m28EDEA4B6A9DF285910079B34B540331FFE727F6(L_9, L_10, List_1_get_Item_m28EDEA4B6A9DF285910079B34B540331FFE727F6_RuntimeMethod_var);
		NullCheck(L_8);
		UnityEvent_RemoveListener_m0E138F5575CB4363019D3DA570E98FAD502B812C(L_8, L_11, NULL);
	}

IL_0044:
	{
		int32_t L_12 = V_0;
		V_0 = ((int32_t)il2cpp_codegen_add(L_12, 1));
	}

IL_0048:
	{
		int32_t L_13 = V_0;
		List_1_tCB5B3B0C70F6DFB6B059BB2C65DC77B101849444* L_14 = __this->___buttons;
		NullCheck(L_14);
		int32_t L_15;
		L_15 = List_1_get_Count_m2817F5F47727C2C1FB87CE382D77F92D2658EE78_inline(L_14, List_1_get_Count_m2817F5F47727C2C1FB87CE382D77F92D2658EE78_RuntimeMethod_var);
		if ((((int32_t)L_13) < ((int32_t)L_15)))
		{
			goto IL_000f;
		}
	}
	{
		List_1_tCB5B3B0C70F6DFB6B059BB2C65DC77B101849444* L_16 = __this->___buttons;
		NullCheck(L_16);
		List_1_Clear_mA44D7453AD861432A068BDB8B927D1639AA9F2AF_inline(L_16, List_1_Clear_mA44D7453AD861432A068BDB8B927D1639AA9F2AF_RuntimeMethod_var);
		List_1_t81DD6D8E3F2D498C5E128E9488F7CC05E1881C4D* L_17 = __this->___listeners;
		NullCheck(L_17);
		List_1_Clear_m0444A69F2E16C517FEDBEB99B58E0F8D853E4073_inline(L_17, List_1_Clear_m0444A69F2E16C517FEDBEB99B58E0F8D853E4073_RuntimeMethod_var);
		ProductionSettingsView_set_ActionsBound_m0F31E94D46C9E64308B3E0DE0F15639FC7F87084_inline(__this, (bool)0, NULL);
		return;
	}
}
