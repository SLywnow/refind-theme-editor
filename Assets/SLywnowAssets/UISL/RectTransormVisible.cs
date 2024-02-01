using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SLywnow
{
    public static class RectTransormExtensions 
    {
        public static int CountVisibleCorners(RectTransform rectTransform)
        {
            Rect screenBounds = new Rect(0f, 0f, Screen.width, Screen.height); 
            Vector3[] objectCorners = new Vector3[4];
            rectTransform.GetWorldCorners(objectCorners);

            int visibleCorners = 0;
            Vector3 tempScreenSpaceCorner; 
            for (var i = 0; i < objectCorners.Length; i++) 
            {
                if (screenBounds.Contains(objectCorners[i])) 
                {
                    visibleCorners++;
                }
            }
            return visibleCorners;
        }

        public static bool IsFullyVisible(RectTransform rectTransform)
        {
            return CountVisibleCorners(rectTransform) == 4; 
        }

        public static bool IsVisible(RectTransform rectTransform)
        {
            return CountVisibleCorners(rectTransform) > 0; 
        }
    }
}