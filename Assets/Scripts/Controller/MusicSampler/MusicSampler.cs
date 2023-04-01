using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Item.Control
{
    public class MusicSampler : MonoBehaviour
    {
        #region Inspector
        /// <summary>
        /// 这个参数用于设置进行采样的精度
        /// </summary>
        [Tooltip("采样精度")]public SpectrumLength SpectrumCount = SpectrumLength.Spectrum256;
        private int spectrumLength => (int)Mathf.Pow(2, ((int)SpectrumCount + 6));
        /// <summary>
        /// 这个属性返回采样得到的原始数据
        /// </summary>
        [Tooltip("原始数据")]public float[] samples = new float[64];
        private int[] sampleCount = new int[8];
        /// <summary>
        /// 这个参数用于设置将采样的结果分为几组进行讨论
        /// </summary>
        [Tooltip("拆分组数")]public uint BandCount = 8;
        /// <summary>
        /// 这个参数用于设置组别采样数值减小时使用的平滑策略
        /// </summary>
        [Tooltip("平衡采样值滑落的平滑策略")]public BufferDecreasingType decreasingType = BufferDecreasingType.Jump;
        /// <summary>
        /// 这个参数用于设置在Slide和Falling设置下，组别采样数值减小时每帧下降的大小。
        /// </summary>
        [Tooltip("Slide/Falling:采样值的滑落幅度")]public float decreasing = 0.003f;
        /// <summary>
        /// 这个参数用于设置在Falling设置下，组别采样数值减小时每帧下降时加速度的大小。
        /// </summary>
        [Tooltip("Falling:采样值滑落的加速度")]public float DecreaseAcceleration = 0.2f;
        /// <summary>
        /// 这个参数用于设置组别采样数值增大时使用的平滑策略
        /// </summary>
        [Tooltip("平衡采样值提升的平滑策略")]public BufferIncreasingType increasingType = BufferIncreasingType.Jump;
        /// <summary>
        /// 这个参数用于设置在Slide设置下，组别采样数值增大时每帧增加的大小。
        /// </summary>
        [Tooltip("Slide:采样值的提升幅度")]public float increasing = 0.003f;
        /// <summary>
        /// 这个属性返回经过平滑和平均的几组数据
        /// </summary>
        [Tooltip("经处理后的数据")]public float[] bands = new float[8];
        private float[] freqBands = new float[8];
        private float[] bandBuffers = new float[8];
        private float[] bufferDecrease = new float[8];
        /// <summary>
        /// 这个属性返回总平均采样结果
        /// </summary>
        public float average
        {
            get
            {
                float average = 0;
                for (int i = 0; i < BandCount; i++)
                {
                    average += normalizedBands[i];
                }
                average /= BandCount;
                return average;
            }
        }

        private float[] bandHighest = new float[8];
        /// <summary>
        /// 这个属性返回经过平滑、平均和归一化的几组数据
        /// </summary>
        [Tooltip("经过平滑、平均和归一化的几组数据")]public float[] normalizedBands = new float[8];
        private float[] normalizedBandBuffers = new float[8];
        #endregion

        #region RequiredComponent
        [SerializeField]
        AudioSource AudioSource = null;
        #endregion

        #region LifePeriod
        private void Start()
        {
            GetSampleCount();
        }
        private void OnValidate()
        {
            if (samples.Length != spectrumLength) samples = new float[spectrumLength];
            if (bands.Length != BandCount) bands = new float[BandCount];
            if (freqBands.Length != BandCount) freqBands = new float[BandCount];
            if (bandBuffers.Length != BandCount) bandBuffers = new float[BandCount];
            if (bufferDecrease.Length != BandCount) bufferDecrease = new float[BandCount];
            if (bandHighest.Length != BandCount) bandHighest = new float[BandCount];
            if (normalizedBands.Length != BandCount) normalizedBands = new float[BandCount];
            if (normalizedBandBuffers.Length != BandCount) normalizedBandBuffers = new float[BandCount];
            if (sampleCount.Length != BandCount) sampleCount = new int[BandCount];
        }
        private void Update()
        {
            GetSpectrums();
            GetFrequencyBands();
            GetNormalizedBands();
            GetBandBuffers(increasingType, decreasingType);
            BandNegativeCheck();
        }
        #endregion

        #region Programs
        private void GetSampleCount()
        {
            float acc = (((float)((int)SpectrumCount + 6)) / BandCount);
            int sum = 0;
            int last = 0;
            for (int i = 0; i < BandCount - 1; i++)
            {
                int pow = (int)Mathf.Pow(2, acc * (i));
                sampleCount[i] = pow - sum;
                if (sampleCount[i] < last) sampleCount[i] = last;
                sum += sampleCount[i];
                last = sampleCount[i];
            }
            sampleCount[BandCount - 1] = samples.Length - sum;
        }
        private void GetSpectrums()
        {
            AudioSource.GetSpectrumData(samples, 0, FFTWindow.Blackman);
        }
        private void GetFrequencyBands()
        {
            int counter = 0;
            for (int i = 0; i < BandCount; i++)
            {
                float average = 0;
                for (int j = 0; j < sampleCount[i]; j++)
                {
                    average += samples[counter] * (counter + 1);
                    counter++;
                }
                average /= sampleCount[i];
                freqBands[i] = average * 10;
            }
        }
        private void GetNormalizedBands()
        {
            for (int i = 0; i < BandCount; i++)
            {
                if (freqBands[i] > bandHighest[i])
                {
                    bandHighest[i] = freqBands[i];
                }
            }
        }
        private void GetBandBuffers(BufferIncreasingType increasingType, BufferDecreasingType decreasingType)
        {
            for (int i = 0; i < BandCount; i++)
            {
                if (freqBands[i] > bandBuffers[i])
                {
                    switch (increasingType)
                    {
                        case BufferIncreasingType.Jump:
                            bandBuffers[i] = freqBands[i];
                            bufferDecrease[i] = decreasing;
                            break;
                        case BufferIncreasingType.Slide:
                            bufferDecrease[i] = decreasing;
                            bandBuffers[i] += increasing;
                            break;
                    }
                    if (freqBands[i] < bandBuffers[i]) bandBuffers[i] = freqBands[i];
                }
                if (freqBands[i] < bandBuffers[i])
                {
                    switch (decreasingType)
                    {
                        case BufferDecreasingType.Jump:
                            bandBuffers[i] = freqBands[i];
                            break;
                        case BufferDecreasingType.Falling:
                            bandBuffers[i] -= decreasing;
                            break;
                        case BufferDecreasingType.Slide:
                            bandBuffers[i] -= bufferDecrease[i];
                            bufferDecrease[i] *= 1 + DecreaseAcceleration;
                            break;
                    }
                    if (freqBands[i] > bandBuffers[i]) bandBuffers[i] = freqBands[i]; ;
                }
                bands[i] = bandBuffers[i];
                if (bandHighest[i] == 0) continue;
                normalizedBands[i] = (freqBands[i] / bandHighest[i]);
                normalizedBandBuffers[i] = (bandBuffers[i] / bandHighest[i]);
                if (normalizedBands[i] > normalizedBandBuffers[i])
                {
                    switch (increasingType)
                    {
                        case BufferIncreasingType.Jump:
                            normalizedBandBuffers[i] = normalizedBands[i];
                            bufferDecrease[i] = decreasing;
                            break;
                        case BufferIncreasingType.Slide:
                            bufferDecrease[i] = decreasing;
                            normalizedBandBuffers[i] += increasing;
                            break;
                    }
                    if (normalizedBands[i] < normalizedBandBuffers[i]) normalizedBandBuffers[i] = normalizedBands[i];
                }
                if (normalizedBands[i] < normalizedBandBuffers[i])
                {
                    switch (decreasingType)
                    {
                        case BufferDecreasingType.Jump:
                            normalizedBandBuffers[i] = normalizedBands[i];
                            break;
                        case BufferDecreasingType.Falling:
                            normalizedBandBuffers[i] -= decreasing;
                            break;
                        case BufferDecreasingType.Slide:
                            normalizedBandBuffers[i] -= bufferDecrease[i];
                            bufferDecrease[i] *= 1 + DecreaseAcceleration;
                            break;
                    }
                    if (normalizedBands[i] > normalizedBandBuffers[i]) normalizedBandBuffers[i] = normalizedBands[i];
                }
                normalizedBands[i] = normalizedBandBuffers[i];
            }
        }
        private void BandNegativeCheck()
        {
            for (int i = 0; i < BandCount; i++)
            {
                if (bands[i] < 0)
                {
                    bands[i] = 0;
                }
                if (normalizedBands[i] < 0)
                {
                    normalizedBands[i] = 0;
                }
            }
        }
        #endregion

        /// <summary>
        /// 通过这个函数来生成一个MusicSampler,并初始化其播放的片段为audioClip
        /// </summary>
        /// <param name="audioClip">播放的片段</param>
        /// <returns></returns>
        public static MusicSampler CreateSampler(AudioClip audioClip)
        {
            GameObject go = new GameObject("MusicPlayer");
            AudioSource asr = go.AddComponent<AudioSource>();
            asr.clip = audioClip;
            asr.loop = false;
            asr.Play();
            MusicSampler ms = go.AddComponent<MusicSampler>();
            return ms;
        }
        
        // 传入一个AudioClip 会将AudioClip上挂载的音频文件生成频谱到一张Texture2D上
        public static Texture2D BakeAudioWaveform(AudioClip _clip)
        {
            int resolution = 60;    // 这个值可以控制频谱的密度
            int width = 1920;       // 这个是最后生成的Texture2D图片的宽度
            int height = 200;       // 这个是最后生成的Texture2D图片的高度

            resolution = _clip.frequency / resolution;

            float[] samples = new float[_clip.samples * _clip.channels];
            _clip.GetData(samples, 0);

            float[] waveForm = new float[(samples.Length / resolution)];

            float min = 0;
            float max = 0;
            bool inited = false;

            for (int i = 0; i < waveForm.Length; i++)
            {
                waveForm[i] = 0;

                for (int j = 0; j < resolution; j++)
                {
                    waveForm[i] += Mathf.Abs(samples[(i * resolution) + j]);
                }

                if (!inited)
                {
                    min = waveForm[i];
                    max = waveForm[i];
                    inited = true;
                }
                else
                {
                    if (waveForm[i] < min)
                    {
                        min = waveForm[i];
                    }

                    if (waveForm[i] > max)
                    {
                        max = waveForm[i];
                    }
                }
                //waveForm[i] /= resolution;
            }


            Color backgroundColor = Color.black;
            Color waveformColor = Color.green;
            Color[] blank = new Color[width * height];
            Texture2D texture = new Texture2D(width, height);

            for (int i = 0; i < blank.Length; ++i)
            {
                blank[i] = backgroundColor;
            }

            texture.SetPixels(blank, 0);

            float xScale = (float)width / (float)waveForm.Length;

            int tMid = (int)(height / 2.0f);
            float yScale = 1;

            if (max > tMid)
            {
                yScale = tMid / max;
            }

            for (int i = 0; i < waveForm.Length; ++i)
            {
                int x = (int)(i * xScale);
                int yOffset = (int)(waveForm[i] * yScale);
                int startY = tMid - yOffset;
                int endY = tMid + yOffset;

                for (int y = startY; y <= endY; ++y)
                {
                    texture.SetPixel(x, y, waveformColor);
                }
            }

            texture.Apply();
            return texture;
        }
    }
    public enum SpectrumLength
    {
        Spectrum64, Spectrum128, Spectrum256, Spectrum512, Spectrum1024, Spectrum2048, Spectrum4096, Spectrum8192
    }
    public enum BufferDecreasingType
    {
        Jump, Slide, Falling
    }
    public enum BufferIncreasingType
    {
        Jump, Slide
    }
}
