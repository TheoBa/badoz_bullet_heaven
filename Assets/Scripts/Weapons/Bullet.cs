using System;
using UnityEngine;
using BulletHeaven.Core;

namespace BulletHeaven.Weapons
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(CircleCollider2D))]
    public class Bullet : MonoBehaviour
    {
        private GameObjectPool _pool;
        private Vector2        _direction;
        private float          _speed;
        private float          _damage;
        private float          _maxRange;
        private Vector2        _startPos;
        private int            _piercesRemaining;
        private Action<float>   _onHit;
        private Action<Vector2> _onKill;

        private Rigidbody2D _rb;

        void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _rb.gravityScale   = 0f;
            _rb.freezeRotation = true;

            var col = GetComponent<CircleCollider2D>();
            col.isTrigger = true;
        }

        public void Init(GameObjectPool pool, Vector2 direction, float speed,
                         float damage, float maxRange, int pierces = 0,
                         Action<float> onHit = null, Action<Vector2> onKill = null)
        {
            _pool             = pool;
            _direction        = direction.normalized;
            _speed            = speed;
            _damage           = damage;
            _maxRange         = maxRange;
            _startPos         = transform.position;
            _piercesRemaining = pierces;
            _onHit            = onHit;
            _onKill           = onKill;
            _rb.linearVelocity = _direction * _speed;
        }

        void OnEnable()
        {
            // Reset velocity when re-used from pool (Init re-sets it)
            if (_rb != null) _rb.linearVelocity = Vector2.zero;
        }

        void Update()
        {
            if (Vector2.Distance(transform.position, _startPos) >= _maxRange)
                ReturnToPool();
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            var target = other.GetComponent<IDamageable>();
            if (target == null) return;

            Vector2 hitPos = transform.position;
            bool    killed = target.TakeDamage(_damage);
            _onHit?.Invoke(_damage);
            if (killed) _onKill?.Invoke(hitPos);

            if (_piercesRemaining > 0)
                _piercesRemaining--;
            else
                ReturnToPool();
        }

        public void ReturnToPool()
        {
            _rb.linearVelocity = Vector2.zero;
            if (_pool != null)
                _pool.Release(gameObject);
            else
                gameObject.SetActive(false);
        }
    }
}
