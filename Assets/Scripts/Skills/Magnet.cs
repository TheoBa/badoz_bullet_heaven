using UnityEngine;
using BulletHeaven.Player;

namespace BulletHeaven.Skills
{
    [CreateAssetMenu(fileName = "Magnet", menuName = "BulletHeaven/Skills/Magnet")]
    public class Magnet : SkillBase
    {
        [SerializeField] private float pickupRadiusPercent = 0.5f; // +50% pickup radius

        public override void ApplyToPlayer(PlayerStatsRuntime runtime) =>
            runtime.AddPercentBonus(StatType.PickupRadius, pickupRadiusPercent);
    }
}
