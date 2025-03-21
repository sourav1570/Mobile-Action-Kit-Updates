using UnityEngine;
using TMPro;
using System.Collections;

namespace MobileActionKit
{
    public class PlayerCheckpoint : MonoBehaviour
    {
        [TextArea]
        public string ScriptInfo = "This script saves and restores the player's position and rotation using PlayerPrefs when they enter a checkpoint.";
        [Space(10)]

        [Tooltip("Unique key used to store and retrieve the player's checkpoint data in PlayerPrefs.")]
        public string UniqueNameForSavingCheckpoint = "PlayerCheckpoint";

        [Tooltip("Reference to the player's transform.")]
        public Transform Player;

        [Tooltip("Whether to save and load the player's X position.")]
        public bool SavePositionX = true;
        [Tooltip("Whether to save and load the player's Y position.")]
        public bool SavePositionY = true;
        [Tooltip("Whether to save and load the player's Z position.")]
        public bool SavePositionZ = true;

        [Tooltip("Whether to save and load the player's X rotation.")]
        public bool SaveRotationX = true;
        [Tooltip("Whether to save and load the player's Y rotation.")]
        public bool SaveRotationY = true;
        [Tooltip("Whether to save and load the player's Z rotation.")]
        public bool SaveRotationZ = true;

        [Tooltip("Text element that displays a message when a checkpoint is reached.")]
        public TextMeshProUGUI CheckpointReachedText;

        [Tooltip("The message that will be shown when the checkpoint is reached.")]
        public string MessageToDisplay = "Checkpoint Reached!";

        [Tooltip("Duration (in seconds) for which the checkpoint reached message stays visible.")]
        public float CheckpointReachedTextActiveDuration = 2f;


        float posX;
        float posY;
        float posZ;

        float rotX;
        float rotY;
        float rotZ;
        float rotW;

        Collider collider;

        private void Start()
        {
            if(GetComponent<Collider>() != null)
            {
                collider = GetComponent<Collider>();
            }

            if (PlayerPrefs.GetInt(UniqueNameForSavingCheckpoint) == 1)
            {
                if(collider != null)
                {
                    collider.enabled = false;
                }
            }
            posX = Player.transform.position.x;
            posY = Player.transform.position.y;
            posZ = Player.transform.position.z;

            rotX = Player.transform.rotation.x;
            rotY = Player.transform.rotation.y;
            rotZ = Player.transform.rotation.z;
            rotW = Player.transform.rotation.w;

            // Check if a saved position exists in PlayerPrefs

            // Load and set the player's position

            if (SavePositionX == true && PlayerPrefs.HasKey("PlayerPosX"))
            {
                posX = PlayerPrefs.GetFloat("PlayerPosX");
            }
            if (SavePositionY == true && PlayerPrefs.HasKey("PlayerPosY"))
            {
                posY = PlayerPrefs.GetFloat("PlayerPosY");
            }
            if (SavePositionZ == true && PlayerPrefs.HasKey("PlayerPosZ"))
            {
                posZ = PlayerPrefs.GetFloat("PlayerPosZ");
            }

            Player.position = new Vector3(posX, posY, posZ);

            // Load and set the player's rotation

            if (PlayerPrefs.HasKey("PlayerRotX") && SaveRotationX == true)
            {
                rotX = PlayerPrefs.GetFloat("PlayerRotX");
            }

            if (PlayerPrefs.HasKey("PlayerRotY") && SaveRotationY == true)
            {
                rotY = PlayerPrefs.GetFloat("PlayerRotY");
            }

            if (PlayerPrefs.HasKey("PlayerRotZ") && SaveRotationZ == true)
            {
                rotZ = PlayerPrefs.GetFloat("PlayerRotZ");
            }

            if (PlayerPrefs.HasKey("PlayerRotW"))
            {
                rotW = PlayerPrefs.GetFloat("PlayerRotW");
            }

            Player.rotation = new Quaternion(rotX, rotY, rotZ, rotW);
        }

        private void OnTriggerEnter(Collider other)
        {
            // Check if the object that entered the trigger is the player
            if (other.transform == Player)
            {
                if(PlayerPrefs.GetInt(UniqueNameForSavingCheckpoint) == 0)
                {
                    if (CheckpointReachedText != null)
                    {
                        CheckpointReachedText.gameObject.SetActive(true);
                        StartCoroutine(Coro());
                        CheckpointReachedText.text = MessageToDisplay;
                    }

                    if (SavePositionX == true)
                    {
                        PlayerPrefs.SetFloat("PlayerPosX", Player.position.x);
                    }
                    if (SavePositionY == true)
                    {
                        PlayerPrefs.SetFloat("PlayerPosY", Player.position.y);
                    }
                    if (SavePositionZ == true)
                    {
                        PlayerPrefs.SetFloat("PlayerPosZ", Player.position.z);
                    }

                    if (SaveRotationX == true)
                    {
                        PlayerPrefs.SetFloat("PlayerRotX", Player.rotation.x);
                    }

                    if (SaveRotationY == true)
                    {
                        PlayerPrefs.SetFloat("PlayerRotY", Player.rotation.y);
                    }

                    if (SaveRotationZ == true)
                    {
                        PlayerPrefs.SetFloat("PlayerRotZ", Player.rotation.z);
                    }

                    PlayerPrefs.SetFloat("PlayerRotW", Player.rotation.w);

                    PlayerPrefs.SetInt(UniqueNameForSavingCheckpoint, 1);

                    PlayerPrefs.Save();

                    if (collider != null)
                    {
                        collider.enabled = false;
                    }
                }
            
            }
        }
        IEnumerator Coro()
        {
            yield return new WaitForSeconds(CheckpointReachedTextActiveDuration);
            CheckpointReachedText.gameObject.SetActive(false);

        }
    }
}