using UnityEngine.UI;
using UnityEngine;
using System.Collections.Generic;
using TMPro;
using System.Collections;

// This Script Is Responsible for the Player Collecting Weapons From The Game 

namespace MobileActionKit
{
    public class WeaponPickupManager : MonoBehaviour
    {
        [TextArea]
        public string ScriptInfo = "Handles weapon collection, including adding weapons to inventory, replacing weapons, and collecting ammo.";

        [Tooltip("Singleton instance of CollectWeapons.")]
        public static WeaponPickupManager instance;

        [Tooltip("Reference to the PlayerWeaponsManager script.")]
        public PlayerWeaponsManager PlayerWeaponsManagerScript;

        [Tooltip("Position where the dropped weapon should appear.")]
        public Transform DroppedWeaponPosition;

        [Tooltip("UI panel displaying weapon information.")]
        public GameObject WeaponInfo;

        [Tooltip("UI image to display pickup weapon icon.")]
        public Image WeaponIcon;

        [Tooltip("Button for collecting a weapon.")]
        public Button CollectWeaponButton;

        [Tooltip("Button for collecting ammo.")]
        public Button CollectAmmoButton;

        [Tooltip("Text UI displaying available ammo.")]
        public TextMeshProUGUI AmmoText;

        [HideInInspector]
        [Tooltip("Reference to the PickupWeapon script.")]
        public PickupWeapon PickupWeaponScript;

        [HideInInspector]
        [Tooltip("Amount of ammo available for collection.")]
        public int Ammo;

        [HideInInspector]
        [Tooltip("Weapon icon sprite.")]
        public Sprite Icon;

        [HideInInspector]
        [Tooltip("Reference to the weapon object available for collection.")]
        public GameObject WeaponToCollect;
        
        GameObject PrevWeapon;
        AttachmentsActivator Attachments;

        AttachmentsActivator PreviousWeaponAttachments;

        string PrevWeapon_WeaponID_WeaponName;

        [HideInInspector]
        public string SlotName;
        bool DoNotContinueWithPickup = false;
        bool StopSwitchingWeapon = false;

        bool WeaponPickedSuccess = false;
        bool StopDroppingWeapons = false;

        [HideInInspector]
        public string NameOfTheWeaponToPick;



