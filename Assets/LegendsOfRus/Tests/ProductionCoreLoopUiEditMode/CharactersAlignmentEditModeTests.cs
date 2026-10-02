#if UNITY_EDITOR
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
namespace LegendsOfRus.Tests.ProductionCoreLoopUi {
 public sealed class CharactersAlignmentEditModeTests {
 const string Path="Assets/LegendsOfRus/Resources/ProductionUI/Pages/Characters.prefab";
 [Test] public void NativePageHasCompleteBindingsAndNoScreenshotDependencies(){
 var root=AssetDatabase.LoadAssetAtPath<GameObject>(Path);Assert.That(root,Is.Not.Null);var v=root.GetComponent<ProductionCharactersView>();Assert.That(v.HasRequiredBindings,Is.True);Assert.That(v.HasHeroOwnershipService,Is.False);Assert.That(v.CanUpgrade,Is.False);Assert.That(v.UpgradeButton.interactable||v.SelectButton.interactable,Is.False);
 Assert.That(root.GetComponent<ProductionCoreLoopPageMarker>().ScreenshotFree,Is.True);Assert.That(root.GetComponentsInChildren<Button>(true).Length,Is.GreaterThanOrEqualTo(17));Assert.That(root.GetComponentsInChildren<Canvas>(true),Is.Empty);Assert.That(root.GetComponentsInChildren<AnimatedHotspot>(true),Is.Empty);
 Assert.That(v.CollectionScroll.horizontal,Is.False);Assert.That(v.CollectionScroll.vertical,Is.True);Assert.That(v.CollectionScroll.movementType,Is.EqualTo(ScrollRect.MovementType.Clamped));Assert.That(v.CollectionViewport.GetComponent<RectMask2D>(),Is.Not.Null);
 Assert.That(v.CollectionScroll.content,Is.SameAs(v.CollectionContent));Assert.That(v.CollectionScroll.viewport,Is.SameAs(v.CollectionViewport));
 Assert.That(v.InfoButton.transform.IsChildOf(v.CollectionContent)||v.BackButton.transform.IsChildOf(v.CollectionContent),Is.False);
 var portrait=(RectTransform)v.Body.Find("PortraitWindow");var portraitFrame=v.Body.Find("PortraitFrame");
 Assert.That(portraitFrame.GetSiblingIndex(),Is.GreaterThan(portrait.GetSiblingIndex()),"Frame conceals source portrait cutoff");
 Assert.That(v.DetailArt.sprite.name,Is.EqualTo("hero_firebird_r7"));Assert.That(portrait.Find("PortraitBackdrop").GetComponent<Image>().sprite.name,Is.EqualTo("sunset_r7"));
 Assert.That(v.DetailArt.rectTransform.anchoredPosition.y-v.DetailArt.rectTransform.rect.height,Is.LessThan(-portrait.rect.height),"Original subject extends below the viewport");
 Assert.That(v.Heading.font,Is.SameAs(v.TitleFont));Assert.That(v.Heading.material.shader.name,Is.EqualTo("LegendsOfRus/UI/HomePlayGold"));
 Assert.That(v.Heading.GetComponent<ProductionHomeGoldText>().BeveledFace,Is.True);
 Assert.That(v.Heading.raycastTarget,Is.False);
 Assert.That(v.OuterFrame.GetComponentsInChildren<Button>(true),Is.Empty);
 foreach(var decoration in v.OuterFrame.GetComponentsInChildren<Graphic>(true))Assert.That(decoration.raycastTarget,Is.False,"Carved perimeter cannot intercept scrolling or controls");
 Assert.That(v.OuterFrame.GetSiblingIndex(),Is.LessThan(v.Hud.GetSiblingIndex()));
 Assert.That(v.OuterFrame.GetSiblingIndex(),Is.LessThan(v.CollectionViewport.GetSiblingIndex()));
 Assert.That(v.OuterFrame.GetSiblingIndex(),Is.LessThan(v.Title.transform.GetSiblingIndex()));
 var footer=v.Body.Find("CarvedFooter").GetComponent<Image>();Assert.That(footer.sprite.name,Is.EqualTo("footer_backing_r8"));Assert.That(footer.raycastTarget,Is.False);
 string[] footerNames={"map","shop","events","team"};for(int i=0;i<4;i++){
 Assert.That(v.NavButtons[i].GetComponent<Image>().sprite.name,Is.EqualTo("nav_"+footerNames[i]+"_r8"));
 Assert.That(v.NavButtons[i].transform.Find("Plaque").GetComponent<Image>().sprite.name,Is.EqualTo("nav_caption_r8"));
 Assert.That(v.NavLabels[i].transform.GetSiblingIndex(),Is.GreaterThan(v.NavButtons[i].transform.Find("Plaque").GetSiblingIndex()),"Live localized caption remains above source plaque");
 }
 foreach(var dep in AssetDatabase.GetDependencies(Path,true)){Assert.That(dep,Does.Not.Contain("ReferenceOnly"));Assert.That(dep,Does.Not.Contain("DesignScreens"));}
 foreach(var image in root.GetComponentsInChildren<Image>(true))Assert.That(image.sprite,Is.Not.Null,image.name);Assert.That(v.InfoPanel.activeSelf,Is.False);
 for(int i=0;i<8;i++){
 var window=v.Cards[i].transform.Find("PortraitWindow");var bust=window.Find("Portrait").GetComponent<Image>();var scenery=window.Find("CleanScenery").GetComponent<Image>();
 Assert.That(bust.sprite.name,Is.EqualTo("card_portrait_"+i+"_r12"));Assert.That(bust.raycastTarget||scenery.raycastTarget,Is.False);
 Assert.That(bust.preserveAspect,Is.True);Assert.That(bust.transform.GetSiblingIndex(),Is.GreaterThan(scenery.transform.GetSiblingIndex()));
 Assert.That(AssetDatabase.GetAssetPath(scenery.sprite),Does.StartWith("Assets/LegendsOfRus/Resources/ProductionUI/CharactersAligned/card_"));
 }
 for(int i=0;i<8;i++)Assert.That(v.CardStatuses[i].transform.GetSiblingIndex(),Is.GreaterThan(v.CardFrames[i].transform.GetSiblingIndex()),"Frame must not obscure preview caption");
 foreach(var future in v.FutureCards){Assert.That(future.GetComponent<Image>().sprite.name,Is.EqualTo("card_normal_r6"));Assert.That(future.transform.Find("FrameEdge").GetComponent<Image>().sprite.name,Is.EqualTo("card_normal_r6"));}
 for(int i=0;i<4;i++)Assert.That(v.FutureLabels[i].transform.GetSiblingIndex(),Is.GreaterThan(v.FutureCards[i].transform.Find("FrameEdge").GetSiblingIndex()),"Frame must not obscure future label");
 }
 [Test]public void BuilderIsIdempotent(){var root=Object.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>(Path));try{int count=root.GetComponentsInChildren<Transform>(true).Length;ProductionCharactersRuntime.BuildInto((RectTransform)root.transform);ProductionCharactersRuntime.BuildInto((RectTransform)root.transform);Assert.That(root.GetComponentsInChildren<Transform>(true).Length,Is.EqualTo(count));}finally{Object.DestroyImmediate(root);}}
 }
}
#endif
