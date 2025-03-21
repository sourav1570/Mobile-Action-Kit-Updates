using System.Collections;
using System.Collections.Generic;
using MobileActionKit;
using UnityEngine.UI;
using UnityEngine;


namespace MobileActionKit
{
    public class CrossHairColorChanger : MonoBehaviour
    {
        [TextArea]
        public string ScriptInfo = "This script sets default crosshair color and  changes the color of the crosshair to the specified colors for the cases when crosshair is on friendly or  enemy Ai agents.";

        [Space(10)]

      
        [Tooltip("Drag and drop Player root game object with FriendlyFire script attached to it into this field.")]
        public FriendlyFire FriendlyFireScript;

        [Tooltip("Drag and drop crosshair image Ui  elements into this list.")]
        public Image[] CrossHair;

        [Tooltip("Drag and dropPlayer Manager from player`s hierarchy into this field.")]
        public PlayerManager PlayerManagerComponent;

        [Tooltip("Choose a default color to use when not aiming at friendlies or enemies, such as when looking at the environment or props.")]
        public Color DefaultCrossHairColor = Color.white;

        [Tooltip("Choose a color for crosshair to change when aiming at an enemy.")]
        public Color OnEnemyCrosshairColor = Color.red;

        [Tooltip("Choose a color for crosshair to change when aiming at an friendly.")]
        public Color OnFriendlyCrossHairColor = Color.green;

        int CurrentIndex = 0;

        public void EnableCrossHair()
        {
            for (int x = 0; x < PlayerManagerComponent.PlayerWeaponScripts.Count; x++)
            {
                if (PlayerManagerComponent.PlayerWeaponScripts[x].gameObject.activeInHierarchy == true)
                {
                    CurrentIndex = x;
                }
            }
            for (int x = 0; x < CrossHair.Length; x++)
            {
                CrossHair[x].gameObject.SetActive(true);
                CrossHair[x].color = DefaultCrossHairColor;
            }
        }
        private void Update()
        {
            if (PlayerManagerComponent.PlayerWeaponScripts.Count >= 1)
            {
                if (PlayerManagerComponent.PlayerWeaponScripts[CurrentIndex] != null)
                {
                    if (PlayerManagerComponent.PlayerWeaponScripts[CurrentIndex].IsAimed == false)
                    {
                        SwitchCrossHairColor(PlayerManagerComponent.PlayerWeaponScripts[CurrentIndex].transform);
                    }

                }
                else
                {
                    EnableCrossHair();
                }
            }

        }
        public void SwitchCrossHairColor(Transform Direction)
        {
            // Get the position and direction of the shooting point
            Vector3 shootingPointPosition = Direction.transform.position;
            Vector3 shootingPointDirection = Direction.transform.forward;

            // Perform the raycast
            RaycastHit hit;
            if (Physics.Raycast(shootingPointPosition, shootingPointDirection, out hit, PlayerManagerComponent.PlayerWeaponScripts[CurrentIndex].ShootingFeatures.RaycastShootingRange))
            {
                if (hit.transform.root.GetComponent<Targets>() != null)
                {
                    if (hit.transform.root.GetComponent<HumanoidAiHealth>() != null)
                    {
                        if (hit.transform.root.GetComponent<HumanoidAiHealth>().IsDied == false)
                        {
                            if (hit.transform.root.GetComponent<Targets>().MyTeamID == FriendlyFireScript.TargetsScript.MyTeamID)
                            {
                                if (FriendlyFireScript.FriendlyFireOptions == FriendlyFire.FriendlyFireOptionsClass.DisableShootingOnFriendlies)
                                {
                                    PlayerManagerComponent.PlayerWeaponScripts[CurrentIndex].IsShootingNotAllowed = true;
                                }
                                for (int x = 0; x < CrossHair.Length; x++)
                                {
                                    CrossHair[x].gameObject.SetActive(true);
                                    CrossHair[x].color = OnFriendlyCrossHairColor;
                                }

                            }
                            else if (hit.transform.root.GetComponent<Targets>().MyTeamID != FriendlyFireScript.TargetsScript.MyTeamID)
                            {
                                if (FriendlyFireScript.FriendlyFireOptions == FriendlyFire.FriendlyFireOptionsClass.DisableShootingOnFriendlies)
                                {
                                    PlayerManagerComponent.PlayerWeaponScripts[CurrentIndex].IsShootingNotAllowed = false;
                                }
                                for (int x = 0; x < CrossHair.Length; x++)
                                {
                                    CrossHair[x].gameObject.SetActive(true);
                                    CrossHair[x].color = OnEnemyCrosshairColor;
                                }

                            }
                            else
                            {
                                if (FriendlyFireScript.FriendlyFireOptions == FriendlyFire.FriendlyFireOptionsClass.DisableShootingOnFriendlies)
                                {
                                    PlayerManagerComponent.PlayerWeaponScripts[CurrentIndex].IsShootingNotAllowed = false;
                                }
                                EnableCrossHair();
                            }
                        }
                        else
                        {
                            if (FriendlyFireScript.FriendlyFireOptions == FriendlyFire.FriendlyFireOptionsClass.DisableShootingOnFriendlies)
                            {
                                PlayerManagerComponent.PlayerWeaponScripts[CurrentIndex].IsShootingNotAllowed = false;
                            }
                            EnableCrossHair();

                        }
                    }
                }
                else
                {
                    if (FriendlyFireScript.FriendlyFireOptions == FriendlyFire.FriendlyFireOptionsClass.DisableShootingOnFriendlies)
                    {
                        PlayerManagerComponent.PlayerWeaponScripts[CurrentIndex].IsShootingNotAllowed = false;
                    }
                    EnableCrossHair();
                }
            }
            else
            {
                if (FriendlyFireScript.FriendlyFireOptions == FriendlyFire.FriendlyFireOptionsClass.DisableShootingOnFriendlies)
                {
                    PlayerManagerComponent.PlayerWeaponScripts[CurrentIndex].IsShootingNotAllowed = false;
                }
                EnableCrossHair();
            }
        }
    }
}