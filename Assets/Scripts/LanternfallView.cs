using System.Linq;
using UnityEngine;

namespace Lanternfall
{
    public sealed class LanternfallView : MonoBehaviour
    {
        LanternfallGame game;
        Camera cam;
        GUIStyle title, body, button, center, small;
        float tile = 1f;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void Boot()
        {
            if (FindAnyObjectByType<LanternfallGame>() == null)
                new GameObject("Lanternfall Tactics").AddComponent<LanternfallView>();
        }

        void Awake()
        {
            Application.targetFrameRate = 30;
            QualitySettings.vSyncCount = 0;
            QualitySettings.antiAliasing = 0;
            QualitySettings.shadows = ShadowQuality.Disable;
            QualitySettings.shadowDistance = 0;
            QualitySettings.softParticles = false;
            QualitySettings.realtimeReflectionProbes = false;
            QualitySettings.anisotropicFiltering = AnisotropicFiltering.Disable;
            Input.multiTouchEnabled = false;

            game = gameObject.AddComponent<LanternfallGame>();
            cam = new GameObject("Camera").AddComponent<Camera>();
            cam.orthographic = true;
            cam.transform.position = new Vector3(4, 5, -10);
            cam.backgroundColor = new Color(.025f, .02f, .06f);
            cam.orthographicSize = 6.7f;
            game.Changed += () => {};
        }

        void InitStyles()
        {
            int s = Mathf.Clamp(Mathf.Min(Screen.width, Screen.height) / 28, 18, 30);
            title = new GUIStyle(GUI.skin.label){fontSize = s + 5, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter, wordWrap = true, normal = {textColor = new Color(1f, .78f, .28f)}};
            body = new GUIStyle(GUI.skin.label){fontSize = s, alignment = TextAnchor.MiddleLeft, wordWrap = true, normal = {textColor = Color.white}};
            center = new GUIStyle(body){alignment = TextAnchor.MiddleCenter};
            small = new GUIStyle(center){fontSize = Mathf.Max(14, s - 3)};
            button = new GUIStyle(GUI.skin.button)
            {
                fontSize = s,
                fontStyle = FontStyle.Bold,
                wordWrap = true,
                normal = {textColor = Color.white, background = Tex(new Color(.12f, .12f, .2f))},
                hover = {background = Tex(new Color(.23f, .2f, .32f))},
                active = {background = Tex(new Color(.55f, .32f, .08f))}
            };
        }

        Texture2D Tex(Color c)
        {
            var t = new Texture2D(1, 1);
            t.SetPixel(0, 0, c);
            t.Apply();
            return t;
        }

        void OnGUI()
        {
            if (title == null) InitStyles();
            var guiSafe = MobileLayout.ToGuiSafeArea(Screen.height, Screen.safeArea);
            GUI.BeginGroup(guiSafe);

            if (!game.HasStarted) DrawStartScreen(new Rect(0, 0, guiSafe.width, guiSafe.height));
            else
            {
                var layout = MobileLayout.Compute(guiSafe.width, guiSafe.height);
                DrawBoard(layout.Board, layout.CompactLandscape);
                if (layout.Portrait) DrawPortraitPanel(layout.Panel);
                else DrawPanel(layout.Panel, layout.CompactLandscape);
            }

            if (game.HelpVisible) DrawHelpOverlay(new Rect(0, 0, guiSafe.width, guiSafe.height));
            GUI.EndGroup();
        }

