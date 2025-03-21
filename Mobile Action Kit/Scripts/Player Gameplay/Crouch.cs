using UnityEngine.UI;
using UnityEngine;

// This Script is Responsible for Player Crouching Behaviour

namespace MobileActionKit
{
    public class Crouch : MonoBehaviour
    {

        public static Crouch instance;

        [TextArea]
        public string ScriptInfo = "This Script Controls The Player Crouch Behaviour";
        [Space(10)]

        [Tooltip("Reference to the player's camera. Used for adjusting the camera's height when crouching or standing.")]
        public GameObject PlayerCamera;

        [HideInInspector]
        [Tooltip("Indicates if the player is currently crouching.")]
        public bool IsCrouching = false;

        [Tooltip("Speed at which the player crouches down.")]
        public float SitDownSpeed = 7f;

        [Tooltip("Speed at which the player stands up.")]
        public float StandUpSpeed = 7f;

        [Tooltip("The height of the camera when crouching.")]
        public float CrouchHeight = 0.3f;

        [Tooltip("The height of the camera when standing.")]
        public float StandHeight = 0.6f;

        [Tooltip("Collider used when the player is standing.")]
        public CapsuleCollider StandCapsuleCollider;

        [Tooltip("Collider used when the player is crouching.")]
        public CapsuleCollider CrouchCapsuleCollider;

        [Tooltip("Standing Sprite to show when the crouch button is clicked.")]
        public Sprite StandingSprite;

        [Tooltip("Crouching Sprite to show when the crouch button is clicked.")]
        public Sprite CrouchingSprite;

        bool WasInCrouch = false;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
        }
        private void FixedUpdate()
        {
            //if (cc.isGrounded == true)
            //{
            //    Vector3 moveDirection = transform.position ;
            //    moveDirection.y -= GravitationalForce;//* Time.smoothDeltaTime;                         
            //    transform.position = moveDirection;
            //}

            if (IsCrouching == true)
            {
                StandCapsuleCollider.enabled = false;
                CrouchCapsuleCollider.enabled = true;
                //   Debug.Break();
                Vector3 Crouch = PlayerCamera.transform.localPosition;
                Crouch.y = Mathf.Lerp(Crouch.y, CrouchHeight, SitDownSpeed * Time.deltaTime);
                PlayerCamera.transform.localPosition = Crouch;
                WasInCrouch = true;
            }
            else
            {
                if (WasInCrouch == true)
                {
                    StandCapsuleCollider.enabled = true;
                    CrouchCapsuleCollider.enabled = false;
                    //Debug.Break();
                    //cc.height = Mathf.Lerp(cc.height, height, StandSpeed * Time.deltaTime);
                    //if(cc.height >= height)
                    //{
                    //    WasInCrouch = false;
                    //}

                    Vector3 Crouch = PlayerCamera.transform.localPosition;
                    Crouch.y = Mathf.Lerp(Crouch.y, StandHeight, StandUpSpeed * Time.deltaTime);
                    PlayerCamera.transform.localPosition = Crouch;
                }

            }
        }
        public void Crouching(Image i)
        {
            if (PlayerManager.instance != null)
            {

                if (PlayerManager.instance.AllBobbingScripts != null)
                {

                    for (int x = 0; x < PlayerManager.instance.AllBobbingScripts.Length; x++)
                    {
                        PlayerManager.instance.AllBobbingScripts[x].StopLoop = false;
                        PlayerManager.instance.AllBobbingScripts[x].StartLoop = false;
                        PlayerManager.instance.AllBobbingScripts[x].WalkStopLoop = false;
                        PlayerManager.instance.AllBobbingScripts[x].WalkStartLoop = false;
                    }
                }
            }

            if (IsCrouching == false)
            {
                IsCrouching = true;
                i.sprite = StandingSprite;
            }
            else
            {
                IsCrouching = false;
                i.sprite = CrouchingSprite;
            }

            if (PlayerManager.instance != null)
            {
                for (int x = 0; x < PlayerManager.instance.AllBobbingScripts.Length; x++)
                {
                    if (PlayerManager.instance.AllBobbingScripts[x].gameObject.activeInHierarchy == true)
                    {
                        PlayerManager.instance.AllBobbingScripts[x].DefaultLoopingValues(PlayerManager.instance.AllBobbingScripts[x].BobResetDurations.StationaryStandHipFireResetDuration,
                            PlayerManager.instance.AllBobbingScripts[x].BobResetDurations.StationaryCrouchHipFireResetDuration,
                           PlayerManager.instance.AllBobbingScripts[x].BobResetDurations.StationaryStandAimedResetDuration,
                           PlayerManager.instance.AllBobbingScripts[x].BobResetDurations.StationaryCrouchAimedResetDuration, "Crouching");
                    }
                }
            }
        }
        public void Pc_Controls_Crouch()
        {
            if (PlayerManager.instance != null)
            {
                if (PlayerManager.instance.AllBobbingScripts[0] != null)
                {
                    for (int x = 0; x < PlayerManager.instance.AllBobbingScripts.Length; x++)
                    {
                        PlayerManager.instance.AllBobbingScripts[x].StopLoop = false;
                        PlayerManager.instance.AllBobbingScripts[x].StartLoop = false;
                        PlayerManager.instance.AllBobbingScripts[x].WalkStopLoop = false;
                        PlayerManager.instance.AllBobbingScripts[x].WalkStartLoop = false;
                    }
                }
            }

            if (IsCrouching == false)
            {
                IsCrouching = true;

            }
            else
            {
                IsCrouching = false;

            }

            if (PlayerManager.instance != null)
            {
                for (int x = 0; x < PlayerManager.instance.AllBobbingScripts.Length; x++)
                {
                    if (PlayerManager.instance.AllBobbingScripts[x].gameObject.activeInHierarchy == true)
                    {
                        PlayerManager.instance.AllBobbingScripts[x].DefaultLoopingValues(PlayerManager.instance.AllBobbingScripts[x].BobResetDurations.StationaryStandHipFireResetDuration,
                            PlayerManager.instance.AllBobbingScripts[x].BobResetDurations.StationaryCrouchHipFireResetDuration,
                           PlayerManager.instance.AllBobbingScripts[x].BobResetDurations.StationaryStandAimedResetDuration,
                           PlayerManager.instance.AllBobbingScripts[x].BobResetDurations.StationaryCrouchAimedResetDuration, "PcCrouch");
                    }
                }
            }
        }
    }
}