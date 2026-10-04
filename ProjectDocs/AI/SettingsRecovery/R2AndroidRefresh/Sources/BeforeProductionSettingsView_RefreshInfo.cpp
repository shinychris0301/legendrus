IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void ProductionSettingsView_RefreshInfo_m62ABB75BB2DA58CF2CC1C2CD3F144E6C19260BA8 (ProductionSettingsView_t8B601F105D46E705808BBD9FA8B26DD7877703BC* __this, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Application_tDB03BE91CDF0ACA614A5E0B67CFB77C44EB19B21_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Component_GetComponent_TisProductionHomePanel_t68F7372DF8B16B3B80B97B7D75767209C05A3D3D_mEA9746494AC933A8834CEECC850BA87B16A0086D_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ProductionSettingsView_t8B601F105D46E705808BBD9FA8B26DD7877703BC_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&_stringLiteral0284102940A701D59ED70E6024D3BDD0B02DA3A3);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&_stringLiteral0A9A92BB0556A8DC7B977F3036C7CE6CC7C34506);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&_stringLiteral196B82C109FCD25C046F4E28C5CDCFBC72C51472);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&_stringLiteral1D70804418991B55F6891F86D5D1A4B0CE089560);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&_stringLiteral2A360D335033FE11746196AF612E364892656F9E);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&_stringLiteral36A47FD7B7ECFC94C5BD79F2C0E886294FE33152);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&_stringLiteral3CBCA975002639C0ED6D6DF9FD808E6EF89C4ABE);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&_stringLiteral422045746104ACF4029C206392E9EFA50F7C3F21);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&_stringLiteral4AA5A022DC43E320DA6672FFE0251903CBE55044);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&_stringLiteral4E3231677F4BEC61BC5A1A9C2C25275EE7068B57);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&_stringLiteral524BDDE63CCD776D01346DE4F636DE6B00C649C9);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&_stringLiteral6C40457B3851657AF3A205A497E77EFDFA7CEF2A);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&_stringLiteral748D694B15E6C70CBF87FA1C8DA6E676C369C197);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&_stringLiteral775560201C2896D4C427CE6285B973CCD95BB8AD);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&_stringLiteral7858815C0805CE1692355A3D4F58C3E9E1AF8CD0);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&_stringLiteral79F4E474F1DCFFF771A10EC96FBFA984D05FB3DA);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&_stringLiteral7A2DFB9BDB08B28F4D9883A43052E896E8BC751C);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&_stringLiteral81013BB96CDB74E34F07F48D2BF0CF50CDE72BD8);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&_stringLiteral8D446933147049608A7960FB2B390793B54DFEC0);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&_stringLiteral951CAFF15AE6B57C439FD11455CDE241AC548240);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&_stringLiteralAC4E761D7F69C76FF003AAF03FC41D91C62285C1);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&_stringLiteralC6DE535D828F30C374EB3BE3FFDE520228FC3096);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&_stringLiteralC791C5F1F60F8B42B47E0DA37E93D6F67B8AE280);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&_stringLiteralE2AAA5218911472742E35F37799F636A5BE422EF);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&_stringLiteralE91551391F725E9E4838E5887D8EF4EB04CBB66F);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&_stringLiteralF2DF0A90F137D7F1C07A0A82066132A651B53DA6);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&_stringLiteralF780505A5F995C63C1997BC4E61839F6D5AC7FDD);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&_stringLiteralFE490FBBD2AA21917CC616D1C48466F84525E137);
		s_Il2CppMethodInitialized = true;
	}
	bool V_0 = false;
	StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248* V_1 = NULL;
	StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248* V_2 = NULL;
	Color_tD001788D726C3A7F1379BEED0260B9591F440C1F V_3;
	memset((&V_3), 0, sizeof(V_3));
	int32_t V_4 = 0;
	Text_tD60B2346DAA6666BF0D822FF607F0B220C2B9E62* G_B5_0 = NULL;
	Text_tD60B2346DAA6666BF0D822FF607F0B220C2B9E62* G_B1_0 = NULL;
	Text_tD60B2346DAA6666BF0D822FF607F0B220C2B9E62* G_B3_0 = NULL;
	Text_tD60B2346DAA6666BF0D822FF607F0B220C2B9E62* G_B2_0 = NULL;
	StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248* G_B4_0 = NULL;
	Text_tD60B2346DAA6666BF0D822FF607F0B220C2B9E62* G_B4_1 = NULL;
	String_t* G_B8_0 = NULL;
	Text_tD60B2346DAA6666BF0D822FF607F0B220C2B9E62* G_B8_1 = NULL;
	Text_tD60B2346DAA6666BF0D822FF607F0B220C2B9E62* G_B7_0 = NULL;
	Text_tD60B2346DAA6666BF0D822FF607F0B220C2B9E62* G_B6_0 = NULL;
	Text_tD60B2346DAA6666BF0D822FF607F0B220C2B9E62* G_B10_0 = NULL;
	Text_tD60B2346DAA6666BF0D822FF607F0B220C2B9E62* G_B9_0 = NULL;
	StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248* G_B11_0 = NULL;
	Text_tD60B2346DAA6666BF0D822FF607F0B220C2B9E62* G_B11_1 = NULL;
	Text_tD60B2346DAA6666BF0D822FF607F0B220C2B9E62* G_B13_0 = NULL;
	Text_tD60B2346DAA6666BF0D822FF607F0B220C2B9E62* G_B12_0 = NULL;
	String_t* G_B14_0 = NULL;
	Text_tD60B2346DAA6666BF0D822FF607F0B220C2B9E62* G_B14_1 = NULL;
	float G_B16_0 = 0.0f;
	float G_B16_1 = 0.0f;
	float G_B16_2 = 0.0f;
	ProductionHomePanel_t68F7372DF8B16B3B80B97B7D75767209C05A3D3D* G_B16_3 = NULL;
	ProductionHomePanel_t68F7372DF8B16B3B80B97B7D75767209C05A3D3D* G_B16_4 = NULL;
	ProductionHomePanel_t68F7372DF8B16B3B80B97B7D75767209C05A3D3D* G_B16_5 = NULL;
	float G_B15_0 = 0.0f;
	float G_B15_1 = 0.0f;
	float G_B15_2 = 0.0f;
	ProductionHomePanel_t68F7372DF8B16B3B80B97B7D75767209C05A3D3D* G_B15_3 = NULL;
	ProductionHomePanel_t68F7372DF8B16B3B80B97B7D75767209C05A3D3D* G_B15_4 = NULL;
	ProductionHomePanel_t68F7372DF8B16B3B80B97B7D75767209C05A3D3D* G_B15_5 = NULL;
	float G_B17_0 = 0.0f;
	float G_B17_1 = 0.0f;
	float G_B17_2 = 0.0f;
	float G_B17_3 = 0.0f;
	ProductionHomePanel_t68F7372DF8B16B3B80B97B7D75767209C05A3D3D* G_B17_4 = NULL;
	ProductionHomePanel_t68F7372DF8B16B3B80B97B7D75767209C05A3D3D* G_B17_5 = NULL;
	ProductionHomePanel_t68F7372DF8B16B3B80B97B7D75767209C05A3D3D* G_B17_6 = NULL;
	Image_tBC1D03F63BF71132E9A5E472B8742F172A011E7E* G_B19_0 = NULL;
	Image_tBC1D03F63BF71132E9A5E472B8742F172A011E7E* G_B18_0 = NULL;
	String_t* G_B20_0 = NULL;
	Image_tBC1D03F63BF71132E9A5E472B8742F172A011E7E* G_B20_1 = NULL;
	Text_tD60B2346DAA6666BF0D822FF607F0B220C2B9E62* G_B22_0 = NULL;
	Text_tD60B2346DAA6666BF0D822FF607F0B220C2B9E62* G_B21_0 = NULL;
	String_t* G_B23_0 = NULL;
	Text_tD60B2346DAA6666BF0D822FF607F0B220C2B9E62* G_B23_1 = NULL;
	Image_tBC1D03F63BF71132E9A5E472B8742F172A011E7E* G_B26_0 = NULL;
	Image_tBC1D03F63BF71132E9A5E472B8742F172A011E7E* G_B25_0 = NULL;
	String_t* G_B27_0 = NULL;
	Image_tBC1D03F63BF71132E9A5E472B8742F172A011E7E* G_B27_1 = NULL;
	{
		String_t* L_0;
		L_0 = DemoFeatureState_get_LanguageCode_m3DF9CF2D676DB6CDA9612686549EA2EEE3BB5438(NULL);
		bool L_1;
		L_1 = String_op_Equality_m030E1B219352228970A076136E455C4E568C02C1(L_0, _stringLiteralC6DE535D828F30C374EB3BE3FFDE520228FC3096, NULL);
		V_0 = L_1;
		Text_tD60B2346DAA6666BF0D822FF607F0B220C2B9E62* L_2 = __this->___InfoHeading;
		int32_t L_3 = __this->___infoKind;
		if ((((int32_t)L_3) == ((int32_t)6)))
		{
			G_B5_0 = L_2;
			goto IL_0037;
		}
		G_B1_0 = L_2;
	}
	{
		bool L_4 = V_0;
		if (L_4)
		{
			G_B3_0 = G_B1_0;
			goto IL_0029;
		}
		G_B2_0 = G_B1_0;
	}
	{
		il2cpp_codegen_runtime_class_init_inline(ProductionSettingsView_t8B601F105D46E705808BBD9FA8B26DD7877703BC_il2cpp_TypeInfo_var);
		StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248* L_5 = ((ProductionSettingsView_t8B601F105D46E705808BBD9FA8B26DD7877703BC_StaticFields*)il2cpp_codegen_static_fields_for(ProductionSettingsView_t8B601F105D46E705808BBD9FA8B26DD7877703BC_il2cpp_TypeInfo_var))->___RuRows;
		G_B4_0 = L_5;
		G_B4_1 = G_B2_0;
		goto IL_002e;
	}

