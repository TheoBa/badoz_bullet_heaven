using UnityEngine;
using BulletHeaven.Core;

namespace BulletHeaven.UI
{
    // First screen loaded at startup.
    public class MainMenuUI : MonoBehaviour
    {
        // Cached GUIStyles -- OnGUI runs every frame, so allocating these inline would GC every frame.
        private GUIStyle _titleStyle;
        private GUIStyle _btnStyle;

        void OnGUI()
        {
            var screenRect = new Rect(0, 0, Screen.width, Screen.height);
            GUI.color = new Color(0.06f, 0.06f, 0.08f, 1f);
            GUI.DrawTexture(screenRect, Texture2D.whiteTexture);
            GUI.color = Color.white;

            _titleStyle ??= new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontStyle = FontStyle.Bold,
                normal    = { textColor = Color.white }
            };
            _titleStyle.fontSize = Mathf.RoundToInt(Screen.height * 0.08f);
            GUI.Label(new Rect(0, Screen.height * 0.25f, Screen.width, Screen.height * 0.2f),
                      "BADOZ BULLET HEAVEN", _titleStyle);

            _btnStyle ??= new GUIStyle(GUI.skin.button);
            _btnStyle.fontSize = Mathf.RoundToInt(Screen.height * 0.04f);
            var btnStyle = _btnStyle;
            float btnW = Screen.width * 0.25f;
            float btnH = Screen.height * 0.08f;
            float btnX = (Screen.width - btnW) * 0.5f;

            if (GUI.Button(MobileUI.EnsureMinSize(new Rect(btnX, Screen.height * 0.55f, btnW, btnH)), "Play", btnStyle))
                GameManager.Instance?.ReturnToHub();

            if (GUI.Button(MobileUI.EnsureMinSize(new Rect(btnX, Screen.height * 0.68f, btnW, btnH)), "Quit", btnStyle))
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
            }
        }
    }
}
