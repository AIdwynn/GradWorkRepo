using System;
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

        [Header("Unchangeable on Play")]
        [SerializeField] private int _attackPoolSize = 1000;
        [SerializeField] private List<ObstaclesView> _obstaclesViews;
        
        private List<ObstaclesModel> _obstaclesModels = new List<ObstaclesModel>();
        private ObjectPoolManager _objectPoolManager;
        private AttackManager _attackManager;

        private void Awake()
        {
            foreach (var obstacles in _obstaclesViews)
            {
                obstacles.ObstaclesCreated += (s,e) => _obstaclesModels.Add(e.ObstaclesModel);
            }
        }

        void Start()
        {
            _objectPoolManager = new ObjectPoolManager();
            new BirdModel();

            _objectPoolManager.CreateObjectPool(_attackPrefab, _attackPoolSize, true)
                .CreateObjectPool<BirdModel>(BirdModel.NameStatic, _attackPoolSize, true);

            _attackManager = new AttackManager(_obstaclesModels, transform.position);
            SpawnAttacks();
        }

        private void SpawnAttacks()
        {
            for (int i = 0; i < _amountPerWave; i++)
            {
                if (_objectPoolManager.TryGet(_attackPrefab.name, out GameObject attack) &&
                    _objectPoolManager.TryGetScript(BirdModel.NameStatic, out BirdModel birdModel))
                {
                    birdModel.SetView(attack).SetPosition(transform.position).SetRotation(GetRotation(i)).SetActive(true);
                    attack.SetActive(true);
                }
            }

            StartCoroutine(WaitForNextWave());
        }

        private void Update()
        {
            _attackManager.Update();
        }

        private Vector3 GetRotation(int i)
        {
            var angle = (360f / _amountPerWave) * i;
            return new Vector3(0, angle, 0);
        }

        public IEnumerator WaitForNextWave()
        {
            yield return new WaitForSeconds(_timeBetweenWaves);
            SpawnAttacks();
        }
    }
}