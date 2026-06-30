using UnityEngine;
using BulletHeaven.Core;

namespace BulletHeaven.Enemies
{
    // Singleton that handles XP orb and macro resource spawning on enemy death
    public class DropManager : MonoBehaviour
    {
        public static DropManager Instance { get; private set; }

        [Header("Pools")]
        [SerializeField] private GameObjectPool xpOrbPool;
        [SerializeField] private GameObjectPool macroResourcePool;

        void Awake()
        {
            if (Instance != null) { Destroy(gameObject); return; }
            Instance = this;
        }

        public void OnEnemyDied(EnemyBase enemy)
        {
            SpawnXPOrb(enemy.transform.position, enemy.XPValue);

            if (Random.value < enemy.ResourceDropChance)
                SpawnMacroResource(enemy.transform.position);
        }

        private void SpawnXPOrb(Vector3 pos, float xpValue)
        {
            if (xpOrbPool == null) return;
            var go  = xpOrbPool.Get();
            go.transform.position = pos;
            go.GetComponent<XPOrb>()?.Init(xpOrbPool, xpValue);
        }

        private void SpawnMacroResource(Vector3 pos)
        {
            if (macroResourcePool == null) return;
            var go = macroResourcePool.Get();
            go.transform.position = pos;
            go.GetComponent<MacroResourcePickup>()?.Init(macroResourcePool);
        }
    }
}
