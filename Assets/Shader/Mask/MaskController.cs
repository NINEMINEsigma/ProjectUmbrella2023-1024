using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[Serializable]public class MaskEvent : UnityEvent<MaskMessage> { }
[Serializable]public struct MaskMessage
{
    public float _DistanceKey;
    public float _Distance;
    public float _low_xClip;
    public float _high_xClip;
    public float _low_yClip;
    public float _high_yClip;
    public float time;
}

public class MaskController : MonoBehaviour
{
    [SerializeField] Shader shader;
    [SerializeField] Image image;
    [SerializeField, Tooltip("触发")] private float trigger = 0;
    [SerializeField, Tooltip("持续")] private float duration = 1;
    [SerializeField]
    MaskEvent maskEvent;
    [Header("目标")]
    [SerializeField,Tooltip("高斯模糊倍率(0,1000)")]int DistanceKey = 1;
    [SerializeField, Tooltip("高斯模糊等级(0,1)")] float Distance = 0;
    [SerializeField] float Left = 0;
    [SerializeField] float Right=1;
    [SerializeField] float Bottom = 0;
    [SerializeField] float High = 1;
    [SerializeField, Header("曲线")]
    AnimationCurve curve = AnimationCurve.Linear(0, 0, 1, 1);

    Material material_;
    Material material
    {
        get { if (material_ == null) material_ = new(shader); return material_; }
    }

    private void Start()
    {
        if (image == null) image = GetComponent<Image>();
        image.material = material;
    }

    private void Update()
    {
        if (Time.time < trigger) return;
        float t = (Time.time % duration) / duration;
        MaskMessage message = new()
        {
            time = t,
            _DistanceKey = DistanceKey * 0.001f,
            _Distance = Distance * 0.001f,
            _low_xClip = Left,
            _high_xClip = Right,
            _low_yClip = Bottom,
            _high_yClip = High
        };
        maskEvent.Invoke(message);
    }

    [SerializeField, Header("Loading")] float time_long = 0.25f;
    [SerializeField]float time_per = 1.25f;
    public void Loading_Mask(MaskMessage from)
    {
        material.SetFloat("_low_xClip", curve.Evaluate(Mathf.Clamp(from.time * time_per - time_long, 0, 1)));
        material.SetFloat("_high_xClip", curve.Evaluate(Mathf.Clamp(from.time * time_per + time_long, 0, 1)));
    }

}
