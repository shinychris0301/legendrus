#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
using UnityEngine;
namespace LegendsOfRus.Editor
{
    public static class MechanicsSandboxMenu
    {
        const string Menu="Legends of Rus/Development/Gameplay Mechanics Sandbox";
        [MenuItem(Menu,true)] static bool Available()=>Application.isPlaying&&ScreenRouter.Instance!=null&&ScreenRouter.Instance.CurrentScreen==ScreenId.Home&&!ScreenRouter.Instance.HasActiveModalBlocker&&Object.FindFirstObjectByType<MechanicsSandbox>()==null&&Object.FindFirstObjectByType<CampaignDevelopmentUI>()==null;
        [MenuItem(Menu)] static void Open(){if(!Available())return;var parent=ScreenRouter.Instance.SafeArea;var previous=parent.Cast<Transform>().Where(t=>t.gameObject.activeSelf).Select(t=>t.gameObject).ToArray();MechanicsSandbox.Open(parent,()=>{foreach(var p in previous)if(p)p.SetActive(true);});foreach(var p in previous)p.SetActive(false);}
    }
}
#endif
