using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vital.ObjectPools;

namespace Gradwork.Attacks
{
    public class AttackManager
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
        }

        public void Update()
        {
            foreach (var model in _birdModels)
            {
                if (model.IsViewActive)
                {
                    model.SetPosition(model.Position + (model.Speed * Time.deltaTime * model.Forward));
                    CheckDistanceFromObstacles(model);
                    model.TimeAlive += Time.deltaTime;
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
                    model.rotating = obstacle;
                }
            }
            else if(model.rotating == obstacle)
            {
                Vector3 spawnDirection = (_spawnPoint - obstacle.Position).normalized;
                Vector3 bridDirection = (model.Position - obstacle.Position).normalized;
                var angle = Vector3.Angle(spawnDirection, bridDirection);

                Vector3 obstacleToBird = (model.Position - obstacle.Position).normalized;

                float newForward = Vector3.Dot(spawnDirection, obstacleToBird);
                angle = Mathf.Asin(newForward) * Mathf.Rad2Deg;

                model.SetRotation(new Vector3(0,angle,0));
                
                
            }
        }
    }
}