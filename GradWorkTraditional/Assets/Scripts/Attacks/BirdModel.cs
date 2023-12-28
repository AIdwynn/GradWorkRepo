using System;
using System.Numerics;
using UnityEngine;
using Vital.ObjectPools;
using Vital.Spatial_Partitioning;
using Grid = Vital.Spatial_Partitioning.Grid;
using Vector3 = UnityEngine.Vector3;

namespace Gradwork.Attacks
{
    public class BirdModel : Unit, IPoolableScript
    {
        GameObject IPoolableScript.GO
        {
            get => GO;
            set => GO = value;
        }

        public GameObject GO
        {
            get { return _go; }
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

        protected BoundingBox AABB { get; set; }

        public BoundingBox RAABB
        {
            get
            {
                var boundingBox = AABB.RAABB(Rotation.y);
                return boundingBox;
            }
        }


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

        public BirdModel() : base()
        {
            NameStatic = typeof(BirdModel).ToString();
            _poolManager = ObjectPoolManager.Instance;
            Lifetime = 10f;
            Speed = 10f;
            RotationAroundObjectSpeed = 10f;
            var topNorthernCorner = new Vector3(0.273f, 0.229f, 0.8965f);
            AABB = new BoundingBox(topNorthernCorner, -topNorthernCorner);
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
            if (isActive) TimeAlive = 0f;
            return this;
        }

        protected void ReturnToPool()
        {
            Grid.Instance.RemoveUnit(this);
            if (_poolManager.TryReturnScript(NameStatic, this))
                IsViewActive = false;
        }

#if UNITY_EDITOR
        public void DRAWRAABB()
        {
            var boundingBox = RAABB;

            Debug.DrawLine(boundingBox.TopNorthernRight + Position, boundingBox.BottemNorthernRight + Position, Color.yellow);
            Debug.DrawLine(boundingBox.TopNorthernRight + Position, boundingBox.TopSouthernRight + Position, Color.yellow);
            Debug.DrawLine(boundingBox.TopNorthernRight + Position, boundingBox.TopNorthernLeft + Position, Color.yellow);
            Debug.DrawLine(boundingBox.BottemSouthernRight + Position, boundingBox.BottemSouthernLeft + Position, Color.yellow);
            Debug.DrawLine(boundingBox.BottemSouthernRight + Position, boundingBox.BottemNorthernRight + Position, Color.yellow);
            Debug.DrawLine(boundingBox.BottemSouthernRight + Position, boundingBox.TopSouthernRight + Position, Color.yellow);
            Debug.DrawLine(boundingBox.TopSouthernLeft + Position, boundingBox.TopSouthernRight + Position, Color.yellow);
            Debug.DrawLine(boundingBox.TopSouthernLeft + Position, boundingBox.BottemSouthernLeft + Position, Color.yellow);
            Debug.DrawLine(boundingBox.TopSouthernLeft + Position, boundingBox.TopNorthernLeft + Position, Color.yellow);
            Debug.DrawLine(boundingBox.BottemNorthernLeft + Position, boundingBox.TopNorthernLeft + Position, Color.yellow);
            Debug.DrawLine(boundingBox.BottemNorthernLeft + Position, boundingBox.BottemSouthernLeft + Position, Color.yellow);
            Debug.DrawLine(boundingBox.BottemNorthernLeft + Position, boundingBox.BottemNorthernRight + Position, Color.yellow);
        }
#endif
    }


    public struct BoundingBox
    {
        public Vector3 TopNorthernLeft;
        public Vector3 BottemSouthernRight;
        public Vector3 TopNorthernRight;
        public Vector3 TopSouthernRight;
        public Vector3 BottemNorthernRight;
        public Vector3 TopSouthernLeft;
        public Vector3 BottemNorthernLeft;
        public Vector3 BottemSouthernLeft;

        public BoundingBox RAABB(float angle)
        {
            Vector3 RotatedTopNorthernLeft = RotatePoint(TopNorthernLeft, angle);
            Vector3 RotatedBottemSouthernRight = RotatePoint(BottemSouthernRight, angle);
            Vector3 RotatedTopNorthernRight = RotatePoint(TopNorthernRight, angle);
            Vector3 RotatedTopSouthernRight = RotatePoint(TopSouthernRight, angle);
            Vector3 RotatedBottemNorthernRight = RotatePoint(BottemNorthernRight, angle);
            Vector3 RotatedTopSouthernLeft = RotatePoint(TopSouthernLeft, angle);
            Vector3 RotatedBottemNorthernLeft = RotatePoint(BottemNorthernLeft, angle);
            Vector3 RotatedBottemSouthernLeft = RotatePoint(BottemSouthernLeft, angle);
            var box = new BoundingBox(RotatedTopNorthernLeft, RotatedBottemSouthernRight, RotatedTopNorthernRight,
                RotatedTopSouthernRight, RotatedBottemNorthernRight, RotatedTopSouthernLeft, RotatedBottemNorthernLeft,
                RotatedBottemSouthernLeft);
            return box;
        }

        private BoundingBox(Vector3 TopNorthernLeft, Vector3 BottemSouthernRight, Vector3 TopNorthernRight,
         Vector3 TopSouthernRight, Vector3 BottemNorthernRight, Vector3 TopSouthernLeft, Vector3 BottemNorthernLeft,
         Vector3 BottemSouthernLeft)
        {
            this.TopNorthernLeft = TopNorthernLeft;
            this.BottemSouthernRight = BottemSouthernRight;
            this.TopNorthernRight = TopNorthernRight;
            this.TopSouthernRight = TopSouthernRight;
            this.BottemNorthernRight =BottemNorthernRight;
            this.TopSouthernLeft = TopSouthernLeft;
            this.BottemNorthernLeft = BottemNorthernLeft;
            this.BottemSouthernLeft = BottemSouthernLeft;
        }
        
        public BoundingBox(Vector3 TopNorthernLeft, Vector3 BottemSouthernRight)
        {
            this.TopNorthernLeft = TopNorthernLeft;
            this.BottemSouthernRight = BottemSouthernRight;
            var Top = TopNorthernLeft.x;
            var Northern = TopNorthernLeft.y;
            var Left = TopNorthernLeft.z;
            var Bottem = BottemSouthernRight.x;
            var Southern = BottemSouthernRight.y;
            var Right = BottemSouthernRight.z;
            TopNorthernRight = new Vector3(Top, Northern, Right);
            TopSouthernRight = new Vector3(Top, Southern, Right);
            BottemNorthernRight = new Vector3(Bottem, Northern, Right);
            TopSouthernLeft = new Vector3(Top, Southern, Left);
            BottemNorthernLeft = new Vector3(Bottem, Northern, Left);
            BottemSouthernLeft = new Vector3(Bottem, Southern, Left);
        }

        private Vector3 RotatePoint(Vector3 point, float angleInRadians)
        {
            var rotatedPoint = point;
            rotatedPoint.x = Mathf.Cos(angleInRadians) * point.x - Mathf.Sin(angleInRadians) * point.z;
            rotatedPoint.z = Mathf.Sin(angleInRadians) * point.x + Mathf.Cos(angleInRadians) * point.z;
            return rotatedPoint;
        }
    }
}