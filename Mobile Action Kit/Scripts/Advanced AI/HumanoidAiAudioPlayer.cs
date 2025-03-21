using System.Collections;
using UnityEngine;

namespace MobileActionKit
{
    public class HumanoidAiAudioPlayer : MonoBehaviour
    {
        [TextArea]
        public string ScriptInfo = "The script is responsible for playback of different sounds and phrases in various situations. When Ai agent is wounded or dying or reloading weapons or engaging his enemy etc.";

        private HumanoidAiHealth HumanoidAiHealthComponent;

        [System.Serializable]
        public class DeathSoundsClass
        {
            [Tooltip("Drag and drop audio source component which is the child of this Ai agent from the hierarchy into this field")]
            public AudioSource AudioSourceComponent;
            [Tooltip("Drag and drop one or more dying sound clips from the project into this field")]
            public AudioClip[] DyingAudioClips;
        }
        [System.Serializable]
        public class WeaponSoundsClass
        {
            [Tooltip("Drag and drop audio source component which is the child of this Ai agent from the hierarchy into this field")]
            public AudioSource FireSoundAudioSourceComponent;
            [Tooltip("Drag and drop audio source component which is the child of this Ai agent from the hierarchy into this field")]
            public AudioSource ReloadSoundAudioSourceComponent;
            [Tooltip("Drag and drop 'Fire sound clip' from the project window into this field.")]
            public AudioClip WeaponShotAudioClip;
            [Tooltip("Drag and drop the Reload Sound clip from the project window into this field.")]
            public AudioClip WeaponReloadAudioClip;
        }
        [System.Serializable]
        public class FootStepSoundsClass
        {
            [Tooltip("Drag and drop audio source component which is the child of this Ai agent from the hierarchy into this field")]
            public AudioSource FootStepsAudioSourceComponent;
            [Tooltip("Drag and drop 'Footstep sound clip' from the project window into this field.")]
            public AudioClip FootStepAudioClip;
        }
        [System.Serializable]
        public class ReplayableVoicesClass
        {

            [Tooltip("Drag and drop the audio source component which is the child of this AI agent from the hierarchy into this field.")]
            public AudioSource AudioSourceComponent;

            [Tooltip("Drag and drop default behavior audio clips from the project window into this field.")]
            public AudioClip[] DefaultBehaviourAudioClips;
            [Tooltip("Drag and drop engaging the enemy audio clips from the project window into this field.")]
            public AudioClip[] EngagingTheEnemyAudioClips;

            [Tooltip("Drag and drop follower audio clips from the project window into this field.")]
            public AudioClip[] FollowerAudioClips;
            [Tooltip("Drag and drop leader audio clips from the project window into this field.")]
            public AudioClip[] LeaderAudioClips;

            [Tooltip("Minimum time interval between recurring audio clips. Example: '1.0' seconds.")]
            public float MinTimeBetweenRecurringAudioClips = 1f;
            [Tooltip("Maximum time interval between recurring audio clips. Example: '2.0' seconds.")]
            public float MaxTimeBetweenRecurringAudioClips = 2f;

            [Tooltip("Minimum delay after interruption before recurring audio clips can play again. Example: '2.0' seconds.")]
            public float MinRecurringAudioClipsDelayAfterInterruption = 2f;
            [Tooltip("Maximum delay after interruption before recurring audio clips can play again. Example: '4.0' seconds.")]
            public float MaxRecurringAudioClipsDelayAfterInterruption = 4f;
        }

        [System.Serializable]
        public class OneTimeVoicesClass
        {
            [Tooltip("Drag and drop the audio source component which is the child of this AI agent from the hierarchy into this field.")]
            public AudioSource AudioSourceComponent;

