using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


namespace MobileActionKit
{
    public class PlayerAirSupport : MonoBehaviour
    {
        [TextArea]
        public string ScriptInfo = "This script is responsible for 'CAS' (close air support) to player.";

        [Tooltip("Drag and drop the UI button from the hierarchy into this field.")]
        public Button AirSupportUIButton;
        [Tooltip("Drag and drop player camera from player's hierarchy into this field.")]
        public Transform PlayerCamera; // Reference to the FPS camera
        [Tooltip("Range within which player can designate the target for close air support.")]
        public float TargetDesignatorRange;
        RaycastHit hit;
        [Tooltip("Drag and drop the Jet prefab from the project into this field.")]
        public GameObject AirSupportPlanePrefab;

        [Tooltip("Specify the visible layers for raycasting.")]
        public LayerMask TargetDesignatorVisibleLayers;

        [Tooltip("Air support spawn altitude from target point surface.")]
        public float JetSpawnAltitude = 10f;


        [Tooltip("Payload release altitude from target point surface.")]
        public float PayloadReleaseAltitude = 10f;


        [Tooltip("[Specifies the minimum distance from the 'MinSpawnPointDistanceFromHitPoint' coordinate at which the plane can be spawned.")]
        public float MinSpawnPointDistanceFromHitPoint = 0f;

        [Tooltip("Specifies the maximum distance from the 'MaxSpawnPointDistanceFromHitPoint' coordinate at which the plane can be spawned.")]
        public float MaxSpawnPointDistanceFromHitPoint = 100f;

        [Tooltip("If checked, the plane will rotate on both the X and Y axes to look at the explosive drop point until it reaches there. If unchecked, the jet will only rotate on the Y axis.")]
        public bool EnableDiveAttack = true;

        [Tooltip("Enter the number of plane to spawn.")]
        public int NumberOfPlanesToSpawn = 1;

        public float MinPlaneSpawnDelay = 1f;

        public float MaxPlaneSpawnDelay = 1f;


        [Tooltip("If the 'number of planes to spawn' is greater than 1, you can control the minimum offset and distance between each aircraft from the spawn point.")]
        public Vector3 MinDistanceBetweenAircraft = Vector3.zero;

        [Tooltip("If the 'number of planes to spawn' is greater than 1, you can control the minimum offset and distance between each aircraft from the spawn point.")]
        public Vector3 MaxDistanceBetweenAircraft = Vector3.zero;

        [Tooltip("If the 'number of planes to spawn' is greater than 1, you can create an minimum offset to the explosive drop point for each jet to avoid collisions while moving towards the destination.")]
        public Vector3 MinTargetAreaRadius = Vector3.zero;

        [Tooltip("If the 'number of planes to spawn' is greater than 1, you can create an maximum offset to the explosive drop point for each jet to avoid collisions while moving towards the destination.")]
        public Vector3 MaxTargetAreaRadius = Vector3.zero;

        [Tooltip("[If checked than the plane prefab will be spawned from the right side of the player camera view.")]
        public bool AttackTargetAreaFromTheRight = true;

        [Tooltip(" If checked than the plane prefab will be spawned from the left side of the player camera view.")]
        public bool AttackTargetAreaFromTheLeft = true;

        [Tooltip("If checked than the plane prefab will be spawned from the front side of the player camera view.")]
        public bool AttackTargetAreaFromTheFront = true;

        [Tooltip("If checked than the plane prefab will be spawned from the rear side of the player camera view.")]
        public bool AttackTargetAreaFromTheRear = true;

        [Tooltip("[Specify the duration (in seconds) before the UI button becomes interactable again after using the Air support.")]
        public float AirSupportButtonResetDelay = 3f;

        [Tooltip("Specify the color of the UI button when it is interactable.")]
        public Color ActivatedButtonColor = Color.white;

        [Tooltip("Specify the color of the UI button when it is not interactable.")]
        public Color DeactivatedButtonColor = Color.gray;

        bool CanAttack = true;

        [Tooltip("Drag and drop a 'TextMeshPro' text component from the hierarchy into this field to display the message of Air support availability.")]
        public TextMeshProUGUI AirSupportAvailablityText;

