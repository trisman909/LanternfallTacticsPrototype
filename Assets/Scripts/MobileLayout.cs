using System.Linq;
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
        public Rect[] StatChips;
        public Rect[] SkillButtons;
        public Rect[] SkillContentRects;
        public Rect[] RewardButtons;
        public Rect ActionButton;
        public Rect EndTurnArt;
        public Rect EndTurnLabel;
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
            int s = phoneLandscape ? Mathf.Clamp(min / 22, 17, 21) : phoneSized ? Mathf.Clamp(min / 20, 17, 22) : Mathf.Clamp(min / 28, 18, 30);
            if (!phoneSized)
                return new MobileHudReadabilitySnapshot(s, Mathf.Clamp(s - 10, 15, 19), Mathf.Clamp(s - 1, 15, 19), Mathf.Clamp(s - 8, 13, 16), Mathf.Clamp(s - 4, 13, 17), Mathf.Clamp(s - 8, 13, 16), Mathf.Clamp(s - 10, 12, 14));
            return new MobileHudReadabilitySnapshot(
                s,
                Mathf.Clamp(s + 1, 18, 24),
                Mathf.Clamp(s + 4, 20, 26),
                Mathf.Clamp(s - 2, 14, 19),
                Mathf.Clamp(s + 1, 18, 23),
                Mathf.Clamp(s, 16, 21),
                Mathf.Clamp(s - 2, 15, 19));
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
                if(result.PhoneLandscape) result.FontSize=Mathf.Clamp(Mathf.RoundToInt(height/22f),17,21);
                float panelW=result.PhoneLandscape?Mathf.Clamp(width*.235f,200f,228f):result.CompactLandscape?Mathf.Clamp(width*.30f,280f,340f):Mathf.Clamp(width*.32f,300f,360f);
                float panelH=result.PhoneLandscape?height:height;
                if(result.PhoneLandscape)
                {
                    float topH=Mathf.Clamp(height*.132f,50f,58f),bottomH=Mathf.Clamp(height*.155f,63f,70f),actionH=Mathf.Clamp(height*.17f,64f,72f);
                    float boardW=width-panelW;
                    result.TopBar=new Rect(0,0,boardW,topH);
                    result.Board=new Rect(0,topH,boardW,height-topH-bottomH);
                    result.SkillBar=new Rect(0,height-bottomH,boardW,bottomH);
                    result.ThreatPanel=new Rect(boardW,0,panelW,height-actionH-6f);
                    result.Panel=result.ThreatPanel;
                    float actionY=height-actionH;
                    result.CancelButton=new Rect(boardW+8f,actionY+4f,Mathf.Max(0f,panelW*.25f-8f),actionH-8f);
                    result.ActionButton=new Rect(boardW+panelW*.27f,actionY+4f,panelW*.70f-8f,actionH-8f);
                    result.EndTurnArt=AspectFit(Inset(result.ActionButton,6f,5f),3.35f);
                    result.EndTurnLabel=Inset(result.EndTurnArt,16f,6f);
                    float statPad=5f,statsW=result.TopBar.width*.52f,statGap=4f,statW=(statsW-statPad*2-statGap*2)/3f;
                    result.StatChips=new[]{new Rect(statPad,4f,statW,result.TopBar.height-8f),new Rect(statPad+statW+statGap,4f,statW,result.TopBar.height-8f),new Rect(statPad+(statW+statGap)*2,4f,statW,result.TopBar.height-8f)};
                }
                else {result.Board=new Rect(0,0,width-panelW,height);result.Panel=new Rect(width-panelW,0,panelW,height);}
                float pad=10,y=result.PhoneLandscape?result.Panel.y+58f:result.CompactLandscape?94:198,h=result.PhoneLandscape?Mathf.Max(60f,(panelH-82f)*.45f):result.CompactLandscape?50:68;
                if(result.PhoneLandscape)
                {
                    float gap=5,bw=(result.SkillBar.width-8f*2-gap*2)/3f,sy=result.SkillBar.y+3f;
                    result.SkillButtons=new[]{new Rect(result.SkillBar.x+8f,sy,bw,result.SkillBar.height-6f),new Rect(result.SkillBar.x+8f+bw+gap,sy,bw,result.SkillBar.height-6f),new Rect(result.SkillBar.x+8f+(bw+gap)*2,sy,bw,result.SkillBar.height-6f)};
                    result.SkillContentRects=result.SkillButtons.Select(r=>Inset(r,12f,7f)).ToArray();
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
        public static Rect Inset(Rect r,float horizontal,float vertical)=>new(r.x+horizontal,r.y+vertical,Mathf.Max(0f,r.width-horizontal*2f),Mathf.Max(0f,r.height-vertical*2f));
        public static Rect AspectFit(Rect r,float aspect)
        {
            float w=r.width,h=w/aspect;if(h>r.height){h=r.height;w=h*aspect;}
            return new Rect(r.center.x-w*.5f,r.center.y-h*.5f,w,h);
        }
        public static Rect ToGuiSafeArea(float screenHeight,Rect unitySafeArea)=>new(unitySafeArea.x,screenHeight-unitySafeArea.yMax,unitySafeArea.width,unitySafeArea.height);
    }
}