            [Tooltip("Drag and drop reloading audio clips from the project window into this field.")]
            public AudioClip[] OnceReloadingAudioClips;
            [Tooltip("Drag and drop melee audio clips from the project window into this field.")]
            public AudioClip[] OnceMeleeAudioClips;
            [Tooltip("Drag and drop emergency audio clips from the project window into this field.")]
            public AudioClip[] OnceEmergencyAudioClips;
            [Tooltip("Drag and drop dead body investigation audio clips from the project window into this field.")]
            public AudioClip[] OnceDeadBodyInvestigationAudioClips;
            [Tooltip("Drag and drop hearing investigation audio clips from the project window into this field.")]
            public AudioClip[] OnceHearingInvestigationAudioClips;
            [Tooltip("Drag and drop wounded audio clips from the project window into this field.")]
            public AudioClip[] OnceWoundedAudioClips;
            [Tooltip("Drag and drop enemy lost audio clips from the project window into this field.")]
            public AudioClip[] OnceEnemyLostAudioClips;
            [Tooltip("Drag and drop target killed audio clips from the project window into this field.")]
            public AudioClip[] OnceTargetKilledAudioClips;
            [Tooltip("Drag and drop grenade alert audio clips from the project window into this field.")]
            public AudioClip[] OnceGrenadeAlertAudioClips;
            [Tooltip("Drag and drop throwing grenade audio clips from the project window into this field.")]
            public AudioClip[] OnceThrowingGrenadeAudioClips;
            [Tooltip("Drag and drop leader at halt point audio clips from the project window into this field.")]
            public AudioClip[] OnceLeaderAtHaltPointAudioClips;

            [Tooltip("Minimum delay between non-recurring audio clips. Example: '1.0' seconds.")]
            public float MinNonRecurringAudioClipsDelay = 1f;
            [Tooltip("Maximum delay between non-recurring audio clips. Example: '2.0' seconds.")]
            public float MaxNonRecurringAudioClipsDelay = 2f;

        }

        [Tooltip("Settings for recurring sound clips based on time intervals. Example: 'Default behavior sounds that play at random intervals while the AI is patrolling.'")]
        public ReplayableVoicesClass RecurringSounds;

        [Tooltip("Settings for non recurring sound clips that play once under specific conditions. Example: 'Emergency state sound that plays once when the AI enters an emergency state.'")]
        public OneTimeVoicesClass NonRecurringSounds;

        [Tooltip("Settings for dying sound clips that play when the AI agent dies.")]
        public DeathSoundsClass DyingSounds;
        [Tooltip("Settings for weapon sound clips that play during firing and reloading actions.")]
        public WeaponSoundsClass WeaponSounds;
        [Tooltip("Settings for footstep sound clips that play when the AI agent moves.")]
        public FootStepSoundsClass DefaultFootStepSounds;


        [HideInInspector]
        public AudioClip StoredFootStepAudioClip;

        bool CanPlayNewIntervalAudio = false;
        bool CanPlayNewSingleIntervalAudio = false;

        [HideInInspector]
        public bool CanPlayRecurringSounds = false;

        [HideInInspector]
        public bool CanPlayNonRecurringSounds = false;

        bool ShouldResetDelayCoroutineBeforePlayingMultipleVoices = false;
        bool IsIntruppted = false;

        bool IsRecurringCoroutineIsRunning = false;
        bool IsIntrupptionCompleted = false;

        AudioClip[] RecurringAudioClipsToPlay;
        AudioClip[] NonRecurringAudioClipsToPlay;

        float Timer;
        float RecurringTimer;
        float NonRecurringTimer;

