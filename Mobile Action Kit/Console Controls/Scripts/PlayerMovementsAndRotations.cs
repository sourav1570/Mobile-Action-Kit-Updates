using UnityEngine;
using UnityEngine.InputSystem;
using MobileActionKit;
using System.Collections;

public class PlayerMovementsAndRotations : MonoBehaviour
{
    public PlayerInput PlayerInputAction;
    public string ActionMapName = "Player";
    public Transform Player; // Assign the Player (Yaw Rotation)
    public Transform PlayerCamera; // Assign the Camera (Pitch Rotation)
    public FirstPersonController FirstPersonControllerScript;
    public PlayerManager PlayerManagerScript;

    public float minXRotation = -90f; // Minimum vertical rotation (looking down)
    public float maxXRotation = 90f;  // Maximum vertical rotation (looking up)

    [System.Serializable]
    public class PlayerLookValues
    {
        public float sensitivityX = 100f; // Sensitivity for horizontal look
        public float sensitivityY = 100f; // Sensitivity for vertical look
        public bool invertY = false; // Option to invert Y-axis movement
        public bool enableHorizontalLook = true; // Enable/Disable horizontal look
        public bool enableVerticalLook = true; // Enable/Disable vertical look
    }

    public PlayerLookValues PlayerDefaultLookValues;
    public PlayerLookValues PlayerWeaponAimedLookValues;

    private float rotationX = 0f; // Stores vertical rotation (pitch)
    private float rotationY = 0f; // Stores horizontal rotation (yaw)

    private Vector2 lookInput = Vector2.zero; // Stores joystick input
    private InputActionMap gameplayActionMap;

    void Start()
    {
        // Initialize rotation
        rotationY = Player.eulerAngles.y;
        rotationX = PlayerCamera.localEulerAngles.x;

        // Adjust X rotation for correct startup behavior
        if (rotationX > 180) rotationX -= 360;

       gameplayActionMap = PlayerInputAction.actions.FindActionMap(ActionMapName);

    }
    void Update()
    {
        if (lookInput != Vector2.zero) // Only process when there's input
        {
            if(PlayerManagerScript.CurrentHoldingPlayerWeapon.IsAimed == false)
            {
                float lookX = lookInput.x; // Horizontal stick movement (left/right)
                float lookY = lookInput.y; // Vertical stick movement (up/down)

                // Apply horizontal rotation (Yaw)
                if (PlayerDefaultLookValues.enableHorizontalLook)
                {
                    rotationY += lookX * PlayerDefaultLookValues.sensitivityX * Time.deltaTime;
                }

                // Apply vertical rotation (Pitch)
                if (PlayerDefaultLookValues.enableVerticalLook)
                {
                    float adjustedY = PlayerDefaultLookValues.invertY ? lookY : -lookY; // Invert if needed
                    rotationX += adjustedY * PlayerDefaultLookValues.sensitivityY * Time.deltaTime;
                    rotationX = Mathf.Clamp(rotationX, minXRotation, maxXRotation); // Clamp pitch
                }
            }
            else
            {
                float lookX = lookInput.x; // Horizontal stick movement (left/right)
                float lookY = lookInput.y; // Vertical stick movement (up/down)

                // Apply horizontal rotation (Yaw)
                if (PlayerWeaponAimedLookValues.enableHorizontalLook)
                {
                    rotationY += lookX * PlayerWeaponAimedLookValues.sensitivityX * Time.deltaTime;
                }

                // Apply vertical rotation (Pitch)
                if (PlayerWeaponAimedLookValues.enableVerticalLook)
                {
                    float adjustedY = PlayerWeaponAimedLookValues.invertY ? lookY : -lookY; // Invert if needed
                    rotationX += adjustedY * PlayerWeaponAimedLookValues.sensitivityY * Time.deltaTime;
                    rotationX = Mathf.Clamp(rotationX, minXRotation, maxXRotation); // Clamp pitch
                }
            }
            // Apply rotations
            Player.rotation = Quaternion.Euler(0, rotationY, 0); // Yaw on player
            PlayerCamera.localRotation = Quaternion.Euler(rotationX, 0, 0); // Pitch on camera

        }
    }
    void OnRightJoystick_LookAround(InputValue value)
    {
        lookInput = value.Get<Vector2>(); // Read joystick movement (X = horizontal, Y = vertical)
    }
    void OnLeftJoystick_Walk(InputValue value)
    { 
        if (FirstPersonControllerScript != null)
        {
            if (PlayerManagerScript.IsMoving == false)
            {
                Vector2 walkinput = value.Get<Vector2>();

                if (walkinput != Vector2.zero) // Only process when there's input
                {
                    FirstPersonControllerScript.PlayerMovement(walkinput.x, walkinput.y);
                    FirstPersonControllerScript.JoystickScript.Pc_Controls_StartWalking();
                }
                else
                {
                    FirstPersonControllerScript.JoystickScript.Pc_Controls_StopWalking();
                }
            }
        }
    }
    public void PlayerStartSprintForConsoles()
    {
        PlayerManagerScript.Pc_Controls_StartSprinting();

        foreach (var action in gameplayActionMap.actions)
        {
            if (action.name != "RightJoystick_LookAround" && action.name != "L3_Sprint") 
            {
                action.Disable();
            }
        }
    }
    public void PlayerStopSprintForConsoles()
    {
        PlayerManagerScript.Pc_Controls_StopSprinting();

        gameplayActionMap.Enable();
    }
}
