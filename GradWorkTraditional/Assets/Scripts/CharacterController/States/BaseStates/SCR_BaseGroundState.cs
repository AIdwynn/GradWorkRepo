using System;
using DG.Tweening;
using UnityEngine;

namespace StateCharacterController.CharacterStates
{
    public abstract class SCR_BaseGroundState : SCR_CharacterBaseState, ICheckTerrain
    {
        protected Vector3 FullVelocity;
        protected Vector2 FullSpeed;
        protected float uphillModifier = 1;

        public static int _lightIndex;
        public static int _darkIndex;
        protected int _currentTextureIndex;

        protected Tweener coyoteTimeTweener;
        
        protected SCR_BaseGroundState(SCR_CharacterStateCreationVariables creationVariables) : base(creationVariables)
        {
        }

        public override Vector3 VelocityCalculations(Vector3 velocity, Vector2 input, Transform playerTransform, Transform CameraTransform)
        {
            FullVelocity.y = SCR_GameSettings.PlayerGroundGravity;
            FullVelocity =  base.VelocityCalculations(FullVelocity, input, playerTransform, CameraTransform);
            var returnVelocity = /*uphillModifier * */  new Vector3(FullVelocity.x * input.magnitude, FullVelocity.y, FullVelocity.z * input.magnitude);
            //Debug.Log(_maxSpeed);
            //Debug.Log(_currentTextureIndex);
            return returnVelocity;
            
        }

        public override Vector3 PartialStateSwitch(Vector3 velocity)
        {
            return StateSwitch(velocity);
        }

        public override void ActiveSwitchCheck(InputButtons input)
        {
            switch (input)
            {
                case InputButtons.A:
                    ThrowSwitchEvent(CharacterStatesEnum.InitiatingJump);
                    break;
                case InputButtons.RightTrigger:
                    if(SCRMO_CharacterController.CanAirDash)
                        ThrowSwitchEvent(CharacterStatesEnum.GroundDash);
                    break;
            }
        }
        
        protected override void PassiveSwitchCheck(Vector2 Input, Vector3 velocity, Transform transform)
        {
            CheckDistanceToGround(transform);

        }

        private void CheckDistanceToGround(Transform transform)
        {
            var rayDistance = 100;
            var fallDistance = 1f;
            //var HillSlideAngle = 35;
            var layermask = 1 << SCR_GameSettings.WalkableObjectLayers;
            if (Physics.Raycast(transform.position + new Vector3(0, 0.5f, 0), Vector3.down, out var hit, rayDistance, layermask))
            {
                if (hit.distance > fallDistance)
                {
                    StartCoyoteTime();
                }
                else
                {
                    coyoteTimeTweener?.Kill();
                    coyoteTimeTweener = null;
                    var terrain = hit.collider.GetComponent<Terrain>();

                    if (ChatGPTGetTextureFromTerrain(terrain, hit.point, out var index))
                    {
                        if (index != _currentTextureIndex)
                            SCR_EventHelper.TrySendEvent(TerrainChanged, this);
                    }
                }
            }
            else StartCoyoteTime();
        }

        private void StartCoyoteTime()
        {
            if(coyoteTimeTweener != null) return;
            var coyoteTime = 0.1f;
            var currentTime = 0f;
            coyoteTimeTweener = DOTween.To(() => currentTime, x => currentTime = x, 1, coyoteTime)
                .SetUpdate(UpdateType.Fixed)
                .OnComplete(
                    () =>
                    {
                        ThrowSwitchEvent(CharacterStatesEnum.Falling);
                        coyoteTimeTweener = null;
                    });
        }

        public event EventHandler TerrainChanged;
        public void SetIndexes(int lightIndex, int DarkIndex, int currentTextureIndex)
        {
            _lightIndex = lightIndex;
            _darkIndex = DarkIndex;
            _currentTextureIndex = currentTextureIndex;
        }

        protected bool ChatGPTGetTextureFromTerrain(Terrain terrain, Vector3 samplePoint, out int textureIndex)
        {
            textureIndex = -1;
            if (terrain == null)
            {
                return false;
            }

            // Convert the world space position to terrain local coordinates
            Vector3 terrainLocalPos = terrain.transform.InverseTransformPoint(samplePoint);

            // Get the terrain data
            TerrainData terrainData = terrain.terrainData;

            // Calculate the normalized position within the terrain (0 to 1)
            float normalizedX = Mathf.InverseLerp(0, terrainData.size.x, terrainLocalPos.x);
            float normalizedZ = Mathf.InverseLerp(0, terrainData.size.z, terrainLocalPos.z);

            // Calculate the terrain texture coordinates
            int textureCoordX = Mathf.FloorToInt(normalizedX * terrainData.alphamapWidth);
            int textureCoordY = Mathf.FloorToInt(normalizedZ * terrainData.alphamapHeight);

            // Get the alpha map (texture map)
            float[,,] alphaMap = terrainData.GetAlphamaps(textureCoordX, textureCoordY, 1, 1);

            // Assuming the terrain has multiple texture layers
            int numLayers = alphaMap.GetLength(2);

            // Loop through the layers and get their values
            for (int layerIndex = 0; layerIndex < numLayers; layerIndex++)
            {
                float textureValue = alphaMap[0, 0, layerIndex];
                if (textureValue > 0.5f)
                {
                    //Debug.Log($"Value: {textureValue} Layer: {layerIndex}");
                    if(layerIndex == _lightIndex)
                    {
                        textureIndex = 0;
                        return true;
                    }
                    else if (layerIndex == _darkIndex)
                    {
                        textureIndex = 1;
                        return true;
                    }
                }
            }

            return false;
            //Thank you chat gpt
        }
    }
}
