using UnityEngine;

namespace Lanternfall
{
    public static class BalanceConfig
    {
        public const int BetweenRoomRecovery = 0;
        public const int HealingPickupAmount = 3;
        public static int EnemyCount(int room) => room switch { 1 => 2, 2 => 2, 3 => 3, 4 => 4, _ => 1 };
        public static EnemyKind EnemyFor(int room, int slot)
        {
            if (room >= 5) return EnemyKind.LanternWarden;
            EnemyKind[][] curve =
            {
                new[]{EnemyKind.Ashling, EnemyKind.GloomArcher},
                new[]{EnemyKind.Ashling, EnemyKind.StoneSentinel},
                new[]{EnemyKind.GloomArcher, EnemyKind.Ashling, EnemyKind.StoneSentinel},
                new[]{EnemyKind.StoneSentinel, EnemyKind.GloomArcher, EnemyKind.Ashling, EnemyKind.GloomArcher}
            };
            return curve[Mathf.Clamp(room - 1, 0, 3)][slot];
        }
        public static (int health, int damage, int move) EnemyStats(EnemyKind kind) => kind switch
        {
            EnemyKind.Ashling => (4, 2, 2),
            EnemyKind.GloomArcher => (5, 2, 2),
            EnemyKind.StoneSentinel => (6, 3, 1),
            _ => (24, 4, 2)
        };

        public static void ApplyRoomScaling(EnemyModel enemy, int room)
        {
            int depth = Mathf.Clamp(room - 1, 0, 4);
            if (enemy.Kind == EnemyKind.LanternWarden)
            {
                enemy.MaxHealth += 6;
                enemy.Health = enemy.MaxHealth;
                enemy.AttackDamage += 1;
                enemy.MoveRange += 1;
                return;
            }
            enemy.MaxHealth += depth / 2;
            enemy.Health = enemy.MaxHealth;
            if (room >= 3) enemy.AttackDamage += 1;
            if (room >= 4) enemy.AttackDamage += 1;
            if (room >= 4 && enemy.Kind != EnemyKind.StoneSentinel) enemy.MoveRange += 1;
        }
    }
}
