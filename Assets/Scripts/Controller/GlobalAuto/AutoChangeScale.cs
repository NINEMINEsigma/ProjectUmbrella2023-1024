using UnityEngine;

namespace Base.AD.Auto
{
    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    public class AutoChangeScale : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private Vector3 originalScale;
        [SerializeField] private bool withWideScreen, withPadScreen = true, inverse, inverse2;

        private void Awake() => Match();

#if UNITY_EDITOR
        private void Update() => Match();
#endif

        private void Match()
        {
            if (!target) target = transform;
            var delta = (float)Screen.width / Screen.height / (4f / 3f);
            if (!withWideScreen) delta = Mathf.Min(delta, 1f);
            if (!withPadScreen) delta = Mathf.Max(delta, 1f);
            if (inverse) delta = 1f / delta;
            if (inverse && inverse2 && delta < 1f) delta = 1f / delta;
            target.localScale = originalScale * delta;
        }
    }
}