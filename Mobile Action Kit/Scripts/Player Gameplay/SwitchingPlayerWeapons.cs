using System.Collections;
using System.Collections.Generic;
using MobileActionKit;
using UnityEngine;
using UnityEngine.UI;


namespace MobileActionKit
{
    public class SwitchingPlayerWeapons : MonoBehaviour
    {
        [TextArea]
        public string ScriptInfo = "This script handles player HUD UI swiping and arrow buttons for switching weapons.";

        public static SwitchingPlayerWeapons ins;

        [System.Serializable]
        public enum WeaponActivationType
        {
            ShopDependent,
            LevelDependent,
        }

        [Tooltip("Defines how weapons are unlocked and activated: 'ShopDependent' (based on purchased weapons) or 'LevelDependent' (based on level).")]
        public WeaponActivationType AvailableWeapons;

        [Tooltip("Automatically aligns the aim when switching from a weapon that was in aim mode.")]
        public bool AutoAimIfSwitchedFromAimedWeapon = true;

        [Tooltip("Reference to the PlayerWeaponsManager script responsible for managing weapons.")]
        public PlayerWeaponsManager PlayerWeaponsManagerScript;

        [Tooltip("List of UI buttons that will be disabled during weapon switching to prevent interaction.")]
        public Button[] ButtonsToDisableDuringWeaponSwitching;

        [Tooltip("The swipeable area in the UI Canvas used to detect swipe gestures for weapon switching.")]
        public GameObject SwipeArea;

        [Tooltip("The button in the UI Canvas used to switch to the next weapon.")]
        public Button SwipeRightButton;

        [Tooltip("The button in the UI Canvas used to switch to the previous weapon.")]
        public Button SwipeLeftButton;

        [HideInInspector]
        [Tooltip("The current index of the active weapon in the weapon list.")]
        public int currentIndex = 0;

        [HideInInspector]
        [Tooltip("Indicates whether the previous weapon is still in use or in transition.")]
        public bool IsPreviousWeaponStillWalking;

        [HideInInspector]
        [Tooltip("Stores the index of the currently activated weapon.")]
        public int WeaponActivated;

        [HideInInspector]
        [Tooltip("Indicates whether to reactivate the same weapon after switching.")]
        public bool ActivateSameWeapon = false;

        [HideInInspector]
        [Tooltip("The index of the weapon to activate if 'ActivateSameWeapon' is true.")]
        public int SameWeaponIndex;


        private Vector2 startTouchPosition;
        private Vector2 endTouchPosition;
        private float swipeThreshold = 50f; // Adjust this value as needed for swipe sensitivity
 

        Image[] RaycastImages;

 
        bool DoNotContinue = false;

        PlayerGrenadeThrower PreviousGrenade;
        MeleeAttack PreviousMeleeAttack;

        //[HideInInspector]
        //public bool IsSwitchingWeapon = false;
 

