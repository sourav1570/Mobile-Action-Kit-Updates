using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MobileActionKit
{
    public class MiniMap : MonoBehaviour
    {
        [TextArea]
        public string ScriptInfo = "The purpose of a minimap script is to provide player with a smaller, simplified version of the game world that helps them navigate and understand their surroundings.";
        
        public static MiniMap instance;

        [Tooltip("Enables or disables the MiniMap functionality. If unchecked, the MiniMap will not be displayed.")]
        public bool EnableMiniMap = true;

        [Tooltip("Dedicated orthographic 'Mini_Map' camera from hierarchy is to be placed into this field.")]
        public Camera MiniMapCamera;
       
        [Tooltip("Player root gameObject is to be placed into this field.")]
        public Transform PlayerTransform;

        [Tooltip("Toggles whether the MiniMap rotates to match the player's facing direction.")]
        public bool RotateMinimapWithPlayer = false;

        [Tooltip("GameObject for the small-sized MiniMap view.")]
        public GameObject SmallMap;

        [Tooltip("GameObject for the enlarged MiniMap view.")]
        public GameObject BigMap;

        [Tooltip("The orthographic size of the MiniMap camera when in the small view.")]
        public float SmallMiniMapCameraSize = 60f;

        [Tooltip("The orthographic size of the MiniMap camera when in the big view.")]
        public float BigMiniMapCameraSize = 400f;

        [Tooltip("The size of the indicators on the MiniMap when in the small view.")]
        public float SmallMiniMapIndicatorSize = 15f;

        [Tooltip("The size of the indicators on the MiniMap when in the big view.")]
        public float BigMiniMapIndicatorSize = 130f;

        [Tooltip("An array of UI buttons used for interacting with the MiniMap (e.g., switching between small and big map views).")]
        public Button[] MiniMapUIButtons;

        bool OpenMap = false;
        Vector3 rot;

        [HideInInspector]
        public List<GameObject> AllIndicator = new List<GameObject>();

        private void Awake()
        {
            if(instance == null)
            {
                instance = this;
            }

            if (EnableMiniMap == true)
            {
                SmallMap.SetActive(true);
            }
            else
            {
                SmallMap.SetActive(false);
            }
        }
        void Start()
        {
            rot = MiniMapCamera.transform.localEulerAngles;
            for (int x = 0; x < MiniMapUIButtons.Length; x++)
            {
                MiniMapUIButtons[x].onClick.AddListener(SwitchMap);
            }
        }
        private void LateUpdate()
        {
            Vector3 newposition = PlayerTransform.position;
            newposition.y = MiniMapCamera.transform.position.y;
            MiniMapCamera.transform.position = newposition;

            if (RotateMinimapWithPlayer == true)
            {             
                MiniMapCamera.transform.rotation = Quaternion.Euler(rot.x, PlayerTransform.eulerAngles.y, 0f);
            }
        }
        public void SwitchMap()
        {
            if (OpenMap == false)
            {
                SmallMap.SetActive(false);
                BigMap.SetActive(true);
                //Vector3 temp = MiniMapCamera.transform.localPosition;
                //temp.x = CameraZoomOutPosition.x;
                //temp.y = CameraZoomOutPosition.y;
                //temp.z = CameraZoomOutPosition.z;
                //MiniMapCamera.transform.localPosition = temp;
                MiniMapCamera.orthographicSize = BigMiniMapCameraSize;
                for(int x = 0;x < AllIndicator.Count; x++)
                {
                    if (AllIndicator[x] == null)
                    {
                        AllIndicator.Remove(AllIndicator[x]);
                    }
                    else
                    {
                        AllIndicator[x].transform.localScale = new Vector3(BigMiniMapIndicatorSize, BigMiniMapIndicatorSize, BigMiniMapIndicatorSize);
                    }
                }
                OpenMap = true;
            }
            else
            {
                SmallMap.SetActive(true);
                BigMap.SetActive(false);
                //Vector3 temp = MiniMapCamera.transform.localPosition;
                //temp.x = CameraZoomInPostion.x;
                //temp.y = CameraZoomInPostion.y;
                //temp.z = CameraZoomInPostion.z;
             
                //MiniMapCamera.transform.localPosition = temp;
                MiniMapCamera.orthographicSize = SmallMiniMapCameraSize;
                for (int x = 0; x < AllIndicator.Count; x++)
                {
                    if (AllIndicator[x] == null)
                    {
                        AllIndicator.Remove(AllIndicator[x]);
                    }
                    else
                    {
                        AllIndicator[x].transform.localScale = new Vector3(SmallMiniMapIndicatorSize, SmallMiniMapIndicatorSize, SmallMiniMapIndicatorSize);
                    }
                }
                OpenMap = false;
            }
        }

    }
}