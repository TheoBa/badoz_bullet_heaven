using System;
using UnityEngine;
using BulletHeaven.Enemies;

namespace BulletHeaven.Core
{
    public class TierManager : MonoBehaviour
    {
        public static TierManager Instance { get; private set; }

        [SerializeField] private TierConfig config;

        public event Action OnTierAdvanced;

        private int _tierIndex;

        public int                  CurrentTierNumber => _tierIndex + 1;
        public TierConfig.TierData  CurrentTierData   => config.tiers[_tierIndex];
        public WaveConfig           CurrentWaveConfig => CurrentTierData.waveConfig;
        public bool                 IsLastTier        => _tierIndex >= config.tiers.Length - 1;

        void Awake()
        {
            if (Instance != null) { Destroy(gameObject); return; }
            Instance = this;
        }

        void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        public void ApplyTierScaling(EnemyBase enemy)
        {
            var t = CurrentTierData;
            enemy.ApplyTierScaling(t.hpMult, t.speedMult, t.dmgMult);
        }

        public void OnBossDefeated()
        {
            GameManager.Instance?.RunData.AddResource(CurrentTierData.macroResourceReward);

            if (IsLastTier)
            {
                GameManager.Instance?.EndRun(RunResult.FullClear);
            }
            else
            {
                _tierIndex++;
                GameManager.Instance?.RunData.IncrementTier();
                OnTierAdvanced?.Invoke();
            }
        }
    }
}
