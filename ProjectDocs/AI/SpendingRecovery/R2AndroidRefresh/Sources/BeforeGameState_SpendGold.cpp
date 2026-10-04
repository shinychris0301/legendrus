IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool GameState_SpendGold_m3D5CFF3F7B20A404576DB8E2F6A1AE6795DCBE64 (GameState_t5E590D2FB317152A4011A3CA69E3F311E0A7393A* __this, int32_t ___0_amount, const RuntimeMethod* method) 
{
	{
		int32_t L_0 = ___0_amount;
		if ((((int32_t)L_0) < ((int32_t)0)))
		{
			goto IL_0012;
		}
	}
	{
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_1 = __this->___Save;
		NullCheck(L_1);
		int32_t L_2 = L_1->___gold;
		int32_t L_3 = ___0_amount;
		if ((((int32_t)L_2) >= ((int32_t)L_3)))
		{
			goto IL_0014;
		}
	}

IL_0012:
	{
		return (bool)0;
	}

IL_0014:
	{
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_4 = __this->___Save;
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_5 = L_4;
		NullCheck(L_5);
		int32_t L_6 = L_5->___gold;
		int32_t L_7 = ___0_amount;
		NullCheck(L_5);
		L_5->___gold = ((int32_t)il2cpp_codegen_subtract(L_6, L_7));
		GameState_Persist_m0261476FB722D9A1BAB0BB601B749EFC7E307C8D(__this, NULL);
		return (bool)1;
	}
}