        private void Start()
        {
            if(WeaponSounds.FireSoundAudioSourceComponent != null)
            {
                WeaponSounds.FireSoundAudioSourceComponent.clip = WeaponSounds.WeaponShotAudioClip;
            }
            if (WeaponSounds.ReloadSoundAudioSourceComponent != null)
            {
                WeaponSounds.ReloadSoundAudioSourceComponent.clip = WeaponSounds.WeaponReloadAudioClip;
            }
            if (DefaultFootStepSounds.FootStepsAudioSourceComponent != null)
            {
                DefaultFootStepSounds.FootStepsAudioSourceComponent.clip = DefaultFootStepSounds.FootStepAudioClip;
            }
            StoredFootStepAudioClip = DefaultFootStepSounds.FootStepAudioClip;
            RecurringTimer = Random.Range(RecurringSounds.MinTimeBetweenRecurringAudioClips, RecurringSounds.MaxTimeBetweenRecurringAudioClips);
            NonRecurringTimer = Random.Range(NonRecurringSounds.MinNonRecurringAudioClipsDelay, NonRecurringSounds.MaxNonRecurringAudioClipsDelay);
            HumanoidAiHealthComponent = GetComponent<HumanoidAiHealth>();
        }
        public void PlayReloadSound()
        {
            WeaponSounds.ReloadSoundAudioSourceComponent.PlayOneShot(WeaponSounds.WeaponReloadAudioClip);
        }
        public void PlayFiringSound()
        {
            WeaponSounds.FireSoundAudioSourceComponent.PlayOneShot(WeaponSounds.WeaponShotAudioClip);
        }
        public void PlayDeathSound()
        {
            if(DyingSounds.DyingAudioClips.Length >= 1)
            {
                AudioClip clip = GetRandomClip(DyingSounds.DyingAudioClips);
                DyingSounds.AudioSourceComponent.clip = clip;
                DyingSounds.AudioSourceComponent.PlayOneShot(clip);
            }
          
        }
        private AudioClip GetRandomClip(AudioClip[] Clip)
        {
            return Clip[Random.Range(0, Clip.Length)];
        }
        private void FootStepSound()
        {
            DefaultFootStepSounds.FootStepsAudioSourceComponent.PlayOneShot(DefaultFootStepSounds.FootStepAudioClip);
        }
        public void PlayRecurringSoundClips(AudioClip[] audioClips)
        {
            RecurringAudioClipsToPlay = audioClips;
            CanPlayRecurringSounds = true;         
        }
        public void PlayNonRecurringSoundClips(AudioClip[] audioClips)
        {
            NonRecurringAudioClipsToPlay = audioClips;
            CanPlayNonRecurringSounds = true;       
        }
        public void PlayRecurringAndNonRecurringSounds()
        {
            if (CanPlayRecurringSounds == true && CanPlayNewIntervalAudio == false)
            {
                CanPlayNewIntervalAudio = true;
                CanPlayNewSingleIntervalAudio = false;
                CanPlayNonRecurringSounds = false;
                Timer = 0;
            }
            else if (CanPlayNonRecurringSounds == true && CanPlayNewSingleIntervalAudio == false)
            {
                CanPlayNewSingleIntervalAudio = true;
                CanPlayRecurringSounds = false;
                Timer = 0;
            }

            if(HumanoidAiHealthComponent != null)
            {
                if (HumanoidAiHealthComponent.IsDied == false)
                {
                    Timer += Time.deltaTime;

                    if (Timer >= RecurringTimer && CanPlayRecurringSounds == true)
                    {
                        if (RecurringAudioClipsToPlay.Length >= 1)
                        {
                            // Debug.Log("Playing Recurring Audio Clips");
                            RecurringTimer = Random.Range(RecurringSounds.MinTimeBetweenRecurringAudioClips, RecurringSounds.MaxTimeBetweenRecurringAudioClips);
                            int Randomise = Random.Range(0, RecurringAudioClipsToPlay.Length);
                            RecurringSounds.AudioSourceComponent.clip = RecurringAudioClipsToPlay[Randomise];
                            RecurringSounds.AudioSourceComponent.Play();
                            CanPlayRecurringSounds = false;
                            Timer = 0;
                        }
                    }
                    else if (Timer >= NonRecurringTimer && CanPlayNonRecurringSounds == true)
                    {
                        if (NonRecurringAudioClipsToPlay.Length >= 1)
                        {
                            //  Debug.Log("Playing Non Recurring Audio Clips");
                            RecurringTimer = Random.Range(RecurringSounds.MinRecurringAudioClipsDelayAfterInterruption, RecurringSounds.MaxRecurringAudioClipsDelayAfterInterruption);
                            NonRecurringTimer = Random.Range(NonRecurringSounds.MinNonRecurringAudioClipsDelay, NonRecurringSounds.MaxNonRecurringAudioClipsDelay);
                            int RandomiseAudioClips = Random.Range(0, NonRecurringAudioClipsToPlay.Length);
                            NonRecurringSounds.AudioSourceComponent.clip = NonRecurringAudioClipsToPlay[RandomiseAudioClips];
                            NonRecurringSounds.AudioSourceComponent.Play();
                            CanPlayNonRecurringSounds = false;
                            CanPlayNewIntervalAudio = false;
                            Timer = 0;
                        }
                    }
                }
            }
          
        }
    }
}









