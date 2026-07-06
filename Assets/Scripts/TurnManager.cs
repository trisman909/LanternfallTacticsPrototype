using System;
namespace Lanternfall
{
    public enum TurnPhase { Player, Enemy, Reward, Won, Lost }
    public sealed class TurnManager
    {
        public TurnPhase Phase { get; private set; } = TurnPhase.Player;
        public event Action<TurnPhase> Changed;
        public bool TryBeginEnemyTurn() { if (Phase != TurnPhase.Player) return false; Set(TurnPhase.Enemy); return true; }
        public void BeginPlayerTurn() => Set(TurnPhase.Player);
        public void ShowReward() => Set(TurnPhase.Reward);
        public void Win() => Set(TurnPhase.Won);
        public void Lose() => Set(TurnPhase.Lost);
        void Set(TurnPhase p) { Phase=p; Changed?.Invoke(p); }
    }
}
