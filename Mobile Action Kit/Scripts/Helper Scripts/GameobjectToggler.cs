using UnityEngine;
using System.Collections.Generic;

public class GameobjectToggler : MonoBehaviour
{
    [TextArea]
    public string ScriptInfo = "GameObjectToggler allows enabling or disabling multiple GameObjects at once. " +
                               "It creates separate parent objects for each GameObject when disabling and restores them when enabling, preserving hierarchy position.";

    [Space(10)]
    [Tooltip("List of GameObjects to be toggled on or off.")]
    public List<GameObject> DisplayNestedGameObjects = new List<GameObject>();

    [Tooltip("Stores GameObjects that were unparented.")]
    public List<GameObject> UnparentedGameObjects = new List<GameObject>();

    [Tooltip("Indicates whether the GameObjects are currently active.")]
    public bool AreGameObjectsCurrentlyActive = true;

    private Dictionary<GameObject, Transform> originalParents = new Dictionary<GameObject, Transform>();
    private Dictionary<GameObject, int> siblingIndexes = new Dictionary<GameObject, int>();
    private Dictionary<GameObject, GameObject> parentObjects = new Dictionary<GameObject, GameObject>();

    public void ToggleGameObjects()
    {
        AreGameObjectsCurrentlyActive = !AreGameObjectsCurrentlyActive;

        foreach (var obj in DisplayNestedGameObjects)
        {
            if (obj != null)
            {
                obj.SetActive(AreGameObjectsCurrentlyActive);
                if (obj.transform.parent != null)
                {
                    obj.transform.parent.gameObject.SetActive(AreGameObjectsCurrentlyActive);
                }
            }
        }
    }
    public void ShouldActivateUI(bool Activate)
    {
        foreach (var obj in DisplayNestedGameObjects)
        {
            if (obj != null)
            {
                obj.SetActive(Activate);
                if (obj.transform.parent != null)
                {
                    obj.transform.parent.gameObject.SetActive(Activate);
                }
            }
        }
    }

    public void CreateParentForAdditionalGameObjects()
    {
        List<GameObject> newlyNestedObjects = new List<GameObject>();

        foreach (var obj in UnparentedGameObjects)
        {
            if (obj != null && !parentObjects.ContainsKey(obj))
            {
                // Save original hierarchy information
                originalParents[obj] = obj.transform.parent;
                siblingIndexes[obj] = obj.transform.GetSiblingIndex();

                // Create new parent
                GameObject newParent = new GameObject(obj.name + "_Parent");
                newParent.transform.SetParent(originalParents[obj]); // Maintain hierarchy
                newParent.transform.SetSiblingIndex(siblingIndexes[obj]); // Keep original index

                obj.transform.SetParent(newParent.transform);
                parentObjects[obj] = newParent;

                newlyNestedObjects.Add(obj);
            }
        }

        // Move objects from Unparented to DisplayNestedGameObjects list
        DisplayNestedGameObjects.AddRange(newlyNestedObjects);
        UnparentedGameObjects.Clear();
    }

    public void RemoveNestedGameObjects()
    {
        List<GameObject> unparentedObjects = new List<GameObject>();
        List<GameObject> parentsToDestroy = new List<GameObject>();

        foreach (var obj in DisplayNestedGameObjects)
        {
            if (obj != null && obj.transform.parent != null)
            {
                Transform parentTransform = obj.transform.parent;
                int parentIndex = parentTransform.GetSiblingIndex(); // Get parent's position

                obj.transform.SetParent(parentTransform.parent); // Unparent
                obj.transform.SetSiblingIndex(parentIndex); // Keep same index in hierarchy

                if (!parentsToDestroy.Contains(parentTransform.gameObject))
                {
                    parentsToDestroy.Add(parentTransform.gameObject);
                }

                unparentedObjects.Add(obj);
            }
        }

        // Destroy empty parents
        foreach (var parent in parentsToDestroy)
        {
            if (parent.transform.childCount == 0)
            {
                DestroyImmediate(parent);
            }
        }

        // Reset script state completely
        DisplayNestedGameObjects.Clear();
        UnparentedGameObjects.Clear();
        originalParents.Clear();
        siblingIndexes.Clear();
        parentObjects.Clear();

        // here add the DisplayNestedGameObjects back to unparented gameObjects
        // Add DisplayNestedGameObjects back to UnparentedGameObjects
        UnparentedGameObjects.AddRange(unparentedObjects);
    }
}









//using UnityEngine;
//using System.Collections.Generic;

//public class GameobjectToggler : MonoBehaviour
//{
//    [TextArea]
//    public string ScriptInfo = "GameObjectToggler allows enabling or disabling multiple GameObjects at once. " +
//                               "It creates separate parent objects for each GameObject when disabling and restores them when enabling, preserving hierarchy position.";

