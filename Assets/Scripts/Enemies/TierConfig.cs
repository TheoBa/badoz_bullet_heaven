using System;
using UnityEngine;

namespace BulletHeaven.Enemies
{
    [CreateAssetMenu(menuName = "BulletHeaven/TierConfig")]
    public class TierConfig : ScriptableObject
    {
        [Serializable]
        public class TierData
        {
            public WaveConfig  waveConfig;
            public GameObject  bossPrefab;
            [Tooltip("Spawn a boss every N waves within this tier")]
            public int         bossEveryNWaves      = 5;
            public int         macroResourceReward  = 10;
            [Header("Enemy stat multipliers for this tier")]
            public float       hpMult               = 1f;
            public float       speedMult            = 1f;
            public float       dmgMult              = 1f;
        }

        public TierData[] tiers; // index 0 = tier 1
    }
}
