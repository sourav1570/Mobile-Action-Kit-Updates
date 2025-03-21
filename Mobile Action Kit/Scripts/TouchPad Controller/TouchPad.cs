using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MobileActionKit
{
    public class TouchPad : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [HideInInspector]
        public Vector2 TouchDistance;
        [HideInInspector]
        public Vector2 OldPointerID;
        [HideInInspector]
        protected int NewPointerId;
        [HideInInspector]
        public bool Hold;

        bool CreateNewTouchRef = false;

        void Update()
        {
            if (Input.GetAxis("Mouse X") < 0 || Input.GetAxis("Mouse X") > 0 || Input.GetAxis("Mouse Y") > 0 || Input.GetAxis("Mouse Y") < 0)
            {
                if (PlayerManager.instance != null)
                {
                    if(PlayerManager.instance.CurrentHoldingPlayerWeapon != null)
                    {
                        if (PlayerManager.instance.CurrentHoldingPlayerWeapon.ShootingFeatures.CanRotatePlayerWithPressedFireButton == true)
                        {
                            if (PlayerManager.instance.IsShooting == true)
                            {
                                Hold = true;
                                if (CreateNewTouchRef == false)
                                {
                                    OldPointerID = Input.mousePosition;
                                    //  TouchDistance = new Vector2();
                                    CreateNewTouchRef = true;
                                }
                            }
                        }
                    }
                
                }
            }
            if (Hold)
            {
                if (NewPointerId >= 0 && NewPointerId < Input.touches.Length)
                {
                    TouchDistance = Input.touches[NewPointerId].position - OldPointerID;
                    OldPointerID = Input.touches[NewPointerId].position;
                }
                else
                {
                    TouchDistance = new Vector2(Input.mousePosition.x, Input.mousePosition.y) - OldPointerID;
                    OldPointerID = Input.mousePosition;
                }
            }
            else
            {
                CreateNewTouchRef = false;
                TouchDistance = new Vector2();
            }
        }
        public void OnPointerDown(PointerEventData eventData)
        {
            PlayerManager.instance.IsShooting = false;
            Hold = true;
            NewPointerId = eventData.pointerId;
            OldPointerID = eventData.position;
        }
        public void OnPointerUp(PointerEventData eventData)
        {
            Hold = false;
        }
    }
}