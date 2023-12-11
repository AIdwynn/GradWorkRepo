using System;

namespace StateCharacterController.CharacterStates
{

    public interface ICheckTerrain
    {
        public event EventHandler TerrainChanged;

        public abstract void SetIndexes(int lightIndex, int DarkIndex, int currentTextureIndex);
    }
    public interface IRoll
    {
        public event EventHandler Roll;
        public event EventHandler RollStopped;
    }

    public interface ISlide
    {
        public event EventHandler Slide;
        public event EventHandler SlideStopped;
    }
    
    public interface IJump
    {
        public event EventHandler Jump;
        public event EventHandler JumpStopped;
    }

    public interface IUseStamina
    {
        public event EventHandler StaminaUsed;
    }
}