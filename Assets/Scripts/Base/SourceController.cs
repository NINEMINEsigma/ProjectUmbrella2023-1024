using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Base
{
    [Serializable]
    public class SourceCInException : BaseInException
    {
        public SourceCInException() { }
        public SourceCInException(string message) : base(message) { }
        public SourceCInException(string message, Exception inner) : base(message, inner) { }
        public SourceCInException(MonoBehaviour mono) : base(mono.name) { }
        protected SourceCInException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

    [Serializable]
    public class SourceCOutException : BaseOutException
    {
        public SourceCOutException() { }
        public SourceCOutException(string message) : base(message) { }
        public SourceCOutException(string message, Exception inner) : base(message, inner) { }
        public SourceCOutException(SourceCInException baseIn) : base(baseIn.Message) { }
        protected SourceCOutException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

    public class SourceController : UnderlyingObject
    {
        public static SourceController Instance { get; private set; }
        public bool IsMain = false;

        private AudioSource Source_;
        public AudioSource Source
        {
            get
            {
                try
                {
                    if (Source_ == null)
                    {
                        TryGetComponent(out Source_);
                    }
                    if (Source_ == null)
                    {
                        Source_ = gameObject.AddComponent<AudioSource>();
                        throw new SourceCInException("No Source");
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogWarning(ex.Message);
                }
                return Source_;
            }
            set { Source_ = value; }
        }
        public AnimationCurve interim;
        public float duration = 5, trigger = 1;

        public AudioClip First;
        public AudioClip Second;

        public AudioClip Clip
        {
            get { return Source.clip; }
            set { Source.clip = value; }
        }
        public float Length => (Clip != null) ? Clip.length : -1;
        public AudioClip GetNow() => Clip;

        public void Offset_Func_Execute(Action func, float delay) => Add(T =>
        {
            if (delay > 0) delay -= Time.deltaTime;
            else
            {
                try { func(); } catch (SourceCInException ex) { Debug.LogWarning(ex.Message); } catch (BaseInException ex) { Debug.LogWarning(ex.Message); }
                T.state = State.Destroy;
            }
        });
        public void Offset_Func_Execute<F>(Action<F> func, float delay, F first_ = default) => Add(T =>
        {
            if (delay > 0) delay -= Time.deltaTime;
            else
            {
                try { func(first_); } catch (SourceCInException ex) { Debug.LogWarning(ex.Message); } catch (BaseInException ex) { Debug.LogWarning(ex.Message); }
                T.state = State.Destroy;
            }
        });
        public void Offset_Func_Execute<F, S>(Action<F, S> func, float delay, F first_ = default, S second_ = default) => Add(T =>
        {
            if (delay > 0) delay -= Time.deltaTime;
            else
            {
                try { func(first_, second_); } catch (SourceCInException ex) { Debug.LogWarning(ex.Message); } catch (BaseInException ex) { Debug.LogWarning(ex.Message); }
                T.state = State.Destroy;
            }
        });

        private void Start()
        {
            try
            {
                if(IsMain)
                {
                    if (Instance != null) throw new SourceCInException("The primary instance already exists");
                    Instance = this;
                }
            }
            catch(SourceCInException ex)
            {
                Debug.LogWarning(ex.Message);
            }
            if (Source.playOnAwake && First != null)
            {
                Source.Stop();
                Add(OnAwake_Delay_Play0, new() { Value = trigger });
            }
        }

        private void OnAwake_Delay_Play0(Carrier carrier)
        {
            try
            {
                if (carrier.Value <= 0)
                {
                    Source.Play();
                    Interim_Clip_Update_BT(First, duration);
                    carrier.state = State.Destroy;
                }
                else carrier.Value -= Time.deltaTime;
            }
            catch (Exception ex)
            {
                throw new SourceCOutException("private void OnAwake_Delay_Play0(Carrier carrier),unknow error", ex);
            }
        }

        public void ConversionToOtherScene()
        {
            transform.parent = null;
            DontDestroyOnLoad(gameObject);
            StartCoroutine(__ConversionToOtherScene());
        }

        public void LoadFromResources(string name)
        {
            AudioClip tmp = Resources.Load<AudioClip>(name);
            try
            {
                if (tmp == null) throw new SourceCInException("public void LoadFromResources(string name),Resources.Load<AudioClip>(name),name'file cann't found");
                else
                {
                    First = tmp;
                    Clip_Update0();
                }
            }
            catch (SourceCInException ex)
            {
                Debug.LogWarning(ex.Message);
            }
            catch (Exception ex)
            {
                throw new SourceCOutException("public void LoadFromResources(string name),unknow error", ex);
            }
        }

        public void ConversionFromTwo()
        {
            if (GetNow() == null || GetNow() != First) Clip_Update0(First);
            else Clip_Update0(Second);
        }

        public void Clip_Update0()
        {
            Clip = First;
        }

        public void Clip_Update0(AudioClip clip)
        {
            Clip = clip;
        }

        /// <summary>
        /// Recover after left
        /// </summary>
        /// <param name="clip"></param>
        /// <param name="time"></param>
        public void Interim_Clip_Update_BTB(AudioClip clip, float time = 50)
        {
            StartCoroutine(__Interim_Clip_Update_TB(clip, time));
        }

        /// <summary>
        /// Play new audio directly while transitioning
        /// </summary>
        /// <param name="clip"></param>
        /// <param name="time"></param>
        public void Interim_Clip_Update_BT(AudioClip clip, float time = 50)
        {
            StartCoroutine(__Interim_Clip_Update_BT(clip, time));
        }

        private IEnumerator __Interim_Clip_Update_BT(AudioClip clip, float Lt)
        {
            Clip_Update0(clip);
            Source.Play();
            float time = Lt;
            while (time > 0)
            {
                try
                {
                    float t = (Lt - time) / (float)Lt;
                    SetVolume(interim.Evaluate(t));
                }
                catch (Exception ex)
                {
                    throw new SourceCOutException("private IEnumerator __Interim_Clip_Update_BT(AudioClip clip, float Lt),unknow error", ex);
                }
                yield return null;
                time -= Time.deltaTime;
            }
        }

        private IEnumerator __Interim_Clip_Update_TB(AudioClip clip, float time = 50)
        {
            float Lt = time;
            while (time > 0)
            {
                try
                {
                    float t = (Lt - time) / (float)Lt;
                    SetVolume(interim.Evaluate(1 - t));
                }
                catch (Exception ex)
                {
                    throw new SourceCOutException("private IEnumerator __Interim_Clip_Update_TB(AudioClip clip, float time = 50),unknow error", ex);
                }
                yield return null;
                time -= Time.deltaTime;
            }
            StartCoroutine(__Interim_Clip_Update_BT(clip, Lt));
        }

        public void SetPlay(bool a)
        {
            if (a) Source.Play();
            else Source.Pause();
        }
        public void SetStop(bool a)
        {
            if (a) Source.Play();
            else Source.Stop();
        }
        public void SetSpeed(float s)
        {
            Source.pitch = s;
        }
        public void SetVolume(float v)
        {
            Source.volume = v;
        }

        private IEnumerator __ConversionToOtherScene()
        {
            float start = Time.time;
            while (Time.time - start < 1)
            {
                try
                {
                    //SetSpeed(Mathf.Clamp(1 - (Time.time - start), 0.95f, 1f));
                    SetVolume(1 - (Time.time - start));
                }
                catch (Exception ex)
                {
                    throw new SourceCOutException("private IEnumerator __ConversionToOtherScene(),unknow error", ex);
                }
                yield return null;
            }
            Destroy(gameObject);
        }
    }
}
