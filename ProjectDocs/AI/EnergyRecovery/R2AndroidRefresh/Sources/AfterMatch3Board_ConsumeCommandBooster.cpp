IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool Match3Board_ConsumeCommandBooster_m1F50D9D6EA0481DA928DE77C649C4DE6F3B739ED (Match3Board_tE4CAA58AF06191DC578D60DE5FA7659B8C8ACD69* __this, int32_t ___0_type, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ICampaignBoardInventory_tE3818DEC91979E54420832962522A249095E28AF_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	String_t* V_0 = NULL;
	int32_t G_B5_0 = 0;
	int32_t G_B9_0 = 0;
	int32_t G_B6_0 = 0;
	RuntimeCommandTransaction_tF69C17DAF81E9F0EAAEEF603E293B533B97AC945* G_B8_0 = NULL;
	int32_t G_B8_1 = 0;
	RuntimeCommandTransaction_tF69C17DAF81E9F0EAAEEF603E293B533B97AC945* G_B7_0 = NULL;
	int32_t G_B7_1 = 0;
	{
		V_0 = (String_t*)NULL;
		bool L_0 = __this->___campaignManaged;
		if (L_0)
		{
			goto IL_0029;
		}
	}
	{
		GameState_t5E590D2FB317152A4011A3CA69E3F311E0A7393A* L_1;
		L_1 = GameState_get_Instance_m3202061BC5E0A39B53697C35D0BEE15692173D74_inline(NULL);
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_2;
		L_2 = Object_op_Inequality_mD0BE578448EAA61948F25C32F8DD55AB1F778602(L_1, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (!L_2)
		{
			goto IL_0026;
		}
	}
	{
		GameState_t5E590D2FB317152A4011A3CA69E3F311E0A7393A* L_3;
		L_3 = GameState_get_Instance_m3202061BC5E0A39B53697C35D0BEE15692173D74_inline(NULL);
		int32_t L_4 = ___0_type;
		NullCheck(L_3);
		bool L_5;
		L_5 = GameState_TryBeginBoosterCommand_mBB45448CFDD6FDB85CA1743E013935412BE11F83(L_3, L_4, (&V_0), NULL);
		G_B5_0 = ((int32_t)(L_5));
		goto IL_0035;
	}

IL_0026:
	{
		G_B5_0 = 0;
		goto IL_0035;
	}

IL_0029:
	{
		RuntimeObject* L_6 = __this->___campaignInventory;
		int32_t L_7 = ___0_type;
		NullCheck(L_6);
		bool L_8;
		L_8 = InterfaceFuncInvoker1< bool, int32_t >::Invoke(1, ICampaignBoardInventory_tE3818DEC91979E54420832962522A249095E28AF_il2cpp_TypeInfo_var, L_6, L_7);
		G_B5_0 = ((int32_t)(L_8));
	}

IL_0035:
	{
		int32_t L_9 = G_B5_0;
		if (!L_9)
		{
			G_B9_0 = L_9;
			goto IL_004a;
		}
		G_B6_0 = L_9;
	}
	{
		RuntimeCommandTransaction_tF69C17DAF81E9F0EAAEEF603E293B533B97AC945* L_10 = __this->___activeCommand;
		RuntimeCommandTransaction_tF69C17DAF81E9F0EAAEEF603E293B533B97AC945* L_11 = L_10;
		if (L_11)
		{
			G_B8_0 = L_11;
			G_B8_1 = G_B6_0;
			goto IL_0043;
		}
		G_B7_0 = L_11;
		G_B7_1 = G_B6_0;
	}
	{
		return (bool)G_B7_1;
	}

IL_0043:
	{
		int32_t L_12 = ___0_type;
		String_t* L_13 = V_0;
		NullCheck(G_B8_0);
		RuntimeCommandTransaction_MarkBoosterConsumed_m6F3BCDD6C0E95B1EFA290D5A51255D0D38DAB64A(G_B8_0, L_12, L_13, NULL);
		G_B9_0 = G_B8_1;
	}

IL_004a:
	{
		return (bool)G_B9_0;
	}
}
