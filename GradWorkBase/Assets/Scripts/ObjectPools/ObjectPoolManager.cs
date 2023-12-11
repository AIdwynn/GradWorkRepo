using System;
using System.Collections.Generic;
using UnityEngine;

namespace Vital.ObjectPools
{
    public class ObjectPoolManager
    {
        private Transform _parent;
        private Dictionary<string, object> _pools = new Dictionary<string, object>();
        
        public static ObjectPoolManager Instance;
        
        public ObjectPoolManager ()
        {
            CreateParent();
            Instance = this;
        }
        
        private void CreateParent ()
        {
            _parent =  new GameObject("PoolManager").transform;
        }
        
        public ObjectPoolManager CreateObjectPool<T> (T original, int initialPoolSize, bool canGrow)
            where T : MonoBehaviour
        {
            var name = original.gameObject.name;
            if(_pools.ContainsKey(name)) { Debug.Log($"Pool already exists for {name}"); return this; }
            if(!_pools.TryAdd(name, ObjectPool<T>.CreateObjectPool(original, initialPoolSize, canGrow, _parent)))
                Debug.Log($"Pool already exists for {name}");
            return this;
        }
        
        public ObjectPoolManager CreateObjectPool (GameObject original, int initialPoolSize, bool canGrow)
        {
            var name = original.gameObject.name;
            if(_pools.ContainsKey(name)) { Debug.Log($"Pool already exists for {name}"); return this; }
            if(!_pools.TryAdd(name, GameObjectPool.CreateObjectPool(original, initialPoolSize, canGrow, _parent)))
                Debug.Log($"Pool already exists for {name}");
            return this;
        }
        
        public bool TryGet<T> (string name, out T poolableObject)
            where T : MonoBehaviour
        {
            if (_pools.TryGetValue(name, out object pool))
            {
                return ((ObjectPool<T>)pool).TryGet(out poolableObject);
            }
            
            poolableObject = null;
            return false;
        }
        
        public bool TryGet (string name, out GameObject poolableObject)
        {
            if (_pools.TryGetValue(name, out object pool))
            {
                return ((GameObjectPool)pool).TryGet(out poolableObject);
            }
            
            poolableObject = null;
            return false;
        }
        
        public bool TryReturn<T> (string name, T poolableObject)
            where T : MonoBehaviour
        {
            if (CloneNameRemover(name, out var editedName))
            {
                if (_pools.TryGetValue(editedName, out object pool))
                {
                    return ((ObjectPool<T>)pool).TryReturn(poolableObject);
                }
            }
            else
            {
                if (_pools.TryGetValue(name, out object pool))
                {
                    return ((ObjectPool<T>)pool).TryReturn(poolableObject);
                }
            }

            throw new Exception("Pool does not exist");
        }
        
        public bool TryReturn (string name, GameObject poolableObject)
        {
            if (CloneNameRemover(name, out var editedName))
            {
                if (_pools.TryGetValue(editedName, out object pool))
                {
                    return ((GameObjectPool)pool).TryReturn(poolableObject);
                }
            }
            else
            {
                if (_pools.TryGetValue(name, out object pool))
                {
                    return ((GameObjectPool)pool).TryReturn(poolableObject);
                }
            }

            throw new Exception("Pool does not exist");
        }

        private bool CloneNameRemover(string name, out string editedName)
        {
            try
            {
                string ToRemove = "(Clone)";
                editedName = name.Remove(name.IndexOf(ToRemove), ToRemove.Length);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                editedName = "";
                return false;
            }
        }
    }
}
