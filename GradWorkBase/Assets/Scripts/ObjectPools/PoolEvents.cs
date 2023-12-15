using System;
using UnityEngine;

namespace Vital.ObjectPools
{
    public class GameObjectPoolGrewEventArgs : EventArgs
    {
        public GameObjectPoolGrewEventArgs(GameObject newObject)
        {
            NewObject = newObject;
        }
        public GameObject NewObject { get; }
    }
    
    public class MonoBehaviourPoolGrewEventArgs<T> : EventArgs
        where T : MonoBehaviour
    {
        public MonoBehaviourPoolGrewEventArgs( T newObject)
        {
            NewObject = newObject;
        }
        public T NewObject { get; }
    }
    
    public class ScriptObjectPoolGrewEventArgs<T> : EventArgs
        where T : IPoolableScript
    {
        public ScriptObjectPoolGrewEventArgs(T newObject)
        {
            NewObject = newObject;
        }
        
        public T NewObject { get; }
    }
}