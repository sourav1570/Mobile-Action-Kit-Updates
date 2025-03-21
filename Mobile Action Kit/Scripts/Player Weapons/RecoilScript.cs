using UnityEngine;

namespace MobileActionKit
{
    public class RecoilScript : MonoBehaviour
    {
        public static RecoilScript instance;

        [TextArea]
        public string ScriptInfo = "This Script provides random behavior for player weapon recoil, including speed and shake variations.";

        [Space(10)]
        [Tooltip("Transform that determines the recoil position.")]
        public Transform RecoilPosition;

        [Tooltip("Transform that determines the recoil rotation point.")]
        public Transform RotationPoint;

        [Header("Player Stand Recoil Values")]
        [Tooltip("Minimum speed for positional recoil when standing.")]
        public float MinStandPositionalRecoilSpeed = 8f;

        [Tooltip("Maximum speed for positional recoil when standing.")]
        public float MaxStandPositionalRecoilSpeed = 8f;

        [Tooltip("Minimum speed for rotational recoil when standing.")]
        public float MinStandRotationalRecoilSpeed = 8f;

        [Tooltip("Maximum speed for rotational recoil when standing.")]
        public float MaxStandRotationalRecoilSpeed = 8f;

        [Tooltip("Minimum rotation values for hip fire recoil when standing.")]
        public Vector3 MinStandHipRecoilRotation;

        [Tooltip("Maximum rotation values for hip fire recoil when standing.")]
        public Vector3 MaxStandHipRecoilRotation;

        [Tooltip("Minimum positional kickback for hip fire recoil when standing.")]
        public Vector3 MinStandHipRecoilKickBack;

        [Tooltip("Maximum positional kickback for hip fire recoil when standing.")]
        public Vector3 MaxStandHipRecoilKickBack;

        [Tooltip("Minimum rotation values for aimed recoil when standing.")]
        public Vector3 MinStandAimedRecoilRotation;

        [Tooltip("Maximum rotation values for aimed recoil when standing.")]
        public Vector3 MaxStandAimedRecoilRotation;

        [Tooltip("Minimum positional kickback for aimed recoil when standing.")]
        public Vector3 MinStandAimedRecoilKickBack;

        [Tooltip("Maximum positional kickback for aimed recoil when standing.")]
        public Vector3 MaxStandAimedRecoilKickBack;

        [Header("Player Crouch Recoil Values")]
        [Tooltip("Reference to the player's crouch script.")]
        public Crouch CrouchScript;

        [Tooltip("Minimum speed for positional recoil when crouching.")]
        public float MinCrouchPositionalRecoilSpeed = 8f;

        [Tooltip("Maximum speed for positional recoil when crouching.")]
        public float MaxCrouchPositionalRecoilSpeed = 8f;

        [Tooltip("Minimum speed for rotational recoil when crouching.")]
        public float MinCrouchRotationalRecoilSpeed = 8f;

        [Tooltip("Maximum speed for rotational recoil when crouching.")]
        public float MaxCrouchRotationalRecoilSpeed = 8f;

        [Tooltip("Minimum rotation values for hip fire recoil when crouching.")]
        public Vector3 MinCrouchHipRecoilRotation;

        [Tooltip("Maximum rotation values for hip fire recoil when crouching.")]
        public Vector3 MaxCrouchHipRecoilRotation;

        [Tooltip("Minimum positional kickback for hip fire recoil when crouching.")]
        public Vector3 MinCrouchHipRecoilKickBack;

        [Tooltip("Maximum positional kickback for hip fire recoil when crouching.")]
        public Vector3 MaxCrouchHipRecoilKickBack;

        [Tooltip("Minimum rotation values for aimed recoil when crouching.")]
        public Vector3 MinCrouchAimedRecoilRotation;

        [Tooltip("Maximum rotation values for aimed recoil when crouching.")]
        public Vector3 MaxCrouchAimedRecoilRotation;

        [Tooltip("Minimum positional kickback for aimed recoil when crouching.")]
        public Vector3 MinCrouchAimedRecoilKickBack;

        [Tooltip("Maximum positional kickback for aimed recoil when crouching.")]
        public Vector3 MaxCrouchAimedRecoilKickBack;

        private Vector3 currentRotationalRecoil;
        private Vector3 currentPositionalRecoil;

