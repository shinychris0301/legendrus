IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool GameState_SelectLevel_m6055ECAE894AC442764BC224B6B3FB0ABA949570 (GameState_t5E590D2FB317152A4011A3CA69E3F311E0A7393A* __this, int32_t ___0_chapterId, int32_t ___1_levelId, const RuntimeMethod* method) 
{
	RetainedRoundData_t3EA1BA932DFF2408098F1FB87EC5A5A24130D7A1* V_0 = NULL;
	{
		bool L_0;
		L_0 = GameState_ContinuationBlocksNewAttempt_m908136F30BB6F308311486AF5F487FAD53014484(__this, NULL);
		if (!L_0)
		{
			goto IL_000a;
		}
	}
	{
		return (bool)0;
	}

IL_000a:
	{
		bool L_1;
		L_1 = GameState_TryGetRetainedRound_m2295FF12D42D33C9E7E1FF1D8B33B8E3859793A5(__this, (&V_0), NULL);
		if (!L_1)
		{
			goto IL_0028;
		}
	}
	{
		RetainedRoundData_t3EA1BA932DFF2408098F1FB87EC5A5A24130D7A1* L_2 = V_0;
		NullCheck(L_2);
		int32_t L_3 = L_2->___chapter;
		int32_t L_4 = ___0_chapterId;
		if ((!(((uint32_t)L_3) == ((uint32_t)L_4))))
		{
			goto IL_0026;
		}
	}
	{
		RetainedRoundData_t3EA1BA932DFF2408098F1FB87EC5A5A24130D7A1* L_5 = V_0;
		NullCheck(L_5);
		int32_t L_6 = L_5->___level;
		int32_t L_7 = ___1_levelId;
		if ((((int32_t)L_6) == ((int32_t)L_7)))
		{
			goto IL_0028;
		}
	}

IL_0026:
	{
		return (bool)0;
	}

IL_0028:
	{
		int32_t L_8 = ___0_chapterId;
		int32_t L_9 = ___1_levelId;
		bool L_10;
		L_10 = GameState_IsLevelUnlocked_mA15470234C68329950931198C078B05B900F0689(__this, L_8, L_9, NULL);
		if (L_10)
		{
			goto IL_0034;
		}
	}
	{
		return (bool)0;
	}

IL_0034:
	{
		int32_t L_11 = ___0_chapterId;
		int32_t L_12 = ___1_levelId;
		LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* L_13;
		L_13 = ChapterLevelCatalog_Find_mB0A3275A64F35FB511F90A6049719C9B1971FDC6(L_11, L_12, NULL);
		if (L_13)
		{
			goto IL_003f;
		}
	}
	{
		return (bool)0;
	}

IL_003f:
	{
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_14 = __this->___Save;
		int32_t L_15 = ___0_chapterId;
		NullCheck(L_14);
		L_14->___selectedChapter = L_15;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_16 = __this->___Save;
		int32_t L_17 = ___1_levelId;
		NullCheck(L_16);
		L_16->___selectedLevel = L_17;
		GameState_Persist_m0261476FB722D9A1BAB0BB601B749EFC7E307C8D(__this, NULL);
		return (bool)1;
	}
}
