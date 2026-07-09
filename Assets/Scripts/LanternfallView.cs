using System.Linq;
using UnityEngine;

namespace Lanternfall
{
    public sealed class LanternfallView : MonoBehaviour
    {
        LanternfallGame game; Camera cam; GUIStyle title,body,button,center; float tile=1f;
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void Boot(){if(FindAnyObjectByType<LanternfallGame>()==null)new GameObject("Lanternfall Tactics").AddComponent<LanternfallView>();}
        void Awake()
        {
            Application.targetFrameRate=30;QualitySettings.vSyncCount=0;QualitySettings.antiAliasing=0;QualitySettings.shadows=ShadowQuality.Disable;QualitySettings.shadowDistance=0;QualitySettings.softParticles=false;QualitySettings.realtimeReflectionProbes=false;QualitySettings.anisotropicFiltering=AnisotropicFiltering.Disable;Input.multiTouchEnabled=false;
            game=gameObject.AddComponent<LanternfallGame>();cam=new GameObject("Camera").AddComponent<Camera>();cam.orthographic=true;cam.transform.position=new Vector3(4,5,-10);cam.backgroundColor=new Color(.025f,.02f,.06f);cam.orthographicSize=6.7f;
            game.Changed+=()=>{};game.StartRun();
        }
        void InitStyles()
        {
            int s=Mathf.Clamp(Mathf.Min(Screen.width,Screen.height)/28,18,30);title=new GUIStyle(GUI.skin.label){fontSize=s+5,fontStyle=FontStyle.Bold,alignment=TextAnchor.MiddleCenter,normal={textColor=new Color(1,.78f,.28f)}};
            body=new GUIStyle(GUI.skin.label){fontSize=s,alignment=TextAnchor.MiddleLeft,normal={textColor=Color.white}};center=new GUIStyle(body){alignment=TextAnchor.MiddleCenter};
            button=new GUIStyle(GUI.skin.button){fontSize=s,fontStyle=FontStyle.Bold,wordWrap=true,normal={textColor=Color.white,background=Tex(new Color(.12f,.12f,.2f))},hover={background=Tex(new Color(.23f,.2f,.32f))},active={background=Tex(new Color(.5f,.3f,.1f))}};
        }
        Texture2D Tex(Color c){var t=new Texture2D(1,1);t.SetPixel(0,0,c);t.Apply();return t;}
        void OnGUI()
        {
            if(title==null)InitStyles();
            var guiSafe=MobileLayout.ToGuiSafeArea(Screen.height,Screen.safeArea);GUI.BeginGroup(guiSafe);
            var layout=MobileLayout.Compute(guiSafe.width,guiSafe.height);
            DrawBoard(layout.Board,layout.CompactLandscape);
            if(layout.Portrait)DrawPortraitPanel(layout.Panel);else DrawPanel(layout.Panel,layout.CompactLandscape);
            GUI.EndGroup();
        }
        void DrawBoard(Rect area,bool compact=false)
        {
            DrawRect(area,game.Theme.Background);float top=compact?54:72;tile=Mathf.Min((area.width-24)/game.Grid.Width,(area.height-top-16)/game.Grid.Height);float ox=(area.width-game.Grid.Width*tile)/2;float oy=area.y+top+(area.height-top-game.Grid.Height*tile)/2;
            GUI.Label(new Rect(area.x,area.y+2,area.width,compact?28:38),game.Turns.Phase==TurnPhase.Enemy?"ENEMY TURN":"PLAYER TURN",title);
            GUI.Label(new Rect(area.x,area.y+(compact?28:38),area.width,compact?22:28),game.Theme.Name,center);
            foreach(var p in game.Grid.Floors())
            {
                var r=new Rect(ox+p.x*tile,oy+(game.Grid.Height-1-p.y)*tile,tile-2,tile-2);Color c=(p.x+p.y)%2==0?game.Theme.Floor:game.Theme.Alternate;
                if(game.HazardTiles.Contains(p))c=game.ArmedHazards.Contains(p)?game.Theme.WarningColor:game.Theme.HazardColor;
                if(game.Enemies.Any(e=>e.Alive&&e.Preview.Contains(p)))c=new Color(.62f,.12f,.13f);
                if(game.ValidTargets.Contains(p)&&game.Turns.Phase==TurnPhase.Player)c=game.SelectedSkill.HasValue?new Color(.65f,.48f,.08f):new Color(.08f,.5f,.55f);
                DrawRect(r,c);if(Event.current.type==EventType.MouseDown&&r.Contains(Event.current.mousePosition)){game.TapTile(p);Event.current.Use();}
            }
            string hazardGlyph=game.Theme.Hazard switch{HazardKind.ShallowWater=>"~",HazardKind.Prism=>"◇",HazardKind.EmberVent=>"!",HazardKind.GraspingRoots=>"⌁",_=>"⚡"};
            foreach(var p in game.HazardTiles){var r=new Rect(ox+p.x*tile,oy+(game.Grid.Height-1-p.y)*tile,tile-2,tile-2);GUI.Label(r,hazardGlyph,new GUIStyle(center){fontSize=Mathf.Max(14,Mathf.RoundToInt(tile*.32f)),fontStyle=FontStyle.Bold,normal={textColor=Color.white}});}
            foreach(var p in game.PropTiles)
            {
                var r=new Rect(ox+p.x*tile,oy+(game.Grid.Height-1-p.y)*tile,tile-2,tile-2);
                GUI.Label(r,game.Theme.PropGlyph,new GUIStyle(center){fontSize=Mathf.RoundToInt(tile*.42f),fontStyle=FontStyle.Bold,normal={textColor=game.Theme.Accent}});
            }
            DrawToken(game.Player.Position,ox,oy,new Color(.2f,.9f,1f),"✦");
            foreach(var e in game.Enemies.Where(e=>e.Alive))DrawToken(e.Position,ox,oy,e.Kind==EnemyKind.LanternWarden?new Color(.9f,.25f,.8f):new Color(.85f,.33f,.2f),e.Kind switch{EnemyKind.Ashling=>"A",EnemyKind.GloomArcher=>"G",EnemyKind.StoneSentinel=>"S",_=>"W"},$"{e.Health}");
        }
        void DrawToken(Vector2Int p,float ox,float oy,Color c,string glyph,string hp="")
        {
            var r=new Rect(ox+p.x*tile+tile*.12f,oy+(game.Grid.Height-1-p.y)*tile+tile*.08f,tile*.72f,tile*.72f);DrawRect(r,c);var st=new GUIStyle(center){fontSize=Mathf.RoundToInt(tile*.42f),fontStyle=FontStyle.Bold,normal={textColor=Color.black}};GUI.Label(r,glyph,st);if(hp!="")GUI.Label(new Rect(r.x,r.y+r.height-18,r.width,20),hp,new GUIStyle(center){fontSize=14,fontStyle=FontStyle.Bold,normal={textColor=Color.white}});
        }
        void DrawPanel(Rect r,bool compact)
        {
            DrawRect(r,new Color(.045f,.04f,.075f));float x=r.x+10,w=r.width-20,y=compact?2:12;GUI.Label(new Rect(x,y,w,compact?30:42),"LANTERNFALL",title);y+=compact?30:48;
            GUI.Label(new Rect(x,y,w,compact?58:110),compact?$"R{game.RoomNumber}/5 • HP {game.Player.Health}/{game.Player.MaxHealth} • {game.Turns.Phase.ToString().ToUpper()}\n{game.Message}":$"{game.Theme.Name}\nRoom {game.RoomNumber}/5  •  HP {game.Player.Health}/{game.Player.MaxHealth}  •  Foes {game.LivingEnemies}\n{game.Message}",compact?center:body);y+=compact?62:116;
            if(game.Turns.Phase==TurnPhase.Reward){GUI.Label(new Rect(x,y,w,compact?32:45),"CHOOSE A BLESSING",title);y+=compact?34:50;if(compact)DrawCompactRewards(x,w,ref y);else{RewardButton(0,"VITAL EMBER\n+3 max HP",ref y,x,w);RewardButton(1,"BRIGHT WICK\n+1 skill damage",ref y,x,w);RewardButton(2,"SWIFT FLAME\n+1 move range",ref y,x,w);}return;}
            if(game.Turns.Phase==TurnPhase.Won||game.Turns.Phase==TurnPhase.Lost){GUI.Label(new Rect(x,y,w,100),game.Turns.Phase==TurnPhase.Won?"VICTORY\nThe Warden falls.":"DEFEAT\nThe dark closes in.",title);y+=120;if(GUI.Button(new Rect(x,y,w,64),"START NEW RUN",button))game.Restart();return;}
            if(!compact){GUI.Label(new Rect(x,y,w,34),"SKILLS",title);y+=40;}
            foreach(var s in SkillBook.All){int cd=game.Player.Cooldowns[s.Name];string selected=game.SelectedSkill==s.Id?"▶ ":"";string label=compact?$"{selected}{s.Name} • {(cd>0?$"CD {cd}":"READY")}":$"{selected}{s.Name}  {(cd>0?$"[{cd}]":"[READY]")}\n{s.Hint}";GUI.enabled=game.Turns.Phase==TurnPhase.Player&&cd==0;if(GUI.Button(new Rect(x,y,w,compact?50:68),label,button))game.SelectSkill(s.Id);GUI.enabled=true;y+=compact?56:76;}
            float actionH=compact?48:54;if(game.SelectedSkill.HasValue&&GUI.Button(new Rect(x,y,w*.48f,actionH),"CANCEL",button)){game.CancelSkill();}
            GUI.enabled=game.Turns.Phase==TurnPhase.Player;if(GUI.Button(new Rect(x+w*.52f,y,w*.48f,actionH),"WAIT",button))game.WaitTurn();GUI.enabled=true;y+=actionH+4;
            GUI.Label(new Rect(x,y,w,compact?38:150),compact?$"{game.Theme.HazardName}: {game.Theme.HazardRule}":$"{game.Theme.HazardName}: {game.Theme.HazardRule}\n\nRED: enemy attack  •  CYAN: move\nGOLD: skill target",compact?center:body);
        }
        void DrawPortraitPanel(Rect r)
        {
            DrawRect(r,new Color(.045f,.04f,.075f));float pad=10,x=r.x+pad,w=r.width-pad*2,y=r.y+6;
            GUI.Label(new Rect(x,y,w,32),$"{game.Theme.Name.ToUpper()}  •  ROOM {game.RoomNumber}/5  •  HP {game.Player.Health}/{game.Player.MaxHealth}",center);y+=34;
            GUI.Label(new Rect(x,y,w,50),game.Message,center);y+=54;
            if(game.Turns.Phase==TurnPhase.Reward){DrawPortraitRewards(x,w,ref y);return;}
            if(game.Turns.Phase==TurnPhase.Won||game.Turns.Phase==TurnPhase.Lost){GUI.Label(new Rect(x,y,w,72),game.Turns.Phase==TurnPhase.Won?"VICTORY — THE LANTERN IS RESTORED":"DEFEAT — YOUR LANTERN IS EXTINGUISHED",title);y+=78;if(GUI.Button(new Rect(x,y,w,62),"START NEW RUN",button))game.Restart();return;}
            float gap=8,bw=(w-gap*2)/3f;
            for(int i=0;i<SkillBook.All.Length;i++)
            {
                var s=SkillBook.All[i];int cd=game.Player.Cooldowns[s.Name];GUI.enabled=game.Turns.Phase==TurnPhase.Player&&cd==0;
                string shortName=s.Id==SkillId.EmberBolt?"EMBER BOLT":s.Id==SkillId.LanternDash?"LANTERN DASH":"RADIANT SWEEP";
                string selected=game.SelectedSkill==s.Id?"▶ ":"";if(GUI.Button(new Rect(x+i*(bw+gap),y,bw,64),$"{selected}{shortName}\n{(cd>0?$"COOLDOWN {cd}":"READY")}",button))game.SelectSkill(s.Id);
            }
            GUI.enabled=true;y+=80;
            if(game.SelectedSkill.HasValue){if(GUI.Button(new Rect(x,y,w*.48f,54),"CANCEL SKILL",button))game.CancelSkill();}
            GUI.enabled=game.Turns.Phase==TurnPhase.Player;if(GUI.Button(new Rect(x+w*.52f,y,w*.48f,54),"WAIT",button))game.WaitTurn();GUI.enabled=true;y+=58;
            GUI.Label(new Rect(x,y,w,48),$"{game.Theme.HazardName}: {game.Theme.HazardRule}\nRED = attack   •   CYAN = move   •   GOLD = skill",center);
        }
        void DrawPortraitRewards(float x,float w,ref float y)
        {
            GUI.Label(new Rect(x,y,w,38),"CHOOSE ONE BLESSING",title);y+=42;float gap=8,bw=(w-gap*2)/3f;
            string[] labels={"VITAL EMBER\n+3 MAX HP","BRIGHT WICK\n+1 DAMAGE","SWIFT FLAME\n+1 MOVE"};
            for(int i=0;i<3;i++)if(GUI.Button(new Rect(x+i*(bw+gap),y,bw,82),labels[i],button))game.ChooseReward(i);
        }
        void DrawCompactRewards(float x,float w,ref float y){float gap=6,bw=(w-gap*2)/3f;string[] labels={"VITAL\n+3 HP","WICK\n+1 DMG","SWIFT\n+1 MOVE"};for(int i=0;i<3;i++)if(GUI.Button(new Rect(x+i*(bw+gap),y,bw,76),labels[i],button))game.ChooseReward(i);}
        void RewardButton(int id,string text,ref float y,float x,float w){if(GUI.Button(new Rect(x,y,w,76),text,button))game.ChooseReward(id);y+=84;}
        void DrawRect(Rect r,Color c){var old=GUI.color;GUI.color=c;GUI.DrawTexture(r,Texture2D.whiteTexture);GUI.color=old;}
    }
}
