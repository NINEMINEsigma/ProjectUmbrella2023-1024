using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Item.Data
{
    [Serializable]
    public class AD_Content
    {
        [Header("TargetCharater")]
        public string Charater="null";
        [Header("Content"), Multiline]
        public string text="null";
        [Header("Asset")]
        public float startTime = 0;
        public float endTime = 1;
        public Vector2 Pos = new(0, -0.25f);
        public Vector2 Size = new(1, 0.5f);
        public Color textColor = Color.white;
        public Sprite Background;
        [Header("CharaterAsset")]
        [Tooltip("TargetAnimationState")] public string TAS = "";
        public bool TargetState = false;

        public AD_Content()
        {
            Charater = "null";
            text = "null";
            startTime = 0;
            endTime = 1;
            Pos = new(0, -0.25f);
            Size = new(1, 0.5f);
            textColor = Color.white;
        }

        public AD_Content(AD_Content_Map from)
        {
            Charater = from.Charater;
            this.text = from.text;
            this.startTime = from.startTime;
            this.endTime = from.endTime;
            Pos = new(from.Pos_x, from.Pos_y);
            Size = new(from.Size_x, from.Size_y);
            this.textColor = new(from.textColor_r, from.textColor_g, from.textColor_b, from.textColor_a);
            if (from.Background.Length != 0) Background = Resources.Load<Sprite>("Sprites/" + from.Background);
            TAS = from.TAS;
            TargetState = from.TargetState;
        }

    }

    [Serializable]
    public class AD_Content_Map
    {
        public string Charater;
        public string text;
        public float startTime = 0;
        public float endTime = 1;
        public float Pos_x = 0, Pos_y = -0.25f;
        public float Size_x = 1, Size_y = 0.5f;
        public float textColor_r = 1, textColor_g = 1, textColor_b = 1, textColor_a = 1;
        public string Background;
        public string TAS = "";
        public bool TargetState = false;

        public static AD_Content_Map Generate(AD_Content from)
        {
            return new(from);
        }

        public AD_Content_Map(AD_Content from)
        {
            Charater = from.Charater;
            text = from.text;
            startTime = from.startTime;
            endTime = from.endTime;
            Pos_x = from.Pos.x;
            Pos_y = from.Pos.y;
            Size_x = from.Size.x;
            Size_y = from.Size.y;
            textColor_r = from.textColor.r;
            textColor_g = from.textColor.g;
            textColor_b = from.textColor.b;
            textColor_a = from.textColor.a;
            if (from.Background != null) Background = from.Background.name;
            TAS = from.TAS;
            TargetState = from.TargetState;
        }

        public AD_Content_Map()
        {
            Charater = "";
            text = "";
            startTime = 0;
            endTime = 1;
            Pos_x = 0;
            Pos_y = -0.25f;
            Size_x = 1;
            Size_y = 0.5f;
            textColor_r = 1;
            textColor_g = 1;
            textColor_b = 1;
            textColor_a = 1;
            Background = "default";
            TAS = "idle";
            TargetState = false;
        }
    }

}