        private void Awake()
        {
            instance = this;
        }
        private void Start()
        {
            CollectWeaponButton.onClick.AddListener(CollectWeapon);
            CollectAmmoButton.onClick.AddListener(CollectAmmo);
        }
        public void UpdateWeapons(GameObject WeaponToAdd, GameObject WeaponToRemove)
        {
            PlayerWeaponsManagerScript.AddWeaponInListWithoutSaving(WeaponToAdd, WeaponToRemove);
        }
        public void ActivateWeaponInfo()
        {
            if (PlayerManager.instance != null)
            {
                PrevWeapon = PlayerManager.instance.CurrentHoldingPlayerWeapon.RequiredComponents.WeaponAnimatorComponent.gameObject;
            }

           
            WeaponInfo.SetActive(true);
            AmmoText.text = Ammo.ToString();
            WeaponIcon.sprite = Icon;


            CollectAmmoButton.interactable = false;

            for (int activeweapon = 0; activeweapon < PlayerWeaponsManagerScript.ActiveWeaponsList.Count; activeweapon++)
            {
                if (PlayerWeaponsManagerScript.ActiveWeaponsList.Contains(WeaponToCollect))
                {
                    CollectAmmoButton.interactable = true;
                }
            }
        }
        public void PcControls_WeaponPickup()
        {
            if(NameOfTheWeaponToPick != "")
            {
                if (CollectWeaponButton != null)
                {
                    if (CollectWeaponButton.interactable == true)
                    {
                         CollectWeaponButton.onClick.Invoke();
                         NameOfTheWeaponToPick = "";
                    }
                }

            }
           
        }
        public void PcControls_AmmoPickup()
        {
            if (CollectAmmoButton != null)
            {
                if (CollectAmmoButton.interactable == true)
                {
                    CollectAmmoButton.onClick.Invoke();
                }
            }
        }
        public void DeactivateWeaponInfo()
        {
            WeaponInfo.SetActive(false);
        }
        public void CollectWeapon()
        {
            StartCoroutine(WeaponActivator(WeaponToCollect));
        }
        public void CollectAmmo()
        {
            bool isAmmoFound = false;

            for (int d = 0; d < PlayerWeaponsManagerScript.WeaponSlots.Count; d++)
            {
                for (int p = 0; p < PlayerWeaponsManagerScript.WeaponSlots.Count; p++)
                {
                    if (PlayerWeaponsManagerScript.WeaponSlots[d].Weapons[p].Weapon == WeaponToCollect)
                    {
                        for (int pl = 0; pl < PlayerManager.instance.PlayerWeaponScripts.Count; pl++)
                        {
                            if (PlayerManager.instance.PlayerWeaponScripts[d].RequiredComponents.WeaponIdScript != null)
                            {
                                if (PlayerManager.instance.PlayerWeaponScripts[pl].RequiredComponents.WeaponIdScript == PlayerWeaponsManagerScript.WeaponSlots[d].Weapons[p].WeaponIdComponent)
                                {
                                    PlayerManager.instance.PlayerWeaponScripts[pl].Reload.TotalAmmo = PlayerManager.instance.PlayerWeaponScripts[pl].Reload.TotalAmmo + Ammo;
                                    isAmmoFound = true;
                                    break; // Break the innermost loop (pl loop)
                                }
                            }
                        }
                    }
                    if (isAmmoFound)
                    {
                        break; // Break the middle loop (p loop)
                    }
                }
                if (isAmmoFound)
                {
                    break; // Break the outermost loop (d loop)
                }
            }

            PickupWeaponScript.InitialAmmo = 0;
            Ammo = 0;
            AmmoText.text = Ammo.ToString();
            WeaponInfo.SetActive(false);
           // PickupWeaponScript.gameObject.SetActive(false);
            PickupWeaponScript = null;
        }
        public IEnumerator WeaponActivator(GameObject Weapon)
        {
            DoNotContinueWithPickup = false;
            //for (int x = 0; x < ButtonsToDisableOnSwitch.Length; x++)
            //{
            //    ButtonsToDisableOnSwitch[x].interactable = false;
            //}
            //for (int x = 0; x < RaycastImages.Length; x++)
            //{
            //    RaycastImages[x].raycastTarget = false;
            //}
            WeaponInfo.SetActive(false);

            //if (PlayerWeaponsManagerScript.LimitedWeaponSlotsScript == null)
            //{
            if (PickupWeaponScript.ReplaceWeaponBySlotIfExist == false)
            {
                if (PlayerManager.instance != null)
                {
                    PrevWeapon = PlayerManager.instance.CurrentHoldingPlayerWeapon.RequiredComponents.WeaponAnimatorComponent.gameObject;
                    PrevWeapon_WeaponID_WeaponName = PlayerManager.instance.CurrentHoldingPlayerWeapon.RequiredComponents.WeaponIdScript.WeaponName;
                    PreviousWeaponAttachments = PlayerManager.instance.CurrentHoldingPlayerWeapon.RequiredComponents.AttachmentsActivatorComponent;

                    for (int pl = 0; pl < PlayerWeaponsManagerScript.WeaponSlots.Count; pl++)
                    {
                        for (int ws = 0; ws < PlayerWeaponsManagerScript.WeaponSlots[pl].Weapons.Count; ws++)
                        {
                            if (PlayerWeaponsManagerScript.WeaponSlots[pl].Weapons[ws].Weapon == Weapon)
                            {
                                Attachments = PlayerWeaponsManagerScript.WeaponSlots[pl].Weapons[ws].WeaponAttachments;
                            }

                        }
                    }

                    // Replaced the below code by above code from line 125 till line 136
                    //Attachments = PlayerManager.instance.ob.RequiredComponents.AttachmentsActivatorComponent;
                }
            }
            else
            {
                bool IsPrevWeaponSelected = false;

                for (int x = 0; x < PlayerWeaponsManagerScript.WeaponSlots.Count; x++)
                {
                    if (PlayerWeaponsManagerScript.WeaponSlots[x].SlotName == SlotName)
                    {
                        for (int i = 0; i < PlayerWeaponsManagerScript.WeaponSlots[x].Weapons.Count; i++)
                        {
                            for (int z = 0; z < PlayerWeaponsManagerScript.ActiveWeaponsList.Count; z++)
                            {
                                if (PlayerWeaponsManagerScript.ActiveWeaponsList[z] == PlayerWeaponsManagerScript.WeaponSlots[x].Weapons[i].Weapon)
                                {
                                    if (IsPrevWeaponSelected == false)
                                    {
                                        IsPrevWeaponSelected = true;
                                        PrevWeapon = PlayerWeaponsManagerScript.WeaponSlots[x].Weapons[i].Weapon;
                                        PrevWeapon_WeaponID_WeaponName = PlayerWeaponsManagerScript.WeaponSlots[x].Weapons[i].WeaponIdComponent.WeaponName;
                                        PreviousWeaponAttachments = PlayerWeaponsManagerScript.WeaponSlots[x].Weapons[i].WeaponAttachments;

                                        for (int pl = 0; pl < PlayerWeaponsManagerScript.WeaponSlots.Count; pl++)
                                        {
                                            for (int ws = 0; ws < PlayerWeaponsManagerScript.WeaponSlots[pl].Weapons.Count; ws++)
                                            {
                                                if (PlayerWeaponsManagerScript.WeaponSlots[pl].Weapons[ws].Weapon == Weapon)
                                                {
                                                    Attachments = PlayerWeaponsManagerScript.WeaponSlots[pl].Weapons[ws].WeaponAttachments;

                                                }

                                            }
                                        }
                                    }


                                    // Replaced the below code by above code from line 159 till line 170
                                    //Attachments = PlayerWeaponsManagerScript.WeaponSlots[x].Weapons[i].WeaponAttachments;
                                }
                            }
                        }
                    }
                }

                if (IsPrevWeaponSelected == false)
                {
                    if (PlayerManager.instance != null)
                    {
                        PrevWeapon = PlayerManager.instance.CurrentHoldingPlayerWeapon.RequiredComponents.WeaponAnimatorComponent.gameObject;
                        PrevWeapon_WeaponID_WeaponName = PlayerManager.instance.CurrentHoldingPlayerWeapon.RequiredComponents.WeaponIdScript.WeaponName;
                       
                        PreviousWeaponAttachments = PlayerManager.instance.CurrentHoldingPlayerWeapon.RequiredComponents.AttachmentsActivatorComponent;

                        for (int pl = 0; pl < PlayerWeaponsManagerScript.WeaponSlots.Count; pl++)
                        {
                            for (int ws = 0; ws < PlayerWeaponsManagerScript.WeaponSlots[pl].Weapons.Count; ws++)
                            {
                                if (PlayerWeaponsManagerScript.WeaponSlots[pl].Weapons[ws].Weapon == Weapon)
                                {
                                    Attachments = PlayerWeaponsManagerScript.WeaponSlots[pl].Weapons[ws].WeaponAttachments;
                                }

                            }
                        }
                    }
                }
            }
            //}
            //else
            //{
            //    if (PickupWeaponScript.ReplaceThisWeaponByCategory == false)
            //    {
            //        if (PlayerManager.instance != null)
            //        {
            //            PrevWeapon = PlayerManager.instance.ob.RequiredComponents.WeaponAnimatorComponent.gameObject;
            //            PrevWeapon_WeaponID_WeaponName = PlayerManager.instance.ob.RequiredComponents.WeaponIdScript.WeaponName;
            //            Attachments = PlayerManager.instance.ob.RequiredComponents.AttachmentsActivatorComponent;
            //        }
            //    }

            //}


            if (PlayerManager.instance != null)
            {
                PlayerManager.instance.CurrentHoldingPlayerWeapon.RequiredComponents.WeaponAnimatorComponent.Play(PlayerManager.instance.CurrentHoldingPlayerWeapon.RemoveAnimationName, -1, 0f);
            }

            yield return new WaitForSeconds(PlayerManager.instance.CurrentHoldingPlayerWeapon.RemoveLength);

            if (PlayerManager.instance != null)
            {
                if (PlayerManager.instance.CurrentHoldingPlayerWeapon.IsAimed == true)
                {
                    PlayerManager.instance.Aiming();
                    PlayerManager.instance.CurrentHoldingPlayerWeapon.IsAimed = false;
                    PlayerManager.instance.CurrentHoldingPlayerWeapon.IsHipFire = true;
                    //WasPreviousWeaponAimed = true;
                }
            }
            UpdateWeaponActivation(Weapon);

            if (PlayerManager.instance != null)
            {
                PlayerManager.instance.FindRequiredObjects();
                if (PlayerManager.instance.CurrentHoldingPlayerWeapon.IsAimed == true)
                {
                    PlayerManager.instance.Aiming();
                    PlayerManager.instance.CurrentHoldingPlayerWeapon.IsAimed = false;
                    PlayerManager.instance.CurrentHoldingPlayerWeapon.IsHipFire = true;
                }
            }

            if (PlayerManager.instance != null)
            {
                if (PlayerManager.instance.CurrentHoldingPlayerWeapon != null)
                {
                    PlayerManager.instance.CurrentHoldingPlayerWeapon.Reload.TotalAmmo = PickupWeaponScript.InitialAmmo;
                    PlayerManager.instance.CurrentHoldingPlayerWeapon.DoNotResetValues = false;
                }
            }

            PickupWeaponScript.gameObject.SetActive(false);
            PickupWeaponScript = null;

            yield return new WaitForSeconds(PlayerManager.instance.CurrentHoldingPlayerWeapon.WieldTime);
            ActivateButtons();

            // important so if for example you use Svk with sniper scope now the aimed position and aimed rotation values are already overrides now in this case
            // if you pick up the svk without sniper scope you need to make sure to put back the default aimed rotation values and aimed position values back to svk
            if (PlayerManager.instance != null)
            {
                if (PlayerManager.instance.CurrentHoldingPlayerWeapon != null)
                {
                    PlayerManager.instance.CurrentHoldingPlayerWeapon.ResetAimedWeaponPositionsAndRotations();
                }
            }

        }
        public void ActivateButtons()
        {
            //for (int x = 0; x < ButtonsToDisableOnSwitch.Length; x++)
            //{
            //    ButtonsToDisableOnSwitch[x].interactable = true;
            //}
            //for (int x = 0; x < RaycastImages.Length; x++)
            //{
            //    RaycastImages[x].raycastTarget = true;
            //}
        }
        private void UpdateWeaponActivation(GameObject WeaponToActivate)
        {
            StopSwitchingWeapon = false;
            WeaponPickedSuccess = false;
            StopDroppingWeapons = false;

            if (PlayerManager.instance != null)
            {
                if (PlayerManager.instance.CurrentHoldingPlayerWeapon != null)
                {
                    if (PlayerManager.instance.CurrentHoldingPlayerWeapon.gameObject.activeInHierarchy == true)
                    {
                        PlayerManager.instance.CurrentHoldingPlayerWeapon.RequiredComponents.WeaponAnimatorComponent.gameObject.SetActive(false);
                    }
                }
            }

            if (PrevWeapon != null)
            {
                if (PrevWeapon.activeInHierarchy == false)
                {
                    for (int x = 0; x < SwitchingPlayerWeapons.ins.PlayerWeaponsManagerScript.ActiveWeaponsList.Count; x++)
                    {
                        if (SwitchingPlayerWeapons.ins.PlayerWeaponsManagerScript.ActiveWeaponsList[x] != null)
                        {
                            if (SwitchingPlayerWeapons.ins.PlayerWeaponsManagerScript.ActiveWeaponsList[x].gameObject == PrevWeapon)
                            {
                                SwitchingPlayerWeapons.ins.SameWeaponIndex = x;
                                SwitchingPlayerWeapons.ins.ActivateSameWeapon = true;
                            }
                        }

                    }

                    // SwitchingPlayerWeapons.ins.ActivateNextWeapon(); // Important so that weapon switching can work properly ( commented and placed below in this function )
                }
                PrevWeapon.SetActive(false);

            }

            // Previously from 218 to 251 was in last lines after 297 basically
            if (PrevWeapon != null)
            {
                for (int w = 0; w < PlayerWeaponsManagerScript.ActiveWeaponsList.Count; w++)
                {
                    if (PlayerWeaponsManagerScript.ActiveWeaponsList[w] == null && WeaponToActivate != PrevWeapon)
                    {
                        StopDroppingWeapons = true;
                    }
                }

                if (StopDroppingWeapons == false) // was here before this checkbox --- code --- CanDropWeapons == true && 
                {
                    for (int x = 0; x < PlayerWeaponsManagerScript.AvailableWeaponPickups.Count; x++)
                    {
                        if (PlayerWeaponsManagerScript.AvailableWeaponPickups[x].WeaponName == PrevWeapon_WeaponID_WeaponName)
                        {
                            GameObject DroppedWeapon = Instantiate(PlayerWeaponsManagerScript.AvailableWeaponPickups[x].WeaponPickupPrefab, DroppedWeaponPosition.position, PlayerWeaponsManagerScript.AvailableWeaponPickups[x].WeaponPickupPrefab.transform.rotation);
                            DroppedWeapon.GetComponent<PickupWeapon>().ActivateRandomAttachment = false;



                            if (PlayerManager.instance != null)
                            {
                                for (int d = 0; d < PlayerManager.instance.PlayerWeaponScripts.Count; d++)
                                {
                                    if (PlayerManager.instance.PlayerWeaponScripts[d] != null)
                                    {
                                        if(PlayerManager.instance.PlayerWeaponScripts[d].RequiredComponents.WeaponIdScript != null)
                                        {
                                            if (PlayerManager.instance.PlayerWeaponScripts[d].RequiredComponents.WeaponIdScript.WeaponName == PrevWeapon_WeaponID_WeaponName)
                                            {
                                                DroppedWeapon.GetComponent<PickupWeapon>().InitialAmmo = PlayerManager.instance.PlayerWeaponScripts[d].Reload.TotalAmmo;
                                            }
                                        }
                                       
                                    }
                                }
                            }

                           

                            for (int a = 0; a < DroppedWeapon.GetComponent<PickupWeapon>().AttachmentsToActivate.Length; a++)
                            {
                                DroppedWeapon.GetComponent<PickupWeapon>().AttachmentsToActivate[a].gameObject.SetActive(false);
                            }

                            for (int i = 0; i < PreviousWeaponAttachments.inventoryItems.Count; i++) // Newly Added so If holding Ak with scope by default can be dropped correctly with Ak scope
                            {
                                if (PreviousWeaponAttachments.inventoryItems[i].IsThisItemActivatedCurrently == true)
                                {
                                    for (int g = 0; g < DroppedWeapon.GetComponent<PickupWeapon>().AttachmentsToActivate.Length; g++)
                                    {
                                        if (PreviousWeaponAttachments.inventoryItems[i].keyName == DroppedWeapon.GetComponent<PickupWeapon>().AttachmentsToActivate[g].KeyName)
                                        {
                                            DroppedWeapon.GetComponent<PickupWeapon>().AttachmentsToActivate[g].gameObject.SetActive(true);
                                        }
                                    }
                                }
                            }

                            //for (int i = 0; i < Attachments.inventoryItems.Count; i++)
                            //{
                            //    if (Attachments.inventoryItems[i].IsThisItemActivatedCurrently == true)
                            //    {
                            //        for (int g = 0; g < DroppedWeapon.GetComponent<PickupWeapon>().AttachmentsToActivate.Length; g++)
                            //        {
                            //            if (Attachments.inventoryItems[i].keyName == DroppedWeapon.GetComponent<PickupWeapon>().AttachmentsToActivate[g].KeyName)
                            //            {
                            //                DroppedWeapon.GetComponent<PickupWeapon>().AttachmentsToActivate[g].gameObject.SetActive(true);
                            //            }
                            //        }
                            //    }
                            //}


                        }
                    }
                }
            }


            // New code to make before picking we deactivate current attachments of the same weapon or previous weapon like AK before picking new AK with scope.
            for (int i = 0; i < Attachments.inventoryItems.Count; i++)
            {
                for (int g = 0; g < Attachments.inventoryItems[i].ObjectsToActivate.Length; g++)
                {
                    Attachments.inventoryItems[i].IsThisItemActivatedCurrently = false;
                    Attachments.inventoryItems[i].ObjectsToActivate[g].SetActive(false);
                }
            }


            for (int x = 0; x < PlayerWeaponsManagerScript.WeaponSlots.Count; x++)
            {
                for (int i = 0; i < PlayerWeaponsManagerScript.WeaponSlots[x].Weapons.Count; i++)
                {
                    if (PlayerWeaponsManagerScript.WeaponSlots[x].Weapons[i].Weapon == WeaponToActivate)
                    {
                        for (int z = 0; z < PickupWeaponScript.AttachmentsToActivate.Length; z++)
                        {
                            if(PickupWeaponScript.AttachmentsToActivate[z] != null)
                            {
                                if (PickupWeaponScript.AttachmentsToActivate[z].gameObject.activeInHierarchy == true)
                                {
                                    PlayerWeaponsManagerScript.WeaponSlots[x].Weapons[i].WeaponAttachments.ActivateItemsWithoutSaving(PickupWeaponScript.AttachmentsToActivate[z].KeyName);
                                }
                            }
                           
                        }
                    }

                }
            }

            if (PrevWeapon != WeaponToActivate) // if same weapon activated no need to re-activate it
            {
                WeaponToActivate.SetActive(true);
            }

            if (PlayerWeaponsManagerScript.ShowAllAvailableWeaponSlots == false)
            {
                UpdateWeapons(WeaponToActivate, PrevWeapon);
            }
            else
            {
                for (int x = 0; x < PlayerWeaponsManagerScript.WeaponSlots.Count; x++)
                {
                    for (int i = 0; i < PlayerWeaponsManagerScript.WeaponSlots[x].Weapons.Count; i++)
                    {
                        //if (PlayerWeaponsManagerScript.LimitedWeaponSlotsScript == null)
                        //{
                        //    if (PlayerWeaponsManagerScript.WeaponSlots[x].Weapons[i].Weapon == WeaponToActivate && SlotName == PlayerWeaponsManagerScript.WeaponSlots[x].SlotName && PlayerWeaponsManagerScript.ActiveWeaponsList[x] == null)
                        //    {
                        //        if (WeaponPickedSuccess == false)
                        //        {
                        //            PlayerWeaponsManagerScript.ActiveWeaponsList[x] = PlayerWeaponsManagerScript.WeaponSlots[x].Weapons[i].Weapon;
                        //            ++SwitchingPlayerWeapons.ins.WeaponActivated;
                        //            SwitchingPlayerWeapons.ins.UpdatePreviuosWeapon();

                        //            WeaponPickedSuccess = true;
                        //        }

                        //    }
                        //    else if (PlayerWeaponsManagerScript.WeaponSlots[x].Weapons[i].Weapon == WeaponToActivate && SlotName == PlayerWeaponsManagerScript.WeaponSlots[x].SlotName && PlayerWeaponsManagerScript.ActiveWeaponsList[x] != null)
                        //    {
                        //        if (WeaponPickedSuccess == false)
                        //        {
                        //            UpdateWeapons(WeaponToActivate, PrevWeapon);
                        //            WeaponPickedSuccess = true;
                        //        }

                        //    }
                        //}
                        //else
                        //{
                        for (int w = 0; w < PlayerWeaponsManagerScript.ActiveWeaponsList.Count; w++)
                        {
                            if (PickupWeaponScript.ReplaceWeaponBySlotIfExist == true)
                            {
                                if (PlayerWeaponsManagerScript.WeaponSlots[x].Weapons[i].Weapon == WeaponToActivate && SlotName == PlayerWeaponsManagerScript.WeaponSlots[x].SlotName && PlayerWeaponsManagerScript.ActiveWeaponsList[w] == null && WeaponToActivate != PrevWeapon)
                                {
                                    if (WeaponPickedSuccess == false)
                                    {
                                        DoNotContinueWithPickup = true;
                                        PlayerWeaponsManagerScript.ActiveWeaponsList[w] = PlayerWeaponsManagerScript.WeaponSlots[x].Weapons[i].Weapon;
                                        ++SwitchingPlayerWeapons.ins.WeaponActivated;
                                        SwitchingPlayerWeapons.ins.UpdatePreviuosWeapon();
                                        StopSwitchingWeapon = true;
                                        WeaponPickedSuccess = true;

                                        SwitchingPlayerWeapons.ins.currentIndex = w; // Why w because when we pickup and add a weapon to player inventory without dropping and click prev (left button) to switch to prev weapon
                                                                                     // we need to make sure to get the previous weapon as previous weapon will be like w - 1 which in result if G17 is activated in element 2
                                                                                     // than it will activate SVK if exist in element 1 as the previous weapon.
                                        SwitchingPlayerWeapons.ins.SameWeaponIndex = 0; // Why 0 because when we pickup and add a weapon to player inventory without dropping and click next (right button) to switch to next weapon
                                                                                        // we basically want to activate the weapon in element 0 so restarting..
                                        SwitchingPlayerWeapons.ins.ActivateSameWeapon = true; // important to make sure when we click the next weapon (right button) we make sure to ovverrid these calculation to the logic )
                                    }


                                }
                            }
                            else
                            {
                                if (PlayerWeaponsManagerScript.WeaponSlots[x].Weapons[i].Weapon == WeaponToActivate && PlayerWeaponsManagerScript.ActiveWeaponsList[w] == null && WeaponToActivate != PrevWeapon)
                                {
                                    if (WeaponPickedSuccess == false)
                                    {
                                        DoNotContinueWithPickup = true;
                                        PlayerWeaponsManagerScript.ActiveWeaponsList[w] = PlayerWeaponsManagerScript.WeaponSlots[x].Weapons[i].Weapon;
                                        ++SwitchingPlayerWeapons.ins.WeaponActivated;
                                        SwitchingPlayerWeapons.ins.UpdatePreviuosWeapon();
                                        StopSwitchingWeapon = true;
                                        WeaponPickedSuccess = true;

                                        SwitchingPlayerWeapons.ins.currentIndex = w; // Why w because when we pickup and add a weapon to player inventory without dropping and click prev (left button) to switch to prev weapon
                                                                                     // we need to make sure to get the previous weapon as previous weapon will be like w - 1 which in result if G17 is activated in element 2
                                                                                     // than it will activate SVK if exist in element 1 as the previous weapon.
                                        SwitchingPlayerWeapons.ins.SameWeaponIndex = 0; // Why 0 because when we pickup and add a weapon to player inventory without dropping and click next (right button) to switch to next weapon
                                                                                        // we basically want to activate the weapon in element 0 so restarting..
                                        SwitchingPlayerWeapons.ins.ActivateSameWeapon = true; // important to make sure when we click the next weapon (right button) we make sure to ovverrid these calculation to the logic )
                                    }

                                }
                            }


                        }

                        if (DoNotContinueWithPickup == false)
                        {
                            if (PickupWeaponScript.ReplaceWeaponBySlotIfExist == true)
                            {
                                if (PlayerWeaponsManagerScript.WeaponSlots[x].Weapons[i].Weapon == WeaponToActivate && SlotName == PlayerWeaponsManagerScript.WeaponSlots[x].SlotName && PlayerWeaponsManagerScript.ActiveWeaponsList[x] == null && WeaponToActivate != PrevWeapon)
                                {
                                    if (WeaponPickedSuccess == false)
                                    {
                                        PlayerWeaponsManagerScript.ActiveWeaponsList[x] = PlayerWeaponsManagerScript.WeaponSlots[x].Weapons[i].Weapon;
                                        ++SwitchingPlayerWeapons.ins.WeaponActivated;
                                        SwitchingPlayerWeapons.ins.UpdatePreviuosWeapon();
                                        WeaponPickedSuccess = true;
                                    }

                                }
                                else if (PlayerWeaponsManagerScript.WeaponSlots[x].Weapons[i].Weapon == WeaponToActivate && SlotName == PlayerWeaponsManagerScript.WeaponSlots[x].SlotName && PlayerWeaponsManagerScript.ActiveWeaponsList[x] != null)
                                {
                                    if (WeaponPickedSuccess == false)
                                    {
                                        UpdateWeapons(WeaponToActivate, PrevWeapon);
                                        WeaponPickedSuccess = true;
                                    }
                                }
                            }
                            else
                            {
                                if (PlayerWeaponsManagerScript.WeaponSlots[x].Weapons[i].Weapon == WeaponToActivate && PlayerWeaponsManagerScript.ActiveWeaponsList[x] == null && WeaponToActivate != PrevWeapon)
                                {
                                    if (WeaponPickedSuccess == false)
                                    {
                                        PlayerWeaponsManagerScript.ActiveWeaponsList[x] = PlayerWeaponsManagerScript.WeaponSlots[x].Weapons[i].Weapon;
                                        ++SwitchingPlayerWeapons.ins.WeaponActivated;
                                        SwitchingPlayerWeapons.ins.UpdatePreviuosWeapon();
                                        WeaponPickedSuccess = true;
                                    }

                                }
                                else if (PlayerWeaponsManagerScript.WeaponSlots[x].Weapons[i].Weapon == WeaponToActivate && PlayerWeaponsManagerScript.ActiveWeaponsList[x] != null)
                                {
                                    if (WeaponPickedSuccess == false)
                                    {
                                        UpdateWeapons(WeaponToActivate, PrevWeapon);
                                        WeaponPickedSuccess = true;
                                    }

                                }
                            }

                        }

                        //}
                    }
                }
            }

            if (PrevWeapon != null)
            {
                if (PrevWeapon.activeInHierarchy == false && StopSwitchingWeapon == false)
                {
                    SwitchingPlayerWeapons.ins.ActivateNextWeapon(); // Important so that weapon switching can work properly
                }
            }

        }
    }
}