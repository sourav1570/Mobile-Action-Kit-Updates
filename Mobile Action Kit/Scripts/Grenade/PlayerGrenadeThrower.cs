using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using System.Collections.Generic;

namespace MobileActionKit
{
    public class PlayerGrenadeThrower : MonoBehaviour
    {
        public static PlayerGrenadeThrower instance;

        [TextArea]
        public string ScriptInfo = "This script regulates various parameters of player`s grenade throwing functionality.";

        [ReadOnly]
        public string GrenadeUtilizationType = "Immediate Throw";

        [Tooltip("Select one of the two implementation options for grenade. Instantiation or Object pooling.")]
        public GrenadeType ImplementationOptions;

        [Tooltip("This must be exact same name as the one inside Object pooler.")]
        public string NameInsideObjectPooler;

        bool KeepActive = true;
        bool DelayFirstThrowsOnly = true;

        [System.Serializable]
        public class Components
        {
            [Tooltip("Player camera from the hierarchy is to be placed into this field.")]
            public Transform PlayerCamera;
            [Tooltip("'GrenadeAnimations' gameobject with respective Animator controller from player`s hierarchy is to be placed into this field.")]
            public Animator GrenadeAnimatorComponent;
            [Tooltip("'Grenade' Ui image(Icon) from 2D Canvas is to be placed into this field.")]
            public GameObject GrenadeIcon;
            [Tooltip("'GrenadeThrow' audio source that is a child of 'PlayerAudioSourceComponents' gameobject of the player`s hierarchy is to be placed into this field.")]
            public AudioSource GrenadeThrowSound;

            [Tooltip("'GrenadePin' audio source that is a child of 'PlayerAudioSourceComponents' gameobject of the player`s hierarchy is to be placed into this field.")]
            public AudioSource GrenadePinSound;

            [Tooltip("GrenadeSpawnPoint child gameObject of player`s hierarchy is to be placed into this field.")]
            public Transform GrenadeSpawnPoint;

            [Tooltip("Grenade prefab from project tab is to be placed into this field.")]
            public GameObject GrenadePrefab;

            public PlayerWeapon PlayerWeaponScript;

            public List<Button> GrenadeSelectOrThrowUIButtons = new List<Button>();
        }

        [Tooltip("All required components are to be placed into their respective fields.")]
        public Components RequiredComponents = new Components();

        [System.Serializable]
        public class Delay
        {
            //public float AfterThrowIdleDelay;
            [Tooltip("Specify the delay of pin removal sound playback.")]
            public float PinSoundDelay;
            [Tooltip("Player`s hands with grenade activation delay regardless of the grenade utilisation option(Immediate Throw or Selection).")]
            public float GrenadeHandsActivationDelay = 2f;
            [Tooltip("Specify the grenade prefab (activation or instantiation) delay so that it would match your grenade throw animation.")]
            public float GrenadeSpawnDelay = 1f;
          
        }

        public Delay Delays;


        GameObject PrevIcon;

        public enum GrenadeType
        {
            UseObjectPooler,
            UseInstantiate
        }


        [System.Serializable]
        public class Force
        {  
            [Tooltip("Minimal possible angle for intended throw distance.")]
            public float MinAngle;
            [Tooltip(" Maximal possible angle for intended throw distance.")]
            public float MaxAngle;
            [Tooltip("Set the force to apply to grenade prefab to achieve desired throw distance for this case.")]
            public float ForceToApply = 7f;
        }

        [Tooltip("You can specify few different ranges for how far grenade will be thrown depending of the player camera angle at the moment of the throw.")]
        public List<Force> ThrowPower = new List<Force>();

        [System.Serializable]
        public class Interaction
        {
            public float UiReactivationDelay;
            [Tooltip("The Disabled Color Of Image For Example : Grey ")]
            public Color UIDisabledColor = Color.gray;
            [Tooltip("Specify Ui elements to disable before player hands with grenade will appear in the view(if 'Selection' option was selected in the grenade creation wizard).")]
            public Image[] BeforeSelection;
            [Tooltip("Specify Ui elements to disable for the duration of the player hands grenade throw process(if 'ImmediateThrow' option was selected in the grenade creation wizard).")]
            public Image[] UIToDisableBeforeThrow;
        }

        public Interaction UIInteractions;

