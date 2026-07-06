using System.Linq;
using NUnit.Framework;
using UnityEngine;

namespace Lanternfall.Tests
{
    public class CoreTests
    {
        [Test] public void Movement_RespectsRangeAndWalls(){var g=new GridModel(5,5);for(int x=0;x<5;x++)g.SetFloor(new Vector2Int(x,2));var r=g.Reachable(new Vector2Int(0,2),2,_=>false);Assert.That(r.Contains(new Vector2Int(2,2)));Assert.False(r.Contains(new Vector2Int(3,2)));}
        [Test] public void TurnOrder_RejectsDoubleEnd(){var t=new TurnManager();Assert.True(t.TryBeginEnemyTurn());Assert.False(t.TryBeginEnemyTurn());t.BeginPlayerTurn();Assert.AreEqual(TurnPhase.Player,t.Phase);}
        [Test] public void SkillTargeting_DashRejectsOccupiedTile(){var g=new GridModel(5,5);for(int x=0;x<5;x++)for(int y=0;y<5;y++)g.SetFloor(new Vector2Int(x,y));var p=new PlayerModel{Position=new Vector2Int(2,2)};var set=SkillBook.Targets(g,p,SkillId.LanternDash,q=>q==new Vector2Int(2,3));Assert.False(set.Contains(new Vector2Int(2,3)));Assert.True(set.Contains(new Vector2Int(2,4)));}
        [Test] public void SkillTargeting_BoltHighlightsEnemiesOnly(){var g=new GridModel(5,5);for(int x=0;x<5;x++)for(int y=0;y<5;y++)g.SetFloor(new Vector2Int(x,y));var p=new PlayerModel{Position=new Vector2Int(2,2)};var set=SkillBook.Targets(g,p,SkillId.EmberBolt,q=>q==new Vector2Int(2,4));Assert.That(set.Count,Is.EqualTo(1));Assert.True(set.Contains(new Vector2Int(2,4)));}
        [Test] public void AttackPreview_AshlingShowsAdjacentTiles(){var g=new GridModel(5,5);for(int x=0;x<5;x++)for(int y=0;y<5;y++)g.SetFloor(new Vector2Int(x,y));var e=new EnemyModel(EnemyKind.Ashling,new Vector2Int(2,2));var p=EnemyAI.BuildPreview(e,new Vector2Int(2,3),g);Assert.True(p.Contains(new Vector2Int(2,3)));Assert.False(p.Contains(new Vector2Int(4,4)));}
        [Test] public void RoomGeneration_IsConnectedAndHasSpawns(){var gen=new RoomGenerator();for(int i=1;i<=5;i++){var r=gen.Generate(100+i,i);Assert.True(gen.IsConnected(r.Grid));Assert.True(r.Grid.IsFloor(r.PlayerSpawn));Assert.That(r.EnemySpawns.Count,Is.GreaterThan(0));}}
        [Test] public void WinLoss_ResolveClearly(){var p=new PlayerModel();var dead=new[]{new EnemyModel(EnemyKind.Ashling,Vector2Int.zero)};dead[0].Damage(99);Assert.AreEqual(TurnPhase.Won,GameRules.ResolveOutcome(p,dead,5));p.Damage(99);Assert.AreEqual(TurnPhase.Lost,GameRules.ResolveOutcome(p,dead,1));}
        [Test] public void DifficultyCurve_UsesFixedReadableEncounters(){Assert.That(Enumerable.Range(1,5).Select(BalanceConfig.EnemyCount),Is.EqualTo(new[]{2,2,3,3,1}));Assert.AreEqual(EnemyKind.LanternWarden,BalanceConfig.EnemyFor(5,0));Assert.AreEqual(15,BalanceConfig.EnemyStats(EnemyKind.LanternWarden).health);}
        [Test] public void Recovery_IsCappedAndNeverOverheals(){var p=new PlayerModel();p.Damage(5);Assert.AreEqual(2,p.Recover(BalanceConfig.BetweenRoomRecovery));Assert.AreEqual(9,p.Health);Assert.AreEqual(3,p.Recover(99));Assert.AreEqual(p.MaxHealth,p.Health);}
    }
}
