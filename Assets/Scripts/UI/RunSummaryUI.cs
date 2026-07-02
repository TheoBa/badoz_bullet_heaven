using UnityEngine;
using BulletHeaven.Core;

namespace BulletHeaven.UI
{
    // Shown on any run end (death or full clear) via GameManager's GameOver state,
    // so it works regardless of which system triggered EndRun.
    public class RunSummaryUI : MonoBehaviour
    {
        private bool _visible;

        void Start()
        {
            if (GameManager.Instance == null) return;
            GameManager.Instance.OnStateChanged += HandleStateChanged;
        }

        void OnDestroy()
        {
            if (GameManager.Instance == null) return;
            GameManager.Instance.OnStateChanged -= HandleStateChanged;
        }

        private void HandleStateChanged(GameState state)
        {
            if (state != GameState.GameOver)
            {
                _visible = false;
                return;
            }
            _visible       = true;
            Time.timeScale = 0f;
        }

        void OnGUI()
        {
            if (!_visible) return;
            var gm      = GameManager.Instance;
            var runData = gm.RunData;

            var screenRect = new Rect(0, 0, Screen.width, Screen.height);
            GUI.color = new Color(0f, 0f, 0f, 0.85f);
            GUI.DrawTexture(screenRect, Texture2D.whiteTexture);
            GUI.color = Color.white;

            var titleStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize  = Mathf.RoundToInt(Screen.height * 0.06f),
                alignment = TextAnchor.MiddleCenter,
                normal    = { textColor = Color.white }
            };
            string title = gm.LastRunResult == RunResult.FullClear ? "VICTORY!" : "RUN OVER";
            GUI.Label(new Rect(0, Screen.height * 0.15f, Screen.width, Screen.height * 0.15f), title, titleStyle);

            var statStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize  = Mathf.RoundToInt(Screen.height * 0.035f),
                alignment = TextAnchor.MiddleCenter,
                wordWrap  = true,
                normal    = { textColor = Color.white }
            };

            int minutes = Mathf.FloorToInt(runData.ElapsedTime / 60f);
            int seconds = Mathf.FloorToInt(runData.ElapsedTime % 60f);
            string stats =
                $"Resources Earned: {runData.ResourcesEarnedThisRun}\n" +
                $"Enemies Killed: {runData.EnemiesKilled}\n" +
                $"Waves Survived: {runData.CurrentWave}\n" +
                $"Time: {minutes:00}:{seconds:00}";

            GUI.Label(new Rect(0, Screen.height * 0.35f, Screen.width, Screen.height * 0.3f), stats, statStyle);

            var btnStyle = new GUIStyle(GUI.skin.button) { fontSize = Mathf.RoundToInt(Screen.height * 0.035f) };
            var btnRect  = MobileUI.EnsureMinSize(new Rect(Screen.width * 0.35f, Screen.height * 0.7f, Screen.width * 0.3f, Screen.height * 0.08f));
            if (GUI.Button(btnRect, "Return to Hub"))
            {
                _visible       = false;
                Time.timeScale = 1f;
                gm.ReturnToHub();
            }
        }
    }
}
