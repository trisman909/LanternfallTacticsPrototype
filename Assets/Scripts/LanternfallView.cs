using System.Linq;
using UnityEngine;

namespace Lanternfall
{
    public sealed class LanternfallView : MonoBehaviour
    {
        public const string PrototypeVersion = "Prototype v0.6E";
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
            var readable = MobileHudReadability.Compute(Screen.width, Screen.height);
            int s = readable.BaseFont;
            title = new GUIStyle(GUI.skin.label){fontSize = s + 5, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter, wordWrap = true, normal = {textColor = new Color(1f, .78f, .28f)}};
            body = new GUIStyle(GUI.skin.label){fontSize = s, alignment = TextAnchor.MiddleLeft, wordWrap = true, normal = {textColor = Color.white}};
            center = new GUIStyle(body){alignment = TextAnchor.MiddleCenter};
            small = new GUIStyle(center){fontSize = Mathf.Max(14, s - 3)};
            button = new GUIStyle(GUI.skin.button)
            {
                fontSize = s,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                wordWrap = true,
                normal = {textColor = Color.white, background = Tex(new Color(.12f, .12f, .2f))},
                hover = {background = Tex(new Color(.23f, .2f, .32f))},
                active = {background = Tex(new Color(.55f, .32f, .08f))}
            };
            hudHeader = new GUIStyle(center){fontSize = readable.HeaderFont, fontStyle = FontStyle.Bold, wordWrap = true, normal = {textColor = new Color(1f, .80f, .32f)}};
            hudChip = new GUIStyle(center){fontSize = readable.StatFont, fontStyle = FontStyle.Bold, wordWrap = false, normal = {textColor = Color.white}};
            hudMessage = new GUIStyle(center){fontSize = readable.MessageFont, fontStyle = FontStyle.Normal, wordWrap = true, normal = {textColor = Color.white}};
            hudTiny = new GUIStyle(center){fontSize = readable.MessageFont, fontStyle = FontStyle.Bold, wordWrap = true, normal = {textColor = new Color(.88f, .90f, 1f)}};
            hudButton = new GUIStyle(button){fontSize = readable.ButtonFont, fontStyle = FontStyle.Bold, alignment=TextAnchor.MiddleCenter, wordWrap = true, normal={textColor=Color.white,background=Tex(Color.clear)},hover={background=Tex(new Color(1f,1f,1f,.08f))},active={background=Tex(new Color(1f,.62f,.18f,.16f))}};
            hudSkill = new GUIStyle(button){fontSize = readable.SkillFont, fontStyle = FontStyle.Bold, wordWrap = true, alignment = TextAnchor.MiddleCenter, normal={textColor=Color.white,background=Tex(Color.clear)},hover={background=Tex(new Color(1f,1f,1f,.08f))},active={background=Tex(new Color(1f,.62f,.18f,.16f))}};
            hudSkillCompact = new GUIStyle(button){fontSize = readable.CompactSkillFont, fontStyle = FontStyle.Bold, wordWrap = true, alignment = TextAnchor.MiddleCenter, normal={textColor=Color.white,background=Tex(Color.clear)},hover={background=Tex(new Color(1f,1f,1f,.08f))},active={background=Tex(new Color(1f,.62f,.18f,.16f))}};
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
            var startTitle = compact ? new GUIStyle(title){fontSize=Mathf.Clamp(Mathf.RoundToInt(area.height/12f),26,34)} : title;
            var startBody = compact ? new GUIStyle(center){fontSize=Mathf.Clamp(Mathf.RoundToInt(area.height/20f),17,22)} : center;
            var startSmall = compact ? new GUIStyle(small){fontSize=Mathf.Clamp(Mathf.RoundToInt(area.height/23f),15,19)} : small;
            var startButton = compact ? new GUIStyle(button){fontSize=Mathf.Clamp(Mathf.RoundToInt(area.height/17f),20,25),alignment=TextAnchor.MiddleCenter} : button;
            float pad = Mathf.Max(18, area.width * .06f);
            float w = area.width - pad * 2;
            float y = compact ? 14 : Mathf.Max(26, area.height * .10f);
            DrawRect(new Rect(pad * .55f, y - 10, area.width - pad * 1.1f, area.height - y * 1.35f), new Color(.055f, .045f, .085f));
            DrawOutline(new Rect(pad * .55f, y - 10, area.width - pad * 1.1f, area.height - y * 1.35f), new Color(.55f, .38f, .12f), 2);

