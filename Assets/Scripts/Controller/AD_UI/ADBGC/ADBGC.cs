using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Item.UI
{
    public class ADBGC : Item
    {
        public List<ADBG_ChildC> childs;
        public bool Active_ = false;

        public override void Awake()
        {
            base.Awake();
            //SetActive(Active_);
        }

        public void SetActive()
        {
            Active_ = !Active_;
            foreach (var it in childs) it.SetActive(Active_);
        }

        public void SetActive(bool t)
        {
            Active_ = t;
            foreach (var it in childs) it.SetActive(t);
        }

        public override bool IsIgnore()
        {
            return true;
        }

        public override void Init(Item_Map from)
        {
            throw new Main.MainSystemException("ADBGC cannt initmap");
        }

        public override Item_Map ToMap()
        {
            throw new Main.MainSystemException("ADBGC cannt tomap");
        }
    }
}
