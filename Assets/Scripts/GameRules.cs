using System.Linq;
namespace Lanternfall
{
    public static class GameRules
    {
        public static TurnPhase ResolveOutcome(PlayerModel player, System.Collections.Generic.IEnumerable<EnemyModel> enemies, int room)
        {
            if (!player.Alive) return TurnPhase.Lost;
            if (enemies.Any(e=>e.Alive)) return TurnPhase.Player;
            return room >= 5 ? TurnPhase.Won : TurnPhase.Reward;
        }
    }
}
