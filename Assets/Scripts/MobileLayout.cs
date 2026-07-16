using UnityEngine;

namespace Lanternfall
{
    public enum MobileLayoutMode { Desktop, TabletLandscape, PhoneLandscape, PhonePortrait }

    public sealed class MobileLayoutSnapshot
    {
        public const float MinimumTouchTarget = 48f;
        public const float ComfortableTouchTarget = 56f;
        public bool Portrait;
        public bool CompactLandscape;
        public bool PhoneLandscape;
        public bool PhoneHud;
        public MobileLayoutMode Mode;
        public Rect Board;
        public Rect Panel;
        public Rect TopBar;
        public Rect SkillBar;
        public Rect ThreatPanel;
        public Rect CancelButton;
        public Rect[] SkillButtons;
        public Rect[] RewardButtons;
        public Rect ActionButton;
        public Rect RestartButton;
        public int FontSize;
        public float EstimatedTileSize;
        public bool HasOverlap => Board.Overlaps(Panel);
        public bool TouchTargetsValid
        {
            get
            {
                foreach(var r in SkillButtons)if(r.width<MinimumTouchTarget||r.height<MinimumTouchTarget)return false;
                foreach(var r in RewardButtons)if(r.width<MinimumTouchTarget||r.height<MinimumTouchTarget)return false;
                return ActionButton.width>=MinimumTouchTarget&&ActionButton.height>=MinimumTouchTarget&&RestartButton.width>=MinimumTouchTarget&&RestartButton.height>=MinimumTouchTarget;
            }
        }
    }

    public readonly struct MobileHudReadabilitySnapshot
    {
        public readonly int BaseFont;
        public readonly int HeaderFont;
        public readonly int StatFont;
        public readonly int MessageFont;
        public readonly int ButtonFont;
        public readonly int SkillFont;
        public readonly int CompactSkillFont;
        public MobileHudReadabilitySnapshot(int baseFont, int headerFont, int statFont, int messageFont, int buttonFont, int skillFont, int compactSkillFont)
        {
            BaseFont = baseFont;
            HeaderFont = headerFont;
            StatFont = statFont;
            MessageFont = messageFont;
            ButtonFont = buttonFont;
            SkillFont = skillFont;
            CompactSkillFont = compactSkillFont;
        }
    }

    public static class MobileHudReadability
    {
        public static MobileHudReadabilitySnapshot Compute(float width, float height)
        {
            bool phoneSized = Mathf.Min(width, height) < 620 && Mathf.Max(width, height) <= 1200;
            bool phoneLandscape = phoneSized && width > height;
            int min = Mathf.RoundToInt(Mathf.Min(width, height));
            int s = phoneLandscape ? Mathf.Clamp(min / 7, 42, 52) : phoneSized ? Mathf.Clamp(min / 8, 38, 48) : Mathf.Clamp(min / 28, 18, 30);
            return new MobileHudReadabilitySnapshot(
                s,
                Mathf.Clamp(s - 10, phoneLandscape ? 32 : phoneSized ? 28 : 15, phoneLandscape ? 40 : phoneSized ? 36 : 19),
                Mathf.Clamp(s - 1, phoneLandscape ? 42 : phoneSized ? 36 : 15, phoneLandscape ? 52 : phoneSized ? 46 : 19),
                Mathf.Clamp(s - 8, phoneLandscape ? 30 : phoneSized ? 28 : 13, phoneLandscape ? 36 : phoneSized ? 34 : 16),
                Mathf.Clamp(s - 4, phoneLandscape ? 38 : phoneSized ? 34 : 13, phoneLandscape ? 48 : phoneSized ? 44 : 17),
                Mathf.Clamp(s - 8, phoneLandscape ? 34 : phoneSized ? 32 : 13, phoneLandscape ? 42 : phoneSized ? 40 : 16),
                Mathf.Clamp(s - 10, phoneLandscape ? 32 : phoneSized ? 31 : 12, phoneLandscape ? 40 : phoneSized ? 38 : 14));
        }
    }

