IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void GameState_NormalizeLevelProgress_m2905E3A4F6C3B9801AE636257CCF1C938DABAF6C (PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* ___0_save, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&HashSet_1_Add_mF3D4CF6D0150392127D5561A87DCDEDA9A81BE5D_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&HashSet_1__ctor_m795438439D8E973F4069A442EFA458942BCD5B3F_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&HashSet_1_tEFC6605F7DE53F71946C33FD371E53C3100F2178_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&List_1_RemoveAt_m4ABB4F33BBBB342E14E14781EEC6EEAB3EC4F7EB_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&List_1__ctor_m2011DE22B9D007A29C2E04C6E21189D5168CBEBC_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&List_1_get_Count_m902C59B5D8A9B81B17C42B76D36EA8A90C4469C5_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&List_1_get_Item_m1F1284F00C110DFF43F68E31E454BEE036FA10F4_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&List_1_tE17596A66614F9E1802150B437325DB3004F8681_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&StringComparer_t6268F19CA34879176651429C0D8A3D0002BB8E06_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&_stringLiteralAD6CD2C36915DEB6A18BCF0F46B294FC1D97072F);
		s_Il2CppMethodInitialized = true;
	}
	ChapterLevelCatalogData_t7FBD790522D46F992FDED25A44446C1D266739E7* V_0 = NULL;
	int32_t V_1 = 0;
	HashSet_1_tEFC6605F7DE53F71946C33FD371E53C3100F2178* V_2 = NULL;
	int32_t V_3 = 0;
	LevelProgressData_tF4CF8BE6A46189E730098D0C3DEE22CAA57E0C9E* V_4 = NULL;
	String_t* V_5 = NULL;
	String_t* G_B9_0 = NULL;
	{
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_0 = ___0_save;
		NullCheck(L_0);
		List_1_tE17596A66614F9E1802150B437325DB3004F8681* L_1 = L_0->___levelProgress;
		if (L_1)
		{
			goto IL_0013;
		}
	}
	{
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_2 = ___0_save;
		List_1_tE17596A66614F9E1802150B437325DB3004F8681* L_3 = (List_1_tE17596A66614F9E1802150B437325DB3004F8681*)il2cpp_codegen_object_new(List_1_tE17596A66614F9E1802150B437325DB3004F8681_il2cpp_TypeInfo_var);
		List_1__ctor_m2011DE22B9D007A29C2E04C6E21189D5168CBEBC(L_3, List_1__ctor_m2011DE22B9D007A29C2E04C6E21189D5168CBEBC_RuntimeMethod_var);
		NullCheck(L_2);
		L_2->___levelProgress = L_3;
		Il2CppCodeGenWriteBarrier((void**)(&L_2->___levelProgress), (void*)L_3);
	}

IL_0013:
	{
		ChapterLevelCatalogData_t7FBD790522D46F992FDED25A44446C1D266739E7* L_4;
		L_4 = ChapterLevelCatalog_LoadChapter1_m0A94743CE818E0E2CE0C0109F5C96478099CB8BC((bool)0, NULL);
		V_0 = L_4;
		V_1 = 1;
		ChapterLevelCatalogData_t7FBD790522D46F992FDED25A44446C1D266739E7* L_5 = V_0;
		NullCheck(L_5);
		LevelDataU5BU5D_t57DB75FA496D463A6B691F85C7C01E750D9163A6* L_6 = L_5->___levels;
		if (!L_6)
		{
			goto IL_0044;
		}
	}
	{
		ChapterLevelCatalogData_t7FBD790522D46F992FDED25A44446C1D266739E7* L_7 = V_0;
		NullCheck(L_7);
		LevelDataU5BU5D_t57DB75FA496D463A6B691F85C7C01E750D9163A6* L_8 = L_7->___levels;
		NullCheck(L_8);
		if (!(((RuntimeArray*)L_8)->max_length))
		{
			goto IL_0044;
		}
	}
	{
		ChapterLevelCatalogData_t7FBD790522D46F992FDED25A44446C1D266739E7* L_9 = V_0;
		NullCheck(L_9);
		LevelDataU5BU5D_t57DB75FA496D463A6B691F85C7C01E750D9163A6* L_10 = L_9->___levels;
		ChapterLevelCatalogData_t7FBD790522D46F992FDED25A44446C1D266739E7* L_11 = V_0;
		NullCheck(L_11);
		LevelDataU5BU5D_t57DB75FA496D463A6B691F85C7C01E750D9163A6* L_12 = L_11->___levels;
		NullCheck(L_12);
		int32_t L_13 = (il2cpp_codegen_conv<int32_t,int64_t,int64_t,false,false>((((RuntimeArray*)L_12)->max_length),NULL));
		NullCheck(L_10);
		int32_t L_14 = ((int32_t)il2cpp_codegen_subtract(L_13, 1));
		LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* L_15 = (L_10)->GetAt(static_cast<il2cpp_array_size_t>(L_14));
		NullCheck(L_15);
		int32_t L_16 = L_15->___levelId;
		V_1 = L_16;
	}

