using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MobileActionKit
{
    public class ActivateScopes : MonoBehaviour
    {
        public PlayerWeapon PlayerWeaponScript;

        public void ActivateNonSniperScope()
        {
            PlayerWeaponScript.ShootingFeatures.UseSniperScopeUI = false;
        }

        public void ActivateSniperScope()
        {
            PlayerWeaponScript.ShootingFeatures.UseSniperScopeUI = true;
        }
    }
}