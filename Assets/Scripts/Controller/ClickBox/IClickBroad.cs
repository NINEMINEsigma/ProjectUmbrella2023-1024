using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Item.Control
{
    public class IClickBroad : Item, IPointerClickHandler
    {
        [Serializable]public class OnClick : ItemEvent<Vector3> { }
        public OnClick OnClickEvent = new();

        public override void Init(Item_Map from) { }

        public override bool IsIgnore() { return true; }

        public void OnPointerClick(PointerEventData eventData)
        {
            RectTransformUtility.ScreenPointToWorldPointInRectangle(rectTransform, eventData.position, eventData.pressEventCamera, out Vector3 mousePos);
            OnClickEvent.Invoke(mousePos);
        }

        public override Item_Map ToMap()
        {
            throw new Main.MainSystemException("IClickBroad cannt tomap");
        }
    }
}
