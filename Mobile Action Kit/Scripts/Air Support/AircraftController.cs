using System.Collections;
using UnityEngine;


namespace MobileActionKit
{
    public class AircraftController : MonoBehaviour
    {
        [TextArea]
        public string ScriptInfo = "This Script is responsible for plane movements and spawning shells.";

        [Tooltip("Time (in seconds) for the plane to reach its destination. Lower values make the plane faster; higher values slow it down.")]
        public float TargetReachDuration = 10f;

        [Tooltip("Multiplier to adjust the plane's forward speed after reaching the payload drop point. Set to 0 to maintain the same speed or use a positive value to increase speed.")]
        public float SpeedMultiplierAfterPayloadRelease = 0.7f;

        [Tooltip("Duration (in seconds) the plane remains active after dropping the payload.")]
        public float ActiveDurationAfterPayloadRelease = 6f;

        [Tooltip("Tilt angle (in degrees) applied to the plane on the X-axis after payload release.")]
        public float TiltAngleAfterPayloadRelease = 45f;

        [Tooltip("Time (in seconds) for the plane to rotate to the specified tilt angle after payload release.")]
        public float TiltDurationAfterPayloadRelease = 0.3f;

        [Tooltip("Minimum distance (in units) to the payload drop point before triggering the payload drop.")]
        public float DropPayloadProximity = 5f;

        [Tooltip("Number of explosives to spawn at the drop point.")]
        public int ShellsCount = 1;

        [Tooltip("Explosive prefab to instantiate for the payload.")]
        public GameObject Shells;

        [Tooltip("Shells spawn point position.")]
        public Transform ShellsSpawnPoint;

        [Tooltip("Minimum delay (in seconds) between dropping consecutive explosives.")]
        public float MinShellsDropInterval = 0.01f;

        [Tooltip("Maximum delay (in seconds) between dropping consecutive explosives.")]
        public float MaxShellsDropInterval = 0.02f;

        [Tooltip("Falling speed of the explosives.")]
        public float ShellsFallingSpeed = 30f;

        [HideInInspector]
        [Tooltip("Displays the forward speed of the plane after calculating its travel time to the payload drop point.")]
        public float DebugPlaneForwardSpeed;

        [Tooltip("Audio source for the plane's engine sounds.")]
        public AudioSource AircraftEngineSounds;



        //[Tooltip("Specify the time (in seconds) it takes for the Plane to reach the destination. " +
        //    "Tweak this value to control the speed of the Plane. Smaller values will move the Plane towards the destination faster, while larger values will move it slower.")]
        //public float TargetReachDuration = 10f;

        //[Tooltip("If the value in this field is 0, after reaching the destination (explosives drop point)," +
        //    " this script extracts the speed based on the time it took to reach the destination and applies it to the forward speed of the plane, maintaining the same speed. " +
        //    "If the value is greater than 0, the extracted speed will be multiplied by the number specified in this field, increasing the speed of the plane after it has reached the explosive drop point coordinate.")]
        //public float SpeedMultiplierAfterPayloadRelease = 0.7f; // Multiplier for jet speed after reaching destination

        //[Tooltip("[Specify the duration (in seconds) that the jet will remain active in the game after reaching the explosive drop point coordinate.")]
        //public float ActiveDurationAfterPayloadRelease = 6f; // Duration in seconds the jet remains active after reaching destination

        //[Tooltip("Specify the angle (in degrees) that the jet will be rotated on the X-axis after completing its task of dropping the bombs at the received explosive drop point coordinate.")]
        //public float TiltAngleAfterPayloadRelease = 45f; // Maximum angle of tilt on the X-axis

        //[Tooltip("Specify the time (in seconds) it will take the Jet to rotate to the angle specified in the 'TiltAngle' field after reaching the explosive drop point coordinate.")]
        //public float TiltDurationAfterPayloadRelease = 0.3f;

        //[Tooltip("Specify the closest distance (in units) that the jet must reach from the explosive drop point coordinate before starting to drop the explosives. " +
        //    "You can tweak this value based on the jet speed to ensure that explosives are dropped at the right place.")]
        //public float DropPayloadProximity = 5f;


