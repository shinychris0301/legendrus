IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void GameState_MigrateSave_m21A52D105BED5ADE645083E6A277934611E42B00 (GameState_t5E590D2FB317152A4011A3CA69E3F311E0A7393A* __this, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&DateTime_t66193957C73913903DDAD89FEDC46139BCA5802D_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Debug_t8394C7EEAECA3689C2C9B9DE9C7166D73596276F_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Func_2_tCDF0B706370B73CA384A871235C0F913147B8FD6_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&GameState_BuildLegacyProgress_m29D317320666AB6F228EE5BBBA8855FC81BCB8E5_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&_stringLiteral40E013878832B9162F4CEF967527A5E3EC83E574);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&_stringLiteralFC0B56648AF9FCEE8B7DB8823A4A9086500D41AD);
		s_Il2CppMethodInitialized = true;
	}
	String_t* V_0 = NULL;
	PlayerSaveMigrationResult_t1986C343B499B3EBC442B21B5663A49CEA598A19* V_1 = NULL;
	String_t* V_2 = NULL;
	DateTime_t66193957C73913903DDAD89FEDC46139BCA5802D V_3;
	memset((&V_3), 0, sizeof(V_3));
	String_t* G_B6_0 = NULL;
	String_t* G_B5_0 = NULL;
	String_t* G_B7_0 = NULL;
	String_t* G_B7_1 = NULL;
	{
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_0 = __this->___Save;
		if (L_0)
		{
			goto IL_0013;
		}
	}
	{
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_1 = (PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30*)il2cpp_codegen_object_new(PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30_il2cpp_TypeInfo_var);
		PlayerSaveData__ctor_m7A7E9DB622A56147969520D66AF2CD47097003D5(L_1, NULL);
		__this->___Save = L_1;
		Il2CppCodeGenWriteBarrier((void**)(&__this->___Save), (void*)L_1);
	}

IL_0013:
	{
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_2 = __this->___Save;
		String_t* L_3;
		L_3 = JsonUtility_ToJson_m28CC6843B9D3723D88AD13EA3829B71FDE7826BA(L_2, NULL);
		V_0 = L_3;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_4 = __this->___Save;
		Func_2_tCDF0B706370B73CA384A871235C0F913147B8FD6* L_5 = (Func_2_tCDF0B706370B73CA384A871235C0F913147B8FD6*)il2cpp_codegen_object_new(Func_2_tCDF0B706370B73CA384A871235C0F913147B8FD6_il2cpp_TypeInfo_var);
		Func_2__ctor_m57AA45441A6431D5158CEBD3B493A45CF0006045(L_5, __this, (intptr_t)((void*)GameState_BuildLegacyProgress_m29D317320666AB6F228EE5BBBA8855FC81BCB8E5_RuntimeMethod_var), NULL);
		il2cpp_codegen_runtime_class_init_inline(DateTime_t66193957C73913903DDAD89FEDC46139BCA5802D_il2cpp_TypeInfo_var);
		DateTime_t66193957C73913903DDAD89FEDC46139BCA5802D L_6;
		L_6 = DateTime_get_UtcNow_m06B6E9995FE16846A0F71EC9DB23E90BE2C5F9FA(NULL);
		V_3 = L_6;
		int64_t L_7;
		L_7 = DateTime_get_Ticks_mC2CF04ED0EAB425C72C2532FFC5743777F3C93A6((&V_3), NULL);
		PlayerSaveMigrationResult_t1986C343B499B3EBC442B21B5663A49CEA598A19* L_8;
		L_8 = PlayerSaveMigrationRegistry_Migrate_mA925853C0A92B4D2CC9FD5CD9C5C115F28183ADC(L_4, L_5, L_7, NULL);
		V_1 = L_8;
		PlayerSaveMigrationResult_t1986C343B499B3EBC442B21B5663A49CEA598A19* L_9 = V_1;
		NullCheck(L_9);
		bool L_10;
		L_10 = PlayerSaveMigrationResult_get_Succeeded_mE137AA88405037CA3324E07C3995D7C3FC49AC41_inline(L_9, NULL);
		if (!L_10)
		{
			goto IL_0054;
		}
	}
	{
		PlayerSaveMigrationResult_t1986C343B499B3EBC442B21B5663A49CEA598A19* L_11 = V_1;
		NullCheck(L_11);
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_12;
		L_12 = PlayerSaveMigrationResult_get_Data_m98230900B350131A64403C45DF020B38D1A4625F_inline(L_11, NULL);
		if (L_12)
		{
			goto IL_0074;
		}
	}

IL_0054:
	{
		PlayerSaveMigrationResult_t1986C343B499B3EBC442B21B5663A49CEA598A19* L_13 = V_1;
		if (L_13)
		{
			G_B6_0 = _stringLiteral40E013878832B9162F4CEF967527A5E3EC83E574;
			goto IL_0063;
		}
		G_B5_0 = _stringLiteral40E013878832B9162F4CEF967527A5E3EC83E574;
	}
	{
		G_B7_0 = _stringLiteralFC0B56648AF9FCEE8B7DB8823A4A9086500D41AD;
		G_B7_1 = G_B5_0;
		goto IL_0069;
	}

