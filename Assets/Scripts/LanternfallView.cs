using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Lanternfall
{
    public sealed class LanternfallView : MonoBehaviour
    {
        public const string PrototypeVersion = "Prototype v0.6N.2";
        public const string BuildProofLabel = "Phase 6N.2 — HUD QA";
        LanternfallGame game;
        LanternfallAudio audioLayer;
        Camera cam;
        GUIStyle title, body, button, center, small;
        GUIStyle hudHeader, hudChip, hudMessage, hudThreat, hudThreatCategory, hudThreatAction, hudButton, hudSkill, hudSkillCompact, hudTiny;
        float tile = 1f;
        sealed class TokenMotion { public Vector2 From, To; public float Started; }
        sealed class BoardEffect { public Vector2 From, To; public Color Color; public CombatEffectCue Cue; public float Started, Duration; public bool Death; public EnemyKind Enemy; }
        readonly Dictionary<object, TokenMotion> tokenMotions = new();
        readonly Dictionary<EnemyModel, Vector2Int> observedEnemyPositions = new();
        readonly Dictionary<EnemyModel, bool> observedEnemyAlive = new();
        readonly List<BoardEffect> boardEffects = new();
        Vector2Int observedPlayerPosition;
        bool presentationSnapshotReady;
        int observedRoom;
        TurnPhase observedPhase;
        string observedEffectSignature = "";
        string flowBanner = "";
        float flowBannerUntil;
        MobileLayoutMode lastLayoutMode;
        Vector2 lastViewport;

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
            audioLayer = gameObject.AddComponent<LanternfallAudio>();
            cam = new GameObject("Camera").AddComponent<Camera>();
            cam.orthographic = true;
            cam.transform.position = new Vector3(4, 5, -10);
            cam.backgroundColor = new Color(.025f, .02f, .06f);
            cam.orthographicSize = 6.7f;
            game.Changed += OnGameChanged;
        }

        void OnGameChanged()
        {
            if (!game.HasStarted || game.Player == null) return;
            audioLayer.Observe(game.Message,game.RoomNumber==5);
            float now = Time.unscaledTime;
            if (!presentationSnapshotReady || observedRoom != game.RoomNumber)
            {
                tokenMotions.Clear(); boardEffects.Clear(); observedEnemyPositions.Clear(); observedEnemyAlive.Clear();
                observedPlayerPosition = game.Player.Position; observedRoom = game.RoomNumber; observedPhase = game.Turns.Phase;
                flowBanner = game.RoomNumber == 5 ? "THE LANTERN WARDEN" : $"ROOM {game.RoomNumber}";
                flowBannerUntil = now + PresentationMotion.Duration(1.15f, .45f);
                presentationSnapshotReady = true;
            }
            else
            {
                if (observedPlayerPosition != game.Player.Position)
                {
                    tokenMotions[game.Player] = new TokenMotion{From=observedPlayerPosition,To=game.Player.Position,Started=now};
                    boardEffects.Add(new BoardEffect{From=observedPlayerPosition,To=game.Player.Position,Color=new Color(.34f,1f,.92f),Cue=CombatEffectCue.Move,Started=now,Duration=PresentationMotion.Duration(.32f,.08f)});
                }
                foreach (var enemy in game.Enemies)
                {
                    if (observedEnemyPositions.TryGetValue(enemy, out var old) && old != enemy.Position)
                    {
                        tokenMotions[enemy] = new TokenMotion{From=old,To=enemy.Position,Started=now};
                        boardEffects.Add(new BoardEffect{From=old,To=enemy.Position,Color=new Color(.72f,.32f,.26f),Cue=CombatEffectCue.Move,Started=now,Duration=PresentationMotion.Duration(.28f,.08f)});
                    }
                    if (observedEnemyAlive.TryGetValue(enemy, out var alive) && alive && !enemy.Alive)
                        boardEffects.Add(new BoardEffect{From=enemy.Position,To=enemy.Position,Color=VisualReadability.EnemyColor(enemy.Kind),Started=now,Duration=PresentationMotion.Duration(.48f,.12f),Death=true,Enemy=enemy.Kind});
                }
                if (observedPhase != game.Turns.Phase)
                {
                    flowBanner = game.Turns.Phase switch { TurnPhase.Reward => "ROOM CLEAR", TurnPhase.Won => "VICTORY", TurnPhase.Lost => "DEFEAT", _ => "" };
                    if(game.Turns.Phase==TurnPhase.Enemy)audioLayer.Play(AudioCue.EnemyPhase);
                    else if(game.Turns.Phase==TurnPhase.Reward)audioLayer.Play(AudioCue.RoomClear);
                    else if(game.Turns.Phase==TurnPhase.Won)audioLayer.Play(AudioCue.Victory);
                    else if(game.Turns.Phase==TurnPhase.Lost)audioLayer.Play(AudioCue.Defeat);
                    if (flowBanner != "") flowBannerUntil = now + PresentationMotion.Duration(1.0f,.4f);
                }
            }
            string signature = game.Message + ":" + string.Join(";", game.HitTiles.OrderBy(p=>p.x).ThenBy(p=>p.y));
            if (signature != observedEffectSignature && game.HitTiles.Count > 0)
            {
                CombatEffectCue cue=CombatEffectLanguage.ForMessage(game.Message); Color color=CombatEffectLanguage.Color(cue);
                audioLayer.PlayCombat(cue);
                foreach (var hit in game.HitTiles)
                {
                    Vector2 source=game.Player.Position;
                    if(game.Message.Contains("strikes")||game.Message.Contains("triggers")||game.Message.Contains("drain"))
                    {
                        var attacker=game.Enemies.Where(e=>e.Alive).OrderBy(e=>Mathf.Abs(e.Position.x-hit.x)+Mathf.Abs(e.Position.y-hit.y)).FirstOrDefault();
                        if(attacker!=null)source=attacker.Position;
                    }
                    boardEffects.Add(new BoardEffect{From=source,To=hit,Color=color,Cue=cue,Started=now,Duration=PresentationMotion.Duration(.38f,.10f)});
                }
                observedEffectSignature = signature;
            }
            observedPlayerPosition = game.Player.Position;
            observedEnemyPositions.Clear(); observedEnemyAlive.Clear();
            foreach (var enemy in game.Enemies){observedEnemyPositions[enemy]=enemy.Position; observedEnemyAlive[enemy]=enemy.Alive;}
            observedRoom = game.RoomNumber; observedPhase = game.Turns.Phase;
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
            hudThreat = new GUIStyle(hudMessage){wordWrap=false,alignment=TextAnchor.MiddleLeft,clipping=TextClipping.Clip};
            hudThreatCategory=new GUIStyle(hudThreat){fontSize=Mathf.Max(14,readable.MessageFont),fontStyle=FontStyle.Bold};
            hudThreatAction=new GUIStyle(hudThreat){fontSize=Mathf.Max(15,readable.MessageFont)};
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
            if(Event.current.type==EventType.MouseDown&&!audioLayer.Unlocked){audioLayer.Unlock(game.HasStarted&&game.RoomNumber==5);audioLayer.Play(AudioCue.UiTap);}
            var guiSafe = MobileLayout.ToGuiSafeArea(Screen.height, Screen.safeArea);
            GUI.BeginGroup(guiSafe);
            var layout = MobileLayout.Compute(guiSafe.width, guiSafe.height);
            lastLayoutMode=layout.Mode;lastViewport=new Vector2(guiSafe.width,guiSafe.height);
            if (layout.PhoneHud && layout.Portrait)
            {
                DrawRotatePhoneScreen(new Rect(0, 0, guiSafe.width, guiSafe.height));
                GUI.EndGroup();
                return;
            }

            if (!game.HasStarted) DrawStartScreen(new Rect(0, 0, guiSafe.width, guiSafe.height));
            else
            {
                DrawBoard(layout.Board, layout.Portrait || layout.CompactLandscape, layout.PhoneLandscape);
                if(layout.PhoneLandscape)DrawPhoneLandscapeHud(layout);
                else if (layout.Portrait) DrawPortraitPanel(layout.Panel);
                else DrawPanel(layout.Panel, layout.CompactLandscape);
                if (!layout.PhoneHud) GUI.Label(new Rect(8, guiSafe.height - 24, 160, 20), PrototypeVersion, small);
            }

            if (game.HelpVisible) DrawHelpOverlay(new Rect(0, 0, guiSafe.width, guiSafe.height));
            if (game.PlaytestInfoVisible) DrawPlaytestInfoOverlay(new Rect(0, 0, guiSafe.width, guiSafe.height));
            if (game.BossPhasePresentationActive) DrawBossPhaseOverlay(new Rect(0, 0, guiSafe.width, guiSafe.height));
            if (flowBannerUntil > Time.unscaledTime && !game.HelpVisible && !game.PlaytestInfoVisible) DrawFlowBanner(new Rect(0,0,guiSafe.width,guiSafe.height));
            GUI.EndGroup();
        }

        void DrawFlowBanner(Rect area)
        {
            float remaining=Mathf.Clamp01((flowBannerUntil-Time.unscaledTime)/PresentationMotion.Duration(1f,.4f));
            float width=Mathf.Min(area.width*.58f,620f), height=Mathf.Clamp(area.height*.13f,52f,96f);
            var r=new Rect(area.center.x-width*.5f,area.height*.17f,width,height);
            DrawRect(r,new Color(.035f,.025f,.055f,.72f*remaining)); DrawOutline(r,new Color(1f,.62f,.16f,.9f*remaining),3);
            var old=GUI.color; GUI.color=new Color(1f,1f,1f,remaining); GUI.Label(r,flowBanner,title); GUI.color=old;
        }

        void DrawBossPhaseOverlay(Rect area)
        {
            DrawRect(area, new Color(0f, 0f, 0f, .48f));
            float shake = PresentationMotion.Reduced ? 0f : Mathf.Sin(Time.time * 48f) * 5f;
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
            bool phoneHelp=area.height<500f;
            if(phoneHelp)
            {
                float gapColumns=16f,columnW=(w-gapColumns)*.5f,lineH=44f;
                for(int i=0;i<LanternfallGame.HowToPlayLines.Length;i++)
                {
                    int column=i%2,row=i/2;
                    GUI.Label(new Rect(pad+column*(columnW+gapColumns),y+row*(lineH+3f),columnW,lineH),"- "+LanternfallGame.HowToPlayLines[i],small);
                }
            }
            else foreach (var line in LanternfallGame.HowToPlayLines)
            {
                const float lineH=54f;GUI.Label(new Rect(pad,y,w,lineH),"- "+line,body);y+=lineH+4;
            }
            float actionY=area.height-(area.height<500f?58:84), actionH=area.height<500f?46:62, gap=10f, motionW=w*.34f;
            float audioY=actionY-actionH-8f,audioGap=6f,audioW=(w-audioGap*3f)/4f;
            var audioButton=new GUIStyle(button){fontSize=phoneHelp?Mathf.Max(14,small.fontSize):Mathf.Max(14,body.fontSize-2)};
            if(GUI.Button(new Rect(pad,audioY,audioW,actionH),$"MASTER {Mathf.RoundToInt(LanternfallAudioSettings.Master*100)}",audioButton)){LanternfallAudioSettings.Master=NextVolume(LanternfallAudioSettings.Master);audioLayer.RefreshVolumes();}
            if(GUI.Button(new Rect(pad+(audioW+audioGap),audioY,audioW,actionH),$"SFX {Mathf.RoundToInt(LanternfallAudioSettings.Sfx*100)}",audioButton)){LanternfallAudioSettings.Sfx=NextVolume(LanternfallAudioSettings.Sfx);audioLayer.RefreshVolumes();audioLayer.Play(AudioCue.UiTap);}
            if(GUI.Button(new Rect(pad+(audioW+audioGap)*2,audioY,audioW,actionH),$"MUSIC {Mathf.RoundToInt(LanternfallAudioSettings.Music*100)}",audioButton)){LanternfallAudioSettings.Music=NextVolume(LanternfallAudioSettings.Music);audioLayer.RefreshVolumes();}
            if(GUI.Button(new Rect(pad+(audioW+audioGap)*3,audioY,audioW,actionH),LanternfallAudioSettings.Muted?"UNMUTE":"MUTE",audioButton)){LanternfallAudioSettings.Muted=!LanternfallAudioSettings.Muted;audioLayer.RefreshVolumes();}
            if(GUI.Button(new Rect(pad,actionY,motionW,actionH),PresentationMotion.Reduced?"MOTION: REDUCED":"MOTION: FULL",button))PresentationMotion.Reduced=!PresentationMotion.Reduced;
            if (GUI.Button(new Rect(pad+motionW+gap,actionY,w-motionW-gap,actionH), game.HasStarted ? "BACK TO RUN" : "GOT IT", button))
                game.HideHelp();
        }

        static float NextVolume(float value)=>value>.75f?.5f:value>.25f?0f:1f;

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
            float diagnosticH=area.height<500f?30f:42f;
            GUI.Label(new Rect(pad,y,w,diagnosticH),$"Build {Application.version} · {lastLayoutMode} · {Mathf.RoundToInt(lastViewport.x)}×{Mathf.RoundToInt(lastViewport.y)}",area.height<500f?small:body);
            if (GUI.Button(new Rect(pad, area.height - (area.height < 500f ? 58 : 84), w, area.height < 500f ? 46 : 62), "BACK", button))
                game.HidePlaytestInfo();
        }

        void DrawBoard(Rect area, bool compact = false, bool phoneBoard = false)
        {
            DrawRect(area, game.Theme.Background);
            DrawOutline(area, new Color(.12f, .10f, .18f), 3);
            float top = compact ? (phoneBoard ? 28 : 42) : 64;
            var floors = game.Grid.Floors().ToList();
            int minX = floors.Min(p => p.x);
            int maxX = floors.Max(p => p.x);
            int minY = floors.Min(p => p.y);
            int maxY = floors.Max(p => p.y);
            int boardCols = Mathf.Max(1, maxX - minX + 1);
            int boardRows = Mathf.Max(1, maxY - minY + 1);
            var fit = BoardFitLayout.Compute(area, boardCols, boardRows, compact, phoneBoard);
            tile = fit.TileSize;
            float ox = fit.Bounds.x;
            float oy = fit.Bounds.y;

            string turn = game.Turns.Phase == TurnPhase.Enemy ? "ENEMY TURN" : game.Turns.Phase == TurnPhase.Reward ? "ROOM CLEAR" : game.Turns.Phase.ToString().ToUpper();
            if(!phoneBoard)
            {
                GUI.Label(new Rect(area.x, area.y + 10, area.width, compact ? 24 : 30), turn, compact ? small : title);
                GUI.Label(new Rect(area.x, area.y + (compact ? 24 : 34), area.width, compact ? 20 : 26), game.RoomNumber == 5 ? "BOSS ROOM - " + game.Theme.Name : game.Theme.Name, compact ? small : center);
            }

            foreach (var p in floors)
            {
                var r = TileRect(p, ox, oy, minX, maxY);
                bool alternate = Mathf.Abs(p.x * 3 + p.y * 5) % 4 == 0;
                TileVisualState state = TileVisualState.Floor;
                if (game.HazardTiles.Contains(p)) state = game.ArmedHazards.Contains(p) ? TileVisualState.ArmedHazard : TileVisualState.Hazard;
                if (game.ArmedHazardDamageTiles.Contains(p)) state = TileVisualState.ArmedHazard;
                if (game.Enemies.Any(e => e.Alive && e.Preview.Contains(p))) state = TileVisualState.EnemyPreview;
                if (game.PreviewArea.Contains(p)) state = TileVisualState.AreaPreview;
                if (game.ValidTargets.Contains(p) && game.Turns.Phase == TurnPhase.Player) state = game.SelectedSkill.HasValue ? TileVisualState.SkillTarget : TileVisualState.MoveTarget;
                if (game.HitTiles.Contains(p)) state = TileVisualState.Hit;
                Color c = VisualReadability.TileColor(game.Theme, state, alternate);
                DrawRect(r, alternate ? game.Theme.Alternate : game.Theme.Floor);
                BiomeArtCell artCell = alternate ? BiomeArtCell.Alternate : BiomeArtCell.Floor;
                if (game.RoomNumber == 5 && !game.HazardTiles.Contains(p) && Mathf.Abs(p.x * 13 + p.y * 7) % 17 == 0) artCell = BiomeArtCell.BossAccent;
                bool authored = AuthoredBiomes.Draw(r, game.Theme.Id, artCell, VisualReadability.EnvironmentTextureTint(game.Theme.Id));
                if(game.HazardTiles.Contains(p))
                {
                    var hazardRect=VisualReadability.EnvironmentalHazardRect(r);
                    AuthoredBiomes.Draw(hazardRect,game.Theme.Id,BiomeArtCell.Hazard,VisualReadability.EnvironmentalHazardTint(game.Theme));
                }
                if(authored)
                {
                    DrawRect(r,VisualReadability.QuietEnvironmentOverlay(game.Theme,game.HazardTiles.Contains(p)));
                    float diagonal=((p.x-minX)/(float)Mathf.Max(1,maxX-minX)+(maxY-p.y)/(float)Mathf.Max(1,maxY-minY))*.5f;
                    DrawRect(r,VisualReadability.PaintedLightOverlay(game.Theme,diagonal));
                }
                if (state != TileVisualState.Floor)
                {
                    DrawRect(r,new Color(.01f,.012f,.018f,game.HazardTiles.Contains(p) ? (game.SelectedSkill.HasValue?.22f:.12f) : .22f));
                    float overlayAlpha=VisualReadability.TacticalOverlayAlpha(state);
                    if(game.SelectedSkill.HasValue&&state==TileVisualState.EnemyPreview)overlayAlpha*=.38f;
                    DrawRect(r, new Color(c.r, c.g, c.b, overlayAlpha));
                }
                if(game.SelectedSkill.HasValue&&!game.ValidTargets.Contains(p))
                {
                    if(game.SkillRangeTiles.Contains(p))DrawRect(r,new Color(.42f,.28f,.08f,.22f));
                    else if(game.OutOfRangeSkillTiles.Contains(p))DrawRect(r,new Color(.01f,.012f,.02f,.30f));
                }
                DrawOutline(r, new Color(0f, 0f, 0f, .45f), Mathf.Max(1, tile * .035f));
                if(game.SelectedSkill.HasValue&&game.BlockedSkillTiles.Contains(p))DrawOutline(r,new Color(.72f,.66f,.58f,.72f),Mathf.Max(2,tile*.035f));
                if(game.SelectedSkill.HasValue&&game.PotentialImpactTiles.Contains(p)&&!game.ValidTargets.Contains(p))DrawOutline(r,new Color(1f,.48f,.10f,.82f),Mathf.Max(2,tile*.045f));
                if (game.PreviewArea.Contains(p)) DrawOutline(r, new Color(1f, .84f, .32f), Mathf.Max(3, tile * .055f));
                if (game.ValidTargets.Contains(p) && game.Turns.Phase == TurnPhase.Player) DrawOutline(r, game.SelectedSkill.HasValue ? new Color(1f, .94f, .28f) : new Color(.36f, 1f, 1f), Mathf.Max(3, tile * .065f));
                if (game.LastTappedTile == p) DrawOutline(r, Color.white, 3);
                if (game.RejectedTile == p){ DrawOutline(r, VisualReadability.TileColor(game.Theme, TileVisualState.Invalid), 4); DrawIcon(new Rect(r.x + tile * .28f, r.y + tile * .28f, tile * .40f, tile * .40f), VisualIcon.InvalidTarget); }
                if (Event.current.type == EventType.MouseDown && r.Contains(Event.current.mousePosition)){game.TapTile(p); Event.current.Use();}
            }

            foreach (var p in game.BlockerTiles)
            {
                var r = TileRect(p, ox, oy, minX, maxY);
                DrawRect(r, new Color(.025f, .022f, .035f));
                bool authored = AuthoredBiomes.Draw(r, game.Theme.Id, BiomeArtCell.Obstacle, Color.white);
                DrawOutline(r, new Color(.38f, .32f, .48f), Mathf.Max(2, tile * .045f));
                if(game.SelectedSkill.HasValue&&game.BlockedSkillTiles.Contains(p))DrawOutline(r,new Color(.92f,.72f,.32f),Mathf.Max(3,tile*.06f));
                if (!authored) DrawIcon(new Rect(r.x + tile * .25f, r.y + tile * .25f, tile * .46f, tile * .46f), VisualIcon.Obstacle);
            }

            foreach (var p in game.HazardTiles)
            {
                var r = TileRect(p, ox, oy, minX, maxY);
                if(game.ArmedHazards.Contains(p))DrawOutline(r,Color.white,Mathf.Max(2,tile*.045f));
            }

            foreach (var p in game.PropTiles)
            {
                var r = TileRect(p, ox, oy, minX, maxY);
                var propRect=new Rect(r.x+r.width*.09f,r.y+r.height*.09f,r.width*.82f,r.height*.82f);
                DrawRect(new Rect(propRect.x+propRect.width*.12f,propRect.y+propRect.height*.68f,propRect.width*.76f,propRect.height*.16f),new Color(0f,0f,0f,.22f));
                if (!AuthoredBiomes.Draw(propRect, game.Theme.Id, AuthoredBiomes.PropCell(p),VisualReadability.EnvironmentalPropTint(game.Theme)))
                    DrawIcon(propRect,VisualIcon.Obstacle);
            }

            if (game.HealingPickup.HasValue)
            {
                var r = TileRect(game.HealingPickup.Value, ox, oy, minX, maxY);
                bool authored = AuthoredBiomes.Draw(r, game.Theme.Id, BiomeArtCell.HealingPickup, Color.white);
                if (!authored) DrawRect(new Rect(r.x + tile * .08f, r.y + tile * .08f, tile * .78f, tile * .78f), new Color(.16f, 1f, .44f, .92f));
                DrawOutline(r, new Color(.82f, 1f, .76f), Mathf.Max(4, tile * .075f));
                DrawIcon(new Rect(r.x + tile * .34f, r.y + tile * .34f, tile * .30f, tile * .30f), VisualIcon.HealingPickup);
            }

            var delayedTiles=game.Enemies.Where(e=>e.Alive).SelectMany(e=>e.DelayedPreview).ToHashSet();
            DrawMergedThreatBoundary(delayedTiles,ox,oy,minX,maxY,new Color(.62f,.30f,.76f,game.SelectedSkill.HasValue?.16f:.42f),Mathf.Max(2,tile*VisualReadability.FutureThreatBorderScale(IsPhoneViewport())));

            var immediateTiles=game.Enemies.Where(e=>e.Alive).SelectMany(e=>e.Preview).ToHashSet();
            bool bossAttack=game.Enemies.Any(e=>e.Alive&&e.Kind==EnemyKind.LanternWarden&&e.Preview.Count>0);
            float immediateAlpha=game.SelectedSkill.HasValue?.34f:.92f;
            DrawMergedThreatBoundary(immediateTiles,ox,oy,minX,maxY,bossAttack?new Color(1f,.54f,.12f,immediateAlpha):new Color(1f,.20f,.18f,immediateAlpha),Mathf.Max(3,tile*(bossAttack?.09f:.065f)));

            DrawToken(AnimatedPosition(game.Player, game.Player.Position), ox, oy, minX, maxY, VisualReadability.ClassAccent(game.Player.ClassId), "", game.Player, true, false, game.Player.ClassId, null, game.HitTiles.Contains(game.Player.Position));
            if(game.PlayerBiomeEffectActive)
            {
                var playerTile=TileRect(game.Player.Position,ox,oy,minX,maxY);
                DrawOutline(playerTile,game.Theme.WarningColor,Mathf.Max(4,tile*.08f));
                DrawIcon(new Rect(playerTile.x+tile*.04f,playerTile.y+tile*.04f,tile*.24f,tile*.24f),IconLanguage.ForHazard(game.Theme.Hazard));
            }
            foreach (var e in game.Enemies.Where(e => e.Alive))
            {
                DrawToken(AnimatedPosition(e, e.Position), ox, oy, minX, maxY, VisualReadability.EnemyColor(e.Kind), e.Shield > 0 ? $"{e.Health}+{e.Shield}" : $"{e.Health}", e, false, e.Kind == EnemyKind.LanternWarden, null, e.Kind, game.HitTiles.Contains(e.Position));
                var er = TileRect(e.Position, ox, oy, minX, maxY);
                var badge = new Rect(er.xMax - tile * .32f, er.y + tile * .02f, tile * .28f, tile * .28f);
                DrawRect(badge, new Color(.04f, .03f, .06f, .88f));
                DrawOutline(badge, ThreatReadability.TileMarkerColor(e.Threat), 1);
                DrawIcon(new Rect(badge.x + 2, badge.y + 2, badge.width - 4, badge.height - 4), IconLanguage.ForThreat(e.Threat));
            }
            DrawBoardEffects(ox,oy,minX,maxY);
        }

        void DrawMergedThreatBoundary(HashSet<Vector2Int> tiles,float ox,float oy,int minX,int maxY,Color color,float width)
        {
            foreach(var p in tiles)
            {
                var r=TileRect(p,ox,oy,minX,maxY);
                if(VisualReadability.IsExposedThreatEdge(tiles,p,Vector2Int.up,game.Grid))DrawRect(new Rect(r.x,r.y,r.width,width),color);
                if(VisualReadability.IsExposedThreatEdge(tiles,p,Vector2Int.down,game.Grid))DrawRect(new Rect(r.x,r.yMax-width,r.width,width),color);
                if(VisualReadability.IsExposedThreatEdge(tiles,p,Vector2Int.left,game.Grid))DrawRect(new Rect(r.x,r.y,width,r.height),color);
                if(VisualReadability.IsExposedThreatEdge(tiles,p,Vector2Int.right,game.Grid))DrawRect(new Rect(r.xMax-width,r.y,width,r.height),color);
            }
        }

        Vector2 AnimatedPosition(object unit, Vector2Int destination)
        {
            if(!tokenMotions.TryGetValue(unit,out var motion))return destination;
            float t=(Time.unscaledTime-motion.Started)/PresentationMotion.Duration(.24f,.06f);
            if(t>=1f){tokenMotions.Remove(unit);return destination;}
            return Vector2.Lerp(motion.From,motion.To,PresentationMotion.Ease(t));
        }

        void DrawBoardEffects(float ox,float oy,int minX,int maxY)
        {
            float now=Time.unscaledTime;
            for(int i=boardEffects.Count-1;i>=0;i--)
            {
                var fx=boardEffects[i]; float t=(now-fx.Started)/fx.Duration;
                if(t>=1f){boardEffects.RemoveAt(i);continue;}
                float fade=1f-t;
                Vector2 Center(Vector2 p)=>new(ox+(p.x-minX+.5f)*tile,oy+(maxY-p.y+.5f)*tile);
                var a=Center(fx.From); var b=Center(fx.To);
                if(!fx.Death)
                {
                    int segments=fx.Cue==CombatEffectCue.Spear||fx.Cue==CombatEffectCue.Heavy?8:fx.Cue==CombatEffectCue.Slash?3:5;
                    for(int j=1;j<=segments;j++)
                    {
                        Vector2 p=Vector2.Lerp(a,b,j/(segments+1f)); float s=Mathf.Max(2f,tile*(.045f+.025f*fade));
                        DrawRect(new Rect(p.x-s*.5f,p.y-s*.5f,s,s),new Color(fx.Color.r,fx.Color.g,fx.Color.b,fade*.75f));
                    }
                    float pulse=tile*((fx.Cue==CombatEffectCue.Heavy?.46f:.32f)+t*(fx.Cue==CombatEffectCue.Fire||fx.Cue==CombatEffectCue.Prism?.55f:.42f)); var ring=new Rect(b.x-pulse*.5f,b.y-pulse*.5f,pulse,pulse);
                    DrawOutline(ring,new Color(fx.Color.r,fx.Color.g,fx.Color.b,fade),Mathf.Max(2f,tile*.035f));
                    if(fx.Cue==CombatEffectCue.Fire||fx.Cue==CombatEffectCue.Prism||fx.Cue==CombatEffectCue.Heavy)
                    {float inner=pulse*.62f;DrawOutline(new Rect(b.x-inner*.5f,b.y-inner*.5f,inner,inner),new Color(1f,1f,1f,fade*.7f),Mathf.Max(1f,tile*.018f));}
                    if(!PresentationMotion.Reduced)
                        for(int d=0;d<4;d++){float ang=d*Mathf.PI*.5f+t*2f;Vector2 p=b+new Vector2(Mathf.Cos(ang),Mathf.Sin(ang))*tile*(.12f+t*.28f);DrawRect(new Rect(p.x-2,p.y-2,4,4),new Color(fx.Color.r,fx.Color.g,fx.Color.b,fade));}
                }
                else
                {
                    float size=tile*(.86f+.28f*t); var r=new Rect(b.x-size*.5f,b.y-size*.5f,size,size); var old=GUI.color;
                    GUI.color=new Color(1f,1f,1f,fade); AuthoredUnits.Draw(r,null,fx.Enemy,new Color(fx.Color.r,fx.Color.g,fx.Color.b,fade)); GUI.color=old;
                    DrawOutline(r,new Color(fx.Color.r,fx.Color.g,fx.Color.b,fade),Mathf.Max(2f,tile*.04f));
                }
            }
        }

        Rect TileRect(Vector2Int p, float ox, float oy, int minX, int maxY) => new(ox + (p.x - minX) * tile, oy + (maxY - p.y) * tile, tile - 2, tile - 2);

        void DrawToken(Vector2 p, float ox, float oy, int minX, int maxY, Color c, string hp = "", UnitModel unit = null, bool player = false, bool boss = false, PlayerClassId? playerClass = null, EnemyKind? enemyKind = null, bool hit = false)
        {
            float size=VisualReadability.UnitTokenScale(player,boss),inset=(1f-size)*.5f;
            var r = new Rect(ox + (p.x - minX) * tile + tile * inset, oy + (maxY - p.y) * tile + tile * inset, tile * size, tile * size);
            DrawRect(new Rect(r.x - 3, r.y - 3, r.width + 6, r.height + 6), new Color(0f, 0f, 0f, .70f));
            float idle=player ? .30f : boss ? .23f : .18f;
            DrawRect(r, new Color(c.r * idle, c.g * idle, c.b * idle, .96f));
            Color outline = player ? Color.white : boss ? new Color(1f, .75f, .2f) : new Color(.08f, .04f, .05f);
            DrawOutline(r, hit ? new Color(1f, .92f, .28f) : outline, Mathf.Max(player ? 3 : 2, tile * (hit ? .085f : player ? .065f : boss ? .075f : .05f)));
            if(player)DrawOutline(new Rect(r.x-3,r.y-3,r.width+6,r.height+6),new Color(.28f,1f,.92f,.86f),Mathf.Max(2,tile*.025f));
            if(boss)DrawOutline(new Rect(r.x-4,r.y-4,r.width+8,r.height+8),new Color(1f,.42f,.10f,.9f),Mathf.Max(2,tile*.035f));
            var spriteRect = new Rect(r.x+r.width*.01f,r.y+r.height*.01f,r.width*.98f,r.height*.98f);
            if (!AuthoredUnits.Draw(spriteRect, playerClass, enemyKind, AuthoredUnits.Tint(hit)))
                DrawUnitSymbol(r, playerClass, enemyKind, boss);
            if (boss && unit is EnemyModel bossUnit && AuthoredUnits.IsOverchargedBoss(bossUnit))
            {
                DrawOutline(new Rect(r.x - 5, r.y - 5, r.width + 10, r.height + 10), new Color(1f, .22f, .78f), Mathf.Max(2, tile * .035f));
                DrawIcon(new Rect(r.x + r.width * .34f, r.y - r.height * .16f, r.width * .32f, r.height * .32f), VisualIcon.BossOvercharge);
            }
            if (hp != "") GUI.Label(new Rect(r.x, r.y + r.height - 18, r.width, 20), hp, new GUIStyle(center){fontSize = 14, fontStyle = FontStyle.Bold, normal = {textColor = Color.white}});
            if (unit != null)
            {
                int count = (unit.Shield > 0 ? 1 : 0) + (unit.BurnTurns > 0 ? 1 : 0) + (unit.RootTurns > 0 ? 1 : 0) + (unit.MarkedTurns > 0 ? 1 : 0);
                float iconSize = tile * VisualReadability.StatusIconScale(IsPhoneViewport()), x = r.center.x - count * iconSize * .5f;
                if (unit.Shield > 0) { DrawStatusBadge(new Rect(x, r.yMax - iconSize * .62f, iconSize, iconSize), VisualIcon.Shield, unit.Shield); x += iconSize; }
                if (unit.BurnTurns > 0) { DrawStatusBadge(new Rect(x, r.yMax - iconSize * .62f, iconSize, iconSize), VisualIcon.Burn, unit.BurnTurns); x += iconSize; }
                if (unit.RootTurns > 0) { DrawStatusBadge(new Rect(x, r.yMax - iconSize * .62f, iconSize, iconSize), VisualIcon.Root, unit.RootTurns); x += iconSize; }
                if (unit.MarkedTurns > 0) DrawStatusBadge(new Rect(x, r.yMax - iconSize * .62f, iconSize, iconSize), VisualIcon.Mark, unit.MarkedTurns);
                if (!PresentationMotion.Reduced && count > 0)
                {
                    Color aura=unit.BurnTurns>0?new Color(1f,.30f,.05f):unit.RootTurns>0?new Color(.35f,1f,.38f):unit.MarkedTurns>0?new Color(.76f,.26f,1f):new Color(.32f,.78f,1f);
                    float pulse=3f+Mathf.Sin(Time.unscaledTime*6f)*2f;
                    DrawOutline(new Rect(r.x-pulse,r.y-pulse,r.width+pulse*2,r.height+pulse*2),new Color(aura.r,aura.g,aura.b,.58f),2);
                }
            }
        }

        void DrawStatusBadge(Rect r,VisualIcon icon,int amount)
        {
            var color=IconLanguage.Describe(icon).Color;
            DrawRect(r,new Color(.015f,.012f,.025f,.92f));
            DrawOutline(r,color,Mathf.Max(1f,r.width*.07f));
            DrawIcon(new Rect(r.x+r.width*.12f,r.y+r.height*.08f,r.width*.76f,r.height*.76f),icon);
            if(amount>1)GUI.Label(new Rect(r.x+r.width*.48f,r.y+r.height*.48f,r.width*.48f,r.height*.46f),amount.ToString(),new GUIStyle(center){fontSize=Mathf.Max(12,Mathf.RoundToInt(r.width*.38f)),fontStyle=FontStyle.Bold,normal={textColor=Color.white}});
        }

        void DrawPanel(Rect r, bool compact)
        {
            DrawPanelFrame(r);
            var hud = CombatHudLayout.Compute(r, false, compact);
            DrawCombatHeader(hud.Header, compact);
            DrawStatChips(hud.StatChips);
            DrawHazardNote(hud.HazardNote);
            AuthoredArt.DrawSkin(hud.HelpButton, UiSkin.Utility,new Color(.78f,.78f,.78f));
            AuthoredArt.DrawSkin(hud.InfoButton, UiSkin.Utility,new Color(.78f,.78f,.78f));
            if (GUI.Button(hud.HelpButton, HudText.HelpButton, hudButton)) game.ShowHelp();
            if (GUI.Button(hud.InfoButton, HudText.InfoButton, hudButton)) game.ShowPlaytestInfo();

            float y = hud.SelectedSkill.y;
            float x = hud.SelectedSkill.x, w = hud.SelectedSkill.width;
            if (DrawEndOrReward(x, w, ref y, compact)) return;

            DrawSelectedSkillInfo(hud.SelectedSkill);
            DrawSkills(hud.SkillCards, compact || hud.SkillCards[0].width < 140f);

            bool hasSkill = game.SelectedSkill.HasValue;
            if (hasSkill) AuthoredArt.DrawSkin(hud.CancelButton, UiSkin.Utility,new Color(.78f,.78f,.78f));
            if (hasSkill && GUI.Button(hud.CancelButton, compact ? "CANCEL" : HudText.CancelSkillButton, hudButton)) game.CancelSkill();
            GUI.enabled = game.Turns.Phase == TurnPhase.Player;
            var endTurn = !hasSkill && !IsPhoneViewport() ? new Rect(hud.CancelButton.x, hud.EndTurnButton.y, hud.EndTurnButton.xMax - hud.CancelButton.x, hud.EndTurnButton.height) : hud.EndTurnButton;
            AuthoredArt.DrawSkin(endTurn, UiSkin.EndTurn);
            if (GUI.Button(endTurn, HudText.EndTurnButton, hudButton)){audioLayer.Play(AudioCue.EndTurn);game.WaitTurn();}
            GUI.enabled = true;
            DrawMessageBox(hud.Message);
        }

        void DrawPhoneLandscapeHud(MobileLayoutSnapshot layout)
        {
            if(game.Turns.Phase==TurnPhase.Reward||game.Turns.Phase==TurnPhase.Won||game.Turns.Phase==TurnPhase.Lost)
            {
                var full=new Rect(0,0,layout.ThreatPanel.xMax,layout.SkillBar.yMax);DrawRect(full,new Color(0f,0f,0f,.72f));
                var overlay=new Rect(full.width*.12f,full.height*.08f,full.width*.76f,full.height*.84f);DrawPanel(overlay,false);return;
            }
            DrawPanelFrame(layout.TopBar);DrawPanelFrame(layout.SkillBar);DrawPanelFrame(layout.ThreatPanel);
            float statsW=layout.TopBar.width*.52f;
            DrawStatChips(layout.StatChips);
            var cls=ClassCatalog.Get(game.Player.ClassId);
            GUI.Label(new Rect(layout.TopBar.x+statsW+8f,layout.TopBar.y+3f,layout.TopBar.width-statsW-14f,layout.TopBar.height-6f),$"{HudText.TurnLabel(game.Turns.Phase)}\n{game.Theme.Name}",hudHeader);
            DrawPhoneSkills(layout.SkillButtons,layout.SkillContentRects);
            bool hasSkill=game.SelectedSkill.HasValue;
            if(hasSkill){AuthoredArt.DrawSkin(layout.CancelButton,UiSkin.Utility,new Color(.78f,.78f,.78f));if(GUI.Button(layout.CancelButton,"X",hudButton))game.CancelSkill();}
            var action=hasSkill?layout.ActionButton:new Rect(layout.ThreatPanel.x+8f,layout.ActionButton.y,layout.ThreatPanel.width-16f,layout.ActionButton.height);
            var actionArt=MobileLayout.AspectFit(MobileLayout.Inset(action,6f,5f),3.35f);var actionLabel=MobileLayout.Inset(actionArt,4f,4f);
            GUI.enabled=game.Turns.Phase==TurnPhase.Player;AuthoredArt.DrawSkin(actionArt,UiSkin.EndTurn);if(GUI.Button(action,string.Empty,hudButton)){audioLayer.Play(AudioCue.EndTurn);game.WaitTurn();}GUI.Label(actionLabel,HudText.EndTurnButton,hudSkillCompact);GUI.enabled=true;
            DrawPhoneThreatRail(layout.ThreatPanel,cls.name);
        }

        void DrawPhoneThreatRail(Rect r,string className)
        {
            float pad=8f,utilityH=48f,headerH=34f;
            GUI.Label(new Rect(r.x+pad,r.y+3f,r.width-pad*2,headerH),"THREATS",hudHeader);
            var rows=game.MobileThreatRows(4);float contentTop=r.y+headerH+4f,contentBottom=r.yMax-utilityH-6f,rowH=rows.Length==0?0f:(contentBottom-contentTop)/rows.Length;
            for(int i=0;i<rows.Length;i++)
            {
                var row=new Rect(r.x+pad,contentTop+i*rowH,r.width-pad*2,rowH-3f);if(i>0)DrawRect(new Rect(row.x,row.y,row.width,1f),new Color(.25f,.22f,.32f,.7f));
                Color color=ThreatReadability.TileMarkerColor(rows[i].Kind);var category=new GUIStyle(hudThreatCategory){normal={textColor=color}};
                GUI.Label(new Rect(row.x+2f,row.y+1f,row.width-4f,Mathf.Min(28f,row.height*.42f)),rows[i].Category,category);
                string action=Shorten(rows[i].Action,Mathf.Max(12,Mathf.FloorToInt((row.width-4f)/Mathf.Max(1f,hudThreatAction.fontSize*.5f))));
                GUI.Label(new Rect(row.x+2f,row.y+Mathf.Min(25f,row.height*.40f),row.width-4f,row.height-Mathf.Min(25f,row.height*.40f)),action,hudThreatAction);
            }
            float by=r.yMax-utilityH;Rect help=new Rect(r.x+pad,by+2f,(r.width-pad*3)/2f,utilityH-6f);Rect info=new Rect(help.xMax+pad,by+2f,help.width,utilityH-6f);
            AuthoredArt.DrawSkin(help,UiSkin.Utility,new Color(.78f,.78f,.78f));AuthoredArt.DrawSkin(info,UiSkin.Utility,new Color(.78f,.78f,.78f));
            if(GUI.Button(help,"?",hudButton))game.ShowHelp();if(GUI.Button(info,"i",hudButton))game.ShowPlaytestInfo();
        }

        void DrawPortraitPanel(Rect r)
        {
            DrawPanelFrame(r);
            var hud = CombatHudLayout.Compute(r, true, false);
            DrawCombatHeader(hud.Header, true);
            DrawStatChips(hud.StatChips);
            DrawHazardNote(hud.HazardNote);
            AuthoredArt.DrawSkin(hud.HelpButton, UiSkin.Utility,new Color(.78f,.78f,.78f));
            AuthoredArt.DrawSkin(hud.InfoButton, UiSkin.Utility,new Color(.78f,.78f,.78f));
            if (GUI.Button(hud.HelpButton, HudText.HelpButton, hudButton)){game.ShowHelp(); return;}
            if (GUI.Button(hud.InfoButton, HudText.InfoButton, hudButton)){game.ShowPlaytestInfo(); return;}

            float y = hud.SelectedSkill.y;
            float x = hud.SelectedSkill.x, w = hud.SelectedSkill.width;
            if (DrawEndOrReward(x, w, ref y, false)) return;
            DrawSelectedSkillInfo(hud.SelectedSkill);
            DrawSkills(hud.SkillCards, true);
            bool hasSkill = game.SelectedSkill.HasValue;
            if (hasSkill) AuthoredArt.DrawSkin(hud.CancelButton, UiSkin.Utility,new Color(.78f,.78f,.78f));
            if (hasSkill && GUI.Button(hud.CancelButton, HudText.CancelSkillButton, hudButton)) game.CancelSkill();
            GUI.enabled = game.Turns.Phase == TurnPhase.Player;
            var endTurn = !hasSkill && !IsPhoneViewport() ? new Rect(hud.CancelButton.x, hud.EndTurnButton.y, hud.EndTurnButton.xMax - hud.CancelButton.x, hud.EndTurnButton.height) : hud.EndTurnButton;
            AuthoredArt.DrawSkin(endTurn, UiSkin.EndTurn);
            if (GUI.Button(endTurn, HudText.EndTurnButton, hudButton)){audioLayer.Play(AudioCue.EndTurn);game.WaitTurn();}
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
                if (!AuthoredArt.DrawSkin(chips[i], UiSkin.StatChip,new Color(.84f,.84f,.84f))) DrawRect(chips[i], new Color(.08f, .075f, .13f));
                DrawOutline(chips[i], i == 1 ? new Color(.92f, .68f, .22f) : i == 2 ? new Color(.35f, .88f, .95f) : new Color(.82f, .25f, .25f), 2);
                float iconSize = Mathf.Min(chips[i].height * .42f, chips[i].width * .18f);
                DrawIcon(new Rect(chips[i].x + 6, chips[i].center.y - iconSize * .5f, iconSize, iconSize), i == 0 ? VisualIcon.Health : i == 1 ? VisualIcon.ActionPoint : VisualIcon.MovementPoint);
                GUI.Label(new Rect(chips[i].x + iconSize + 8, chips[i].y, chips[i].width - iconSize - 12, chips[i].height), values[i], hudChip);
            }
        }

        void DrawHazardNote(Rect r)
        {
            if (!AuthoredArt.DrawSkin(r, UiSkin.Tooltip,new Color(.76f,.76f,.76f))) DrawRect(r, new Color(.055f, .047f, .075f));
            DrawOutline(r, new Color(.26f, .22f, .34f), 1);
            bool phone = IsPhoneViewport();
            string text = phone ? game.MobileThreatSummary(Mathf.Max(18,Mathf.FloorToInt((r.width-12)/Mathf.Max(1f,hudThreat.fontSize*.52f)))) : $"{game.Theme.HazardName}: {Shorten(game.Theme.HazardRule, 58)}";
            GUI.Label(new Rect(r.x + 6, r.y + 2, r.width - 12, r.height - 4), text, phone?hudThreat:hudMessage);
        }

        void DrawMessageBox(Rect r)
        {
            if (!AuthoredArt.DrawSkin(r, UiSkin.Tooltip,new Color(.74f,.74f,.74f))) DrawRect(r, new Color(.03f, .03f, .055f));
            DrawOutline(r, new Color(.24f, .22f, .34f), 1);
            string detail = game.FocusThreatSummary;
            bool phone = IsPhoneViewport();
            string extra = string.IsNullOrWhiteSpace(detail) ? "Current: RED   Next: PURPLE   Then: GOLD" : Shorten(detail, phone ? 50 : 74);
            GUI.Label(new Rect(r.x + 6, r.y + 2, r.width - 12, r.height - 4), phone ? extra : $"{Shorten(game.Message, 68)}\n{extra}", hudMessage);
        }

        void DrawSelectedSkillInfo(Rect r)
        {
            bool invalid=game.SelectedSkill.HasValue&&!game.LastInputAccepted&&game.Message.StartsWith("INVALID:");
            if(invalid){DrawRect(r,new Color(.11f,.025f,.035f,.96f));DrawOutline(r,new Color(1f,.24f,.18f),2);}
            else AuthoredArt.DrawSkin(r, UiSkin.SelectedSkill,new Color(.82f,.82f,.82f));
            string label = "Selected skill: none";
            if (game.SelectedSkill.HasValue)
            {
                var s = SkillBook.Get(game.SelectedSkill.Value);
                label = invalid ? game.Message.Replace("INVALID: ","").Split('.')[0] : $"Selected: {s.Name}\nGold: in range  •  Dark: out of range";
            }
            GUI.Label(new Rect(r.x+5,r.y+2,r.width-10,r.height-4), label, hudTiny);
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
                if (GUI.Button(cards[i], label, cards[i].width < 145f ? hudSkillCompact : hudSkill)){audioLayer.Play(AudioCue.SelectSkill);game.SelectSkill(s.Id);}
                GUI.enabled = true;
            }
        }

        void DrawPhoneSkills(Rect[] cards,Rect[] content)
        {
            var skills=SkillBook.ForClass(game.Player.ClassId);
            for(int i=0;i<skills.Length&&i<cards.Length;i++)
            {
                var s=skills[i];int cd=game.Player.Cooldowns[s.Name];bool selected=game.SelectedSkill==s.Id;
                bool usable=game.Turns.Phase==TurnPhase.Player&&cd==0&&game.Player.ActionPoints>=s.ApCost;
                DrawCardFrame(cards[i],selected?VisualReadability.ClassAccent(game.Player.ClassId):usable?new Color(.24f,.22f,.34f):new Color(.12f,.11f,.16f),UiSkin.SkillCard);
                GUI.enabled=usable;if(GUI.Button(cards[i],string.Empty,hudButton)){audioLayer.Play(AudioCue.SelectSkill);game.SelectSkill(s.Id);}
                GUI.Label(content[i],HudText.MobileSkillCard(s,cd,game.Player.ActionPoints,game.Turns.Phase,selected),hudSkillCompact);GUI.enabled=true;
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
                if (GUI.Button(r, RewardCatalog.Get(i).CompactLabel, hudSkillCompact)){audioLayer.Play(AudioCue.Reward);game.ChooseReward(i);}
            }
        }

        void DrawSideRewards(float x, float w, ref float y)
        {
            for (int i = 0; i < 3; i++)
            {
                var r = new Rect(x, y, w, RewardPanelLayout.SideCardHeight);
                DrawCardFrame(r, new Color(1f, .62f, .18f), UiSkin.RewardCard);
                if (GUI.Button(r, RewardCatalog.WebGLCardLabel(i), hudSkill)){audioLayer.Play(AudioCue.Reward);game.ChooseReward(i);}
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
            if (!AuthoredArt.DrawSkin(new Rect(r.x - 2, r.y - 2, r.width + 4, r.height + 4), skin,new Color(.80f,.80f,.80f)))
                DrawRect(new Rect(r.x - 2, r.y - 2, r.width + 4, r.height + 4), new Color(0f, 0f, 0f, .35f));
            DrawOutline(new Rect(r.x - 2, r.y - 2, r.width + 4, r.height + 4), accent, 2);
        }

        void DrawUnitSymbol(Rect r, PlayerClassId? cls, EnemyKind? enemyKind, bool boss)
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
            VisualIcon enemyIcon = enemyKind == EnemyKind.Ashling ? VisualIcon.Burn : enemyKind == EnemyKind.GloomArcher ? VisualIcon.Mark : VisualIcon.Shield;
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


