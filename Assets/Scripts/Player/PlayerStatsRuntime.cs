using System.Collections.Generic;
using UnityEngine;
using BulletHeaven.Core;
using BulletHeaven.Meta;

namespace BulletHeaven.Player
{
    public class PlayerStatsRuntime : MonoBehaviour
    {
        [SerializeField] private PlayerStats      baseStats;
        [SerializeField] private PassiveTreeData  passiveTree;

        // Current HP
        public float CurrentHP  { get; private set; }

        // Merged stat values (base + hub bonuses + run skill bonuses)
        public float MaxHP            => baseStats.maxHP            + GetBonus(StatType.MaxHP);
        public float Speed            => baseStats.speed            + GetBonus(StatType.Speed);
        public float DamageMultiplier => baseStats.damageMultiplier + GetBonus(StatType.DamageMultiplier);
        public float XPGainMultiplier => baseStats.xpGainMultiplier + GetBonus(StatType.XPGainMultiplier);
        public float PickupRadius     => baseStats.pickupRadius     + GetBonus(StatType.PickupRadius);

        public bool  IsDead           { get; private set; }

        // Bespoke skill hooks that don't fit the base+hub+run StatType bonus system.
        public float ArmorFlat     { get; private set; }
        public bool  ShieldEnabled { get; private set; }

        private const float ShieldCooldownDuration = 8f;
        private float        _shieldCooldownRemaining;

        private readonly Dictionary<StatType, float> _runBonuses = new();

        void Start() => Initialize();

        void Update()
        {
            if (_shieldCooldownRemaining > 0f)
                _shieldCooldownRemaining -= Time.deltaTime;
        }

        public void Initialize()
        {
            _runBonuses.Clear();
            IsDead = false;

            if (passiveTree != null && SaveManager.Instance != null)
            {
                foreach (StatType stat in System.Enum.GetValues(typeof(StatType)))
                {
                    float hubBonus = SaveManager.Instance.GetPassiveBonus(passiveTree, stat);
                    if (hubBonus != 0f) AddRunBonus(stat, hubBonus);
                }
            }

            CurrentHP = MaxHP;
        }

        public void AddRunBonus(StatType stat, float value)
        {
            _runBonuses.TryGetValue(stat, out float current);
            _runBonuses[stat] = current + value;
        }

        // Convenience for skills expressed as "+N% of base X" (e.g. +50% pickup radius).
        public void AddPercentBonus(StatType stat, float percent)
        {
            float baseValue = stat switch
            {
                StatType.MaxHP            => baseStats.maxHP,
                StatType.Speed            => baseStats.speed,
                StatType.DamageMultiplier => baseStats.damageMultiplier,
                StatType.XPGainMultiplier => baseStats.xpGainMultiplier,
                StatType.PickupRadius     => baseStats.pickupRadius,
                _                         => 0f
            };
            AddRunBonus(stat, baseValue * percent);
        }

        public void AddArmor(float amount) => ArmorFlat += amount;

        public void EnableShield()
        {
            ShieldEnabled            = true;
            _shieldCooldownRemaining = 0f; // ready immediately on pickup
        }

        public void ApplyDamage(float amount)
        {
            if (IsDead) return;

            if (ShieldEnabled && _shieldCooldownRemaining <= 0f)
            {
                _shieldCooldownRemaining = ShieldCooldownDuration;
                return; // fully absorbed
            }

            float mitigated = Mathf.Max(0f, amount - ArmorFlat);
            CurrentHP = Mathf.Max(0f, CurrentHP - mitigated);
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
