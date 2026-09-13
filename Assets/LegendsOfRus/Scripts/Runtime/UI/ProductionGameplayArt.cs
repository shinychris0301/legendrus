using System;
using System.Collections.Generic;
using UnityEngine;
using LegendsOfRus.GameplayCore;

namespace LegendsOfRus
{
    /// <summary>User-authored isolated atlas regions, never reference-screen regions.</summary>
    public static class ProductionGameplayArt
    {
        public const string Root="ProductionUI/GameplayAligned/";
        public static readonly string[] Files={"environment","panel","chain","gems","boosters","obstacles","isolated_seapearl","isolated_feather","isolated_column","isolated_color","moves_frame","cell_lacquer"};
        public readonly struct Region
        {
            public readonly string Sheet,Name;
            public readonly Rect TopLeftRect;
            public Region(string sheet,string name,int x,int y,int w,int h){Sheet=sheet;Name=name;TopLeftRect=new Rect(x,y,w,h);}
        }
        public static readonly Region[] Regions={
            new Region("gems","ruby",16,57,211,239),new Region("gems","sapphire",486,57,218,239),
            new Region("gems","emerald",979,62,205,232),new Region("gems","amber",18,340,207,242),
            new Region("gems","amethyst",485,340,218,242),new Region("gems","shield",969,330,215,258),
            new Region("gems","burst",3,594,390,421),new Region("gems","row",738,701,315,218),
            new Region("boosters","mace",43,0,339,323),new Region("boosters","firebird",383,0,325,323),
            new Region("boosters","livinglight",709,0,328,323),new Region("boosters","seapearl",1047,0,348,323),
            new Region("boosters","feather",510,785,225,294),
            new Region("obstacles","crate",330,274,235,234),new Region("obstacles","reinforced",603,271,240,239),
            new Region("obstacles","ice",1144,271,247,241),new Region("obstacles","column",1038,487,194,300),
            new Region("obstacles","color",312,748,287,318)
        };
        public static bool HasIsolatedOverride(string name)=>name=="seapearl"||name=="feather"||name=="column"||name=="color";
        static readonly Dictionary<string,Sprite> cache=new Dictionary<string,Sprite>(StringComparer.Ordinal);
        public static Sprite Load(string name)
        {
            if(HasIsolatedOverride(name))return Resources.Load<Sprite>(Root+"isolated_"+name)??throw new InvalidOperationException("Missing isolated Gameplay art: "+name);
            if(cache.TryGetValue(name,out var sprite)&&sprite!=null)return sprite;
            foreach(var region in Regions)if(region.Name==name)
            {
                foreach(var entry in Resources.LoadAll<Sprite>(Root+region.Sheet))cache[entry.name]=entry;
                if(cache.TryGetValue(name,out sprite)&&sprite!=null)return sprite;
                throw new InvalidOperationException("Missing Gameplay atlas region: "+name);
            }
            sprite=Resources.Load<Sprite>(Root+name);
            if(sprite==null)throw new InvalidOperationException("Missing Gameplay art: "+name);
            cache[name]=sprite;return sprite;
        }
        public static Sprite Gem(int tile)=>Load(new[]{"ruby","sapphire","emerald","amber","amethyst","shield"}[Mathf.Clamp(tile,0,5)]);
        public static Sprite Obstacle(BoardObstacleKind kind,int layers=1)
        {
            switch(kind){case BoardObstacleKind.Ice:return Load("ice");case BoardObstacleKind.Chain:return Load("chain");case BoardObstacleKind.Crate:return Load(layers>1?"reinforced":"crate");default:return null;}
        }
    }
}
