using UnityEngine;

namespace Base.AD.Auto
{
    [ExecuteInEditMode]
    public class AutoBackGround : MonoBehaviour
    {
        [SerializeField] private float factor;


        private void Awake()
        {
            Match();
        }

#if UNITY_EDITOR
        private void Update() => Match();
#endif

        private void Match()
        {
            float size = (float)Screen.width / Screen.height / (4f / 3f) * factor;
            if (size < factor) size = factor;
            transform.localScale = new Vector3(size, size, 1f);
        }
    }
}