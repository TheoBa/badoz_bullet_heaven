using UnityEngine;
using BulletHeaven.Core;

namespace BulletHeaven.UI
{
    // Shown while GameManager.CurrentState == Paused (toggled by the Pause input action).
    public class PauseMenuUI : MonoBehaviour
    {
        private bool  _visible;
        private float _volume = 1f;

        // Cached GUIStyles -- OnGUI runs every frame, so allocating these inline would GC every frame.
        private GUIStyle _titleStyle;
        private GUIStyle _labelStyle;
        private GUIStyle _btnStyle;

        void Start()
        {
            if (GameManager.Instance == null) return;
            GameManager.Instance.OnStateChanged += HandleStateChanged;
            _volume = AudioListener.volume;
        }

        void OnDestroy()
        {
            if (GameManager.Instance == null) return;
            GameManager.Instance.OnStateChanged -= HandleStateChanged;
        }

        private void HandleStateChanged(GameState state) => _visible = state == GameState.Paused;

        void OnGUI()
        {
            if (!_visible) return;

            var screenRect = new Rect(0, 0, Screen.width, Screen.height);
            GUI.color = new Color(0f, 0f, 0f, 0.8f);
            GUI.DrawTexture(screenRect, Texture2D.whiteTexture);
            GUI.color = Color.white;

            _titleStyle ??= new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, normal = { textColor = Color.white } };
            _titleStyle.fontSize = Mathf.RoundToInt(Screen.height * 0.05f);
            GUI.Label(new Rect(0, Screen.height * 0.15f, Screen.width, Screen.height * 0.12f), "PAUSED", _titleStyle);

            _labelStyle ??= new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, normal = { textColor = Color.white } };
            _labelStyle.fontSize = Mathf.RoundToInt(Screen.height * 0.025f);
            var labelStyle = _labelStyle;

            _btnStyle ??= new GUIStyle(GUI.skin.button);
            _btnStyle.fontSize = Mathf.RoundToInt(Screen.height * 0.03f);
            var btnStyle = _btnStyle;

            float btnW = Screen.width * 0.25f;
            float btnH = Screen.height * 0.07f;
            float btnX = (Screen.width - btnW) * 0.5f;

            if (GUI.Button(MobileUI.EnsureMinSize(new Rect(btnX, Screen.height * 0.35f, btnW, btnH)), "Resume", btnStyle))
                GameManager.Instance?.ResumeGame();

            if (GUI.Button(MobileUI.EnsureMinSize(new Rect(btnX, Screen.height * 0.45f, btnW, btnH)), "Abandon Run", btnStyle))
            {
                // No EndRun() call -> this run's earned resources are forfeited, per design.
                Time.timeScale = 1f;
                GameManager.Instance?.ReturnToHub();
            }

            GUI.Label(new Rect(btnX, Screen.height * 0.58f, btnW, Screen.height * 0.04f), "Volume", labelStyle);
            float newVolume = GUI.HorizontalSlider(new Rect(btnX, Screen.height * 0.63f, btnW, Screen.height * 0.03f), _volume, 0f, 1f);
            if (!Mathf.Approximately(newVolume, _volume))
            {
                _volume            = newVolume;
                AudioListener.volume = _volume;
            }
        }
    }
}