        private void Awake()
        {
            ins = this;
        }
        void Start()
        {
            RaycastImages = new Image[ButtonsToDisableDuringWeaponSwitching.Length];
            for (int x = 0; x < ButtonsToDisableDuringWeaponSwitching.Length; x++)
            {
                if (ButtonsToDisableDuringWeaponSwitching[x].GetComponent<Image>() != null)
                {
                    RaycastImages[x] = ButtonsToDisableDuringWeaponSwitching[x].GetComponent<Image>();
                }
            }

            Recall();
            currentIndex = 0; // Set to the first weapon by default
            UpdateWeaponActivation();

            // Assign button listeners
            SwipeRightButton.onClick.AddListener(ActivateNextWeapon);
            SwipeLeftButton.onClick.AddListener(ActivatePreviousWeapon);
        }
        public void Recall()
        {
            if (AvailableWeapons == WeaponActivationType.ShopDependent)
            {
                for (int x = 0; x < PlayerWeaponsManagerScript.WeaponSlots.Count; x++)
                {
                    for (int i = 0; i < PlayerWeaponsManagerScript.WeaponSlots[x].Weapons.Count; i++)
                    {
                        if (PlayerPrefs.GetInt(PlayerWeaponsManagerScript.WeaponSlots[x].Weapons[i].WeaponIdComponent.WeaponName + currentIndex) == 1)
                        {
                            PlayerWeaponsManagerScript.WeaponSlots[x].Weapons[i].Weapon.gameObject.SetActive(true);
                            PlayerWeaponsManagerScript.ActiveWeaponsList.Add(PlayerWeaponsManagerScript.WeaponSlots[x].Weapons[i].Weapon);
                            ++currentIndex;
                        }
                    }
                }
            }
            else
            {
                if (PlayerWeaponsManagerScript.WeaponSlotManagerScript == null)
                {
                    PlayerPrefs.SetInt(PlayerWeaponsManagerScript.WeaponSlots[0].Weapons[0].WeaponIdComponent.WeaponName + currentIndex, 0);

                    for (int x = 0; x < PlayerWeaponsManagerScript.WeaponSlots.Count; x++)
                    {
                        for (int i = 0; i < PlayerWeaponsManagerScript.WeaponSlots[x].Weapons.Count; i++)
                        {
                            PlayerWeaponsManagerScript.ActiveWeaponsList.Add(PlayerWeaponsManagerScript.WeaponSlots[x].Weapons[i].Weapon);
                            if (PlayerPrefs.GetInt(PlayerWeaponsManagerScript.WeaponSlots[x].Weapons[i].WeaponIdComponent.WeaponName + currentIndex) == 1)
                            {
                                PlayerWeaponsManagerScript.WeaponSlots[x].Weapons[i].Weapon.gameObject.SetActive(true);
                                ++currentIndex;
                            }
                        }
                    }

                }
                else
                {
                    if(PlayerWeaponsManagerScript.WeaponSlotManagerScript.WeaponsToActivateByDefault <= 0)
                    {
                        PlayerWeaponsManagerScript.WeaponSlotManagerScript.WeaponsToActivateByDefault = 0;
                    }

                    PlayerPrefs.SetInt(PlayerWeaponsManagerScript.WeaponSlots[0].Weapons[0].WeaponIdComponent.WeaponName + currentIndex, 0);

                    for (int x = 0; x < PlayerWeaponsManagerScript.WeaponSlots.Count; x++)
                    {
                        for (int i = 0; i < PlayerWeaponsManagerScript.WeaponSlots[x].Weapons.Count; i++)
                        {
                            if (PlayerWeaponsManagerScript.ActiveWeaponsList.Count <= PlayerWeaponsManagerScript.WeaponSlotManagerScript.WeaponsToActivateByDefault - 1)
                            {
                                PlayerWeaponsManagerScript.ActiveWeaponsList.Add(PlayerWeaponsManagerScript.WeaponSlots[x].Weapons[i].Weapon);
                                if (PlayerPrefs.GetInt(PlayerWeaponsManagerScript.WeaponSlots[x].Weapons[i].WeaponIdComponent.WeaponName + currentIndex) == 1)
                                {
                                    PlayerWeaponsManagerScript.WeaponSlots[x].Weapons[i].Weapon.gameObject.SetActive(true);
                                    ++currentIndex;
                                }
                            }
                        }
                    }
                }    
            }

            if (PlayerWeaponsManagerScript.ShowAllAvailableWeaponSlots)
            {
                if (PlayerWeaponsManagerScript.WeaponSlotManagerScript == null)
                {
                    int getSlots = PlayerPrefs.GetInt("SlotsDefined", 0);
                    int currentWeaponCount = PlayerWeaponsManagerScript.ActiveWeaponsList.Count;
                    WeaponActivated = currentWeaponCount;

                    // Calculate how many empty slots to add
                    int emptySlotsToAdd = Mathf.Max(0, getSlots - currentWeaponCount);

                    // Add empty slots as placeholders
                    for (int i = 0; i < emptySlotsToAdd; i++)
                    {
                        PlayerWeaponsManagerScript.ActiveWeaponsList.Add(null); // Add a null or placeholder entry
                    }

                    // Optional: Update the UI to reflect empty slots
                   // UpdateUIWithEmptySlots();
                }
                else
                {
                    PlayerPrefs.SetInt("SlotsDefined", PlayerWeaponsManagerScript.WeaponSlotManagerScript.MaxWeaponsPlayerCarry);
                    int getSlots = PlayerPrefs.GetInt("SlotsDefined", 0);
                    int currentWeaponCount = PlayerWeaponsManagerScript.ActiveWeaponsList.Count;
                    WeaponActivated = currentWeaponCount;

                    // Calculate how many empty slots to add
                    int emptySlotsToAdd = Mathf.Max(0, getSlots - currentWeaponCount);

                    // Add empty slots as placeholders
                    for (int i = 0; i < emptySlotsToAdd; i++)
                    {
                        PlayerWeaponsManagerScript.ActiveWeaponsList.Add(null); // Add a null or placeholder entry
                    }

                    // Optional: Update the UI to reflect empty slots
                   // UpdateUIWithEmptySlots();
                }
            }

            if (PlayerWeaponsManagerScript.ActiveWeaponsList.Count <= 0)
            {
                if (PlayerWeaponsManagerScript.WeaponSlots[0] != null)
                {
                    if (PlayerWeaponsManagerScript.WeaponSlots[0].Weapons[0] != null)
                    {
                        PlayerWeaponsManagerScript.WeaponSlots[0].Weapons[0].Weapon.gameObject.SetActive(true);
                        PlayerWeaponsManagerScript.ActiveWeaponsList.Add(PlayerWeaponsManagerScript.WeaponSlots[0].Weapons[0].Weapon);
                        ++currentIndex;
                    }
                }
            }

        }

