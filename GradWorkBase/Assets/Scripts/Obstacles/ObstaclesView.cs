using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstaclesView : MonoBehaviour
{
    [SerializeField] private float radius;
    
    public event EventHandler<ObstaclesCreatedEventArgs> ObstaclesCreated;

    private void Start()
    {
        var model = new ObstaclesModel().SetPosition(transform.position).SetRadius(radius).SetView(gameObject);
        SCR_EventHelper.TrySendEvent(ObstaclesCreated, this, new ObstaclesCreatedEventArgs(model));
        Destroy(this);
    }
}

public class ObstaclesCreatedEventArgs : EventArgs
{
    public ObstaclesModel ObstaclesModel { get; private set; }

    public ObstaclesCreatedEventArgs(ObstaclesModel obstaclesModel)
    {
        ObstaclesModel = obstaclesModel;
    }
}
