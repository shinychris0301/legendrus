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
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ScreenRouter_OpenSelectedLevel_m5D09F1E395BAFA1FA8463BD96E5EE28254CFA62E_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ScreenRouter_OpenStoryRecap_mDCFB012178541AEA849D6187CA92D02DA4D05E59_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ScreenRouter_RequestDemoFriendEnergy_mABC8AA11E27D15CAC582554E248BEA14C1C4BE67_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ScreenRouter_RequestRewardedDouble_m6EFD82DBC640EC55676C4E92F43F9B978EEFC737_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ScreenRouter_RequestRewardedEnergy_m2A6CDD4D1C0A3CA058E948D1DFC2245B3353F27D_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ScreenRouter_RestartCurrentLevel_m6A128993B82F46F7070A79E86AFB7E7E42D2149B_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ScreenRouter_U3CBuildInteractionLayerU3Eb__85_0_mEB80E65DFC7E171CA5BA544A39E038C75A779386_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ScreenRouter_U3CBuildInteractionLayerU3Eb__85_10_m27BF61CD62E0A263A434A2E76D3F806376C05324_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ScreenRouter_U3CBuildInteractionLayerU3Eb__85_11_mD4CB6106649C621C4512CD6317D36F871678B761_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ScreenRouter_U3CBuildInteractionLayerU3Eb__85_12_mBCFC481AE1D5FFC8EF344AB7295B996B98266D81_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ScreenRouter_U3CBuildInteractionLayerU3Eb__85_13_m5273AD925AAF6C4454E62046D3B35964D3CB55D1_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ScreenRouter_U3CBuildInteractionLayerU3Eb__85_14_mC8BDC82DB12A2297E411DA656172365262CDB3E0_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ScreenRouter_U3CBuildInteractionLayerU3Eb__85_15_mBEFB27EE475A918E5B183429A6C4AF8569D3ECA1_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ScreenRouter_U3CBuildInteractionLayerU3Eb__85_16_m2751885DD00FDB126492688F26CD9BB357A2EDD1_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ScreenRouter_U3CBuildInteractionLayerU3Eb__85_17_mB0CD8B1B53F591BFDC6287C6CBDF66B106F16019_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ScreenRouter_U3CBuildInteractionLayerU3Eb__85_18_m5F484F506A22A4E454EA4CB078BABECF0D3B4A25_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ScreenRouter_U3CBuildInteractionLayerU3Eb__85_19_m3920E5B2FB0C0D23F94B2B319D25AB084AEF99CC_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ScreenRouter_U3CBuildInteractionLayerU3Eb__85_1_mF9DF0E00659D5AC18651CF9CC3DC571AA95E2AEB_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ScreenRouter_U3CBuildInteractionLayerU3Eb__85_20_mAF8095FC8FA247F96838436F2A20E1FE6612A711_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ScreenRouter_U3CBuildInteractionLayerU3Eb__85_21_m532E692101625F4FA7018DD002C14611474898F9_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ScreenRouter_U3CBuildInteractionLayerU3Eb__85_22_mAB5810B32799CC6717BB3A0DCC2D7310C4596BDF_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ScreenRouter_U3CBuildInteractionLayerU3Eb__85_23_mA5D665893517F7816BF9BBBEE55EB831094F057B_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ScreenRouter_U3CBuildInteractionLayerU3Eb__85_24_m00C9D65468792D340A88535448504C49A3C2A7E2_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ScreenRouter_U3CBuildInteractionLayerU3Eb__85_25_m3DEF03E9B9121F07964E548223EEC0C2D698C3F8_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ScreenRouter_U3CBuildInteractionLayerU3Eb__85_26_mCB52745857DB75B08585A992C622D0CAD9470970_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ScreenRouter_U3CBuildInteractionLayerU3Eb__85_27_m9C800467121F963AE5B3A9E8F5CC2203A3C66D46_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ScreenRouter_U3CBuildInteractionLayerU3Eb__85_28_mECE239325F4BDCEFD73E8B9A2501219A00676DC0_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ScreenRouter_U3CBuildInteractionLayerU3Eb__85_29_mD6E827343FFD4309A0B2C4660BD7766B8464F240_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ScreenRouter_U3CBuildInteractionLayerU3Eb__85_2_m0805B286915672A47207598ADAF1268EAF637B1C_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ScreenRouter_U3CBuildInteractionLayerU3Eb__85_30_mA612239D6D34658F3C3C3C8C182F6FBBC690EB22_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ScreenRouter_U3CBuildInteractionLayerU3Eb__85_31_mABB31581CFEA2E3464700A1D3C38F11A85CDD90D_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ScreenRouter_U3CBuildInteractionLayerU3Eb__85_32_m7E00ECDB658A086BB754CB9F5CDC0C88AA842CD9_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ScreenRouter_U3CBuildInteractionLayerU3Eb__85_33_m3478FDC66F07FE7DEEE2F637B24CBB2C4CD79378_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ScreenRouter_U3CBuildInteractionLayerU3Eb__85_34_m0161AB5666F9B89989C6411C279527492243FCFE_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ScreenRouter_U3CBuildInteractionLayerU3Eb__85_35_mBD3B34D91CECAAA38C265B5EAD6A55B16E6DAB96_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ScreenRouter_U3CBuildInteractionLayerU3Eb__85_36_m7AE1101A2E95AC1AB5A7C839802A5BECC654794C_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ScreenRouter_U3CBuildInteractionLayerU3Eb__85_37_mAA83FAF83307F0A99B4D1E49ABD51A8F08D694F2_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ScreenRouter_U3CBuildInteractionLayerU3Eb__85_38_mD65FF4DC09B6A87D55760923472FB858CCBD9B6F_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ScreenRouter_U3CBuildInteractionLayerU3Eb__85_39_m32DB0ABB961515770DAA4EAA9AC9D195D3552E8E_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ScreenRouter_U3CBuildInteractionLayerU3Eb__85_3_m799FFA9073866D261D4095B7A530B888E53B8257_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ScreenRouter_U3CBuildInteractionLayerU3Eb__85_40_mE134B961FD87D2DB2CCE6566EC16EC65BFA5041A_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ScreenRouter_U3CBuildInteractionLayerU3Eb__85_41_mC5D0006D724BCE91BEEE26EF1A24731F63FCBCC7_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ScreenRouter_U3CBuildInteractionLayerU3Eb__85_42_m1800D31859AF9C79953636469D38789894FB3CBD_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ScreenRouter_U3CBuildInteractionLayerU3Eb__85_43_m4A3D1532E3BC1CC7B878E27621B41A074293CB6F_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ScreenRouter_U3CBuildInteractionLayerU3Eb__85_4_m3174CB25DA931B076C073973F9AD2A4CA14B0BB8_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ScreenRouter_U3CBuildInteractionLayerU3Eb__85_5_mED495B14F26D6F05A9641023DE3D058DCB490266_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ScreenRouter_U3CBuildInteractionLayerU3Eb__85_6_mB773E63EDB0AD8BA9DD53F507302CB8D25E5A462_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ScreenRouter_U3CBuildInteractionLayerU3Eb__85_7_m58DCCDCDAA88D7407184A8C23C814F8E85A3A351_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ScreenRouter_U3CBuildInteractionLayerU3Eb__85_8_m945C1A43FE03C0F9E18F024ED2F5779CCDFCAC8E_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ScreenRouter_U3CBuildInteractionLayerU3Eb__85_9_mCEB9C08DBFA04535EB8C206758505CA1E735A991_RuntimeMethod_var);
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
				goto IL_0196;
			}
			case 4:
			{
				goto IL_01f0;
			}
			case 5:
			{
				goto IL_01f8;
			}
			case 6:
			{
				goto IL_0232;
			}
			case 7:
			{
				goto IL_0284;
			}
			case 8:
			{
				goto IL_02d6;
			}
			case 9:
			{
				goto IL_0a14;
			}
			case 10:
			{
				goto IL_0328;
			}
			case 11:
			{
				goto IL_0420;
			}
			case 12:
			{
				goto IL_037a;
			}
			case 13:
			{
				goto IL_0465;
			}
			case 14:
			{
				goto IL_04ab;
			}
			case 15:
			{
				goto IL_04fd;
			}
			case 16:
			{
				goto IL_0a42;
			}
			case 17:
			{
				goto IL_0505;
			}
			case 18:
			{
				goto IL_0557;
			}
			case 19:
			{
				goto IL_03c1;
			}
			case 20:
			{
				goto IL_09da;
			}
			case 21:
			{
				goto IL_059d;
			}
			case 22:
			{
				goto IL_0626;
			}
			case 23:
			{
				goto IL_066c;
			}
			case 24:
			{
				goto IL_06b2;
			}
			case 25:
			{
				goto IL_06f8;
			}
			case 26:
			{
				goto IL_073e;
			}
			case 27:
			{
				goto IL_0784;
			}
			case 28:
			{
				goto IL_07ca;
			}
			case 29:
			{
				goto IL_081c;
			}
			case 30:
			{
				goto IL_086e;
			}
			case 31:
			{
				goto IL_08a8;
			}
			case 32:
			{
				goto IL_08ee;
			}
			case 33:
			{
				goto IL_094d;
			}
			case 34:
			{
				goto IL_09a0;
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
		Action_1__ctor_m5798433B5A0855FC0015630BF99C272AF00F3D75(L_8, __this, (intptr_t)((void*)ScreenRouter_U3CBuildInteractionLayerU3Eb__85_0_mEB80E65DFC7E171CA5BA544A39E038C75A779386_RuntimeMethod_var), NULL);
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
		Action_1__ctor_m5798433B5A0855FC0015630BF99C272AF00F3D75(L_17, __this, (intptr_t)((void*)ScreenRouter_U3CBuildInteractionLayerU3Eb__85_1_mF9DF0E00659D5AC18651CF9CC3DC571AA95E2AEB_RuntimeMethod_var), NULL);
		Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* L_18 = (Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07*)il2cpp_codegen_object_new(Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07_il2cpp_TypeInfo_var);
		Action__ctor_mBDC7B0B4A3F583B64C2896F01BDED360772F67DC(L_18, __this, (intptr_t)((void*)ScreenRouter_OpenChapterMap_mE8CA65193CAA1659F7CD1B2F5AF1810D41D9EE9B_RuntimeMethod_var), NULL);
		Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* L_19 = (Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07*)il2cpp_codegen_object_new(Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07_il2cpp_TypeInfo_var);
		Action__ctor_mBDC7B0B4A3F583B64C2896F01BDED360772F67DC(L_19, __this, (intptr_t)((void*)ScreenRouter_U3CBuildInteractionLayerU3Eb__85_2_m0805B286915672A47207598ADAF1268EAF637B1C_RuntimeMethod_var), NULL);
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
		Action_1__ctor_m5798433B5A0855FC0015630BF99C272AF00F3D75(L_30, __this, (intptr_t)((void*)ScreenRouter_U3CBuildInteractionLayerU3Eb__85_3_m799FFA9073866D261D4095B7A530B888E53B8257_RuntimeMethod_var), NULL);
		Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* L_31 = (Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07*)il2cpp_codegen_object_new(Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07_il2cpp_TypeInfo_var);
		Action__ctor_mBDC7B0B4A3F583B64C2896F01BDED360772F67DC(L_31, __this, (intptr_t)((void*)ScreenRouter_OpenSelectedLevel_m5D09F1E395BAFA1FA8463BD96E5EE28254CFA62E_RuntimeMethod_var), NULL);
		Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* L_32 = (Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07*)il2cpp_codegen_object_new(Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07_il2cpp_TypeInfo_var);
		Action__ctor_mBDC7B0B4A3F583B64C2896F01BDED360772F67DC(L_32, __this, (intptr_t)((void*)ScreenRouter_U3CBuildInteractionLayerU3Eb__85_4_m3174CB25DA931B076C073973F9AD2A4CA14B0BB8_RuntimeMethod_var), NULL);
		ProductionChapterMapView_t73FA61D194FCDDD0F89F8021A5BBFC50FD5D1075* L_33 = V_2;
		NullCheck(L_33);
		ProductionMapNotice_tDF38B2FD90624E90B58D500B0C8519AAD4662475* L_34 = L_33->___Notice;
		Action_1_t3CB5D1A819C3ED3F99E9E39F890F18633253949A* L_35 = (Action_1_t3CB5D1A819C3ED3F99E9E39F890F18633253949A*)il2cpp_codegen_object_new(Action_1_t3CB5D1A819C3ED3F99E9E39F890F18633253949A_il2cpp_TypeInfo_var);
		Action_1__ctor_m9DC2953C55C4D7D4B7BEFE03D84DA1F9362D652C(L_35, L_34, (intptr_t)((void*)ProductionMapNotice_Show_mB6EAAA8554BBD1922FC4ABE702FBF6154F9344CF_RuntimeMethod_var), NULL);
		NullCheck(L_29);
		ProductionChapterMapView_BindNavigation_mFD3328B8E09188FF10F1C0300A8B90EF773DC26B(L_29, L_30, L_31, L_32, L_35, NULL);
		return;
	}

