using System.Linq;
using UnityEngine;

namespace Lanternfall
{
    public sealed class LanternfallView : MonoBehaviour
    {
        public const string PrototypeVersion = "Prototype v0.6A";
        LanternfallGame game;
        Camera cam;
        GUIStyle title, body, button, center, small;
        GUIStyle hudHeader, hudChip, hudMessage, hudButton, hudSkill, hudSkillCompact, hudTiny;
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
            bool phoneSized = Mathf.Min(Screen.width, Screen.height) < 620 && Mathf.Max(Screen.width, Screen.height) <= 1200;
            int s = phoneSized ? Mathf.Clamp(Mathf.Min(Screen.width, Screen.height) / 8, 38, 48) : Mathf.Clamp(Mathf.Min(Screen.width, Screen.height) / 28, 18, 30);
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
            hudHeader = new GUIStyle(center){fontSize = Mathf.Clamp(s - 10, phoneSized ? 28 : 15, phoneSized ? 36 : 19), fontStyle = FontStyle.Bold, wordWrap = true, normal = {textColor = new Color(1f, .80f, .32f)}};
            hudChip = new GUIStyle(center){fontSize = Mathf.Clamp(s - 1, phoneSized ? 36 : 15, phoneSized ? 46 : 19), fontStyle = FontStyle.Bold, wordWrap = false, normal = {textColor = Color.white}};
            hudMessage = new GUIStyle(center){fontSize = Mathf.Clamp(s - 7, phoneSized ? 28 : 13, phoneSized ? 34 : 16), fontStyle = FontStyle.Normal, wordWrap = true, normal = {textColor = Color.white}};
            hudTiny = new GUIStyle(center){fontSize = Mathf.Clamp(s - 7, phoneSized ? 28 : 12, phoneSized ? 34 : 15), fontStyle = FontStyle.Bold, wordWrap = true, normal = {textColor = new Color(.88f, .90f, 1f)}};
            hudButton = new GUIStyle(button){fontSize = Mathf.Clamp(s - 3, phoneSized ? 34 : 13, phoneSized ? 44 : 17), fontStyle = FontStyle.Bold, wordWrap = true};
            hudSkill = new GUIStyle(button){fontSize = Mathf.Clamp(s - 5, phoneSized ? 32 : 13, phoneSized ? 40 : 16), fontStyle = FontStyle.Bold, wordWrap = true, alignment = TextAnchor.MiddleCenter};
            hudSkillCompact = new GUIStyle(button){fontSize = Mathf.Clamp(s - 6, phoneSized ? 31 : 12, phoneSized ? 38 : 14), fontStyle = FontStyle.Bold, wordWrap = true, alignment = TextAnchor.MiddleCenter};
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
            var layout = MobileLayout.Compute(guiSafe.width, guiSafe.height);
            if (layout.PhoneHud && layout.Portrait)
            {
                DrawRotatePhoneScreen(new Rect(0, 0, guiSafe.width, guiSafe.height));
                GUI.EndGroup();
                return;
            }

            if (!game.HasStarted) DrawStartScreen(new Rect(0, 0, guiSafe.width, guiSafe.height));
            else
            {
                DrawBoard(layout.Board, layout.Portrait || layout.CompactLandscape);
                if (layout.Portrait) DrawPortraitPanel(layout.Panel);
                else DrawPanel(layout.Panel, layout.CompactLandscape);
                if (!layout.PhoneHud) GUI.Label(new Rect(8, guiSafe.height - 24, 160, 20), PrototypeVersion, small);
            }

            if (game.HelpVisible) DrawHelpOverlay(new Rect(0, 0, guiSafe.width, guiSafe.height));
            if (game.PlaytestInfoVisible) DrawPlaytestInfoOverlay(new Rect(0, 0, guiSafe.width, guiSafe.height));
            if (game.BossPhasePresentationActive) DrawBossPhaseOverlay(new Rect(0, 0, guiSafe.width, guiSafe.height));
            GUI.EndGroup();
        }