        //[Tooltip("Enter the number of explosives to instantiate at the coordinate.")]
        //public int ShellsCount = 1;

        //[Tooltip("Drag and drop the explosive prefab from the project into this field.")]
        //public GameObject Shells;

        //[Tooltip("Drag and drop the explosive spawn point GameObject from the Jet hierarchy into this field.")]
        //public Transform ShellsSpawnPoint;

        //[Tooltip("Minimal time interval between falling shells.")]
        //public float MinShellsDropInterval = 0.01f;

        //[Tooltip("Maximal time interval between falling shells.")]
        //public float MaxShellsDropInterval = 0.02f;

        //[Tooltip("Speed of incoming shells.")]
        //public float ShellsFallingSpeed = 30f;

        private Vector3 destinationPosition;
        private bool isTilting = false;
        private float initialTiltAngle;

        //[HideInInspector] [Tooltip("This field displays the forward speed of the jet, which is the speed extracted based on the time it took the jet to reach the destination (explosives drop point).")]
        //public float DebugJetForwardSpeed;

        bool BombSpawnOnce = false;

        //public AudioSource AircraftEngineSounds;

        // Call this function from another script to set the destination
        public void SetDestination(Vector3 destination)
        {
            AircraftEngineSounds.Play();
            destinationPosition = destination;
            StartCoroutine(MoveToDestination());
        }

        private IEnumerator MoveToDestination()
        {
            float elapsedTime = 0f;
            Vector3 startingPosition = transform.position;

            while (elapsedTime < TargetReachDuration)
            {

                // Move towards the destination position
                transform.position = Vector3.Lerp(startingPosition, destinationPosition, elapsedTime / TargetReachDuration);
                elapsedTime += Time.deltaTime;
                // Calculate distance between destination position and jet position
                Vector3 distance = destinationPosition - transform.position;

                //Debug.Log(distance.magnitude);
                if (distance.magnitude <= DropPayloadProximity)
                {
                    if (BombSpawnOnce == false)
                    {
                        StartCoroutine(SpawnBombsWithDelay());
                        BombSpawnOnce = true;
                    }
                }
                yield return null;
            }

            // Ensure the jet reaches exactly the destination position
            transform.position = destinationPosition;

            // Calculate jet forward speed based on reaching destination time and multiplier
            DebugPlaneForwardSpeed = (destinationPosition - startingPosition).magnitude / TargetReachDuration;

            // Apply speed multiplier if it's greater than 0
            if (SpeedMultiplierAfterPayloadRelease > 0f)
            {
                DebugPlaneForwardSpeed *= SpeedMultiplierAfterPayloadRelease;
            }

            //initialTiltAngle = transform.rotation.eulerAngles.x; // Store the initial tilt angle

            float forwardStartTime = Time.time;
            //float tiltStartTime = Time.time;
            LeanTween.rotateX(gameObject, TiltAngleAfterPayloadRelease, TiltDurationAfterPayloadRelease); // better and efficient way 
            while (Time.time - forwardStartTime < ActiveDurationAfterPayloadRelease)
            {
                // Move the jet forward
                transform.Translate(Vector3.forward * DebugPlaneForwardSpeed * Time.deltaTime);


                

                //// Calculate tilt amount based on time elapsed
                //float tiltAmount = Mathf.Clamp01((Time.time - tiltStartTime) / TiltDurationAfterPayloadRelease) * TiltAngleAfterPayloadRelease;

                //// Apply tilt starting from the initial tilt angle
                //float finalTiltAngle = tiltAmount; // initialTiltAngle + 
                //transform.rotation = Quaternion.Euler(finalTiltAngle, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);

                yield return null;
            }

            // Deactivate the jet
            gameObject.SetActive(false);
        }

