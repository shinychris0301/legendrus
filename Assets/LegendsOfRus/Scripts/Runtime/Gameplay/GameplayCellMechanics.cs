using UnityEngine;
using UnityEngine.UI;
using LegendsOfRus.GameplayCore;
namespace LegendsOfRus
{
    /// <summary>Stationary obstacle art; prototype stages use distinct sprites instead of layer labels.</summary>
    internal sealed class GameplayCellMechanics
    {
        readonly Image image;readonly Text layers;readonly bool prototype;
        public GameplayCellMechanics(RectTransform parent,Font font,bool prototype=false)
        {
            this.prototype=prototype;
            var go=new GameObject("Obstacle",typeof(RectTransform),typeof(Image));go.transform.SetParent(parent,false);
            var r=(RectTransform)go.transform;r.anchorMin=Vector2.zero;r.anchorMax=Vector2.one;r.offsetMin=new Vector2(4,4);r.offsetMax=new Vector2(-4,-4);
            image=go.GetComponent<Image>();image.preserveAspect=true;image.raycastTarget=false;
            if(prototype)return; // Damage is communicated by material/silhouette, not numerals.
            var label=new GameObject("ObstacleLayers",typeof(RectTransform),typeof(Text),typeof(Outline));label.transform.SetParent(parent,false);
            r=(RectTransform)label.transform;r.anchorMin=r.anchorMax=r.pivot=Vector2.one;r.anchoredPosition=new Vector2(-6,-3);r.sizeDelta=new Vector2(28,28);
            layers=label.GetComponent<Text>();layers.font=font;layers.fontStyle=FontStyle.Bold;layers.fontSize=22;layers.alignment=TextAnchor.MiddleCenter;layers.color=new Color32(255,233,177,255);layers.raycastTarget=false;
            label.GetComponent<Outline>().effectColor=new Color(0,0,0,.95f);label.GetComponent<Outline>().effectDistance=new Vector2(1.5f,-1.5f);
        }
        public void Refresh(BoardObstacleData data)
        {
            image.enabled=!data.IsEmpty;image.sprite=ProductionGameplayArt.Obstacle(data.Kind,data.Layers);
            // Ice stays translucent so the underlying match identity remains legible.
            image.color=new Color(1,1,1,data.Kind==BoardObstacleKind.Ice?.48f:1);
            if(prototype)PrototypeObstacleArt.Apply(image,data.Kind,data.Layers,true);
            else RunicSealGraphic.Refresh(image,data.Kind,data.Layers);
            if(data.IsEmpty)image.enabled=false;
            if(layers==null)return;
            layers.enabled=!data.IsEmpty&&(data.Layers>1||data.Kind==BoardObstacleKind.RunicSeal);layers.text=data.Layers.ToString();
        }
    }
}
