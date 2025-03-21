using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace MobileActionKit
{
    public class SwipeSnap : MonoBehaviour, IEndDragHandler, IBeginDragHandler, IDragHandler
    {
        public ScrollRect scrollRect;           // The ScrollRect component
        public RectTransform content;           // The content that holds the images
        public float snapSpeed = 10f;           // Speed of snapping
        public int imagesToShow = 3;            // Number of images to show at a time

        private float imageWidth;               // Width of each image (with spacing)
        private float targetPosX;               // Target X position for snapping
        private int currentImageIndex = 0;      // Index of the leftmost visible image
        private Vector2 dragStartPos;           // Start position of the drag (for touch/mouse swipe)
        private int totalImages;                // Total number of images available
        private bool isDragging = false;        // To track if the user is still dragging

        private void Start()
        {
            // Calculate the width of each image including spacing
            imageWidth = content.GetChild(0).GetComponent<RectTransform>().rect.width +
                         content.GetComponent<HorizontalLayoutGroup>().spacing;

            // Get the total number of images (children under content)
            totalImages = content.childCount;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            // Store the initial drag position (works for both mouse and touch)
            dragStartPos = eventData.position;
            isDragging = true; // Mark as dragging
        }

        public void OnDrag(PointerEventData eventData)
        {
            // Allow free dragging without snapping during the drag process
            isDragging = true;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            // When the drag ends, calculate the nearest set of fully visible images and snap to that position
            isDragging = false;

            // Calculate how far the user dragged/swiped
            float dragEndPosX = eventData.position.x;
            float swipeDistance = dragStartPos.x - dragEndPosX;

            // Determine how many full images should be visible
            int imagesToScroll = Mathf.RoundToInt(swipeDistance / imageWidth);

            // Update the index of the leftmost visible image (ensuring we stay within valid bounds)
            currentImageIndex = Mathf.Clamp(currentImageIndex + imagesToScroll, 0, Mathf.Max(0, totalImages - imagesToShow));

            // Calculate the target X position based on the new index
            targetPosX = -currentImageIndex * imageWidth;

            // Snap to that position smoothly
            StartCoroutine(SnapToPosition());
        }

        private void Update()
        {
            // Only snap when dragging stops
            if (!isDragging)
            {
                scrollRect.velocity = Vector2.zero; // Stop the ScrollRect from moving
            }
        }

        private System.Collections.IEnumerator SnapToPosition()
        {
            // Smoothly move towards the target position
            while (Mathf.Abs(content.anchoredPosition.x - targetPosX) > 0.1f)
            {
                content.anchoredPosition = Vector2.Lerp(content.anchoredPosition,
                    new Vector2(targetPosX, content.anchoredPosition.y), Time.deltaTime * snapSpeed);
                yield return null;
            }

            // Ensure it's exactly at the target position at the end
            content.anchoredPosition = new Vector2(targetPosX, content.anchoredPosition.y);
        }
    }
}




//using UnityEngine;
//using UnityEngine.UI;
//using UnityEngine.EventSystems;

//public class SwipeSnap : MonoBehaviour, IEndDragHandler, IBeginDragHandler
//{
//    public ScrollRect scrollRect;           // The ScrollRect component
//    public RectTransform content;           // The content that holds the images
//    public float snapSpeed = 10f;           // Speed of snapping
//    public int imagesToShow = 3;            // Number of images to show at a time

//    private float imageWidth;               // Width of each image (with spacing)
//    private float targetPosX;               // Target X position for snapping
//    private int currentImageIndex = 0;      // Index of the leftmost visible image
//    private Vector2 dragStartPos;           // Start position of the drag (for touch/mouse swipe)
//    private int totalImages;                // Total number of images available

//    private void Start()
//    {
//        // Calculate the width of each image including spacing
//        imageWidth = content.GetChild(0).GetComponent<RectTransform>().rect.width +
//                     content.GetComponent<HorizontalLayoutGroup>().spacing;

//        // Get the total number of images (children under content)
//        totalImages = content.childCount;
//    }

//    public void OnBeginDrag(PointerEventData eventData)
//    {
//        // Store the initial drag position (works for both mouse and touch)
//        dragStartPos = eventData.position;
//    }

//    public void OnEndDrag(PointerEventData eventData)
//    {
//        // Calculate how far the user dragged/swiped
//        float dragEndPosX = eventData.position.x;
//        float swipeDistance = dragStartPos.x - dragEndPosX;

//        // Determine how many full images should be visible
//        int imagesToScroll = Mathf.RoundToInt(swipeDistance / imageWidth);

//        // Update the index of the leftmost visible image (ensuring we stay within valid bounds)
//        currentImageIndex = Mathf.Clamp(currentImageIndex + imagesToScroll, 0, Mathf.Max(0, totalImages - imagesToShow));

//        // Calculate the target X position based on the new index
//        targetPosX = -currentImageIndex * imageWidth;

