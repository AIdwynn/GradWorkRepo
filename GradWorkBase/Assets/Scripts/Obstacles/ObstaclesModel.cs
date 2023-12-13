using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstaclesModel
{
    public Vector3 Position { get; protected set; }
    public float radius { get; protected set; }
    public GameObject View { get; protected set; }
    
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
        this.radius = radius;
        return this;
    }
}
