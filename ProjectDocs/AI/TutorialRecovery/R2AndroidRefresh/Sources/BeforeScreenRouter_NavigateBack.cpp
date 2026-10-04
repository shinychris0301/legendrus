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
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ScreenRouter_U3CNavigateBackU3Eb__69_0_m74BB363E411172643EA54435C673FECC2E7AD1AE_RuntimeMethod_var);
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
			goto IL_0071;
		}
	}
	{
		bool L_5;
		L_5 = ScreenRouter_get_IsModalTransitioning_mA7EE64426C2CB00227FEC49004C2775C8016F40A(__this, NULL);
		if (L_5)
		{
			goto IL_0070;
		}
	}
	{
		Match3Board_tE4CAA58AF06191DC578D60DE5FA7659B8C8ACD69* L_6 = __this->___activeBoard;
		bool L_7;
		L_7 = ScreenRouter_IsGameplayInventoryError_m47226D732980E626CF68E184A9841D891357D8A1(__this, L_6, NULL);
		if (!L_7)
		{
			goto IL_004f;
		}
	}
	{
		Match3Board_tE4CAA58AF06191DC578D60DE5FA7659B8C8ACD69* L_8 = __this->___activeBoard;
		ScreenRouter_CloseGameplayInventoryError_mCCAB0FFFBFE2623F3DB7AC6A3ADA0D87E011F61B(__this, L_8, NULL);
		return;
	}

IL_004f:
	{
		Match3Board_tE4CAA58AF06191DC578D60DE5FA7659B8C8ACD69* L_9 = __this->___activeBoard;
		bool L_10;
		L_10 = ScreenRouter_IsPendingGameplayVictory_m5F15794588383298DAEC0C365A0B18E172F84F3F(__this, L_9, NULL);
		if (!L_10)
		{
			goto IL_006a;
		}
	}
	{
		Match3Board_tE4CAA58AF06191DC578D60DE5FA7659B8C8ACD69* L_11 = __this->___activeBoard;
		ScreenRouter_CloseRejectedGameplayVictory_mFB79D73676B39FE240F55DE90D455B6015A28B8B(__this, L_11, NULL);
		return;
	}

IL_006a:
	{
		ScreenRouter_CloseModal_mDA5B2C06AE67DE089FA24083FA5781E5976D607B(__this, NULL);
	}

IL_0070:
	{
		return;
	}

IL_0071:
	{
		Nullable_1_t4FCFD27B7FC762CFC2F64096CD6201A0B0AF4894* L_12 = (Nullable_1_t4FCFD27B7FC762CFC2F64096CD6201A0B0AF4894*)(&__this->___activeModalId);
		int32_t L_13;
		L_13 = Nullable_1_GetValueOrDefault_m9E93E2CEA72E2689103A641157336A4F674A67A3_inline(L_12, Nullable_1_GetValueOrDefault_m9E93E2CEA72E2689103A641157336A4F674A67A3_RuntimeMethod_var);
		if ((!(((uint32_t)L_13) == ((uint32_t)((int32_t)33)))))
		{
			goto IL_00ba;
		}
	}
	{
		bool L_14;
		L_14 = ScreenRouter_get_IsModalTransitioning_mA7EE64426C2CB00227FEC49004C2775C8016F40A(__this, NULL);
		if (!L_14)
		{
			goto IL_0089;
		}
	}
	{
		return;
	}

