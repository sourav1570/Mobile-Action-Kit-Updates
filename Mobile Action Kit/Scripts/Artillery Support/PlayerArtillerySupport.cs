using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace MobileActionKit
{
    public class PlayerArtillerySupport : MonoBehaviour
    {
        [TextArea]
        public string ScriptInfo = "This Script is responsible for calling artillery support when the UI button is clicked by the player.";

        [Tooltip("Drag and drop the UI button from the hierarchy into this field.")]
        public Button ArtillerySupportButton;

        [Tooltip("[Drag and drop the player's camera attached with the player hierarchy into this field.")]
        public Transform PlayerCamera; // Reference to the FPS camera

        [Tooltip("Range within which player can designate the target for artillery support.")]
        public float TargetDesignatorRange = 100f;
        RaycastHit hit;

        [Tooltip("Artillery shells spawn altitude above the target point surface.")]
        public float ShellsSpawnAltitudeFromImpactPoint = 10f;

        [Tooltip("Number of artillery shells in a barrage.")]
        public int ShellsInBarrage = 5;
        [Tooltip("Drag and drop the explosive prefab from the project into this field.")]
        public GameObject ArtilleryShellPrefab;

        [Tooltip("Minimal positional offset from the target point between falling artillery shells.")]
        public Vector3 MinSpawnOffset = Vector3.zero;
        [Tooltip("Maximal positional offset from the target point between falling artillery shells.")]
        public Vector3 MaxSpawnOffset = Vector3.zero;

        [Tooltip("Minimal time interval between falling shells.")]
        public float MinArtilleryShellsInterval = 0.1f;

        [Tooltip("Maximal time interval between falling shells")]
        public float MaxArtilleryShellsInterval = 0.5f;

        [Tooltip("Speed of incoming artillery shells.")]
        public float ArtilleryShellSpeed = 10f;

        public float BarrageDelay = 0.5f;

        [Tooltip("Artillery support button reactivation time since previous use.")]
        public float ButtonReactivationDelay = 3f;

        [Tooltip("Specify the color of the UI button when it is interactable.")]
        public Color ActivatedButtonColor = Color.white;

        [Tooltip("Specify the color of the UI button when it is not interactable.")]
        public Color DeactivatedButtonColor = Color.gray;
        bool CanAttack = true;

        void Start()
        {
            ArtillerySupportButton.onClick.AddListener(ActivateArtillerySupport);
        }

        public void ActivateArtillerySupport()
        {
            if (Physics.Raycast(PlayerCamera.transform.position, PlayerCamera.transform.forward, out hit, TargetDesignatorRange))
            {
                if (hit.collider != null)
                {
                    if (CanAttack == true)
                    {
                        StartCoroutine(RefreshUIButton());
                        StartCoroutine(SpawnShellsWithDelay(hit));
                        CanAttack = false;
                        PlayerPrefs.SetInt("Item_ArtillerySupportIsInUse", 1);
                    }
                }

            }
        }
        private IEnumerator SpawnShellsWithDelay(RaycastHit hit)
        {
            bool DelayFirstBomb = false;
            for (int i = 0; i < ShellsInBarrage; i++)
            {
                if(DelayFirstBomb == false)
                {
                    yield return new WaitForSeconds(BarrageDelay);
                    DelayFirstBomb = true;
                }
                Vector3 Offset = new Vector3(Random.Range(MinSpawnOffset.x, MaxSpawnOffset.x), 0f, Random.Range(MinSpawnOffset.z, MaxSpawnOffset.z));
                Vector3 spawnPosition = hit.point + Vector3.up * ShellsSpawnAltitudeFromImpactPoint + Offset;

                GameObject bomb = Instantiate(ArtilleryShellPrefab, spawnPosition, ArtilleryShellPrefab.transform.rotation);
                Rigidbody bombRigidbody = bomb.GetComponent<Rigidbody>();
                if (bombRigidbody != null)
                {
                    // Set velocity to give the bombs downward motion
                    bombRigidbody.velocity = Vector3.down * ArtilleryShellSpeed; // Adjust the downward speed as needed
                }
                float delay = Random.Range(MinArtilleryShellsInterval, MaxArtilleryShellsInterval); // Random delay between 0.1f and 0.3f
                yield return new WaitForSeconds(delay);
            }
        }
        IEnumerator RefreshUIButton()
        {
            ArtillerySupportButton.enabled = false;
            ArtillerySupportButton.GetComponent<Image>().color = DeactivatedButtonColor;
            yield return new WaitForSeconds(ButtonReactivationDelay);
            ArtillerySupportButton.enabled = true;
            ArtillerySupportButton.GetComponent<Image>().color = ActivatedButtonColor;
            CanAttack = true;
        }

    }
}
