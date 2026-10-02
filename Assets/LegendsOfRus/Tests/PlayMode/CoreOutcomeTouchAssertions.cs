using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
namespace LegendsOfRus.Tests.PlayMode
{
    internal static class CoreOutcomeTouchAssertions
    {
        [Serializable] sealed class Evidence {public string page,language;public Rect artwork,touch,safe;public Vector2 sampledPoint;public bool nativeOwnerReached;}
        internal static Rect Bounds(RectTransform r,Camera c){var p=new Vector3[4];r.GetWorldCorners(p);var a=RectTransformUtility.WorldToScreenPoint(c,p[0]);var b=RectTransformUtility.WorldToScreenPoint(c,p[2]);return Rect.MinMaxRect(a.x,a.y,b.x,b.y);}
        internal static GameObject Check(Button button,Camera camera,Rect safe,string page,string size)
        {
            var target=button.transform.Find("TouchTarget").GetComponent<ProductionControlTouchTarget>();Assert.That(target.Owner,Is.SameAs(button));Assert.That(button.targetGraphic,Is.Not.SameAs(target));
            Rect artwork=Bounds((RectTransform)button.transform,camera),touch=Bounds(target.rectTransform,camera);
            Assert.That(touch.width,Is.GreaterThanOrEqualTo(43.9f));Assert.That(touch.height,Is.GreaterThanOrEqualTo(43.9f));Assert.That(touch.xMin,Is.GreaterThanOrEqualTo(safe.xMin-.1f));Assert.That(touch.yMin,Is.GreaterThanOrEqualTo(safe.yMin-.1f));Assert.That(touch.xMax,Is.LessThanOrEqualTo(safe.xMax+.1f));Assert.That(touch.yMax,Is.LessThanOrEqualTo(safe.yMax+.1f));
            Vector2 point=touch.center;
            if(touch.yMax>artwork.yMax+1)point.y=touch.yMax-.4f;else if(touch.yMin<artwork.yMin-1)point.y=touch.yMin+.4f;else if(touch.xMin<artwork.xMin-1)point.x=touch.xMin+.4f;
            var hits=new List<RaycastResult>();EventSystem.current.RaycastAll(new PointerEventData(EventSystem.current){position=point},hits);Assert.That(hits.Count,Is.GreaterThan(0));Assert.That(hits[0].gameObject.GetComponentInParent<Button>(),Is.SameAs(button));
            var dir=Environment.GetEnvironmentVariable("LOR_CORE_OUTCOME_READABILITY_DIR");if(!string.IsNullOrEmpty(dir))File.WriteAllText(Path.Combine(dir,"Touch_"+page+"_"+DemoFeatureState.LanguageCode+"_"+size+".json"),JsonUtility.ToJson(new Evidence{page=page,language=DemoFeatureState.LanguageCode,artwork=artwork,touch=touch,safe=safe,sampledPoint=point,nativeOwnerReached=true},true));
            return hits[0].gameObject;
        }
        internal static void Click(GameObject target)
        {
            var pointer=new PointerEventData(EventSystem.current){button=PointerEventData.InputButton.Left};
            ExecuteEvents.ExecuteHierarchy(target,pointer,ExecuteEvents.pointerDownHandler);ExecuteEvents.ExecuteHierarchy(target,pointer,ExecuteEvents.pointerUpHandler);ExecuteEvents.ExecuteHierarchy(target,pointer,ExecuteEvents.pointerClickHandler);
        }
    }
}
