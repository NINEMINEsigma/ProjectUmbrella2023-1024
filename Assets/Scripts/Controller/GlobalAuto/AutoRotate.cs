using UnityEngine;
using Base;
using System.Collections.Generic;

namespace Base.AD.Auto
{
    public class AutoRotate : MonoBehaviour
    {
        [SerializeField, Tooltip("触发")] private float trigger = 1;
        [SerializeField, Tooltip("持续")] private float duration = 1;
        [SerializeField] private Vector3 from, to;
        [SerializeField] private AnimationCurve curve;
        [SerializeField] private bool isLocal, isLoop, isSmooth;
        [SerializeField, Tooltip("使用序数")] private bool isSort;
        [SerializeField, Tooltip("序数")] private float index = 0;
        [SerializeField, Tooltip("控制的子件")] private List<AutoRotate> subobj;
        [SerializeField, Tooltip("可见序数")] private int shownindex = 1;
        private float _increment, _derta;
        public void indexChange(float increment)
        {
            foreach (var it in subobj)
            {
                it._increment = increment;
                it._derta = Mathf.Abs(increment);
            }
        }

        [Range(0, 15)] public float speed = 1;


        private float time;

        private void Awake()
        {
            subobj.Add(this);
            time = 0;
        }

        private void Update()
        {

            if (isSort) SortAuto();
            else LocalAuto();
        }

        private void SortAuto()
        {
            if (_derta > 0)
            {
                float tain = (isSmooth ? Time.smoothDeltaTime : Time.deltaTime) * speed;
                index += ((_increment > 0) ? 1 : -1) * tain;
                _derta -= tain;
            }
            if (_derta < 0) _derta = 0;
            float t = Mathf.Clamp((index + shownindex) / (2.0f * shownindex), 0, 1);
            SetEulerAngles(curve.Evaluate(t));//progress
        }

        private void SetEulerAngles(float t)
        {
            if (isLocal)
            {
                transform.localEulerAngles = EasingFunction.Curve(from, to, t);
            }
            else
            {
                transform.eulerAngles = EasingFunction.Curve(from, to, t);
            }
        }

        private void LocalAuto()
        {
            time += isSmooth ? Time.smoothDeltaTime : Time.deltaTime;
            if (time < trigger || time > duration + trigger && !isLoop)
            {
                return;
            }
            SetEulerAngles((float)curve.Evaluate(time % duration / duration));//progress
        }
    }
}