        void DrawStartScreen(Rect area)
        {
            DrawRect(area, new Color(.025f, .02f, .055f));
            bool compact = area.height < 500f;
            float pad = Mathf.Max(18, area.width * .06f);
            float w = area.width - pad * 2;
            float y = compact ? 14 : Mathf.Max(26, area.height * .10f);

            GUI.Label(new Rect(pad, y, w, compact ? 40 : 72), "LANTERNFALL TACTICS", title); y += compact ? 46 : 82;
            GUI.Label(new Rect(pad, y, w, compact ? 54 : 118),
                compact ? "Turn tactics: spend AP/MP, read red previews, survive five rooms."
                        : "A short turn-based roguelite prototype.\nMove on cyan tiles, read the red enemy previews, choose rewards, and survive five rooms.",
                center); y += compact ? 58 : 128;
            var cls = ClassCatalog.Get(game.SelectedClass);
            GUI.Label(new Rect(pad, y, w, compact ? 42 : 72), compact ? $"{cls.name} / {cls.title}" : $"{cls.name} / {cls.title}\n{cls.description}", center); y += compact ? 48 : 82;
            if (game.BestRoomReached > 0 && !compact){GUI.Label(new Rect(pad, y, w, 34), $"Best run: room {game.BestRoomReached}/5", center); y += 42;}
            float gap = compact ? 8 : 12;
            float h = compact ? 46 : 62;
            float bw = compact ? (w - gap * 2) / 3f : w;
            if (compact)
            {
                if (GUI.Button(new Rect(pad, y, bw, h), "CLASS", button)) game.CycleClass();
                if (GUI.Button(new Rect(pad + bw + gap, y, bw, h), "START", button)) game.StartRun();
                if (GUI.Button(new Rect(pad + (bw + gap) * 2, y, bw, h), "HELP", button)) game.ShowHelp();
                y += h + 10;
            }
            else
            {
                if (GUI.Button(new Rect(pad, y, w, 58), "CHANGE CLASS", button)) game.CycleClass(); y += 70;
                if (GUI.Button(new Rect(pad, y, w, 66), "START RUN", button)) game.StartRun(); y += 78;
                if (GUI.Button(new Rect(pad, y, w, 60), "HOW TO PLAY", button)) game.ShowHelp(); y += 76;
            }
            if(!compact)GUI.Label(new Rect(pad, y, w, 80), "Built for touch first. Mouse clicks work in the editor and Windows build.", small);
        }

        void DrawHelpOverlay(Rect area)
        {
            DrawRect(area, new Color(0f, 0f, 0f, .82f));
            float pad = Mathf.Max(18, area.width * .06f);
            float w = area.width - pad * 2;
            float y = Mathf.Max(20, area.height * .08f);
            DrawRect(new Rect(pad * .5f, y - 12, area.width - pad, area.height - y * 2 + 24), new Color(.055f, .045f, .085f));
            GUI.Label(new Rect(pad, y, w, 44), "HOW TO PLAY", title); y += 54;
            foreach (var line in LanternfallGame.HowToPlayLines)
            {
                float lineH = area.height < 500f ? 34 : 54;
                GUI.Label(new Rect(pad, y, w, lineH), "- " + line, area.height < 500f ? small : body);
                y += lineH + 4;
            }
            if (GUI.Button(new Rect(pad, area.height - (area.height < 500f ? 58 : 84), w, area.height < 500f ? 46 : 62), game.HasStarted ? "BACK TO RUN" : "GOT IT", button))
                game.HideHelp();
        }

