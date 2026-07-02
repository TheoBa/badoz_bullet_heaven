using UnityEngine;
using BulletHeaven.Core;
using BulletHeaven.Enemies;
using BulletHeaven.Player;

namespace BulletHeaven.UI
{
    // Always-on in-run overlay: HP bar, XP bar, wave/tier badge, resource counter, boss-wave flash.
    public class HUDUI : MonoBehaviour
    {
        private const float BossFlashDuration = 2f;

        private PlayerStatsRuntime _player;
        private WaveSpawner        _waveSpawner;
        private float              _bossFlashRemaining;

        void Start()
        {
            var playerGO = GameObject.FindGameObjectWithTag("Player");
            if (playerGO != null) _player = playerGO.GetComponent<PlayerStatsRuntime>();

            _waveSpawner = FindAnyObjectByType<WaveSpawner>();
            if (_waveSpawner != null) _waveSpawner.OnBossWaveStarted += HandleBossWaveStarted;
        }

        void OnDestroy()
        {
            if (_waveSpawner != null) _waveSpawner.OnBossWaveStarted -= HandleBossWaveStarted;
        }

        void Update()
        {
            if (_bossFlashRemaining > 0f)
                _bossFlashRemaining -= Time.deltaTime;
        }

        private void HandleBossWaveStarted() => _bossFlashRemaining = BossFlashDuration;

        void OnGUI()
        {
            var gm = GameManager.Instance;
            if (gm == null || gm.CurrentState == GameState.GameOver) return;

            DrawHPBar();
            DrawXPBar();
            DrawWaveTierBadge(gm);
            DrawResourceCounter(gm);
            if (_bossFlashRemaining > 0f) DrawBossFlash();
        }

        private void DrawHPBar()
        {
            if (_player == null) return;

            float barW = Screen.width * 0.25f;
            float barH = Screen.height * 0.04f;
            var   bgRect = new Rect(Screen.width * 0.02f, Screen.height * 0.02f, barW, barH);

            float pct = _player.MaxHP > 0f ? Mathf.Clamp01(_player.CurrentHP / _player.MaxHP) : 0f;

            GUI.color = new Color(0.15f, 0.15f, 0.15f, 0.85f);
            GUI.DrawTexture(bgRect, Texture2D.whiteTexture);
            GUI.color = new Color(0.8f, 0.15f, 0.15f, 1f);
            GUI.DrawTexture(new Rect(bgRect.x, bgRect.y, bgRect.width * pct, bgRect.height), Texture2D.whiteTexture);
            GUI.color = Color.white;

            var style = new GUIStyle(GUI.skin.label)
            {
                fontSize  = Mathf.RoundToInt(Screen.height * 0.02f),
                alignment = TextAnchor.MiddleCenter,
                normal    = { textColor = Color.white }
            };
            GUI.Label(bgRect, $"HP {Mathf.CeilToInt(_player.CurrentHP)}/{Mathf.CeilToInt(_player.MaxHP)}", style);
        }

        private void DrawXPBar()
        {
            if (XPManager.Instance == null) return;

            float barW = Screen.width * 0.6f;
            float barH = Screen.height * 0.03f;
            var   bgRect = new Rect((Screen.width - barW) * 0.5f, Screen.height * 0.95f, barW, barH);

            float pct = XPManager.Instance.XPToNextLevel > 0f
                ? Mathf.Clamp01(XPManager.Instance.XPIntoLevel / XPManager.Instance.XPToNextLevel)
                : 0f;

            GUI.color = new Color(0.15f, 0.15f, 0.15f, 0.85f);
            GUI.DrawTexture(bgRect, Texture2D.whiteTexture);
            GUI.color = new Color(0.2f, 0.5f, 0.9f, 1f);
            GUI.DrawTexture(new Rect(bgRect.x, bgRect.y, bgRect.width * pct, bgRect.height), Texture2D.whiteTexture);
            GUI.color = Color.white;

            var style = new GUIStyle(GUI.skin.label)
            {
                fontSize  = Mathf.RoundToInt(Screen.height * 0.018f),
                alignment = TextAnchor.MiddleCenter,
                normal    = { textColor = Color.white }
            };
            GUI.Label(bgRect, $"LV {XPManager.Instance.CurrentLevel}", style);
        }

        private void DrawWaveTierBadge(GameManager gm)
        {
            int tier = TierManager.Instance != null ? TierManager.Instance.CurrentTierNumber : gm.RunData.CurrentTier;

            var style = new GUIStyle(GUI.skin.label)
            {
                fontSize  = Mathf.RoundToInt(Screen.height * 0.025f),
                alignment = TextAnchor.UpperRight,
                normal    = { textColor = Color.white }
            };
            var rect = new Rect(Screen.width - Screen.width * 0.27f, Screen.height * 0.02f, Screen.width * 0.25f, Screen.height * 0.08f);
            GUI.Label(rect, $"Tier {tier}   Wave {gm.RunData.CurrentWave}", style);
        }

        private void DrawResourceCounter(GameManager gm)
        {
            var style = new GUIStyle(GUI.skin.label)
            {
                fontSize  = Mathf.RoundToInt(Screen.height * 0.022f),
                alignment = TextAnchor.LowerRight,
                normal    = { textColor = Color.white }
            };
            var rect = new Rect(Screen.width - Screen.width * 0.27f, Screen.height * 0.9f, Screen.width * 0.25f, Screen.height * 0.05f);
            GUI.Label(rect, $"Resources: {gm.RunData.ResourcesEarnedThisRun}", style);
        }

        private void DrawBossFlash()
        {
            float alpha = Mathf.PingPong(_bossFlashRemaining * 4f, 1f);
            GUI.color = new Color(0.8f, 0f, 0f, alpha * 0.3f);
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Texture2D.whiteTexture);
            GUI.color = Color.white;

            var style = new GUIStyle(GUI.skin.label)
            {
                fontSize  = Mathf.RoundToInt(Screen.height * 0.08f),
                alignment = TextAnchor.MiddleCenter,
                fontStyle = FontStyle.Bold,
                normal    = { textColor = new Color(1f, 0.2f, 0.2f) }
            };
            GUI.Label(new Rect(0, Screen.height * 0.4f, Screen.width, Screen.height * 0.2f), "BOSS WAVE", style);
        }
    }
}