//using System.Collections;
//using UnityEngine;

//namespace MobileActionKit
//{
//    public class HumanoidAiAudioPlayer : MonoBehaviour
//    {
//        [TextArea]
//        public string ScriptInfo = "The script is responsible for playback of different sounds and phrases in various situations. When Ai agent is wounded or dying or reloading weapons or engaging his enemy etc.";

//        [System.Serializable]
//        public class DeathSoundsClass
//        {
//            [Tooltip("[Draft]Drag and drop audio source component which is the child of this Ai agent from the hierarchy into this field")]
//            public AudioSource AudioSourceComponent;
//            [Tooltip("[Draft]Drag and drop one or more dying sound clips from the project into this field")]
//            public AudioClip[] DyingAudioClips;
//        }
//        [System.Serializable]
//        public class WeaponSoundsClass
//        {
//            [Tooltip("[Draft]Drag and drop audio source component which is the child of this Ai agent from the hierarchy into this field")]
//            public AudioSource FireSoundAudioSourceComponent;
//            [Tooltip("[Draft]Drag and drop audio source component which is the child of this Ai agent from the hierarchy into this field")]
//            public AudioSource ReloadSoundAudioSourceComponent;
//            [Tooltip("[Draft]Drag and drop 'Fire sound clip' from the project window into this field.")]
//            public AudioClip WeaponShotAudioClip;
//            [Tooltip("[Draft]Drag and drop the Reload Sound clip from the project window into this field.")]
//            public AudioClip WeaponReloadAudioClip;
//        }
//        [System.Serializable]
//        public class FootStepSoundsClass
//        {
//            [Tooltip("[Draft]Drag and drop audio source component which is the child of this Ai agent from the hierarchy into this field")]
//            public AudioSource FootStepsAudioSourceComponent;
//            [Tooltip("[Draft]Drag and drop 'Footstep sound clip' from the project window into this field.")]
//            public AudioClip FootStepAudioClip;
//        }
//        [System.Serializable]
//        public class ReplayableVoicesClass
//        {
//            [Tooltip("[Draft]Drag and drop audio source component which is the child of this Ai agent from the hierarchy into this field")]
//            public AudioSource AudioSourceComponent;

//            public AudioClip[] DefaultBehaviourAudioClips;
//            public AudioClip[] EngagingTheEnemyAudioClips;

//            public AudioClip[] FollowerAudioClips;
//            public AudioClip[] LeaderAudioClips;

//            public float MinTimeBetweenRecurringAudioClips = 1f;
//            public float MaxTimeBetweenRecurringAudioClips = 2f;

//            public float MinRecurringAudioClipsDelayAfterInterruption = 2f;
//            public float MaxRecurringAudioClipsDelayAfterInterruption = 4f;
//        }

//        [System.Serializable]
//        public class OneTimeVoicesClass
//        {
//            [Tooltip("[Draft]Drag and drop audio source component which is the child of this Ai agent from the hierarchy into this field")]
//            public AudioSource AudioSourceComponent;

//            public AudioClip[] OnceReloadingAudioClips;
//            public AudioClip[] OnceMeleeAudioClips;
//            public AudioClip[] OnceEmergencyAudioClips;
//            public AudioClip[] OnceDeadBodyInvestigationAudioClips;
//            public AudioClip[] OnceHearingInvestigationAudioClips;
//            public AudioClip[] OnceWoundedAudioClips;
//            public AudioClip[] OnceEnemyLostAudioClips;
//            public AudioClip[] OnceTargetKilledAudioClips;
//            public AudioClip[] OnceGrenadeAlertAudioClips;
//            public AudioClip[] OnceThrowingGrenadeAudioClips;
//            public AudioClip[] OnceLeaderAtHaltPointAudioClips;


//            public float MinNonRecurringAudioClipsDelay = 1f;
//            public float MaxNonRecurringAudioClipsDelay = 2f;

//        }

