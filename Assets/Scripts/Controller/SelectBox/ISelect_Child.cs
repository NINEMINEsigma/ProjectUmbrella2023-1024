using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Item.Control.Appendage
{
    public class ISelect_Child : MonoBehaviour
    {
        [Serializable] public class SelectDragEvent : UnityEvent<Vector3, Vector3> { }
        public SelectDragEvent OnDragEnded;
        public string ParentName = "Default";

        private void Start()
        {
            foreach (var it in ISelect.selects.FindAll(T => T.AssetName == ParentName))
            {
                it.Targets.Add(this);
            }
        }
    }
}

