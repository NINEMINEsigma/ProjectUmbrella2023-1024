using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Base;

namespace Item.Control.Appendage
{
    [RequireComponent(typeof(LineRenderer))]
    public class MusicSamplerLine : MonoBehaviour
    {
        enum Type
        {
            Spectrum64, Spectrum128, Spectrum256, Spectrum512, Spectrum1024, Spectrum2048, Spectrum4096, Spectrum8192, bands
        }

        LineRenderer lineRenderer_;
        LineRenderer lineRenderer
        {
            get { if (lineRenderer_ == null) lineRenderer_ = GetComponent<LineRenderer>();return lineRenderer_; }
        }
        [SerializeField] MusicSampler sampler;
        [SerializeField] Type sampler_Type = Type.bands;
        [SerializeField] protected float Distance = 2.5f;
        [SerializeField] AnimationCurve curve = AnimationCurve.Linear(0, 0, 1, 1);
        [SerializeField] protected int vextcount = 256;

        private void Update() 
        {
            vextcount = (int)sampler.BandCount;
            Keyframe[] keyframes = new Keyframe[vextcount * 2];
            {
                /*
                switch (sampler_Type)
                {
                    case Type.Spectrum64:
                        sampler.SpectrumCount = SpectrumLength.Spectrum64;
                        keyframes = new Keyframe[128];
                        break;
                    case Type.Spectrum128:
                        sampler.SpectrumCount = SpectrumLength.Spectrum128;
                        keyframes = new Keyframe[128];
                        break;
                    case Type.Spectrum256:
                        sampler.SpectrumCount = SpectrumLength.Spectrum256;
                        keyframes = new Keyframe[128];
                        break;
                    case Type.Spectrum512:
                        sampler.SpectrumCount = SpectrumLength.Spectrum512;
                        keyframes = new Keyframe[1024];
                        break;
                    case Type.Spectrum1024:
                        sampler.SpectrumCount = SpectrumLength.Spectrum1024;
                        keyframes = new Keyframe[1024];
                        break;
                    case Type.Spectrum2048:
                        sampler.SpectrumCount = SpectrumLength.Spectrum2048;
                        keyframes = new Keyframe[1024];
                        break;
                    case Type.Spectrum4096:
                        sampler.SpectrumCount = SpectrumLength.Spectrum4096;
                        keyframes = new Keyframe[4096];
                        break;
                    case Type.Spectrum8192:
                        sampler.SpectrumCount = SpectrumLength.Spectrum8192;
                        keyframes = new Keyframe[8192];
                        break;
                    default:
                        sampler.SpectrumCount = SpectrumLength.Spectrum256;
                        keyframes = new Keyframe[8];
                        return;
                }
                */
            }
            //var vexts = GenerateNode();
            Vector3[] vexts = new Vector3[vextcount];
            SetVexts(vexts);
            lineRenderer.positionCount = vextcount;
            lineRenderer.SetPositions(/*vexts*/vexts);
            AnimationCurve m_curve = AnimationCurve.Linear(0, 1, 1, 1);
            SetKeyframes(keyframes);
            m_curve.keys = keyframes;
            lineRenderer.widthCurve = m_curve;
        }

        virtual public void SetVexts(Vector3[] vexts)
        {
            for (int i = 0; i < vextcount; i++)
            {
                float vextcount_f = vextcount;
                vexts[i] = transform.position + Distance * new Vector3(Mathf.Cos(i / vextcount_f * 2 * Mathf.PI), Mathf.Sin(i / vextcount_f * 2 * Mathf.PI));
            }
        }

        /*public List<Vector3> GenerateNode()
        {
            List<Vector3> cat = new();
            switch (sampler_Type)
            {
                case Type.Spectrum64:
                case Type.Spectrum128:
                case Type.Spectrum256:
                    {
                        int x128 = 128 / Pos.Count;
                        for (int i = 0; i < 128; i++)
                        {
                            Vector3 vext = EasingFunction.Curve(Pos[i / x128], Pos[(i / x128) + 1], curve.Evaluate(i / x128));
                            cat.Add(vext);
                        }
                        lineRenderer.positionCount = 128;
                    }
                    break;
                case Type.Spectrum512:
                case Type.Spectrum1024:
                case Type.Spectrum2048:
                    {
                        int x1024 = 1024 / Pos.Count;
                        for (int i = 0; i < 128; i++)
                        {
                            Vector3 vext = EasingFunction.Curve(Pos[i / x1024], Pos[(i / x1024) + 1], curve.Evaluate(i / x1024));
                            cat.Add(vext);
                        }
                        lineRenderer.positionCount = 1024;
                    }
                    break;
                case Type.Spectrum4096:
                    {
                        int x4096 = 4096 / Pos.Count;
                        for (int i = 0; i < 128; i++)
                        {
                            Vector3 vext = EasingFunction.Curve(Pos[i / x4096], Pos[(i / x4096) + 1], curve.Evaluate(i / x4096));
                            cat.Add(vext);
                        }
                        lineRenderer.positionCount = 4096;
                    }
                    break;
                case Type.Spectrum8192:
                    {
                        int x8192 = 8192 / Pos.Count;
                        for (int i = 0; i < 128; i++)
                        {
                            Vector3 vext = EasingFunction.Curve(Pos[i / x8192], Pos[(i / x8192) + 1], curve.Evaluate(i / x8192));
                            cat.Add(vext);
                        }
                        lineRenderer.positionCount = 8192;
                    }
                    break;
                default:
                    {
                        int x256 = 256 / Pos.Count;
                        for (int i = 0; i < 128; i++)
                        {
                            Vector3 vext = EasingFunction.Curve(Pos[i / x256], Pos[(i / x256) + 1], curve.Evaluate(i / x256));
                            cat.Add(vext);
                        }
                        lineRenderer.positionCount = 256;
                    }
                    break;
            }
            return cat;
        }*/

        public void SetKeyframes(Keyframe[] from)
        {
            for (int i = 0; i < vextcount; i++)
            {
                float vextcount_f = vextcount * 2;
                from[i].time = i / vextcount_f;
                from[i].value = Mathf.Clamp(sampler.bands[i], 0.05f, 100);
                from[^(i + 1)].time = 1 - i / vextcount_f;
                from[^(i + 1)].value = Mathf.Clamp(sampler.bands[i], 0.05f, 100);
            }
        }
    }
}