        // Optional helper method to update UI for empty slots
        //private void UpdateUIWithEmptySlots()
        //{
        //    foreach (var weapon in PlayerWeaponsManagerScript.ActiveWeaponsList)
        //    {
        //        if (weapon == null)
        //        {
        //            // Logic to display an empty slot in the UI
        //            //Debug.Log("Empty slot added.");
        //        }
        //    }
        //}


        //public void Recall()
        //{
        //    if(WeaponActivation == WeaponActivationType.SwitchBetweenShopMenuEquippedWeapons)
        //    {
        //        for (int x = 0; x < AllWeaponsInLevelScript.WeaponsSlot.Count; x++)
        //        {
        //            for (int i = 0; i < AllWeaponsInLevelScript.WeaponsSlot[x].Weapons.Count; i++)
        //            {
        //                if (PlayerPrefs.GetInt(AllWeaponsInLevelScript.WeaponsSlot[x].Weapons[i].WeaponIdComponent.WeaponName + currentIndex) == 1)
        //                {
        //                    AllWeaponsInLevelScript.WeaponsSlot[x].Weapons[i].Weapon.gameObject.SetActive(true);
        //                    AllWeaponsInLevelScript.DebugWeapons.Add(AllWeaponsInLevelScript.WeaponsSlot[x].Weapons[i].Weapon);
        //                    ++currentIndex;
        //                }
        //            }
        //        }
        //    }
        //    else
        //    {
        //        PlayerPrefs.SetInt(AllWeaponsInLevelScript.WeaponsSlot[0].Weapons[0].WeaponIdComponent.WeaponName + currentIndex, 0);

        //        for (int x = 0; x < AllWeaponsInLevelScript.WeaponsSlot.Count; x++)
        //        {
        //            for (int i = 0; i < AllWeaponsInLevelScript.WeaponsSlot[x].Weapons.Count; i++)
        //            {
        //                AllWeaponsInLevelScript.DebugWeapons.Add(AllWeaponsInLevelScript.WeaponsSlot[x].Weapons[i].Weapon);
        //                if (PlayerPrefs.GetInt(AllWeaponsInLevelScript.WeaponsSlot[x].Weapons[i].WeaponIdComponent.WeaponName + currentIndex) == 1)
        //                {
        //                    AllWeaponsInLevelScript.WeaponsSlot[x].Weapons[i].Weapon.gameObject.SetActive(true);
        //                    ++currentIndex;
        //                }
        //            }
        //        }
        //    }

        //}
        void Update()
        {
            // Handle swipe input for both touch and mouse
            HandleSwipe();
            //if (Input.GetKeyDown(KeyCode.Space))
            //{
            //    ActivateNextWeapon();
            //}
        }

