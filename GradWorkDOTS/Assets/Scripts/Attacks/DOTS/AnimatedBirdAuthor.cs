using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Gradwork.Attacks.DOTS
{
    public class AnimatedBirdAuthor : MonoBehaviour
    {
        public float Speed = 1f;
        public float Lifetime = 1f;
        public float RotationAroundObjectSpeed = 1f;
        public bool StartEnabled = true;
        class Baker : Baker<AnimatedBirdAuthor>
        {
            public override void Bake(AnimatedBirdAuthor authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic | TransformUsageFlags.NonUniformScale);
                AddComponent(entity, new AnimatedBird
                {
                    Speed = authoring.Speed,
                    Lifetime = authoring.Lifetime,
                    RotationAroundObjectSpeed = authoring.RotationAroundObjectSpeed,
                    TimeAlive = 0f,
                    RotatingAround = float3.zero
                });
                SetComponentEnabled<AnimatedBird>(entity, authoring.StartEnabled);
            }
        }
    }

    public struct AnimatedBird : IComponentData, IEnableableComponent
    {
        public float Speed;
        public float Lifetime;
        public float RotationAroundObjectSpeed;
        public float TimeAlive;

        public float3 RotatingAround;
    }

    public class AnimatorComp : ICleanupComponentData
    {
        public Animator Animator;
    }
}
