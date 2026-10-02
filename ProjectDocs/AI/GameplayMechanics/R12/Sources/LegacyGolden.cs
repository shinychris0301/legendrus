using System;using LegendsOfRus.GameplayCore;
class LegacyGolden {
 static BoardModel Board(){var b=new BoardModel(8,7,6,new SeededBoardRandom(1));for(int y=0;y<7;y++)for(int x=0;x<8;x++)b.SetCell(new BoardPosition(x,y),new BoardCellData((x+2*y)%5));return b;}
 static void Main(){foreach(GameplayRuleVersion r in new[]{GameplayRuleVersion.Legacy,GameplayRuleVersion.PrototypeR1,GameplayRuleVersion.PrototypeR2,GameplayRuleVersion.PrototypeR3})for(int k=0;k<6;k++)for(int j=0;j<6;j++){
 if((r==GameplayRuleVersion.Legacy||r==GameplayRuleVersion.PrototypeR1)&&(k==5||j==5))continue;
 var b=Board();b.SetCell(new BoardPosition(3,3),new BoardCellData(5,(SpecialGemKind)k));b.SetCell(new BoardPosition(4,3),new BoardCellData(4,(SpecialGemKind)j));
 for(int reverse=0;reverse<2;reverse++){var a=new BoardPosition(reverse==0?3:4,3);var c=new BoardPosition(reverse==0?4:3,3);var p=new BoardResolutionPlanner(b,null,r).PlanSwap(a,c);Console.WriteLine((int)r+"\t"+a.X+"\t"+c.X+"\t"+b.CreateSnapshot().ToReplayToken()+"\t"+p.PlanHash+"\t"+p.FinalSnapshot.StateHash);}
 }}
}
