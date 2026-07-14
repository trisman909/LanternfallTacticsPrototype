using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Lanternfall
{
    public sealed class LanternfallGame : MonoBehaviour
    {
        const string BestRoomKey = "LanternfallTactics.BestRoom";
        const string ClassKey = "LanternfallTactics.SelectedClass";

        public GridModel Grid { get; private set; }
        public PlayerModel Player { get; private set; }
        public List<EnemyModel> Enemies { get; } = new();
        public TurnManager Turns { get; } = new();
        public int RoomNumber { get; private set; } = 1;
        public string Message { get; private set; } = "Press Start Run to light the lantern.";
        public bool HasStarted { get; private set; }
        public bool HelpVisible { get; private set; }
        public bool PlaytestInfoVisible { get; private set; }
        public int BestRoomReached { get; private set; }
        public int? RunSeed { get; private set; }
        public PlayerClassId SelectedClass { get; private set; } = PlayerClassId.Cantor;
        public SkillId? SelectedSkill { get; private set; }
        public bool LastInputAccepted { get; private set; } = true;
        public Vector2Int? LastTappedTile { get; private set; }
        public Vector2Int? RejectedTile { get; private set; }
        public HashSet<Vector2Int> HitTiles { get; private set; } = new();
        public HashSet<Vector2Int> PreviewArea { get; private set; } = new();
        public HashSet<Vector2Int> ValidTargets { get; private set; } = new();
        public BiomeTheme Theme { get; private set; }
        public HashSet<Vector2Int> HazardTiles { get; private set; } = new();
        public HashSet<Vector2Int> ArmedHazards { get; private set; } = new();
        public HashSet<Vector2Int> PropTiles { get; private set; } = new();
        public HashSet<Vector2Int> BlockerTiles { get; private set; } = new();
        public Vector2Int? HealingPickup { get; private set; }
        public event Action Changed;

        readonly RoomGenerator generator = new();
        string pendingRoomIntro = "";
        int pendingApDrain;
        int pendingMpDrain;
        public int PendingApDrain => pendingApDrain;
        public int PendingMpDrain => pendingMpDrain;
        public string BossPhaseBanner { get; private set; } = "";
        float bossPhaseBannerUntil;
        public bool BossPhasePresentationActive => !string.IsNullOrEmpty(BossPhaseBanner) && Time.time < bossPhaseBannerUntil;

        public static readonly string[] HowToPlayLines =
        {
            "Goal: clear four rooms, then defeat the Lantern Warden in room five.",
            "AP means Action Points for skills. MP means Movement Points for tile movement.",
            "Tap cyan tiles to move. Tap a skill, then tap a gold target to use it.",
            "Red tiles hit this turn. Purple intent tiles show AP/MP or delayed pressure.",
            "Green HEAL tiles restore 3 HP when stepped on.",
            "If a target is not gold, it is out of range, blocked, or not valid for that skill.",
            "After each cleared room, pick one blessing. Win or lose, Start New Run restarts."
        };

        public static readonly string[] PlaytestInfoLines =
        {
            "Prototype v0.6B: short WebGL/Windows playtest build.",
            "Best tested on a desktop browser first; mobile browser is experimental.",
            "Please note what confused you, what felt fun, and if anything broke.",
            "Useful feedback: device/browser, board size, HUD readability, AP/MP, skill targets.",
            "If stuck, refresh the page or use Start New Run after win/loss.",
            "Known limits: placeholder art, no final audio, no physical iPhone Safari pass yet."
        };

        void Awake()
        {
            BestRoomReached = PlayerPrefs.GetInt(BestRoomKey, 0);
            SelectedClass = (PlayerClassId)Mathf.Clamp(PlayerPrefs.GetInt(ClassKey, (int)PlayerClassId.Cantor), 0, ClassCatalog.All.Length - 1);
        }

        public void SelectClass(PlayerClassId id)
        {
            if (HasStarted) return;
            SelectedClass = id;
            PlayerPrefs.SetInt(ClassKey, (int)id);
            PlayerPrefs.Save();
            Changed?.Invoke();
        }

        public void CycleClass()
        {
            SelectClass((PlayerClassId)(((int)SelectedClass + 1) % ClassCatalog.All.Length));
        }

        public void StartRun()
        {
            StartRun(null);
        }

        public void StartRun(int? seed)
        {
            RunSeed = seed;
            HasStarted = true;
            HelpVisible = false;
            PlaytestInfoVisible = false;
            RoomNumber = 1;
            Player = new PlayerModel(SelectedClass);
            LoadRoom();
        }

        public void ShowHelp(){HelpVisible = true; Changed?.Invoke();}
        public void HideHelp(){HelpVisible = false; Changed?.Invoke();}
        public void ToggleHelp(){HelpVisible = !HelpVisible; Changed?.Invoke();}
        public void ShowPlaytestInfo(){PlaytestInfoVisible = true; Changed?.Invoke();}
        public void HidePlaytestInfo(){PlaytestInfoVisible = false; Changed?.Invoke();}

        void LoadRoom()
        {
            RecordProgress();
            HitTiles.Clear();
            PreviewArea.Clear();
            LastTappedTile = null;
            RejectedTile = null;

            int seed = (RunSeed ?? Environment.TickCount) + RoomNumber * 97;
            var r = generator.Generate(seed, RoomNumber);
            Grid = r.Grid;
            Player.Position = r.PlayerSpawn;
            Player.ResetTurnResources();
            Theme = r.Theme;
            HazardTiles = r.HazardTiles;
            PropTiles = r.PropTiles;
            BlockerTiles = r.BlockerTiles;
            HealingPickup = r.HealingPickup;
            ArmedHazards.Clear();
            Enemies.Clear();

            for (int i = 0; i < r.EnemySpawns.Count; i++)
            {
                var enemy = new EnemyModel(BalanceConfig.EnemyFor(RoomNumber, i), r.EnemySpawns[i]);
                BalanceConfig.ApplyRoomScaling(enemy, RoomNumber);
                Enemies.Add(enemy);
            }

            Turns.BeginPlayerTurn();
            SelectedSkill = null;
            RefreshTargets();
            RefreshPreviews();
            string intro = RoomNumber == 5
                ? $"{Theme.Name}: BOSS ROOM - the Lantern Warden awakens."
                : $"{Theme.Name}: {Theme.HazardRule}";
            Message = string.IsNullOrEmpty(pendingRoomIntro) ? intro : $"{pendingRoomIntro} Next: {intro}";
            pendingRoomIntro = "";
            Changed?.Invoke();
        }

        public bool Occupied(Vector2Int p) => Enemies.Any(e => e.Alive && e.Position == p);
        public int LivingEnemies => Enemies.Count(e => e.Alive);
        public int ThreatDamageAt(Vector2Int p) => Enemies.Where(e => e.Alive && e.Preview.Contains(p)).Sum(e => e.AttackDamage);
        public string ThreatIntentAt(Vector2Int p)
        {
            var intents = Enemies.Where(e => e.Alive && (e.Preview.Contains(p) || e.DelayedPreview.Contains(p))).Select(e => $"{e.IntentLabel} {e.Threat}").Distinct().ToArray();
            return intents.Length == 0 ? "" : string.Join(", ", intents);
        }
        public ThreatKind ThreatKindAt(Vector2Int p)
        {
            var enemy = Enemies.FirstOrDefault(e => e.Alive && (e.Preview.Contains(p) || e.DelayedPreview.Contains(p)));
            return enemy?.Threat ?? ThreatKind.HP;
        }
        public string ThreatDetailAt(Vector2Int p)
        {
            var details = Enemies.Where(e => e.Alive && (e.Preview.Contains(p) || e.DelayedPreview.Contains(p)))
                .Select(e => e.Kind == EnemyKind.LanternWarden
                    ? $"{NameOf(e.Kind)}: {e.IntentLabel} {(e.Preview.Contains(p) ? $"now for {e.AttackDamage} damage" : "next turn")} - {EnemyAI.BossPhaseSummary(e)}"
                    : $"{NameOf(e.Kind)}: {ThreatReadability.ThreatName(e.Threat)} {(e.Preview.Contains(p) ? "now" : "next turn")}")
                .Distinct()
                .ToArray();
            if (details.Length > 0) return string.Join(" | ", details);
            if (HazardTiles.Contains(p)) return $"{Theme.HazardName}: {Theme.HazardRule}";
            if (HealingPickup.HasValue && HealingPickup.Value == p) return "Lantern bloom: step here to heal 3 HP.";
            if (BlockerTiles.Contains(p)) return "Blocker: blocks movement and line of sight.";
            return "";
        }
        public string IntentSummary => ThreatDamageAt(Player.Position) > 0
            ? $"DANGER: {ThreatDamageAt(Player.Position)} incoming damage on your tile"
            : Enemies.Any(e=>e.Alive&&e.DelayedPreview.Contains(Player.Position)) ? "WARNING: enemy intent targets your AP/MP or next tile" : "Safe tile - red spaces strike after End Turn";
        public bool HasFocusTile => LastTappedTile.HasValue;
        public string FocusThreatSummary => ThreatDetailAt(LastTappedTile ?? Player.Position);

        public void SelectSkill(SkillId id)
        {
            var def = SkillBook.Get(id);
            if (Turns.Phase != TurnPhase.Player) return;
            if (def.ClassId != Player.ClassId){Reject("That skill belongs to another class."); return;}
            if (Player.Cooldowns[def.Name] > 0){Reject($"{def.Name} is cooling down."); return;}
            if (Player.ActionPoints < def.ApCost){Reject($"Need {def.ApCost} AP."); return;}
            LastInputAccepted = true;
            RejectedTile = null;
            SelectedSkill = id;
            RefreshTargets();
            Message = ValidTargets.Count == 0 ? $"No valid targets for {def.Name}." : $"{def.Name}: choose a gold target. Costs {def.ApCost} AP.";
            Changed?.Invoke();
        }

        public void CancelSkill()
        {
            LastInputAccepted = true;
            RejectedTile = null;
            SelectedSkill = null;
            PreviewArea.Clear();
            RefreshTargets();
            Message = "Skill cancelled. Choose a tile, skill, or End Turn.";
            Changed?.Invoke();
        }

        public void TapTile(Vector2Int p)
        {
            if (Turns.Phase != TurnPhase.Player) return;
            LastTappedTile = p;
            HitTiles.Clear();
            if (!Grid.IsFloor(p)){Reject("The void cannot be crossed."); return;}
            if (!ValidTargets.Contains(p)){Reject(SelectedSkill.HasValue ? ExplainInvalidTarget(p) : "Tile is blocked, occupied, or beyond your MP."); return;}

            LastInputAccepted = true;
            RejectedTile = null;
            if (SelectedSkill.HasValue) UseSkill(SelectedSkill.Value, p);
            else MoveTo(p);
        }

        void MoveTo(Vector2Int p)
        {
            var path = Grid.ShortestPath(Player.Position, p, Occupied);
            int cost = path.Count;
            if (cost <= 0 || !Player.SpendMP(cost)){Reject($"Need {cost} MP."); return;}
            Player.Position = p;
            RefreshTargets();
            RefreshPreviews();
            string pickup = TryCollectHealingPickup(p);
            Message = !string.IsNullOrEmpty(pickup) ? pickup : Player.MovementPoints > 0 ? $"Moved {cost}. MP {Player.MovementPoints}/{Player.MoveRange}." : "No MP left. Use AP or End Turn.";
            Changed?.Invoke();
        }

        void UseSkill(SkillId id, Vector2Int p)
        {
            var def = SkillBook.Get(id);
            if (!Player.SpendAP(def.ApCost)){Reject($"Need {def.ApCost} AP."); return;}
            Player.Cooldowns[def.Name] = def.Cooldown + 1;
            SelectedSkill = null;
            PreviewArea = SkillBook.AffectedTiles(Grid, p, def);
            ApplySkill(def, p);
            ResolvePostAction();
        }

        void ApplySkill(SkillDefinition def, Vector2Int p)
        {
            if (def.Effect == SkillEffect.SelfShield)
            {
                int shield = Player.ClassId == PlayerClassId.Artificer ? 3 : 4;
                Player.Shield += shield;
                HitTiles.Add(Player.Position);
                Message = $"{def.Name}: gained {shield} shield.";
                return;
            }

            if (def.Effect == SkillEffect.DashDamage || def.Effect == SkillEffect.DiagonalMove)
            {
                Player.Position = p;
                foreach (var e in Enemies.Where(e => e.Alive && Manhattan(e.Position, p) == 1))
                {
                    e.Damage(def.Damage + Player.Power + MarkBonus(e));
                    bool pushed = TryPush(e, e.Position - p);
                    HitTiles.Add(e.Position);
                    if (pushed) HitTiles.Add(e.Position);
                }
                Message = def.Effect == SkillEffect.DiagonalMove ? "Diagonal Dash repositions you." : "Sun Charge crashes forward and pushes nearby foes.";
                return;
            }

            var target = Enemies.FirstOrDefault(e => e.Alive && e.Position == p);
            if (target == null){Message = $"{def.Name} fizzles."; return;}

            switch (def.Effect)
            {
                case SkillEffect.Mark:
                    if (def.Damage > 0) target.Damage(def.Damage + Player.Power);
                    target.MarkedTurns = 2;
                    HitTiles.Add(target.Position);
                    Message = "Marked: next hit deals bonus damage.";
                    break;
                case SkillEffect.AreaBurn:
                    foreach (var e in Enemies.Where(e => e.Alive && PreviewArea.Contains(e.Position)))
                    {
                        e.Damage(def.Damage + Player.Power + MarkBonus(e));
                        e.BurnTurns = 2;
                        HitTiles.Add(e.Position);
                    }
                    Message = $"Cinder Bloom burns {HitTiles.Count} tile(s).";
                    break;
                case SkillEffect.DelayedArea:
                    foreach (var e in Enemies.Where(e => e.Alive && PreviewArea.Contains(e.Position)))
                    {
                        e.Damage(def.Damage + Player.Power + MarkBonus(e));
                        HitTiles.Add(e.Position);
                    }
                    Message = $"Delayed Blast detonates {HitTiles.Count} tile(s).";
                    break;
                case SkillEffect.Swap:
                    (Player.Position, target.Position) = (target.Position, Player.Position);
                    HitTiles.Add(Player.Position);
                    HitTiles.Add(target.Position);
                    Message = "Shadow Swap: positions traded.";
                    break;
                case SkillEffect.Root:
                    target.Damage(def.Damage + Player.Power + MarkBonus(target));
                    target.RootTurns = Player.ClassId == PlayerClassId.Artificer ? 2 : 1;
                    HitTiles.Add(target.Position);
                    Message = "Lens Trap: target rooted.";
                    break;
                default:
                    int damage = def.Damage + Player.Power + MarkBonus(target) + BiomeRules.SkillDamageBonus(Theme, Player.Position, HazardTiles, def.Id);
                    target.Damage(damage);
                    HitTiles.Add(target.Position);
                    bool pushed = (def.Id == SkillId.SpearThrust || def.Id == SkillId.SunCharge) && TryPush(target, target.Position - Player.Position);
                    Message = target.Alive ? $"{def.Name}: {damage} damage{(pushed ? " + push" : "")}." : $"{def.Name}: {NameOf(target.Kind)} defeated.";
                    break;
            }
        }

        int MarkBonus(EnemyModel e)
        {
            if (e.MarkedTurns <= 0) return 0;
            e.MarkedTurns = 0;
            return 2;
        }

        bool TryPush(EnemyModel e, Vector2Int direction)
        {
            direction = new Vector2Int(Mathf.Clamp(direction.x, -1, 1), Mathf.Clamp(direction.y, -1, 1));
            var next = e.Position + direction;
            if (direction != Vector2Int.zero && Grid.IsFloor(next) && !Occupied(next) && next != Player.Position){e.Position = next; return true;}
            return false;
        }

        void ResolvePostAction()
        {
            RefreshPreviews();
            var outcome = GameRules.ResolveOutcome(Player, Enemies, RoomNumber);
            if (outcome == TurnPhase.Reward)
            {
                Turns.ShowReward();
                Message = "Room cleared. Choose one blessing.";
            }
            else if (outcome == TurnPhase.Won)
            {
                Turns.Win();
                RecordProgress(5);
                Message = "VICTORY - Lantern Warden defeated. Start New Run to replay.";
            }
            else
            {
                RefreshTargets();
                Message += $" AP {Player.ActionPoints}/{Player.MaxActionPoints}, MP {Player.MovementPoints}/{Player.MoveRange}.";
            }
            Changed?.Invoke();
        }

        public void WaitTurn()
        {
            if (Turns.Phase == TurnPhase.Player)
            {
                Message = "ENEMY TURN - red previews resolve now.";
                Turns.TryBeginEnemyTurn();
                StartCoroutine(EnemyTurn());
            }
        }

        IEnumerator EnemyTurn()
        {
            Changed?.Invoke();
            yield return new WaitForSeconds(.35f);
            ResolveArmedHazards();
            if (!Player.Alive){Turns.Lose(); RecordProgress(); Message = "DEFEAT - your lantern is extinguished. Start New Run to retry."; Changed?.Invoke(); yield break;}

            foreach (var e in Enemies.Where(x => x.Alive).ToList())
            {
                e.TickStatuses();
                if (!e.Alive){HitTiles.Add(e.Position); Message = $"{NameOf(e.Kind)} burns away."; Changed?.Invoke(); yield return new WaitForSeconds(.15f); continue;}
                HitTiles.Clear();
                if (TryAnnounceBossPhase(e))
                {
                    RefreshPreviews();
                    Changed?.Invoke();
                    yield return new WaitForSeconds(.85f);
                    continue;
                }
                if (e.Preview.Contains(Player.Position))
                {
                    Player.Damage(e.AttackDamage);
                    HitTiles.Add(Player.Position);
                    Message = $"{NameOf(e.Kind)} strikes your tile for {e.AttackDamage}.";
                }
                else if (e.DelayedPreview.Contains(Player.Position))
                {
                    ApplyIntentPressure(e);
                    HitTiles.Add(Player.Position);
                    Message = $"{NameOf(e.Kind)} triggers {e.IntentLabel}.";
                }
                else if (e.RootTurns <= 0)
                {
                    var before = e.Position;
                    var next = EnemyAI.ChooseReposition(e, Player.Position, Grid, q => Occupied(q) || q == Player.Position, p => HazardTiles.Contains(p), Enemies);
                    if (next != e.Position) e.Position = next;
                    Message = next == before ? $"{NameOf(e.Kind)} holds a threatening angle." : $"{NameOf(e.Kind)} repositions to pressure your next move.";
                }
                else Message = $"{NameOf(e.Kind)} is rooted.";
                Changed?.Invoke();
                yield return new WaitForSeconds(.2f);
            }

            if (!Player.Alive){Turns.Lose(); RecordProgress(); Message = "DEFEAT - your lantern is extinguished. Start New Run to retry."; Changed?.Invoke(); yield break;}
            var outcome = GameRules.ResolveOutcome(Player, Enemies, RoomNumber);
            if (outcome == TurnPhase.Reward)
            {
                Turns.ShowReward();
                Message = "Room cleared. Choose one blessing.";
                Changed?.Invoke();
                yield break;
            }
            if (outcome == TurnPhase.Won){Turns.Win(); RecordProgress(5); Message = "VICTORY - Lantern Warden defeated. Start New Run to replay."; Changed?.Invoke(); yield break;}

            Player.TickStatuses();
            Player.TickCooldowns();
            Player.ResetTurnResources();
            if (pendingApDrain > 0) Player.ActionPoints = Mathf.Max(0, Player.ActionPoints - pendingApDrain);
            if (pendingMpDrain > 0) Player.MovementPoints = Mathf.Max(0, Player.MovementPoints - pendingMpDrain);
            pendingApDrain = pendingMpDrain = 0;
            ArmHazards();
            Turns.BeginPlayerTurn();
            RefreshTargets();
            RefreshPreviews();
            Message = "PLAYER TURN - " + IntentSummary + " - " + Theme.HazardRule;
            Changed?.Invoke();
        }

        void RefreshTargets()
        {
            if (Grid == null || Player == null) return;
            if (SelectedSkill.HasValue)
            {
                var def = SkillBook.Get(SelectedSkill.Value);
                ValidTargets = SkillBook.Targets(Grid, Player, def, Occupied, BiomeRules.SkillRangeBonus(Theme, Player.Position, HazardTiles, def.Id));
                PreviewArea = ValidTargets.Count > 0 ? SkillBook.AffectedTiles(Grid, ValidTargets.First(), def) : new HashSet<Vector2Int>();
            }
            else ValidTargets = Grid.Reachable(Player.Position, BiomeRules.MoveRange(Player, Theme, HazardTiles), Occupied)
                .Where(p => Grid.ShortestPath(Player.Position, p, Occupied).Count <= Player.MovementPoints)
                .ToHashSet();
        }

        public void RefreshPreviews()
        {
            foreach (var e in Enemies.Where(x => x.Alive)) EnemyAI.AssignIntent(e, Player.Position, Grid);
        }

        void ApplyIntentPressure(EnemyModel e)
        {
            if (e.Threat == ThreatKind.AP || e.Threat == ThreatKind.Mixed) pendingApDrain += e.Kind == EnemyKind.LanternWarden ? 2 : 1;
            if (e.Threat == ThreatKind.MP || e.Threat == ThreatKind.Mixed) pendingMpDrain += 1;
        }

        bool TryAnnounceBossPhase(EnemyModel e)
        {
            if (e.Kind != EnemyKind.LanternWarden) return false;
            int phase = EnemyAI.BossPhase(e);
            if (phase <= e.BossPhaseAnnounced) return false;
            e.BossPhaseAnnounced = phase;
            if (phase == 2)
            {
                e.Shield += 4;
                BossPhaseBanner = "PHASE TWO";
                bossPhaseBannerUntil = Time.time + 2.2f;
                Message = "PHASE TWO - The Lantern Warden awakens. New attack pattern: overcharged range lines.";
            }
            else
            {
                BossPhaseBanner = "FINAL SURGE";
                bossPhaseBannerUntil = Time.time + 1.8f;
                Message = "Lantern Warden enters Phase 3 - HEAVY BLAST telegraphs. Avoid red and purple danger tiles.";
            }
            HitTiles.Add(e.Position);
            return true;
        }

        void ArmHazards()
        {
            ArmedHazards = BiomeRules.IsDelayedDamage(Theme) ? new HashSet<Vector2Int>(HazardTiles) : new HashSet<Vector2Int>();
        }

        void ResolveArmedHazards()
        {
            int playerDamage = BiomeRules.HazardDamage(Theme, Player.Position, ArmedHazards);
            if (playerDamage > 0)
            {
                Player.Damage(playerDamage);
                HitTiles.Add(Player.Position);
                Message = Theme.Hazard == HazardKind.EmberVent ? "An ember vent erupts for 2 damage." : "Charged plates arc for 2 damage.";
            }
            if (Theme.Hazard == HazardKind.ChargedFloor)
            {
                foreach (var e in Enemies.Where(e => e.Alive))
                {
                    int damage = BiomeRules.HazardDamage(Theme, e.Position, ArmedHazards);
                    if (damage > 0){e.Damage(damage); HitTiles.Add(e.Position);}
                }
            }
        }

        string TryCollectHealingPickup(Vector2Int p)
        {
            if (!HealingPickup.HasValue || HealingPickup.Value != p) return "";
            int healed = Player.Recover(BalanceConfig.HealingPickupAmount);
            HealingPickup = null;
            HitTiles.Add(p);
            return healed > 0 ? $"Lantern bloom restored {healed} HP." : "Lantern bloom collected, but HP is already full.";
        }

        public void ChooseReward(int choice)
        {
            if (Turns.Phase != TurnPhase.Reward) return;
            if (choice < 0 || choice >= RewardCatalog.All.Length){Reject("Choose one visible reward card."); return;}
            LastInputAccepted = true;
            RejectedTile = null;
            if (choice == 0){Player.MaxHealth += 3; Player.Health = Mathf.Min(Player.MaxHealth, Player.Health + 3); pendingRoomIntro = "Reward applied: Vital Ember (+3 Max HP, heal 3 now).";}
            else if (choice == 1){Player.Power += 1; pendingRoomIntro = "Reward applied: Bright Wick (+1 all skill damage).";}
            else {Player.MoveRange += 1; Player.MovementPoints += 1; pendingRoomIntro = "Reward applied: Swift Flame (+1 MP movement).";}
            RoomNumber++;
            LoadRoom();
        }

        public void Restart()
        {
            StopAllCoroutines();
            StartRun();
        }

        string ExplainInvalidTarget(Vector2Int p)
        {
            var def = SelectedSkill.HasValue ? SkillBook.Get(SelectedSkill.Value) : null;
            if (def == null) return "Invalid target.";
            int d = Manhattan(Player.Position, p);
            if (d > def.Range) return "Target is out of range.";
            if (def.RequiresLineOfSight && !SkillBook.HasLineOfSight(Grid, Player.Position, p)) return "Line of sight is blocked.";
            return "No valid target there.";
        }

        void Reject(string why)
        {
            LastInputAccepted = false;
            RejectedTile = LastTappedTile;
            Message = "INVALID: " + why + " Cyan = move, gold = skill target, red = danger.";
            Changed?.Invoke();
        }

        void RecordProgress(int roomOverride = 0)
        {
            int reached = Mathf.Max(roomOverride, RoomNumber);
            if (reached > BestRoomReached)
            {
                BestRoomReached = reached;
                PlayerPrefs.SetInt(BestRoomKey, BestRoomReached);
                PlayerPrefs.Save();
            }
        }

        static int Manhattan(Vector2Int a, Vector2Int b) => Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
        public static string NameOf(EnemyKind k) => k switch
        {
            EnemyKind.Ashling => "Ashling",
            EnemyKind.GloomArcher => "Gloom Archer",
            EnemyKind.StoneSentinel => "Stone Sentinel",
            _ => "Lantern Warden"
        };
    }
}



