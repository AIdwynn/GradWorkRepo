using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vital.ObjectPools;
using Unity.Entities;

namespace Gradwork.Attacks
{
    public partial class AttackManager : SystemBase
    {
        List<BirdModel> _birdModels;
        List<ObstaclesModel> _obstaclesModels;
        private Vector3 _spawnPoint;

        public AttackManager(List<ObstaclesModel> obstaclesModels, Vector3 spawnPoint)
        {
            this._obstaclesModels = obstaclesModels;
            this._spawnPoint = spawnPoint;
            ObjectPoolManager.Instance.TryGetScriptPool<BirdModel>(BirdModel.NameStatic, out var objectpool);
            _birdModels = objectpool.Pool;
            objectpool.PoolGrew += (s, e) => _birdModels.Add(e.NewObject);
        }

        public void Update()
        {
            foreach (var model in _birdModels)
            {
                if (model.IsViewActive)
                {
                    model.SetPosition(model.Position + (model.Speed * SystemAPI.Time.fixedDeltaTime * model.Forward));
                    CheckDistanceFromObstacles(model);
                    model.TimeAlive += SystemAPI.Time.fixedDeltaTime;
                }
            }
        }

        private void CheckDistanceFromObstacles(BirdModel model)
        {
            foreach (var obstacle in _obstaclesModels)
            {
                CheckDistanceFromObstacle(model, obstacle);
            }
        }

        private void CheckDistanceFromObstacle(BirdModel model, ObstaclesModel obstacle)
        {
            if (model.rotating == null)
            {
                var distance = Vector3.Distance(model.Position, obstacle.Position);
                if (distance < obstacle.Radius)
                {
                    var birdToSpawn = (_spawnPoint - model.Position).magnitude;
                    var obstacleToSpawn = (_spawnPoint - obstacle.Position).magnitude;
                    if (birdToSpawn < obstacleToSpawn)
                    {
                        model.rotating = obstacle;
                    }
                    
                }
            }
            else if(model.rotating == obstacle)
            {
                Vector3 bridDirection = (model.Position - obstacle.Position).normalized;
                
                var birdToSpawn = (_spawnPoint - model.Position).magnitude;
                var obstacleToSpawn = (_spawnPoint - obstacle.Position).magnitude;
                if (birdToSpawn > obstacleToSpawn)
                {
                    model.rotating = null;
                    return;
                }

                var rotation = Quaternion.AngleAxis(model.RotationAroundObjectSpeed * SystemAPI.Time.fixedDeltaTime, Vector3.up);
                bridDirection = rotation * bridDirection * obstacle.Radius;
                model.SetPosition(bridDirection + obstacle.Position);
                
                
                
            }
        }

        protected override void OnUpdate()
        {
            throw new NotImplementedException();
        }
    }
}