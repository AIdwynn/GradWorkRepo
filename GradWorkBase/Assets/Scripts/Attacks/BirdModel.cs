using UnityEngine;
using Vital.ObjectPools;

namespace Gradwork.Attacks
{
    public class BirdModel: IPoolableScript
    {
        public GameObject View { get; protected set; }
        public string Name
        {
            get => NameStatic;
        }
        
        public static string NameStatic
        {
            get => typeof(BirdModel).ToString();
        }

        bool IPoolableScript.IsViewActive
        {
            get
            {
                if(View == null)
                    return false;
                return View.activeSelf;
            }
        }

        public BirdModel SetView(GameObject view)
        {
            View = view;
            return this;
        }
        
        public BirdModel SetPosition(Vector3 position)
        {
            View.transform.position = position;
            return this;
        }
        
        public BirdModel SetActive(bool isActive)
        {
            View.SetActive(isActive);
            return this;
        }
    }
}