            GUI.Label(new Rect(pad, y, w, compact ? 40 : 72), "LANTERNFALL TACTICS", startTitle); y += compact ? 34 : 58;
            GUI.Label(new Rect(pad, y, w, compact ? 22 : 28), PrototypeVersion, startSmall); y += compact ? 24 : 34;
            GUI.Label(new Rect(pad, y, w, compact ? 54 : 118),
                compact ? "Turn tactics: spend AP/MP, avoid red previews, survive five rooms."
                        : "A short turn-based roguelite prototype.\nFirst time? Open How to Play, then move on cyan tiles, spend AP on skills, avoid red previews, and survive five rooms.",
                startBody); y += compact ? 58 : 128;
            var cls = ClassCatalog.Get(game.SelectedClass);
            GUI.Label(new Rect(pad, y, w, compact ? 42 : 72), compact ? $"{cls.name} / {cls.title}" : $"{cls.name} / {cls.title}\n{cls.description}", startBody); y += compact ? 48 : 82;
            if (game.BestRoomReached > 0 && !compact){GUI.Label(new Rect(pad, y, w, 34), $"Best run: room {game.BestRoomReached}/5", center); y += 42;}
            float gap = compact ? 8 : 12;
            float h = compact ? 46 : 62;
            float bw = compact ? (w - gap * 2) / 3f : w;
            if (compact)
            {
                bw = (w - gap * 3) / 4f;
                if (GUI.Button(new Rect(pad, y, bw, h), "CLASS", startButton)) game.CycleClass();
                if (GUI.Button(new Rect(pad + bw + gap, y, bw, h), "START", startButton)) game.StartRun();
                if (GUI.Button(new Rect(pad + (bw + gap) * 2, y, bw, h), "HELP", startButton)) game.ShowHelp();
                if (GUI.Button(new Rect(pad + (bw + gap) * 3, y, bw, h), "INFO", startButton)) game.ShowPlaytestInfo();
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
            GUI.Label(new Rect(area.x, area.y + (phoneBoard ? 1 : 10), area.width, phoneBoard ? 15 : compact ? 24 : 30), turn, phoneBoard ? small : title);
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
                if (game.RejectedTile == p){ DrawOutline(r, VisualReadability.TileColor(game.Theme, TileVisualState.Invalid), 4); DrawIcon(new Rect(r.x + tile * .28f, r.y + tile * .28f, tile * .40f, tile * .40f), VisualIcon.InvalidTarget); }
                if (Event.current.type == EventType.MouseDown && r.Contains(Event.current.mousePosition)){game.TapTile(p); Event.current.Use();}
            }

            foreach (var p in game.BlockerTiles)
            {
                var r = TileRect(p, ox, oy, minX, maxY);
                DrawRect(r, new Color(.025f, .022f, .035f));
                DrawOutline(r, new Color(.38f, .32f, .48f), Mathf.Max(2, tile * .045f));
                DrawIcon(new Rect(r.x + tile * .25f, r.y + tile * .25f, tile * .46f, tile * .46f), VisualIcon.Obstacle);
            }

            foreach (var p in game.HazardTiles)
            {
                var r = TileRect(p, ox, oy, minX, maxY);
                DrawOutline(r, game.ArmedHazards.Contains(p) ? Color.white : game.Theme.Accent, Mathf.Max(2, tile * .045f));
                DrawIcon(new Rect(r.x + tile * .23f, r.y + tile * .23f, tile * .50f, tile * .50f), IconLanguage.ForHazard(game.Theme.Hazard));
            }

            foreach (var p in game.PropTiles)
                DrawTileGlyph(TileRect(p, ox, oy, minX, maxY), game.Theme.PropGlyph, game.Theme.Accent, .34f);

            if (game.HealingPickup.HasValue)
            {
                var r = TileRect(game.HealingPickup.Value, ox, oy, minX, maxY);
                DrawRect(new Rect(r.x + tile * .08f, r.y + tile * .08f, tile * .78f, tile * .78f), new Color(.16f, 1f, .44f, .92f));
                DrawOutline(r, new Color(.82f, 1f, .76f), Mathf.Max(4, tile * .075f));
                DrawIcon(new Rect(r.x + tile * .20f, r.y + tile * .20f, tile * .56f, tile * .56f), VisualIcon.HealingPickup);
            }

