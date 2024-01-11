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
            objectpool.PoolGrew += (s, e) => _birdModels.Add(e.NewObject);
        }

        public void Update()
        {
            var grid = Grid.Instance;
            foreach (var model in _birdModels)
            {
                if (model.IsViewActive)
                {
                    model.SetPosition(model.Position + (model.Speed * Time.deltaTime * model.Forward));
                    var newCell = grid.PositionToCell(model.Position.x, model.Position.z);
                    if (newCell.x != model.x || newCell.y != model.y)
                    {
                        grid.UnitMoved(model, newCell);
                    }

                    model.TimeAlive += Time.deltaTime;

#if UNITY_EDITOR
                    /*
                    var next = model.Next as BirdModel;
                    if (next != null)
                    {
                        Debug.DrawLine(model.Position, next.Position, Color.red);
                    }
                    var prev = model.Prev as BirdModel;
                    if (prev != null)
                    {
                        Debug.DrawLine(model.Position, prev.Position, Color.blue);
                    }
                    grid.GetDrawingInfromation(out _, out var cellSizee, out var offset);
                    var x = model.x * cellSizee + offset.X;
                    var z = model.y * cellSizee + offset.Z;
                    Debug.DrawLine(model.Position, new Vector3(x,0,z), Color.yellow);
                    */
                    //model.DRAWRAABB();
#endif
                }
            }

            foreach (var obstacle in _obstaclesModels)
            {
                var cells = (obstacle.x, obstacle.y);
                cells.x -= 1;
                cells.y -= 1;
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        CheckCollisionInCell(grid, cells, obstacle);
                        cells.x += 1;
                    }

                    cells.x -= 3;
                    cells.y += 1;
                }
            }

            foreach (var player in Gameloop.Instance.players)
            {
                var position = player.transform.position;
                var radius = player.radius;

                var cell = grid.PositionToCell(position.x, position.z);
                cell.x -= 1;
                cell.y -= 1;
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        var birds = grid.GetBirdsInCell(cell);
                        while (birds != null)
                        {
                            if ((birds as BirdModel).CheckForCollision(position, radius)) break;
                            birds = birds.Next;
                        }
                        cell.x += 1;
                    }

                    cell.x -= 3;
                    cell.y += 1;
                }
            }
        }

        private void CheckCollisionInCell(Grid grid, (int x, int y) cells, ObstaclesModel obstacle)
        {
            var unit = grid.GetBirdsInCell(cells);
            if (unit == null)
                return;
            while (unit.Next != null)
            {
                CheckDistanceFromObstacle(unit as BirdModel, obstacle);
                unit = unit.Next;
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
            else if (model.rotating == obstacle)
            {
                Vector3 bridDirection = (model.Position - obstacle.Position).normalized;

                var birdToSpawn = (_spawnPoint - model.Position).magnitude;
                var obstacleToSpawn = (_spawnPoint - obstacle.Position).magnitude;
                if (birdToSpawn > obstacleToSpawn)
                {
                    model.rotating = null;
                    return;
                }

                var rotation = Quaternion.AngleAxis(model.RotationAroundObjectSpeed * Time.deltaTime, Vector3.up);
                bridDirection = rotation * bridDirection * obstacle.Radius;
                model.SetPosition(bridDirection + obstacle.Position);
            }
        }
    }
}