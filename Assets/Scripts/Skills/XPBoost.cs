using UnityEngine;
using BulletHeaven.Player;

namespace BulletHeaven.Skills
{
    [CreateAssetMenu(fileName = "XPBoost", menuName = "BulletHeaven/Skills/XPBoost")]
    public class XPBoost : SkillBase
    {
        [SerializeField] private float xpGainPercent = 0.3f; // +30% XP gain

        public override void ApplyToPlayer(PlayerStatsRuntime runtime) =>
            runtime.AddPercentBonus(StatType.XPGainMultiplier, xpGainPercent);
    }
}
