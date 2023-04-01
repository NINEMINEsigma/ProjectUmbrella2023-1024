using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainningC : MonoBehaviour
{
    public Material EffectMaterial;

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        EffectMaterial.SetFloat("Speed", 1);
        Graphics.Blit(source, destination, EffectMaterial);
        Graphics.Blit(source, destination);
    }
}
