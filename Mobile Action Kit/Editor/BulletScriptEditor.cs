using UnityEditor;
using UnityEngine;

namespace MobileActionKit
{
    [CustomEditor(typeof(BulletScript))]
    public class BulletScriptEditor : Editor
    {
        public SerializedProperty ScriptInfo;
        public SerializedProperty UseRigidbody;
        public SerializedProperty AffectedLayers;
        public SerializedProperty TimeToDeactivate;
        public SerializedProperty BulletSpeed;
        public SerializedProperty DamageToTarget;
        public SerializedProperty TrajectoryDeclineSpeed;
        public SerializedProperty EnableGuidedProjectile;
        public SerializedProperty DelayTrailRendering;
        public SerializedProperty ImpactEffectName;

        public SerializedProperty SoundManagerScript;

        public SerializedProperty AddBulletForce;
        public SerializedProperty AIMinBulletImpactForce;
        public SerializedProperty AIMaxBulletImpactForce;
        public SerializedProperty NonAIBulletImpactForce;
        public SerializedProperty RadiusToApplyForce;

        public SerializedProperty PlayerBulletImpactForceOptions;

        public SerializedProperty PlayerBulletImpactForce;

        //public SerializedProperty UseBulletSpread ;
        //public SerializedProperty BulletSpreadXRotationValue ;
        //public SerializedProperty BulletSpreadYRotationValue ;

        BulletScript st;

        void OnEnable()
        {
            // Setup the SerializedProperties

            ScriptInfo = serializedObject.FindProperty("ScriptInfo");
            UseRigidbody = serializedObject.FindProperty("UseRigidbody");
            TimeToDeactivate = serializedObject.FindProperty("TimeToDeactivate");
            BulletSpeed = serializedObject.FindProperty("BulletSpeed");
            DamageToTarget = serializedObject.FindProperty("DamageToTarget");
            TrajectoryDeclineSpeed = serializedObject.FindProperty("TrajectoryDeclineSpeed");
            DelayTrailRendering = serializedObject.FindProperty("DelayTrailRendering");

            AffectedLayers = serializedObject.FindProperty("AffectedLayers");
            EnableGuidedProjectile = serializedObject.FindProperty("EnableGuidedProjectile");
            ImpactEffectName = serializedObject.FindProperty("ImpactEffectName");

            SoundManagerScript = serializedObject.FindProperty("SoundManagerScript");

            AddBulletForce = serializedObject.FindProperty("AddBulletForce");
            AIMinBulletImpactForce = serializedObject.FindProperty("AIMinBulletImpactForce");
            AIMaxBulletImpactForce = serializedObject.FindProperty("AIMaxBulletImpactForce");

            NonAIBulletImpactForce = serializedObject.FindProperty("NonAIBulletImpactForce");
            RadiusToApplyForce = serializedObject.FindProperty("RadiusToApplyForce");

            PlayerBulletImpactForceOptions = serializedObject.FindProperty("PlayerBulletImpactForceOptions");

            PlayerBulletImpactForce = serializedObject.FindProperty("PlayerBulletImpactForce");
            //UseBulletSpread = serializedObject.FindProperty("UseBulletSpread");
            //BulletSpreadXRotationValue = serializedObject.FindProperty("BulletSpreadXRotationValue");
            //BulletSpreadYRotationValue = serializedObject.FindProperty("BulletSpreadYRotationValue");
            st = target as BulletScript;
        }
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            // EditorGUILayout.PropertyField(UseBulletSpread);

            EditorGUILayout.PropertyField(ScriptInfo);
            EditorGUILayout.PropertyField(UseRigidbody);

            EditorGUILayout.PropertyField(AffectedLayers);
            EditorGUILayout.PropertyField(TimeToDeactivate);
            EditorGUILayout.PropertyField(BulletSpeed);
            EditorGUILayout.PropertyField(DamageToTarget);
            EditorGUILayout.PropertyField(TrajectoryDeclineSpeed);
            EditorGUILayout.PropertyField(DelayTrailRendering);
            EditorGUILayout.PropertyField(EnableGuidedProjectile);
            EditorGUILayout.PropertyField(ImpactEffectName);

            EditorGUILayout.PropertyField(SoundManagerScript);

            EditorGUILayout.PropertyField(AddBulletForce);

            if (st.AddBulletForce == true)
            {
                EditorGUILayout.PropertyField(AIMinBulletImpactForce);
                EditorGUILayout.PropertyField(AIMaxBulletImpactForce);


                EditorGUILayout.PropertyField(NonAIBulletImpactForce);
                EditorGUILayout.PropertyField(RadiusToApplyForce);
            }

            EditorGUILayout.PropertyField(PlayerBulletImpactForceOptions);

            if (st.PlayerBulletImpactForceOptions == BulletScript.PlayerForce.ApplyForceToPlayerRigidbody || st.PlayerBulletImpactForceOptions == BulletScript.PlayerForce.ApplyForceToPlayerRigidbodyAndShakePlayerCamera)
            {
                EditorGUILayout.PropertyField(PlayerBulletImpactForce);
            }


            //else
            //{      
            //    EditorGUILayout.PropertyField(BulletSpeed);
            //    EditorGUILayout.PropertyField(DamageToTarget);
            //    EditorGUILayout.PropertyField(DelayTrailRendering);

            //    //   EditorGUILayout.PropertyField(DamageToWeakpoint);
            //}

            //if(st.UseBulletSpread == true)
            //{
            //    EditorGUILayout.PropertyField(BulletSpreadXRotationValue);
            //    EditorGUILayout.PropertyField(BulletSpreadYRotationValue);
            //}

            serializedObject.ApplyModifiedProperties();
        }
    }
}