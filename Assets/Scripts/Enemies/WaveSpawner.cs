using System.Collections;
using UnityEngine;
using BulletHeaven.Core;

namespace BulletHeaven.Enemies
{
    public class WaveSpawner : MonoBehaviour
    {
        [Tooltip("Fallback config used when TierManager is not present (solo testing)")]
        [SerializeField] private WaveConfig fallbackConfig;

        public event System.Action OnBossWaveStarted;

        private int   _wave;        // total waves across all tiers
        private int   _waveInTier;  // waves completed within the current tier
        private float _timer;
        private bool  _spawning;
        private bool  _waitingForBoss;

        private WaveConfig ActiveConfig =>
            TierManager.Instance != null ? TierManager.Instance.CurrentWaveConfig : fallbackConfig;

        void Start()
        {
            _timer = ActiveConfig != null ? ActiveConfig.initialDelay : 2f;

            if (TierManager.Instance != null)
                TierManager.Instance.OnTierAdvanced += OnTierAdvanced;
        }

        void OnDestroy()
        {
            if (TierManager.Instance != null)
                TierManager.Instance.OnTierAdvanced -= OnTierAdvanced;
        }

        void Update()
        {
            if (ActiveConfig == null || _spawning || _waitingForBoss) return;

            var gm = GameManager.Instance;
            if (gm != null && gm.CurrentState == GameState.GameOver) return;

            _timer -= Time.deltaTime;
            if (_timer <= 0f)
                StartCoroutine(SpawnWave());
        }

        private IEnumerator SpawnWave()
        {
            _spawning = true;
            _wave++;
            _waveInTier++;

            var cfg          = ActiveConfig;
            var tierMgr      = TierManager.Instance;
            int bossEvery    = tierMgr != null ? tierMgr.CurrentTierData.bossEveryNWaves : int.MaxValue;
            bool isBossWave  = tierMgr != null && _waveInTier % bossEvery == 0;

            if (isBossWave)
            {
                SpawnBoss(tierMgr);
                _waitingForBoss = true;
                OnBossWaveStarted?.Invoke();
            }
            else
            {
                int count = cfg.GetEnemyCount(_wave);
                for (int i = 0; i < count; i++)
                {
                    SpawnOneEnemy(cfg, tierMgr);
                    yield return new WaitForSeconds(cfg.spawnInterval);
                }
                GameManager.Instance?.RunData.IncrementWave();
                _timer = cfg.timeBetweenWaves;
            }

            _spawning = false;
        }

        private void SpawnBoss(TierManager tierMgr)
        {
            var bossPrefab = tierMgr?.CurrentTierData.bossPrefab;
            if (bossPrefab == null) { _waitingForBoss = false; return; }

            Vector2 pos = GetSpawnPosition(ActiveConfig.spawnRadius);
            var go = Instantiate(bossPrefab, pos, Quaternion.identity);

            // Tier scaling applies to boss too
            var boss = go.GetComponent<EnemyBase>();
            if (boss != null) tierMgr.ApplyTierScaling(boss);
        }

        private void SpawnOneEnemy(WaveConfig cfg, TierManager tierMgr)
        {
            var entry = PickWeightedRandom(cfg);
            if (entry?.prefab == null) return;

            Vector2 pos = GetSpawnPosition(cfg.spawnRadius);
            var go      = Instantiate(entry.prefab, pos, Quaternion.identity);

            if (tierMgr != null)
            {
                var enemy = go.GetComponent<EnemyBase>();
                if (enemy != null) tierMgr.ApplyTierScaling(enemy);
            }
        }

        private Vector2 GetSpawnPosition(float radius)
        {
            Vector2 center = GetPlayerPosition();
            float   angle  = Random.Range(0f, Mathf.PI * 2f);
            return center + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
        }

        private Vector2 GetPlayerPosition()
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            return p != null ? (Vector2)p.transform.position : Vector2.zero;
        }

        private WaveConfig.EnemyEntry PickWeightedRandom(WaveConfig cfg)
        {
            if (cfg.enemies == null || cfg.enemies.Length == 0) return null;

            int total = 0;
            foreach (var e in cfg.enemies) total += e.weight;

            int roll = Random.Range(0, total);
            int cum  = 0;
            foreach (var e in cfg.enemies)
            {
                cum += e.weight;
                if (roll < cum) return e;
            }
            return cfg.enemies[0];
        }

        // Called by TierManager when boss dies and tier advances
        private void OnTierAdvanced()
        {
            _waitingForBoss = false;
            _waveInTier     = 0;
            _timer          = ActiveConfig != null ? ActiveConfig.timeBetweenWaves : 10f;
        }

#if UNITY_EDITOR
        void OnDrawGizmosSelected()
        {
            var cfg = ActiveConfig;
            if (cfg == null) return;
            Gizmos.color = new Color(1f, 0.5f, 0f, 0.4f);
            Gizmos.DrawWireSphere(GetPlayerPosition(), cfg.spawnRadius);
        }
#endif
    }
}
