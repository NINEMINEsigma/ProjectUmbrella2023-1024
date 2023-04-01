using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Warning
{
    public class WARNINGXXX : MonoBehaviour
    {
        [HideInInspector]public string WARNING = "RicoTen,inokana,luch4736,Fer,Twiiz";
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}