        private void HandleSwipe()
        {
            RectTransform swipeRect = SwipeArea.GetComponent<RectTransform>();

            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Began)
                {
                    // Check if the touch began within the swipe area
                    if (RectTransformUtility.RectangleContainsScreenPoint(swipeRect, touch.position, null))
                    {
                        startTouchPosition = touch.position;
                    }
                }
                else if (touch.phase == TouchPhase.Ended && startTouchPosition != Vector2.zero)
                {
                    endTouchPosition = touch.position;
                    DetectSwipe();
                    startTouchPosition = Vector2.zero; // Reset to avoid detecting invalid swipes
                }
            }
            else if (Input.GetMouseButtonDown(0))
            {
                // Check if the mouse click started within the swipe area
                if (RectTransformUtility.RectangleContainsScreenPoint(swipeRect, Input.mousePosition, null))
                {
                    startTouchPosition = Input.mousePosition;
                }
            }
            else if (Input.GetMouseButtonUp(0) && startTouchPosition != Vector2.zero)
            {
                endTouchPosition = Input.mousePosition;
                DetectSwipe();
                startTouchPosition = Vector2.zero; // Reset to avoid detecting invalid swipes
            }
        }

        private void DetectSwipe()
        {
            float horizontalSwipeDistance = endTouchPosition.x - startTouchPosition.x;

            if (Mathf.Abs(horizontalSwipeDistance) > swipeThreshold)
            {
                if (horizontalSwipeDistance > 0)
                {
                    ActivateNextWeapon();
                }
                else
                {
                    ActivatePreviousWeapon();
                }
            }
        }

        public void ActivateNextWeapon()
        {
            DoNotContinue = false;

            for (int i = 0; i < PlayerWeaponsManagerScript.PlayerGrenadeThrowerScripts.Length; i++)
            {
                if (PlayerWeaponsManagerScript.PlayerGrenadeThrowerScripts[i].RequiredComponents.GrenadeAnimatorComponent.gameObject.activeInHierarchy == true)
                {
                    PreviousGrenade = PlayerWeaponsManagerScript.PlayerGrenadeThrowerScripts[i];
                    StartCoroutine(WeaponActivator(true));
                    DoNotContinue = true;
                }
            }



            for (int i = 0; i < PlayerWeaponsManagerScript.MeleeAttackScripts.Length; i++)
            {
                if (PlayerWeaponsManagerScript.MeleeAttackScripts[i].RequiredComponents.MeleeAnimatorComponent.gameObject.activeInHierarchy == true)
                {
                    PreviousMeleeAttack = PlayerWeaponsManagerScript.MeleeAttackScripts[i];
                    StartCoroutine(WeaponActivator(true));
                    DoNotContinue = true;
                }
            }

            if (DoNotContinue == false)
            {
                if (WeaponActivated >= 2)
                {
                    StartCoroutine(WeaponActivator(true));
                }
            }

        }
        public void ActivatePreviousWeapon()
        {
            DoNotContinue = false;

            for (int i = 0; i < PlayerWeaponsManagerScript.PlayerGrenadeThrowerScripts.Length; i++)
            {
                if (PlayerWeaponsManagerScript.PlayerGrenadeThrowerScripts[i].RequiredComponents.GrenadeAnimatorComponent.gameObject.activeInHierarchy == true)
                {
                    PreviousGrenade = PlayerWeaponsManagerScript.PlayerGrenadeThrowerScripts[i];
                    StartCoroutine(WeaponActivator(false));
                    DoNotContinue = true;
                }
            }

            for (int i = 0; i < PlayerWeaponsManagerScript.MeleeAttackScripts.Length; i++)
            {
                if (PlayerWeaponsManagerScript.MeleeAttackScripts[i].RequiredComponents.MeleeAnimatorComponent.gameObject.activeInHierarchy == true)
                {
                    PreviousMeleeAttack = PlayerWeaponsManagerScript.MeleeAttackScripts[i];
                    StartCoroutine(WeaponActivator(false));
                    DoNotContinue = true;
                }
            }

            if (DoNotContinue == false)
            {
                if (WeaponActivated >= 2)
                {
                    StartCoroutine(WeaponActivator(false));
                }
            }
        }
        public void UpdateNextWeapon()
        {
            if (DoNotContinue == false)
            {
                currentIndex = (currentIndex + 1) % PlayerWeaponsManagerScript.ActiveWeaponsList.Count;
            }
        }
        public void UpdatePreviuosWeapon()
        {
            if (DoNotContinue == false)
            {
                currentIndex = (currentIndex - 1 + PlayerWeaponsManagerScript.ActiveWeaponsList.Count) % PlayerWeaponsManagerScript.ActiveWeaponsList.Count;
            }
        }
        IEnumerator WeaponActivator(bool IsNextWeapon)
        {
            //IsSwitchingWeapon = true;
            for (int x = 0; x < ButtonsToDisableDuringWeaponSwitching.Length; x++)
            {
                ButtonsToDisableDuringWeaponSwitching[x].interactable = false;
            }
            for (int x = 0; x < RaycastImages.Length; x++)
            {
                RaycastImages[x].raycastTarget = false;
            }

            if (PlayerManager.instance != null)
            {
                PlayerManager.instance.CurrentHoldingPlayerWeapon.RequiredComponents.WeaponAnimatorComponent.Play(PlayerManager.instance.CurrentHoldingPlayerWeapon.RemoveAnimationName, -1, 0f);
            }

            if (PlayerManager.instance.CurrentHoldingPlayerWeapon != null)
            {
                if (PlayerManager.instance.CurrentHoldingPlayerWeapon.gameObject.activeInHierarchy == true)
                {
                    yield return new WaitForSeconds(PlayerManager.instance.CurrentHoldingPlayerWeapon.RemoveLength);
                }
                else
                {
                    yield return new WaitForSeconds(0.01f);
                }
            }
            else
            {
                yield return new WaitForSeconds(0.01f);
            }

            if (PreviousGrenade != null)
            {
                PreviousGrenade.DeactiveHand();
            }

            if (PreviousMeleeAttack != null)
            {
                PreviousMeleeAttack.DeactiveHand();
            }

            for (int i = 0; i < PlayerWeaponsManagerScript.PlayerGrenadeThrowerScripts.Length; i++)
            {
                PlayerWeaponsManagerScript.PlayerGrenadeThrowerScripts[i].RequiredComponents.GrenadeAnimatorComponent.gameObject.SetActive(false);
            }

            for (int i = 0; i < PlayerWeaponsManagerScript.MeleeAttackScripts.Length; i++)
            {
                PlayerWeaponsManagerScript.MeleeAttackScripts[i].RequiredComponents.MeleeAnimatorComponent.gameObject.SetActive(false);
            }

            if (PlayerManager.instance != null)
            {
                if (AutoAimIfSwitchedFromAimedWeapon == false)
                {
                    if (PlayerManager.instance.CurrentHoldingPlayerWeapon.IsAimed == true)
                    {
                        PlayerManager.instance.Aiming();
                        PlayerManager.instance.CurrentHoldingPlayerWeapon.IsAimed = false;
                        PlayerManager.instance.CurrentHoldingPlayerWeapon.IsHipFire = true;
                    }
                }

                IsPreviousWeaponStillWalking = PlayerManager.instance.CurrentHoldingPlayerWeapon.IsWalking;
            }

            // new code
            if (IsNextWeapon == true)
            {
                if (ActivateSameWeapon == false)
                {
                    UpdateNextWeapon();

                    if (PlayerWeaponsManagerScript.ActiveWeaponsList[currentIndex] == null)
                    {
                        for (int x = 0; x < PlayerWeaponsManagerScript.ActiveWeaponsList.Count; x++)
                        {
                            UpdateNextWeapon();
                            if (PlayerWeaponsManagerScript.ActiveWeaponsList[currentIndex] != null)
                            {
                                break;
                            }
                        }
                    }

                    UpdateWeaponActivation();
                    if (PlayerManager.instance != null)
                    {
                        PlayerManager.instance.FindRequiredObjects();
                        if (AutoAimIfSwitchedFromAimedWeapon == false)
                        {
                            if (PlayerManager.instance.CurrentHoldingPlayerWeapon.IsAimed == true)
                            {
                                PlayerManager.instance.Aiming();
                                PlayerManager.instance.CurrentHoldingPlayerWeapon.IsAimed = false;
                                PlayerManager.instance.CurrentHoldingPlayerWeapon.IsHipFire = true;
                            }
                        }
                    }
                }
                else
                {
                    currentIndex = SameWeaponIndex;
                    UpdateWeaponActivation();
                    if (PlayerManager.instance != null)
                    {
                        PlayerManager.instance.FindRequiredObjects();
                        if (AutoAimIfSwitchedFromAimedWeapon == false)
                        {
                            if (PlayerManager.instance.CurrentHoldingPlayerWeapon.IsAimed == true)
                            {
                                PlayerManager.instance.Aiming();
                                PlayerManager.instance.CurrentHoldingPlayerWeapon.IsAimed = false;
                                PlayerManager.instance.CurrentHoldingPlayerWeapon.IsHipFire = true;
                            }
                        }
                    }
                }
            }
            else
            {
                UpdatePreviuosWeapon();
                if (PlayerWeaponsManagerScript.ActiveWeaponsList[currentIndex] == null)
                {
                    for (int x = 0; x < PlayerWeaponsManagerScript.ActiveWeaponsList.Count; x++)
                    {
                        UpdatePreviuosWeapon();
                        if (PlayerWeaponsManagerScript.ActiveWeaponsList[currentIndex] != null)
                        {
                            break;
                        }
                    }
                }

                UpdateWeaponActivation();
                if (PlayerManager.instance != null)
                {
                    PlayerManager.instance.FindRequiredObjects();
                    if (AutoAimIfSwitchedFromAimedWeapon == false)
                    {
                        if (PlayerManager.instance.CurrentHoldingPlayerWeapon.IsAimed == true)
                        {
                            PlayerManager.instance.Aiming();
                            PlayerManager.instance.CurrentHoldingPlayerWeapon.IsAimed = false;
                            PlayerManager.instance.CurrentHoldingPlayerWeapon.IsHipFire = true;
                        }
                    }
                }
            }


            yield return new WaitForSeconds(PlayerManager.instance.CurrentHoldingPlayerWeapon.WieldTime);
            //IsSwitchingWeapon = false;
            ActivateButtons();
            ActivateSameWeapon = false;
        }
        public void ActivateButtons()
        {
            for (int x = 0; x < ButtonsToDisableDuringWeaponSwitching.Length; x++)
            {
                ButtonsToDisableDuringWeaponSwitching[x].interactable = true;
            }
            for (int x = 0; x < RaycastImages.Length; x++)
            {
                RaycastImages[x].raycastTarget = true;
            }
        }
        private void UpdateWeaponActivation()
        {
            // Deactivate all weapons first
            foreach (GameObject weapon in PlayerWeaponsManagerScript.ActiveWeaponsList)
            {
                if (weapon != null)
                {
                    weapon.SetActive(false);
                }
            }

            // Activate the current weapon
            if (PlayerWeaponsManagerScript.ActiveWeaponsList.Count > 0 && currentIndex >= 0 && currentIndex < PlayerWeaponsManagerScript.ActiveWeaponsList.Count)
            {
                if (PlayerWeaponsManagerScript.ActiveWeaponsList[currentIndex] != null)
                {
                    PlayerWeaponsManagerScript.ActiveWeaponsList[currentIndex].SetActive(true);
                }

            }
        }
        public void ResetOtherMeleeOrGrenadeHandsIfActivated()
        {
            for (int i = 0; i < PlayerWeaponsManagerScript.PlayerGrenadeThrowerScripts.Length; i++)
            {
                if (PlayerWeaponsManagerScript.PlayerGrenadeThrowerScripts[i].RequiredComponents.GrenadeAnimatorComponent.gameObject.activeInHierarchy == false)
                {
                    PlayerWeaponsManagerScript.PlayerGrenadeThrowerScripts[i].ResetHands();
                }
            }

            for (int i = 0; i < PlayerWeaponsManagerScript.MeleeAttackScripts.Length; i++)
            {
                if (PlayerWeaponsManagerScript.MeleeAttackScripts[i].RequiredComponents.MeleeAnimatorComponent.gameObject.activeInHierarchy == false)
                {
                    PlayerWeaponsManagerScript.MeleeAttackScripts[i].ResetHands();
                }
            }
        }


        //public void ActivateNextWeapon()
        //{
        //    currentIndex = (currentIndex + 1) % AllWeaponsInLevelScript.DebugWeapons.Count;
        //    UpdateWeaponActivation();
        //    PlayerManager.instance.FindRequiredObjects();
        //}

        //public void ActivatePreviousWeapon()
        //{
        //    currentIndex = (currentIndex - 1 + AllWeaponsInLevelScript.DebugWeapons.Count) % AllWeaponsInLevelScript.DebugWeapons.Count;
        //    UpdateWeaponActivation();
        //    PlayerManager.instance.FindRequiredObjects();
        //}

        //private void UpdateWeaponActivation()
        //{
        //    // Deactivate all weapons first
        //    foreach (GameObject weapon in AllWeaponsInLevelScript.DebugWeapons)
        //    {
        //        weapon.SetActive(false);
        //    }

        //    // Activate the current weapon
        //    if (AllWeaponsInLevelScript.DebugWeapons.Count > 0 && currentIndex >= 0 && currentIndex < AllWeaponsInLevelScript.DebugWeapons.Count)
        //    {
        //        AllWeaponsInLevelScript.DebugWeapons[currentIndex].SetActive(true);
        //    }
        //}
    }

}

