using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MobileActionKit
{
    public class CrossHair : MonoBehaviour
    {
        [TextArea]     
        public string ScriptInfo = "This Script Controls The Reticle Lines of Cross Hair";
        [Space(10)]

        [Space(10)]
        [Tooltip("Reference to the player's crouch script.")]
        public Crouch CrouchScript;

        [Tooltip("The parent RectTransform that holds the reticle lines.")]
        public RectTransform ReticleLinesParent;

        [Tooltip("Default width of the crosshair when standing.")]
        public float StandDefaultSizeX;

        [Tooltip("Default height of the crosshair when standing.")]
        public float StandDefaultSizeY;

        [Tooltip("Maximum width of the crosshair when standing.")]
        public float StandMaxSizeX;

        [Tooltip("Maximum height of the crosshair when standing.")]
        public float StandMaxSizeY;

        [Tooltip("Speed at which the crosshair expands when standing.")]
        public float StandExpandSpeed;

        [Tooltip("Speed at which the crosshair resets when standing.")]
        public float StandResetSpeed;

        [Tooltip("Default width of the crosshair when crouching.")]
        public float CrouchDefaultSizeX;

        [Tooltip("Default height of the crosshair when crouching.")]
        public float CrouchDefaultSizeY;

        [Tooltip("Maximum width of the crosshair when crouching.")]
        public float CrouchMaxSizeX;

        [Tooltip("Maximum height of the crosshair when crouching.")]
        public float CrouchMaxSizeY;

        [Tooltip("Speed at which the crosshair expands when crouching.")]
        public float CrouchExpandSpeed;

        [Tooltip("Speed at which the crosshair resets when crouching.")]
        public float CrouchResetSpeed;

        private float currentSizeX;
        private float currentSizeY;

        bool Shoot = false;

     
        private void Start()
        {
            ReticleLinesParent = GetComponent<RectTransform>();
        }
        public void UpdateCrossHair(bool IsShoot)
        {
            Shoot = IsShoot;
        }
        void Update()
        {
            if (CrouchScript != null)
            {
                if (CrouchScript.IsCrouching == false)
                {
                    if (Shoot)
                    {
                        currentSizeX = Mathf.Lerp(ReticleLinesParent.sizeDelta.x, StandMaxSizeX, Time.deltaTime * StandExpandSpeed);
                        currentSizeY = Mathf.Lerp(ReticleLinesParent.sizeDelta.y, StandMaxSizeY, Time.deltaTime * StandExpandSpeed);
                    }
                    else
                    {
                        currentSizeX = Mathf.Lerp(ReticleLinesParent.sizeDelta.x, StandDefaultSizeX, Time.deltaTime * StandResetSpeed);
                        currentSizeY = Mathf.Lerp(ReticleLinesParent.sizeDelta.y, StandDefaultSizeY, Time.deltaTime * StandResetSpeed);
                    }
                }
                else
                {
                    if (Shoot)
                    {
                        currentSizeX = Mathf.Lerp(ReticleLinesParent.sizeDelta.x, CrouchMaxSizeX, Time.deltaTime * CrouchExpandSpeed);
                        currentSizeY = Mathf.Lerp(ReticleLinesParent.sizeDelta.y, CrouchMaxSizeY, Time.deltaTime * CrouchExpandSpeed);
                    }
                    else
                    {
                        currentSizeX = Mathf.Lerp(ReticleLinesParent.sizeDelta.x, CrouchDefaultSizeX, Time.deltaTime * CrouchResetSpeed);
                        currentSizeY = Mathf.Lerp(ReticleLinesParent.sizeDelta.y, CrouchDefaultSizeY, Time.deltaTime * CrouchResetSpeed);
                    }
                }
            }
            else
            {
                if (Shoot)
                {
                    currentSizeX = Mathf.Lerp(ReticleLinesParent.sizeDelta.x, StandMaxSizeX, Time.deltaTime * StandExpandSpeed);
                    currentSizeY = Mathf.Lerp(ReticleLinesParent.sizeDelta.y, StandMaxSizeY, Time.deltaTime * StandExpandSpeed);
                }
                else
                {
                    currentSizeX = Mathf.Lerp(ReticleLinesParent.sizeDelta.x, StandDefaultSizeX, Time.deltaTime * StandResetSpeed);
                    currentSizeY = Mathf.Lerp(ReticleLinesParent.sizeDelta.y, StandDefaultSizeY, Time.deltaTime * StandResetSpeed);
                }

            }

            ReticleLinesParent.sizeDelta = new Vector2(currentSizeX, currentSizeY);
        }
    }
}
