using UnityEngine;
using BulletHeaven.Player;
using BulletHeaven.Weapons;

namespace BulletHeaven.Skills
{
    [CreateAssetMenu(fileName = "Lifesteal", menuName = "BulletHeaven/Skills/Lifesteal")]
    public class Lifesteal : SkillBase
    {
        [SerializeField] private float lifestealFraction = 0.05f; // 5% of damage dealt heals player

        public override void ApplyToPlayer(PlayerStatsRuntime runtime)
        {
            var weapon = runtime.GetComponent<WeaponSystem>();
            if (weapon != null) weapon.LifestealFraction += lifestealFraction;
        }
    }
}
