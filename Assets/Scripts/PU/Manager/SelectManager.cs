using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Base;
using Item.UI;
using PU.Card.Instance;
using PU.Card;
using PU.Characters.Default;

namespace PU.System
{
    public class SelectManager : UnderlyingObject
    {
        public static SelectManager instance;
        [SerializeField] SelectCard card_Prefab;
        [SerializeField] Transform card_Parent;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                SelectBag();
            }
            else throw new BaseOutException("Too many instances");
        }

        //S Bag
        //BLOCK

        [SerializeField] ADIFC selectBagFieldInput;

        public void SelectBag()
        {

            //TODO

            //Default

            SelectDefaultBag();
        }

        public void SelectDefaultBag()
        {
            foreach (var card in cards_objects) { Destroy(card.gameObject); }
            int index_c_t = 0;
            GenerateDefaultCharaacter<SanRen1>();
            GenerateDefaultCharaacter<SanRen2>();

            void GenerateDefaultCharaacter<T>() where T : Character, new()
            {
                SelectCard current = Instantiate(card_Prefab, card_Parent);
                current.Init(Character.Make<T>());
                current.InitLocalPos(index_c_t++);
                cards_objects.Add(current);
            }
        }

        [SerializeField] List<SelectCard> cards_objects;
        [SerializeField] IndexStateController cards_indexStateController;

        public void LeftPage()
        {
            cards_indexStateController.ChangeIndex_less();
        }

        public void RightPage()
        {
            cards_indexStateController.ChangeIndex_greater();
        }

        //S
        //BLOCK

        public void ChooseSelectCard(SelectCard select)
        {
            current_select_card = select;
        }

        public void FinishSelect()
        {
            info.characters = select_characters;
            Base.Start.LaunchObject.instance.info.data_information = info;
        }



        [SerializeField] SelectCard current_select_card = null;
        public List<Character> select_characters = new List<PU.Card.Character>();
        SelectInfo info = new();
    }

    public class SelectInfo:Base.Information.SubInformation
    {
        public List<Character> characters = new();

        public void InitSelect()
        {
            CoreSystem.instance.characters = characters;
        }
    }
}