//    [Space(10)]
//    [Tooltip("List of GameObjects to be toggled on or off.")]
//    public List<GameObject> DisplayNestedGameObjects = new List<GameObject>();

//    //public bool ParentAlreadyExists = false;

//    //public bool AddGameObjectsWithNoParent = false;

//    public List<GameObject> UnparentedGameObjects = new List<GameObject>();

//    [Tooltip("Indicates whether the GameObjects are currently active.")]
//    public bool AreGameObjectsCurrentlyActive = true;

//    private Dictionary<GameObject, GameObject> parentObjects = new Dictionary<GameObject, GameObject>();
//    private Dictionary<GameObject, Transform> originalParents = new Dictionary<GameObject, Transform>();
//    private Dictionary<GameObject, int> siblingIndexes = new Dictionary<GameObject, int>();

//    public void ToggleGameObjects()
//    {
//        AreGameObjectsCurrentlyActive = !AreGameObjectsCurrentlyActive;

//        if (AreGameObjectsCurrentlyActive)
//        {
//            // If parent already exists, activate parents and children
//            //if (ParentAlreadyExists)
//            //{
//                foreach (var obj in DisplayNestedGameObjects)
//                {
//                    if (obj != null)
//                    {
//                        obj.SetActive(true);
//                        if (obj.transform.parent != null)
//                        {
//                            obj.transform.parent.gameObject.SetActive(true); // Activate parent if exists
//                        }
//                    }
//                }
//            //}
//            //else
//            //{
//            //    // Restore hierarchy and destroy parent objects
//            //    foreach (var obj in new List<GameObject>(parentObjects.Keys))
//            //    {
//            //        if (obj != null && parentObjects[obj] != null)
//            //        {
//            //            obj.transform.SetParent(originalParents[obj]);
//            //            obj.transform.SetSiblingIndex(siblingIndexes[obj]);
//            //            obj.SetActive(true);
//            //            DestroyImmediate(parentObjects[obj]);
//            //        }
//            //    }
//            //    parentObjects.Clear();
//            //    originalParents.Clear();
//            //    siblingIndexes.Clear();
//            //}
//        }
//        else
//        {
//            //if (ParentAlreadyExists)
//            //{
//                // Deactivate only the parent objects
//                foreach (var obj in DisplayNestedGameObjects)
//                {
//                    if (obj != null && obj.transform.parent != null)
//                    {
//                        obj.transform.parent.gameObject.SetActive(false);
//                    }
//                }
//            //}
//            //else
//            //{
//            //    // Create and deactivate parent objects
//            //    foreach (var obj in DisplayGameObjectsWithParent)
//            //    {
//            //        if (obj != null && !parentObjects.ContainsKey(obj))
//            //        {
//            //            originalParents[obj] = obj.transform.parent;
//            //            siblingIndexes[obj] = obj.transform.GetSiblingIndex();

//            //            GameObject newParent = new GameObject(obj.name + "_Parent");
//            //            newParent.transform.SetParent(originalParents[obj]);
//            //            newParent.transform.SetSiblingIndex(siblingIndexes[obj]);

//            //            obj.transform.SetParent(newParent.transform);
//            //            parentObjects[obj] = newParent;
//            //            newParent.SetActive(false);
//            //        }
//            //    }
//            //}
//        }
//    }

//    public void CreateParentForAdditionalGameObjects()
//    {
//        foreach (var obj in UnparentedGameObjects)
//        {
//            if (obj != null && !parentObjects.ContainsKey(obj))
//            {
//                originalParents[obj] = obj.transform.parent;
//                siblingIndexes[obj] = obj.transform.GetSiblingIndex();

//                GameObject newParent = new GameObject(obj.name + "_Parent");
//                newParent.transform.SetParent(originalParents[obj]);
//                newParent.transform.SetSiblingIndex(siblingIndexes[obj]);

//                obj.transform.SetParent(newParent.transform);
//                parentObjects[obj] = newParent;
//            }
//        }

//        DisplayNestedGameObjects.AddRange(UnparentedGameObjects);
//        UnparentedGameObjects.Clear();
//        //AddGameObjectsWithNoParent = false;
//    }
//}




//using UnityEngine;
//using System.Collections.Generic;

//public class GameobjectToggler : MonoBehaviour
//{
//    [TextArea]
//    public string ScriptInfo = "GameObjectToggler allows enabling or disabling multiple GameObjects at once. " +
//                               "It creates separate parent objects for each GameObject when disabling and restores them when enabling, preserving hierarchy position.";

//    [Space(10)]
//    [Tooltip("List of GameObjects to be toggled on or off.")]
//    public List<GameObject> gameObjects = new List<GameObject>();

//    [Tooltip("Indicates whether the GameObjects are currently active. " +
//             "True means they are active, false means they are disabled.")]
//    public bool AreObjectsCurrentlyActive = true;

