using MobileActionKit;
using UnityEngine;
using UnityEngine.EventSystems;


namespace MobileActionKit
{
    [RequireComponent(typeof(WeaponId))]
    public class DraggableImage : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        private RectTransform rectTransform;
        private CanvasGroup canvasGroup;

        void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            canvasGroup = GetComponent<CanvasGroup>();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            canvasGroup.blocksRaycasts = false; // Allow slot manager to detect proximity
        }

        public void OnDrag(PointerEventData eventData)
        {
            rectTransform.anchoredPosition += eventData.delta;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            canvasGroup.blocksRaycasts = true; // Reset raycast blocking after dragging
        }
    }
}
