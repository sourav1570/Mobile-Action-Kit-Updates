using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

namespace MobileActionKit
{
	public class DragImage : MonoBehaviour, IPointerDownHandler, IDragHandler
	{
		[TextArea]	
		public string ScriptInfo = "This script enables dragging UI elements within a defined parent container. The position is saved and loaded using PlayerPrefs.";

		[Tooltip("A unique identifier used to save and retrieve the dragging position from PlayerPrefs.")]
		public string UniqueNameToSaveDragging = "";

		private Vector2 originalLocalPointerPosition;
		private Vector3 originalPanelLocalPosition;
		private RectTransform panelRectTransform;
		private RectTransform parentRectTransform;

		[HideInInspector]
		public string SavePositionX;
		[HideInInspector]
		public string SavePositionY;

		[HideInInspector]
		public string DefaultPositionX = "";
		[HideInInspector]
		public string DefaultPositionY = "";

		[HideInInspector]
		public bool EnableDragging = false;

		void Awake()
		{
			panelRectTransform = transform as RectTransform;
			parentRectTransform = transform.parent as RectTransform;
		}
		private void Start()
		{
			Vector3 pos = panelRectTransform.localPosition;
			pos.x = PlayerPrefs.GetFloat(UniqueNameToSaveDragging + "SavePositionX", pos.x);
			pos.y = PlayerPrefs.GetFloat(UniqueNameToSaveDragging + "SavePositionY", pos.y);
			panelRectTransform.localPosition = pos;

			if (PlayerPrefs.GetInt(UniqueNameToSaveDragging + "SaveDraggingDefaultValues") == 0)
			{
				PlayerPrefs.SetFloat(UniqueNameToSaveDragging + "DefaultPositionX", pos.x);
				PlayerPrefs.SetFloat(UniqueNameToSaveDragging + "DefaultPositionY", pos.y);
				PlayerPrefs.SetInt(UniqueNameToSaveDragging + "SaveDraggingDefaultValues", 1);
			}


		}
		public void ResetDrag()
		{
			Vector3 pos = panelRectTransform.localPosition;
			PlayerPrefs.SetInt(UniqueNameToSaveDragging + "SaveDraggingDefaultValues", 0);
			pos.x = PlayerPrefs.GetFloat(UniqueNameToSaveDragging + "DefaultPositionX", pos.x);
			pos.y = PlayerPrefs.GetFloat(UniqueNameToSaveDragging + "DefaultPositionY", pos.y);
			panelRectTransform.localPosition = pos;
		}
		public void OnPointerDown(PointerEventData data)
		{
			if(EnableDragging == true)
            {
				originalPanelLocalPosition = panelRectTransform.localPosition;
				RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRectTransform, data.position, data.pressEventCamera, out originalLocalPointerPosition);
			}		
		}

		public void OnDrag(PointerEventData data)
		{
			if (EnableDragging == true)
			{
				if (panelRectTransform == null || parentRectTransform == null)
					return;

				Vector2 localPointerPosition;
				if (RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRectTransform, data.position, data.pressEventCamera, out localPointerPosition))
				{
					Vector3 offsetToOriginal = localPointerPosition - originalLocalPointerPosition;
					panelRectTransform.localPosition = originalPanelLocalPosition + offsetToOriginal;
				}

				ClampToWindow();
			}
		}

		// Clamp panel to area of parent
		void ClampToWindow()
		{
			Vector3 pos = panelRectTransform.localPosition;

			Vector3 minPosition = parentRectTransform.rect.min - panelRectTransform.rect.min;
			Vector3 maxPosition = parentRectTransform.rect.max - panelRectTransform.rect.max;

			pos.x = Mathf.Clamp(panelRectTransform.localPosition.x, minPosition.x, maxPosition.x);
			pos.y = Mathf.Clamp(panelRectTransform.localPosition.y, minPosition.y, maxPosition.y);

			panelRectTransform.localPosition = pos;

			PlayerPrefs.SetFloat(UniqueNameToSaveDragging + "SavePositionX", pos.x);
			PlayerPrefs.SetFloat(UniqueNameToSaveDragging + "SavePositionY", pos.y);
		}
		public void SaveSettings()
		{
			Vector3 pos = panelRectTransform.localPosition;
			pos.x = PlayerPrefs.GetFloat(UniqueNameToSaveDragging + "SavePositionX", pos.x);
			pos.y = PlayerPrefs.GetFloat(UniqueNameToSaveDragging + "SavePositionY", pos.y);
			panelRectTransform.localPosition = pos;
		}

	}
}



