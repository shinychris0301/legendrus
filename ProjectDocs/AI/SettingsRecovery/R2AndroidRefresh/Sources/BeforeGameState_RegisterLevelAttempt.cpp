IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t GameState_RegisterLevelAttempt_m31F23CCF55EB0C2270E2E0376980F8F61C4F9504 (GameState_t5E590D2FB317152A4011A3CA69E3F311E0A7393A* __this, LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* ___0_levelData, const RuntimeMethod* method) 
{
	int32_t V_0 = 0;
	{
		LevelData_tCD69EAEAB70C2F2CD76C9F4727F8491C1C989CDC* L_0 = ___0_levelData;
		bool L_1;
		L_1 = GameState_TryRegisterLevelAttempt_mAF97A5BE4A922ECAE59CE0DCD3DCD6649AFFC57E(__this, L_0, (&V_0), NULL);
		if (L_1)
		{
			goto IL_000d;
		}
	}
	{
		return 0;
	}

IL_000d:
	{
		int32_t L_2 = V_0;
		return L_2;
	}
}
