using System;
using System.Linq;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.VisualScripting;
using UnityEngine;

namespace Gradwork.Attacks.DOTS
{

    public partial struct AttackSystem : ISystem
    {
        private float timePassed;
        private float timeToSpawn;
        private int amountPerWave;
        private float3 spawnPoint;
        
        public void OnCreate(ref SystemState state)
        {
            timePassed = 0;
            timeToSpawn = 0.5f;
            amountPerWave = 150;
            spawnPoint = new float3(0, 0, 0);
            
            state.RequireForUpdate<Spawner>();
            //state.RequireForUpdate<Execute.EnableableComponents>();
        }
        
        public void OnUpdate(ref SystemState state)
        {
            var deltaTime = SystemAPI.Time.DeltaTime;
            timePassed += deltaTime;
            if (timePassed > timeToSpawn)
            {
                timePassed = 0;
                SpawnWave(ref state);
            }
            

            var i = 0;

            foreach (var obstacle in SystemAPI.Query<RefRO<Obstacle>>())
            {
                i++;
            }
            
            NativeArray<float3> obstaclePositions = new NativeArray<float3>(i, Allocator.TempJob);
            NativeArray<float> obstacleRadiuses = new NativeArray<float>(i, Allocator.TempJob);
            i = 0;
            
            foreach(var obstacle in SystemAPI.Query<RefRO<Obstacle>>())
            {
                obstaclePositions[i] = obstacle.ValueRO.Position;
                obstacleRadiuses[i] = obstacle.ValueRO.Radius;
                i++;
            }
            
            
            var job = new AttackJob
            {
                deltaTime = deltaTime,
                SpawnPoint = spawnPoint,
                obstaclePositions = obstaclePositions,
                obstacleRadiuses = obstacleRadiuses
            };
            state.Dependency = job.Schedule(state.Dependency);
            state.Dependency.Complete();
            obstaclePositions.Dispose();
            obstacleRadiuses.Dispose();
            




        }

        private void SpawnWave(ref SystemState state)
        {
            var prefab = SystemAPI.GetSingleton<Spawner>().Prefab;
            var instances = state.EntityManager.Instantiate(prefab,amountPerWave, Allocator.Temp);

            for (int i = 0; i < instances.Length; i++)
            {
                var en = instances[i];
                var transform = SystemAPI.GetComponentRW<LocalTransform>(en);
                transform.ValueRW.Position = spawnPoint;
                transform.ValueRW.Rotation = Quaternion.Euler(GetRotation(i));

            }

        }
        
        private Vector3 GetRotation(int i)
        {
            var angle = (360f / amountPerWave) * i;
            return new Vector3(angle, 0, 0);
        }
    }
    
    partial struct AttackJob : IJobEntity
    {
        public float deltaTime;
        public float3 SpawnPoint;
        public NativeArray<float3> obstaclePositions;
        public NativeArray<float> obstacleRadiuses;

        void Execute(ref LocalTransform transform, ref Bird Bird)
        {
            transform.Position += transform.Forward() * Bird.Speed * deltaTime;
            CheckDistanceFromObstacles(ref transform, ref Bird);
            Bird.TimeAlive += deltaTime;
        }
        
        private void CheckDistanceFromObstacles(ref LocalTransform transform, ref Bird Bird)
        {
            for (int i = 0; i < obstaclePositions.Length; i++)
            {
                var obstaclePosition = obstaclePositions[i];
                var obstacleRadius = obstacleRadiuses[i];
                CheckDistanceFromObstacle(ref transform, ref Bird, obstaclePosition, obstacleRadius);
            }
        }
        
        private void CheckDistanceFromObstacle(ref LocalTransform transform, ref Bird Bird, in float3 obstacle, in float obstacleRadius)
        {
            if (CompareFloat3(Bird.RotatingAround, float3.zero))
            {
                var subtract = (transform.Position - obstacle);
                float distance = CalculateMagnitude(subtract);
                if (distance < obstacleRadius)
                {
                    var birdToSpawn = CalculateMagnitude(SpawnPoint - transform.Position);
                    var obstacleToSpawn = CalculateMagnitude(SpawnPoint - obstacle);
                    if (birdToSpawn < obstacleToSpawn)
                    {
                        Bird.RotatingAround = obstacle;
                    }
                    
                }
            }
            else if(CompareFloat3(Bird.RotatingAround,obstacle))
            {
                float3 bridDirection = CalculateNormalised(transform.Position - obstacle);
                
                var birdToSpawn = CalculateMagnitude(SpawnPoint - transform.Position);
                var obstacleToSpawn = CalculateMagnitude(SpawnPoint - obstacle);
                if (birdToSpawn > obstacleToSpawn)
                {
                    Bird.RotatingAround = float3.zero;
                    return;
                }

                var rotation = Quaternion.AngleAxis(Bird.RotationAroundObjectSpeed * deltaTime, Vector3.up);
                bridDirection = rotation * bridDirection * obstacleRadius;
                transform.Position = (bridDirection + obstacle);
                
                
                
            }
        }
        
        private float CalculateMagnitude(float3 vector)
        {
            return Mathf.Sqrt(vector.x * vector.x + vector.y * vector.y + vector.z * vector.z);
        }
        
        private float3 CalculateNormalised(float3 vector)
        {
            float magnitude = CalculateMagnitude(vector);
            return new float3(vector.x / magnitude, vector.y / magnitude, vector.z / magnitude);
        }
        
        private bool CompareFloat3(float3 a, float3 b)
        {
            return a.x == b.x && a.y == b.y && a.z == b.z;
        }
    }
}