//    private Dictionary<GameObject, GameObject> parentObjects = new Dictionary<GameObject, GameObject>();
//    private Dictionary<GameObject, Transform> originalParents = new Dictionary<GameObject, Transform>();
//    private Dictionary<GameObject, int> siblingIndexes = new Dictionary<GameObject, int>();

//    public void ToggleGameObjects()
//    {
//        AreObjectsCurrentlyActive = !AreObjectsCurrentlyActive;

//        if (AreObjectsCurrentlyActive)
//        {
//            // Activate: Unparent, restore hierarchy, and destroy individual parent objects
//            foreach (var obj in new List<GameObject>(parentObjects.Keys))
//            {
//                if (obj != null && parentObjects[obj] != null)
//                {
//                    obj.transform.SetParent(originalParents[obj]);
//                    obj.transform.SetSiblingIndex(siblingIndexes[obj]);
//                    obj.SetActive(true); // Ensure all GameObjects are activated
//                    DestroyImmediate(parentObjects[obj]);
//                }
//            }
//            parentObjects.Clear();
//            originalParents.Clear();
//            siblingIndexes.Clear();
//        }
//        else
//        {
//            // Deactivate: Create separate parent objects while keeping hierarchy position
//            foreach (var obj in gameObjects)
//            {
//                if (obj != null && !parentObjects.ContainsKey(obj)) // Ensure objects not already handled
//                {
//                    originalParents[obj] = obj.transform.parent;
//                    siblingIndexes[obj] = obj.transform.GetSiblingIndex();

//                    GameObject newParent = new GameObject(obj.name + "_Parent");
//                    newParent.transform.SetParent(originalParents[obj]);
//                    newParent.transform.SetSiblingIndex(siblingIndexes[obj]);

//                    obj.transform.SetParent(newParent.transform);
//                    parentObjects[obj] = newParent;
//                    newParent.SetActive(false); // Deactivate only the parent
//                }
//            }
//        }
//    }
//}


//using UnityEngine;
//using System.Collections.Generic;

//public class GameobjectToggler : MonoBehaviour
//{
//    [TextArea]
//    public string ScriptInfo = "GameObjectToggler allows enabling or disabling multiple GameObjects at once. " +
//                               "It creates separate parent objects for each GameObject when disabling and restores them when enabling, preserving hierarchy position.";

//    [Space(10)]
//    public List<GameObject> gameObjects = new List<GameObject>();
//    public bool areObjectsActive = true;
//    private Dictionary<GameObject, GameObject> parentObjects = new Dictionary<GameObject, GameObject>();
//    private Dictionary<GameObject, Transform> originalParents = new Dictionary<GameObject, Transform>();
//    private Dictionary<GameObject, int> siblingIndexes = new Dictionary<GameObject, int>();

//    public void ToggleGameObjects()
//    {
//        areObjectsActive = !areObjectsActive;

//        if (areObjectsActive)
//        {
//            // Activate: Unparent and destroy individual parent objects while maintaining hierarchy position
//            foreach (var obj in gameObjects)
//            {
//                if (obj != null && parentObjects.ContainsKey(obj))
//                {
//                    GameObject parent = parentObjects[obj];
//                    obj.transform.SetParent(originalParents[obj]);
//                    obj.transform.SetSiblingIndex(siblingIndexes[obj]);
//                    DestroyImmediate(parent);
//                }
//            }
//            parentObjects.Clear();
//        }
//        else
//        {
//            // Deactivate: Create separate parent objects and assign each GameObject
//            foreach (var obj in gameObjects)
//            {
//                if (obj != null)
//                {
//                    originalParents[obj] = obj.transform.parent;
//                    siblingIndexes[obj] = obj.transform.GetSiblingIndex();

//                    GameObject newParent = new GameObject(obj.name + "_Parent");
//                    newParent.transform.SetParent(originalParents[obj]);
//                    newParent.transform.SetSiblingIndex(siblingIndexes[obj]);

//                    obj.transform.SetParent(newParent.transform);
//                    parentObjects[obj] = newParent;
//                    newParent.SetActive(false);
//                }
//            }
//        }
//    }
//}




//using UnityEngine;
//using System.Collections.Generic;

//public class GameobjectToggler : MonoBehaviour
//{
//    [TextArea]
//    public string ScriptInfo = "GameObjectToggler allows enabling or disabling multiple GameObjects at once. " +
//                           "It keeps track of a list of GameObjects and toggles their active state whenever ToggleGameObjects() is called. " +
//                           "This is useful for managing UI elements, game objects, or groups of objects efficiently in a scene.";


//    [Space(10)]
//    public List<GameObject> gameObjects = new List<GameObject>();
//    public bool areObjectsActive = true;

//    public void ToggleGameObjects()
//    {
//        areObjectsActive = !areObjectsActive;
//        foreach (var obj in gameObjects)
//        {
//            if (obj != null)
//                obj.SetActive(areObjectsActive);
//        }
//    }
//}
