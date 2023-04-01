using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Base;
using System;
using System.Runtime.InteropServices;
using TMPro;
using Item.UI;

namespace Item.Control.AD
{
    public class ADBC : Item, IPointerClickHandler
    {
        [Header("Event")] public ADEvent OnClick, OnDoubleClick;
        float LastClick;
        [Header("Others")]
        public Animator animator;
        public float timepasser = 0.35f;//响应时间
        public TMP_Text NameObj;
        public TMP_InputField NameInputObj;
        public ADDC ChooseObj;
        [Header("Value")]
        public float value;
        public string Name => NameObj.text;
        public bool WillIgnore = true;

        public override void Awake()
        {
            base.Awake();
            Add(TrySingleClick, new(State.Preparation));
        }

        //点击检测，具有至多timepasser的处理延迟
        public void OnPointerClick(PointerEventData eventData)
        {
            if (Time.time - LastClick <= timepasser)
            {
                OnDoubleClick.Invoke(value);
                if (SetState(TrySingleClick, State.Ended))
                    Debug.Log("DoubleClick");
            }
            else
            {
                LastClick = Time.time;
                if (SetState(TrySingleClick, State.Active))
                    SingleClick();
            }
        }

        public void TrySingleClick(Carrier carrier)
        {
            if (Time.time - LastClick > timepasser)
            {
                OnClick.Invoke(value);
                carrier.state = State.Ended;
            }
        }

        public void SingleClick()
        {
            if (animator != null) animator.SetFloat("ClickCount", 1);
        }
        public void DoubleClick()
        {
            if (animator != null) animator.SetFloat("ClickCount", 2);
        }
        public void End()
        {
            if (animator != null) animator.SetFloat("ClickCount", 0);
        }

        public void SetName(float no)
        {
            NameInputObj.gameObject.SetActive(true);
            NameInputObj.text = NameObj.text;
        }

        public void ChooseOption()
        {
            ChooseObj.gameObject.SetActive(true);
        }

        public override void Init(Item_Map from)
        {
            throw new Main.MainSystemException("ADBC cannt initmap(currnet)");
            //ADBC_Map cat = from as ADBC_Map;
        }

        public override bool IsIgnore()
        {
            return WillIgnore;
        }

        public override Item_Map ToMap()
        {
            throw new Main.MainSystemException("ADBC cannt tomap(currnet)");
        }
    }


    [Serializable]
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public class ADBC_Map : Item_Map
    {
        public override void Init(Item from)
        {
            Init_Lineage(from);
            ADBC cat = from as ADBC;
        }
    }
}
