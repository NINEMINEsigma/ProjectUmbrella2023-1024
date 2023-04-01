using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Item.UI;
using Item.Data;
using UnityEngine.Events;
using Item.UI.Appendage;
using System;
using static UnityEngine.GraphicsBuffer;

namespace Item.Editor
{
    [Serializable]public class Next : ItemEvent<bool> { };

    public class PassageCreater : MonoBehaviour
    {
        [Header("Asset")]
        public ADIFC Charater;
        public ADIFC description;
        public ADIFC text;
        public ADIFC startTime;
        public ADIFC endTime;
        public ADIFC Pos_x, Pos_y;
        public ADIFC Size_x, Size_y;
        public ADIFC textColor_r, textColor_g, textColor_b, textColor_a;
        public ADIFC TAS;
        public ADSC TargetState;
        public ADIFC background;
        public Next OnNext;
        [Header("Target")]
        public AD_Content target;

        public void Init(AD_Content t)
        {
            if (target == t) return;
            target = t;
            Charater.SetText(target.Charater);
            text.SetText(target.text);
            startTime.SetText(target.startTime.ToString());
            endTime.SetText(target.endTime.ToString());
            Pos_x.SetText(target.Pos.x.ToString());
            Pos_y.SetText(target.Pos.y.ToString());
            Size_x.SetText(target.Size.x.ToString());
            Size_y.SetText(target.Size.y.ToString());
            textColor_r.SetText(target.textColor.r.ToString());
            textColor_g.SetText(target.textColor.g.ToString());
            textColor_b.SetText(target.textColor.b.ToString());
            textColor_a.SetText(target.textColor.a.ToString());
            TAS.SetText(target.TAS);
            TargetState.value_ = (target.TargetState) ? 1 : 0;
            background.SetText((target.Background == null) ? "" : target.Background.name);
        }

        public void Next()
        {
            OnNext.Invoke(false);
        }

        public void SetPassageDescription(StoryWriter storyWriter)
        {
            storyWriter.items[storyWriter.index].description = description.GetText();
        }

        public void Apply()
        {
            target.Charater = Charater.GetText();
            target.text = text.GetText();
            target.startTime = float.Parse(startTime.GetText());
            target.endTime = float.Parse(endTime.GetText());
            target.Pos = new(float.Parse(Pos_x.GetText()), float.Parse(Pos_y.GetText()));
            target.Size = new(float.Parse(Size_x.GetText()), float.Parse(Size_y.GetText()));
            target.textColor = new(float.Parse(textColor_r.GetText()), float.Parse(textColor_g.GetText()),
                float.Parse(textColor_b.GetText()), float.Parse(textColor_a.GetText()));
            target.TAS = TAS.GetText();
            target.TargetState = (TargetState.TargetItem as ADS_Name).GetValue() == 1;
            target.Background = Resources.Load<Sprite>("Sprites/" + background.GetText());
        }
    }
}
