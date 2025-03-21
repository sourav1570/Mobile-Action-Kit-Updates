namespace MobileActionKit
{
    using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
[ExecuteInEditMode]
public class ClickSpawn : MonoBehaviour
{
    private GameObject go;

    public GameObject[] Prefabs;
    public float MinimumPrefabRotationAtY;
    public float MaximumPrefabRotationAtY;

    //private void OnEnable()
    //{
    //    if (!Application.isEditor)
    //    {
    //        Destroy(this);
    //    }
    //    SceneView.onSceneGUIDelegate += OnScene;
    //}

    //void OnScene(SceneView scene)
    //{
    //    Event e = Event.current;

    //    if (e.type == EventType.MouseDown && e.button == 2)
    //    {
    //        Debug.Log("Middle Mouse was pressed");

    //        Vector3 mousePos = e.mousePosition;
    //        float ppp = EditorGUIUtility.pixelsPerPoint;
    //        mousePos.y = scene.camera.pixelHeight - mousePos.y * ppp;
    //        mousePos.x *= ppp;

    //        Ray ray = scene.camera.ScreenPointToRay(mousePos);
    //        RaycastHit hit;

    //        if (Physics.Raycast(ray, out hit))
    //        { 
    //            go = Instantiate(Prefabs[Random.Range(0, Prefabs.Length)], hit.point, Quaternion.identity);               
    //            Vector3 temp = go.transform.eulerAngles;
    //            temp.y = Random.Range(MinimumPrefabRotationAtY, MaximumPrefabRotationAtY);
    //            go.transform.eulerAngles = temp;
    //            Debug.Log("Instantiated Primitive " + hit.point);
    //        }
    //        e.Use();
    //    }
    //}
}
#endif
}