        void DrawBoard(Rect area, bool compact = false)
        {
            DrawRect(area, game.Theme.Background);
            float top = compact ? 54 : 72;
            tile = Mathf.Min((area.width - 24) / game.Grid.Width, (area.height - top - 16) / game.Grid.Height);
            float ox = area.x + (area.width - game.Grid.Width * tile) / 2;
            float oy = area.y + top + (area.height - top - game.Grid.Height * tile) / 2;

            string turn = game.Turns.Phase == TurnPhase.Enemy ? "ENEMY TURN" : game.Turns.Phase == TurnPhase.Reward ? "ROOM CLEAR" : game.Turns.Phase.ToString().ToUpper();
            GUI.Label(new Rect(area.x, area.y + 2, area.width, compact ? 28 : 38), turn, title);
            GUI.Label(new Rect(area.x, area.y + (compact ? 28 : 38), area.width, compact ? 22 : 28), game.RoomNumber == 5 ? "BOSS ROOM - " + game.Theme.Name : game.Theme.Name, center);

            foreach (var p in game.Grid.Floors())
            {
                var r = TileRect(p, ox, oy);
                Color c = (p.x + p.y) % 2 == 0 ? game.Theme.Floor : game.Theme.Alternate;
                if (game.HazardTiles.Contains(p)) c = game.ArmedHazards.Contains(p) ? game.Theme.WarningColor : game.Theme.HazardColor;
                if (game.Enemies.Any(e => e.Alive && e.Preview.Contains(p))) c = new Color(.62f, .12f, .13f);
                if (game.PreviewArea.Contains(p)) c = new Color(.7f, .35f, .04f);
                if (game.ValidTargets.Contains(p) && game.Turns.Phase == TurnPhase.Player) c = game.SelectedSkill.HasValue ? new Color(.78f, .55f, .08f) : new Color(.08f, .52f, .56f);
                if (game.HitTiles.Contains(p)) c = new Color(1f, .86f, .15f);
                DrawRect(r, c);
                if (game.LastTappedTile == p) DrawOutline(r, Color.white, 3);
                if (game.RejectedTile == p) DrawOutline(r, new Color(1f, .1f, .1f), 4);
                if (Event.current.type == EventType.MouseDown && r.Contains(Event.current.mousePosition)){game.TapTile(p); Event.current.Use();}
            }

            string hazardGlyph = game.Theme.Hazard switch
            {
                HazardKind.ShallowWater => "~",
                HazardKind.Prism => "<>",
                HazardKind.EmberVent => "!",
                HazardKind.GraspingRoots => "#",
                _ => "Z"
            };
            foreach (var p in game.HazardTiles)
                GUI.Label(TileRect(p, ox, oy), hazardGlyph, new GUIStyle(center){fontSize = Mathf.Max(14, Mathf.RoundToInt(tile * .32f)), fontStyle = FontStyle.Bold, normal = {textColor = Color.white}});

            foreach (var p in game.PropTiles)
                GUI.Label(TileRect(p, ox, oy), game.Theme.PropGlyph, new GUIStyle(center){fontSize = Mathf.RoundToInt(tile * .38f), fontStyle = FontStyle.Bold, normal = {textColor = game.Theme.Accent}});

            foreach (var p in game.Enemies.Where(e => e.Alive).SelectMany(e => e.Preview).Distinct())
                GUI.Label(TileRect(p, ox, oy), "!", new GUIStyle(center){fontSize = Mathf.Max(16, Mathf.RoundToInt(tile * .42f)), fontStyle = FontStyle.Bold, normal = {textColor = Color.white}});

            DrawToken(game.Player.Position, ox, oy, new Color(.2f, .9f, 1f), "*");
            foreach (var e in game.Enemies.Where(e => e.Alive))
            {
                Color c = e.Kind == EnemyKind.LanternWarden ? new Color(.9f, .25f, .8f) : new Color(.85f, .33f, .2f);
                string glyph = e.Kind switch {EnemyKind.Ashling => "A", EnemyKind.GloomArcher => "G", EnemyKind.StoneSentinel => "S", _ => "W"};
                DrawToken(e.Position, ox, oy, c, glyph, $"{e.Health}");
            }
        }

        Rect TileRect(Vector2Int p, float ox, float oy) => new(ox + p.x * tile, oy + (game.Grid.Height - 1 - p.y) * tile, tile - 2, tile - 2);

        void DrawToken(Vector2Int p, float ox, float oy, Color c, string glyph, string hp = "")
        {
            var r = new Rect(ox + p.x * tile + tile * .12f, oy + (game.Grid.Height - 1 - p.y) * tile + tile * .08f, tile * .72f, tile * .72f);
            DrawRect(r, c);
            GUI.Label(r, glyph, new GUIStyle(center){fontSize = Mathf.RoundToInt(tile * .42f), fontStyle = FontStyle.Bold, normal = {textColor = Color.black}});
            if (hp != "") GUI.Label(new Rect(r.x, r.y + r.height - 18, r.width, 20), hp, new GUIStyle(center){fontSize = 14, fontStyle = FontStyle.Bold, normal = {textColor = Color.white}});
        }