//        [Tooltip("[Draft]Fields in this paragraph are responsible to replay any sound clips based upon time interval for example in default behaviour when the Ai agent is patrolling and the 'TimeBetweenNextSound' after" +
//            " Min/Max is resulted in 3 seconds than after every 3 seconds clips inside the 'ClipsToPlayInDefaultBehaviour' field will randomly chosen and be played.")]
//        public ReplayableVoicesClass RecurringSounds;

//        [Tooltip("[Draft]Fields in this paragraph are responsible to play sound only once for example : If the Ai agent goes into the emergency state than firstly a random clip will be chosen from the field name" +
//            " 'ClipsToPlayInEmergencyState' after that it will playback that sound clip only once and will not repeat until the emergency state expired.")]
//        public OneTimeVoicesClass NonRecurringSounds;

//        [Tooltip("[Draft]Fields in this paragraph are responsible to playback Dying sound for the Ai agent")]
//        public DeathSoundsClass DyingSounds;
//        [Tooltip("[Draft]Fields in this paragraph are responsible to playback weapon sounds for the Ai agent")]
//        public WeaponSoundsClass WeaponSounds;
//        [Tooltip("[Draft]Fields in this paragraph are responsible to playback footsteps sounds for the Ai agent")]
//        public FootStepSoundsClass DefaultFootStepSounds;


//        [HideInInspector]
//        public AudioClip StoredFootStepAudioClip;

//        bool CanPlayNewIntervalAudio = false;
//        bool CanPlayNewSingleIntervalAudio = false;

//        [HideInInspector]
//        public bool CanPlayRecurringSounds = false;

//        bool ShouldResetDelayCoroutineBeforePlayingMultipleVoices = false;
//        bool IsIntruppted = false;

//        bool IsRecurringCoroutineIsRunning = false;
//        bool IsIntrupptionCompleted = false;

//        AudioClip[] RecurringAudioClipsToPlay;
//        AudioClip[] NonRecurringAudioClipsToPlay;

//        private void Start()
//        {
//            WeaponSounds.FireSoundAudioSourceComponent.clip = WeaponSounds.WeaponShotAudioClip;
//            WeaponSounds.ReloadSoundAudioSourceComponent.clip = WeaponSounds.WeaponReloadAudioClip;
//            DefaultFootStepSounds.FootStepsAudioSourceComponent.clip = DefaultFootStepSounds.FootStepAudioClip;
//            StoredFootStepAudioClip = DefaultFootStepSounds.FootStepAudioClip;
//        }
//        IEnumerator CoroForPlayingNextMultipleAiIntervalVoices()
//        {
//            IsRecurringCoroutineIsRunning = true;
//           // Debug.Log("Here it is");
//            if (IsIntruppted == false)
//            {
//                float RandomTime = Random.Range(RecurringSounds.MinTimeBetweenRecurringAudioClips, RecurringSounds.MaxTimeBetweenRecurringAudioClips);
//                yield return new WaitForSeconds(RandomTime);
//                if (CanPlayRecurringSounds == false)
//                {
//                    int Randomise = Random.Range(0, RecurringAudioClipsToPlay.Length);
//                    RecurringSounds.AudioSourceComponent.clip = RecurringAudioClipsToPlay[Randomise];
//                    RecurringSounds.AudioSourceComponent.Play();
//                    CanPlayNewIntervalAudio = false;
//                    Debug.Log("Playing Recurring Audio");

//                }
//            }
//            else
//            {
//                if (CanPlayRecurringSounds == false)
//                {
//                    int Randomise = Random.Range(0, RecurringAudioClipsToPlay.Length);
//                    RecurringSounds.AudioSourceComponent.clip = RecurringAudioClipsToPlay[Randomise];
//                    RecurringSounds.AudioSourceComponent.Play();
//                    CanPlayNewIntervalAudio = false;
//                    Debug.Log("Playing Recurring Audio");
//                }
//                IsIntrupptionCompleted = true; 
//            }
//          //  Debug.Log("Coro Ended");

//            IsRecurringCoroutineIsRunning = false;

