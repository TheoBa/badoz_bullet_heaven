using UnityEngine;
using BulletHeaven.Core;
using BulletHeaven.Player;

namespace BulletHeaven.Enemies
{
    public class MacroResourcePickup : MonoBehaviour
    {
        private GameObjectPool     _pool;
        private PlayerStatsRuntime _playerStats;
        private Transform          _player;

        public void Init(GameObjectPool pool)
        {
            _pool = pool;
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
            float dist   = Vector2.Distance(transform.position, _player.position);
            float radius = _playerStats != null ? _playerStats.PickupRadius : 2f;

            if (dist <= radius)
            {
                transform.position = Vector2.MoveTowards(
                    transform.position, _player.position, 8f * Time.deltaTime);
            }

            if (dist < 0.2f) Collect();
        }

        private void Collect()
        {
            GameManager.Instance?.RunData.AddResource(1);
            _pool?.Release(gameObject);
        }
    }
}
