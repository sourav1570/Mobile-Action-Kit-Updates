using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;
using UnityEngine.UI;

namespace MobileActionKit
{
    public class HumanoidBodyHits : MonoBehaviour
    {
        [HideInInspector]
        public string DebugHitState;
        [HideInInspector]
        public string AnimationResetedNames;

        [TextArea]
        [Tooltip("This script will play impact animation clips included in animation tree in case Ai will be hit by a bullet. " +
            "Humanoid Ai agent animator has two layers of hit impacts animations." +
            "One for upper body(Torso), in case bullet hits this Ai above the waist line. " +
            "And another for the lower body (Legs) i.e below waist line")]
        public string ScriptInfo = "This script will play impact animation clips included in animation tree in case Ai will be hit by a bullet. " +
            "Humanoid Ai agent animator has two layers of hit impacts animations." +
            "One for upper body(Torso), in case bullet hits this Ai above the waist line. " +
            "And another for the lower body (Legs) i.e below waist line";

        [Space(10)]
        [Tooltip("Drag and drop into this field 'Animator' component located above in the inspector")]
        public Animator AnimatorComponent;
        [Tooltip("Drag and drop into this field 'Humanoid Ai Health' component located above in the inspector")]
        public HumanoidAiHealth HumanoidAiHealthScript;
        float BotPreviousSpeed;
        NavMeshAgent agent;

        float DebugAnimationTime = 2f;

        [Tooltip("Minimum time to wait until the next animation.")]
        public float MinTimeToWaitUntilNextAnimation = 5f;
        [Tooltip("Maximum time to wait until the next animation.")]
        public float MaxTimeToWaitUntilNextAnimation = 8f;

        [Tooltip("Enable to play combined upper and lower body hit animations.")]
        public bool PlayCombinedBodyHitAnimations = false;

        [System.Serializable]
        public class LowerBodyHitClass
        {
            [Range(1, 3)][Tooltip("Specify number of animations to add for this humanoid AI agent.")]
            public int LowerBodyAnimationsToPlay = 1;

            [Tooltip("Put the number of animation clips for AI agent to play when being hit by a bullet. Copy and paste names of those clips into the fields." +
               "To introduce visual variety and to avoid repetitive animations this list can have any amount of animation clips. " +
               "Each time Ai agent is hit by a bullet random animation from this list will be played. ")]
            public List<LowerBodyAnimationsNameClass> LowerBodyAnimationNames = new List<LowerBodyAnimationsNameClass>();

            [Tooltip("Drag and drop all the bones from lower body into this list. " +
             "As long as all intented bones are in this list the script will register all the hits to those colliders on those bones and will play respective animation clips.")]
            public Collider[] LowerBodyColliders;

            [HideInInspector]
            public int PreviousLowerBodyListCount;
        }

        [System.Serializable]
        public class UpperBodyHitClass
        {
            [Tooltip("Drag and drop all the bones from upper body into this list. " +
             "As long as all intented bones are in this list the script will register all the hits to those colliders on those bones and will play respective animation clips.")]
            public Collider[] UpperBodyColliders;

            [Range(1, 3)][Tooltip("Specify number of animations to add for this humanoid AI agent.")]
            public int UpperBodyAnimationsToPlay = 1;

            [Tooltip("Put the number of animation clips for AI agent to play when being hit by a bullet. Copy and paste names of those clips into the fields." +
                 "To introduce visual variety and to avoid repetitive animations this list can have any amount of animation clips. " +
                 "Each time Ai agent is hit by a bullet random animation from this list will be played. ")]
            public List<UpperBodyAnimationsNamesClass> UpperBodyAnimationNames = new List<UpperBodyAnimationsNamesClass>();

            [HideInInspector]
            public int PreviousUpperBodyListCount;
        }


        [Tooltip("Configuration for lower body hit animations.")]
        public LowerBodyHitClass LowerBodyHit;

        [Tooltip("Configuration for upper body hit animations.")]
        public UpperBodyHitClass UpperBodyHit;