        [Tooltip("Enter the message to display when Air support is available.")]
        public string MessageIfAirSupportAvailable = "JET IS ARRIVING!";

        [Tooltip("" +
            "Enter the message to display when Air support is unavailable.")]
        public string MessageIfAirSupportUnavailable = "JET SUPPORT NOT AVAILABLE!";

        [Tooltip("Specify the duration (in seconds) for which the message will be displayed.")]
        public float TextMessageDisplayDuration = 3f;

        bool IsTextActive = false;
        Quaternion horizontalRotation;
        Quaternion jetRotation;

        public bool DebugJetPayloadReleasePoint = false;
        public GameObject PayloadReleasePointVisualiser;

        void Start()
        {
            AirSupportUIButton.onClick.AddListener(ActivateAirSupport);
        }
        // Commented because this code allow Jet to come from fixed coordinates like in world space for example if the player camera is looking backward
        // the spawn point will still going to remain the same for the Jet for the right,left,forward and backward rather than according to the player camera orientation.
        //void DoExplosion()
        //{
        //    if (Physics.Raycast(PlayerCamera.transform.position, PlayerCamera.transform.forward, out hit, Range, VisibleLayers))
        //    {
        //        if (hit.collider != null && CanAttack == true)
        //        {
        //            if (AirSupportAvailablityText != null)
        //            {
        //                if (IsTextActive == false)
        //                {
        //                    StartCoroutine(AirSupportTimerForText());
        //                    IsTextActive = true;
        //                }
        //                AirSupportAvailablityText.gameObject.SetActive(true);
        //                AirSupportAvailablityText.text = MessageIfJetAvailable;
        //            }

        //            Vector3 destinationPosition = hit.point + Vector3.up * DropHeightFromHitPoint;
        //            Vector3 Spawnposition = hit.point + Vector3.up * JetSpawnHeightFromHitPoint;
        //            Vector3 StoreReachCoordinate = destinationPosition;

        //            // Create a list to store enabled sides
        //            List<Vector3> enabledSides = new List<Vector3>();

        //            // Add enabled sides to the list
        //            if (SpawnFromRightSideOfHitPoint)
        //                enabledSides.Add(Vector3.right);
        //            if (SpawnFromLeftSideOfHitPoint)
        //                enabledSides.Add(Vector3.left);
        //            if (SpawnFromForwardSideOfHitPoint)
        //                enabledSides.Add(Vector3.forward);
        //            if (SpawnFromBackwardSideOfHitPoint)
        //                enabledSides.Add(Vector3.back);

        //            // Shuffle the list of enabled sides
        //            Shuffle(enabledSides);

        //            // Randomly select an enabled side
        //            Vector3 selectedSide = enabledSides[Random.Range(0, enabledSides.Count)];

        //            // Randomly offset the spawn position within the specified distance based on the selected side
        //            float offsetX = Random.Range(MinSpawnPointsCreationDistanceFromHitPoint, MaxSpawnPointsCreationDistanceFromHitPoint);
        //            float offsetZ = Random.Range(MinSpawnPointsCreationDistanceFromHitPoint, MaxSpawnPointsCreationDistanceFromHitPoint);
        //            Spawnposition += selectedSide * offsetX + selectedSide * offsetZ;

        //            // Calculate the direction vector from jet to spawn point
        //            Vector3 lookDirection = hit.point - Spawnposition;

        //            if(RotateJetOnXWhenMovingTowardsDestination == true)
        //            {
        //                //  lookDirection.y = 0f; // Ignore vertical component

        //                // Rotate the jet to face the spawn point horizontally
        //                 horizontalRotation = Quaternion.LookRotation(lookDirection);
        //                 jetRotation = Quaternion.Euler(horizontalRotation.eulerAngles.x, horizontalRotation.eulerAngles.y, 0f); // Only rotate horizontally
        //            }
        //            else
        //            {
        //                lookDirection.y = 0f; // Ignore vertical component
        //                // Rotate the jet to face the spawn point horizontally
        //                 horizontalRotation = Quaternion.LookRotation(lookDirection);
        //                 jetRotation = Quaternion.Euler(0f, horizontalRotation.eulerAngles.y, 0f); // Only rotate horizontally
        //            }


