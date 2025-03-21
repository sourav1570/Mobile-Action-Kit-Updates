using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using System.Collections.Generic;

namespace MobileActionKit
{
    public class MeleeAttack : MonoBehaviour
    {
        public static MeleeAttack instance;
 
        [TextArea]
        public string ScriptInfo = "This script is responsible for player Melee attack actions.";
        [Space(10)]

        [Tooltip("Indicates the type of melee attack. 'Instant Attack' is the default.")]
        [ReadOnly]
        public string MeleeType = "Instant Attack";

        [System.Serializable]
        public class Components
        {
            [Tooltip("List of buttons in the UI that trigger melee attacks.")]
            public List<Button> MeleeUIButtons = new List<Button>();

            [Tooltip("Animator component used to play melee animations.")]
            public Animator MeleeAnimatorComponent;

            [Tooltip("Icon that represents melee functionality in the UI.")]
            public GameObject MeleeIcon;

            [Tooltip("Audio source used to play melee-related sounds.")]
            public AudioSource AudioSourceComponent;

            public PlayerWeapon PlayerWeaponScript;

        }

        [Tooltip("Container for all the required components for the melee attack system.")]
        public Components RequiredComponents;

        [System.Serializable]
        public class AttackAnim
        {
            [Tooltip("Scripts that handle melee damage for this attack.")]
            public MeleeDamage[] MeleeDamageScripts;

            [Tooltip("Animation clip to play for this melee attack.")]
            public AnimationClip AttackAnimationClip;

            [Tooltip("Audio clip to play during this melee attack.")]
            public AudioClip AttackAudioClip;

            [Tooltip("Delay before applying damage after the animation starts.")]
            public float DelayDamage = 0.2f;
        }

        [System.Serializable]
        public class Anims
        {
            [Tooltip("List of melee attack animations available for this system.")]
            public List<AttackAnim> AttackAnimations = new List<AttackAnim>();
        }

        [Tooltip("Container for all melee attack animations and their configurations.")]
        public Anims Animations;

        [System.Serializable]
        public class Delay
        {
            [Tooltip("Time to wait before activating the knife in a melee attack.")]
            public float KnifeActivationDelay = 2f;

            [Tooltip("Delay before starting an instant attack animation.")]
            public float InstantAttackAnimationDelay = 0f;
        }

        [Tooltip("Delays for different actions within the melee attack system.")]
        public Delay Delays;

        [System.Serializable]
        public class Interaction
        {
            [Tooltip("Color to apply to disabled UI elements (e.g., gray).")]
            public Color UIDisableColor;

            [Tooltip("UI images to disable while melee hands are activated (e.g., during weapon removal animation).")]
            public Image[] BeforeSelection;

            [Tooltip("UI images to disable while the melee attack animation is playing.")]
            public Image[] UIToDisableBeforeAttack;
        }

        [Tooltip("Handles interactions and UI changes during melee actions.")]
        public Interaction UIInteraction;

        bool KeepActive = true;
        bool DelayFirstAttacksOnly = true;
 

        [HideInInspector]
        public bool FirstTimeIter = false;

        [HideInInspector]
        public bool StopFirstThrow = false;

        GameObject PrevIcon;

        [HideInInspector]
        public bool DonotDelay = false;

        int animtoplayattack;

        GameObject StoreSelectedWeapon;

        [HideInInspector]
        public bool Directattack = false;

        GameObject PrevWeapon;

        void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }

