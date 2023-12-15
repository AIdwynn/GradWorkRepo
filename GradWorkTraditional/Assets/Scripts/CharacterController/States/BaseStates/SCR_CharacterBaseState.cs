using System;
using UnityEngine;
namespace StateCharacterController.CharacterStates
{
    public abstract class SCR_CharacterBaseState
    {
        protected float _acceleration;
        protected float _movingFriction;
        protected float _stoppingFriction;
        public float _maxSpeed;
        protected float _rotateSpeed;
        protected float _transitionToReallyLongFallTime;
        public CharacterStatesEnum _stateEnum;
        public EventHandler<EventSwitchArgs> OnSwitch;

        public static Vector3 currentEulerRotation;
        protected static Vector2 currentInput;

        public SCR_CharacterBaseState(SCR_CharacterStateCreationVariables creationVariables)
        {
            _stateEnum = creationVariables.statesEnum;
            _acceleration = creationVariables.acceleration;
            _movingFriction = creationVariables.movingFriction;
            _stoppingFriction = creationVariables.stoppingFriction;
            _maxSpeed = creationVariables.maxSpeed;
            _rotateSpeed = creationVariables.rotateSpeed;
        }

        public virtual Vector3 VelocityCalculations(Vector3 velocity, Vector2 input, Transform playerTransform, Transform cameraTransform)
        {
            ChangeCurrentInput(input);
            velocity = ClampVelocity(velocity);
            var CameraInput = InputWithCamera(currentInput, cameraTransform);
            TurningCharacter(playerTransform, CameraInput);
            velocity = ApplyFriction(velocity, CameraInput);
            velocity = CalculateVelocityWithForward(velocity, CameraInput, playerTransform);
            velocity = ClampVelocity(velocity);

            PassiveSwitchCheck(CameraInput, velocity, playerTransform);
            return velocity;
        }

        protected virtual void ChangeCurrentInput(Vector2 input)
        {
            ChangeInputDirection(input);
            ChangeInputMagnitude(input);
        }

        protected virtual void ChangeInputMagnitude(Vector2 input)
        {
            if(currentInput.magnitude != input.magnitude)
            {
                var multiplier = input.magnitude / currentInput.magnitude;
                currentInput *= multiplier;
            }
        }

        protected virtual void ChangeInputDirection(Vector2 input)
        {
            //Debug.Log($"Input: {input}, CurrentInput: {currentInput}, Dot: {Vector2.Dot(input, currentInput)}");
            var dot = Vector2.Dot(input.normalized, currentInput.normalized);
            if (dot < 0.99f)
                currentInput = input;
        }

        protected Vector3 ApplyFriction(Vector3 velocity, Vector2 Input)
        {
            Vector2 velocity2D = new Vector2(velocity.x, velocity.z);
            if (Input.magnitude > 0.1)
                velocity2D = ApplyMovingFriction(velocity2D);
            else
                velocity2D = ApplyStoppingFriction(velocity2D);
            velocity.x = velocity2D.x;
            velocity.z = velocity2D.y;
            return velocity;

        }
        private Vector3 ApplyStoppingFriction(Vector2 velocity)
        {
            return velocity * (1 - _stoppingFriction);
        }
        public static Vector2 InputWithCamera(Vector2 input, Transform cameraTransform)
        {
            Vector3 cameraForward = cameraTransform.forward;
            Vector2 cameraForwad2D = new Vector2(cameraForward.x, cameraForward.z);
            cameraForwad2D.Normalize();

            // Get the right direction of the camera without its y-component
            Vector3 cameraRight = cameraTransform.right;
            Vector2 cameraRight2D = new Vector2(cameraRight.x, cameraRight.z);
            cameraRight2D.Normalize();

            // Calculate the movement direction relative to the camera
            Vector2 movementDirection = input.x * cameraRight2D + input.y * cameraForwad2D;

            return movementDirection;
            //return input * CalculateCameraDirection(cameraTransform.forward);
        }

        protected virtual Vector3 CalculateVelocityWithForward(Vector3 velocity, Vector2 Input, Transform CharacterController)
        {
            var inputMagnitude = Input.magnitude;
            var InputDirection = Input.normalized; //CalculateForwardWithoutX();
            SCRMO_CharacterController.ForwardText?.SetText("Forward: " + InputDirection);
            //var InputDirection = CharacterController.forward;
            velocity += new Vector3(inputMagnitude * InputDirection.x * _acceleration, 0, inputMagnitude * InputDirection.y * _acceleration);
            return velocity;
        }

        private Vector3 CalculateForwardWithoutX()
        {
            float x = Mathf.Sin((currentEulerRotation.y + currentEulerRotation.z) * Mathf.Deg2Rad) * Mathf.Cos(0);
            float y = Mathf.Sin(0);
            float z = Mathf.Cos((currentEulerRotation.y + currentEulerRotation.z) * Mathf.Deg2Rad) * Mathf.Cos(0);

            return new Vector3(x, y, z);
        }

        protected virtual void TurningCharacter(Transform transform, Vector2 input)
        {
            if (input.magnitude > 0.05f)
            {
                input.Normalize();
                var angle = AngleCalculations(input);
                currentEulerRotation.y = Mathf.LerpAngle(currentEulerRotation.y, angle, _rotateSpeed);
                transform.rotation = Quaternion.Euler(currentEulerRotation.x, currentEulerRotation.y, currentEulerRotation.z);
            }
        }

        protected Vector3 ApplyMovingFriction(Vector2 velocity)
        {
            return velocity * (1 - _movingFriction);
        }

        protected Vector3 ClampVelocity(Vector3 velocity)
        {
            Vector2 velocity2D = new Vector2(velocity.x, velocity.z);
            velocity2D = Vector2.ClampMagnitude(velocity2D, _maxSpeed);
            velocity.x = velocity2D.x;
            velocity.z = velocity2D.y;
            return velocity;
        }

        protected float AngleCalculations(Vector2 input)
        {
            var angle = Mathf.Atan2(input.x, input.y);
            return angle * Mathf.Rad2Deg;
        }

        protected abstract void PassiveSwitchCheck(Vector2 input, Vector3 velocity, Transform transform);
        public abstract void ActiveSwitchCheck(InputButtons input);

        public abstract Vector3 StateSwitch(Vector3 velocity);
        public abstract Vector3 PartialStateSwitch(Vector3 velocity);

        public abstract void OnStateExit();
        protected void ThrowSwitchEvent(CharacterStatesEnum state)
        {
            SCR_EventHelper.TrySendEvent(OnSwitch, this, new EventSwitchArgs(state));
        }
        public virtual bool PlayerHit()
        {
            ThrowSwitchEvent(CharacterStatesEnum.Hit);
            return true;
        }
    }
}
