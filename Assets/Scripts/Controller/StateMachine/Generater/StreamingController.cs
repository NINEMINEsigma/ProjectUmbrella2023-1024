using System.Collections;
using System.Collections.Generic;
using System.IO;
using Item.UI;
using Item.UI.Appendage;
using UnityEngine;
using TMPro;
using Base.Start;
using Base;
using Item.UI.Plot;

namespace Item.Control
{
    public class StreamingController : Item
    {
        string path => Path.Combine(Application.persistentDataPath, "AD");

        [Header("Prefab"), SerializeField]
        GameObject Prefab;
        [Header("Parent"),SerializeField]
        Transform parent_;
        [Header("Asset")]
        [SerializeField] int group = 0;
        [SerializeField] IndexStateController ISC;
        [HideInInspector] public FileInfo info;
        [SerializeField] TMP_Text text;
        [Header("SceneC"), SerializeField] SceneController sceneController;

        private void Start()
        {
            info = null;
            string groupType = "IndexButton" + group.ToString();
            FileC.FileC.ReGetFiles(groupType, path, CheakFiles);
            int catnum = 0;
            if (FileC.FileC.Files.ContainsKey(groupType))
                catnum = FileC.FileC.Files[groupType].Count;
            for (int i = 0; i < catnum; i++)
            {
                var cat = Instantiate(Prefab, parent_);
                Init_StreamingButton(i, cat);
            }
            var cat_max = Instantiate(Prefab, parent_);
            Init_StreamingButton(catnum, cat_max);

            void Init_StreamingButton(int i, GameObject cat)
            {
                StreamingButton catb = cat.GetComponent<StreamingButton>();
                catb.group = group;
                catb.index = i;
                catb.info_func.AddListener(InfoUpdata);
                catb.parent = ISC;
            }
        }

        public void ApplyAndNext()
        {
            if (info == null)
            {
                using (var info_sf = File.Create(Path.Combine(path, "default.ad")))
                {
                    info_sf.WriteByte(0);
                    info_sf.Close();
                }

                string groupType = "IndexButton" + group.ToString();
                FileC.FileC.ReGetFiles(groupType, path, CheakFiles);
                info = FileC.FileC.Files[groupType].Find(T => T.Name == "default.ad");
            }
            LaunchObject.instance.info.fileinfo = info;
            LaunchObject.instance.info.SetScene(sceneController.current.name);
            sceneController.interim_next();
        }

        public void InfoUpdata(FileInfo from)
        {
            info = from;
            TextInit();
        }

        void TextInit()
        {
            if (info == null) text.text = "Genereate New";
            else if (info.Name == "default.ad") text.text = "Nameless";
            else text.text = Path.GetFileNameWithoutExtension(info.Name);
        }

        public FileInfo GetFileInfo() => info;

        bool CheakFiles(string name) => name.EndsWith(".ad");

        public override void Init(Item_Map from)
        {
            throw new Main.MainSystemException("StreamingController cannt initmap");
        }

        public override bool IsIgnore()
        {
            return true;
        }

        public override Item_Map ToMap()
        {
            throw new Main.MainSystemException("StreamingController cannt tomap");
        }
    }
}
