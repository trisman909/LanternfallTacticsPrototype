using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Lanternfall
{
    public sealed class LanternfallGame : MonoBehaviour
    {
        public GridModel Grid { get; private set; }
        public PlayerModel Player { get; private set; }
        public List<EnemyModel> Enemies { get; } = new();
        public TurnManager Turns { get; } = new();
        public int RoomNumber { get; private set; } = 1;
        public string Message { get; private set; } = "Choose a glowing tile.";
        public SkillId? SelectedSkill { get; set; }
        public HashSet<Vector2Int> ValidTargets { get; private set; } = new();
        public event Action Changed;
        RoomGenerator generator = new();

        public void StartRun()
        {
            RoomNumber=1; Player=new PlayerModel(); LoadRoom();
        }
        void LoadRoom()
        {
            var r=generator.Generate(Environment.TickCount + RoomNumber*97, RoomNumber); Grid=r.Grid; Player.Position=r.PlayerSpawn; Enemies.Clear();
            for(int i=0;i<r.EnemySpawns.Count;i++)
            {
                var kind = RoomNumber==5 ? EnemyKind.LanternWarden : (EnemyKind)((RoomNumber+i-1)%3);
                Enemies.Add(new EnemyModel(kind,r.EnemySpawns[i]));
            }
            Turns.BeginPlayerTurn(); SelectedSkill=null; RefreshTargets(); RefreshPreviews(); Message=RoomNumber==5?"BOSS: The Lantern Warden awakens.":$"Room {RoomNumber}: Player Turn"; Changed?.Invoke();
        }
        public bool Occupied(Vector2Int p) => Enemies.Any(e=>e.Alive&&e.Position==p);
        public void SelectSkill(SkillId id)
        {
            var def=SkillBook.Get(id); if(Turns.Phase!=TurnPhase.Player) return;
            if(Player.Cooldowns[def.Name]>0){Reject($"{def.Name} is cooling down.");return;}
            if(id==SkillId.RadiantSweep){UseSweep();return;}
            SelectedSkill=id; RefreshTargets(); Message=$"{def.Name}: choose a gold target."; Changed?.Invoke();
        }
        public void CancelSkill(){SelectedSkill=null;RefreshTargets();Message="Skill cancelled.";Changed?.Invoke();}
        public void TapTile(Vector2Int p)
        {
            if(Turns.Phase!=TurnPhase.Player) return;
            if(!Grid.IsFloor(p)){Reject("The void cannot be crossed.");return;}
            if(!ValidTargets.Contains(p)){Reject(SelectedSkill.HasValue?"Invalid skill target.":"Tile is blocked or out of range.");return;}
            if(SelectedSkill.HasValue) UseTargetedSkill(SelectedSkill.Value,p); else {Player.Position=p;Message="Moved.";EndPlayerAction();}
        }
        void UseTargetedSkill(SkillId id,Vector2Int p)
        {
            var def=SkillBook.Get(id);
            if(id==SkillId.EmberBolt){var e=Enemies.FirstOrDefault(x=>x.Alive&&x.Position==p);if(e==null){Reject("Ember Bolt needs an enemy.");return;}e.Damage(3+Player.Power);Message=$"Ember Bolt hits for {3+Player.Power}.";}
            else {Player.Position=p;foreach(var e in Enemies.Where(x=>x.Alive&&Manhattan(x.Position,p)==1))e.Damage(1+Player.Power);Message="Lantern Dash scorches nearby foes.";}
            Player.Cooldowns[def.Name]=def.Cooldown+1;SelectedSkill=null;EndPlayerAction();
        }
        void UseSweep()
        {
            foreach(var e in Enemies.Where(x=>x.Alive&&Manhattan(x.Position,Player.Position)<=1))e.Damage(2+Player.Power);
            var d=SkillBook.Get(SkillId.RadiantSweep);Player.Cooldowns[d.Name]=d.Cooldown+1;Message="Radiant fire sweeps the dark.";EndPlayerAction();
        }
        public void WaitTurn(){if(Turns.Phase==TurnPhase.Player){Message="You hold position.";EndPlayerAction();}}
        void EndPlayerAction()
        {
            Changed?.Invoke(); var outcome=GameRules.ResolveOutcome(Player,Enemies,RoomNumber);
            if(outcome==TurnPhase.Reward){Turns.ShowReward();Message="Room cleared — choose one blessing.";Changed?.Invoke();return;}
            if(outcome==TurnPhase.Won){Turns.Win();Message="LANTERN RESTORED — RUN COMPLETE";Changed?.Invoke();return;}
            Turns.TryBeginEnemyTurn();StartCoroutine(EnemyTurn());
        }
        IEnumerator EnemyTurn()
        {
            Message="Enemy Turn — committed attacks resolve.";Changed?.Invoke();yield return new WaitForSeconds(.35f);
            foreach(var e in Enemies.Where(x=>x.Alive).ToList())
            {
                if(e.Preview.Contains(Player.Position)){Player.Damage(e.AttackDamage);Message=$"{NameOf(e.Kind)} strikes for {e.AttackDamage}.";}
                else
                {
                    var path=Grid.ShortestPath(e.Position,Player.Position,q=>Occupied(q)||q==Player.Position);
                    int steps=Mathf.Min(e.MoveRange,Mathf.Max(0,path.Count-1));if(steps>0)e.Position=path[steps-1];
                }
                Changed?.Invoke();yield return new WaitForSeconds(.2f);
            }
            if(!Player.Alive){Turns.Lose();Message="YOUR LANTERN IS EXTINGUISHED";Changed?.Invoke();yield break;}
            Player.TickCooldowns();Turns.BeginPlayerTurn();RefreshTargets();RefreshPreviews();Message="Player Turn — red tiles will be attacked next.";Changed?.Invoke();
        }
        void RefreshTargets(){ValidTargets=SelectedSkill.HasValue?SkillBook.Targets(Grid,Player,SelectedSkill.Value,Occupied):Grid.Reachable(Player.Position,Player.MoveRange,Occupied);}
        public void RefreshPreviews(){foreach(var e in Enemies.Where(x=>x.Alive))e.Preview=EnemyAI.BuildPreview(e,Player.Position,Grid);}
        public void ChooseReward(int choice)
        {
            if(Turns.Phase!=TurnPhase.Reward)return;
            if(choice==0){Player.MaxHealth+=3;Player.Health=Mathf.Min(Player.MaxHealth,Player.Health+3);Message="Vital Ember: +3 max HP.";}
            else if(choice==1){Player.Power+=1;Message="Bright Wick: +1 skill damage.";} else {Player.MoveRange+=1;Message="Swift Flame: +1 move range.";}
            RoomNumber++;LoadRoom();
        }
        public void Restart(){StopAllCoroutines();StartRun();}
        void Reject(string why){Message="✕ "+why;Changed?.Invoke();}
        static int Manhattan(Vector2Int a,Vector2Int b)=>Mathf.Abs(a.x-b.x)+Mathf.Abs(a.y-b.y);
        public static string NameOf(EnemyKind k)=>k switch{EnemyKind.Ashling=>"Ashling",EnemyKind.GloomArcher=>"Gloom Archer",EnemyKind.StoneSentinel=>"Stone Sentinel",_=>"Lantern Warden"};
    }
}