        private Vector3 defaultRotation;
        private Vector3 defaultPosition;

        void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }

            // Save default positions
            defaultRotation = RotationPoint.localRotation.eulerAngles;
            defaultPosition = RecoilPosition.localPosition;
        }

        void FixedUpdate()
        {
            if (CrouchScript.IsCrouching)
            {
                SmoothReturn(
                    MinCrouchRotationalRecoilSpeed,
                    MaxCrouchRotationalRecoilSpeed,
                    MinCrouchPositionalRecoilSpeed,
                    MaxCrouchPositionalRecoilSpeed
                );
            }
            else
            {
                SmoothReturn(
                    MinStandRotationalRecoilSpeed,
                    MaxStandRotationalRecoilSpeed,
                    MinStandPositionalRecoilSpeed,
                    MaxStandPositionalRecoilSpeed
                );
            }
        }

        //private void SmoothReturn(float minRotSpeed, float maxRotSpeed, float minPosSpeed, float maxPosSpeed)
        //{
        //    float rotationalReturnSpeed = Random.Range(minRotSpeed, maxRotSpeed);
        //    float positionalReturnSpeed = Random.Range(minPosSpeed, maxPosSpeed);

        //    currentRotationalRecoil = Vector3.Lerp(currentRotationalRecoil, Vector3.zero, rotationalReturnSpeed * Time.deltaTime);
        //    currentPositionalRecoil = Vector3.Lerp(currentPositionalRecoil, Vector3.zero, positionalReturnSpeed * Time.deltaTime);

        //    RotationPoint.localRotation = Quaternion.Euler(defaultRotation + currentRotationalRecoil);
        //    RecoilPosition.localPosition = defaultPosition + currentPositionalRecoil;
        //}
        private void SmoothReturn(float minRotSpeed, float maxRotSpeed, float minPosSpeed, float maxPosSpeed)
        {
            float rotationalReturnSpeed = Random.Range(minRotSpeed, maxRotSpeed);
            float positionalReturnSpeed = Random.Range(minPosSpeed, maxPosSpeed);

            // Smoothly return rotational and positional recoil to zero
            currentRotationalRecoil = Vector3.Lerp(currentRotationalRecoil, Vector3.zero, rotationalReturnSpeed * Time.deltaTime);
            currentPositionalRecoil = Vector3.Lerp(currentPositionalRecoil, Vector3.zero, positionalReturnSpeed * Time.deltaTime);

            // Explicitly clamp very small positional recoil values to avoid floating-point drift
            if (currentPositionalRecoil.magnitude < 0.001f)
            {
                currentPositionalRecoil = Vector3.zero;
            }

            // Apply the recoil values to the transforms
            RotationPoint.localRotation = Quaternion.Euler(defaultRotation + currentRotationalRecoil);
            RecoilPosition.localPosition = defaultPosition + currentPositionalRecoil;
        }


        public void Recoil(bool aiming)
        {
            if (CrouchScript.IsCrouching)
            {
                ApplyRecoil(
                    aiming,
                    MinCrouchAimedRecoilRotation, MaxCrouchAimedRecoilRotation,
                    MinCrouchAimedRecoilKickBack, MaxCrouchAimedRecoilKickBack,
                    MinCrouchHipRecoilRotation, MaxCrouchHipRecoilRotation,
                    MinCrouchHipRecoilKickBack, MaxCrouchHipRecoilKickBack
                );
            }
            else
            {
                ApplyRecoil(
                    aiming,
                    MinStandAimedRecoilRotation, MaxStandAimedRecoilRotation,
                    MinStandAimedRecoilKickBack, MaxStandAimedRecoilKickBack,
                    MinStandHipRecoilRotation, MaxStandHipRecoilRotation,
                    MinStandHipRecoilKickBack, MaxStandHipRecoilKickBack
                );
            }
        }

        //private void ApplyRecoil(bool aiming,
        //                         Vector3 minAimedRot, Vector3 maxAimedRot,
        //                         Vector3 minAimedKick, Vector3 maxAimedKick,
        //                         Vector3 minHipRot, Vector3 maxHipRot,
        //                         Vector3 minHipKick, Vector3 maxHipKick)
        //{
        //    Vector3 rotationalRecoil;
        //    Vector3 positionalRecoil;

        //    if (aiming)
        //    {
        //        rotationalRecoil = new Vector3(
        //            Random.Range(minAimedRot.x, maxAimedRot.x),
        //            Random.Range(minAimedRot.y, maxAimedRot.y),
        //            Random.Range(minAimedRot.z, maxAimedRot.z)
        //        );
        //        positionalRecoil = new Vector3(
        //            Random.Range(minAimedKick.x, maxAimedKick.x),
        //            Random.Range(minAimedKick.y, maxAimedKick.y),
        //            Random.Range(minAimedKick.z, maxAimedKick.z)
        //        );
        //    }
        //    else
        //    {
        //        rotationalRecoil = new Vector3(
        //            Random.Range(minHipRot.x, maxHipRot.x),
        //            Random.Range(minHipRot.y, maxHipRot.y),
        //            Random.Range(minHipRot.z, maxHipRot.z)
        //        );
        //        positionalRecoil = new Vector3(
        //            Random.Range(minHipKick.x, maxHipKick.x),
        //            Random.Range(minHipKick.y, maxHipKick.y),
        //            Random.Range(minHipKick.z, maxHipKick.z)
        //        );
        //    }

        //    currentRotationalRecoil += rotationalRecoil;
        //    currentPositionalRecoil += positionalRecoil;
        //}
        private void ApplyRecoil(bool aiming,
                         Vector3 minAimedRot, Vector3 maxAimedRot,
                         Vector3 minAimedKick, Vector3 maxAimedKick,
                         Vector3 minHipRot, Vector3 maxHipRot,
                         Vector3 minHipKick, Vector3 maxHipKick)
        {
            Vector3 rotationalRecoil;
            Vector3 positionalRecoil;

            if (aiming)
            {
                rotationalRecoil = new Vector3(
                    Random.Range(minAimedRot.x, maxAimedRot.x),
                    Random.Range(minAimedRot.y, maxAimedRot.y),
                    Random.Range(minAimedRot.z, maxAimedRot.z)
                );
                positionalRecoil = new Vector3(
                    Random.Range(minAimedKick.x, maxAimedKick.x),
                    Random.Range(minAimedKick.y, maxAimedKick.y),
                    Random.Range(minAimedKick.z, maxAimedKick.z)
                );
            }
            else
            {
                rotationalRecoil = new Vector3(
                    Random.Range(minHipRot.x, maxHipRot.x),
                    Random.Range(minHipRot.y, maxHipRot.y),
                    Random.Range(minHipRot.z, maxHipRot.z)
                );
                positionalRecoil = new Vector3(
                    Random.Range(minHipKick.x, maxHipKick.x),
                    Random.Range(minHipKick.y, maxHipKick.y),
                    Random.Range(minHipKick.z, maxHipKick.z)
                );
            }

            // Add the recoil, but clamp it to prevent exceeding max values
            currentRotationalRecoil += rotationalRecoil;
            currentPositionalRecoil += positionalRecoil;

            // Clamp positional recoil to the min and max values
            //currentPositionalRecoil.x = Mathf.Clamp(currentPositionalRecoil.x, minHipKick.x, maxHipKick.x);
            //currentPositionalRecoil.y = Mathf.Clamp(currentPositionalRecoil.y, minHipKick.y, maxHipKick.y);
            //currentPositionalRecoil.z = Mathf.Clamp(currentPositionalRecoil.z, minHipKick.z, maxHipKick.z);
        }

    }
}




