using System.Numerics;
using UnityEngine;
using Vital.ObjectPools;
using Vector3 = UnityEngine.Vector3;

namespace Gradwork.Attacks
{
    public class BirdModel : IPoolableScript
    {
        public GameObject View { get; protected set; }
        public Transform Transform { get; protected set; }

        public Vector3 Position
        {
            get { return _position; }
            protected set
            {
                Transform.position = value;
                _position = value;
            }
        }

        public Vector3 Forward
        {
            get { return _forward; }
            protected set
            {
                Transform.forward = value;
                _forward = value;
            }
        }

        public Vector3 Rotation
        {
            get { return _rotation; }
            protected set
            {
                Transform.eulerAngles = value;
                Forward = Transform.forward;
                _rotation = value;
            }
        }

        private Vector3 _position;
        private Vector3 _forward;
        private Vector3 _rotation;
        public ObstaclesModel rotating = null;

        private bool _isActive = false;
        private ObjectPoolManager _poolManager;

        public float Speed { get; protected set; }
        public float Lifetime { get; protected set; }

        public float TimeAlive
        {
            get { return _timeAlive; }
            set
            {
                if (Lifetime < value) ReturnToPool();
                _timeAlive = value;
            }
        }

        private float _timeAlive = 0f;

        public string Name
        {
            get => NameStatic;
        }

        bool IPoolableScript.IsViewActive
        {
            get => IsViewActive;
            set => IsViewActive = value;
        }

        public bool IsViewActive
        {
            get => _isActive;
            protected set
            {
                _isActive = value;
                View.SetActive(value);
            }
        }

        public static string NameStatic { get; private set; }

        public BirdModel()
        {
            NameStatic = typeof(BirdModel).ToString();
            _poolManager = ObjectPoolManager.Instance;
            Lifetime = 10;
            Speed = 10f;
        }

        public BirdModel SetView(GameObject view)
        {
            View = view;
            Transform = view.transform;
            TimeAlive = 0f;
            return this;
        }

        public BirdModel SetPosition(Vector3 position)
        {
            Position = position;
            return this;
        }

        public BirdModel SetRotation(Vector3 rotation)
        {
            Rotation = rotation;
            return this;
        }
        
        public BirdModel SetForward(Vector3 forward)
        {
            Forward = forward;
            return this;
        }

        public BirdModel SetActive(bool isActive)
        {
            IsViewActive = isActive;
            return this;
        }

        protected void ReturnToPool()
        {
            _poolManager.TryReturn(View.name, View);
            _poolManager.TryReturnScript(NameStatic, this);
        }
    }
}