        bool PoolSpawnType = false;
        private ObjectPooler pooler;
        GameObject bomb;

        [HideInInspector]
        public bool FirstTimeIter = false;

        [HideInInspector]
        public bool SelectGrenadeHands = false;

        [HideInInspector]
        public bool DonotDelay = false;

        RuntimeAnimatorController ac;

        [HideInInspector]
        public float ThrowingPower = 0;

        GameObject StoreSelectedWeapon;

        [HideInInspector]
        public bool StartSpawning = false;

        GameObject PreviousWeapon;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }

            if (RequiredComponents.GrenadeAnimatorComponent.gameObject.GetComponent<PlayerWeapon>() != null)
            {
                KeepActive = true;
                DelayFirstThrowsOnly = true;
            }
            else
            {
                KeepActive = false;
                DelayFirstThrowsOnly = false;
            }
        }
        private void Start()
        {
            if (ObjectPooler.instance != null)
            {
                pooler = ObjectPooler.instance;
            }

            if (GrenadeType.UseObjectPooler == ImplementationOptions)
            {
                PoolSpawnType = true;
            }
            else
            {
                PoolSpawnType = false;
            }

            if(RequiredComponents.GrenadeSelectOrThrowUIButtons != null)
            {
                for (int x = 0;x <  RequiredComponents.GrenadeSelectOrThrowUIButtons.Count;x++)
                {
                    RequiredComponents.GrenadeSelectOrThrowUIButtons[x].onClick.AddListener(ThrowGrenade);
                }
            }
            if (RequiredComponents.PlayerWeaponScript != null)
            {
                RequiredComponents.PlayerWeaponScript.NotAShootingWeapon = true;
            }

            ac = RequiredComponents.GrenadeAnimatorComponent.runtimeAnimatorController;    
                                                                                           
        }
        public void ThrowGrenade()
        {
            if (PlayerManager.instance.CurrentHoldingPlayerWeapon != null)
            {
                if (PlayerManager.instance.CurrentHoldingPlayerWeapon.IsAimed == true)
                {
                    PlayerManager.instance.Aiming();
                    PlayerManager.instance.CurrentHoldingPlayerWeapon.IsAimed = false;
                    PlayerManager.instance.CurrentHoldingPlayerWeapon.IsHipFire = true;
                    
                }
            }

            ButtonsFunction(UIInteractions.BeforeSelection, false);

            if (RequiredComponents.GrenadeAnimatorComponent.gameObject.GetComponent<PlayerWeapon>() != null && SelectGrenadeHands == false && RequiredComponents.GrenadeAnimatorComponent.gameObject.activeInHierarchy == false)
            {
                StartCoroutine(ActivateHands());
                SelectGrenadeHands = true;
            }
            else
            {
                if (StartSpawning == false)
                {
                    PlayerPrefs.SetInt("Item_Grenade_InsendiaryIsInUse", 1);
                    StartCoroutine(GrenadeSpawn());
                    StartSpawning = true;
                }
            }

        }
        public void CalculateForce()
        {
            bool DoOnce = false;

            for (int x = 0; x < ThrowPower.Count; x++)
            {
                if (DoOnce == false)
                {               
                    float angle = RequiredComponents.PlayerCamera.localEulerAngles.x;
                    angle = (angle > 180) ? angle - 360 : angle;
  
                    if (angle <= ThrowPower[x].MaxAngle && angle >= ThrowPower[x].MinAngle)
                    {
                        ThrowingPower = ThrowPower[x].ForceToApply;
                        DoOnce = true;
                    }
                }
            }
        }
        //public void BackToPrevWeapon()
        //{

        //    RequiredComponents.
        //}
        //IEnumerator RemoveGrenadeHands()
        //{
        //    yield return new WaitForSeconds(0.01f);
        //    if (FirstTimeIter == false)
        //    {
        //        for (int i = 0; i < RequiredComponents.AllShootingScripts.Length; i++)
        //        {
        //            if (RequiredComponents.AllShootingScripts[i].gameObject.activeInHierarchy == true)
        //            {
        //                RequiredComponents.AllShootingScripts[i].Components.WeaponAnimatorComponent.Play(RequiredComponents.AllShootingScripts[i].RemoveAnimationName, -1, 0f);
        //                PreviousWeapon = RequiredComponents.AllShootingScripts[i].Components.WeaponAnimatorComponent.gameObject;
        //            }
        //        }
        //        FirstTimeIter = true;
        //    }
        //    if (DelayFirstThrowsOnly == true && DonotDelay == false)
        //    {
        //        yield return new WaitForSeconds(Delays.GrenadeAnimationDelay);
        //        DonotDelay = true;
        //    }
        //    else if (DelayFirstThrowsOnly == false)
        //    {
        //        yield return new WaitForSeconds(Delays.GrenadeAnimationDelay);
        //    }

        //    ButtonsFunction(UIInteraction.UIToDisableBeforeActivation, true);
        //    RequiredComponents.GrenadeThrowSound.Play();
        //    RequiredComponents.GrenadeAnimatorComponent.gameObject.SetActive(true);

        //    for (int i = 0; i < RequiredComponents.AllShootingScripts.Length; i++)
        //    {
        //        if (RequiredComponents.AllShootingScripts[i].gameObject.activeInHierarchy == true)
        //        {
        //            RequiredComponents.AllShootingScripts[i].Components.WeaponAnimatorComponent.enabled = false;
        //            RequiredComponents.AllShootingScripts[i].gameObject.SetActive(false);
        //        }
        //    }
        //    RequiredComponents.GrenadeIcon.SetActive(true);
        //    if (PlayerManager.instance != null)
        //    {
        //        PlayerManager.instance.FindRequiredObjects();
        //    }
        //}

        IEnumerator ActivateHands()
        {
            yield return new WaitForSeconds(0.01f);
            if (FirstTimeIter == false)
            {
                for (int i = 0; i < PlayerManager.instance.PlayerWeaponScripts.Count; i++)
                {
                    if (PlayerManager.instance.PlayerWeaponScripts[i].gameObject.activeInHierarchy == true && RequiredComponents.GrenadeAnimatorComponent.gameObject.activeInHierarchy == false)
                    {
                        //    ResetAimedState(AllWeaponsShootingScripts[i]);

                        PlayerManager.instance.PlayerWeaponScripts[i].RequiredComponents.WeaponAnimatorComponent.Play(PlayerManager.instance.PlayerWeaponScripts[i].RemoveAnimationName, -1, 0f);
                        PreviousWeapon = PlayerManager.instance.PlayerWeaponScripts[i].RequiredComponents.WeaponAnimatorComponent.gameObject;
                    }
                }
                FirstTimeIter = true;
            }
            if (DelayFirstThrowsOnly == true && DonotDelay == false)
            {
                yield return new WaitForSeconds(Delays.GrenadeHandsActivationDelay);
                DonotDelay = true;
            }
            else if (DelayFirstThrowsOnly == false)
            {
                yield return new WaitForSeconds(Delays.GrenadeHandsActivationDelay);
            }

            for (int i = 0; i < PlayerManager.instance.PlayerWeaponScripts.Count; i++)
            {
                if (PlayerManager.instance.PlayerWeaponScripts[i].gameObject.activeInHierarchy == true && RequiredComponents.GrenadeAnimatorComponent.gameObject.activeInHierarchy == false)
                {
                    PlayerManager.instance.PlayerWeaponScripts[i].RequiredComponents.WeaponAnimatorComponent.gameObject.SetActive(false);
                }
            }

            ButtonsFunction(UIInteractions.BeforeSelection, true);
            //   MeshesDisable(false);
            //EnableParent(false);
            RequiredComponents.GrenadeThrowSound.Play();
            RequiredComponents.GrenadeAnimatorComponent.gameObject.SetActive(true);
            //if (SwitchWeapons.instance != null)
            //{
            //    for (int x = 0; x < SwitchWeapons.instance.AllWeaponIcons.Length; x++)
            //    {
            //        if (SwitchWeapons.instance.AllWeaponIcons[x].activeInHierarchy == true)
            //        {
            //            PrevIcon = SwitchWeapons.instance.AllWeaponIcons[x].gameObject;
            //        }
            //        SwitchWeapons.instance.AllWeaponIcons[x].SetActive(false);
            //    }
            //}
          
            //Making sure to reset previous grenade hand or melee hands if activated ( Enable Melee Selection or Enable Grenade Selection checkbox is checked )

            if(SwitchingPlayerWeapons.ins != null)
            {
                SwitchingPlayerWeapons.ins.ResetOtherMeleeOrGrenadeHandsIfActivated();
            }

            RequiredComponents.GrenadeIcon.SetActive(true);

          

            if (PlayerManager.instance != null)
            {
                PlayerManager.instance.FindRequiredObjects();
            }
        }
        IEnumerator StartPinSound()
        {
            yield return new WaitForSeconds(Delays.PinSoundDelay);
            RequiredComponents.GrenadePinSound.Play();
        }
        IEnumerator GrenadeSpawn()
        {
            yield return new WaitForSeconds(0.01f);
            if (SelectGrenadeHands == false)
            {
                if (FirstTimeIter == false)
                {
                    for (int i = 0; i < PlayerManager.instance.PlayerWeaponScripts.Count; i++)
                    {
                        if (PlayerManager.instance.PlayerWeaponScripts[i].gameObject.activeInHierarchy == true && RequiredComponents.GrenadeAnimatorComponent.gameObject.activeInHierarchy == false)
                        {
                            // ResetAimedState(AllWeaponsShootingScripts[i]);
                            PlayerManager.instance.PlayerWeaponScripts[i].RequiredComponents.WeaponAnimatorComponent.Play(PlayerManager.instance.PlayerWeaponScripts[i].RemoveAnimationName, -1, 0f);
                            PreviousWeapon = PlayerManager.instance.PlayerWeaponScripts[i].gameObject; // Added so if previous weapon is greande hands we can reactivate it ( this is important because this fixed issue with AK47 activated and than if you
                                                // Selected insendiary grenade for instant throw than both Ak hands and grenade hands were activated at the same time . In this solution we make sure that if we
                                                // want to select grenade hands again after instant insendiary grenade throw than we reactivate it.( linked )
                        }
                    }
                    FirstTimeIter = true;
                }
                ButtonsFunction(UIInteractions.UIToDisableBeforeThrow, false);

                if(RequiredComponents.GrenadeAnimatorComponent.gameObject.activeInHierarchy == false)
                {
                    if (DelayFirstThrowsOnly == true && DonotDelay == false)
                    {
                        yield return new WaitForSeconds(Delays.GrenadeHandsActivationDelay);
                        DonotDelay = true;
                    }
                    else if (DelayFirstThrowsOnly == false)
                    {
                        yield return new WaitForSeconds(Delays.GrenadeHandsActivationDelay);
                    }
                }
              
                ButtonsFunction(UIInteractions.BeforeSelection, true);
                //MeshesDisable(false);
                //EnableParent(false);

                // ResetWeaponState();

                for (int i = 0; i < PlayerManager.instance.PlayerWeaponScripts.Count; i++)
                {
                    if (PlayerManager.instance.PlayerWeaponScripts[i].gameObject.activeInHierarchy == true && RequiredComponents.GrenadeAnimatorComponent.gameObject.activeInHierarchy == false)
                    {
                        if (PlayerManager.instance.PlayerWeaponScripts[i].RequiredComponents.WeaponAnimatorComponent.GetComponent<GameobjectActivationManager>() != null)
                        {
                            PlayerManager.instance.PlayerWeaponScripts[i].RequiredComponents.WeaponAnimatorComponent.GetComponent<GameobjectActivationManager>().enabled = false;
                        }
                        PlayerManager.instance.PlayerWeaponScripts[i].RequiredComponents.WeaponAnimatorComponent.enabled = false;
                        PlayerManager.instance.PlayerWeaponScripts[i].gameObject.SetActive(false);
                    }

                }

                RequiredComponents.GrenadeThrowSound.Play();
                RequiredComponents.GrenadeAnimatorComponent.gameObject.SetActive(true);
                //if (SwitchWeapons.instance != null)
                //{
                //    for (int x = 0; x < SwitchWeapons.instance.AllWeaponIcons.Length; x++)
                //    {
                //        if (SwitchWeapons.instance.AllWeaponIcons[x].activeInHierarchy == true)
                //        {
                //            PrevIcon = SwitchWeapons.instance.AllWeaponIcons[x].gameObject;

                //        }
                //        SwitchWeapons.instance.AllWeaponIcons[x].SetActive(false);
                //    }
                //}
              
                RequiredComponents.GrenadeIcon.SetActive(true);
                RequiredComponents.GrenadeAnimatorComponent.Play("GrenadeThrow", -1, 0f);
                if (PlayerManager.instance != null)
                {
                    PlayerManager.instance.FindRequiredObjects();
                }
            }
            else
            {
                RequiredComponents.GrenadeAnimatorComponent.Play("GrenadeThrow", -1, 0f);
            }
            if (RequiredComponents.GrenadePinSound != null)
            {
                StartCoroutine(StartPinSound());
            }
            yield return new WaitForSeconds(Delays.GrenadeSpawnDelay);
            CalculateForce();
            if (PoolSpawnType == true)
            {
                if (pooler != null)
                {
                    bomb = pooler.SpawnFromPool(NameInsideObjectPooler, RequiredComponents.GrenadeSpawnPoint.position, RequiredComponents.GrenadeSpawnPoint.rotation);

                    Rigidbody rb = bomb.GetComponent<Rigidbody>();
                    rb.AddForce(RequiredComponents.GrenadeSpawnPoint.forward * ThrowingPower, ForceMode.VelocityChange);
                }
            }
            else
            {
                bomb = Instantiate(RequiredComponents.GrenadePrefab, RequiredComponents.GrenadeSpawnPoint.position, RequiredComponents.GrenadeSpawnPoint.rotation);
                Rigidbody rb = bomb.GetComponent<Rigidbody>();
                rb.AddForce(RequiredComponents.GrenadeSpawnPoint.forward * ThrowingPower, ForceMode.VelocityChange);
            }
            //for (int x = 0; x < bomb.transform.childCount; x++)
            //{
            if (bomb.transform.root.GetComponent<ExplosiveDevice>() != null)
            {
                if (bomb.transform.root.GetComponent<ExplosiveDevice>().IsCollided == true)
                {
                    bomb.transform.root.GetComponent<ExplosiveDevice>().ExplodeFunctionality();
                }
                if (PoolSpawnType == false)
                {
                    bomb.transform.root.GetComponent<ExplosiveDevice>().CanDestroy = true;
                }
                else
                {
                    if (pooler != null)
                    {
                        bomb.transform.root.GetComponent<ExplosiveDevice>().CanDestroy = false;
                    }
                }
            }
            //}       
            yield return new WaitForSeconds(UIInteractions.UiReactivationDelay);
            // MeshesDisable(true);
            // EnableParent(true);
            ButtonsFunction(UIInteractions.UIToDisableBeforeThrow, true);
            RequiredComponents.GrenadeAnimatorComponent.Play("Wield", -1, 0f);
            if (KeepActive == false)
            {
                // MeshesDisable(true);
                //EnableParent(true);

                FirstTimeIter = false;
                RequiredComponents.GrenadeAnimatorComponent.Play("Wield", -1, 0f);
                //if (SwitchWeapons.instance != null)
                //{
                //    for (int x = 0; x < SwitchWeapons.instance.AllWeaponIcons.Length; x++)
                //    {
                //        SwitchWeapons.instance.AllWeaponIcons[x].SetActive(false);
                //        if (PrevIcon == SwitchWeapons.instance.AllWeaponIcons[x].gameObject)
                //        {
                //            SwitchWeapons.instance.AllWeaponIcons[x].gameObject.SetActive(true);
                //        }
                //    }
                //}
                RequiredComponents.GrenadeIcon.SetActive(false);
                for (int i = 0; i < PlayerManager.instance.PlayerWeaponScripts.Count; i++)
                {
                    if (PlayerManager.instance.PlayerWeaponScripts[i].RequiredComponents.WeaponAnimatorComponent.GetComponent<GameobjectActivationManager>() != null)
                    {
                        PlayerManager.instance.PlayerWeaponScripts[i].RequiredComponents.WeaponAnimatorComponent.GetComponent<GameobjectActivationManager>().enabled = true;
                    }
                    PlayerManager.instance.PlayerWeaponScripts[i].RequiredComponents.WeaponAnimatorComponent.GetComponent<Animator>().enabled = true;

                    if(PlayerManager.instance.PlayerWeaponScripts[i].RequiredComponents.WeaponAnimatorComponent.GetComponent<Animator>().gameObject != PlayerManager.instance.PlayerWeaponScripts[i].gameObject)
                    {
                        // this if statement is added because grenade model itself contain both the animator component and the player weapon script so we make sure to not to activate that if Ak47 or any other weapon is activated previously. 
                        
                        PlayerManager.instance.PlayerWeaponScripts[i].gameObject.SetActive(true);
                    }
                    else
                    {
                        if(PreviousWeapon != null) // Added so if previous weapon is greande hands we can reactivate it ( this is important because this fixed issue with AK47 activated and than if you
                                                   // Selected insendiary grenade for instant throw than both Ak hands and grenade hands were activated at the same time . In this solution we make sure that if we
                                                   // want to select grenade hands again after instant insendiary grenade throw than we reactivate it.( linked )
                        {
                            PreviousWeapon.SetActive(true);
                        }
                        
                    }
                    if (PlayerManager.instance.PlayerWeaponScripts[i].gameObject.activeInHierarchy == true)
                    {
                        PlayerManager.instance.PlayerWeaponScripts[i].RequiredComponents.WeaponAnimatorComponent.Rebind();
                    }
                }
                DonotDelay = false;
                RequiredComponents.GrenadeAnimatorComponent.gameObject.SetActive(false);
                if (PlayerManager.instance != null)
                {
                    PlayerManager.instance.FindRequiredObjects();
                }
            }
            //yield return new WaitForSeconds(Delays.AfterThrowIdleDelay);
            //RequiredComponents.GrenadeAnimatorComponent.Play("Idle", -1, 0f);
            StartSpawning = false;
        }
        public void DeactiveHand()
        {
            //  MeshesDisable(true);
            //EnableParent(true);

            //if (SwitchWeapons.instance != null)
            //{
            //    for (int x = 0; x < SwitchWeapons.instance.AllWeaponIcons.Length; x++)
            //    {
            //        SwitchWeapons.instance.AllWeaponIcons[x].SetActive(false);
            //        if (PrevIcon == SwitchWeapons.instance.AllWeaponIcons[x].gameObject)
            //        {
            //            SwitchWeapons.instance.AllWeaponIcons[x].gameObject.SetActive(true);
            //        }
            //    }
            //}

           
            ResetHands();

            //if (RequiredComponents.GrenadeAnimatorComponent.gameObject.GetComponent<PlayerWeapon>() != null && StopFirstThrow == false && RequiredComponents.GrenadeAnimatorComponent.gameObject.activeInHierarchy == false)
            //{
                RequiredComponents.GrenadeIcon.SetActive(false);
                RequiredComponents.GrenadeAnimatorComponent.Play("Wield", -1, 0f);
                ActivationFunction();


                RequiredComponents.GrenadeAnimatorComponent.gameObject.SetActive(false);

                if (PlayerManager.instance != null)
                {
                    PlayerManager.instance.FindRequiredObjects();
                }
            //}
           
        }
        public void ResetHands()
        {
            SelectGrenadeHands = false;
            FirstTimeIter = false;
            StartSpawning = false;
            DonotDelay = false;
        }
        public void DeactiveJoystick(bool Enable)
        {
            if (JoyStick.Instance != null)
            {
                JoyStick.Instance.enabled = Enable;
            }
        }
        public void ButtonsFunction(Image[] WhichImage, bool Enable)
        {
            for (int x = 0; x < WhichImage.Length; x++)
            {
                WhichImage[x].raycastTarget = Enable;
                if (Enable == true)
                {
                    WhichImage[x].color = Color.white;
                }
                else
                {
                    WhichImage[x].color = UIInteractions.UIDisabledColor;
                }
            }
        }
        public void ActivationFunction()
        {
            for (int i = 0; i < PlayerManager.instance.PlayerWeaponScripts.Count; i++)
            {
                PlayerManager.instance.PlayerWeaponScripts[i].RequiredComponents.WeaponAnimatorComponent.enabled = true;
                if (PlayerManager.instance.PlayerWeaponScripts[i].RequiredComponents.WeaponAnimatorComponent.GetComponent<Animator>().gameObject == PlayerManager.instance.PlayerWeaponScripts[i].gameObject)
                {
                    if (PreviousWeapon != null) // Added so if previous weapon is greande hands we can reactivate it ( this is important because this fixed issue with AK47 activated and than if you
                                                // Selected insendiary grenade for instant throw than both Ak hands and grenade hands were activated at the same time . In this solution we make sure that if we
                                                // want to select grenade hands again after instant insendiary grenade throw than we reactivate it.
                    {
                        PreviousWeapon.SetActive(true);
                    }
                }

                // Previous Code 
                //if (PlayerManager.instance.PlayerWeaponScripts[i].RequiredComponents.WeaponAnimatorComponent.GetComponent<Animator>().gameObject != PlayerManager.instance.PlayerWeaponScripts[i].gameObject)
                //{
                //    // this if statement is added because grenade model itself contain both the animator component and the player weapon script so we make sure to not to activate that if Ak47 or any other weapon is activated previously. 
                //    PlayerManager.instance.PlayerWeaponScripts[i].gameObject.SetActive(true);
                //}
                //else
                //{
                //    if (PreviousWeapon != null) // Added so if previous weapon is greande hands we can reactivate it ( this is important because this fixed issue with AK47 activated and than if you
                //                                // Selected insendiary grenade for instant throw than both Ak hands and grenade hands were activated at the same time . In this solution we make sure that if we
                //                                // want to select grenade hands again after instant insendiary grenade throw than we reactivate it.
                //    {
                //        PreviousWeapon.SetActive(true);
                //    }
                //}
            }
            ButtonsFunction(UIInteractions.BeforeSelection, true);
        }
        //public void ResetAimedState(Gunscript Weapon)
        //{
        //    if (Weapon.IsAimed == true)
        //    {
        //        Weapon.IsplayingAimedAnim = false;
        //        if (Weapon.WeaponPosition.SniperScopeScript != null)
        //        {
        //            Weapon.OutScope();
        //            if (PlayerManager.instance != null)
        //            {
        //                PlayerManager.instance.WeaponCrosshair.SetActive(true);
        //                PlayerManager.instance.IsScoping = false;
        //            }

        //            Weapon.WeaponPosition.SniperScopeScript.Sniperscope();

        //        }
        //        else
        //        {
        //            Weapon.OutScope();
        //            if (PlayerManager.instance != null)
        //            {
        //                PlayerManager.instance.IsScoping = false;
        //                PlayerManager.instance.WeaponCrosshair.SetActive(true);
        //            }
        //        }
        //    }
        //}

        // public void MeshesDisable(bool ShouldDisable)
        //{
        //    foreach (SkinnedMeshRenderer g in Gunscript.instance.WeaponPosition.ShootingPointParent.transform.GetComponentsInChildren<SkinnedMeshRenderer>())
        //    {
        //        if (g.gameObject.GetComponent<SkinnedMeshRenderer>() != null)
        //        {
        //            g.gameObject.GetComponent<SkinnedMeshRenderer>().enabled = ShouldDisable;
        //        }
        //    }
        //    foreach (MeshRenderer g in Gunscript.instance.WeaponPosition.ShootingPointParent.transform.GetComponentsInChildren<MeshRenderer>())
        //    {
        //        if (g.gameObject.GetComponent<MeshRenderer>() != null)
        //        {
        //            g.gameObject.GetComponent<MeshRenderer>().enabled = ShouldDisable;
        //        }
        //    }
        //}

        //public void EnableParent(bool Active)
        //{
        //    if (PlayerManager.instance != null)
        //    {
        //        if (PlayerManager.instance.ob != null)
        //        {
        //            if (StoreSelectedWeapon != null)
        //            {
        //                if (StoreSelectedWeapon.gameObject.activeInHierarchy == false && RequiredComponents.GrenadeAnimatorComponent.gameObject.activeInHierarchy == true)
        //                {
        //                    StoreSelectedWeapon.SetActive(Active);
        //                }
        //                else
        //                {
        //                    if (PlayerManager.instance.ob.WeaponPosition.ShootingPointParent != null)
        //                    {
        //                        PlayerManager.instance.ob.WeaponPosition.ShootingPointParent.SetActive(Active);
        //                        StoreSelectedWeapon = PlayerManager.instance.ob.WeaponPosition.ShootingPointParent.gameObject;
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                if(PlayerManager.instance.ob.WeaponPosition.ShootingPointParent != null)
        //                {
        //                    PlayerManager.instance.ob.WeaponPosition.ShootingPointParent.SetActive(Active);
        //                    StoreSelectedWeapon = PlayerManager.instance.ob.WeaponPosition.ShootingPointParent.gameObject;
        //                }
                      
        //            }

        //        }
        //    }
        //}

    }
}