        void DrawPanel(Rect r, bool compact)
        {
            DrawRect(r, new Color(.045f, .04f, .075f));
            float x = r.x + 10, w = r.width - 20, y = compact ? 2 : 12;
            GUI.Label(new Rect(x, y, w, compact ? 30 : 42), "LANTERNFALL", title); y += compact ? 30 : 48;
            if (GUI.Button(new Rect(x, y, w, compact ? 38 : 44), "HOW TO PLAY", button)) game.ShowHelp(); y += compact ? 42 : 50;
            GUI.Label(new Rect(x, y, w, compact ? 58 : 110),
                compact ? $"R{game.RoomNumber}/5 - HP {game.Player.Health}/{game.Player.MaxHealth} - AP {game.Player.ActionPoints}/{game.Player.MaxActionPoints} - MP {game.Player.MovementPoints}/{game.Player.MoveRange}\n{game.Message}"
                        : $"{ClassCatalog.Get(game.Player.ClassId).name} - {game.Theme.Name}\nRoom {game.RoomNumber}/5 - HP {game.Player.Health}/{game.Player.MaxHealth} - AP {game.Player.ActionPoints}/{game.Player.MaxActionPoints} - MP {game.Player.MovementPoints}/{game.Player.MoveRange}\n{game.Message}",
                compact ? center : body); y += compact ? 62 : 116;

            if (DrawEndOrReward(x, w, ref y, compact)) return;
            if (!compact){GUI.Label(new Rect(x, y, w, 34), "SKILLS", title); y += 40;}
            DrawSkills(x, w, ref y, compact);

            float actionH = compact ? 48 : 54;
            if (game.SelectedSkill.HasValue && GUI.Button(new Rect(x, y, w * .48f, actionH), "CANCEL", button)) game.CancelSkill();
            GUI.enabled = game.Turns.Phase == TurnPhase.Player;
            if (GUI.Button(new Rect(x + w * .52f, y, w * .48f, actionH), "END TURN", button)) game.WaitTurn();
            GUI.enabled = true; y += actionH + 4;
            GUI.Label(new Rect(x, y, w, compact ? 48 : 150), $"{game.Theme.HazardName}: {game.Theme.HazardRule}\nRED attack - CYAN move - GOLD skill", compact ? center : body);
        }

        void DrawPortraitPanel(Rect r)
        {
            DrawRect(r, new Color(.045f, .04f, .075f));
            float pad = 10, x = r.x + pad, w = r.width - pad * 2, y = r.y + 6;
            GUI.Label(new Rect(x, y, w, 32), $"{ClassCatalog.Get(game.Player.ClassId).name.ToUpper()} - ROOM {game.RoomNumber}/5 - HP {game.Player.Health}/{game.Player.MaxHealth} - AP {game.Player.ActionPoints}/{game.Player.MaxActionPoints} - MP {game.Player.MovementPoints}/{game.Player.MoveRange}", center); y += 34;
            GUI.Label(new Rect(x, y, w, 46), game.Message, center); y += 48;
            if (GUI.Button(new Rect(x, y, w, 46), "HOW TO PLAY", button)){game.ShowHelp(); return;} y += 52;
            if (DrawEndOrReward(x, w, ref y, false)) return;
            DrawSkills(x, w, ref y, true); y += 4;
            if (game.SelectedSkill.HasValue && GUI.Button(new Rect(x, y, w * .48f, 54), "CANCEL SKILL", button)) game.CancelSkill();
            GUI.enabled = game.Turns.Phase == TurnPhase.Player;
            if (GUI.Button(new Rect(x + w * .52f, y, w * .48f, 54), "END TURN", button)) game.WaitTurn();
            GUI.enabled = true; y += 58;
            GUI.Label(new Rect(x, y, w, 48), $"{game.Theme.HazardName}: {game.Theme.HazardRule}\nRED = attack   CYAN = move   GOLD = skill", center);
        }

