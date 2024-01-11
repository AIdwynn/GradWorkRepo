using System;
using System.Linq;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

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
            amountPerWave = 250;
            spawnPoint = new float3(0, 0, 0);
            
            state.RequireForUpdate<Spawner>();
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
            //state.RequireForUpdate<Execute.EnableableComponents>();
        }
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var spawner = SystemAPI.GetSingleton<Spawner>();
            if(spawner.Animted) return;
            
            var deltaTime = SystemAPI.Time.DeltaTime;
            timePassed += deltaTime;
            
            
            if (timePassed > timeToSpawn)
            {
                var prefab = spawner.Prefab;
                timePassed = 0;
                SpawnWave(ref state, prefab);
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

            #region EntityJob
            var job = new AttackJob
            {
                deltaTime = deltaTime,
                SpawnPoint = spawnPoint,
                obstaclePositions = obstaclePositions,
                obstacleRadiuses = obstacleRadiuses
            };
            state.Dependency = job.ScheduleParallel(state.Dependency);
            #endregion

            #region AspectEntityJob
            /*var job = new AspectAttackJob
            {
                deltaTime = deltaTime,
                SpawnPoint = spawnPoint,
                obstaclePositions = obstaclePositions,
                obstacleRadiuses = obstacleRadiuses
            };
            state.Dependency = job.Schedule(state.Dependency);*/

            #endregion

            #region ParallelJob

            /*i = 0;
            
            foreach (var (transform, bird) in SystemAPI.Query<RefRW<LocalTransform>, RefRW<Bird>>())
            {
                i++;
            }
            
            NativeArray<LocalTransform> transforms = new NativeArray<LocalTransform>(i, Allocator.TempJob);
            NativeArray<Bird> birds = new NativeArray<Bird>(i, Allocator.TempJob);
            i = 0;
            
            foreach(var (transform, bird) in SystemAPI.Query<RefRW<LocalTransform>, RefRW<Bird>>())
            {
                transforms[i] = transform.ValueRW;
                birds[i] = bird.ValueRW;
                i++;
            }

            var parallelAttackJob = new ParallelAttackJob()
            {
                deltaTime = deltaTime,
                SpawnPoint = spawnPoint,
                obstaclePositions = obstaclePositions,
                obstacleRadiuses = obstacleRadiuses,
                
                transforms = transforms,
                birds = birds
            };
            JobHandle parallelHandle = parallelAttackJob.Schedule(transforms.Length, 100);
            state.Dependency = parallelHandle;*/
            #endregion

            state.Dependency.Complete();

            obstaclePositions.Dispose();
            obstacleRadiuses.Dispose();
            /*transforms.Dispose();
            birds.Dispose();*/
            
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            
            foreach (var (bird, entity) in SystemAPI.Query<RefRW<Bird>>().WithEntityAccess())
            {
                bird.ValueRW.TimeAlive += deltaTime;
                if (bird.ValueRO.TimeAlive > bird.ValueRO.Lifetime)
                {
                    
                    ecb.DestroyEntity(entity);
                }
            }
            
            ecb.Playback(state.EntityManager);
            ecb.Dispose();

        }
        [BurstCompile]
        private void SpawnWave(ref SystemState state, Entity prefab)
        {
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
    
   
}
