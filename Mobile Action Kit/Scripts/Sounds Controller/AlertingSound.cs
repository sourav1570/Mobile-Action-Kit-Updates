using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MobileActionKit
{
    public class AlertingSound : MonoBehaviour
    {
        [TextArea]
        public string ScriptInfo = "This component alerts humanoid Ai agents as well as other AI types (drones etc.) within its collider to react to it. This reaction can be performed in a few ways." +
            "And this script can alter some of the parameters of this reaction behaviour.";

        //[Tooltip("Decide whether to create coordinate on trigger or activation. In case 'CreateCoordinateOnActivation' has been chosen than the coordinates for other Ai agents will be created when this gameobject will" +
        //    " gets activated . for example : If the Player make a gunshot to distract enemies and sprint behind the buildings to hide. In this case 'CreateCoordinateOnActivation' will be most suitable as it will allow creating" +
        //    " the coordinate at the location where the player actually made the gunshot. Another example is if the Player is having an 'RPG gun' and shoots from very far away towards the enemies and miss the shot. you can" +
        //    " choose this option which will than" +
        //    " create the coordinate from the RPG Spawn point which will alert the other Ai agents to move towards these coordinate." +
        //    " The second option provided is called 'CreateCoordinateOnTrigger' in case if this option is chosen than the coordinates for the other" +
        //    " Ai agents will be created only when they enter the trigger area for example : If the player is sprinting than we only need to create coordinate when any other Ai agent gets within the trigger area to send them" +
        //    " the coordinates.")]
        //public CoordinateCreationType CoordinateCreation;

        [Tooltip("Choose one of the 2 behavioural responses of Ai agents affected by this sound alert. " +
            "'GoNearSoundCoordinates' to go towards the coordinate of this sound, or choose 'ForceEmergencyState' for Ai agent to go into emergency state.")]
        public ChooseHumanoidAiState AlertingSoundReactions;

        [Tooltip("Choose the team which will be affected by this sound.")]
        public ChooseHumanoidAiTeamSelection TeamAffectedBySound;

        [Tooltip("Type the name of the team to be affected by this sound.")]
        public string TeamName = "Team2";

        [Tooltip("This field represents the sound location mistake the Ai agent will make. " +
            "The greater the number, the further away the resulted investigation coordinate will get from the initial coordinate of the alerting sound .")]
        [Range(0.0f, 100.0f)]
        public float SoundInvestigationError;

        [Tooltip("Specify the time to deactivate this alerting sound game object.")]
        public float TimeToDeactivate = 2f;

        [Tooltip("If checked it will override approach style of sound investigating Ai agents." +
            "If only one of those options is checked then it will force itself on the way Ai agent will approach the sound and it will discard all the other approach styles." +
            "If two of those options are checked then they will discard unchecked approach style all together." +
            "Quick Note - If all of checkboxes are checked it will have the same effect as if none of them were checked i.e it will not take any effect over initial approach settings.")]
        public bool OverriteOtherAiApproach = false;

        [Tooltip("Allows only selected approach styles. If all or none of these options are selected then default approach values will not be overridden.")]
        public bool AllowSprinting = true;
        [Tooltip("Allows only selected approach styles. If all or none of these options are selected then default approach values will not be overridden.")]
        public bool AllowRunning = true;
        [Tooltip("Allows only selected approach styles. If all or none of these options are selected then default approach values will not be overridden.")]
        public bool AllowWalking = true;

        //public enum CoordinateCreationType
        //{
        //    CreateCoordinateOnActivation,
        //    CreateCoordinateOnTrigger
        //}

        [System.Serializable]
        public enum ChooseHumanoidAiState
        {
            GoNearSoundCoordinates,
            ForceEmergencyState,
        }

        [System.Serializable]
        public enum ChooseHumanoidAiTeamSelection
        {
            AllTeams,
            SelectedTeam
        }

        Vector3 SendCoordinates;

        [HideInInspector]
        public Transform SoundCreatorEntity;

        [HideInInspector]
        public Transform Enemy;

        [HideInInspector]
        public bool DoNotActivateEffect = false;

        private void Start()
        {
            gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
            if(OverriteOtherAiApproach == false)
            {
                AllowSprinting = true;
                AllowRunning = true;
                AllowWalking = true;
            }
            //if (CoordinateCreation == CoordinateCreationType.CreateCoordinateOnActivation)
            //{
            //    SendCoordinates = transform.position;
            //}

 
        }
        private void OnTriggerEnter(Collider other)
        {
            if(Enemy != null)
            {
                if (Enemy.GetComponent<CoreAiBehaviour>() != null)
                {
                    if(Enemy.GetComponent<CoreAiBehaviour>().HealthScript.IsDied == true)
                    {
                        DoNotActivateEffect = true;
                    }
                }
            }
            if(DoNotActivateEffect == false)
            {
                if (other.gameObject.transform.root.tag == "AI" && other.gameObject.layer != LayerMask.NameToLayer("Ignore Raycast") && TeamAffectedBySound == ChooseHumanoidAiTeamSelection.AllTeams
                    && other.gameObject.transform.root != SoundCreatorEntity
                || other.gameObject.transform.root.tag == "AI" && other.gameObject.layer != LayerMask.NameToLayer("Ignore Raycast") && TeamAffectedBySound == ChooseHumanoidAiTeamSelection.SelectedTeam
                && other.gameObject.transform.root.GetComponent<Targets>().MyTeamID == TeamName && other.gameObject.transform.root && other.gameObject.transform.root != SoundCreatorEntity)
                {
                    if (other.gameObject.transform.root.GetComponent<CoreAiBehaviour>() != null)
                    {
                        //Debug.Log(other.gameObject.transform + "StatusOfBot" + other.gameObject.transform.root.gameObject.GetComponent<CoreAiBehaviour>().CombatStarted
                        //    + "IsInInvestigation" + other.gameObject.transform.root.gameObject.GetComponent<CoreAiBehaviour>().IsInDefaultInvestigation +
                        //    "BotName" + other.gameObject.transform.root.gameObject.name); 
                    
                        if (other.gameObject.transform.root.gameObject.GetComponent<CoreAiBehaviour>().CombatStarted == false
                            && other.gameObject.transform.root.gameObject.GetComponent<CoreAiBehaviour>().IsInDefaultInvestigation == true && other.gameObject.transform.root.gameObject.GetComponent<CoreAiBehaviour>().WasInCombatLastTime == false
                                || other.gameObject.transform.root.gameObject.GetComponent<CoreAiBehaviour>().CombatStarted == false && other.gameObject.transform.root.gameObject.GetComponent<CoreAiBehaviour>().WasInCombatLastTime == false
                                 &&  other.gameObject.transform.root.gameObject.GetComponent<CoreAiBehaviour>().IsBodyguard == true ) // added another check so in case it is the follower than make sure to make him go near the
                                                                                                                                   // sound coordinate.
                        {
                          
                            //Debug.Log(other.gameObject.transform.root.name);
                            SendCoordinates = transform.position;
                            if (AlertingSoundReactions == ChooseHumanoidAiState.ForceEmergencyState && other.gameObject.transform.root.gameObject.GetComponent<CoreAiBehaviour>().AgentRole != CoreAiBehaviour.Role.Zombie )
                            {
                                if (other.gameObject.transform.root.gameObject.GetComponent<CoreAiBehaviour>().IsNearDeadBody == false
                                    || other.gameObject.transform.root.gameObject.GetComponent<CoreAiBehaviour>().IsInEmergencyState == false)
                                {
                                    //Debug.Break();
                                    // other.gameObject.transform.root.gameObject.GetComponent<CoreAiBehaviour>().ResetEmergencyBehaviourVariables();
                                    other.gameObject.transform.root.gameObject.GetComponent<CoreAiBehaviour>().InvestigationCoordinates = SendCoordinates;
//                                    Debug.Log("Alerting Sound Alert");
                                    other.gameObject.transform.root.gameObject.GetComponent<CoreAiBehaviour>().IsNearDeadBody = true;
                                    other.gameObject.transform.root.gameObject.GetComponent<CoreAiBehaviour>().IsEmergencyRun = true;
                                    other.gameObject.transform.root.gameObject.GetComponent<CoreAiBehaviour>().IsInEmergencyState = true;
                                }
                            }
                            else 
                            {
                                // Added SearchingForSound check when doing tutorial as it is important to make sure the AI agent do not receive any new coordinate when moving to the current sound coordinate for example: Grenade explosion while
                                // moving to friendly fire sound already. As this could reset its approach style again and again.
                                if (other.gameObject.transform.root.gameObject.GetComponent<CoreAiBehaviour>().SearchingForSound == false
                                    && AlertingSoundReactions != ChooseHumanoidAiState.ForceEmergencyState)
                                {
                                    other.gameObject.transform.root.gameObject.GetComponent<CoreAiBehaviour>().AiHearing.ErrorSoundPercentage = SoundInvestigationError;
                                    other.gameObject.transform.root.GetComponent<CoreAiBehaviour>().GetSoundCoordinate = SendCoordinates;
                                    other.gameObject.transform.root.GetComponent<CoreAiBehaviour>().InitialiseHearing();
                                    other.gameObject.transform.root.GetComponent<CoreAiBehaviour>().OverriteSprintingForSounds = AllowSprinting;
                                    other.gameObject.transform.root.GetComponent<CoreAiBehaviour>().OverriteRunningForSounds = AllowRunning;
                                    other.gameObject.transform.root.GetComponent<CoreAiBehaviour>().OverriteWalkingForSounds = AllowWalking;
                                }
                               
                            }
                        }
                    }
                }
            }           
        }
    }
}