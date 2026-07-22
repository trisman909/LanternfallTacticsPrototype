using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;

namespace Lanternfall.Tests
{
    public sealed class Phase6MTests
    {
        static GridModel FullGrid(int width,int height){var g=new GridModel(width,height);for(int y=0;y<height;y++)for(int x=0;x<width;x++)g.SetFloor(new Vector2Int(x,y));return g;}
        static void ResolveEnemyTurn(LanternfallGame game){game.Turns.TryBeginEnemyTurn();var turn=(IEnumerator)typeof(LanternfallGame).GetMethod("EnemyTurn",BindingFlags.Instance|BindingFlags.NonPublic).Invoke(game,null);while(turn.MoveNext()){} }
        static LanternfallGame GameWithEnemy(EnemyKind kind,int distance)
        {
            var go=new GameObject("Phase6M");var game=go.AddComponent<LanternfallGame>();game.StartRun(13600+distance);var player=game.Player.Position;
            var tile=game.Grid.Floors().Where(p=>p!=player).OrderBy(p=>Mathf.Abs(p.x-player.x)+Mathf.Abs(p.y-player.y)).First(p=>Mathf.Abs(p.x-player.x)+Mathf.Abs(p.y-player.y)==distance);
            game.Enemies.Clear();game.Enemies.Add(new EnemyModel(kind,tile));game.RefreshPreviews();return game;
        }

        [Test] public void AshlingImmediatePreviewEqualsItsExactMeleeDamagePattern()
        {
            var g=FullGrid(7,7);var ashling=new EnemyModel(EnemyKind.Ashling,new Vector2Int(3,3));EnemyAI.AssignIntent(ashling,new Vector2Int(3,4),g);
            CollectionAssert.AreEquivalent(g.Neighbors(ashling.Position),ashling.Preview);Assert.False(ashling.DelayedPreview.Any());Assert.AreEqual("Claw Strike",ashling.IntentLabel);
        }

        [Test] public void AshlingFlameSigilIsNamedDelayedThreatAndNotImmediateRange()
        {
            var g=FullGrid(7,7);var player=new Vector2Int(3,3);var ashling=new EnemyModel(EnemyKind.Ashling,new Vector2Int(3,1));EnemyAI.AssignIntent(ashling,player,g);
            Assert.False(ashling.Preview.Contains(player));Assert.True(ashling.DelayedPreview.Contains(player));Assert.AreEqual("Flame Sigil",ashling.IntentLabel);Assert.AreEqual(ThreatKind.HP,ashling.Threat);
        }

        [Test] public void AshlingDelayedDamageUsesOnlyCommittedDelayedTilesAndResolvesOnce()
        {
            var game=GameWithEnemy(EnemyKind.Ashling,2);var ashling=game.Enemies.Single();int hp=game.Player.Health;ResolveEnemyTurn(game);
            Assert.AreEqual(hp-ashling.AttackDamage,game.Player.Health);Assert.That(game.LastDamageSource,Does.Contain("Ashling").And.Contain("Flame Sigil"));Assert.AreEqual(ashling.AttackDamage,game.LastDamageAmount);Object.DestroyImmediate(game.gameObject);
        }

        [Test] public void AshlingOutsideBothPatternsCannotDamagePlayer()
        {
            var game=GameWithEnemy(EnemyKind.Ashling,3);int hp=game.Player.Health;ResolveEnemyTurn(game);Assert.AreEqual(hp,game.Player.Health);Object.DestroyImmediate(game.gameObject);
        }

        [Test] public void MovingOutOfFlameSigilRebuildsAndRemovesStaleThreat()
        {
            var game=GameWithEnemy(EnemyKind.Ashling,2);var ashling=game.Enemies.Single();var old=game.Player.Position;var escape=game.Grid.Neighbors(old).First(p=>Mathf.Abs(p.x-ashling.Position.x)+Mathf.Abs(p.y-ashling.Position.y)>2&&!game.Occupied(p));game.Player.Position=escape;game.RefreshPreviews();
            Assert.False(ashling.DelayedPreview.Contains(old));Assert.False(ashling.DelayedPreview.Contains(escape));Object.DestroyImmediate(game.gameObject);
        }

        [Test] public void AshlingThreatTextNamesSourceDamageTimingAndAvoidance()
        {
            var game=GameWithEnemy(EnemyKind.Ashling,2);string detail=game.ThreatDetailAt(game.Player.Position);Assert.That(detail,Does.Contain("DELAYED THREAT").And.Contain("Flame Sigil").And.Contain("damage after End Turn").And.Contain("leave marked tiles"));Object.DestroyImmediate(game.gameObject);
        }

        [Test] public void SentinelAtDistanceTwoInOpenSpaceHasNoGenericBindAndAdvances()
        {
            var g=FullGrid(9,9);var player=new Vector2Int(4,4);var sentinel=new EnemyModel(EnemyKind.StoneSentinel,new Vector2Int(4,2)){MoveRange=1};EnemyAI.AssignIntent(sentinel,player,g);
            Assert.False(sentinel.Preview.Contains(player));Assert.False(sentinel.DelayedPreview.Contains(player));Assert.False(EnemyAI.ShouldHoldPosition(sentinel,player,g,new[]{sentinel}));var plan=EnemyAI.BuildSquadPlan(new[]{sentinel},player,g,_=>false);Assert.AreNotEqual(sentinel.Position,plan.DestinationFor(sentinel));Assert.Less(Manhattan(plan.DestinationFor(sentinel),player),2);
        }

