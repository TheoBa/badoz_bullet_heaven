using UnityEngine;
using BulletHeaven.Player;

namespace BulletHeaven.Skills
{
    [CreateAssetMenu(fileName = "SpeedBurst", menuName = "BulletHeaven/Skills/SpeedBurst")]
    public class SpeedBurst : SkillBase
    {
        [SerializeField] private float speedPercent = 0.2f; // +20% move speed

        public override void ApplyToPlayer(PlayerStatsRuntime runtime) =>
            runtime.AddPercentBonus(StatType.Speed, speedPercent);
    }
}
