IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void GameState_RecordLevelFailure_m03EA2F1F11FB60F1D69DFBFE12EB680248756450 (GameState_t5E590D2FB317152A4011A3CA69E3F311E0A7393A* __this, LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* ___0_levelData, int32_t ___1_score, RuntimeObject* ___2_goalProgress, const RuntimeMethod* method) 
{
	{
		LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* L_0 = ___0_levelData;
		int32_t L_1 = ___1_score;
		RuntimeObject* L_2 = ___2_goalProgress;
		bool L_3;
		L_3 = GameState_TryRecordLevelFailure_m8C0D994176FC03D3AD8E5A9B60CD8B1B0E8A3DC6(__this, L_0, L_1, L_2, NULL);
		return;
	}
}
