using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Item.Control
{
    public class IMovement : Item, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [Serializable]
        public class MoveEvent : ItemEvent<Vector3> { }
        [Header("Others")]
        private Vector3 pos;                            //控件初始位置
        private Vector3 mousePos;                       //鼠标初始位置

        public MoveEvent OnStart;
        public MoveEvent OnEnd;
        public MoveEvent OnMove;    
        public bool IsCanMove = true;

        public void OnBeginDrag(PointerEventData eventData)
        {
            pos = this.GetComponent<RectTransform>().position;
            RectTransformUtility.ScreenPointToWorldPointInRectangle(rectTransform, eventData.position, eventData.pressEventCamera, out mousePos);

            OnStart.Invoke(mousePos);
        }

        public void OnDrag(PointerEventData eventData)
        {
            RectTransformUtility.ScreenPointToWorldPointInRectangle(rectTransform, eventData.position, eventData.pressEventCamera, out Vector3 newVec);
            Vector3 offset = new(newVec.x - mousePos.x, newVec.y - mousePos.y, 0);
            Vector2 OnMo = pos + offset; 
            OnMove.Invoke(newVec-mousePos);
            if (!IsCanMove) return;
            rectTransform.position = OnMo;

        }

        public void OnEndDrag(PointerEventData eventData)
        {
            RectTransformUtility.ScreenPointToWorldPointInRectangle(rectTransform, eventData.position, eventData.pressEventCamera, out Vector3 newVec);
            OnEnd.Invoke(newVec);
        }


        public void TestStart(Vector3 vec)
        {
            Debug.Log("开始拖拽");
        }

        public void TestEnd(Vector3 vec)
        {
            Debug.Log("结束拖拽");
        }

        public override void Init(Item_Map from)
        {
            rectTransform.position = new((from as Movement_Map).x, (from as Movement_Map).y);
        }

        public bool isignore = false;

        public override bool IsIgnore()
        {
            return isignore;
        }

        public override Item_Map ToMap()
        {
            Movement_Map cat = new();
            cat.Init(this);
            return cat;
        }
    }


    [Serializable]
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public class Movement_Map:Item_Map
    {
        public float x;
        public float y;

        public override void Init(Item from)
        {
            Init_Lineage(from);
            x = from.rectTransform.position.x;
            y = from.rectTransform.position.y;
        }
    }
}