    public static class MobileLayout
    {
        public static MobileLayoutSnapshot Compute(float width,float height)
        {
            bool portrait=height>width;var result=new MobileLayoutSnapshot{Portrait=portrait,FontSize=Mathf.Clamp(Mathf.RoundToInt(Mathf.Min(width,height)/25f),16,28)};
            if(portrait)
            {
                result.PhoneHud = width <= 700f;
                result.Mode = result.PhoneHud ? MobileLayoutMode.PhonePortrait : MobileLayoutMode.Desktop;
                result.FontSize=Mathf.Clamp(Mathf.RoundToInt(Mathf.Min(width,height)/10f),32,40);
                float boardH = result.PhoneHud ? Mathf.Clamp(height*.44f,250f,370f) : Mathf.Clamp(height*.38f,260f,360f);
                float panelH=height-boardH;
                result.Board=new Rect(0,0,width,height-panelH);result.Panel=new Rect(0,height-panelH,width,panelH);
                float pad=8,gap=6,w=width-pad*2,y=result.Panel.y+(height<700f?106:116);
                float skillH=height<700f?76f:86f;
                float half=(w-gap)/2f;
                result.SkillButtons=new[]{new Rect(pad,y,half,skillH),new Rect(pad+half+gap,y,half,skillH),new Rect(pad,y+skillH+gap,w,skillH)};
                float rewardY=result.Panel.y+82,bw=(width-pad*2-gap*2)/3f;result.RewardButtons=new[]{new Rect(pad,rewardY,bw,92),new Rect(pad+bw+gap,rewardY,bw,92),new Rect(pad+(bw+gap)*2,rewardY,bw,92)};
                result.ActionButton=new Rect(pad,result.Panel.y+panelH-(height<700f?82f:90f),width-pad*2,height<700f?72f:80f);
                result.RestartButton=new Rect(pad,result.Panel.y+panelH-(height<700f?82f:90f),width-pad*2,height<700f?72f:80f);
            }
            else
            {
                result.CompactLandscape=height<760;
                result.PhoneLandscape=height<=620f&&width<=1200f&&width>height*1.15f;
                result.PhoneHud = result.PhoneLandscape;
                result.Mode = result.PhoneLandscape ? MobileLayoutMode.PhoneLandscape : result.CompactLandscape ? MobileLayoutMode.TabletLandscape : MobileLayoutMode.Desktop;
                if(result.PhoneLandscape) result.FontSize=Mathf.Clamp(Mathf.RoundToInt(height/8f),38,46);
                float panelW=result.PhoneLandscape?Mathf.Clamp(width*.255f,210f,250f):result.CompactLandscape?Mathf.Clamp(width*.30f,280f,340f):Mathf.Clamp(width*.32f,300f,360f);
                float panelH=result.PhoneLandscape?height:height;
                if(result.PhoneLandscape)
                {
                    float topH=Mathf.Clamp(height*.15f,56f,66f),bottomH=Mathf.Clamp(height*.18f,68f,80f),actionH=Mathf.Clamp(height*.18f,64f,76f);
                    float boardW=width-panelW;
                    result.TopBar=new Rect(0,0,boardW,topH);
                    result.Board=new Rect(0,topH,boardW,height-topH-bottomH);
                    result.SkillBar=new Rect(0,height-bottomH,boardW,bottomH);
                    result.ThreatPanel=new Rect(boardW,0,panelW,height-actionH-6f);
                    result.Panel=result.ThreatPanel;
                    float actionY=height-actionH;
                    result.CancelButton=new Rect(boardW+8f,actionY+4f,Mathf.Max(0f,panelW*.25f-8f),actionH-8f);
                    result.ActionButton=new Rect(boardW+panelW*.27f,actionY+4f,panelW*.70f-8f,actionH-8f);
                }
                else {result.Board=new Rect(0,0,width-panelW,height);result.Panel=new Rect(width-panelW,0,panelW,height);}
                float pad=10,y=result.PhoneLandscape?result.Panel.y+58f:result.CompactLandscape?94:198,h=result.PhoneLandscape?Mathf.Max(60f,(panelH-82f)*.45f):result.CompactLandscape?50:68;
                if(result.PhoneLandscape)
                {
                    float gap=6,bw=(result.SkillBar.width-pad*2-gap*2)/3f,sy=result.SkillBar.y+6f;
                    result.SkillButtons=new[]{new Rect(result.SkillBar.x+pad,sy,bw,result.SkillBar.height-12f),new Rect(result.SkillBar.x+pad+bw+gap,sy,bw,result.SkillBar.height-12f),new Rect(result.SkillBar.x+pad+(bw+gap)*2,sy,bw,result.SkillBar.height-12f)};
                }
                else result.SkillButtons=new[]{new Rect(result.Panel.x+pad,y,panelW-pad*2,h),new Rect(result.Panel.x+pad,y+h+6,panelW-pad*2,h),new Rect(result.Panel.x+pad,y+(h+6)*2,panelW-pad*2,h)};
                if(result.CompactLandscape){float gap=6,bw=(panelW-pad*2-gap*2)/3f,ry=128;result.RewardButtons=new[]{new Rect(result.Panel.x+pad,ry,bw,76),new Rect(result.Panel.x+pad+bw+gap,ry,bw,76),new Rect(result.Panel.x+pad+(bw+gap)*2,ry,bw,76)};}
                else result.RewardButtons=new[]{new Rect(result.Panel.x+pad,210,panelW-pad*2,76),new Rect(result.Panel.x+pad,294,panelW-pad*2,76),new Rect(result.Panel.x+pad,378,panelW-pad*2,76)};
                if(!result.PhoneLandscape)result.ActionButton=new Rect(result.Panel.x+pad,result.CompactLandscape?y+(h+6)*3:500,panelW-pad*2,48f);
                result.RestartButton=new Rect(result.Panel.x+pad,result.CompactLandscape?176:260,panelW-pad*2,64);
            }
            float boardHeader=result.PhoneLandscape?0:result.Portrait||result.CompactLandscape?42:64;
            result.EstimatedTileSize=Mathf.Min((result.Board.width-24)/9f,(result.Board.height-boardHeader-16)/11f);
            return result;
        }
        public static Rect ToGuiSafeArea(float screenHeight,Rect unitySafeArea)=>new(unitySafeArea.x,screenHeight-unitySafeArea.yMax,unitySafeArea.width,unitySafeArea.height);
    }
}
