IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void ScreenRouter_StartLevel_m6B0E46700FE5719B45D6234AA7705C8A66915F81 (ScreenRouter_t9EC367B45B1296F4CF0CBDEA57AEF86CC5340C00* __this, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ScreenRouter_StartLevel_m6B0E46700FE5719B45D6234AA7705C8A66915F81_RuntimeMethod_var);
		s_Il2CppMethodInitialized = true;
	}
	LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* V_0 = NULL;
	int32_t V_1 = 0;
	bool V_2 = false;
	RetainedRoundData_t3EA1BA932DFF2408098F1FB87EC5A5A24130D7A1* V_3 = NULL;
	ScreenRouter_t9EC367B45B1296F4CF0CBDEA57AEF86CC5340C00* G_B11_0 = NULL;
	ScreenRouter_t9EC367B45B1296F4CF0CBDEA57AEF86CC5340C00* G_B10_0 = NULL;
	Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* G_B12_0 = NULL;
	ScreenRouter_t9EC367B45B1296F4CF0CBDEA57AEF86CC5340C00* G_B12_1 = NULL;
	{
		bool L_0 = __this->___levelStartInProgress;
		if (!L_0)
		{
			goto IL_0009;
		}
	}
	{
		return;
	}

IL_0009:
	{
		__this->___levelStartInProgress = (bool)1;
	}
	{
		auto __finallyBlock = il2cpp::utils::Finally([&]
		{

FINALLY_0110:
			{
				__this->___pendingGameplayLevel = (LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC*)NULL;
				Il2CppCodeGenWriteBarrier((void**)(&__this->___pendingGameplayLevel), (void*)(LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC*)NULL);
				__this->___pendingGameplayAttempt = 0;
				__this->___levelStartInProgress = (bool)0;
				return;
			}
		});
		try
		{
			{
				GameState_t5E590D2FB317152A4011A3CA69E3F311E0A7393A* L_1;
				L_1 = GameState_get_Instance_m3202061BC5E0A39B53697C35D0BEE15692173D74_inline(NULL);
				NullCheck(L_1);
				bool L_2;
				L_2 = GameState_TryGetRetainedRound_m2295FF12D42D33C9E7E1FF1D8B33B8E3859793A5(L_1, (&V_3), NULL);
				if (!L_2)
				{
					goto IL_002a_1;
				}
			}
			{
				ScreenRouter_Show_mB289A1408F5F71A5043EFE930648595D5DCE3734(__this, 8, NULL);
				goto IL_0126;
			}

IL_002a_1:
			{
				GameState_t5E590D2FB317152A4011A3CA69E3F311E0A7393A* L_3;
				L_3 = GameState_get_Instance_m3202061BC5E0A39B53697C35D0BEE15692173D74_inline(NULL);
				NullCheck(L_3);
				bool L_4;
				L_4 = GameState_get_CanPersist_m7BB53E3FC5B0F45E04855E3B98180B34F0373ED4_inline(L_3, NULL);
				if (L_4)
				{
					goto IL_0043_1;
				}
			}
			{
				bool L_5;
				L_5 = ScreenRouter_ShowErrorDialog_m54E0B54656DDD07A0CC9A5294216CC8A992C4823(__this, (Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07*)NULL, NULL);
				goto IL_0126;
			}

IL_0043_1:
			{
				GameState_t5E590D2FB317152A4011A3CA69E3F311E0A7393A* L_6;
				L_6 = GameState_get_Instance_m3202061BC5E0A39B53697C35D0BEE15692173D74_inline(NULL);
				NullCheck(L_6);
				bool L_7;
				L_7 = GameState_TryRecoverPendingBoosterCommand_mD849EAE55391848A18D9B3D447FD83FA2B4B6823(L_6, NULL);
				if (!L_7)
				{
					goto IL_005b_1;
				}
			}
			{
				GameState_t5E590D2FB317152A4011A3CA69E3F311E0A7393A* L_8;
				L_8 = GameState_get_Instance_m3202061BC5E0A39B53697C35D0BEE15692173D74_inline(NULL);
				NullCheck(L_8);
				bool L_9;
				L_9 = GameState_get_CanPersist_m7BB53E3FC5B0F45E04855E3B98180B34F0373ED4_inline(L_8, NULL);
				if (L_9)
				{
					goto IL_0082_1;
				}
			}

IL_005b_1:
			{
				GameState_t5E590D2FB317152A4011A3CA69E3F311E0A7393A* L_10;
				L_10 = GameState_get_Instance_m3202061BC5E0A39B53697C35D0BEE15692173D74_inline(NULL);
				NullCheck(L_10);
				bool L_11;
				L_11 = GameState_get_CanPersist_m7BB53E3FC5B0F45E04855E3B98180B34F0373ED4_inline(L_10, NULL);
				if (L_11)
				{
					G_B11_0 = __this;
					goto IL_006b_1;
				}
				G_B10_0 = __this;
			}
			{
				G_B12_0 = ((Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07*)(NULL));
				G_B12_1 = G_B10_0;
				goto IL_0077_1;
			}

IL_006b_1:
			{
				Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* L_12 = (Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07*)il2cpp_codegen_object_new(Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07_il2cpp_TypeInfo_var);
				Action__ctor_mBDC7B0B4A3F583B64C2896F01BDED360772F67DC(L_12, __this, (intptr_t)((void*)ScreenRouter_StartLevel_m6B0E46700FE5719B45D6234AA7705C8A66915F81_RuntimeMethod_var), NULL);
				G_B12_0 = L_12;
				G_B12_1 = G_B11_0;
			}

IL_0077_1:
			{
				NullCheck(G_B12_1);
				bool L_13;
				L_13 = ScreenRouter_ShowErrorDialog_m54E0B54656DDD07A0CC9A5294216CC8A992C4823(G_B12_1, G_B12_0, NULL);
				goto IL_0126;
			}

IL_0082_1:
			{
				GameState_t5E590D2FB317152A4011A3CA69E3F311E0A7393A* L_14;
				L_14 = GameState_get_Instance_m3202061BC5E0A39B53697C35D0BEE15692173D74_inline(NULL);
				NullCheck(L_14);
				bool L_15;
				L_15 = GameState_TryEnsureSelectedLevelIsUnlocked_mB10422023D3F25FEEAFAC46AF1BC583D4F220D90(L_14, NULL);
				if (L_15)
				{
					goto IL_009b_1;
				}
			}
			{
				bool L_16;
				L_16 = ScreenRouter_ShowErrorDialog_m54E0B54656DDD07A0CC9A5294216CC8A992C4823(__this, (Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07*)NULL, NULL);
				goto IL_0126;
			}

IL_009b_1:
			{
				GameState_t5E590D2FB317152A4011A3CA69E3F311E0A7393A* L_17;
				L_17 = GameState_get_Instance_m3202061BC5E0A39B53697C35D0BEE15692173D74_inline(NULL);
				NullCheck(L_17);
				PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_18 = L_17->___Save;
				LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* L_19;
				L_19 = ChapterLevelCatalog_ResolveSelected_m31E802BDED4683B59A35F355FA271E8165513D93(L_18, NULL);
				V_0 = L_19;
				GameState_t5E590D2FB317152A4011A3CA69E3F311E0A7393A* L_20;
				L_20 = GameState_get_Instance_m3202061BC5E0A39B53697C35D0BEE15692173D74_inline(NULL);
				LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* L_21 = V_0;
				NullCheck(L_20);
				bool L_22;
				L_22 = GameState_TryStartLevel_mF8B8C9FE72F7FDB5CFDAFF42E0EA26303D283D30(L_20, L_21, (&V_1), (&V_2), NULL);
				if (L_22)
				{
					goto IL_00f9_1;
				}
			}
			{
				bool L_23 = V_2;
				if (L_23)
				{
					goto IL_00c9_1;
				}
			}
			{
				bool L_24;
				L_24 = ScreenRouter_ShowErrorDialog_m54E0B54656DDD07A0CC9A5294216CC8A992C4823(__this, (Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07*)NULL, NULL);
				goto IL_0126;
			}

IL_00c9_1:
			{
				LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* L_25 = V_0;
				if (!L_25)
				{
					goto IL_00ee_1;
				}
			}
			{
				LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* L_26 = V_0;
				NullCheck(L_26);
				String_t* L_27 = L_26->___id;
				GameState_t5E590D2FB317152A4011A3CA69E3F311E0A7393A* L_28;
				L_28 = GameState_get_Instance_m3202061BC5E0A39B53697C35D0BEE15692173D74_inline(NULL);
				NullCheck(L_28);
				PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_29 = L_28->___Save;
				NullCheck(L_29);
				int32_t L_30 = L_29->___energy;
				int32_t L_31;
				L_31 = Mathf_Max_m7FA442918DE37E3A00106D1F2E789D65829792B8_inline(0, L_30, NULL);
				AnalyticsRecordResult_tEBE87AEB03C6D60620CF177295FC8B80E2EF350A* L_32;
				L_32 = PrivacyAwareAnalytics_TryEnergyGateBlocked_m00AF140F73FAA715FB6DD465E009772D2645EFA4(L_27, 1, L_31, NULL);
			}

IL_00ee_1:
			{
				bool L_33;
				L_33 = ScreenRouter_ShowModal_m195E7C54FEA3162B7A49F9BF61E3CA321B3AE5D9(__this, ((int32_t)20), NULL);
				goto IL_0126;
			}

IL_00f9_1:
			{
				LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* L_34 = V_0;
				__this->___pendingGameplayLevel = L_34;
				Il2CppCodeGenWriteBarrier((void**)(&__this->___pendingGameplayLevel), (void*)L_34);
				int32_t L_35 = V_1;
				__this->___pendingGameplayAttempt = L_35;
				ScreenRouter_Show_mB289A1408F5F71A5043EFE930648595D5DCE3734(__this, 5, NULL);
				goto IL_0126;
			}
		}
		catch(Il2CppNativeThreadAbortException&)
		{
			__finallyBlock.SetNativeThreadAbortOccurred();
		}
		catch(Il2CppExceptionWrapper& e)
		{
			__finallyBlock.StoreException(e.ex);
		}
	}

IL_0126:
	{
		return;
	}
}
