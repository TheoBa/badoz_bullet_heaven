using UnityEngine;

namespace BulletHeaven.Enemies
{
    // Balanced: medium HP, medium speed — the bread-and-butter enemy
    public class WalkerEnemy : EnemyBase
    {
        protected override void Awake()
        {
            base.Awake();
            maxHP         = 30f;
            moveSpeed     = 2.5f;
            contactDamage = 8f;
            xpValue       = 5f;
        }
    }
}
