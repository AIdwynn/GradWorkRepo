using UnityEngine;

namespace Vital.ObjectPools
{
    public interface IPoolableScript
    {
        public string Name { get; }
        public bool IsViewActive { get; protected set; }
        public GameObject GO { get; set; }
    }
}