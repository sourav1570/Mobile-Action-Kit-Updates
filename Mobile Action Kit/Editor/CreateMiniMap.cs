using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

namespace MobileActionKit
{
    public class CreateMiniMap : EditorWindow
    {
      
        public GameObject Player;
        public GameObject RequiredScripts;
        public Canvas Canvas2D;
        public RenderTexture RenderTextureUI;

        private const string PrefabPath = "Assets/Mobile Action Kit/Editor/Editor Prefabs/Others/";
        private readonly string[] PrefabNames = { "MiniMap UI", "MiniMap", "Minimap Player Indicator" };

        private MiniMap MiniMapComponent;
        private SpriteRenderer MiniMapSprite;
        private RectTransform MiniMapCanvasGameObject;

        [MenuItem("Tools/Mobile Action Kit/Player/Player/Create Minimap", priority = 2)]
        public static void ShowWindow()
        {
            GetWindow<CreateMiniMap>("Minimap setup");
        }

        private void OnGUI()
        {
            GUILayout.Label("Setup MiniMap UI", EditorStyles.boldLabel);

            Player = (GameObject)EditorGUILayout.ObjectField("Player", Player, typeof(GameObject), true);
            RequiredScripts = (GameObject)EditorGUILayout.ObjectField("Required Scripts", RequiredScripts, typeof(GameObject), true);
            Canvas2D = (Canvas)EditorGUILayout.ObjectField("Canvas 2D", Canvas2D, typeof(Canvas), true);
            RenderTextureUI = (RenderTexture)EditorGUILayout.ObjectField("Render Texture", RenderTextureUI, typeof(RenderTexture), false);

            if (GUILayout.Button("Add Required UI"))
            {
                if (Player == null || Canvas2D == null)
                {
                    Debug.LogError("Please assign both Player and Canvas2D.");
                    return;
                }

                AddRequiredUI();

                MiniMapComponent.GetComponent<MiniMap>().MiniMapCamera = MiniMapComponent.GetComponent<Camera>();
                MiniMapComponent.GetComponent<MiniMap>().PlayerTransform = Player.transform;
                MiniMapComponent.GetComponent<MiniMap>().SmallMap = MiniMapCanvasGameObject.transform.GetChild(0).gameObject;
                MiniMapComponent.GetComponent<MiniMap>().BigMap = MiniMapCanvasGameObject.transform.GetChild(1).gameObject;
                MiniMapComponent.GetComponent<Camera>().targetTexture = RenderTextureUI;
                MiniMapComponent.GetComponent<MiniMap>().MiniMapUIButtons = new Button[2];
                MiniMapComponent.GetComponent<MiniMap>().MiniMapUIButtons[0] = MiniMapCanvasGameObject.transform.GetChild(0).GetComponent<Button>();
                MiniMapComponent.GetComponent<MiniMap>().MiniMapUIButtons[1] = MiniMapCanvasGameObject.transform.GetChild(1).GetComponent<Button>();

                MiniMapComponent.transform.SetParent(RequiredScripts.transform);
                MiniMapSprite.transform.SetParent(Player.transform);

                MiniMapSprite.transform.localPosition = new Vector3(0f, 900f, 0f);
                MiniMapSprite.transform.localEulerAngles = new Vector3(90f, 0f, 0f);

                MiniMapCanvasGameObject.transform.SetParent(Canvas2D.transform);

                MiniMapCanvasGameObject.transform.GetChild(0).transform.GetChild(1).GetComponent<RawImage>().texture = RenderTextureUI;
                MiniMapCanvasGameObject.transform.GetChild(1).transform.GetChild(1).GetComponent<RawImage>().texture = RenderTextureUI;

               
                // Set position to (0, 1000, 0)
                MiniMapComponent.GetComponent<MiniMap>().MiniMapCamera.transform.localPosition = new Vector3(0f, 1000f, 0f);


                //// Set rotation to (90, 0, 0) around X-axis only (no Z rotation)
                MiniMapComponent.GetComponent<MiniMap>().MiniMapCamera.transform.localEulerAngles = new Vector3(90f, 0f, 0f); 
               


                // Set scale to (1, 1, 1)
                MiniMapComponent.GetComponent<MiniMap>().MiniMapCamera.transform.localScale = Vector3.one;
            }
        }

        private void AddRequiredUI()
        {
            foreach (var prefabName in PrefabNames)
            {
                string fullPath = PrefabPath + prefabName + ".prefab";
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(fullPath);

                if (prefab == null)
                {
                    Debug.LogError($"Prefab not found at {fullPath}");
                    continue;
                }

                GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, Canvas2D.transform);
                PrefabUtility.UnpackPrefabInstance(instance, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);

                SetupUI(instance, prefabName); 
            }
        }

        private void SetupUI(GameObject uiInstance, string prefabName)
        { 
            if (prefabName == "MiniMap UI")
            {
                MiniMapCanvasGameObject = uiInstance.GetComponent<RectTransform>();
            }
            else if (prefabName == "MiniMap")
            {
                MiniMapComponent = uiInstance.GetComponent<MiniMap>();
            }
            else if (prefabName == "Minimap Player Indicator")
            {
                MiniMapSprite = uiInstance.GetComponent<SpriteRenderer>();
            }
        }
    }
}
