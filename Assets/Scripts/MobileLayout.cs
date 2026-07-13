using UnityEngine;

namespace Lanternfall
{
    public sealed class MobileLayoutSnapshot
    {
        public const float MinimumTouchTarget = 48f;
        public const float ComfortableTouchTarget = 56f;
        public bool Portrait;
        public bool CompactLandscape;
        public bool PhoneLandscape;
        public bool PhoneHud;
        public Rect Board;
        public Rect Panel;
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

    public static class MobileLayout
    {
        public static MobileLayoutSnapshot Compute(float width,float height)
        {
            bool portrait=height>width;var result=new MobileLayoutSnapshot{Portrait=portrait,FontSize=Mathf.Clamp(Mathf.RoundToInt(Mathf.Min(width,height)/25f),16,28)};
            if(portrait)
            {
                result.PhoneHud = width < 560f;
                result.FontSize=Mathf.Clamp(Mathf.RoundToInt(Mathf.Min(width,height)/12f),28,36);
                float boardH = result.PhoneHud ? Mathf.Clamp(height*.23f,150f,190f) : Mathf.Clamp(height*.38f,260f,360f);
                float panelH=height-boardH;
                result.Board=new Rect(0,0,width,height-panelH);result.Panel=new Rect(0,height-panelH,width,panelH);
                float pad=12,gap=8,w=width-pad*2,y=result.Panel.y+(height<700f?198:222);
                float skillH=height<700f?72f:84f;
                result.SkillButtons=new[]{new Rect(pad,y,w,skillH),new Rect(pad,y+skillH+gap,w,skillH),new Rect(pad,y+(skillH+gap)*2,w,skillH)};
                float rewardY=result.Panel.y+82,bw=(width-pad*2-gap*2)/3f;result.RewardButtons=new[]{new Rect(pad,rewardY,bw,92),new Rect(pad+bw+gap,rewardY,bw,92),new Rect(pad+(bw+gap)*2,rewardY,bw,92)};
                result.ActionButton=new Rect(pad,result.Panel.y+panelH-(height<700f?82f:92f),width-pad*2,height<700f?74f:84f);
                result.RestartButton=new Rect(pad,result.Panel.y+panelH-(height<700f?82f:92f),width-pad*2,height<700f?74f:84f);
            }
            else
            {
                result.CompactLandscape=height<760;
                result.PhoneLandscape=height<500f&&width<950f;
                result.PhoneHud = result.PhoneLandscape;
                if(result.PhoneLandscape) result.FontSize=Mathf.Clamp(Mathf.RoundToInt(height/11f),30,38);
                float panelW=result.PhoneLandscape?Mathf.Clamp(width*.72f,520f,640f):result.CompactLandscape?Mathf.Clamp(width*.30f,280f,340f):Mathf.Clamp(width*.32f,300f,360f);
                result.Board=new Rect(0,0,width-panelW,height);result.Panel=new Rect(width-panelW,0,panelW,height);
                float pad=10,y=result.PhoneLandscape?154:result.CompactLandscape?94:198,h=result.PhoneLandscape?96:result.CompactLandscape?50:68;
                if(result.PhoneLandscape)
                {
                    float gap=6,bw=(panelW-pad*2-gap*2)/3f;
                    result.SkillButtons=new[]{new Rect(result.Panel.x+pad,y,bw,h),new Rect(result.Panel.x+pad+bw+gap,y,bw,h),new Rect(result.Panel.x+pad+(bw+gap)*2,y,bw,h)};
                }
                else result.SkillButtons=new[]{new Rect(result.Panel.x+pad,y,panelW-pad*2,h),new Rect(result.Panel.x+pad,y+h+6,panelW-pad*2,h),new Rect(result.Panel.x+pad,y+(h+6)*2,panelW-pad*2,h)};
                if(result.CompactLandscape){float gap=6,bw=(panelW-pad*2-gap*2)/3f,ry=128;result.RewardButtons=new[]{new Rect(result.Panel.x+pad,ry,bw,76),new Rect(result.Panel.x+pad+bw+gap,ry,bw,76),new Rect(result.Panel.x+pad+(bw+gap)*2,ry,bw,76)};}
                else result.RewardButtons=new[]{new Rect(result.Panel.x+pad,210,panelW-pad*2,76),new Rect(result.Panel.x+pad,294,panelW-pad*2,76),new Rect(result.Panel.x+pad,378,panelW-pad*2,76)};
                result.ActionButton=new Rect(result.Panel.x+pad,result.PhoneLandscape?height-76f:result.CompactLandscape?y+(h+6)*3:500,panelW-pad*2,result.PhoneLandscape?68f:48f);
                result.RestartButton=new Rect(result.Panel.x+pad,result.CompactLandscape?176:260,panelW-pad*2,64);
            }
            float boardHeader=result.Portrait||result.CompactLandscape?42:64;
            result.EstimatedTileSize=Mathf.Min((result.Board.width-24)/9f,(result.Board.height-boardHeader-16)/11f);
            return result;
        }
        public static Rect ToGuiSafeArea(float screenHeight,Rect unitySafeArea)=>new(unitySafeArea.x,screenHeight-unitySafeArea.yMax,unitySafeArea.width,unitySafeArea.height);
    }
}
