using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Item.UI.Plot;
using Item.UI;
using Item.Data;
using System.IO;
using DG.Tweening.Plugins.Core.PathCore;
using UnityEngine.Events;
using System;
using static UnityEngine.GraphicsBuffer;

namespace Item.Editor
{
    [Serializable]public class Init : ItemEvent<AD_Content> { }

    [RequireComponent(typeof(ADBGC))]
    public class StoryWriter : Storyteller
    {
        [Header("WriterAsset")]
        ADBGC childC_;
        ADBGC childC
        {
            get { if (childC_ == null) childC_ = GetComponent<ADBGC>();return childC_; }
        }
        public string project_name;
        bool isPathInput = false;
        [SerializeField] Init OnInit;

        public void OnChangePath(string from)
        {
            if (from.Length == 0) return;
            isPathInput = true;
            project_name = from;
        }

        public void LoadPassage(bool no)
        {
            LoadPassage((isPathInput) ? "" : project_name);
        }

        public void SavePassage(bool no)
        {
            SavePassage(index, (isPathInput) ? project_name : "");
        }
        public void SavePassage(int index,string Path = "")
        {
            if (index >= items.Count) return;
            if (Path.Length == 0) Path = Application.persistentDataPath + "/default.plot";
            else Path = Application.persistentDataPath + "/" + Path;
            FileC.FileC.TryCreateDirectroryOfFile(Path);
            StreamWriter sw = new StreamWriter(Path,false);
            AD_Passage target = items[index];
            AD_Passage_Map cat = new(target);
            Debug.Log(JsonUtility.ToJson(cat));
            foreach(var it in JsonUtility.ToJson(cat))
            {
                sw.Write(it);
            }
            sw.Flush();
            sw.Close();
        }

        bool isSelectTimeADIF = false;
        public void OnSelectTimeADIF()
        {
            isSelectTimeADIF = true;
        }
        public void isEndTimeADIF()
        {
            isSelectTimeADIF = false;
        }
        public void OnEndChangeTimeADIF(string time)
        {
            if (isauto) Auto();
            isEndTimeADIF();
            base.time = float.Parse(time);
        }
        public void KeepChange(ADIFC from)
        {
            if(!isSelectTimeADIF) from.SetText(time.ToString());
        }

        public void SetIndex(string index_)
        {
            if (int.Parse(index_) >= items.Count) return;
            index = int.Parse(index_);
        }

        public override void update()
        {
            base.update();
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                childC.SetActive();
            }
            if (index < items.Count && items[index].passage.Count > items[index].head && items[index].head >= 0)
                OnInit.Invoke(items[index].passage[items[index].head]);

        }

        public override void Init(Item_Map from)
        {
            base.Init(from);
            Storyteller_Map cat = from as Storyteller_Map;
        }

        public override bool IsIgnore()
        {
            return false;
        }

    }

    public class StoryWriter_Map : Storyteller_Map
    {
        public override void Init(Item from)
        {
            base.Init(from);
            StoryWriter cat = from as StoryWriter;
        }
    }
}