        void DrawBossPhaseOverlay(Rect area)
        {
            DrawRect(area, new Color(0f, 0f, 0f, .48f));
            float shake = Mathf.Sin(Time.time * 48f) * 5f;
            var banner = new Rect(area.x + area.width * .08f + shake, area.y + area.height * .30f, area.width * .84f, Mathf.Clamp(area.height * .22f, 96f, 160f));
            DrawRect(banner, new Color(.12f, .035f, .10f, .94f));
            DrawOutline(banner, new Color(1f, .48f, .12f), 4);
            GUI.Label(new Rect(banner.x + 12f, banner.y + 10f, banner.width - 24f, banner.height * .48f), game.BossPhaseBanner, new GUIStyle(title){fontSize = Mathf.Max(title.fontSize + 12, 44), normal = {textColor = new Color(1f, .54f, .16f)}});
            GUI.Label(new Rect(banner.x + 16f, banner.y + banner.height * .54f, banner.width - 32f, banner.height * .38f), "The Lantern Warden awakens.\nOvercharged range lines flare across the arena.", center);
        }

        void DrawRotatePhoneScreen(Rect area)
        {
            DrawRect(area, new Color(.025f, .02f, .055f));
            float pad = Mathf.Max(18f, area.width * .08f);
            var panel = new Rect(pad, Mathf.Max(24f, area.height * .18f), area.width - pad * 2f, area.height * .58f);
            DrawRect(panel, new Color(.055f, .045f, .085f));
            DrawOutline(panel, new Color(.72f, .47f, .14f), 3);
            float y = panel.y + 22f;
            GUI.Label(new Rect(panel.x + 12f, y, panel.width - 24f, 62f), "LANTERNFALL TACTICS", title); y += 76f;
            GUI.Label(new Rect(panel.x + 12f, y, panel.width - 24f, 96f), "Rotate your phone to play", new GUIStyle(title){fontSize = Mathf.Max(title.fontSize, 36)}); y += 106f;
            GUI.Label(new Rect(panel.x + 18f, y, panel.width - 36f, 70f), "Lanternfall Tactics is best played in landscape.", center); y += 78f;
            GUI.Label(new Rect(panel.x + 18f, y, panel.width - 36f, 56f), "For best phone play, add to Home Screen or use fullscreen if available.", small);
        }

        void DrawStartScreen(Rect area)
        {
            DrawRect(area, new Color(.025f, .02f, .055f));
            bool compact = area.height < 500f;
            float pad = Mathf.Max(18, area.width * .06f);
            float w = area.width - pad * 2;
            float y = compact ? 14 : Mathf.Max(26, area.height * .10f);
            DrawRect(new Rect(pad * .55f, y - 10, area.width - pad * 1.1f, area.height - y * 1.35f), new Color(.055f, .045f, .085f));
            DrawOutline(new Rect(pad * .55f, y - 10, area.width - pad * 1.1f, area.height - y * 1.35f), new Color(.55f, .38f, .12f), 2);

            GUI.Label(new Rect(pad, y, w, compact ? 40 : 72), "LANTERNFALL TACTICS", title); y += compact ? 34 : 58;
            GUI.Label(new Rect(pad, y, w, compact ? 22 : 28), PrototypeVersion, small); y += compact ? 24 : 34;
            GUI.Label(new Rect(pad, y, w, compact ? 54 : 118),
                compact ? "Turn tactics: spend AP/MP, avoid red previews, survive five rooms."
                        : "A short turn-based roguelite prototype.\nFirst time? Open How to Play, then move on cyan tiles, spend AP on skills, avoid red previews, and survive five rooms.",
                center); y += compact ? 58 : 128;
            var cls = ClassCatalog.Get(game.SelectedClass);
            GUI.Label(new Rect(pad, y, w, compact ? 42 : 72), compact ? $"{cls.name} / {cls.title}" : $"{cls.name} / {cls.title}\n{cls.description}", center); y += compact ? 48 : 82;
            if (game.BestRoomReached > 0 && !compact){GUI.Label(new Rect(pad, y, w, 34), $"Best run: room {game.BestRoomReached}/5", center); y += 42;}
            float gap = compact ? 8 : 12;
            float h = compact ? 46 : 62;
            float bw = compact ? (w - gap * 2) / 3f : w;
            if (compact)
            {
                bw = (w - gap * 3) / 4f;
                if (GUI.Button(new Rect(pad, y, bw, h), "CLASS", button)) game.CycleClass();
                if (GUI.Button(new Rect(pad + bw + gap, y, bw, h), "START", button)) game.StartRun();
                if (GUI.Button(new Rect(pad + (bw + gap) * 2, y, bw, h), "HELP", button)) game.ShowHelp();
                if (GUI.Button(new Rect(pad + (bw + gap) * 3, y, bw, h), "INFO", button)) game.ShowPlaytestInfo();
                y += h + 10;
            }
            else
            {
                if (GUI.Button(new Rect(pad, y, w, 58), "CHANGE CLASS", button)) game.CycleClass(); y += 70;
                if (GUI.Button(new Rect(pad, y, w, 66), "START RUN", button)) game.StartRun(); y += 78;
                if (GUI.Button(new Rect(pad, y, w, 60), "HOW TO PLAY", button)) game.ShowHelp(); y += 76;
                if (GUI.Button(new Rect(pad, y, w, 56), "PLAYTEST INFO", button)) game.ShowPlaytestInfo(); y += 66;
            }
            if(!compact)GUI.Label(new Rect(pad, y, w, 90), "Prototype playtest - please note what confused you, what felt fun, and if anything broke.\nBuilt for touch first. Mouse clicks work in browser, editor, and Windows build.", small);
        }

