using Unity.Entities;
using UnityEngine;

namespace Gradwork.Attacks.DOTS
{
    public class ColliderWithPlayerAuthor : MonoBehaviour
    {
        public float LivesToDecrease;
        class Baker : Baker<ColliderWithPlayerAuthor>
        {
            public override void Bake(ColliderWithPlayerAuthor authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None | TransformUsageFlags.NonUniformScale);
                AddComponent(entity, new HitPlayerComp(){LivesToDecrease = authoring.LivesToDecrease, HasHit = false});
            }
        }
    }

    public struct HitPlayerComp : IComponentData
    {
        public float LivesToDecrease;
        public bool HasHit;
    }
}