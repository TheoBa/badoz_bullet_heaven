using UnityEngine;
using BulletHeaven.Player;

namespace BulletHeaven.Skills
{
    [CreateAssetMenu(fileName = "Shield", menuName = "BulletHeaven/Skills/Shield")]
    public class Shield : SkillBase
    {
        public override void ApplyToPlayer(PlayerStatsRuntime runtime) => runtime.EnableShield();
    }
}