            foreach (var p in game.Enemies.Where(e => e.Alive).SelectMany(e => e.DelayedPreview).Distinct())
            {
                var r = TileRect(p, ox, oy, minX, maxY);
                var threat = game.ThreatKindAt(p);
                DrawOutline(r, new Color(.72f, .30f, .90f, .82f), Mathf.Max(2, tile * .035f));
                DrawIcon(new Rect(r.x + tile * .35f, r.y + tile * .35f, tile * .26f, tile * .26f), threat == ThreatKind.HP ? VisualIcon.DelayedDanger : IconLanguage.ForThreat(threat));
            }

            foreach (var p in game.Enemies.Where(e => e.Alive).SelectMany(e => e.Preview).Distinct())
            {
                var r = TileRect(p, ox, oy, minX, maxY);
                bool bossAttack = game.Enemies.Any(e => e.Alive && e.Kind == EnemyKind.LanternWarden && e.Preview.Contains(p));
                bool heavyBoss = game.Enemies.Any(e => e.Alive && e.Kind == EnemyKind.LanternWarden && EnemyAI.BossPhase(e) >= 3 && e.Preview.Contains(p));
                DrawOutline(r, bossAttack ? new Color(1f, .54f, .12f) : new Color(1f, .20f, .18f), Mathf.Max(2, tile * (bossAttack ? .075f : .05f)));
                DrawIcon(new Rect(r.x + tile * .30f, r.y + tile * .26f, tile * .40f, tile * .48f), bossAttack ? VisualIcon.BossDanger : VisualIcon.ImmediateDanger);
            }

            DrawToken(game.Player.Position, ox, oy, minX, maxY, VisualReadability.ClassAccent(game.Player.ClassId), VisualReadability.ClassGlyph(game.Player.ClassId), "", game.Player, true, false, game.Player.ClassId, null, game.HitTiles.Contains(game.Player.Position));
            foreach (var e in game.Enemies.Where(e => e.Alive))
            {
                DrawToken(e.Position, ox, oy, minX, maxY, VisualReadability.EnemyColor(e.Kind), VisualReadability.EnemyGlyph(e.Kind), e.Shield > 0 ? $"{e.Health}+{e.Shield}" : $"{e.Health}", e, false, e.Kind == EnemyKind.LanternWarden, null, e.Kind, game.HitTiles.Contains(e.Position));
                var er = TileRect(e.Position, ox, oy, minX, maxY);
                var badge = new Rect(er.xMax - tile * .32f, er.y + tile * .02f, tile * .28f, tile * .28f);
                DrawRect(badge, new Color(.04f, .03f, .06f, .88f));
                DrawOutline(badge, ThreatReadability.TileMarkerColor(e.Threat), 1);
                DrawIcon(new Rect(badge.x + 2, badge.y + 2, badge.width - 4, badge.height - 4), IconLanguage.ForThreat(e.Threat));
            }
        }

        Rect TileRect(Vector2Int p, float ox, float oy, int minX, int maxY) => new(ox + (p.x - minX) * tile, oy + (maxY - p.y) * tile, tile - 2, tile - 2);

