IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool GameState_TryRecoverPendingBoosterCommand_mD849EAE55391848A18D9B3D447FD83FA2B4B6823 (GameState_t5E590D2FB317152A4011A3CA69E3F311E0A7393A* __this, const RuntimeMethod* method) 
{
	{
		bool L_0;
		L_0 = GameState_get_HasPendingBoosterCommand_m3E1BA9F545A38895AD411E75F5DE3C9F67C76C15(__this, NULL);
		if (!L_0)
		{
			goto IL_0020;
		}
	}
	{
		PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_1 = __this->___Save;
		NullCheck(L_1);
		BoosterCommandReceipt_tC1406EAC660F3A183904BE357F158495A694DE6C* L_2 = L_1->___boosterCommand;
		NullCheck(L_2);
		String_t* L_3 = L_2->___commandId;
		bool L_4;
		L_4 = GameState_TrySettleBoosterCommand_mAB9DD0D7B65D3E9D40665FCA710B1E678166C27E(__this, L_3, (bool)0, NULL);
		return L_4;
	}

IL_0020:
	{
		return (bool)1;
	}
}
