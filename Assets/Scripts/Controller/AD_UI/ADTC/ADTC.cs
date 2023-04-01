using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Runtime.InteropServices;

namespace Item.Control.AD
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public class ADTC : Item
    {
        public override void Init(Item_Map from)
        {
            throw new Main.MainSystemException("ADTC cannt initmap");
        }

        public override bool IsIgnore() { return true; }

        [SerializeField] TMP_Text text;
        [SerializeField] string BehindEnder;

        public override void Awake()
        {
            base.Awake();
            if (text == null) if (TryGetComponent(out text)) return;
            if (text.text.Length == 0) text.text = BehindEnder;
        }

        public void SetText(string from)
        {
            text.text = from;
        }

        public void SetColor(Color from)
        {
            text.color = from;
        }

        public void Init(string text_,Color color_)
        {
            SetText(text_);
            SetColor(color_);
        }

        public override Item_Map ToMap()
        {
            throw new Main.MainSystemException("ADTC cannt tomap");
        }
    }
}
