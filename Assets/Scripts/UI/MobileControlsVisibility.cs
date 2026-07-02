using UnityEngine;

namespace BulletHeaven.UI
{
    // Hides the on-screen joystick canvas on desktop; only Android/iOS builds show it.
    public class MobileControlsVisibility : MonoBehaviour
    {
        void Awake()
        {
#if UNITY_ANDROID || UNITY_IOS
            gameObject.SetActive(true);
#else
            gameObject.SetActive(false);
#endif
        }
    }
}
