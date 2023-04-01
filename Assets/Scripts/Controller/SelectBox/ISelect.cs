using Item.Control.Appendage;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Item.Control
{
    [RequireComponent(typeof(IMovement))]
    public class ISelect : Item
    {
        [Header("Asset")]
        [SerializeField] GameObject Panel;
        [HideInInspector]public bool isStart = false, isEnd = true, isMove = false;
        public string AssetName= "Default"; 
        [Header("Target")] public List<ISelect_Child> Targets = new();
        [HideInInspector] public static List<ISelect> selects = new();

        GameObject PanelGameObject_;
        GameObject PanelGameObject
        {
            get
            {
                if (PanelGameObject_ == null) PanelGameObject_ = Instantiate(Panel, transform);
                return PanelGameObject_;
            }
        }
        RectTransform PanelRect_;
        RectTransform PanelRect
        {
            get
            {
                if (PanelRect_ == null) PanelRect_ = PanelGameObject.GetComponent<RectTransform>();
                return PanelRect_;
            }
        }
        Vector3 Start_, End_, MoveRoad_ = new();

        public override void Awake()
        {
            base.Awake();
            var cat = GetComponent<IMovement>();
            cat.OnStart.AddListener(OnStart);
            cat.OnEnd.AddListener(OnEnd);
            cat.OnMove.AddListener(OnMove);
            selects.Add(this);
        }

        public override void Init(Item_Map from)
        {
            throw new Main.MainSystemException("ISelect cannt initmap");
        }

        public override bool IsIgnore() { return true; }

        public void Add(ISelect_Child target) { if (Targets.FindIndex(T => T == target) != -1) Targets.Add(target); }

        public void OnMove(Vector3 MovePos)
        {
            MoveRoad_ = MovePos;
            Debug.Log(MovePos);
            PanelUpdata();
        }
        public void OnEnd(Vector3 Pos)
        {
            End_ = Pos;
            MoveRoad_ = new();
            PanelUpdata();
            foreach (var it in Targets) it.OnDragEnded.Invoke(Start_,End_);
        }
        public void OnStart(Vector3 Pos)
        {
            Start_ = Pos;
        }

        void PanelUpdata()
        {
            if (Input.GetKey(KeyCode.LeftControl))
            {
                PanelRect.position = Start_ + MoveRoad_ * 0.5f;
                PanelRect.sizeDelta = new(Mathf.Abs(MoveRoad_.x), (Mathf.Abs(MoveRoad_.y)));
            }
            else
            {
                PanelRect.sizeDelta = new();
                Start_ = End_ = MoveRoad_ = new();
            }
        }

        public override Item_Map ToMap()
        {
            throw new Main.MainSystemException("ISelect cannt tomap");
        }
    }
}
