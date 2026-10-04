IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool GameState_TryPersist_mF3A65DB34A79CC303E1F44137C6BDF4C69B1E13C (GameState_t5E590D2FB317152A4011A3CA69E3F311E0A7393A* __this, const RuntimeMethod* method) 
{
	bool V_0 = false;
	{
		bool L_0 = __this->___canPersist;
		if (!L_0)
		{
			goto IL_0010;
		}
	}
	{
		bool L_1 = __this->___persistenceInProgress;
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
		__this->___persistenceInProgress = (bool)1;
	}
	{
		auto __finallyBlock = il2cpp::utils::Finally([&]
		{

FINALLY_0028:
			{
				__this->___persistenceInProgress = (bool)0;
				return;
			}
		});
		try
		{
			PlayerSaveData_t44D743359749F73BC6386FE5FCED6A67D1E51F30* L_2 = __this->___Save;
			bool L_3;
			L_3 = GameState_TryCommitBalance_mD9DA4B9AE5D934AD30A0F070F9C9AE01F165CCD7(__this, L_2, NULL);
			V_0 = L_3;
			goto IL_0030;
		}
		catch(Il2CppNativeThreadAbortException&)
		{
			__finallyBlock.SetNativeThreadAbortOccurred();
		}
		catch(Il2CppExceptionWrapper& e)
		{
			__finallyBlock.StoreException(e.ex);
		}
	}

IL_0030:
	{
		bool L_4 = V_0;
		return L_4;
	}
}