IL_0044:
	{
		il2cpp_codegen_runtime_class_init_inline(StringComparer_t6268F19CA34879176651429C0D8A3D0002BB8E06_il2cpp_TypeInfo_var);
		StringComparer_t6268F19CA34879176651429C0D8A3D0002BB8E06* L_17;
		L_17 = StringComparer_get_Ordinal_m77A8C28E78759556E06A6925E949737A45599E26_inline(NULL);
		HashSet_1_tEFC6605F7DE53F71946C33FD371E53C3100F2178* L_18 = (HashSet_1_tEFC6605F7DE53F71946C33FD371E53C3100F2178*)il2cpp_codegen_object_new(HashSet_1_tEFC6605F7DE53F71946C33FD371E53C3100F2178_il2cpp_TypeInfo_var);
		HashSet_1__ctor_m795438439D8E973F4069A442EFA458942BCD5B3F(L_18, L_17, HashSet_1__ctor_m795438439D8E973F4069A442EFA458942BCD5B3F_RuntimeMethod_var);
		V_2 = L_18;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_19 = ___0_save;
		NullCheck(L_19);
		List_1_tE17596A66614F9E1802150B437325DB3004F8681* L_20 = L_19->___levelProgress;
		NullCheck(L_20);
		int32_t L_21;
		L_21 = List_1_get_Count_m902C59B5D8A9B81B17C42B76D36EA8A90C4469C5_inline(L_20, List_1_get_Count_m902C59B5D8A9B81B17C42B76D36EA8A90C4469C5_RuntimeMethod_var);
		V_3 = ((int32_t)il2cpp_codegen_subtract(L_21, 1));
		goto IL_0188;
	}

IL_0062:
	{
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_22 = ___0_save;
		NullCheck(L_22);
		List_1_tE17596A66614F9E1802150B437325DB3004F8681* L_23 = L_22->___levelProgress;
		int32_t L_24 = V_3;
		NullCheck(L_23);
		LevelProgressData_tF4CF8BE6A46189E730098D0C3DEE22CAA57E0C9E* L_25;
		L_25 = List_1_get_Item_m1F1284F00C110DFF43F68E31E454BEE036FA10F4(L_23, L_24, List_1_get_Item_m1F1284F00C110DFF43F68E31E454BEE036FA10F4_RuntimeMethod_var);
		V_4 = L_25;
		LevelProgressData_tF4CF8BE6A46189E730098D0C3DEE22CAA57E0C9E* L_26 = V_4;
		if (L_26)
		{
			goto IL_007b;
		}
	}
	{
		String_t* L_27 = ((String_t_StaticFields*)il2cpp_codegen_static_fields_for(il2cpp_defaults.string_class))->___Empty;
		G_B9_0 = L_27;
		goto IL_009d;
	}

IL_007b:
	{
		LevelProgressData_tF4CF8BE6A46189E730098D0C3DEE22CAA57E0C9E* L_28 = V_4;
		NullCheck(L_28);
		int32_t L_29 = L_28->___chapterId;
		int32_t L_30 = L_29;
		RuntimeObject* L_31 = Box(il2cpp_defaults.int32_class, &L_30);
		LevelProgressData_tF4CF8BE6A46189E730098D0C3DEE22CAA57E0C9E* L_32 = V_4;
		NullCheck(L_32);
		int32_t L_33 = L_32->___levelId;
		int32_t L_34 = L_33;
		RuntimeObject* L_35 = Box(il2cpp_defaults.int32_class, &L_34);
		String_t* L_36;
		L_36 = String_Format_mFB7DA489BD99F4670881FF50EC017BFB0A5C0987(_stringLiteralAD6CD2C36915DEB6A18BCF0F46B294FC1D97072F, L_31, L_35, NULL);
		G_B9_0 = L_36;
	}

