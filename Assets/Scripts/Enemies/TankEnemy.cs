using UnityEngine;

namespace BulletHeaven.Enemies
{
    // Slow, high HP, high contact damage
    public class TankEnemy : EnemyBase
    {
        protected override void Awake()
        {
            base.Awake();
            maxHP         = 100f;
            moveSpeed     = 1.2f;
            contactDamage = 20f;
            xpValue       = 15f;
        }
    }
}
