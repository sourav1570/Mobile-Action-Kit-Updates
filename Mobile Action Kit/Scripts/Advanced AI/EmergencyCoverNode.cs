using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace MobileActionKit
{
    public class EmergencyCoverNode : MonoBehaviour
    {
        [TextArea]
        public string ScriptInfo = "Emergency Cover is designed for the non combat cover behaviour. " +
            "For example  when Ai agents are under the threat but can not detect and locate the attackers which is required for them to enter the combat state." +
            "In this case they will sprint towards emergency cover and will hide there for some specified amount of time before moving out towards supposed enemy position by performing " +
            "movement between emergency covers and sprinting in a zigzag pattern to increase their survival chances. ";

        [System.Serializable]
        public enum Pose
        {
            CrouchEmergencyCover,
            StandEmergencyCover
        }

        public Transform[] EmergencyCoverShootingPoints;
        public Pose EmergencyCoverType;

        [HideInInspector]
        public float Fov = 120;

        [HideInInspector]
        public bool DebugCoverValidation = false;

        Vector3 dir;

        [HideInInspector]
        public float Angle;

        [HideInInspector]
        public bool IsAlreadyRegistered = false;

        [HideInInspector]
        public float RotationIHaveToDo;

        [HideInInspector]
        public TextMeshProUGUI spawnedText; // Reference to the spawned text object

        [HideInInspector]
        public DebugCoverState DebugCoverStateScript;
        bool DoOnce = false;
        int Index;

        private void Start()
        {
            if (transform.parent != null)
            {
                DebugCoverStateScript = transform.parent.GetComponent<DebugCoverState>();
            }

            gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
        }
        public void CheckForTargetPosition(Transform enemy)
        {
            dir = enemy.transform.position - this.transform.position;

            Angle = Vector3.Angle(dir, this.transform.forward);

            if (Angle < Fov)
            {
                DebugCoverValidation = false;
            }
            else
            {
                DebugCoverValidation = true;
            }

        }
        //public void CheckRotations(Transform enemy)
        //{
        //    // Calculate the direction from the current object's position to the enemy's position
        //    Vector3 direction = enemy.transform.position - transform.position;

        //    // Convert the direction to local space
        //    Vector3 localDirection = transform.InverseTransformDirection(direction);

        //    // Calculate the local rotation needed to point towards the target on the Y-axis
        //    float localRotation = Mathf.Atan2(localDirection.x, localDirection.z) * Mathf.Rad2Deg;

        //    // Log the positive local rotation value on the Y-axis
        //    float positiveRotation = Mathf.Abs(localRotation);
        //    RotationIHaveToDo = positiveRotation;

        //}
        public void Info(Transform enemy, Targets Team)
        {
            if (DebugCoverStateScript != null && DoOnce == false)
            {
                if(DebugCoverStateScript.EnableDebugging == true)
                {
                    for (int x = 0; x < DebugCoverStateScript.DebugTextColors.Count; x++)
                    {
                        if (DebugCoverStateScript.DebugTextColors[x].TeamName == Team.MyTeamID)
                        {
                            Index = x;
                        }
                    }

                    spawnedText.gameObject.SetActive(true);
                    spawnedText.text = "OCCUPIED";

                    spawnedText.color = DebugCoverStateScript.DebugTextColors[Index].OccupyingTextColor;

                    // We using for loop because we have to make other cover points either to be invalid or vacant while making sure that current cover point is occupied for the Ai agent.
                    for (int x = 0; x < DebugCoverStateScript.emergencycoverNodes.Length; x++)
                    {
                        if (DebugCoverStateScript.emergencycoverNodes[x].gameObject != spawnedText.gameObject)
                        {
                            DebugCoverStateScript.emergencycoverNodes[x].GetComponent<EmergencyCoverNode>().CheckForTargetPosition(enemy);

                            if (DebugCoverStateScript.emergencycoverNodes[x].GetComponent<EmergencyCoverNode>().spawnedText.text != "OCCUPIED")
                            {
                                if (DebugCoverStateScript.emergencycoverNodes[x].GetComponent<EmergencyCoverNode>().DebugCoverValidation == false)
                                {
                                    DebugCoverStateScript.emergencycoverNodes[x].GetComponent<EmergencyCoverNode>().spawnedText.gameObject.SetActive(true);
                                    DebugCoverStateScript.emergencycoverNodes[x].GetComponent<EmergencyCoverNode>().spawnedText.text = "INVALID";
                                    DebugCoverStateScript.emergencycoverNodes[x].GetComponent<EmergencyCoverNode>().spawnedText.color = DebugCoverStateScript.DebugTextColors[Index].InvalidTextColor;
                                }
                                else
                                {
                                    DebugCoverStateScript.emergencycoverNodes[x].GetComponent<EmergencyCoverNode>().spawnedText.gameObject.SetActive(true);
                                    DebugCoverStateScript.emergencycoverNodes[x].GetComponent<EmergencyCoverNode>().spawnedText.text = "VACANT";
                                    DebugCoverStateScript.emergencycoverNodes[x].GetComponent<EmergencyCoverNode>().spawnedText.color = DebugCoverStateScript.DebugTextColors[Index].VacantTextColor;
                                }
                            }
                        }
                    }

                    StartCoroutine(TextStayTime());
                    DoOnce = true;
                }
               
            }

        }
        public void DeactivateOccupiedText()
        {
            if (spawnedText != null)
            {
                spawnedText.text = "VACANT";
                spawnedText.gameObject.SetActive(false);
            }
        }
        IEnumerator TextStayTime()
        {
            float Randomise = Random.Range(DebugCoverStateScript.MinDebugTextVisibilityTime, DebugCoverStateScript.MaxDebugTextVisibilityTime);
            yield return new WaitForSeconds(Randomise);
            for (int x = 0; x < DebugCoverStateScript.emergencycoverNodes.Length; x++)
            {
                if (DebugCoverStateScript.emergencycoverNodes[x].GetComponent<EmergencyCoverNode>().spawnedText.text != "OCCUPIED")
                {
                    DebugCoverStateScript.emergencycoverNodes[x].GetComponent<EmergencyCoverNode>().spawnedText.gameObject.SetActive(false);
                }
            }
            DoOnce = false;

        }
    }

}