IL_0029:
	{
		il2cpp_codegen_runtime_class_init_inline(ProductionSettingsView_t8B601F105D46E705808BBD9FA8B26DD7877703BC_il2cpp_TypeInfo_var);
		StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248* L_6 = ((ProductionSettingsView_t8B601F105D46E705808BBD9FA8B26DD7877703BC_StaticFields*)il2cpp_codegen_static_fields_for(ProductionSettingsView_t8B601F105D46E705808BBD9FA8B26DD7877703BC_il2cpp_TypeInfo_var))->___EnRows;
		G_B4_0 = L_6;
		G_B4_1 = G_B3_0;
	}

IL_002e:
	{
		int32_t L_7 = __this->___infoKind;
		NullCheck(G_B4_0);
		int32_t L_8 = L_7;
		String_t* L_9 = (G_B4_0)->GetAt(static_cast<il2cpp_array_size_t>(L_8));
		G_B8_0 = L_9;
		G_B8_1 = G_B4_1;
		goto IL_0046;
	}

IL_0037:
	{
		bool L_10 = V_0;
		if (L_10)
		{
			G_B7_0 = G_B5_0;
			goto IL_0041;
		}
		G_B6_0 = G_B5_0;
	}
	{
		G_B8_0 = _stringLiteralFE490FBBD2AA21917CC616D1C48466F84525E137;
		G_B8_1 = G_B6_0;
		goto IL_0046;
	}