IL_009d:
	{
		V_5 = G_B9_0;
		LevelProgressData_tF4CF8BE6A46189E730098D0C3DEE22CAA57E0C9E* L_37 = V_4;
		if (!L_37)
		{
			goto IL_00c2;
		}
	}
	{
		LevelProgressData_tF4CF8BE6A46189E730098D0C3DEE22CAA57E0C9E* L_38 = V_4;
		NullCheck(L_38);
		int32_t L_39 = L_38->___chapterId;
		LevelProgressData_tF4CF8BE6A46189E730098D0C3DEE22CAA57E0C9E* L_40 = V_4;
		NullCheck(L_40);
		int32_t L_41 = L_40->___levelId;
		LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* L_42;
		L_42 = ChapterLevelCatalog_Find_mB0A3275A64F35FB511F90A6049719C9B1971FDC6(L_39, L_41, NULL);
		if (!L_42)
		{
			goto IL_00c2;
		}
	}
	{
		HashSet_1_tEFC6605F7DE53F71946C33FD371E53C3100F2178* L_43 = V_2;
		String_t* L_44 = V_5;
		NullCheck(L_43);
		bool L_45;
		L_45 = HashSet_1_Add_mF3D4CF6D0150392127D5561A87DCDEDA9A81BE5D(L_43, L_44, HashSet_1_Add_mF3D4CF6D0150392127D5561A87DCDEDA9A81BE5D_RuntimeMethod_var);
		if (L_45)
		{
			goto IL_00d3;
		}
	}

IL_00c2:
	{
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_46 = ___0_save;
		NullCheck(L_46);
		List_1_tE17596A66614F9E1802150B437325DB3004F8681* L_47 = L_46->___levelProgress;
		int32_t L_48 = V_3;
		NullCheck(L_47);
		List_1_RemoveAt_m4ABB4F33BBBB342E14E14781EEC6EEAB3EC4F7EB(L_47, L_48, List_1_RemoveAt_m4ABB4F33BBBB342E14E14781EEC6EEAB3EC4F7EB_RuntimeMethod_var);
		goto IL_0184;
	}

IL_00d3:
	{
		LevelProgressData_tF4CF8BE6A46189E730098D0C3DEE22CAA57E0C9E* L_49 = V_4;
		LevelProgressData_tF4CF8BE6A46189E730098D0C3DEE22CAA57E0C9E* L_50 = V_4;
		NullCheck(L_50);
		int32_t L_51 = L_50->___bestStars;
		int32_t L_52;
		L_52 = Mathf_Clamp_m4DC36EEFDBE5F07C16249DA568023C5ECCFF0E7B_inline(L_51, 0, 3, NULL);
		NullCheck(L_49);
		L_49->___bestStars = L_52;
		LevelProgressData_tF4CF8BE6A46189E730098D0C3DEE22CAA57E0C9E* L_53 = V_4;
		LevelProgressData_tF4CF8BE6A46189E730098D0C3DEE22CAA57E0C9E* L_54 = V_4;
		NullCheck(L_54);
		int32_t L_55 = L_54->___bestScore;
		int32_t L_56;
		L_56 = Mathf_Max_m7FA442918DE37E3A00106D1F2E789D65829792B8_inline(0, L_55, NULL);
		NullCheck(L_53);
		L_53->___bestScore = L_56;
		LevelProgressData_tF4CF8BE6A46189E730098D0C3DEE22CAA57E0C9E* L_57 = V_4;
		LevelProgressData_tF4CF8BE6A46189E730098D0C3DEE22CAA57E0C9E* L_58 = V_4;
		NullCheck(L_58);
		int32_t L_59 = L_58->___attempts;
		int32_t L_60;
		L_60 = Mathf_Max_m7FA442918DE37E3A00106D1F2E789D65829792B8_inline(0, L_59, NULL);
		NullCheck(L_57);
		L_57->___attempts = L_60;
		LevelProgressData_tF4CF8BE6A46189E730098D0C3DEE22CAA57E0C9E* L_61 = V_4;
		LevelProgressData_tF4CF8BE6A46189E730098D0C3DEE22CAA57E0C9E* L_62 = V_4;
		NullCheck(L_62);
		int32_t L_63 = L_62->___retries;
		LevelProgressData_tF4CF8BE6A46189E730098D0C3DEE22CAA57E0C9E* L_64 = V_4;
		NullCheck(L_64);
		int32_t L_65 = L_64->___attempts;
		int32_t L_66;
		L_66 = Mathf_Clamp_m4DC36EEFDBE5F07C16249DA568023C5ECCFF0E7B_inline(L_63, 0, L_65, NULL);
		NullCheck(L_61);
		L_61->___retries = L_66;
		LevelProgressData_tF4CF8BE6A46189E730098D0C3DEE22CAA57E0C9E* L_67 = V_4;
		LevelProgressData_tF4CF8BE6A46189E730098D0C3DEE22CAA57E0C9E* L_68 = V_4;
		NullCheck(L_68);
		int32_t L_69 = L_68->___failures;
		LevelProgressData_tF4CF8BE6A46189E730098D0C3DEE22CAA57E0C9E* L_70 = V_4;
		NullCheck(L_70);
		int32_t L_71 = L_70->___attempts;
		int32_t L_72;
		L_72 = Mathf_Clamp_m4DC36EEFDBE5F07C16249DA568023C5ECCFF0E7B_inline(L_69, 0, L_71, NULL);
		NullCheck(L_67);
		L_67->___failures = L_72;
		LevelProgressData_tF4CF8BE6A46189E730098D0C3DEE22CAA57E0C9E* L_73 = V_4;
		LevelProgressData_tF4CF8BE6A46189E730098D0C3DEE22CAA57E0C9E* L_74 = V_4;
		NullCheck(L_74);
		int32_t L_75 = L_74->___completions;
		int32_t L_76;
		L_76 = Mathf_Max_m7FA442918DE37E3A00106D1F2E789D65829792B8_inline(0, L_75, NULL);
		NullCheck(L_73);
		L_73->___completions = L_76;
		LevelProgressData_tF4CF8BE6A46189E730098D0C3DEE22CAA57E0C9E* L_77 = V_4;
		NullCheck(L_77);
		int32_t L_78 = L_77->___completions;
		if ((((int32_t)L_78) <= ((int32_t)0)))
		{
			goto IL_0184;
		}
	}
	{
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_79 = ___0_save;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_80 = ___0_save;
		NullCheck(L_80);
		int32_t L_81 = L_80->___highestUnlockedLevel;
		int32_t L_82 = V_1;
		LevelProgressData_tF4CF8BE6A46189E730098D0C3DEE22CAA57E0C9E* L_83 = V_4;
		NullCheck(L_83);
		int32_t L_84 = L_83->___levelId;
		int32_t L_85;
		L_85 = Mathf_Min_m888083F74FF5655778F0403BB5E9608BEFDEA8CB_inline(L_82, ((int32_t)il2cpp_codegen_add(L_84, 1)), NULL);
		int32_t L_86;
		L_86 = Mathf_Max_m7FA442918DE37E3A00106D1F2E789D65829792B8_inline(L_81, L_85, NULL);
		NullCheck(L_79);
		L_79->___highestUnlockedLevel = L_86;
	}

