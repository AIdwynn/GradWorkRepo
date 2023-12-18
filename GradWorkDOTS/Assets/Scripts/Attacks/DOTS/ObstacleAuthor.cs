using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Gradwork.Attacks.DOTS
{
    public class ObstacleAuthor : MonoBehaviour
    {
        public float Radius = 1f;
        
        class Baker : Baker<ObstacleAuthor>
        {
            public override void Bake(ObstacleAuthor authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new Obstacle
                {
                    Radius = authoring.Radius,
                    Position = authoring.transform.position
                });
            }
        }
    }
    
    public struct Obstacle : IComponentData
    {
        public float Radius;
        public float3 Position;
    }
}