IL_0063:
	{
		PlayerSaveMigrationResult_t1986C343B499B3EBC442B21B5663A49CEA598A19* L_14 = V_1;
		NullCheck(L_14);
		String_t* L_15;
		L_15 = PlayerSaveMigrationResult_get_ReasonCode_m5F0A2D5DCFE3D4CA31055187FC7A4B2866DFFBFB_inline(L_14, NULL);
		G_B7_0 = L_15;
		G_B7_1 = G_B6_0;
	}

IL_0069:
	{
		String_t* L_16;
		L_16 = String_Concat_m9E3155FB84015C823606188F53B47CB44C444991(G_B7_1, G_B7_0, NULL);
		il2cpp_codegen_runtime_class_init_inline(Debug_t8394C7EEAECA3689C2C9B9DE9C7166D73596276F_il2cpp_TypeInfo_var);
		Debug_LogError_mB00B2B4468EF3CAF041B038D840820FB84C924B2(L_16, NULL);
		return;
	}

IL_0074:
	{
		PlayerSaveMigrationResult_t1986C343B499B3EBC442B21B5663A49CEA598A19* L_17 = V_1;
		NullCheck(L_17);
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_18;
		L_18 = PlayerSaveMigrationResult_get_Data_m98230900B350131A64403C45DF020B38D1A4625F_inline(L_17, NULL);
		__this->___Save = L_18;
		Il2CppCodeGenWriteBarrier((void**)(&__this->___Save), (void*)L_18);
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_19 = __this->___Save;
		NullCheck(L_19);
		RetainedRoundData_t3EA1BA932DFF2408098F1FB87EC5A5A24130D7A1* L_20 = L_19->___retainedRound;
		if (!L_20)
		{
			goto IL_00b0;
		}
	}
	{
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_21 = __this->___Save;
		NullCheck(L_21);
		RetainedRoundData_t3EA1BA932DFF2408098F1FB87EC5A5A24130D7A1* L_22 = L_21->___retainedRound;
		NullCheck(L_22);
		String_t* L_23 = L_22->___attemptId;
		bool L_24;
		L_24 = String_IsNullOrEmpty_mEA9E3FB005AC28FE02E69FCF95A7B8456192B478(L_23, NULL);
		if (!L_24)
		{
			goto IL_00b0;
		}
	}
	{
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_25 = __this->___Save;
		NullCheck(L_25);
		L_25->___retainedRound = (RetainedRoundData_t3EA1BA932DFF2408098F1FB87EC5A5A24130D7A1*)NULL;
		Il2CppCodeGenWriteBarrier((void**)(&L_25->___retainedRound), (void*)(RetainedRoundData_t3EA1BA932DFF2408098F1FB87EC5A5A24130D7A1*)NULL);
	}

IL_00b0:
	{
		GameState_NormalizeLevelProgress_mBF9AC2241D825C45C9CA835B29E689C991152E56(__this, NULL);
		GameState_NormalizeTutorialProgress_m4498B5C32A4EE504B4EC4E92ED3586F6D54A10DE(__this, NULL);
		GameState_NormalizeStoryProgress_mE7A45F8500313553FB22B4A75404FCE270582216(__this, NULL);
		GameState_NormalizeChapterCompletionProgress_mB1CA87B59988D112B3B967E6539D1C7BBE6BBBB8(__this, NULL);
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_26 = __this->___Save;
		String_t* L_27;
		L_27 = JsonUtility_ToJson_m28CC6843B9D3723D88AD13EA3829B71FDE7826BA(L_26, NULL);
		V_2 = L_27;
		bool L_28 = __this->___saveRewriteRequired;
		if (L_28)
		{
			goto IL_00ee;
		}
	}
	{
		PlayerSaveMigrationResult_t1986C343B499B3EBC442B21B5663A49CEA598A19* L_29 = V_1;
		NullCheck(L_29);
		bool L_30;
		L_30 = PlayerSaveMigrationResult_get_Changed_m63BC23F8FCEB41A45E769F668983951A0572E845_inline(L_29, NULL);
		if (L_30)
		{
			goto IL_00ee;
		}
	}
	{
		String_t* L_31 = V_0;
		String_t* L_32 = V_2;
		bool L_33;
		L_33 = String_Equals_mCC34895D0DB2AD440C9D8767032215BC86B5C48B(L_31, L_32, 4, NULL);
		if (L_33)
		{
			goto IL_00f4;
		}
	}

IL_00ee:
	{
		GameState_Persist_m0261476FB722D9A1BAB0BB601B749EFC7E307C8D(__this, NULL);
	}

IL_00f4:
	{
		__this->___saveRewriteRequired = (bool)0;
		return;
	}
}
