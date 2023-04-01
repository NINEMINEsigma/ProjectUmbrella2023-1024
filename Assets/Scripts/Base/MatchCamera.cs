using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Base
{
    [ExecuteAlways]
    public class MatchCamera : MonoBehaviour
    {
        RectTransform rect;
        RectTransform rectTransform
        {
            get
            {
                if (rect == null) rect = GetComponent<RectTransform>();
                return rect;
            }
        }
        public Camera camera_;

        private void Update()
        {
            rectTransform.sizeDelta = new(camera_.pixelWidth, camera_.pixelHeight);
        }

    }
}