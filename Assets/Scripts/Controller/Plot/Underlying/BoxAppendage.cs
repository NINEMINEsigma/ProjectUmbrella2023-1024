using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Item.Data;
using Base;
using Item.Control.AD;

namespace Item.UI.Appendage
{
    public class BoxAppendage : MonoBehaviour
    {
        public SpriteController Sprite_;
        public ADTC text;
        public RectTransform rectTransform => GetComponent<RectTransform>();

        virtual public void Init(AD_Passage backGround, AD_Content from, AD_Content to, float time, Canvas canvas)
        {
            Sprite_.Sprite_Update0(backGround.sprite);
            text.Init(to.text, EasingFunction.Curve(from.textColor, to.textColor, backGround.textColorCurve.Evaluate((time - to.startTime) / (to.endTime - to.startTime))));
            var cat = canvas.GetComponent<RectTransform>().sizeDelta;
            Sprite_.gameObject.GetComponent<RectTransform>().anchoredPosition
                = EasingFunction.Curve(from.Pos * cat, to.Pos * cat, backGround.curve.Evaluate((time - to.startTime) / (to.endTime - to.startTime)));
            Sprite_.gameObject.GetComponent<RectTransform>().sizeDelta
                = EasingFunction.Curve(from.Size * cat, to.Size * cat, backGround.curve.Evaluate((time - to.startTime) / (to.endTime - to.startTime)));

        }
    }
}
