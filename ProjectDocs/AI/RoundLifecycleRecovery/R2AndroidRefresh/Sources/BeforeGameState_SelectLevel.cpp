IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool GameState_SelectLevel_m6055ECAE894AC442764BC224B6B3FB0ABA949570 (GameState_t5E590D2FB317152A4011A3CA69E3F311E0A7393A* __this, int32_t ___0_chapterId, int32_t ___1_levelId, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&JsonUtility_FromJson_TisPlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30_m5FA532D489BA59FC0DD187FA87D56E71F1F63B04_RuntimeMethod_var);
		s_Il2CppMethodInitialized = true;
	}
	RetainedRoundData_t3EA1BA932DFF2408098F1FB87EC5A5A24130D7A1* V_0 = NULL;
	PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* V_1 = NULL;
	{
		bool L_0 = __this->___canPersist;
		if (!L_0)
		{
			goto IL_0010;
		}
	}
	{
		bool L_1;
		L_1 = GameState_ContinuationBlocksNewAttempt_m908136F30BB6F308311486AF5F487FAD53014484(__this, NULL);
		if (!L_1)
		{
			goto IL_0012;
		}
	}

IL_0010:
	{
		return (bool)0;
	}

IL_0012:
	{
		bool L_2;
		L_2 = GameState_TryGetRetainedRound_m2295FF12D42D33C9E7E1FF1D8B33B8E3859793A5(__this, (&V_0), NULL);
		if (!L_2)
		{
			goto IL_0030;
		}
	}
	{
		RetainedRoundData_t3EA1BA932DFF2408098F1FB87EC5A5A24130D7A1* L_3 = V_0;
		NullCheck(L_3);
		int32_t L_4 = L_3->___chapter;
		int32_t L_5 = ___0_chapterId;
		if ((!(((uint32_t)L_4) == ((uint32_t)L_5))))
		{
			goto IL_002e;
		}
	}
	{
		RetainedRoundData_t3EA1BA932DFF2408098F1FB87EC5A5A24130D7A1* L_6 = V_0;
		NullCheck(L_6);
		int32_t L_7 = L_6->___level;
		int32_t L_8 = ___1_levelId;
		if ((((int32_t)L_7) == ((int32_t)L_8)))
		{
			goto IL_0030;
		}
	}

IL_002e:
	{
		return (bool)0;
	}

IL_0030:
	{
		int32_t L_9 = ___0_chapterId;
		int32_t L_10 = ___1_levelId;
		bool L_11;
		L_11 = GameState_IsLevelUnlocked_mA15470234C68329950931198C078B05B900F0689(__this, L_9, L_10, NULL);
		if (L_11)
		{
			goto IL_003c;
		}
	}
	{
		return (bool)0;
	}

IL_003c:
	{
		int32_t L_12 = ___0_chapterId;
		int32_t L_13 = ___1_levelId;
		LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* L_14;
		L_14 = ChapterLevelCatalog_Find_mB0A3275A64F35FB511F90A6049719C9B1971FDC6(L_12, L_13, NULL);
		if (L_14)
		{
			goto IL_0047;
		}
	}
	{
		return (bool)0;
	}

IL_0047:
	{
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_15 = __this->___Save;
		String_t* L_16;
		L_16 = JsonUtility_ToJson_m28CC6843B9D3723D88AD13EA3829B71FDE7826BA(L_15, NULL);
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_17;
		L_17 = JsonUtility_FromJson_TisPlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30_m5FA532D489BA59FC0DD187FA87D56E71F1F63B04(L_16, JsonUtility_FromJson_TisPlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30_m5FA532D489BA59FC0DD187FA87D56E71F1F63B04_RuntimeMethod_var);
		V_1 = L_17;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_18 = V_1;
		int32_t L_19 = ___0_chapterId;
		NullCheck(L_18);
		L_18->___selectedChapter = L_19;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_20 = V_1;
		int32_t L_21 = ___1_levelId;
		NullCheck(L_20);
		L_20->___selectedLevel = L_21;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_22 = V_1;
		bool L_23;
		L_23 = GameState_TryCommitBalance_mD9DA4B9AE5D934AD30A0F070F9C9AE01F165CCD7(__this, L_22, NULL);
		return L_23;
	}
}