//using UnityEngine;

//namespace MobileActionKit
//{
//    public class RecoilScript : MonoBehaviour
//    {
//        public static RecoilScript instance;

//        [TextArea]
//        [ContextMenuItem("Reset Description", "ResettingDescription")]
//        public string ScriptInfo = "This Script is one of the variation for Player Weapon recoil and provides a random behaviour for weapon recoil . It Provides " +
//                "the random speed and random shakes ";
//        [Space(10)]

//        public Transform RecoilPosition;
//        public Transform RotationPoint;

//        [Header("Player Stand Recoil Values")]
//        public float MinStandPositionalRecoilSpeed = 8f;
//        public float MaxStandPositionalRecoilSpeed = 8f;

//        public float MinStandRotationalRecoilSpeed = 8f;
//        public float MaxStandRotationalRecoilSpeed = 8f;

//        public float MinStandPositionalReturnSpeed = 18f;
//        public float MaxStandPositionalReturnSpeed = 18f;

//        public float MinStandRotationalReturnSpeed = 38f;
//        public float MaxStandRotationalReturnSpeed = 38f;


//        public Vector3 MinStandHipRecoilRotation = new Vector3(10, 5, 7);
//        public Vector3 MaxStandHipRecoilRotation = new Vector3(10, 5, 7);

