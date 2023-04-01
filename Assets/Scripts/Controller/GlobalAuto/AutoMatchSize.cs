using UnityEngine;
using UnityEngine.UI;

namespace Base.AD.Auto
{
    [ExecuteInEditMode, RequireComponent(typeof(CanvasScaler))]
    public class AutoMatchSize : MonoBehaviour
    {
        [SerializeField] private float max, min;

        private CanvasScaler scaler;

        private void Awake()
        {
            scaler = GetComponent<CanvasScaler>();
            Match();
        }

#if UNITY_EDITOR
        private void Update() => Match();
#endif

        private void Match()
        {
            scaler.matchWidthOrHeight = ((float)(Screen.width / Screen.height) > (4f / 3f) ? max : min);
        }
    }
}
