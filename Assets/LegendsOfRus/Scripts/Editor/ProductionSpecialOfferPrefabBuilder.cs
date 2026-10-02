#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEngine;
namespace LegendsOfRus.Editor
{
    public static class ProductionSpecialOfferPrefabBuilder
    {
        public const string PrefabPath="Assets/LegendsOfRus/Resources/ProductionUI/Pages/SpecialOffer.prefab";
        [MenuItem("Legends of Rus/Production UI/Rebuild SpecialOffer Alignment")]
        public static void Rebuild()
        {
            if(EditorApplication.isPlayingOrWillChangePlaymode)throw new InvalidOperationException("Close Play Mode before rebuilding SpecialOffer.");
            AssetDatabase.Refresh();
            foreach(var file in Directory.GetFiles("Assets/LegendsOfRus/Resources/ProductionUI/SpecialOfferAligned","*.png"))
            {
                var importer=AssetImporter.GetAtPath(file) as TextureImporter;
                if(importer==null)throw new InvalidOperationException(file);
                LockedDesignImporter.ConfigureShopTexture(importer);importer.SaveAndReimport();
            }
            bool existing=File.Exists(PrefabPath);GameObject root=existing?PrefabUtility.LoadPrefabContents(PrefabPath):new GameObject("SpecialOffer",typeof(RectTransform));
            try
            {
                var rect=(RectTransform)root.transform;rect.anchorMin=Vector2.zero;rect.anchorMax=Vector2.one;rect.offsetMin=rect.offsetMax=Vector2.zero;root.layer=5;
                var marker=root.GetComponent<ProductionCoreLoopPageMarker>()??root.AddComponent<ProductionCoreLoopPageMarker>();marker.Configure(ScreenId.SpecialOffer);
                ProductionSpecialOfferRuntime.BuildInto(rect);var view=root.GetComponent<ProductionSpecialOfferView>();view.RefreshContent();
                if(!view.HasRequiredBindings)throw new InvalidOperationException("SpecialOffer serialized bindings are incomplete.");
                PrefabUtility.SaveAsPrefabAsset(root,PrefabPath);
            }
            finally{if(existing)PrefabUtility.UnloadPrefabContents(root);else UnityEngine.Object.DestroyImmediate(root);}
            string state="Assets/LegendsOfRus/Resources/ProductionUI/CutoverState/SpecialOfferCutoverState.txt";
            File.WriteAllText(state,"{\n  \"enabled\": true,\n  \"magic\": \"v1.20-step10.3-meta-page-development-cutover:SpecialOffer\",\n  \"revision\": 4,\n  \"purchaseCatalog\": \"unavailable\"\n}\n");
            AssetDatabase.ImportAsset(state);AssetDatabase.SaveAssets();
            var saved=AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);if(saved==null||!saved.GetComponent<ProductionSpecialOfferView>().HasRequiredBindings)throw new InvalidOperationException("SpecialOffer prefab readback failed.");
            var savedView=saved.GetComponent<ProductionSpecialOfferView>();
            var chest=AssetDatabase.LoadAssetAtPath<Sprite>("Assets/LegendsOfRus/Resources/ProductionUI/SpecialOfferAligned/hero_source_r2.png");
            if(chest==null||savedView.Hero.sprite!=chest||savedView.RewardIcons[2].sprite!=chest)throw new InvalidOperationException("SpecialOffer fresh chest readback failed.");
            if(!savedView.PlayerHeader.Capsules[2].gameObject.activeSelf||savedView.EnergyText.transform.parent!=savedView.PlayerHeader.Capsules[2].transform)throw new InvalidOperationException("SpecialOffer shared energy indicator is missing.");
            if(savedView.ShopButton.gameObject.activeSelf)throw new InvalidOperationException("SpecialOffer Store button is still visible.");
            Debug.Log("SPECIAL_OFFER_STORE_ENERGY_R3_REBUILD_PASS: Store button removed, full shared resources, revision and cutover readback.");
        }
    }
}
#endif