IL_0196:
	{
		RectTransform_t6C5DA5E41A89E0F488B001E45E58963480E543A5* L_36 = ___1_parent;
		NullCheck(L_36);
		ProductionPreLevelView_t19543F1A5A95A83F99E01D12193810B0543C6BDD* L_37;
		L_37 = Component_GetComponentInChildren_TisProductionPreLevelView_t19543F1A5A95A83F99E01D12193810B0543C6BDD_m46F83BF669465C64237D11120DCB68E2117022FE(L_36, Component_GetComponentInChildren_TisProductionPreLevelView_t19543F1A5A95A83F99E01D12193810B0543C6BDD_m46F83BF669465C64237D11120DCB68E2117022FE_RuntimeMethod_var);
		V_3 = L_37;
		ProductionPreLevelView_t19543F1A5A95A83F99E01D12193810B0543C6BDD* L_38 = V_3;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_39;
		L_39 = Object_op_Equality_mB6120F782D83091EF56A198FCEBCF066DB4A9605(L_38, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (L_39)
		{
			goto IL_01ae;
		}
	}
	{
		ProductionPreLevelView_t19543F1A5A95A83F99E01D12193810B0543C6BDD* L_40 = V_3;
		NullCheck(L_40);
		bool L_41;
		L_41 = ProductionPreLevelView_get_HasRequiredBindings_m0522608AB4A2DC60F4BAAE12A8E83A24FEEB162A(L_40, NULL);
		if (L_41)
		{
			goto IL_01b9;
		}
	}

IL_01ae:
	{
		InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB* L_42 = (InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB*)il2cpp_codegen_object_new(((RuntimeClass*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB_il2cpp_TypeInfo_var)));
		InvalidOperationException__ctor_mE4CB6F4712AB6D99A2358FBAE2E052B3EE976162(L_42, ((String_t*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&_stringLiteral2AA1A1F32CBBBB36C763CFA78354AB9F5B2061CB)), NULL);
		IL2CPP_RAISE_MANAGED_EXCEPTION(L_42, ((RuntimeMethod*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&ScreenRouter_BuildInteractionLayer_m098546A1A74D7C435B6EAE0086CE1D53AD4D93EF_RuntimeMethod_var)));
	}

IL_01b9:
	{
		ProductionPreLevelView_t19543F1A5A95A83F99E01D12193810B0543C6BDD* L_43 = V_3;
		Action_1_t913CB0C3CDC5C59F6B8A1A7D957A63049F5A6BCA* L_44 = (Action_1_t913CB0C3CDC5C59F6B8A1A7D957A63049F5A6BCA*)il2cpp_codegen_object_new(Action_1_t913CB0C3CDC5C59F6B8A1A7D957A63049F5A6BCA_il2cpp_TypeInfo_var);
		Action_1__ctor_m5798433B5A0855FC0015630BF99C272AF00F3D75(L_44, __this, (intptr_t)((void*)ScreenRouter_U3CBuildInteractionLayerU3Eb__85_5_mED495B14F26D6F05A9641023DE3D058DCB490266_RuntimeMethod_var), NULL);
		Func_1_t2BE7F58348C9CC544A8973B3A9E55541DE43C457* L_45 = (Func_1_t2BE7F58348C9CC544A8973B3A9E55541DE43C457*)il2cpp_codegen_object_new(Func_1_t2BE7F58348C9CC544A8973B3A9E55541DE43C457_il2cpp_TypeInfo_var);
		Func_1__ctor_mDFFAE9C73346372438B5B04C4558AC42F1A3DA22(L_45, __this, (intptr_t)((void*)ScreenRouter_U3CBuildInteractionLayerU3Eb__85_6_mB773E63EDB0AD8BA9DD53F507302CB8D25E5A462_RuntimeMethod_var), NULL);
		Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* L_46 = (Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07*)il2cpp_codegen_object_new(Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07_il2cpp_TypeInfo_var);
		Action__ctor_mBDC7B0B4A3F583B64C2896F01BDED360772F67DC(L_46, __this, (intptr_t)((void*)ScreenRouter_U3CBuildInteractionLayerU3Eb__85_7_m58DCCDCDAA88D7407184A8C23C814F8E85A3A351_RuntimeMethod_var), NULL);
		Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* L_47 = (Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07*)il2cpp_codegen_object_new(Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07_il2cpp_TypeInfo_var);
		Action__ctor_mBDC7B0B4A3F583B64C2896F01BDED360772F67DC(L_47, __this, (intptr_t)((void*)ScreenRouter_U3CBuildInteractionLayerU3Eb__85_8_m945C1A43FE03C0F9E18F024ED2F5779CCDFCAC8E_RuntimeMethod_var), NULL);
		NullCheck(L_43);
		ProductionPreLevelView_BindNavigation_m1229ED716A00269793F7E8FAB3CAE2B8FABADBA8(L_43, L_44, L_45, L_46, L_47, NULL);
		return;
	}

IL_01f0:
	{
		RectTransform_t6C5DA5E41A89E0F488B001E45E58963480E543A5* L_48 = ___1_parent;
		ScreenRouter_BuildGameplayOverlay_m2F3676D27FEACFFE9E838EF9B5EAF70A96E2BEBD(__this, L_48, NULL);
		return;
	}

IL_01f8:
	{
		RectTransform_t6C5DA5E41A89E0F488B001E45E58963480E543A5* L_49 = ___1_parent;
		NullCheck(L_49);
		ProductionDialogueView_t9612D941765F8A0545DD0212DE1D76B0BDD0F8C5* L_50;
		L_50 = Component_GetComponentInChildren_TisProductionDialogueView_t9612D941765F8A0545DD0212DE1D76B0BDD0F8C5_m1D008ACA9283C198043BDFE9174D5580BD5136E0(L_49, Component_GetComponentInChildren_TisProductionDialogueView_t9612D941765F8A0545DD0212DE1D76B0BDD0F8C5_m1D008ACA9283C198043BDFE9174D5580BD5136E0_RuntimeMethod_var);
		V_4 = L_50;
		ProductionDialogueView_t9612D941765F8A0545DD0212DE1D76B0BDD0F8C5* L_51 = V_4;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_52;
		L_52 = Object_op_Equality_mB6120F782D83091EF56A198FCEBCF066DB4A9605(L_51, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (L_52)
		{
			goto IL_0213;
		}
	}
	{
		ProductionDialogueView_t9612D941765F8A0545DD0212DE1D76B0BDD0F8C5* L_53 = V_4;
		NullCheck(L_53);
		bool L_54;
		L_54 = ProductionDialogueView_get_HasRequiredBindings_m187105A6FA2C9649BFB183C25D6091DFDA92D019(L_53, NULL);
		if (L_54)
		{
			goto IL_021e;
		}
	}

IL_0213:
	{
		InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB* L_55 = (InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB*)il2cpp_codegen_object_new(((RuntimeClass*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB_il2cpp_TypeInfo_var)));
		InvalidOperationException__ctor_mE4CB6F4712AB6D99A2358FBAE2E052B3EE976162(L_55, ((String_t*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&_stringLiteral45D42AA4F148A0C21ADDEB41302244BA7B9714DC)), NULL);
		IL2CPP_RAISE_MANAGED_EXCEPTION(L_55, ((RuntimeMethod*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&ScreenRouter_BuildInteractionLayer_m098546A1A74D7C435B6EAE0086CE1D53AD4D93EF_RuntimeMethod_var)));
	}

IL_021e:
	{
		ProductionDialogueView_t9612D941765F8A0545DD0212DE1D76B0BDD0F8C5* L_56 = V_4;
		Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* L_57 = (Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07*)il2cpp_codegen_object_new(Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07_il2cpp_TypeInfo_var);
		Action__ctor_mBDC7B0B4A3F583B64C2896F01BDED360772F67DC(L_57, __this, (intptr_t)((void*)ScreenRouter_ContinueAfterDialogue_mF1C4512958024BF0A6D98A90E5A6BD89BE09322C_RuntimeMethod_var), NULL);
		NullCheck(L_56);
		ProductionDialogueView_Bind_m620E9F9D67FC1E7E52C0BC3DC18059732472DB9C(L_56, L_57, NULL);
		return;
	}

IL_0232:
	{
		RectTransform_t6C5DA5E41A89E0F488B001E45E58963480E543A5* L_58 = ___1_parent;
		NullCheck(L_58);
		ProductionVictoryView_t09841A4498FAE3A2C54918934DC80DBD053E851A* L_59;
		L_59 = Component_GetComponentInChildren_TisProductionVictoryView_t09841A4498FAE3A2C54918934DC80DBD053E851A_mC5D774D98C17EC0007845B4B58C751C718D28185(L_58, Component_GetComponentInChildren_TisProductionVictoryView_t09841A4498FAE3A2C54918934DC80DBD053E851A_mC5D774D98C17EC0007845B4B58C751C718D28185_RuntimeMethod_var);
		V_5 = L_59;
		ProductionVictoryView_t09841A4498FAE3A2C54918934DC80DBD053E851A* L_60 = V_5;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_61;
		L_61 = Object_op_Equality_mB6120F782D83091EF56A198FCEBCF066DB4A9605(L_60, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (L_61)
		{
			goto IL_024d;
		}
	}
	{
		ProductionVictoryView_t09841A4498FAE3A2C54918934DC80DBD053E851A* L_62 = V_5;
		NullCheck(L_62);
		bool L_63;
		L_63 = ProductionVictoryView_get_HasRequiredBindings_mEEFB0AA4B928E0A10C97B7A24FC7A793C1CE0B0F(L_62, NULL);
		if (L_63)
		{
			goto IL_0258;
		}
	}

IL_024d:
	{
		InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB* L_64 = (InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB*)il2cpp_codegen_object_new(((RuntimeClass*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB_il2cpp_TypeInfo_var)));
		InvalidOperationException__ctor_mE4CB6F4712AB6D99A2358FBAE2E052B3EE976162(L_64, ((String_t*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&_stringLiteral39DA62FE0E935F32B1D2AD40ED8D4F8CED27FF19)), NULL);
		IL2CPP_RAISE_MANAGED_EXCEPTION(L_64, ((RuntimeMethod*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&ScreenRouter_BuildInteractionLayer_m098546A1A74D7C435B6EAE0086CE1D53AD4D93EF_RuntimeMethod_var)));
	}

IL_0258:
	{
		ProductionVictoryView_t09841A4498FAE3A2C54918934DC80DBD053E851A* L_65 = V_5;
		Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* L_66 = (Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07*)il2cpp_codegen_object_new(Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07_il2cpp_TypeInfo_var);
		Action__ctor_mBDC7B0B4A3F583B64C2896F01BDED360772F67DC(L_66, __this, (intptr_t)((void*)ScreenRouter_ContinueAfterVictory_mB21EDFD516ED99A0A1C1BADA2E583F3216118EDC_RuntimeMethod_var), NULL);
		Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* L_67 = (Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07*)il2cpp_codegen_object_new(Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07_il2cpp_TypeInfo_var);
		Action__ctor_mBDC7B0B4A3F583B64C2896F01BDED360772F67DC(L_67, __this, (intptr_t)((void*)ScreenRouter_RequestRewardedDouble_m6EFD82DBC640EC55676C4E92F43F9B978EEFC737_RuntimeMethod_var), NULL);
		Action_1_t913CB0C3CDC5C59F6B8A1A7D957A63049F5A6BCA* L_68 = (Action_1_t913CB0C3CDC5C59F6B8A1A7D957A63049F5A6BCA*)il2cpp_codegen_object_new(Action_1_t913CB0C3CDC5C59F6B8A1A7D957A63049F5A6BCA_il2cpp_TypeInfo_var);
		Action_1__ctor_m5798433B5A0855FC0015630BF99C272AF00F3D75(L_68, __this, (intptr_t)((void*)ScreenRouter_U3CBuildInteractionLayerU3Eb__85_9_mCEB9C08DBFA04535EB8C206758505CA1E735A991_RuntimeMethod_var), NULL);
		NullCheck(L_65);
		ProductionVictoryView_BindNavigation_mAC3BEB83708416B2DD0EBCA34B48EFA4D9FD4A1B(L_65, L_66, L_67, L_68, NULL);
		return;
	}

IL_0284:
	{
		RectTransform_t6C5DA5E41A89E0F488B001E45E58963480E543A5* L_69 = ___1_parent;
		NullCheck(L_69);
		ProductionFailView_t5D82248F0E88B603813B2F82EE1CA191D740F251* L_70;
		L_70 = Component_GetComponentInChildren_TisProductionFailView_t5D82248F0E88B603813B2F82EE1CA191D740F251_mDE013F807A95E81C33979B237BF5F2FC2C4FBE40(L_69, Component_GetComponentInChildren_TisProductionFailView_t5D82248F0E88B603813B2F82EE1CA191D740F251_mDE013F807A95E81C33979B237BF5F2FC2C4FBE40_RuntimeMethod_var);
		V_6 = L_70;
		ProductionFailView_t5D82248F0E88B603813B2F82EE1CA191D740F251* L_71 = V_6;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_72;
		L_72 = Object_op_Equality_mB6120F782D83091EF56A198FCEBCF066DB4A9605(L_71, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (L_72)
		{
			goto IL_029f;
		}
	}
	{
		ProductionFailView_t5D82248F0E88B603813B2F82EE1CA191D740F251* L_73 = V_6;
		NullCheck(L_73);
		bool L_74;
		L_74 = ProductionFailView_get_HasRequiredBindings_m5BA63BE0534C0D1D9B2C19F14CE33B79965C2A1B(L_73, NULL);
		if (L_74)
		{
			goto IL_02aa;
		}
	}

IL_029f:
	{
		InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB* L_75 = (InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB*)il2cpp_codegen_object_new(((RuntimeClass*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB_il2cpp_TypeInfo_var)));
		InvalidOperationException__ctor_mE4CB6F4712AB6D99A2358FBAE2E052B3EE976162(L_75, ((String_t*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&_stringLiteral16CCEDFAC42DC7B76BF4A28B63A8F20D4E02EDD3)), NULL);
		IL2CPP_RAISE_MANAGED_EXCEPTION(L_75, ((RuntimeMethod*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&ScreenRouter_BuildInteractionLayer_m098546A1A74D7C435B6EAE0086CE1D53AD4D93EF_RuntimeMethod_var)));
	}

IL_02aa:
	{
		ProductionFailView_t5D82248F0E88B603813B2F82EE1CA191D740F251* L_76 = V_6;
		Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* L_77 = (Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07*)il2cpp_codegen_object_new(Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07_il2cpp_TypeInfo_var);
		Action__ctor_mBDC7B0B4A3F583B64C2896F01BDED360772F67DC(L_77, __this, (intptr_t)((void*)ScreenRouter_RestartCurrentLevel_m6A128993B82F46F7070A79E86AFB7E7E42D2149B_RuntimeMethod_var), NULL);
		Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* L_78 = (Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07*)il2cpp_codegen_object_new(Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07_il2cpp_TypeInfo_var);
		Action__ctor_mBDC7B0B4A3F583B64C2896F01BDED360772F67DC(L_78, __this, (intptr_t)((void*)ScreenRouter_U3CBuildInteractionLayerU3Eb__85_10_m27BF61CD62E0A263A434A2E76D3F806376C05324_RuntimeMethod_var), NULL);
		Action_1_t913CB0C3CDC5C59F6B8A1A7D957A63049F5A6BCA* L_79 = (Action_1_t913CB0C3CDC5C59F6B8A1A7D957A63049F5A6BCA*)il2cpp_codegen_object_new(Action_1_t913CB0C3CDC5C59F6B8A1A7D957A63049F5A6BCA_il2cpp_TypeInfo_var);
		Action_1__ctor_m5798433B5A0855FC0015630BF99C272AF00F3D75(L_79, __this, (intptr_t)((void*)ScreenRouter_U3CBuildInteractionLayerU3Eb__85_11_mD4CB6106649C621C4512CD6317D36F871678B761_RuntimeMethod_var), NULL);
		NullCheck(L_76);
		ProductionFailView_BindNavigation_m05E49889C4B4DEA00A9F6B3141282CF326EDF01C(L_76, L_77, L_78, L_79, NULL);
		return;
	}

IL_02d6:
	{
		RectTransform_t6C5DA5E41A89E0F488B001E45E58963480E543A5* L_80 = ___1_parent;
		NullCheck(L_80);
		ProductionShopView_tCAE5004D358A893BA810EE3CC9331FF6A5AAB245* L_81;
		L_81 = Component_GetComponentInChildren_TisProductionShopView_tCAE5004D358A893BA810EE3CC9331FF6A5AAB245_m2BB6613012F1FC7BFDF2D183D920E0124FB8CA63(L_80, Component_GetComponentInChildren_TisProductionShopView_tCAE5004D358A893BA810EE3CC9331FF6A5AAB245_m2BB6613012F1FC7BFDF2D183D920E0124FB8CA63_RuntimeMethod_var);
		V_7 = L_81;
		ProductionShopView_tCAE5004D358A893BA810EE3CC9331FF6A5AAB245* L_82 = V_7;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_83;
		L_83 = Object_op_Inequality_mD0BE578448EAA61948F25C32F8DD55AB1F778602(L_82, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (!L_83)
		{
			goto IL_031d;
		}
	}
	{
		ProductionShopView_tCAE5004D358A893BA810EE3CC9331FF6A5AAB245* L_84 = V_7;
		NullCheck(L_84);
		bool L_85;
		L_85 = ProductionShopView_get_HasRequiredBindings_m2E560F53850517FAC148ED7AA746524DA9D96404(L_84, NULL);
		if (!L_85)
		{
			goto IL_031d;
		}
	}
	{
		ProductionShopView_tCAE5004D358A893BA810EE3CC9331FF6A5AAB245* L_86 = V_7;
		Action_1_t913CB0C3CDC5C59F6B8A1A7D957A63049F5A6BCA* L_87 = (Action_1_t913CB0C3CDC5C59F6B8A1A7D957A63049F5A6BCA*)il2cpp_codegen_object_new(Action_1_t913CB0C3CDC5C59F6B8A1A7D957A63049F5A6BCA_il2cpp_TypeInfo_var);
		Action_1__ctor_m5798433B5A0855FC0015630BF99C272AF00F3D75(L_87, __this, (intptr_t)((void*)ScreenRouter_U3CBuildInteractionLayerU3Eb__85_12_mBCFC481AE1D5FFC8EF344AB7295B996B98266D81_RuntimeMethod_var), NULL);
		Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* L_88 = (Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07*)il2cpp_codegen_object_new(Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07_il2cpp_TypeInfo_var);
		Action__ctor_mBDC7B0B4A3F583B64C2896F01BDED360772F67DC(L_88, __this, (intptr_t)((void*)ScreenRouter_NavigateBack_m8755B6CD7EDC8A0A4E6892C2EC74A718571E232B_RuntimeMethod_var), NULL);
		Action_1_t3CB5D1A819C3ED3F99E9E39F890F18633253949A* L_89 = (Action_1_t3CB5D1A819C3ED3F99E9E39F890F18633253949A*)il2cpp_codegen_object_new(Action_1_t3CB5D1A819C3ED3F99E9E39F890F18633253949A_il2cpp_TypeInfo_var);
		Action_1__ctor_m9DC2953C55C4D7D4B7BEFE03D84DA1F9362D652C(L_89, __this, (intptr_t)((void*)ScreenRouter_U3CBuildInteractionLayerU3Eb__85_13_m5273AD925AAF6C4454E62046D3B35964D3CB55D1_RuntimeMethod_var), NULL);
		NullCheck(L_86);
		ProductionShopView_Bind_mCCC6F354F7A48C9A79B757E6D288456D0033BCBD(L_86, L_87, L_88, L_89, NULL);
		return;
	}

IL_031d:
	{
		InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB* L_90 = (InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB*)il2cpp_codegen_object_new(((RuntimeClass*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB_il2cpp_TypeInfo_var)));
		InvalidOperationException__ctor_mE4CB6F4712AB6D99A2358FBAE2E052B3EE976162(L_90, ((String_t*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&_stringLiteralD076113CEE62AE8CB3E3D2B951B576E17E030C2D)), NULL);
		IL2CPP_RAISE_MANAGED_EXCEPTION(L_90, ((RuntimeMethod*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&ScreenRouter_BuildInteractionLayer_m098546A1A74D7C435B6EAE0086CE1D53AD4D93EF_RuntimeMethod_var)));
	}

IL_0328:
	{
		RectTransform_t6C5DA5E41A89E0F488B001E45E58963480E543A5* L_91 = ___1_parent;
		NullCheck(L_91);
		ProductionDailyRewardView_tD3D86E84BE0D44D4F0F4963448D4CB68094FD0A9* L_92;
		L_92 = Component_GetComponentInChildren_TisProductionDailyRewardView_tD3D86E84BE0D44D4F0F4963448D4CB68094FD0A9_mD08648BCC4B6467F143C3305D46818BE4EFD4F6F(L_91, Component_GetComponentInChildren_TisProductionDailyRewardView_tD3D86E84BE0D44D4F0F4963448D4CB68094FD0A9_mD08648BCC4B6467F143C3305D46818BE4EFD4F6F_RuntimeMethod_var);
		V_8 = L_92;
		ProductionDailyRewardView_tD3D86E84BE0D44D4F0F4963448D4CB68094FD0A9* L_93 = V_8;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_94;
		L_94 = Object_op_Equality_mB6120F782D83091EF56A198FCEBCF066DB4A9605(L_93, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (L_94)
		{
			goto IL_0343;
		}
	}
	{
		ProductionDailyRewardView_tD3D86E84BE0D44D4F0F4963448D4CB68094FD0A9* L_95 = V_8;
		NullCheck(L_95);
		bool L_96;
		L_96 = ProductionDailyRewardView_get_HasRequiredBindings_m7399580C50D3895C904D67B47D55B28F279B7698(L_95, NULL);
		if (L_96)
		{
			goto IL_034e;
		}
	}

IL_0343:
	{
		InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB* L_97 = (InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB*)il2cpp_codegen_object_new(((RuntimeClass*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB_il2cpp_TypeInfo_var)));
		InvalidOperationException__ctor_mE4CB6F4712AB6D99A2358FBAE2E052B3EE976162(L_97, ((String_t*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&_stringLiteralF7612F3B3BD60973597FCA4B9147F74CA7D01379)), NULL);
		IL2CPP_RAISE_MANAGED_EXCEPTION(L_97, ((RuntimeMethod*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&ScreenRouter_BuildInteractionLayer_m098546A1A74D7C435B6EAE0086CE1D53AD4D93EF_RuntimeMethod_var)));
	}

IL_034e:
	{
		ProductionDailyRewardView_tD3D86E84BE0D44D4F0F4963448D4CB68094FD0A9* L_98 = V_8;
		Func_1_t2BE7F58348C9CC544A8973B3A9E55541DE43C457* L_99 = (Func_1_t2BE7F58348C9CC544A8973B3A9E55541DE43C457*)il2cpp_codegen_object_new(Func_1_t2BE7F58348C9CC544A8973B3A9E55541DE43C457_il2cpp_TypeInfo_var);
		Func_1__ctor_mDFFAE9C73346372438B5B04C4558AC42F1A3DA22(L_99, __this, (intptr_t)((void*)ScreenRouter_ClaimDailyRewardWithReceipt_mCEA3FE3252F39792C39F24CBC76E16CD6A7C0283_RuntimeMethod_var), NULL);
		Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* L_100 = (Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07*)il2cpp_codegen_object_new(Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07_il2cpp_TypeInfo_var);
		Action__ctor_mBDC7B0B4A3F583B64C2896F01BDED360772F67DC(L_100, __this, (intptr_t)((void*)ScreenRouter_NavigateBack_m8755B6CD7EDC8A0A4E6892C2EC74A718571E232B_RuntimeMethod_var), NULL);
		Action_1_t913CB0C3CDC5C59F6B8A1A7D957A63049F5A6BCA* L_101 = (Action_1_t913CB0C3CDC5C59F6B8A1A7D957A63049F5A6BCA*)il2cpp_codegen_object_new(Action_1_t913CB0C3CDC5C59F6B8A1A7D957A63049F5A6BCA_il2cpp_TypeInfo_var);
		Action_1__ctor_m5798433B5A0855FC0015630BF99C272AF00F3D75(L_101, __this, (intptr_t)((void*)ScreenRouter_U3CBuildInteractionLayerU3Eb__85_14_mC8BDC82DB12A2297E411DA656172365262CDB3E0_RuntimeMethod_var), NULL);
		NullCheck(L_98);
		ProductionDailyRewardView_Bind_mDDA34552E62AAF419B0CC473969191BBD9008DD4(L_98, L_99, L_100, L_101, NULL);
		return;
	}

IL_037a:
	{
		RectTransform_t6C5DA5E41A89E0F488B001E45E58963480E543A5* L_102 = ___1_parent;
		NullCheck(L_102);
		ProductionBoosterSelectionView_tC3AB07A664B7F97CBD28AB8693822704399FF79C* L_103;
		L_103 = Component_GetComponentInChildren_TisProductionBoosterSelectionView_tC3AB07A664B7F97CBD28AB8693822704399FF79C_m2CAE450D384AC463D693AA8F0C4BFBDF64009253(L_102, Component_GetComponentInChildren_TisProductionBoosterSelectionView_tC3AB07A664B7F97CBD28AB8693822704399FF79C_m2CAE450D384AC463D693AA8F0C4BFBDF64009253_RuntimeMethod_var);
		V_9 = L_103;
		ProductionBoosterSelectionView_tC3AB07A664B7F97CBD28AB8693822704399FF79C* L_104 = V_9;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_105;
		L_105 = Object_op_Equality_mB6120F782D83091EF56A198FCEBCF066DB4A9605(L_104, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (L_105)
		{
			goto IL_0395;
		}
	}
	{
		ProductionBoosterSelectionView_tC3AB07A664B7F97CBD28AB8693822704399FF79C* L_106 = V_9;
		NullCheck(L_106);
		bool L_107;
		L_107 = ProductionBoosterSelectionView_get_HasRequiredBindings_m1CF2DA8DBC8376805CF9AAA319D5C4E998B86C45(L_106, NULL);
		if (L_107)
		{
			goto IL_03a0;
		}
	}

IL_0395:
	{
		InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB* L_108 = (InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB*)il2cpp_codegen_object_new(((RuntimeClass*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB_il2cpp_TypeInfo_var)));
		InvalidOperationException__ctor_mE4CB6F4712AB6D99A2358FBAE2E052B3EE976162(L_108, ((String_t*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&_stringLiteral9070D4A690E72CE624303037A84F67C8A3AAEC36)), NULL);
		IL2CPP_RAISE_MANAGED_EXCEPTION(L_108, ((RuntimeMethod*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&ScreenRouter_BuildInteractionLayer_m098546A1A74D7C435B6EAE0086CE1D53AD4D93EF_RuntimeMethod_var)));
	}

IL_03a0:
	{
		ProductionBoosterSelectionView_tC3AB07A664B7F97CBD28AB8693822704399FF79C* L_109 = V_9;
		Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* L_110 = (Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07*)il2cpp_codegen_object_new(Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07_il2cpp_TypeInfo_var);
		Action__ctor_mBDC7B0B4A3F583B64C2896F01BDED360772F67DC(L_110, __this, (intptr_t)((void*)ScreenRouter_NavigateBack_m8755B6CD7EDC8A0A4E6892C2EC74A718571E232B_RuntimeMethod_var), NULL);
		Func_1_t2BE7F58348C9CC544A8973B3A9E55541DE43C457* L_111 = (Func_1_t2BE7F58348C9CC544A8973B3A9E55541DE43C457*)il2cpp_codegen_object_new(Func_1_t2BE7F58348C9CC544A8973B3A9E55541DE43C457_il2cpp_TypeInfo_var);
		Func_1__ctor_mDFFAE9C73346372438B5B04C4558AC42F1A3DA22(L_111, __this, (intptr_t)((void*)ScreenRouter_U3CBuildInteractionLayerU3Eb__85_15_mBEFB27EE475A918E5B183429A6C4AF8569D3ECA1_RuntimeMethod_var), NULL);
		NullCheck(L_109);
		ProductionBoosterSelectionView_Bind_m3E211B29FFB790717A21804CC8BA4B014F24A58F(L_109, (bool)0, L_110, L_111, NULL);
		return;
	}

IL_03c1:
	{
		RectTransform_t6C5DA5E41A89E0F488B001E45E58963480E543A5* L_112 = ___1_parent;
		NullCheck(L_112);
		ProductionOutOfEnergyView_t9ED536371FCF21CC2E58121FFF0561349450226A* L_113;
		L_113 = Component_GetComponentInChildren_TisProductionOutOfEnergyView_t9ED536371FCF21CC2E58121FFF0561349450226A_mC085D077806717D5DB805D8AA8F462257CECEEAB(L_112, Component_GetComponentInChildren_TisProductionOutOfEnergyView_t9ED536371FCF21CC2E58121FFF0561349450226A_mC085D077806717D5DB805D8AA8F462257CECEEAB_RuntimeMethod_var);
		V_10 = L_113;
		ProductionOutOfEnergyView_t9ED536371FCF21CC2E58121FFF0561349450226A* L_114 = V_10;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_115;
		L_115 = Object_op_Equality_mB6120F782D83091EF56A198FCEBCF066DB4A9605(L_114, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (L_115)
		{
			goto IL_03dc;
		}
	}
	{
		ProductionOutOfEnergyView_t9ED536371FCF21CC2E58121FFF0561349450226A* L_116 = V_10;
		NullCheck(L_116);
		bool L_117;
		L_117 = ProductionOutOfEnergyView_get_HasRequiredBindings_mCE7847D9D1E512AAF643463FC6DB826B92025FB6(L_116, NULL);
		if (L_117)
		{
			goto IL_03e7;
		}
	}

IL_03dc:
	{
		InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB* L_118 = (InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB*)il2cpp_codegen_object_new(((RuntimeClass*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB_il2cpp_TypeInfo_var)));
		InvalidOperationException__ctor_mE4CB6F4712AB6D99A2358FBAE2E052B3EE976162(L_118, ((String_t*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&_stringLiteral38AE809D0E3ADFDDD3984A121EC59F33998F8DC1)), NULL);
		IL2CPP_RAISE_MANAGED_EXCEPTION(L_118, ((RuntimeMethod*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&ScreenRouter_BuildInteractionLayer_m098546A1A74D7C435B6EAE0086CE1D53AD4D93EF_RuntimeMethod_var)));
	}

IL_03e7:
	{
		ProductionOutOfEnergyView_t9ED536371FCF21CC2E58121FFF0561349450226A* L_119 = V_10;
		Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* L_120 = (Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07*)il2cpp_codegen_object_new(Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07_il2cpp_TypeInfo_var);
		Action__ctor_mBDC7B0B4A3F583B64C2896F01BDED360772F67DC(L_120, __this, (intptr_t)((void*)ScreenRouter_NavigateBack_m8755B6CD7EDC8A0A4E6892C2EC74A718571E232B_RuntimeMethod_var), NULL);
		Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* L_121 = (Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07*)il2cpp_codegen_object_new(Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07_il2cpp_TypeInfo_var);
		Action__ctor_mBDC7B0B4A3F583B64C2896F01BDED360772F67DC(L_121, __this, (intptr_t)((void*)ScreenRouter_U3CBuildInteractionLayerU3Eb__85_16_m2751885DD00FDB126492688F26CD9BB357A2EDD1_RuntimeMethod_var), NULL);
		Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* L_122 = (Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07*)il2cpp_codegen_object_new(Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07_il2cpp_TypeInfo_var);
		Action__ctor_mBDC7B0B4A3F583B64C2896F01BDED360772F67DC(L_122, __this, (intptr_t)((void*)ScreenRouter_RequestDemoFriendEnergy_mABC8AA11E27D15CAC582554E248BEA14C1C4BE67_RuntimeMethod_var), NULL);
		Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* L_123 = (Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07*)il2cpp_codegen_object_new(Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07_il2cpp_TypeInfo_var);
		Action__ctor_mBDC7B0B4A3F583B64C2896F01BDED360772F67DC(L_123, __this, (intptr_t)((void*)ScreenRouter_RequestRewardedEnergy_m2A6CDD4D1C0A3CA058E948D1DFC2245B3353F27D_RuntimeMethod_var), NULL);
		NullCheck(L_119);
		ProductionOutOfEnergyView_Bind_mC4851677ED358B20CBBED8FDFA65EDD60BB2FA70(L_119, (bool)0, L_120, L_121, L_122, L_123, NULL);
		return;
	}

IL_0420:
	{
		RectTransform_t6C5DA5E41A89E0F488B001E45E58963480E543A5* L_124 = ___1_parent;
		NullCheck(L_124);
		ProductionRewardReceivedView_t308036C90189F05146CA7191FC9674291133EC11* L_125;
		L_125 = Component_GetComponentInChildren_TisProductionRewardReceivedView_t308036C90189F05146CA7191FC9674291133EC11_mDB789F0CDD8849D2B9F480B102EFB6ED6E316CCB(L_124, Component_GetComponentInChildren_TisProductionRewardReceivedView_t308036C90189F05146CA7191FC9674291133EC11_mDB789F0CDD8849D2B9F480B102EFB6ED6E316CCB_RuntimeMethod_var);
		V_11 = L_125;
		ProductionRewardReceivedView_t308036C90189F05146CA7191FC9674291133EC11* L_126 = V_11;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_127;
		L_127 = Object_op_Equality_mB6120F782D83091EF56A198FCEBCF066DB4A9605(L_126, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (L_127)
		{
			goto IL_043b;
		}
	}
	{
		ProductionRewardReceivedView_t308036C90189F05146CA7191FC9674291133EC11* L_128 = V_11;
		NullCheck(L_128);
		bool L_129;
		L_129 = ProductionRewardReceivedView_get_HasRequiredBindings_m5904F48704BFEB4761A6D5F758CA74607F492402(L_128, NULL);
		if (L_129)
		{
			goto IL_0446;
		}
	}

IL_043b:
	{
		InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB* L_130 = (InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB*)il2cpp_codegen_object_new(((RuntimeClass*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB_il2cpp_TypeInfo_var)));
		InvalidOperationException__ctor_mE4CB6F4712AB6D99A2358FBAE2E052B3EE976162(L_130, ((String_t*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&_stringLiteral7AADC8B71CC9CCB8C211938B0BC8D0F070A3B083)), NULL);
		IL2CPP_RAISE_MANAGED_EXCEPTION(L_130, ((RuntimeMethod*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&ScreenRouter_BuildInteractionLayer_m098546A1A74D7C435B6EAE0086CE1D53AD4D93EF_RuntimeMethod_var)));
	}

IL_0446:
	{
		ProductionRewardReceivedView_t308036C90189F05146CA7191FC9674291133EC11* L_131 = V_11;
		il2cpp_codegen_initobj((&V_31), sizeof(Nullable_1_tFF46807EF2622F15A62B26F542EC38258BA8254E));
		Nullable_1_tFF46807EF2622F15A62B26F542EC38258BA8254E L_132 = V_31;
		Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* L_133 = (Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07*)il2cpp_codegen_object_new(Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07_il2cpp_TypeInfo_var);
		Action__ctor_mBDC7B0B4A3F583B64C2896F01BDED360772F67DC(L_133, __this, (intptr_t)((void*)ScreenRouter_NavigateBack_m8755B6CD7EDC8A0A4E6892C2EC74A718571E232B_RuntimeMethod_var), NULL);
		NullCheck(L_131);
		ProductionRewardReceivedView_Bind_m586EEA7BAC623D7C089B74FD1485DAE2EE433186(L_131, L_132, 0, L_133, NULL);
		return;
	}

IL_0465:
	{
		RectTransform_t6C5DA5E41A89E0F488B001E45E58963480E543A5* L_134 = ___1_parent;
		NullCheck(L_134);
		ProductionEventsView_t60F8250E60FB6585506BBB36AB2FA5C392D130FA* L_135;
		L_135 = Component_GetComponentInChildren_TisProductionEventsView_t60F8250E60FB6585506BBB36AB2FA5C392D130FA_mF04A7124BA3D8D332303AA248881507B215636B3(L_134, Component_GetComponentInChildren_TisProductionEventsView_t60F8250E60FB6585506BBB36AB2FA5C392D130FA_mF04A7124BA3D8D332303AA248881507B215636B3_RuntimeMethod_var);
		V_12 = L_135;
		ProductionEventsView_t60F8250E60FB6585506BBB36AB2FA5C392D130FA* L_136 = V_12;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_137;
		L_137 = Object_op_Equality_mB6120F782D83091EF56A198FCEBCF066DB4A9605(L_136, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (L_137)
		{
			goto IL_0480;
		}
	}
	{
		ProductionEventsView_t60F8250E60FB6585506BBB36AB2FA5C392D130FA* L_138 = V_12;
		NullCheck(L_138);
		bool L_139;
		L_139 = ProductionEventsView_get_HasRequiredBindings_m77A71FC7A8A93CE1958F65237C0223EE67D225F5(L_138, NULL);
		if (L_139)
		{
			goto IL_048b;
		}
	}

IL_0480:
	{
		InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB* L_140 = (InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB*)il2cpp_codegen_object_new(((RuntimeClass*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB_il2cpp_TypeInfo_var)));
		InvalidOperationException__ctor_mE4CB6F4712AB6D99A2358FBAE2E052B3EE976162(L_140, ((String_t*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&_stringLiteralAB25BE29C3F84B0AE2FED4000FC93EA673577A21)), NULL);
		IL2CPP_RAISE_MANAGED_EXCEPTION(L_140, ((RuntimeMethod*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&ScreenRouter_BuildInteractionLayer_m098546A1A74D7C435B6EAE0086CE1D53AD4D93EF_RuntimeMethod_var)));
	}

IL_048b:
	{
		ProductionEventsView_t60F8250E60FB6585506BBB36AB2FA5C392D130FA* L_141 = V_12;
		Action_1_t913CB0C3CDC5C59F6B8A1A7D957A63049F5A6BCA* L_142 = (Action_1_t913CB0C3CDC5C59F6B8A1A7D957A63049F5A6BCA*)il2cpp_codegen_object_new(Action_1_t913CB0C3CDC5C59F6B8A1A7D957A63049F5A6BCA_il2cpp_TypeInfo_var);
		Action_1__ctor_m5798433B5A0855FC0015630BF99C272AF00F3D75(L_142, __this, (intptr_t)((void*)ScreenRouter_U3CBuildInteractionLayerU3Eb__85_17_mB0CD8B1B53F591BFDC6287C6CBDF66B106F16019_RuntimeMethod_var), NULL);
		Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* L_143 = (Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07*)il2cpp_codegen_object_new(Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07_il2cpp_TypeInfo_var);
		Action__ctor_mBDC7B0B4A3F583B64C2896F01BDED360772F67DC(L_143, __this, (intptr_t)((void*)ScreenRouter_NavigateBack_m8755B6CD7EDC8A0A4E6892C2EC74A718571E232B_RuntimeMethod_var), NULL);
		NullCheck(L_141);
		ProductionEventsView_Bind_mB2F25A84E41C8716D836BBA2393DA1A5557C67CD(L_141, L_142, L_143, NULL);
		return;
	}

IL_04ab:
	{
		RectTransform_t6C5DA5E41A89E0F488B001E45E58963480E543A5* L_144 = ___1_parent;
		NullCheck(L_144);
		ProductionProfileView_tFBAE39BD387DB9B67C01A51EB40718FD7461992D* L_145;
		L_145 = Component_GetComponentInChildren_TisProductionProfileView_tFBAE39BD387DB9B67C01A51EB40718FD7461992D_m446C73FD0151B25AD1CBD36400F2A63645FE1667(L_144, Component_GetComponentInChildren_TisProductionProfileView_tFBAE39BD387DB9B67C01A51EB40718FD7461992D_m446C73FD0151B25AD1CBD36400F2A63645FE1667_RuntimeMethod_var);
		V_13 = L_145;
		ProductionProfileView_tFBAE39BD387DB9B67C01A51EB40718FD7461992D* L_146 = V_13;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_147;
		L_147 = Object_op_Equality_mB6120F782D83091EF56A198FCEBCF066DB4A9605(L_146, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (L_147)
		{
			goto IL_04c6;
		}
	}
	{
		ProductionProfileView_tFBAE39BD387DB9B67C01A51EB40718FD7461992D* L_148 = V_13;
		NullCheck(L_148);
		bool L_149;
		L_149 = ProductionProfileView_get_HasRequiredBindings_mD4FDD50B6D3717ADFCCAD52450501B6920FADE36(L_148, NULL);
		if (L_149)
		{
			goto IL_04d1;
		}
	}

IL_04c6:
	{
		InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB* L_150 = (InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB*)il2cpp_codegen_object_new(((RuntimeClass*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB_il2cpp_TypeInfo_var)));
		InvalidOperationException__ctor_mE4CB6F4712AB6D99A2358FBAE2E052B3EE976162(L_150, ((String_t*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&_stringLiteral43C13685E18771FA9394499CA128D809E2A12493)), NULL);
		IL2CPP_RAISE_MANAGED_EXCEPTION(L_150, ((RuntimeMethod*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&ScreenRouter_BuildInteractionLayer_m098546A1A74D7C435B6EAE0086CE1D53AD4D93EF_RuntimeMethod_var)));
	}

IL_04d1:
	{
		ProductionProfileView_tFBAE39BD387DB9B67C01A51EB40718FD7461992D* L_151 = V_13;
		Action_1_t913CB0C3CDC5C59F6B8A1A7D957A63049F5A6BCA* L_152 = (Action_1_t913CB0C3CDC5C59F6B8A1A7D957A63049F5A6BCA*)il2cpp_codegen_object_new(Action_1_t913CB0C3CDC5C59F6B8A1A7D957A63049F5A6BCA_il2cpp_TypeInfo_var);
		Action_1__ctor_m5798433B5A0855FC0015630BF99C272AF00F3D75(L_152, __this, (intptr_t)((void*)ScreenRouter_U3CBuildInteractionLayerU3Eb__85_18_m5F484F506A22A4E454EA4CB078BABECF0D3B4A25_RuntimeMethod_var), NULL);
		Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* L_153 = (Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07*)il2cpp_codegen_object_new(Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07_il2cpp_TypeInfo_var);
		Action__ctor_mBDC7B0B4A3F583B64C2896F01BDED360772F67DC(L_153, __this, (intptr_t)((void*)ScreenRouter_NavigateBack_m8755B6CD7EDC8A0A4E6892C2EC74A718571E232B_RuntimeMethod_var), NULL);
		Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* L_154 = (Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07*)il2cpp_codegen_object_new(Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07_il2cpp_TypeInfo_var);
		Action__ctor_mBDC7B0B4A3F583B64C2896F01BDED360772F67DC(L_154, __this, (intptr_t)((void*)ScreenRouter_OpenStoryRecap_mDCFB012178541AEA849D6187CA92D02DA4D05E59_RuntimeMethod_var), NULL);
		NullCheck(L_151);
		ProductionProfileView_Bind_m79329BE4D57D5C4FC0AFF6D6E132E051B59FA55F(L_151, L_152, L_153, L_154, NULL);
		return;
	}

IL_04fd:
	{
		RectTransform_t6C5DA5E41A89E0F488B001E45E58963480E543A5* L_155 = ___1_parent;
		ScreenRouter_BindNativeSettings_mB2E797A3CC282D1570275A7A42AFD251CEB7D8D1(__this, L_155, NULL);
		return;
	}

IL_0505:
	{
		RectTransform_t6C5DA5E41A89E0F488B001E45E58963480E543A5* L_156 = ___1_parent;
		NullCheck(L_156);
		ProductionCharactersView_t80FF719EBB00992075D821990BF966D5E95577DE* L_157;
		L_157 = Component_GetComponentInChildren_TisProductionCharactersView_t80FF719EBB00992075D821990BF966D5E95577DE_mF14E8F137A064E850586F44746439D4CEDF5912B(L_156, Component_GetComponentInChildren_TisProductionCharactersView_t80FF719EBB00992075D821990BF966D5E95577DE_mF14E8F137A064E850586F44746439D4CEDF5912B_RuntimeMethod_var);
		V_14 = L_157;
		ProductionCharactersView_t80FF719EBB00992075D821990BF966D5E95577DE* L_158 = V_14;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_159;
		L_159 = Object_op_Equality_mB6120F782D83091EF56A198FCEBCF066DB4A9605(L_158, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (L_159)
		{
			goto IL_0520;
		}
	}
	{
		ProductionCharactersView_t80FF719EBB00992075D821990BF966D5E95577DE* L_160 = V_14;
		NullCheck(L_160);
		bool L_161;
		L_161 = ProductionCharactersView_get_HasRequiredBindings_m0EE996C34F33F897528C8866367DFE6D01DD84BE(L_160, NULL);
		if (L_161)
		{
			goto IL_052b;
		}
	}

IL_0520:
	{
		InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB* L_162 = (InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB*)il2cpp_codegen_object_new(((RuntimeClass*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB_il2cpp_TypeInfo_var)));
		InvalidOperationException__ctor_mE4CB6F4712AB6D99A2358FBAE2E052B3EE976162(L_162, ((String_t*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&_stringLiteralED5A9044C74411D102A4574DCEE164B7941069DD)), NULL);
		IL2CPP_RAISE_MANAGED_EXCEPTION(L_162, ((RuntimeMethod*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&ScreenRouter_BuildInteractionLayer_m098546A1A74D7C435B6EAE0086CE1D53AD4D93EF_RuntimeMethod_var)));
	}

IL_052b:
	{
		ProductionCharactersView_t80FF719EBB00992075D821990BF966D5E95577DE* L_163 = V_14;
		Action_1_t913CB0C3CDC5C59F6B8A1A7D957A63049F5A6BCA* L_164 = (Action_1_t913CB0C3CDC5C59F6B8A1A7D957A63049F5A6BCA*)il2cpp_codegen_object_new(Action_1_t913CB0C3CDC5C59F6B8A1A7D957A63049F5A6BCA_il2cpp_TypeInfo_var);
		Action_1__ctor_m5798433B5A0855FC0015630BF99C272AF00F3D75(L_164, __this, (intptr_t)((void*)ScreenRouter_U3CBuildInteractionLayerU3Eb__85_19_m3920E5B2FB0C0D23F94B2B319D25AB084AEF99CC_RuntimeMethod_var), NULL);
		Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* L_165 = (Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07*)il2cpp_codegen_object_new(Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07_il2cpp_TypeInfo_var);
		Action__ctor_mBDC7B0B4A3F583B64C2896F01BDED360772F67DC(L_165, __this, (intptr_t)((void*)ScreenRouter_NavigateBack_m8755B6CD7EDC8A0A4E6892C2EC74A718571E232B_RuntimeMethod_var), NULL);
		Action_1_t3CB5D1A819C3ED3F99E9E39F890F18633253949A* L_166 = (Action_1_t3CB5D1A819C3ED3F99E9E39F890F18633253949A*)il2cpp_codegen_object_new(Action_1_t3CB5D1A819C3ED3F99E9E39F890F18633253949A_il2cpp_TypeInfo_var);
		Action_1__ctor_m9DC2953C55C4D7D4B7BEFE03D84DA1F9362D652C(L_166, __this, (intptr_t)((void*)ScreenRouter_U3CBuildInteractionLayerU3Eb__85_20_mAF8095FC8FA247F96838436F2A20E1FE6612A711_RuntimeMethod_var), NULL);
		NullCheck(L_163);
		ProductionCharactersView_Bind_m7E3F8823CC8225B6BF90E51B11D2E95A3D678E6B(L_163, L_164, L_165, L_166, NULL);
		return;
	}

IL_0557:
	{
		RectTransform_t6C5DA5E41A89E0F488B001E45E58963480E543A5* L_167 = ___1_parent;
		NullCheck(L_167);
		ProductionMailView_t613C10918DAA8582972FFE0681462F4DAE73B966* L_168;
		L_168 = Component_GetComponentInChildren_TisProductionMailView_t613C10918DAA8582972FFE0681462F4DAE73B966_m2D5BCFC55825FE84588DD471D9FEE1177C6C658C(L_167, Component_GetComponentInChildren_TisProductionMailView_t613C10918DAA8582972FFE0681462F4DAE73B966_m2D5BCFC55825FE84588DD471D9FEE1177C6C658C_RuntimeMethod_var);
		V_15 = L_168;
		ProductionMailView_t613C10918DAA8582972FFE0681462F4DAE73B966* L_169 = V_15;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_170;
		L_170 = Object_op_Equality_mB6120F782D83091EF56A198FCEBCF066DB4A9605(L_169, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (L_170)
		{
			goto IL_0572;
		}
	}
	{
		ProductionMailView_t613C10918DAA8582972FFE0681462F4DAE73B966* L_171 = V_15;
		NullCheck(L_171);
		bool L_172;
		L_172 = ProductionMailView_get_HasRequiredBindings_m7187E42289FAD23F4F40088F5657330BB86A0C96(L_171, NULL);
		if (L_172)
		{
			goto IL_057d;
		}
	}

IL_0572:
	{
		InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB* L_173 = (InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB*)il2cpp_codegen_object_new(((RuntimeClass*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB_il2cpp_TypeInfo_var)));
		InvalidOperationException__ctor_mE4CB6F4712AB6D99A2358FBAE2E052B3EE976162(L_173, ((String_t*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&_stringLiteral2130BAC49DEAA588E1D005F0AF29AFA03A360790)), NULL);
		IL2CPP_RAISE_MANAGED_EXCEPTION(L_173, ((RuntimeMethod*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&ScreenRouter_BuildInteractionLayer_m098546A1A74D7C435B6EAE0086CE1D53AD4D93EF_RuntimeMethod_var)));
	}

IL_057d:
	{
		ProductionMailView_t613C10918DAA8582972FFE0681462F4DAE73B966* L_174 = V_15;
		Action_1_t913CB0C3CDC5C59F6B8A1A7D957A63049F5A6BCA* L_175 = (Action_1_t913CB0C3CDC5C59F6B8A1A7D957A63049F5A6BCA*)il2cpp_codegen_object_new(Action_1_t913CB0C3CDC5C59F6B8A1A7D957A63049F5A6BCA_il2cpp_TypeInfo_var);
		Action_1__ctor_m5798433B5A0855FC0015630BF99C272AF00F3D75(L_175, __this, (intptr_t)((void*)ScreenRouter_U3CBuildInteractionLayerU3Eb__85_21_m532E692101625F4FA7018DD002C14611474898F9_RuntimeMethod_var), NULL);
		Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* L_176 = (Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07*)il2cpp_codegen_object_new(Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07_il2cpp_TypeInfo_var);
		Action__ctor_mBDC7B0B4A3F583B64C2896F01BDED360772F67DC(L_176, __this, (intptr_t)((void*)ScreenRouter_NavigateBack_m8755B6CD7EDC8A0A4E6892C2EC74A718571E232B_RuntimeMethod_var), NULL);
		NullCheck(L_174);
		ProductionMailView_Bind_mA1EB35BDB91BDA5CA217ECD91A66714830127D0C(L_174, L_175, L_176, NULL);
		return;
	}

IL_059d:
	{
		RectTransform_t6C5DA5E41A89E0F488B001E45E58963480E543A5* L_177 = ___1_parent;
		NullCheck(L_177);
		SplashTitleView_t5C9B3C703C7BCA2970A475F6B1EF763DD452FB17* L_178;
		L_178 = Component_GetComponentInChildren_TisSplashTitleView_t5C9B3C703C7BCA2970A475F6B1EF763DD452FB17_mFF739B05797AB82FC85EC4521506F415FA6D5169(L_177, (bool)1, Component_GetComponentInChildren_TisSplashTitleView_t5C9B3C703C7BCA2970A475F6B1EF763DD452FB17_mFF739B05797AB82FC85EC4521506F415FA6D5169_RuntimeMethod_var);
		V_16 = L_178;
		SplashTitleView_t5C9B3C703C7BCA2970A475F6B1EF763DD452FB17* L_179 = V_16;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_180;
		L_180 = Object_op_Equality_mB6120F782D83091EF56A198FCEBCF066DB4A9605(L_179, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (L_180)
		{
			goto IL_05b9;
		}
	}
	{
		SplashTitleView_t5C9B3C703C7BCA2970A475F6B1EF763DD452FB17* L_181 = V_16;
		NullCheck(L_181);
		bool L_182;
		L_182 = SplashTitleView_get_HasRequiredBindings_m6CF8265CDE18B453AA3A6D3F6301DF9F85152DE4(L_181, NULL);
		if (L_182)
		{
			goto IL_05c4;
		}
	}

IL_05b9:
	{
		il2cpp_codegen_runtime_class_init_inline(Debug_t8394C7EEAECA3689C2C9B9DE9C7166D73596276F_il2cpp_TypeInfo_var);
		Debug_LogError_mB00B2B4468EF3CAF041B038D840820FB84C924B2(_stringLiteral92971B9CC0D8D91320245699D154C5BF895FE8E0, NULL);
		return;
	}

IL_05c4:
	{
		SplashTitleView_t5C9B3C703C7BCA2970A475F6B1EF763DD452FB17* L_183 = V_16;
		Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* L_184 = (Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07*)il2cpp_codegen_object_new(Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07_il2cpp_TypeInfo_var);
		Action__ctor_mBDC7B0B4A3F583B64C2896F01BDED360772F67DC(L_184, __this, (intptr_t)((void*)ScreenRouter_U3CBuildInteractionLayerU3Eb__85_22_mAB5810B32799CC6717BB3A0DCC2D7310C4596BDF_RuntimeMethod_var), NULL);
		Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* L_185;
		L_185 = ScreenRouter_GuardSplashAction_m32ABC35578B79CAD26DA9D5AABBC4890342B3EA3(__this, L_184, NULL);
		Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* L_186 = (Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07*)il2cpp_codegen_object_new(Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07_il2cpp_TypeInfo_var);
		Action__ctor_mBDC7B0B4A3F583B64C2896F01BDED360772F67DC(L_186, __this, (intptr_t)((void*)ScreenRouter_U3CBuildInteractionLayerU3Eb__85_23_mA5D665893517F7816BF9BBBEE55EB831094F057B_RuntimeMethod_var), NULL);
		Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* L_187;
		L_187 = ScreenRouter_GuardSplashAction_m32ABC35578B79CAD26DA9D5AABBC4890342B3EA3(__this, L_186, NULL);
		Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* L_188 = (Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07*)il2cpp_codegen_object_new(Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07_il2cpp_TypeInfo_var);
		Action__ctor_mBDC7B0B4A3F583B64C2896F01BDED360772F67DC(L_188, __this, (intptr_t)((void*)ScreenRouter_U3CBuildInteractionLayerU3Eb__85_24_m00C9D65468792D340A88535448504C49A3C2A7E2_RuntimeMethod_var), NULL);
		Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* L_189;
		L_189 = ScreenRouter_GuardSplashAction_m32ABC35578B79CAD26DA9D5AABBC4890342B3EA3(__this, L_188, NULL);
		Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* L_190 = (Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07*)il2cpp_codegen_object_new(Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07_il2cpp_TypeInfo_var);
		Action__ctor_mBDC7B0B4A3F583B64C2896F01BDED360772F67DC(L_190, __this, (intptr_t)((void*)ScreenRouter_U3CBuildInteractionLayerU3Eb__85_25_m3DEF03E9B9121F07964E548223EEC0C2D698C3F8_RuntimeMethod_var), NULL);
		Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* L_191;
		L_191 = ScreenRouter_GuardSplashAction_m32ABC35578B79CAD26DA9D5AABBC4890342B3EA3(__this, L_190, NULL);
		Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* L_192 = (Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07*)il2cpp_codegen_object_new(Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07_il2cpp_TypeInfo_var);
		Action__ctor_mBDC7B0B4A3F583B64C2896F01BDED360772F67DC(L_192, __this, (intptr_t)((void*)ScreenRouter_U3CBuildInteractionLayerU3Eb__85_26_mCB52745857DB75B08585A992C622D0CAD9470970_RuntimeMethod_var), NULL);
		Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* L_193;
		L_193 = ScreenRouter_GuardSplashAction_m32ABC35578B79CAD26DA9D5AABBC4890342B3EA3(__this, L_192, NULL);
		NullCheck(L_183);
		SplashTitleView_BindActions_mF09F5A17F27B4854E427D6EEB1FF2C6096B40505(L_183, L_185, L_187, L_189, L_191, L_193, NULL);
		return;
	}

IL_0626:
	{
		RectTransform_t6C5DA5E41A89E0F488B001E45E58963480E543A5* L_194 = ___1_parent;
		NullCheck(L_194);
		ProductionEventDetailView_t7360325479FBCA2559711C551444B3B712CF6CE5* L_195;
		L_195 = Component_GetComponentInChildren_TisProductionEventDetailView_t7360325479FBCA2559711C551444B3B712CF6CE5_mC8B301908CD50059D72FA6B292C6A4BD7A1E8E4B(L_194, Component_GetComponentInChildren_TisProductionEventDetailView_t7360325479FBCA2559711C551444B3B712CF6CE5_mC8B301908CD50059D72FA6B292C6A4BD7A1E8E4B_RuntimeMethod_var);
		V_17 = L_195;
		ProductionEventDetailView_t7360325479FBCA2559711C551444B3B712CF6CE5* L_196 = V_17;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_197;
		L_197 = Object_op_Equality_mB6120F782D83091EF56A198FCEBCF066DB4A9605(L_196, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (L_197)
		{
			goto IL_0641;
		}
	}
	{
		ProductionEventDetailView_t7360325479FBCA2559711C551444B3B712CF6CE5* L_198 = V_17;
		NullCheck(L_198);
		bool L_199;
		L_199 = ProductionEventDetailView_get_HasRequiredBindings_m47F64373D9CEF2F608E6AF65DC57D21289712D25(L_198, NULL);
		if (L_199)
		{
			goto IL_064c;
		}
	}

IL_0641:
	{
		InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB* L_200 = (InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB*)il2cpp_codegen_object_new(((RuntimeClass*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB_il2cpp_TypeInfo_var)));
		InvalidOperationException__ctor_mE4CB6F4712AB6D99A2358FBAE2E052B3EE976162(L_200, ((String_t*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&_stringLiteralBDC63061AA283469530D56F28CEF5A0D6CAA65E1)), NULL);
		IL2CPP_RAISE_MANAGED_EXCEPTION(L_200, ((RuntimeMethod*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&ScreenRouter_BuildInteractionLayer_m098546A1A74D7C435B6EAE0086CE1D53AD4D93EF_RuntimeMethod_var)));
	}

IL_064c:
	{
		ProductionEventDetailView_t7360325479FBCA2559711C551444B3B712CF6CE5* L_201 = V_17;
		Action_1_t913CB0C3CDC5C59F6B8A1A7D957A63049F5A6BCA* L_202 = (Action_1_t913CB0C3CDC5C59F6B8A1A7D957A63049F5A6BCA*)il2cpp_codegen_object_new(Action_1_t913CB0C3CDC5C59F6B8A1A7D957A63049F5A6BCA_il2cpp_TypeInfo_var);
		Action_1__ctor_m5798433B5A0855FC0015630BF99C272AF00F3D75(L_202, __this, (intptr_t)((void*)ScreenRouter_U3CBuildInteractionLayerU3Eb__85_27_m9C800467121F963AE5B3A9E8F5CC2203A3C66D46_RuntimeMethod_var), NULL);
		Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* L_203 = (Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07*)il2cpp_codegen_object_new(Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07_il2cpp_TypeInfo_var);
		Action__ctor_mBDC7B0B4A3F583B64C2896F01BDED360772F67DC(L_203, __this, (intptr_t)((void*)ScreenRouter_NavigateBack_m8755B6CD7EDC8A0A4E6892C2EC74A718571E232B_RuntimeMethod_var), NULL);
		NullCheck(L_201);
		ProductionEventDetailView_Bind_mA6A360C7757206878C65130141623F0D80CC6B10(L_201, L_202, L_203, NULL);
		return;
	}

IL_066c:
	{
		RectTransform_t6C5DA5E41A89E0F488B001E45E58963480E543A5* L_204 = ___1_parent;
		NullCheck(L_204);
		ProductionIndividualRankingView_tF449BDA2B9876849864F7F78C4C57CE0BB9726B3* L_205;
		L_205 = Component_GetComponentInChildren_TisProductionIndividualRankingView_tF449BDA2B9876849864F7F78C4C57CE0BB9726B3_mB4338486B6C14D9D4616520DBB20E5A74E544E5C(L_204, Component_GetComponentInChildren_TisProductionIndividualRankingView_tF449BDA2B9876849864F7F78C4C57CE0BB9726B3_mB4338486B6C14D9D4616520DBB20E5A74E544E5C_RuntimeMethod_var);
		V_18 = L_205;
		ProductionIndividualRankingView_tF449BDA2B9876849864F7F78C4C57CE0BB9726B3* L_206 = V_18;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_207;
		L_207 = Object_op_Equality_mB6120F782D83091EF56A198FCEBCF066DB4A9605(L_206, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (L_207)
		{
			goto IL_0687;
		}
	}
	{
		ProductionIndividualRankingView_tF449BDA2B9876849864F7F78C4C57CE0BB9726B3* L_208 = V_18;
		NullCheck(L_208);
		bool L_209;
		L_209 = ProductionIndividualRankingView_get_HasRequiredBindings_mDEA80DABC59C602FCF6A8F1F3E12DAF9B5CF329B(L_208, NULL);
		if (L_209)
		{
			goto IL_0692;
		}
	}

IL_0687:
	{
		InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB* L_210 = (InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB*)il2cpp_codegen_object_new(((RuntimeClass*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB_il2cpp_TypeInfo_var)));
		InvalidOperationException__ctor_mE4CB6F4712AB6D99A2358FBAE2E052B3EE976162(L_210, ((String_t*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&_stringLiteral7BACEA1BC8D712EE91133DD31B33CC6B034460CA)), NULL);
		IL2CPP_RAISE_MANAGED_EXCEPTION(L_210, ((RuntimeMethod*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&ScreenRouter_BuildInteractionLayer_m098546A1A74D7C435B6EAE0086CE1D53AD4D93EF_RuntimeMethod_var)));
	}

IL_0692:
	{
		ProductionIndividualRankingView_tF449BDA2B9876849864F7F78C4C57CE0BB9726B3* L_211 = V_18;
		Action_1_t913CB0C3CDC5C59F6B8A1A7D957A63049F5A6BCA* L_212 = (Action_1_t913CB0C3CDC5C59F6B8A1A7D957A63049F5A6BCA*)il2cpp_codegen_object_new(Action_1_t913CB0C3CDC5C59F6B8A1A7D957A63049F5A6BCA_il2cpp_TypeInfo_var);
		Action_1__ctor_m5798433B5A0855FC0015630BF99C272AF00F3D75(L_212, __this, (intptr_t)((void*)ScreenRouter_U3CBuildInteractionLayerU3Eb__85_28_mECE239325F4BDCEFD73E8B9A2501219A00676DC0_RuntimeMethod_var), NULL);
		Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* L_213 = (Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07*)il2cpp_codegen_object_new(Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07_il2cpp_TypeInfo_var);
		Action__ctor_mBDC7B0B4A3F583B64C2896F01BDED360772F67DC(L_213, __this, (intptr_t)((void*)ScreenRouter_NavigateBack_m8755B6CD7EDC8A0A4E6892C2EC74A718571E232B_RuntimeMethod_var), NULL);
		NullCheck(L_211);
		ProductionIndividualRankingView_Bind_mA46BFDF81B72C7E8CE9AA5023A65A15C93E7EADE(L_211, L_212, L_213, NULL);
		return;
	}

IL_06b2:
	{
		RectTransform_t6C5DA5E41A89E0F488B001E45E58963480E543A5* L_214 = ___1_parent;
		NullCheck(L_214);
		ProductionTeamHubView_t242CA702D2D2A557A5145C582FE6955173320992* L_215;
		L_215 = Component_GetComponentInChildren_TisProductionTeamHubView_t242CA702D2D2A557A5145C582FE6955173320992_m8CD52018D226794AAAEBCBE906B957945FFE86F1(L_214, Component_GetComponentInChildren_TisProductionTeamHubView_t242CA702D2D2A557A5145C582FE6955173320992_m8CD52018D226794AAAEBCBE906B957945FFE86F1_RuntimeMethod_var);
		V_19 = L_215;
		ProductionTeamHubView_t242CA702D2D2A557A5145C582FE6955173320992* L_216 = V_19;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_217;
		L_217 = Object_op_Equality_mB6120F782D83091EF56A198FCEBCF066DB4A9605(L_216, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (L_217)
		{
			goto IL_06cd;
		}
	}
	{
		ProductionTeamHubView_t242CA702D2D2A557A5145C582FE6955173320992* L_218 = V_19;
		NullCheck(L_218);
		bool L_219;
		L_219 = ProductionTeamHubView_get_HasRequiredBindings_m6023FFF90A9AAD37D7E3C2BE1224C7DE50B30E98(L_218, NULL);
		if (L_219)
		{
			goto IL_06d8;
		}
	}

IL_06cd:
	{
		InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB* L_220 = (InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB*)il2cpp_codegen_object_new(((RuntimeClass*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB_il2cpp_TypeInfo_var)));
		InvalidOperationException__ctor_mE4CB6F4712AB6D99A2358FBAE2E052B3EE976162(L_220, ((String_t*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&_stringLiteral3D61E679FDBF9BCBE208A3674C4FDF6867E1C487)), NULL);
		IL2CPP_RAISE_MANAGED_EXCEPTION(L_220, ((RuntimeMethod*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&ScreenRouter_BuildInteractionLayer_m098546A1A74D7C435B6EAE0086CE1D53AD4D93EF_RuntimeMethod_var)));
	}

IL_06d8:
	{
		ProductionTeamHubView_t242CA702D2D2A557A5145C582FE6955173320992* L_221 = V_19;
		Action_1_t913CB0C3CDC5C59F6B8A1A7D957A63049F5A6BCA* L_222 = (Action_1_t913CB0C3CDC5C59F6B8A1A7D957A63049F5A6BCA*)il2cpp_codegen_object_new(Action_1_t913CB0C3CDC5C59F6B8A1A7D957A63049F5A6BCA_il2cpp_TypeInfo_var);
		Action_1__ctor_m5798433B5A0855FC0015630BF99C272AF00F3D75(L_222, __this, (intptr_t)((void*)ScreenRouter_U3CBuildInteractionLayerU3Eb__85_29_mD6E827343FFD4309A0B2C4660BD7766B8464F240_RuntimeMethod_var), NULL);
		Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* L_223 = (Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07*)il2cpp_codegen_object_new(Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07_il2cpp_TypeInfo_var);
		Action__ctor_mBDC7B0B4A3F583B64C2896F01BDED360772F67DC(L_223, __this, (intptr_t)((void*)ScreenRouter_NavigateBack_m8755B6CD7EDC8A0A4E6892C2EC74A718571E232B_RuntimeMethod_var), NULL);
		NullCheck(L_221);
		ProductionTeamHubView_Bind_m70103FA4DA781275AD83596FFE1C8137C46AE121(L_221, L_222, L_223, NULL);
		return;
	}

IL_06f8:
	{
		RectTransform_t6C5DA5E41A89E0F488B001E45E58963480E543A5* L_224 = ___1_parent;
		NullCheck(L_224);
		ProductionTeamRankingView_t6A2E723B48D2D1F086684E393EC71E7A19E82DD4* L_225;
		L_225 = Component_GetComponentInChildren_TisProductionTeamRankingView_t6A2E723B48D2D1F086684E393EC71E7A19E82DD4_mE250DA820F30C1322456DBA230F6E0E1E4B2B5C3(L_224, Component_GetComponentInChildren_TisProductionTeamRankingView_t6A2E723B48D2D1F086684E393EC71E7A19E82DD4_mE250DA820F30C1322456DBA230F6E0E1E4B2B5C3_RuntimeMethod_var);
		V_20 = L_225;
		ProductionTeamRankingView_t6A2E723B48D2D1F086684E393EC71E7A19E82DD4* L_226 = V_20;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_227;
		L_227 = Object_op_Equality_mB6120F782D83091EF56A198FCEBCF066DB4A9605(L_226, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (L_227)
		{
			goto IL_0713;
		}
	}
	{
		ProductionTeamRankingView_t6A2E723B48D2D1F086684E393EC71E7A19E82DD4* L_228 = V_20;
		NullCheck(L_228);
		bool L_229;
		L_229 = ProductionTeamRankingView_get_HasRequiredBindings_m8D5E99E33A4E4007FC4FBECC93268933F1DDF130(L_228, NULL);
		if (L_229)
		{
			goto IL_071e;
		}
	}

IL_0713:
	{
		InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB* L_230 = (InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB*)il2cpp_codegen_object_new(((RuntimeClass*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB_il2cpp_TypeInfo_var)));
		InvalidOperationException__ctor_mE4CB6F4712AB6D99A2358FBAE2E052B3EE976162(L_230, ((String_t*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&_stringLiteralF9AEE31498A2A4FF6048DBAC38B696DB85C25281)), NULL);
		IL2CPP_RAISE_MANAGED_EXCEPTION(L_230, ((RuntimeMethod*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&ScreenRouter_BuildInteractionLayer_m098546A1A74D7C435B6EAE0086CE1D53AD4D93EF_RuntimeMethod_var)));
	}

IL_071e:
	{
		ProductionTeamRankingView_t6A2E723B48D2D1F086684E393EC71E7A19E82DD4* L_231 = V_20;
		Action_1_t913CB0C3CDC5C59F6B8A1A7D957A63049F5A6BCA* L_232 = (Action_1_t913CB0C3CDC5C59F6B8A1A7D957A63049F5A6BCA*)il2cpp_codegen_object_new(Action_1_t913CB0C3CDC5C59F6B8A1A7D957A63049F5A6BCA_il2cpp_TypeInfo_var);
		Action_1__ctor_m5798433B5A0855FC0015630BF99C272AF00F3D75(L_232, __this, (intptr_t)((void*)ScreenRouter_U3CBuildInteractionLayerU3Eb__85_30_mA612239D6D34658F3C3C3C8C182F6FBBC690EB22_RuntimeMethod_var), NULL);
		Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* L_233 = (Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07*)il2cpp_codegen_object_new(Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07_il2cpp_TypeInfo_var);
		Action__ctor_mBDC7B0B4A3F583B64C2896F01BDED360772F67DC(L_233, __this, (intptr_t)((void*)ScreenRouter_NavigateBack_m8755B6CD7EDC8A0A4E6892C2EC74A718571E232B_RuntimeMethod_var), NULL);
		NullCheck(L_231);
		ProductionTeamRankingView_Bind_mD3D5BD3A8A4915FBAD53031DBC0E3DCAC7724793(L_231, L_232, L_233, NULL);
		return;
	}

IL_073e:
	{
		RectTransform_t6C5DA5E41A89E0F488B001E45E58963480E543A5* L_234 = ___1_parent;
		NullCheck(L_234);
		ProductionTeamChatView_t3876DDF380A2DA789CB223EB9E32A75578DFE07E* L_235;
		L_235 = Component_GetComponentInChildren_TisProductionTeamChatView_t3876DDF380A2DA789CB223EB9E32A75578DFE07E_mF9F7EBF80FE2E7E514B194BCAFCF940A06FA65C2(L_234, Component_GetComponentInChildren_TisProductionTeamChatView_t3876DDF380A2DA789CB223EB9E32A75578DFE07E_mF9F7EBF80FE2E7E514B194BCAFCF940A06FA65C2_RuntimeMethod_var);
		V_21 = L_235;
		ProductionTeamChatView_t3876DDF380A2DA789CB223EB9E32A75578DFE07E* L_236 = V_21;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_237;
		L_237 = Object_op_Equality_mB6120F782D83091EF56A198FCEBCF066DB4A9605(L_236, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (L_237)
		{
			goto IL_0759;
		}
	}
	{
		ProductionTeamChatView_t3876DDF380A2DA789CB223EB9E32A75578DFE07E* L_238 = V_21;
		NullCheck(L_238);
		bool L_239;
		L_239 = ProductionTeamChatView_get_HasRequiredBindings_m8849469310C93D2F8215AFFD0DC84B797DBFC0A4(L_238, NULL);
		if (L_239)
		{
			goto IL_0764;
		}
	}

IL_0759:
	{
		InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB* L_240 = (InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB*)il2cpp_codegen_object_new(((RuntimeClass*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB_il2cpp_TypeInfo_var)));
		InvalidOperationException__ctor_mE4CB6F4712AB6D99A2358FBAE2E052B3EE976162(L_240, ((String_t*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&_stringLiteralBCC323C03A19BF48351882A07BCA310BEA035D54)), NULL);
		IL2CPP_RAISE_MANAGED_EXCEPTION(L_240, ((RuntimeMethod*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&ScreenRouter_BuildInteractionLayer_m098546A1A74D7C435B6EAE0086CE1D53AD4D93EF_RuntimeMethod_var)));
	}

IL_0764:
	{
		ProductionTeamChatView_t3876DDF380A2DA789CB223EB9E32A75578DFE07E* L_241 = V_21;
		Action_1_t913CB0C3CDC5C59F6B8A1A7D957A63049F5A6BCA* L_242 = (Action_1_t913CB0C3CDC5C59F6B8A1A7D957A63049F5A6BCA*)il2cpp_codegen_object_new(Action_1_t913CB0C3CDC5C59F6B8A1A7D957A63049F5A6BCA_il2cpp_TypeInfo_var);
		Action_1__ctor_m5798433B5A0855FC0015630BF99C272AF00F3D75(L_242, __this, (intptr_t)((void*)ScreenRouter_U3CBuildInteractionLayerU3Eb__85_31_mABB31581CFEA2E3464700A1D3C38F11A85CDD90D_RuntimeMethod_var), NULL);
		Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* L_243 = (Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07*)il2cpp_codegen_object_new(Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07_il2cpp_TypeInfo_var);
		Action__ctor_mBDC7B0B4A3F583B64C2896F01BDED360772F67DC(L_243, __this, (intptr_t)((void*)ScreenRouter_NavigateBack_m8755B6CD7EDC8A0A4E6892C2EC74A718571E232B_RuntimeMethod_var), NULL);
		NullCheck(L_241);
		ProductionTeamChatView_Bind_mFA72F92FBDBF04B9E2A51430C32419AC1AB0B43E(L_241, L_242, L_243, NULL);
		return;
	}

IL_0784:
	{
		RectTransform_t6C5DA5E41A89E0F488B001E45E58963480E543A5* L_244 = ___1_parent;
		NullCheck(L_244);
		ProductionFriendsListView_t568A80A4F587DE5E0D9156810474BD1B241353D1* L_245;
		L_245 = Component_GetComponentInChildren_TisProductionFriendsListView_t568A80A4F587DE5E0D9156810474BD1B241353D1_mA39DD78FA3CB7B4FC4475179DECC45F18647B69F(L_244, Component_GetComponentInChildren_TisProductionFriendsListView_t568A80A4F587DE5E0D9156810474BD1B241353D1_mA39DD78FA3CB7B4FC4475179DECC45F18647B69F_RuntimeMethod_var);
		V_22 = L_245;
		ProductionFriendsListView_t568A80A4F587DE5E0D9156810474BD1B241353D1* L_246 = V_22;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_247;
		L_247 = Object_op_Equality_mB6120F782D83091EF56A198FCEBCF066DB4A9605(L_246, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (L_247)
		{
			goto IL_079f;
		}
	}
	{
		ProductionFriendsListView_t568A80A4F587DE5E0D9156810474BD1B241353D1* L_248 = V_22;
		NullCheck(L_248);
		bool L_249;
		L_249 = ProductionFriendsListView_get_HasRequiredBindings_m16BB6D6BA5619F5A7A67C69AB83653894040E340(L_248, NULL);
		if (L_249)
		{
			goto IL_07aa;
		}
	}

IL_079f:
	{
		InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB* L_250 = (InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB*)il2cpp_codegen_object_new(((RuntimeClass*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB_il2cpp_TypeInfo_var)));
		InvalidOperationException__ctor_mE4CB6F4712AB6D99A2358FBAE2E052B3EE976162(L_250, ((String_t*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&_stringLiteralE790CDA62968F2B91C7535BDA4D82B46C784F6AF)), NULL);
		IL2CPP_RAISE_MANAGED_EXCEPTION(L_250, ((RuntimeMethod*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&ScreenRouter_BuildInteractionLayer_m098546A1A74D7C435B6EAE0086CE1D53AD4D93EF_RuntimeMethod_var)));
	}

IL_07aa:
	{
		ProductionFriendsListView_t568A80A4F587DE5E0D9156810474BD1B241353D1* L_251 = V_22;
		Action_1_t913CB0C3CDC5C59F6B8A1A7D957A63049F5A6BCA* L_252 = (Action_1_t913CB0C3CDC5C59F6B8A1A7D957A63049F5A6BCA*)il2cpp_codegen_object_new(Action_1_t913CB0C3CDC5C59F6B8A1A7D957A63049F5A6BCA_il2cpp_TypeInfo_var);
		Action_1__ctor_m5798433B5A0855FC0015630BF99C272AF00F3D75(L_252, __this, (intptr_t)((void*)ScreenRouter_U3CBuildInteractionLayerU3Eb__85_32_m7E00ECDB658A086BB754CB9F5CDC0C88AA842CD9_RuntimeMethod_var), NULL);
		Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* L_253 = (Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07*)il2cpp_codegen_object_new(Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07_il2cpp_TypeInfo_var);
		Action__ctor_mBDC7B0B4A3F583B64C2896F01BDED360772F67DC(L_253, __this, (intptr_t)((void*)ScreenRouter_NavigateBack_m8755B6CD7EDC8A0A4E6892C2EC74A718571E232B_RuntimeMethod_var), NULL);
		NullCheck(L_251);
		ProductionFriendsListView_Bind_mE38C47533D0DD6595980DD60DB06C267C91B21F9(L_251, L_252, L_253, NULL);
		return;
	}

IL_07ca:
	{
		RectTransform_t6C5DA5E41A89E0F488B001E45E58963480E543A5* L_254 = ___1_parent;
		NullCheck(L_254);
		ProductionInventoryView_tBFCB541C14E4ED40B2CB6633CE17AC0EB9D14E8E* L_255;
		L_255 = Component_GetComponentInChildren_TisProductionInventoryView_tBFCB541C14E4ED40B2CB6633CE17AC0EB9D14E8E_mCC3AAA9A5726E977612F8790700DB37795529949(L_254, Component_GetComponentInChildren_TisProductionInventoryView_tBFCB541C14E4ED40B2CB6633CE17AC0EB9D14E8E_mCC3AAA9A5726E977612F8790700DB37795529949_RuntimeMethod_var);
		V_23 = L_255;
		ProductionInventoryView_tBFCB541C14E4ED40B2CB6633CE17AC0EB9D14E8E* L_256 = V_23;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_257;
		L_257 = Object_op_Equality_mB6120F782D83091EF56A198FCEBCF066DB4A9605(L_256, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (L_257)
		{
			goto IL_07e5;
		}
	}
	{
		ProductionInventoryView_tBFCB541C14E4ED40B2CB6633CE17AC0EB9D14E8E* L_258 = V_23;
		NullCheck(L_258);
		bool L_259;
		L_259 = ProductionInventoryView_get_HasRequiredBindings_mA86EF0DA9574E2F0963703FE53A18B4D14E71FC4(L_258, NULL);
		if (L_259)
		{
			goto IL_07f0;
		}
	}

IL_07e5:
	{
		InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB* L_260 = (InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB*)il2cpp_codegen_object_new(((RuntimeClass*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB_il2cpp_TypeInfo_var)));
		InvalidOperationException__ctor_mE4CB6F4712AB6D99A2358FBAE2E052B3EE976162(L_260, ((String_t*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&_stringLiteralC1F8FD82FEAF272D946DA74AB181475828629082)), NULL);
		IL2CPP_RAISE_MANAGED_EXCEPTION(L_260, ((RuntimeMethod*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&ScreenRouter_BuildInteractionLayer_m098546A1A74D7C435B6EAE0086CE1D53AD4D93EF_RuntimeMethod_var)));
	}

IL_07f0:
	{
		ProductionInventoryView_tBFCB541C14E4ED40B2CB6633CE17AC0EB9D14E8E* L_261 = V_23;
		Action_1_t913CB0C3CDC5C59F6B8A1A7D957A63049F5A6BCA* L_262 = (Action_1_t913CB0C3CDC5C59F6B8A1A7D957A63049F5A6BCA*)il2cpp_codegen_object_new(Action_1_t913CB0C3CDC5C59F6B8A1A7D957A63049F5A6BCA_il2cpp_TypeInfo_var);
		Action_1__ctor_m5798433B5A0855FC0015630BF99C272AF00F3D75(L_262, __this, (intptr_t)((void*)ScreenRouter_U3CBuildInteractionLayerU3Eb__85_33_m3478FDC66F07FE7DEEE2F637B24CBB2C4CD79378_RuntimeMethod_var), NULL);
		Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* L_263 = (Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07*)il2cpp_codegen_object_new(Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07_il2cpp_TypeInfo_var);
		Action__ctor_mBDC7B0B4A3F583B64C2896F01BDED360772F67DC(L_263, __this, (intptr_t)((void*)ScreenRouter_NavigateBack_m8755B6CD7EDC8A0A4E6892C2EC74A718571E232B_RuntimeMethod_var), NULL);
		Action_1_t3CB5D1A819C3ED3F99E9E39F890F18633253949A* L_264 = (Action_1_t3CB5D1A819C3ED3F99E9E39F890F18633253949A*)il2cpp_codegen_object_new(Action_1_t3CB5D1A819C3ED3F99E9E39F890F18633253949A_il2cpp_TypeInfo_var);
		Action_1__ctor_m9DC2953C55C4D7D4B7BEFE03D84DA1F9362D652C(L_264, __this, (intptr_t)((void*)ScreenRouter_U3CBuildInteractionLayerU3Eb__85_34_m0161AB5666F9B89989C6411C279527492243FCFE_RuntimeMethod_var), NULL);
		NullCheck(L_261);
		ProductionInventoryView_Bind_m7E1E06FB348E55DB6B251F5933A3BA4A3643CE38(L_261, L_262, L_263, L_264, NULL);
		return;
	}

IL_081c:
	{
		RectTransform_t6C5DA5E41A89E0F488B001E45E58963480E543A5* L_265 = ___1_parent;
		NullCheck(L_265);
		ProductionSpecialOfferView_t26243C08B7058A2D21A48610B99150458CC7CDD7* L_266;
		L_266 = Component_GetComponentInChildren_TisProductionSpecialOfferView_t26243C08B7058A2D21A48610B99150458CC7CDD7_m2EB6BF8D2D72A3803150E1A35B5FA08F735A4E17(L_265, Component_GetComponentInChildren_TisProductionSpecialOfferView_t26243C08B7058A2D21A48610B99150458CC7CDD7_m2EB6BF8D2D72A3803150E1A35B5FA08F735A4E17_RuntimeMethod_var);
		V_24 = L_266;
		ProductionSpecialOfferView_t26243C08B7058A2D21A48610B99150458CC7CDD7* L_267 = V_24;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_268;
		L_268 = Object_op_Equality_mB6120F782D83091EF56A198FCEBCF066DB4A9605(L_267, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (L_268)
		{
			goto IL_0837;
		}
	}
	{
		ProductionSpecialOfferView_t26243C08B7058A2D21A48610B99150458CC7CDD7* L_269 = V_24;
		NullCheck(L_269);
		bool L_270;
		L_270 = ProductionSpecialOfferView_get_HasRequiredBindings_m86A7EB2236B47DFC48F0BEB9E576DEA8BCA50B42(L_269, NULL);
		if (L_270)
		{
			goto IL_0842;
		}
	}

IL_0837:
	{
		InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB* L_271 = (InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB*)il2cpp_codegen_object_new(((RuntimeClass*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB_il2cpp_TypeInfo_var)));
		InvalidOperationException__ctor_mE4CB6F4712AB6D99A2358FBAE2E052B3EE976162(L_271, ((String_t*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&_stringLiteralA2EE73F3580A4BBBED719A693C88CDB66C9477BC)), NULL);
		IL2CPP_RAISE_MANAGED_EXCEPTION(L_271, ((RuntimeMethod*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&ScreenRouter_BuildInteractionLayer_m098546A1A74D7C435B6EAE0086CE1D53AD4D93EF_RuntimeMethod_var)));
	}

IL_0842:
	{
		ProductionSpecialOfferView_t26243C08B7058A2D21A48610B99150458CC7CDD7* L_272 = V_24;
		Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* L_273 = (Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07*)il2cpp_codegen_object_new(Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07_il2cpp_TypeInfo_var);
		Action__ctor_mBDC7B0B4A3F583B64C2896F01BDED360772F67DC(L_273, __this, (intptr_t)((void*)ScreenRouter_NavigateBack_m8755B6CD7EDC8A0A4E6892C2EC74A718571E232B_RuntimeMethod_var), NULL);
		Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* L_274 = (Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07*)il2cpp_codegen_object_new(Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07_il2cpp_TypeInfo_var);
		Action__ctor_mBDC7B0B4A3F583B64C2896F01BDED360772F67DC(L_274, __this, (intptr_t)((void*)ScreenRouter_U3CBuildInteractionLayerU3Eb__85_35_mBD3B34D91CECAAA38C265B5EAD6A55B16E6DAB96_RuntimeMethod_var), NULL);
		Action_1_t3CB5D1A819C3ED3F99E9E39F890F18633253949A* L_275 = (Action_1_t3CB5D1A819C3ED3F99E9E39F890F18633253949A*)il2cpp_codegen_object_new(Action_1_t3CB5D1A819C3ED3F99E9E39F890F18633253949A_il2cpp_TypeInfo_var);
		Action_1__ctor_m9DC2953C55C4D7D4B7BEFE03D84DA1F9362D652C(L_275, __this, (intptr_t)((void*)ScreenRouter_U3CBuildInteractionLayerU3Eb__85_36_m7AE1101A2E95AC1AB5A7C839802A5BECC654794C_RuntimeMethod_var), NULL);
		NullCheck(L_272);
		ProductionSpecialOfferView_Bind_mA90673C0E923C37E0031C754F5AE61C2DD75A3A1(L_272, L_273, L_274, L_275, NULL);
		return;
	}

IL_086e:
	{
		RectTransform_t6C5DA5E41A89E0F488B001E45E58963480E543A5* L_276 = ___1_parent;
		NullCheck(L_276);
		ProductionLanguageSelectView_tC9B9BF5C49A8106E900C4E16F3C587BAB476E212* L_277;
		L_277 = Component_GetComponentInChildren_TisProductionLanguageSelectView_tC9B9BF5C49A8106E900C4E16F3C587BAB476E212_mC482E7021328C34544943E03EDFAB17A75A453BF(L_276, Component_GetComponentInChildren_TisProductionLanguageSelectView_tC9B9BF5C49A8106E900C4E16F3C587BAB476E212_mC482E7021328C34544943E03EDFAB17A75A453BF_RuntimeMethod_var);
		V_25 = L_277;
		ProductionLanguageSelectView_tC9B9BF5C49A8106E900C4E16F3C587BAB476E212* L_278 = V_25;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_279;
		L_279 = Object_op_Equality_mB6120F782D83091EF56A198FCEBCF066DB4A9605(L_278, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (L_279)
		{
			goto IL_0889;
		}
	}
	{
		ProductionLanguageSelectView_tC9B9BF5C49A8106E900C4E16F3C587BAB476E212* L_280 = V_25;
		NullCheck(L_280);
		bool L_281;
		L_281 = ProductionLanguageSelectView_get_HasRequiredBindings_mEBDF2CB9EAB2D6BD527490446CF4916667597C2D(L_280, NULL);
		if (L_281)
		{
			goto IL_0894;
		}
	}

IL_0889:
	{
		InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB* L_282 = (InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB*)il2cpp_codegen_object_new(((RuntimeClass*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB_il2cpp_TypeInfo_var)));
		InvalidOperationException__ctor_mE4CB6F4712AB6D99A2358FBAE2E052B3EE976162(L_282, ((String_t*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&_stringLiteralBA9872336A3E9C4D6C4CDDD8BAF66CF20F64359D)), NULL);
		IL2CPP_RAISE_MANAGED_EXCEPTION(L_282, ((RuntimeMethod*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&ScreenRouter_BuildInteractionLayer_m098546A1A74D7C435B6EAE0086CE1D53AD4D93EF_RuntimeMethod_var)));
	}

IL_0894:
	{
		ProductionLanguageSelectView_tC9B9BF5C49A8106E900C4E16F3C587BAB476E212* L_283 = V_25;
		Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* L_284 = (Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07*)il2cpp_codegen_object_new(Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07_il2cpp_TypeInfo_var);
		Action__ctor_mBDC7B0B4A3F583B64C2896F01BDED360772F67DC(L_284, __this, (intptr_t)((void*)ScreenRouter_NavigateBack_m8755B6CD7EDC8A0A4E6892C2EC74A718571E232B_RuntimeMethod_var), NULL);
		NullCheck(L_283);
		ProductionLanguageSelectView_Bind_m147CEB21339F892BAC0AFBA2B222373B4CBF52F2(L_283, L_284, NULL);
		return;
	}

IL_08a8:
	{
		RectTransform_t6C5DA5E41A89E0F488B001E45E58963480E543A5* L_285 = ___1_parent;
		NullCheck(L_285);
		ProductionLoginView_t36729B7F5B697B3828DE80A1A0EB2509C7F66B81* L_286;
		L_286 = Component_GetComponentInChildren_TisProductionLoginView_t36729B7F5B697B3828DE80A1A0EB2509C7F66B81_mB364C4DEC9E8A2A8378CB63891CFE9A294BDE258(L_285, Component_GetComponentInChildren_TisProductionLoginView_t36729B7F5B697B3828DE80A1A0EB2509C7F66B81_mB364C4DEC9E8A2A8378CB63891CFE9A294BDE258_RuntimeMethod_var);
		V_26 = L_286;
		ProductionLoginView_t36729B7F5B697B3828DE80A1A0EB2509C7F66B81* L_287 = V_26;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_288;
		L_288 = Object_op_Equality_mB6120F782D83091EF56A198FCEBCF066DB4A9605(L_287, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (L_288)
		{
			goto IL_08c3;
		}
	}
	{
		ProductionLoginView_t36729B7F5B697B3828DE80A1A0EB2509C7F66B81* L_289 = V_26;
		NullCheck(L_289);
		bool L_290;
		L_290 = ProductionLoginView_get_HasRequiredBindings_mA03C54A18823A10B93ADC0EF9D3A69F0C79A9404(L_289, NULL);
		if (L_290)
		{
			goto IL_08ce;
		}
	}

IL_08c3:
	{
		InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB* L_291 = (InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB*)il2cpp_codegen_object_new(((RuntimeClass*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB_il2cpp_TypeInfo_var)));
		InvalidOperationException__ctor_mE4CB6F4712AB6D99A2358FBAE2E052B3EE976162(L_291, ((String_t*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&_stringLiteral0B516F9EB595E2D91FB4E375423185098DD15038)), NULL);
		IL2CPP_RAISE_MANAGED_EXCEPTION(L_291, ((RuntimeMethod*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&ScreenRouter_BuildInteractionLayer_m098546A1A74D7C435B6EAE0086CE1D53AD4D93EF_RuntimeMethod_var)));
	}

IL_08ce:
	{
		ProductionLoginView_t36729B7F5B697B3828DE80A1A0EB2509C7F66B81* L_292 = V_26;
		Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* L_293 = (Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07*)il2cpp_codegen_object_new(Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07_il2cpp_TypeInfo_var);
		Action__ctor_mBDC7B0B4A3F583B64C2896F01BDED360772F67DC(L_293, __this, (intptr_t)((void*)ScreenRouter_ContinueAsDemoGuest_mA3B54A9162ADA3DCA8020BD7F2A6494277A79140_RuntimeMethod_var), NULL);
		Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* L_294 = (Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07*)il2cpp_codegen_object_new(Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07_il2cpp_TypeInfo_var);
		Action__ctor_mBDC7B0B4A3F583B64C2896F01BDED360772F67DC(L_294, __this, (intptr_t)((void*)ScreenRouter_NavigateBack_m8755B6CD7EDC8A0A4E6892C2EC74A718571E232B_RuntimeMethod_var), NULL);
		NullCheck(L_292);
		ProductionLoginView_Bind_m396120ECACFB89BB21444CA9FB5C8DD267D361AF(L_292, L_293, L_294, NULL);
		return;
	}

IL_08ee:
	{
		RectTransform_t6C5DA5E41A89E0F488B001E45E58963480E543A5* L_295 = ___1_parent;
		NullCheck(L_295);
		ProductionTutorialView_t17A2D559FD4E71F841E57E107C3B8FADDC3DA4CB* L_296;
		L_296 = Component_GetComponentInChildren_TisProductionTutorialView_t17A2D559FD4E71F841E57E107C3B8FADDC3DA4CB_m47D948D0758AF603AAE420B79DA907127ED01965(L_295, Component_GetComponentInChildren_TisProductionTutorialView_t17A2D559FD4E71F841E57E107C3B8FADDC3DA4CB_m47D948D0758AF603AAE420B79DA907127ED01965_RuntimeMethod_var);
		V_27 = L_296;
		ProductionTutorialView_t17A2D559FD4E71F841E57E107C3B8FADDC3DA4CB* L_297 = V_27;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_298;
		L_298 = Object_op_Equality_mB6120F782D83091EF56A198FCEBCF066DB4A9605(L_297, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (L_298)
		{
			goto IL_0909;
		}
	}
	{
		ProductionTutorialView_t17A2D559FD4E71F841E57E107C3B8FADDC3DA4CB* L_299 = V_27;
		NullCheck(L_299);
		bool L_300;
		L_300 = ProductionTutorialView_get_HasRequiredBindings_m5767DF56AFE5B2DDDD3E4DA6650C22C71D5BA1B1(L_299, NULL);
		if (L_300)
		{
			goto IL_0914;
		}
	}

IL_0909:
	{
		InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB* L_301 = (InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB*)il2cpp_codegen_object_new(((RuntimeClass*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB_il2cpp_TypeInfo_var)));
		InvalidOperationException__ctor_mE4CB6F4712AB6D99A2358FBAE2E052B3EE976162(L_301, ((String_t*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&_stringLiteral1832313EA54BFA396EDBFC5A10C754EC0A1FC03F)), NULL);
		IL2CPP_RAISE_MANAGED_EXCEPTION(L_301, ((RuntimeMethod*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&ScreenRouter_BuildInteractionLayer_m098546A1A74D7C435B6EAE0086CE1D53AD4D93EF_RuntimeMethod_var)));
	}

IL_0914:
	{
		ProductionTutorialView_t17A2D559FD4E71F841E57E107C3B8FADDC3DA4CB* L_302 = V_27;
		Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* L_303 = (Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07*)il2cpp_codegen_object_new(Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07_il2cpp_TypeInfo_var);
		Action__ctor_mBDC7B0B4A3F583B64C2896F01BDED360772F67DC(L_303, __this, (intptr_t)((void*)ScreenRouter_NavigateBack_m8755B6CD7EDC8A0A4E6892C2EC74A718571E232B_RuntimeMethod_var), NULL);
		Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* L_304 = (Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07*)il2cpp_codegen_object_new(Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07_il2cpp_TypeInfo_var);
		Action__ctor_mBDC7B0B4A3F583B64C2896F01BDED360772F67DC(L_304, __this, (intptr_t)((void*)ScreenRouter_U3CBuildInteractionLayerU3Eb__85_37_mAA83FAF83307F0A99B4D1E49ABD51A8F08D694F2_RuntimeMethod_var), NULL);
		Action_1_t913CB0C3CDC5C59F6B8A1A7D957A63049F5A6BCA* L_305 = (Action_1_t913CB0C3CDC5C59F6B8A1A7D957A63049F5A6BCA*)il2cpp_codegen_object_new(Action_1_t913CB0C3CDC5C59F6B8A1A7D957A63049F5A6BCA_il2cpp_TypeInfo_var);
		Action_1__ctor_m5798433B5A0855FC0015630BF99C272AF00F3D75(L_305, __this, (intptr_t)((void*)ScreenRouter_U3CBuildInteractionLayerU3Eb__85_38_mD65FF4DC09B6A87D55760923472FB858CCBD9B6F_RuntimeMethod_var), NULL);
		Action_1_t3CB5D1A819C3ED3F99E9E39F890F18633253949A* L_306 = (Action_1_t3CB5D1A819C3ED3F99E9E39F890F18633253949A*)il2cpp_codegen_object_new(Action_1_t3CB5D1A819C3ED3F99E9E39F890F18633253949A_il2cpp_TypeInfo_var);
		Action_1__ctor_m9DC2953C55C4D7D4B7BEFE03D84DA1F9362D652C(L_306, __this, (intptr_t)((void*)ScreenRouter_U3CBuildInteractionLayerU3Eb__85_39_m32DB0ABB961515770DAA4EAA9AC9D195D3552E8E_RuntimeMethod_var), NULL);
		NullCheck(L_302);
		ProductionTutorialView_Bind_m2DA7ECEF1A5FE9DD97B8D5D2C7D9034EEAA30E3D(L_302, L_303, L_304, L_305, (bool)0, L_306, NULL);
		return;
	}

IL_094d:
	{
		RectTransform_t6C5DA5E41A89E0F488B001E45E58963480E543A5* L_307 = ___1_parent;
		NullCheck(L_307);
		ProductionOfflineView_t2A35DC0266DAE631DB63C259E5095E1F51E46451* L_308;
		L_308 = Component_GetComponentInChildren_TisProductionOfflineView_t2A35DC0266DAE631DB63C259E5095E1F51E46451_mDE12F1909962FD311F5F0F62E78371673DCA0E59(L_307, Component_GetComponentInChildren_TisProductionOfflineView_t2A35DC0266DAE631DB63C259E5095E1F51E46451_mDE12F1909962FD311F5F0F62E78371673DCA0E59_RuntimeMethod_var);
		V_28 = L_308;
		ProductionOfflineView_t2A35DC0266DAE631DB63C259E5095E1F51E46451* L_309 = V_28;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_310;
		L_310 = Object_op_Equality_mB6120F782D83091EF56A198FCEBCF066DB4A9605(L_309, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (L_310)
		{
			goto IL_0968;
		}
	}
	{
		ProductionOfflineView_t2A35DC0266DAE631DB63C259E5095E1F51E46451* L_311 = V_28;
		NullCheck(L_311);
		bool L_312;
		L_312 = ProductionOfflineView_get_HasRequiredBindings_mDA400CD3BB0253E47063A47ED90CB6276E539162(L_311, NULL);
		if (L_312)
		{
			goto IL_0973;
		}
	}

IL_0968:
	{
		InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB* L_313 = (InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB*)il2cpp_codegen_object_new(((RuntimeClass*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB_il2cpp_TypeInfo_var)));
		InvalidOperationException__ctor_mE4CB6F4712AB6D99A2358FBAE2E052B3EE976162(L_313, ((String_t*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&_stringLiteral78ABF3DDB4BEF97A92F5862AF61D27E9DD47E7B5)), NULL);
		IL2CPP_RAISE_MANAGED_EXCEPTION(L_313, ((RuntimeMethod*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&ScreenRouter_BuildInteractionLayer_m098546A1A74D7C435B6EAE0086CE1D53AD4D93EF_RuntimeMethod_var)));
	}

IL_0973:
	{
		ProductionOfflineView_t2A35DC0266DAE631DB63C259E5095E1F51E46451* L_314 = V_28;
		Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* L_315 = (Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07*)il2cpp_codegen_object_new(Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07_il2cpp_TypeInfo_var);
		Action__ctor_mBDC7B0B4A3F583B64C2896F01BDED360772F67DC(L_315, __this, (intptr_t)((void*)ScreenRouter_NavigateBack_m8755B6CD7EDC8A0A4E6892C2EC74A718571E232B_RuntimeMethod_var), NULL);
		Action_1_t913CB0C3CDC5C59F6B8A1A7D957A63049F5A6BCA* L_316 = (Action_1_t913CB0C3CDC5C59F6B8A1A7D957A63049F5A6BCA*)il2cpp_codegen_object_new(Action_1_t913CB0C3CDC5C59F6B8A1A7D957A63049F5A6BCA_il2cpp_TypeInfo_var);
		Action_1__ctor_m5798433B5A0855FC0015630BF99C272AF00F3D75(L_316, __this, (intptr_t)((void*)ScreenRouter_U3CBuildInteractionLayerU3Eb__85_40_mE134B961FD87D2DB2CCE6566EC16EC65BFA5041A_RuntimeMethod_var), NULL);
		Action_1_t3CB5D1A819C3ED3F99E9E39F890F18633253949A* L_317 = (Action_1_t3CB5D1A819C3ED3F99E9E39F890F18633253949A*)il2cpp_codegen_object_new(Action_1_t3CB5D1A819C3ED3F99E9E39F890F18633253949A_il2cpp_TypeInfo_var);
		Action_1__ctor_m9DC2953C55C4D7D4B7BEFE03D84DA1F9362D652C(L_317, __this, (intptr_t)((void*)ScreenRouter_U3CBuildInteractionLayerU3Eb__85_41_mC5D0006D724BCE91BEEE26EF1A24731F63FCBCC7_RuntimeMethod_var), NULL);
		NullCheck(L_314);
		ProductionOfflineView_Bind_mE99EE07935CEDDA7CA4930D3FDB78547E92A4BB1(L_314, L_315, (Func_1_tF8D036A4906DE44ED371866B42F5C8835105FA50*)NULL, L_316, L_317, NULL);
		return;
	}

IL_09a0:
	{
		RectTransform_t6C5DA5E41A89E0F488B001E45E58963480E543A5* L_318 = ___1_parent;
		NullCheck(L_318);
		ProductionMaintenanceView_tE78AA4EB7FC8F41C25AF278B1341DAA6B8F091A1* L_319;
		L_319 = Component_GetComponentInChildren_TisProductionMaintenanceView_tE78AA4EB7FC8F41C25AF278B1341DAA6B8F091A1_mBC7AFB6C8BAA17700B77BCEF2981DD586B8F25D0(L_318, Component_GetComponentInChildren_TisProductionMaintenanceView_tE78AA4EB7FC8F41C25AF278B1341DAA6B8F091A1_mBC7AFB6C8BAA17700B77BCEF2981DD586B8F25D0_RuntimeMethod_var);
		V_29 = L_319;
		ProductionMaintenanceView_tE78AA4EB7FC8F41C25AF278B1341DAA6B8F091A1* L_320 = V_29;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_321;
		L_321 = Object_op_Equality_mB6120F782D83091EF56A198FCEBCF066DB4A9605(L_320, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (L_321)
		{
			goto IL_09bb;
		}
	}
	{
		ProductionMaintenanceView_tE78AA4EB7FC8F41C25AF278B1341DAA6B8F091A1* L_322 = V_29;
		NullCheck(L_322);
		bool L_323;
		L_323 = ProductionMaintenanceView_get_HasRequiredBindings_mB172155797E34F4455D0747517F55B2E54840D5E(L_322, NULL);
		if (L_323)
		{
			goto IL_09c6;
		}
	}

IL_09bb:
	{
		InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB* L_324 = (InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB*)il2cpp_codegen_object_new(((RuntimeClass*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB_il2cpp_TypeInfo_var)));
		InvalidOperationException__ctor_mE4CB6F4712AB6D99A2358FBAE2E052B3EE976162(L_324, ((String_t*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&_stringLiteral3602993764A5F6008EFF15EC5913D112032ACC51)), NULL);
		IL2CPP_RAISE_MANAGED_EXCEPTION(L_324, ((RuntimeMethod*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&ScreenRouter_BuildInteractionLayer_m098546A1A74D7C435B6EAE0086CE1D53AD4D93EF_RuntimeMethod_var)));
	}

IL_09c6:
	{
		ProductionMaintenanceView_tE78AA4EB7FC8F41C25AF278B1341DAA6B8F091A1* L_325 = V_29;
		Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* L_326 = (Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07*)il2cpp_codegen_object_new(Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07_il2cpp_TypeInfo_var);
		Action__ctor_mBDC7B0B4A3F583B64C2896F01BDED360772F67DC(L_326, __this, (intptr_t)((void*)ScreenRouter_NavigateBack_m8755B6CD7EDC8A0A4E6892C2EC74A718571E232B_RuntimeMethod_var), NULL);
		NullCheck(L_325);
		ProductionMaintenanceView_Bind_m9C90258D69486D14F3F15A78BEF1543C4FE45314(L_325, L_326, NULL);
		return;
	}

IL_09da:
	{
		RectTransform_t6C5DA5E41A89E0F488B001E45E58963480E543A5* L_327 = ___1_parent;
		NullCheck(L_327);
		ProductionChapterCompleteView_t8F3C64B99DD107C039783DF1D0A39530DDAA9F70* L_328;
		L_328 = Component_GetComponentInChildren_TisProductionChapterCompleteView_t8F3C64B99DD107C039783DF1D0A39530DDAA9F70_mBAF44037DDF939EBA0D1038A72DB9B112B50FC73(L_327, Component_GetComponentInChildren_TisProductionChapterCompleteView_t8F3C64B99DD107C039783DF1D0A39530DDAA9F70_mBAF44037DDF939EBA0D1038A72DB9B112B50FC73_RuntimeMethod_var);
		V_30 = L_328;
		ProductionChapterCompleteView_t8F3C64B99DD107C039783DF1D0A39530DDAA9F70* L_329 = V_30;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_330;
		L_330 = Object_op_Equality_mB6120F782D83091EF56A198FCEBCF066DB4A9605(L_329, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (L_330)
		{
			goto IL_09f5;
		}
	}
	{
		ProductionChapterCompleteView_t8F3C64B99DD107C039783DF1D0A39530DDAA9F70* L_331 = V_30;
		NullCheck(L_331);
		bool L_332;
		L_332 = ProductionChapterCompleteView_get_HasRequiredBindings_m05B6A19C59BC128908338CF75C4317579F8C8E2F(L_331, NULL);
		if (L_332)
		{
			goto IL_0a00;
		}
	}

IL_09f5:
	{
		InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB* L_333 = (InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB*)il2cpp_codegen_object_new(((RuntimeClass*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB_il2cpp_TypeInfo_var)));
		InvalidOperationException__ctor_mE4CB6F4712AB6D99A2358FBAE2E052B3EE976162(L_333, ((String_t*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&_stringLiteralA07BB88AD0A406559B0BF50D446F7734E19B6339)), NULL);
		IL2CPP_RAISE_MANAGED_EXCEPTION(L_333, ((RuntimeMethod*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&ScreenRouter_BuildInteractionLayer_m098546A1A74D7C435B6EAE0086CE1D53AD4D93EF_RuntimeMethod_var)));
	}

IL_0a00:
	{
		ProductionChapterCompleteView_t8F3C64B99DD107C039783DF1D0A39530DDAA9F70* L_334 = V_30;
		Func_1_t2BE7F58348C9CC544A8973B3A9E55541DE43C457* L_335 = (Func_1_t2BE7F58348C9CC544A8973B3A9E55541DE43C457*)il2cpp_codegen_object_new(Func_1_t2BE7F58348C9CC544A8973B3A9E55541DE43C457_il2cpp_TypeInfo_var);
		Func_1__ctor_mDFFAE9C73346372438B5B04C4558AC42F1A3DA22(L_335, __this, (intptr_t)((void*)ScreenRouter_U3CBuildInteractionLayerU3Eb__85_42_m1800D31859AF9C79953636469D38789894FB3CBD_RuntimeMethod_var), NULL);
		NullCheck(L_334);
		ProductionChapterCompleteView_Bind_mCE68E40BB0CA52850D400F0F3C51723F3B7E6D37(L_334, L_335, NULL);
		return;
	}

IL_0a14:
	{
		RectTransform_t6C5DA5E41A89E0F488B001E45E58963480E543A5* L_336 = ___1_parent;
		Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* L_337 = (Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07*)il2cpp_codegen_object_new(Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07_il2cpp_TypeInfo_var);
		Action__ctor_mBDC7B0B4A3F583B64C2896F01BDED360772F67DC(L_337, __this, (intptr_t)((void*)ScreenRouter_U3CBuildInteractionLayerU3Eb__85_43_m4A3D1532E3BC1CC7B878E27621B41A074293CB6F_RuntimeMethod_var), NULL);
		ScreenRouter_Hotspot_m6FB76C3D7739C724E2AC8A2D8A12CC960248D44D(__this, L_336, _stringLiteral4A261EB7E7319776625F5A015EA18053797E6890, (0.0f), (0.0f), (180.0f), (180.0f), L_337, 0, (bool)0, NULL);
	}

IL_0a42:
	{
		return;
	}
}
