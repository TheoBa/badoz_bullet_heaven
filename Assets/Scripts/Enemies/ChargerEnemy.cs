using UnityEngine;

namespace BulletHeaven.Enemies
{
    // Fast, low HP, periodic dash at the player
    public class ChargerEnemy : EnemyBase
    {
        [SerializeField] private float dashSpeed    = 14f;
        [SerializeField] private float dashDuration = 0.25f;
        [SerializeField] private float dashCooldown = 3f;

        private enum State { Chase, WindUp, Dashing, Cooldown }

        private State  _state        = State.Chase;
        private float  _stateTimer;
        private Vector2 _dashDir;

        protected override void Awake()
        {
            base.Awake();
            maxHP         = 15f;
            moveSpeed     = 3.5f;
            contactDamage = 12f;
            xpValue       = 8f;
        }

        protected override void FixedUpdate()
        {
            if (PlayerTransform == null) return;

            _stateTimer -= Time.fixedDeltaTime;

            switch (_state)
            {
                case State.Chase:
                    base.MoveTowardPlayer();
                    if (_stateTimer <= 0f) EnterWindUp();
                    break;

                case State.WindUp:
                    Rb.linearVelocity = Vector2.zero;
                    if (_stateTimer <= 0f) EnterDash();
                    break;

                case State.Dashing:
                    Rb.linearVelocity = _dashDir * dashSpeed;
                    if (_stateTimer <= 0f) EnterCooldown();
                    break;

                case State.Cooldown:
                    Rb.linearVelocity = Vector2.zero;
                    if (_stateTimer <= 0f) EnterChase();
                    break;
            }
        }

        private void EnterChase()
        {
            _state      = State.Chase;
            _stateTimer = dashCooldown * 0.5f;
        }

        private void EnterWindUp()
        {
            _state      = State.WindUp;
            _stateTimer = 0.4f;
            _dashDir    = (PlayerTransform.position - transform.position).normalized;
        }

        private void EnterDash()
        {
            _state      = State.Dashing;
            _stateTimer = dashDuration;
        }

        private void EnterCooldown()
        {
            _state      = State.Cooldown;
            _stateTimer = dashCooldown;
        }

        protected override void Start()
        {
            base.Start();
            _stateTimer = dashCooldown; // start in chase, first dash after cooldown
        }
    }
}
