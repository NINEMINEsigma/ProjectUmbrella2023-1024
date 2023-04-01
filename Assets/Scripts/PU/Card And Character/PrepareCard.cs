using System.Collections;
using System.Collections.Generic;
using Base;
using UnityEngine;
using Item.UI;
using Item.Control.AD;
using PU.System;

namespace PU.Card.Instance
{
    public class PrepareCard : UnderlyingObject
    {
        [HideInInspector] public PU.Card.Character _m_character = null;

        [SerializeField] UIButton _m_button;

        public void Init()
        {
            if (_m_character.TryUpdataIllustration())
                _m_button.SC.Sprite_Update0(_m_character._m_illustration);
        }

        public void Init(Character character)
        {
            _m_character = character;
            Init();
        }

        public void OnClick()
        {
            CoreSystem.instance.UpdatePreparationSlot_OperateCard(this);
        }
    }

    public static class PrepareCardPool
    {
        static public PrepareCard PrepareCard_Prefab => CoreSystem.instance.PrepareCard_Prefab;
        static public Transform PreparationSlot_Prefab => CoreSystem.instance.Current_PreparationSlot_Parent;
        static public List<PrepareCard> prepareCard_List;

        static public void Init()
        {
            prepareCard_List = new();
        }

        static PrepareCard GenerateCord()
        {
            return GameObject.Instantiate(PrepareCard_Prefab, PreparationSlot_Prefab);
        }

        static public PrepareCard TryObtainPrepareCard(Character character)
        {
            PrepareCard cat = null;
            if (prepareCard_List.Count == 0) cat = GenerateCord();
            else
            {
                cat = prepareCard_List[^1];
                prepareCard_List.Remove(cat);
            }
            cat.gameObject.SetActive(true);
            cat.Init(character);
            cat.transform.localPosition = new Vector3(0, -500, 0);
            return cat;
        }

        static public void Back(PrepareCard card)
        {
            card.gameObject.SetActive(false);
            prepareCard_List.Add(card);
        }
    }
}