        private IEnumerator SpawnBombsWithDelay()
        {
            for (int i = 0; i < ShellsCount; i++)
            {
                GameObject bomb = Instantiate(Shells, ShellsSpawnPoint.transform.position, Shells.transform.rotation);
                Rigidbody bombRigidbody = bomb.GetComponent<Rigidbody>();
                if (bombRigidbody != null)
                {
                    // Set velocity to give the bombs downward motion
                    bombRigidbody.velocity = Vector3.down * ShellsFallingSpeed; // Adjust the downward speed as needed
                }
                float delay = Random.Range(MinShellsDropInterval, MaxShellsDropInterval); // Random delay between 0.1f and 0.3f
                yield return new WaitForSeconds(delay);
            }
        }
    }

}
//using System.Collections;
//using UnityEngine;

//public class JetController : MonoBehaviour
//{
//    public float ReachingDestinationTime = 3f;
//    public float JetForwardSpeedAfterDestinationIsReached = 5f; // Speed at which the jet moves forward after reaching destination
//    public float JetActiveDurationAfterDestinationIsReached = 5f; // Duration in seconds the jet remains active after reaching destination
//    public float tiltAngle = 45f; // Maximum angle of tilt on the X axis
//    public float ClosestDistanceToCheckBeforeDropping = 3f;
//    public float AngleCompletionTimeAfterDestinationReached = 2f;

//    public int NumberOfBombsToInstantiate = 5;
//    public GameObject Bombs;
//    public Transform BombSpawnPoint;
//    public float MinEachBombSpawnDelay = 0.1f;
//    public float MaxEachBombSpawnDelay = 0.5f;
//    public float BombFallingSpeed = 10f;

//    private Vector3 destinationPosition;
//    private bool isTilting = false;
//    private float initialTiltAngle;

//    bool BombSpawnOnce = false;

//    // Call this function from another script to set the destination
//    public void SetDestination(Vector3 destination)
//    {
//        destinationPosition = destination;
//        StartCoroutine(MoveToDestination());
//    }

//    private IEnumerator MoveToDestination()
//    {
//        float elapsedTime = 0f;
//        Vector3 startingPosition = transform.position;

//        while (elapsedTime < ReachingDestinationTime)
//        {
//            // Move towards the destination position
//            transform.position = Vector3.Lerp(startingPosition, destinationPosition, elapsedTime / ReachingDestinationTime);
//            elapsedTime += Time.deltaTime;
//            // Calculate distance between destination position and jet position
//            Vector3 distance = destinationPosition - transform.position;

//            //Debug.Log(distance.magnitude);
//            if (distance.magnitude <= ClosestDistanceToCheckBeforeDropping)
//            {
//                if (BombSpawnOnce == false)
//                {
//                    StartCoroutine(SpawnBombsWithDelay());
//                    BombSpawnOnce = true;
//                }
//            }
//            yield return null;
//        }

//        // Ensure the jet reaches exactly the destination position
//        transform.position = destinationPosition;

//        initialTiltAngle = transform.rotation.eulerAngles.x; // Store the initial tilt angle

//        float forwardStartTime = Time.time;
//        float tiltStartTime = Time.time;

//        while (Time.time - forwardStartTime < JetActiveDurationAfterDestinationIsReached)
//        {
//            // Move the jet forward
//            transform.Translate(Vector3.forward * JetForwardSpeedAfterDestinationIsReached * Time.deltaTime);

//            // Calculate tilt amount based on time elapsed
//            float tiltAmount = Mathf.Clamp01((Time.time - tiltStartTime) / AngleCompletionTimeAfterDestinationReached) * tiltAngle;

//            // Apply tilt starting from the initial tilt angle
//            float finalTiltAngle = initialTiltAngle + tiltAmount;
//            transform.rotation = Quaternion.Euler(finalTiltAngle, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);

//            yield return null;
//        }

//        // Deactivate the jet
//        gameObject.SetActive(false);
//    }
//    private IEnumerator SpawnBombsWithDelay()
//    {
//        for (int i = 0; i < NumberOfBombsToInstantiate; i++)
//        {
//            GameObject bomb = Instantiate(Bombs, BombSpawnPoint.transform.position, Quaternion.identity);
//            Rigidbody bombRigidbody = bomb.GetComponent<Rigidbody>();
//            if (bombRigidbody != null)
//            {
//                // Set velocity to give the bombs downward motion
//                bombRigidbody.velocity = Vector3.down * BombFallingSpeed; // Adjust the downward speed as needed
//            }
//            float delay = Random.Range(MinEachBombSpawnDelay, MaxEachBombSpawnDelay); // Random delay between 0.1f and 0.3f
//            yield return new WaitForSeconds(delay);
//        }
//    }
//}