IL_0041:
	{
		G_B8_0 = _stringLiteralF780505A5F995C63C1997BC4E61839F6D5AC7FDD;
		G_B8_1 = G_B7_0;
	}

IL_0046:
	{
		NullCheck(G_B8_1);
		VirtualActionInvoker1< String_t* >::Invoke(75, G_B8_1, G_B8_0);
		StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248* L_11 = (StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248*)(StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248*)SZArrayNew(StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248_il2cpp_TypeInfo_var, (uint32_t)7);
		StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248* L_12 = L_11;
		NullCheck(L_12);
		(L_12)->SetAt(static_cast<il2cpp_array_size_t>(0), (String_t*)_stringLiteral1D70804418991B55F6891F86D5D1A4B0CE089560);
		StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248* L_13 = L_12;
		NullCheck(L_13);
		(L_13)->SetAt(static_cast<il2cpp_array_size_t>(1), (String_t*)_stringLiteral7858815C0805CE1692355A3D4F58C3E9E1AF8CD0);
		StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248* L_14 = L_13;
		NullCheck(L_14);
		(L_14)->SetAt(static_cast<il2cpp_array_size_t>(2), (String_t*)_stringLiteral0284102940A701D59ED70E6024D3BDD0B02DA3A3);
		StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248* L_15 = L_14;
		il2cpp_codegen_runtime_class_init_inline(Application_tDB03BE91CDF0ACA614A5E0B67CFB77C44EB19B21_il2cpp_TypeInfo_var);
		String_t* L_16;
		L_16 = Application_get_version_m9CC0E94F12493682C1A2CA803083F6F01BA43124(NULL);
		String_t* L_17;
		L_17 = String_Concat_m9E3155FB84015C823606188F53B47CB44C444991(_stringLiteralAC4E761D7F69C76FF003AAF03FC41D91C62285C1, L_16, NULL);
		NullCheck(L_15);
		(L_15)->SetAt(static_cast<il2cpp_array_size_t>(3), (String_t*)L_17);
		StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248* L_18 = L_15;
		NullCheck(L_18);
		(L_18)->SetAt(static_cast<il2cpp_array_size_t>(4), (String_t*)_stringLiteral36A47FD7B7ECFC94C5BD79F2C0E886294FE33152);
		StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248* L_19 = L_18;
		NullCheck(L_19);
		(L_19)->SetAt(static_cast<il2cpp_array_size_t>(5), (String_t*)_stringLiteral8D446933147049608A7960FB2B390793B54DFEC0);
		StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248* L_20 = L_19;
		NullCheck(L_20);
		(L_20)->SetAt(static_cast<il2cpp_array_size_t>(6), (String_t*)_stringLiteral2A360D335033FE11746196AF612E364892656F9E);
		V_1 = L_20;
		StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248* L_21 = (StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248*)(StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248*)SZArrayNew(StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248_il2cpp_TypeInfo_var, (uint32_t)7);
		StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248* L_22 = L_21;
		NullCheck(L_22);
		(L_22)->SetAt(static_cast<il2cpp_array_size_t>(0), (String_t*)_stringLiteral4E3231677F4BEC61BC5A1A9C2C25275EE7068B57);
		StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248* L_23 = L_22;
		NullCheck(L_23);
		(L_23)->SetAt(static_cast<il2cpp_array_size_t>(1), (String_t*)_stringLiteral524BDDE63CCD776D01346DE4F636DE6B00C649C9);
		StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248* L_24 = L_23;
		NullCheck(L_24);
		(L_24)->SetAt(static_cast<il2cpp_array_size_t>(2), (String_t*)_stringLiteral7A2DFB9BDB08B28F4D9883A43052E896E8BC751C);
		StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248* L_25 = L_24;
		String_t* L_26;
		L_26 = Application_get_version_m9CC0E94F12493682C1A2CA803083F6F01BA43124(NULL);
		String_t* L_27;
		L_27 = String_Concat_m9E3155FB84015C823606188F53B47CB44C444991(_stringLiteral775560201C2896D4C427CE6285B973CCD95BB8AD, L_26, NULL);
		NullCheck(L_25);
		(L_25)->SetAt(static_cast<il2cpp_array_size_t>(3), (String_t*)L_27);
		StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248* L_28 = L_25;
		NullCheck(L_28);
		(L_28)->SetAt(static_cast<il2cpp_array_size_t>(4), (String_t*)_stringLiteralE2AAA5218911472742E35F37799F636A5BE422EF);
		StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248* L_29 = L_28;
		NullCheck(L_29);
		(L_29)->SetAt(static_cast<il2cpp_array_size_t>(5), (String_t*)_stringLiteral951CAFF15AE6B57C439FD11455CDE241AC548240);
		StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248* L_30 = L_29;
		NullCheck(L_30);
		(L_30)->SetAt(static_cast<il2cpp_array_size_t>(6), (String_t*)_stringLiteralC791C5F1F60F8B42B47E0DA37E93D6F67B8AE280);
		V_2 = L_30;
		Text_tD60B2346DAA6666BF0D822FF607F0B220C2B9E62* L_31 = __this->___InfoDescription;
		bool L_32 = V_0;
		if (L_32)
		{
			G_B10_0 = L_31;
			goto IL_00e9;
		}
		G_B9_0 = L_31;
	}
	{
		StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248* L_33 = V_1;
		G_B11_0 = L_33;
		G_B11_1 = G_B9_0;
		goto IL_00ea;
	}

