using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstaclesModel
{
    public Vector3 Position
    {
        get { return _position; }
        protected set
        {
            View.transform.position = value;
            _position = value;
        }
    }

    public float Radius { get; protected set; }
    public GameObject View { get; protected set; }

    private Vector3 _position;

    public ObstaclesModel SetView(GameObject view)
    {
        View = view;
        return this;
    }

    public ObstaclesModel SetPosition(Vector3 position)
    {
        Position = position;
        return this;
    }

    public ObstaclesModel SetRadius(float radius)
    {
        this.Radius = radius;
        return this;
    }
}