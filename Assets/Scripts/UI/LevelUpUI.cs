using UnityEngine;
using BulletHeaven.Core;

namespace BulletHeaven.UI
{
    // Placeholder pause screen for level-up. Skill selection replaces the Continue
    // button once feature/skills + feature/skill-pick-ui land.
    public class LevelUpUI : MonoBehaviour
    {
        private bool _visible;
        private int  _displayLevel;
        private int  _pendingCount;

        void Start()
        {
            // Start (not OnEnable) so this runs after every object's Awake() —
            // guarantees XPManager.Instance is already assigned regardless of hierarchy order.
            if (XPManager.Instance == null) return;
            XPManager.Instance.OnLevelUp += HandleLevelUp;
        }

        void OnDestroy()
        {
            if (XPManager.Instance == null) return;
            XPManager.Instance.OnLevelUp -= HandleLevelUp;
        }

        private void HandleLevelUp(int level)
        {
            _displayLevel = level;
            _pendingCount++;
            if (!_visible) Show();
        }

        private void Show()
        {
            _visible       = true;
            Time.timeScale = 0f;
        }

        private void Continue()
        {
            _pendingCount--;
            if (_pendingCount > 0) return; // more level-ups queued from this XP gain

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
                      $"LEVEL {_displayLevel}!", style);

            var btnRect = new Rect(Screen.width * 0.35f, Screen.height * 0.62f,
                                   Screen.width * 0.3f,  Screen.height * 0.08f);

            if (GUI.Button(btnRect, "Continue"))
                Continue();
        }
    }
}
