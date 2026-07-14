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
                result.PhoneLandscape=height<500f&&width<950f;
                result.PhoneHud = result.PhoneLandscape;
                if(result.PhoneLandscape) result.FontSize=Mathf.Clamp(Mathf.RoundToInt(height/9f),34,42);
                float panelW=result.PhoneLandscape?width:result.CompactLandscape?Mathf.Clamp(width*.30f,280f,340f):Mathf.Clamp(width*.32f,300f,360f);
                float panelH=result.PhoneLandscape?Mathf.Clamp(height*.44f,158f,184f):height;
                result.Board=result.PhoneLandscape?new Rect(0,0,width,height-panelH):new Rect(0,0,width-panelW,height);
                result.Panel=result.PhoneLandscape?new Rect(0,height-panelH,width,panelH):new Rect(width-panelW,0,panelW,height);
                float pad=10,y=result.PhoneLandscape?result.Panel.y+58f:result.CompactLandscape?94:198,h=result.PhoneLandscape?Mathf.Max(88f,panelH-68f):result.CompactLandscape?50:68;
                if(result.PhoneLandscape)
                {
                    float gap=6,bw=Mathf.Clamp((panelW-pad*2-gap*4)*.20f,140f,172f),sy=result.Panel.y+58f;
                    result.SkillButtons=new[]{new Rect(result.Panel.x+pad,sy,bw,h),new Rect(result.Panel.x+pad+bw+gap,sy,bw,h),new Rect(result.Panel.x+pad+(bw+gap)*2,sy,bw,h)};
                }
                else result.SkillButtons=new[]{new Rect(result.Panel.x+pad,y,panelW-pad*2,h),new Rect(result.Panel.x+pad,y+h+6,panelW-pad*2,h),new Rect(result.Panel.x+pad,y+(h+6)*2,panelW-pad*2,h)};
                if(result.CompactLandscape){float gap=6,bw=(panelW-pad*2-gap*2)/3f,ry=128;result.RewardButtons=new[]{new Rect(result.Panel.x+pad,ry,bw,76),new Rect(result.Panel.x+pad+bw+gap,ry,bw,76),new Rect(result.Panel.x+pad+(bw+gap)*2,ry,bw,76)};}
                else result.RewardButtons=new[]{new Rect(result.Panel.x+pad,210,panelW-pad*2,76),new Rect(result.Panel.x+pad,294,panelW-pad*2,76),new Rect(result.Panel.x+pad,378,panelW-pad*2,76)};
                result.ActionButton=new Rect(result.Panel.x+pad,result.PhoneLandscape?result.Panel.y+58f:result.CompactLandscape?y+(h+6)*3:500,panelW-pad*2,result.PhoneLandscape?h:48f);
                result.RestartButton=new Rect(result.Panel.x+pad,result.CompactLandscape?176:260,panelW-pad*2,64);
            }
            float boardHeader=result.Portrait||result.CompactLandscape?42:64;
            result.EstimatedTileSize=Mathf.Min((result.Board.width-24)/9f,(result.Board.height-boardHeader-16)/11f);
            return result;
        }
        public static Rect ToGuiSafeArea(float screenHeight,Rect unitySafeArea)=>new(unitySafeArea.x,screenHeight-unitySafeArea.yMax,unitySafeArea.width,unitySafeArea.height);
    }
}