        void DrawHelpOverlay(Rect area)
        {
            DrawRect(area, new Color(0f, 0f, 0f, .82f));
            float pad = Mathf.Max(18, area.width * .06f);
            float w = area.width - pad * 2;
            float y = Mathf.Max(20, area.height * .08f);
            var panel = new Rect(pad * .5f, y - 12, area.width - pad, area.height - y * 2 + 24);
            DrawRect(panel, new Color(.055f, .045f, .085f));
            DrawOutline(panel, new Color(.55f, .38f, .12f), 2);
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

        void DrawPlaytestInfoOverlay(Rect area)
        {
            DrawRect(area, new Color(0f, 0f, 0f, .82f));
            float pad = Mathf.Max(18, area.width * .06f);
            float w = area.width - pad * 2;
            float y = Mathf.Max(20, area.height * .08f);
            var panel = new Rect(pad * .5f, y - 12, area.width - pad, area.height - y * 2 + 24);
            DrawRect(panel, new Color(.055f, .045f, .085f));
            DrawOutline(panel, new Color(.55f, .38f, .12f), 2);
            GUI.Label(new Rect(pad, y, w, 44), "PLAYTEST INFO", title); y += 54;
            foreach (var line in LanternfallGame.PlaytestInfoLines)
            {
                float lineH = area.height < 500f ? 36 : 56;
                GUI.Label(new Rect(pad, y, w, lineH), "- " + line, area.height < 500f ? small : body);
                y += lineH + 4;
            }
            if (GUI.Button(new Rect(pad, area.height - (area.height < 500f ? 58 : 84), w, area.height < 500f ? 46 : 62), "BACK", button))
                game.HidePlaytestInfo();
        }

        void DrawBoard(Rect area, bool compact = false)
        {
            DrawRect(area, game.Theme.Background);
            DrawOutline(area, new Color(.12f, .10f, .18f), 3);
            bool phoneBoard = compact && area.height < 300f;
            float top = compact ? (phoneBoard ? 28 : 42) : 64;
            var floors = game.Grid.Floors().ToList();
            int minX = floors.Min(p => p.x);
            int maxX = floors.Max(p => p.x);
            int minY = floors.Min(p => p.y);
            int maxY = floors.Max(p => p.y);
            int boardCols = Mathf.Max(1, maxX - minX + 1);
            int boardRows = Mathf.Max(1, maxY - minY + 1);
            var fit = BoardFitLayout.Compute(area, boardCols, boardRows, compact);
            tile = fit.TileSize;
            float ox = fit.Bounds.x;
            float oy = fit.Bounds.y;

            string turn = game.Turns.Phase == TurnPhase.Enemy ? "ENEMY TURN" : game.Turns.Phase == TurnPhase.Reward ? "ROOM CLEAR" : game.Turns.Phase.ToString().ToUpper();
            GUI.Label(new Rect(area.x, area.y + 1, area.width, phoneBoard ? 15 : compact ? 24 : 34), turn, phoneBoard ? small : title);
            GUI.Label(new Rect(area.x, area.y + (phoneBoard ? 15 : compact ? 24 : 34), area.width, phoneBoard ? 12 : compact ? 20 : 26), game.RoomNumber == 5 ? "BOSS ROOM - " + game.Theme.Name : game.Theme.Name, phoneBoard ? small : center);

            foreach (var p in floors)
            {
                var r = TileRect(p, ox, oy, minX, maxY);
                bool alternate = (p.x + p.y) % 2 == 0;
                Color c = VisualReadability.TileColor(game.Theme, TileVisualState.Floor, alternate);
                if (game.HazardTiles.Contains(p)) c = VisualReadability.TileColor(game.Theme, game.ArmedHazards.Contains(p) ? TileVisualState.ArmedHazard : TileVisualState.Hazard, alternate);
                if (game.Enemies.Any(e => e.Alive && e.Preview.Contains(p))) c = VisualReadability.TileColor(game.Theme, TileVisualState.EnemyPreview, alternate);
                if (game.PreviewArea.Contains(p)) c = VisualReadability.TileColor(game.Theme, TileVisualState.AreaPreview, alternate);
                if (game.ValidTargets.Contains(p) && game.Turns.Phase == TurnPhase.Player) c = VisualReadability.TileColor(game.Theme, game.SelectedSkill.HasValue ? TileVisualState.SkillTarget : TileVisualState.MoveTarget, alternate);
                if (game.HitTiles.Contains(p)) c = VisualReadability.TileColor(game.Theme, TileVisualState.Hit, alternate);
                DrawRect(r, c);
                DrawOutline(r, new Color(0f, 0f, 0f, .45f), Mathf.Max(1, tile * .035f));
                if (!game.HazardTiles.Contains(p) && !game.ValidTargets.Contains(p))
                    DrawTileGlyph(r, VisualReadability.FloorGlyph(game.Theme.Id, alternate), new Color(1f, 1f, 1f, .18f), .22f);
                if (game.PreviewArea.Contains(p)) DrawOutline(r, new Color(1f, .84f, .32f), Mathf.Max(2, tile * .045f));
                if (game.ValidTargets.Contains(p) && game.Turns.Phase == TurnPhase.Player) DrawOutline(r, game.SelectedSkill.HasValue ? new Color(1f, .94f, .28f) : new Color(.36f, 1f, 1f), Mathf.Max(2, tile * .05f));
                if (game.LastTappedTile == p) DrawOutline(r, Color.white, 3);
                if (game.RejectedTile == p){ DrawOutline(r, VisualReadability.TileColor(game.Theme, TileVisualState.Invalid), 4); DrawTileGlyph(r, "X", Color.white, .46f); }
                if (Event.current.type == EventType.MouseDown && r.Contains(Event.current.mousePosition)){game.TapTile(p); Event.current.Use();}
            }

            foreach (var p in game.BlockerTiles)
            {
                var r = TileRect(p, ox, oy, minX, maxY);
                DrawRect(r, new Color(.025f, .022f, .035f));
                DrawOutline(r, new Color(.38f, .32f, .48f), Mathf.Max(2, tile * .045f));
                DrawTileGlyph(r, "■", new Color(.75f, .68f, .92f), .38f);
            }

            string hazardGlyph = VisualReadability.HazardGlyph(game.Theme.Hazard);
            foreach (var p in game.HazardTiles)
            {
                var r = TileRect(p, ox, oy, minX, maxY);
                DrawOutline(r, game.ArmedHazards.Contains(p) ? Color.white : game.Theme.Accent, Mathf.Max(2, tile * .045f));
                DrawTileGlyph(r, hazardGlyph, Color.white, .36f);
            }

            foreach (var p in game.PropTiles)
                DrawTileGlyph(TileRect(p, ox, oy, minX, maxY), game.Theme.PropGlyph, game.Theme.Accent, .34f);

            if (game.HealingPickup.HasValue)
            {
                var r = TileRect(game.HealingPickup.Value, ox, oy, minX, maxY);
                DrawRect(new Rect(r.x + tile * .08f, r.y + tile * .08f, tile * .78f, tile * .78f), new Color(.16f, 1f, .44f, .92f));
                DrawOutline(r, new Color(.82f, 1f, .76f), Mathf.Max(4, tile * .075f));
                DrawTileGlyph(r, "♥", Color.white, .50f);
                GUI.Label(new Rect(r.x - tile * .15f, r.yMax - tile * .30f, tile * 1.25f, tile * .34f), "HEAL +3", new GUIStyle(center){fontSize=Mathf.Max(10,Mathf.RoundToInt(tile*.18f)),fontStyle=FontStyle.Bold,normal={textColor=Color.white}});
            }

            foreach (var p in game.Enemies.Where(e => e.Alive).SelectMany(e => e.DelayedPreview).Distinct())
            {
                var r = TileRect(p, ox, oy, minX, maxY);
                var threat = game.ThreatKindAt(p);
                DrawOutline(r, new Color(.72f, .30f, .90f, .82f), Mathf.Max(2, tile * .035f));
                DrawTileGlyph(r, ThreatReadability.TileMarker(threat), ThreatReadability.TileMarkerColor(threat), .22f);
            }

            foreach (var p in game.Enemies.Where(e => e.Alive).SelectMany(e => e.Preview).Distinct())
            {
                var r = TileRect(p, ox, oy, minX, maxY);
                bool bossAttack = game.Enemies.Any(e => e.Alive && e.Kind == EnemyKind.LanternWarden && e.Preview.Contains(p));
                bool heavyBoss = game.Enemies.Any(e => e.Alive && e.Kind == EnemyKind.LanternWarden && EnemyAI.BossPhase(e) >= 3 && e.Preview.Contains(p));
                DrawOutline(r, bossAttack ? new Color(1f, .54f, .12f) : new Color(1f, .20f, .18f), Mathf.Max(2, tile * (bossAttack ? .075f : .05f)));
                DrawTileGlyph(r, heavyBoss ? "!!" : "!", Color.white, heavyBoss ? .34f : .42f);
            }

            DrawToken(game.Player.Position, ox, oy, minX, maxY, VisualReadability.ClassAccent(game.Player.ClassId), VisualReadability.ClassGlyph(game.Player.ClassId), "", game.Player, true);
            foreach (var e in game.Enemies.Where(e => e.Alive))
            {
                DrawToken(e.Position, ox, oy, minX, maxY, VisualReadability.EnemyColor(e.Kind), VisualReadability.EnemyGlyph(e.Kind), e.Shield > 0 ? $"{e.Health}+{e.Shield}" : $"{e.Health}", e, false, e.Kind == EnemyKind.LanternWarden);
                var er = TileRect(e.Position, ox, oy, minX, maxY);
                var badge = new Rect(er.xMax - tile * .42f, er.y - tile * .05f, tile * .48f, tile * .25f);
                DrawRect(badge, new Color(.04f, .03f, .06f, .88f));
                DrawOutline(badge, ThreatReadability.TileMarkerColor(e.Threat), 1);
                GUI.Label(badge, ThreatReadability.EnemyBadge(e), new GUIStyle(center){fontSize=Mathf.Max(8,Mathf.RoundToInt(tile*.12f)),fontStyle=FontStyle.Bold,normal={textColor=Color.white}});
            }
        }

        Rect TileRect(Vector2Int p, float ox, float oy, int minX, int maxY) => new(ox + (p.x - minX) * tile, oy + (maxY - p.y) * tile, tile - 2, tile - 2);

        void DrawToken(Vector2Int p, float ox, float oy, int minX, int maxY, Color c, string glyph, string hp = "", UnitModel unit = null, bool player = false, bool boss = false)
        {
            float inset = boss ? .04f : .12f;
            float size = boss ? .88f : .72f;
            var r = new Rect(ox + (p.x - minX) * tile + tile * inset, oy + (maxY - p.y) * tile + tile * .08f, tile * size, tile * size);
            DrawRect(new Rect(r.x - 3, r.y - 3, r.width + 6, r.height + 6), new Color(0f, 0f, 0f, .70f));
            DrawRect(r, c);
            DrawOutline(r, player ? Color.white : (boss ? new Color(1f, .75f, .2f) : new Color(.08f, .04f, .05f)), Mathf.Max(2, tile * .045f));
            GUI.Label(r, glyph, new GUIStyle(center){fontSize = Mathf.RoundToInt(tile * (boss ? .48f : .42f)), fontStyle = FontStyle.Bold, normal = {textColor = Color.black}});
            if (hp != "") GUI.Label(new Rect(r.x, r.y + r.height - 18, r.width, 20), hp, new GUIStyle(center){fontSize = 14, fontStyle = FontStyle.Bold, normal = {textColor = Color.white}});
            if (unit != null)
            {
                string statuses = VisualReadability.StatusGlyph(unit);
                if (!string.IsNullOrEmpty(statuses))
                    GUI.Label(new Rect(r.x - 4, r.yMax - 2, r.width + 8, 18), statuses, new GUIStyle(center){fontSize = Mathf.Max(12, Mathf.RoundToInt(tile * .20f)), fontStyle = FontStyle.Bold, normal = {textColor = Color.white}});
            }
        }

        void DrawPanel(Rect r, bool compact)
        {
            DrawPanelFrame(r);
            var hud = CombatHudLayout.Compute(r, false, compact);
            DrawCombatHeader(hud.Header, compact);
            DrawStatChips(hud.StatChips);
            DrawHazardNote(hud.HazardNote);
            if (GUI.Button(hud.HelpButton, HudText.HelpButton, hudButton)) game.ShowHelp();
            if (GUI.Button(hud.InfoButton, HudText.InfoButton, hudButton)) game.ShowPlaytestInfo();

            float y = hud.SelectedSkill.y;
            float x = hud.SelectedSkill.x, w = hud.SelectedSkill.width;
            if (DrawEndOrReward(x, w, ref y, compact)) return;

            DrawSelectedSkillInfo(hud.SelectedSkill);
            DrawSkills(hud.SkillCards, compact || hud.SkillCards[0].width < 140f);

            bool hasSkill = game.SelectedSkill.HasValue;
            if (hasSkill && GUI.Button(hud.CancelButton, compact ? "CANCEL" : HudText.CancelSkillButton, hudButton)) game.CancelSkill();
            GUI.enabled = game.Turns.Phase == TurnPhase.Player;
            if (GUI.Button(hasSkill ? hud.EndTurnButton : new Rect(hud.CancelButton.x, hud.EndTurnButton.y, hud.EndTurnButton.xMax - hud.CancelButton.x, hud.EndTurnButton.height), HudText.EndTurnButton, hudButton)) game.WaitTurn();
            GUI.enabled = true;
            DrawMessageBox(hud.Message);
        }

        void DrawPortraitPanel(Rect r)
        {
            DrawPanelFrame(r);
            var hud = CombatHudLayout.Compute(r, true, false);
            DrawCombatHeader(hud.Header, true);
            DrawStatChips(hud.StatChips);
            DrawHazardNote(hud.HazardNote);
            if (GUI.Button(hud.HelpButton, HudText.HelpButton, hudButton)){game.ShowHelp(); return;}
            if (GUI.Button(hud.InfoButton, HudText.InfoButton, hudButton)){game.ShowPlaytestInfo(); return;}

            float y = hud.SelectedSkill.y;
            float x = hud.SelectedSkill.x, w = hud.SelectedSkill.width;
            if (DrawEndOrReward(x, w, ref y, false)) return;
            DrawSelectedSkillInfo(hud.SelectedSkill);
            DrawSkills(hud.SkillCards, true);
            bool hasSkill = game.SelectedSkill.HasValue;
            if (hasSkill && GUI.Button(hud.CancelButton, HudText.CancelSkillButton, hudButton)) game.CancelSkill();
            GUI.enabled = game.Turns.Phase == TurnPhase.Player;
            if (GUI.Button(hasSkill ? hud.EndTurnButton : new Rect(hud.CancelButton.x, hud.EndTurnButton.y, hud.EndTurnButton.xMax - hud.CancelButton.x, hud.EndTurnButton.height), HudText.EndTurnButton, hudButton)) game.WaitTurn();
            GUI.enabled = true;
            DrawMessageBox(hud.Message);
        }

        bool DrawEndOrReward(float x, float w, ref float y, bool compact)
        {
            if (game.Turns.Phase == TurnPhase.Reward)
            {
                float headerHeight = compact ? RewardPanelLayout.SideHeaderHeight : RewardPanelLayout.PortraitHeaderHeight;
                GUI.Label(new Rect(x, y, w, headerHeight), "ROOM CLEAR\nCHOOSE ONE", hudHeader);
                y += headerHeight + RewardPanelLayout.Gap;
                if (compact) DrawSideRewards(x, w, ref y);
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

        void DrawCombatHeader(Rect r, bool compact)
        {
            var cls = ClassCatalog.Get(game.Player.ClassId);
            string label = $"{HudText.TurnLabel(game.Turns.Phase)} - ROOM {game.RoomNumber}/5\n{cls.name} - {game.Theme.Name}";
            GUI.Label(r, label, hudHeader);
        }

        void DrawStatChips(Rect[] chips)
        {
            string[] values =
            {
                HudText.Hp(game.Player.Health, game.Player.MaxHealth),
                HudText.Ap(game.Player.ActionPoints, game.Player.MaxActionPoints),
                HudText.Mp(game.Player.MovementPoints, game.Player.MoveRange)
            };
            for (int i = 0; i < chips.Length; i++)
            {
                DrawRect(chips[i], new Color(.08f, .075f, .13f));
                DrawOutline(chips[i], i == 1 ? new Color(.92f, .68f, .22f) : i == 2 ? new Color(.35f, .88f, .95f) : new Color(.82f, .25f, .25f), 2);
                GUI.Label(chips[i], values[i], hudChip);
            }
        }

        void DrawHazardNote(Rect r)
        {
            DrawRect(r, new Color(.055f, .047f, .075f));
            DrawOutline(r, new Color(.26f, .22f, .34f), 1);
            bool phone = IsPhoneViewport();
            string focus = game.HasFocusTile ? game.FocusThreatSummary : "";
            string text = phone ? $"{HudText.TurnLabel(game.Turns.Phase)} - {Shorten(string.IsNullOrWhiteSpace(focus) ? game.Message : focus, 44)}" : $"{game.Theme.HazardName}: {Shorten(game.Theme.HazardRule, 58)}";
            GUI.Label(new Rect(r.x + 6, r.y + 2, r.width - 12, r.height - 4), text, hudMessage);
        }

        void DrawMessageBox(Rect r)
        {
            DrawRect(r, new Color(.03f, .03f, .055f));
            DrawOutline(r, new Color(.24f, .22f, .34f), 1);
            string detail = game.FocusThreatSummary;
            bool phone = IsPhoneViewport();
            string extra = string.IsNullOrWhiteSpace(detail) ? "RED now  PURPLE next  GOLD skill" : Shorten(detail, phone ? 50 : 74);
            GUI.Label(new Rect(r.x + 6, r.y + 2, r.width - 12, r.height - 4), phone ? extra : $"{Shorten(game.Message, 68)}\n{extra}", hudMessage);
        }

        void DrawSelectedSkillInfo(Rect r)
        {
            string label = "Selected skill: none";
            if (game.SelectedSkill.HasValue)
            {
                var s = SkillBook.Get(game.SelectedSkill.Value);
                label = $"Selected: {s.Name} - tap a GOLD target";
            }
            GUI.Label(r, label, hudTiny);
        }

        void DrawSkills(Rect[] cards, bool compact)
        {
            var skills = SkillBook.ForClass(game.Player.ClassId);
            for (int i = 0; i < skills.Length && i < cards.Length; i++)
            {
                var s = skills[i];
                int cd = game.Player.Cooldowns[s.Name];
                bool selected = game.SelectedSkill == s.Id;
                bool usable = game.Turns.Phase == TurnPhase.Player && cd == 0 && game.Player.ActionPoints >= s.ApCost;
                var accent = selected ? VisualReadability.ClassAccent(game.Player.ClassId) : usable ? new Color(.24f, .22f, .34f) : new Color(.12f, .11f, .16f);
                DrawCardFrame(cards[i], accent);
                GUI.enabled = usable;
                bool phone = IsPhoneViewport();
                bool hover = !phone && cards[i].Contains(Event.current.mousePosition);
                string label = phone ? HudText.MobileSkillCard(s, cd, game.Player.ActionPoints, game.Turns.Phase, selected) : HudText.SkillCard(s, cd, game.Player.ActionPoints, game.Turns.Phase, selected, compact && !(selected || hover), selected || hover);
                if (!phone && compact && cards[i].width < 145f)
                    label = $"{(selected ? "SEL " : "")}{ShortSkill(s)}\nAP {s.ApCost} - {HudText.SkillState(s, cd, game.Player.ActionPoints, game.Turns.Phase)}";
                if (GUI.Button(cards[i], label, cards[i].width < 145f ? hudSkillCompact : hudSkill)) game.SelectSkill(s.Id);
                GUI.enabled = true;
            }
        }

        string ShortSkill(SkillDefinition s)
        {
            var parts = s.Name.Split(' ');
            return parts.Length == 1 ? s.Name.ToUpper() : parts[0].ToUpper();
        }

        bool IsPhoneViewport() => Mathf.Min(Screen.width, Screen.height) < 620 && Mathf.Max(Screen.width, Screen.height) <= 1200;

        string Shorten(string text, int max)
        {
            if (string.IsNullOrEmpty(text) || text.Length <= max) return text;
            return text.Substring(0, max - 1) + "…";
        }

        void DrawPortraitRewards(float x, float w, ref float y)
        {
            float gap = RewardPanelLayout.Gap, bw = (w - gap * 2) / 3f;
            for (int i = 0; i < 3; i++)
            {
                var r = new Rect(x + i * (bw + gap), y, bw, RewardPanelLayout.PortraitCardHeight);
                DrawCardFrame(r, new Color(1f, .62f, .18f));
                if (GUI.Button(r, RewardCatalog.Get(i).CompactLabel, hudSkillCompact)) game.ChooseReward(i);
            }
        }

        void DrawSideRewards(float x, float w, ref float y)
        {
            for (int i = 0; i < 3; i++)
            {
                var r = new Rect(x, y, w, RewardPanelLayout.SideCardHeight);
                DrawCardFrame(r, new Color(1f, .62f, .18f));
                if (GUI.Button(r, RewardCatalog.WebGLCardLabel(i), hudSkill)) game.ChooseReward(i);
                y += RewardPanelLayout.SideCardHeight + RewardPanelLayout.Gap;
            }
        }

        void DrawPanelFrame(Rect r)
        {
            DrawRect(r, new Color(.045f, .04f, .075f));
            DrawOutline(r, new Color(.36f, .27f, .12f), 2);
            DrawRect(new Rect(r.x, r.y, r.width, 4), new Color(.72f, .47f, .14f));
        }

        void DrawCardFrame(Rect r, Color accent)
        {
            DrawRect(new Rect(r.x - 2, r.y - 2, r.width + 4, r.height + 4), new Color(0f, 0f, 0f, .35f));
            DrawOutline(new Rect(r.x - 2, r.y - 2, r.width + 4, r.height + 4), accent, 2);
        }

        void DrawTileGlyph(Rect r, string glyph, Color color, float scale)
        {
            GUI.Label(r, glyph, new GUIStyle(center){fontSize = Mathf.Max(12, Mathf.RoundToInt(tile * scale)), fontStyle = FontStyle.Bold, normal = {textColor = color}});
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


