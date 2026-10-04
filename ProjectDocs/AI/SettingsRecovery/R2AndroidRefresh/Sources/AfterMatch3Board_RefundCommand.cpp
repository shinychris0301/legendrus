IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool Match3Board_RefundCommand_mDC69071AE976F66ECC89D7949220F397689B1039 (Match3Board_tE4CAA58AF06191DC578D60DE5FA7659B8C8ACD69* __this, RuntimeCommandTransaction_tF69C17DAF81E9F0EAAEEF603E293B533B97AC945* ___0_command, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ICampaignBoardInventory_tE3818DEC91979E54420832962522A249095E28AF_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Nullable_1_get_HasValue_m4651E27BB4CB970BCB109D66BDD7B0B131AFFA92_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	Nullable_1_t9CE92CE712BC5CC12E66AF5FBC540402BF567C73 V_0;
	memset((&V_0), 0, sizeof(V_0));
	{
		RuntimeCommandTransaction_tF69C17DAF81E9F0EAAEEF603E293B533B97AC945* L_0 = ___0_command;
		NullCheck(L_0);
		Nullable_1_t9CE92CE712BC5CC12E66AF5FBC540402BF567C73 L_1;
		L_1 = RuntimeCommandTransaction_get_ConsumedBooster_mF36C3846051C768498BE76E7C59A49C2AABC0B2A_inline(L_0, NULL);
		V_0 = L_1;
		bool L_2;
		L_2 = Nullable_1_get_HasValue_m4651E27BB4CB970BCB109D66BDD7B0B131AFFA92_inline((&V_0), Nullable_1_get_HasValue_m4651E27BB4CB970BCB109D66BDD7B0B131AFFA92_RuntimeMethod_var);
		if (L_2)
		{
			goto IL_0012;
		}
	}
	{
		return (bool)1;
	}

IL_0012:
	{
		bool L_3 = __this->___campaignManaged;
		if (L_3)
		{
			goto IL_003b;
		}
	}
	{
		GameState_t5E590D2FB317152A4011A3CA69E3F311E0A7393A* L_4;
		L_4 = GameState_get_Instance_m3202061BC5E0A39B53697C35D0BEE15692173D74_inline(NULL);
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_5;
		L_5 = Object_op_Inequality_mD0BE578448EAA61948F25C32F8DD55AB1F778602(L_4, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (!L_5)
		{
			goto IL_0039;
		}
	}
	{
		GameState_t5E590D2FB317152A4011A3CA69E3F311E0A7393A* L_6;
		L_6 = GameState_get_Instance_m3202061BC5E0A39B53697C35D0BEE15692173D74_inline(NULL);
		RuntimeCommandTransaction_tF69C17DAF81E9F0EAAEEF603E293B533B97AC945* L_7 = ___0_command;
		NullCheck(L_7);
		String_t* L_8;
		L_8 = RuntimeCommandTransaction_get_BoosterCommandId_m070A6F7AD17790037375F4AF817AE818259A5577_inline(L_7, NULL);
		NullCheck(L_6);
		bool L_9;
		L_9 = GameState_TrySettleBoosterCommand_mAB9DD0D7B65D3E9D40665FCA710B1E678166C27E(L_6, L_8, (bool)0, NULL);
		return L_9;
	}

IL_0039:
	{
		return (bool)0;
	}

IL_003b:
	{
		RuntimeObject* L_10 = __this->___campaignInventory;
		if (!L_10)
		{
			goto IL_004f;
		}
	}
	{
		RuntimeObject* L_11 = __this->___campaignInventory;
		NullCheck(L_11);
		bool L_12;
		L_12 = InterfaceFuncInvoker0< bool >::Invoke(3, ICampaignBoardInventory_tE3818DEC91979E54420832962522A249095E28AF_il2cpp_TypeInfo_var, L_11);
		return L_12;
	}

IL_004f:
	{
		return (bool)0;
	}
}
