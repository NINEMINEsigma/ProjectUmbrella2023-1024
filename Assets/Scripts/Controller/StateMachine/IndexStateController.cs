using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Item.UI
{
    [Serializable]
    public class IndexState
    {
        [SerializeField, Tooltip("位置")] public Vector3 Pos = Vector3.zero;
        [SerializeField, Tooltip("旋转")] public Vector3 Rot = Vector3.zero;
        [SerializeField, Tooltip("大小")] public Vector3 Scale = Vector3.one;

        public IndexState() { }

        public IndexState(Vector3 pos, Vector3 rot, Vector3 scale)
        {
            Pos = pos;
            Rot = rot;
            Scale = scale;
        }
    }

    public class IndexStateController : Item
    {
        [SerializeField] bool isIgnore = false;
        [Header("Asset")]
        public List<Transform> Targets;
        [SerializeField] private float speed=1;
        [SerializeField] private int index = 0;
        [SerializeField] private bool isupdate = false;
        [SerializeField] private bool issingle = false;
        [Header("State")]
        [SerializeField, Tooltip("合法状态集")] List<IndexState> indexStates;
        [SerializeField, Tooltip("位置曲线")] AnimationCurve PosCurve;
        [SerializeField, Tooltip("旋转曲线")] AnimationCurve RotCurve;
        [SerializeField, Tooltip("大小曲线")] AnimationCurve ScaleCurve;

        public List<IndexState> GetIndexStates() { return indexStates; }
        public void IndexUpdate_AfterChange() { isupdate = true; }
        public void Add(Transform targert) { Targets.Add(targert); }

        public void ChangeIndex(int i)
        {
            StartCoroutine(conversion(i));
            isupdate = true;
        }

        public void Conversion_FromFirstAndScend()
        {
            if (index != 0) ChangeIndex(0);
            else ChangeIndex(1);
        }

        public void ChangeIndex_greater()
        {
            StartCoroutine(conversion(index+1));
            isupdate = true;
        }
        public void ChangeIndex_less()
        {
            StartCoroutine(conversion(index - 1));
            isupdate = true;
        }

        IEnumerator conversion(int i)
        {
            index = i;
            DoTimeClock = 0;
            while(DoTimeClock<1)
            {
                DoTimeClock += Time.deltaTime * speed;
                yield return null;
            }
            DoTimeClock = 1;
            isupdate = false;
        }

        public override void Awake()
        {
            base.Awake();
            if (indexStates.Count > 0) ChangeIndex(0);
            index = (issingle) ? index : (indexStates.Count / 2);
        }

        public override void update()
        {
            if (isupdate)
            {
                /*for (int i = 0, n = 0; i < Targets.Count; i++)
                {
                    Transform current = Targets[i];
                    if (i < index) Init(current, 0, DoTimeClock);
                    else if (n < indexStates.Count) Init(current, n++, DoTimeClock);
                    else Init(current, n-1, DoTimeClock);
                }*/
                if (Targets.Count == 1 && issingle)
                {
                    index = Mathf.Clamp(index, 0, indexStates.Count - 1);
                    Init(Targets[0], index, DoTimeClock);
                }
                else
                {
                    for (int i = 0; i + index < indexStates.Count && i < Targets.Count; i++)
                    {
                        if (i + index < 0) continue;
                        int n = i + index;
                        Init(Targets[i], n, DoTimeClock);
                    }
                }
            }
        }

        void Init(Transform current, int curindex, float t)
        {
            /*current.localScale = Vector3.Lerp(indexStates[Mathf.Clamp(curindex - 1, 0, indexStates.Count)].Scale, indexStates[curindex].Scale, ScaleCurve.Evaluate(t));
            current.localPosition = Vector3.Lerp(indexStates[Mathf.Clamp(curindex - 1, 0, indexStates.Count)].Pos, indexStates[curindex].Pos, PosCurve.Evaluate(t));
            current.localEulerAngles = Vector3.Lerp(indexStates[Mathf.Clamp(curindex - 1, 0, indexStates.Count)].Rot, indexStates[curindex].Rot, RotCurve.Evaluate(t));*/
            current.localScale = Vector3.Lerp(current.localScale, indexStates[curindex].Scale, ScaleCurve.Evaluate(t));
            current.localPosition = Vector3.Lerp(current.localPosition, indexStates[curindex].Pos, PosCurve.Evaluate(t));
            current.localEulerAngles = Vector3.Lerp(current.eulerAngles, indexStates[curindex].Rot, RotCurve.Evaluate(t));
        }

        public override void Init(Item_Map from)
        {
            throw new Main.MainSystemException("IndexStateController cannt initmap");
        }

        public override bool IsIgnore()
        {
            return isIgnore;
        }

        public override Item_Map ToMap()
        {
            throw new Main.MainSystemException("IndexStateController cannt tomap");
        }
    }

    public class IndexStateController_Map : Item_Map
    {
        public override void Init(Item from)
        {
            throw new Main.MainSystemException("IndexStateController_Map cannt init");
            //Init_Lineage(from);
        }
    }
}