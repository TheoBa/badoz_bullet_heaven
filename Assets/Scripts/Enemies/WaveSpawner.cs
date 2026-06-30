using System.Collections;
using UnityEngine;
using BulletHeaven.Core;

namespace BulletHeaven.Enemies
{
    public class WaveSpawner : MonoBehaviour
    {
        [SerializeField] private WaveConfig config;

        private int   _wave;
        private float _timer;
        private bool  _spawning;

        void Start()
        {
            _timer = config != null ? config.initialDelay : 2f;
        }

        void Update()
        {
            if (config == null || _spawning) return;

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

            int count = config.GetEnemyCount(_wave);

            for (int i = 0; i < count; i++)
            {
                SpawnOneEnemy();
                yield return new WaitForSeconds(config.spawnInterval);
            }

            GameManager.Instance?.RunData.IncrementWave();
            _timer    = config.timeBetweenWaves;
            _spawning = false;
        }

        private void SpawnOneEnemy()
        {
            var entry = PickWeightedRandom();
            if (entry?.prefab == null) return;

            Vector2 center = GetPlayerPosition();
            float   angle  = Random.Range(0f, Mathf.PI * 2f);
            Vector2 pos    = center + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * config.spawnRadius;

            Instantiate(entry.prefab, pos, Quaternion.identity);
        }

        private Vector2 GetPlayerPosition()
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            return p != null ? (Vector2)p.transform.position : Vector2.zero;
        }

        private WaveConfig.EnemyEntry PickWeightedRandom()
        {
            if (config.enemies == null || config.enemies.Length == 0) return null;

            int total = 0;
            foreach (var e in config.enemies) total += e.weight;

            int roll = Random.Range(0, total);
            int cum  = 0;
            foreach (var e in config.enemies)
            {
                cum += e.weight;
                if (roll < cum) return e;
            }
            return config.enemies[0];
        }

#if UNITY_EDITOR
        void OnDrawGizmosSelected()
        {
            if (config == null) return;
            Gizmos.color = new Color(1f, 0.5f, 0f, 0.4f);
            Gizmos.DrawWireSphere(GetPlayerPosition(), config.spawnRadius);
        }
#endif
    }
}
