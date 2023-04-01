using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Item.UI
{
    public class ADSC : MonoBehaviour
    {
        [SerializeField] Scrollbar scrollbar;
        public Item TargetItem;
        public float value_
        {
            get
            {
                return scrollbar.value;
            }
            set
            {
                scrollbar.value = value;
            }
        }

        private void Start()
        {
            if (TargetItem != null)
            {
                scrollbar.onValueChanged.AddListener(_ => TargetItem.ItemValue = scrollbar.value);
                scrollbar.value = TargetItem.ItemValue;
            }
        }

        public void AddListener(UnityAction<float> from)
        {
            scrollbar.onValueChanged.AddListener(from);
        }

        public void SetTarget(Item from)
        {
            scrollbar.onValueChanged.RemoveListener(_ => TargetItem.ItemValue = scrollbar.value);
            TargetItem = from;
            scrollbar.onValueChanged.AddListener(_ => TargetItem.ItemValue = scrollbar.value);
        }
    }
}