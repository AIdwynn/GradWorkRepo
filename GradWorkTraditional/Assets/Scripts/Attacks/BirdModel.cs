using System;
using UnityEngine;
using Vital.ObjectPools;
using Vital.Spatial_Partitioning;
using Grid = Vital.Spatial_Partitioning.Grid;
using Matrix4x4 = UnityEngine.Matrix4x4;
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
        protected BoundingBox Collider { get; set; }

        protected BoundingBox RCollider
        {
            get
            {
                var boundingBox = Collider.RAABB(Rotation.y);
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
            Collider = new BoundingBox(topNorthernCorner, -topNorthernCorner);
            var max = Mathf.Max(topNorthernCorner.x, Mathf.Max(topNorthernCorner.y, topNorthernCorner.z));
            var AABBCorner = new Vector3(max, max, max);
            AABB = new BoundingBox(AABBCorner, -AABBCorner);
        }

        public bool CheckForCollision(Vector3 playerPosition, float playerRadius)
        {
            if (AABB.CheckCollisionWithAABB(Position, playerPosition, playerRadius))
            {
                if(Collider.CheckCollisionsWithRAABB(Position, Rotation, playerPosition, playerRadius))
                {
                    OnHit();
                    return true;
                }
            }

            return false;
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
            var boundingBox = RCollider;

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

            boundingBox = AABB;
            
            Debug.DrawLine(boundingBox.TopNorthernRight + Position, boundingBox.BottemNorthernRight + Position, Color.magenta);
            Debug.DrawLine(boundingBox.TopNorthernRight + Position, boundingBox.TopSouthernRight + Position, Color.magenta);
            Debug.DrawLine(boundingBox.TopNorthernRight + Position, boundingBox.TopNorthernLeft + Position, Color.magenta);
            Debug.DrawLine(boundingBox.BottemSouthernRight + Position, boundingBox.BottemSouthernLeft + Position, Color.magenta);
            Debug.DrawLine(boundingBox.BottemSouthernRight + Position, boundingBox.BottemNorthernRight + Position, Color.magenta);
            Debug.DrawLine(boundingBox.BottemSouthernRight + Position, boundingBox.TopSouthernRight + Position, Color.magenta);
            Debug.DrawLine(boundingBox.TopSouthernLeft + Position, boundingBox.TopSouthernRight + Position, Color.magenta);
            Debug.DrawLine(boundingBox.TopSouthernLeft + Position, boundingBox.BottemSouthernLeft + Position, Color.magenta);
            Debug.DrawLine(boundingBox.TopSouthernLeft + Position, boundingBox.TopNorthernLeft + Position, Color.magenta);
            Debug.DrawLine(boundingBox.BottemNorthernLeft + Position, boundingBox.TopNorthernLeft + Position, Color.magenta);
            Debug.DrawLine(boundingBox.BottemNorthernLeft + Position, boundingBox.BottemSouthernLeft + Position, Color.magenta);
            Debug.DrawLine(boundingBox.BottemNorthernLeft + Position, boundingBox.BottemNorthernRight + Position, Color.magenta);
        }
#endif
    }


    public struct BoundingBox
    {
        public Vector3 Size;
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
            angle *= Mathf.Deg2Rad;
            Vector3 RotatedTopNorthernLeft = RotatePoint(TopNorthernLeft, angle );
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

            Size = new Vector3(TopNorthernLeft.x - BottemSouthernRight.x,
                TopNorthernLeft.y - BottemSouthernRight.y,
                TopNorthernLeft.z - BottemSouthernRight.z);
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
            
            Size = new Vector3(TopNorthernLeft.x - BottemSouthernRight.x,
                TopNorthernLeft.y - BottemSouthernRight.y,
                TopNorthernLeft.z - BottemSouthernRight.z);
        }

        private Vector3 RotatePoint(Vector3 point, float angleInRadians)
        {
            var rotatedPoint = point;
            rotatedPoint.z = Mathf.Cos(angleInRadians) * point.z - Mathf.Sin(angleInRadians) * point.x;
            rotatedPoint.x = Mathf.Sin(angleInRadians) * point.z + Mathf.Cos(angleInRadians) * point.x;
            return rotatedPoint;
        }

        public bool CheckCollisionWithAABB(Vector3 thisPosition, Vector3 otherPosition, float radius)
        {
            var centeredPoint = otherPosition - thisPosition;
            var direction = (thisPosition - centeredPoint).normalized;
            centeredPoint += radius * direction;
            
            return centeredPoint.x >= BottemSouthernRight.x && centeredPoint.x <= TopNorthernLeft.x &&
                   centeredPoint.y >= BottemSouthernRight.y && centeredPoint.y <= TopNorthernLeft.y &&
                   centeredPoint.z >= BottemSouthernRight.z && centeredPoint.z <= TopNorthernLeft.z;;
        }

        public bool CheckCollisionsWithRAABB(Vector3 thisPosition, Vector3 Rotation, Vector3 otherPosition, float radius)
        {
            var centeredPoint = otherPosition - thisPosition;
            var direction = (thisPosition - centeredPoint).normalized;
            centeredPoint += radius * direction;
            
            Matrix4x4 beamRotationMatrix = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(Rotation), Vector3.one);

            centeredPoint = beamRotationMatrix.MultiplyPoint3x4(centeredPoint);
            
            return centeredPoint.x >= BottemSouthernRight.x && centeredPoint.x <= TopNorthernLeft.x &&
                   centeredPoint.y >= BottemSouthernRight.y && centeredPoint.y <= TopNorthernLeft.y &&
                   centeredPoint.z >= BottemSouthernRight.z && centeredPoint.z <= TopNorthernLeft.z;;
            
            return false;
        }
        
    }
}