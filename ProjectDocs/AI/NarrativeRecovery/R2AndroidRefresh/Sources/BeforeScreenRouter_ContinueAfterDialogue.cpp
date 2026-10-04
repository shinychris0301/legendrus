IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void ScreenRouter_ContinueAfterDialogue_mF1C4512958024BF0A6D98A90E5A6BD89BE09322C (ScreenRouter_t9EC367B45B1296F4CF0CBDEA57AEF86CC5340C00* __this, const RuntimeMethod* method) 
{
	int32_t V_0 = 0;
	{
		int32_t L_0;
		L_0 = StoryService_AdvanceDialogue_m607394AACFC68F30AD59B70975E3C764C1CCAAB0(NULL);
		V_0 = L_0;
		int32_t L_1 = V_0;
		switch (((int32_t)il2cpp_codegen_subtract((int32_t)L_1, 1)))
		{
			case 0:
			{
				goto IL_0020;
			}
			case 1:
			{
				goto IL_0028;
			}
			case 2:
			{
				goto IL_0039;
			}
			case 3:
			{
				goto IL_0030;
			}
		}
	}
	{
		goto IL_0041;
	}

IL_0020:
	{
		ScreenRouter_Show_mB289A1408F5F71A5043EFE930648595D5DCE3734(__this, 6, NULL);
		return;
	}

IL_0028:
	{
		ScreenRouter_Show_mB289A1408F5F71A5043EFE930648595D5DCE3734(__this, 4, NULL);
		return;
	}

IL_0030:
	{
		ScreenRouter_Show_mB289A1408F5F71A5043EFE930648595D5DCE3734(__this, ((int32_t)21), NULL);
		return;
	}

IL_0039:
	{
		ScreenRouter_Show_mB289A1408F5F71A5043EFE930648595D5DCE3734(__this, 3, NULL);
		return;
	}

IL_0041:
	{
		ScreenRouter_Show_mB289A1408F5F71A5043EFE930648595D5DCE3734(__this, 3, NULL);
		return;
	}
}
