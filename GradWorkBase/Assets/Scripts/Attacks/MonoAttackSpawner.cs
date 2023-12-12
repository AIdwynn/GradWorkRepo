using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vital.ObjectPools;

namespace Gradwork.Attacks
{
    public class MonoAttackSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject _attackPrefab;

        [Header("Changeable on Play")] [SerializeField]
        private int _amountPerWave = 50;

        [SerializeField] private float _timeBetweenWaves = 2;

        [Header("Unchangeable on Play")] [SerializeField]
        private int _attackPoolSize = 1000;

        private ObjectPoolManager _objectPoolManager;
        private AttackManager _attackManager;

        void Start()
        {
            _objectPoolManager = new ObjectPoolManager();
            _attackManager = new AttackManager();
            _objectPoolManager.CreateObjectPool(_attackPrefab, _attackPoolSize, true)
                .CreateObjectPool<BirdModel>(BirdModel.NameStatic, _attackPoolSize, true);
            
            SpawnAttacks();
        }

        private void SpawnAttacks()
        {
            for (int i = 0; i < _amountPerWave; i++)
            {
                if (_objectPoolManager.TryGet(_attackPrefab.name, out GameObject attack) &&
                    _objectPoolManager.TryGetScript(BirdModel.NameStatic, out BirdModel birdModel))
                {
                    birdModel.SetView(attack).SetPosition(transform.position).SetActive(true);
                    attack.SetActive(true);
                }
            }

            StartCoroutine(WaitForNextWave());
        }
        
        public IEnumerator WaitForNextWave()
        {
            yield return new WaitForSeconds(_timeBetweenWaves);
            SpawnAttacks();
        }
    }
}