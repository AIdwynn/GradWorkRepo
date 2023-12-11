
using System.Collections.Generic;
using UnityEngine;

namespace Vital.ObjectPools
{
    public class ObjectPool<T>
        where T : MonoBehaviour
    {
        private Queue<T> _pool = new Queue<T>();
        private bool _canGrow = true;
        private T _original;
        private Transform _parent;
        
        public  Queue<T> Pool
        {
            get { return _pool; }
        }

        public ObjectPool(bool canGrow, T original)
        {
            _canGrow = canGrow;
            _original = original;
        }

        public static ObjectPool<Y> CreateObjectPool<Y>(Y original, int initialPoolSize, bool canGrow,
            Transform parent)
            where Y : MonoBehaviour
        {
            ObjectPool<Y> objectPool = new ObjectPool<Y>(canGrow, original);
            objectPool.CreateParent(parent, original.name);
            objectPool.FillPool(initialPoolSize);
            return objectPool;
        }

        private void CreateParent(Transform parent, string name)
        {
            _parent = new GameObject("Pool" + name).transform;
            _parent.parent = parent;
        }

        private void FillPool(int initialPoolSize)
        {
            for (int i = 0; i < initialPoolSize; i++)
            {
                _pool.Enqueue(InstantiateOriginal());
            }
        }

        private T InstantiateOriginal()
        {
            var obj = GameObject.Instantiate(_original, _parent);

            obj.gameObject.SetActive(false);


            return obj;
        }

        public bool TryGet(out T poolableObject)
        {
            if (_pool.Count == 0)
            {
                if (_canGrow)
                {
                    poolableObject = InstantiateOriginal();
                    poolableObject.gameObject.SetActive(true);
                    return true;
                }

                poolableObject = null;
                return false;
            }

            poolableObject = _pool.Dequeue();
            poolableObject.gameObject.SetActive(true);
            return true;
        }

        public bool TryReturn(T poolableObject)
        {
            if (poolableObject == null)
            {
                return false;
            }

            poolableObject.gameObject.SetActive(false);
            _pool.Enqueue(poolableObject);
            return true;
        }
    }
}