//        // Snap to that position smoothly
//        StartCoroutine(SnapToPosition());
//    }

//    private System.Collections.IEnumerator SnapToPosition()
//    {
//        // Smoothly move towards the target position
//        while (Mathf.Abs(content.anchoredPosition.x - targetPosX) > 0.1f)
//        {
//            content.anchoredPosition = Vector2.Lerp(content.anchoredPosition,
//                new Vector2(targetPosX, content.anchoredPosition.y), Time.deltaTime * snapSpeed);
//            yield return null;
//        }

//        // Ensure it's exactly at the target position at the end
//        content.anchoredPosition = new Vector2(targetPosX, content.anchoredPosition.y);
//    }
//}



//using UnityEngine;
//using UnityEngine.UI;
//using UnityEngine.EventSystems;

//public class SwipeSnap : MonoBehaviour, IEndDragHandler, IBeginDragHandler
//{
//    public ScrollRect scrollRect;           // The ScrollRect component
//    public RectTransform content;           // The content that holds the images
//    public float snapSpeed = 10f;           // Speed of snapping
//    public int imagesToShow = 3;            // Number of images to show at a time
//    public float minSwipeSpeed = 0.1f;      // Minimum swipe speed to move to next image
//    public float maxSwipeSpeed = 1.0f;      // Maximum swipe speed for the fastest scroll

//    private float imageWidth;               // Width of each image (with spacing)
//    private float targetPosX;               // Target X position for snapping
//    private int currentImageIndex = 0;      // Index of the leftmost visible image
//    private Vector2 dragStartPos;           // Start position of the drag (for touch/mouse swipe)
//    private float dragStartTime;            // Time when the drag started
//    private int totalImages;                // Total number of images available

//    private void Start()
//    {
//        // Calculate the width of each image including spacing
//        imageWidth = content.GetChild(0).GetComponent<RectTransform>().rect.width +
//                     content.GetComponent<HorizontalLayoutGroup>().spacing;

//        // Get the total number of images (children under content)
//        totalImages = content.childCount;
//    }

//    public void OnBeginDrag(PointerEventData eventData)
//    {
//        // Store the initial drag position and time (works for both mouse and touch)
//        dragStartPos = eventData.position;
//        dragStartTime = Time.time;
//    }

//    public void OnEndDrag(PointerEventData eventData)
//    {
//        // Calculate the swipe speed and direction
//        float dragEndPosX = eventData.position.x;
//        float swipeDistance = dragStartPos.x - dragEndPosX;
//        float swipeTime = Time.time - dragStartTime;
//        float swipeSpeed = Mathf.Abs(swipeDistance / swipeTime);

//        // Determine how many images to scroll based on swipe speed
//        int imagesToScroll = 1; // Default to 1 image for slow swipe

//        if (swipeSpeed > maxSwipeSpeed)
//        {
//            // Fast swipe, move 3 images
//            imagesToScroll = 3;
//        }
//        else if (swipeSpeed > minSwipeSpeed)
//        {
//            // Medium swipe, move 2 images
//            imagesToScroll = 2;
//        }

//        // Determine the direction of the swipe (left or right)
//        if (swipeDistance > 0)
//        {
//            // Swipe left, move forward in the list
//            currentImageIndex = Mathf.Min(currentImageIndex + imagesToScroll, Mathf.Max(0, totalImages - imagesToShow));
//        }
//        else if (swipeDistance < 0)
//        {
//            // Swipe right, move backward in the list
//            currentImageIndex = Mathf.Max(currentImageIndex - imagesToScroll, 0);
//        }

//        // Calculate the target X position based on the current index
//        targetPosX = -currentImageIndex * imageWidth;

//        // Snap to that position smoothly
//        StartCoroutine(SnapToPosition());
//    }

//    private System.Collections.IEnumerator SnapToPosition()
//    {
//        // Smoothly move towards the target position
//        while (Mathf.Abs(content.anchoredPosition.x - targetPosX) > 0.1f)
//        {
//            content.anchoredPosition = Vector2.Lerp(content.anchoredPosition,
//                new Vector2(targetPosX, content.anchoredPosition.y), Time.deltaTime * snapSpeed);
//            yield return null;
//        }

//        // Ensure it's exactly at the target position at the end
//        content.anchoredPosition = new Vector2(targetPosX, content.anchoredPosition.y);
//    }
//}



//using UnityEngine;
//using UnityEngine.UI;
//using UnityEngine.EventSystems;

//public class SwipeSnap : MonoBehaviour, IEndDragHandler, IBeginDragHandler
//{
//    public ScrollRect scrollRect;           // The ScrollRect component
//    public RectTransform content;           // The content that holds the images
//    public float snapSpeed = 10f;           // Speed of snapping
//    public int imagesToShow = 3;            // Number of images to show at a time

