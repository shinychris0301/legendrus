IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void ScreenRouter_ContinueAfterChapterComplete_mEDFB40318905AAE2A02E3B76EC15BBA430C72F65 (ScreenRouter_t9EC367B45B1296F4CF0CBDEA57AEF86CC5340C00* __this, int32_t ___0_destination, const RuntimeMethod* method) 
{
	{
		bool L_0;
		L_0 = StoryService_ShouldShowChapterComplete_m1045C4F1A2B820F636E5F14FE2CC60911AC8B40D(NULL);
		if (!L_0)
		{
			goto IL_0017;
		}
	}
	{
		bool L_1;
		L_1 = StoryService_AcknowledgeChapterComplete_mD184BE7FAFDC8EDF1533BE3551594919939E951A(NULL);
		if (L_1)
		{
			goto IL_0017;
		}
	}
	{
		bool L_2;
		L_2 = ScreenRouter_ShowErrorDialog_m54E0B54656DDD07A0CC9A5294216CC8A992C4823(__this, (Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07*)NULL, NULL);
		return;
	}

IL_0017:
	{
		int32_t L_3 = ___0_destination;
		ScreenRouter_Show_mB289A1408F5F71A5043EFE930648595D5DCE3734(__this, L_3, NULL);
		return;
	}
}