        void DrawToken(Vector2Int p, float ox, float oy, int minX, int maxY, Color c, string glyph, string hp = "", UnitModel unit = null, bool player = false, bool boss = false, PlayerClassId? playerClass = null, EnemyKind? enemyKind = null, bool hit = false)
        {
            float inset = boss ? .00f : .05f;
            float size = boss ? .96f : .86f;
            var r = new Rect(ox + (p.x - minX) * tile + tile * inset, oy + (maxY - p.y) * tile + tile * .08f, tile * size, tile * size);
            DrawRect(new Rect(r.x - 3, r.y - 3, r.width + 6, r.height + 6), new Color(0f, 0f, 0f, .70f));
            DrawRect(r, new Color(c.r * .18f, c.g * .18f, c.b * .18f, .92f));
            Color outline = player ? Color.white : boss ? new Color(1f, .75f, .2f) : new Color(.08f, .04f, .05f);
            DrawOutline(r, hit ? new Color(1f, .92f, .28f) : outline, Mathf.Max(2, tile * (hit ? .075f : .045f)));
            var spriteRect = new Rect(r.x - r.width * (boss ? .08f : .02f), r.y - r.height * (boss ? .10f : .04f), r.width * (boss ? 1.16f : 1.04f), r.height * (boss ? 1.16f : 1.04f));
            if (!AuthoredUnits.Draw(spriteRect, playerClass, enemyKind, AuthoredUnits.Tint(hit)))
                DrawUnitSymbol(r, player ? game.Player.ClassId : (PlayerClassId?)null, glyph, boss);
            if (boss && unit is EnemyModel bossUnit && AuthoredUnits.IsOverchargedBoss(bossUnit))
            {
                DrawOutline(new Rect(r.x - 5, r.y - 5, r.width + 10, r.height + 10), new Color(1f, .22f, .78f), Mathf.Max(2, tile * .035f));
                DrawIcon(new Rect(r.x + r.width * .34f, r.y - r.height * .16f, r.width * .32f, r.height * .32f), VisualIcon.BossOvercharge);
            }
            if (hp != "") GUI.Label(new Rect(r.x, r.y + r.height - 18, r.width, 20), hp, new GUIStyle(center){fontSize = 14, fontStyle = FontStyle.Bold, normal = {textColor = Color.white}});
            if (unit != null)
            {
                int count = (unit.Shield > 0 ? 1 : 0) + (unit.BurnTurns > 0 ? 1 : 0) + (unit.RootTurns > 0 ? 1 : 0) + (unit.MarkedTurns > 0 ? 1 : 0);
                float iconSize = tile * .19f, x = r.center.x - count * iconSize * .5f;
                if (unit.Shield > 0) { DrawIcon(new Rect(x, r.yMax - iconSize * .55f, iconSize, iconSize), VisualIcon.Shield); x += iconSize; }
                if (unit.BurnTurns > 0) { DrawIcon(new Rect(x, r.yMax - iconSize * .55f, iconSize, iconSize), VisualIcon.Burn); x += iconSize; }
                if (unit.RootTurns > 0) { DrawIcon(new Rect(x, r.yMax - iconSize * .55f, iconSize, iconSize), VisualIcon.Root); x += iconSize; }
                if (unit.MarkedTurns > 0) DrawIcon(new Rect(x, r.yMax - iconSize * .55f, iconSize, iconSize), VisualIcon.Mark);
            }
        }

