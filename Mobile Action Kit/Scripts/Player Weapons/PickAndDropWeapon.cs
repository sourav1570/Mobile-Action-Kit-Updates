using System.Collections.Generic;
using UnityEngine;

// This Script is Responsible For Dropping The Current Weapon in the Scene

namespace MobileActionKit
{
    public class PickAndDropWeapon : MonoBehaviour
    {
        [TextArea]
        [ContextMenuItem("Reset Description", "ResettingDescription")]
        public string ScriptInfo = "This Script controls the weapon picking and dropping behaviour";
        [Space(10)]

        [HideInInspector]
        public string WeaponName;

        [Tooltip("Prefab to instantiate When Dropping the weapon")]
        public GameObject DroppedWeaponPrefab;
        [Tooltip("Shooting Script attached With this weapon")]
        public PlayerWeapon ShootingScript;
        int WeaponId;
        [HideInInspector]
        public GameObject Weapon;
        [Tooltip("Add the same dropped Weapon already exists in the scene")]
        public List<GameObject> WeaponsToPick = new List<GameObject>();
        public void ResettingDescription()
        {
            ScriptInfo = "This Script controls the weapon picking and dropping behaviour";
        }
        private void Start()
        {
            WeaponName = GetComponent<WeaponId>().WeaponName;
        }
        public void SpawnWeapon(Transform a, Transform b) // Spawn Dropped Weapon in Scene
        {
            Weapon = Instantiate(DroppedWeaponPrefab, a.position, b.rotation);
            WeaponsToPick.Add(Weapon);
        }
        public void ChangeWeaponId() // Change Current Weapon Id
        {
           // WeaponId = GetComponent<WeaponId>().Weaponid;

            //for (int i = 0; i < CollectWeapons.instance.AllWeaponsParent.Length; i++)
            //{
            //    for (int x = 0; x < CollectWeapons.instance.AllWeaponsParent[i].GetComponent<PickAndDropWeapon>().WeaponsToPick.Count; x++)
            //    {
            //        if (CollectWeapons.instance.AllWeaponsParent[i].GetComponent<PickAndDropWeapon>().WeaponsToPick[x].activeInHierarchy == true)
            //        {
            //            Debug.Log("CELL");
            //          //  CollectWeapons.instance.AllWeaponsParent[i].GetComponent<PickAndDropWeapon>().WeaponsToPick[x].gameObject.GetComponent<PickupWeapon>().WeaponId = WeaponId;
            //        }
            //    }
            //}

        }
    }
}