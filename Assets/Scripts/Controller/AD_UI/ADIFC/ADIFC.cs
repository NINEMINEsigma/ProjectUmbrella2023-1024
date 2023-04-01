using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Base;
using System;
using System.Runtime.InteropServices;
using TMPro;

namespace Item.UI
{
    [Serializable]public class ADIFCEvent : ItemEvent<string> { }
    [Serializable] public class ADIFC_KeepEvent : ItemEvent<ADIFC> { }

    public class ADIFC :MonoBehaviour
    {
        public TMP_Text Target;
        public bool isEndFalseActive = true;
        public ADIFCEvent OnChangeE, OnEndE, OnSelect;
        public ADIFC_KeepEvent KeepChange;

        TMP_InputField m_InputField;
        TMP_InputField inputField
        {
            get
            {
                if (m_InputField == null)
                    m_InputField = GetComponent<TMP_InputField>();
                return m_InputField;
            }
        }

        private void Update()
        {
            KeepChange.Invoke(this);
        }

        public void SetText(string text)
        {
            inputField.text = text;
        }

        public string GetText()
        {
            return inputField.text;
        }


        public void OnSelecte()
        {
            OnSelect.Invoke(inputField.text);
        }

        public void OnChange()
        {
            if(Target!=null) Target.text = inputField.text;
            OnChangeE.Invoke(inputField.text);
        }

        public void OnEnd()
        {
            if (Target != null) Target.text = inputField.text;
            if(isEndFalseActive) gameObject.SetActive(false);
            OnEndE.Invoke(inputField.text);
        }
    }
}