IL_00e9:
	{
		StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248* L_34 = V_2;
		G_B11_0 = L_34;
		G_B11_1 = G_B10_0;
	}

IL_00ea:
	{
		int32_t L_35 = __this->___infoKind;
		NullCheck(G_B11_0);
		int32_t L_36 = L_35;
		String_t* L_37 = (G_B11_0)->GetAt(static_cast<il2cpp_array_size_t>(L_36));
		NullCheck(G_B11_1);
		VirtualActionInvoker1< String_t* >::Invoke(75, G_B11_1, L_37);
		Text_tD60B2346DAA6666BF0D822FF607F0B220C2B9E62* L_38 = __this->___CloseInfoLabel;
		bool L_39 = V_0;
		if (L_39)
		{
			G_B13_0 = L_38;
			goto IL_0106;
		}
		G_B12_0 = L_38;
	}
	{
		G_B14_0 = _stringLiteral748D694B15E6C70CBF87FA1C8DA6E676C369C197;
		G_B14_1 = G_B12_0;
		goto IL_010b;
	}

IL_0106:
	{
		G_B14_0 = _stringLiteral3CBCA975002639C0ED6D6DF9FD808E6EF89C4ABE;
		G_B14_1 = G_B13_0;
	}

IL_010b:
	{
		NullCheck(G_B14_1);
		VirtualActionInvoker1< String_t* >::Invoke(75, G_B14_1, G_B14_0);
		GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_40 = __this->___LanguageChoices;
		int32_t L_41 = __this->___infoKind;
		NullCheck(L_40);
		GameObject_SetActive_m638E92E1E75E519E5B24CF150B08CA8E0CDFAB92(L_40, (bool)((((int32_t)L_41) == ((int32_t)0))? 1 : 0), NULL);
		GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_42 = __this->___GeneralInfoContent;
		int32_t L_43 = __this->___infoKind;
		NullCheck(L_42);
		GameObject_SetActive_m638E92E1E75E519E5B24CF150B08CA8E0CDFAB92(L_42, (bool)((!(((uint32_t)L_43) <= ((uint32_t)0)))? 1 : 0), NULL);
		GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_44 = __this->___InfoPanel;
		NullCheck(L_44);
		Transform_tB27202C6F4E36D225EE28A13E4D662BF99785DB1* L_45;
		L_45 = GameObject_get_transform_m0BC10ADFA1632166AE5544BDF9038A2650C2AE56(L_44, NULL);
		NullCheck(L_45);
		Transform_tB27202C6F4E36D225EE28A13E4D662BF99785DB1* L_46;
		L_46 = Transform_Find_m3087032B0E1C5B96A2D2C27020BAEAE2DA08F932(L_45, _stringLiteral6C40457B3851657AF3A205A497E77EFDFA7CEF2A, NULL);
		NullCheck(L_46);
		ProductionHomePanel_t68F7372DF8B16B3B80B97B7D75767209C05A3D3D* L_47;
		L_47 = Component_GetComponent_TisProductionHomePanel_t68F7372DF8B16B3B80B97B7D75767209C05A3D3D_mEA9746494AC933A8834CEECC850BA87B16A0086D(L_46, Component_GetComponent_TisProductionHomePanel_t68F7372DF8B16B3B80B97B7D75767209C05A3D3D_mEA9746494AC933A8834CEECC850BA87B16A0086D_RuntimeMethod_var);
		ProductionHomePanel_t68F7372DF8B16B3B80B97B7D75767209C05A3D3D* L_48 = L_47;
		ProductionHomePanel_t68F7372DF8B16B3B80B97B7D75767209C05A3D3D* L_49 = L_48;
		int32_t L_50 = __this->___infoKind;
		if (!L_50)
		{
			G_B16_0 = (0.0f);
			G_B16_1 = (0.0f);
			G_B16_2 = (0.0f);
			G_B16_3 = L_49;
			G_B16_4 = L_49;
			G_B16_5 = L_48;
			goto IL_0172;
		}
		G_B15_0 = (0.0f);
		G_B15_1 = (0.0f);
		G_B15_2 = (0.0f);
		G_B15_3 = L_49;
		G_B15_4 = L_49;
		G_B15_5 = L_48;
	}
	{
		G_B17_0 = (0.920000017f);
		G_B17_1 = G_B15_0;
		G_B17_2 = G_B15_1;
		G_B17_3 = G_B15_2;
		G_B17_4 = G_B15_3;
		G_B17_5 = G_B15_4;
		G_B17_6 = G_B15_5;
		goto IL_0177;
	}

