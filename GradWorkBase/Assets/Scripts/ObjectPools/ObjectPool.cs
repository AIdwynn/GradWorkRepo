using System.Collections.Generic;
using UnityEngine;

namespace Vital.ObjectPools
{
    public class GameObjectPool
    {
        private Queue<GameObject> _pool = new Queue<GameObject>();
        private bool _canGrow = true;
        private GameObject _original;
        private Transform _parent;

        public GameObjectPool(bool canGrow, GameObject original)
        {
            _canGrow = canGrow;
            _original = original;
        }

        public static GameObjectPool CreateObjectPool(GameObject original, int initialPoolSize, bool canGrow,
            Transform parent)
        {
            GameObjectPool objectPool = new GameObjectPool(canGrow, original);
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

        private GameObject InstantiateOriginal()
        {
            var obj = GameObject.Instantiate(_original, _parent) as GameObject;

            obj.SetActive(false);


            return obj;
        }

        public bool TryGet(out GameObject poolableObject)
        {
            if (_pool.Count == 0)
            {
                if (_canGrow)
                {
                    poolableObject = InstantiateOriginal();
                    poolableObject.SetActive(true);
                    //Debug.Log($"{poolableObject.name} pool grew");
                    return true;
                }

                poolableObject = null;
                return false;
            }

            poolableObject = _pool.Dequeue();
            poolableObject.SetActive(true);
            return true;
        }

        public bool TryReturn(GameObject poolableObject)
        {
            if (poolableObject == null)
            {
                return false;
            }

            poolableObject.SetActive(false);
            _pool.Enqueue(poolableObject);
            return true;
        }
    }
}