//using System.Collections.Generic;
//using MobileActionKit;
//using UnityEngine;
//using UnityEngine.UI;

//public class ActivateShopMenuWeapons : MonoBehaviour
//{
//    public GameObject SwipeArea;
//    public Button SwipeRightButton;
//    public Button SwipeLeftButton;

//    public int SlotsSpecifiedInShop = 2;

//    [System.Serializable]
//    public class WeaponHolder
//    {
//        public WeaponId WeaponId;
//        public GameObject Weapon;
//    }

//    public List<WeaponHolder> WeaponsList = new List<WeaponHolder>();

//    int currentIndex = 0;
//    public List<GameObject> DebugWeapons = new List<GameObject>();

//    private Vector2 startTouchPosition;
//    private Vector2 endTouchPosition;
//    private float swipeThreshold = 50f;  // Adjust this value as needed for swipe sensitivity

//    void Start()
//    {
//        Recall();
//        currentIndex = 0; // Set to the first weapon by default
//        UpdateWeaponActivation();

//        // Assign button listeners
//        SwipeRightButton.onClick.AddListener(ActivateNextWeapon);
//        SwipeLeftButton.onClick.AddListener(ActivatePreviousWeapon);
//    }

//    public void Recall()
//    {
//        for (int x = 0; x < WeaponsList.Count; x++)
//        {
//            if (PlayerPrefs.GetInt(WeaponsList[x].WeaponId.WeaponName + currentIndex) == 1)
//            {
//                WeaponsList[x].Weapon.gameObject.SetActive(true);
//                DebugWeapons.Add(WeaponsList[x].Weapon);
//                ++currentIndex;
//            }
//        }
//    }