//        public Vector3 MinStandHipRecoilKickBack = new Vector3(0.015f, 0f, -0.2f);
//        public Vector3 MaxStandHipRecoilKickBack = new Vector3(0.015f, 0f, -0.2f);

//        public Vector3 MinStandAimedRecoilRotation = new Vector3(10, 4, 6);
//        public Vector3 MaxStandAimedRecoilRotation = new Vector3(10, 4, 6);

//        public Vector3 MinStandAimedRecoilKickBack = new Vector3(0.015f, 0f, -0.2f);
//        public Vector3 MaxStandAimedRecoilKickBack = new Vector3(0.015f, 0f, -0.2f);


//        [Header("Player Crouch Recoil Values")]

//        public Crouch CrouchScript;

//        public float MinCrouchPositionalRecoilSpeed = 8f;
//        public float MaxCrouchPositionalRecoilSpeed = 8f;

//        public float MinCrouchRotationalRecoilSpeed = 8f;
//        public float MaxCrouchRotationalRecoilSpeed = 8f;

//        public float MinCrouchPositionalReturnSpeed = 18f;
//        public float MaxCrouchPositionalReturnSpeed = 18f;

//        public float MinCrouchRotationalReturnSpeed = 38f;
//        public float MaxCrouchRotationalReturnSpeed = 38f;

//        public Vector3 MinCrouchHipRecoilRotation = new Vector3(10, 5, 7);
//        public Vector3 MaxCrouchHipRecoilRotation = new Vector3(10, 5, 7);

//        public Vector3 MinCrouchHipRecoilKickBack = new Vector3(0.015f, 0f, -0.2f);
//        public Vector3 MaxCrouchHipRecoilKickBack = new Vector3(0.015f, 0f, -0.2f);

//        public Vector3 MinCrouchAimedRecoilRotation = new Vector3(10, 4, 6);
//        public Vector3 MaxCrouchAimedRecoilRotation = new Vector3(10, 4, 6);

//        public Vector3 MinCrouchAimedRecoilKickBack = new Vector3(0.015f, 0f, -0.2f);
//        public Vector3 MaxCrouchAimedRecoilKickBack = new Vector3(0.015f, 0f, -0.2f);

//        Vector3 rotationalRecoil;
//        Vector3 positionalRecoil;
//        Vector3 rot;

//        Vector3 CrouchrotationalRecoil;
//        Vector3 CrouchpositionalRecoil;
//        Vector3 Crouchrot;
//        public void ResettingDescription()
//        {
//            ScriptInfo = "This Script is one of the variation for Player Weapon recoil and provides a random behaviour for weapon recoil . It Provides " +
//                "the random speed and random shakes ";
//        }
//        void Awake()
//        {
//            if (instance == null)
//            {
//                instance = this;
//            }
//        }
//        void FixedUpdate()
//        {
//            if (CrouchScript.IsCrouching == true)
//            {
//                float RandomiseCrouchRotationalReturnSpeed = Random.Range(MinCrouchRotationalReturnSpeed, MaxCrouchRotationalReturnSpeed);
//                float RandomiseCrouchPositionalReturnSpeed = Random.Range(MinCrouchPositionalReturnSpeed, MaxCrouchPositionalReturnSpeed);
//                CrouchrotationalRecoil = Vector3.Lerp(CrouchrotationalRecoil, Vector3.zero, RandomiseCrouchRotationalReturnSpeed * Time.deltaTime);
//                CrouchpositionalRecoil = Vector3.Lerp(CrouchpositionalRecoil, Vector3.zero, RandomiseCrouchPositionalReturnSpeed * Time.deltaTime);


