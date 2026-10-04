IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool GameState_TryDoubleLastLevelGoldReward_m1F4365DDE5E82F099DB4EB8A97D7AC871E0D23F3 (GameState_t5E590D2FB317152A4011A3CA69E3F311E0A7393A* __this, const RuntimeMethod* method) 
{
	{
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_0 = __this->___Save;
		NullCheck(L_0);
		bool L_1 = L_0->___lastRewardEligibleForDouble;
		if (!L_1)
		{
			goto IL_0028;
		}
	}
	{
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_2 = __this->___Save;
		NullCheck(L_2);
		bool L_3 = L_2->___lastRewardDoubled;
		if (L_3)
		{
			goto IL_0028;
		}
	}
	{
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_4 = __this->___Save;
		NullCheck(L_4);
		int32_t L_5 = L_4->___lastRewardGold;
		if ((((int32_t)L_5) > ((int32_t)0)))
		{
			goto IL_002a;
		}
	}

IL_0028:
	{
		return (bool)0;
	}

IL_002a:
	{
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_6 = __this->___Save;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_7 = L_6;
		NullCheck(L_7);
		int32_t L_8 = L_7->___gold;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_9 = __this->___Save;
		NullCheck(L_9);
		int32_t L_10 = L_9->___lastRewardGold;
		NullCheck(L_7);
		L_7->___gold = ((int32_t)il2cpp_codegen_add(L_8, L_10));
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_11 = __this->___Save;
		NullCheck(L_11);
		L_11->___lastRewardDoubled = (bool)1;
		GameState_Persist_m0261476FB722D9A1BAB0BB601B749EFC7E307C8D(__this, NULL);
		return (bool)1;
	}
}
