using System.Collections;
using System.Collections.Generic;
using MobileActionKit;
using UnityEngine;


namespace MobileActionKit
{
    public class IncreasePlayerHealth : MonoBehaviour
    {
        public float HealthToIncrease = 25f;

        public void IncreaseHealth()
        {
            if (PlayerHealth.instance != null)
            {
                PlayerHealth.instance.PlayerHealthbar.Curvalue += HealthToIncrease;
                PlayerHealth.instance.CheckPlayerHealth();
            }
        }
        public void NoFunction()
        {

        }
    }
}