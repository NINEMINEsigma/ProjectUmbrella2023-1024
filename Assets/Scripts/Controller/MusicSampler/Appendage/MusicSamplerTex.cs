using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Base;
using UnityEngine.UI;

namespace Item.Control.Appendage
{
    public class MusicSamplerTex : MonoBehaviour
    {
        public SourceController source;
        public Texture2D texture;
        public Shader shader;
        Material material_;
        Material material
        {
            get { if (material_ == null) material_ = new(shader);return material_; }
        }

        private void Start()
        {
            GetComponent<Image>().material = material;
        }

        public void Init()
        {
            texture = MusicSampler.BakeAudioWaveform(source.GetNow());
            material.SetTexture("_MusicSamplerTex", texture);
        }
    }
}
