using System.Collections.Generic;
using UnityEngine;

namespace Lanternfall
{
    public enum EnemyKind { Ashling, GloomArcher, StoneSentinel, LanternWarden }

    public abstract class UnitModel
    {
        public Vector2Int Position;
        public int Health;
        public int MaxHealth;
        public bool Alive => Health > 0;
        public void Damage(int amount) => Health = Mathf.Max(0, Health - amount);
    }

    public sealed class PlayerModel : UnitModel
    {
        public int MoveRange = 3;
        public int Power = 0;
        public readonly Dictionary<string, int> Cooldowns = new();
        public PlayerModel() { MaxHealth = Health = 12; Cooldowns["Ember Bolt"] = 0; Cooldowns["Lantern Dash"] = 0; Cooldowns["Radiant Sweep"] = 0; }
        public void TickCooldowns() { foreach (var key in new List<string>(Cooldowns.Keys)) Cooldowns[key] = Mathf.Max(0, Cooldowns[key] - 1); }
        public int Recover(int amount) { int before = Health; Health = Mathf.Min(MaxHealth, Health + amount); return Health - before; }
    }

    public sealed class EnemyModel : UnitModel
    {
        public EnemyKind Kind;
        public HashSet<Vector2Int> Preview = new();
        public int AttackDamage;
        public int MoveRange;
        public EnemyModel(EnemyKind kind, Vector2Int position)
        {
            Kind = kind; Position = position;
            (MaxHealth, AttackDamage, MoveRange) = BalanceConfig.EnemyStats(kind);
            Health = MaxHealth;
        }
    }
}
