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
            game=gameObject.AddComponent<LanternfallGame>();cam=new GameObject("Camera").AddComponent<Camera>();cam.orthographic=true;cam.transform.position=new Vector3(4,5,-10);cam.backgroundColor=new Color(.025f,.02f,.06f);cam.orthographicSize=6.7f;
            game.Changed+=()=>{};game.StartRun();
        }
        void InitStyles()
        {
            int s=Mathf.Clamp(Screen.width/28,18,34);title=new GUIStyle(GUI.skin.label){fontSize=s+5,fontStyle=FontStyle.Bold,alignment=TextAnchor.MiddleCenter,normal={textColor=new Color(1,.78f,.28f)}};
            body=new GUIStyle(GUI.skin.label){fontSize=s,alignment=TextAnchor.MiddleLeft,normal={textColor=Color.white}};center=new GUIStyle(body){alignment=TextAnchor.MiddleCenter};
            button=new GUIStyle(GUI.skin.button){fontSize=s,fontStyle=FontStyle.Bold,wordWrap=true,normal={textColor=Color.white,background=Tex(new Color(.12f,.12f,.2f))},hover={background=Tex(new Color(.23f,.2f,.32f))},active={background=Tex(new Color(.5f,.3f,.1f))}};
        }
        Texture2D Tex(Color c){var t=new Texture2D(1,1);t.SetPixel(0,0,c);t.Apply();return t;}
        void OnGUI()
        {
            if(title==null)InitStyles();float ui=Mathf.Min(360,Screen.width*.32f);float boardW=Screen.width-ui;DrawBoard(new Rect(0,0,boardW,Screen.height));DrawPanel(new Rect(boardW,0,ui,Screen.height));
        }
        void DrawBoard(Rect area)
        {
            float top=58;tile=Mathf.Min((area.width-24)/game.Grid.Width,(area.height-top-24)/game.Grid.Height);float ox=(area.width-game.Grid.Width*tile)/2;float oy=top+(area.height-top-game.Grid.Height*tile)/2;
            GUI.Label(new Rect(0,5,area.width,45),game.Turns.Phase==TurnPhase.Enemy?"ENEMY TURN":"PLAYER TURN",title);
            foreach(var p in game.Grid.Floors())
            {
                var r=new Rect(ox+p.x*tile,oy+(game.Grid.Height-1-p.y)*tile,tile-2,tile-2);Color c=new Color(.12f,.14f,.18f);
                if(game.Enemies.Any(e=>e.Alive&&e.Preview.Contains(p)))c=new Color(.62f,.12f,.13f);
                if(game.ValidTargets.Contains(p)&&game.Turns.Phase==TurnPhase.Player)c=game.SelectedSkill.HasValue?new Color(.65f,.48f,.08f):new Color(.08f,.5f,.55f);
                DrawRect(r,c);if(Event.current.type==EventType.MouseDown&&r.Contains(Event.current.mousePosition)){game.TapTile(p);Event.current.Use();}
            }
            DrawToken(game.Player.Position,ox,oy,new Color(.2f,.9f,1f),"✦");
            foreach(var e in game.Enemies.Where(e=>e.Alive))DrawToken(e.Position,ox,oy,e.Kind==EnemyKind.LanternWarden?new Color(.9f,.25f,.8f):new Color(.85f,.33f,.2f),e.Kind switch{EnemyKind.Ashling=>"A",EnemyKind.GloomArcher=>"G",EnemyKind.StoneSentinel=>"S",_=>"W"},$"{e.Health}");
        }
        void DrawToken(Vector2Int p,float ox,float oy,Color c,string glyph,string hp="")
        {
            var r=new Rect(ox+p.x*tile+tile*.12f,oy+(game.Grid.Height-1-p.y)*tile+tile*.08f,tile*.72f,tile*.72f);DrawRect(r,c);var st=new GUIStyle(center){fontSize=Mathf.RoundToInt(tile*.42f),fontStyle=FontStyle.Bold,normal={textColor=Color.black}};GUI.Label(r,glyph,st);if(hp!="")GUI.Label(new Rect(r.x,r.y+r.height-18,r.width,20),hp,new GUIStyle(center){fontSize=14,fontStyle=FontStyle.Bold,normal={textColor=Color.white}});
        }
        void DrawPanel(Rect r)
        {
            DrawRect(r,new Color(.045f,.04f,.075f));float x=r.x+12,w=r.width-24,y=12;GUI.Label(new Rect(x,y,w,42),"LANTERNFALL",title);y+=48;
            GUI.Label(new Rect(x,y,w,66),$"Room {game.RoomNumber}/5  •  HP {game.Player.Health}/{game.Player.MaxHealth}\n{game.Message}",body);y+=74;
            if(game.Turns.Phase==TurnPhase.Reward){GUI.Label(new Rect(x,y,w,45),"CHOOSE A BLESSING",title);y+=50;RewardButton(0,"VITAL EMBER\n+3 max HP",ref y,x,w);RewardButton(1,"BRIGHT WICK\n+1 skill damage",ref y,x,w);RewardButton(2,"SWIFT FLAME\n+1 move range",ref y,x,w);return;}
            if(game.Turns.Phase==TurnPhase.Won||game.Turns.Phase==TurnPhase.Lost){GUI.Label(new Rect(x,y,w,100),game.Turns.Phase==TurnPhase.Won?"VICTORY\nThe Warden falls.":"DEFEAT\nThe dark closes in.",title);y+=120;if(GUI.Button(new Rect(x,y,w,64),"START NEW RUN",button))game.Restart();return;}
            GUI.Label(new Rect(x,y,w,34),"SKILLS",title);y+=40;
            foreach(var s in SkillBook.All){int cd=game.Player.Cooldowns[s.Name];string label=$"{s.Name}  {(cd>0?$"[{cd}]":"[READY]")}\n{s.Hint}";GUI.enabled=game.Turns.Phase==TurnPhase.Player&&cd==0;if(GUI.Button(new Rect(x,y,w,68),label,button))game.SelectSkill(s.Id);GUI.enabled=true;y+=76;}
            if(game.SelectedSkill.HasValue&&GUI.Button(new Rect(x,y,w,54),"CANCEL SKILL",button)){game.CancelSkill();}y+=62;
            GUI.enabled=game.Turns.Phase==TurnPhase.Player;if(GUI.Button(new Rect(x,y,w,58),"WAIT",button))game.WaitTurn();GUI.enabled=true;y+=66;
            GUI.Label(new Rect(x,y,w,150),"RED tiles: enemy's next attack\nCYAN tiles: valid movement\nGOLD tiles: valid skill targets\n\nEnemies: A Ashling • G Archer • S Sentinel",body);
        }
        void RewardButton(int id,string text,ref float y,float x,float w){if(GUI.Button(new Rect(x,y,w,76),text,button))game.ChooseReward(id);y+=84;}
        void DrawRect(Rect r,Color c){var old=GUI.color;GUI.color=c;GUI.DrawTexture(r,Texture2D.whiteTexture);GUI.color=old;}
    }
}
