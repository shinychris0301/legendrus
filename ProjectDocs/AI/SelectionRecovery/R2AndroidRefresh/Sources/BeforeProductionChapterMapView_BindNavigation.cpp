IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void ProductionChapterMapView_BindNavigation_mFD3328B8E09188FF10F1C0300A8B90EF773DC26B (ProductionChapterMapView_t73FA61D194FCDDD0F89F8021A5BBFC50FD5D1075* __this, Action_1_t913CB0C3CDC5C59F6B8A1A7D957A63049F5A6BCA* ___0_show, Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* ___1_openSelectedLevel, Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* ___2_back, Action_1_t3CB5D1A819C3ED3F99E9E39F890F18633253949A* ___3_notify, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ProductionChapterMapScroller_JumpToCurrent_mAADDDE9D4842765E918BE5553231C63440F1D169_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&U3CU3Ec__DisplayClass52_0_U3CBindNavigationU3Eb__0_mA1084A0904F93E11D9B76B9853F3BA6FAADB48B1_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&U3CU3Ec__DisplayClass52_0_U3CBindNavigationU3Eb__1_m93C80F4577F4BF57504E64A30E5849D538677C10_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&U3CU3Ec__DisplayClass52_0_t9CED2D43B105C8CFAF5F15DEEF9A7644E9AD64D2_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&U3CU3Ec__DisplayClass52_1_U3CBindNavigationU3Eb__2_m373DB10D8158B3AEEF0B9C6E29873332BD457DBA_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&U3CU3Ec__DisplayClass52_1_t0FC06C2BFBC14F6F7C3B38ACE4BE6AE9C0A95ACF_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&U3CU3Ec__DisplayClass52_2_U3CBindNavigationU3Eb__3_m29C7B6BCADC6C94ACDFB4EAED844936A1BD3EAB4_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&U3CU3Ec__DisplayClass52_2_t9F9A10E71F3E3EF236A645BA0B3196F07E438A8C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	U3CU3Ec__DisplayClass52_0_t9CED2D43B105C8CFAF5F15DEEF9A7644E9AD64D2* V_0 = NULL;
	int32_t V_1 = 0;
	U3CU3Ec__DisplayClass52_1_t0FC06C2BFBC14F6F7C3B38ACE4BE6AE9C0A95ACF* V_2 = NULL;
	int32_t V_3 = 0;
	U3CU3Ec__DisplayClass52_2_t9F9A10E71F3E3EF236A645BA0B3196F07E438A8C* V_4 = NULL;
	{
		U3CU3Ec__DisplayClass52_0_t9CED2D43B105C8CFAF5F15DEEF9A7644E9AD64D2* L_0 = (U3CU3Ec__DisplayClass52_0_t9CED2D43B105C8CFAF5F15DEEF9A7644E9AD64D2*)il2cpp_codegen_object_new(U3CU3Ec__DisplayClass52_0_t9CED2D43B105C8CFAF5F15DEEF9A7644E9AD64D2_il2cpp_TypeInfo_var);
		U3CU3Ec__DisplayClass52_0__ctor_m98E846DAC4997713A32F489A7C2283C1BD8EE333(L_0, NULL);
		V_0 = L_0;
		U3CU3Ec__DisplayClass52_0_t9CED2D43B105C8CFAF5F15DEEF9A7644E9AD64D2* L_1 = V_0;
		Action_1_t913CB0C3CDC5C59F6B8A1A7D957A63049F5A6BCA* L_2 = ___0_show;
		NullCheck(L_1);
		L_1->___show = L_2;
		Il2CppCodeGenWriteBarrier((void**)(&L_1->___show), (void*)L_2);
		U3CU3Ec__DisplayClass52_0_t9CED2D43B105C8CFAF5F15DEEF9A7644E9AD64D2* L_3 = V_0;
		NullCheck(L_3);
		L_3->___U3CU3E4__this = __this;
		Il2CppCodeGenWriteBarrier((void**)(&L_3->___U3CU3E4__this), (void*)__this);
		U3CU3Ec__DisplayClass52_0_t9CED2D43B105C8CFAF5F15DEEF9A7644E9AD64D2* L_4 = V_0;
		Action_1_t3CB5D1A819C3ED3F99E9E39F890F18633253949A* L_5 = ___3_notify;
		NullCheck(L_4);
		L_4->___notify = L_5;
		Il2CppCodeGenWriteBarrier((void**)(&L_4->___notify), (void*)L_5);
		U3CU3Ec__DisplayClass52_0_t9CED2D43B105C8CFAF5F15DEEF9A7644E9AD64D2* L_6 = V_0;
		Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* L_7 = ___1_openSelectedLevel;
		NullCheck(L_6);
		L_6->___openSelectedLevel = L_7;
		Il2CppCodeGenWriteBarrier((void**)(&L_6->___openSelectedLevel), (void*)L_7);
		ProductionChapterMapView_Unbind_m4ECA5A192E8852161E53DDC9679091C8FE26B128(__this, NULL);
		__this->___committed = (bool)0;
		Button_t6786514A57F7AFDEE5431112FEA0CAB24F5AE098* L_8 = __this->___ProfileButton;
		U3CU3Ec__DisplayClass52_0_t9CED2D43B105C8CFAF5F15DEEF9A7644E9AD64D2* L_9 = V_0;
		Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* L_10 = (Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07*)il2cpp_codegen_object_new(Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07_il2cpp_TypeInfo_var);
		Action__ctor_mBDC7B0B4A3F583B64C2896F01BDED360772F67DC(L_10, L_9, (intptr_t)((void*)U3CU3Ec__DisplayClass52_0_U3CBindNavigationU3Eb__0_mA1084A0904F93E11D9B76B9853F3BA6FAADB48B1_RuntimeMethod_var), NULL);
		ProductionChapterMapView_Bind_m780E19FF014938C9FE24685DB50F82D58E0B3B12(__this, L_8, L_10, (bool)1, NULL);
		Button_t6786514A57F7AFDEE5431112FEA0CAB24F5AE098* L_11 = __this->___SettingsButton;
		U3CU3Ec__DisplayClass52_0_t9CED2D43B105C8CFAF5F15DEEF9A7644E9AD64D2* L_12 = V_0;
		Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* L_13 = (Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07*)il2cpp_codegen_object_new(Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07_il2cpp_TypeInfo_var);
		Action__ctor_mBDC7B0B4A3F583B64C2896F01BDED360772F67DC(L_13, L_12, (intptr_t)((void*)U3CU3Ec__DisplayClass52_0_U3CBindNavigationU3Eb__1_m93C80F4577F4BF57504E64A30E5849D538677C10_RuntimeMethod_var), NULL);
		ProductionChapterMapView_Bind_m780E19FF014938C9FE24685DB50F82D58E0B3B12(__this, L_11, L_13, (bool)1, NULL);
		Button_t6786514A57F7AFDEE5431112FEA0CAB24F5AE098* L_14 = __this->___BackButton;
		Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* L_15 = ___2_back;
		ProductionChapterMapView_Bind_m780E19FF014938C9FE24685DB50F82D58E0B3B12(__this, L_14, L_15, (bool)1, NULL);
		Button_t6786514A57F7AFDEE5431112FEA0CAB24F5AE098* L_16 = __this->___PlayButton;
		U3CU3Ec__DisplayClass52_0_t9CED2D43B105C8CFAF5F15DEEF9A7644E9AD64D2* L_17 = V_0;
		NullCheck(L_17);
		Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* L_18 = L_17->___openSelectedLevel;
		ProductionChapterMapView_Bind_m780E19FF014938C9FE24685DB50F82D58E0B3B12(__this, L_16, L_18, (bool)1, NULL);
		ProductionChapterMapScroller_t51327F0475F52DB572838C6E0792E7D7EBC90EA6* L_19 = __this->___Scroller;
		NullCheck(L_19);
		Button_t6786514A57F7AFDEE5431112FEA0CAB24F5AE098* L_20 = L_19->___CurrentButton;
		ProductionChapterMapScroller_t51327F0475F52DB572838C6E0792E7D7EBC90EA6* L_21 = __this->___Scroller;
		Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* L_22 = (Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07*)il2cpp_codegen_object_new(Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07_il2cpp_TypeInfo_var);
		Action__ctor_mBDC7B0B4A3F583B64C2896F01BDED360772F67DC(L_22, L_21, (intptr_t)((void*)ProductionChapterMapScroller_JumpToCurrent_mAADDDE9D4842765E918BE5553231C63440F1D169_RuntimeMethod_var), NULL);
		ProductionChapterMapView_Bind_m780E19FF014938C9FE24685DB50F82D58E0B3B12(__this, L_20, L_22, (bool)0, NULL);
		V_1 = 0;
		goto IL_00dd;
	}

IL_00aa:
	{
		U3CU3Ec__DisplayClass52_1_t0FC06C2BFBC14F6F7C3B38ACE4BE6AE9C0A95ACF* L_23 = (U3CU3Ec__DisplayClass52_1_t0FC06C2BFBC14F6F7C3B38ACE4BE6AE9C0A95ACF*)il2cpp_codegen_object_new(U3CU3Ec__DisplayClass52_1_t0FC06C2BFBC14F6F7C3B38ACE4BE6AE9C0A95ACF_il2cpp_TypeInfo_var);
		U3CU3Ec__DisplayClass52_1__ctor_m3A03703B3E968BA0DFA767986BE3FEB7A5FA65AB(L_23, NULL);
		V_2 = L_23;
		U3CU3Ec__DisplayClass52_1_t0FC06C2BFBC14F6F7C3B38ACE4BE6AE9C0A95ACF* L_24 = V_2;
		U3CU3Ec__DisplayClass52_0_t9CED2D43B105C8CFAF5F15DEEF9A7644E9AD64D2* L_25 = V_0;
		NullCheck(L_24);
		L_24->___CSU24U3CU3E8__locals1 = L_25;
		Il2CppCodeGenWriteBarrier((void**)(&L_24->___CSU24U3CU3E8__locals1), (void*)L_25);
		U3CU3Ec__DisplayClass52_1_t0FC06C2BFBC14F6F7C3B38ACE4BE6AE9C0A95ACF* L_26 = V_2;
		int32_t L_27 = V_1;
		NullCheck(L_26);
		L_26->___resource = L_27;
		ButtonU5BU5D_t0D512FF2FF5A72DAC04754F2C10182F850328BEF* L_28 = __this->___PlusButtons;
		int32_t L_29 = V_1;
		NullCheck(L_28);
		int32_t L_30 = L_29;
		Button_t6786514A57F7AFDEE5431112FEA0CAB24F5AE098* L_31 = (L_28)->GetAt(static_cast<il2cpp_array_size_t>(L_30));
		U3CU3Ec__DisplayClass52_1_t0FC06C2BFBC14F6F7C3B38ACE4BE6AE9C0A95ACF* L_32 = V_2;
		Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* L_33 = (Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07*)il2cpp_codegen_object_new(Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07_il2cpp_TypeInfo_var);
		Action__ctor_mBDC7B0B4A3F583B64C2896F01BDED360772F67DC(L_33, L_32, (intptr_t)((void*)U3CU3Ec__DisplayClass52_1_U3CBindNavigationU3Eb__2_m373DB10D8158B3AEEF0B9C6E29873332BD457DBA_RuntimeMethod_var), NULL);
		ProductionChapterMapView_Bind_m780E19FF014938C9FE24685DB50F82D58E0B3B12(__this, L_31, L_33, (bool)1, NULL);
		int32_t L_34 = V_1;
		V_1 = ((int32_t)il2cpp_codegen_add(L_34, 1));
	}

IL_00dd:
	{
		int32_t L_35 = V_1;
		if ((((int32_t)L_35) < ((int32_t)3)))
		{
			goto IL_00aa;
		}
	}
	{
		V_3 = 0;
		goto IL_0127;
	}

IL_00e5:
	{
		U3CU3Ec__DisplayClass52_2_t9F9A10E71F3E3EF236A645BA0B3196F07E438A8C* L_36 = (U3CU3Ec__DisplayClass52_2_t9F9A10E71F3E3EF236A645BA0B3196F07E438A8C*)il2cpp_codegen_object_new(U3CU3Ec__DisplayClass52_2_t9F9A10E71F3E3EF236A645BA0B3196F07E438A8C_il2cpp_TypeInfo_var);
		U3CU3Ec__DisplayClass52_2__ctor_m931FBA9B408FC71910DECDBC46E37126E1282748(L_36, NULL);
		V_4 = L_36;
		U3CU3Ec__DisplayClass52_2_t9F9A10E71F3E3EF236A645BA0B3196F07E438A8C* L_37 = V_4;
		U3CU3Ec__DisplayClass52_0_t9CED2D43B105C8CFAF5F15DEEF9A7644E9AD64D2* L_38 = V_0;
		NullCheck(L_37);
		L_37->___CSU24U3CU3E8__locals2 = L_38;
		Il2CppCodeGenWriteBarrier((void**)(&L_37->___CSU24U3CU3E8__locals2), (void*)L_38);
		U3CU3Ec__DisplayClass52_2_t9F9A10E71F3E3EF236A645BA0B3196F07E438A8C* L_39 = V_4;
		ChapterMapNodeU5BU5D_t15B97B03AAAED27F28F610C46E5A14348A93F929* L_40 = __this->___Nodes;
		int32_t L_41 = V_3;
		NullCheck(L_40);
		int32_t L_42 = L_41;
		ChapterMapNode_t6EE3B1AED87D25E8453712E8A8883B374A0129B0* L_43 = (L_40)->GetAt(static_cast<il2cpp_array_size_t>(L_42));
		NullCheck(L_39);
		L_39->___node = L_43;
		Il2CppCodeGenWriteBarrier((void**)(&L_39->___node), (void*)L_43);
		U3CU3Ec__DisplayClass52_2_t9F9A10E71F3E3EF236A645BA0B3196F07E438A8C* L_44 = V_4;
		NullCheck(L_44);
		ChapterMapNode_t6EE3B1AED87D25E8453712E8A8883B374A0129B0* L_45 = L_44->___node;
		NullCheck(L_45);
		Button_t6786514A57F7AFDEE5431112FEA0CAB24F5AE098* L_46 = L_45->___Button;
		U3CU3Ec__DisplayClass52_2_t9F9A10E71F3E3EF236A645BA0B3196F07E438A8C* L_47 = V_4;
		Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* L_48 = (Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07*)il2cpp_codegen_object_new(Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07_il2cpp_TypeInfo_var);
		Action__ctor_mBDC7B0B4A3F583B64C2896F01BDED360772F67DC(L_48, L_47, (intptr_t)((void*)U3CU3Ec__DisplayClass52_2_U3CBindNavigationU3Eb__3_m29C7B6BCADC6C94ACDFB4EAED844936A1BD3EAB4_RuntimeMethod_var), NULL);
		ProductionChapterMapView_Bind_m780E19FF014938C9FE24685DB50F82D58E0B3B12(__this, L_46, L_48, (bool)0, NULL);
		int32_t L_49 = V_3;
		V_3 = ((int32_t)il2cpp_codegen_add(L_49, 1));
	}

IL_0127:
	{
		int32_t L_50 = V_3;
		ChapterMapNodeU5BU5D_t15B97B03AAAED27F28F610C46E5A14348A93F929* L_51 = __this->___Nodes;
		NullCheck(L_51);
		int32_t L_52 = (il2cpp_codegen_conv<int32_t,int64_t,int64_t,false,false>((((RuntimeArray*)L_51)->max_length),NULL));
		if ((((int32_t)L_50) < ((int32_t)L_52)))
		{
			goto IL_00e5;
		}
	}
	{
		ProductionChapterMapView_set_ActionsBound_mF54082C047E957201ADF961571A513B0F7EFF60A_inline(__this, (bool)1, NULL);
		return;
	}
}
