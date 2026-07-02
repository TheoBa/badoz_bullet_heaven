using UnityEngine;
using BulletHeaven.Core;

namespace BulletHeaven.UI
{
    // Mid-run "Tier Complete" popup. Full-clear victory and death are both
    // handled by RunSummaryUI via GameManager's GameOver state instead.
    public class TierCompleteUI : MonoBehaviour
    {
        private bool   _visible;
        private string _message;

        // Cached GUIStyle -- OnGUI runs every frame, so allocating this inline would GC every frame.
        private GUIStyle _messageStyle;

        void Start()
        {
            // Start (not OnEnable) so this runs after every object's Awake() —
            // guarantees TierManager.Instance is already assigned regardless of hierarchy order.
            if (TierManager.Instance == null) return;
            TierManager.Instance.OnTierAdvanced += ShowTierComplete;
        }

        void OnDestroy()
        {
            if (TierManager.Instance == null) return;
            TierManager.Instance.OnTierAdvanced -= ShowTierComplete;
        }

        private void ShowTierComplete()
        {
            int tier = TierManager.Instance.CurrentTierNumber - 1; // we already advanced
            int resourcesEarned = TierManager.Instance.CurrentTierData.macroResourceReward;
            _message       = $"TIER {tier} COMPLETE!\n+{resourcesEarned} Resources";
            _visible       = true;
            Time.timeScale = 0f;
        }

        void OnGUI()
        {
            if (!_visible) return;

            var screenRect = new Rect(0, 0, Screen.width, Screen.height);
            GUI.color = new Color(0f, 0f, 0f, 0.7f);
            GUI.DrawTexture(screenRect, Texture2D.whiteTexture);
            GUI.color = Color.white;

            _messageStyle ??= new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, wordWrap = true, normal = { textColor = Color.white } };
            _messageStyle.fontSize = Mathf.RoundToInt(Screen.height * 0.06f);

            GUI.Label(new Rect(0, Screen.height * 0.3f, Screen.width, Screen.height * 0.3f),
                      _message, _messageStyle);

            var btnRect = MobileUI.EnsureMinSize(new Rect(Screen.width * 0.35f, Screen.height * 0.62f,
                                   Screen.width * 0.3f,  Screen.height * 0.08f));

            if (GUI.Button(btnRect, "Continue"))
            {
                _visible       = false;
                Time.timeScale = 1f;
            }
        }
    }
}
