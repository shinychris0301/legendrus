IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool Match3Board_TryResume_m1576AA7FCE103DF071BE8905F8F9348E214B8D30 (Match3Board_tE4CAA58AF06191DC578D60DE5FA7659B8C8ACD69* __this, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	RuntimeCommandTransaction_tF69C17DAF81E9F0EAAEEF603E293B533B97AC945* V_0 = NULL;
	bool V_1 = false;
	int32_t G_B7_0 = 0;
	GameplayStateMachine_t01C75AC2E3E92D6A53E4B80363DB5C26D85DA0CA* G_B9_0 = NULL;
	GameplayStateMachine_t01C75AC2E3E92D6A53E4B80363DB5C26D85DA0CA* G_B8_0 = NULL;
	int32_t G_B10_0 = 0;
	GameplayStateMachine_t01C75AC2E3E92D6A53E4B80363DB5C26D85DA0CA* G_B10_1 = NULL;
	{
		bool L_0 = __this->___refundPending;
		if (!L_0)
		{
			goto IL_0097;
		}
	}
	{
		RuntimeCommandTransaction_tF69C17DAF81E9F0EAAEEF603E293B533B97AC945* L_1 = __this->___activeCommand;
		if (!L_1)
		{
			goto IL_0021;
		}
	}
	{
		RuntimeCommandTransaction_tF69C17DAF81E9F0EAAEEF603E293B533B97AC945* L_2 = __this->___activeCommand;
		bool L_3;
		L_3 = Match3Board_RefundCommand_mDC69071AE976F66ECC89D7949220F397689B1039(__this, L_2, NULL);
		if (L_3)
		{
			goto IL_0023;
		}
	}

IL_0021:
	{
		return (bool)0;
	}

IL_0023:
	{
		RuntimeCommandTransaction_tF69C17DAF81E9F0EAAEEF603E293B533B97AC945* L_4 = __this->___activeCommand;
		V_0 = L_4;
		__this->___refundPending = (bool)0;
		__this->___activeCommand = (RuntimeCommandTransaction_tF69C17DAF81E9F0EAAEEF603E293B533B97AC945*)NULL;
		Il2CppCodeGenWriteBarrier((void**)(&__this->___activeCommand), (void*)(RuntimeCommandTransaction_tF69C17DAF81E9F0EAAEEF603E293B533B97AC945*)NULL);
		bool L_5;
		L_5 = Match3Board_get_HasInventoryRecoveryError_m2F77ADD473EBFDA4F38F30D9CAB956220AC2A846(__this, NULL);
		if (L_5)
		{
			goto IL_004b;
		}
	}
	{
		bool L_6;
		L_6 = Match3Board_get_CampaignInventoryBlocked_mBD9371C16DFC74E4E255168688003BBFE0D0B563(__this, NULL);
		G_B7_0 = ((((int32_t)L_6) == ((int32_t)0))? 1 : 0);
		goto IL_004c;
	}

IL_004b:
	{
		G_B7_0 = 0;
	}

IL_004c:
	{
		V_1 = (bool)G_B7_0;
		GameplayStateMachine_t01C75AC2E3E92D6A53E4B80363DB5C26D85DA0CA* L_7 = __this->___interaction;
		bool L_8 = V_1;
		if (L_8)
		{
			G_B9_0 = L_7;
			goto IL_005a;
		}
		G_B8_0 = L_7;
	}
	{
		G_B10_0 = ((int32_t)10);
		G_B10_1 = G_B8_0;
		goto IL_005b;
	}

IL_005a:
	{
		G_B10_0 = 0;
		G_B10_1 = G_B9_0;
	}

IL_005b:
	{
		NullCheck(G_B10_1);
		GameplayStateMachine_Force_m611EBC14C71224E77DCEC80729A25636364BD1AD(G_B10_1, G_B10_0, NULL);
		Match3Board_PublishState_m888BDD9A14D75FA1915246672C1F7B94F83580FF(__this, NULL);
		Vector2Int_t69B2886EBAB732D9B880565E18E7568F3DE0CE6A L_9;
		L_9 = Vector2Int_get_zero_mF92C338E9CB9434105090E675E04D20A29649553_inline(NULL);
		Vector2Int_t69B2886EBAB732D9B880565E18E7568F3DE0CE6A L_10;
		L_10 = Vector2Int_get_zero_mF92C338E9CB9434105090E675E04D20A29649553_inline(NULL);
		String_t* L_11 = __this->___lastResolutionPlanHash;
		RuntimeCommandTransaction_tF69C17DAF81E9F0EAAEEF603E293B533B97AC945* L_12 = V_0;
		NullCheck(L_12);
		String_t* L_13;
		L_13 = RuntimeCommandTransaction_get_Label_mAA9C72AEE5F924A8C5DDF7378CEAF55E1EAE8DB4_inline(L_12, NULL);
		RuntimeCommandTransaction_tF69C17DAF81E9F0EAAEEF603E293B533B97AC945* L_14 = V_0;
		NullCheck(L_14);
		Nullable_1_t9CE92CE712BC5CC12E66AF5FBC540402BF567C73 L_15;
		L_15 = RuntimeCommandTransaction_get_ConsumedBooster_mF36C3846051C768498BE76E7C59A49C2AABC0B2A_inline(L_14, NULL);
		int32_t L_16;
		L_16 = Match3Board_ToGameplayBooster_m7ECF703BBE8B09C51471F096EF460C8F9745E659(L_13, L_15, NULL);
		Match3Board_PublishMotion_mCD47F32AAFA9F0EB01223DEF130ABB1FA2387D16(__this, ((int32_t)14), L_9, L_10, 0, 0, 0, L_11, 0, L_16, (RuntimeObject*)NULL, 0, NULL);
		bool L_17 = V_1;
		return L_17;
	}

IL_0097:
	{
		bool L_18 = __this->___campaignManaged;
		if (L_18)
		{
			goto IL_00ce;
		}
	}
	{
		RuntimeCommandTransaction_tF69C17DAF81E9F0EAAEEF603E293B533B97AC945* L_19 = __this->___activeCommand;
		if (L_19)
		{
			goto IL_00ce;
		}
	}
	{
		GameState_t5E590D2FB317152A4011A3CA69E3F311E0A7393A* L_20;
		L_20 = GameState_get_Instance_m3202061BC5E0A39B53697C35D0BEE15692173D74_inline(NULL);
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_21;
		L_21 = Object_op_Inequality_mD0BE578448EAA61948F25C32F8DD55AB1F778602(L_20, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (!L_21)
		{
			goto IL_00ce;
		}
	}
	{
		GameState_t5E590D2FB317152A4011A3CA69E3F311E0A7393A* L_22;
		L_22 = GameState_get_Instance_m3202061BC5E0A39B53697C35D0BEE15692173D74_inline(NULL);
		NullCheck(L_22);
		bool L_23;
		L_23 = GameState_get_HasPendingBoosterCommand_m3E1BA9F545A38895AD411E75F5DE3C9F67C76C15(L_22, NULL);
		if (!L_23)
		{
			goto IL_00ce;
		}
	}
	{
		GameState_t5E590D2FB317152A4011A3CA69E3F311E0A7393A* L_24;
		L_24 = GameState_get_Instance_m3202061BC5E0A39B53697C35D0BEE15692173D74_inline(NULL);
		NullCheck(L_24);
		bool L_25;
		L_25 = GameState_TryRecoverPendingBoosterCommand_mD849EAE55391848A18D9B3D447FD83FA2B4B6823(L_24, NULL);
		if (L_25)
		{
			goto IL_00ce;
		}
	}
	{
		return (bool)0;
	}

IL_00ce:
	{
		bool L_26;
		L_26 = Match3Board_get_CampaignInventoryBlocked_mBD9371C16DFC74E4E255168688003BBFE0D0B563(__this, NULL);
		if (L_26)
		{
			goto IL_00de;
		}
	}
	{
		bool L_27;
		L_27 = Match3Board_get_HasInventoryRecoveryError_m2F77ADD473EBFDA4F38F30D9CAB956220AC2A846(__this, NULL);
		if (!L_27)
		{
			goto IL_00e0;
		}
	}

IL_00de:
	{
		return (bool)0;
	}

IL_00e0:
	{
		GameplayStateMachine_t01C75AC2E3E92D6A53E4B80363DB5C26D85DA0CA* L_28 = __this->___interaction;
		NullCheck(L_28);
		bool L_29;
		L_29 = GameplayStateMachine_TryResume_mB1253C9B58BBB09AAC5F8AEA614FF3B16D47D325(L_28, NULL);
		return L_29;
	}
}
