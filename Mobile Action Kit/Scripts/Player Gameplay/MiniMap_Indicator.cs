using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MobileActionKit
{
    public class MiniMap_Indicator : MonoBehaviour
    {
        void Start()
        {
            if (MiniMap.instance != null)
            {
                if (!MiniMap.instance.AllIndicator.Contains(gameObject))
                {
                    MiniMap.instance.AllIndicator.Add(gameObject);
                }
            }
        }
    }
}