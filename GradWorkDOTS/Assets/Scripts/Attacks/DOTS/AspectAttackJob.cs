using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Gradwork.Attacks.DOTS
{
    [BurstCompile]
    partial struct AspectAttackJob : IJobEntity
    {
        [ReadOnly] public float deltaTime;
        [ReadOnly] public float3 SpawnPoint;
        [ReadOnly] public NativeArray<float3> obstaclePositions;
        [ReadOnly] public NativeArray<float> obstacleRadiuses;

        void Execute(MovingBirdAspect aspect)
        {
            aspect.Move(deltaTime, SpawnPoint, obstaclePositions, obstacleRadiuses);
        }
    }
    
    [BurstCompile]
    readonly partial struct MovingBirdAspect : IAspect
    {
        readonly RefRW<LocalTransform> Transform;
        readonly RefRW<Bird> Bird;

        public void Move(in float deltaTime, in float3 SpawnPoint, in NativeArray<float3> obstaclePositions,
            in NativeArray<float> obstacleRadiuses)
        {
            Transform.ValueRW.Position += Transform.ValueRO.Forward() * Bird.ValueRO.Speed * deltaTime;
            CheckDistanceFromObstacles(deltaTime, SpawnPoint, obstaclePositions, obstacleRadiuses);
        }

        private void CheckDistanceFromObstacles(in float deltaTime, in float3 SpawnPoint, in NativeArray<float3> obstaclePositions,
            in NativeArray<float> obstacleRadiuses)
        {
            for (int i = 0; i < obstaclePositions.Length; i++)
            {
                var obstaclePosition = obstaclePositions[i];
                var obstacleRadius = obstacleRadiuses[i];
                CheckDistanceFromObstacle(deltaTime, SpawnPoint, obstaclePosition, obstacleRadius);
            }
        }

        private void CheckDistanceFromObstacle(in float deltaTime, in float3 SpawnPoint, in float3 obstacle,
            in float obstacleRadius)
        {
            if (CompareFloat3(Bird.ValueRO.RotatingAround, float3.zero))
            {
                var subtract = (Transform.ValueRO.Position - obstacle);
                float distance = CalculateMagnitude(subtract);
                if (distance < obstacleRadius)
                {
                    var birdToSpawn = CalculateMagnitude(SpawnPoint - Transform.ValueRO.Position);
                    var obstacleToSpawn = CalculateMagnitude(SpawnPoint - obstacle);
                    if (birdToSpawn < obstacleToSpawn)
                    {
                        Bird.ValueRW.RotatingAround = obstacle;
                    }
                }
            }
            else if (CompareFloat3(Bird.ValueRO.RotatingAround, obstacle))
            {
                float3 bridDirection = CalculateNormalised(Transform.ValueRO.Position - obstacle);

                var birdToSpawn = CalculateMagnitude(SpawnPoint - Transform.ValueRO.Position);
                var obstacleToSpawn = CalculateMagnitude(SpawnPoint - obstacle);
                if (birdToSpawn > obstacleToSpawn)
                {
                    Bird.ValueRW.RotatingAround = float3.zero;
                    return;
                }

                var rotation = Quaternion.AngleAxis(Bird.ValueRO.RotationAroundObjectSpeed * deltaTime, Vector3.up);
                bridDirection = rotation * bridDirection * obstacleRadius;
                Transform.ValueRW.Position = (bridDirection + obstacle);
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