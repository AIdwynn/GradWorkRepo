using System;
using System.Numerics;
using UnityEngine;
using Vital.ObjectPools;
using Vector3 = UnityEngine.Vector3;

namespace Gradwork.Attacks
{
    public class BirdModel : IPoolableScript
    {
        GameObject IPoolableScript.GO
        {
            get => GO;
            set => GO = value;
        }
        
        public GameObject GO
        {
            get
            {
                return _go;
            }
            protected set
            {
                _go = value;
                Transform = value.transform;
                TimeAlive = 0f;
            }
        }
        private GameObject _go;
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
        public event EventHandler HitEvent;
        private bool _isActive = false;
        private ObjectPoolManager _poolManager;

        public float Speed { get; protected set; }
        public float Lifetime { get; protected set; }
        public float RotationAroundObjectSpeed { get; protected set; }

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
                GO.SetActive(value);
            }
        }

        public static string NameStatic { get; private set; }

        public BirdModel()
        {
            NameStatic = typeof(BirdModel).ToString();
            _poolManager = ObjectPoolManager.Instance;
            Lifetime = 10;
            Speed = 10f;
            RotationAroundObjectSpeed = 10f;
        }

        private void OnHit()
        {
            SCR_EventHelper.TrySendEvent(HitEvent, this);
            ReturnToPool();
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
            if(isActive) TimeAlive = 0f;
            return this;
        }

        protected void ReturnToPool()
        {
            _poolManager.TryReturnScript(NameStatic, this);
        }
    }
}