//using UnityEngine;
//using UnityEngine.UI;
//using UnityEngine.EventSystems;
//using System.Collections;

//namespace MobileActionKit
//{
//	public class DragPanel : MonoBehaviour, IPointerDownHandler, IDragHandler
//	{
//		public string UniqueNameToSaveDragging = "";
//		private Vector2 originalLocalPointerPosition;
//		private Vector3 originalPanelLocalPosition;
//		private RectTransform panelRectTransform;
//		private RectTransform parentRectTransform;

//		[HideInInspector]
//		public string SavePositionX;
//		[HideInInspector]
//		public string SavePositionY;

//		[HideInInspector]
//		public string DefaultPositionX = "";
//		[HideInInspector]
//		public string DefaultPositionY = "";

//		void Awake()
//		{
//			panelRectTransform = transform.parent as RectTransform;
//			parentRectTransform = panelRectTransform.parent as RectTransform;
//		}
//		private void Start()
//		{
//			Vector3 pos = panelRectTransform.localPosition;
//			pos.x = PlayerPrefs.GetFloat(UniqueNameToSaveDragging + "SavePositionX", pos.x);
//			pos.y = PlayerPrefs.GetFloat(UniqueNameToSaveDragging + "SavePositionY", pos.y);
//			panelRectTransform.localPosition = pos;

//			PlayerPrefs.SetFloat(UniqueNameToSaveDragging + "DefaultPositionX", pos.x);
//			PlayerPrefs.SetFloat(UniqueNameToSaveDragging + "DefaultPositionY", pos.y);

//		}
//		public void ResetDrag()
//        {
//			Vector3 pos = panelRectTransform.localPosition;
//			pos.x = PlayerPrefs.GetFloat(UniqueNameToSaveDragging + "DefaultPositionX", pos.x);
//			pos.y = PlayerPrefs.GetFloat(UniqueNameToSaveDragging + "DefaultPositionY", pos.y);
//			panelRectTransform.localPosition = pos;
//		}
//		public void OnPointerDown(PointerEventData data)
//		{
//			originalPanelLocalPosition = panelRectTransform.localPosition;
//			RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRectTransform, data.position, data.pressEventCamera, out originalLocalPointerPosition);
//		}

//		public void OnDrag(PointerEventData data)
//		{
//			if (panelRectTransform == null || parentRectTransform == null)
//				return;

//			Vector2 localPointerPosition;
//			if (RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRectTransform, data.position, data.pressEventCamera, out localPointerPosition))
//			{
//				Vector3 offsetToOriginal = localPointerPosition - originalLocalPointerPosition;
//				panelRectTransform.localPosition = originalPanelLocalPosition + offsetToOriginal;
//			}

//			ClampToWindow();
//		}

//		// Clamp panel to area of parent
//		void ClampToWindow()
//		{
//			Vector3 pos = panelRectTransform.localPosition;

//			Vector3 minPosition = parentRectTransform.rect.min - panelRectTransform.rect.min;
//			Vector3 maxPosition = parentRectTransform.rect.max - panelRectTransform.rect.max;

//			pos.x = Mathf.Clamp(panelRectTransform.localPosition.x, minPosition.x, maxPosition.x);
//			pos.y = Mathf.Clamp(panelRectTransform.localPosition.y, minPosition.y, maxPosition.y);

//			panelRectTransform.localPosition = pos;

//			PlayerPrefs.SetFloat(UniqueNameToSaveDragging + "SavePositionX", pos.x);
//			PlayerPrefs.SetFloat(UniqueNameToSaveDragging + "SavePositionY", pos.y);
//		}
//		public void SaveSettings()
//		{
//			Vector3 pos = panelRectTransform.localPosition;
//			pos.x = PlayerPrefs.GetFloat(UniqueNameToSaveDragging + "SavePositionX", pos.x);
//			pos.y = PlayerPrefs.GetFloat(UniqueNameToSaveDragging + "SavePositionY", pos.y);
//			panelRectTransform.localPosition = pos;
//		}

//	}
//}
