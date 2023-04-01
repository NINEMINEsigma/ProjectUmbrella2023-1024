using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Base;

namespace Base.Start
{
    public class LaunchObject : MonoBehaviour
    {
        public static LaunchObject instance;

        public Information.Information info = new();

        public void Launch()
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
        }
    }
}