//                float RandomiseCrouchRotationalRecoilSpeed = Random.Range(MinCrouchRotationalRecoilSpeed, MaxCrouchRotationalRecoilSpeed);
//                float RandomiseCrouchPositionalRecoilSpeed = Random.Range(MinCrouchPositionalRecoilSpeed, MaxCrouchPositionalRecoilSpeed);
//                RecoilPosition.localPosition = Vector3.Slerp(RecoilPosition.localPosition, CrouchpositionalRecoil, RandomiseCrouchPositionalRecoilSpeed * Time.deltaTime);
//                Crouchrot = Vector3.Slerp(Crouchrot, CrouchrotationalRecoil, RandomiseCrouchRotationalRecoilSpeed * Time.fixedDeltaTime);
//                RotationPoint.localRotation = Quaternion.Euler(Crouchrot);
//            }
//            else
//            {
//                float RandomiseStandRotationalReturnSpeed = Random.Range(MinStandRotationalReturnSpeed, MaxStandRotationalReturnSpeed);
//                float RandomiseStandPositionalReturnSpeed = Random.Range(MinStandPositionalReturnSpeed, MaxStandPositionalReturnSpeed);
//                rotationalRecoil = Vector3.Lerp(rotationalRecoil, Vector3.zero, RandomiseStandRotationalReturnSpeed * Time.deltaTime);
//                positionalRecoil = Vector3.Lerp(positionalRecoil, Vector3.zero, RandomiseStandPositionalReturnSpeed * Time.deltaTime);

//                float RandomisePositionalRecoil = Random.Range(MinStandPositionalRecoilSpeed, MaxStandPositionalRecoilSpeed);
//                RecoilPosition.localPosition = Vector3.Slerp(RecoilPosition.localPosition, positionalRecoil, RandomisePositionalRecoil * Time.deltaTime);

//                float RandomiseRotationalRecoil = Random.Range(MinStandRotationalRecoilSpeed, MaxStandRotationalRecoilSpeed);
//                rot = Vector3.Slerp(rot, rotationalRecoil, RandomiseRotationalRecoil * Time.fixedDeltaTime);
//                RotationPoint.localRotation = Quaternion.Euler(rot);
//            }
//        }
//        public void Recoil(bool Aiming)
//        {
//            if (CrouchScript.IsCrouching == true)
//            {
//                if (Aiming)
//                {
//                    CrouchrotationalRecoil = new Vector3(Random.Range(MaxCrouchAimedRecoilRotation.x, MinCrouchAimedRecoilRotation.x), Random.Range(MaxCrouchAimedRecoilRotation.y, MinCrouchAimedRecoilRotation.y), Random.Range(MaxCrouchAimedRecoilRotation.z, MaxCrouchAimedRecoilRotation.z));
//                    CrouchpositionalRecoil = new Vector3(Random.Range(MaxCrouchAimedRecoilKickBack.x, MinCrouchAimedRecoilKickBack.x), Random.Range(MaxCrouchAimedRecoilKickBack.y, MinCrouchAimedRecoilKickBack.y), Random.Range(MaxCrouchAimedRecoilKickBack.z, MinCrouchAimedRecoilKickBack.z));
//                }
//                else
//                {
//                    CrouchrotationalRecoil = new Vector3(Random.Range(MaxCrouchHipRecoilRotation.x, MinCrouchHipRecoilRotation.x), Random.Range(MaxCrouchHipRecoilRotation.y, MinCrouchHipRecoilRotation.y), Random.Range(MaxCrouchHipRecoilRotation.z, MinCrouchHipRecoilRotation.z));
//                    CrouchpositionalRecoil = new Vector3(Random.Range(MaxCrouchHipRecoilKickBack.x, MinCrouchHipRecoilKickBack.x), Random.Range(MaxCrouchHipRecoilKickBack.y, MinCrouchHipRecoilKickBack.y), Random.Range(MaxCrouchHipRecoilKickBack.z, MinCrouchHipRecoilKickBack.z));
//                }
//            }
//            else
//            {
//                if (Aiming)
//                {
//                    rotationalRecoil = new Vector3(Random.Range(MaxStandAimedRecoilRotation.x, MinStandAimedRecoilRotation.x), Random.Range(MaxStandAimedRecoilRotation.y, MinStandAimedRecoilRotation.y), Random.Range(MaxStandAimedRecoilRotation.z, MinStandAimedRecoilRotation.z));
//                    positionalRecoil = new Vector3(Random.Range(MaxStandAimedRecoilKickBack.x, MinStandAimedRecoilKickBack.x), Random.Range(MaxStandAimedRecoilKickBack.y, MinStandAimedRecoilKickBack.y), Random.Range(MaxStandAimedRecoilKickBack.z, MinStandAimedRecoilKickBack.z));
//                }
//                else
//                {

