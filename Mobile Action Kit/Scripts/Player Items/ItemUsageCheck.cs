using UnityEngine;

public class ItemUsageCheck : MonoBehaviour
{
    [TextArea]
    public string ScriptInfo = "This script modifies the assigned MonoBehaviour by injecting additional code into the selected function. The item's usage is reduced only when the selected function is called.";


    [Tooltip("Drag and drop the script that contains the function you want to modify.")]
    public MonoBehaviour targetScript;

    [Tooltip("Enter the name of the function that should trigger item usage reduction.")]
    public string selectedMethod;

    [Tooltip("Drag and drop the ItemUsageController GameObject here to manage item usage.")]
    public GameObject itemUsageController;
}
