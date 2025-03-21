using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace MobileActionKit
{
    public class ResizeImage : MonoBehaviour, IPointerDownHandler, IDragHandler
    {

        [TextArea]
        public string ScriptInfo = "This script enables users to resize a UI image dynamically using touch or mouse drag. The size is saved and restored using PlayerPrefs, and default values are stored for resetting.";

        [Space(10)]
        [Tooltip("Unique key to store and retrieve the resized dimensions in PlayerPrefs.")]
        public string UniqueNameToSaveResizing = "";

        [Tooltip("Minimum allowed size of the image.")]
        public Vector2 minSize = new Vector2(100, 100);

        [Tooltip("Maximum allowed size of the image.")]
        public Vector2 maxSize = new Vector2(400, 400);

        private RectTransform panelRectTransform;
        private Vector2 originalLocalPointerPosition;
        private Vector2 originalSizeDelta;

        [HideInInspector]
        public string SaveSizeX;
        [HideInInspector]
        public string SaveSizeY;

        [HideInInspector]
        public string DefaultSizeX = "";
        [HideInInspector]
        public string DefaultSizeY = "";

        [HideInInspector]
        public bool EnableResizing = false;

        void Awake()
        {
            panelRectTransform = transform.GetComponent<RectTransform>();
            Vector3 pos = panelRectTransform.sizeDelta;
            pos.x = PlayerPrefs.GetFloat(UniqueNameToSaveResizing + "SaveSizeX", pos.x);
            pos.y = PlayerPrefs.GetFloat(UniqueNameToSaveResizing + "SaveSizeY", pos.y);
            panelRectTransform.sizeDelta = pos;

            if (PlayerPrefs.GetInt(UniqueNameToSaveResizing + "SaveResizingDefaultValues") == 0)
            {
                PlayerPrefs.SetFloat(UniqueNameToSaveResizing + "DefaultSizeX", pos.x);
                PlayerPrefs.SetFloat(UniqueNameToSaveResizing + "DefaultSizeY", pos.y);
                PlayerPrefs.SetInt(UniqueNameToSaveResizing + "SaveResizingDefaultValues", 1);
            }

        }
        public void ResetSize()
        {
            panelRectTransform = transform.GetComponent<RectTransform>();
            Vector3 pos = panelRectTransform.sizeDelta;
            PlayerPrefs.SetInt(UniqueNameToSaveResizing + "SaveResizingDefaultValues", 0);
            pos.x = PlayerPrefs.GetFloat(UniqueNameToSaveResizing + "DefaultSizeX", pos.x);
            pos.y = PlayerPrefs.GetFloat(UniqueNameToSaveResizing + "DefaultSizeY", pos.y);
            panelRectTransform.sizeDelta = pos;
        }
        public void OnPointerDown(PointerEventData data)
        {
            if (EnableResizing == true)
            {
                originalSizeDelta = panelRectTransform.sizeDelta;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(panelRectTransform, data.position, data.pressEventCamera, out originalLocalPointerPosition);
            }
           
          
        }

        public void OnDrag(PointerEventData data)
        {
            if (EnableResizing == true)
            {
                if (panelRectTransform == null)
                    return;

                Vector2 localPointerPosition;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(panelRectTransform, data.position, data.pressEventCamera, out localPointerPosition);
                Vector3 offsetToOriginal = localPointerPosition - originalLocalPointerPosition;

                Vector2 sizeDelta = originalSizeDelta + new Vector2(offsetToOriginal.x, -offsetToOriginal.y);
                sizeDelta = new Vector2(
                    Mathf.Clamp(sizeDelta.x, minSize.x, maxSize.x),
                    Mathf.Clamp(sizeDelta.y, minSize.y, maxSize.y)
                );

                panelRectTransform.sizeDelta = sizeDelta;

                PlayerPrefs.SetFloat(UniqueNameToSaveResizing + "SaveSizeX", sizeDelta.x);
                PlayerPrefs.SetFloat(UniqueNameToSaveResizing + "SaveSizeY", sizeDelta.y);
            }
        }
    }
}



//using UnityEngine;
//using UnityEngine.UI;
//using UnityEngine.EventSystems;

//namespace MobileActionKit
//{
//	public class ResizePanel : MonoBehaviour, IPointerDownHandler, IDragHandler
//	{
//		public string UniqueNameToSaveResizing = "";
//		public Vector2 minSize = new Vector2(100, 100);
//		public Vector2 maxSize = new Vector2(400, 400);

//		private RectTransform panelRectTransform;
//		private Vector2 originalLocalPointerPosition;
//		private Vector2 originalSizeDelta;

//		[HideInInspector]
//		public string SaveSizeX;
//		[HideInInspector]
//		public string SaveSizeY;

//		[HideInInspector]
//		public string DefaultSizeX = "";
//		[HideInInspector]
//		public string DefaultSizeY = "";

//		void Awake()
//		{
//			panelRectTransform = transform.parent.GetComponent<RectTransform>();
//			Vector3 pos = panelRectTransform.sizeDelta;
//			pos.x = PlayerPrefs.GetFloat(UniqueNameToSaveResizing + "SaveSizeX", pos.x);
//			pos.y = PlayerPrefs.GetFloat(UniqueNameToSaveResizing + "SaveSizeY", pos.y);
//			panelRectTransform.sizeDelta = pos;

//			PlayerPrefs.SetFloat(UniqueNameToSaveResizing + "DefaultSizeX", pos.x);
//			PlayerPrefs.SetFloat(UniqueNameToSaveResizing + "DefaultSizeY", pos.y);
//		}
//		public void ResetSize()
//		{
//			panelRectTransform = transform.parent.GetComponent<RectTransform>();
//			Vector3 pos = panelRectTransform.sizeDelta;
//			pos.x = PlayerPrefs.GetFloat(UniqueNameToSaveResizing + "DefaultSizeX", pos.x);
//			pos.y = PlayerPrefs.GetFloat(UniqueNameToSaveResizing + "DefaultSizeY", pos.y);
//			panelRectTransform.sizeDelta = pos;
//		}
//		public void OnPointerDown(PointerEventData data)
//		{
//			originalSizeDelta = panelRectTransform.sizeDelta;
//			RectTransformUtility.ScreenPointToLocalPointInRectangle(panelRectTransform, data.position, data.pressEventCamera, out originalLocalPointerPosition);
//		}

//		public void OnDrag(PointerEventData data)
//		{
//			if (panelRectTransform == null)
//				return;

//			Vector2 localPointerPosition;
//			RectTransformUtility.ScreenPointToLocalPointInRectangle(panelRectTransform, data.position, data.pressEventCamera, out localPointerPosition);
//			Vector3 offsetToOriginal = localPointerPosition - originalLocalPointerPosition;

//			Vector2 sizeDelta = originalSizeDelta + new Vector2(offsetToOriginal.x, -offsetToOriginal.y);
//			sizeDelta = new Vector2(
//				Mathf.Clamp(sizeDelta.x, minSize.x, maxSize.x),
//				Mathf.Clamp(sizeDelta.y, minSize.y, maxSize.y)
//			);

//			panelRectTransform.sizeDelta = sizeDelta;

//			PlayerPrefs.SetFloat(UniqueNameToSaveResizing + "SaveSizeX", sizeDelta.x);
//			PlayerPrefs.SetFloat(UniqueNameToSaveResizing + "SaveSizeY", sizeDelta.y);
//		}
//	}
//}