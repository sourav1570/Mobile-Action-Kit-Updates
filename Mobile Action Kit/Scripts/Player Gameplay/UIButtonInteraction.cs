using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIButtonInteraction : MonoBehaviour
{
    [TextArea]
    public string ScriptInfo = "This script controls UI button interactability when gameObject is enabled and disabled.";
    [Space(10)]

    public Button[] UIButtons;

    [System.Serializable]
    public enum SwitchInteraction
    {
        MakeUIButtonsInteractable,
        MakeUIButtonsNonInteractable
    }

    public SwitchInteraction InteractionTypeOnEnable = SwitchInteraction.MakeUIButtonsNonInteractable;

    public SwitchInteraction InteractionTypeOnDisable = SwitchInteraction.MakeUIButtonsNonInteractable;

    private void OnEnable()
    {
        for(int x = 0; x < UIButtons.Length; x++)
        {
            if(InteractionTypeOnEnable == SwitchInteraction.MakeUIButtonsInteractable)
            {
                ChangeUIButtonState(UIButtons[x], true);
            }
            else
            {
                ChangeUIButtonState(UIButtons[x], false);
            }
        }
    }
    private void OnDisable()
    {
        for (int x = 0; x < UIButtons.Length; x++)
        {
            if (InteractionTypeOnDisable == SwitchInteraction.MakeUIButtonsInteractable)
            {
                ChangeUIButtonState(UIButtons[x], true);
            }
            else
            {
                ChangeUIButtonState(UIButtons[x], false);
            }
        }
    }
    public void ChangeUIButtonState(Button button , bool IsInteractable)
    {
        if (button != null)
        {
            button.interactable = IsInteractable;
        }
    }
}
