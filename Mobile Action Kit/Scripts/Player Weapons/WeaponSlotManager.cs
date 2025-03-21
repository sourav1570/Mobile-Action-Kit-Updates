using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MobileActionKit
{
    public class WeaponSlotManager : MonoBehaviour
    {
        [TextArea]
        public string ScriptInfo = "This script manages weapon slot restrictions, determining the default active weapons and enforcing a maximum weapon limit during gameplay." +
                           " Disable this script (along with its GameObject) when displaying the weapons available in the shop menu." +
                           " Keep it enabled if you do not want to link it to the shop system Or want to activate specific weapon in this Level.";

        [Tooltip("Reference to the PlayerWeaponsManager script that handles the player's weapons.")]
        public PlayerWeaponsManager PlayerWeaponsManagerScript;

        [Tooltip("Number of weapons that will be available to the player by default at the start of the game.")]
        public int WeaponsToActivateByDefault = 2;

        [Tooltip("Maximum number of weapons a player can hold at any time during gameplay.")]
        public int MaxWeaponsPlayerCarry = 3;

        private void Awake()
        {
            PlayerWeaponsManagerScript.WeaponSlotManagerScript = GetComponent<WeaponSlotManager>();
        }
    }
}