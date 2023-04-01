using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Base;
using Item.Data;
using Item.UI.Appendage;
using System;
using Item.Control.AD;
using System.IO;

namespace Item.UI.Plot
{
    [Serializable] public class CharaterEvenet : ItemEvent<string> { };

    public class PlotController : Item
    {
        [Header("Prefab")]
        [SerializeField] GameObject BoxPrefab;
        [Header("Resource")]
        public Sprite BoxSprite;
        public Sprite BackgroundS;
        [Header("State")]
        public bool IsIgnore_ = false;
        [Header("Asset")]
        [SerializeField] List<BoxAppendage> Boxs;
        [SerializeField] Canvas target;
        [SerializeField] ADBGC Histroy_;
        [SerializeField] ADTC Histrory_text;
        [SerializeField] ADSC History_two;
        [SerializeField] SpriteController BackgroundSC;
        [SerializeField] List<KeyValuePair<string, string>> Histroy_list = new();
        public Dictionary<string, CharaterAnim> Charaters = new();

        public List<AD_Passage> items;
        public int index = 0;

        /*[SerializeField]*/ bool  isfast = false, FroNext = false;
        [HideInInspector] public bool isHistroy = false,isauto = false;

        public float time
        {
            get { return DoTimeClock; }
            set { DoTimeClock = value; }
        }

        [SerializeField,Header("These callbacks need to be relied upon to generate operational \ncharacter animators" +
            " and the generated objects need to \nhave tag:Charater and the same name as the character")] 
        CharaterEvenet OnGenerateCharater;

        virtual public void Start()
        {
            if (History_two != null) History_two.AddListener(InitHistroy);
            Add(AutoPrograss, new Carrier() { state = State.Preparation });
            UpdateCharater();
        }

        public void LoadPassage(string Path = "")
        {
            if (Path.Length == 0) Path = Application.persistentDataPath + "/default.plot";
            else Path = Application.persistentDataPath + "/" + Path;
            StreamReader sr = new StreamReader(new FileStream(Path, FileMode.Open, FileAccess.Read, FileShare.None), System.Text.Encoding.Default);
            if (null == sr) return;
            AD_Passage_Map cat = JsonUtility.FromJson<AD_Passage_Map>(sr.ReadToEnd());
            AD_Passage target = AD_Passage.CreateInstance<AD_Passage>();
            target.Init(cat);
            target.name = Time.time.ToString();
            if (target.Index >= items.Count)
            {
                for (int i = items.Count; i < target.Index; i++) items.Add(new());
                items.Add(target);
            }
            else items[target.Index] = target;
            int index_ = 0;
            foreach (var it in items)
            {
                if (it.sprite == null) it.sprite = BoxSprite;
                if (it.Index == 0) it.Index = index_++;
                foreach (var cat_ in it.passage)
                {
                    cat_.Background = BackgroundS;
                }
            }
            updataBoxs(target.Index);
            UpdateCharater();
        }

        virtual public void Sort()
        {
            items.Sort((T, S) => T.Index.CompareTo(S.Index));
        }

        virtual public void UpdateCharater()
        {
            GameObject[] tmps = GameObject.FindGameObjectsWithTag("Charater");
            if (items.Count > 0)
                foreach (var it in items[index].GetCharatersName())
                {
                    OnGenerateCharater.Invoke(it);
                    if (!Charaters.ContainsKey(it))
                    {
                        foreach (var t in tmps)
                        {
                            if (t.name == it)
                            {
                                Charaters[it] = t.GetComponent<CharaterAnim>();
                                Debug.Log(t.name);
                            }
                            break;
                        }
                    }
                }
        }

        virtual public void Skip_all()
        {
            time = items[index].passage[^1].startTime;
}

        [HideInInspector] float lasttime = 0;
        virtual public void Skip_next()
        {
            if (Time.time - 0.5f < lasttime) return;
            lasttime = Time.time;
            if (!FroNext)
            {
                FroNext = true;
                return;
            }
            if (index >= items.Count) return;
            items[index].update(time);
            if (items[index].head != items[index].tail && items[index].tail < items[index].passage.Count)
                time = items[index].passage[items[index].tail].startTime;
            if (items[index].head == -1)
                time = items[index].passage[0].startTime;
            if (items[index].tail == items[index].passage.Count)
                time = items[index].passage[^1].endTime;
        }

        virtual public void Try_Skep_new_next()
        {
            if (Time.time - 0.5f < lasttime) return;
            lasttime = Time.time;
            if (!FroNext)
            {
                FroNext = true;
                return;
            }
            if (index >= items.Count) return;
            items[index].update(time);
            if (items[index].head != items[index].tail && items[index].tail < items[index].passage.Count)
                time = items[index].passage[items[index].tail].startTime;
            if (items[index].head == -1)
                time = items[index].passage[0].startTime;
            if (items[index].tail == items[index].passage.Count)
            {
                items[index].passage.Add(new() { startTime = items[index].passage[^1].endTime, endTime = items[index].passage[^1].endTime + 1 });
                time = items[index].passage[^1].startTime;
            }
        }

