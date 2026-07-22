using System.Runtime.InteropServices;
using UnityEngine;

namespace Lanternfall
{
    public readonly struct BrowserViewportSnapshot
    {
        public readonly float Width, Height, SafeLeft, SafeTop, SafeRight, SafeBottom;
        public readonly int Revision;
        public readonly bool BrowserProvided;

        public BrowserViewportSnapshot(float width,float height,float safeLeft,float safeTop,float safeRight,float safeBottom,int revision,bool browserProvided)
        {
            Width=Mathf.Max(1f,width);Height=Mathf.Max(1f,height);
            SafeLeft=Mathf.Max(0f,safeLeft);SafeTop=Mathf.Max(0f,safeTop);
            SafeRight=Mathf.Max(0f,safeRight);SafeBottom=Mathf.Max(0f,safeBottom);
            Revision=revision;BrowserProvided=browserProvided;
        }

        public Rect LogicalSafeArea => new(
            SafeLeft,SafeTop,
            Mathf.Max(1f,Width-SafeLeft-SafeRight),
            Mathf.Max(1f,Height-SafeTop-SafeBottom));

        public static BrowserViewportSnapshot FromUnity(float screenWidth,float screenHeight,Rect safeArea)
        {
            var guiSafe=MobileLayout.ToGuiSafeArea(screenHeight,safeArea);
            return new BrowserViewportSnapshot(screenWidth,screenHeight,guiSafe.x,guiSafe.y,
                screenWidth-guiSafe.xMax,screenHeight-guiSafe.yMax,0,false);
        }
    }

    public static class BrowserViewport
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")] static extern int LanternfallViewport_Revision();
        [DllImport("__Internal")] static extern float LanternfallViewport_Width();
        [DllImport("__Internal")] static extern float LanternfallViewport_Height();
        [DllImport("__Internal")] static extern float LanternfallViewport_SafeLeft();
        [DllImport("__Internal")] static extern float LanternfallViewport_SafeTop();
        [DllImport("__Internal")] static extern float LanternfallViewport_SafeRight();
        [DllImport("__Internal")] static extern float LanternfallViewport_SafeBottom();
#endif

        public static BrowserViewportSnapshot Read(float screenWidth,float screenHeight,Rect unitySafeArea)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            float width=LanternfallViewport_Width(),height=LanternfallViewport_Height();
            if(width>0f&&height>0f)
                return new BrowserViewportSnapshot(width,height,LanternfallViewport_SafeLeft(),LanternfallViewport_SafeTop(),
                    LanternfallViewport_SafeRight(),LanternfallViewport_SafeBottom(),LanternfallViewport_Revision(),true);
#endif
            return BrowserViewportSnapshot.FromUnity(screenWidth,screenHeight,unitySafeArea);
        }
    }
}
