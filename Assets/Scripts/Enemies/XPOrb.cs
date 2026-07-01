using UnityEngine;
using BulletHeaven.Core;
using BulletHeaven.Player;

namespace BulletHeaven.Enemies
{
    public class XPOrb : MonoBehaviour
    {
        private GameObjectPool _pool;
        private float          _xpValue;
        private float          _magnetSpeed = 8f;
        private Transform      _player;
        private PlayerStatsRuntime _playerStats;
        private bool           _attracted;

        public void Init(GameObjectPool pool, float xpValue)
        {
            _pool    = pool;
            _xpValue = xpValue;
            _attracted = false;

            var playerGO = GameObject.FindGameObjectWithTag("Player");
            if (playerGO != null)
            {
                _player      = playerGO.transform;
                _playerStats = playerGO.GetComponent<PlayerStatsRuntime>();
            }
        }

        void Update()
        {
            if (_player == null) return;

            float dist = Vector2.Distance(transform.position, _player.position);
            float radius = _playerStats != null ? _playerStats.PickupRadius : 2f;

            if (dist <= radius || _attracted)
            {
                _attracted = true;
                transform.position = Vector2.MoveTowards(
                    transform.position, _player.position, _magnetSpeed * Time.deltaTime);
            }

            if (dist < 0.2f) Collect();
        }

        private void Collect()
        {
            float gain = _xpValue * (_playerStats != null ? _playerStats.XPGainMultiplier : 1f);
            XPManager.Instance?.AddXP(gain);
            _pool?.Release(gameObject);
        }
    }
}