        //[Tooltip("This field is required by this Humanoid Ai animation tree." +
        //    "Name of animation state  that you write in this field should match the name of the default animation state inside animator." +
        //    "Default animation state should be always empty and not have any animation clip in it.")]
        //public string DefaultUpperBodyAnimationName;
        //[Tooltip("This field is required by this Humanoid Ai animation tree." +
        //   "Name of animation state  that you write in this field should match the name of the default animation state inside animator." +
        //   "Default animation state should be always empty and not have any animation clip in it.")]
        //public string DefaultLowerBodyAnimationName;



        [System.Serializable]
        public class LowerBodyAnimationsNameClass
        {
            public ChoosenLowerBodyAnimations AnimationName;
            public AnimationClip AnimationClip;
            public float AnimationSpeed = 1;
        }

        [System.Serializable]
        public enum ChoosenLowerBodyAnimations
        {
            LowerBodyHitAnimation1,
            LowerBodyHitAnimation2,
            LowerBodyHitAnimation3
        }



        [System.Serializable]
        public class UpperBodyAnimationsNamesClass
        {
            public ChoosenUpperBodyAnimations AnimationName;
            public AnimationClip AnimationClip;
            public float AnimationSpeed = 1;

        }

        [System.Serializable]
        public enum ChoosenUpperBodyAnimations
        {
            UpperBodyHitAnimation1,
            UpperBodyHitAnimation2,
            UpperBodyHitAnimation3
        }

        bool IsAnimationStarted = false;
        // string[] clipNameFromLayer0;
        // string[] clipNameFromLayer1; 

        RuntimeAnimatorController ac;
        string UpperBodyAnimationNamePlaying;
        string LowerBodyAnimationNamePlaying;

        CoreAiBehaviour CoreAiBehaviourScript;

        bool CanPlayBodyHitAnimation = false;

        string GetAnimationParameterName;
        int Randomise;

        bool IsReallyShot = false;

        int PreviousLowerBodyAnimation;
        int PreviousUpperBodyAnimation;

        private void OnValidate()
        {
            if (LowerBodyHit.LowerBodyAnimationsToPlay != LowerBodyHit.PreviousLowerBodyListCount)
            {
                if (LowerBodyHit.LowerBodyAnimationsToPlay > LowerBodyHit.PreviousLowerBodyListCount)
                {
                    // Increase count: Preserve existing items and add new ones
                    int itemsToAdd = LowerBodyHit.LowerBodyAnimationsToPlay - LowerBodyHit.PreviousLowerBodyListCount;
                    for (int i = 0; i < itemsToAdd; i++)
                    {
                        LowerBodyHit.LowerBodyAnimationNames.Add(null);
                    }
                }
                else if (LowerBodyHit.LowerBodyAnimationsToPlay < LowerBodyHit.PreviousLowerBodyListCount && LowerBodyHit.LowerBodyAnimationsToPlay > 0)
                {
                    // Decrease count while preserving values and keeping it above 0
                    AdjustListSize();
                }

                LowerBodyHit.PreviousLowerBodyListCount = LowerBodyHit.LowerBodyAnimationsToPlay;
            }


            if (UpperBodyHit.UpperBodyAnimationsToPlay != UpperBodyHit.PreviousUpperBodyListCount)
            {
                if (UpperBodyHit.UpperBodyAnimationsToPlay > UpperBodyHit.PreviousUpperBodyListCount)
                {
                    // Increase count: Preserve existing items and add new ones
                    int itemsToAdd = UpperBodyHit.UpperBodyAnimationsToPlay - UpperBodyHit.PreviousUpperBodyListCount;
                    for (int i = 0; i < itemsToAdd; i++)
                    {
                        UpperBodyHit.UpperBodyAnimationNames.Add(null);
                    }
                }
                else if (UpperBodyHit.UpperBodyAnimationsToPlay < UpperBodyHit.PreviousUpperBodyListCount && UpperBodyHit.UpperBodyAnimationsToPlay > 0)
                {
                    // Decrease count while preserving values and keeping it above 0
                    AdjustListSize();
                }

                UpperBodyHit.PreviousUpperBodyListCount = UpperBodyHit.UpperBodyAnimationsToPlay;
            }


        }

