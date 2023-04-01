using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Base;
using System;
using UnityEngine.Events;

namespace Item.UI
{
    //这里是用于Canva的Button
    [RequireComponent(typeof(SpriteController))]
    public class UIButton : MonoBehaviour, ICancelHandler, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [Serializable] public class OnClickEvent : ItemEvent<int> { }//基于整数
        [Serializable] public class OnEndterEvent : ItemEvent<bool> { }
        [Serializable] public class OnExitEvent : ItemEvent<bool> { }
        public static Dictionary<string, List<UIButton>> Buttons = new();

        SpriteController sc_;
        public SpriteController SC
        {
            get
            {
                if (sc_ == null)
                    sc_ = GetComponent<SpriteController>();
                return sc_;
            }
        }
        public Animator animator;
        public string Type = "Default";
        public OnClickEvent OnClick = new();
        public OnEndterEvent OnEndter = new();
        public OnExitEvent OnExit = new();
        public int value = 0;

        virtual public void Start()
        {
            Buttons.TryAdd(Type, new());
            Buttons[Type].Remove(this);
            Buttons[Type].Add(this);
            OnExit.AddListener(_ => { if (animator != null) animator.SetBool("OnClickKeep", false); });
            OnClick.AddListener(_ =>
            {
                if (Type != "Default") foreach (var it in Buttons[Type]) it.Exit();
                if (Type != "Default") { if (animator != null) animator.SetBool("OnClickKeep", true); }
                else StartCoroutine(OverEnter(Time.time));
            });
        }

        public void OnCancel(BaseEventData eventData)
        {
            Debug.Log("OnCancel");
        }

        public void OnPointerDown(PointerEventData eventData) => OnClick.Invoke(value);

        IEnumerator OverEnter(float time)
        {
            if (animator != null) animator.SetBool("OnClickKeep", true);
            while (Time.time < time + 1) yield return null;
            if (animator != null) animator.SetBool("OnClickKeep", false);
        }

        public void Exit() => OnExit.Invoke(false);

        public void OnPointerEnter(PointerEventData eventData) => OnEndter.Invoke(true);

        public void OnPointerExit(PointerEventData eventData) => OnExit.Invoke(false);

        [Header("Default Setting")]
        public AnimationCurve curve = AnimationCurve.Linear(0, 0, 1, 1);
        public Color EnterColor=Color.white,ExitColor = new Color(0.9f, 0.9f, 0.9f);
        public Vector3 EnterScale = new Vector3(1.1f, 1.1f, 1.1f), ExitScale = Vector3.one;

        public void Change_ColorAndSize_IsEnter(bool IsEnter)
        {
            StopCoroutine(nameof(__Change_ColorAndSize));
            if (IsEnter) StartCoroutine(__Change_ColorAndSize(EnterColor, EnterScale));
            else StartCoroutine(__Change_ColorAndSize(ExitColor, ExitScale));
        }

        private IEnumerator __Change_ColorAndSize(Color endc, Vector3 ends)
        {
            float dalte = 1;
            SpriteController sprite = GetComponent<SpriteController>();
            while(dalte>0)
            {
                transform.localScale = EasingFunction.Curve(transform.localScale, ends, curve.Evaluate(1 - dalte));
                sprite.SetColor(EasingFunction.Curve(sprite.GetNowColor(), endc, curve.Evaluate(1 - dalte)));
                dalte -= Time.deltaTime;
                yield return null;
            }
            transform.localScale = ends;
            sprite.SetColor(endc);
        }
    }
}