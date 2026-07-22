using System.Linq;
using UnityEngine;

namespace Lanternfall
{
    public enum MobileLayoutMode { Desktop, TabletLandscape, PhoneLandscape, PhonePortrait }

    public sealed class MobileLayoutSnapshot
    {
        public const float MinimumTouchTarget = 44f;
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
        public Rect[] StatContentRects;
        public Rect TitleContentRect;
        public Rect[] SkillButtons;
        public Rect[] SkillContentRects;
        public Rect[] SkillNameRects;
        public Rect[] SkillCostRects;
        public Rect[] SkillStateRects;
        public Rect[] RewardButtons;
        public Rect ActionButton;
        public Rect FullActionButton;
        public Rect EndTurnArt;
        public Rect EndTurnLabel;
        public Rect FullEndTurnArt;
        public Rect FullEndTurnLabel;
        public Rect ThreatContentRect;
        public Rect HelpButton;
        public Rect HelpContentRect;
        public Rect InfoButton;
        public Rect InfoContentRect;
        public Rect ModalPanel;
        public Rect ModalHeaderRect;
        public Rect ModalSubtitleRect;
        public Rect[] ModalCards;
        public Rect[] ModalCardContentRects;
        public Rect ModalHelpButton;
        public Rect ModalInfoButton;
        public Rect ModalPrimaryButton;
        public Rect RestartButton;
        public int FontSize;
        public float EstimatedTileSize;
        public bool HasOverlap => Board.Overlaps(Panel);
        public bool TouchTargetsValid
        {
            get
            {
                foreach(var r in SkillButtons)if(r.width<MinimumTouchTarget||r.height<MinimumTouchTarget)return false;
                if(!PhoneLandscape)foreach(var r in RewardButtons)if(r.width<MinimumTouchTarget||r.height<MinimumTouchTarget)return false;
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
                float panelW=result.PhoneLandscape?Mathf.Clamp(width*.18f,152f,174f):result.CompactLandscape?Mathf.Clamp(width*.30f,280f,340f):Mathf.Clamp(width*.32f,300f,360f);
                float panelH=result.PhoneLandscape?height:height;
                if(result.PhoneLandscape)
                {
                    float topH=Mathf.Clamp(height*.09f,34f,39f),bottomH=Mathf.Clamp(height*.12f,48f,52f),actionH=Mathf.Clamp(height*.13f,52f,58f);
                    float boardW=width-panelW;
                    result.TopBar=new Rect(0,0,boardW,topH);
                    result.Board=new Rect(0,topH,boardW,height-topH-bottomH);
                    result.SkillBar=new Rect(0,height-bottomH,boardW,bottomH);
                    result.ThreatPanel=new Rect(boardW,0,panelW,height-actionH-4f);
                    result.Panel=result.ThreatPanel;
                    float actionY=height-actionH;
                    float outer=4f,cancelW=44f,gap=4f;
                    result.CancelButton=new Rect(boardW+outer,actionY+4f,cancelW,actionH-8f);
                    result.ActionButton=new Rect(result.CancelButton.xMax+gap,actionY+4f,panelW-outer*2-cancelW-gap,actionH-8f);
                    result.FullActionButton=new Rect(boardW+outer,actionY+4f,panelW-outer*2,actionH-8f);
                    result.EndTurnArt=result.ActionButton;
                    result.EndTurnLabel=Inset(result.EndTurnArt,8f,4f);
                    result.FullEndTurnArt=result.FullActionButton;
                    result.FullEndTurnLabel=Inset(result.FullEndTurnArt,10f,4f);
                    float statPad=4f,statsW=result.TopBar.width*.48f,statGap=3f,statW=(statsW-statPad*2-statGap*2)/3f;
                    result.StatChips=new[]{new Rect(statPad,3f,statW,result.TopBar.height-6f),new Rect(statPad+statW+statGap,3f,statW,result.TopBar.height-6f),new Rect(statPad+(statW+statGap)*2,3f,statW,result.TopBar.height-6f)};
                    result.StatContentRects=result.StatChips.Select(r=>Inset(r,5f,3f)).ToArray();
                    result.TitleContentRect=Inset(new Rect(statsW+5f,2f,result.TopBar.width-statsW-8f,result.TopBar.height-4f),6f,2f);
                    float utilityH=Mathf.Clamp(height*.10f,40f,44f),utilityY=result.ThreatPanel.yMax-utilityH-3f,utilityGap=4f,utilityW=(panelW-outer*2-utilityGap)/2f;
                    result.HelpButton=new Rect(boardW+outer,utilityY,utilityW,utilityH);
                    result.InfoButton=new Rect(result.HelpButton.xMax+utilityGap,utilityY,utilityW,utilityH);
                    result.HelpContentRect=Inset(result.HelpButton,7f,5f);result.InfoContentRect=Inset(result.InfoButton,7f,5f);
                    result.ThreatContentRect=new Rect(boardW+6f,30f,panelW-12f,Mathf.Max(0f,utilityY-34f));

                    float modalPad=Mathf.Clamp(width*.018f,8f,16f),modalGap=6f;
                    result.ModalPanel=new Rect(modalPad,8f,width-modalPad*2f,height-16f);
                    result.ModalHeaderRect=Inset(new Rect(result.ModalPanel.x+6f,result.ModalPanel.y+5f,result.ModalPanel.width-12f,34f),4f,1f);
                    result.ModalSubtitleRect=Inset(new Rect(result.ModalPanel.x+6f,result.ModalHeaderRect.yMax,result.ModalPanel.width-12f,24f),4f,1f);
                    float modalUtilityH=44f,modalUtilityY=result.ModalPanel.yMax-modalUtilityH-6f;
                    float modalUtilityW=56f;
                    result.ModalHelpButton=new Rect(result.ModalPanel.x+6f,modalUtilityY,modalUtilityW,modalUtilityH);
                    result.ModalInfoButton=new Rect(result.ModalHelpButton.xMax+modalGap,modalUtilityY,modalUtilityW,modalUtilityH);
                    result.ModalPrimaryButton=new Rect(result.ModalInfoButton.xMax+modalGap,modalUtilityY,result.ModalPanel.xMax-result.ModalInfoButton.xMax-modalGap-6f,modalUtilityH);
                    float cardsY=result.ModalSubtitleRect.yMax+4f,cardsH=modalUtilityY-cardsY-6f,cardW=(result.ModalPanel.width-12f-modalGap*2f)/3f;
                    result.ModalCards=new[]{new Rect(result.ModalPanel.x+6f,cardsY,cardW,cardsH),new Rect(result.ModalPanel.x+6f+cardW+modalGap,cardsY,cardW,cardsH),new Rect(result.ModalPanel.x+6f+(cardW+modalGap)*2f,cardsY,cardW,cardsH)};
                    result.ModalCardContentRects=result.ModalCards.Select(r=>Inset(r,Mathf.Max(8f,r.width*.05f),8f)).ToArray();
                }
                else {result.Board=new Rect(0,0,width-panelW,height);result.Panel=new Rect(width-panelW,0,panelW,height);}
                float pad=10,y=result.PhoneLandscape?result.Panel.y+58f:result.CompactLandscape?94:198,h=result.PhoneLandscape?Mathf.Max(60f,(panelH-82f)*.45f):result.CompactLandscape?50:68;
                if(result.PhoneLandscape)
                {
                    float gap=4,bw=(result.SkillBar.width-4f*2-gap*2)/3f,sy=result.SkillBar.y+2f;
                    result.SkillButtons=new[]{new Rect(result.SkillBar.x+4f,sy,bw,result.SkillBar.height-4f),new Rect(result.SkillBar.x+4f+bw+gap,sy,bw,result.SkillBar.height-4f),new Rect(result.SkillBar.x+4f+(bw+gap)*2,sy,bw,result.SkillBar.height-4f)};
                    result.SkillContentRects=result.SkillButtons.Select(r=>Inset(r,Mathf.Max(8f,r.width*.045f),3f)).ToArray();
                    result.SkillNameRects=result.SkillContentRects.Select(r=>new Rect(r.x,r.y,r.width*.68f,r.height*.56f)).ToArray();
                    result.SkillCostRects=result.SkillContentRects.Select(r=>new Rect(r.x+r.width*.69f,r.y,r.width*.31f,r.height*.56f)).ToArray();
                    result.SkillStateRects=result.SkillContentRects.Select(r=>new Rect(r.x,r.y+r.height*.54f,r.width,r.height*.46f)).ToArray();
                }
                else result.SkillButtons=new[]{new Rect(result.Panel.x+pad,y,panelW-pad*2,h),new Rect(result.Panel.x+pad,y+h+6,panelW-pad*2,h),new Rect(result.Panel.x+pad,y+(h+6)*2,panelW-pad*2,h)};
                if(result.CompactLandscape){float gap=6,bw=(panelW-pad*2-gap*2)/3f,ry=128;result.RewardButtons=new[]{new Rect(result.Panel.x+pad,ry,bw,76),new Rect(result.Panel.x+pad+bw+gap,ry,bw,76),new Rect(result.Panel.x+pad+(bw+gap)*2,ry,bw,76)};}
                else result.RewardButtons=new[]{new Rect(result.Panel.x+pad,210,panelW-pad*2,76),new Rect(result.Panel.x+pad,294,panelW-pad*2,76),new Rect(result.Panel.x+pad,378,panelW-pad*2,76)};
                if(!result.PhoneLandscape)result.ActionButton=new Rect(result.Panel.x+pad,result.CompactLandscape?y+(h+6)*3:500,panelW-pad*2,48f);
                result.RestartButton=new Rect(result.Panel.x+pad,result.CompactLandscape?176:260,panelW-pad*2,64);
            }
            float boardHeader=result.PhoneLandscape?0:result.Portrait||result.CompactLandscape?42:64;
            result.EstimatedTileSize=result.PhoneLandscape?BoardFitLayout.ComputePhoneOccupied(result.Board,9,11).TileSize:Mathf.Min((result.Board.width-24)/9f,(result.Board.height-boardHeader-16)/11f);
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
