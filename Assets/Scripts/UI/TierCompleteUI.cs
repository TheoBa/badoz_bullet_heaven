using UnityEngine;
using BulletHeaven.Core;

namespace BulletHeaven.UI
{
    public class TierCompleteUI : MonoBehaviour
    {
        private bool   _visible;
        private bool   _isVictory;
        private string _message;
        private int    _resourcesEarned;

        void OnEnable()
        {
            if (TierManager.Instance == null) return;
            TierManager.Instance.OnTierAdvanced += ShowTierComplete;
            TierManager.Instance.OnRunCleared   += ShowVictory;
        }

        void OnDisable()
        {
            if (TierManager.Instance == null) return;
            TierManager.Instance.OnTierAdvanced -= ShowTierComplete;
            TierManager.Instance.OnRunCleared   -= ShowVictory;
        }

        private void ShowTierComplete()
        {
            int tier = TierManager.Instance.CurrentTierNumber - 1; // we already advanced
            _resourcesEarned = TierManager.Instance.CurrentTierData.macroResourceReward;
            _message   = $"TIER {tier} COMPLETE!\n+{_resourcesEarned} Resources";
            _isVictory = false;
            Show();
        }

        private void ShowVictory()
        {
            _message   = "VICTORY!\nAll tiers cleared!";
            _isVictory = true;
            Show();
        }

        private void Show()
        {
            _visible       = true;
            Time.timeScale = 0f;
        }

        private void Hide()
        {
            _visible       = false;
            Time.timeScale = 1f;
        }

        void OnGUI()
        {
            if (!_visible) return;

            var screenRect = new Rect(0, 0, Screen.width, Screen.height);
            GUI.color = new Color(0f, 0f, 0f, 0.7f);
            GUI.DrawTexture(screenRect, Texture2D.whiteTexture);
            GUI.color = Color.white;

            var style = new GUIStyle(GUI.skin.label)
            {
                fontSize  = Mathf.RoundToInt(Screen.height * 0.06f),
                alignment = TextAnchor.MiddleCenter,
                wordWrap  = true,
                normal    = { textColor = Color.white }
            };

            GUI.Label(new Rect(0, Screen.height * 0.3f, Screen.width, Screen.height * 0.3f),
                      _message, style);

            string btnText = _isVictory ? "Return to Hub" : "Continue";
            var btnRect = new Rect(Screen.width * 0.35f, Screen.height * 0.62f,
                                   Screen.width * 0.3f,  Screen.height * 0.08f);

            if (GUI.Button(btnRect, btnText))
            {
                Hide();
                if (_isVictory)
                    GameManager.Instance?.ReturnToHub();
            }
        }
    }
}