//using System.Collections;
//using UnityEngine;

//public class JetController : MonoBehaviour
//{
//    public float ReachingDestinationTime = 3f;
//    public float JetForwardSpeedAfterDestinationIsReached = 5f; // Speed at which the jet moves forward after reaching destination
//    public float JetActiveDurationAfterDestinationIsReached = 5f; // Duration in seconds the jet remains active after reaching destination
//    public float tiltAngle = 45f; // Maximum angle of tilt on the X axis
//    public float ClosestDistanceToCheckBeforeDropping = 3f;
//    public float AngleCompletionTimeAfterDestinationReached = 2f;

//    public int NumberOfBombsToInstantiate = 5;
//    public GameObject Bombs;
//    public Transform BombSpawnPoint;
//    public float MinEachBombSpawnDelay = 0.1f;
//    public float MaxEachBombSpawnDelay = 0.5f;
//    public float BombFallingSpeed = 10f;

//    private Vector3 destinationPosition;
//    private bool isTilting = false;

//    bool BombSpawnOnce = false;

//    // Call this function from another script to set the destination
//    public void SetDestination(Vector3 destination)
//    {
//        destinationPosition = destination;
//        StartCoroutine(MoveToDestination());
//    }

//    private IEnumerator MoveToDestination()
//    {
//        float elapsedTime = 0f;
//        Vector3 startingPosition = transform.position;

//        while (elapsedTime < ReachingDestinationTime)
//        {
//            // Move towards the destination position
//            transform.position = Vector3.Lerp(startingPosition, destinationPosition, elapsedTime / ReachingDestinationTime);
//            elapsedTime += Time.deltaTime;
//            // Calculate distance between destination position and jet position
//            Vector3 distance = destinationPosition - transform.position;

//            //Debug.Log(distance.magnitude);
//            if (distance.magnitude <= ClosestDistanceToCheckBeforeDropping)
//            {
//                if (BombSpawnOnce == false)
//                {
//                    StartCoroutine(SpawnBombsWithDelay());
//                    BombSpawnOnce = true;
//                }
//            }
//            yield return null;
//        }

//        // Ensure the jet reaches exactly the destination position
//        transform.position = destinationPosition;

//        float forwardStartTime = Time.time;
//        float tiltStartTime = Time.time;

//        while (Time.time - forwardStartTime < JetActiveDurationAfterDestinationIsReached)
//        {
//            // Move the jet forward
//            transform.Translate(Vector3.forward * JetForwardSpeedAfterDestinationIsReached * Time.deltaTime);

//            // Calculate tilt amount based on time elapsed
//            float tiltAmount = Mathf.Clamp01((Time.time - tiltStartTime) / AngleCompletionTimeAfterDestinationReached) * tiltAngle;

//            // Apply tilt
//            transform.rotation = Quaternion.Euler(tiltAmount, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);

//            // Check if tilt has reached maximum angle
//            if (tiltAmount >= tiltAngle)
//            {
//                isTilting = false;
//            }



//            yield return null;
//        }

//        // Deactivate the jet
//        gameObject.SetActive(false);
//    }
//    private IEnumerator SpawnBombsWithDelay()
//    {
//        for (int i = 0; i < NumberOfBombsToInstantiate; i++)
//        {
//            GameObject bomb = Instantiate(Bombs, BombSpawnPoint.transform.position, Quaternion.identity);
//            Rigidbody bombRigidbody = bomb.GetComponent<Rigidbody>();
//            if (bombRigidbody != null)
//            {
//                // Set velocity to give the bombs downward motion
//                bombRigidbody.velocity = Vector3.down * BombFallingSpeed; // Adjust the downward speed as needed
//            }
//            float delay = Random.Range(MinEachBombSpawnDelay, MaxEachBombSpawnDelay); // Random delay between 0.1f and 0.3f
//            yield return new WaitForSeconds(delay);
//        }
//    }
//}




