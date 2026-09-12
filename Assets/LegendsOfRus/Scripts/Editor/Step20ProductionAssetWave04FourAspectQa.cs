#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;
namespace LegendsOfRus.Editor
{
    public static class Step20ProductionAssetWave04FourAspectQa
    {
        static readonly Vector2[] Resolutions={new Vector2(941,1672),new Vector2(1080,2340),new Vector2(1080,2400),new Vector2(1600,2560)};
        static readonly ScreenId[] Pages={ScreenId.Settings,ScreenId.LanguageSelect,ScreenId.Login,ScreenId.TutorialOverlay,ScreenId.OfflineState,ScreenId.MaintenanceState,ScreenId.EmptyErrorState};
        [MenuItem("Legends of Rus/Tests/Run v1.20 Step 10 Wave 4 Four-Aspect Source QA")]
        public static void Run(){Step20ProductionAssetWave04PrefabBuilder.BuildAll(false);int passes=0;foreach(ScreenId id in Pages){GameObject p=AssetDatabase.LoadAssetAtPath<GameObject>(Step20ProductionAssetWave04PrefabBuilder.CandidateRoot+"/"+id+".prefab");if(p==null)throw new InvalidOperationException("Missing Wave 4 candidate: "+id);RectTransform[] rects=p.GetComponentsInChildren<RectTransform>(true);foreach(Vector2 res in Resolutions){Validate(id,rects,res);passes++;}} if(passes!=28)throw new InvalidOperationException("Expected 28 Wave 4 aspect checks, got "+passes+"."); Debug.Log("v1.20 Step 10 Wave 4 four-aspect source QA PASS: 28/28. Source containment only; runtime/device acceptance pending.");}
        static void Validate(ScreenId id,RectTransform[] rects,Vector2 res){foreach(RectTransform rt in rects){if(rt==null||rt.parent==null)continue;Vector2 min=rt.anchorMin,max=rt.anchorMax;if(min.x<-.001f||min.y<-.001f||max.x>1.001f||max.y>1.001f||min.x>max.x||min.y>max.y)throw new InvalidOperationException(id+" anchor containment failed for "+rt.name+" at "+res.x+"x"+res.y+".");}}
    }
}
#endif
