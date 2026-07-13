using System.Collections.Generic;
using UnityEngine;

namespace Lanternfall
{
    public enum EnemyKind { Ashling, GloomArcher, StoneSentinel, LanternWarden }
    public enum PlayerClassId { Vanguard, Wayfinder, Cantor, Gloamstep, Artificer }
    public enum ThreatKind { HP, AP, MP, Mixed }

    public abstract class UnitModel
    {
        public Vector2Int Position;
        public int Health;
        public int MaxHealth;
        public int Shield;
        public int BurnTurns;
        public int RootTurns;
        public int MarkedTurns;
        public bool Alive => Health > 0;
        public void Damage(int amount)
        {
            int blocked = Mathf.Min(Shield, amount);
            Shield -= blocked;
            Health = Mathf.Max(0, Health - (amount - blocked));
        }
        public void TickStatuses()
        {
            if (BurnTurns > 0){Damage(1); BurnTurns--;}
            if (RootTurns > 0) RootTurns--;
            if (MarkedTurns > 0) MarkedTurns--;
            Shield = Mathf.Max(0, Shield - 1);
        }
    }

    public sealed class PlayerModel : UnitModel
    {
        public PlayerClassId ClassId;
        public int MaxActionPoints = 6;
        public int ActionPoints = 6;
        public int MoveRange = 3;
        public int MovementPoints = 3;
        public int Power = 0;
        public readonly Dictionary<string, int> Cooldowns = new();
        public PlayerModel(PlayerClassId classId = PlayerClassId.Cantor)
        {
            ClassId = classId;
            ApplyClassStats(classId);
            foreach (var skill in SkillBook.ForClass(classId)) Cooldowns[skill.Name] = 0;
            ResetTurnResources();
        }
        void ApplyClassStats(PlayerClassId classId)
        {
            (MaxHealth, MaxActionPoints, MoveRange) = classId switch
            {
                PlayerClassId.Vanguard => (15, 6, 3),
                PlayerClassId.Wayfinder => (11, 6, 3),
                PlayerClassId.Gloamstep => (11, 6, 4),
                PlayerClassId.Artificer => (12, 7, 3),
                _ => (12, 6, 3)
            };
            Health = MaxHealth;
        }
        public void ResetTurnResources(){ActionPoints = MaxActionPoints; MovementPoints = MoveRange;}
        public bool SpendAP(int amount){if(ActionPoints<amount)return false;ActionPoints-=amount;return true;}
        public bool SpendMP(int amount){if(MovementPoints<amount)return false;MovementPoints-=amount;return true;}
        public void TickCooldowns() { foreach (var key in new List<string>(Cooldowns.Keys)) Cooldowns[key] = Mathf.Max(0, Cooldowns[key] - 1); }
        public int Recover(int amount) { int before = Health; Health = Mathf.Min(MaxHealth, Health + amount); return Health - before; }
    }

    public sealed class EnemyModel : UnitModel
    {
        public EnemyKind Kind;
        public HashSet<Vector2Int> Preview = new();
        public HashSet<Vector2Int> DelayedPreview = new();
        public string IntentLabel = "strike";
        public ThreatKind Threat = ThreatKind.HP;
        public int AttackDamage;
        public int MoveRange;
        public int BossPhaseAnnounced;
        public EnemyModel(EnemyKind kind, Vector2Int position)
        {
            Kind = kind; Position = position;
            (MaxHealth, AttackDamage, MoveRange) = BalanceConfig.EnemyStats(kind);
            Health = MaxHealth;
            BossPhaseAnnounced = kind == EnemyKind.LanternWarden ? 1 : 0;
        }
    }
}
