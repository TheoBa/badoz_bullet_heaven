using UnityEngine;
using BulletHeaven.Player;

namespace BulletHeaven.Skills
{
    [CreateAssetMenu(fileName = "Armor", menuName = "BulletHeaven/Skills/Armor")]
    public class Armor : SkillBase
    {
        [SerializeField] private float armorAmount = 2f; // flat damage reduction per hit

        public override void ApplyToPlayer(PlayerStatsRuntime runtime) => runtime.AddArmor(armorAmount);
    }
}
