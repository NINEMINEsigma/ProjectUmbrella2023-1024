using System;
using System.Collections;
using System.Collections.Generic;
using Item.UI.Appendage;
using UnityEngine;


namespace Item.Data
{
    [CreateAssetMenu(menuName = "Plot/Passage", fileName = "New Passage Data")]
    public class AD_Passage : ScriptableObject
    {
        [Header("Asset")]
        public int ID;
        public int Index;
        public string description;
        public Sprite sprite;
        public AnimationCurve curve = AnimationCurve.Linear(0, 0, 1, 1);
        public AnimationCurve textColorCurve = AnimationCurve.Linear(0, 0, 1, 1);
        [Header("State")]
        public int head;
        public int tail;

        public List<AD_Content> passage = new() { new AD_Content() };

        public void update(float time)
        {
            tail = head = -1;
            if (passage.Count > 0)
            {
                if (time < passage[0].startTime) return;
                if (time >= passage[^1].endTime)
                {
                    tail = passage.Count;
                    return;
                }
            }
            bool Findstate = false;
            for (int i = 0; i < passage.Count; i++)
            {
                if (!Findstate && time >= passage[i].startTime && time < passage[i].endTime)
                {
                    head = i;
                    Findstate = true;
                }
                if (Findstate) tail = i;
                if (time < passage[i].startTime && Findstate) return;
            }
            tail++;
        }

        public List<string> GetCharatersName()
        {
            List<string> cat = new();
            foreach (var it in passage)
            {
                if (cat.Contains(it.Charater)) continue;
                cat.Add(it.Charater);
            }
            return cat;
        }

        public List<KeyValuePair<string, string>> LoadHistroy(float time)
        {
            List<KeyValuePair<string, string>> returnist = new();

            int index = 0;
            while (index < passage.Count && passage[index].startTime <= time)
            {
                returnist.Add(new KeyValuePair<string, string>(((passage[index].Charater.Length == 0) ? "Exberlahet[AUTO]" : passage[index].Charater), passage[index].text));
                index++;
            }
            return returnist;
        }

        public AD_Passage()
        {
            ID = 0;
            Index = 0;
            this.description = "";
            this.sprite = null;
            this.curve = AnimationCurve.Linear(0, 0, 1, 1);
            this.textColorCurve = AnimationCurve.Linear(0, 0, 1, 1);
            this.passage = new();
        }

        public void Init(AD_Passage_Map from)
        {
            ID = from.ID;
            Index = from.Index;
            this.description = from.description;
            if (from.sprite.Length != 0) this.sprite = Resources.Load<Sprite>("Sprites/" + from.sprite);
            passage.Clear();
            foreach (var it in from.passage)
            {
                passage.Add(new(it));
            }
        }

        public AD_Passage(AD_Passage_Map from)
        {
            ID = from.ID;
            Index = from.Index;
            this.description = from.description;
            this.sprite = Resources.Load<Sprite>("Sprites/" + from.sprite);
            passage.Clear();
            foreach (var it in from.passage)
            {
                passage.Add(new(it));
            }
        }
    }

    [Serializable]
    public class AD_Passage_Map
    {
        public int ID;
        public int Index;
        public string description;
        public string sprite;
        public List<AD_Content_Map> passage = new();

        public AD_Passage_Map()
        {
            ID = 0;
            Index = 0;
            description = "";
            sprite = "";
            passage = new();
        }

        public AD_Passage_Map(AD_Passage from)
        {
            ID = from.ID;
            Index = from.Index;
            description = from.description;
            if (from.sprite != null) sprite = from.sprite.name;
            foreach (var it in from.passage)
            {
                passage.Add(new(it));
            }
        }
    }


    public static class ListEx
    {
        public static bool CustomContains<T>(this IList<T> list, T t) where T : class
        {
            foreach (var item in list)
            {
                if (item == t)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
