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
        public static readonly string[] Files={"environment","panel","chain","gems","boosters","obstacles","isolated_seapearl","isolated_feather","isolated_column","isolated_color","moves_frame","cell_lacquer","hud_header_r3","score_panel_r3","pause_gear_r3","goal_score_r5","goal_survival_r5","gem_ruby_r6","gem_sapphire_r6","gem_emerald_r6","gem_gold_r6","gem_amethyst_r6","gem_orange_r6","card_wood_r6","panel_parchment_r6","panel_moves_r6","panel_score_r6","environment_r6","gem_ruby_r7","gem_sapphire_r7","gem_emerald_r7","gem_gold_r7","gem_amethyst_r7","gem_orange_r7","board_frame_r7","board_wood_r7","gem_ruby_r8","gem_sapphire_r8","gem_emerald_r8","gem_gold_r8","gem_amethyst_r8","gem_orange_r8","booster_mace_r9","booster_firebird_r9","booster_livinglight_r9","booster_seapearl_r9","card_wood_r9","hud_moves_r10","hud_goals_r10","hud_score_r10","hud_portrait_r10","special_ruby_r10","special_sapphire_r10","special_emerald_r10","special_gold_r10","special_amethyst_r10","special_orange_r10"};
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
        public static Sprite Gem(int tile)=>ProductionGameplaySourceControls.Load(new[]{"gem_ruby","gem_sapphire","gem_emerald","gem_gold","gem_amethyst","gem_orange"}[Mathf.Clamp(tile,0,5)]);
        public static Sprite LineSpecial(int tile)=>Load(new[]{"special_ruby_r10","special_sapphire_r10","special_emerald_r10","special_gold_r10","special_amethyst_r10","special_orange_r10"}[Mathf.Clamp(tile,0,5)]);
        public static Sprite Booster(int index)=>ProductionBoosterArt.Icon(Mathf.Clamp(index,0,3));
        public static Sprite Obstacle(BoardObstacleKind kind,int layers=1)
        {
            switch(kind){case BoardObstacleKind.Ice:return Load("ice");case BoardObstacleKind.Chain:return Load("chain");case BoardObstacleKind.Crate:return Load(layers>1?"reinforced":"crate");default:return null;}
        }
    }
}
