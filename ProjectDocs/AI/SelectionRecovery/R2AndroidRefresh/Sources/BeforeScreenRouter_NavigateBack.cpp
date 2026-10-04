IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void ScreenRouter_NavigateBack_m8755B6CD7EDC8A0A4E6892C2EC74A718571E232B (ScreenRouter_t9EC367B45B1296F4CF0CBDEA57AEF86CC5340C00* __this, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Application_tDB03BE91CDF0ACA614A5E0B67CFB77C44EB19B21_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&GameObject_GetComponentInChildren_TisProductionCharactersView_t80FF719EBB00992075D821990BF966D5E95577DE_m78DB6C38217BAE039CE674B3E17BD05DFED5B883_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&GameObject_GetComponentInChildren_TisProductionFriendsListView_t568A80A4F587DE5E0D9156810474BD1B241353D1_m6F925C1CFEB8568058656E82EA523E4A8E17AB84_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&GameObject_GetComponentInChildren_TisProductionIndividualRankingView_tF449BDA2B9876849864F7F78C4C57CE0BB9726B3_mF6970CA9815117F55D1E1753E42D02AE5C2B90CE_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&GameObject_GetComponentInChildren_TisProductionInventoryView_tBFCB541C14E4ED40B2CB6633CE17AC0EB9D14E8E_m31D4B3D9DC5DA6B7B0C89F9A080C6C59993B9CC3_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&GameObject_GetComponentInChildren_TisProductionLoginView_t36729B7F5B697B3828DE80A1A0EB2509C7F66B81_mEF98EC1C6B2D581C59612058A763DD1DC035DFF5_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&GameObject_GetComponentInChildren_TisProductionMailView_t613C10918DAA8582972FFE0681462F4DAE73B966_mCA8281FDE2E17871A24E6899ABBB35F495AD58A9_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&GameObject_GetComponentInChildren_TisProductionProfileView_tFBAE39BD387DB9B67C01A51EB40718FD7461992D_mBA813B51212DD365F39C3E916E546EE66AB77092_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&GameObject_GetComponentInChildren_TisProductionSettingsView_t8B601F105D46E705808BBD9FA8B26DD7877703BC_mED8D0D4965CBE42D0BCE417CD1FAF493FDDC26E8_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&GameObject_GetComponentInChildren_TisProductionTeamChatView_t3876DDF380A2DA789CB223EB9E32A75578DFE07E_m44D1755932740500A249FF3E7E2C1EFDB3C06D08_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&GameObject_GetComponentInChildren_TisProductionTeamHubView_t242CA702D2D2A557A5145C582FE6955173320992_m616093F4088896ABD5FAD30CC28A725C50FAD70D_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&GameObject_GetComponentInChildren_TisProductionTeamRankingView_t6A2E723B48D2D1F086684E393EC71E7A19E82DD4_m9DBFF39E9DEB2EF3D3644EEA62DF2F17C33F2E15_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&GameObject_GetComponentInChildren_TisProductionTutorialView_t17A2D559FD4E71F841E57E107C3B8FADDC3DA4CB_m7D0309A9BADDE66BB452C63C5781EE1ED641BA53_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&GameObject_GetComponentInChildren_TisProductionWorldMapView_t9EE066392822F11A7943261785563B6FFB262CF7_m99972800DA31C3BF70B60B858FA0817A13CDFB41_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Nullable_1_GetValueOrDefault_m9E93E2CEA72E2689103A641157336A4F674A67A3_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Nullable_1_GetValueOrDefault_mCCA4F58C01C85CA318EE7C5DA8D366F3330CBD64_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Nullable_1_get_HasValue_mF10B2CE8C739C005123C719AF092180FAC34D0E1_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Nullable_1_get_Value_m91C7AE54C6A0782DCB2FF528979CC3450C25652F_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ScreenRouter_U3CNavigateBackU3Eb__70_0_m84D24FCE1B079B4614F67FDAE5A1BD141B1DC387_RuntimeMethod_var);
		s_Il2CppMethodInitialized = true;
	}
	int32_t V_0 = 0;
	ProductionTutorialView_t17A2D559FD4E71F841E57E107C3B8FADDC3DA4CB* V_1 = NULL;
	ProductionTutorialView_t17A2D559FD4E71F841E57E107C3B8FADDC3DA4CB* V_2 = NULL;
	ProductionLoginView_t36729B7F5B697B3828DE80A1A0EB2509C7F66B81* V_3 = NULL;
	ProductionSettingsView_t8B601F105D46E705808BBD9FA8B26DD7877703BC* V_4 = NULL;
	ProductionSettingsView_t8B601F105D46E705808BBD9FA8B26DD7877703BC* V_5 = NULL;
	ProductionCharactersView_t80FF719EBB00992075D821990BF966D5E95577DE* V_6 = NULL;
	ProductionInventoryView_tBFCB541C14E4ED40B2CB6633CE17AC0EB9D14E8E* V_7 = NULL;
	ProductionProfileView_tFBAE39BD387DB9B67C01A51EB40718FD7461992D* V_8 = NULL;
	ProductionMailView_t613C10918DAA8582972FFE0681462F4DAE73B966* V_9 = NULL;
	ProductionFriendsListView_t568A80A4F587DE5E0D9156810474BD1B241353D1* V_10 = NULL;
	ProductionTeamChatView_t3876DDF380A2DA789CB223EB9E32A75578DFE07E* V_11 = NULL;
	ProductionTeamRankingView_t6A2E723B48D2D1F086684E393EC71E7A19E82DD4* V_12 = NULL;
	ProductionTeamHubView_t242CA702D2D2A557A5145C582FE6955173320992* V_13 = NULL;
	ProductionIndividualRankingView_tF449BDA2B9876849864F7F78C4C57CE0BB9726B3* V_14 = NULL;
	ProductionWorldMapView_t9EE066392822F11A7943261785563B6FFB262CF7* V_15 = NULL;
	{
		Nullable_1_t4FCFD27B7FC762CFC2F64096CD6201A0B0AF4894* L_0 = (Nullable_1_t4FCFD27B7FC762CFC2F64096CD6201A0B0AF4894*)(&__this->___activeModalId);
		int32_t L_1;
		L_1 = Nullable_1_GetValueOrDefault_m9E93E2CEA72E2689103A641157336A4F674A67A3_inline(L_0, Nullable_1_GetValueOrDefault_m9E93E2CEA72E2689103A641157336A4F674A67A3_RuntimeMethod_var);
		if ((!(((uint32_t)L_1) == ((uint32_t)6))))
		{
			goto IL_001d;
		}
	}
	{
		bool L_2;
		L_2 = ScreenRouter_get_IsModalTransitioning_mA7EE64426C2CB00227FEC49004C2775C8016F40A(__this, NULL);
		if (L_2)
		{
			goto IL_001c;
		}
	}
	{
		ScreenRouter_CloseModal_mDA5B2C06AE67DE089FA24083FA5781E5976D607B(__this, NULL);
	}

