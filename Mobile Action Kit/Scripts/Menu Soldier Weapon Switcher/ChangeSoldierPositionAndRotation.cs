using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MobileActionKit
{
    public class ChangeSoldierPositionAndRotation : MonoBehaviour
    {
        [TextArea]
        public string ScriptInfo = "This script allows you to change the position and rotation of a Soldier GameObject and its Parent dynamically when enabled.";

        [Tooltip("The GameObject that will act as the new parent for the Soldier Parent.")]
        public GameObject ParentGameObject;

        [Tooltip("The current parent of the Soldier. This will be reparented to 'ParentGameObject'.")]
        public GameObject SoldierParent;

        [Tooltip("The local position of 'SoldierParent' after reparenting.")]
        public Vector3 Position;

        [Tooltip("The local rotation of 'SoldierParent' after reparenting.")]
        public Vector3 Rotation;

        [Tooltip("The Soldier GameObject that will be repositioned and rotated.")]
        public GameObject Soldier;

        [Tooltip("The local position of the Soldier relative to 'SoldierParent'.")]
        public Vector3 SoldierPosition;

        [Tooltip("The local rotation of the Soldier relative to 'SoldierParent'.")]
        public Vector3 SoldierRotation;

        private void OnEnable()
        {
            SoldierParent.transform.parent = ParentGameObject.transform;
            SoldierParent.transform.localPosition = Position;
            SoldierParent.transform.localEulerAngles = Rotation;

            Soldier.transform.localPosition = SoldierPosition;
            Soldier.transform.localEulerAngles = SoldierRotation;

            if (Soldier.GetComponent<RotateAndReset_GameObject>() != null)
            {
                Soldier.GetComponent<RotateAndReset_GameObject>().defaultRotation = Soldier.transform.localRotation;
            }
         }
    }
}