using UnityEngine;

namespace UIController
{
    //这是一个投影仪，可以将这个摄像机的图像渲染到这个RenderTexture上
    [RequireComponent(typeof(Camera))]
    public class Projection : MonoBehaviour
    {
        [SerializeField] [Header("Target")] private Camera TargetCamera_;

        [SerializeField] [Header("Projection")]
        private Material TargetMaterial_;

        [SerializeField] private RenderTexture TargetRenderTexture_;

        public Camera TargetCamera
        {
            get
            {
                if (TargetCamera_ == null)
                    TargetCamera_ = GetComponent<Camera>();
                return TargetCamera_;
            }
        }

        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            TargetRenderTexture_ = source;
        }
    }
}