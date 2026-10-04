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
	GameState_t5E590D2FB317152A4011A3CA69E3F311E0A7393A* G_B7_0 = NULL;
	GameState_t5E590D2FB317152A4011A3CA69E3F311E0A7393A* G_B6_0 = NULL;
	String_t* G_B8_0 = NULL;
	GameState_t5E590D2FB317152A4011A3CA69E3F311E0A7393A* G_B8_1 = NULL;
	int32_t G_B10_0 = 0;
	String_t* G_B14_0 = NULL;
	String_t* G_B16_0 = NULL;
	String_t* G_B15_0 = NULL;
	String_t* G_B20_0 = NULL;
	StoryChapterCompletionData_tC9C085B3DD26C1C13EC45F2FDE18199EBB09FCCF* G_B29_0 = NULL;
	String_t* G_B33_0 = NULL;
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
		bool L_2;
		L_2 = StorySequenceRuntime_Advance_mC9ECD206FAA99CE9854BDED5BB85DD92B142E93F(L_1, NULL);
		if (!L_2)
		{
			goto IL_0017;
		}
	}
	{
		return (int32_t)(1);
	}

IL_0017:
	{
		StorySequenceRuntime_tE25F5316E4362523E731BEA985585EEF655D754D* L_3 = ((StoryService_t3C43407CE680194EA30F1337D5B8F05824E25419_StaticFields*)il2cpp_codegen_static_fields_for(StoryService_t3C43407CE680194EA30F1337D5B8F05824E25419_il2cpp_TypeInfo_var))->___activeSequence;
		NullCheck(L_3);
		StorySceneData_tD375EAB5A696E276CB12FBA76A9BA3836B716D2E* L_4;
		L_4 = StorySequenceRuntime_get_Scene_m9DD8AC50F2C595051725B50B977E000A7AF8514C_inline(L_3, NULL);
		V_0 = L_4;
		((StoryService_t3C43407CE680194EA30F1337D5B8F05824E25419_StaticFields*)il2cpp_codegen_static_fields_for(StoryService_t3C43407CE680194EA30F1337D5B8F05824E25419_il2cpp_TypeInfo_var))->___activeSequence = (StorySequenceRuntime_tE25F5316E4362523E731BEA985585EEF655D754D*)NULL;
		Il2CppCodeGenWriteBarrier((void**)(&((StoryService_t3C43407CE680194EA30F1337D5B8F05824E25419_StaticFields*)il2cpp_codegen_static_fields_for(StoryService_t3C43407CE680194EA30F1337D5B8F05824E25419_il2cpp_TypeInfo_var))->___activeSequence), (void*)(StorySequenceRuntime_tE25F5316E4362523E731BEA985585EEF655D754D*)NULL);
		GameState_t5E590D2FB317152A4011A3CA69E3F311E0A7393A* L_5;
		L_5 = GameState_get_Instance_m3202061BC5E0A39B53697C35D0BEE15692173D74_inline(NULL);
		V_1 = L_5;
		GameState_t5E590D2FB317152A4011A3CA69E3F311E0A7393A* L_6 = V_1;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_7;
		L_7 = Object_op_Inequality_mD0BE578448EAA61948F25C32F8DD55AB1F778602(L_6, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (!L_7)
		{
			goto IL_004b;
		}
	}
	{
		GameState_t5E590D2FB317152A4011A3CA69E3F311E0A7393A* L_8 = V_1;
		StorySceneData_tD375EAB5A696E276CB12FBA76A9BA3836B716D2E* L_9 = V_0;
		if (L_9)
		{
			G_B7_0 = L_8;
			goto IL_003e;
		}
		G_B6_0 = L_8;
	}
	{
		G_B8_0 = ((String_t*)(NULL));
		G_B8_1 = G_B6_0;
		goto IL_0044;
	}

IL_003e:
	{
		StorySceneData_tD375EAB5A696E276CB12FBA76A9BA3836B716D2E* L_10 = V_0;
		NullCheck(L_10);
		String_t* L_11 = L_10->___id;
		G_B8_0 = L_11;
		G_B8_1 = G_B7_0;
	}

IL_0044:
	{
		NullCheck(G_B8_1);
		bool L_12;
		L_12 = GameState_CompleteStoryScene_m74C9B3BB2CF98FEBFC15B4F3D9CA78568814A005(G_B8_1, G_B8_0, NULL);
		G_B10_0 = ((int32_t)(L_12));
		goto IL_004c;
	}

IL_004b:
	{
		G_B10_0 = 0;
	}

IL_004c:
	{
		V_2 = (bool)G_B10_0;
		bool L_13 = V_2;
		if (!L_13)
		{
			goto IL_006b;
		}
	}
	{
		StorySceneData_tD375EAB5A696E276CB12FBA76A9BA3836B716D2E* L_14 = V_0;
		if (L_14)
		{
			goto IL_0056;
		}
	}
	{
		G_B14_0 = ((String_t*)(NULL));
		goto IL_005c;
	}

IL_0056:
	{
		StorySceneData_tD375EAB5A696E276CB12FBA76A9BA3836B716D2E* L_15 = V_0;
		NullCheck(L_15);
		String_t* L_16 = L_15->___id;
		G_B14_0 = L_16;
	}

IL_005c:
	{
		String_t* L_17 = G_B14_0;
		if (L_17)
		{
			G_B16_0 = L_17;
			goto IL_0065;
		}
		G_B15_0 = L_17;
	}
	{
		String_t* L_18 = ((String_t_StaticFields*)il2cpp_codegen_static_fields_for(il2cpp_defaults.string_class))->___Empty;
		G_B16_0 = L_18;
	}

IL_0065:
	{
		AnalyticsRecordResult_tEBE87AEB03C6D60620CF177295FC8B80E2EF350A* L_19;
		L_19 = PrivacyAwareAnalytics_TryStorySceneComplete_m85E90B1F88C64D5540A3DC54B84C3E955375A613(G_B16_0, NULL);
	}

IL_006b:
	{
		StorySceneData_tD375EAB5A696E276CB12FBA76A9BA3836B716D2E* L_20 = V_0;
		if (L_20)
		{
			goto IL_0071;
		}
	}
	{
		G_B20_0 = ((String_t*)(NULL));
		goto IL_0077;
	}

IL_0071:
	{
		StorySceneData_tD375EAB5A696E276CB12FBA76A9BA3836B716D2E* L_21 = V_0;
		NullCheck(L_21);
		String_t* L_22 = L_21->___returnRoute;
		G_B20_0 = L_22;
	}

IL_0077:
	{
		V_4 = G_B20_0;
		String_t* L_23 = V_4;
		bool L_24;
		L_24 = String_op_Equality_m030E1B219352228970A076136E455C4E568C02C1(L_23, _stringLiteralE58C58F564E422E19147F21FFCD108F873525030, NULL);
		if (L_24)
		{
			goto IL_00a5;
		}
	}
	{
		String_t* L_25 = V_4;
		bool L_26;
		L_26 = String_op_Equality_m030E1B219352228970A076136E455C4E568C02C1(L_25, _stringLiteral8B2845342FA2C42DF223764F1ABEB2AC03D8EC4C, NULL);
		if (L_26)
		{
			goto IL_00a7;
		}
	}
	{
		String_t* L_27 = V_4;
		bool L_28;
		L_28 = String_op_Equality_m030E1B219352228970A076136E455C4E568C02C1(L_27, _stringLiteral8EF12CDC28C5A548D03CB52BB114214231854AEE, NULL);
		if (L_28)
		{
			goto IL_010f;
		}
	}
	{
		goto IL_0111;
	}

IL_00a5:
	{
		return (int32_t)(2);
	}

IL_00a7:
	{
		ChapterStoryCatalogData_t725340CD2FDB5256381D20B6F24C5A49578A6258* L_29;
		L_29 = StoryCatalog_LoadChapter1_m839F61020927D75959FA4E7CD214F7A5478A9433((bool)0, NULL);
		V_3 = L_29;
		GameState_t5E590D2FB317152A4011A3CA69E3F311E0A7393A* L_30 = V_1;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_31;
		L_31 = Object_op_Inequality_mD0BE578448EAA61948F25C32F8DD55AB1F778602(L_30, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (!L_31)
		{
			goto IL_0103;
		}
	}
	{
		ChapterStoryCatalogData_t725340CD2FDB5256381D20B6F24C5A49578A6258* L_32 = V_3;
		if (L_32)
		{
			goto IL_00bd;
		}
	}
	{
		G_B29_0 = ((StoryChapterCompletionData_tC9C085B3DD26C1C13EC45F2FDE18199EBB09FCCF*)(NULL));
		goto IL_00c3;
	}

IL_00bd:
	{
		ChapterStoryCatalogData_t725340CD2FDB5256381D20B6F24C5A49578A6258* L_33 = V_3;
		NullCheck(L_33);
		StoryChapterCompletionData_tC9C085B3DD26C1C13EC45F2FDE18199EBB09FCCF* L_34 = L_33->___completion;
		G_B29_0 = L_34;
	}

IL_00c3:
	{
		if (!G_B29_0)
		{
			goto IL_0103;
		}
	}
	{
		StorySceneData_tD375EAB5A696E276CB12FBA76A9BA3836B716D2E* L_35 = V_0;
		if (L_35)
		{
			goto IL_00cb;
		}
	}
	{
		G_B33_0 = ((String_t*)(NULL));
		goto IL_00d1;
	}

IL_00cb:
	{
		StorySceneData_tD375EAB5A696E276CB12FBA76A9BA3836B716D2E* L_36 = V_0;
		NullCheck(L_36);
		String_t* L_37 = L_36->___id;
		G_B33_0 = L_37;
	}

IL_00d1:
	{
		ChapterStoryCatalogData_t725340CD2FDB5256381D20B6F24C5A49578A6258* L_38 = V_3;
		NullCheck(L_38);
		StoryChapterCompletionData_tC9C085B3DD26C1C13EC45F2FDE18199EBB09FCCF* L_39 = L_38->___completion;
		NullCheck(L_39);
		String_t* L_40 = L_39->___finaleSceneId;
		bool L_41;
		L_41 = String_Equals_mCC34895D0DB2AD440C9D8767032215BC86B5C48B(G_B33_0, L_40, 4, NULL);
		if (!L_41)
		{
			goto IL_0103;
		}
	}
	{
		GameState_t5E590D2FB317152A4011A3CA69E3F311E0A7393A* L_42 = V_1;
		ChapterStoryCatalogData_t725340CD2FDB5256381D20B6F24C5A49578A6258* L_43 = V_3;
		NullCheck(L_43);
		int32_t L_44 = L_43->___chapterId;
		NullCheck(L_42);
		bool L_45;
		L_45 = GameState_HasPendingChapterCompletion_m68BEFCF6BEA8B02F74FC2355391324E9FB46E8A1(L_42, L_44, NULL);
		if (!L_45)
		{
			goto IL_0103;
		}
	}
	{
		bool L_46 = V_2;
		if (!L_46)
		{
			goto IL_0101;
		}
	}
	{
		ChapterStoryCatalogData_t725340CD2FDB5256381D20B6F24C5A49578A6258* L_47 = V_3;
		NullCheck(L_47);
		int32_t L_48 = L_47->___chapterId;
		AnalyticsRecordResult_tEBE87AEB03C6D60620CF177295FC8B80E2EF350A* L_49;
		L_49 = PrivacyAwareAnalytics_TryChapterComplete_m3AF25BA4859D9A3A37E57AF47A1D61A5A02AA7B3(L_48, NULL);
	}

IL_0101:
	{
		return (int32_t)(4);
	}

IL_0103:
	{
		il2cpp_codegen_runtime_class_init_inline(Debug_t8394C7EEAECA3689C2C9B9DE9C7166D73596276F_il2cpp_TypeInfo_var);
		Debug_LogError_mB00B2B4468EF3CAF041B038D840820FB84C924B2(_stringLiteral567412B430512EC07C99F863C4AB47828BE73F7B, NULL);
		return (int32_t)(3);
	}

IL_010f:
	{
		return (int32_t)(3);
	}

IL_0111:
	{
		return (int32_t)(0);
	}
}
