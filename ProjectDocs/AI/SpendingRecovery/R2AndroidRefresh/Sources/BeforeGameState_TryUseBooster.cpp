IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool GameState_TryUseBooster_m19110E094D19701126B89AC8583412890372C299 (GameState_t5E590D2FB317152A4011A3CA69E3F311E0A7393A* __this, int32_t ___0_type, const RuntimeMethod* method) 
{
	{
		int32_t L_0 = ___0_type;
		switch (L_0)
		{
			case 0:
			{
				goto IL_001b;
			}
			case 1:
			{
				goto IL_0040;
			}
			case 2:
			{
				goto IL_0065;
			}
			case 3:
			{
				goto IL_008a;
			}
		}
	}
	{
		goto IL_00af;
	}

IL_001b:
	{
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_1 = __this->___Save;
		NullCheck(L_1);
		int32_t L_2 = L_1->___boosterMace;
		if ((((int32_t)L_2) > ((int32_t)0)))
		{
			goto IL_002b;
		}
	}
	{
		return (bool)0;
	}

IL_002b:
	{
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_3 = __this->___Save;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_4 = L_3;
		NullCheck(L_4);
		int32_t L_5 = L_4->___boosterMace;
		NullCheck(L_4);
		L_4->___boosterMace = ((int32_t)il2cpp_codegen_subtract(L_5, 1));
		goto IL_00b1;
	}

IL_0040:
	{
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_6 = __this->___Save;
		NullCheck(L_6);
		int32_t L_7 = L_6->___boosterFirebird;
		if ((((int32_t)L_7) > ((int32_t)0)))
		{
			goto IL_0050;
		}
	}
	{
		return (bool)0;
	}

IL_0050:
	{
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_8 = __this->___Save;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_9 = L_8;
		NullCheck(L_9);
		int32_t L_10 = L_9->___boosterFirebird;
		NullCheck(L_9);
		L_9->___boosterFirebird = ((int32_t)il2cpp_codegen_subtract(L_10, 1));
		goto IL_00b1;
	}

IL_0065:
	{
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_11 = __this->___Save;
		NullCheck(L_11);
		int32_t L_12 = L_11->___boosterLivingLight;
		if ((((int32_t)L_12) > ((int32_t)0)))
		{
			goto IL_0075;
		}
	}
	{
		return (bool)0;
	}

IL_0075:
	{
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_13 = __this->___Save;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_14 = L_13;
		NullCheck(L_14);
		int32_t L_15 = L_14->___boosterLivingLight;
		NullCheck(L_14);
		L_14->___boosterLivingLight = ((int32_t)il2cpp_codegen_subtract(L_15, 1));
		goto IL_00b1;
	}

IL_008a:
	{
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_16 = __this->___Save;
		NullCheck(L_16);
		int32_t L_17 = L_16->___boosterSeaPearl;
		if ((((int32_t)L_17) > ((int32_t)0)))
		{
			goto IL_009a;
		}
	}
	{
		return (bool)0;
	}

IL_009a:
	{
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_18 = __this->___Save;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_19 = L_18;
		NullCheck(L_19);
		int32_t L_20 = L_19->___boosterSeaPearl;
		NullCheck(L_19);
		L_19->___boosterSeaPearl = ((int32_t)il2cpp_codegen_subtract(L_20, 1));
		goto IL_00b1;
	}

IL_00af:
	{
		return (bool)0;
	}

IL_00b1:
	{
		GameState_Persist_m0261476FB722D9A1BAB0BB601B749EFC7E307C8D(__this, NULL);
		return (bool)1;
	}
}