//    void Update()
//    {
//        // Handle swipe input for both touch and mouse
//        HandleSwipe();
//    }

//    private void HandleSwipe()
//    {
//        if (Input.touchCount > 0)
//        {
//            Touch touch = Input.GetTouch(0);

//            if (touch.phase == TouchPhase.Began)
//            {
//                startTouchPosition = touch.position;
//            }
//            else if (touch.phase == TouchPhase.Ended)
//            {
//                endTouchPosition = touch.position;
//                DetectSwipe();
//            }
//        }
//        else if (Input.GetMouseButtonDown(0))
//        {
//            startTouchPosition = Input.mousePosition;
//        }
//        else if (Input.GetMouseButtonUp(0))
//        {
//            endTouchPosition = Input.mousePosition;
//            DetectSwipe();
//        }
//    }

//    private void DetectSwipe()
//    {
//        float horizontalSwipeDistance = endTouchPosition.x - startTouchPosition.x;

//        if (Mathf.Abs(horizontalSwipeDistance) > swipeThreshold)
//        {
//            if (horizontalSwipeDistance > 0)
//            {
//                ActivateNextWeapon();
//            }
//            else
//            {
//                ActivatePreviousWeapon();
//            }
//        }
//    }

//    private void ActivateNextWeapon()
//    {
//        // Move to the next weapon, wrapping around if at the last weapon
//        currentIndex = (currentIndex + 1) % DebugWeapons.Count;
//        UpdateWeaponActivation();
//    }

