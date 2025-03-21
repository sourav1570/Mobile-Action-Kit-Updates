using UnityEngine;

namespace MobileActionKit
{
    public class WeaponActivationCheckerForConsoles : MonoBehaviour
    {
        PlayerWeapon PlayerWeaponComponent;

        private void Awake()
        {
            if (GetComponent<PlayerWeapon>() != null)
            {
                PlayerWeaponComponent = GetComponent<PlayerWeapon>();
            }
        }
        private void OnEnable()
        {
            if (WeaponZoomControlForConsoles.instance != null && PlayerWeaponComponent != null)
            {
                WeaponZoomControlForConsoles.instance.UpdatePlayerWeapon(PlayerWeaponComponent);
            }
        }
    }
}