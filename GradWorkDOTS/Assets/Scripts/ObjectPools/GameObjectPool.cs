using System;
using System.Collections.Generic;
using UnityEngine;

namespace Vital.ObjectPools
{
    public class GameObjectPool
    {
        private List<GameObject> _pool = new List<GameObject>();
        private bool _canGrow = true;
        private GameObject _original;
        private Transform _parent;
        private int index = 0;
        public event EventHandler<GameObjectPoolGrewEventArgs> PoolGrew;

        public List<GameObject> Pool
        {
            get { return _pool; }
        }

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
                _pool.Add(InstantiateOriginal());
            }
        }

        private GameObject InstantiateOriginal()
        {
            var obj = GameObject.Instantiate(_original, _parent) as GameObject;

            SCR_EventHelper.TrySendEvent(PoolGrew, this, new GameObjectPoolGrewEventArgs(obj));
            obj.SetActive(false);


            return obj;
        }

        public bool TryGet(out GameObject poolableObject)
        {
            poolableObject = _pool[index%_pool.Count];
            
            if (poolableObject.activeSelf)
            {
                if (_canGrow)
                {
                    poolableObject = InstantiateOriginal();
                    poolableObject.SetActive(true);
                    index++;
                    //Debug.Log($"{poolableObject.name} pool grew");
                    return true;
                }

                poolableObject = null;
                return false;
            }
            

            poolableObject.SetActive(true);
            index++;
            return true;
        }

        public bool TryReturn(GameObject poolableObject)
        {
            if (poolableObject == null)
            {
                return false;
            }
            
            poolableObject.SetActive(false);

            return true;
        }
    }
}