IL_0184:
	{
		int32_t L_87 = V_3;
		V_3 = ((int32_t)il2cpp_codegen_subtract(L_87, 1));
	}

IL_0188:
	{
		int32_t L_88 = V_3;
		if ((((int32_t)L_88) >= ((int32_t)0)))
		{
			goto IL_0062;
		}
	}
	{
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_89 = ___0_save;
		ChapterLevelCatalogData_t7FBD790522D46F992FDED25A44446C1D266739E7* L_90 = V_0;
		NullCheck(L_90);
		int32_t L_91 = L_90->___chapterId;
		NullCheck(L_89);
		L_89->___selectedChapter = L_91;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_92 = ___0_save;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_93 = ___0_save;
		NullCheck(L_93);
		int32_t L_94 = L_93->___highestUnlockedLevel;
		int32_t L_95;
		L_95 = Mathf_Max_m7FA442918DE37E3A00106D1F2E789D65829792B8_inline(1, L_94, NULL);
		int32_t L_96 = V_1;
		int32_t L_97;
		L_97 = Mathf_Clamp_m4DC36EEFDBE5F07C16249DA568023C5ECCFF0E7B_inline(L_95, 1, L_96, NULL);
		NullCheck(L_92);
		L_92->___highestUnlockedLevel = L_97;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_98 = ___0_save;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_99 = ___0_save;
		NullCheck(L_99);
		int32_t L_100 = L_99->___selectedLevel;
		int32_t L_101;
		L_101 = Mathf_Max_m7FA442918DE37E3A00106D1F2E789D65829792B8_inline(1, L_100, NULL);
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_102 = ___0_save;
		NullCheck(L_102);
		int32_t L_103 = L_102->___highestUnlockedLevel;
		int32_t L_104;
		L_104 = Mathf_Clamp_m4DC36EEFDBE5F07C16249DA568023C5ECCFF0E7B_inline(L_101, 1, L_103, NULL);
		NullCheck(L_98);
		L_98->___selectedLevel = L_104;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_105 = ___0_save;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_106 = ___0_save;
		NullCheck(L_106);
		int32_t L_107 = L_106->___selectedChapter;
		NullCheck(L_105);
		L_105->___chapter = L_107;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_108 = ___0_save;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_109 = ___0_save;
		NullCheck(L_109);
		int32_t L_110 = L_109->___highestUnlockedLevel;
		NullCheck(L_108);
		L_108->___level = L_110;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_111 = ___0_save;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_112 = ___0_save;
		int32_t L_113;
		L_113 = GameState_CalculateTotalStars_m59DF79CAF1120C86E571DFED1EBC6F615FC41FC5(L_112, NULL);
		NullCheck(L_111);
		L_111->___stars = L_113;
		return;
	}
}