        //            for (int x = 0; x < NumberOfJetsToSpawn; x++)
        //            {
        //                GameObject jet = Instantiate(JetToSpawn, Spawnposition, jetRotation);

        //                jet.transform.position = jet.transform.position + new Vector3(
        //                    Random.Range(MinJetOffsetValueFromSpawnPoint.x, MaxJetOffsetValueFromSpawnPoint.x),
        //                    Random.Range(MinJetOffsetValueFromSpawnPoint.y, MaxJetOffsetValueFromSpawnPoint.y),
        //                    Random.Range(MinJetOffsetValueFromSpawnPoint.z, MaxJetOffsetValueFromSpawnPoint.z));


        //                StoreReachCoordinate = StoreReachCoordinate + new Vector3
        //                    (Random.Range(MinDestinationErrorOffset.x, MaxDestinationErrorOffset.x),
        //                     Random.Range(MinDestinationErrorOffset.y, MaxDestinationErrorOffset.y),
        //                     Random.Range(MinDestinationErrorOffset.z, MaxDestinationErrorOffset.z));

        //                jet.GetComponent<JetController>().SetDestination(StoreReachCoordinate);
        //            }

        //            StartCoroutine(RefreshUIButton());
        //            CanAttack = false;
        //        }
        //        else
        //        {
        //            if (AirSupportAvailablityText != null)
        //            {
        //                if (IsTextActive == false)
        //                {
        //                    StartCoroutine(AirSupportTimerForText());
        //                    IsTextActive = true;
        //                }
        //                AirSupportAvailablityText.gameObject.SetActive(true);
        //                AirSupportAvailablityText.text = MessageIfJetNotAvailable;
        //            }
        //        }
        //    }
        //    else
        //    {
        //        if (AirSupportAvailablityText != null)
        //        {
        //            if (IsTextActive == false)
        //            {
        //                StartCoroutine(AirSupportTimerForText());
        //                IsTextActive = true;
        //            }
        //            AirSupportAvailablityText.gameObject.SetActive(true);
        //            AirSupportAvailablityText.text = MessageIfJetNotAvailable;
        //        }
        //    }
        //}
        public void ActivateAirSupport()
        {
            if (Physics.Raycast(PlayerCamera.transform.position, PlayerCamera.transform.forward, out hit, TargetDesignatorRange, TargetDesignatorVisibleLayers))
            {
                if (hit.collider != null && CanAttack == true)
                {
                    if (AirSupportAvailablityText != null)
                    {
                        if (IsTextActive == false)
                        {
                            StartCoroutine(AirSupportTimerForText());
                            IsTextActive = true;
                            PlayerPrefs.SetInt("Item_AirSupportIsInUse", 1);
                        }
                        AirSupportAvailablityText.gameObject.SetActive(true);
                        AirSupportAvailablityText.text = MessageIfAirSupportAvailable;
                    }

                    // Calculate the spawn position and store the hit point
                    Vector3 Spawnposition = hit.point + Vector3.up * JetSpawnAltitude;
                    Vector3 StoreReachCoordinate = hit.point + Vector3.up * PayloadReleaseAltitude;

                    // Calculate the camera's right vector in world space
                    Vector3 cameraRight = PlayerCamera.transform.TransformDirection(Vector3.right);

                    // Create a list to store enabled sides
                    List<Vector3> enabledSides = new List<Vector3>();

                    // Add enabled sides to the list
                    if (AttackTargetAreaFromTheRight)
                        enabledSides.Add(cameraRight);
                    if (AttackTargetAreaFromTheLeft)
                        enabledSides.Add(-cameraRight);
                    if (AttackTargetAreaFromTheFront)
                        enabledSides.Add(PlayerCamera.transform.forward);
                    if (AttackTargetAreaFromTheRear)
                        enabledSides.Add(-PlayerCamera.transform.forward);

                    // Shuffle the list of enabled sides
                    Shuffle(enabledSides);

                    // Randomly select an enabled side
                    Vector3 selectedSide = enabledSides[Random.Range(0, enabledSides.Count)];

                    // Randomly offset the spawn position within the specified distance based on the selected side
                    float offsetX = Random.Range(MinSpawnPointDistanceFromHitPoint, MaxSpawnPointDistanceFromHitPoint);
                    float offsetZ = Random.Range(MinSpawnPointDistanceFromHitPoint, MaxSpawnPointDistanceFromHitPoint);
                    Spawnposition += selectedSide * offsetX + selectedSide * offsetZ;

                    // Calculate the direction vector from jet to spawn point
                    Vector3 lookDirection = hit.point - Spawnposition;
                    if (EnableDiveAttack)
                    {
                        // Rotate the jet to face the spawn point horizontally
                        horizontalRotation = Quaternion.LookRotation(lookDirection);
                        jetRotation = Quaternion.Euler(horizontalRotation.eulerAngles.x, horizontalRotation.eulerAngles.y, 0f); // Only rotate horizontally
                    }
                    else
                    {
                        lookDirection.y = 0f; // Ignore vertical component
                                              // Rotate the jet to face the spawn point horizontally
                        horizontalRotation = Quaternion.LookRotation(lookDirection);
                        jetRotation = Quaternion.Euler(0f, horizontalRotation.eulerAngles.y, 0f); // Only rotate horizontally
                    }

                    // Spawn jets

                    StartCoroutine(AirCraftSpawnIntervalTime(Spawnposition, StoreReachCoordinate));
                    StartCoroutine(RefreshUIButton());
                    CanAttack = false;
                }
                else
                {
                    if (AirSupportAvailablityText != null)
                    {
                        if (IsTextActive == false)
                        {
                            StartCoroutine(AirSupportTimerForText());
                            IsTextActive = true;
                        }
                        AirSupportAvailablityText.gameObject.SetActive(true);
                        AirSupportAvailablityText.text = MessageIfAirSupportUnavailable;
                    }
                }
            }
            else
            {
                if (AirSupportAvailablityText != null)
                {
                    if (IsTextActive == false)
                    {
                        StartCoroutine(AirSupportTimerForText());
                        IsTextActive = true;
                    }
                    AirSupportAvailablityText.gameObject.SetActive(true);
                    AirSupportAvailablityText.text = MessageIfAirSupportUnavailable;
                }
            }
        }

