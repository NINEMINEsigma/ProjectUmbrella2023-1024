using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Base;
using UnityEngine.Events;
using System;

namespace Item.MonoSingleton//这里俩玩意是单例模式，第二个是场景跳转
{
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
    {

        public static T Instance { get; private set; }

        public static string SceneToDo { get; private set; } = "Scenes/114514Scene（看到我就是你忘了设SceneToDo）";
        public static void SetSceneToDo(string sceneName) => SceneToDo = sceneName;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = GetComponent<T>();
            }
            else Destroy(this);
            OnAwake();
        }

        protected virtual void OnAwake() { }
    }

    public class TransitionManager : MonoSingleton<TransitionManager>
    {
        [SerializeField] private Canvas canvas;
        [SerializeField] private Image[] transitionImage;

        public static void DoScene(string sceneName, Color transitionColor, float duration = 0f, float upTime = 0.5f, float downTime = 0.5f)
        {
            if (Instance == null)
            {
                SceneManager.LoadSceneAsync(sceneName);
                return;
            }
            Instance.StartCoroutine(Instance.CO_DoScene(sceneName, transitionColor, duration, upTime, downTime));
        }

        private IEnumerator CO_DoScene(string sceneName, Color transitionColor, float duration = 0f, float upTime = 0.5f, float downTime = 0.5f)
        {
            canvas.gameObject.SetActive(true);

            transitionImage[0].raycastTarget = true;
            transitionImage[1].raycastTarget = true;
            transitionImage[2].raycastTarget = true;

            var originColor = new Color(transitionColor.r, transitionColor.g, transitionColor.b, 0f);
            transitionImage[0].color = originColor;
            transitionImage[1].color = originColor;
            transitionImage[2].color = originColor;

            DOTween.To(() => transitionImage[0].color, x => transitionImage[0].color = x, transitionColor, upTime).SetEase(Ease.InOutSine);
            DOTween.To(() => transitionImage[1].color, x => transitionImage[1].color = x, transitionColor, upTime).SetEase(Ease.InOutSine);
            DOTween.To(() => transitionImage[2].color, x => transitionImage[2].color = x, transitionColor, upTime).SetEase(Ease.InOutSine);

            yield return new WaitForSecondsRealtime(upTime);

            var operation = SceneManager.LoadSceneAsync(sceneName);

            operation.allowSceneActivation = false;

            yield return new WaitForSecondsRealtime(duration);

            operation.allowSceneActivation = true;

            yield return new WaitUntil(() => operation.isDone);

            AudioListener.pause = false;

            DOTween.To(() => transitionImage[0].color, x => transitionImage[0].color = x, originColor, downTime).SetEase(Ease.InOutSine);
            DOTween.To(() => transitionImage[1].color, x => transitionImage[1].color = x, originColor, downTime).SetEase(Ease.InOutSine);
            DOTween.To(() => transitionImage[2].color, x => transitionImage[2].color = x, originColor, downTime).SetEase(Ease.InOutSine);

            yield return new WaitForSecondsRealtime(downTime);

            transitionImage[0].raycastTarget = false;
            transitionImage[1].raycastTarget = false;
            transitionImage[2].raycastTarget = false;

            canvas.gameObject.SetActive(false);
        }
    }
}

namespace Item//这里重写Item的基类
{
    public static class ItemManager
    {
        public static Dictionary<string,List<Item>> items = new();
        public static string path => Main.MainSystem.main.path;
    }

    [Serializable]
    public class ItemException : Exception
    {
        public ItemException(string message, Item exceptionTarget) : base(message) { ExceptionTarget = exceptionTarget; }
        public ItemException(string message, Exception inner, Item exceptionTarget) : base(message, inner) { ExceptionTarget = exceptionTarget; }
        protected ItemException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
        /********************************************************************************/
        public Item ExceptionTarget;
    }

    public abstract class Item : UnderlyingObject
    {
        RectTransform rectTransform_;

        public RectTransform rectTransform
        {
            get
            {
                if (rectTransform_ == null) rectTransform_ = GetComponent<RectTransform>();
                return rectTransform_;
            }
        }
        public float ItemValue;

        public static GameObject TargetItem;
        public GameObject TargetItemPrefab;

        virtual public void Awake()
        {
            //if (!ItemManager.items.ContainsKey(gameObject.name))
            ItemManager.items.TryAdd(name, new List<Item>());
            ItemManager.items[name].Add(this);
            /*throw
                Throw_ItemException("Items have same name,although may it is ignore but you still coundnt give it the same name:" + gameObject.name, this);*/
            if (TargetItem == null) TargetItem = TargetItemPrefab;
        }

        abstract public void Init(Item_Map from);
        abstract public Item_Map ToMap();
        public static void Init_<T>(Item_Map from) where T : Item
        {
            if (TargetItem == null) return;
            GameObject cat = Instantiate(TargetItem);
            cat.GetComponent<T>().Init(from);
        }
        virtual public float GetValue() { return ItemValue; }
        virtual public void SetValue(float from) { ItemValue = from; }

        abstract public bool IsIgnore();

        public ItemException Throw_ItemException(string message, Item exceptionTarget)
        {
            return new ItemException(message, exceptionTarget);
        }
        public ItemException Throw_ItemException(string message, Exception ex, Item exceptionTarget)
        {
            return new ItemException(message, ex, exceptionTarget);
        }

        private void OnDestroy()
        {
            ItemManager.items[name].Remove(this);
            if (ItemManager.items[name].Count == 0) ItemManager.items.Remove(name);
        }
    }

    public abstract class Item_Map
    {
        //Name,Types
        //from nearest parent
        public List<KeyValuePair<string, List<string>>> _m_Lineage = new();

        protected void Init_Lineage(Item target)
        {
            Transform cat = target.gameObject.transform;
            do
            {
                List<string> components_tmp = new();
                foreach (var it in cat.GetComponents<Component>()) components_tmp.Add(it.GetType().Name);
                _m_Lineage.Add(new(cat.name, components_tmp));
            } while (cat.parent != null);
        }
        public void TryReconstruct_Lineage_WithInstance() => Main.MainSystem.main.Load(this, _m_Lineage);

        abstract public void Init(Item from);
    }

    public class ItemEvent<T> : UnityEvent<T>
    {

    }
}