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
        
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            timePassed = 0;
            timeToSpawn = 0.5f;
            amountPerWave = 150;
            spawnPoint = new float3(0, 0, 0);
            
            state.RequireForUpdate<Spawner>();
            //state.RequireForUpdate<Execute.EnableableComponents>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var deltaTime = SystemAPI.Time.DeltaTime;
            timePassed += deltaTime;
            if (timePassed > timeToSpawn)
            {
                timePassed = 0;
                SpawnWave(ref state);
            }
            
            var obstaclesQuery = SystemAPI.Query<RefRO<Obstacle>>();
            Obstacle[] obstacles = new Obstacle[obstaclesQuery.Count()];
            int i = 0;
            foreach(var obstacle in obstaclesQuery)
            {
                obstacles[i] = obstacle.ValueRO; 
                i++;
            }
            
            var job = new AttackJob
            {
                deltaTime = deltaTime,
                SpawnPoint = spawnPoint,
                obstacles = obstacles
            };
            job.Schedule();
            


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
            return new Vector3(0, angle, 0);
        }
    }
    
    [BurstCompile]
    partial struct AttackJob : IJobEntity
    {
        public float deltaTime;
        public float3 SpawnPoint;
        public Obstacle[] obstacles;

        void Execute(ref LocalTransform transform, ref PostTransformMatrix postTransform, ref Bird Bird)
        {
            transform.Position += transform.Forward() * Bird.Speed * deltaTime;
            CheckDistanceFromObstacles(ref transform, ref Bird);
            Bird.TimeAlive += deltaTime;
        }
        
        private void CheckDistanceFromObstacles(ref LocalTransform transform, ref Bird Bird)
        {
            foreach (var obstacle in obstacles)
            {
                CheckDistanceFromObstacle(ref transform, ref Bird, obstacle);
            }
        }
        
        private void CheckDistanceFromObstacle(ref LocalTransform transform, ref Bird Bird, in Obstacle obstacle)
        {
            if (CompareFloat3(Bird.RotatingAround, float3.zero))
            {
                var subtract = (transform.Position - obstacle.Position);
                float distance = CalculateMagnitude(subtract);
                if (distance < obstacle.Radius)
                {
                    var birdToSpawn = CalculateMagnitude(SpawnPoint - transform.Position);
                    var obstacleToSpawn = CalculateMagnitude(SpawnPoint - obstacle.Position);
                    if (birdToSpawn < obstacleToSpawn)
                    {
                        Bird.RotatingAround = obstacle.Position;
                    }
                    
                }
            }
            else if(CompareFloat3(Bird.RotatingAround,obstacle.Position))
            {
                float3 bridDirection = (transform.Position - obstacle.Position);
                
                var birdToSpawn = CalculateMagnitude(SpawnPoint - transform.Position);
                var obstacleToSpawn = CalculateMagnitude(SpawnPoint - obstacle.Position);
                if (birdToSpawn > obstacleToSpawn)
                {
                    Bird.RotatingAround = float3.zero;
                    return;
                }

                var rotation = Quaternion.AngleAxis(Bird.RotationAroundObjectSpeed * Time.fixedDeltaTime, Vector3.up);
                bridDirection = rotation * bridDirection * obstacle.Radius;
                transform.Position = (bridDirection + obstacle.Position);
                
                
                
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
