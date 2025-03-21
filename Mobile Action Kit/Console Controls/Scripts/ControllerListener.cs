using UnityEngine;

public class PS5ControllerListener : MonoBehaviour
{
    void Update()
    {
        // Log button presses
        foreach (KeyCode key in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(key))
            {
                Debug.Log($"Key Pressed: {key}");
            }
        }

        // Log Left Joystick movement
        float leftX = Input.GetAxis("Horizontal");
        float leftY = Input.GetAxis("Vertical");
        if (Mathf.Abs(leftX) > 0.1f || Mathf.Abs(leftY) > 0.1f)
        {
            Debug.Log($"Left Joystick: X={leftX}, Y={leftY}");
        }

        // Log Right Joystick movement
        float rightX = Input.GetAxis("RightStickHorizontal");
        float rightY = Input.GetAxis("RightStickVertical");
        if (Mathf.Abs(rightX) > 0.1f || Mathf.Abs(rightY) > 0.1f)
        {
            Debug.Log($"Right Joystick: X={rightX}, Y={rightY}");
        }

        // Log D-pad (Directional Pad) presses
        float dpadX = Input.GetAxis("DPadHorizontal");
        float dpadY = Input.GetAxis("DPadVertical");
        if (Mathf.Abs(dpadX) > 0.1f || Mathf.Abs(dpadY) > 0.1f)
        {
            Debug.Log($"D-Pad: X={dpadX}, Y={dpadY}");
        }
    }
}