        IEnumerator AirCraftSpawnIntervalTime(Vector3 Spawnposition,Vector3 StoreReachCoordinate)
        {
            float Randomise = Random.Range(MinPlaneSpawnDelay, MaxPlaneSpawnDelay);
            yield return new WaitForSeconds(Randomise);
            for (int x = 0; x < NumberOfPlanesToSpawn; x++)
            {
                // Offset jet spawn position
                Vector3 jetSpawnOffset = new Vector3(
                    Random.Range(MinDistanceBetweenAircraft.x, MaxDistanceBetweenAircraft.x),
                    Random.Range(MinDistanceBetweenAircraft.y, MaxDistanceBetweenAircraft.y),
                    Random.Range(MinDistanceBetweenAircraft.z, MaxDistanceBetweenAircraft.z));

                GameObject jet = Instantiate(AirSupportPlanePrefab, Spawnposition + jetSpawnOffset, jetRotation);

                // Offset destination error
                Vector3 destinationErrorOffset = new Vector3(
                    Random.Range(MinTargetAreaRadius.x, MaxTargetAreaRadius.x),
                    Random.Range(MinTargetAreaRadius.y, MaxTargetAreaRadius.y),
                    Random.Range(MinTargetAreaRadius.z, MaxTargetAreaRadius.z));

#if UNITY_EDITOR
                if (DebugJetPayloadReleasePoint == true)
                {
                    GameObject DebugPoint = Instantiate(PayloadReleasePointVisualiser, StoreReachCoordinate + destinationErrorOffset, PayloadReleasePointVisualiser.transform.rotation);
                }
#endif

                // Set destination for the jet
                jet.GetComponent<AircraftController>().SetDestination(StoreReachCoordinate + destinationErrorOffset);
            }
        }
        IEnumerator AirSupportTimerForText()
        {
            yield return new WaitForSeconds(TextMessageDisplayDuration);
            AirSupportAvailablityText.gameObject.SetActive(false);
            IsTextActive = false;
        }
        IEnumerator RefreshUIButton()
        {
            AirSupportUIButton.enabled = false;
            AirSupportUIButton.GetComponent<Image>().color = DeactivatedButtonColor;
            yield return new WaitForSeconds(AirSupportButtonResetDelay);
            AirSupportUIButton.enabled = true;
            AirSupportUIButton.GetComponent<Image>().color = ActivatedButtonColor;
            CanAttack = true;
        }