        [Test] public void SentinelCanSelectMpBindOnlyAtARealChokepoint()
        {
            var g=new GridModel(7,7);foreach(var p in new[]{new Vector2Int(3,1),new Vector2Int(3,2),new Vector2Int(3,3),new Vector2Int(3,4)})g.SetFloor(p);var player=new Vector2Int(3,4);var sentinel=new EnemyModel(EnemyKind.StoneSentinel,new Vector2Int(3,2));EnemyAI.AssignIntent(sentinel,player,g);
            Assert.True(sentinel.DelayedPreview.Contains(player));Assert.AreEqual("MP Bind",sentinel.IntentLabel);Assert.True(EnemyAI.ShouldHoldPosition(sentinel,player,g,new[]{sentinel}));
        }

        [Test] public void SentinelPlanAndExecutionUseTheSameLegalDestination()
        {
            var game=GameWithEnemy(EnemyKind.StoneSentinel,4);var sentinel=game.Enemies.Single();var plan=EnemyAI.BuildSquadPlan(game.Enemies,game.Player.Position,game.Grid,p=>game.Occupied(p)||p==game.Player.Position);var expected=plan.DestinationFor(sentinel);ResolveEnemyTurn(game);Assert.AreEqual(expected,sentinel.Position);Assert.Less(Manhattan(sentinel.Position,game.Player.Position),4);Object.DestroyImmediate(game.gameObject);
        }

        [Test] public void NonAdjacentSentinelCannotDealMeleeDamage()
        {
            var game=GameWithEnemy(EnemyKind.StoneSentinel,3);int hp=game.Player.Health;ResolveEnemyTurn(game);Assert.AreEqual(hp,game.Player.Health);Object.DestroyImmediate(game.gameObject);
        }

        [Test] public void AdjacentSentinelMeleeUsesActualFinalAdjacency()
        {
            var game=GameWithEnemy(EnemyKind.StoneSentinel,1);int hp=game.Player.Health;ResolveEnemyTurn(game);Assert.Less(game.Player.Health,hp);Assert.That(game.LastDamageSource,Does.Contain("Stone Sentinel").And.Contain("Shield Bash"));Object.DestroyImmediate(game.gameObject);
        }

        [Test] public void StructuredThreatPanelSeparatesDelayedStatusAndMovement()
        {
            var game=GameWithEnemy(EnemyKind.Ashling,2);game.Player.BurnTurns=2;string text=game.StructuredThreatSummary;Assert.That(text,Does.Contain("DELAYED THREATS").And.Contain("Flame Sigil").And.Contain("ACTIVE EFFECTS").And.Contain("Burning"));Object.DestroyImmediate(game.gameObject);
        }

        [Test] public void PhoneThreatStripUsesOneContainedPriorityOrderedLine()
        {
            var game=GameWithEnemy(EnemyKind.Ashling,2);game.Player.BurnTurns=2;string text=game.MobileThreatSummary(32);
            Assert.That(text,Does.StartWith("DELAYED ").And.Contain("Flame Sigil").And.Contain("+1"));Assert.LessOrEqual(text.Length,32);Assert.False(text.Contains("\n"));Object.DestroyImmediate(game.gameObject);
        }

        [Test] public void PhoneThreatStripCollapsesEmptyCategories()
        {
            var game=GameWithEnemy(EnemyKind.Ashling,3);game.Enemies.Clear();Assert.AreEqual("SAFE",game.MobileThreatSummary(24));Object.DestroyImmediate(game.gameObject);
        }

        [Test] public void PhoneThreatStripContainsLongActionsWithoutLosingCategory()
        {
            var game=GameWithEnemy(EnemyKind.StoneSentinel,1);string text=game.MobileThreatSummary(18);Assert.That(text,Does.StartWith("NOW "));Assert.LessOrEqual(text.Length,18);Assert.False(text.Contains("\n"));Object.DestroyImmediate(game.gameObject);
        }

        [TestCase(844f,390f)] [TestCase(932f,430f)] [TestCase(1080f,540f)]
        public void PhoneLandscapeKeepsStatsThreatsSkillsAndEndTurnVisible(float width,float height)
        {
            var layout=MobileLayout.Compute(width,height);Assert.True(layout.PhoneLandscape);Assert.GreaterOrEqual(layout.ThreatPanel.width,152f);Assert.True(layout.SkillButtons.All(r=>r.height>=44f));Assert.GreaterOrEqual(layout.ActionButton.height,44f);Assert.LessOrEqual(layout.Board.xMax,layout.ThreatPanel.xMin+.01f);Assert.LessOrEqual(layout.Board.yMax,layout.SkillBar.yMin+.01f);
        }

        static int Manhattan(Vector2Int a,Vector2Int b)=>Mathf.Abs(a.x-b.x)+Mathf.Abs(a.y-b.y);
    }
}
