using UnityEngine;
using BulletHeaven.Core;
using BulletHeaven.Player;

namespace BulletHeaven.Weapons
{
    public class WeaponSystem : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameObjectPool bulletPool;

        [Header("Weapon Config")]
        [SerializeField] private float fireInterval   = 0.5f;
        [SerializeField] private float bulletSpeed    = 12f;
        [SerializeField] private float bulletRange    = 14f;
        [SerializeField] private float baseDamage     = 10f;
        [SerializeField] private float detectionRange = 15f;
        [SerializeField] private LayerMask enemyLayerMask;

        // Run-skill modifiers (set by skills at level-up)
        public int   ExtraPierces   { get; set; }
        public float FireRateBonus  { get; set; } // multiplicative, e.g. 0.7 = 30% faster

        private float              _timer;
        private PlayerStatsRuntime _playerStats;
        private Collider2D[]       _overlapBuffer = new Collider2D[32];

        void Awake()
        {
            _playerStats = GetComponent<PlayerStatsRuntime>();
            if (enemyLayerMask == 0)
                enemyLayerMask = 1 << 7; // Enemy layer fallback — inspector value unreliable across prefab ops
        }

        void Update()
        {
            if (_playerStats != null && _playerStats.IsDead) return;
            var gm = GameManager.Instance;
            if (gm != null && gm.CurrentState == GameState.GameOver) return;

            _timer += Time.deltaTime;

            float interval = fireInterval * (FireRateBonus > 0f ? FireRateBonus : 1f);
            if (_timer >= interval)
            {
                _timer = 0f;
                TryFire();
            }
        }

        private void TryFire()
        {
            var nearest = FindNearestEnemy();
            if (nearest == null) return;

            Vector2 dir = (nearest.position - transform.position).normalized;
            SpawnBullet(dir);
        }

        public void SpawnBullet(Vector2 direction)
        {
            if (bulletPool == null) return;

            var go     = bulletPool.Get();
            go.transform.position = transform.position;
            go.transform.rotation = Quaternion.identity;

            float dmg = baseDamage * (_playerStats != null ? _playerStats.DamageMultiplier : 1f);
            go.GetComponent<Bullet>().Init(bulletPool, direction, bulletSpeed, dmg, bulletRange, ExtraPierces);
        }

        private Transform FindNearestEnemy()
        {
            int count = Physics2D.OverlapCircleNonAlloc(
                transform.position, detectionRange, _overlapBuffer, enemyLayerMask);

            Transform nearest = null;
            float minDist     = float.MaxValue;

            for (int i = 0; i < count; i++)
            {
                float dist = Vector2.Distance(transform.position, _overlapBuffer[i].transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    nearest = _overlapBuffer[i].transform;
                }
            }
            return nearest;
        }

#if UNITY_EDITOR
        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, detectionRange);
        }
#endif
    }
}
