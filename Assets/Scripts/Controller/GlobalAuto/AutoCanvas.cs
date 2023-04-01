using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Item.UI
{
    public class AutoCanvas : MonoBehaviour
    {
        [SerializeField] Camera Camera;
        [SerializeField] RectTransform Canvas;
        //[SerializeField] float Distance = 10;

        private void Awake()
        {
            Vector3[] cam_c = Camera.GetCameraFovPositionByDistance(Vector3.Distance(Camera.transform.position, Canvas.transform.position));
            Vector3[] can_c = Canvas.GetRect();
            float y_t = Vector3.Distance(cam_c[0], cam_c[1]) / Vector3.Distance(can_c[0], can_c[1]);
            float x_t = Vector3.Distance(cam_c[1], cam_c[2]) / Vector3.Distance(can_c[1], can_c[2]);
            Canvas.transform.localScale = new Vector3(Canvas.transform.localScale.x * x_t, Canvas.transform.localScale.y * y_t, Canvas.transform.localScale.z);
            Debug.Log(x_t + " " + y_t);
        }
        /*private void Update()
        {
            if (Camera == null || Canvas == null || Distance <= 0) return;
            Canvas.position = Camera.transform.position + Camera.transform.forward * Distance;
            var corners = Camera.GetCameraFovPositionByDistance(Distance);
            Canvas.sizeDelta = new((corners[3] - corners[2]).magnitude, (corners[0] - corners[2]).magnitude);
            Canvas.localRotation = Camera.transform.localRotation;
        }*/
        /// <summary>
        /// 使sprite铺满整个屏幕
        /// </summary>
        /*private void MatchingAnimatorToCamera(GameObject _objanimator)
        {
           /* if (_objanimator != null)
            {
                m_AnimatorSprite = _objanimator.GetComponent<SpriteRenderer>();
            }
           
            Vector3 scale = _objanimator.transform.localScale;
            float cameraheight = Camera.main.orthographicSize * 2;
            float camerawidth = cameraheight * Camera.main.aspect;

            if (cameraheight >= camerawidth)
            {
                scale *= cameraheight / Canvas.transform.bounds.size.y;
            }
            else
            {
                scale *= camerawidth / m_AnimatorSprite.bounds.size.x;
            }
            _objanimator.transform.localScale = scale;
            _objanimator.transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, _objanimator.transform.position.z);
        }*/

    }

    static public class _Camera_
    {/// <summary>
     /// 获取指定距离下相机视口四个角的坐标
     /// </summary>
     /// <param name="cam"></param>
     /// <param name="distance">相对于相机的距离</param>
     /// <returns></returns>
        public static Vector3[] GetCameraFovPositionByDistance(this Camera cam, float distance)
        {
            Vector3[] corners = new Vector3[4];
            Vector3[] corners_ = new Vector3[4];

            cam.CalculateFrustumCorners(new Rect(0, 0, 1, 1), distance, Camera.MonoOrStereoscopicEye.Mono, corners_);

            corners[0] = cam.transform.TransformPoint(corners_[0]);
            corners[1] = cam.transform.TransformPoint(corners_[1]);
            corners[2] = cam.transform.TransformPoint(corners_[2]);
            corners[3] = cam.transform.TransformPoint(corners_[3]);
            /*Vector3 v = corners[0];
            var worldSpaceCorner = cam.transform.TransformVector(v);
            Debug.DrawRay(cam.transform.position, worldSpaceCorner, Color.blue);
            v = corners[1];
            worldSpaceCorner = cam.transform.TransformVector(v);
            Debug.DrawRay(cam.transform.position, worldSpaceCorner, Color.red);
            v = corners[2];
            worldSpaceCorner = cam.transform.TransformVector(v);
            Debug.DrawRay(cam.transform.position, worldSpaceCorner, Color.white);*/
            return corners;
        }
    }

    static public class _Rect_
    {
        public static Vector3[] GetRect(this RectTransform rect)
        {
            Vector3[] corners = new Vector3[4];
            rect.GetWorldCorners(corners);
            /*Debug.DrawRay(rect.transform.position, corners[0], Color.blue);
            Debug.DrawRay(rect.transform.position, corners[1], Color.red);
            Debug.DrawRay(rect.transform.position, corners[2], Color.white);*/
            return corners;
        }
    }

}