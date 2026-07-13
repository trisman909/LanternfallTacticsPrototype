using UnityEngine;

namespace Lanternfall
{
    public sealed class MobileLayoutSnapshot
    {
        public const float MinimumTouchTarget = 48f;
        public bool Portrait;
        public bool CompactLandscape;
        public bool PhoneLandscape;
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
                result.FontSize=Mathf.Clamp(Mathf.RoundToInt(Mathf.Min(width,height)/19f),19,28);
                float minimumBoardH=height<=740f?240f:322f;
                float panelH=Mathf.Min(height-minimumBoardH,Mathf.Clamp(height*.62f,460f,520f));
                result.Board=new Rect(0,0,width,height-panelH);result.Panel=new Rect(0,height-panelH,width,panelH);
                float pad=12,gap=8,bw=(width-pad*2-gap*2)/3f,y=result.Panel.y+236;
                result.SkillButtons=new[]{new Rect(pad,y,bw,86),new Rect(pad+bw+gap,y,bw,86),new Rect(pad+(bw+gap)*2,y,bw,86)};
                float rewardY=result.Panel.y+78;result.RewardButtons=new[]{new Rect(pad,rewardY,bw,88),new Rect(pad+bw+gap,rewardY,bw,88),new Rect(pad+(bw+gap)*2,rewardY,bw,88)};
                result.ActionButton=new Rect(width*.52f,result.Panel.y+330,width*.48f-pad,60);
                result.RestartButton=new Rect(pad,result.Panel.y+330,width-pad*2,66);
            }
            else
            {
                result.CompactLandscape=height<760;
                result.PhoneLandscape=height<500f&&width<950f;
                if(result.PhoneLandscape) result.FontSize=Mathf.Clamp(Mathf.RoundToInt(height/17f),20,26);
                float panelW=result.PhoneLandscape?Mathf.Clamp(width*.52f,380f,450f):result.CompactLandscape?Mathf.Clamp(width*.30f,280f,340f):Mathf.Clamp(width*.32f,300f,360f);
                result.Board=new Rect(0,0,width-panelW,height);result.Panel=new Rect(width-panelW,0,panelW,height);
                float pad=10,y=result.PhoneLandscape?202:result.CompactLandscape?94:198,h=result.PhoneLandscape?74:result.CompactLandscape?50:68;
                if(result.PhoneLandscape)
                {
                    float gap=6,bw=(panelW-pad*2-gap*2)/3f;
                    result.SkillButtons=new[]{new Rect(result.Panel.x+pad,y,bw,h),new Rect(result.Panel.x+pad+bw+gap,y,bw,h),new Rect(result.Panel.x+pad+(bw+gap)*2,y,bw,h)};
                }
                else result.SkillButtons=new[]{new Rect(result.Panel.x+pad,y,panelW-pad*2,h),new Rect(result.Panel.x+pad,y+h+6,panelW-pad*2,h),new Rect(result.Panel.x+pad,y+(h+6)*2,panelW-pad*2,h)};
                if(result.CompactLandscape){float gap=6,bw=(panelW-pad*2-gap*2)/3f,ry=128;result.RewardButtons=new[]{new Rect(result.Panel.x+pad,ry,bw,76),new Rect(result.Panel.x+pad+bw+gap,ry,bw,76),new Rect(result.Panel.x+pad+(bw+gap)*2,ry,bw,76)};}
                else result.RewardButtons=new[]{new Rect(result.Panel.x+pad,210,panelW-pad*2,76),new Rect(result.Panel.x+pad,294,panelW-pad*2,76),new Rect(result.Panel.x+pad,378,panelW-pad*2,76)};
                result.ActionButton=new Rect(result.Panel.x+panelW*.52f,result.CompactLandscape?y+(h+6)*3:500,panelW*.48f-pad,48);
                result.RestartButton=new Rect(result.Panel.x+pad,result.CompactLandscape?176:260,panelW-pad*2,64);
            }
            float boardHeader=result.Portrait||result.CompactLandscape?42:64;
            result.EstimatedTileSize=Mathf.Min((result.Board.width-24)/9f,(result.Board.height-boardHeader-16)/11f);
            return result;
        }
        public static Rect ToGuiSafeArea(float screenHeight,Rect unitySafeArea)=>new(unitySafeArea.x,screenHeight-unitySafeArea.yMax,unitySafeArea.width,unitySafeArea.height);
    }
}
