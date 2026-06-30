using System;
using UnityEngine;

namespace BulletHeaven.Enemies
{
    [CreateAssetMenu(menuName = "BulletHeaven/WaveConfig")]
    public class WaveConfig : ScriptableObject
    {
        [Serializable]
        public class EnemyEntry
        {
            public GameObject prefab;
            [Range(1, 10)] public int weight = 1;
        }

        [Header("Enemy Pool")]
        public EnemyEntry[] enemies;

        [Header("Wave Sizing")]
        public int   baseCount          = 5;
        public float countScalePerWave  = 1.5f; // enemies added per wave

        [Header("Timing")]
        public float spawnInterval    = 0.5f;  // seconds between individual enemy spawns
        public float timeBetweenWaves = 10f;   // rest time after a wave completes
        public float initialDelay     = 2f;    // before the very first wave

        [Header("Positioning")]
        public float spawnRadius = 14f; // distance from player to spawn at

        public int GetEnemyCount(int wave) =>
            Mathf.Max(1, Mathf.RoundToInt(baseCount + (wave - 1) * countScalePerWave));
    }
}
