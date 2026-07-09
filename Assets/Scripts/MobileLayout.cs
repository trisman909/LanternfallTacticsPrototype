using UnityEngine;

namespace Lanternfall
{
    public sealed class MobileLayoutSnapshot
    {
        public const float MinimumTouchTarget = 48f;
        public bool Portrait;
        public bool CompactLandscape;
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
                float panelH=Mathf.Min(height-260f,Mathf.Clamp(height*.42f,290f,430f));
                result.Board=new Rect(0,0,width,height-panelH);result.Panel=new Rect(0,height-panelH,width,panelH);
                float pad=10,gap=8,bw=(width-pad*2-gap*2)/3f,y=result.Panel.y+94;
                result.SkillButtons=new[]{new Rect(pad,y,bw,64),new Rect(pad+bw+gap,y,bw,64),new Rect(pad+(bw+gap)*2,y,bw,64)};
                float rewardY=result.Panel.y+136;result.RewardButtons=new[]{new Rect(pad,rewardY,bw,82),new Rect(pad+bw+gap,rewardY,bw,82),new Rect(pad+(bw+gap)*2,rewardY,bw,82)};
                result.ActionButton=new Rect(width*.52f,result.Panel.y+166,width*.48f-pad,50);
                result.RestartButton=new Rect(pad,result.Panel.y+172,width-pad*2,62);
            }
            else
            {
                float panelW=Mathf.Clamp(width*.36f,280f,390f);result.CompactLandscape=height<600;
                result.Board=new Rect(0,0,width-panelW,height);result.Panel=new Rect(width-panelW,0,panelW,height);
                float pad=10,y=result.CompactLandscape?94:198,h=result.CompactLandscape?50:68;
                result.SkillButtons=new[]{new Rect(result.Panel.x+pad,y,panelW-pad*2,h),new Rect(result.Panel.x+pad,y+h+6,panelW-pad*2,h),new Rect(result.Panel.x+pad,y+(h+6)*2,panelW-pad*2,h)};
                if(result.CompactLandscape){float gap=6,bw=(panelW-pad*2-gap*2)/3f,ry=128;result.RewardButtons=new[]{new Rect(result.Panel.x+pad,ry,bw,76),new Rect(result.Panel.x+pad+bw+gap,ry,bw,76),new Rect(result.Panel.x+pad+(bw+gap)*2,ry,bw,76)};}
                else result.RewardButtons=new[]{new Rect(result.Panel.x+pad,210,panelW-pad*2,76),new Rect(result.Panel.x+pad,294,panelW-pad*2,76),new Rect(result.Panel.x+pad,378,panelW-pad*2,76)};
                result.ActionButton=new Rect(result.Panel.x+panelW*.52f,result.CompactLandscape?y+(h+6)*3:500,panelW*.48f-pad,48);
                result.RestartButton=new Rect(result.Panel.x+pad,result.CompactLandscape?176:260,panelW-pad*2,64);
            }
            float boardHeader=result.CompactLandscape?54:72;
            result.EstimatedTileSize=Mathf.Min((result.Board.width-24)/9f,(result.Board.height-boardHeader-16)/11f);
            return result;
        }
        public static Rect ToGuiSafeArea(float screenHeight,Rect unitySafeArea)=>new(unitySafeArea.x,screenHeight-unitySafeArea.yMax,unitySafeArea.width,unitySafeArea.height);
    }
}
