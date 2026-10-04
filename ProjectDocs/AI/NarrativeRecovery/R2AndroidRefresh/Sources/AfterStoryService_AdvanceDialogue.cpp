IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t StoryService_AdvanceDialogue_m607394AACFC68F30AD59B70975E3C764C1CCAAB0 (const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Debug_t8394C7EEAECA3689C2C9B9DE9C7166D73596276F_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&StoryService_t3C43407CE680194EA30F1337D5B8F05824E25419_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&_stringLiteral567412B430512EC07C99F863C4AB47828BE73F7B);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&_stringLiteral8B2845342FA2C42DF223764F1ABEB2AC03D8EC4C);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&_stringLiteral8EF12CDC28C5A548D03CB52BB114214231854AEE);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&_stringLiteralE58C58F564E422E19147F21FFCD108F873525030);
		s_Il2CppMethodInitialized = true;
	}
	StorySceneData_tD375EAB5A696E276CB12FBA76A9BA3836B716D2E* V_0 = NULL;
	GameState_t5E590D2FB317152A4011A3CA69E3F311E0A7393A* V_1 = NULL;
	bool V_2 = false;
	ChapterStoryCatalogData_t725340CD2FDB5256381D20B6F24C5A49578A6258* V_3 = NULL;
	String_t* V_4 = NULL;
	GameState_t5E590D2FB317152A4011A3CA69E3F311E0A7393A* G_B8_0 = NULL;
	GameState_t5E590D2FB317152A4011A3CA69E3F311E0A7393A* G_B7_0 = NULL;
	String_t* G_B9_0 = NULL;
	GameState_t5E590D2FB317152A4011A3CA69E3F311E0A7393A* G_B9_1 = NULL;
	int32_t G_B11_0 = 0;
	GameState_t5E590D2FB317152A4011A3CA69E3F311E0A7393A* G_B15_0 = NULL;
	GameState_t5E590D2FB317152A4011A3CA69E3F311E0A7393A* G_B14_0 = NULL;
	String_t* G_B16_0 = NULL;
	GameState_t5E590D2FB317152A4011A3CA69E3F311E0A7393A* G_B16_1 = NULL;
	String_t* G_B22_0 = NULL;
	String_t* G_B24_0 = NULL;
	String_t* G_B23_0 = NULL;
	String_t* G_B28_0 = NULL;
	StoryChapterCompletionData_tC9C085B3DD26C1C13EC45F2FDE18199EBB09FCCF* G_B37_0 = NULL;
	String_t* G_B41_0 = NULL;
	{
		StorySequenceRuntime_tE25F5316E4362523E731BEA985585EEF655D754D* L_0 = ((StoryService_t3C43407CE680194EA30F1337D5B8F05824E25419_StaticFields*)il2cpp_codegen_static_fields_for(StoryService_t3C43407CE680194EA30F1337D5B8F05824E25419_il2cpp_TypeInfo_var))->___activeSequence;
		if (L_0)
		{
			goto IL_0009;
		}
	}
	{
		return (int32_t)(0);
	}

IL_0009:
	{
		StorySequenceRuntime_tE25F5316E4362523E731BEA985585EEF655D754D* L_1 = ((StoryService_t3C43407CE680194EA30F1337D5B8F05824E25419_StaticFields*)il2cpp_codegen_static_fields_for(StoryService_t3C43407CE680194EA30F1337D5B8F05824E25419_il2cpp_TypeInfo_var))->___activeSequence;
		NullCheck(L_1);
		int32_t L_2;
		L_2 = StorySequenceRuntime_get_LineIndex_mF0BB9E6F271DB7D390200860605A234235613209_inline(L_1, NULL);
		StorySequenceRuntime_tE25F5316E4362523E731BEA985585EEF655D754D* L_3 = ((StoryService_t3C43407CE680194EA30F1337D5B8F05824E25419_StaticFields*)il2cpp_codegen_static_fields_for(StoryService_t3C43407CE680194EA30F1337D5B8F05824E25419_il2cpp_TypeInfo_var))->___activeSequence;
		NullCheck(L_3);
		StorySceneData_tD375EAB5A696E276CB12FBA76A9BA3836B716D2E* L_4;
		L_4 = StorySequenceRuntime_get_Scene_m9DD8AC50F2C595051725B50B977E000A7AF8514C_inline(L_3, NULL);
		NullCheck(L_4);
		StoryLineDataU5BU5D_t836CE45843BFF0C733F1EC73F7ADD4708932D4FD* L_5 = L_4->___lines;
		NullCheck(L_5);
		int32_t L_6 = (il2cpp_codegen_conv<int32_t,int64_t,int64_t,false,false>((((RuntimeArray*)L_5)->max_length),NULL));
		if ((((int32_t)((int32_t)il2cpp_codegen_add(L_2, 1))) >= ((int32_t)L_6)))
		{
			goto IL_0036;
		}
	}
	{
		StorySequenceRuntime_tE25F5316E4362523E731BEA985585EEF655D754D* L_7 = ((StoryService_t3C43407CE680194EA30F1337D5B8F05824E25419_StaticFields*)il2cpp_codegen_static_fields_for(StoryService_t3C43407CE680194EA30F1337D5B8F05824E25419_il2cpp_TypeInfo_var))->___activeSequence;
		NullCheck(L_7);
		bool L_8;
		L_8 = StorySequenceRuntime_Advance_mC9ECD206FAA99CE9854BDED5BB85DD92B142E93F(L_7, NULL);
		if (!L_8)
		{
			goto IL_0036;
		}
	}
	{
		return (int32_t)(1);
	}

