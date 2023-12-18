using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Gradwork.Attacks.DOTS
{
    public class BirdAuthor : MonoBehaviour
    {
        public float Speed = 1f;
        public float Lifetime = 1f;
        public float RotationAroundObjectSpeed = 1f;
        public bool StartEnabled = true;

        class Baker : Baker<BirdAuthor>
        {
            public override void Bake(BirdAuthor authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic | TransformUsageFlags.NonUniformScale);
                AddComponent(entity, new Bird
                {
                    Speed = authoring.Speed,
                    Lifetime = authoring.Lifetime,
                    RotationAroundObjectSpeed = authoring.RotationAroundObjectSpeed,
                    TimeAlive = 0f,
                    RotatingAround = float3.zero
                });
                SetComponentEnabled<Bird>(entity, authoring.StartEnabled);
            }
        }
    }

    public struct Bird : IComponentData, IEnableableComponent
    {
        public float Speed;
        public float Lifetime;
        public float RotationAroundObjectSpeed;
        public float TimeAlive;

        public float3 RotatingAround;

        /*public float3 Position;
        public float3 Forward;
        public float3 Rotation;*/
    }
}