using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// This Script is Responsible For Different Settings in Game
namespace MobileActionKit
{

    public class FPSClockArrow : MonoBehaviour
    {
        public Transform clockArrowPoint;

        private float resetTimer;

        // Update is called once per frame
        void Update()
        {
            RotateArrow();
        }

        void RotateArrow()
        {
            transform.Rotate(0, 6, 0);
        }
    }

}