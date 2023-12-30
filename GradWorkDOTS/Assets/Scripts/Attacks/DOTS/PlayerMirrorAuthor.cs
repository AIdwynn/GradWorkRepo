using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics.GraphicsIntegration;
using UnityEngine;

namespace Gradwork.Attacks.DOTS
{
    public class PlayerMirrorAuthor : MonoBehaviour
    {

        class Baker : Baker<PlayerMirrorAuthor>
        {
            public override void Bake(PlayerMirrorAuthor authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None | TransformUsageFlags.NonUniformScale);
                AddComponent(entity, new PlayerMirrorComp());
            }
        }
    }
    public struct PlayerMirrorComp : IComponentData
    {
        public bool IsHit { get; set; }
        public Vector3 Position { get; private set; }
        public float LivesToDecrease { get; private set; }


        public void Hit(Vector3 position, float LivesToDecrease)
        {
            Position = position;
            this.LivesToDecrease = LivesToDecrease;
            IsHit = true;
        }
        

    }
}
