using System;
using UnityEngine;

namespace MobileActionKit
{
    [Serializable]
    public class TouchPadLook
    {
        [HideInInspector]
        public bool EnableMouseClickAndTouchInput = true;
        public float XSensitivity = 2f;
        public float YSensitivity = 2f;
        public bool clampVerticalRotation = true;
        public float MinimumX = -90F;
        public float MaximumX = 90F;
        public bool smooth;
        public float smoothTime = 5f;
        public bool lockCursor = true;


        private Quaternion m_CharacterTargetRot;
        private Quaternion m_CameraTargetRot;
        private bool m_cursorIsLocked = true;

        float MyangleX;
        public Vector2 LookAxis;

        public void Init(Transform character, Transform camera)
        {
            if (EnableMouseClickAndTouchInput == true)
            {
                m_CharacterTargetRot = character.localRotation;
                m_CameraTargetRot = camera.localRotation;
            }
        }


        public void LookRotation(Transform character, Transform camera)
        {
            if (EnableMouseClickAndTouchInput == true)
            {
                float yRot = LookAxis.x * XSensitivity;
                float xRot = LookAxis.y * YSensitivity;

                m_CharacterTargetRot *= Quaternion.Euler(0f, yRot, 0f);
                m_CameraTargetRot *= Quaternion.Euler(-xRot, 0f, 0f);

                if (clampVerticalRotation)
                {
                    m_CameraTargetRot = ClampRotationAroundXAxis(m_CameraTargetRot);
                    //MyangleX = ClampAngles(camera.localRotation.x, MinimumX, MaximumX);
                    //camera.localRotation = Quaternion.Euler(new Vector3(MyangleX, camera.localEulerAngles.y, 0));
                }
                if (smooth)
                {
                    character.localRotation = Quaternion.Slerp(character.localRotation, m_CharacterTargetRot,
                        smoothTime * Time.deltaTime);
                    camera.localRotation = Quaternion.Slerp(camera.localRotation, m_CameraTargetRot,
                        smoothTime * Time.deltaTime);
                }
                else
                {
                    character.localRotation = m_CharacterTargetRot;
                    camera.localRotation = m_CameraTargetRot;
                }

                UpdateCursorLock();
            }
        }

        public void SetCursorLock(bool value)
        {
            lockCursor = value;
            if (!lockCursor)
            {//we force unlock the cursor if the user disable the cursor locking helper
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }

        public void UpdateCursorLock()
        {
            //if the user set "lockCursor" we check & properly lock the cursos
            if (lockCursor)
                InternalLockUpdate();
        }

        private void InternalLockUpdate()
        {
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                m_cursorIsLocked = false;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                m_cursorIsLocked = true;
            }

            if (m_cursorIsLocked)
            {
                //Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else if (!m_cursorIsLocked)
            {
                //Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }

        Quaternion ClampRotationAroundXAxis(Quaternion q)
        {
            q.x /= q.w;
            q.y /= q.w;
            q.z /= q.w;
            q.w = 1.0f;

            float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);

            angleX = Mathf.Clamp(angleX, MinimumX, MaximumX);

            q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);

            return q;
        }
        //public float ClampAngles(float angle, float min, float max)
        //{
        //    if (angle < 90 || angle > 270)
        //    {       // if angle in the critic region...
        //        if (angle > 180) angle -= 360;  // convert all angles to -180..+180
        //        if (max > 180) max -= 360;
        //        if (min > 180) min -= 360;
        //    }
        //    angle = Mathf.Clamp(angle, min, max);
        //    if (angle < 0) angle += 360;  // if angle negative, convert to 0..360
        //    return angle;
        //}

    }
}