//        }
//        public void PlayReloadSound()
//        {
//            WeaponSounds.ReloadSoundAudioSourceComponent.PlayOneShot(WeaponSounds.WeaponReloadAudioClip);
//        }
//        public void PlayFiringSound()
//        {
//            WeaponSounds.FireSoundAudioSourceComponent.PlayOneShot(WeaponSounds.WeaponShotAudioClip);
//        }
//        public void PlayDeathSound()
//        {
//            AudioClip clip = GetRandomClip(DyingSounds.DyingAudioClips);
//            DyingSounds.AudioSourceComponent.clip = clip;
//            DyingSounds.AudioSourceComponent.PlayOneShot(clip);
//        }
//        private AudioClip GetRandomClip(AudioClip[] Clip)
//        {
//            return Clip[Random.Range(0, Clip.Length)];
//        }
//        private void FootStepSound()
//        {
//            DefaultFootStepSounds.FootStepsAudioSourceComponent.PlayOneShot(DefaultFootStepSounds.FootStepAudioClip);
//        }
//        public void PlayRecurringSoundClips(AudioClip[] audioClips)
//        {
//            RecurringAudioClipsToPlay = audioClips;
//            if (CanPlayRecurringSounds == false)
//            {
//                if (audioClips != null)
//                {
//                    if (audioClips.Length >= 1)
//                    {
//                        if (IsRecurringCoroutineIsRunning == false)
//                        {
//                            if(IsIntrupptionCompleted == true)
//                            {
//                                CanPlayNewIntervalAudio = false;
//                                IsIntruppted = false;
//                                IsIntrupptionCompleted = false;
//                            }
//                            if (CanPlayNewIntervalAudio == false)
//                            {
//                               // Debug.Log("Coro started");
//                                StartCoroutine(CoroForPlayingNextMultipleAiIntervalVoices());
//                                CanPlayNewIntervalAudio = true;
//                            }
//                        }

//                    }
//                }

//            }
//        }
//        public void PlayNonRecurringSoundClips(AudioClip[] audioClips)
//        {
//            NonRecurringAudioClipsToPlay = audioClips;
//            if (CanPlayNewSingleIntervalAudio == false)
//            {
//                if (audioClips != null)
//                {
//                    if (audioClips.Length >= 1)
//                    {
//                        StartCoroutine(DelayBeforePlayingSingleClip());
//                        CanPlayNewSingleIntervalAudio = true;
//                    }
//                }

//            }

//        }
//        IEnumerator DelayBeforePlayingSingleClip()
//        {
//            StopCoroutine(CoroForPlayingNextMultipleAiIntervalVoices());
//            StopCoroutine(DelayBeforeMultipleAudioClips());
//            ShouldResetDelayCoroutineBeforePlayingMultipleVoices = false;
//            IsRecurringCoroutineIsRunning = false; 
//            float Randomise = Random.Range(NonRecurringSounds.MinNonRecurringAudioClipsDelay, NonRecurringSounds.MaxNonRecurringAudioClipsDelay);
//            yield return new WaitForSeconds(Randomise);
//            CanPlayNewIntervalAudio = false;
//            int RandomiseAudioClips = Random.Range(0, NonRecurringAudioClipsToPlay.Length);
//            NonRecurringSounds.AudioSourceComponent.clip = NonRecurringAudioClipsToPlay[RandomiseAudioClips];
//            NonRecurringSounds.AudioSourceComponent.Play();
//            Debug.Log("Playing Non Recurring Audio");
//            CanPlayRecurringSounds = true;
//            if (ShouldResetDelayCoroutineBeforePlayingMultipleVoices == false)
//            {
//                StartCoroutine(DelayBeforeMultipleAudioClips());
//                ShouldResetDelayCoroutineBeforePlayingMultipleVoices = true;
//            }

//            CanPlayNewSingleIntervalAudio = false;

//        }
//        IEnumerator DelayBeforeMultipleAudioClips()
//        {
//            float Randomise = Random.Range(RecurringSounds.MinRecurringAudioClipsDelayAfterInterruption, RecurringSounds.MaxRecurringAudioClipsDelayAfterInterruption);
//            yield return new WaitForSeconds(Randomise);
////            Debug.Log("After Intrupption completed");
//            CanPlayRecurringSounds = false;
//            ShouldResetDelayCoroutineBeforePlayingMultipleVoices = false;
//            IsIntruppted = true;
//        }
//    }
//}