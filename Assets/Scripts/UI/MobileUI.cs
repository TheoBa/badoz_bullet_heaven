using UnityEngine;

namespace BulletHeaven.UI
{
    // Shared helpers so every OnGUI screen meets the same mobile constraints
    // (this project uses IMGUI throughout, not uGUI, so there's no single
    // root Canvas to apply these to -- each screen opts in per-element).
    public static class MobileUI
    {
        private const float MinTouchDp = 48f;
        private const float FallbackDpi = 160f; // editor/desktop dpi is often 0; assume 1x density

        public static float MinTouchPx =>
            MinTouchDp * ((Screen.dpi > 0f ? Screen.dpi : FallbackDpi) / 160f);

        // Grows a button rect (from its center) up to the minimum touch target size.
        public static Rect EnsureMinSize(Rect r)
        {
            float min = MinTouchPx;
            if (r.width < min)
            {
                r.x    -= (min - r.width) * 0.5f;
                r.width = min;
            }
            if (r.height < min)
            {
                r.y     -= (min - r.height) * 0.5f;
                r.height = min;
            }
            return r;
        }

        // Screen.safeArea excludes notches/home indicators/rounded corners; on desktop
        // it's just the full screen, so these are all 0 there. safeArea uses a
        // bottom-left origin (like Screen), so these convert to IMGUI's top-left origin.
        public static float SafeTop    => Screen.height - (Screen.safeArea.y + Screen.safeArea.height);
        public static float SafeBottom => Screen.safeArea.y;
        public static float SafeLeft   => Screen.safeArea.x;
        public static float SafeRight  => Screen.width - (Screen.safeArea.x + Screen.safeArea.width);
    }
}