//    private void ActivatePreviousWeapon()
//    {
//        // Move to the previous weapon, wrapping around if at the first weapon
//        currentIndex = (currentIndex - 1 + DebugWeapons.Count) % DebugWeapons.Count;
//        UpdateWeaponActivation();
//    }

//    private void UpdateWeaponActivation()
//    {
//        // Deactivate all weapons first
//        foreach (GameObject weapon in DebugWeapons)
//        {
//            weapon.SetActive(false);
//        }

//        // Activate the current weapon
//        if (DebugWeapons.Count > 0 && currentIndex >= 0 && currentIndex < DebugWeapons.Count)
//        {
//            DebugWeapons[currentIndex].SetActive(true);
//        }
//    }
//}



//using System.Collections.Generic;
//using MobileActionKit;
//using UnityEngine;
//using UnityEngine.UI;

//public class ActivateShopMenuWeapons : MonoBehaviour
//{
//    public GameObject SwipeArea;
//    public Button SwipeRightButton;
//    public Button SwipeLeftButton;

//    public int SlotsSpecifiedInShop = 2;

//    [System.Serializable]
//    public class WeaponHolder
//    {
//        public WeaponId WeaponId;
//        public GameObject Weapon;
//    }

//    public List<WeaponHolder> WeaponsList = new List<WeaponHolder>();

//    int currentIndex = 0;
//    public List<GameObject> DebugWeapons = new List<GameObject>();

//    private Vector2 startTouchPosition;
//    private Vector2 endTouchPosition;
//    private float swipeThreshold = 50f;  // Adjust this value as needed for swipe sensitivity

//    void Start()
//    {
//        Recall();
//        currentIndex = 0; // Set to the first weapon by default
//        UpdateWeaponActivation();

//        // Assign button listeners
//        SwipeRightButton.onClick.AddListener(ActivateNextWeapon);
//        SwipeLeftButton.onClick.AddListener(ActivatePreviousWeapon);
//    }

//    public void Recall()
//    {
//        for (int x = 0; x < WeaponsList.Count; x++)
//        {
//            if (PlayerPrefs.GetInt(WeaponsList[x].WeaponId.WeaponName + currentIndex) == 1)
//            {
//                WeaponsList[x].Weapon.gameObject.SetActive(true);
//                DebugWeapons.Add(WeaponsList[x].Weapon);
//                ++currentIndex;
//            }
//        }
//    }

//    void Update()
//    {
//        // Handle swipe input for both touch and mouse
//        HandleSwipe();
//    }

//    private void HandleSwipe()
//    {
//        if (Input.touchCount > 0)
//        {
//            Touch touch = Input.GetTouch(0);

//            if (touch.phase == TouchPhase.Began)
//            {
//                startTouchPosition = touch.position;
//            }
//            else if (touch.phase == TouchPhase.Ended)
//            {
//                endTouchPosition = touch.position;
//                DetectSwipe();
//            }
//        }
//        else if (Input.GetMouseButtonDown(0))
//        {
//            startTouchPosition = Input.mousePosition;
//        }
//        else if (Input.GetMouseButtonUp(0))
//        {
//            endTouchPosition = Input.mousePosition;
//            DetectSwipe();
//        }
//    }

//    private void DetectSwipe()
//    {
//        float horizontalSwipeDistance = endTouchPosition.x - startTouchPosition.x;

//        if (Mathf.Abs(horizontalSwipeDistance) > swipeThreshold)
//        {
//            if (horizontalSwipeDistance > 0)
//            {
//                ActivateNextWeapon();
//            }
//            else
//            {
//                ActivatePreviousWeapon();
//            }
//        }
//    }

//    private void ActivateNextWeapon()
//    {
//        if (currentIndex < DebugWeapons.Count - 1)
//        {
//            currentIndex++;
//            UpdateWeaponActivation();
//        }
//    }

//    private void ActivatePreviousWeapon()
//    {
//        if (currentIndex > 0)
//        {
//            currentIndex--;
//            UpdateWeaponActivation();
//        }
//    }

//    private void UpdateWeaponActivation()
//    {
//        // Deactivate all weapons first
//        foreach (GameObject weapon in DebugWeapons)
//        {
//            weapon.SetActive(false);
//        }

