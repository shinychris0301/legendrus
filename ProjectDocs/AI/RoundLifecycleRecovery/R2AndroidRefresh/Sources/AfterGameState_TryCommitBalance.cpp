IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool GameState_TryCommitBalance_mD9DA4B9AE5D934AD30A0F070F9C9AE01F165CCD7 (GameState_t5E590D2FB317152A4011A3CA69E3F311E0A7393A* __this, PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* ___0_candidate, const RuntimeMethod* method) 
{
	bool V_0 = false;
	PlayerSaveWriteResult_tD3521C4F0AD4F171529F5475A2BD7B28C0CB1D9C* V_1 = NULL;
	Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* G_B7_0 = NULL;
	Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* G_B6_0 = NULL;
	{
		bool L_0 = __this->___canPersist;
		if (L_0)
		{
			goto IL_000a;
		}
	}
	{
		return (bool)0;
	}

IL_000a:
	{
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_1 = ___0_candidate;
		bool L_2;
		L_2 = SaveService_TrySave_m11A043A57FCBE0607BDAAA6C34B4E969064615B0(L_1, (&V_1), NULL);
		V_0 = L_2;
		PlayerSaveWriteResult_tD3521C4F0AD4F171529F5475A2BD7B28C0CB1D9C* L_3 = V_1;
		GameState_set_LastWriteResult_mE42274830424D045544D465431B5C582BC50A567_inline(__this, L_3, NULL);
		bool L_4 = V_0;
		if (L_4)
		{
			goto IL_002e;
		}
	}
	{
		PlayerSaveWriteResult_tD3521C4F0AD4F171529F5475A2BD7B28C0CB1D9C* L_5 = V_1;
		NullCheck(L_5);
		bool L_6;
		L_6 = PlayerSaveWriteResult_get_PrimaryDurablyCommitted_m0D28E9CF94BC730C6E592F0CC6D4638D8961D000_inline(L_5, NULL);
		if (L_6)
		{
			goto IL_002e;
		}
	}
	{
		__this->___canPersist = (bool)0;
		return (bool)0;
	}

IL_002e:
	{
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_7 = ___0_candidate;
		__this->___Save = L_7;
		Il2CppCodeGenWriteBarrier((void**)(&__this->___Save), (void*)L_7);
		bool L_8 = V_0;
		__this->___canPersist = L_8;
		Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* L_9 = __this->___Changed;
		Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* L_10 = L_9;
		if (L_10)
		{
			G_B7_0 = L_10;
			goto IL_0048;
		}
		G_B6_0 = L_10;
	}
	{
		goto IL_004d;
	}

IL_0048:
	{
		NullCheck(G_B7_0);
		Action_Invoke_m7126A54DACA72B845424072887B5F3A51FC3808E_inline(G_B7_0, NULL);
	}

IL_004d:
	{
		return (bool)1;
	}
}
