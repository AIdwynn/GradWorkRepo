using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Rendering;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
/*public partial class ChatGPTExample : SystemBase
{
    protected override void OnCreate()
    {
        // Create an entity with components
        Entity entity = EntityManager.CreateEntity(
            typeof(MeshRenderer), // Rendering component
            typeof(LocalToWorld), // Transform component
            typeof(MoveSpeed) // Custom component
        );

        // Set the render mesh
        EntityManager.SetSharedComponentData(entity, new MeshRenderer
        {
            mesh = your mesh ,
            material =  your material 
        });

        // Set the initial position
        EntityManager.SetComponentData(entity, new LocalToWorld { Value = new float3(0, 0, 0) });

        // Set the move speed
        EntityManager.SetComponentData(entity, new MoveSpeed { speed = 5.0f });
    }

    protected override void OnUpdate()
    {
        // Get delta time
        float deltaTime = World.Time.DeltaTime;

        // Iterate over all entities with MoveSpeed component
        Entities.ForEach((ref Translation translation, in MoveSpeed moveSpeed) =>
        {
            // Move the entity based on its speed
            translation.Value.x += moveSpeed.speed * deltaTime;
        }).Schedule();
    }
}

// Custom component for move speed
public struct MoveSpeed : IComponentData
{
    public float speed;
}*/

