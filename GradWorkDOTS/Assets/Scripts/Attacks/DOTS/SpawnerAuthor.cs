using Unity.Entities;
using UnityEngine;

namespace Gradwork.Attacks.DOTS
{
    public class SpawnerAuthor : MonoBehaviour
    {
        public BirdAuthor Prefab;
        
        class Baker : Baker<SpawnerAuthor>
        {
            public override void Bake(SpawnerAuthor authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new Spawner
                {
                    Prefab = GetEntity(authoring.Prefab.gameObject, TransformUsageFlags.Dynamic)
                });
            }
        }
    }
    
    struct Spawner : IComponentData
    {
        public Entity Prefab;
    }
}