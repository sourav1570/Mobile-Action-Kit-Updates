using UnityEditor;
using UnityEngine;
using TMPro;


namespace MobileActionKit
{
    [CustomEditor(typeof(WeaponAttachments))]
    public class WeaponCustomisationEditor : Editor
    {
        private SerializedProperty ScriptInfo;
        private SerializedProperty inventoryItemsProperty;
        private SerializedProperty useConfirmationWindowProperty;
        private SerializedProperty WeaponAttachmentsManagerComponent;
        private SerializedProperty confirmationWindowProperty;
        private SerializedProperty confirmationTextProperty;
        private SerializedProperty confirmationMessageProperty;
        private SerializedProperty showItemPriceProperty;
        private SerializedProperty buyTextProperty;
        private SerializedProperty confirmYesButtonProperty;
        private SerializedProperty confirmNoButtonProperty;
        // private SerializedProperty currentItemToPurchase;

        private void OnEnable()
        {
            ScriptInfo = serializedObject.FindProperty("ScriptInfo");
            inventoryItemsProperty = serializedObject.FindProperty("inventoryItems");
            WeaponAttachmentsManagerComponent = serializedObject.FindProperty("WeaponAttachmentsManagerComponent");
            useConfirmationWindowProperty = serializedObject.FindProperty("UseConfirmationWindow");
            confirmationWindowProperty = serializedObject.FindProperty("confirmationWindow");
            confirmationTextProperty = serializedObject.FindProperty("ConfirmationText");
            confirmationMessageProperty = serializedObject.FindProperty("ConfirmationMessage");
            showItemPriceProperty = serializedObject.FindProperty("ShowItemPrice");
            buyTextProperty = serializedObject.FindProperty("BuyText");
            confirmYesButtonProperty = serializedObject.FindProperty("confirmYesButton");
            confirmNoButtonProperty = serializedObject.FindProperty("confirmNoButton");
            //currentItemToPurchase = serializedObject.FindProperty("currentItemToPurchase");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            // Title

            EditorGUILayout.PropertyField(ScriptInfo, new GUIContent("Script Info"));

            // Confirmation Window Fields
            EditorGUILayout.LabelField("Confirmation Window Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(useConfirmationWindowProperty, new GUIContent("Use Confirmation Window"));

            if (useConfirmationWindowProperty.boolValue)
            {
                EditorGUILayout.PropertyField(WeaponAttachmentsManagerComponent, new GUIContent("Weapon Attachments Manager Component"));
                EditorGUILayout.PropertyField(confirmationWindowProperty, new GUIContent("Confirmation Window"));
                EditorGUILayout.PropertyField(confirmationTextProperty, new GUIContent("Confirmation Text"));
                EditorGUILayout.PropertyField(confirmationMessageProperty, new GUIContent("Confirmation Message"));
                EditorGUILayout.PropertyField(showItemPriceProperty, new GUIContent("Show Item Price"));
                EditorGUILayout.PropertyField(buyTextProperty, new GUIContent("Buy Text"));
                EditorGUILayout.PropertyField(confirmYesButtonProperty, new GUIContent("Confirm Yes Button"));
                EditorGUILayout.PropertyField(confirmNoButtonProperty, new GUIContent("Confirm No Button"));
                // EditorGUILayout.PropertyField(currentItemToPurchase, new GUIContent("currentItemToPurchase"),true);
            }

            EditorGUILayout.Space();

            // Display each item in the inventoryItems list
            for (int i = 0; i < inventoryItemsProperty.arraySize; i++)
            {
                SerializedProperty itemProperty = inventoryItemsProperty.GetArrayElementAtIndex(i);

                EditorGUILayout.LabelField("Attachment" + " " + i, EditorStyles.boldLabel);

                EditorGUILayout.BeginVertical("box");

                // Display fields of each InventoryItem
                EditorGUILayout.PropertyField(itemProperty.FindPropertyRelative("Category"));
                EditorGUILayout.PropertyField(itemProperty.FindPropertyRelative("Weapon"));
                EditorGUILayout.PropertyField(itemProperty.FindPropertyRelative("ItemPrice"));
                EditorGUILayout.PropertyField(itemProperty.FindPropertyRelative("BuyButton"));
                EditorGUILayout.PropertyField(itemProperty.FindPropertyRelative("UniqueNameForSavingData"));
                EditorGUILayout.PropertyField(itemProperty.FindPropertyRelative("BuyText"));
                EditorGUILayout.PropertyField(itemProperty.FindPropertyRelative("EquippedText"));
                EditorGUILayout.PropertyField(itemProperty.FindPropertyRelative("UnequippedText"));
                EditorGUILayout.PropertyField(itemProperty.FindPropertyRelative("BuyButtonText"));
                EditorGUILayout.PropertyField(itemProperty.FindPropertyRelative("ObjectsToActivateAfterPurchase"), true);
                EditorGUILayout.PropertyField(itemProperty.FindPropertyRelative("ObjectsToDeactivateAfterPurchase"), true);

                EditorGUILayout.EndVertical();
                EditorGUILayout.Space();
            }

            // "Add Inventory Item" and "Remove Last Inventory Item" buttons
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Add New Attachment"))
            {
                inventoryItemsProperty.arraySize++;
            }
            if (GUILayout.Button("Remove Last Added Attachment") && inventoryItemsProperty.arraySize > 0)
            {
                inventoryItemsProperty.arraySize--;
            }
            EditorGUILayout.EndHorizontal();

            serializedObject.ApplyModifiedProperties();
        }
    }
}