//    private float imageWidth;               // Width of each image (with spacing)
//    private float targetPosX;               // Target X position for snapping
//    private int currentImageIndex = 0;      // Index of the leftmost visible image
//    private Vector2 dragStartPos;           // Start position of the drag (for touch/mouse swipe)
//    private int totalImages;                // Total number of images available

//    private void Start()
//    {
//        // Calculate the width of each image including spacing
//        imageWidth = content.GetChild(0).GetComponent<RectTransform>().rect.width +
//                     content.GetComponent<HorizontalLayoutGroup>().spacing;

//        // Get the total number of images (children under content)
//        totalImages = content.childCount;
//    }

//    public void OnBeginDrag(PointerEventData eventData)
//    {
//        // Store the initial drag position (works for both mouse and touch)
//        dragStartPos = eventData.position;
//    }

//    public void OnEndDrag(PointerEventData eventData)
//    {
//        // Calculate the direction of the swipe (mouse or touch)
//        float dragEndPosX = eventData.position.x;
//        float swipeDirection = dragStartPos.x - dragEndPosX;

//        // Determine if we can move to the next or previous image based on the swipe direction
//        if (swipeDirection > 0)
//        {
//            // Swipe left, go to the next image if available
//            if (currentImageIndex < Mathf.Max(0, totalImages - imagesToShow))
//                currentImageIndex++;
//        }
//        else if (swipeDirection < 0)
//        {
//            // Swipe right, go to the previous image if available
//            if (currentImageIndex > 0)
//                currentImageIndex--;
//        }

//        // Calculate the target X position based on the current index
//        targetPosX = -currentImageIndex * imageWidth;

//        // Snap to that position
//        StartCoroutine(SnapToPosition());
//    }

//    private System.Collections.IEnumerator SnapToPosition()
//    {
//        // Smoothly move towards the target position
//        while (Mathf.Abs(content.anchoredPosition.x - targetPosX) > 0.1f)
//        {
//            content.anchoredPosition = Vector2.Lerp(content.anchoredPosition,
//                new Vector2(targetPosX, content.anchoredPosition.y), Time.deltaTime * snapSpeed);
//            yield return null;
//        }

//        // Ensure it's exactly at the target position at the end
//        content.anchoredPosition = new Vector2(targetPosX, content.anchoredPosition.y);
//    }
//}




//using UnityEngine;
//using UnityEngine.UI;
//using UnityEngine.EventSystems;

//public class SwipeSnap : MonoBehaviour, IEndDragHandler, IBeginDragHandler
//{
//    public ScrollRect scrollRect;           // The ScrollRect component
//    public RectTransform content;           // The content that holds the images
//    public float snapSpeed = 10f;           // Speed of snapping
//    public int imagesToShow = 3;            // Number of images to show at a time

//    private float imageWidth;               // Width of each image (with spacing)
//    private float targetPosX;               // Target X position for snapping
//    private int currentImageIndex = 0;      // Index of the leftmost visible image
//    private Vector2 dragStartPos;           // Start position of the drag (for touch/mouse swipe)

//    private void Start()
//    {
//        // Calculate the width of each image including spacing
//        imageWidth = content.GetChild(0).GetComponent<RectTransform>().rect.width +
//                     content.GetComponent<HorizontalLayoutGroup>().spacing;
//    }

//    public void OnBeginDrag(PointerEventData eventData)
//    {
//        // Store the initial drag position (works for both mouse and touch)
//        dragStartPos = eventData.position;
//    }

//    public void OnEndDrag(PointerEventData eventData)
//    {
//        // Calculate the direction of the swipe (mouse or touch)
//        float dragEndPosX = eventData.position.x;
//        float swipeDirection = dragStartPos.x - dragEndPosX;

//        // Move to the next image (left or right) depending on the swipe direction
//        if (swipeDirection > 0)
//        {
//            // Swipe left, go to next image if available
//            if (currentImageIndex < content.childCount - imagesToShow)
//                currentImageIndex++;
//        }
//        else if (swipeDirection < 0)
//        {
//            // Swipe right, go to previous image if available
//            if (currentImageIndex > 0)
//                currentImageIndex--;
//        }

//        // Calculate the target X position based on the current index
//        targetPosX = -currentImageIndex * imageWidth;

//        // Snap to that position
//        StartCoroutine(SnapToPosition());
//    }

//    private System.Collections.IEnumerator SnapToPosition()
//    {
//        // Smoothly move towards the target position
//        while (Mathf.Abs(content.anchoredPosition.x - targetPosX) > 0.1f)
//        {
//            content.anchoredPosition = Vector2.Lerp(content.anchoredPosition,
//                new Vector2(targetPosX, content.anchoredPosition.y), Time.deltaTime * snapSpeed);
//            yield return null;
//        }

//        // Ensure it's exactly at the target position at the end
//        content.anchoredPosition = new Vector2(targetPosX, content.anchoredPosition.y);
//    }
//}
