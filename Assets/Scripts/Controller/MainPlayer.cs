using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Base;
using System;
using System.Runtime.InteropServices;

namespace Main
{
    /// <summary>
    /// 所有的主控制器都继承这个
    /// </summary>
    public abstract class MainPlayer : UnderlyingObject
    {
        Camera MainCamera_;
        public Camera MainCamera
        {
            get
            {
                if (MainCamera_ == null)
                    MainCamera_ = Camera.main;
                return MainCamera_;
            }
        }
        public Transform MainTransform
        {
            get
            {
                return MainCamera.transform;
            }
        }

        //从Map初始化，全局保存，全局初始化
        abstract public void Init(MainMap from);
        abstract public void OnSave(string path);
        abstract public void OnLoad(string path);
    }
    /// <summary>
    /// 所有的主Map都继承这个
    /// </summary>
    public abstract class MainMap
    {
        abstract public void Init(MainPlayer from);
    }
}