        // Fisher-Yates shuffle algorithm to shuffle a list
        void Shuffle<T>(List<T> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                T temp = list[i];
                list[i] = list[j];
                list[j] = temp;
            }
        }
    }


}





//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;

//public class AirSupportCall : MonoBehaviour
//{
//    public Button AirSupportUIButton;
//    public Transform PlayerCamera; // Reference to the FPS camera
//    public float Range;
//    RaycastHit hit;
//    public GameObject JetToSpawn;
//    public float SpawnHeightFromTheHitPoint = 10f;
//    public float SpawnPointsCreationDistanceFromHitPoint = 100f;

//    public bool SpawnFromRightSideOfHitPoint = true;
//    public bool SpawnFromLeftSideOfHitPoint = true;
//    public bool SpawnFromForwardSideOfHitPoint = true;
//    public bool SpawnFromBackwardSideOfHitPoint = true;

//    public float UIRefreshTime = 3f;
//    public Color UIButtonColorWhenReady = Color.white;
//    public Color UIButtonColorWhenNotReady = Color.gray;
//    bool CanAttack = true;

//    void Start()
//    {
//        AirSupportUIButton.onClick.AddListener(DoExplosion);
//    }

//    void DoExplosion()
//    {
//        if (Physics.Raycast(PlayerCamera.transform.position, PlayerCamera.transform.forward, out hit, Range))
//        {
//            if(hit.collider != null && CanAttack == true)
//            {
//                Vector3 spawnPosition = hit.point + Vector3.up * SpawnHeightFromTheHitPoint;
//                Vector3 StoreReachCoordinate = spawnPosition;

//                // Randomly offset the spawn position within the specified distance
//                float offsetX = Random.Range(-SpawnPointsCreationDistanceFromHitPoint, SpawnPointsCreationDistanceFromHitPoint);
//                float offsetZ = Random.Range(-SpawnPointsCreationDistanceFromHitPoint, SpawnPointsCreationDistanceFromHitPoint);
//                spawnPosition += new Vector3(offsetX, 0f, offsetZ);

//                // Calculate the direction vector from jet to spawn point
//                Vector3 lookDirection = hit.point - spawnPosition;
//                lookDirection.y = 0f; // Ignore vertical component

//                // Rotate the jet to face the spawn point horizontally
//                Quaternion horizontalRotation = Quaternion.LookRotation(lookDirection);
//                Quaternion jetRotation = Quaternion.Euler(0f, horizontalRotation.eulerAngles.y, 0f); // Only rotate horizontally

//                // Spawn the jet at the calculated position and rotation
//                GameObject jet = Instantiate(JetToSpawn, spawnPosition, jetRotation);

//                // Store the position as (X, NewY, Z)
//                float newX = spawnPosition.x;
//                float newZ = spawnPosition.z;
//                float newY = spawnPosition.y; // The height after incrementing by SpawnHeightFromTheHitPoint

//                jet.GetComponent<JetController>().SetDestination(StoreReachCoordinate);
//                Debug.Log("Spawned jet at: (" + newX + ", " + newY + ", " + newZ + ")");
//                StartCoroutine(RefreshUIButton());
//                CanAttack = false;
//            }
//        }
//    }
//    IEnumerator RefreshUIButton()
//    {
//        AirSupportUIButton.enabled = false;
//        AirSupportUIButton.GetComponent<Image>().color = UIButtonColorWhenNotReady;
//        yield return new WaitForSeconds(UIRefreshTime);
//        AirSupportUIButton.enabled = true;
//        AirSupportUIButton.GetComponent<Image>().color = UIButtonColorWhenReady;
//        CanAttack = true;
//    }
//}
