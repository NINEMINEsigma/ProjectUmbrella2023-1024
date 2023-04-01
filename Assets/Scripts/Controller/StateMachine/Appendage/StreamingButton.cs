using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Item.UI;
using FileC;
using System.IO;
using TMPro;
using System;
using Item.Control;
using sc = Item.Control.StreamingController;

namespace Item.UI.Appendage
{
    public class InfoCarrier : ItemEvent<FileInfo> { }

    public class StreamingButton : UIButton
    {
        [Header("Asset")]
        public int group = 0;
        public int index = 0;
        public string GroupName = "IndexButton";
        [Header("IndexStateController")]
        [HideInInspector] public IndexStateController parent;
        FileInfo info;
        string path => Path.Combine(Application.persistentDataPath, "AD");
        [HideInInspector]public InfoCarrier info_func = new();

        public override void Start()
        {
            try
            {
                base.Start();
                Init_StreamingButton_FileInfo();
                OnClick.AddListener(_ => info_func.Invoke(info));
                parent.Targets.Add(transform);
                parent.Targets.Sort((T, S) => T.GetComponent<StreamingButton>().index.CompareTo(S.GetComponent<StreamingButton>().index));
            }
            catch (Main.MainSystemException ex)
            {
                Debug.LogException(ex);
            }
            catch(Exception ex)
            {
                Debug.LogWarning(ex.Message);
            }

        }

        private void Init_StreamingButton_FileInfo()
        {
            try
            {
                string groupType = GroupName + group.ToString();
                FileC.FileC.GetFiles(groupType, path, CheakFiles);
                var text = transform.GetComponentInChildren<TMP_Text>();
                if (FileC.FileC.Files.ContainsKey(groupType) && FileC.FileC.Files[groupType].Count > index)
                {
                    Debug.Log(index + "/" + FileC.FileC.Files[groupType].Count);
                    info = FileC.FileC.Files[groupType][index];
                    text.text = info.Name;
                }
                else
                {
                    text.text = "Genereate New";
                    Debug.Log(text.text);
                }
            }
            catch (Exception ex)
            {
                throw new Main.MainSystemException("private void Init_StreamingButton_FileInfo()", ex);
            }
        }

        bool CheakFiles(string name)
        {
            return name.EndsWith(".ad");
        }
    }
}