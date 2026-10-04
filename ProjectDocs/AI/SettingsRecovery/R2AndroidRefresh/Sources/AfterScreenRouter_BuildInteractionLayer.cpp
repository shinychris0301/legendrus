IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void ScreenRouter_BuildInteractionLayer_m098546A1A74D7C435B6EAE0086CE1D53AD4D93EF (ScreenRouter_t9EC367B45B1296F4CF0CBDEA57AEF86CC5340C00* __this, int32_t ___0_id, RectTransform_t6C5DA5E41A89E0F488B001E45E58963480E543A5* ___1_parent, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Action_1_t3CB5D1A819C3ED3F99E9E39F890F18633253949A_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Action_1_t913CB0C3CDC5C59F6B8A1A7D957A63049F5A6BCA_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Component_GetComponentInChildren_TisProductionBoosterSelectionView_tC3AB07A664B7F97CBD28AB8693822704399FF79C_m2CAE450D384AC463D693AA8F0C4BFBDF64009253_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Component_GetComponentInChildren_TisProductionChapterCompleteView_t8F3C64B99DD107C039783DF1D0A39530DDAA9F70_mBAF44037DDF939EBA0D1038A72DB9B112B50FC73_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Component_GetComponentInChildren_TisProductionChapterMapView_t73FA61D194FCDDD0F89F8021A5BBFC50FD5D1075_m7A6AAD6BEAA16D72940FF80B66D58CCFB67BE62F_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Component_GetComponentInChildren_TisProductionCharactersView_t80FF719EBB00992075D821990BF966D5E95577DE_mF14E8F137A064E850586F44746439D4CEDF5912B_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Component_GetComponentInChildren_TisProductionDailyRewardView_tD3D86E84BE0D44D4F0F4963448D4CB68094FD0A9_mD08648BCC4B6467F143C3305D46818BE4EFD4F6F_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Component_GetComponentInChildren_TisProductionDialogueView_t9612D941765F8A0545DD0212DE1D76B0BDD0F8C5_m1D008ACA9283C198043BDFE9174D5580BD5136E0_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Component_GetComponentInChildren_TisProductionEventDetailView_t7360325479FBCA2559711C551444B3B712CF6CE5_mC8B301908CD50059D72FA6B292C6A4BD7A1E8E4B_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Component_GetComponentInChildren_TisProductionEventsView_t60F8250E60FB6585506BBB36AB2FA5C392D130FA_mF04A7124BA3D8D332303AA248881507B215636B3_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Component_GetComponentInChildren_TisProductionFailView_t5D82248F0E88B603813B2F82EE1CA191D740F251_mDE013F807A95E81C33979B237BF5F2FC2C4FBE40_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Component_GetComponentInChildren_TisProductionFriendsListView_t568A80A4F587DE5E0D9156810474BD1B241353D1_mA39DD78FA3CB7B4FC4475179DECC45F18647B69F_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Component_GetComponentInChildren_TisProductionHomeView_tB424DCD664CAE857FD5323EEB6B98B1CFB918694_mABA6BCAC08EB0EC7E26E912407EC13910805FE47_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Component_GetComponentInChildren_TisProductionIndividualRankingView_tF449BDA2B9876849864F7F78C4C57CE0BB9726B3_mB4338486B6C14D9D4616520DBB20E5A74E544E5C_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Component_GetComponentInChildren_TisProductionInventoryView_tBFCB541C14E4ED40B2CB6633CE17AC0EB9D14E8E_mCC3AAA9A5726E977612F8790700DB37795529949_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Component_GetComponentInChildren_TisProductionLanguageSelectView_tC9B9BF5C49A8106E900C4E16F3C587BAB476E212_mC482E7021328C34544943E03EDFAB17A75A453BF_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Component_GetComponentInChildren_TisProductionLoginView_t36729B7F5B697B3828DE80A1A0EB2509C7F66B81_mB364C4DEC9E8A2A8378CB63891CFE9A294BDE258_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Component_GetComponentInChildren_TisProductionMailView_t613C10918DAA8582972FFE0681462F4DAE73B966_m2D5BCFC55825FE84588DD471D9FEE1177C6C658C_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Component_GetComponentInChildren_TisProductionMaintenanceView_tE78AA4EB7FC8F41C25AF278B1341DAA6B8F091A1_mBC7AFB6C8BAA17700B77BCEF2981DD586B8F25D0_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Component_GetComponentInChildren_TisProductionOfflineView_t2A35DC0266DAE631DB63C259E5095E1F51E46451_mDE12F1909962FD311F5F0F62E78371673DCA0E59_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Component_GetComponentInChildren_TisProductionOutOfEnergyView_t9ED536371FCF21CC2E58121FFF0561349450226A_mC085D077806717D5DB805D8AA8F462257CECEEAB_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Component_GetComponentInChildren_TisProductionPreLevelView_t19543F1A5A95A83F99E01D12193810B0543C6BDD_m46F83BF669465C64237D11120DCB68E2117022FE_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Component_GetComponentInChildren_TisProductionProfileView_tFBAE39BD387DB9B67C01A51EB40718FD7461992D_m446C73FD0151B25AD1CBD36400F2A63645FE1667_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Component_GetComponentInChildren_TisProductionRewardReceivedView_t308036C90189F05146CA7191FC9674291133EC11_mDB789F0CDD8849D2B9F480B102EFB6ED6E316CCB_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Component_GetComponentInChildren_TisProductionShopView_tCAE5004D358A893BA810EE3CC9331FF6A5AAB245_m2BB6613012F1FC7BFDF2D183D920E0124FB8CA63_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Component_GetComponentInChildren_TisProductionSpecialOfferView_t26243C08B7058A2D21A48610B99150458CC7CDD7_m2EB6BF8D2D72A3803150E1A35B5FA08F735A4E17_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Component_GetComponentInChildren_TisProductionTeamChatView_t3876DDF380A2DA789CB223EB9E32A75578DFE07E_mF9F7EBF80FE2E7E514B194BCAFCF940A06FA65C2_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Component_GetComponentInChildren_TisProductionTeamHubView_t242CA702D2D2A557A5145C582FE6955173320992_m8CD52018D226794AAAEBCBE906B957945FFE86F1_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Component_GetComponentInChildren_TisProductionTeamRankingView_t6A2E723B48D2D1F086684E393EC71E7A19E82DD4_mE250DA820F30C1322456DBA230F6E0E1E4B2B5C3_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Component_GetComponentInChildren_TisProductionTutorialView_t17A2D559FD4E71F841E57E107C3B8FADDC3DA4CB_m47D948D0758AF603AAE420B79DA907127ED01965_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Component_GetComponentInChildren_TisProductionVictoryView_t09841A4498FAE3A2C54918934DC80DBD053E851A_mC5D774D98C17EC0007845B4B58C751C718D28185_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Component_GetComponentInChildren_TisProductionWorldMapView_t9EE066392822F11A7943261785563B6FFB262CF7_mF04510D979FA706C943DD04AAE13D47E17DE9A8E_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Component_GetComponentInChildren_TisSplashTitleView_t5C9B3C703C7BCA2970A475F6B1EF763DD452FB17_mFF739B05797AB82FC85EC4521506F415FA6D5169_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Debug_t8394C7EEAECA3689C2C9B9DE9C7166D73596276F_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Func_1_t2BE7F58348C9CC544A8973B3A9E55541DE43C457_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ProductionMapNotice_Show_mB6EAAA8554BBD1922FC4ABE702FBF6154F9344CF_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ScreenRouter_ClaimDailyRewardWithReceipt_mCEA3FE3252F39792C39F24CBC76E16CD6A7C0283_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ScreenRouter_ContinueAfterDialogue_mF1C4512958024BF0A6D98A90E5A6BD89BE09322C_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ScreenRouter_ContinueAfterVictory_mB21EDFD516ED99A0A1C1BADA2E583F3216118EDC_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ScreenRouter_ContinueAsDemoGuest_mA3B54A9162ADA3DCA8020BD7F2A6494277A79140_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ScreenRouter_NavigateBack_m8755B6CD7EDC8A0A4E6892C2EC74A718571E232B_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ScreenRouter_OpenChapterMap_mE8CA65193CAA1659F7CD1B2F5AF1810D41D9EE9B_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ScreenRouter_OpenSelectedLevel_m77909CC24AC5AEF4490139F276116EF060A4D0FD_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ScreenRouter_OpenStoryRecap_mDCFB012178541AEA849D6187CA92D02DA4D05E59_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ScreenRouter_RequestDemoFriendEnergy_mABC8AA11E27D15CAC582554E248BEA14C1C4BE67_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ScreenRouter_RequestRewardedDouble_m6EFD82DBC640EC55676C4E92F43F9B978EEFC737_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ScreenRouter_RequestRewardedEnergy_m2A6CDD4D1C0A3CA058E948D1DFC2245B3353F27D_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ScreenRouter_RestartCurrentLevel_m6A128993B82F46F7070A79E86AFB7E7E42D2149B_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ScreenRouter_U3CBuildInteractionLayerU3Eb__88_0_m51CE7BDD47FA8181156A22685B7CF5FCC2568D51_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ScreenRouter_U3CBuildInteractionLayerU3Eb__88_10_m6315B31010C8B496B229CADE0A2B03BEE339D10D_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ScreenRouter_U3CBuildInteractionLayerU3Eb__88_11_m52B72CFCDAC45E8ACC87779648A68D880A7EEB24_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ScreenRouter_U3CBuildInteractionLayerU3Eb__88_12_mAB062A6BBE652C5B9AD6E171C5E396A773BAE7CE_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ScreenRouter_U3CBuildInteractionLayerU3Eb__88_13_m781EBBF8CB1FD051F753556FBFFF44215EACB59D_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ScreenRouter_U3CBuildInteractionLayerU3Eb__88_14_m932AD31845A63E6685F0D16BF47CAC6A912C2041_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ScreenRouter_U3CBuildInteractionLayerU3Eb__88_15_m3E30443F15905AA47F5A72EF271D79F9A8E857B7_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ScreenRouter_U3CBuildInteractionLayerU3Eb__88_16_mD34CE6D564B272AB4FC6D6C90E6FCCFB9879E001_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ScreenRouter_U3CBuildInteractionLayerU3Eb__88_17_m0AE334A333E5C42E5725FA8FA28D5AE7EABD7182_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ScreenRouter_U3CBuildInteractionLayerU3Eb__88_18_m8975C4564A83CC6AD59F1D8D43C58178396BE888_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ScreenRouter_U3CBuildInteractionLayerU3Eb__88_19_mDE12BA841C5A1AA11043F2B7E26F280750D6E90D_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ScreenRouter_U3CBuildInteractionLayerU3Eb__88_1_mFC085E76DEE74CE3EBBB8834BAAA5E37FB2F6832_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ScreenRouter_U3CBuildInteractionLayerU3Eb__88_20_m37094E5176C0C2329D23BA344BD175BD9C27597B_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ScreenRouter_U3CBuildInteractionLayerU3Eb__88_21_m83BF9D2F1C685E821060320F2B55C104AF8F25C4_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ScreenRouter_U3CBuildInteractionLayerU3Eb__88_22_mA2090FB6C63BB8A5E4FDA5A8D32BFBCCCF263D65_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ScreenRouter_U3CBuildInteractionLayerU3Eb__88_23_m0CFC21EF0528B2064494C19A5CA60A63A24668AE_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ScreenRouter_U3CBuildInteractionLayerU3Eb__88_24_mBCF34D7B2D54BC5524B769F1CB32C1ADB008531B_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ScreenRouter_U3CBuildInteractionLayerU3Eb__88_25_m5DD6413E023EC3D7F5ED481ACB9754002A328141_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ScreenRouter_U3CBuildInteractionLayerU3Eb__88_26_mC213F0D218F1E6256DF66E4281AA85CE3F61BBA9_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ScreenRouter_U3CBuildInteractionLayerU3Eb__88_27_m81792042B0336211A06E4A08CAD064F110ED6BE9_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ScreenRouter_U3CBuildInteractionLayerU3Eb__88_28_mAD5CF4DA4FB28EC36A2D1D5515CB459BE4287F60_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ScreenRouter_U3CBuildInteractionLayerU3Eb__88_29_m617F7FC1396E52801EEB0C02EAE7A7FB63C3C9AB_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ScreenRouter_U3CBuildInteractionLayerU3Eb__88_2_m1D2B020E7A8C45CE2E6555CD0E05230361F3FAA8_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ScreenRouter_U3CBuildInteractionLayerU3Eb__88_30_m122F078C53A4C1D986DF5D07E04BDD6A29249CB9_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ScreenRouter_U3CBuildInteractionLayerU3Eb__88_31_mE307A802FF8CF690234F1A28E024BF17A857F668_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ScreenRouter_U3CBuildInteractionLayerU3Eb__88_32_m148E2AB3CFE6B287CA1E9CFBC443D3E1417D71C2_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ScreenRouter_U3CBuildInteractionLayerU3Eb__88_33_mCB1DD4DCD50EDBFAA2DC7D24ED510D96E8B56892_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ScreenRouter_U3CBuildInteractionLayerU3Eb__88_34_m995856A78BDBD84E8CD72752C42D6F150386DE13_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ScreenRouter_U3CBuildInteractionLayerU3Eb__88_35_mA958203EED0B9D52953007AF6057C76B4F813F52_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ScreenRouter_U3CBuildInteractionLayerU3Eb__88_36_m8D2CCC60D4EB13CE35ABC7D7B1828868E0C033A3_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ScreenRouter_U3CBuildInteractionLayerU3Eb__88_37_m434EAD46B6124BC03A8F2D2FF11390B37AECFF57_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ScreenRouter_U3CBuildInteractionLayerU3Eb__88_38_mF70039B16CBA1ED4FDEEC3A9FCDA387A7D28BE7E_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ScreenRouter_U3CBuildInteractionLayerU3Eb__88_39_m939436B6CD4A5D70B7C67D8CB28F50ACD7E1DC68_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ScreenRouter_U3CBuildInteractionLayerU3Eb__88_3_mE8CA0BE818A088576584640F709A98C9DE483A26_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ScreenRouter_U3CBuildInteractionLayerU3Eb__88_40_m4A3787528B1FF40CACA655619C782037610C01FD_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ScreenRouter_U3CBuildInteractionLayerU3Eb__88_41_mFEEE748300CDDF8BAED238C62E71437FBEEABB86_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ScreenRouter_U3CBuildInteractionLayerU3Eb__88_42_m745883A650D6D39796F9BDB1F36147A3B9C114A7_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ScreenRouter_U3CBuildInteractionLayerU3Eb__88_43_m12DECF666ECBCEC1BFD5CCC0833D8C65143D6FAE_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ScreenRouter_U3CBuildInteractionLayerU3Eb__88_44_mCD43168293167FB3231BAD872F1642810D22D85D_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ScreenRouter_U3CBuildInteractionLayerU3Eb__88_4_mB9024B13F46AA6AD5246176FF0194C4BC28674DE_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ScreenRouter_U3CBuildInteractionLayerU3Eb__88_5_m1011CBB8FE9AA4478878888C27311321AFE50352_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ScreenRouter_U3CBuildInteractionLayerU3Eb__88_6_mFE3908F13A97554AD7C3B39C3CDA35FE18BB3CF7_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ScreenRouter_U3CBuildInteractionLayerU3Eb__88_7_m3BF87725BCED6F007C9EE8F596399D10CD7F841A_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ScreenRouter_U3CBuildInteractionLayerU3Eb__88_8_mD9AC4463DE086DECEA15B3550E56E010DDAD517C_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ScreenRouter_U3CBuildInteractionLayerU3Eb__88_9_m06531CC90BAE20937A33C27CFB6E173BA79BDAFD_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&_stringLiteral4A261EB7E7319776625F5A015EA18053797E6890);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&_stringLiteral758F9F7600149356035075378D7710CE738E6CC2);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&_stringLiteral92971B9CC0D8D91320245699D154C5BF895FE8E0);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&_stringLiteralDB25773E7872A73CB193672D40268600EDAB04C8);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&_stringLiteralF2CF5A7966A17D11D505271E6D0172459B637BB4);
		s_Il2CppMethodInitialized = true;
	}
	ProductionHomeView_tB424DCD664CAE857FD5323EEB6B98B1CFB918694* V_0 = NULL;
	ProductionWorldMapView_t9EE066392822F11A7943261785563B6FFB262CF7* V_1 = NULL;
	ProductionChapterMapView_t73FA61D194FCDDD0F89F8021A5BBFC50FD5D1075* V_2 = NULL;
	ProductionPreLevelView_t19543F1A5A95A83F99E01D12193810B0543C6BDD* V_3 = NULL;
	ProductionDialogueView_t9612D941765F8A0545DD0212DE1D76B0BDD0F8C5* V_4 = NULL;
	ProductionVictoryView_t09841A4498FAE3A2C54918934DC80DBD053E851A* V_5 = NULL;
	ProductionFailView_t5D82248F0E88B603813B2F82EE1CA191D740F251* V_6 = NULL;
	ProductionShopView_tCAE5004D358A893BA810EE3CC9331FF6A5AAB245* V_7 = NULL;
	ProductionDailyRewardView_tD3D86E84BE0D44D4F0F4963448D4CB68094FD0A9* V_8 = NULL;
	ProductionBoosterSelectionView_tC3AB07A664B7F97CBD28AB8693822704399FF79C* V_9 = NULL;
	ProductionOutOfEnergyView_t9ED536371FCF21CC2E58121FFF0561349450226A* V_10 = NULL;
	ProductionRewardReceivedView_t308036C90189F05146CA7191FC9674291133EC11* V_11 = NULL;
	ProductionEventsView_t60F8250E60FB6585506BBB36AB2FA5C392D130FA* V_12 = NULL;
	ProductionProfileView_tFBAE39BD387DB9B67C01A51EB40718FD7461992D* V_13 = NULL;
	ProductionCharactersView_t80FF719EBB00992075D821990BF966D5E95577DE* V_14 = NULL;
	ProductionMailView_t613C10918DAA8582972FFE0681462F4DAE73B966* V_15 = NULL;
	SplashTitleView_t5C9B3C703C7BCA2970A475F6B1EF763DD452FB17* V_16 = NULL;
	ProductionEventDetailView_t7360325479FBCA2559711C551444B3B712CF6CE5* V_17 = NULL;
	ProductionIndividualRankingView_tF449BDA2B9876849864F7F78C4C57CE0BB9726B3* V_18 = NULL;
	ProductionTeamHubView_t242CA702D2D2A557A5145C582FE6955173320992* V_19 = NULL;
	ProductionTeamRankingView_t6A2E723B48D2D1F086684E393EC71E7A19E82DD4* V_20 = NULL;
	ProductionTeamChatView_t3876DDF380A2DA789CB223EB9E32A75578DFE07E* V_21 = NULL;
	ProductionFriendsListView_t568A80A4F587DE5E0D9156810474BD1B241353D1* V_22 = NULL;
	ProductionInventoryView_tBFCB541C14E4ED40B2CB6633CE17AC0EB9D14E8E* V_23 = NULL;
	ProductionSpecialOfferView_t26243C08B7058A2D21A48610B99150458CC7CDD7* V_24 = NULL;
	ProductionLanguageSelectView_tC9B9BF5C49A8106E900C4E16F3C587BAB476E212* V_25 = NULL;
	ProductionLoginView_t36729B7F5B697B3828DE80A1A0EB2509C7F66B81* V_26 = NULL;
	ProductionTutorialView_t17A2D559FD4E71F841E57E107C3B8FADDC3DA4CB* V_27 = NULL;
	ProductionOfflineView_t2A35DC0266DAE631DB63C259E5095E1F51E46451* V_28 = NULL;
	ProductionMaintenanceView_tE78AA4EB7FC8F41C25AF278B1341DAA6B8F091A1* V_29 = NULL;
	ProductionChapterCompleteView_t8F3C64B99DD107C039783DF1D0A39530DDAA9F70* V_30 = NULL;
	Nullable_1_tFF46807EF2622F15A62B26F542EC38258BA8254E V_31;
	memset((&V_31), 0, sizeof(V_31));
	{
		int32_t L_0 = ___0_id;
		switch (((int32_t)il2cpp_codegen_subtract((int32_t)L_0, 1)))
		{
			case 0:
			{
				goto IL_0095;
			}
			case 1:
			{
				goto IL_00d8;
			}
			case 2:
			{
				goto IL_0137;
			}
			case 3:
			{
				goto IL_01a2;
			}
			case 4:
			{
				goto IL_01fc;
			}
			case 5:
			{
				goto IL_0204;
			}
			case 6:
			{
				goto IL_023e;
			}
			case 7:
			{
				goto IL_0290;
			}
			case 8:
			{
				goto IL_02e2;
			}
			case 9:
			{
				goto IL_0a20;
			}
			case 10:
			{
				goto IL_0334;
			}
			case 11:
			{
				goto IL_042c;
			}
			case 12:
			{
				goto IL_0386;
			}
			case 13:
			{
				goto IL_0471;
			}
			case 14:
			{
				goto IL_04b7;
			}
			case 15:
			{
				goto IL_0509;
			}
			case 16:
			{
				goto IL_0a4e;
			}
			case 17:
			{
				goto IL_0511;
			}
			case 18:
			{
				goto IL_0563;
			}
			case 19:
			{
				goto IL_03cd;
			}
			case 20:
			{
				goto IL_09e6;
			}
			case 21:
			{
				goto IL_05a9;
			}
			case 22:
			{
				goto IL_0632;
			}
			case 23:
			{
				goto IL_0678;
			}
			case 24:
			{
				goto IL_06be;
			}
			case 25:
			{
				goto IL_0704;
			}
			case 26:
			{
				goto IL_074a;
			}
			case 27:
			{
				goto IL_0790;
			}
			case 28:
			{
				goto IL_07d6;
			}
			case 29:
			{
				goto IL_0828;
			}
			case 30:
			{
				goto IL_087a;
			}
			case 31:
			{
				goto IL_08b4;
			}
			case 32:
			{
				goto IL_08fa;
			}
			case 33:
			{
				goto IL_0959;
			}
			case 34:
			{
				goto IL_09ac;
			}
		}
	}
	{
		return;
	}