//                    rotationalRecoil = new Vector3(Random.Range(MaxStandHipRecoilRotation.x, MinStandHipRecoilRotation.x), Random.Range(MaxStandHipRecoilRotation.y, MinStandHipRecoilRotation.y), Random.Range(MaxStandHipRecoilRotation.z, MinStandHipRecoilRotation.z));
//                    positionalRecoil = new Vector3(Random.Range(MaxStandHipRecoilKickBack.x, MinStandHipRecoilKickBack.x), Random.Range(MaxStandHipRecoilKickBack.y, MinStandHipRecoilKickBack.y), Random.Range(MaxStandHipRecoilKickBack.z, MinStandHipRecoilKickBack.z));
//                }
//            }
//        }
//    }
//}



//public void Recoil(bool Aiming)
//{
//    if (CrouchScript.IsCrouching == true)
//    {
//        if (Aiming)
//        {

//            CrouchrotationalRecoil += new Vector3(Random.Range(-MaxCrouchAimedRecoilRotation.x, -MinCrouchAimedRecoilRotation.x), Random.Range(-MaxCrouchAimedRecoilRotation.y, -MinCrouchAimedRecoilRotation.y), Random.Range(-MaxCrouchAimedRecoilRotation.z, MaxCrouchAimedRecoilRotation.z));
//            CrouchpositionalRecoil += new Vector3(Random.Range(-MaxCrouchAimedRecoilKickBack.x, -MinCrouchAimedRecoilKickBack.x), Random.Range(-MaxCrouchAimedRecoilKickBack.y, -MinCrouchAimedRecoilKickBack.y), Random.Range(-MaxCrouchAimedRecoilKickBack.z, -MinCrouchAimedRecoilKickBack.z));

//        }
//        else
//        {

//            CrouchrotationalRecoil += new Vector3(Random.Range(-MaxCrouchHipRecoilRotation.x, -MinCrouchHipRecoilRotation.x), Random.Range(-MaxCrouchHipRecoilRotation.y, -MinCrouchHipRecoilRotation.y), Random.Range(-MaxCrouchHipRecoilRotation.z, -MaxCrouchHipRecoilRotation.z));
//            CrouchpositionalRecoil += new Vector3(Random.Range(-MaxCrouchHipRecoilKickBack.x, -MinCrouchHipRecoilKickBack.x), Random.Range(-MaxCrouchHipRecoilKickBack.y, -MinCrouchHipRecoilKickBack.y), Random.Range(-MaxCrouchHipRecoilKickBack.z, -MinCrouchHipRecoilKickBack.z));

//        }
//    }
//    else
//    {
//        if (Aiming)
//        {

//            rotationalRecoil += new Vector3(Random.Range(-MaxStandAimedRecoilRotation.x, -MinStandAimedRecoilRotation.x), Random.Range(-MaxStandAimedRecoilRotation.y, -MinStandAimedRecoilRotation.y), Random.Range(-MaxStandAimedRecoilRotation.z, -MinStandAimedRecoilRotation.z));
//            positionalRecoil += new Vector3(Random.Range(-MaxStandAimedRecoilKickBack.x, -MinStandAimedRecoilKickBack.x), Random.Range(-MaxStandAimedRecoilKickBack.y, -MinStandAimedRecoilKickBack.y), Random.Range(-MaxStandAimedRecoilKickBack.z, -MinStandAimedRecoilKickBack.z));

//        }
//        else
//        {

//            rotationalRecoil += new Vector3(Random.Range(-MaxStandHipRecoilRotation.x, -MinStandHipRecoilRotation.x), Random.Range(-MaxStandHipRecoilRotation.y, -MinStandHipRecoilRotation.y), Random.Range(-MaxStandHipRecoilRotation.z, -MaxStandHipRecoilRotation.z));
//            positionalRecoil += new Vector3(Random.Range(-MaxStandHipRecoilKickBack.x, -MinStandHipRecoilKickBack.x), Random.Range(-MaxStandHipRecoilKickBack.y, -MinStandHipRecoilKickBack.y), Random.Range(-MaxStandHipRecoilKickBack.z, -MinStandHipRecoilKickBack.z));



//        }
//    }