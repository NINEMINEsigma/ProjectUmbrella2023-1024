using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Base;
using Item;
using Item.Control.AD;
using Item.Control;
using Item.Control.Appendage;
using Item.UI.Appendage;
using Item.UI;


[CreateAssetMenu(fileName = "ADUIObject", menuName = "AD/ADUIObject", order = 0)]
public class ADUIObject : ScriptableObject
{
    public GameObject ADBC_;
    public GameObject ADTC_;
    public GameObject UIButton_;
    public GameObject ADBGC_;
    public GameObject ADBG_ChildC_;
    public GameObject ADDC_;
    public GameObject ADIFC_;
    public GameObject ADSC_;
    public GameObject ADS_Name_;
}
