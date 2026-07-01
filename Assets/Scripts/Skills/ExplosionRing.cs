using UnityEngine;
using BulletHeaven.Player;
using BulletHeaven.Weapons;

namespace BulletHeaven.Skills
{
    [CreateAssetMenu(fileName = "ExplosionRing", menuName = "BulletHeaven/Skills/ExplosionRing")]
    public class ExplosionRing : SkillBase
    {
        [SerializeField] private float explosionRadius = 2.5f;

        public override void ApplyToPlayer(PlayerStatsRuntime runtime)
        {
            var weapon = runtime.GetComponent<WeaponSystem>();
            if (weapon != null)
                weapon.ExplosionRadius = Mathf.Max(weapon.ExplosionRadius, explosionRadius);
        }
    }
}
