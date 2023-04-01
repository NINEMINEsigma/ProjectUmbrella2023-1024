using System.Collections.Generic;
using UnityEngine;

public class WaterPost : MonoBehaviour
{
    [Tooltip("距离系数")]
    public float distanceFactor = 60.0f;
    [Tooltip("时间系数")]
    public float timeFactor = -30.0f;
    [Tooltip("sin函数结果系数")]
    public float totalFactor = 1.0f;
    [Tooltip("波纹宽度")]
    public float waveWidth = 0.3f;
    [Tooltip("波纹扩散的速度")]
    public float waveSpeed = 0.3f;

    private float waveStartTime;
    [SerializeField] private Vector4 startPos = new(0.5f, 1f, 0);
    [SerializeField] private bool isfix = false;
    float lasttime;
    [Header("Shader")]
    public Shader EffectShader;
    [SerializeField]private Material EffectMaterial;
    Material _Material
    {
        get
        {
            if(EffectMaterial==null)
                EffectMaterial = new Material(EffectShader);
            return EffectMaterial;
        }
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        float curWaveDistance = (Time.time - waveStartTime) * waveSpeed;
        _Material.SetFloat("_distanceFactor", distanceFactor);
        _Material.SetFloat("_timeFactor", timeFactor);
        _Material.SetFloat("_totalFactor", totalFactor);
        _Material.SetFloat("_waveWidth", waveWidth);
        _Material.SetFloat("_curWaveDis", curWaveDistance);
        _Material.SetVector("_startPos", startPos);
        Graphics.Blit(source, destination, _Material);
    }

    void Update()
    {
        if (isfix)
        { 
            if(Time.time-lasttime> waveSpeed*10)
            {
                waveStartTime = Time.time;
                _Material.SetVector("_startPos", startPos);
            }
        }
        else if (Input.GetMouseButton(0))
        {
            Vector2 mousePos = Input.mousePosition;
            startPos = new Vector4(mousePos.x / Screen.width, mousePos.y / Screen.height, 0, 0);
            waveStartTime = Time.time;
        }
    }
}