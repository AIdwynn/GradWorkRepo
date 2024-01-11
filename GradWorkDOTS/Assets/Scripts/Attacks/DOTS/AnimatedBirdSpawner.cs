using System.Collections;
using System.Collections.Generic;
using Gradwork.Attacks.DOTS;
using UnityEngine;
using Vital.ObjectPools;

public class AnimatedBirdSpawner : MonoBehaviour
{
    [SerializeField] private GameObject Animatedbird;
    void Awake()
    {
        new ObjectPoolManager().CreateObjectPool(Animatedbird, 20500, true);
        PoolNames.AnimatedGameObject = Animatedbird.name;
        
        Destroy(this.gameObject);
    }
}
