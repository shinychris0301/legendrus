using System;
using UnityEngine;
namespace LegendsOfRus
{
    /// <summary>One source-art family for actual boosters and explicitly unavailable previews.</summary>
    public static class ProductionBoosterArt
    {
        public const string Root="ProductionUI/SharedBoosters/";
        public static readonly string[] Names={"mace","firebird","shield","potion","scroll","lightning","bomb","swap"};
        public static Sprite Icon(int index,bool framed=false)
        {
            if(index<0||index>=Names.Length)throw new ArgumentOutOfRangeException(nameof(index));
            return Resources.Load<Sprite>(Root+Names[index]+(framed?"_framed":""))??throw new InvalidOperationException("Missing shared booster art: "+Names[index]);
        }
        public static int ToolIndex(string name)
        {
            for(int i=0;i<Names.Length;i++)if(name==Names[i])return i;
            return -1;
        }
        // Used by the serialized migration and legacy page loaders. Never map a shield gem,
        // a character Firebird, scenery, navigation artwork or a special board piece.
        public static Sprite Replacement(string resourcePath)
        {
            const string prefix="ProductionUI/";
            if(resourcePath.StartsWith(prefix,StringComparison.Ordinal))resourcePath=resourcePath.Substring(prefix.Length);
            int slash=resourcePath.LastIndexOf('/');if(slash<0)return null;
            string folder=resourcePath.Substring(0,slash),name=resourcePath.Substring(slash+1);
            if(folder=="MailAligned"&&(name=="hammer"||name=="potion"))return Icon(name=="hammer"?0:3);
            if(folder=="GameplayAligned")
            {
                string[] old={"booster_mace_r9","booster_firebird_r9","booster_livinglight_r9","booster_seapearl_r9"};
                int n=Array.IndexOf(old,name);return n>=0?Icon(n):null;
            }
            if(folder=="PreLevelAligned"||folder=="PreLevelReferenceAligned"||folder=="PauseAligned"||folder=="FailAligned")
            {
                if(name.EndsWith("_r2",StringComparison.Ordinal))name=name.Substring(0,name.Length-3);
                if(name.EndsWith("_r11",StringComparison.Ordinal))name=name.Substring(0,name.Length-4);
                int n=ToolIndex(name);return n>=0&&n<4?Icon(n,true):null;
            }
            if(folder=="BoosterSelectionAligned"&&name.StartsWith("preview_",StringComparison.Ordinal)&&name.EndsWith("_r2",StringComparison.Ordinal))
            {int n;if(int.TryParse(name.Substring(8,1),out n)&&n>=0&&n<4)return Icon(n+4,true);}
            return null;
        }
    }
}