//        // Activate the current weapon
//        if (DebugWeapons.Count > 0 && currentIndex >= 0 && currentIndex < DebugWeapons.Count)
//        {
//            DebugWeapons[currentIndex].SetActive(true);
//        }
//    }
//}



//using System.Collections.Generic;
//using MobileActionKit;
//using UnityEngine;
//using UnityEngine.UI;

//public class ActivateShopMenuWeapons : MonoBehaviour
//{
//    public GameObject SwipeArea;
//    public Button SwipeRightButton;
//    public Button SwipeLeftButton;

//    public int SlotsSpecifiedInShop = 2;
//    public WeaponId[] WeaponsWithWeaponIDScript;
//    public GameObject[] Weapons;

//    int currentIndex = 0;
//    public List<GameObject> DebugWeapons = new List<GameObject>();

//    private Vector2 startTouchPosition;
//    private Vector2 endTouchPosition;
//    private float swipeThreshold = 50f;  // Adjust this value as needed for swipe sensitivity

//    void Start()
//    {
//        Recall();
//        UpdateWeaponActivation();

//        // Assign button listeners
//        SwipeRightButton.onClick.AddListener(ActivateNextWeapon);
//        SwipeLeftButton.onClick.AddListener(ActivatePreviousWeapon);
//    }
//    public void Recall()
//    {
//        for (int x = 0; x < WeaponsWithWeaponIDScript.Length; x++)
//        {
//            if (PlayerPrefs.GetInt(WeaponsWithWeaponIDScript[x].WeaponName + currentIndex) == 1)
//            {
//                Weapons[x].gameObject.SetActive(true);
//                DebugWeapons.Add(Weapons[x].gameObject);
//                ++currentIndex;
//            }
//        }
//    }
//    void Update()
//    {
//        // Handle swipe input for both touch and mouse
//        HandleSwipe();
//    }

//    private void HandleSwipe()
//    {
//        if (Input.touchCount > 0)
//        {
//            Touch touch = Input.GetTouch(0);

//            if (touch.phase == TouchPhase.Began)
//            {
//                startTouchPosition = touch.position;
//            }
//            else if (touch.phase == TouchPhase.Ended)
//            {
//                endTouchPosition = touch.position;
//                DetectSwipe();
//            }
//        }
//        else if (Input.GetMouseButtonDown(0))
//        {
//            startTouchPosition = Input.mousePosition;
//        }
//        else if (Input.GetMouseButtonUp(0))
//        {
//            endTouchPosition = Input.mousePosition;
//            DetectSwipe();
//        }
//    }

//    private void DetectSwipe()
//    {
//        float horizontalSwipeDistance = endTouchPosition.x - startTouchPosition.x;

//        if (Mathf.Abs(horizontalSwipeDistance) > swipeThreshold)
//        {
//            if (horizontalSwipeDistance > 0)
//            {
//                ActivateNextWeapon();
//            }
//            else
//            {
//                ActivatePreviousWeapon();
//            }
//        }
//    }

//    private void ActivateNextWeapon()
//    {
//        if (currentIndex < DebugWeapons.Count - 1)
//        {
//            currentIndex++;
//            UpdateWeaponActivation();
//        }
//    }

//    private void ActivatePreviousWeapon()
//    {
//        if (currentIndex > 0)
//        {
//            currentIndex--;
//            UpdateWeaponActivation();
//        }
//    }

//    private void UpdateWeaponActivation()
//    {
//        // Deactivate all weapons first
//        foreach (GameObject weapon in DebugWeapons)
//        {
//            weapon.SetActive(false);
//        }

//        // Activate the current weapon
//        if (DebugWeapons.Count > 0 && currentIndex >= 0 && currentIndex < DebugWeapons.Count)
//        {
//            DebugWeapons[currentIndex].SetActive(true);
//        }
//    }
//}



//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using MobileActionKit;
//using UnityEngine.UI;

//public class ActivateShopMenuWeapons : MonoBehaviour
//{
//    public GameObject SwipeArea;
//    public Button SwipeRightButton;
//    public Button SwipeLeftButton;

//    public int SlotsSpecifiedInShop = 2;
//    public WeaponId[] WeaponsWithWeaponIDScript;

//    int currentIndex;

//    public List<GameObject> DebugWeapons = new List<GameObject>();

//    void Start()
//    {
//        Recall();
//    }
//    public void Recall()
//    {
//        for (int x = 0; x < WeaponsWithWeaponIDScript.Length; x++)
//        {
//            if (PlayerPrefs.GetInt(WeaponsWithWeaponIDScript[x].WeaponName + currentIndex) == 1)
//            {
//                WeaponsWithWeaponIDScript[x].gameObject.SetActive(true);
//                DebugWeapons.Add(WeaponsWithWeaponIDScript[x].gameObject);
//                ++currentIndex;
//            }
//        }
//    }
//}