IL_0095:
	{
		RectTransform_t6C5DA5E41A89E0F488B001E45E58963480E543A5* L_1 = ___1_parent;
		NullCheck(L_1);
		ProductionHomeView_tB424DCD664CAE857FD5323EEB6B98B1CFB918694* L_2;
		L_2 = Component_GetComponentInChildren_TisProductionHomeView_tB424DCD664CAE857FD5323EEB6B98B1CFB918694_mABA6BCAC08EB0EC7E26E912407EC13910805FE47(L_1, (bool)1, Component_GetComponentInChildren_TisProductionHomeView_tB424DCD664CAE857FD5323EEB6B98B1CFB918694_mABA6BCAC08EB0EC7E26E912407EC13910805FE47_RuntimeMethod_var);
		V_0 = L_2;
		ProductionHomeView_tB424DCD664CAE857FD5323EEB6B98B1CFB918694* L_3 = V_0;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_4;
		L_4 = Object_op_Equality_mB6120F782D83091EF56A198FCEBCF066DB4A9605(L_3, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (L_4)
		{
			goto IL_00ae;
		}
	}
	{
		ProductionHomeView_tB424DCD664CAE857FD5323EEB6B98B1CFB918694* L_5 = V_0;
		NullCheck(L_5);
		bool L_6;
		L_6 = ProductionHomeView_get_HasRequiredBindings_m1E8E85F3E8E6F35AD49B1BA04DA6F70D71B87237(L_5, NULL);
		if (L_6)
		{
			goto IL_00b9;
		}
	}

IL_00ae:
	{
		il2cpp_codegen_runtime_class_init_inline(Debug_t8394C7EEAECA3689C2C9B9DE9C7166D73596276F_il2cpp_TypeInfo_var);
		Debug_LogError_mB00B2B4468EF3CAF041B038D840820FB84C924B2(_stringLiteralF2CF5A7966A17D11D505271E6D0172459B637BB4, NULL);
		return;
	}

IL_00b9:
	{
		ProductionHomeView_tB424DCD664CAE857FD5323EEB6B98B1CFB918694* L_7 = V_0;
		Action_1_t913CB0C3CDC5C59F6B8A1A7D957A63049F5A6BCA* L_8 = (Action_1_t913CB0C3CDC5C59F6B8A1A7D957A63049F5A6BCA*)il2cpp_codegen_object_new(Action_1_t913CB0C3CDC5C59F6B8A1A7D957A63049F5A6BCA_il2cpp_TypeInfo_var);
		Action_1__ctor_m5798433B5A0855FC0015630BF99C272AF00F3D75(L_8, __this, (intptr_t)((void*)ScreenRouter_U3CBuildInteractionLayerU3Eb__88_0_m51CE7BDD47FA8181156A22685B7CF5FCC2568D51_RuntimeMethod_var), NULL);
		Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* L_9 = (Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07*)il2cpp_codegen_object_new(Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07_il2cpp_TypeInfo_var);
		Action__ctor_mBDC7B0B4A3F583B64C2896F01BDED360772F67DC(L_9, __this, (intptr_t)((void*)ScreenRouter_OpenChapterMap_mE8CA65193CAA1659F7CD1B2F5AF1810D41D9EE9B_RuntimeMethod_var), NULL);
		NullCheck(L_7);
		ProductionHomeView_BindNavigation_mA5C04A1AEC1A2DB66AEF97B92FC8CB3D8C576C6A(L_7, L_8, L_9, NULL);
		return;
	}

IL_00d8:
	{
		RectTransform_t6C5DA5E41A89E0F488B001E45E58963480E543A5* L_10 = ___1_parent;
		NullCheck(L_10);
		ProductionWorldMapView_t9EE066392822F11A7943261785563B6FFB262CF7* L_11;
		L_11 = Component_GetComponentInChildren_TisProductionWorldMapView_t9EE066392822F11A7943261785563B6FFB262CF7_mF04510D979FA706C943DD04AAE13D47E17DE9A8E(L_10, Component_GetComponentInChildren_TisProductionWorldMapView_t9EE066392822F11A7943261785563B6FFB262CF7_mF04510D979FA706C943DD04AAE13D47E17DE9A8E_RuntimeMethod_var);
		V_1 = L_11;
		ProductionWorldMapView_t9EE066392822F11A7943261785563B6FFB262CF7* L_12 = V_1;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_13;
		L_13 = Object_op_Equality_mB6120F782D83091EF56A198FCEBCF066DB4A9605(L_12, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (L_13)
		{
			goto IL_00f0;
		}
	}
	{
		ProductionWorldMapView_t9EE066392822F11A7943261785563B6FFB262CF7* L_14 = V_1;
		NullCheck(L_14);
		bool L_15;
		L_15 = ProductionWorldMapView_get_HasRequiredBindings_m1A1E50A1C74904DED16DDBD30BD488095F72C722(L_14, NULL);
		if (L_15)
		{
			goto IL_00fb;
		}
	}

IL_00f0:
	{
		il2cpp_codegen_runtime_class_init_inline(Debug_t8394C7EEAECA3689C2C9B9DE9C7166D73596276F_il2cpp_TypeInfo_var);
		Debug_LogError_mB00B2B4468EF3CAF041B038D840820FB84C924B2(_stringLiteralDB25773E7872A73CB193672D40268600EDAB04C8, NULL);
		return;
	}

IL_00fb:
	{
		ProductionWorldMapView_t9EE066392822F11A7943261785563B6FFB262CF7* L_16 = V_1;
		Action_1_t913CB0C3CDC5C59F6B8A1A7D957A63049F5A6BCA* L_17 = (Action_1_t913CB0C3CDC5C59F6B8A1A7D957A63049F5A6BCA*)il2cpp_codegen_object_new(Action_1_t913CB0C3CDC5C59F6B8A1A7D957A63049F5A6BCA_il2cpp_TypeInfo_var);
		Action_1__ctor_m5798433B5A0855FC0015630BF99C272AF00F3D75(L_17, __this, (intptr_t)((void*)ScreenRouter_U3CBuildInteractionLayerU3Eb__88_1_mFC085E76DEE74CE3EBBB8834BAAA5E37FB2F6832_RuntimeMethod_var), NULL);
		Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* L_18 = (Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07*)il2cpp_codegen_object_new(Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07_il2cpp_TypeInfo_var);
		Action__ctor_mBDC7B0B4A3F583B64C2896F01BDED360772F67DC(L_18, __this, (intptr_t)((void*)ScreenRouter_OpenChapterMap_mE8CA65193CAA1659F7CD1B2F5AF1810D41D9EE9B_RuntimeMethod_var), NULL);
		Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* L_19 = (Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07*)il2cpp_codegen_object_new(Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07_il2cpp_TypeInfo_var);
		Action__ctor_mBDC7B0B4A3F583B64C2896F01BDED360772F67DC(L_19, __this, (intptr_t)((void*)ScreenRouter_U3CBuildInteractionLayerU3Eb__88_2_m1D2B020E7A8C45CE2E6555CD0E05230361F3FAA8_RuntimeMethod_var), NULL);
		ProductionWorldMapView_t9EE066392822F11A7943261785563B6FFB262CF7* L_20 = V_1;
		NullCheck(L_20);
		ProductionMapNotice_tDF38B2FD90624E90B58D500B0C8519AAD4662475* L_21 = L_20->___Notice;
		Action_1_t3CB5D1A819C3ED3F99E9E39F890F18633253949A* L_22 = (Action_1_t3CB5D1A819C3ED3F99E9E39F890F18633253949A*)il2cpp_codegen_object_new(Action_1_t3CB5D1A819C3ED3F99E9E39F890F18633253949A_il2cpp_TypeInfo_var);
		Action_1__ctor_m9DC2953C55C4D7D4B7BEFE03D84DA1F9362D652C(L_22, L_21, (intptr_t)((void*)ProductionMapNotice_Show_mB6EAAA8554BBD1922FC4ABE702FBF6154F9344CF_RuntimeMethod_var), NULL);
		NullCheck(L_16);
		ProductionWorldMapView_BindNavigation_m5060DDE2F072C233798501A44D90A048CE23EADB(L_16, L_17, L_18, L_19, L_22, NULL);
		return;
	}

IL_0137:
	{
		RectTransform_t6C5DA5E41A89E0F488B001E45E58963480E543A5* L_23 = ___1_parent;
		NullCheck(L_23);
		ProductionChapterMapView_t73FA61D194FCDDD0F89F8021A5BBFC50FD5D1075* L_24;
		L_24 = Component_GetComponentInChildren_TisProductionChapterMapView_t73FA61D194FCDDD0F89F8021A5BBFC50FD5D1075_m7A6AAD6BEAA16D72940FF80B66D58CCFB67BE62F(L_23, Component_GetComponentInChildren_TisProductionChapterMapView_t73FA61D194FCDDD0F89F8021A5BBFC50FD5D1075_m7A6AAD6BEAA16D72940FF80B66D58CCFB67BE62F_RuntimeMethod_var);
		V_2 = L_24;
		ProductionChapterMapView_t73FA61D194FCDDD0F89F8021A5BBFC50FD5D1075* L_25 = V_2;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_26;
		L_26 = Object_op_Equality_mB6120F782D83091EF56A198FCEBCF066DB4A9605(L_25, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (L_26)
		{
			goto IL_014f;
		}
	}
	{
		ProductionChapterMapView_t73FA61D194FCDDD0F89F8021A5BBFC50FD5D1075* L_27 = V_2;
		NullCheck(L_27);
		bool L_28;
		L_28 = ProductionChapterMapView_get_HasRequiredBindings_mA53DD1358A29340994C8A5722F52E72856C60D5F(L_27, NULL);
		if (L_28)
		{
			goto IL_015a;
		}
	}

IL_014f:
	{
		il2cpp_codegen_runtime_class_init_inline(Debug_t8394C7EEAECA3689C2C9B9DE9C7166D73596276F_il2cpp_TypeInfo_var);
		Debug_LogError_mB00B2B4468EF3CAF041B038D840820FB84C924B2(_stringLiteral758F9F7600149356035075378D7710CE738E6CC2, NULL);
		return;
	}

IL_015a:
	{
		ProductionChapterMapView_t73FA61D194FCDDD0F89F8021A5BBFC50FD5D1075* L_29 = V_2;
		Action_1_t913CB0C3CDC5C59F6B8A1A7D957A63049F5A6BCA* L_30 = (Action_1_t913CB0C3CDC5C59F6B8A1A7D957A63049F5A6BCA*)il2cpp_codegen_object_new(Action_1_t913CB0C3CDC5C59F6B8A1A7D957A63049F5A6BCA_il2cpp_TypeInfo_var);
		Action_1__ctor_m5798433B5A0855FC0015630BF99C272AF00F3D75(L_30, __this, (intptr_t)((void*)ScreenRouter_U3CBuildInteractionLayerU3Eb__88_3_mE8CA0BE818A088576584640F709A98C9DE483A26_RuntimeMethod_var), NULL);
		Func_1_t2BE7F58348C9CC544A8973B3A9E55541DE43C457* L_31 = (Func_1_t2BE7F58348C9CC544A8973B3A9E55541DE43C457*)il2cpp_codegen_object_new(Func_1_t2BE7F58348C9CC544A8973B3A9E55541DE43C457_il2cpp_TypeInfo_var);
		Func_1__ctor_mDFFAE9C73346372438B5B04C4558AC42F1A3DA22(L_31, __this, (intptr_t)((void*)ScreenRouter_OpenSelectedLevel_m77909CC24AC5AEF4490139F276116EF060A4D0FD_RuntimeMethod_var), NULL);
		Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* L_32 = (Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07*)il2cpp_codegen_object_new(Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07_il2cpp_TypeInfo_var);
		Action__ctor_mBDC7B0B4A3F583B64C2896F01BDED360772F67DC(L_32, __this, (intptr_t)((void*)ScreenRouter_U3CBuildInteractionLayerU3Eb__88_4_mB9024B13F46AA6AD5246176FF0194C4BC28674DE_RuntimeMethod_var), NULL);
		ProductionChapterMapView_t73FA61D194FCDDD0F89F8021A5BBFC50FD5D1075* L_33 = V_2;
		NullCheck(L_33);
		ProductionMapNotice_tDF38B2FD90624E90B58D500B0C8519AAD4662475* L_34 = L_33->___Notice;
		Action_1_t3CB5D1A819C3ED3F99E9E39F890F18633253949A* L_35 = (Action_1_t3CB5D1A819C3ED3F99E9E39F890F18633253949A*)il2cpp_codegen_object_new(Action_1_t3CB5D1A819C3ED3F99E9E39F890F18633253949A_il2cpp_TypeInfo_var);
		Action_1__ctor_m9DC2953C55C4D7D4B7BEFE03D84DA1F9362D652C(L_35, L_34, (intptr_t)((void*)ProductionMapNotice_Show_mB6EAAA8554BBD1922FC4ABE702FBF6154F9344CF_RuntimeMethod_var), NULL);
		Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* L_36 = (Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07*)il2cpp_codegen_object_new(Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07_il2cpp_TypeInfo_var);
		Action__ctor_mBDC7B0B4A3F583B64C2896F01BDED360772F67DC(L_36, __this, (intptr_t)((void*)ScreenRouter_U3CBuildInteractionLayerU3Eb__88_5_m1011CBB8FE9AA4478878888C27311321AFE50352_RuntimeMethod_var), NULL);
		NullCheck(L_29);
		ProductionChapterMapView_BindNavigation_m13C947A4454F1F5A8A6B9ADF7575617C7FE90152(L_29, L_30, L_31, L_32, L_35, L_36, NULL);
		return;
	}

IL_01a2:
	{
		RectTransform_t6C5DA5E41A89E0F488B001E45E58963480E543A5* L_37 = ___1_parent;
		NullCheck(L_37);
		ProductionPreLevelView_t19543F1A5A95A83F99E01D12193810B0543C6BDD* L_38;
		L_38 = Component_GetComponentInChildren_TisProductionPreLevelView_t19543F1A5A95A83F99E01D12193810B0543C6BDD_m46F83BF669465C64237D11120DCB68E2117022FE(L_37, Component_GetComponentInChildren_TisProductionPreLevelView_t19543F1A5A95A83F99E01D12193810B0543C6BDD_m46F83BF669465C64237D11120DCB68E2117022FE_RuntimeMethod_var);
		V_3 = L_38;
		ProductionPreLevelView_t19543F1A5A95A83F99E01D12193810B0543C6BDD* L_39 = V_3;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_40;
		L_40 = Object_op_Equality_mB6120F782D83091EF56A198FCEBCF066DB4A9605(L_39, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (L_40)
		{
			goto IL_01ba;
		}
	}
	{
		ProductionPreLevelView_t19543F1A5A95A83F99E01D12193810B0543C6BDD* L_41 = V_3;
		NullCheck(L_41);
		bool L_42;
		L_42 = ProductionPreLevelView_get_HasRequiredBindings_m0522608AB4A2DC60F4BAAE12A8E83A24FEEB162A(L_41, NULL);
		if (L_42)
		{
			goto IL_01c5;
		}
	}

IL_01ba:
	{
		InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB* L_43 = (InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB*)il2cpp_codegen_object_new(((RuntimeClass*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB_il2cpp_TypeInfo_var)));
		InvalidOperationException__ctor_mE4CB6F4712AB6D99A2358FBAE2E052B3EE976162(L_43, ((String_t*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&_stringLiteral2AA1A1F32CBBBB36C763CFA78354AB9F5B2061CB)), NULL);
		IL2CPP_RAISE_MANAGED_EXCEPTION(L_43, ((RuntimeMethod*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&ScreenRouter_BuildInteractionLayer_m098546A1A74D7C435B6EAE0086CE1D53AD4D93EF_RuntimeMethod_var)));
	}

IL_01c5:
	{
		ProductionPreLevelView_t19543F1A5A95A83F99E01D12193810B0543C6BDD* L_44 = V_3;
		Action_1_t913CB0C3CDC5C59F6B8A1A7D957A63049F5A6BCA* L_45 = (Action_1_t913CB0C3CDC5C59F6B8A1A7D957A63049F5A6BCA*)il2cpp_codegen_object_new(Action_1_t913CB0C3CDC5C59F6B8A1A7D957A63049F5A6BCA_il2cpp_TypeInfo_var);
		Action_1__ctor_m5798433B5A0855FC0015630BF99C272AF00F3D75(L_45, __this, (intptr_t)((void*)ScreenRouter_U3CBuildInteractionLayerU3Eb__88_6_mFE3908F13A97554AD7C3B39C3CDA35FE18BB3CF7_RuntimeMethod_var), NULL);
		Func_1_t2BE7F58348C9CC544A8973B3A9E55541DE43C457* L_46 = (Func_1_t2BE7F58348C9CC544A8973B3A9E55541DE43C457*)il2cpp_codegen_object_new(Func_1_t2BE7F58348C9CC544A8973B3A9E55541DE43C457_il2cpp_TypeInfo_var);
		Func_1__ctor_mDFFAE9C73346372438B5B04C4558AC42F1A3DA22(L_46, __this, (intptr_t)((void*)ScreenRouter_U3CBuildInteractionLayerU3Eb__88_7_m3BF87725BCED6F007C9EE8F596399D10CD7F841A_RuntimeMethod_var), NULL);
		Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* L_47 = (Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07*)il2cpp_codegen_object_new(Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07_il2cpp_TypeInfo_var);
		Action__ctor_mBDC7B0B4A3F583B64C2896F01BDED360772F67DC(L_47, __this, (intptr_t)((void*)ScreenRouter_U3CBuildInteractionLayerU3Eb__88_8_mD9AC4463DE086DECEA15B3550E56E010DDAD517C_RuntimeMethod_var), NULL);
		Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* L_48 = (Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07*)il2cpp_codegen_object_new(Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07_il2cpp_TypeInfo_var);
		Action__ctor_mBDC7B0B4A3F583B64C2896F01BDED360772F67DC(L_48, __this, (intptr_t)((void*)ScreenRouter_U3CBuildInteractionLayerU3Eb__88_9_m06531CC90BAE20937A33C27CFB6E173BA79BDAFD_RuntimeMethod_var), NULL);
		NullCheck(L_44);
		ProductionPreLevelView_BindNavigation_m1229ED716A00269793F7E8FAB3CAE2B8FABADBA8(L_44, L_45, L_46, L_47, L_48, NULL);
		return;
	}

IL_01fc:
	{
		RectTransform_t6C5DA5E41A89E0F488B001E45E58963480E543A5* L_49 = ___1_parent;
		ScreenRouter_BuildGameplayOverlay_m2F3676D27FEACFFE9E838EF9B5EAF70A96E2BEBD(__this, L_49, NULL);
		return;
	}

IL_0204:
	{
		RectTransform_t6C5DA5E41A89E0F488B001E45E58963480E543A5* L_50 = ___1_parent;
		NullCheck(L_50);
		ProductionDialogueView_t9612D941765F8A0545DD0212DE1D76B0BDD0F8C5* L_51;
		L_51 = Component_GetComponentInChildren_TisProductionDialogueView_t9612D941765F8A0545DD0212DE1D76B0BDD0F8C5_m1D008ACA9283C198043BDFE9174D5580BD5136E0(L_50, Component_GetComponentInChildren_TisProductionDialogueView_t9612D941765F8A0545DD0212DE1D76B0BDD0F8C5_m1D008ACA9283C198043BDFE9174D5580BD5136E0_RuntimeMethod_var);
		V_4 = L_51;
		ProductionDialogueView_t9612D941765F8A0545DD0212DE1D76B0BDD0F8C5* L_52 = V_4;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_53;
		L_53 = Object_op_Equality_mB6120F782D83091EF56A198FCEBCF066DB4A9605(L_52, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (L_53)
		{
			goto IL_021f;
		}
	}
	{
		ProductionDialogueView_t9612D941765F8A0545DD0212DE1D76B0BDD0F8C5* L_54 = V_4;
		NullCheck(L_54);
		bool L_55;
		L_55 = ProductionDialogueView_get_HasRequiredBindings_m187105A6FA2C9649BFB183C25D6091DFDA92D019(L_54, NULL);
		if (L_55)
		{
			goto IL_022a;
		}
	}

IL_021f:
	{
		InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB* L_56 = (InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB*)il2cpp_codegen_object_new(((RuntimeClass*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB_il2cpp_TypeInfo_var)));
		InvalidOperationException__ctor_mE4CB6F4712AB6D99A2358FBAE2E052B3EE976162(L_56, ((String_t*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&_stringLiteral45D42AA4F148A0C21ADDEB41302244BA7B9714DC)), NULL);
		IL2CPP_RAISE_MANAGED_EXCEPTION(L_56, ((RuntimeMethod*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&ScreenRouter_BuildInteractionLayer_m098546A1A74D7C435B6EAE0086CE1D53AD4D93EF_RuntimeMethod_var)));
	}

IL_022a:
	{
		ProductionDialogueView_t9612D941765F8A0545DD0212DE1D76B0BDD0F8C5* L_57 = V_4;
		Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* L_58 = (Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07*)il2cpp_codegen_object_new(Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07_il2cpp_TypeInfo_var);
		Action__ctor_mBDC7B0B4A3F583B64C2896F01BDED360772F67DC(L_58, __this, (intptr_t)((void*)ScreenRouter_ContinueAfterDialogue_mF1C4512958024BF0A6D98A90E5A6BD89BE09322C_RuntimeMethod_var), NULL);
		NullCheck(L_57);
		ProductionDialogueView_Bind_m620E9F9D67FC1E7E52C0BC3DC18059732472DB9C(L_57, L_58, NULL);
		return;
	}

IL_023e:
	{
		RectTransform_t6C5DA5E41A89E0F488B001E45E58963480E543A5* L_59 = ___1_parent;
		NullCheck(L_59);
		ProductionVictoryView_t09841A4498FAE3A2C54918934DC80DBD053E851A* L_60;
		L_60 = Component_GetComponentInChildren_TisProductionVictoryView_t09841A4498FAE3A2C54918934DC80DBD053E851A_mC5D774D98C17EC0007845B4B58C751C718D28185(L_59, Component_GetComponentInChildren_TisProductionVictoryView_t09841A4498FAE3A2C54918934DC80DBD053E851A_mC5D774D98C17EC0007845B4B58C751C718D28185_RuntimeMethod_var);
		V_5 = L_60;
		ProductionVictoryView_t09841A4498FAE3A2C54918934DC80DBD053E851A* L_61 = V_5;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_62;
		L_62 = Object_op_Equality_mB6120F782D83091EF56A198FCEBCF066DB4A9605(L_61, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (L_62)
		{
			goto IL_0259;
		}
	}
	{
		ProductionVictoryView_t09841A4498FAE3A2C54918934DC80DBD053E851A* L_63 = V_5;
		NullCheck(L_63);
		bool L_64;
		L_64 = ProductionVictoryView_get_HasRequiredBindings_mEEFB0AA4B928E0A10C97B7A24FC7A793C1CE0B0F(L_63, NULL);
		if (L_64)
		{
			goto IL_0264;
		}
	}

IL_0259:
	{
		InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB* L_65 = (InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB*)il2cpp_codegen_object_new(((RuntimeClass*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB_il2cpp_TypeInfo_var)));
		InvalidOperationException__ctor_mE4CB6F4712AB6D99A2358FBAE2E052B3EE976162(L_65, ((String_t*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&_stringLiteral39DA62FE0E935F32B1D2AD40ED8D4F8CED27FF19)), NULL);
		IL2CPP_RAISE_MANAGED_EXCEPTION(L_65, ((RuntimeMethod*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&ScreenRouter_BuildInteractionLayer_m098546A1A74D7C435B6EAE0086CE1D53AD4D93EF_RuntimeMethod_var)));
	}

IL_0264:
	{
		ProductionVictoryView_t09841A4498FAE3A2C54918934DC80DBD053E851A* L_66 = V_5;
		Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* L_67 = (Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07*)il2cpp_codegen_object_new(Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07_il2cpp_TypeInfo_var);
		Action__ctor_mBDC7B0B4A3F583B64C2896F01BDED360772F67DC(L_67, __this, (intptr_t)((void*)ScreenRouter_ContinueAfterVictory_mB21EDFD516ED99A0A1C1BADA2E583F3216118EDC_RuntimeMethod_var), NULL);
		Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* L_68 = (Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07*)il2cpp_codegen_object_new(Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07_il2cpp_TypeInfo_var);
		Action__ctor_mBDC7B0B4A3F583B64C2896F01BDED360772F67DC(L_68, __this, (intptr_t)((void*)ScreenRouter_RequestRewardedDouble_m6EFD82DBC640EC55676C4E92F43F9B978EEFC737_RuntimeMethod_var), NULL);
		Action_1_t913CB0C3CDC5C59F6B8A1A7D957A63049F5A6BCA* L_69 = (Action_1_t913CB0C3CDC5C59F6B8A1A7D957A63049F5A6BCA*)il2cpp_codegen_object_new(Action_1_t913CB0C3CDC5C59F6B8A1A7D957A63049F5A6BCA_il2cpp_TypeInfo_var);
		Action_1__ctor_m5798433B5A0855FC0015630BF99C272AF00F3D75(L_69, __this, (intptr_t)((void*)ScreenRouter_U3CBuildInteractionLayerU3Eb__88_10_m6315B31010C8B496B229CADE0A2B03BEE339D10D_RuntimeMethod_var), NULL);
		NullCheck(L_66);
		ProductionVictoryView_BindNavigation_mAC3BEB83708416B2DD0EBCA34B48EFA4D9FD4A1B(L_66, L_67, L_68, L_69, NULL);
		return;
	}

IL_0290:
	{
		RectTransform_t6C5DA5E41A89E0F488B001E45E58963480E543A5* L_70 = ___1_parent;
		NullCheck(L_70);
		ProductionFailView_t5D82248F0E88B603813B2F82EE1CA191D740F251* L_71;
		L_71 = Component_GetComponentInChildren_TisProductionFailView_t5D82248F0E88B603813B2F82EE1CA191D740F251_mDE013F807A95E81C33979B237BF5F2FC2C4FBE40(L_70, Component_GetComponentInChildren_TisProductionFailView_t5D82248F0E88B603813B2F82EE1CA191D740F251_mDE013F807A95E81C33979B237BF5F2FC2C4FBE40_RuntimeMethod_var);
		V_6 = L_71;
		ProductionFailView_t5D82248F0E88B603813B2F82EE1CA191D740F251* L_72 = V_6;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_73;
		L_73 = Object_op_Equality_mB6120F782D83091EF56A198FCEBCF066DB4A9605(L_72, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (L_73)
		{
			goto IL_02ab;
		}
	}
	{
		ProductionFailView_t5D82248F0E88B603813B2F82EE1CA191D740F251* L_74 = V_6;
		NullCheck(L_74);
		bool L_75;
		L_75 = ProductionFailView_get_HasRequiredBindings_m5BA63BE0534C0D1D9B2C19F14CE33B79965C2A1B(L_74, NULL);
		if (L_75)
		{
			goto IL_02b6;
		}
	}

IL_02ab:
	{
		InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB* L_76 = (InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB*)il2cpp_codegen_object_new(((RuntimeClass*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB_il2cpp_TypeInfo_var)));
		InvalidOperationException__ctor_mE4CB6F4712AB6D99A2358FBAE2E052B3EE976162(L_76, ((String_t*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&_stringLiteral16CCEDFAC42DC7B76BF4A28B63A8F20D4E02EDD3)), NULL);
		IL2CPP_RAISE_MANAGED_EXCEPTION(L_76, ((RuntimeMethod*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&ScreenRouter_BuildInteractionLayer_m098546A1A74D7C435B6EAE0086CE1D53AD4D93EF_RuntimeMethod_var)));
	}

IL_02b6:
	{
		ProductionFailView_t5D82248F0E88B603813B2F82EE1CA191D740F251* L_77 = V_6;
		Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* L_78 = (Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07*)il2cpp_codegen_object_new(Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07_il2cpp_TypeInfo_var);
		Action__ctor_mBDC7B0B4A3F583B64C2896F01BDED360772F67DC(L_78, __this, (intptr_t)((void*)ScreenRouter_RestartCurrentLevel_m6A128993B82F46F7070A79E86AFB7E7E42D2149B_RuntimeMethod_var), NULL);
		Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* L_79 = (Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07*)il2cpp_codegen_object_new(Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07_il2cpp_TypeInfo_var);
		Action__ctor_mBDC7B0B4A3F583B64C2896F01BDED360772F67DC(L_79, __this, (intptr_t)((void*)ScreenRouter_U3CBuildInteractionLayerU3Eb__88_11_m52B72CFCDAC45E8ACC87779648A68D880A7EEB24_RuntimeMethod_var), NULL);
		Action_1_t913CB0C3CDC5C59F6B8A1A7D957A63049F5A6BCA* L_80 = (Action_1_t913CB0C3CDC5C59F6B8A1A7D957A63049F5A6BCA*)il2cpp_codegen_object_new(Action_1_t913CB0C3CDC5C59F6B8A1A7D957A63049F5A6BCA_il2cpp_TypeInfo_var);
		Action_1__ctor_m5798433B5A0855FC0015630BF99C272AF00F3D75(L_80, __this, (intptr_t)((void*)ScreenRouter_U3CBuildInteractionLayerU3Eb__88_12_mAB062A6BBE652C5B9AD6E171C5E396A773BAE7CE_RuntimeMethod_var), NULL);
		NullCheck(L_77);
		ProductionFailView_BindNavigation_m05E49889C4B4DEA00A9F6B3141282CF326EDF01C(L_77, L_78, L_79, L_80, NULL);
		return;
	}

IL_02e2:
	{
		RectTransform_t6C5DA5E41A89E0F488B001E45E58963480E543A5* L_81 = ___1_parent;
		NullCheck(L_81);
		ProductionShopView_tCAE5004D358A893BA810EE3CC9331FF6A5AAB245* L_82;
		L_82 = Component_GetComponentInChildren_TisProductionShopView_tCAE5004D358A893BA810EE3CC9331FF6A5AAB245_m2BB6613012F1FC7BFDF2D183D920E0124FB8CA63(L_81, Component_GetComponentInChildren_TisProductionShopView_tCAE5004D358A893BA810EE3CC9331FF6A5AAB245_m2BB6613012F1FC7BFDF2D183D920E0124FB8CA63_RuntimeMethod_var);
		V_7 = L_82;
		ProductionShopView_tCAE5004D358A893BA810EE3CC9331FF6A5AAB245* L_83 = V_7;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_84;
		L_84 = Object_op_Inequality_mD0BE578448EAA61948F25C32F8DD55AB1F778602(L_83, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (!L_84)
		{
			goto IL_0329;
		}
	}
	{
		ProductionShopView_tCAE5004D358A893BA810EE3CC9331FF6A5AAB245* L_85 = V_7;
		NullCheck(L_85);
		bool L_86;
		L_86 = ProductionShopView_get_HasRequiredBindings_m2E560F53850517FAC148ED7AA746524DA9D96404(L_85, NULL);
		if (!L_86)
		{
			goto IL_0329;
		}
	}
	{
		ProductionShopView_tCAE5004D358A893BA810EE3CC9331FF6A5AAB245* L_87 = V_7;
		Action_1_t913CB0C3CDC5C59F6B8A1A7D957A63049F5A6BCA* L_88 = (Action_1_t913CB0C3CDC5C59F6B8A1A7D957A63049F5A6BCA*)il2cpp_codegen_object_new(Action_1_t913CB0C3CDC5C59F6B8A1A7D957A63049F5A6BCA_il2cpp_TypeInfo_var);
		Action_1__ctor_m5798433B5A0855FC0015630BF99C272AF00F3D75(L_88, __this, (intptr_t)((void*)ScreenRouter_U3CBuildInteractionLayerU3Eb__88_13_m781EBBF8CB1FD051F753556FBFFF44215EACB59D_RuntimeMethod_var), NULL);
		Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* L_89 = (Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07*)il2cpp_codegen_object_new(Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07_il2cpp_TypeInfo_var);
		Action__ctor_mBDC7B0B4A3F583B64C2896F01BDED360772F67DC(L_89, __this, (intptr_t)((void*)ScreenRouter_NavigateBack_m8755B6CD7EDC8A0A4E6892C2EC74A718571E232B_RuntimeMethod_var), NULL);
		Action_1_t3CB5D1A819C3ED3F99E9E39F890F18633253949A* L_90 = (Action_1_t3CB5D1A819C3ED3F99E9E39F890F18633253949A*)il2cpp_codegen_object_new(Action_1_t3CB5D1A819C3ED3F99E9E39F890F18633253949A_il2cpp_TypeInfo_var);
		Action_1__ctor_m9DC2953C55C4D7D4B7BEFE03D84DA1F9362D652C(L_90, __this, (intptr_t)((void*)ScreenRouter_U3CBuildInteractionLayerU3Eb__88_14_m932AD31845A63E6685F0D16BF47CAC6A912C2041_RuntimeMethod_var), NULL);
		NullCheck(L_87);
		ProductionShopView_Bind_mCCC6F354F7A48C9A79B757E6D288456D0033BCBD(L_87, L_88, L_89, L_90, NULL);
		return;
	}

IL_0329:
	{
		InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB* L_91 = (InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB*)il2cpp_codegen_object_new(((RuntimeClass*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB_il2cpp_TypeInfo_var)));
		InvalidOperationException__ctor_mE4CB6F4712AB6D99A2358FBAE2E052B3EE976162(L_91, ((String_t*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&_stringLiteralD076113CEE62AE8CB3E3D2B951B576E17E030C2D)), NULL);
		IL2CPP_RAISE_MANAGED_EXCEPTION(L_91, ((RuntimeMethod*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&ScreenRouter_BuildInteractionLayer_m098546A1A74D7C435B6EAE0086CE1D53AD4D93EF_RuntimeMethod_var)));
	}

IL_0334:
	{
		RectTransform_t6C5DA5E41A89E0F488B001E45E58963480E543A5* L_92 = ___1_parent;
		NullCheck(L_92);
		ProductionDailyRewardView_tD3D86E84BE0D44D4F0F4963448D4CB68094FD0A9* L_93;
		L_93 = Component_GetComponentInChildren_TisProductionDailyRewardView_tD3D86E84BE0D44D4F0F4963448D4CB68094FD0A9_mD08648BCC4B6467F143C3305D46818BE4EFD4F6F(L_92, Component_GetComponentInChildren_TisProductionDailyRewardView_tD3D86E84BE0D44D4F0F4963448D4CB68094FD0A9_mD08648BCC4B6467F143C3305D46818BE4EFD4F6F_RuntimeMethod_var);
		V_8 = L_93;
		ProductionDailyRewardView_tD3D86E84BE0D44D4F0F4963448D4CB68094FD0A9* L_94 = V_8;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_95;
		L_95 = Object_op_Equality_mB6120F782D83091EF56A198FCEBCF066DB4A9605(L_94, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (L_95)
		{
			goto IL_034f;
		}
	}
	{
		ProductionDailyRewardView_tD3D86E84BE0D44D4F0F4963448D4CB68094FD0A9* L_96 = V_8;
		NullCheck(L_96);
		bool L_97;
		L_97 = ProductionDailyRewardView_get_HasRequiredBindings_m7399580C50D3895C904D67B47D55B28F279B7698(L_96, NULL);
		if (L_97)
		{
			goto IL_035a;
		}
	}

IL_034f:
	{
		InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB* L_98 = (InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB*)il2cpp_codegen_object_new(((RuntimeClass*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB_il2cpp_TypeInfo_var)));
		InvalidOperationException__ctor_mE4CB6F4712AB6D99A2358FBAE2E052B3EE976162(L_98, ((String_t*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&_stringLiteralF7612F3B3BD60973597FCA4B9147F74CA7D01379)), NULL);
		IL2CPP_RAISE_MANAGED_EXCEPTION(L_98, ((RuntimeMethod*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&ScreenRouter_BuildInteractionLayer_m098546A1A74D7C435B6EAE0086CE1D53AD4D93EF_RuntimeMethod_var)));
	}

IL_035a:
	{
		ProductionDailyRewardView_tD3D86E84BE0D44D4F0F4963448D4CB68094FD0A9* L_99 = V_8;
		Func_1_t2BE7F58348C9CC544A8973B3A9E55541DE43C457* L_100 = (Func_1_t2BE7F58348C9CC544A8973B3A9E55541DE43C457*)il2cpp_codegen_object_new(Func_1_t2BE7F58348C9CC544A8973B3A9E55541DE43C457_il2cpp_TypeInfo_var);
		Func_1__ctor_mDFFAE9C73346372438B5B04C4558AC42F1A3DA22(L_100, __this, (intptr_t)((void*)ScreenRouter_ClaimDailyRewardWithReceipt_mCEA3FE3252F39792C39F24CBC76E16CD6A7C0283_RuntimeMethod_var), NULL);
		Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* L_101 = (Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07*)il2cpp_codegen_object_new(Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07_il2cpp_TypeInfo_var);
		Action__ctor_mBDC7B0B4A3F583B64C2896F01BDED360772F67DC(L_101, __this, (intptr_t)((void*)ScreenRouter_NavigateBack_m8755B6CD7EDC8A0A4E6892C2EC74A718571E232B_RuntimeMethod_var), NULL);
		Action_1_t913CB0C3CDC5C59F6B8A1A7D957A63049F5A6BCA* L_102 = (Action_1_t913CB0C3CDC5C59F6B8A1A7D957A63049F5A6BCA*)il2cpp_codegen_object_new(Action_1_t913CB0C3CDC5C59F6B8A1A7D957A63049F5A6BCA_il2cpp_TypeInfo_var);
		Action_1__ctor_m5798433B5A0855FC0015630BF99C272AF00F3D75(L_102, __this, (intptr_t)((void*)ScreenRouter_U3CBuildInteractionLayerU3Eb__88_15_m3E30443F15905AA47F5A72EF271D79F9A8E857B7_RuntimeMethod_var), NULL);
		NullCheck(L_99);
		ProductionDailyRewardView_Bind_mDDA34552E62AAF419B0CC473969191BBD9008DD4(L_99, L_100, L_101, L_102, NULL);
		return;
	}

IL_0386:
	{
		RectTransform_t6C5DA5E41A89E0F488B001E45E58963480E543A5* L_103 = ___1_parent;
		NullCheck(L_103);
		ProductionBoosterSelectionView_tC3AB07A664B7F97CBD28AB8693822704399FF79C* L_104;
		L_104 = Component_GetComponentInChildren_TisProductionBoosterSelectionView_tC3AB07A664B7F97CBD28AB8693822704399FF79C_m2CAE450D384AC463D693AA8F0C4BFBDF64009253(L_103, Component_GetComponentInChildren_TisProductionBoosterSelectionView_tC3AB07A664B7F97CBD28AB8693822704399FF79C_m2CAE450D384AC463D693AA8F0C4BFBDF64009253_RuntimeMethod_var);
		V_9 = L_104;
		ProductionBoosterSelectionView_tC3AB07A664B7F97CBD28AB8693822704399FF79C* L_105 = V_9;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_106;
		L_106 = Object_op_Equality_mB6120F782D83091EF56A198FCEBCF066DB4A9605(L_105, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (L_106)
		{
			goto IL_03a1;
		}
	}
	{
		ProductionBoosterSelectionView_tC3AB07A664B7F97CBD28AB8693822704399FF79C* L_107 = V_9;
		NullCheck(L_107);
		bool L_108;
		L_108 = ProductionBoosterSelectionView_get_HasRequiredBindings_m1CF2DA8DBC8376805CF9AAA319D5C4E998B86C45(L_107, NULL);
		if (L_108)
		{
			goto IL_03ac;
		}
	}

IL_03a1:
	{
		InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB* L_109 = (InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB*)il2cpp_codegen_object_new(((RuntimeClass*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB_il2cpp_TypeInfo_var)));
		InvalidOperationException__ctor_mE4CB6F4712AB6D99A2358FBAE2E052B3EE976162(L_109, ((String_t*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&_stringLiteral9070D4A690E72CE624303037A84F67C8A3AAEC36)), NULL);
		IL2CPP_RAISE_MANAGED_EXCEPTION(L_109, ((RuntimeMethod*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&ScreenRouter_BuildInteractionLayer_m098546A1A74D7C435B6EAE0086CE1D53AD4D93EF_RuntimeMethod_var)));
	}

IL_03ac:
	{
		ProductionBoosterSelectionView_tC3AB07A664B7F97CBD28AB8693822704399FF79C* L_110 = V_9;
		Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* L_111 = (Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07*)il2cpp_codegen_object_new(Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07_il2cpp_TypeInfo_var);
		Action__ctor_mBDC7B0B4A3F583B64C2896F01BDED360772F67DC(L_111, __this, (intptr_t)((void*)ScreenRouter_NavigateBack_m8755B6CD7EDC8A0A4E6892C2EC74A718571E232B_RuntimeMethod_var), NULL);
		Func_1_t2BE7F58348C9CC544A8973B3A9E55541DE43C457* L_112 = (Func_1_t2BE7F58348C9CC544A8973B3A9E55541DE43C457*)il2cpp_codegen_object_new(Func_1_t2BE7F58348C9CC544A8973B3A9E55541DE43C457_il2cpp_TypeInfo_var);
		Func_1__ctor_mDFFAE9C73346372438B5B04C4558AC42F1A3DA22(L_112, __this, (intptr_t)((void*)ScreenRouter_U3CBuildInteractionLayerU3Eb__88_16_mD34CE6D564B272AB4FC6D6C90E6FCCFB9879E001_RuntimeMethod_var), NULL);
		NullCheck(L_110);
		ProductionBoosterSelectionView_Bind_m3E211B29FFB790717A21804CC8BA4B014F24A58F(L_110, (bool)0, L_111, L_112, NULL);
		return;
	}

IL_03cd:
	{
		RectTransform_t6C5DA5E41A89E0F488B001E45E58963480E543A5* L_113 = ___1_parent;
		NullCheck(L_113);
		ProductionOutOfEnergyView_t9ED536371FCF21CC2E58121FFF0561349450226A* L_114;
		L_114 = Component_GetComponentInChildren_TisProductionOutOfEnergyView_t9ED536371FCF21CC2E58121FFF0561349450226A_mC085D077806717D5DB805D8AA8F462257CECEEAB(L_113, Component_GetComponentInChildren_TisProductionOutOfEnergyView_t9ED536371FCF21CC2E58121FFF0561349450226A_mC085D077806717D5DB805D8AA8F462257CECEEAB_RuntimeMethod_var);
		V_10 = L_114;
		ProductionOutOfEnergyView_t9ED536371FCF21CC2E58121FFF0561349450226A* L_115 = V_10;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_116;
		L_116 = Object_op_Equality_mB6120F782D83091EF56A198FCEBCF066DB4A9605(L_115, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (L_116)
		{
			goto IL_03e8;
		}
	}
	{
		ProductionOutOfEnergyView_t9ED536371FCF21CC2E58121FFF0561349450226A* L_117 = V_10;
		NullCheck(L_117);
		bool L_118;
		L_118 = ProductionOutOfEnergyView_get_HasRequiredBindings_mCE7847D9D1E512AAF643463FC6DB826B92025FB6(L_117, NULL);
		if (L_118)
		{
			goto IL_03f3;
		}
	}

IL_03e8:
	{
		InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB* L_119 = (InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB*)il2cpp_codegen_object_new(((RuntimeClass*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB_il2cpp_TypeInfo_var)));
		InvalidOperationException__ctor_mE4CB6F4712AB6D99A2358FBAE2E052B3EE976162(L_119, ((String_t*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&_stringLiteral38AE809D0E3ADFDDD3984A121EC59F33998F8DC1)), NULL);
		IL2CPP_RAISE_MANAGED_EXCEPTION(L_119, ((RuntimeMethod*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&ScreenRouter_BuildInteractionLayer_m098546A1A74D7C435B6EAE0086CE1D53AD4D93EF_RuntimeMethod_var)));
	}

IL_03f3:
	{
		ProductionOutOfEnergyView_t9ED536371FCF21CC2E58121FFF0561349450226A* L_120 = V_10;
		Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* L_121 = (Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07*)il2cpp_codegen_object_new(Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07_il2cpp_TypeInfo_var);
		Action__ctor_mBDC7B0B4A3F583B64C2896F01BDED360772F67DC(L_121, __this, (intptr_t)((void*)ScreenRouter_NavigateBack_m8755B6CD7EDC8A0A4E6892C2EC74A718571E232B_RuntimeMethod_var), NULL);
		Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* L_122 = (Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07*)il2cpp_codegen_object_new(Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07_il2cpp_TypeInfo_var);
		Action__ctor_mBDC7B0B4A3F583B64C2896F01BDED360772F67DC(L_122, __this, (intptr_t)((void*)ScreenRouter_U3CBuildInteractionLayerU3Eb__88_17_m0AE334A333E5C42E5725FA8FA28D5AE7EABD7182_RuntimeMethod_var), NULL);
		Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* L_123 = (Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07*)il2cpp_codegen_object_new(Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07_il2cpp_TypeInfo_var);
		Action__ctor_mBDC7B0B4A3F583B64C2896F01BDED360772F67DC(L_123, __this, (intptr_t)((void*)ScreenRouter_RequestDemoFriendEnergy_mABC8AA11E27D15CAC582554E248BEA14C1C4BE67_RuntimeMethod_var), NULL);
		Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* L_124 = (Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07*)il2cpp_codegen_object_new(Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07_il2cpp_TypeInfo_var);
		Action__ctor_mBDC7B0B4A3F583B64C2896F01BDED360772F67DC(L_124, __this, (intptr_t)((void*)ScreenRouter_RequestRewardedEnergy_m2A6CDD4D1C0A3CA058E948D1DFC2245B3353F27D_RuntimeMethod_var), NULL);
		NullCheck(L_120);
		ProductionOutOfEnergyView_Bind_mC4851677ED358B20CBBED8FDFA65EDD60BB2FA70(L_120, (bool)0, L_121, L_122, L_123, L_124, NULL);
		return;
	}

IL_042c:
	{
		RectTransform_t6C5DA5E41A89E0F488B001E45E58963480E543A5* L_125 = ___1_parent;
		NullCheck(L_125);
		ProductionRewardReceivedView_t308036C90189F05146CA7191FC9674291133EC11* L_126;
		L_126 = Component_GetComponentInChildren_TisProductionRewardReceivedView_t308036C90189F05146CA7191FC9674291133EC11_mDB789F0CDD8849D2B9F480B102EFB6ED6E316CCB(L_125, Component_GetComponentInChildren_TisProductionRewardReceivedView_t308036C90189F05146CA7191FC9674291133EC11_mDB789F0CDD8849D2B9F480B102EFB6ED6E316CCB_RuntimeMethod_var);
		V_11 = L_126;
		ProductionRewardReceivedView_t308036C90189F05146CA7191FC9674291133EC11* L_127 = V_11;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_128;
		L_128 = Object_op_Equality_mB6120F782D83091EF56A198FCEBCF066DB4A9605(L_127, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (L_128)
		{
			goto IL_0447;
		}
	}
	{
		ProductionRewardReceivedView_t308036C90189F05146CA7191FC9674291133EC11* L_129 = V_11;
		NullCheck(L_129);
		bool L_130;
		L_130 = ProductionRewardReceivedView_get_HasRequiredBindings_m5904F48704BFEB4761A6D5F758CA74607F492402(L_129, NULL);
		if (L_130)
		{
			goto IL_0452;
		}
	}

IL_0447:
	{
		InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB* L_131 = (InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB*)il2cpp_codegen_object_new(((RuntimeClass*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB_il2cpp_TypeInfo_var)));
		InvalidOperationException__ctor_mE4CB6F4712AB6D99A2358FBAE2E052B3EE976162(L_131, ((String_t*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&_stringLiteral7AADC8B71CC9CCB8C211938B0BC8D0F070A3B083)), NULL);
		IL2CPP_RAISE_MANAGED_EXCEPTION(L_131, ((RuntimeMethod*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&ScreenRouter_BuildInteractionLayer_m098546A1A74D7C435B6EAE0086CE1D53AD4D93EF_RuntimeMethod_var)));
	}

IL_0452:
	{
		ProductionRewardReceivedView_t308036C90189F05146CA7191FC9674291133EC11* L_132 = V_11;
		il2cpp_codegen_initobj((&V_31), sizeof(Nullable_1_tFF46807EF2622F15A62B26F542EC38258BA8254E));
		Nullable_1_tFF46807EF2622F15A62B26F542EC38258BA8254E L_133 = V_31;
		Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* L_134 = (Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07*)il2cpp_codegen_object_new(Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07_il2cpp_TypeInfo_var);
		Action__ctor_mBDC7B0B4A3F583B64C2896F01BDED360772F67DC(L_134, __this, (intptr_t)((void*)ScreenRouter_NavigateBack_m8755B6CD7EDC8A0A4E6892C2EC74A718571E232B_RuntimeMethod_var), NULL);
		NullCheck(L_132);
		ProductionRewardReceivedView_Bind_m586EEA7BAC623D7C089B74FD1485DAE2EE433186(L_132, L_133, 0, L_134, NULL);
		return;
	}

IL_0471:
	{
		RectTransform_t6C5DA5E41A89E0F488B001E45E58963480E543A5* L_135 = ___1_parent;
		NullCheck(L_135);
		ProductionEventsView_t60F8250E60FB6585506BBB36AB2FA5C392D130FA* L_136;
		L_136 = Component_GetComponentInChildren_TisProductionEventsView_t60F8250E60FB6585506BBB36AB2FA5C392D130FA_mF04A7124BA3D8D332303AA248881507B215636B3(L_135, Component_GetComponentInChildren_TisProductionEventsView_t60F8250E60FB6585506BBB36AB2FA5C392D130FA_mF04A7124BA3D8D332303AA248881507B215636B3_RuntimeMethod_var);
		V_12 = L_136;
		ProductionEventsView_t60F8250E60FB6585506BBB36AB2FA5C392D130FA* L_137 = V_12;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_138;
		L_138 = Object_op_Equality_mB6120F782D83091EF56A198FCEBCF066DB4A9605(L_137, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (L_138)
		{
			goto IL_048c;
		}
	}
	{
		ProductionEventsView_t60F8250E60FB6585506BBB36AB2FA5C392D130FA* L_139 = V_12;
		NullCheck(L_139);
		bool L_140;
		L_140 = ProductionEventsView_get_HasRequiredBindings_m77A71FC7A8A93CE1958F65237C0223EE67D225F5(L_139, NULL);
		if (L_140)
		{
			goto IL_0497;
		}
	}

IL_048c:
	{
		InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB* L_141 = (InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB*)il2cpp_codegen_object_new(((RuntimeClass*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB_il2cpp_TypeInfo_var)));
		InvalidOperationException__ctor_mE4CB6F4712AB6D99A2358FBAE2E052B3EE976162(L_141, ((String_t*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&_stringLiteralAB25BE29C3F84B0AE2FED4000FC93EA673577A21)), NULL);
		IL2CPP_RAISE_MANAGED_EXCEPTION(L_141, ((RuntimeMethod*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&ScreenRouter_BuildInteractionLayer_m098546A1A74D7C435B6EAE0086CE1D53AD4D93EF_RuntimeMethod_var)));
	}

IL_0497:
	{
		ProductionEventsView_t60F8250E60FB6585506BBB36AB2FA5C392D130FA* L_142 = V_12;
		Action_1_t913CB0C3CDC5C59F6B8A1A7D957A63049F5A6BCA* L_143 = (Action_1_t913CB0C3CDC5C59F6B8A1A7D957A63049F5A6BCA*)il2cpp_codegen_object_new(Action_1_t913CB0C3CDC5C59F6B8A1A7D957A63049F5A6BCA_il2cpp_TypeInfo_var);
		Action_1__ctor_m5798433B5A0855FC0015630BF99C272AF00F3D75(L_143, __this, (intptr_t)((void*)ScreenRouter_U3CBuildInteractionLayerU3Eb__88_18_m8975C4564A83CC6AD59F1D8D43C58178396BE888_RuntimeMethod_var), NULL);
		Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* L_144 = (Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07*)il2cpp_codegen_object_new(Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07_il2cpp_TypeInfo_var);
		Action__ctor_mBDC7B0B4A3F583B64C2896F01BDED360772F67DC(L_144, __this, (intptr_t)((void*)ScreenRouter_NavigateBack_m8755B6CD7EDC8A0A4E6892C2EC74A718571E232B_RuntimeMethod_var), NULL);
		NullCheck(L_142);
		ProductionEventsView_Bind_mB2F25A84E41C8716D836BBA2393DA1A5557C67CD(L_142, L_143, L_144, NULL);
		return;
	}

IL_04b7:
	{
		RectTransform_t6C5DA5E41A89E0F488B001E45E58963480E543A5* L_145 = ___1_parent;
		NullCheck(L_145);
		ProductionProfileView_tFBAE39BD387DB9B67C01A51EB40718FD7461992D* L_146;
		L_146 = Component_GetComponentInChildren_TisProductionProfileView_tFBAE39BD387DB9B67C01A51EB40718FD7461992D_m446C73FD0151B25AD1CBD36400F2A63645FE1667(L_145, Component_GetComponentInChildren_TisProductionProfileView_tFBAE39BD387DB9B67C01A51EB40718FD7461992D_m446C73FD0151B25AD1CBD36400F2A63645FE1667_RuntimeMethod_var);
		V_13 = L_146;
		ProductionProfileView_tFBAE39BD387DB9B67C01A51EB40718FD7461992D* L_147 = V_13;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_148;
		L_148 = Object_op_Equality_mB6120F782D83091EF56A198FCEBCF066DB4A9605(L_147, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (L_148)
		{
			goto IL_04d2;
		}
	}
	{
		ProductionProfileView_tFBAE39BD387DB9B67C01A51EB40718FD7461992D* L_149 = V_13;
		NullCheck(L_149);
		bool L_150;
		L_150 = ProductionProfileView_get_HasRequiredBindings_mD4FDD50B6D3717ADFCCAD52450501B6920FADE36(L_149, NULL);
		if (L_150)
		{
			goto IL_04dd;
		}
	}

IL_04d2:
	{
		InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB* L_151 = (InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB*)il2cpp_codegen_object_new(((RuntimeClass*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB_il2cpp_TypeInfo_var)));
		InvalidOperationException__ctor_mE4CB6F4712AB6D99A2358FBAE2E052B3EE976162(L_151, ((String_t*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&_stringLiteral43C13685E18771FA9394499CA128D809E2A12493)), NULL);
		IL2CPP_RAISE_MANAGED_EXCEPTION(L_151, ((RuntimeMethod*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&ScreenRouter_BuildInteractionLayer_m098546A1A74D7C435B6EAE0086CE1D53AD4D93EF_RuntimeMethod_var)));
	}

IL_04dd:
	{
		ProductionProfileView_tFBAE39BD387DB9B67C01A51EB40718FD7461992D* L_152 = V_13;
		Action_1_t913CB0C3CDC5C59F6B8A1A7D957A63049F5A6BCA* L_153 = (Action_1_t913CB0C3CDC5C59F6B8A1A7D957A63049F5A6BCA*)il2cpp_codegen_object_new(Action_1_t913CB0C3CDC5C59F6B8A1A7D957A63049F5A6BCA_il2cpp_TypeInfo_var);
		Action_1__ctor_m5798433B5A0855FC0015630BF99C272AF00F3D75(L_153, __this, (intptr_t)((void*)ScreenRouter_U3CBuildInteractionLayerU3Eb__88_19_mDE12BA841C5A1AA11043F2B7E26F280750D6E90D_RuntimeMethod_var), NULL);
		Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* L_154 = (Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07*)il2cpp_codegen_object_new(Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07_il2cpp_TypeInfo_var);
		Action__ctor_mBDC7B0B4A3F583B64C2896F01BDED360772F67DC(L_154, __this, (intptr_t)((void*)ScreenRouter_NavigateBack_m8755B6CD7EDC8A0A4E6892C2EC74A718571E232B_RuntimeMethod_var), NULL);
		Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* L_155 = (Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07*)il2cpp_codegen_object_new(Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07_il2cpp_TypeInfo_var);
		Action__ctor_mBDC7B0B4A3F583B64C2896F01BDED360772F67DC(L_155, __this, (intptr_t)((void*)ScreenRouter_OpenStoryRecap_mDCFB012178541AEA849D6187CA92D02DA4D05E59_RuntimeMethod_var), NULL);
		NullCheck(L_152);
		ProductionProfileView_Bind_m79329BE4D57D5C4FC0AFF6D6E132E051B59FA55F(L_152, L_153, L_154, L_155, NULL);
		return;
	}

IL_0509:
	{
		RectTransform_t6C5DA5E41A89E0F488B001E45E58963480E543A5* L_156 = ___1_parent;
		ScreenRouter_BindNativeSettings_mB2E797A3CC282D1570275A7A42AFD251CEB7D8D1(__this, L_156, NULL);
		return;
	}

IL_0511:
	{
		RectTransform_t6C5DA5E41A89E0F488B001E45E58963480E543A5* L_157 = ___1_parent;
		NullCheck(L_157);
		ProductionCharactersView_t80FF719EBB00992075D821990BF966D5E95577DE* L_158;
		L_158 = Component_GetComponentInChildren_TisProductionCharactersView_t80FF719EBB00992075D821990BF966D5E95577DE_mF14E8F137A064E850586F44746439D4CEDF5912B(L_157, Component_GetComponentInChildren_TisProductionCharactersView_t80FF719EBB00992075D821990BF966D5E95577DE_mF14E8F137A064E850586F44746439D4CEDF5912B_RuntimeMethod_var);
		V_14 = L_158;
		ProductionCharactersView_t80FF719EBB00992075D821990BF966D5E95577DE* L_159 = V_14;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_160;
		L_160 = Object_op_Equality_mB6120F782D83091EF56A198FCEBCF066DB4A9605(L_159, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (L_160)
		{
			goto IL_052c;
		}
	}
	{
		ProductionCharactersView_t80FF719EBB00992075D821990BF966D5E95577DE* L_161 = V_14;
		NullCheck(L_161);
		bool L_162;
		L_162 = ProductionCharactersView_get_HasRequiredBindings_m0EE996C34F33F897528C8866367DFE6D01DD84BE(L_161, NULL);
		if (L_162)
		{
			goto IL_0537;
		}
	}

IL_052c:
	{
		InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB* L_163 = (InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB*)il2cpp_codegen_object_new(((RuntimeClass*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB_il2cpp_TypeInfo_var)));
		InvalidOperationException__ctor_mE4CB6F4712AB6D99A2358FBAE2E052B3EE976162(L_163, ((String_t*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&_stringLiteralED5A9044C74411D102A4574DCEE164B7941069DD)), NULL);
		IL2CPP_RAISE_MANAGED_EXCEPTION(L_163, ((RuntimeMethod*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&ScreenRouter_BuildInteractionLayer_m098546A1A74D7C435B6EAE0086CE1D53AD4D93EF_RuntimeMethod_var)));
	}

IL_0537:
	{
		ProductionCharactersView_t80FF719EBB00992075D821990BF966D5E95577DE* L_164 = V_14;
		Action_1_t913CB0C3CDC5C59F6B8A1A7D957A63049F5A6BCA* L_165 = (Action_1_t913CB0C3CDC5C59F6B8A1A7D957A63049F5A6BCA*)il2cpp_codegen_object_new(Action_1_t913CB0C3CDC5C59F6B8A1A7D957A63049F5A6BCA_il2cpp_TypeInfo_var);
		Action_1__ctor_m5798433B5A0855FC0015630BF99C272AF00F3D75(L_165, __this, (intptr_t)((void*)ScreenRouter_U3CBuildInteractionLayerU3Eb__88_20_m37094E5176C0C2329D23BA344BD175BD9C27597B_RuntimeMethod_var), NULL);
		Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* L_166 = (Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07*)il2cpp_codegen_object_new(Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07_il2cpp_TypeInfo_var);
		Action__ctor_mBDC7B0B4A3F583B64C2896F01BDED360772F67DC(L_166, __this, (intptr_t)((void*)ScreenRouter_NavigateBack_m8755B6CD7EDC8A0A4E6892C2EC74A718571E232B_RuntimeMethod_var), NULL);
		Action_1_t3CB5D1A819C3ED3F99E9E39F890F18633253949A* L_167 = (Action_1_t3CB5D1A819C3ED3F99E9E39F890F18633253949A*)il2cpp_codegen_object_new(Action_1_t3CB5D1A819C3ED3F99E9E39F890F18633253949A_il2cpp_TypeInfo_var);
		Action_1__ctor_m9DC2953C55C4D7D4B7BEFE03D84DA1F9362D652C(L_167, __this, (intptr_t)((void*)ScreenRouter_U3CBuildInteractionLayerU3Eb__88_21_m83BF9D2F1C685E821060320F2B55C104AF8F25C4_RuntimeMethod_var), NULL);
		NullCheck(L_164);
		ProductionCharactersView_Bind_m7E3F8823CC8225B6BF90E51B11D2E95A3D678E6B(L_164, L_165, L_166, L_167, NULL);
		return;
	}

IL_0563:
	{
		RectTransform_t6C5DA5E41A89E0F488B001E45E58963480E543A5* L_168 = ___1_parent;
		NullCheck(L_168);
		ProductionMailView_t613C10918DAA8582972FFE0681462F4DAE73B966* L_169;
		L_169 = Component_GetComponentInChildren_TisProductionMailView_t613C10918DAA8582972FFE0681462F4DAE73B966_m2D5BCFC55825FE84588DD471D9FEE1177C6C658C(L_168, Component_GetComponentInChildren_TisProductionMailView_t613C10918DAA8582972FFE0681462F4DAE73B966_m2D5BCFC55825FE84588DD471D9FEE1177C6C658C_RuntimeMethod_var);
		V_15 = L_169;
		ProductionMailView_t613C10918DAA8582972FFE0681462F4DAE73B966* L_170 = V_15;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_171;
		L_171 = Object_op_Equality_mB6120F782D83091EF56A198FCEBCF066DB4A9605(L_170, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (L_171)
		{
			goto IL_057e;
		}
	}
	{
		ProductionMailView_t613C10918DAA8582972FFE0681462F4DAE73B966* L_172 = V_15;
		NullCheck(L_172);
		bool L_173;
		L_173 = ProductionMailView_get_HasRequiredBindings_m7187E42289FAD23F4F40088F5657330BB86A0C96(L_172, NULL);
		if (L_173)
		{
			goto IL_0589;
		}
	}

IL_057e:
	{
		InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB* L_174 = (InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB*)il2cpp_codegen_object_new(((RuntimeClass*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB_il2cpp_TypeInfo_var)));
		InvalidOperationException__ctor_mE4CB6F4712AB6D99A2358FBAE2E052B3EE976162(L_174, ((String_t*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&_stringLiteral2130BAC49DEAA588E1D005F0AF29AFA03A360790)), NULL);
		IL2CPP_RAISE_MANAGED_EXCEPTION(L_174, ((RuntimeMethod*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&ScreenRouter_BuildInteractionLayer_m098546A1A74D7C435B6EAE0086CE1D53AD4D93EF_RuntimeMethod_var)));
	}

IL_0589:
	{
		ProductionMailView_t613C10918DAA8582972FFE0681462F4DAE73B966* L_175 = V_15;
		Action_1_t913CB0C3CDC5C59F6B8A1A7D957A63049F5A6BCA* L_176 = (Action_1_t913CB0C3CDC5C59F6B8A1A7D957A63049F5A6BCA*)il2cpp_codegen_object_new(Action_1_t913CB0C3CDC5C59F6B8A1A7D957A63049F5A6BCA_il2cpp_TypeInfo_var);
		Action_1__ctor_m5798433B5A0855FC0015630BF99C272AF00F3D75(L_176, __this, (intptr_t)((void*)ScreenRouter_U3CBuildInteractionLayerU3Eb__88_22_mA2090FB6C63BB8A5E4FDA5A8D32BFBCCCF263D65_RuntimeMethod_var), NULL);
		Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* L_177 = (Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07*)il2cpp_codegen_object_new(Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07_il2cpp_TypeInfo_var);
		Action__ctor_mBDC7B0B4A3F583B64C2896F01BDED360772F67DC(L_177, __this, (intptr_t)((void*)ScreenRouter_NavigateBack_m8755B6CD7EDC8A0A4E6892C2EC74A718571E232B_RuntimeMethod_var), NULL);
		NullCheck(L_175);
		ProductionMailView_Bind_mA1EB35BDB91BDA5CA217ECD91A66714830127D0C(L_175, L_176, L_177, NULL);
		return;
	}

IL_05a9:
	{
		RectTransform_t6C5DA5E41A89E0F488B001E45E58963480E543A5* L_178 = ___1_parent;
		NullCheck(L_178);
		SplashTitleView_t5C9B3C703C7BCA2970A475F6B1EF763DD452FB17* L_179;
		L_179 = Component_GetComponentInChildren_TisSplashTitleView_t5C9B3C703C7BCA2970A475F6B1EF763DD452FB17_mFF739B05797AB82FC85EC4521506F415FA6D5169(L_178, (bool)1, Component_GetComponentInChildren_TisSplashTitleView_t5C9B3C703C7BCA2970A475F6B1EF763DD452FB17_mFF739B05797AB82FC85EC4521506F415FA6D5169_RuntimeMethod_var);
		V_16 = L_179;
		SplashTitleView_t5C9B3C703C7BCA2970A475F6B1EF763DD452FB17* L_180 = V_16;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_181;
		L_181 = Object_op_Equality_mB6120F782D83091EF56A198FCEBCF066DB4A9605(L_180, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (L_181)
		{
			goto IL_05c5;
		}
	}
	{
		SplashTitleView_t5C9B3C703C7BCA2970A475F6B1EF763DD452FB17* L_182 = V_16;
		NullCheck(L_182);
		bool L_183;
		L_183 = SplashTitleView_get_HasRequiredBindings_m6CF8265CDE18B453AA3A6D3F6301DF9F85152DE4(L_182, NULL);
		if (L_183)
		{
			goto IL_05d0;
		}
	}

IL_05c5:
	{
		il2cpp_codegen_runtime_class_init_inline(Debug_t8394C7EEAECA3689C2C9B9DE9C7166D73596276F_il2cpp_TypeInfo_var);
		Debug_LogError_mB00B2B4468EF3CAF041B038D840820FB84C924B2(_stringLiteral92971B9CC0D8D91320245699D154C5BF895FE8E0, NULL);
		return;
	}

IL_05d0:
	{
		SplashTitleView_t5C9B3C703C7BCA2970A475F6B1EF763DD452FB17* L_184 = V_16;
		Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* L_185 = (Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07*)il2cpp_codegen_object_new(Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07_il2cpp_TypeInfo_var);
		Action__ctor_mBDC7B0B4A3F583B64C2896F01BDED360772F67DC(L_185, __this, (intptr_t)((void*)ScreenRouter_U3CBuildInteractionLayerU3Eb__88_23_m0CFC21EF0528B2064494C19A5CA60A63A24668AE_RuntimeMethod_var), NULL);
		Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* L_186;
		L_186 = ScreenRouter_GuardSplashAction_m32ABC35578B79CAD26DA9D5AABBC4890342B3EA3(__this, L_185, NULL);
		Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* L_187 = (Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07*)il2cpp_codegen_object_new(Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07_il2cpp_TypeInfo_var);
		Action__ctor_mBDC7B0B4A3F583B64C2896F01BDED360772F67DC(L_187, __this, (intptr_t)((void*)ScreenRouter_U3CBuildInteractionLayerU3Eb__88_24_mBCF34D7B2D54BC5524B769F1CB32C1ADB008531B_RuntimeMethod_var), NULL);
		Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* L_188;
		L_188 = ScreenRouter_GuardSplashAction_m32ABC35578B79CAD26DA9D5AABBC4890342B3EA3(__this, L_187, NULL);
		Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* L_189 = (Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07*)il2cpp_codegen_object_new(Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07_il2cpp_TypeInfo_var);
		Action__ctor_mBDC7B0B4A3F583B64C2896F01BDED360772F67DC(L_189, __this, (intptr_t)((void*)ScreenRouter_U3CBuildInteractionLayerU3Eb__88_25_m5DD6413E023EC3D7F5ED481ACB9754002A328141_RuntimeMethod_var), NULL);
		Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* L_190;
		L_190 = ScreenRouter_GuardSplashAction_m32ABC35578B79CAD26DA9D5AABBC4890342B3EA3(__this, L_189, NULL);
		Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* L_191 = (Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07*)il2cpp_codegen_object_new(Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07_il2cpp_TypeInfo_var);
		Action__ctor_mBDC7B0B4A3F583B64C2896F01BDED360772F67DC(L_191, __this, (intptr_t)((void*)ScreenRouter_U3CBuildInteractionLayerU3Eb__88_26_mC213F0D218F1E6256DF66E4281AA85CE3F61BBA9_RuntimeMethod_var), NULL);
		Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* L_192;
		L_192 = ScreenRouter_GuardSplashAction_m32ABC35578B79CAD26DA9D5AABBC4890342B3EA3(__this, L_191, NULL);
		Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* L_193 = (Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07*)il2cpp_codegen_object_new(Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07_il2cpp_TypeInfo_var);
		Action__ctor_mBDC7B0B4A3F583B64C2896F01BDED360772F67DC(L_193, __this, (intptr_t)((void*)ScreenRouter_U3CBuildInteractionLayerU3Eb__88_27_m81792042B0336211A06E4A08CAD064F110ED6BE9_RuntimeMethod_var), NULL);
		Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* L_194;
		L_194 = ScreenRouter_GuardSplashAction_m32ABC35578B79CAD26DA9D5AABBC4890342B3EA3(__this, L_193, NULL);
		NullCheck(L_184);
		SplashTitleView_BindActions_mF09F5A17F27B4854E427D6EEB1FF2C6096B40505(L_184, L_186, L_188, L_190, L_192, L_194, NULL);
		return;
	}

IL_0632:
	{
		RectTransform_t6C5DA5E41A89E0F488B001E45E58963480E543A5* L_195 = ___1_parent;
		NullCheck(L_195);
		ProductionEventDetailView_t7360325479FBCA2559711C551444B3B712CF6CE5* L_196;
		L_196 = Component_GetComponentInChildren_TisProductionEventDetailView_t7360325479FBCA2559711C551444B3B712CF6CE5_mC8B301908CD50059D72FA6B292C6A4BD7A1E8E4B(L_195, Component_GetComponentInChildren_TisProductionEventDetailView_t7360325479FBCA2559711C551444B3B712CF6CE5_mC8B301908CD50059D72FA6B292C6A4BD7A1E8E4B_RuntimeMethod_var);
		V_17 = L_196;
		ProductionEventDetailView_t7360325479FBCA2559711C551444B3B712CF6CE5* L_197 = V_17;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_198;
		L_198 = Object_op_Equality_mB6120F782D83091EF56A198FCEBCF066DB4A9605(L_197, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (L_198)
		{
			goto IL_064d;
		}
	}
	{
		ProductionEventDetailView_t7360325479FBCA2559711C551444B3B712CF6CE5* L_199 = V_17;
		NullCheck(L_199);
		bool L_200;
		L_200 = ProductionEventDetailView_get_HasRequiredBindings_m47F64373D9CEF2F608E6AF65DC57D21289712D25(L_199, NULL);
		if (L_200)
		{
			goto IL_0658;
		}
	}

IL_064d:
	{
		InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB* L_201 = (InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB*)il2cpp_codegen_object_new(((RuntimeClass*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB_il2cpp_TypeInfo_var)));
		InvalidOperationException__ctor_mE4CB6F4712AB6D99A2358FBAE2E052B3EE976162(L_201, ((String_t*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&_stringLiteralBDC63061AA283469530D56F28CEF5A0D6CAA65E1)), NULL);
		IL2CPP_RAISE_MANAGED_EXCEPTION(L_201, ((RuntimeMethod*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&ScreenRouter_BuildInteractionLayer_m098546A1A74D7C435B6EAE0086CE1D53AD4D93EF_RuntimeMethod_var)));
	}

IL_0658:
	{
		ProductionEventDetailView_t7360325479FBCA2559711C551444B3B712CF6CE5* L_202 = V_17;
		Action_1_t913CB0C3CDC5C59F6B8A1A7D957A63049F5A6BCA* L_203 = (Action_1_t913CB0C3CDC5C59F6B8A1A7D957A63049F5A6BCA*)il2cpp_codegen_object_new(Action_1_t913CB0C3CDC5C59F6B8A1A7D957A63049F5A6BCA_il2cpp_TypeInfo_var);
		Action_1__ctor_m5798433B5A0855FC0015630BF99C272AF00F3D75(L_203, __this, (intptr_t)((void*)ScreenRouter_U3CBuildInteractionLayerU3Eb__88_28_mAD5CF4DA4FB28EC36A2D1D5515CB459BE4287F60_RuntimeMethod_var), NULL);
		Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* L_204 = (Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07*)il2cpp_codegen_object_new(Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07_il2cpp_TypeInfo_var);
		Action__ctor_mBDC7B0B4A3F583B64C2896F01BDED360772F67DC(L_204, __this, (intptr_t)((void*)ScreenRouter_NavigateBack_m8755B6CD7EDC8A0A4E6892C2EC74A718571E232B_RuntimeMethod_var), NULL);
		NullCheck(L_202);
		ProductionEventDetailView_Bind_mA6A360C7757206878C65130141623F0D80CC6B10(L_202, L_203, L_204, NULL);
		return;
	}

IL_0678:
	{
		RectTransform_t6C5DA5E41A89E0F488B001E45E58963480E543A5* L_205 = ___1_parent;
		NullCheck(L_205);
		ProductionIndividualRankingView_tF449BDA2B9876849864F7F78C4C57CE0BB9726B3* L_206;
		L_206 = Component_GetComponentInChildren_TisProductionIndividualRankingView_tF449BDA2B9876849864F7F78C4C57CE0BB9726B3_mB4338486B6C14D9D4616520DBB20E5A74E544E5C(L_205, Component_GetComponentInChildren_TisProductionIndividualRankingView_tF449BDA2B9876849864F7F78C4C57CE0BB9726B3_mB4338486B6C14D9D4616520DBB20E5A74E544E5C_RuntimeMethod_var);
		V_18 = L_206;
		ProductionIndividualRankingView_tF449BDA2B9876849864F7F78C4C57CE0BB9726B3* L_207 = V_18;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_208;
		L_208 = Object_op_Equality_mB6120F782D83091EF56A198FCEBCF066DB4A9605(L_207, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (L_208)
		{
			goto IL_0693;
		}
	}
	{
		ProductionIndividualRankingView_tF449BDA2B9876849864F7F78C4C57CE0BB9726B3* L_209 = V_18;
		NullCheck(L_209);
		bool L_210;
		L_210 = ProductionIndividualRankingView_get_HasRequiredBindings_mDEA80DABC59C602FCF6A8F1F3E12DAF9B5CF329B(L_209, NULL);
		if (L_210)
		{
			goto IL_069e;
		}
	}

IL_0693:
	{
		InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB* L_211 = (InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB*)il2cpp_codegen_object_new(((RuntimeClass*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB_il2cpp_TypeInfo_var)));
		InvalidOperationException__ctor_mE4CB6F4712AB6D99A2358FBAE2E052B3EE976162(L_211, ((String_t*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&_stringLiteral7BACEA1BC8D712EE91133DD31B33CC6B034460CA)), NULL);
		IL2CPP_RAISE_MANAGED_EXCEPTION(L_211, ((RuntimeMethod*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&ScreenRouter_BuildInteractionLayer_m098546A1A74D7C435B6EAE0086CE1D53AD4D93EF_RuntimeMethod_var)));
	}

IL_069e:
	{
		ProductionIndividualRankingView_tF449BDA2B9876849864F7F78C4C57CE0BB9726B3* L_212 = V_18;
		Action_1_t913CB0C3CDC5C59F6B8A1A7D957A63049F5A6BCA* L_213 = (Action_1_t913CB0C3CDC5C59F6B8A1A7D957A63049F5A6BCA*)il2cpp_codegen_object_new(Action_1_t913CB0C3CDC5C59F6B8A1A7D957A63049F5A6BCA_il2cpp_TypeInfo_var);
		Action_1__ctor_m5798433B5A0855FC0015630BF99C272AF00F3D75(L_213, __this, (intptr_t)((void*)ScreenRouter_U3CBuildInteractionLayerU3Eb__88_29_m617F7FC1396E52801EEB0C02EAE7A7FB63C3C9AB_RuntimeMethod_var), NULL);
		Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* L_214 = (Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07*)il2cpp_codegen_object_new(Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07_il2cpp_TypeInfo_var);
		Action__ctor_mBDC7B0B4A3F583B64C2896F01BDED360772F67DC(L_214, __this, (intptr_t)((void*)ScreenRouter_NavigateBack_m8755B6CD7EDC8A0A4E6892C2EC74A718571E232B_RuntimeMethod_var), NULL);
		NullCheck(L_212);
		ProductionIndividualRankingView_Bind_mA46BFDF81B72C7E8CE9AA5023A65A15C93E7EADE(L_212, L_213, L_214, NULL);
		return;
	}

IL_06be:
	{
		RectTransform_t6C5DA5E41A89E0F488B001E45E58963480E543A5* L_215 = ___1_parent;
		NullCheck(L_215);
		ProductionTeamHubView_t242CA702D2D2A557A5145C582FE6955173320992* L_216;
		L_216 = Component_GetComponentInChildren_TisProductionTeamHubView_t242CA702D2D2A557A5145C582FE6955173320992_m8CD52018D226794AAAEBCBE906B957945FFE86F1(L_215, Component_GetComponentInChildren_TisProductionTeamHubView_t242CA702D2D2A557A5145C582FE6955173320992_m8CD52018D226794AAAEBCBE906B957945FFE86F1_RuntimeMethod_var);
		V_19 = L_216;
		ProductionTeamHubView_t242CA702D2D2A557A5145C582FE6955173320992* L_217 = V_19;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_218;
		L_218 = Object_op_Equality_mB6120F782D83091EF56A198FCEBCF066DB4A9605(L_217, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (L_218)
		{
			goto IL_06d9;
		}
	}
	{
		ProductionTeamHubView_t242CA702D2D2A557A5145C582FE6955173320992* L_219 = V_19;
		NullCheck(L_219);
		bool L_220;
		L_220 = ProductionTeamHubView_get_HasRequiredBindings_m6023FFF90A9AAD37D7E3C2BE1224C7DE50B30E98(L_219, NULL);
		if (L_220)
		{
			goto IL_06e4;
		}
	}

IL_06d9:
	{
		InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB* L_221 = (InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB*)il2cpp_codegen_object_new(((RuntimeClass*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB_il2cpp_TypeInfo_var)));
		InvalidOperationException__ctor_mE4CB6F4712AB6D99A2358FBAE2E052B3EE976162(L_221, ((String_t*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&_stringLiteral3D61E679FDBF9BCBE208A3674C4FDF6867E1C487)), NULL);
		IL2CPP_RAISE_MANAGED_EXCEPTION(L_221, ((RuntimeMethod*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&ScreenRouter_BuildInteractionLayer_m098546A1A74D7C435B6EAE0086CE1D53AD4D93EF_RuntimeMethod_var)));
	}

IL_06e4:
	{
		ProductionTeamHubView_t242CA702D2D2A557A5145C582FE6955173320992* L_222 = V_19;
		Action_1_t913CB0C3CDC5C59F6B8A1A7D957A63049F5A6BCA* L_223 = (Action_1_t913CB0C3CDC5C59F6B8A1A7D957A63049F5A6BCA*)il2cpp_codegen_object_new(Action_1_t913CB0C3CDC5C59F6B8A1A7D957A63049F5A6BCA_il2cpp_TypeInfo_var);
		Action_1__ctor_m5798433B5A0855FC0015630BF99C272AF00F3D75(L_223, __this, (intptr_t)((void*)ScreenRouter_U3CBuildInteractionLayerU3Eb__88_30_m122F078C53A4C1D986DF5D07E04BDD6A29249CB9_RuntimeMethod_var), NULL);
		Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* L_224 = (Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07*)il2cpp_codegen_object_new(Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07_il2cpp_TypeInfo_var);
		Action__ctor_mBDC7B0B4A3F583B64C2896F01BDED360772F67DC(L_224, __this, (intptr_t)((void*)ScreenRouter_NavigateBack_m8755B6CD7EDC8A0A4E6892C2EC74A718571E232B_RuntimeMethod_var), NULL);
		NullCheck(L_222);
		ProductionTeamHubView_Bind_m70103FA4DA781275AD83596FFE1C8137C46AE121(L_222, L_223, L_224, NULL);
		return;
	}

IL_0704:
	{
		RectTransform_t6C5DA5E41A89E0F488B001E45E58963480E543A5* L_225 = ___1_parent;
		NullCheck(L_225);
		ProductionTeamRankingView_t6A2E723B48D2D1F086684E393EC71E7A19E82DD4* L_226;
		L_226 = Component_GetComponentInChildren_TisProductionTeamRankingView_t6A2E723B48D2D1F086684E393EC71E7A19E82DD4_mE250DA820F30C1322456DBA230F6E0E1E4B2B5C3(L_225, Component_GetComponentInChildren_TisProductionTeamRankingView_t6A2E723B48D2D1F086684E393EC71E7A19E82DD4_mE250DA820F30C1322456DBA230F6E0E1E4B2B5C3_RuntimeMethod_var);
		V_20 = L_226;
		ProductionTeamRankingView_t6A2E723B48D2D1F086684E393EC71E7A19E82DD4* L_227 = V_20;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_228;
		L_228 = Object_op_Equality_mB6120F782D83091EF56A198FCEBCF066DB4A9605(L_227, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (L_228)
		{
			goto IL_071f;
		}
	}
	{
		ProductionTeamRankingView_t6A2E723B48D2D1F086684E393EC71E7A19E82DD4* L_229 = V_20;
		NullCheck(L_229);
		bool L_230;
		L_230 = ProductionTeamRankingView_get_HasRequiredBindings_m8D5E99E33A4E4007FC4FBECC93268933F1DDF130(L_229, NULL);
		if (L_230)
		{
			goto IL_072a;
		}
	}

IL_071f:
	{
		InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB* L_231 = (InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB*)il2cpp_codegen_object_new(((RuntimeClass*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB_il2cpp_TypeInfo_var)));
		InvalidOperationException__ctor_mE4CB6F4712AB6D99A2358FBAE2E052B3EE976162(L_231, ((String_t*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&_stringLiteralF9AEE31498A2A4FF6048DBAC38B696DB85C25281)), NULL);
		IL2CPP_RAISE_MANAGED_EXCEPTION(L_231, ((RuntimeMethod*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&ScreenRouter_BuildInteractionLayer_m098546A1A74D7C435B6EAE0086CE1D53AD4D93EF_RuntimeMethod_var)));
	}

IL_072a:
	{
		ProductionTeamRankingView_t6A2E723B48D2D1F086684E393EC71E7A19E82DD4* L_232 = V_20;
		Action_1_t913CB0C3CDC5C59F6B8A1A7D957A63049F5A6BCA* L_233 = (Action_1_t913CB0C3CDC5C59F6B8A1A7D957A63049F5A6BCA*)il2cpp_codegen_object_new(Action_1_t913CB0C3CDC5C59F6B8A1A7D957A63049F5A6BCA_il2cpp_TypeInfo_var);
		Action_1__ctor_m5798433B5A0855FC0015630BF99C272AF00F3D75(L_233, __this, (intptr_t)((void*)ScreenRouter_U3CBuildInteractionLayerU3Eb__88_31_mE307A802FF8CF690234F1A28E024BF17A857F668_RuntimeMethod_var), NULL);
		Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* L_234 = (Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07*)il2cpp_codegen_object_new(Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07_il2cpp_TypeInfo_var);
		Action__ctor_mBDC7B0B4A3F583B64C2896F01BDED360772F67DC(L_234, __this, (intptr_t)((void*)ScreenRouter_NavigateBack_m8755B6CD7EDC8A0A4E6892C2EC74A718571E232B_RuntimeMethod_var), NULL);
		NullCheck(L_232);
		ProductionTeamRankingView_Bind_mD3D5BD3A8A4915FBAD53031DBC0E3DCAC7724793(L_232, L_233, L_234, NULL);
		return;
	}

IL_074a:
	{
		RectTransform_t6C5DA5E41A89E0F488B001E45E58963480E543A5* L_235 = ___1_parent;
		NullCheck(L_235);
		ProductionTeamChatView_t3876DDF380A2DA789CB223EB9E32A75578DFE07E* L_236;
		L_236 = Component_GetComponentInChildren_TisProductionTeamChatView_t3876DDF380A2DA789CB223EB9E32A75578DFE07E_mF9F7EBF80FE2E7E514B194BCAFCF940A06FA65C2(L_235, Component_GetComponentInChildren_TisProductionTeamChatView_t3876DDF380A2DA789CB223EB9E32A75578DFE07E_mF9F7EBF80FE2E7E514B194BCAFCF940A06FA65C2_RuntimeMethod_var);
		V_21 = L_236;
		ProductionTeamChatView_t3876DDF380A2DA789CB223EB9E32A75578DFE07E* L_237 = V_21;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_238;
		L_238 = Object_op_Equality_mB6120F782D83091EF56A198FCEBCF066DB4A9605(L_237, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (L_238)
		{
			goto IL_0765;
		}
	}
	{
		ProductionTeamChatView_t3876DDF380A2DA789CB223EB9E32A75578DFE07E* L_239 = V_21;
		NullCheck(L_239);
		bool L_240;
		L_240 = ProductionTeamChatView_get_HasRequiredBindings_m8849469310C93D2F8215AFFD0DC84B797DBFC0A4(L_239, NULL);
		if (L_240)
		{
			goto IL_0770;
		}
	}

IL_0765:
	{
		InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB* L_241 = (InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB*)il2cpp_codegen_object_new(((RuntimeClass*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB_il2cpp_TypeInfo_var)));
		InvalidOperationException__ctor_mE4CB6F4712AB6D99A2358FBAE2E052B3EE976162(L_241, ((String_t*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&_stringLiteralBCC323C03A19BF48351882A07BCA310BEA035D54)), NULL);
		IL2CPP_RAISE_MANAGED_EXCEPTION(L_241, ((RuntimeMethod*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&ScreenRouter_BuildInteractionLayer_m098546A1A74D7C435B6EAE0086CE1D53AD4D93EF_RuntimeMethod_var)));
	}

IL_0770:
	{
		ProductionTeamChatView_t3876DDF380A2DA789CB223EB9E32A75578DFE07E* L_242 = V_21;
		Action_1_t913CB0C3CDC5C59F6B8A1A7D957A63049F5A6BCA* L_243 = (Action_1_t913CB0C3CDC5C59F6B8A1A7D957A63049F5A6BCA*)il2cpp_codegen_object_new(Action_1_t913CB0C3CDC5C59F6B8A1A7D957A63049F5A6BCA_il2cpp_TypeInfo_var);
		Action_1__ctor_m5798433B5A0855FC0015630BF99C272AF00F3D75(L_243, __this, (intptr_t)((void*)ScreenRouter_U3CBuildInteractionLayerU3Eb__88_32_m148E2AB3CFE6B287CA1E9CFBC443D3E1417D71C2_RuntimeMethod_var), NULL);
		Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* L_244 = (Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07*)il2cpp_codegen_object_new(Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07_il2cpp_TypeInfo_var);
		Action__ctor_mBDC7B0B4A3F583B64C2896F01BDED360772F67DC(L_244, __this, (intptr_t)((void*)ScreenRouter_NavigateBack_m8755B6CD7EDC8A0A4E6892C2EC74A718571E232B_RuntimeMethod_var), NULL);
		NullCheck(L_242);
		ProductionTeamChatView_Bind_mFA72F92FBDBF04B9E2A51430C32419AC1AB0B43E(L_242, L_243, L_244, NULL);
		return;
	}

IL_0790:
	{
		RectTransform_t6C5DA5E41A89E0F488B001E45E58963480E543A5* L_245 = ___1_parent;
		NullCheck(L_245);
		ProductionFriendsListView_t568A80A4F587DE5E0D9156810474BD1B241353D1* L_246;
		L_246 = Component_GetComponentInChildren_TisProductionFriendsListView_t568A80A4F587DE5E0D9156810474BD1B241353D1_mA39DD78FA3CB7B4FC4475179DECC45F18647B69F(L_245, Component_GetComponentInChildren_TisProductionFriendsListView_t568A80A4F587DE5E0D9156810474BD1B241353D1_mA39DD78FA3CB7B4FC4475179DECC45F18647B69F_RuntimeMethod_var);
		V_22 = L_246;
		ProductionFriendsListView_t568A80A4F587DE5E0D9156810474BD1B241353D1* L_247 = V_22;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_248;
		L_248 = Object_op_Equality_mB6120F782D83091EF56A198FCEBCF066DB4A9605(L_247, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (L_248)
		{
			goto IL_07ab;
		}
	}
	{
		ProductionFriendsListView_t568A80A4F587DE5E0D9156810474BD1B241353D1* L_249 = V_22;
		NullCheck(L_249);
		bool L_250;
		L_250 = ProductionFriendsListView_get_HasRequiredBindings_m16BB6D6BA5619F5A7A67C69AB83653894040E340(L_249, NULL);
		if (L_250)
		{
			goto IL_07b6;
		}
	}

IL_07ab:
	{
		InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB* L_251 = (InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB*)il2cpp_codegen_object_new(((RuntimeClass*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB_il2cpp_TypeInfo_var)));
		InvalidOperationException__ctor_mE4CB6F4712AB6D99A2358FBAE2E052B3EE976162(L_251, ((String_t*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&_stringLiteralE790CDA62968F2B91C7535BDA4D82B46C784F6AF)), NULL);
		IL2CPP_RAISE_MANAGED_EXCEPTION(L_251, ((RuntimeMethod*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&ScreenRouter_BuildInteractionLayer_m098546A1A74D7C435B6EAE0086CE1D53AD4D93EF_RuntimeMethod_var)));
	}

IL_07b6:
	{
		ProductionFriendsListView_t568A80A4F587DE5E0D9156810474BD1B241353D1* L_252 = V_22;
		Action_1_t913CB0C3CDC5C59F6B8A1A7D957A63049F5A6BCA* L_253 = (Action_1_t913CB0C3CDC5C59F6B8A1A7D957A63049F5A6BCA*)il2cpp_codegen_object_new(Action_1_t913CB0C3CDC5C59F6B8A1A7D957A63049F5A6BCA_il2cpp_TypeInfo_var);
		Action_1__ctor_m5798433B5A0855FC0015630BF99C272AF00F3D75(L_253, __this, (intptr_t)((void*)ScreenRouter_U3CBuildInteractionLayerU3Eb__88_33_mCB1DD4DCD50EDBFAA2DC7D24ED510D96E8B56892_RuntimeMethod_var), NULL);
		Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* L_254 = (Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07*)il2cpp_codegen_object_new(Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07_il2cpp_TypeInfo_var);
		Action__ctor_mBDC7B0B4A3F583B64C2896F01BDED360772F67DC(L_254, __this, (intptr_t)((void*)ScreenRouter_NavigateBack_m8755B6CD7EDC8A0A4E6892C2EC74A718571E232B_RuntimeMethod_var), NULL);
		NullCheck(L_252);
		ProductionFriendsListView_Bind_mE38C47533D0DD6595980DD60DB06C267C91B21F9(L_252, L_253, L_254, NULL);
		return;
	}

IL_07d6:
	{
		RectTransform_t6C5DA5E41A89E0F488B001E45E58963480E543A5* L_255 = ___1_parent;
		NullCheck(L_255);
		ProductionInventoryView_tBFCB541C14E4ED40B2CB6633CE17AC0EB9D14E8E* L_256;
		L_256 = Component_GetComponentInChildren_TisProductionInventoryView_tBFCB541C14E4ED40B2CB6633CE17AC0EB9D14E8E_mCC3AAA9A5726E977612F8790700DB37795529949(L_255, Component_GetComponentInChildren_TisProductionInventoryView_tBFCB541C14E4ED40B2CB6633CE17AC0EB9D14E8E_mCC3AAA9A5726E977612F8790700DB37795529949_RuntimeMethod_var);
		V_23 = L_256;
		ProductionInventoryView_tBFCB541C14E4ED40B2CB6633CE17AC0EB9D14E8E* L_257 = V_23;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_258;
		L_258 = Object_op_Equality_mB6120F782D83091EF56A198FCEBCF066DB4A9605(L_257, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (L_258)
		{
			goto IL_07f1;
		}
	}
	{
		ProductionInventoryView_tBFCB541C14E4ED40B2CB6633CE17AC0EB9D14E8E* L_259 = V_23;
		NullCheck(L_259);
		bool L_260;
		L_260 = ProductionInventoryView_get_HasRequiredBindings_mA86EF0DA9574E2F0963703FE53A18B4D14E71FC4(L_259, NULL);
		if (L_260)
		{
			goto IL_07fc;
		}
	}

IL_07f1:
	{
		InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB* L_261 = (InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB*)il2cpp_codegen_object_new(((RuntimeClass*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB_il2cpp_TypeInfo_var)));
		InvalidOperationException__ctor_mE4CB6F4712AB6D99A2358FBAE2E052B3EE976162(L_261, ((String_t*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&_stringLiteralC1F8FD82FEAF272D946DA74AB181475828629082)), NULL);
		IL2CPP_RAISE_MANAGED_EXCEPTION(L_261, ((RuntimeMethod*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&ScreenRouter_BuildInteractionLayer_m098546A1A74D7C435B6EAE0086CE1D53AD4D93EF_RuntimeMethod_var)));
	}

IL_07fc:
	{
		ProductionInventoryView_tBFCB541C14E4ED40B2CB6633CE17AC0EB9D14E8E* L_262 = V_23;
		Action_1_t913CB0C3CDC5C59F6B8A1A7D957A63049F5A6BCA* L_263 = (Action_1_t913CB0C3CDC5C59F6B8A1A7D957A63049F5A6BCA*)il2cpp_codegen_object_new(Action_1_t913CB0C3CDC5C59F6B8A1A7D957A63049F5A6BCA_il2cpp_TypeInfo_var);
		Action_1__ctor_m5798433B5A0855FC0015630BF99C272AF00F3D75(L_263, __this, (intptr_t)((void*)ScreenRouter_U3CBuildInteractionLayerU3Eb__88_34_m995856A78BDBD84E8CD72752C42D6F150386DE13_RuntimeMethod_var), NULL);
		Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* L_264 = (Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07*)il2cpp_codegen_object_new(Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07_il2cpp_TypeInfo_var);
		Action__ctor_mBDC7B0B4A3F583B64C2896F01BDED360772F67DC(L_264, __this, (intptr_t)((void*)ScreenRouter_NavigateBack_m8755B6CD7EDC8A0A4E6892C2EC74A718571E232B_RuntimeMethod_var), NULL);
		Action_1_t3CB5D1A819C3ED3F99E9E39F890F18633253949A* L_265 = (Action_1_t3CB5D1A819C3ED3F99E9E39F890F18633253949A*)il2cpp_codegen_object_new(Action_1_t3CB5D1A819C3ED3F99E9E39F890F18633253949A_il2cpp_TypeInfo_var);
		Action_1__ctor_m9DC2953C55C4D7D4B7BEFE03D84DA1F9362D652C(L_265, __this, (intptr_t)((void*)ScreenRouter_U3CBuildInteractionLayerU3Eb__88_35_mA958203EED0B9D52953007AF6057C76B4F813F52_RuntimeMethod_var), NULL);
		NullCheck(L_262);
		ProductionInventoryView_Bind_m7E1E06FB348E55DB6B251F5933A3BA4A3643CE38(L_262, L_263, L_264, L_265, NULL);
		return;
	}

IL_0828:
	{
		RectTransform_t6C5DA5E41A89E0F488B001E45E58963480E543A5* L_266 = ___1_parent;
		NullCheck(L_266);
		ProductionSpecialOfferView_t26243C08B7058A2D21A48610B99150458CC7CDD7* L_267;
		L_267 = Component_GetComponentInChildren_TisProductionSpecialOfferView_t26243C08B7058A2D21A48610B99150458CC7CDD7_m2EB6BF8D2D72A3803150E1A35B5FA08F735A4E17(L_266, Component_GetComponentInChildren_TisProductionSpecialOfferView_t26243C08B7058A2D21A48610B99150458CC7CDD7_m2EB6BF8D2D72A3803150E1A35B5FA08F735A4E17_RuntimeMethod_var);
		V_24 = L_267;
		ProductionSpecialOfferView_t26243C08B7058A2D21A48610B99150458CC7CDD7* L_268 = V_24;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_269;
		L_269 = Object_op_Equality_mB6120F782D83091EF56A198FCEBCF066DB4A9605(L_268, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (L_269)
		{
			goto IL_0843;
		}
	}
	{
		ProductionSpecialOfferView_t26243C08B7058A2D21A48610B99150458CC7CDD7* L_270 = V_24;
		NullCheck(L_270);
		bool L_271;
		L_271 = ProductionSpecialOfferView_get_HasRequiredBindings_m86A7EB2236B47DFC48F0BEB9E576DEA8BCA50B42(L_270, NULL);
		if (L_271)
		{
			goto IL_084e;
		}
	}

IL_0843:
	{
		InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB* L_272 = (InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB*)il2cpp_codegen_object_new(((RuntimeClass*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB_il2cpp_TypeInfo_var)));
		InvalidOperationException__ctor_mE4CB6F4712AB6D99A2358FBAE2E052B3EE976162(L_272, ((String_t*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&_stringLiteralA2EE73F3580A4BBBED719A693C88CDB66C9477BC)), NULL);
		IL2CPP_RAISE_MANAGED_EXCEPTION(L_272, ((RuntimeMethod*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&ScreenRouter_BuildInteractionLayer_m098546A1A74D7C435B6EAE0086CE1D53AD4D93EF_RuntimeMethod_var)));
	}

IL_084e:
	{
		ProductionSpecialOfferView_t26243C08B7058A2D21A48610B99150458CC7CDD7* L_273 = V_24;
		Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* L_274 = (Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07*)il2cpp_codegen_object_new(Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07_il2cpp_TypeInfo_var);
		Action__ctor_mBDC7B0B4A3F583B64C2896F01BDED360772F67DC(L_274, __this, (intptr_t)((void*)ScreenRouter_NavigateBack_m8755B6CD7EDC8A0A4E6892C2EC74A718571E232B_RuntimeMethod_var), NULL);
		Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* L_275 = (Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07*)il2cpp_codegen_object_new(Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07_il2cpp_TypeInfo_var);
		Action__ctor_mBDC7B0B4A3F583B64C2896F01BDED360772F67DC(L_275, __this, (intptr_t)((void*)ScreenRouter_U3CBuildInteractionLayerU3Eb__88_36_m8D2CCC60D4EB13CE35ABC7D7B1828868E0C033A3_RuntimeMethod_var), NULL);
		Action_1_t3CB5D1A819C3ED3F99E9E39F890F18633253949A* L_276 = (Action_1_t3CB5D1A819C3ED3F99E9E39F890F18633253949A*)il2cpp_codegen_object_new(Action_1_t3CB5D1A819C3ED3F99E9E39F890F18633253949A_il2cpp_TypeInfo_var);
		Action_1__ctor_m9DC2953C55C4D7D4B7BEFE03D84DA1F9362D652C(L_276, __this, (intptr_t)((void*)ScreenRouter_U3CBuildInteractionLayerU3Eb__88_37_m434EAD46B6124BC03A8F2D2FF11390B37AECFF57_RuntimeMethod_var), NULL);
		NullCheck(L_273);
		ProductionSpecialOfferView_Bind_mA90673C0E923C37E0031C754F5AE61C2DD75A3A1(L_273, L_274, L_275, L_276, NULL);
		return;
	}

IL_087a:
	{
		RectTransform_t6C5DA5E41A89E0F488B001E45E58963480E543A5* L_277 = ___1_parent;
		NullCheck(L_277);
		ProductionLanguageSelectView_tC9B9BF5C49A8106E900C4E16F3C587BAB476E212* L_278;
		L_278 = Component_GetComponentInChildren_TisProductionLanguageSelectView_tC9B9BF5C49A8106E900C4E16F3C587BAB476E212_mC482E7021328C34544943E03EDFAB17A75A453BF(L_277, Component_GetComponentInChildren_TisProductionLanguageSelectView_tC9B9BF5C49A8106E900C4E16F3C587BAB476E212_mC482E7021328C34544943E03EDFAB17A75A453BF_RuntimeMethod_var);
		V_25 = L_278;
		ProductionLanguageSelectView_tC9B9BF5C49A8106E900C4E16F3C587BAB476E212* L_279 = V_25;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_280;
		L_280 = Object_op_Equality_mB6120F782D83091EF56A198FCEBCF066DB4A9605(L_279, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (L_280)
		{
			goto IL_0895;
		}
	}
	{
		ProductionLanguageSelectView_tC9B9BF5C49A8106E900C4E16F3C587BAB476E212* L_281 = V_25;
		NullCheck(L_281);
		bool L_282;
		L_282 = ProductionLanguageSelectView_get_HasRequiredBindings_mEBDF2CB9EAB2D6BD527490446CF4916667597C2D(L_281, NULL);
		if (L_282)
		{
			goto IL_08a0;
		}
	}

IL_0895:
	{
		InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB* L_283 = (InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB*)il2cpp_codegen_object_new(((RuntimeClass*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB_il2cpp_TypeInfo_var)));
		InvalidOperationException__ctor_mE4CB6F4712AB6D99A2358FBAE2E052B3EE976162(L_283, ((String_t*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&_stringLiteralBA9872336A3E9C4D6C4CDDD8BAF66CF20F64359D)), NULL);
		IL2CPP_RAISE_MANAGED_EXCEPTION(L_283, ((RuntimeMethod*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&ScreenRouter_BuildInteractionLayer_m098546A1A74D7C435B6EAE0086CE1D53AD4D93EF_RuntimeMethod_var)));
	}

IL_08a0:
	{
		ProductionLanguageSelectView_tC9B9BF5C49A8106E900C4E16F3C587BAB476E212* L_284 = V_25;
		Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* L_285 = (Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07*)il2cpp_codegen_object_new(Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07_il2cpp_TypeInfo_var);
		Action__ctor_mBDC7B0B4A3F583B64C2896F01BDED360772F67DC(L_285, __this, (intptr_t)((void*)ScreenRouter_NavigateBack_m8755B6CD7EDC8A0A4E6892C2EC74A718571E232B_RuntimeMethod_var), NULL);
		NullCheck(L_284);
		ProductionLanguageSelectView_Bind_m147CEB21339F892BAC0AFBA2B222373B4CBF52F2(L_284, L_285, NULL);
		return;
	}

IL_08b4:
	{
		RectTransform_t6C5DA5E41A89E0F488B001E45E58963480E543A5* L_286 = ___1_parent;
		NullCheck(L_286);
		ProductionLoginView_t36729B7F5B697B3828DE80A1A0EB2509C7F66B81* L_287;
		L_287 = Component_GetComponentInChildren_TisProductionLoginView_t36729B7F5B697B3828DE80A1A0EB2509C7F66B81_mB364C4DEC9E8A2A8378CB63891CFE9A294BDE258(L_286, Component_GetComponentInChildren_TisProductionLoginView_t36729B7F5B697B3828DE80A1A0EB2509C7F66B81_mB364C4DEC9E8A2A8378CB63891CFE9A294BDE258_RuntimeMethod_var);
		V_26 = L_287;
		ProductionLoginView_t36729B7F5B697B3828DE80A1A0EB2509C7F66B81* L_288 = V_26;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_289;
		L_289 = Object_op_Equality_mB6120F782D83091EF56A198FCEBCF066DB4A9605(L_288, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (L_289)
		{
			goto IL_08cf;
		}
	}
	{
		ProductionLoginView_t36729B7F5B697B3828DE80A1A0EB2509C7F66B81* L_290 = V_26;
		NullCheck(L_290);
		bool L_291;
		L_291 = ProductionLoginView_get_HasRequiredBindings_mA03C54A18823A10B93ADC0EF9D3A69F0C79A9404(L_290, NULL);
		if (L_291)
		{
			goto IL_08da;
		}
	}

IL_08cf:
	{
		InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB* L_292 = (InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB*)il2cpp_codegen_object_new(((RuntimeClass*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB_il2cpp_TypeInfo_var)));
		InvalidOperationException__ctor_mE4CB6F4712AB6D99A2358FBAE2E052B3EE976162(L_292, ((String_t*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&_stringLiteral0B516F9EB595E2D91FB4E375423185098DD15038)), NULL);
		IL2CPP_RAISE_MANAGED_EXCEPTION(L_292, ((RuntimeMethod*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&ScreenRouter_BuildInteractionLayer_m098546A1A74D7C435B6EAE0086CE1D53AD4D93EF_RuntimeMethod_var)));
	}

IL_08da:
	{
		ProductionLoginView_t36729B7F5B697B3828DE80A1A0EB2509C7F66B81* L_293 = V_26;
		Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* L_294 = (Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07*)il2cpp_codegen_object_new(Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07_il2cpp_TypeInfo_var);
		Action__ctor_mBDC7B0B4A3F583B64C2896F01BDED360772F67DC(L_294, __this, (intptr_t)((void*)ScreenRouter_ContinueAsDemoGuest_mA3B54A9162ADA3DCA8020BD7F2A6494277A79140_RuntimeMethod_var), NULL);
		Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* L_295 = (Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07*)il2cpp_codegen_object_new(Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07_il2cpp_TypeInfo_var);
		Action__ctor_mBDC7B0B4A3F583B64C2896F01BDED360772F67DC(L_295, __this, (intptr_t)((void*)ScreenRouter_NavigateBack_m8755B6CD7EDC8A0A4E6892C2EC74A718571E232B_RuntimeMethod_var), NULL);
		NullCheck(L_293);
		ProductionLoginView_Bind_m396120ECACFB89BB21444CA9FB5C8DD267D361AF(L_293, L_294, L_295, NULL);
		return;
	}

IL_08fa:
	{
		RectTransform_t6C5DA5E41A89E0F488B001E45E58963480E543A5* L_296 = ___1_parent;
		NullCheck(L_296);
		ProductionTutorialView_t17A2D559FD4E71F841E57E107C3B8FADDC3DA4CB* L_297;
		L_297 = Component_GetComponentInChildren_TisProductionTutorialView_t17A2D559FD4E71F841E57E107C3B8FADDC3DA4CB_m47D948D0758AF603AAE420B79DA907127ED01965(L_296, Component_GetComponentInChildren_TisProductionTutorialView_t17A2D559FD4E71F841E57E107C3B8FADDC3DA4CB_m47D948D0758AF603AAE420B79DA907127ED01965_RuntimeMethod_var);
		V_27 = L_297;
		ProductionTutorialView_t17A2D559FD4E71F841E57E107C3B8FADDC3DA4CB* L_298 = V_27;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_299;
		L_299 = Object_op_Equality_mB6120F782D83091EF56A198FCEBCF066DB4A9605(L_298, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (L_299)
		{
			goto IL_0915;
		}
	}
	{
		ProductionTutorialView_t17A2D559FD4E71F841E57E107C3B8FADDC3DA4CB* L_300 = V_27;
		NullCheck(L_300);
		bool L_301;
		L_301 = ProductionTutorialView_get_HasRequiredBindings_m5767DF56AFE5B2DDDD3E4DA6650C22C71D5BA1B1(L_300, NULL);
		if (L_301)
		{
			goto IL_0920;
		}
	}

IL_0915:
	{
		InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB* L_302 = (InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB*)il2cpp_codegen_object_new(((RuntimeClass*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB_il2cpp_TypeInfo_var)));
		InvalidOperationException__ctor_mE4CB6F4712AB6D99A2358FBAE2E052B3EE976162(L_302, ((String_t*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&_stringLiteral1832313EA54BFA396EDBFC5A10C754EC0A1FC03F)), NULL);
		IL2CPP_RAISE_MANAGED_EXCEPTION(L_302, ((RuntimeMethod*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&ScreenRouter_BuildInteractionLayer_m098546A1A74D7C435B6EAE0086CE1D53AD4D93EF_RuntimeMethod_var)));
	}

IL_0920:
	{
		ProductionTutorialView_t17A2D559FD4E71F841E57E107C3B8FADDC3DA4CB* L_303 = V_27;
		Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* L_304 = (Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07*)il2cpp_codegen_object_new(Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07_il2cpp_TypeInfo_var);
		Action__ctor_mBDC7B0B4A3F583B64C2896F01BDED360772F67DC(L_304, __this, (intptr_t)((void*)ScreenRouter_NavigateBack_m8755B6CD7EDC8A0A4E6892C2EC74A718571E232B_RuntimeMethod_var), NULL);
		Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* L_305 = (Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07*)il2cpp_codegen_object_new(Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07_il2cpp_TypeInfo_var);
		Action__ctor_mBDC7B0B4A3F583B64C2896F01BDED360772F67DC(L_305, __this, (intptr_t)((void*)ScreenRouter_U3CBuildInteractionLayerU3Eb__88_38_mF70039B16CBA1ED4FDEEC3A9FCDA387A7D28BE7E_RuntimeMethod_var), NULL);
		Action_1_t913CB0C3CDC5C59F6B8A1A7D957A63049F5A6BCA* L_306 = (Action_1_t913CB0C3CDC5C59F6B8A1A7D957A63049F5A6BCA*)il2cpp_codegen_object_new(Action_1_t913CB0C3CDC5C59F6B8A1A7D957A63049F5A6BCA_il2cpp_TypeInfo_var);
		Action_1__ctor_m5798433B5A0855FC0015630BF99C272AF00F3D75(L_306, __this, (intptr_t)((void*)ScreenRouter_U3CBuildInteractionLayerU3Eb__88_39_m939436B6CD4A5D70B7C67D8CB28F50ACD7E1DC68_RuntimeMethod_var), NULL);
		Action_1_t3CB5D1A819C3ED3F99E9E39F890F18633253949A* L_307 = (Action_1_t3CB5D1A819C3ED3F99E9E39F890F18633253949A*)il2cpp_codegen_object_new(Action_1_t3CB5D1A819C3ED3F99E9E39F890F18633253949A_il2cpp_TypeInfo_var);
		Action_1__ctor_m9DC2953C55C4D7D4B7BEFE03D84DA1F9362D652C(L_307, __this, (intptr_t)((void*)ScreenRouter_U3CBuildInteractionLayerU3Eb__88_40_m4A3787528B1FF40CACA655619C782037610C01FD_RuntimeMethod_var), NULL);
		NullCheck(L_303);
		ProductionTutorialView_Bind_m2DA7ECEF1A5FE9DD97B8D5D2C7D9034EEAA30E3D(L_303, L_304, L_305, L_306, (bool)0, L_307, NULL);
		return;
	}

IL_0959:
	{
		RectTransform_t6C5DA5E41A89E0F488B001E45E58963480E543A5* L_308 = ___1_parent;
		NullCheck(L_308);
		ProductionOfflineView_t2A35DC0266DAE631DB63C259E5095E1F51E46451* L_309;
		L_309 = Component_GetComponentInChildren_TisProductionOfflineView_t2A35DC0266DAE631DB63C259E5095E1F51E46451_mDE12F1909962FD311F5F0F62E78371673DCA0E59(L_308, Component_GetComponentInChildren_TisProductionOfflineView_t2A35DC0266DAE631DB63C259E5095E1F51E46451_mDE12F1909962FD311F5F0F62E78371673DCA0E59_RuntimeMethod_var);
		V_28 = L_309;
		ProductionOfflineView_t2A35DC0266DAE631DB63C259E5095E1F51E46451* L_310 = V_28;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_311;
		L_311 = Object_op_Equality_mB6120F782D83091EF56A198FCEBCF066DB4A9605(L_310, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (L_311)
		{
			goto IL_0974;
		}
	}
	{
		ProductionOfflineView_t2A35DC0266DAE631DB63C259E5095E1F51E46451* L_312 = V_28;
		NullCheck(L_312);
		bool L_313;
		L_313 = ProductionOfflineView_get_HasRequiredBindings_mDA400CD3BB0253E47063A47ED90CB6276E539162(L_312, NULL);
		if (L_313)
		{
			goto IL_097f;
		}
	}

IL_0974:
	{
		InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB* L_314 = (InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB*)il2cpp_codegen_object_new(((RuntimeClass*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB_il2cpp_TypeInfo_var)));
		InvalidOperationException__ctor_mE4CB6F4712AB6D99A2358FBAE2E052B3EE976162(L_314, ((String_t*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&_stringLiteral78ABF3DDB4BEF97A92F5862AF61D27E9DD47E7B5)), NULL);
		IL2CPP_RAISE_MANAGED_EXCEPTION(L_314, ((RuntimeMethod*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&ScreenRouter_BuildInteractionLayer_m098546A1A74D7C435B6EAE0086CE1D53AD4D93EF_RuntimeMethod_var)));
	}

IL_097f:
	{
		ProductionOfflineView_t2A35DC0266DAE631DB63C259E5095E1F51E46451* L_315 = V_28;
		Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* L_316 = (Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07*)il2cpp_codegen_object_new(Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07_il2cpp_TypeInfo_var);
		Action__ctor_mBDC7B0B4A3F583B64C2896F01BDED360772F67DC(L_316, __this, (intptr_t)((void*)ScreenRouter_NavigateBack_m8755B6CD7EDC8A0A4E6892C2EC74A718571E232B_RuntimeMethod_var), NULL);
		Action_1_t913CB0C3CDC5C59F6B8A1A7D957A63049F5A6BCA* L_317 = (Action_1_t913CB0C3CDC5C59F6B8A1A7D957A63049F5A6BCA*)il2cpp_codegen_object_new(Action_1_t913CB0C3CDC5C59F6B8A1A7D957A63049F5A6BCA_il2cpp_TypeInfo_var);
		Action_1__ctor_m5798433B5A0855FC0015630BF99C272AF00F3D75(L_317, __this, (intptr_t)((void*)ScreenRouter_U3CBuildInteractionLayerU3Eb__88_41_mFEEE748300CDDF8BAED238C62E71437FBEEABB86_RuntimeMethod_var), NULL);
		Action_1_t3CB5D1A819C3ED3F99E9E39F890F18633253949A* L_318 = (Action_1_t3CB5D1A819C3ED3F99E9E39F890F18633253949A*)il2cpp_codegen_object_new(Action_1_t3CB5D1A819C3ED3F99E9E39F890F18633253949A_il2cpp_TypeInfo_var);
		Action_1__ctor_m9DC2953C55C4D7D4B7BEFE03D84DA1F9362D652C(L_318, __this, (intptr_t)((void*)ScreenRouter_U3CBuildInteractionLayerU3Eb__88_42_m745883A650D6D39796F9BDB1F36147A3B9C114A7_RuntimeMethod_var), NULL);
		NullCheck(L_315);
		ProductionOfflineView_Bind_mE99EE07935CEDDA7CA4930D3FDB78547E92A4BB1(L_315, L_316, (Func_1_tF8D036A4906DE44ED371866B42F5C8835105FA50*)NULL, L_317, L_318, NULL);
		return;
	}

IL_09ac:
	{
		RectTransform_t6C5DA5E41A89E0F488B001E45E58963480E543A5* L_319 = ___1_parent;
		NullCheck(L_319);
		ProductionMaintenanceView_tE78AA4EB7FC8F41C25AF278B1341DAA6B8F091A1* L_320;
		L_320 = Component_GetComponentInChildren_TisProductionMaintenanceView_tE78AA4EB7FC8F41C25AF278B1341DAA6B8F091A1_mBC7AFB6C8BAA17700B77BCEF2981DD586B8F25D0(L_319, Component_GetComponentInChildren_TisProductionMaintenanceView_tE78AA4EB7FC8F41C25AF278B1341DAA6B8F091A1_mBC7AFB6C8BAA17700B77BCEF2981DD586B8F25D0_RuntimeMethod_var);
		V_29 = L_320;
		ProductionMaintenanceView_tE78AA4EB7FC8F41C25AF278B1341DAA6B8F091A1* L_321 = V_29;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_322;
		L_322 = Object_op_Equality_mB6120F782D83091EF56A198FCEBCF066DB4A9605(L_321, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (L_322)
		{
			goto IL_09c7;
		}
	}
	{
		ProductionMaintenanceView_tE78AA4EB7FC8F41C25AF278B1341DAA6B8F091A1* L_323 = V_29;
		NullCheck(L_323);
		bool L_324;
		L_324 = ProductionMaintenanceView_get_HasRequiredBindings_mB172155797E34F4455D0747517F55B2E54840D5E(L_323, NULL);
		if (L_324)
		{
			goto IL_09d2;
		}
	}

IL_09c7:
	{
		InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB* L_325 = (InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB*)il2cpp_codegen_object_new(((RuntimeClass*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB_il2cpp_TypeInfo_var)));
		InvalidOperationException__ctor_mE4CB6F4712AB6D99A2358FBAE2E052B3EE976162(L_325, ((String_t*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&_stringLiteral3602993764A5F6008EFF15EC5913D112032ACC51)), NULL);
		IL2CPP_RAISE_MANAGED_EXCEPTION(L_325, ((RuntimeMethod*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&ScreenRouter_BuildInteractionLayer_m098546A1A74D7C435B6EAE0086CE1D53AD4D93EF_RuntimeMethod_var)));
	}

IL_09d2:
	{
		ProductionMaintenanceView_tE78AA4EB7FC8F41C25AF278B1341DAA6B8F091A1* L_326 = V_29;
		Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* L_327 = (Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07*)il2cpp_codegen_object_new(Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07_il2cpp_TypeInfo_var);
		Action__ctor_mBDC7B0B4A3F583B64C2896F01BDED360772F67DC(L_327, __this, (intptr_t)((void*)ScreenRouter_NavigateBack_m8755B6CD7EDC8A0A4E6892C2EC74A718571E232B_RuntimeMethod_var), NULL);
		NullCheck(L_326);
		ProductionMaintenanceView_Bind_m9C90258D69486D14F3F15A78BEF1543C4FE45314(L_326, L_327, NULL);
		return;
	}

IL_09e6:
	{
		RectTransform_t6C5DA5E41A89E0F488B001E45E58963480E543A5* L_328 = ___1_parent;
		NullCheck(L_328);
		ProductionChapterCompleteView_t8F3C64B99DD107C039783DF1D0A39530DDAA9F70* L_329;
		L_329 = Component_GetComponentInChildren_TisProductionChapterCompleteView_t8F3C64B99DD107C039783DF1D0A39530DDAA9F70_mBAF44037DDF939EBA0D1038A72DB9B112B50FC73(L_328, Component_GetComponentInChildren_TisProductionChapterCompleteView_t8F3C64B99DD107C039783DF1D0A39530DDAA9F70_mBAF44037DDF939EBA0D1038A72DB9B112B50FC73_RuntimeMethod_var);
		V_30 = L_329;
		ProductionChapterCompleteView_t8F3C64B99DD107C039783DF1D0A39530DDAA9F70* L_330 = V_30;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_331;
		L_331 = Object_op_Equality_mB6120F782D83091EF56A198FCEBCF066DB4A9605(L_330, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (L_331)
		{
			goto IL_0a01;
		}
	}
	{
		ProductionChapterCompleteView_t8F3C64B99DD107C039783DF1D0A39530DDAA9F70* L_332 = V_30;
		NullCheck(L_332);
		bool L_333;
		L_333 = ProductionChapterCompleteView_get_HasRequiredBindings_m05B6A19C59BC128908338CF75C4317579F8C8E2F(L_332, NULL);
		if (L_333)
		{
			goto IL_0a0c;
		}
	}

IL_0a01:
	{
		InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB* L_334 = (InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB*)il2cpp_codegen_object_new(((RuntimeClass*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB_il2cpp_TypeInfo_var)));
		InvalidOperationException__ctor_mE4CB6F4712AB6D99A2358FBAE2E052B3EE976162(L_334, ((String_t*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&_stringLiteralA07BB88AD0A406559B0BF50D446F7734E19B6339)), NULL);
		IL2CPP_RAISE_MANAGED_EXCEPTION(L_334, ((RuntimeMethod*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&ScreenRouter_BuildInteractionLayer_m098546A1A74D7C435B6EAE0086CE1D53AD4D93EF_RuntimeMethod_var)));
	}

IL_0a0c:
	{
		ProductionChapterCompleteView_t8F3C64B99DD107C039783DF1D0A39530DDAA9F70* L_335 = V_30;
		Func_1_t2BE7F58348C9CC544A8973B3A9E55541DE43C457* L_336 = (Func_1_t2BE7F58348C9CC544A8973B3A9E55541DE43C457*)il2cpp_codegen_object_new(Func_1_t2BE7F58348C9CC544A8973B3A9E55541DE43C457_il2cpp_TypeInfo_var);
		Func_1__ctor_mDFFAE9C73346372438B5B04C4558AC42F1A3DA22(L_336, __this, (intptr_t)((void*)ScreenRouter_U3CBuildInteractionLayerU3Eb__88_43_m12DECF666ECBCEC1BFD5CCC0833D8C65143D6FAE_RuntimeMethod_var), NULL);
		NullCheck(L_335);
		ProductionChapterCompleteView_Bind_mCE68E40BB0CA52850D400F0F3C51723F3B7E6D37(L_335, L_336, NULL);
		return;
	}

IL_0a20:
	{
		RectTransform_t6C5DA5E41A89E0F488B001E45E58963480E543A5* L_337 = ___1_parent;
		Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* L_338 = (Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07*)il2cpp_codegen_object_new(Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07_il2cpp_TypeInfo_var);
		Action__ctor_mBDC7B0B4A3F583B64C2896F01BDED360772F67DC(L_338, __this, (intptr_t)((void*)ScreenRouter_U3CBuildInteractionLayerU3Eb__88_44_mCD43168293167FB3231BAD872F1642810D22D85D_RuntimeMethod_var), NULL);
		ScreenRouter_Hotspot_m6FB76C3D7739C724E2AC8A2D8A12CC960248D44D(__this, L_337, _stringLiteral4A261EB7E7319776625F5A015EA18053797E6890, (0.0f), (0.0f), (180.0f), (180.0f), L_338, 0, (bool)0, NULL);
	}

IL_0a4e:
	{
		return;
	}
}
