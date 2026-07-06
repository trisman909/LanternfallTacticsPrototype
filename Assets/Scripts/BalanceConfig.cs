using UnityEngine;

namespace Lanternfall
{
    public static class BalanceConfig
    {
        public const int BetweenRoomRecovery = 2;
        public static int EnemyCount(int room) => room switch { 1 => 2, 2 => 2, 3 => 3, 4 => 3, _ => 1 };
        public static EnemyKind EnemyFor(int room, int slot)
        {
            if (room >= 5) return EnemyKind.LanternWarden;
            EnemyKind[][] curve =
            {
                new[]{EnemyKind.Ashling, EnemyKind.GloomArcher},
                new[]{EnemyKind.Ashling, EnemyKind.StoneSentinel},
                new[]{EnemyKind.GloomArcher, EnemyKind.Ashling, EnemyKind.StoneSentinel},
                new[]{EnemyKind.StoneSentinel, EnemyKind.GloomArcher, EnemyKind.Ashling}
            };
            return curve[Mathf.Clamp(room - 1, 0, 3)][slot];
        }
        public static (int health, int damage, int move) EnemyStats(EnemyKind kind) => kind switch
        {
            EnemyKind.Ashling => (3, 2, 2),
            EnemyKind.GloomArcher => (4, 2, 1),
            EnemyKind.StoneSentinel => (6, 3, 1),
            _ => (15, 3, 2)
        };
    }
}