        void DrawPanel(Rect r, bool compact)
        {
            DrawPanelFrame(r);
            var hud = CombatHudLayout.Compute(r, false, compact);
            DrawCombatHeader(hud.Header, compact);
            DrawStatChips(hud.StatChips);
            DrawHazardNote(hud.HazardNote);
            AuthoredArt.DrawSkin(hud.HelpButton, UiSkin.Utility);
            AuthoredArt.DrawSkin(hud.InfoButton, UiSkin.Utility);
            if (GUI.Button(hud.HelpButton, HudText.HelpButton, hudButton)) game.ShowHelp();
            if (GUI.Button(hud.InfoButton, HudText.InfoButton, hudButton)) game.ShowPlaytestInfo();

            float y = hud.SelectedSkill.y;
            float x = hud.SelectedSkill.x, w = hud.SelectedSkill.width;
            if (DrawEndOrReward(x, w, ref y, compact)) return;

            DrawSelectedSkillInfo(hud.SelectedSkill);
            DrawSkills(hud.SkillCards, compact || hud.SkillCards[0].width < 140f);

            bool hasSkill = game.SelectedSkill.HasValue;
            if (hasSkill) AuthoredArt.DrawSkin(hud.CancelButton, UiSkin.Utility);
            if (hasSkill && GUI.Button(hud.CancelButton, compact ? "CANCEL" : HudText.CancelSkillButton, hudButton)) game.CancelSkill();
            GUI.enabled = game.Turns.Phase == TurnPhase.Player;
            var endTurn = !hasSkill && !IsPhoneViewport() ? new Rect(hud.CancelButton.x, hud.EndTurnButton.y, hud.EndTurnButton.xMax - hud.CancelButton.x, hud.EndTurnButton.height) : hud.EndTurnButton;
            AuthoredArt.DrawSkin(endTurn, UiSkin.EndTurn);
            if (GUI.Button(endTurn, HudText.EndTurnButton, hudButton)) game.WaitTurn();
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
            AuthoredArt.DrawSkin(hud.HelpButton, UiSkin.Utility);
            AuthoredArt.DrawSkin(hud.InfoButton, UiSkin.Utility);
            if (GUI.Button(hud.HelpButton, HudText.HelpButton, hudButton)){game.ShowHelp(); return;}
            if (GUI.Button(hud.InfoButton, HudText.InfoButton, hudButton)){game.ShowPlaytestInfo(); return;}

            float y = hud.SelectedSkill.y;
            float x = hud.SelectedSkill.x, w = hud.SelectedSkill.width;
            if (DrawEndOrReward(x, w, ref y, false)) return;
            DrawSelectedSkillInfo(hud.SelectedSkill);
            DrawSkills(hud.SkillCards, true);
            bool hasSkill = game.SelectedSkill.HasValue;
            if (hasSkill) AuthoredArt.DrawSkin(hud.CancelButton, UiSkin.Utility);
            if (hasSkill && GUI.Button(hud.CancelButton, HudText.CancelSkillButton, hudButton)) game.CancelSkill();
            GUI.enabled = game.Turns.Phase == TurnPhase.Player;
            var endTurn = !hasSkill && !IsPhoneViewport() ? new Rect(hud.CancelButton.x, hud.EndTurnButton.y, hud.EndTurnButton.xMax - hud.CancelButton.x, hud.EndTurnButton.height) : hud.EndTurnButton;
            AuthoredArt.DrawSkin(endTurn, UiSkin.EndTurn);
            if (GUI.Button(endTurn, HudText.EndTurnButton, hudButton)) game.WaitTurn();
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
                var outcome = new Rect(x, y, w, compact ? 80 : 100);
                AuthoredArt.DrawSkin(outcome, game.Turns.Phase == TurnPhase.Won ? UiSkin.VictoryPanel : UiSkin.DefeatPanel);
                GUI.Label(outcome, game.Turns.Phase == TurnPhase.Won ? "VICTORY\nThe Warden falls." : "DEFEAT\nThe dark closes in.", title);
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
                $"{game.Player.Health}/{game.Player.MaxHealth}",
                $"{game.Player.ActionPoints}/{game.Player.MaxActionPoints}",
                $"{game.Player.MovementPoints}/{game.Player.MoveRange}"
            };
            for (int i = 0; i < chips.Length; i++)
            {
                if (!AuthoredArt.DrawSkin(chips[i], UiSkin.StatChip)) DrawRect(chips[i], new Color(.08f, .075f, .13f));
                DrawOutline(chips[i], i == 1 ? new Color(.92f, .68f, .22f) : i == 2 ? new Color(.35f, .88f, .95f) : new Color(.82f, .25f, .25f), 2);
                float iconSize = Mathf.Min(chips[i].height * .42f, chips[i].width * .18f);
                DrawIcon(new Rect(chips[i].x + 6, chips[i].center.y - iconSize * .5f, iconSize, iconSize), i == 0 ? VisualIcon.Health : i == 1 ? VisualIcon.ActionPoint : VisualIcon.MovementPoint);
                GUI.Label(new Rect(chips[i].x + iconSize + 8, chips[i].y, chips[i].width - iconSize - 12, chips[i].height), values[i], hudChip);
            }
        }

        void DrawHazardNote(Rect r)
        {
            if (!AuthoredArt.DrawSkin(r, UiSkin.Tooltip)) DrawRect(r, new Color(.055f, .047f, .075f));
            DrawOutline(r, new Color(.26f, .22f, .34f), 1);
            bool phone = IsPhoneViewport();
            string focus = game.HasFocusTile ? game.FocusThreatSummary : "";
            string text = phone ? $"{HudText.TurnLabel(game.Turns.Phase)} - {Shorten(string.IsNullOrWhiteSpace(focus) ? game.Message : focus, 44)}" : $"{game.Theme.HazardName}: {Shorten(game.Theme.HazardRule, 58)}";
            GUI.Label(new Rect(r.x + 6, r.y + 2, r.width - 12, r.height - 4), text, hudMessage);
        }

        void DrawMessageBox(Rect r)
        {
            if (!AuthoredArt.DrawSkin(r, UiSkin.Tooltip)) DrawRect(r, new Color(.03f, .03f, .055f));
            DrawOutline(r, new Color(.24f, .22f, .34f), 1);
            string detail = game.FocusThreatSummary;
            bool phone = IsPhoneViewport();
            string extra = string.IsNullOrWhiteSpace(detail) ? "RED now  PURPLE next  GOLD skill" : Shorten(detail, phone ? 50 : 74);
            GUI.Label(new Rect(r.x + 6, r.y + 2, r.width - 12, r.height - 4), phone ? extra : $"{Shorten(game.Message, 68)}\n{extra}", hudMessage);
        }

        void DrawSelectedSkillInfo(Rect r)
        {
            AuthoredArt.DrawSkin(r, UiSkin.SelectedSkill);
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
                DrawCardFrame(cards[i], accent, UiSkin.SkillCard);
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
                DrawCardFrame(r, new Color(1f, .62f, .18f), UiSkin.RewardCard);
                if (GUI.Button(r, RewardCatalog.Get(i).CompactLabel, hudSkillCompact)) game.ChooseReward(i);
            }
        }

        void DrawSideRewards(float x, float w, ref float y)
        {
            for (int i = 0; i < 3; i++)
            {
                var r = new Rect(x, y, w, RewardPanelLayout.SideCardHeight);
                DrawCardFrame(r, new Color(1f, .62f, .18f), UiSkin.RewardCard);
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

        void DrawCardFrame(Rect r, Color accent, UiSkin skin = UiSkin.SkillCard)
        {
            if (!AuthoredArt.DrawSkin(new Rect(r.x - 2, r.y - 2, r.width + 4, r.height + 4), skin))
                DrawRect(new Rect(r.x - 2, r.y - 2, r.width + 4, r.height + 4), new Color(0f, 0f, 0f, .35f));
            DrawOutline(new Rect(r.x - 2, r.y - 2, r.width + 4, r.height + 4), accent, 2);
        }

        void DrawTileGlyph(Rect r, string glyph, Color color, float scale)
        {
            GUI.Label(r, glyph, new GUIStyle(center){fontSize = Mathf.Max(12, Mathf.RoundToInt(tile * scale)), fontStyle = FontStyle.Bold, normal = {textColor = color}});
        }

        void DrawUnitSymbol(Rect r, PlayerClassId? cls, string enemyGlyph, bool boss)
        {
            Color ink = new Color(.025f, .02f, .035f);
            var icon = new Rect(r.x + r.width * .20f, r.y + r.height * .15f, r.width * .60f, r.height * .60f);
            if (boss)
            {
                DrawOutline(new Rect(r.x + 3, r.y + 3, r.width - 6, r.height - 6), new Color(1f, .42f, .12f), Mathf.Max(2, tile * .025f));
                DrawIcon(icon, VisualIcon.BossDanger, ink);
                return;
            }
            if (cls.HasValue)
            {
                VisualIcon classIcon = cls.Value switch
                {
                    PlayerClassId.Vanguard => VisualIcon.Shield,
                    PlayerClassId.Wayfinder => VisualIcon.Mark,
                    PlayerClassId.Cantor => VisualIcon.Burn,
                    PlayerClassId.Gloamstep => VisualIcon.MovementPoint,
                    _ => VisualIcon.Prism
                };
                DrawIcon(icon, classIcon, ink);
                return;
            }
            VisualIcon enemyIcon = enemyGlyph == "A" ? VisualIcon.Burn : enemyGlyph == "G" ? VisualIcon.Mark : VisualIcon.Shield;
            DrawIcon(icon, enemyIcon, ink);
        }

        void DrawIcon(Rect r, VisualIcon icon)
        {
            if (!AuthoredArt.DrawIcon(r, icon)) DrawIcon(r, icon, IconLanguage.Describe(icon).Color);
        }

        void DrawIcon(Rect r, VisualIcon icon, Color color)
        {
            float u = Mathf.Max(1f, Mathf.Min(r.width, r.height) / 8f);
            Rect P(float x, float y, float w, float h) => new(r.x + r.width * x, r.y + r.height * y, r.width * w, r.height * h);
            void Bar(float x, float y, float w, float h) => DrawRect(P(x, y, w, h), color);
            switch (icon)
            {
                case VisualIcon.Health: case VisualIcon.Heal: case VisualIcon.HealingPickup:
                    Bar(.40f,.10f,.20f,.80f); Bar(.10f,.40f,.80f,.20f); break;
                case VisualIcon.ActionPoint:
                    Bar(.42f,.06f,.16f,.88f); Bar(.17f,.34f,.66f,.16f); Bar(.25f,.66f,.50f,.14f); break;
                case VisualIcon.MovementPoint:
                    Bar(.10f,.42f,.62f,.16f); Bar(.56f,.22f,.16f,.56f); Bar(.68f,.30f,.18f,.16f); Bar(.68f,.54f,.18f,.16f); break;
                case VisualIcon.Shield:
                    DrawOutline(P(.16f,.08f,.68f,.70f),color,u); Bar(.26f,.66f,.48f,.16f); Bar(.40f,.78f,.20f,.12f); break;
                case VisualIcon.Burn: case VisualIcon.EmberVent:
                    Bar(.42f,.08f,.18f,.34f); Bar(.25f,.34f,.50f,.48f); Bar(.39f,.54f,.22f,.36f); break;
                case VisualIcon.Root: case VisualIcon.GraspingRoots:
                    Bar(.43f,.08f,.14f,.84f); Bar(.18f,.32f,.64f,.14f); Bar(.16f,.68f,.30f,.14f); Bar(.54f,.54f,.30f,.14f); break;
                case VisualIcon.Mark:
                    DrawOutline(P(.12f,.12f,.76f,.76f),color,u); Bar(.42f,.02f,.16f,.28f); Bar(.42f,.70f,.16f,.28f); Bar(.02f,.42f,.28f,.16f); Bar(.70f,.42f,.28f,.16f); break;
                case VisualIcon.ActionDrain:
                    Bar(.18f,.18f,.48f,.16f); Bar(.42f,.18f,.16f,.54f); Bar(.18f,.56f,.48f,.16f); Bar(.70f,.42f,.24f,.16f); break;
                case VisualIcon.MovementDrain:
                    Bar(.10f,.42f,.54f,.16f); Bar(.50f,.26f,.16f,.48f); Bar(.70f,.42f,.24f,.16f); break;
                case VisualIcon.ImmediateDanger:
                    Bar(.42f,.10f,.16f,.54f); Bar(.42f,.76f,.16f,.16f); break;
                case VisualIcon.DelayedDanger:
                    DrawOutline(P(.12f,.12f,.76f,.76f),color,u); Bar(.46f,.22f,.12f,.34f); Bar(.46f,.50f,.28f,.12f); break;
                case VisualIcon.BossDanger:
                    DrawOutline(P(.05f,.05f,.90f,.90f),color,u); Bar(.20f,.18f,.14f,.25f); Bar(.66f,.18f,.14f,.25f); Bar(.43f,.28f,.14f,.48f); break;
                case VisualIcon.BlockedLineOfSight:
                    DrawOutline(P(.08f,.24f,.84f,.52f),color,u); Bar(.12f,.42f,.76f,.16f); Bar(.44f,.10f,.12f,.80f); break;
                case VisualIcon.ShallowWater:
                    Bar(.06f,.25f,.28f,.13f); Bar(.36f,.37f,.28f,.13f); Bar(.66f,.25f,.28f,.13f); Bar(.06f,.62f,.28f,.13f); Bar(.36f,.74f,.28f,.13f); Bar(.66f,.62f,.28f,.13f); break;
                case VisualIcon.Prism:
                    Bar(.44f,.05f,.12f,.90f); Bar(.18f,.42f,.64f,.14f); Bar(.26f,.20f,.14f,.60f); Bar(.60f,.20f,.14f,.60f); break;
                case VisualIcon.ChargedFloor:
                    Bar(.50f,.06f,.14f,.34f); Bar(.28f,.34f,.36f,.14f); Bar(.28f,.34f,.14f,.34f); Bar(.28f,.62f,.22f,.14f); Bar(.38f,.62f,.14f,.30f); break;
            }
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


