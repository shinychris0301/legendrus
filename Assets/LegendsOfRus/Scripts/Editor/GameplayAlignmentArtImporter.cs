#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
namespace LegendsOfRus.Editor
{
    internal static class GameplayAlignmentArtImporter
    {
        internal static void Configure(TextureImporter ti,string name)
        {
            LockedDesignImporter.ConfigureWorldMapTexture(ti,name);
            var regions=new List<SpriteMetaData>();
            foreach(var region in ProductionGameplayArt.Regions)
            {
                if(region.Sheet!=name||ProductionGameplayArt.HasIsolatedOverride(region.Name))continue;
                Rect r=region.TopLeftRect;
                regions.Add(new SpriteMetaData{name=region.Name,alignment=(int)SpriteAlignment.Center,pivot=new Vector2(.5f,.5f),rect=new Rect(r.x,1086-r.y-r.height,r.width,r.height)});
            }
            if(name.StartsWith("isolated_")){ti.mipmapEnabled=false;ti.filterMode=FilterMode.Bilinear;}
            if(regions.Count==0)return;
            // Original user RGBA sheets stay byte-for-byte intact. Only sprite rectangles are authored.
            ti.spriteImportMode=SpriteImportMode.Multiple;ti.mipmapEnabled=false;ti.filterMode=FilterMode.Bilinear;
#pragma warning disable 0618
            ti.spritesheet=regions.ToArray();
#pragma warning restore 0618
        }
    }
}
#endif
