using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Base;
using System;

namespace Item.UI.Plot
{
    [Serializable]public class TimeFastImageConversion : ItemEvent<bool> { }
    [Serializable] public class StorytellerEvent : ItemEvent<bool> { }

    public class Storyteller : PlotController
    {
        [Header("StorytellerAsset")]
        [SerializeField] SpriteController FastSC;
        [SerializeField] Animator MenuPanelAnimator,LoadingAnimator; 
        [SerializeField]SpriteController charater1, charater2;
        [Header("Event")]
        [SerializeField] TimeFastImageConversion OnFast;
        [SerializeField] StorytellerEvent OnBack, OnSave;
        [Header("EventSystem")]
        [SerializeField] GameObject __EventSystem;

        public override void Start()
        {
            GameObject curfindsystem = GameObject.Find("EventSystem");
            if (curfindsystem == null || curfindsystem == __EventSystem) __EventSystem.SetActive(true);
            base.Start();
            if (OnFast.GetPersistentEventCount() == 0) OnFast.AddListener(BaseTimeFastImageConversion);
        }

        [Header("State"),SerializeField]bool isMenuPanelDisplay = false;
        public void MenuPanelDisplay()
        {
            if (MenuPanelAnimator == null) return;
            isMenuPanelDisplay = !isMenuPanelDisplay;
            MenuPanelAnimator.SetBool("isMenuPanelDisplay", isMenuPanelDisplay);
            if(isHistroy&&!isMenuPanelDisplay)
            {
                Histroy();
            }
        }

        [SerializeField]bool isLoading;
        public void Loading()
        {
            if (LoadingAnimator == null) return;
            isLoading = !isLoading;
            LoadingAnimator.SetBool("isLoading", isLoading);
        }

        public void Back_()
        {
            Back();
            if(isHistroy)
            {
                OnBack.Invoke(false);
            }
            else
            {
                OnBack.Invoke(false);
                OnSave.Invoke(false);
            }
        }
        public override void Back()
        {
            base.Back();
        }

        public void Fast_()
        {
            Fast();
        }
        public override bool Fast()
        {
            bool cat = base.Fast();
            OnFast.Invoke(cat);
            return cat;
        }
        private void BaseTimeFastImageConversion(bool b)
        {
            if (FastSC == null) return;
            FastSC.SetColor(1, 1, (b) ? 0 : 1);
        }

        public override void Init(Item_Map from)
        {
            base.Init(from);
            Storyteller_Map cat = from as Storyteller_Map;
        }
    }

    public class Storyteller_Map : PlotController_Map
    {

        public override void Init(Item from)
        {
            base.Init(from);
            Storyteller cat = from as Storyteller;
        }
    }


}