        virtual public void Back()
        {
            if(isHistroy)
            {
                isHistroy = false;
                Histroy_.SetActive(isHistroy);
            }
        }

        virtual public bool Fast()
        {
            isfast = !isfast;
            return isfast;
        }

        public void Auto()
        {
            isauto = !isauto;
            if (isauto) SetState(AutoPrograss, State.Active);
            else SetState(AutoPrograss, State.Ended);
        }

        public void Return()
        {
            time = 0;
        }

        virtual public void Histroy()
        {
            isHistroy = !isHistroy;
            Histroy_.SetActive(isHistroy);
            if (isHistroy)
            {
                Histroy_list = LoadHistroy();
                StartCoroutine(TryInitHistroy());
            }
        }

        virtual public IEnumerator TryInitHistroy()
        {
            while (History_two == null && isHistroy) yield return null;
            if (isHistroy)
            {
                InitHistroy(0);
            }
        }
        virtual public void InitHistroy(float n)
        {
            (History_two.TargetItem as ADS_Name).range = new Vector2(1, (Histroy_list.Count / 10 <= 1) ? 1 : Histroy_list.Count / 10);
            int page = Histroy_list.Count * (int)(History_two.TargetItem as ADS_Name).GetValue();
            string display_out = "";
            for (int i = 0; i < 10 && i < Histroy_list.Count; i++)
            {
                display_out += Histroy_list[i].Key + ":" + Histroy_list[i].Value + "\n";
            }
            Histrory_text.SetText(display_out);
        }

        virtual public List<KeyValuePair<string, string>> LoadHistroy()
        {
            return items[index].LoadHistroy(time);
        }

        public void AutoPrograss(Carrier from)
        {
            if (time > items[index].passage[^1].endTime) return;
            time += Time.deltaTime * ((isfast)?0.6f:0.3f);
        }

        virtual public void updataBoxs(int num)
        {
            while(num>Boxs.Count)
            {
                var cat = Instantiate(BoxPrefab, target.transform);
                Boxs.Add(cat.GetComponent<BoxAppendage>());
                cat.SetActive(false);
            }
        }

        [HideInInspector] bool isUpdateAnim = true;
        [HideInInspector] int lasthead, lasttail;
        public override void update()
        {
            if (items.Count == 0) return;
            var it = items[index];
            it.update(time);
            if (it.head == -1)
            {
                foreach (var i in Boxs) i.gameObject.SetActive(false);
                return;
            }
            int sum = it.tail - it.head;
            updataBoxs(sum);
            for (int i = it.head, e = it.tail; i < e; i++)
            {
                //Box
                Boxs[i - it.head].gameObject.SetActive(true);
                AD_Content cat;
                if (i == 0) cat = it.passage[0];
                else cat = it.passage[i - 1];
                Boxs[i - it.head].Init(it, cat, it.passage[i], time, target);
                //Background
                if (it.passage[i].Background != null) BackgroundSC.Sprite_Update0(it.passage[i].Background);
                //Animation
                if (lasthead != it.head && lasttail != it.tail) isUpdateAnim = true;
                if (Charaters.ContainsKey(it.passage[i].Charater) &&
                    Charaters.TryGetValue(it.passage[i].Charater, out CharaterAnim currentanim) &&
                    it.passage[i].TAS.Length != 0 &&
                    isUpdateAnim)
                {
                    isUpdateAnim = false;
                    if (!currentanim.names.Contains(it.passage[i].TAS)) currentanim.names.Add(it.passage[i].TAS);
                    currentanim.Ended();
                    if (it.passage[i].TAS != "idle") currentanim.animator.SetBool(it.passage[i].TAS, it.passage[i].TargetState);
                    lasthead = it.head;
                    lasttail = it.tail;
                }
            }
            for (int i = it.tail; i < Boxs.Count; i++)
            {
                Boxs[i].gameObject.SetActive(false);
            }

        }

        public override void Init(Item_Map from)
        {
            PlotController_Map cat = (from as PlotController_Map);
            time = cat.time;
            items = cat.items;
            index = cat.index;
        }

        public override bool IsIgnore()
        {
            return IsIgnore_;
        }

        public override Item_Map ToMap()
        {
            PlotController_Map cat = new();
            cat.Init(this);
            return cat;
        }
    }

    public class PlotController_Map : Item_Map
    {
        public float time;
        public List<AD_Passage> items;
        public int index;

        public override void Init(Item from)
        {
            PlotController cat = (from as PlotController);
            time = cat.time;
            items = cat.items;
            index = cat.index;
        }
    }
}