//using System.Collections;
//using UnityEngine;

//public class JetController : MonoBehaviour
//{
//    public float ReachingDestinationTime = 3f;
//    public float JetForwardSpeedAfterDestinationIsReached = 5f; // Speed at which the jet moves forward after reaching destination
//    public float JetActiveDurationAfterDestinationIsReached = 5f; // Duration in seconds the jet remains active after reaching destination
//    public float tiltAngle = 45f; // Maximum angle of tilt on the X axis

//    private Vector3 destinationPosition;
//    private bool isTilting = false;

//    // Call this function from another script to set the destination
//    public void SetDestination(Vector3 destination)
//    {
//        destinationPosition = destination;
//        StartCoroutine(MoveToDestination());
//    }

//    private IEnumerator MoveToDestination()
//    {
//        float elapsedTime = 0f;
//        Vector3 startingPosition = transform.position;

//        while (elapsedTime < ReachingDestinationTime)
//        {
//            // Move towards the destination position
//            transform.position = Vector3.Lerp(startingPosition, destinationPosition, elapsedTime / ReachingDestinationTime);
//            elapsedTime += Time.deltaTime;
//            yield return null;
//        }

//        // Ensure the jet reaches exactly the destination position
//        transform.position = destinationPosition;

//        float forwardStartTime = Time.time;
//        float tiltStartTime = Time.time;

//        while (Time.time - forwardStartTime < JetActiveDurationAfterDestinationIsReached)
//        {
//            // Move the jet forward
//            transform.Translate(Vector3.forward * JetForwardSpeedAfterDestinationIsReached * Time.deltaTime);

//            // Calculate tilt amount based on time elapsed
//            float tiltAmount = Mathf.Clamp01((Time.time - tiltStartTime) / JetActiveDurationAfterDestinationIsReached) * tiltAngle;

//            // Apply tilt
//            transform.rotation = Quaternion.Euler(tiltAmount, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);

//            // Check if tilt has reached maximum angle
//            if (tiltAmount >= tiltAngle)
//            {
//                isTilting = false;
//            }

//            yield return null;
//        }

//        // Deactivate the jet
//        gameObject.SetActive(false);
//    }
//}




//using System.Collections;
//using UnityEngine;

//public class JetController : MonoBehaviour
//{
//    public float ReachingDestinationTime = 3f;
//    public float JetForwardSpeedAfterDestinationIsReached = 5f; // Speed at which the jet moves forward after reaching destination
//    public float JetActiveDurationAfterDestinationIsReached = 5f; // Duration in seconds the jet remains active after reaching destination

//    private Vector3 destinationPosition;

//    // Call this function from another script to set the destination
//    public void SetDestination(Vector3 destination)
//    {
//        destinationPosition = destination;
//        StartCoroutine(MoveToDestination());
//    }

//    private IEnumerator MoveToDestination()
//    {
//        // Calculate the time required to reach the destination in 3 seconds

//        float elapsedTime = 0f;
//        Vector3 startingPosition = transform.position;

//        while (elapsedTime < ReachingDestinationTime)
//        {
//            // Move towards the destination position
//            transform.position = Vector3.Lerp(startingPosition, destinationPosition, elapsedTime / ReachingDestinationTime);
//            elapsedTime += Time.deltaTime;
//            yield return null;
//        }

//        // Ensure the jet reaches exactly the destination position
//        transform.position = destinationPosition;

//        float forwardStartTime = Time.time;

//        while (Time.time - forwardStartTime < JetActiveDurationAfterDestinationIsReached)
//        {
//            // Move the jet forward
//            transform.Translate(Vector3.forward * JetForwardSpeedAfterDestinationIsReached * Time.deltaTime);
//            yield return null;
//        }

//        // Deactivate the jet
//        gameObject.SetActive(false);
//    }
//}
