using UnityEngine;
using BulletHeaven.Player;
using BulletHeaven.Weapons;

namespace BulletHeaven.Skills
{
    [CreateAssetMenu(fileName = "PiercingShot", menuName = "BulletHeaven/Skills/PiercingShot")]
    public class PiercingShot : SkillBase
    {
        [SerializeField] private int extraPierces = 2;

        public override void ApplyToPlayer(PlayerStatsRuntime runtime)
        {
            var weapon = runtime.GetComponent<WeaponSystem>();
            if (weapon != null) weapon.ExtraPierces += extraPierces;
        }
    }
}
