using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Base;
using Main.Control;
using UnityEngine.SceneManagement;
using System;
using Newtonsoft.Json;
using System.IO;
using UnityEditor;
using UnityEditor.VersionControl;
using Item;
using Base.Start;
using static UnityEngine.GraphicsBuffer;
using Item.Control;
using FileC;

namespace Main
{

    [Serializable]
    public class MainSystemException : BaseInException
    {
        public MainSystemException() { }
        public MainSystemException(int _warning_level) { warning_level = _warning_level; }
        public MainSystemException(string message) : base(message) { }
        public MainSystemException(string message, Exception inner) : base(message, inner) { }
        protected MainSystemException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }

        public MainSystemException ThrowOutException(string message,Item.Item target)
        {
            var cat= new MainSystemException(target.name + ":" + message);
            cat.warning_level = 256;
            return cat;
        }
        public MainSystemException ThrowOutException(string message, string target)
        {
            var cat = new MainSystemException(target + ":" + message);
            cat.warning_level = 256;
            return cat;
        }

        public string Warning_Message()
        {
            switch(warning_level)
            {
                case -1:
                    return "MainSystem(s)";
                case 0:
                    return "Regular";
                case 2:
                    return "SameName";
                case 104:
                    return "Info";
                default:
                    return "Unknow Error";
            }
        }
        public int warning_level = 1;
    }

    public class MainSystem :MonoBehaviour
    {
        public static MainSystem main;
        public static FileInfo info;
        public SceneController sceneController;
        public ADUIObject ADUIObject;
        public Canvas MainCanvas_World;
        public bool istransitive = false;

        public string path
        {
            get
            {
                FileC.FileC.TryCreateDirectroryOfFile(Application.persistentDataPath + "/X.X");
                return Application.persistentDataPath;
            }
            set => TargetAD = value;
        }
        public string adpath
        {
            get
            {
                string path_;
                path_ = Application.persistentDataPath + "/AD/" + TargetAD + ".ad";
                FileC.FileC.TryCreateDirectroryOfFile(path_);
                return path_;
            }
            set => TargetAD = value;
        }


        [Header("CurrentAD_Project")]
        public string TargetAD_ = "default";
        public string TargetAD
        {
            get { return TargetAD_; }
            set
            {
                string tmp = Here_Json_FullName();
                try
                {
                    ADIndex ad = null;
                    TargetAD_ = value;
                    ad = JsonConvert.DeserializeObject<ADIndex>(File.ReadAllText(tmp));
                    ad.Control_RenameFile_Streaming();
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                    Save();
                    FileC.FileC.DeleteFile(tmp);
                }
            }
        }

        private void Awake()
        {
            try
            {
                if (MainCanvas_World == null) throw new MainSystemException(-1);
                if (sceneController == null) throw new MainSystemException(-1);
                if (main != null)
                {
                    Destroy(gameObject);
                    throw new MainSystemException(-1);
                }
                else main = this;
                info = LaunchObject.instance.info.fileinfo;
                if (info == null) throw new MainSystemException(104);
                else TargetAD_ = Path.GetFileNameWithoutExtension(info.Name);
                Load();
                if (istransitive) DontDestroyOnLoad(gameObject);
            }
            catch (MainSystemException ex)
            {
                if (ex.warning_level == 1) Debug.LogWarning(ex.Message);
                else if (ex.warning_level == -1) throw ex.ThrowOutException("Some Main Level Error Is Throw", "private void Awake()");
                else Debug.LogWarning(ex.Warning_Message());
            }
        }

        [Serializable]
        public class ADWarehouse
        {
            [JsonConstructor]
            public ADWarehouse() { }
            public Dictionary<string,List<string>> items = new();

            public void Add(string key) => items.TryAdd(key, new());
            public void Add(string key, string value) => items[key].Add(value);
            public bool ContainsKey(string key) => items.ContainsKey(key);

            public void Remove(string key) => items.Remove(key);

            public void TryAdd(string key, string value)
            {
                if (ContainsKey(key)) throw new MainSystemException(2);
                else Add(key, value);
            }
        }

        [Serializable]
        public class ADIndex
        {
            [JsonConstructor]
            public ADIndex() { }
            public string here_ = "";
            public string default_ = "";
            public string PMovement_ = "";

            public void Control_RenameFile_Streaming()
            {
                FileC.FileC.FileRename(here_, Here_Json_FullName());
                FileC.FileC.FileRename(default_, Default_Json_FullName(main.path));
                if (PMovement_.Length > 0) FileC.FileC.FileRename(PMovement_, PMovement_Json_FullName(main.path));
            }
        }

        public void Save()
        {
            ADIndex ADINDEX = new();
            ADINDEX.here_ = Here_Json_FullName();
            ADINDEX.default_ = Default_Json_FullName(path);

            ADWarehouse cat = new();
            foreach (var it in ItemManager.items)
            {
                foreach (var item in it.Value)
                {
                    TryAddAsset(cat, it, item);
                }
            }
            File.WriteAllText(ADINDEX.default_, JsonConvert.SerializeObject(cat));

            if (PMovement.main != null)
            {
                PMovement.main.OnSave(path);
                ADINDEX.PMovement_ = PMovement_Json_FullName(path);
            }

            File.WriteAllText(ADINDEX.here_, JsonConvert.SerializeObject(ADINDEX));

        }
        private static void TryAddAsset(ADWarehouse cat, KeyValuePair<string, List<Item.Item>> it, Item.Item item)
        {
            try
            {
                if (!item.IsIgnore())
                {
                    Debug.Log(item.name + ":" + item.GetType().ToString());
                    cat.TryAdd(it.Key, JsonConvert.SerializeObject(item.ToMap())); //TODO
                }
                else Debug.Log("X " + item.name + ":" + item.GetType().ToString());
            }
            catch (MainSystemException ex)
            {
                switch (ex.warning_level)
                {
                    case 2:
                        cat.TryAdd(it.Key + "__SameName_ReName", JsonConvert.SerializeObject(item.ToMap()));
                        Debug.LogWarning(ex.Warning_Message());
                        break;
                    default:
                        Debug.LogException(ex);
                        throw ex.ThrowOutException("Unknow Error", ex.Warning_Message());
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning(ex.Message);
            }
        }

        public static string Here_Json_FullName(string path) => main.adpath;
        public static string Here_Json_FullName() => main.adpath;
        public static string Here_Json_FullName(string path,int no)
        {
            string path_;
            path_ = Application.persistentDataPath + "/AD/" + path + ".ad";
            FileC.FileC.TryCreateDirectroryOfFile(path_);
            return path_;
        }
        public static string Default_Json_FullName(string path) => path + "/" + main.TargetAD + ".adobject";
        public static string PMovement_Json_FullName(string path) => path + "/Movement/" + main.TargetAD + ".player";

        public void Load()
        {
            /*if (PMovement.main != null)
            {
                PMovement.main.OnLoad(path);
            }*///TODO
            //if the file has pm,you need to create it when it null 
        }

        public void Load(Item.Item_Map target, List<KeyValuePair<string, List<string>>> Lineage)
        {

        }

        public void DEBUGTEST()
        {
            Debug.LogWarning("DEBUGTEST");
        }
        public void DEBUGTEST(float from)
        {
            Debug.LogWarning("DEBUGTEST");
        }
        public void DEBUGTEST(int from)
        {
            Debug.LogWarning("DEBUGTEST");
        }
        public void DEBUGTEST(bool from)
        {
            Debug.LogWarning("DEBUGTEST");
        }

        public enum ItemTarget
        {
            Text=0,
            Input=1,
            Bar=2,
            Output=16,
        }

        [Header("ItemTarget")] ItemTarget target_item = ItemTarget.Text;
        public void Set_ItemTarget(ItemTarget target) => target_item = target;
        public ItemTarget Get_ItemTarget() => target_item;

        public void Generate_Item_Streaming(Vector3 pos)
        {
            switch(target_item)
            {
                case ItemTarget.Text:
                     Generate_Item_Text(pos);
                    break;
                case ItemTarget.Input:
                    Generate_Item_Input(pos);
                    break;
                case ItemTarget.Bar:
                    Generate_Item_Bar(pos);
                    break;
                case ItemTarget.Output:
                    Generate_Item_Output(pos);
                    break;
                default:
                    break;
            }
        }

        private void Generate_Item_Output(Vector3 pos)
        {

        }

        private void Generate_Item_Bar(Vector3 pos)
        {
            throw new NotImplementedException();
        }

        private void Generate_Item_Input(Vector3 pos)
        {
            throw new NotImplementedException();
        }

        private void Generate_Item_Text(Vector3 pos)
        {
            GameObject text_ = GameObject.Instantiate(ADUIObject.ADTC_,MainCanvas_World.transform);
            text_.AddComponent<IMovement>();
            text_.transform.position = pos;
        }
    }
}