IL_0089:
	{
		GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_15 = __this->___modalVisualRoot;
		NullCheck(L_15);
		ProductionTutorialView_t17A2D559FD4E71F841E57E107C3B8FADDC3DA4CB* L_16;
		L_16 = GameObject_GetComponentInChildren_TisProductionTutorialView_t17A2D559FD4E71F841E57E107C3B8FADDC3DA4CB_m7D0309A9BADDE66BB452C63C5781EE1ED641BA53(L_15, GameObject_GetComponentInChildren_TisProductionTutorialView_t17A2D559FD4E71F841E57E107C3B8FADDC3DA4CB_m7D0309A9BADDE66BB452C63C5781EE1ED641BA53_RuntimeMethod_var);
		V_1 = L_16;
		ProductionTutorialView_t17A2D559FD4E71F841E57E107C3B8FADDC3DA4CB* L_17 = V_1;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_18;
		L_18 = Object_op_Inequality_mD0BE578448EAA61948F25C32F8DD55AB1F778602(L_17, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (!L_18)
		{
			goto IL_00a7;
		}
	}
	{
		ProductionTutorialView_t17A2D559FD4E71F841E57E107C3B8FADDC3DA4CB* L_19 = V_1;
		NullCheck(L_19);
		bool L_20;
		L_20 = ProductionTutorialView_TryDismissDetails_mC381A9E7EA352E22F41A6E015BD1D3B72584C9DB(L_19, NULL);
		if (!L_20)
		{
			goto IL_00a7;
		}
	}
	{
		return;
	}

IL_00a7:
	{
		Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* L_21 = (Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07*)il2cpp_codegen_object_new(Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07_il2cpp_TypeInfo_var);
		Action__ctor_mBDC7B0B4A3F583B64C2896F01BDED360772F67DC(L_21, __this, (intptr_t)((void*)ScreenRouter_U3CNavigateBackU3Eb__69_0_m74BB363E411172643EA54435C673FECC2E7AD1AE_RuntimeMethod_var), NULL);
		ScreenRouter_RequestCloseModal_m54358FF9D470A8E2D5A80FEA049E40084071FE10(__this, L_21, NULL);
		return;
	}

IL_00ba:
	{
		int32_t L_22 = __this->___current;
		if ((!(((uint32_t)L_22) == ((uint32_t)((int32_t)33)))))
		{
			goto IL_00f6;
		}
	}
	{
		GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_23 = __this->___contentRoot;
		NullCheck(L_23);
		ProductionTutorialView_t17A2D559FD4E71F841E57E107C3B8FADDC3DA4CB* L_24;
		L_24 = GameObject_GetComponentInChildren_TisProductionTutorialView_t17A2D559FD4E71F841E57E107C3B8FADDC3DA4CB_m7D0309A9BADDE66BB452C63C5781EE1ED641BA53(L_23, GameObject_GetComponentInChildren_TisProductionTutorialView_t17A2D559FD4E71F841E57E107C3B8FADDC3DA4CB_m7D0309A9BADDE66BB452C63C5781EE1ED641BA53_RuntimeMethod_var);
		V_2 = L_24;
		ProductionTutorialView_t17A2D559FD4E71F841E57E107C3B8FADDC3DA4CB* L_25 = V_2;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_26;
		L_26 = Object_op_Inequality_mD0BE578448EAA61948F25C32F8DD55AB1F778602(L_25, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (!L_26)
		{
			goto IL_00e2;
		}
	}
	{
		ProductionTutorialView_t17A2D559FD4E71F841E57E107C3B8FADDC3DA4CB* L_27 = V_2;
		NullCheck(L_27);
		bool L_28;
		L_28 = ProductionTutorialView_TryDismissDetails_mC381A9E7EA352E22F41A6E015BD1D3B72584C9DB(L_27, NULL);
		if (!L_28)
		{
			goto IL_00e2;
		}
	}
	{
		return;
	}

IL_00e2:
	{
		Nullable_1_t4FCFD27B7FC762CFC2F64096CD6201A0B0AF4894* L_29 = (Nullable_1_t4FCFD27B7FC762CFC2F64096CD6201A0B0AF4894*)(&__this->___tutorialReturnPage);
		int32_t L_30;
		L_30 = Nullable_1_GetValueOrDefault_mCCA4F58C01C85CA318EE7C5DA8D366F3330CBD64(L_29, ((int32_t)22), Nullable_1_GetValueOrDefault_mCCA4F58C01C85CA318EE7C5DA8D366F3330CBD64_RuntimeMethod_var);
		ScreenRouter_Show_mB289A1408F5F71A5043EFE930648595D5DCE3734(__this, L_30, NULL);
		return;
	}

IL_00f6:
	{
		int32_t L_31 = __this->___current;
		if ((!(((uint32_t)L_31) == ((uint32_t)((int32_t)32)))))
		{
			goto IL_0140;
		}
	}
	{
		GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_32 = __this->___contentRoot;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_33;
		L_33 = Object_op_Inequality_mD0BE578448EAA61948F25C32F8DD55AB1F778602(L_32, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (!L_33)
		{
			goto IL_0140;
		}
	}
	{
		GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_34 = __this->___contentRoot;
		NullCheck(L_34);
		ProductionLoginView_t36729B7F5B697B3828DE80A1A0EB2509C7F66B81* L_35;
		L_35 = GameObject_GetComponentInChildren_TisProductionLoginView_t36729B7F5B697B3828DE80A1A0EB2509C7F66B81_mEF98EC1C6B2D581C59612058A763DD1DC035DFF5(L_34, GameObject_GetComponentInChildren_TisProductionLoginView_t36729B7F5B697B3828DE80A1A0EB2509C7F66B81_mEF98EC1C6B2D581C59612058A763DD1DC035DFF5_RuntimeMethod_var);
		V_3 = L_35;
		ProductionLoginView_t36729B7F5B697B3828DE80A1A0EB2509C7F66B81* L_36 = V_3;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_37;
		L_37 = Object_op_Inequality_mD0BE578448EAA61948F25C32F8DD55AB1F778602(L_36, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (!L_37)
		{
			goto IL_012c;
		}
	}
	{
		ProductionLoginView_t36729B7F5B697B3828DE80A1A0EB2509C7F66B81* L_38 = V_3;
		NullCheck(L_38);
		bool L_39;
		L_39 = ProductionLoginView_TryDismiss_mBE7B3276892B92DCF1CB2CBF5F1FB9F5BC3A4809(L_38, NULL);
		if (!L_39)
		{
			goto IL_012c;
		}
	}
	{
		return;
	}

IL_012c:
	{
		Nullable_1_t4FCFD27B7FC762CFC2F64096CD6201A0B0AF4894* L_40 = (Nullable_1_t4FCFD27B7FC762CFC2F64096CD6201A0B0AF4894*)(&__this->___loginReturnPage);
		int32_t L_41;
		L_41 = Nullable_1_GetValueOrDefault_mCCA4F58C01C85CA318EE7C5DA8D366F3330CBD64(L_40, ((int32_t)22), Nullable_1_GetValueOrDefault_mCCA4F58C01C85CA318EE7C5DA8D366F3330CBD64_RuntimeMethod_var);
		ScreenRouter_Show_mB289A1408F5F71A5043EFE930648595D5DCE3734(__this, L_41, NULL);
		return;
	}

IL_0140:
	{
		int32_t L_42 = __this->___current;
		if ((!(((uint32_t)L_42) == ((uint32_t)((int32_t)31)))))
		{
			goto IL_015e;
		}
	}
	{
		Nullable_1_t4FCFD27B7FC762CFC2F64096CD6201A0B0AF4894* L_43 = (Nullable_1_t4FCFD27B7FC762CFC2F64096CD6201A0B0AF4894*)(&__this->___languageReturnPage);
		int32_t L_44;
		L_44 = Nullable_1_GetValueOrDefault_mCCA4F58C01C85CA318EE7C5DA8D366F3330CBD64(L_43, ((int32_t)22), Nullable_1_GetValueOrDefault_mCCA4F58C01C85CA318EE7C5DA8D366F3330CBD64_RuntimeMethod_var);
		ScreenRouter_Show_mB289A1408F5F71A5043EFE930648595D5DCE3734(__this, L_44, NULL);
		return;
	}

IL_015e:
	{
		Nullable_1_t4FCFD27B7FC762CFC2F64096CD6201A0B0AF4894* L_45 = (Nullable_1_t4FCFD27B7FC762CFC2F64096CD6201A0B0AF4894*)(&__this->___activeModalId);
		int32_t L_46;
		L_46 = Nullable_1_GetValueOrDefault_m9E93E2CEA72E2689103A641157336A4F674A67A3_inline(L_45, Nullable_1_GetValueOrDefault_m9E93E2CEA72E2689103A641157336A4F674A67A3_RuntimeMethod_var);
		if ((!(((uint32_t)L_46) == ((uint32_t)((int32_t)16)))))
		{
			goto IL_01b7;
		}
	}
	{
		bool L_47;
		L_47 = ScreenRouter_get_IsModalTransitioning_mA7EE64426C2CB00227FEC49004C2775C8016F40A(__this, NULL);
		if (!L_47)
		{
			goto IL_0176;
		}
	}
	{
		return;
	}

IL_0176:
	{
		GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_48 = __this->___modalVisualRoot;
		NullCheck(L_48);
		ProductionSettingsView_t8B601F105D46E705808BBD9FA8B26DD7877703BC* L_49;
		L_49 = GameObject_GetComponentInChildren_TisProductionSettingsView_t8B601F105D46E705808BBD9FA8B26DD7877703BC_mED8D0D4965CBE42D0BCE417CD1FAF493FDDC26E8(L_48, GameObject_GetComponentInChildren_TisProductionSettingsView_t8B601F105D46E705808BBD9FA8B26DD7877703BC_mED8D0D4965CBE42D0BCE417CD1FAF493FDDC26E8_RuntimeMethod_var);
		V_4 = L_49;
		ProductionSettingsView_t8B601F105D46E705808BBD9FA8B26DD7877703BC* L_50 = V_4;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_51;
		L_51 = Object_op_Inequality_mD0BE578448EAA61948F25C32F8DD55AB1F778602(L_50, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (!L_51)
		{
			goto IL_0197;
		}
	}
	{
		ProductionSettingsView_t8B601F105D46E705808BBD9FA8B26DD7877703BC* L_52 = V_4;
		NullCheck(L_52);
		bool L_53;
		L_53 = ProductionSettingsView_TryDismiss_m6F0F859CCDBA358CB78306FF2900535538C4501C(L_52, NULL);
		if (!L_53)
		{
			goto IL_0197;
		}
	}
	{
		return;
	}

IL_0197:
	{
		bool L_54 = __this->___settingsReturnToPause;
		if (!L_54)
		{
			goto IL_01b0;
		}
	}
	{
		__this->___settingsReturnToPause = (bool)0;
		bool L_55;
		L_55 = ScreenRouter_ShowModal_m195E7C54FEA3162B7A49F9BF61E3CA321B3AE5D9(__this, ((int32_t)17), NULL);
		return;
	}

IL_01b0:
	{
		ScreenRouter_CloseModal_mDA5B2C06AE67DE089FA24083FA5781E5976D607B(__this, NULL);
		return;
	}

IL_01b7:
	{
		int32_t L_56 = __this->___current;
		if ((!(((uint32_t)L_56) == ((uint32_t)((int32_t)16)))))
		{
			goto IL_0203;
		}
	}
	{
		GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_57 = __this->___contentRoot;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_58;
		L_58 = Object_op_Inequality_mD0BE578448EAA61948F25C32F8DD55AB1F778602(L_57, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (!L_58)
		{
			goto IL_0203;
		}
	}
	{
		GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_59 = __this->___contentRoot;
		NullCheck(L_59);
		ProductionSettingsView_t8B601F105D46E705808BBD9FA8B26DD7877703BC* L_60;
		L_60 = GameObject_GetComponentInChildren_TisProductionSettingsView_t8B601F105D46E705808BBD9FA8B26DD7877703BC_mED8D0D4965CBE42D0BCE417CD1FAF493FDDC26E8(L_59, GameObject_GetComponentInChildren_TisProductionSettingsView_t8B601F105D46E705808BBD9FA8B26DD7877703BC_mED8D0D4965CBE42D0BCE417CD1FAF493FDDC26E8_RuntimeMethod_var);
		V_5 = L_60;
		ProductionSettingsView_t8B601F105D46E705808BBD9FA8B26DD7877703BC* L_61 = V_5;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_62;
		L_62 = Object_op_Inequality_mD0BE578448EAA61948F25C32F8DD55AB1F778602(L_61, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (!L_62)
		{
			goto IL_01f0;
		}
	}
	{
		ProductionSettingsView_t8B601F105D46E705808BBD9FA8B26DD7877703BC* L_63 = V_5;
		NullCheck(L_63);
		bool L_64;
		L_64 = ProductionSettingsView_TryDismiss_m6F0F859CCDBA358CB78306FF2900535538C4501C(L_63, NULL);
		if (!L_64)
		{
			goto IL_01f0;
		}
	}
	{
		return;
	}

IL_01f0:
	{
		Nullable_1_t4FCFD27B7FC762CFC2F64096CD6201A0B0AF4894* L_65 = (Nullable_1_t4FCFD27B7FC762CFC2F64096CD6201A0B0AF4894*)(&__this->___settingsReturnPage);
		int32_t L_66;
		L_66 = Nullable_1_GetValueOrDefault_mCCA4F58C01C85CA318EE7C5DA8D366F3330CBD64(L_65, 1, Nullable_1_GetValueOrDefault_mCCA4F58C01C85CA318EE7C5DA8D366F3330CBD64_RuntimeMethod_var);
		ScreenRouter_Show_mB289A1408F5F71A5043EFE930648595D5DCE3734(__this, L_66, NULL);
		return;
	}

IL_0203:
	{
		int32_t L_67 = __this->___current;
		if ((!(((uint32_t)L_67) == ((uint32_t)((int32_t)18)))))
		{
			goto IL_023c;
		}
	}
	{
		GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_68 = __this->___contentRoot;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_69;
		L_69 = Object_op_Inequality_mD0BE578448EAA61948F25C32F8DD55AB1F778602(L_68, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (!L_69)
		{
			goto IL_023c;
		}
	}
	{
		GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_70 = __this->___contentRoot;
		NullCheck(L_70);
		ProductionCharactersView_t80FF719EBB00992075D821990BF966D5E95577DE* L_71;
		L_71 = GameObject_GetComponentInChildren_TisProductionCharactersView_t80FF719EBB00992075D821990BF966D5E95577DE_m78DB6C38217BAE039CE674B3E17BD05DFED5B883(L_70, GameObject_GetComponentInChildren_TisProductionCharactersView_t80FF719EBB00992075D821990BF966D5E95577DE_m78DB6C38217BAE039CE674B3E17BD05DFED5B883_RuntimeMethod_var);
		V_6 = L_71;
		ProductionCharactersView_t80FF719EBB00992075D821990BF966D5E95577DE* L_72 = V_6;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_73;
		L_73 = Object_op_Inequality_mD0BE578448EAA61948F25C32F8DD55AB1F778602(L_72, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (!L_73)
		{
			goto IL_023c;
		}
	}
	{
		ProductionCharactersView_t80FF719EBB00992075D821990BF966D5E95577DE* L_74 = V_6;
		NullCheck(L_74);
		bool L_75;
		L_75 = ProductionCharactersView_TryDismiss_m618086950BBC574C8EBC829EB41AADF64ADEA819(L_74, NULL);
		if (!L_75)
		{
			goto IL_023c;
		}
	}
	{
		return;
	}

IL_023c:
	{
		int32_t L_76 = __this->___current;
		if ((!(((uint32_t)L_76) == ((uint32_t)((int32_t)29)))))
		{
			goto IL_0275;
		}
	}
	{
		GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_77 = __this->___contentRoot;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_78;
		L_78 = Object_op_Inequality_mD0BE578448EAA61948F25C32F8DD55AB1F778602(L_77, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (!L_78)
		{
			goto IL_0275;
		}
	}
	{
		GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_79 = __this->___contentRoot;
		NullCheck(L_79);
		ProductionInventoryView_tBFCB541C14E4ED40B2CB6633CE17AC0EB9D14E8E* L_80;
		L_80 = GameObject_GetComponentInChildren_TisProductionInventoryView_tBFCB541C14E4ED40B2CB6633CE17AC0EB9D14E8E_m31D4B3D9DC5DA6B7B0C89F9A080C6C59993B9CC3(L_79, GameObject_GetComponentInChildren_TisProductionInventoryView_tBFCB541C14E4ED40B2CB6633CE17AC0EB9D14E8E_m31D4B3D9DC5DA6B7B0C89F9A080C6C59993B9CC3_RuntimeMethod_var);
		V_7 = L_80;
		ProductionInventoryView_tBFCB541C14E4ED40B2CB6633CE17AC0EB9D14E8E* L_81 = V_7;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_82;
		L_82 = Object_op_Inequality_mD0BE578448EAA61948F25C32F8DD55AB1F778602(L_81, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (!L_82)
		{
			goto IL_0275;
		}
	}
	{
		ProductionInventoryView_tBFCB541C14E4ED40B2CB6633CE17AC0EB9D14E8E* L_83 = V_7;
		NullCheck(L_83);
		bool L_84;
		L_84 = ProductionInventoryView_TryDismiss_mF4348FD34E0CF2A5EEA0913CFA533E640648FD82(L_83, NULL);
		if (!L_84)
		{
			goto IL_0275;
		}
	}
	{
		return;
	}

IL_0275:
	{
		int32_t L_85 = __this->___current;
		if ((!(((uint32_t)L_85) == ((uint32_t)((int32_t)15)))))
		{
			goto IL_02ae;
		}
	}
	{
		GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_86 = __this->___contentRoot;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_87;
		L_87 = Object_op_Inequality_mD0BE578448EAA61948F25C32F8DD55AB1F778602(L_86, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (!L_87)
		{
			goto IL_02ae;
		}
	}
	{
		GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_88 = __this->___contentRoot;
		NullCheck(L_88);
		ProductionProfileView_tFBAE39BD387DB9B67C01A51EB40718FD7461992D* L_89;
		L_89 = GameObject_GetComponentInChildren_TisProductionProfileView_tFBAE39BD387DB9B67C01A51EB40718FD7461992D_mBA813B51212DD365F39C3E916E546EE66AB77092(L_88, GameObject_GetComponentInChildren_TisProductionProfileView_tFBAE39BD387DB9B67C01A51EB40718FD7461992D_mBA813B51212DD365F39C3E916E546EE66AB77092_RuntimeMethod_var);
		V_8 = L_89;
		ProductionProfileView_tFBAE39BD387DB9B67C01A51EB40718FD7461992D* L_90 = V_8;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_91;
		L_91 = Object_op_Inequality_mD0BE578448EAA61948F25C32F8DD55AB1F778602(L_90, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (!L_91)
		{
			goto IL_02ae;
		}
	}
	{
		ProductionProfileView_tFBAE39BD387DB9B67C01A51EB40718FD7461992D* L_92 = V_8;
		NullCheck(L_92);
		bool L_93;
		L_93 = ProductionProfileView_TryDismiss_m576EDBFD85433786E22AD0DFCA9DA8BA1AAB8B76(L_92, NULL);
		if (!L_93)
		{
			goto IL_02ae;
		}
	}
	{
		return;
	}

IL_02ae:
	{
		int32_t L_94 = __this->___current;
		if ((!(((uint32_t)L_94) == ((uint32_t)((int32_t)19)))))
		{
			goto IL_02e7;
		}
	}
	{
		GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_95 = __this->___contentRoot;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_96;
		L_96 = Object_op_Inequality_mD0BE578448EAA61948F25C32F8DD55AB1F778602(L_95, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (!L_96)
		{
			goto IL_02e7;
		}
	}
	{
		GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_97 = __this->___contentRoot;
		NullCheck(L_97);
		ProductionMailView_t613C10918DAA8582972FFE0681462F4DAE73B966* L_98;
		L_98 = GameObject_GetComponentInChildren_TisProductionMailView_t613C10918DAA8582972FFE0681462F4DAE73B966_mCA8281FDE2E17871A24E6899ABBB35F495AD58A9(L_97, GameObject_GetComponentInChildren_TisProductionMailView_t613C10918DAA8582972FFE0681462F4DAE73B966_mCA8281FDE2E17871A24E6899ABBB35F495AD58A9_RuntimeMethod_var);
		V_9 = L_98;
		ProductionMailView_t613C10918DAA8582972FFE0681462F4DAE73B966* L_99 = V_9;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_100;
		L_100 = Object_op_Inequality_mD0BE578448EAA61948F25C32F8DD55AB1F778602(L_99, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (!L_100)
		{
			goto IL_02e7;
		}
	}
	{
		ProductionMailView_t613C10918DAA8582972FFE0681462F4DAE73B966* L_101 = V_9;
		NullCheck(L_101);
		bool L_102;
		L_102 = ProductionMailView_TryDismiss_mE3937AB247F9305BB611FBFC6799FF355D4FC345(L_101, NULL);
		if (!L_102)
		{
			goto IL_02e7;
		}
	}
	{
		return;
	}

IL_02e7:
	{
		int32_t L_103 = __this->___current;
		if ((!(((uint32_t)L_103) == ((uint32_t)((int32_t)28)))))
		{
			goto IL_033c;
		}
	}
	{
		GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_104 = __this->___contentRoot;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_105;
		L_105 = Object_op_Inequality_mD0BE578448EAA61948F25C32F8DD55AB1F778602(L_104, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (!L_105)
		{
			goto IL_033c;
		}
	}
	{
		GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_106 = __this->___contentRoot;
		NullCheck(L_106);
		ProductionFriendsListView_t568A80A4F587DE5E0D9156810474BD1B241353D1* L_107;
		L_107 = GameObject_GetComponentInChildren_TisProductionFriendsListView_t568A80A4F587DE5E0D9156810474BD1B241353D1_m6F925C1CFEB8568058656E82EA523E4A8E17AB84(L_106, GameObject_GetComponentInChildren_TisProductionFriendsListView_t568A80A4F587DE5E0D9156810474BD1B241353D1_m6F925C1CFEB8568058656E82EA523E4A8E17AB84_RuntimeMethod_var);
		V_10 = L_107;
		ProductionFriendsListView_t568A80A4F587DE5E0D9156810474BD1B241353D1* L_108 = V_10;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_109;
		L_109 = Object_op_Inequality_mD0BE578448EAA61948F25C32F8DD55AB1F778602(L_108, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (!L_109)
		{
			goto IL_032c;
		}
	}
	{
		ProductionFriendsListView_t568A80A4F587DE5E0D9156810474BD1B241353D1* L_110 = V_10;
		NullCheck(L_110);
		GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_111 = L_110->___InfoPanel;
		NullCheck(L_111);
		bool L_112;
		L_112 = GameObject_get_activeSelf_m4F3E5240E138B66AAA080EA30759A3D0517DA368(L_111, NULL);
		if (!L_112)
		{
			goto IL_032c;
		}
	}
	{
		ProductionFriendsListView_t568A80A4F587DE5E0D9156810474BD1B241353D1* L_113 = V_10;
		NullCheck(L_113);
		ProductionFriendsListView_CloseInfo_m59B08AED32B47FF3B03A2C8B6F82070FC917824C(L_113, NULL);
		return;
	}

IL_032c:
	{
		bool L_114 = __this->___friendsReturnToHome;
		if (!L_114)
		{
			goto IL_033c;
		}
	}
	{
		ScreenRouter_Show_mB289A1408F5F71A5043EFE930648595D5DCE3734(__this, 1, NULL);
		return;
	}

IL_033c:
	{
		int32_t L_115 = __this->___current;
		if ((!(((uint32_t)L_115) == ((uint32_t)((int32_t)27)))))
		{
			goto IL_0381;
		}
	}
	{
		GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_116 = __this->___contentRoot;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_117;
		L_117 = Object_op_Inequality_mD0BE578448EAA61948F25C32F8DD55AB1F778602(L_116, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (!L_117)
		{
			goto IL_0381;
		}
	}
	{
		GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_118 = __this->___contentRoot;
		NullCheck(L_118);
		ProductionTeamChatView_t3876DDF380A2DA789CB223EB9E32A75578DFE07E* L_119;
		L_119 = GameObject_GetComponentInChildren_TisProductionTeamChatView_t3876DDF380A2DA789CB223EB9E32A75578DFE07E_m44D1755932740500A249FF3E7E2C1EFDB3C06D08(L_118, GameObject_GetComponentInChildren_TisProductionTeamChatView_t3876DDF380A2DA789CB223EB9E32A75578DFE07E_m44D1755932740500A249FF3E7E2C1EFDB3C06D08_RuntimeMethod_var);
		V_11 = L_119;
		ProductionTeamChatView_t3876DDF380A2DA789CB223EB9E32A75578DFE07E* L_120 = V_11;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_121;
		L_121 = Object_op_Inequality_mD0BE578448EAA61948F25C32F8DD55AB1F778602(L_120, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (!L_121)
		{
			goto IL_0381;
		}
	}
	{
		ProductionTeamChatView_t3876DDF380A2DA789CB223EB9E32A75578DFE07E* L_122 = V_11;
		NullCheck(L_122);
		GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_123 = L_122->___InfoPanel;
		NullCheck(L_123);
		bool L_124;
		L_124 = GameObject_get_activeSelf_m4F3E5240E138B66AAA080EA30759A3D0517DA368(L_123, NULL);
		if (!L_124)
		{
			goto IL_0381;
		}
	}
	{
		ProductionTeamChatView_t3876DDF380A2DA789CB223EB9E32A75578DFE07E* L_125 = V_11;
		NullCheck(L_125);
		ProductionTeamChatView_CloseInfo_mEADF40386B9F498FBFB3F86F412D15CB407563B2(L_125, NULL);
		return;
	}

IL_0381:
	{
		int32_t L_126 = __this->___current;
		if ((!(((uint32_t)L_126) == ((uint32_t)((int32_t)26)))))
		{
			goto IL_03c6;
		}
	}
	{
		GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_127 = __this->___contentRoot;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_128;
		L_128 = Object_op_Inequality_mD0BE578448EAA61948F25C32F8DD55AB1F778602(L_127, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (!L_128)
		{
			goto IL_03c6;
		}
	}
	{
		GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_129 = __this->___contentRoot;
		NullCheck(L_129);
		ProductionTeamRankingView_t6A2E723B48D2D1F086684E393EC71E7A19E82DD4* L_130;
		L_130 = GameObject_GetComponentInChildren_TisProductionTeamRankingView_t6A2E723B48D2D1F086684E393EC71E7A19E82DD4_m9DBFF39E9DEB2EF3D3644EEA62DF2F17C33F2E15(L_129, GameObject_GetComponentInChildren_TisProductionTeamRankingView_t6A2E723B48D2D1F086684E393EC71E7A19E82DD4_m9DBFF39E9DEB2EF3D3644EEA62DF2F17C33F2E15_RuntimeMethod_var);
		V_12 = L_130;
		ProductionTeamRankingView_t6A2E723B48D2D1F086684E393EC71E7A19E82DD4* L_131 = V_12;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_132;
		L_132 = Object_op_Inequality_mD0BE578448EAA61948F25C32F8DD55AB1F778602(L_131, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (!L_132)
		{
			goto IL_03c6;
		}
	}
	{
		ProductionTeamRankingView_t6A2E723B48D2D1F086684E393EC71E7A19E82DD4* L_133 = V_12;
		NullCheck(L_133);
		GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_134 = L_133->___InfoPanel;
		NullCheck(L_134);
		bool L_135;
		L_135 = GameObject_get_activeSelf_m4F3E5240E138B66AAA080EA30759A3D0517DA368(L_134, NULL);
		if (!L_135)
		{
			goto IL_03c6;
		}
	}
	{
		ProductionTeamRankingView_t6A2E723B48D2D1F086684E393EC71E7A19E82DD4* L_136 = V_12;
		NullCheck(L_136);
		ProductionTeamRankingView_CloseInfo_m459BDB863ADD3EEC282776CD0509318619CFD880(L_136, NULL);
		return;
	}

IL_03c6:
	{
		int32_t L_137 = __this->___current;
		if ((!(((uint32_t)L_137) == ((uint32_t)((int32_t)25)))))
		{
			goto IL_040b;
		}
	}
	{
		GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_138 = __this->___contentRoot;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_139;
		L_139 = Object_op_Inequality_mD0BE578448EAA61948F25C32F8DD55AB1F778602(L_138, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (!L_139)
		{
			goto IL_040b;
		}
	}
	{
		GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_140 = __this->___contentRoot;
		NullCheck(L_140);
		ProductionTeamHubView_t242CA702D2D2A557A5145C582FE6955173320992* L_141;
		L_141 = GameObject_GetComponentInChildren_TisProductionTeamHubView_t242CA702D2D2A557A5145C582FE6955173320992_m616093F4088896ABD5FAD30CC28A725C50FAD70D(L_140, GameObject_GetComponentInChildren_TisProductionTeamHubView_t242CA702D2D2A557A5145C582FE6955173320992_m616093F4088896ABD5FAD30CC28A725C50FAD70D_RuntimeMethod_var);
		V_13 = L_141;
		ProductionTeamHubView_t242CA702D2D2A557A5145C582FE6955173320992* L_142 = V_13;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_143;
		L_143 = Object_op_Inequality_mD0BE578448EAA61948F25C32F8DD55AB1F778602(L_142, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (!L_143)
		{
			goto IL_040b;
		}
	}
	{
		ProductionTeamHubView_t242CA702D2D2A557A5145C582FE6955173320992* L_144 = V_13;
		NullCheck(L_144);
		GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_145 = L_144->___InfoPanel;
		NullCheck(L_145);
		bool L_146;
		L_146 = GameObject_get_activeSelf_m4F3E5240E138B66AAA080EA30759A3D0517DA368(L_145, NULL);
		if (!L_146)
		{
			goto IL_040b;
		}
	}
	{
		ProductionTeamHubView_t242CA702D2D2A557A5145C582FE6955173320992* L_147 = V_13;
		NullCheck(L_147);
		ProductionTeamHubView_CloseInfo_m1B8E6DD65727D97CBD9074363CA45CF4551564A1(L_147, NULL);
		return;
	}

IL_040b:
	{
		int32_t L_148 = __this->___current;
		if ((!(((uint32_t)L_148) == ((uint32_t)((int32_t)24)))))
		{
			goto IL_0450;
		}
	}
	{
		GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_149 = __this->___contentRoot;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_150;
		L_150 = Object_op_Inequality_mD0BE578448EAA61948F25C32F8DD55AB1F778602(L_149, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (!L_150)
		{
			goto IL_0450;
		}
	}
	{
		GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_151 = __this->___contentRoot;
		NullCheck(L_151);
		ProductionIndividualRankingView_tF449BDA2B9876849864F7F78C4C57CE0BB9726B3* L_152;
		L_152 = GameObject_GetComponentInChildren_TisProductionIndividualRankingView_tF449BDA2B9876849864F7F78C4C57CE0BB9726B3_mF6970CA9815117F55D1E1753E42D02AE5C2B90CE(L_151, GameObject_GetComponentInChildren_TisProductionIndividualRankingView_tF449BDA2B9876849864F7F78C4C57CE0BB9726B3_mF6970CA9815117F55D1E1753E42D02AE5C2B90CE_RuntimeMethod_var);
		V_14 = L_152;
		ProductionIndividualRankingView_tF449BDA2B9876849864F7F78C4C57CE0BB9726B3* L_153 = V_14;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_154;
		L_154 = Object_op_Inequality_mD0BE578448EAA61948F25C32F8DD55AB1F778602(L_153, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (!L_154)
		{
			goto IL_0450;
		}
	}
	{
		ProductionIndividualRankingView_tF449BDA2B9876849864F7F78C4C57CE0BB9726B3* L_155 = V_14;
		NullCheck(L_155);
		GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_156 = L_155->___RulesPanel;
		NullCheck(L_156);
		bool L_157;
		L_157 = GameObject_get_activeSelf_m4F3E5240E138B66AAA080EA30759A3D0517DA368(L_156, NULL);
		if (!L_157)
		{
			goto IL_0450;
		}
	}
	{
		ProductionIndividualRankingView_tF449BDA2B9876849864F7F78C4C57CE0BB9726B3* L_158 = V_14;
		NullCheck(L_158);
		ProductionIndividualRankingView_CloseRules_m51225E47021748851B0E5C93F4EF2CF70D86DCB1(L_158, NULL);
		return;
	}

IL_0450:
	{
		int32_t L_159 = __this->___current;
		if ((!(((uint32_t)L_159) == ((uint32_t)2))))
		{
			goto IL_049a;
		}
	}
	{
		GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_160 = __this->___contentRoot;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_161;
		L_161 = Object_op_Inequality_mD0BE578448EAA61948F25C32F8DD55AB1F778602(L_160, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (!L_161)
		{
			goto IL_049a;
		}
	}
	{
		GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_162 = __this->___contentRoot;
		NullCheck(L_162);
		ProductionWorldMapView_t9EE066392822F11A7943261785563B6FFB262CF7* L_163;
		L_163 = GameObject_GetComponentInChildren_TisProductionWorldMapView_t9EE066392822F11A7943261785563B6FFB262CF7_m99972800DA31C3BF70B60B858FA0817A13CDFB41(L_162, GameObject_GetComponentInChildren_TisProductionWorldMapView_t9EE066392822F11A7943261785563B6FFB262CF7_m99972800DA31C3BF70B60B858FA0817A13CDFB41_RuntimeMethod_var);
		V_15 = L_163;
		ProductionWorldMapView_t9EE066392822F11A7943261785563B6FFB262CF7* L_164 = V_15;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_165;
		L_165 = Object_op_Inequality_mD0BE578448EAA61948F25C32F8DD55AB1F778602(L_164, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (!L_165)
		{
			goto IL_049a;
		}
	}
	{
		ProductionWorldMapView_t9EE066392822F11A7943261785563B6FFB262CF7* L_166 = V_15;
		NullCheck(L_166);
		GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_167 = L_166->___TaskPanel;
		NullCheck(L_167);
		bool L_168;
		L_168 = GameObject_get_activeSelf_m4F3E5240E138B66AAA080EA30759A3D0517DA368(L_167, NULL);
		if (!L_168)
		{
			goto IL_049a;
		}
	}
	{
		ProductionWorldMapView_t9EE066392822F11A7943261785563B6FFB262CF7* L_169 = V_15;
		NullCheck(L_169);
		GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_170 = L_169->___TaskPanel;
		NullCheck(L_170);
		GameObject_SetActive_m638E92E1E75E519E5B24CF150B08CA8E0CDFAB92(L_170, (bool)0, NULL);
		return;
	}

IL_049a:
	{
		Nullable_1_t4FCFD27B7FC762CFC2F64096CD6201A0B0AF4894* L_171 = (Nullable_1_t4FCFD27B7FC762CFC2F64096CD6201A0B0AF4894*)(&__this->___splashAuxiliaryRoot);
		bool L_172;
		L_172 = Nullable_1_get_HasValue_mF10B2CE8C739C005123C719AF092180FAC34D0E1_inline(L_171, Nullable_1_get_HasValue_mF10B2CE8C739C005123C719AF092180FAC34D0E1_RuntimeMethod_var);
		if (!L_172)
		{
			goto IL_04cf;
		}
	}
	{
		int32_t L_173 = __this->___current;
		Nullable_1_t4FCFD27B7FC762CFC2F64096CD6201A0B0AF4894* L_174 = (Nullable_1_t4FCFD27B7FC762CFC2F64096CD6201A0B0AF4894*)(&__this->___splashAuxiliaryRoot);
		int32_t L_175;
		L_175 = Nullable_1_get_Value_m91C7AE54C6A0782DCB2FF528979CC3450C25652F(L_174, Nullable_1_get_Value_m91C7AE54C6A0782DCB2FF528979CC3450C25652F_RuntimeMethod_var);
		if ((!(((uint32_t)L_173) == ((uint32_t)L_175))))
		{
			goto IL_04cf;
		}
	}
	{
		Nullable_1_t4FCFD27B7FC762CFC2F64096CD6201A0B0AF4894* L_176 = (Nullable_1_t4FCFD27B7FC762CFC2F64096CD6201A0B0AF4894*)(&__this->___splashAuxiliaryRoot);
		il2cpp_codegen_initobj(L_176, sizeof(Nullable_1_t4FCFD27B7FC762CFC2F64096CD6201A0B0AF4894));
		ScreenRouter_Show_mB289A1408F5F71A5043EFE930648595D5DCE3734(__this, ((int32_t)22), NULL);
		return;
	}

IL_04cf:
	{
		int32_t L_177 = __this->___current;
		if ((!(((uint32_t)L_177) == ((uint32_t)5))))
		{
			goto IL_04df;
		}
	}
	{
		ScreenRouter_ShowPause_m581179F0C976AAD52C98FCBBFE855D2FE3C82B55(__this, NULL);
		return;
	}

IL_04df:
	{
		int32_t L_178 = __this->___current;
		if ((!(((uint32_t)L_178) == ((uint32_t)6))))
		{
			goto IL_04e9;
		}
	}
	{
		return;
	}

IL_04e9:
	{
		int32_t L_179 = __this->___current;
		if ((!(((uint32_t)L_179) == ((uint32_t)((int32_t)21)))))
		{
			goto IL_04fb;
		}
	}
	{
		ScreenRouter_ContinueAfterChapterComplete_mEDFB40318905AAE2A02E3B76EC15BBA430C72F65(__this, 2, NULL);
		return;
	}

IL_04fb:
	{
		int32_t L_180 = __this->___current;
		if ((!(((uint32_t)L_180) == ((uint32_t)1))))
		{
			goto IL_050a;
		}
	}
	{
		il2cpp_codegen_runtime_class_init_inline(Application_tDB03BE91CDF0ACA614A5E0B67CFB77C44EB19B21_il2cpp_TypeInfo_var);
		Application_Quit_mE304382DB9A6455C2A474C8F364C7387F37E9281(NULL);
		return;
	}

IL_050a:
	{
		int32_t L_181 = __this->___current;
		bool L_182;
		L_182 = ScreenNavigationPolicy_TryGetBackDestination_mCCD1B6B484314E21E4F8B6BCAB09DDFE60F4D904(L_181, (&V_0), NULL);
		if (!L_182)
		{
			goto IL_0521;
		}
	}
	{
		int32_t L_183 = V_0;
		ScreenRouter_Show_mB289A1408F5F71A5043EFE930648595D5DCE3734(__this, L_183, NULL);
		return;
	}

IL_0521:
	{
		ScreenRouter_Show_mB289A1408F5F71A5043EFE930648595D5DCE3734(__this, 1, NULL);
		return;
	}
}
