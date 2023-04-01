using System.IO;
using UnityEngine;
using System;

namespace Base.Information
{
    [Serializable]
    public class Information
    {
        public FileInfo fileinfo;
        [SerializeField]string scene0, scene1;
        public SubInformation data_information;

        public void SetScene(string current)
        {
            scene0 = current;
        }

        public void SetScene(string current, string next)
        {
            scene0 = current;
            scene1 = next;
        }

        public string GetSceneName0() => scene0;
        public string GetSceneName1() => scene1;
    }

    [Serializable]
    public class SubInformation { }
}