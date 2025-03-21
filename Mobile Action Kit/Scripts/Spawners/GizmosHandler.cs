using UnityEditor;
using UnityEngine;

namespace MobileActionKit
{
    public class GizmosHandler : MonoBehaviour
    {
#if UNITY_EDITOR

        public static void DrawWireSphereBasedOfRange(Vector3 Position, float Range, Color color)
        {
            Gizmos.color = color;
            Gizmos.DrawWireSphere(Position, Range);
        }
        public static void DrawWireCubeBasedOfRange(Vector3 Position, float Range, Color color)
        {
            Gizmos.color = color;
            Gizmos.DrawWireCube(Position, new Vector3(Range * 2, Range * 2, Range * 2));
        }
        public static void DisplayText(Vector3 Position, string Message)
        {
            Handles.Label(Position, Message);
        }
        public static void DrawColliderWireframe(Collider collider)
        {
            if (collider != null)
            {
                Handles.color = Color.green;

                if (collider is BoxCollider boxCollider)
                {
                    DrawBoxColliderWireframe(boxCollider);
                }
                else if (collider is SphereCollider sphereCollider)
                {
                    DrawSphereColliderWireframe(sphereCollider);
                }
                else if (collider is CapsuleCollider capsuleCollider)
                {
                    DrawCapsuleColliderWireframe(capsuleCollider);
                }
            }
        }

        private static void DrawBoxColliderWireframe(BoxCollider boxCollider)
        {
            Vector3 center = boxCollider.center;
            Vector3 size = boxCollider.size;

            Matrix4x4 rotationMatrix = Matrix4x4.TRS(Vector3.zero, boxCollider.transform.rotation, Vector3.one);

            Handles.matrix = Matrix4x4.TRS(boxCollider.transform.position, Quaternion.identity, Vector3.one);
            Handles.DrawWireCube(center, size);
            Handles.matrix = Matrix4x4.identity;
        }

        private static void DrawSphereColliderWireframe(SphereCollider sphereCollider)
        {
            Vector3 center = sphereCollider.center;
            float radius = sphereCollider.radius;

            Matrix4x4 rotationMatrix = Matrix4x4.TRS(Vector3.zero, sphereCollider.transform.rotation, Vector3.one);

            Handles.matrix = Matrix4x4.TRS(sphereCollider.transform.position, Quaternion.identity, Vector3.one);
            Handles.DrawWireDisc(center, Vector3.up, radius);
            Handles.DrawWireDisc(center, Vector3.forward, radius);
            Handles.DrawWireDisc(center, Vector3.right, radius);
            Handles.DrawWireArc(center, sphereCollider.transform.rotation * Vector3.up, sphereCollider.transform.rotation * Vector3.forward, 360, radius);
            Handles.matrix = Matrix4x4.identity;
        }

        private static void DrawCapsuleColliderWireframe(CapsuleCollider capsuleCollider)
        {
            Vector3 center = capsuleCollider.center;
            float radius = capsuleCollider.radius;
            float height = capsuleCollider.height;

            Matrix4x4 rotationMatrix = Matrix4x4.TRS(Vector3.zero, capsuleCollider.transform.rotation, Vector3.one);

            Handles.matrix = Matrix4x4.TRS(capsuleCollider.transform.position, Quaternion.identity, Vector3.one);

            Vector3 topSphereCenter = center + Vector3.up * (height * 0.5f - radius);
            Vector3 bottomSphereCenter = center - Vector3.up * (height * 0.5f - radius);

            Handles.DrawWireDisc(topSphereCenter, Vector3.up, radius);
            Handles.DrawWireDisc(topSphereCenter, Vector3.forward, radius);
            Handles.DrawWireDisc(topSphereCenter, Vector3.right, radius);
            Handles.DrawWireArc(topSphereCenter, capsuleCollider.transform.rotation * Vector3.up, capsuleCollider.transform.rotation * Vector3.forward, 360, radius);

            Handles.DrawWireDisc(bottomSphereCenter, Vector3.up, radius);
            Handles.DrawWireDisc(bottomSphereCenter, Vector3.forward, radius);
            Handles.DrawWireDisc(bottomSphereCenter, Vector3.right, radius);
            Handles.DrawWireArc(bottomSphereCenter, capsuleCollider.transform.rotation * Vector3.up, capsuleCollider.transform.rotation * Vector3.forward, 360, radius);

            Handles.DrawLine(topSphereCenter + capsuleCollider.transform.rotation * Vector3.right * radius, bottomSphereCenter + capsuleCollider.transform.rotation * Vector3.right * radius);
            Handles.DrawLine(topSphereCenter + capsuleCollider.transform.rotation * Vector3.forward * radius, bottomSphereCenter + capsuleCollider.transform.rotation * Vector3.forward * radius);
            Handles.DrawLine(topSphereCenter - capsuleCollider.transform.rotation * Vector3.right * radius, bottomSphereCenter - capsuleCollider.transform.rotation * Vector3.right * radius);
            Handles.DrawLine(topSphereCenter - capsuleCollider.transform.rotation * Vector3.forward * radius, bottomSphereCenter - capsuleCollider.transform.rotation * Vector3.forward * radius);

            Handles.matrix = Matrix4x4.identity;
        }
#endif
    }
}
