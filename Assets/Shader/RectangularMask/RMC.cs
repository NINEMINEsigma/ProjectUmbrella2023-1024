using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class RMC : MonoBehaviour
{
    [SerializeField] float wait = 0.25f;
    [SerializeField] bool isstart = false, isinterim = false;
    [SerializeField] Shader shader;
    Image image;
    Material material_;
    Material material
    {
        get { if (material_ == null) material_ = new(shader); return material_; }
    }

    private void Awake()
    {
        image = GetComponent<Image>();
        image.material = material;
    }

    void Start()
    {
        if (isstart)
        {
            material.SetFloat("_d", -1);
            material.SetFloat("_l", 0.5f);
            material.SetFloat("_h", 0.5f);
        }
        else
        {
            material.SetFloat("_d", 2);
            material.SetFloat("_l", 0);
            material.SetFloat("_h", 1);
            if(!isinterim)Over();
        }
    }

    public void Cover()
    {
        StartCoroutine(Conversion(false));
    }

    public void Over()
    {
        StartCoroutine(Conversion(true));
    }

    IEnumerator Conversion(bool isover)
    {
        float time = wait;
        while (time > 0)
        {
            float t = time / wait;
            material.SetFloat("_d", (isover) ? Mathf.Lerp(-1, 1, t) : Mathf.Lerp(-1, 1, 1 - t));
            material.SetFloat("_l", (isover) ? Mathf.Lerp(0.5f, 0, t) : Mathf.Lerp(0.5f, 0, 1 - t));
            material.SetFloat("_h", (isover) ? Mathf.Lerp(0.5f, 1, t) : Mathf.Lerp(0.5f, 1, 1 - t));
            yield return null;
            time -= Time.deltaTime;
        }
        if(isover)
        {
            material.SetFloat("_d", -1);
            material.SetFloat("_l", 0.5f);
            material.SetFloat("_h", 0.5f);
        }
        else
        {
            material.SetFloat("_d", 2);
            material.SetFloat("_l", 0);
            material.SetFloat("_h", 1);
        }
    }
}
