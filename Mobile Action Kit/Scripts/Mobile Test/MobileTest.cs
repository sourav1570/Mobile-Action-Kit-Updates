using UnityEngine.UI;
using UnityEngine;
using TMPro;

namespace MobileActionKit
{
    public class MobileTest : MonoBehaviour
    {
        [TextArea]
        [ContextMenuItem("Reset Description", "ResettingDescription")]
        public string ScriptInfo = "Add This Script Only When you want to test the game on Mobile Device.";

        public static MobileTest Instance;

        [System.Serializable]
        public class WeaponDebugger
        {
            public string WeaponName;
            public PlayerWeapon WeaponGunscript;
            public SniperScope WeaponScope;
            public string SimpleBullet;
            public string RealisticBullet;
        }

        public WeaponDebugger[] AllWeapons;

        public FirstPersonController FirstPersonControllerScript;
        public Dropdown ShootingOptionsUI;
        public Dropdown BulletTypesUI;
        public Toggle ScopeToggle;
        public TMP_InputField HipFireXSensitivity;
        public TMP_InputField HipFireYSensitivity;
        public TMP_InputField AimedXSensitivity;
        public TMP_InputField AimedYSensitivity;

        float HipFireXValue;
        float HipFireYValue;
        float AimedXValue;
        float AimedYValue;

        public void ResettingDescription()
        {
            ScriptInfo = "Add This Script Only When you want to test the game on Mobile Device.";
        }
        void Awake()
        {
            Instance = this;
        }
        public void StartTest()
        {
            foreach (WeaponDebugger w in AllWeapons)
            {
                if (w.WeaponGunscript != null)
                {
                    if (w.WeaponGunscript.gameObject.activeInHierarchy == true)
                    {
                        if (w.WeaponGunscript.ShootingMechanics.ShootingOption == PlayerWeapon.ShootingOptions.RaycastShooting)
                        {
                            ShootingOptionsUI.value = 0;
                        }
                        else
                        {
                            ShootingOptionsUI.value = 1;
                        }

                        if (w.WeaponGunscript.ShootingMechanics.ProjectileName == BulletTypesUI.options[0].text)
                        {
                            BulletTypesUI.value = 0;
                        }
                        else
                        {
                            BulletTypesUI.value = 1;
                        }


                        if (w.WeaponGunscript.ShootingFeatures.SniperScopeScript == null)
                        {
                            ScopeToggle.isOn = false;
                        }
                        else
                        {
                            ScopeToggle.isOn = true;
                        }
                    }
                }
            }
        }
        public void ActivateScope(bool IsOn)
        {
            foreach (WeaponDebugger w in AllWeapons)
            {
                if (w.WeaponScope != null)
                {
                    if (w.WeaponGunscript.gameObject.activeInHierarchy == true)
                    {
                        if (w.WeaponGunscript.ShootingFeatures.SniperScopeScript == null)
                        {
                            w.WeaponGunscript.ShootingFeatures.SniperScopeScript = w.WeaponScope;
                            ScopeToggle.isOn = true;
                        }
                        else
                        {
                            w.WeaponGunscript.ShootingFeatures.SniperScopeScript = null;
                            ScopeToggle.isOn = false;
                        }
                    }
                }
            }
        }
        public void SwitchShooting()
        {
            foreach (WeaponDebugger w in AllWeapons)
            {
                if (w.WeaponGunscript != null)
                {
                    if (w.WeaponGunscript.gameObject.activeInHierarchy == true)
                    {
                        if (ShootingOptionsUI.value == 0)
                        {
                            w.WeaponGunscript.ShootingMechanics.ShootingOption = PlayerWeapon.ShootingOptions.RaycastShooting;
                        }
                        else
                        {
                            w.WeaponGunscript.ShootingMechanics.ShootingOption = PlayerWeapon.ShootingOptions.ProjectileShooting;
                        }
                    }
                }
            }
        }
        public void SwitchBulletType()
        {
            foreach (WeaponDebugger w in AllWeapons)
            {
                if (w.WeaponGunscript != null)
                {
                    if (w.WeaponGunscript.gameObject.activeInHierarchy == true)
                    {
                        if (BulletTypesUI.value == 0)
                        {
                            w.WeaponGunscript.ShootingMechanics.ProjectileName = w.RealisticBullet;

                        }
                        else
                        {
                            w.WeaponGunscript.ShootingMechanics.ProjectileName = w.SimpleBullet;
                        }
                    }
                }
            }
        }
        public void OpenDeveloperOptions(GameObject DeveloperMode)
        {
            DeveloperMode.SetActive(true);
        }
        public void SaveSettings(GameObject DeveloperMode)
        {
            DeveloperMode.SetActive(false);
        }
        public void HipFireXSensitivityValue()
        {
            HipFireXValue = float.Parse(HipFireXSensitivity.text);
            FirstPersonControllerScript.DefaultXSenstivity = HipFireXValue;
            FirstPersonControllerScript.TouchpadLook.XSensitivity = HipFireXValue;
        }
        public void HipFireYSensitivityValue()
        {
            HipFireYValue = float.Parse(HipFireYSensitivity.text);
            FirstPersonControllerScript.DefaultYSenstivity = HipFireYValue;
            FirstPersonControllerScript.TouchpadLook.YSensitivity = HipFireYValue;
        }
        public void AimedXSensitivityValue()
        {
            AimedXValue = float.Parse(AimedXSensitivity.text);
            FirstPersonControllerScript.WeaponAimedTouchPadLookValues.XSensitivity = AimedXValue;
        }
        public void AimedYSensitivityValue()
        {
            AimedYValue = float.Parse(AimedYSensitivity.text);
            FirstPersonControllerScript.WeaponAimedTouchPadLookValues.YSensitivity = AimedYValue;
        }
    }
}