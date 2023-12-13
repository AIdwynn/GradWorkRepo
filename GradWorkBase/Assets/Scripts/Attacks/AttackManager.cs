using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vital.ObjectPools;

namespace Gradwork.Attacks
{
    public class AttackManager
    {
        List<BirdModel> birdModels;
        public AttackManager()
        {
            ObjectPoolManager.Instance.TryGetScriptPool<BirdModel>(BirdModel.NameStatic, out var objectpool);
            birdModels = objectpool.Pool;
        }
        public void Update()
        {
            foreach (var model in birdModels)
            {
                if(model.IsViewActive)
                {
                    model.SetPosition(model.Position + (model.Speed * Time.deltaTime * model.Transform.forward));
                    model.TimeAlive += Time.deltaTime;
                }
            }
        }
    }
}