IL_001c:
	{
		return;
	}

IL_001d:
	{
		Nullable_1_t4FCFD27B7FC762CFC2F64096CD6201A0B0AF4894* L_3 = (Nullable_1_t4FCFD27B7FC762CFC2F64096CD6201A0B0AF4894*)(&__this->___activeModalId);
		int32_t L_4;
		L_4 = Nullable_1_GetValueOrDefault_m9E93E2CEA72E2689103A641157336A4F674A67A3_inline(L_3, Nullable_1_GetValueOrDefault_m9E93E2CEA72E2689103A641157336A4F674A67A3_RuntimeMethod_var);
		if ((!(((uint32_t)L_4) == ((uint32_t)((int32_t)36)))))
		{
			goto IL_0086;
		}
	}
	{
		bool L_5;
		L_5 = ScreenRouter_get_IsModalTransitioning_mA7EE64426C2CB00227FEC49004C2775C8016F40A(__this, NULL);
		if (L_5)
		{
			goto IL_0085;
		}
	}
	{
		LevelTutorialPresenter_t294D830AC137EAA461F72B2C73979D74C2FF1254* L_6 = __this->___tutorialErrorOwner;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_7;
		L_7 = Object_op_Inequality_mD0BE578448EAA61948F25C32F8DD55AB1F778602(L_6, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (!L_7)
		{
			goto IL_0049;
		}
	}
	{
		ScreenRouter_CloseModal_mDA5B2C06AE67DE089FA24083FA5781E5976D607B(__this, NULL);
		return;
	}

IL_0049:
	{
		Match3Board_tE4CAA58AF06191DC578D60DE5FA7659B8C8ACD69* L_8 = __this->___activeBoard;
		bool L_9;
		L_9 = ScreenRouter_IsGameplayInventoryError_m47226D732980E626CF68E184A9841D891357D8A1(__this, L_8, NULL);
		if (!L_9)
		{
			goto IL_0064;
		}
	}
	{
		Match3Board_tE4CAA58AF06191DC578D60DE5FA7659B8C8ACD69* L_10 = __this->___activeBoard;
		ScreenRouter_CloseGameplayInventoryError_mCCAB0FFFBFE2623F3DB7AC6A3ADA0D87E011F61B(__this, L_10, NULL);
		return;
	}

IL_0064:
	{
		Match3Board_tE4CAA58AF06191DC578D60DE5FA7659B8C8ACD69* L_11 = __this->___activeBoard;
		bool L_12;
		L_12 = ScreenRouter_IsPendingGameplayVictory_m5F15794588383298DAEC0C365A0B18E172F84F3F(__this, L_11, NULL);
		if (!L_12)
		{
			goto IL_007f;
		}
	}
	{
		Match3Board_tE4CAA58AF06191DC578D60DE5FA7659B8C8ACD69* L_13 = __this->___activeBoard;
		ScreenRouter_CloseRejectedGameplayVictory_mFB79D73676B39FE240F55DE90D455B6015A28B8B(__this, L_13, NULL);
		return;
	}

IL_007f:
	{
		ScreenRouter_CloseModal_mDA5B2C06AE67DE089FA24083FA5781E5976D607B(__this, NULL);
	}

IL_0085:
	{
		return;
	}

IL_0086:
	{
		Nullable_1_t4FCFD27B7FC762CFC2F64096CD6201A0B0AF4894* L_14 = (Nullable_1_t4FCFD27B7FC762CFC2F64096CD6201A0B0AF4894*)(&__this->___activeModalId);
		int32_t L_15;
		L_15 = Nullable_1_GetValueOrDefault_m9E93E2CEA72E2689103A641157336A4F674A67A3_inline(L_14, Nullable_1_GetValueOrDefault_m9E93E2CEA72E2689103A641157336A4F674A67A3_RuntimeMethod_var);
		if ((!(((uint32_t)L_15) == ((uint32_t)((int32_t)33)))))
		{
			goto IL_00cf;
		}
	}
	{
		bool L_16;
		L_16 = ScreenRouter_get_IsModalTransitioning_mA7EE64426C2CB00227FEC49004C2775C8016F40A(__this, NULL);
		if (!L_16)
		{
			goto IL_009e;
		}
	}
	{
		return;
	}

IL_009e:
	{
		GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_17 = __this->___modalVisualRoot;
		NullCheck(L_17);
		ProductionTutorialView_t17A2D559FD4E71F841E57E107C3B8FADDC3DA4CB* L_18;
		L_18 = GameObject_GetComponentInChildren_TisProductionTutorialView_t17A2D559FD4E71F841E57E107C3B8FADDC3DA4CB_m7D0309A9BADDE66BB452C63C5781EE1ED641BA53(L_17, GameObject_GetComponentInChildren_TisProductionTutorialView_t17A2D559FD4E71F841E57E107C3B8FADDC3DA4CB_m7D0309A9BADDE66BB452C63C5781EE1ED641BA53_RuntimeMethod_var);
		V_1 = L_18;
		ProductionTutorialView_t17A2D559FD4E71F841E57E107C3B8FADDC3DA4CB* L_19 = V_1;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_20;
		L_20 = Object_op_Inequality_mD0BE578448EAA61948F25C32F8DD55AB1F778602(L_19, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (!L_20)
		{
			goto IL_00bc;
		}
	}
	{
		ProductionTutorialView_t17A2D559FD4E71F841E57E107C3B8FADDC3DA4CB* L_21 = V_1;
		NullCheck(L_21);
		bool L_22;
		L_22 = ProductionTutorialView_TryDismissDetails_mC381A9E7EA352E22F41A6E015BD1D3B72584C9DB(L_21, NULL);
		if (!L_22)
		{
			goto IL_00bc;
		}
	}
	{
		return;
	}

IL_00bc:
	{
		Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* L_23 = (Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07*)il2cpp_codegen_object_new(Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07_il2cpp_TypeInfo_var);
		Action__ctor_mBDC7B0B4A3F583B64C2896F01BDED360772F67DC(L_23, __this, (intptr_t)((void*)ScreenRouter_U3CNavigateBackU3Eb__70_0_m84D24FCE1B079B4614F67FDAE5A1BD141B1DC387_RuntimeMethod_var), NULL);
		ScreenRouter_RequestCloseModal_m54358FF9D470A8E2D5A80FEA049E40084071FE10(__this, L_23, NULL);
		return;
	}

IL_00cf:
	{
		int32_t L_24 = __this->___current;
		if ((!(((uint32_t)L_24) == ((uint32_t)((int32_t)33)))))
		{
			goto IL_010b;
		}
	}
	{
		GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_25 = __this->___contentRoot;
		NullCheck(L_25);
		ProductionTutorialView_t17A2D559FD4E71F841E57E107C3B8FADDC3DA4CB* L_26;
		L_26 = GameObject_GetComponentInChildren_TisProductionTutorialView_t17A2D559FD4E71F841E57E107C3B8FADDC3DA4CB_m7D0309A9BADDE66BB452C63C5781EE1ED641BA53(L_25, GameObject_GetComponentInChildren_TisProductionTutorialView_t17A2D559FD4E71F841E57E107C3B8FADDC3DA4CB_m7D0309A9BADDE66BB452C63C5781EE1ED641BA53_RuntimeMethod_var);
		V_2 = L_26;
		ProductionTutorialView_t17A2D559FD4E71F841E57E107C3B8FADDC3DA4CB* L_27 = V_2;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_28;
		L_28 = Object_op_Inequality_mD0BE578448EAA61948F25C32F8DD55AB1F778602(L_27, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (!L_28)
		{
			goto IL_00f7;
		}
	}
	{
		ProductionTutorialView_t17A2D559FD4E71F841E57E107C3B8FADDC3DA4CB* L_29 = V_2;
		NullCheck(L_29);
		bool L_30;
		L_30 = ProductionTutorialView_TryDismissDetails_mC381A9E7EA352E22F41A6E015BD1D3B72584C9DB(L_29, NULL);
		if (!L_30)
		{
			goto IL_00f7;
		}
	}
	{
		return;
	}

IL_00f7:
	{
		Nullable_1_t4FCFD27B7FC762CFC2F64096CD6201A0B0AF4894* L_31 = (Nullable_1_t4FCFD27B7FC762CFC2F64096CD6201A0B0AF4894*)(&__this->___tutorialReturnPage);
		int32_t L_32;
		L_32 = Nullable_1_GetValueOrDefault_mCCA4F58C01C85CA318EE7C5DA8D366F3330CBD64(L_31, ((int32_t)22), Nullable_1_GetValueOrDefault_mCCA4F58C01C85CA318EE7C5DA8D366F3330CBD64_RuntimeMethod_var);
		ScreenRouter_Show_mB289A1408F5F71A5043EFE930648595D5DCE3734(__this, L_32, NULL);
		return;
	}

IL_010b:
	{
		int32_t L_33 = __this->___current;
		if ((!(((uint32_t)L_33) == ((uint32_t)((int32_t)32)))))
		{
			goto IL_0155;
		}
	}
	{
		GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_34 = __this->___contentRoot;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_35;
		L_35 = Object_op_Inequality_mD0BE578448EAA61948F25C32F8DD55AB1F778602(L_34, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (!L_35)
		{
			goto IL_0155;
		}
	}
	{
		GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_36 = __this->___contentRoot;
		NullCheck(L_36);
		ProductionLoginView_t36729B7F5B697B3828DE80A1A0EB2509C7F66B81* L_37;
		L_37 = GameObject_GetComponentInChildren_TisProductionLoginView_t36729B7F5B697B3828DE80A1A0EB2509C7F66B81_mEF98EC1C6B2D581C59612058A763DD1DC035DFF5(L_36, GameObject_GetComponentInChildren_TisProductionLoginView_t36729B7F5B697B3828DE80A1A0EB2509C7F66B81_mEF98EC1C6B2D581C59612058A763DD1DC035DFF5_RuntimeMethod_var);
		V_3 = L_37;
		ProductionLoginView_t36729B7F5B697B3828DE80A1A0EB2509C7F66B81* L_38 = V_3;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_39;
		L_39 = Object_op_Inequality_mD0BE578448EAA61948F25C32F8DD55AB1F778602(L_38, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (!L_39)
		{
			goto IL_0141;
		}
	}
	{
		ProductionLoginView_t36729B7F5B697B3828DE80A1A0EB2509C7F66B81* L_40 = V_3;
		NullCheck(L_40);
		bool L_41;
		L_41 = ProductionLoginView_TryDismiss_mBE7B3276892B92DCF1CB2CBF5F1FB9F5BC3A4809(L_40, NULL);
		if (!L_41)
		{
			goto IL_0141;
		}
	}
	{
		return;
	}

IL_0141:
	{
		Nullable_1_t4FCFD27B7FC762CFC2F64096CD6201A0B0AF4894* L_42 = (Nullable_1_t4FCFD27B7FC762CFC2F64096CD6201A0B0AF4894*)(&__this->___loginReturnPage);
		int32_t L_43;
		L_43 = Nullable_1_GetValueOrDefault_mCCA4F58C01C85CA318EE7C5DA8D366F3330CBD64(L_42, ((int32_t)22), Nullable_1_GetValueOrDefault_mCCA4F58C01C85CA318EE7C5DA8D366F3330CBD64_RuntimeMethod_var);
		ScreenRouter_Show_mB289A1408F5F71A5043EFE930648595D5DCE3734(__this, L_43, NULL);
		return;
	}

IL_0155:
	{
		int32_t L_44 = __this->___current;
		if ((!(((uint32_t)L_44) == ((uint32_t)((int32_t)31)))))
		{
			goto IL_0173;
		}
	}
	{
		Nullable_1_t4FCFD27B7FC762CFC2F64096CD6201A0B0AF4894* L_45 = (Nullable_1_t4FCFD27B7FC762CFC2F64096CD6201A0B0AF4894*)(&__this->___languageReturnPage);
		int32_t L_46;
		L_46 = Nullable_1_GetValueOrDefault_mCCA4F58C01C85CA318EE7C5DA8D366F3330CBD64(L_45, ((int32_t)22), Nullable_1_GetValueOrDefault_mCCA4F58C01C85CA318EE7C5DA8D366F3330CBD64_RuntimeMethod_var);
		ScreenRouter_Show_mB289A1408F5F71A5043EFE930648595D5DCE3734(__this, L_46, NULL);
		return;
	}

IL_0173:
	{
		Nullable_1_t4FCFD27B7FC762CFC2F64096CD6201A0B0AF4894* L_47 = (Nullable_1_t4FCFD27B7FC762CFC2F64096CD6201A0B0AF4894*)(&__this->___activeModalId);
		int32_t L_48;
		L_48 = Nullable_1_GetValueOrDefault_m9E93E2CEA72E2689103A641157336A4F674A67A3_inline(L_47, Nullable_1_GetValueOrDefault_m9E93E2CEA72E2689103A641157336A4F674A67A3_RuntimeMethod_var);
		if ((!(((uint32_t)L_48) == ((uint32_t)((int32_t)16)))))
		{
			goto IL_01cc;
		}
	}
	{
		bool L_49;
		L_49 = ScreenRouter_get_IsModalTransitioning_mA7EE64426C2CB00227FEC49004C2775C8016F40A(__this, NULL);
		if (!L_49)
		{
			goto IL_018b;
		}
	}
	{
		return;
	}

IL_018b:
	{
		GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_50 = __this->___modalVisualRoot;
		NullCheck(L_50);
		ProductionSettingsView_t8B601F105D46E705808BBD9FA8B26DD7877703BC* L_51;
		L_51 = GameObject_GetComponentInChildren_TisProductionSettingsView_t8B601F105D46E705808BBD9FA8B26DD7877703BC_mED8D0D4965CBE42D0BCE417CD1FAF493FDDC26E8(L_50, GameObject_GetComponentInChildren_TisProductionSettingsView_t8B601F105D46E705808BBD9FA8B26DD7877703BC_mED8D0D4965CBE42D0BCE417CD1FAF493FDDC26E8_RuntimeMethod_var);
		V_4 = L_51;
		ProductionSettingsView_t8B601F105D46E705808BBD9FA8B26DD7877703BC* L_52 = V_4;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_53;
		L_53 = Object_op_Inequality_mD0BE578448EAA61948F25C32F8DD55AB1F778602(L_52, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (!L_53)
		{
			goto IL_01ac;
		}
	}
	{
		ProductionSettingsView_t8B601F105D46E705808BBD9FA8B26DD7877703BC* L_54 = V_4;
		NullCheck(L_54);
		bool L_55;
		L_55 = ProductionSettingsView_TryDismiss_m6F0F859CCDBA358CB78306FF2900535538C4501C(L_54, NULL);
		if (!L_55)
		{
			goto IL_01ac;
		}
	}
	{
		return;
	}

IL_01ac:
	{
		bool L_56 = __this->___settingsReturnToPause;
		if (!L_56)
		{
			goto IL_01c5;
		}
	}
	{
		__this->___settingsReturnToPause = (bool)0;
		bool L_57;
		L_57 = ScreenRouter_ShowModal_m195E7C54FEA3162B7A49F9BF61E3CA321B3AE5D9(__this, ((int32_t)17), NULL);
		return;
	}

IL_01c5:
	{
		ScreenRouter_CloseModal_mDA5B2C06AE67DE089FA24083FA5781E5976D607B(__this, NULL);
		return;
	}

IL_01cc:
	{
		int32_t L_58 = __this->___current;
		if ((!(((uint32_t)L_58) == ((uint32_t)((int32_t)16)))))
		{
			goto IL_0218;
		}
	}
	{
		GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_59 = __this->___contentRoot;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_60;
		L_60 = Object_op_Inequality_mD0BE578448EAA61948F25C32F8DD55AB1F778602(L_59, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (!L_60)
		{
			goto IL_0218;
		}
	}
	{
		GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_61 = __this->___contentRoot;
		NullCheck(L_61);
		ProductionSettingsView_t8B601F105D46E705808BBD9FA8B26DD7877703BC* L_62;
		L_62 = GameObject_GetComponentInChildren_TisProductionSettingsView_t8B601F105D46E705808BBD9FA8B26DD7877703BC_mED8D0D4965CBE42D0BCE417CD1FAF493FDDC26E8(L_61, GameObject_GetComponentInChildren_TisProductionSettingsView_t8B601F105D46E705808BBD9FA8B26DD7877703BC_mED8D0D4965CBE42D0BCE417CD1FAF493FDDC26E8_RuntimeMethod_var);
		V_5 = L_62;
		ProductionSettingsView_t8B601F105D46E705808BBD9FA8B26DD7877703BC* L_63 = V_5;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_64;
		L_64 = Object_op_Inequality_mD0BE578448EAA61948F25C32F8DD55AB1F778602(L_63, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (!L_64)
		{
			goto IL_0205;
		}
	}
	{
		ProductionSettingsView_t8B601F105D46E705808BBD9FA8B26DD7877703BC* L_65 = V_5;
		NullCheck(L_65);
		bool L_66;
		L_66 = ProductionSettingsView_TryDismiss_m6F0F859CCDBA358CB78306FF2900535538C4501C(L_65, NULL);
		if (!L_66)
		{
			goto IL_0205;
		}
	}
	{
		return;
	}

IL_0205:
	{
		Nullable_1_t4FCFD27B7FC762CFC2F64096CD6201A0B0AF4894* L_67 = (Nullable_1_t4FCFD27B7FC762CFC2F64096CD6201A0B0AF4894*)(&__this->___settingsReturnPage);
		int32_t L_68;
		L_68 = Nullable_1_GetValueOrDefault_mCCA4F58C01C85CA318EE7C5DA8D366F3330CBD64(L_67, 1, Nullable_1_GetValueOrDefault_mCCA4F58C01C85CA318EE7C5DA8D366F3330CBD64_RuntimeMethod_var);
		ScreenRouter_Show_mB289A1408F5F71A5043EFE930648595D5DCE3734(__this, L_68, NULL);
		return;
	}

IL_0218:
	{
		int32_t L_69 = __this->___current;
		if ((!(((uint32_t)L_69) == ((uint32_t)((int32_t)18)))))
		{
			goto IL_0251;
		}
	}
	{
		GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_70 = __this->___contentRoot;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_71;
		L_71 = Object_op_Inequality_mD0BE578448EAA61948F25C32F8DD55AB1F778602(L_70, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (!L_71)
		{
			goto IL_0251;
		}
	}
	{
		GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_72 = __this->___contentRoot;
		NullCheck(L_72);
		ProductionCharactersView_t80FF719EBB00992075D821990BF966D5E95577DE* L_73;
		L_73 = GameObject_GetComponentInChildren_TisProductionCharactersView_t80FF719EBB00992075D821990BF966D5E95577DE_m78DB6C38217BAE039CE674B3E17BD05DFED5B883(L_72, GameObject_GetComponentInChildren_TisProductionCharactersView_t80FF719EBB00992075D821990BF966D5E95577DE_m78DB6C38217BAE039CE674B3E17BD05DFED5B883_RuntimeMethod_var);
		V_6 = L_73;
		ProductionCharactersView_t80FF719EBB00992075D821990BF966D5E95577DE* L_74 = V_6;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_75;
		L_75 = Object_op_Inequality_mD0BE578448EAA61948F25C32F8DD55AB1F778602(L_74, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (!L_75)
		{
			goto IL_0251;
		}
	}
	{
		ProductionCharactersView_t80FF719EBB00992075D821990BF966D5E95577DE* L_76 = V_6;
		NullCheck(L_76);
		bool L_77;
		L_77 = ProductionCharactersView_TryDismiss_m618086950BBC574C8EBC829EB41AADF64ADEA819(L_76, NULL);
		if (!L_77)
		{
			goto IL_0251;
		}
	}
	{
		return;
	}

IL_0251:
	{
		int32_t L_78 = __this->___current;
		if ((!(((uint32_t)L_78) == ((uint32_t)((int32_t)29)))))
		{
			goto IL_028a;
		}
	}
	{
		GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_79 = __this->___contentRoot;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_80;
		L_80 = Object_op_Inequality_mD0BE578448EAA61948F25C32F8DD55AB1F778602(L_79, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (!L_80)
		{
			goto IL_028a;
		}
	}
	{
		GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_81 = __this->___contentRoot;
		NullCheck(L_81);
		ProductionInventoryView_tBFCB541C14E4ED40B2CB6633CE17AC0EB9D14E8E* L_82;
		L_82 = GameObject_GetComponentInChildren_TisProductionInventoryView_tBFCB541C14E4ED40B2CB6633CE17AC0EB9D14E8E_m31D4B3D9DC5DA6B7B0C89F9A080C6C59993B9CC3(L_81, GameObject_GetComponentInChildren_TisProductionInventoryView_tBFCB541C14E4ED40B2CB6633CE17AC0EB9D14E8E_m31D4B3D9DC5DA6B7B0C89F9A080C6C59993B9CC3_RuntimeMethod_var);
		V_7 = L_82;
		ProductionInventoryView_tBFCB541C14E4ED40B2CB6633CE17AC0EB9D14E8E* L_83 = V_7;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_84;
		L_84 = Object_op_Inequality_mD0BE578448EAA61948F25C32F8DD55AB1F778602(L_83, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (!L_84)
		{
			goto IL_028a;
		}
	}
	{
		ProductionInventoryView_tBFCB541C14E4ED40B2CB6633CE17AC0EB9D14E8E* L_85 = V_7;
		NullCheck(L_85);
		bool L_86;
		L_86 = ProductionInventoryView_TryDismiss_mF4348FD34E0CF2A5EEA0913CFA533E640648FD82(L_85, NULL);
		if (!L_86)
		{
			goto IL_028a;
		}
	}
	{
		return;
	}

IL_028a:
	{
		int32_t L_87 = __this->___current;
		if ((!(((uint32_t)L_87) == ((uint32_t)((int32_t)15)))))
		{
			goto IL_02c3;
		}
	}
	{
		GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_88 = __this->___contentRoot;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_89;
		L_89 = Object_op_Inequality_mD0BE578448EAA61948F25C32F8DD55AB1F778602(L_88, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (!L_89)
		{
			goto IL_02c3;
		}
	}
	{
		GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_90 = __this->___contentRoot;
		NullCheck(L_90);
		ProductionProfileView_tFBAE39BD387DB9B67C01A51EB40718FD7461992D* L_91;
		L_91 = GameObject_GetComponentInChildren_TisProductionProfileView_tFBAE39BD387DB9B67C01A51EB40718FD7461992D_mBA813B51212DD365F39C3E916E546EE66AB77092(L_90, GameObject_GetComponentInChildren_TisProductionProfileView_tFBAE39BD387DB9B67C01A51EB40718FD7461992D_mBA813B51212DD365F39C3E916E546EE66AB77092_RuntimeMethod_var);
		V_8 = L_91;
		ProductionProfileView_tFBAE39BD387DB9B67C01A51EB40718FD7461992D* L_92 = V_8;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_93;
		L_93 = Object_op_Inequality_mD0BE578448EAA61948F25C32F8DD55AB1F778602(L_92, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (!L_93)
		{
			goto IL_02c3;
		}
	}
	{
		ProductionProfileView_tFBAE39BD387DB9B67C01A51EB40718FD7461992D* L_94 = V_8;
		NullCheck(L_94);
		bool L_95;
		L_95 = ProductionProfileView_TryDismiss_m576EDBFD85433786E22AD0DFCA9DA8BA1AAB8B76(L_94, NULL);
		if (!L_95)
		{
			goto IL_02c3;
		}
	}
	{
		return;
	}

IL_02c3:
	{
		int32_t L_96 = __this->___current;
		if ((!(((uint32_t)L_96) == ((uint32_t)((int32_t)19)))))
		{
			goto IL_02fc;
		}
	}
	{
		GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_97 = __this->___contentRoot;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_98;
		L_98 = Object_op_Inequality_mD0BE578448EAA61948F25C32F8DD55AB1F778602(L_97, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (!L_98)
		{
			goto IL_02fc;
		}
	}
	{
		GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_99 = __this->___contentRoot;
		NullCheck(L_99);
		ProductionMailView_t613C10918DAA8582972FFE0681462F4DAE73B966* L_100;
		L_100 = GameObject_GetComponentInChildren_TisProductionMailView_t613C10918DAA8582972FFE0681462F4DAE73B966_mCA8281FDE2E17871A24E6899ABBB35F495AD58A9(L_99, GameObject_GetComponentInChildren_TisProductionMailView_t613C10918DAA8582972FFE0681462F4DAE73B966_mCA8281FDE2E17871A24E6899ABBB35F495AD58A9_RuntimeMethod_var);
		V_9 = L_100;
		ProductionMailView_t613C10918DAA8582972FFE0681462F4DAE73B966* L_101 = V_9;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_102;
		L_102 = Object_op_Inequality_mD0BE578448EAA61948F25C32F8DD55AB1F778602(L_101, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (!L_102)
		{
			goto IL_02fc;
		}
	}
	{
		ProductionMailView_t613C10918DAA8582972FFE0681462F4DAE73B966* L_103 = V_9;
		NullCheck(L_103);
		bool L_104;
		L_104 = ProductionMailView_TryDismiss_mE3937AB247F9305BB611FBFC6799FF355D4FC345(L_103, NULL);
		if (!L_104)
		{
			goto IL_02fc;
		}
	}
	{
		return;
	}

IL_02fc:
	{
		int32_t L_105 = __this->___current;
		if ((!(((uint32_t)L_105) == ((uint32_t)((int32_t)28)))))
		{
			goto IL_0351;
		}
	}
	{
		GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_106 = __this->___contentRoot;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_107;
		L_107 = Object_op_Inequality_mD0BE578448EAA61948F25C32F8DD55AB1F778602(L_106, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (!L_107)
		{
			goto IL_0351;
		}
	}
	{
		GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_108 = __this->___contentRoot;
		NullCheck(L_108);
		ProductionFriendsListView_t568A80A4F587DE5E0D9156810474BD1B241353D1* L_109;
		L_109 = GameObject_GetComponentInChildren_TisProductionFriendsListView_t568A80A4F587DE5E0D9156810474BD1B241353D1_m6F925C1CFEB8568058656E82EA523E4A8E17AB84(L_108, GameObject_GetComponentInChildren_TisProductionFriendsListView_t568A80A4F587DE5E0D9156810474BD1B241353D1_m6F925C1CFEB8568058656E82EA523E4A8E17AB84_RuntimeMethod_var);
		V_10 = L_109;
		ProductionFriendsListView_t568A80A4F587DE5E0D9156810474BD1B241353D1* L_110 = V_10;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_111;
		L_111 = Object_op_Inequality_mD0BE578448EAA61948F25C32F8DD55AB1F778602(L_110, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (!L_111)
		{
			goto IL_0341;
		}
	}
	{
		ProductionFriendsListView_t568A80A4F587DE5E0D9156810474BD1B241353D1* L_112 = V_10;
		NullCheck(L_112);
		GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_113 = L_112->___InfoPanel;
		NullCheck(L_113);
		bool L_114;
		L_114 = GameObject_get_activeSelf_m4F3E5240E138B66AAA080EA30759A3D0517DA368(L_113, NULL);
		if (!L_114)
		{
			goto IL_0341;
		}
	}
	{
		ProductionFriendsListView_t568A80A4F587DE5E0D9156810474BD1B241353D1* L_115 = V_10;
		NullCheck(L_115);
		ProductionFriendsListView_CloseInfo_m59B08AED32B47FF3B03A2C8B6F82070FC917824C(L_115, NULL);
		return;
	}

IL_0341:
	{
		bool L_116 = __this->___friendsReturnToHome;
		if (!L_116)
		{
			goto IL_0351;
		}
	}
	{
		ScreenRouter_Show_mB289A1408F5F71A5043EFE930648595D5DCE3734(__this, 1, NULL);
		return;
	}

IL_0351:
	{
		int32_t L_117 = __this->___current;
		if ((!(((uint32_t)L_117) == ((uint32_t)((int32_t)27)))))
		{
			goto IL_0396;
		}
	}
	{
		GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_118 = __this->___contentRoot;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_119;
		L_119 = Object_op_Inequality_mD0BE578448EAA61948F25C32F8DD55AB1F778602(L_118, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (!L_119)
		{
			goto IL_0396;
		}
	}
	{
		GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_120 = __this->___contentRoot;
		NullCheck(L_120);
		ProductionTeamChatView_t3876DDF380A2DA789CB223EB9E32A75578DFE07E* L_121;
		L_121 = GameObject_GetComponentInChildren_TisProductionTeamChatView_t3876DDF380A2DA789CB223EB9E32A75578DFE07E_m44D1755932740500A249FF3E7E2C1EFDB3C06D08(L_120, GameObject_GetComponentInChildren_TisProductionTeamChatView_t3876DDF380A2DA789CB223EB9E32A75578DFE07E_m44D1755932740500A249FF3E7E2C1EFDB3C06D08_RuntimeMethod_var);
		V_11 = L_121;
		ProductionTeamChatView_t3876DDF380A2DA789CB223EB9E32A75578DFE07E* L_122 = V_11;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_123;
		L_123 = Object_op_Inequality_mD0BE578448EAA61948F25C32F8DD55AB1F778602(L_122, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (!L_123)
		{
			goto IL_0396;
		}
	}
	{
		ProductionTeamChatView_t3876DDF380A2DA789CB223EB9E32A75578DFE07E* L_124 = V_11;
		NullCheck(L_124);
		GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_125 = L_124->___InfoPanel;
		NullCheck(L_125);
		bool L_126;
		L_126 = GameObject_get_activeSelf_m4F3E5240E138B66AAA080EA30759A3D0517DA368(L_125, NULL);
		if (!L_126)
		{
			goto IL_0396;
		}
	}
	{
		ProductionTeamChatView_t3876DDF380A2DA789CB223EB9E32A75578DFE07E* L_127 = V_11;
		NullCheck(L_127);
		ProductionTeamChatView_CloseInfo_mEADF40386B9F498FBFB3F86F412D15CB407563B2(L_127, NULL);
		return;
	}

IL_0396:
	{
		int32_t L_128 = __this->___current;
		if ((!(((uint32_t)L_128) == ((uint32_t)((int32_t)26)))))
		{
			goto IL_03db;
		}
	}
	{
		GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_129 = __this->___contentRoot;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_130;
		L_130 = Object_op_Inequality_mD0BE578448EAA61948F25C32F8DD55AB1F778602(L_129, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (!L_130)
		{
			goto IL_03db;
		}
	}
	{
		GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_131 = __this->___contentRoot;
		NullCheck(L_131);
		ProductionTeamRankingView_t6A2E723B48D2D1F086684E393EC71E7A19E82DD4* L_132;
		L_132 = GameObject_GetComponentInChildren_TisProductionTeamRankingView_t6A2E723B48D2D1F086684E393EC71E7A19E82DD4_m9DBFF39E9DEB2EF3D3644EEA62DF2F17C33F2E15(L_131, GameObject_GetComponentInChildren_TisProductionTeamRankingView_t6A2E723B48D2D1F086684E393EC71E7A19E82DD4_m9DBFF39E9DEB2EF3D3644EEA62DF2F17C33F2E15_RuntimeMethod_var);
		V_12 = L_132;
		ProductionTeamRankingView_t6A2E723B48D2D1F086684E393EC71E7A19E82DD4* L_133 = V_12;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_134;
		L_134 = Object_op_Inequality_mD0BE578448EAA61948F25C32F8DD55AB1F778602(L_133, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (!L_134)
		{
			goto IL_03db;
		}
	}
	{
		ProductionTeamRankingView_t6A2E723B48D2D1F086684E393EC71E7A19E82DD4* L_135 = V_12;
		NullCheck(L_135);
		GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_136 = L_135->___InfoPanel;
		NullCheck(L_136);
		bool L_137;
		L_137 = GameObject_get_activeSelf_m4F3E5240E138B66AAA080EA30759A3D0517DA368(L_136, NULL);
		if (!L_137)
		{
			goto IL_03db;
		}
	}
	{
		ProductionTeamRankingView_t6A2E723B48D2D1F086684E393EC71E7A19E82DD4* L_138 = V_12;
		NullCheck(L_138);
		ProductionTeamRankingView_CloseInfo_m459BDB863ADD3EEC282776CD0509318619CFD880(L_138, NULL);
		return;
	}

IL_03db:
	{
		int32_t L_139 = __this->___current;
		if ((!(((uint32_t)L_139) == ((uint32_t)((int32_t)25)))))
		{
			goto IL_0420;
		}
	}
	{
		GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_140 = __this->___contentRoot;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_141;
		L_141 = Object_op_Inequality_mD0BE578448EAA61948F25C32F8DD55AB1F778602(L_140, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (!L_141)
		{
			goto IL_0420;
		}
	}
	{
		GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_142 = __this->___contentRoot;
		NullCheck(L_142);
		ProductionTeamHubView_t242CA702D2D2A557A5145C582FE6955173320992* L_143;
		L_143 = GameObject_GetComponentInChildren_TisProductionTeamHubView_t242CA702D2D2A557A5145C582FE6955173320992_m616093F4088896ABD5FAD30CC28A725C50FAD70D(L_142, GameObject_GetComponentInChildren_TisProductionTeamHubView_t242CA702D2D2A557A5145C582FE6955173320992_m616093F4088896ABD5FAD30CC28A725C50FAD70D_RuntimeMethod_var);
		V_13 = L_143;
		ProductionTeamHubView_t242CA702D2D2A557A5145C582FE6955173320992* L_144 = V_13;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_145;
		L_145 = Object_op_Inequality_mD0BE578448EAA61948F25C32F8DD55AB1F778602(L_144, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (!L_145)
		{
			goto IL_0420;
		}
	}
	{
		ProductionTeamHubView_t242CA702D2D2A557A5145C582FE6955173320992* L_146 = V_13;
		NullCheck(L_146);
		GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_147 = L_146->___InfoPanel;
		NullCheck(L_147);
		bool L_148;
		L_148 = GameObject_get_activeSelf_m4F3E5240E138B66AAA080EA30759A3D0517DA368(L_147, NULL);
		if (!L_148)
		{
			goto IL_0420;
		}
	}
	{
		ProductionTeamHubView_t242CA702D2D2A557A5145C582FE6955173320992* L_149 = V_13;
		NullCheck(L_149);
		ProductionTeamHubView_CloseInfo_m1B8E6DD65727D97CBD9074363CA45CF4551564A1(L_149, NULL);
		return;
	}

IL_0420:
	{
		int32_t L_150 = __this->___current;
		if ((!(((uint32_t)L_150) == ((uint32_t)((int32_t)24)))))
		{
			goto IL_0465;
		}
	}
	{
		GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_151 = __this->___contentRoot;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_152;
		L_152 = Object_op_Inequality_mD0BE578448EAA61948F25C32F8DD55AB1F778602(L_151, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (!L_152)
		{
			goto IL_0465;
		}
	}
	{
		GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_153 = __this->___contentRoot;
		NullCheck(L_153);
		ProductionIndividualRankingView_tF449BDA2B9876849864F7F78C4C57CE0BB9726B3* L_154;
		L_154 = GameObject_GetComponentInChildren_TisProductionIndividualRankingView_tF449BDA2B9876849864F7F78C4C57CE0BB9726B3_mF6970CA9815117F55D1E1753E42D02AE5C2B90CE(L_153, GameObject_GetComponentInChildren_TisProductionIndividualRankingView_tF449BDA2B9876849864F7F78C4C57CE0BB9726B3_mF6970CA9815117F55D1E1753E42D02AE5C2B90CE_RuntimeMethod_var);
		V_14 = L_154;
		ProductionIndividualRankingView_tF449BDA2B9876849864F7F78C4C57CE0BB9726B3* L_155 = V_14;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_156;
		L_156 = Object_op_Inequality_mD0BE578448EAA61948F25C32F8DD55AB1F778602(L_155, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (!L_156)
		{
			goto IL_0465;
		}
	}
	{
		ProductionIndividualRankingView_tF449BDA2B9876849864F7F78C4C57CE0BB9726B3* L_157 = V_14;
		NullCheck(L_157);
		GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_158 = L_157->___RulesPanel;
		NullCheck(L_158);
		bool L_159;
		L_159 = GameObject_get_activeSelf_m4F3E5240E138B66AAA080EA30759A3D0517DA368(L_158, NULL);
		if (!L_159)
		{
			goto IL_0465;
		}
	}
	{
		ProductionIndividualRankingView_tF449BDA2B9876849864F7F78C4C57CE0BB9726B3* L_160 = V_14;
		NullCheck(L_160);
		ProductionIndividualRankingView_CloseRules_m51225E47021748851B0E5C93F4EF2CF70D86DCB1(L_160, NULL);
		return;
	}

IL_0465:
	{
		int32_t L_161 = __this->___current;
		if ((!(((uint32_t)L_161) == ((uint32_t)2))))
		{
			goto IL_04af;
		}
	}
	{
		GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_162 = __this->___contentRoot;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_163;
		L_163 = Object_op_Inequality_mD0BE578448EAA61948F25C32F8DD55AB1F778602(L_162, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (!L_163)
		{
			goto IL_04af;
		}
	}
	{
		GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_164 = __this->___contentRoot;
		NullCheck(L_164);
		ProductionWorldMapView_t9EE066392822F11A7943261785563B6FFB262CF7* L_165;
		L_165 = GameObject_GetComponentInChildren_TisProductionWorldMapView_t9EE066392822F11A7943261785563B6FFB262CF7_m99972800DA31C3BF70B60B858FA0817A13CDFB41(L_164, GameObject_GetComponentInChildren_TisProductionWorldMapView_t9EE066392822F11A7943261785563B6FFB262CF7_m99972800DA31C3BF70B60B858FA0817A13CDFB41_RuntimeMethod_var);
		V_15 = L_165;
		ProductionWorldMapView_t9EE066392822F11A7943261785563B6FFB262CF7* L_166 = V_15;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_167;
		L_167 = Object_op_Inequality_mD0BE578448EAA61948F25C32F8DD55AB1F778602(L_166, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (!L_167)
		{
			goto IL_04af;
		}
	}
	{
		ProductionWorldMapView_t9EE066392822F11A7943261785563B6FFB262CF7* L_168 = V_15;
		NullCheck(L_168);
		GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_169 = L_168->___TaskPanel;
		NullCheck(L_169);
		bool L_170;
		L_170 = GameObject_get_activeSelf_m4F3E5240E138B66AAA080EA30759A3D0517DA368(L_169, NULL);
		if (!L_170)
		{
			goto IL_04af;
		}
	}
	{
		ProductionWorldMapView_t9EE066392822F11A7943261785563B6FFB262CF7* L_171 = V_15;
		NullCheck(L_171);
		GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_172 = L_171->___TaskPanel;
		NullCheck(L_172);
		GameObject_SetActive_m638E92E1E75E519E5B24CF150B08CA8E0CDFAB92(L_172, (bool)0, NULL);
		return;
	}

IL_04af:
	{
		Nullable_1_t4FCFD27B7FC762CFC2F64096CD6201A0B0AF4894* L_173 = (Nullable_1_t4FCFD27B7FC762CFC2F64096CD6201A0B0AF4894*)(&__this->___splashAuxiliaryRoot);
		bool L_174;
		L_174 = Nullable_1_get_HasValue_mF10B2CE8C739C005123C719AF092180FAC34D0E1_inline(L_173, Nullable_1_get_HasValue_mF10B2CE8C739C005123C719AF092180FAC34D0E1_RuntimeMethod_var);
		if (!L_174)
		{
			goto IL_04e4;
		}
	}
	{
		int32_t L_175 = __this->___current;
		Nullable_1_t4FCFD27B7FC762CFC2F64096CD6201A0B0AF4894* L_176 = (Nullable_1_t4FCFD27B7FC762CFC2F64096CD6201A0B0AF4894*)(&__this->___splashAuxiliaryRoot);
		int32_t L_177;
		L_177 = Nullable_1_get_Value_m91C7AE54C6A0782DCB2FF528979CC3450C25652F(L_176, Nullable_1_get_Value_m91C7AE54C6A0782DCB2FF528979CC3450C25652F_RuntimeMethod_var);
		if ((!(((uint32_t)L_175) == ((uint32_t)L_177))))
		{
			goto IL_04e4;
		}
	}
	{
		Nullable_1_t4FCFD27B7FC762CFC2F64096CD6201A0B0AF4894* L_178 = (Nullable_1_t4FCFD27B7FC762CFC2F64096CD6201A0B0AF4894*)(&__this->___splashAuxiliaryRoot);
		il2cpp_codegen_initobj(L_178, sizeof(Nullable_1_t4FCFD27B7FC762CFC2F64096CD6201A0B0AF4894));
		ScreenRouter_Show_mB289A1408F5F71A5043EFE930648595D5DCE3734(__this, ((int32_t)22), NULL);
		return;
	}

IL_04e4:
	{
		int32_t L_179 = __this->___current;
		if ((!(((uint32_t)L_179) == ((uint32_t)5))))
		{
			goto IL_04f4;
		}
	}
	{
		ScreenRouter_ShowPause_m581179F0C976AAD52C98FCBBFE855D2FE3C82B55(__this, NULL);
		return;
	}

IL_04f4:
	{
		int32_t L_180 = __this->___current;
		if ((!(((uint32_t)L_180) == ((uint32_t)6))))
		{
			goto IL_04fe;
		}
	}
	{
		return;
	}

IL_04fe:
	{
		int32_t L_181 = __this->___current;
		if ((!(((uint32_t)L_181) == ((uint32_t)((int32_t)21)))))
		{
			goto IL_0510;
		}
	}
	{
		ScreenRouter_ContinueAfterChapterComplete_mEDFB40318905AAE2A02E3B76EC15BBA430C72F65(__this, 2, NULL);
		return;
	}

IL_0510:
	{
		int32_t L_182 = __this->___current;
		if ((!(((uint32_t)L_182) == ((uint32_t)1))))
		{
			goto IL_051f;
		}
	}
	{
		il2cpp_codegen_runtime_class_init_inline(Application_tDB03BE91CDF0ACA614A5E0B67CFB77C44EB19B21_il2cpp_TypeInfo_var);
		Application_Quit_mE304382DB9A6455C2A474C8F364C7387F37E9281(NULL);
		return;
	}

IL_051f:
	{
		int32_t L_183 = __this->___current;
		bool L_184;
		L_184 = ScreenNavigationPolicy_TryGetBackDestination_mCCD1B6B484314E21E4F8B6BCAB09DDFE60F4D904(L_183, (&V_0), NULL);
		if (!L_184)
		{
			goto IL_0536;
		}
	}
	{
		int32_t L_185 = V_0;
		ScreenRouter_Show_mB289A1408F5F71A5043EFE930648595D5DCE3734(__this, L_185, NULL);
		return;
	}

IL_0536:
	{
		ScreenRouter_Show_mB289A1408F5F71A5043EFE930648595D5DCE3734(__this, 1, NULL);
		return;
	}
}