IL_0036:
	{
		StorySequenceRuntime_tE25F5316E4362523E731BEA985585EEF655D754D* L_9 = ((StoryService_t3C43407CE680194EA30F1337D5B8F05824E25419_StaticFields*)il2cpp_codegen_static_fields_for(StoryService_t3C43407CE680194EA30F1337D5B8F05824E25419_il2cpp_TypeInfo_var))->___activeSequence;
		NullCheck(L_9);
		StorySceneData_tD375EAB5A696E276CB12FBA76A9BA3836B716D2E* L_10;
		L_10 = StorySequenceRuntime_get_Scene_m9DD8AC50F2C595051725B50B977E000A7AF8514C_inline(L_9, NULL);
		V_0 = L_10;
		GameState_t5E590D2FB317152A4011A3CA69E3F311E0A7393A* L_11;
		L_11 = GameState_get_Instance_m3202061BC5E0A39B53697C35D0BEE15692173D74_inline(NULL);
		V_1 = L_11;
		GameState_t5E590D2FB317152A4011A3CA69E3F311E0A7393A* L_12 = V_1;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_13;
		L_13 = Object_op_Inequality_mD0BE578448EAA61948F25C32F8DD55AB1F778602(L_12, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (!L_13)
		{
			goto IL_0064;
		}
	}
	{
		GameState_t5E590D2FB317152A4011A3CA69E3F311E0A7393A* L_14 = V_1;
		StorySceneData_tD375EAB5A696E276CB12FBA76A9BA3836B716D2E* L_15 = V_0;
		if (L_15)
		{
			G_B8_0 = L_14;
			goto IL_0057;
		}
		G_B7_0 = L_14;
	}
	{
		G_B9_0 = ((String_t*)(NULL));
		G_B9_1 = G_B7_0;
		goto IL_005d;
	}

IL_0057:
	{
		StorySceneData_tD375EAB5A696E276CB12FBA76A9BA3836B716D2E* L_16 = V_0;
		NullCheck(L_16);
		String_t* L_17 = L_16->___id;
		G_B9_0 = L_17;
		G_B9_1 = G_B8_0;
	}

IL_005d:
	{
		NullCheck(G_B9_1);
		bool L_18;
		L_18 = GameState_CompleteStoryScene_m74C9B3BB2CF98FEBFC15B4F3D9CA78568814A005(G_B9_1, G_B9_0, NULL);
		G_B11_0 = ((int32_t)(L_18));
		goto IL_0065;
	}

IL_0064:
	{
		G_B11_0 = 0;
	}

IL_0065:
	{
		V_2 = (bool)G_B11_0;
		bool L_19 = V_2;
		if (L_19)
		{
			goto IL_0088;
		}
	}
	{
		GameState_t5E590D2FB317152A4011A3CA69E3F311E0A7393A* L_20 = V_1;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_21;
		L_21 = Object_op_Equality_mB6120F782D83091EF56A198FCEBCF066DB4A9605(L_20, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (L_21)
		{
			goto IL_0086;
		}
	}
	{
		GameState_t5E590D2FB317152A4011A3CA69E3F311E0A7393A* L_22 = V_1;
		StorySceneData_tD375EAB5A696E276CB12FBA76A9BA3836B716D2E* L_23 = V_0;
		if (L_23)
		{
			G_B15_0 = L_22;
			goto IL_0079;
		}
		G_B14_0 = L_22;
	}
	{
		G_B16_0 = ((String_t*)(NULL));
		G_B16_1 = G_B14_0;
		goto IL_007f;
	}

IL_0079:
	{
		StorySceneData_tD375EAB5A696E276CB12FBA76A9BA3836B716D2E* L_24 = V_0;
		NullCheck(L_24);
		String_t* L_25 = L_24->___id;
		G_B16_0 = L_25;
		G_B16_1 = G_B15_0;
	}

IL_007f:
	{
		NullCheck(G_B16_1);
		bool L_26;
		L_26 = GameState_HasCompletedStoryScene_m6EAC4F445849D5D65DE62094EBE05EC0B7A3A480(G_B16_1, G_B16_0, NULL);
		if (L_26)
		{
			goto IL_0088;
		}
	}

IL_0086:
	{
		return (int32_t)(5);
	}

IL_0088:
	{
		StorySequenceRuntime_tE25F5316E4362523E731BEA985585EEF655D754D* L_27 = ((StoryService_t3C43407CE680194EA30F1337D5B8F05824E25419_StaticFields*)il2cpp_codegen_static_fields_for(StoryService_t3C43407CE680194EA30F1337D5B8F05824E25419_il2cpp_TypeInfo_var))->___activeSequence;
		NullCheck(L_27);
		bool L_28;
		L_28 = StorySequenceRuntime_Advance_mC9ECD206FAA99CE9854BDED5BB85DD92B142E93F(L_27, NULL);
		((StoryService_t3C43407CE680194EA30F1337D5B8F05824E25419_StaticFields*)il2cpp_codegen_static_fields_for(StoryService_t3C43407CE680194EA30F1337D5B8F05824E25419_il2cpp_TypeInfo_var))->___activeSequence = (StorySequenceRuntime_tE25F5316E4362523E731BEA985585EEF655D754D*)NULL;
		Il2CppCodeGenWriteBarrier((void**)(&((StoryService_t3C43407CE680194EA30F1337D5B8F05824E25419_StaticFields*)il2cpp_codegen_static_fields_for(StoryService_t3C43407CE680194EA30F1337D5B8F05824E25419_il2cpp_TypeInfo_var))->___activeSequence), (void*)(StorySequenceRuntime_tE25F5316E4362523E731BEA985585EEF655D754D*)NULL);
		bool L_29 = V_2;
		if (!L_29)
		{
			goto IL_00b7;
		}
	}
	{
		StorySceneData_tD375EAB5A696E276CB12FBA76A9BA3836B716D2E* L_30 = V_0;
		if (L_30)
		{
			goto IL_00a2;
		}
	}
	{
		G_B22_0 = ((String_t*)(NULL));
		goto IL_00a8;
	}

IL_00a2:
	{
		StorySceneData_tD375EAB5A696E276CB12FBA76A9BA3836B716D2E* L_31 = V_0;
		NullCheck(L_31);
		String_t* L_32 = L_31->___id;
		G_B22_0 = L_32;
	}

IL_00a8:
	{
		String_t* L_33 = G_B22_0;
		if (L_33)
		{
			G_B24_0 = L_33;
			goto IL_00b1;
		}
		G_B23_0 = L_33;
	}
	{
		String_t* L_34 = ((String_t_StaticFields*)il2cpp_codegen_static_fields_for(il2cpp_defaults.string_class))->___Empty;
		G_B24_0 = L_34;
	}

IL_00b1:
	{
		AnalyticsRecordResult_tEBE87AEB03C6D60620CF177295FC8B80E2EF350A* L_35;
		L_35 = PrivacyAwareAnalytics_TryStorySceneComplete_m85E90B1F88C64D5540A3DC54B84C3E955375A613(G_B24_0, NULL);
	}

IL_00b7:
	{
		StorySceneData_tD375EAB5A696E276CB12FBA76A9BA3836B716D2E* L_36 = V_0;
		if (L_36)
		{
			goto IL_00bd;
		}
	}
	{
		G_B28_0 = ((String_t*)(NULL));
		goto IL_00c3;
	}

IL_00bd:
	{
		StorySceneData_tD375EAB5A696E276CB12FBA76A9BA3836B716D2E* L_37 = V_0;
		NullCheck(L_37);
		String_t* L_38 = L_37->___returnRoute;
		G_B28_0 = L_38;
	}

IL_00c3:
	{
		V_4 = G_B28_0;
		String_t* L_39 = V_4;
		bool L_40;
		L_40 = String_op_Equality_m030E1B219352228970A076136E455C4E568C02C1(L_39, _stringLiteralE58C58F564E422E19147F21FFCD108F873525030, NULL);
		if (L_40)
		{
			goto IL_00f1;
		}
	}
	{
		String_t* L_41 = V_4;
		bool L_42;
		L_42 = String_op_Equality_m030E1B219352228970A076136E455C4E568C02C1(L_41, _stringLiteral8B2845342FA2C42DF223764F1ABEB2AC03D8EC4C, NULL);
		if (L_42)
		{
			goto IL_00f3;
		}
	}
	{
		String_t* L_43 = V_4;
		bool L_44;
		L_44 = String_op_Equality_m030E1B219352228970A076136E455C4E568C02C1(L_43, _stringLiteral8EF12CDC28C5A548D03CB52BB114214231854AEE, NULL);
		if (L_44)
		{
			goto IL_015b;
		}
	}
	{
		goto IL_015d;
	}

IL_00f1:
	{
		return (int32_t)(2);
	}

IL_00f3:
	{
		ChapterStoryCatalogData_t725340CD2FDB5256381D20B6F24C5A49578A6258* L_45;
		L_45 = StoryCatalog_LoadChapter1_m839F61020927D75959FA4E7CD214F7A5478A9433((bool)0, NULL);
		V_3 = L_45;
		GameState_t5E590D2FB317152A4011A3CA69E3F311E0A7393A* L_46 = V_1;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_47;
		L_47 = Object_op_Inequality_mD0BE578448EAA61948F25C32F8DD55AB1F778602(L_46, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (!L_47)
		{
			goto IL_014f;
		}
	}
	{
		ChapterStoryCatalogData_t725340CD2FDB5256381D20B6F24C5A49578A6258* L_48 = V_3;
		if (L_48)
		{
			goto IL_0109;
		}
	}
	{
		G_B37_0 = ((StoryChapterCompletionData_tC9C085B3DD26C1C13EC45F2FDE18199EBB09FCCF*)(NULL));
		goto IL_010f;
	}

IL_0109:
	{
		ChapterStoryCatalogData_t725340CD2FDB5256381D20B6F24C5A49578A6258* L_49 = V_3;
		NullCheck(L_49);
		StoryChapterCompletionData_tC9C085B3DD26C1C13EC45F2FDE18199EBB09FCCF* L_50 = L_49->___completion;
		G_B37_0 = L_50;
	}

IL_010f:
	{
		if (!G_B37_0)
		{
			goto IL_014f;
		}
	}
	{
		StorySceneData_tD375EAB5A696E276CB12FBA76A9BA3836B716D2E* L_51 = V_0;
		if (L_51)
		{
			goto IL_0117;
		}
	}
	{
		G_B41_0 = ((String_t*)(NULL));
		goto IL_011d;
	}

IL_0117:
	{
		StorySceneData_tD375EAB5A696E276CB12FBA76A9BA3836B716D2E* L_52 = V_0;
		NullCheck(L_52);
		String_t* L_53 = L_52->___id;
		G_B41_0 = L_53;
	}

IL_011d:
	{
		ChapterStoryCatalogData_t725340CD2FDB5256381D20B6F24C5A49578A6258* L_54 = V_3;
		NullCheck(L_54);
		StoryChapterCompletionData_tC9C085B3DD26C1C13EC45F2FDE18199EBB09FCCF* L_55 = L_54->___completion;
		NullCheck(L_55);
		String_t* L_56 = L_55->___finaleSceneId;
		bool L_57;
		L_57 = String_Equals_mCC34895D0DB2AD440C9D8767032215BC86B5C48B(G_B41_0, L_56, 4, NULL);
		if (!L_57)
		{
			goto IL_014f;
		}
	}
	{
		GameState_t5E590D2FB317152A4011A3CA69E3F311E0A7393A* L_58 = V_1;
		ChapterStoryCatalogData_t725340CD2FDB5256381D20B6F24C5A49578A6258* L_59 = V_3;
		NullCheck(L_59);
		int32_t L_60 = L_59->___chapterId;
		NullCheck(L_58);
		bool L_61;
		L_61 = GameState_HasPendingChapterCompletion_m68BEFCF6BEA8B02F74FC2355391324E9FB46E8A1(L_58, L_60, NULL);
		if (!L_61)
		{
			goto IL_014f;
		}
	}
	{
		bool L_62 = V_2;
		if (!L_62)
		{
			goto IL_014d;
		}
	}
	{
		ChapterStoryCatalogData_t725340CD2FDB5256381D20B6F24C5A49578A6258* L_63 = V_3;
		NullCheck(L_63);
		int32_t L_64 = L_63->___chapterId;
		AnalyticsRecordResult_tEBE87AEB03C6D60620CF177295FC8B80E2EF350A* L_65;
		L_65 = PrivacyAwareAnalytics_TryChapterComplete_m3AF25BA4859D9A3A37E57AF47A1D61A5A02AA7B3(L_64, NULL);
	}

IL_014d:
	{
		return (int32_t)(4);
	}

IL_014f:
	{
		il2cpp_codegen_runtime_class_init_inline(Debug_t8394C7EEAECA3689C2C9B9DE9C7166D73596276F_il2cpp_TypeInfo_var);
		Debug_LogError_mB00B2B4468EF3CAF041B038D840820FB84C924B2(_stringLiteral567412B430512EC07C99F863C4AB47828BE73F7B, NULL);
		return (int32_t)(3);
	}

IL_015b:
	{
		return (int32_t)(3);
	}

IL_015d:
	{
		return (int32_t)(0);
	}
}
