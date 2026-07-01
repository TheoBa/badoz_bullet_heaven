using UnityEngine;
using BulletHeaven.Core;

namespace BulletHeaven.Enemies
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(CircleCollider2D))]
    public class EnemyBase : MonoBehaviour, IDamageable
    {
        [Header("Stats")]
        [SerializeField] protected float maxHP           = 30f;
        [SerializeField] protected float moveSpeed       = 2.5f;
        [SerializeField] protected float contactDamage   = 10f;
        [SerializeField] protected float xpValue         = 5f;
        [SerializeField] protected float resourceDropChance = 0.02f;

        public float MaxHP           => maxHP;
        public float XPValue         => xpValue;
        public float ResourceDropChance => resourceDropChance;

        protected float       CurrentHP;
        protected Transform   PlayerTransform;
        protected Rigidbody2D Rb;
        private   bool        _isDead;

        protected virtual void Awake()
        {
            Rb         = GetComponent<Rigidbody2D>();
            Rb.gravityScale   = 0f;
            Rb.freezeRotation = true;
            Rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        }

        protected virtual void Start()
        {
            CurrentHP = maxHP;
            var playerGO = GameObject.FindGameObjectWithTag("Player");
            if (playerGO != null) PlayerTransform = playerGO.transform;
        }

        protected virtual void FixedUpdate()
        {
            if (_isDead || PlayerTransform == null) return;
            MoveTowardPlayer();
        }

        protected virtual void MoveTowardPlayer()
        {
            Vector2 dir = (PlayerTransform.position - transform.position).normalized;
            Rb.linearVelocity = dir * moveSpeed;
        }

        public virtual bool TakeDamage(float amount)
        {
            if (_isDead) return false;
            CurrentHP -= amount;
            if (CurrentHP <= 0f) { Die(); return true; }
            return false;
        }

        protected virtual void Die()
        {
            _isDead           = true;
            Rb.linearVelocity = Vector2.zero;
            OnDeath();
            gameObject.SetActive(false);
        }

        // Override in subclasses for special death FX; base notifies managers
        protected virtual void OnDeath()
        {
            GameManager.Instance?.RunData.AddKill();
            DropManager.Instance?.OnEnemyDied(this);
        }

        void OnCollisionStay2D(Collision2D col)
        {
            if (_isDead) return;
            var player = col.gameObject.GetComponent<BulletHeaven.Player.PlayerStatsRuntime>();
            player?.ApplyDamage(contactDamage * Time.fixedDeltaTime);
        }

        // Scale stats for higher tiers (called by WaveSpawner)
        public virtual void ApplyTierScaling(float hpMult, float speedMult, float dmgMult)
        {
            maxHP         *= hpMult;
            moveSpeed     *= speedMult;
            contactDamage *= dmgMult;
            CurrentHP      = maxHP;
        }
    }
}
