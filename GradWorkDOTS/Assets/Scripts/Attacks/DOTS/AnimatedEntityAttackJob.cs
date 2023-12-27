using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Gradwork.Attacks.DOTS
{
    [BurstCompile]
    partial struct AnimatedAttackJob : IJobEntity
    {
        [ReadOnly] public float deltaTime;
        [ReadOnly] public float3 SpawnPoint;
        [ReadOnly] public NativeArray<float3> obstaclePositions;
        [ReadOnly]  public NativeArray<float> obstacleRadiuses;

        void Execute(ref LocalTransform transform, ref AnimatedBird Bird)
        {
            transform.Position += transform.Forward() * Bird.Speed * deltaTime;
            CheckDistanceFromObstacles(ref transform, ref Bird);
        }
        
        private void CheckDistanceFromObstacles(ref LocalTransform transform, ref AnimatedBird Bird)
        {
            for (int i = 0; i < obstaclePositions.Length; i++)
            {
                var obstaclePosition = obstaclePositions[i];
                var obstacleRadius = obstacleRadiuses[i];
                CheckDistanceFromObstacle(ref transform, ref Bird, obstaclePosition, obstacleRadius);
            }
        }
        
        private void CheckDistanceFromObstacle(ref LocalTransform transform, ref AnimatedBird Bird, in float3 obstacle, in float obstacleRadius)
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