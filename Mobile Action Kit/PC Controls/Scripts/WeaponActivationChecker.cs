using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MobileActionKit
{
    public class WeaponActivationChecker : MonoBehaviour
    {
        PlayerWeapon PlayerWeaponComponent;

        private void Awake()
        {
            if(GetComponent<PlayerWeapon>() != null)
            {
                PlayerWeaponComponent = GetComponent<PlayerWeapon>();
            }
        }
        private void OnEnable()
        {
            if (MouseScrollWeaponZoom.instance != null && PlayerWeaponComponent != null)
            {
                MouseScrollWeaponZoom.instance.UpdatePlayerWeapon(PlayerWeaponComponent);
            }
            if (MouseControls.instance != null && PlayerWeaponComponent != null)
            {
                MouseControls.instance.StopFunctioning = false;
            }
        }
    }
}