            if (RequiredComponents.MeleeAnimatorComponent.gameObject.GetComponent<PlayerWeapon>() != null)
            {
                KeepActive = true;
                DelayFirstAttacksOnly = true;
            }
            else
            {
                KeepActive = false;
                DelayFirstAttacksOnly = false;
            }
        }
        private void Start()
        {
            if (RequiredComponents.MeleeUIButtons != null)
            {
                for (int x = 0; x < RequiredComponents.MeleeUIButtons.Count; x++)
                {
                    RequiredComponents.MeleeUIButtons[x].onClick.AddListener(MeleeAttackFunction);
                }
            }
            if (RequiredComponents.PlayerWeaponScript != null)
            {
                RequiredComponents.PlayerWeaponScript.NotAShootingWeapon = true;
            }
        }
        public void MeleeAttackFunction()
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

            ButtonsFunction(UIInteraction.BeforeSelection, false);

            if (RequiredComponents.MeleeAnimatorComponent.gameObject.GetComponent<PlayerWeapon>() != null && StopFirstThrow == false && RequiredComponents.MeleeAnimatorComponent.gameObject.activeInHierarchy == false)
            {
                StartCoroutine(ActivateHands());
                StopFirstThrow = true;
            }
            else
            {
                if (Directattack == false)
                {
                    StartCoroutine(Attack());
                    Directattack = true;
                }
            }
        }
        IEnumerator ActivateHands()
        {
            yield return new WaitForSeconds(0.01f);
            if (FirstTimeIter == false)
            {
                for (int i = 0; i < PlayerManager.instance.PlayerWeaponScripts.Count; i++)
                {
                    if (PlayerManager.instance.PlayerWeaponScripts[i].gameObject.activeInHierarchy == true && RequiredComponents.MeleeAnimatorComponent.gameObject.activeInHierarchy == false)
                    {
                        ResetAimedState(PlayerManager.instance.PlayerWeaponScripts[i]);
                        PlayerManager.instance.PlayerWeaponScripts[i].RequiredComponents.WeaponAnimatorComponent.Play(PlayerManager.instance.PlayerWeaponScripts[i].RemoveAnimationName, -1, 0f);
                        
                    }
                }
                FirstTimeIter = true;
            }

            yield return new WaitForSeconds(Delays.KnifeActivationDelay);
             
            ButtonsFunction(UIInteraction.BeforeSelection, true);

            //for (int i = 0; i < RequiredComponents.AllShootingScripts.Length; i++)
            //{
            //    if (RequiredComponents.AllShootingScripts[i].gameObject.activeInHierarchy == true)
            //    {
            //        RequiredComponents.AllShootingScripts[i].Components.WeaponAnimatorComponent.enabled = false;
            //        RequiredComponents.AllShootingScripts[i].gameObject.SetActive(false);
            //    }

            //}
            for (int i = 0; i < PlayerManager.instance.PlayerWeaponScripts.Count; i++)
            {
                if (PlayerManager.instance.PlayerWeaponScripts[i].gameObject.activeInHierarchy == true && RequiredComponents.MeleeAnimatorComponent.gameObject.activeInHierarchy == false)
                {
                    PlayerManager.instance.PlayerWeaponScripts[i].RequiredComponents.WeaponAnimatorComponent.gameObject.SetActive(false);
                }
            }
            //EnableParent(false);
            RequiredComponents.MeleeAnimatorComponent.gameObject.SetActive(true);
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

            RequiredComponents.MeleeIcon.SetActive(true);
            if (PlayerManager.instance != null)
            {
                PlayerManager.instance.FindRequiredObjects();
            }
        }
        IEnumerator ActivateMelee(MeleeDamage[] MeleeDamageScripts,float DelayDamage)
        {
            yield return new WaitForSeconds(DelayDamage);
            for (int x = 0; x < MeleeDamageScripts.Length; x++)
            {
                MeleeDamageScripts[x].gameObject.SetActive(true);
                MeleeDamageScripts[x].ActivateNow();
            }
        }
        IEnumerator Attack()
        {
            yield return new WaitForSeconds(0.01f);
          //  RequiredComponents.MeleeAnimatorComponent.SetBool("idle", false);
           
            if (StopFirstThrow == false)
            {
                if (FirstTimeIter == false)
                {
                    for (int i = 0; i < PlayerManager.instance.PlayerWeaponScripts.Count; i++)
                    {
                        if (PlayerManager.instance.PlayerWeaponScripts[i].gameObject.activeInHierarchy == true && RequiredComponents.MeleeAnimatorComponent.gameObject.activeInHierarchy == false)
                        {
                            //ResetAimedState(PlayerManager.instance.AllWeaponsPlayerWeaponScript[i]);
                            PlayerManager.instance.PlayerWeaponScripts[i].RequiredComponents.WeaponAnimatorComponent.Play(PlayerManager.instance.PlayerWeaponScripts[i].RemoveAnimationName, -1, 0f);
                            PrevWeapon = PlayerManager.instance.PlayerWeaponScripts[i].RequiredComponents.WeaponAnimatorComponent.gameObject;
                        }
                    }
                   
                    FirstTimeIter = true;
                }
                ButtonsFunction(UIInteraction.UIToDisableBeforeAttack, false);
                if (RequiredComponents.MeleeAnimatorComponent.gameObject.activeInHierarchy == false)
                {
                    if (DelayFirstAttacksOnly == true && DonotDelay == false)
                    {
                        yield return new WaitForSeconds(Delays.InstantAttackAnimationDelay);
                        DonotDelay = true;
                    }
                    else if (DelayFirstAttacksOnly == false)
                    {
                        yield return new WaitForSeconds(Delays.InstantAttackAnimationDelay);
                    }
                }

                ButtonsFunction(UIInteraction.BeforeSelection, true);
                for (int i = 0; i < PlayerManager.instance.PlayerWeaponScripts.Count; i++)
                {
                    if (PlayerManager.instance.PlayerWeaponScripts[i].gameObject.activeInHierarchy == true && RequiredComponents.MeleeAnimatorComponent.gameObject.activeInHierarchy == false)
                    {
                        PlayerManager.instance.PlayerWeaponScripts[i].RequiredComponents.WeaponAnimatorComponent.enabled = false;
                        PlayerManager.instance.PlayerWeaponScripts[i].gameObject.SetActive(false);
                    }
                }
                //EnableParent(false);
                if (PrevWeapon != null)
                {
                    PrevWeapon.gameObject.SetActive(false);
                }

                RequiredComponents.MeleeAnimatorComponent.gameObject.SetActive(true);
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
                RequiredComponents.MeleeIcon.SetActive(true);

                //animtoplayattack = Random.Range(1, AttackAnimation.Count + 1);
                //MeleeAnimator.SetInteger("Attack",animtoplayattack);

                //if(animtoplayattack == 2)
                //{
                //    animtoplayattack = 1;
                //}
                //AttackAnimation[animtoplayattack].SoundToPlay.Play();  


                animtoplayattack = Random.Range(0, Animations.AttackAnimations.Count);
                RequiredComponents.MeleeAnimatorComponent.SetInteger("Attack", animtoplayattack);
                RequiredComponents.MeleeAnimatorComponent.Play(Animations.AttackAnimations[animtoplayattack].AttackAnimationClip.name, -1, 0f);
                RequiredComponents.AudioSourceComponent.clip = Animations.AttackAnimations[animtoplayattack].AttackAudioClip;
                RequiredComponents.AudioSourceComponent.Play();

                StartCoroutine(ActivateMelee(Animations.AttackAnimations[animtoplayattack].MeleeDamageScripts, Animations.AttackAnimations[animtoplayattack].DelayDamage));

                if (PlayerManager.instance != null)
                {
                    PlayerManager.instance.FindRequiredObjects();
                }
            }
            else
            {
                animtoplayattack = Random.Range(0, Animations.AttackAnimations.Count);
                RequiredComponents.MeleeAnimatorComponent.SetInteger("Attack", animtoplayattack);
                RequiredComponents.AudioSourceComponent.clip = Animations.AttackAnimations[animtoplayattack].AttackAudioClip;
                RequiredComponents.AudioSourceComponent.Play();

                StartCoroutine(ActivateMelee(Animations.AttackAnimations[animtoplayattack].MeleeDamageScripts, Animations.AttackAnimations[animtoplayattack].DelayDamage));

                if (animtoplayattack == 2)
                {
                    animtoplayattack = 1;
                }
                if (PlayerManager.instance != null)
                {
                    PlayerManager.instance.FindRequiredObjects();
                }
            }

            yield return new WaitForSeconds(Animations.AttackAnimations[animtoplayattack].AttackAnimationClip.length);
            if (KeepActive == true)
            {
                RequiredComponents.MeleeAnimatorComponent.SetInteger("Attack", 999);
            }
            if (PrevWeapon != null)
            {
                PrevWeapon.gameObject.SetActive(true);
            }
            //if (EnableMeleeSelection == true && KeepActive == true)
            //{
            //    EnableParent(false);
            //}
            //else
            //{
            //    EnableParent(true);
            //}

            ButtonsFunction(UIInteraction.UIToDisableBeforeAttack, true);
            if (KeepActive == false)
            {
                FirstTimeIter = false;
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
                RequiredComponents.MeleeIcon.SetActive(false);
                for (int i = 0; i < PlayerManager.instance.PlayerWeaponScripts.Count; i++)
                {
                    PlayerManager.instance.PlayerWeaponScripts[i].RequiredComponents.WeaponAnimatorComponent.enabled = true;
                    PlayerManager.instance.PlayerWeaponScripts[i].gameObject.SetActive(true);
                    if (PlayerManager.instance.PlayerWeaponScripts[i].gameObject.activeInHierarchy == true)
                    {
                        PlayerManager.instance.PlayerWeaponScripts[i].RequiredComponents.WeaponAnimatorComponent.Rebind();
                    }
                }
                DonotDelay = false;
                RequiredComponents.MeleeAnimatorComponent.gameObject.SetActive(false);

                if (PlayerManager.instance != null)
                {
                    PlayerManager.instance.FindRequiredObjects();
                }


            }

            Directattack = false;
           // RequiredComponents.MeleeAnimatorComponent.SetBool("idle", true);
        }
        public void DeactiveHand()
        {
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
            RequiredComponents.MeleeIcon.SetActive(false);
            ActivationFunction();
            //for (int i = 0; i < AllWeaponsShootingScripts.Length; i++)
            //{
            //    AllWeaponsShootingScripts[i].Components.WeaponIdScript.transform.GetChild(0).gameObject.GetComponent<Animator>().enabled = true;
            //    AllWeaponsShootingScripts[i].gameObject.SetActive(true);
            //}
            //for (int i = 0; i < AllWeaponsShootingScripts.Length; i++)
            //{
            //    if (AllWeaponsShootingScripts[i].gameObject.activeInHierarchy == true)
            //    {
            //        AllWeaponsShootingScripts[i].AnimationsNames.WeaponAnimatorComponent.Rebind();
            //    }
            //}
            //for (int x = 0; x < ButtonsToDisableOnMelee.Length; x++)
            //{
            //    ButtonsToDisableOnMelee[x].interactable = true;
            //}
            //for (int x = 0; x < RaycastImages.Length; x++)
            //{
            //    RaycastImages[x].raycastTarget = true;
            //}
            //if (DeactivateJoystick == true)
            //{
            //    if (JoyStick.Instance != null)
            //    {
            //        JoyStick.Instance.enabled = true;
            //    }
            //}

            ResetHands();

            RequiredComponents.MeleeAnimatorComponent.gameObject.SetActive(false);
           
            if (PlayerManager.instance != null)
            {
                PlayerManager.instance.FindRequiredObjects();
            }
        }
        public void ResetHands()
        {
            StopFirstThrow = false;
            FirstTimeIter = false;
            DonotDelay = false;
            Directattack = false;
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
                    WhichImage[x].color = UIInteraction.UIDisableColor;
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
                    if (PrevWeapon != null) // Added so if previous weapon is melee hands we can reactivate it ( this is important because this fixed issue with AK47 activated and than if you
                                                // Selected insendiary grenade for instant throw than both Ak hands and grenade hands were activated at the same time . In this solution we make sure that if we
                                                // want to select grenade hands again after instant insendiary grenade throw than we reactivate it.
                    {
                        PrevWeapon.SetActive(true);
                    }
                }
            }
           
            // Previous Code
            //for (int i = 0; i < PlayerManager.instance.PlayerWeaponScripts.Count; i++)
            //{
            //    PlayerManager.instance.PlayerWeaponScripts[i].RequiredComponents.WeaponAnimatorComponent.enabled = true;
            //    PlayerManager.instance.PlayerWeaponScripts[i].gameObject.SetActive(true);
            //}
            ButtonsFunction(UIInteraction.BeforeSelection, true);
        }
        public void ResetAimedState(PlayerWeapon Weapon)
        {
            if (Weapon.IsAimed == true)
            {
                Weapon.IsplayingAimedAnim = false;
                if (Weapon.ShootingFeatures.SniperScopeScript != null && Weapon.ShootingFeatures.UseSniperScopeUI == true)
                {
                    Weapon.OutScope();
                    if (PlayerManager.instance != null)
                    {
                        PlayerManager.instance.CurrentHoldingPlayerWeapon.ShootingFeatures.WeaponCrosshair.SetActive(true);
                        PlayerManager.instance.IsScoping = false;
                    }

                    Weapon.ShootingFeatures.SniperScopeScript.Sniperscope();

                }
                else
                {
                    Weapon.OutScope();
                    if (PlayerManager.instance != null)
                    {
                        PlayerManager.instance.IsScoping = false;
                        PlayerManager.instance.CurrentHoldingPlayerWeapon.ShootingFeatures.WeaponCrosshair.SetActive(true);
                    }
                }
            }
        }
        //public void MeshesDisable(bool ShouldDisable)
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
        //                if (StoreSelectedWeapon.gameObject.activeInHierarchy == false && RequiredComponents.MeleeAnimatorComponent.gameObject.activeInHierarchy == true)
        //                {
        //                    StoreSelectedWeapon.SetActive(Active);
        //                }
        //                else
        //                {
        //                    if(PlayerManager.instance.ob.WeaponPosition.ShootingPointParent != null)
        //                    {
        //                        PlayerManager.instance.ob.WeaponPosition.ShootingPointParent.SetActive(Active);
        //                        StoreSelectedWeapon = PlayerManager.instance.ob.WeaponPosition.ShootingPointParent.gameObject;
        //                    }
                           
        //                }
        //            }
        //            else
        //            {
        //                if (PlayerManager.instance.ob.WeaponPosition.ShootingPointParent != null)
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