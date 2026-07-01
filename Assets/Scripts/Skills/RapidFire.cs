using UnityEngine;
using BulletHeaven.Player;
using BulletHeaven.Weapons;

namespace BulletHeaven.Skills
{
    [CreateAssetMenu(fileName = "RapidFire", menuName = "BulletHeaven/Skills/RapidFire")]
    public class RapidFire : SkillBase
    {
        [SerializeField] private float fireIntervalMultiplier = 0.7f; // -30% fire interval

        public override void ApplyToPlayer(PlayerStatsRuntime runtime)
        {
            var weapon = runtime.GetComponent<WeaponSystem>();
            if (weapon == null) return;

            float current = weapon.FireRateBonus > 0f ? weapon.FireRateBonus : 1f;
            weapon.FireRateBonus = current * fireIntervalMultiplier;
        }
    }
}
