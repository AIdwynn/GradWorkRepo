using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vital.ObjectPools;

namespace Gradwork.Attacks
{
    public class MonoAttackSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject _attackPrefab;
        private ObjectPoolManager _objectPoolManager;
        private AttackManager _attackManager;
        void Start()
        {
            _objectPoolManager = new ObjectPoolManager();
            _attackManager = new AttackManager();
            _objectPoolManager.CreateObjectPool(_attackPrefab, 10, true);
        }

        // Update is called once per frame
        void Update()
        {
            
        }
    }
}