IL_0172:
	{
		G_B17_0 = (0.600000024f);
		G_B17_1 = G_B16_0;
		G_B17_2 = G_B16_1;
		G_B17_3 = G_B16_2;
		G_B17_4 = G_B16_3;
		G_B17_5 = G_B16_4;
		G_B17_6 = G_B16_5;
	}

IL_0177:
	{
		Color_tD001788D726C3A7F1379BEED0260B9591F440C1F L_51;
		memset((&L_51), 0, sizeof(L_51));
		Color__ctor_m3786F0D6E510D9CFA544523A955870BD2A514C8C_inline((&L_51), G_B17_3, G_B17_2, G_B17_1, G_B17_0, NULL);
		Color_tD001788D726C3A7F1379BEED0260B9591F440C1F L_52 = L_51;
		V_3 = L_52;
		NullCheck(G_B17_4);
		G_B17_4->___RightColor = L_52;
		Color_tD001788D726C3A7F1379BEED0260B9591F440C1F L_53 = V_3;
		NullCheck(G_B17_5);
		G_B17_5->___LeftColor = L_53;
		NullCheck(G_B17_6);
		VirtualActionInvoker0::Invoke(28, G_B17_6);
		Image_tBC1D03F63BF71132E9A5E472B8742F172A011E7E* L_54 = __this->___LanguagePanel;
		bool L_55 = V_0;
		if (L_55)
		{
			G_B19_0 = L_54;
			goto IL_019e;
		}
		G_B18_0 = L_54;
	}
	{
		G_B20_0 = _stringLiteral79F4E474F1DCFFF771A10EC96FBFA984D05FB3DA;
		G_B20_1 = G_B18_0;
		goto IL_01a3;
	}

