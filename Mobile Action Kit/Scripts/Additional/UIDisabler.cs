using UnityEngine.UI;
using UnityEngine;

namespace MobileActionKit
{
    public class UIDisabler : MonoBehaviour
    {
        public GameObject[] GameObjectsToDeactivate;
        public Image[] ImagesToDisable;

        void Start()
        {
            for (int x = 0; x < GameObjectsToDeactivate.Length; x++)
            {
                GameObjectsToDeactivate[x].gameObject.SetActive(false);
            }

            for (int x = 0; x < ImagesToDisable.Length; x++)
            {
                ImagesToDisable[x].enabled = false;
            }
        }
    }
}
