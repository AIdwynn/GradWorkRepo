using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.SocialPlatforms;

namespace Gradwork.Attacks.DOTS
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    public partial struct CollideWithPlayerSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<Spawner>();
            state.RequireForUpdate<PlayerMirrorComp>();
            state.RequireForUpdate<HitPlayerComp>();
            state.RequireForUpdate<SimulationSingleton>();
        }

        public void OnUpdate(ref SystemState state)
        {
            foreach (var (playerMirrorComp, transform) in SystemAPI.Query<RefRO<PlayerMirrorComp>, RefRW<LocalTransform>>())
            {
                if (GameLoop.Instance == null) new GameLoop();
                var player = GameLoop.Instance.player;
                transform.ValueRW.Position = player.transform.position;
                transform.ValueRW.Rotation = player.transform.rotation;
            }
            
            state.Dependency = new CollisionEventPlayerHitJob
            {
                HitPlayerLookup = SystemAPI.GetComponentLookup<HitPlayerComp>(),
                PlayerMirrorLookup = SystemAPI.GetComponentLookup<PlayerMirrorComp>(),
                LocalTransformLookup = SystemAPI.GetComponentLookup<LocalTransform>(),
            }.Schedule(SystemAPI.GetSingleton<SimulationSingleton>(), state.Dependency);

            state.Dependency.Complete();
            
            foreach (var playerMirrorComp in SystemAPI.Query<RefRW<PlayerMirrorComp>>())
            {
                if (playerMirrorComp.ValueRO.IsHit)
                {
                    var player = GameLoop.Instance.player;
                    player.PlayerHit(playerMirrorComp.ValueRO.Position, playerMirrorComp.ValueRO.LivesToDecrease);
                    playerMirrorComp.ValueRW.IsHit = false;
                }
            }

            var ecb = new EntityCommandBuffer(Allocator.Temp);
            foreach (var (hitPlayerComp, entity) in SystemAPI.Query<HitPlayerComp>().WithEntityAccess())
            {
                if (hitPlayerComp.HasHit)
                {
                    ecb.DestroyEntity(entity);
                }
            }
            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }

        struct CollisionEventPlayerHitJob : ICollisionEventsJob
        {
            public ComponentLookup<PlayerMirrorComp> PlayerMirrorLookup;
            public ComponentLookup<HitPlayerComp> HitPlayerLookup;
            [ReadOnly] public ComponentLookup<LocalTransform> LocalTransformLookup;
            
            public void Execute(CollisionEvent collisionEvent)
            {
                Entity entityA = collisionEvent.EntityA;
                Entity entityB = collisionEvent.EntityB;

                bool isBodyAPlayer = PlayerMirrorLookup.HasComponent(entityA);
                bool isBodyBPlayer = PlayerMirrorLookup.HasComponent(entityB);

                bool isBodyABird = HitPlayerLookup.HasComponent(entityA);
                bool isBodyBBird = HitPlayerLookup.HasComponent(entityB);

                if (isBodyAPlayer && isBodyBBird)
                {
                    var player = PlayerMirrorLookup[entityA];
                    var hitPlayerComp = HitPlayerLookup[entityB];
                    var position = LocalTransformLookup[entityB].Position;
                    player.Hit(position, hitPlayerComp.LivesToDecrease);
                    hitPlayerComp.HasHit = true;
                    
                    PlayerMirrorLookup[entityA] = player;
                    HitPlayerLookup[entityB] = hitPlayerComp;
                }
                else if (isBodyBPlayer && isBodyABird)
                {
                    var player = PlayerMirrorLookup[entityB];
                    var hitPlayerComp = HitPlayerLookup[entityA];
                    var position = LocalTransformLookup[entityA].Position;
                    player.Hit(position, hitPlayerComp.LivesToDecrease);
                    hitPlayerComp.HasHit = true;

                    PlayerMirrorLookup[entityB] = player;
                    HitPlayerLookup[entityA] = hitPlayerComp;
                }
            }
        }
        
    }
}