IL_019e:
	{
		G_B20_0 = _stringLiteral0A9A92BB0556A8DC7B977F3036C7CE6CC7C34506;
		G_B20_1 = G_B19_0;
	}

IL_01a3:
	{
		Sprite_tAFF74BC83CD68037494CB0B4F28CBDF8971CAB99* L_56;
		L_56 = ProductionSettingsRuntime_Load_m6F6202C0987D9830BB7265236AFBA8E0603829AA(G_B20_0, NULL);
		NullCheck(G_B20_1);
		Image_set_sprite_mC0C248340BA27AAEE56855A3FAFA0D8CA12956DE(G_B20_1, L_56, NULL);
		Text_tD60B2346DAA6666BF0D822FF607F0B220C2B9E62* L_57 = __this->___LanguageHeading;
		bool L_58 = V_0;
		NullCheck(L_57);
		Behaviour_set_enabled_mF1DCFE60EB09E0529FE9476CA804A3AA2D72B16A(L_57, L_58, NULL);
		Text_tD60B2346DAA6666BF0D822FF607F0B220C2B9E62* L_59 = __this->___LanguageHeading;
		Font_tC95270EA3198038970422D78B74A7F2E218A96B6* L_60 = __this->___TitleFont;
		NullCheck(L_59);
		Text_set_font_mA0D2999281A72029A5BC7294A886C5674F07DC5F(L_59, L_60, NULL);
		Text_tD60B2346DAA6666BF0D822FF607F0B220C2B9E62* L_61 = __this->___LanguageHeading;
		bool L_62 = V_0;
		if (L_62)
		{
			G_B22_0 = L_61;
			goto IL_01da;
		}
		G_B21_0 = L_61;
	}
	{
		G_B23_0 = _stringLiteral196B82C109FCD25C046F4E28C5CDCFBC72C51472;
		G_B23_1 = G_B21_0;
		goto IL_01df;
	}

IL_01da:
	{
		G_B23_0 = _stringLiteralF2DF0A90F137D7F1C07A0A82066132A651B53DA6;
		G_B23_1 = G_B22_0;
	}

