using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MobileActionKit
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class FovVisualisation : MonoBehaviour
    {

        [TextArea]
        [ContextMenuItem("Reset Description", "ResettingDescription")]
        public string ScriptInfo = "This script is a helper script for the developers to visualise the field of view in scene and game view.";

        private MeshFilter meshFilter;

        [Tooltip("Drag and drop FindEnemies script located in the root gameObject of this Ai agent.")]
        public FindEnemies FindEnemiesScript;

        [Tooltip("Angle of the mesh field of view. This value should be the same as you have entered in the 'Core Ai behaviour Script' field name 'Field of view'.")]
        private float FovValue = 60f;

        [Tooltip("The Distance at which the mesh will be created from the current position. This value should be the same as you have entered in the 'Core Ai behaviour Script' field name 'Detection Radius'.")]
        private float FovViewDistance = 100f;

        [HideInInspector]
        public int FovResolution = 100;

        [Tooltip("If enabled a mesh will be created procedurely using the values above.")]
        public bool EnableDebugFOV = true;
        [Tooltip("A mesh will be created in the form of rectangle.")]
        public bool CreateAFovRectangle = false;
        [Tooltip("If enabled mesh will be shown in the game view as well.")]
        public bool showInGameView = false;

        //public bool ClampxAxis = true;
        //public float LookUpLimit = -30f;
        //public float LookDownLimit = 0f;

        float NewFovValue;
        float NewFovDistance;

        public void ResettingDescription()
        {
            ScriptInfo = "This script is a helper script for the developers to visualise the field of view in scene and game view.";
        }
        private void Awake()
        {
            FovValue = FindEnemiesScript.FieldOfViewValue;
            FovViewDistance = FindEnemiesScript.DetectionRadius;
            //if (CheckForDynamicsObjects != null)
            //{
            //    CheckForDynamicsObjects.transform.parent = transform.root;
            //}

            meshFilter = GetComponent<MeshFilter>();

            NewFovDistance = FovViewDistance * 10;
            NewFovValue = FovValue / 2;
            transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            if (EnableDebugFOV == true)
            {
                if (CreateAFovRectangle == true)
                {
                    FovResolution = 4;
                    Vector3 rot = transform.localEulerAngles;
                    rot.z = 45f;
                    transform.localEulerAngles = rot;
                }
                else
                {
                    FovResolution = 100;
                }

                if (showInGameView == false)
                {
                    transform.gameObject.layer = LayerMask.NameToLayer("Icon");
                }
            }
        }
        void Start()
        {
            if (EnableDebugFOV == true)
            {
                meshFilter.mesh = CreateConeForFieldOfView(NewFovValue, NewFovDistance, FovResolution);
            }
        }
        //private void Update()
        //{
        //    if(ClampxAxis == true)
        //    {
        //        Vector3 rot = transform.localEulerAngles;
        //        rot.x = ClampAngle.ClampAngles(rot.x, LookUpLimit, LookDownLimit);
        //        rot.y = 0f;
        //        rot.z = 0f;
        //        transform.localEulerAngles = rot;
        //    }
        //}
        public Mesh CreateConeForFieldOfView(float FOVAngle, float FOVdistance, int FOVResolution = 30)
        {
            Vector3[] verts = new Vector3[FOVResolution + 1];
            Vector3[] normals = new Vector3[verts.Length];
            int[] tris = new int[FOVResolution * 3];
            Vector3 a = Quaternion.Euler(-FOVAngle, 0, 0) * Vector3.forward * FOVdistance;
            Vector3 n = Quaternion.Euler(-FOVAngle, 0, 0) * Vector3.up;
            Quaternion step = Quaternion.Euler(0, 0, 360f / FOVResolution);
            verts[0] = Vector3.zero;
            normals[0] = Vector3.back;
            for (int i = 0; i < FOVResolution; i++)
            {
                normals[i + 1] = n;
                verts[i + 1] = a;
                a = step * a;
                n = step * n;
                tris[i * 3] = 0;
                tris[i * 3 + 1] = (i + 1) % FOVResolution + 1;
                tris[i * 3 + 2] = i + 1;
            }
            Mesh m = new Mesh();
            m.vertices = verts;
            m.normals = normals;
            m.triangles = tris;
            m.RecalculateBounds();
            return m;
        }
    }
}