        private void AdjustListSize()
        {
            LowerBodyHit.LowerBodyAnimationNames.RemoveRange(LowerBodyHit.LowerBodyAnimationsToPlay, LowerBodyHit.LowerBodyAnimationNames.Count - LowerBodyHit.LowerBodyAnimationsToPlay);
            UpperBodyHit.UpperBodyAnimationNames.RemoveRange(UpperBodyHit.UpperBodyAnimationsToPlay, UpperBodyHit.UpperBodyAnimationNames.Count - UpperBodyHit.UpperBodyAnimationsToPlay);
        }

        void Start()
        {
            PreviousLowerBodyAnimation = 1000;
            PreviousUpperBodyAnimation = 1000;
            agent = transform.root.GetComponent<UnityEngine.AI.NavMeshAgent>();

            ac = AnimatorComponent.runtimeAnimatorController;

            if (GetComponent<CoreAiBehaviour>() != null)
            {
                CoreAiBehaviourScript = GetComponent<CoreAiBehaviour>();
            }

            //int w = AnimatorComponent.GetCurrentAnimatorClipInfo(0).Length;
            //clipNameFromLayer0 = new string[w];
            //for (int i = 0; i < w; i += 1)
            //{
            //    clipNameFromLayer0[i] = AnimatorComponent.GetCurrentAnimatorClipInfo(0)[i].clip.name;
            //}

            //int z = AnimatorComponent.GetCurrentAnimatorClipInfo(1).Length;
            //clipNameFromLayer1 = new string[z];
            //for (int i = 0; i < z; i += 1)
            //{
            //    clipNameFromLayer1[i] = AnimatorComponent.GetCurrentAnimatorClipInfo(1)[i].clip.name;
            //}
        }
        IEnumerator MuteHitImpactAnimation()
        {
            yield return new WaitForSeconds(DebugAnimationTime);
            if (HumanoidAiHealthScript.IsDied == false)
            {

                AnimatorComponent.SetBool(UpperBodyAnimationNamePlaying, false);
                AnimatorComponent.SetBool(LowerBodyAnimationNamePlaying, false);
                CoreAiBehaviourScript.IsPlayingBodyHitAnimation = false;
                //for (int x = 0; x < UpperBodyColliders.Length; x++)
                //{
                //    if (UpperBodyColliders[x].transform.name == HumanoidAiHealthScript.collidername)
                //    {
                //        AnimatorComponent.Play(DefaultUpperBodyAnimationName, -1, 0f);
                //    }
                //}
                //for (int x = 0; x < LowerBodyColliders.Length; x++)
                //{
                //    if (LowerBodyColliders[x].transform.name == HumanoidAiHealthScript.collidername)
                //    {
                //        AnimatorComponent.Play(DefaultLowerBodyAnimationName, -1, 0f);
                //    }
                //}

                if (CoreAiBehaviourScript != null)
                {

                    if (CoreAiBehaviourScript.AgentRole != CoreAiBehaviour.Role.Zombie)
                    {
                        CoreAiBehaviourScript.Components.HumanoidFiringBehaviourComponent.StopShoot = false;

                    }

                }
                if (PlayCombinedBodyHitAnimations == true)
                {
                    if (agent.enabled == true)
                    {
                        agent.isStopped = true;
                    }
                    agent.speed = BotPreviousSpeed;
                    CoreAiBehaviourScript.StopSpineRotation = false;
                }
                //CoreAiBehaviourScript.RotateSpine.StoreLastSpineRotations();
                //CoreAiBehaviourScript.AnimatorLayerWeightControllerScript.ChangeLayerWeight(6, 0);
                //CoreAiBehaviourScript.AnimatorLayerWeightControllerScript.ChangeLayerWeight(7, 0);
                //agent.isStopped = false;
                //agent.speed = BotPreviousSpeed;

                //  CoreAiBehaviourScript.enableIkupperbodyRotations(ref CoreAiBehaviourScript.ActivateNoIk);
                //  CoreAiBehaviourScript.StopSpineRotation = false;
                float Randomise = Random.Range(MinTimeToWaitUntilNextAnimation, MaxTimeToWaitUntilNextAnimation);
                HumanoidAiHealthScript.CompleteFirstHitAnimation = false;
                yield return new WaitForSeconds(Randomise);
                CanPlayBodyHitAnimation = false;
                DebugHitState = "Animation Resetted";
                if(CoreAiBehaviourScript.IsAnyTaskCurrentlyRunning == false)
                {
                    CoreAiBehaviourScript.IsTaskOver = true;
                }
                AnimationResetedNames = UpperBodyAnimationNamePlaying;
            }
        }
        public void PlayHitImpactAnimations()
        {
            if (HumanoidAiHealthScript.IsDied == false && CoreAiBehaviourScript.CombatStateBehaviours.UseImpactAnimations == true )
            {
                if (CanPlayBodyHitAnimation == false && CoreAiBehaviourScript.ThrowingGrenade == false && CoreAiBehaviourScript.BotMovingAwayFromGrenade == false)
                {
                    DebugHitState = "Playing Hit Animation";
                    IsAnimationStarted = false;

                    for (int x = 0; x < UpperBodyHit.UpperBodyColliders.Length; x++)
                    {
                        if (UpperBodyHit.UpperBodyColliders[x].transform.name == HumanoidAiHealthScript.collidername)
                        {
                            if (CoreAiBehaviourScript != null)
                            {
                                if(CoreAiBehaviourScript.AgentRole != CoreAiBehaviour.Role.Zombie)
                                {
                                    CoreAiBehaviourScript.Components.HumanoidFiringBehaviourComponent.StopShoot = true;

                                }

                            }
                           
                            Randomise = Random.Range(0, UpperBodyHit.UpperBodyAnimationNames.Count);
                         
                            UpperBodyAnimationNamePlaying = UpperBodyHit.UpperBodyAnimationNames[Randomise].AnimationName.ToString();

                            if (Randomise == PreviousUpperBodyAnimation && IsAnimationPlaying(AnimatorComponent, UpperBodyAnimationNamePlaying))
                            {
                                AnimatorComponent.Play(UpperBodyAnimationNamePlaying, -1, 0f);
                            }
                            PreviousUpperBodyAnimation = Randomise;

                            DebugAnimationTime = UpperBodyHit.UpperBodyAnimationNames[Randomise].AnimationClip.length;
                            AnimatorComponent.SetFloat("UpperBodyHitAnimationSpeed", UpperBodyHit.UpperBodyAnimationNames[Randomise].AnimationSpeed);
                            DebugAnimationTime = DebugAnimationTime / UpperBodyHit.UpperBodyAnimationNames[Randomise].AnimationSpeed;
                            // CoreAiBehaviourScript.AnimatorLayerWeightControllerScript.ChangeLayerWeight(6, 1,false);
                            //   CoreAiBehaviourScript.AnimatorLayerWeightControllerScript.ChangeLayerWeight(7, 0);
                            IsAnimationStarted = true;
                            AnimatorComponent.SetBool(UpperBodyAnimationNamePlaying, true);
                            IsReallyShot = true;
                            StopBot();
                        }

                    }

                    if (IsAnimationStarted == false)
                    {
                        for (int x = 0; x < LowerBodyHit.LowerBodyColliders.Length; x++)
                        {
                            if (LowerBodyHit.LowerBodyColliders[x].transform.name == HumanoidAiHealthScript.collidername)
                            {
                                if (CoreAiBehaviourScript != null)
                                {
                                    if (CoreAiBehaviourScript.AgentRole != CoreAiBehaviour.Role.Zombie)
                                    {
                                        CoreAiBehaviourScript.Components.HumanoidFiringBehaviourComponent.StopShoot = true;
                                    }
                                }
                                Randomise = Random.Range(0, LowerBodyHit.LowerBodyAnimationNames.Count);
                              
                                LowerBodyAnimationNamePlaying = LowerBodyHit.LowerBodyAnimationNames[Randomise].AnimationName.ToString();

                                if (Randomise == PreviousLowerBodyAnimation && IsAnimationPlaying(AnimatorComponent, LowerBodyAnimationNamePlaying))
                                {
                                    AnimatorComponent.Play(LowerBodyAnimationNamePlaying, -1, 0f);
                                }
                                PreviousLowerBodyAnimation = Randomise;

                                DebugAnimationTime = LowerBodyHit.LowerBodyAnimationNames[Randomise].AnimationClip.length;
                                AnimatorComponent.SetFloat("LowerBodyHitAnimationSpeed", LowerBodyHit.LowerBodyAnimationNames[Randomise].AnimationSpeed);
                                DebugAnimationTime = DebugAnimationTime / LowerBodyHit.LowerBodyAnimationNames[Randomise].AnimationSpeed;
                                // CoreAiBehaviourScript.AnimatorLayerWeightControllerScript.ChangeLayerWeight(7, 1,false);
                                //CoreAiBehaviourScript.AnimatorLayerWeightControllerScript.ChangeLayerWeight(6, 0);
                                IsAnimationStarted = true;
                                AnimatorComponent.SetBool(LowerBodyAnimationNamePlaying, true);
                                IsReallyShot = true;
                                StopBot();

                                //Newly added because when we hit the lower body and the agent is moving we can stop him.otherwise it may look odd 
                                BotPreviousSpeed = agent.speed;
                                if (agent.enabled == true)
                                {
                                    agent.isStopped = true;
                                }
                                agent.speed = 0f;
                            }

                        }
                    }

                    if (PlayCombinedBodyHitAnimations == true && IsReallyShot == true)
                    {
                        UpperBodyAnimationNamePlaying = UpperBodyHit.UpperBodyAnimationNames[Randomise].AnimationName.ToString();
                        DebugAnimationTime = UpperBodyHit.UpperBodyAnimationNames[Randomise].AnimationClip.length;
                        AnimatorComponent.SetFloat("UpperBodyHitAnimationSpeed", UpperBodyHit.UpperBodyAnimationNames[Randomise].AnimationSpeed);
                        DebugAnimationTime = DebugAnimationTime / UpperBodyHit.UpperBodyAnimationNames[Randomise].AnimationSpeed;
                        AnimatorComponent.SetBool(UpperBodyAnimationNamePlaying, true);


                        if (Randomise == PreviousLowerBodyAnimation && IsAnimationPlaying(AnimatorComponent, LowerBodyAnimationNamePlaying))
                        {
                            AnimatorComponent.Play(LowerBodyAnimationNamePlaying, -1, 0f);
                        }
                        PreviousLowerBodyAnimation = Randomise;

                        LowerBodyAnimationNamePlaying = LowerBodyHit.LowerBodyAnimationNames[Randomise].AnimationName.ToString();
                        AnimatorComponent.SetFloat("LowerBodyHitAnimationSpeed", LowerBodyHit.LowerBodyAnimationNames[Randomise].AnimationSpeed);
                        AnimatorComponent.SetBool(LowerBodyAnimationNamePlaying, true);

                        BotPreviousSpeed = agent.speed;
                        if (agent.enabled == true)
                        {
                            agent.isStopped = true;
                        }
                        agent.speed = 0f;


                        CoreAiBehaviourScript.StopSpineRotation = true;
                        IsReallyShot = false;

                    }
                    //Get Animator controller
                    //  ShotLength = WeaponAnimatorComponent.runtimeAnimatorController.animationClips[0].length;
                    //for (int i = 0; i < ac.animationClips.Length; i++)                 //For all animations
                    //{                
                    //    if (ac.animationClips[i].name == AnimationNamePlaying)        //If it has the same name as your clip
                    //    {                  
                    //        DebugAnimationTime = ac.animationClips[i].length;
                    //    }
                    //}



                }
            }
        }
        public void StopBot()
        {
            CoreAiBehaviourScript.IsPlayingBodyHitAnimation = true;
            CoreAiBehaviourScript.SetAnimationForFullBody("AnimationNamePlaying");
            CoreAiBehaviourScript.SetAnimationForUpperBody("AnimationNamePlaying");
            DebugHitState = "Coroutine Started";
            StartCoroutine(MuteHitImpactAnimation());
            CanPlayBodyHitAnimation = true;
            HumanoidAiHealthScript.CompleteFirstHitAnimation = true;
        }
        bool IsAnimationPlaying(Animator anim, string animationName)
        {
            AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);  // 0 is the layer index, use the appropriate layer index if needed
            return stateInfo.IsName(animationName);
        }
    }
}