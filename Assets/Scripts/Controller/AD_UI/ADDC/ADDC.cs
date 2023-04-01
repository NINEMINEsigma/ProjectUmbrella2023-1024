using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.Events;

namespace Item.UI
{
    public class ADDC : Item
    {
        TMP_Dropdown dropdown_;
        public TMP_Dropdown dropdown
        {
            get
            {
                if (dropdown_ == null)
                    dropdown_ = GetComponent<TMP_Dropdown>();
                return dropdown_;
            }
        }
        public delegate float Transvalue(int value);
        public Transvalue transvalue;
        public float value
        {
            get
            {
                if (transvalue == null) return dropdown.value;
                else return transvalue(dropdown.value);
            }
        }

        public void AddListener(UnityAction<int> action)
        {
            dropdown.onValueChanged.AddListener(action);
        }
        public void ClearOptions()
        {
            dropdown.ClearOptions();
        }
        public void AddOption(string option)
        {
            dropdown.AddOptions(new List<string> { option });
        }
        public void AddOptions(List<string> options)
        {
            dropdown.AddOptions(options);
        }

        public void End()
        {
            gameObject.SetActive(false);
        }

        public override void Init(Item_Map from)
        {
            throw new Main.MainSystemException("ADDC cannt initmap");
        }

        public override bool IsIgnore()
        {
            return true;
        }

        public override Item_Map ToMap()
        {
            throw new Main.MainSystemException("ADDC cannt tomap");
        }
    }
}