using UnityEngine;

namespace MobileActionKit
{
    public class SwitchGameControls : MonoBehaviour
    {
        public FirstPersonController FirstPersonControllerScript;
        public PcMouseLook[] PcMouseLookScripts;
        public GameObject PcControlsSettingsUI;
        public GameObject PcControls;
        public GameobjectToggler GameObjectTogglerScript;

        private bool isPcControlsCurrentlyActive = true;

        public void ToggleControls()
        {
            isPcControlsCurrentlyActive = !isPcControlsCurrentlyActive;

            // Toggle GameObjects
            PcControlsSettingsUI.SetActive(isPcControlsCurrentlyActive);
            PcControls.SetActive(isPcControlsCurrentlyActive);

            // Update GameObjectToggler state
            if (GameObjectTogglerScript != null)
            {
                GameObjectTogglerScript.AreGameObjectsCurrentlyActive = isPcControlsCurrentlyActive;

                if (isPcControlsCurrentlyActive)
                {
                   
                    GameObjectTogglerScript.CreateParentForAdditionalGameObjects();
                    GameObjectTogglerScript.ShouldActivateUI(false);
                }
                else
                {
                    GameObjectTogglerScript.ShouldActivateUI(true);
                    GameObjectTogglerScript.RemoveNestedGameObjects();
                }
              
            }

            // Enable/Disable Mouse Look Scripts
            foreach (var mouseLook in PcMouseLookScripts)
            {
                if (mouseLook != null)
                    mouseLook.enabled = isPcControlsCurrentlyActive;
            }

            // Update FirstPersonController settings
            if (FirstPersonControllerScript != null)
            {
                FirstPersonControllerScript.UseCustomLookScripts = isPcControlsCurrentlyActive;
            }
        }
    }
}