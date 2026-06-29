using System.Collections.Generic;
using UnityEngine;
using BulletHeaven.Core;

namespace BulletHeaven.Player
{
    public class PlayerStatsRuntime : MonoBehaviour
    {
        [SerializeField] private PlayerStats baseStats;

        // Current HP
        public float CurrentHP  { get; private set; }

        // Merged stat values (base + hub bonuses + run skill bonuses)
        public float MaxHP            => baseStats.maxHP            + GetBonus(StatType.MaxHP);
        public float Speed            => baseStats.speed            + GetBonus(StatType.Speed);
        public float DamageMultiplier => baseStats.damageMultiplier + GetBonus(StatType.DamageMultiplier);
        public float XPGainMultiplier => baseStats.xpGainMultiplier + GetBonus(StatType.XPGainMultiplier);
        public float PickupRadius     => baseStats.pickupRadius     + GetBonus(StatType.PickupRadius);

        public bool  IsDead           { get; private set; }

        private readonly Dictionary<StatType, float> _runBonuses = new();

        void Start() => Initialize();

        public void Initialize()
        {
            _runBonuses.Clear();
            IsDead    = false;
            CurrentHP = MaxHP;
        }

        public void AddRunBonus(StatType stat, float value)
        {
            _runBonuses.TryGetValue(stat, out float current);
            _runBonuses[stat] = current + value;
        }

        public void ApplyDamage(float amount)
        {
            if (IsDead) return;
            CurrentHP = Mathf.Max(0f, CurrentHP - amount);
            if (CurrentHP <= 0f) Die();
        }

        public void Heal(float amount)
        {
            if (IsDead) return;
            CurrentHP = Mathf.Min(MaxHP, CurrentHP + amount);
        }

        private float GetBonus(StatType stat)
        {
            _runBonuses.TryGetValue(stat, out float val);
            return val;
        }

        private void Die()
        {
            IsDead = true;
            GameManager.Instance?.EndRun(RunResult.Death);
        }
    }
}
