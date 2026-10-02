using System;
using System.Linq;
using LegendsOfRus.GameplayCore;
class RulesProbe {
 static BoardPosition P(int x,int y)=>new BoardPosition(x,y);
 static BoardModel Board(){var b=new BoardModel(8,7,6,new SeededBoardRandom(1)){SquareMatches=true};for(int y=0;y<7;y++)for(int x=0;x<8;x++)b.SetCell(P(x,y),new BoardCellData((x+2*y)%5));return b;}
 static BoardResolutionPlanner Planner(BoardModel b)=>new BoardResolutionPlanner(b,null,GameplayRuleVersion.PrototypeR3);
 static void Main(){
 var kinds=new[]{SpecialGemKind.Row,SpecialGemKind.Column,SpecialGemKind.Burst,SpecialGemKind.Spark,SpecialGemKind.Color};
 foreach(var k in kinds){var b=Board();b.SetCell(P(3,3),new BoardCellData(5,k));var tap=Planner(b).PlanTapSpecial(P(3,3));var swap=Planner(b).PlanSwap(P(3,3),P(4,3));Console.WriteLine("Gesture|"+k+"|tap="+tap.Status+"|swapWithNormal="+swap.Status);}
 for(int i=0;i<kinds.Length;i++)for(int j=i;j<kinds.Length;j++){
 var b=Board();b.SetCell(P(3,3),new BoardCellData(5,kinds[i]));b.SetCell(P(4,3),new BoardCellData(4,kinds[j]));var p=Planner(b).PlanSwap(P(3,3),P(4,3));var w=p.Waves[0];Console.WriteLine("Pair|"+kinds[i]+"+"+kinds[j]+"|"+p.Status+"|clear="+w.ClearedCells.Count+"|activations="+string.Join(",",w.Activations.Select(a=>a.Kind+"@"+a.Position+" color="+a.TileType+" payload="+a.Payload)));
 }
 foreach(var shape in new[]{"H4","V4","H5","Square","L","T"}){
 var b=Board();int[,] xy=shape=="H4"?new int[,]{{1,2},{2,2},{3,2},{4,2}}:shape=="V4"?new int[,]{{2,1},{2,2},{2,3},{2,4}}:shape=="H5"?new int[,]{{1,2},{2,2},{3,2},{4,2},{5,2}}:shape=="Square"?new int[,]{{2,2},{3,2},{2,3},{3,3}}:shape=="L"?new int[,]{{2,2},{3,2},{4,2},{2,3},{2,4}}:new int[,]{{1,2},{2,2},{3,2},{2,3},{2,4}};
 for(int n=0;n<xy.GetLength(0);n++)b.SetCell(P(xy[n,0],xy[n,1]),new BoardCellData(5));Console.WriteLine("Formation|"+shape+"|"+string.Join(",",b.DetermineSpecialCreations(b.FindRuns(),null,true).Select(c=>c.Value+"@"+c.Key)));
 }
 {var b=Board();b.SetCell(P(1,2),new BoardCellData(5));b.SetCell(P(2,2),new BoardCellData(5,SpecialGemKind.Burst));b.SetCell(P(3,2),new BoardCellData(5));Console.WriteLine("HiddenColor|bombBridgesOrdinaryMatch="+b.FindRuns().Any(r=>r.Cells.Contains(P(2,2))));}
 foreach(var k in new[]{SpecialGemKind.Row,SpecialGemKind.Burst,SpecialGemKind.Spark}){var b=Board();b.SetCell(P(3,3),new BoardCellData(2,SpecialGemKind.Color));b.SetCell(P(4,3),new BoardCellData(5,k));var counts=Enumerable.Range(0,6).Select(c=>b.CreateSnapshot().Tiles.Count(t=>t==c)).ToArray();var w=Planner(b).PlanSwap(P(3,3),P(4,3)).Waves[0];Console.WriteLine("ColorPair|"+k+"|mostCommon="+Array.IndexOf(counts,counts.Max())+"|partner=5|activationColors="+string.Join(",",w.Activations.Select(a=>a.TileType).Distinct()));}
 }
}
