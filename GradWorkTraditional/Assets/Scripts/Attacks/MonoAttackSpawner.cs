using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vital;
using Vital.ObjectPools;
using Grid = Vital.Spatial_Partitioning.Grid;
using Vital.Spatial_Partitioning;

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
        [SerializeField] private SCRMO_CharacterController _characterController;

        [Header("Animation")] [SerializeField] private bool Animated;
        [SerializeField] private int _amountOfframes = 43;
        [SerializeField] private float _clipLength = 1.433f;
        [SerializeField] private Material material;
        private VertexAnimationMaterialHandler _vertexAnimationMaterialHandler;
        
        private List<ObstaclesModel> _obstaclesModels = new List<ObstaclesModel>();
        private ObjectPoolManager _objectPoolManager;
        private AttackManager _attackManager;
        private Grid _grid;

        private void Awake()
        {
            foreach (var obstacles in _obstaclesViews)
            {
                obstacles.ObstaclesCreated += (s,e) => _obstaclesModels.Add(e.ObstaclesModel);
            }

            Gameloop gameloop = new Gameloop();
        }

        void Start()
        {
            _objectPoolManager = new ObjectPoolManager();
            new BirdModel();

            _objectPoolManager.CreateObjectPool<BirdModel>(_attackPrefab, BirdModel.NameStatic, _attackPoolSize, true)
            .TryGetScriptPool<BirdModel>(BirdModel.NameStatic, out var birdPool);
            
            foreach (var bird in birdPool.Pool)
            {
                bird.HitEvent += (s, e) =>
                {
                    _characterController.PlayerHit(bird.Position, 1);
                };
                
            }
            
            if (Animated)
            {
                _vertexAnimationMaterialHandler = new VertexAnimationMaterialHandler(material, 5, _amountOfframes, _clipLength);
                foreach (var bird in birdPool.Pool)
                {
                    bird.GO.GetComponentInChildren<MeshRenderer>().material = _vertexAnimationMaterialHandler.GetRandomMaterial();
                }
            }
            
            _attackManager = new AttackManager(_obstaclesModels, transform.position);
            
            _grid = new Grid();
            foreach (var obstacle in _obstaclesModels)
            {
                _grid.AddObstacle(obstacle, _grid.PositionToCell(obstacle.Position.x, obstacle.Position.z));
            }
            
            SpawnAttacks();
        }

        private void SpawnAttacks()
        {
            for (int i = 0; i < _amountPerWave; i++)
            {
                if (_objectPoolManager.TryGetScript(BirdModel.NameStatic, out BirdModel birdModel))
                {
                    birdModel.SetPosition(transform.position).SetRotation(GetRotation(i)).SetActive(true);
                    _grid.AddBird(birdModel, _grid.PositionToCell(transform.position.x, transform.position.z));
                }
            }

            StartCoroutine(WaitForNextWave());
        }

        private Vector3 GetRotation(int i)
        {
            var angle = (360f / _amountPerWave) * i;
            return new Vector3(0, angle, 0);
        }

        private void Update()
        {
            if(Animated)
                _vertexAnimationMaterialHandler.Update(Time.deltaTime);
            _attackManager.Update();
        }

        public IEnumerator WaitForNextWave()
        {
            yield return new WaitForSeconds(_timeBetweenWaves);
            SpawnAttacks();
        }
    }
}