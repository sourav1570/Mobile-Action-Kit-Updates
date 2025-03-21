using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputBindingToggler : MonoBehaviour
{
    public PlayerInput PlayerInputAction;
    public string ActionMapName = "Player";

    public bool DisableActions = true;

    public bool ReEnableAction = true;
    public float TimeToReEnableActions = 3f;

    public string[] ActionNames;

    //public GameObject[] GameObjectsToDeactivateOnActionDisable;

    //public GameObject[] GameObjectsToActivateOnActionDisable;

    private InputActionMap gameplayActionMap;

    bool IsTaskCompleted = false;

    private void Start()
    {
        gameplayActionMap = PlayerInputAction.actions.FindActionMap(ActionMapName);
    }
    public void ActionsToggler()
    {
        if(IsTaskCompleted == false)
        {
            if (DisableActions == true)
            {
                foreach (var action in gameplayActionMap.actions)
                {
                    for (int x = 0; x < ActionNames.Length; x++)
                    {
                        if (action.name == ActionNames[x])
                        {
                            Debug.Log(action.name + "Disabled");
                            action.Disable();
                        }
                    }
                }

                //for (int x = 0; x < GameObjectsToDeactivateOnActionDisable.Length; x++)
                //{
                //    GameObjectsToDeactivateOnActionDisable[x].SetActive(false);
                //}
                //for (int x = 0; x < GameObjectsToActivateOnActionDisable.Length; x++)
                //{
                //    GameObjectsToActivateOnActionDisable[x].SetActive(true);
                //}
            }
            else
            {
                foreach (var action in gameplayActionMap.actions)
                {
                    for (int x = 0; x < ActionNames.Length; x++)
                    {
                        if (action.name == ActionNames[x])
                        {
                            action.Enable();
                        }
                    }

                }
            }

            if (ReEnableAction == true)
            {
                IsTaskCompleted = true;
                StartCoroutine(Coro());
            }
            else
            {
                IsTaskCompleted = false;
            }

            
        }
      
       
    }
    IEnumerator Coro()
    {
        yield return new WaitForSeconds(TimeToReEnableActions);
        foreach (var action in gameplayActionMap.actions)
        {
            for (int x = 0; x < ActionNames.Length; x++)
            {
                if (action.name == ActionNames[x])
                {
                    action.Enable();
                }
            }
        }
        IsTaskCompleted = false;
    }
}
