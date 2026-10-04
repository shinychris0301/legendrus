IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void ScreenRouter_ContinueAfterChapterComplete_mEDFB40318905AAE2A02E3B76EC15BBA430C72F65 (ScreenRouter_t9EC367B45B1296F4CF0CBDEA57AEF86CC5340C00* __this, int32_t ___0_destination, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&_stringLiteral5C42DDF9A7416A3D00E8561C4113483C209B7FC3);
		s_Il2CppMethodInitialized = true;
	}
	{
		bool L_0;
		L_0 = StoryService_ShouldShowChapterComplete_m1045C4F1A2B820F636E5F14FE2CC60911AC8B40D(NULL);
		if (!L_0)
		{
			goto IL_001a;
		}
	}
	{
		bool L_1;
		L_1 = StoryService_AcknowledgeChapterComplete_mD184BE7FAFDC8EDF1533BE3551594919939E951A(NULL);
		if (L_1)
		{
			goto IL_001a;
		}
	}
	{
		ScreenRouter_Toast_mE83B5119D00BFE9909CB7C0BAACDA839EFA3FFF2(__this, _stringLiteral5C42DDF9A7416A3D00E8561C4113483C209B7FC3, NULL);
		return;
	}

IL_001a:
	{
		int32_t L_2 = ___0_destination;
		ScreenRouter_Show_mB289A1408F5F71A5043EFE930648595D5DCE3734(__this, L_2, NULL);
		return;
	}
}
