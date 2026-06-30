using UnityEngine;
using UnityEngine.Pool;

namespace BulletHeaven.Core
{
    public class GameObjectPool : MonoBehaviour
    {
        [SerializeField] private GameObject prefab;
        [SerializeField] private int defaultCapacity = 20;
        [SerializeField] private int maxSize         = 100;

        private ObjectPool<GameObject> _pool;

        public GameObject Prefab => prefab;

        void Awake()
        {
            _pool = new ObjectPool<GameObject>(
                createFunc:      CreateObj,
                actionOnGet:     obj => obj.SetActive(true),
                actionOnRelease: obj => obj.SetActive(false),
                actionOnDestroy: obj => Destroy(obj),
                collectionCheck: false,
                defaultCapacity: defaultCapacity,
                maxSize:         maxSize
            );
        }

        public GameObject Get()                   => _pool.Get();
        public void       Release(GameObject obj) => _pool.Release(obj);

        private GameObject CreateObj()
        {
            var obj = Instantiate(prefab);
            obj.SetActive(false);
            return obj;
        }
    }
}