        bool DrawEndOrReward(float x, float w, ref float y, bool compact)
        {
            if (game.Turns.Phase == TurnPhase.Reward)
            {
                GUI.Label(new Rect(x, y, w, compact ? 32 : 42), "ROOM CLEAR - CHOOSE ONE", title); y += compact ? 34 : 48;
                if (compact) DrawCompactRewards(x, w, ref y);
                else DrawPortraitRewards(x, w, ref y);
                return true;
            }
            if (game.Turns.Phase == TurnPhase.Won || game.Turns.Phase == TurnPhase.Lost)
            {
                GUI.Label(new Rect(x, y, w, compact ? 80 : 100), game.Turns.Phase == TurnPhase.Won ? "VICTORY\nThe Warden falls." : "DEFEAT\nThe dark closes in.", title);
                y += compact ? 86 : 112;
                if (GUI.Button(new Rect(x, y, w, 64), "START NEW RUN", button)) game.Restart();
                return true;
            }
            return false;
        }

        void DrawSkills(float x, float w, ref float y, bool row)
        {
            if (row)
            {
                float gap = 8, bw = (w - gap * 2) / 3f;
                var skills = SkillBook.ForClass(game.Player.ClassId);
                for (int i = 0; i < skills.Length; i++)
                {
                    var s = skills[i]; int cd = game.Player.Cooldowns[s.Name];
                    GUI.enabled = game.Turns.Phase == TurnPhase.Player && cd == 0 && game.Player.ActionPoints >= s.ApCost;
                    string selected = game.SelectedSkill == s.Id ? "> " : "";
                    if (GUI.Button(new Rect(x + i * (bw + gap), y, bw, 64), $"{selected}{ShortSkill(s)}\n{s.ApCost} AP {(cd > 0 ? $"CD {cd}" : "READY")}", button)) game.SelectSkill(s.Id);
                }
                GUI.enabled = true; y += 72;
                return;
            }

            foreach (var s in SkillBook.ForClass(game.Player.ClassId))
            {
                int cd = game.Player.Cooldowns[s.Name];
                string selected = game.SelectedSkill == s.Id ? "> " : "";
                string label = $"{selected}{s.Name} - {s.ApCost} AP - {(cd > 0 ? $"CD {cd}" : "READY")}\n{s.Hint}";
                GUI.enabled = game.Turns.Phase == TurnPhase.Player && cd == 0 && game.Player.ActionPoints >= s.ApCost;
                if (GUI.Button(new Rect(x, y, w, 68), label, button)) game.SelectSkill(s.Id);
                GUI.enabled = true; y += 76;
            }
        }

        string ShortSkill(SkillDefinition s)
        {
            var parts = s.Name.Split(' ');
            return parts.Length == 1 ? s.Name.ToUpper() : parts[0].ToUpper();
        }

        void DrawPortraitRewards(float x, float w, ref float y)
        {
            float gap = 8, bw = (w - gap * 2) / 3f;
            string[] labels = {"VITAL EMBER\n+3 MAX HP", "BRIGHT WICK\n+1 DAMAGE", "SWIFT FLAME\n+1 MP"};
            for (int i = 0; i < 3; i++)
                if (GUI.Button(new Rect(x + i * (bw + gap), y, bw, 82), labels[i], button)) game.ChooseReward(i);
        }

        void DrawCompactRewards(float x, float w, ref float y)
        {
            float gap = 6, bw = (w - gap * 2) / 3f;
            string[] labels = {"VITAL\n+3 HP", "WICK\n+1 DMG", "SWIFT\n+1 MP"};
            for (int i = 0; i < 3; i++)
                if (GUI.Button(new Rect(x + i * (bw + gap), y, bw, 76), labels[i], button)) game.ChooseReward(i);
        }

        void DrawRect(Rect r, Color c)
        {
            var old = GUI.color;
            GUI.color = c;
            GUI.DrawTexture(r, Texture2D.whiteTexture);
            GUI.color = old;
        }

        void DrawOutline(Rect r, Color c, float px)
        {
            DrawRect(new Rect(r.x, r.y, r.width, px), c);
            DrawRect(new Rect(r.x, r.yMax - px, r.width, px), c);
            DrawRect(new Rect(r.x, r.y, px, r.height), c);
            DrawRect(new Rect(r.xMax - px, r.y, px, r.height), c);
        }
    }
}
