using System;
using System.Collections;
using System.Collections.Generic;
//using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Base.Start;

namespace Base
{
    [Serializable]
    public class SceneCInException : BaseInException
    {
        public SceneCInException() { }
        public SceneCInException(string message) : base(message) { }
        public SceneCInException(string message, Exception inner) : base(message, inner) { }
        public SceneCInException(MonoBehaviour mono) : base(mono.name) { }
        protected SceneCInException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

    [Serializable]
    public class SceneCOutException : BaseOutException
    {
        public SceneCOutException() { }
        public SceneCOutException(string message) : base(message) { }
        public SceneCOutException(string message, Exception inner) : base(message, inner) { }
        public SceneCOutException(SceneCInException baseIn) : base(baseIn.Message) { }
        protected SceneCOutException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

    [Serializable]public class SceneEvent : UnityEvent { }

    public class SceneController : UnderlyingObject
    {
        public static List<string> SceneMap = new();
        private static string InterimScene_ = "";
        public static string InterimScene
        {
            get
            {
                return (InterimScene_.Length <= 0) ? "Scenes/InterimScene" : InterimScene_;
            }
            set
            {
                Debug.LogWarning("InterimScene is change:" + InterimScene_ + "->" + value);
                InterimScene_ = value;
            }
        }

        [Header("State")]
        public bool auto = false;
        public bool IsEntry = false;
        [Header("Asset")]
        private AsyncOperation BackAsyncOperation_, NextAsyncOperation_, ReturnAsyncOperation_;
        private AsyncOperation BackAsyncOperation
        {
            get
            {
                if (BackAsyncOperation_ == null)  BackAsyncOperation_ = SceneManager.LoadSceneAsync(back);
                return BackAsyncOperation_;
            }
            set
            {
                BackAsyncOperation_ = value;
            }
        }
        private AsyncOperation NextAsyncOperation
        {
            get
            {
                if (NextAsyncOperation_ == null) NextAsyncOperation_ = SceneManager.LoadSceneAsync(next);
                return NextAsyncOperation_;
            }
            set
            {
                NextAsyncOperation_ = value;
            }
        }
        private AsyncOperation ReturnAsyncOperation
        {
            get
            {
                if (ReturnAsyncOperation_ == null) ReturnAsyncOperation_ = SceneManager.LoadSceneAsync(current.name);
                return ReturnAsyncOperation_;
            }
            set
            {
                ReturnAsyncOperation_ = value;
            }
        }
        public Scene current => SceneManager.GetActiveScene();
        [SerializeField] private string NeedBackScene;
        [SerializeField]private string back_;
        private string back
        {
            get { if (NeedBackScene.Length == 0) return back_; else return NeedBackScene; }
            set { back_ = value; }
        }
        public string next;
        [Tooltip("If this Scene is the end scene,you must set it true")] public bool IsEnd = false;
        public float wait;
        public bool IsAdditive = false;
        [Header("Event")]
        [SerializeField] SceneEvent OnBack, OnNext, OnReturn;

        private void Awake()
        {
            try
            {
                if (next.Length == 0 && !IsEnd) throw new SceneCInException("Is this Scene end?");
            }
            catch (SceneCInException ex)
            {
                if (back.Length == 0) throw new SceneCOutException(ex);
                else { Debug.LogWarning(ex.Message); Back(); }
            }

        }

        private void Start()
        {
            try
            {
                if (auto) interim_auto();
                Information.Information info = null;
                if (LaunchObject.instance == null && !IsEntry) throw new SceneCOutException("LaunchObject is unable to catch");
                if (!IsEntry)
                {
                    info = LaunchObject.instance.info;
                    if (info == null) throw new SceneCInException("LaunchObject's information is null");
                    __InitCurrentSceneInfo(info);
                }
                if (!SceneMap.Contains(current.name)) SceneMap.Add(current.name);
            }
            catch(SceneCInException ex)
            {
                if (back.Length == 0) throw new SceneCOutException(ex);
                else { Debug.LogWarning(ex.Message); Back(); }
            }
        }

        private void __InitCurrentSceneInfo(Information.Information info)
        {
            if (current.name == InterimScene)
            {
                next = info.GetSceneName1();
                auto = true;
            }
            else
            {
                back = info.GetSceneName0();
                info.SetScene(current.name, next);
            }
        }

        public void interim_auto()
        {
            try
            {
                OnNext.Invoke();
                if (next.Length == 0) throw new SceneCInException("public void interim_auto(),next is cannt refer to");
                __Load(NextAsyncOperation, false);
                StartCoroutine(Cheak(NextAsyncOperation));
            }
            finally
            {
                Debug.Log("public void interim_auto()");
            }
        }

        IEnumerator Cheak(AsyncOperation to)
        {
            while(to.progress<=0.8f)
            {
                yield return null;
                DoTimeClock = to.progress;
            }
            DoTimeClock = 1;
            __Load(to, true);
        }

        public void interim_next()
        {
            OnNext.Invoke();
            interim_(NextAsyncOperation,next);
        }
        public void interim_back()
        {
            OnBack.Invoke();
            interim_(BackAsyncOperation,back);
        }
        public void interim_return()
        {
            OnReturn.Invoke();
            interim_(ReturnAsyncOperation,current.name);
        }

        void interim_(AsyncOperation target, string scene)
        {
            Add(T =>
            {
                if (T.Value <= 0) { __Load(target, scene, true); T.state = State.Destroy; }
                else T.Value -= Time.deltaTime;
            }, new() { Value = wait });
        }

        private void __Load(AsyncOperation target, string scene, bool allow)
        {
            if (scene.Length == 0) return;
            if (target == null) target = SceneManager.LoadSceneAsync(scene, (IsAdditive) ? LoadSceneMode.Additive : LoadSceneMode.Single);
            __Load(target, allow);
        }
        private void __Load(AsyncOperation target, bool allow)
        {
            target.allowSceneActivation = allow;
        }

        public void Return()
        {
            OnReturn.Invoke();
            __Load(ReturnAsyncOperation, true);
        }

        public void Back()
        {
            OnBack.Invoke();
            __Load(BackAsyncOperation, back, true);
        }

        public void Next()
        {
            OnNext.Invoke();
            __Load(NextAsyncOperation, next, true);
        }
    }
}
