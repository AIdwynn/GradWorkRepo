using System;
using System.Collections.Generic;
using UnityEngine;

namespace Vital.ObjectPools
{
    public class ScriptObjectPool<T> where T : IPoolableScript
    {
        private List<T> _pool = new List<T>();
        private bool _canGrow = true;
        private T _original;
        private Transform _parent;
        private int index = 0;
        public event EventHandler<ScriptObjectPoolGrewEventArgs<T>> PoolGrew;

        public List<T> Pool
        {
            get { return _pool; }
        }

        public ScriptObjectPool(bool canGrow)
        {
            _canGrow = canGrow;
        }

        public static ScriptObjectPool<T> CreateObjectPool(int initialPoolSize, bool canGrow)
        {
            ScriptObjectPool<T> objectPool = new ScriptObjectPool<T>(canGrow);
            objectPool.FillPool(initialPoolSize);
            return objectPool;
        }
        private void FillPool(int initialPoolSize)
        {
            for (int i = 0; i < initialPoolSize; i++)
            {
                _pool.Add(InstantiateOriginal());
            }
        }

        private T InstantiateOriginal()
        {
            var obj = (T)Activator.CreateInstance(typeof(T));

            SCR_EventHelper.TrySendEvent(PoolGrew, this, new ScriptObjectPoolGrewEventArgs<T>( obj));
            return obj;
        }

        public bool TryGet(out T poolableObject)
        {
            poolableObject = _pool[index % _pool.Count];

            if (poolableObject.IsViewActive)
            {
                if (_canGrow)
                {
                    poolableObject = InstantiateOriginal();
                    index++;
                    //Debug.Log($"{poolableObject.name} pool grew");
                    return true;
                }

                poolableObject = default(T);
                return false;
            }
            
            index++;
            return true;
        }

        public bool TryReturn(T poolableObject)
        {
            if (poolableObject == null)
            {
                return false;
            }

            return true;
        }
    }
}