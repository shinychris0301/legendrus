IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool GameState_TryCommitRoundCandidate_mF567FE151D88B0B07E7F938F7534DF807FD76CE9 (GameState_t5E590D2FB317152A4011A3CA69E3F311E0A7393A* __this, PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* ___0_candidate, LevelFailureResult_tEAFB9DEC27BA6439583A47C07A1D65D0B79D12E0* ___1_failure, bool ___2_clearRetainedAttempt, const RuntimeMethod* method) 
{
	bool V_0 = false;
	PlayerSaveWriteResult_tD3521C4F0AD4F171529F5475A2BD7B28C0CB1D9C* V_1 = NULL;
	Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* G_B11_0 = NULL;
	Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* G_B10_0 = NULL;
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
		bool L_8 = ___2_clearRetainedAttempt;
		if (!L_8)
		{
			goto IL_003f;
		}
	}
	{
		__this->___unsavedRetainedRound = (RetainedRoundData_t3EA1BA932DFF2408098F1FB87EC5A5A24130D7A1*)NULL;
		Il2CppCodeGenWriteBarrier((void**)(&__this->___unsavedRetainedRound), (void*)(RetainedRoundData_t3EA1BA932DFF2408098F1FB87EC5A5A24130D7A1*)NULL);
	}

IL_003f:
	{
		LevelFailureResult_tEAFB9DEC27BA6439583A47C07A1D65D0B79D12E0* L_9 = ___1_failure;
		if (!L_9)
		{
			goto IL_0049;
		}
	}
	{
		LevelFailureResult_tEAFB9DEC27BA6439583A47C07A1D65D0B79D12E0* L_10 = ___1_failure;
		GameState_set_LastFailureResult_m93C05E4ECCB370EAD4C1691FEC10E4F59FCC4DCF_inline(__this, L_10, NULL);
	}

IL_0049:
	{
		bool L_11 = V_0;
		__this->___canPersist = L_11;
		Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* L_12 = __this->___Changed;
		Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* L_13 = L_12;
		if (L_13)
		{
			G_B11_0 = L_13;
			goto IL_005c;
		}
		G_B10_0 = L_13;
	}
	{
		goto IL_0061;
	}

IL_005c:
	{
		NullCheck(G_B11_0);
		Action_Invoke_m7126A54DACA72B845424072887B5F3A51FC3808E_inline(G_B11_0, NULL);
	}

IL_0061:
	{
		return (bool)1;
	}
}
