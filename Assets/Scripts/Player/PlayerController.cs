using UnityEngine;
using BulletHeaven.Core;

namespace BulletHeaven.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(PlayerStatsRuntime))]
    public class PlayerController : MonoBehaviour
    {
        private Rigidbody2D      _rb;
        private PlayerStatsRuntime _stats;

        void Awake()
        {
            _rb    = GetComponent<Rigidbody2D>();
            _stats = GetComponent<PlayerStatsRuntime>();

            _rb.gravityScale  = 0f;
            _rb.freezeRotation = true;
        }

        void FixedUpdate()
        {
            if (_stats.IsDead) { _rb.linearVelocity = Vector2.zero; return; }

            Vector2 input = InputReader.Instance != null
                ? InputReader.Instance.MoveInput
                : Vector2.zero;

            _rb.linearVelocity = input.normalized * _stats.Speed;
        }
    }
}
