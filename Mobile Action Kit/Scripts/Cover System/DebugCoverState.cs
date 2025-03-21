using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace MobileActionKit
{
    public class DebugCoverState : MonoBehaviour
    {
        [TextArea]
        public string ScriptInfo = "The 'DebugCoverState' script is designed to assist in debugging the cover system, specifically for Humanoid AI agents. " +
            "It provides visual feedback on the state of cover points (vacant, occupied, or invalid) by displaying debug information using TextMeshPro components.";

        [Tooltip("Toggle to activate or deactivate debugging features.")]
        public bool EnableDebugging = false;
        [Tooltip("Drag and drop TextMeshPro prefab named 'DebugInfoUI_Covers & FirePoints' from the project window into this field. " +
            "This field is responsible for debugging cover state in a form of text whether it is vacant or occupied or invalid. " +
            "It is displaying the responses of the cover points to AI agent requests. " +
            "Debug text is displayed for specified amount of time right after request is made from Ai agent for available cover points of this assembly. " +
            "The shorter you set the time for the debug text to stay the more subsequent responses it will be able to display. ")]
        public GameObject DebugInfoTextUI;

        [Tooltip("Sets the position of debugging text relative to the cover node.")]
        public Vector3 DebugInfoTextUIOffset = new Vector3(0f, 2f, 0f);

        [Tooltip("Minimum Time of Debug Text Visibility. " +
            "If you will configure the text display time to be very short then it will have better chance to indicate more responses from Cover Points to AI agents for available covers of this cover assembly.")]
        public float MinDebugTextVisibilityTime = 2f;
        [Tooltip("Maximum Time of Debug Text Visibility. " +
        "If you will configure the text display time to be very short then it will have better chance to indicate more responses from Cover Points to AI agents for available covers of this cover assembly.")]
        public float MaxDebugTextVisibilityTime = 4f;

        [System.Serializable]
        public class TextColors
        {
            [Tooltip("Type in the name of the team you came up with into this field.")]
            public string TeamName;
            [Tooltip("This text is displayed on top of cover point as soon as  Ai agent made a request to this cover assembly and got approved with a cover point. " +
                "Text will  start to be displayed immediately after cover point gets assigned to Ai agent even before said Agent will reach that cover point. ")]
            public Color OccupyingTextColor;
            [Tooltip("'Vacant text is displayed right after Ai agent's request for the available cover points on top of the points not chosen by AI agent but are still valid for the time being. " +
                "In this case another Ai agent from the same team might make his own request to this cover assembly for the cover and will be provided with those remaining 'Vacant' cover points for him to choose the most suitable one to be occupied.")]
            public Color VacantTextColor;

            [Tooltip("Invalid text is displayed right after Ai agent's request for the available cover points on top of the points when they are invalid and can't be registered at that moment." +
                "It will display on those cover points which are invalid at that moment.")]
            public Color InvalidTextColor;
        }

        [Tooltip("Configure text colors based on the state of the cover points.")]
        public List<TextColors> DebugTextColors = new List<TextColors>();

        GameObject canvasfound;
        TextMeshProUGUI[] DebugTextsForCombatCover;
        TextMeshProUGUI[] DebugTextsForEmergencyCover;

        [HideInInspector]
        public Transform[] combatcoverNodes;
        int combatcount;
        int assignIndexforcombatcover;

        [HideInInspector]
        public Transform[] emergencycoverNodes;
        int emergencycovercount;
        int assignIndexforemergencycover;

        private void Awake()
        {
#if UNITY_EDITOR
            if (EnableDebugging == true)
            {
                canvasfound = GameObject.FindGameObjectWithTag("Canvas3D");
                for (int x = 0; x < transform.childCount; x++)
                {
                    if (transform.GetChild(x).GetComponent<CoverNode>() != null)
                    {
                        ++combatcount;
                    }
                    else if (transform.GetChild(x).GetComponent<EmergencyCoverNode>() != null)
                    {
                        ++emergencycovercount;
                    }

                }

                combatcoverNodes = new Transform[combatcount];
                DebugTextsForCombatCover = new TextMeshProUGUI[combatcount];

                emergencycoverNodes = new Transform[emergencycovercount];
                DebugTextsForEmergencyCover = new TextMeshProUGUI[emergencycovercount];

                for (int x = 0; x < transform.childCount; x++)
                {
                    if (transform.GetChild(x).GetComponent<CoverNode>() != null)
                    {
                        SpawnTextForCombatCover(assignIndexforcombatcover, transform.GetChild(x).transform);
                        transform.GetChild(x).GetComponent<CoverNode>().spawnedText = DebugTextsForCombatCover[assignIndexforcombatcover];

                        combatcoverNodes[assignIndexforcombatcover] = transform.GetChild(x).transform;
                        ++assignIndexforcombatcover;
                    }
                    else if (transform.GetChild(x).GetComponent<EmergencyCoverNode>() != null)
                    {
                        SpawnTextForEmergencyCover(assignIndexforemergencycover, transform.GetChild(x).transform);
                        transform.GetChild(x).GetComponent<EmergencyCoverNode>().spawnedText = DebugTextsForEmergencyCover[assignIndexforemergencycover];

                        emergencycoverNodes[assignIndexforemergencycover] = transform.GetChild(x).transform;
                        ++assignIndexforemergencycover;
                    }
                }
            }
#endif
        }
        private void SpawnTextForCombatCover(int Count, Transform Pos)
        {
#if UNITY_EDITOR
            if (EnableDebugging == true)
            {
                DebugTextsForCombatCover[Count] = Instantiate(DebugInfoTextUI, Pos.position + DebugInfoTextUIOffset, Quaternion.identity).GetComponent<TextMeshProUGUI>();
                DebugTextsForCombatCover[Count].transform.SetParent(canvasfound.transform, false);
                //Rotate the spawned text 90 degrees up on the X-axis
                DebugTextsForCombatCover[Count].transform.rotation = Quaternion.Euler(-90f, 90f, 0f);
                DebugTextsForCombatCover[Count].gameObject.SetActive(false);
            }
#endif

        }
        private void SpawnTextForEmergencyCover(int Count, Transform Pos)
        {
#if UNITY_EDITOR
            if (EnableDebugging == true)
            {
                DebugTextsForEmergencyCover[Count] = Instantiate(DebugInfoTextUI, Pos.position + DebugInfoTextUIOffset, Quaternion.identity).GetComponent<TextMeshProUGUI>();
                DebugTextsForEmergencyCover[Count].transform.SetParent(canvasfound.transform, false);
                //Rotate the spawned text 90 degrees up on the X-axis
                DebugTextsForEmergencyCover[Count].transform.rotation = Quaternion.Euler(-90f, 90f, 0f);
                DebugTextsForEmergencyCover[Count].gameObject.SetActive(false);
            }
#endif
        }
    }
}