IL_01df:
	{
		NullCheck(G_B23_1);
		VirtualActionInvoker1< String_t* >::Invoke(75, G_B23_1, G_B23_0);
		TextU5BU5D_t1D476A037C4542F053D17ADAFA2723F311F9A353* L_63 = __this->___LanguageLabels;
		NullCheck(L_63);
		int32_t L_64 = 0;
		Text_tD60B2346DAA6666BF0D822FF607F0B220C2B9E62* L_65 = (L_63)->GetAt(static_cast<il2cpp_array_size_t>(L_64));
		NullCheck(L_65);
		VirtualActionInvoker1< String_t* >::Invoke(75, L_65, _stringLiteral81013BB96CDB74E34F07F48D2BF0CF50CDE72BD8);
		TextU5BU5D_t1D476A037C4542F053D17ADAFA2723F311F9A353* L_66 = __this->___LanguageLabels;
		NullCheck(L_66);
		int32_t L_67 = 1;
		Text_tD60B2346DAA6666BF0D822FF607F0B220C2B9E62* L_68 = (L_66)->GetAt(static_cast<il2cpp_array_size_t>(L_67));
		NullCheck(L_68);
		VirtualActionInvoker1< String_t* >::Invoke(75, L_68, _stringLiteral422045746104ACF4029C206392E9EFA50F7C3F21);
		V_4 = 0;
		goto IL_0253;
	}

IL_020d:
	{
		TextU5BU5D_t1D476A037C4542F053D17ADAFA2723F311F9A353* L_69 = __this->___LanguageLabels;
		int32_t L_70 = V_4;
		NullCheck(L_69);
		int32_t L_71 = L_70;
		Text_tD60B2346DAA6666BF0D822FF607F0B220C2B9E62* L_72 = (L_69)->GetAt(static_cast<il2cpp_array_size_t>(L_71));
		Font_tC95270EA3198038970422D78B74A7F2E218A96B6* L_73 = __this->___TitleFont;
		NullCheck(L_72);
		Text_set_font_mA0D2999281A72029A5BC7294A886C5674F07DC5F(L_72, L_73, NULL);
		ButtonU5BU5D_t0D512FF2FF5A72DAC04754F2C10182F850328BEF* L_74 = __this->___LanguageButtons;
		int32_t L_75 = V_4;
		NullCheck(L_74);
		int32_t L_76 = L_75;
		Button_t6786514A57F7AFDEE5431112FEA0CAB24F5AE098* L_77 = (L_74)->GetAt(static_cast<il2cpp_array_size_t>(L_76));
		NullCheck(L_77);
		Image_tBC1D03F63BF71132E9A5E472B8742F172A011E7E* L_78;
		L_78 = Selectable_get_image_m88664022F6BC90E7B8D4BFCBA7FE24B48E90C639(L_77, NULL);
		bool L_79 = V_0;
		int32_t L_80 = V_4;
		if ((((int32_t)((!(((uint32_t)L_79) <= ((uint32_t)0)))? 1 : 0)) == ((int32_t)L_80)))
		{
			G_B26_0 = L_78;
			goto IL_023e;
		}
		G_B25_0 = L_78;
	}
	{
		G_B27_0 = _stringLiteral4AA5A022DC43E320DA6672FFE0251903CBE55044;
		G_B27_1 = G_B25_0;
		goto IL_0243;
	}

IL_023e:
	{
		G_B27_0 = _stringLiteralE91551391F725E9E4838E5887D8EF4EB04CBB66F;
		G_B27_1 = G_B26_0;
	}

IL_0243:
	{
		Sprite_tAFF74BC83CD68037494CB0B4F28CBDF8971CAB99* L_81;
		L_81 = ProductionSettingsRuntime_Load_m6F6202C0987D9830BB7265236AFBA8E0603829AA(G_B27_0, NULL);
		NullCheck(G_B27_1);
		Image_set_sprite_mC0C248340BA27AAEE56855A3FAFA0D8CA12956DE(G_B27_1, L_81, NULL);
		int32_t L_82 = V_4;
		V_4 = ((int32_t)il2cpp_codegen_add(L_82, 1));
	}

IL_0253:
	{
		int32_t L_83 = V_4;
		if ((((int32_t)L_83) < ((int32_t)2)))
		{
			goto IL_020d;
		}
	}
	{
		return;
	}
}
