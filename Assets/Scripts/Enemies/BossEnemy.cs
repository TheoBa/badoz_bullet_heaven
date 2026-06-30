using UnityEngine;
using BulletHeaven.Core;
using BulletHeaven.Player;

namespace BulletHeaven.Enemies
{
    public class BossEnemy : EnemyBase
    {
        [Header("Shockwave Attack")]
        [SerializeField] private float shockwaveInterval = 8f;
        [SerializeField] private float shockwaveRadius   = 4f;
        [SerializeField] private float shockwaveDamage   = 25f;

        private float _shockwaveTimer;

        protected override void Start()
        {
            base.Start();
            _shockwaveTimer = shockwaveInterval;
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();

            _shockwaveTimer -= Time.fixedDeltaTime;
            if (_shockwaveTimer <= 0f)
            {
                _shockwaveTimer = shockwaveInterval;
                DoShockwave();
            }
        }

        private void DoShockwave()
        {
            var hits = Physics2D.OverlapCircleAll(transform.position, shockwaveRadius, 1 << 6);
            foreach (var hit in hits)
                hit.GetComponent<PlayerStatsRuntime>()?.ApplyDamage(shockwaveDamage);
        }

        protected override void OnDeath()
        {
            base.OnDeath();
            TierManager.Instance?.OnBossDefeated();
        }

#if UNITY_EDITOR
        void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(1f, 0f, 0f, 0.25f);
            Gizmos.DrawWireSphere(transform.position, shockwaveRadius);
        }
#endif
    }
}
