using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Item.UI
{
    public class ADBG_ChildC : Item
    {
        public override void Init(Item_Map from)
        {
            throw new Main.MainSystemException("ADBG_ChildC cannt initmap");
        }

        public override bool IsIgnore()
        {
            return true;
        }

        public void SetActive(bool b)
        {
            gameObject.SetActive(b);
        }

        public override Item_Map ToMap()
        {
            throw new Main.MainSystemException("ADBG_ChildC cannt tomap");
        }
    }
}
