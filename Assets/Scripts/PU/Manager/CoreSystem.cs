using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Base;
using PU.Card;
using PU.Card.Instance;
using PU.Map;
using System;
using PU.Element;
using Item.UI;

namespace PU.System
{
    class CoreSystem : UnderlyingObject
    {
        public static CoreSystem instance;

        //Init
        //BLOCK

        [SerializeField] PlayerPlane PlayerPlane_Prefab;
        [SerializeField] IndexStateController PPSIC;//PlayerPlane_IndexStateController

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                PrepareCardPool.Init();
                (Base.Start.LaunchObject.instance.info.data_information as SelectInfo).InitSelect();
                //Find PreParationTransform
                for(int i = 0;i<_s_max_playere;i++)
                {
                    PlayerPlane cat=Instantiate(PlayerPlane_Prefab, PPSIC.transform);
                    PPSIC.GetIndexStates().Add(new(new(cat.PlaneRectTransform.rect.width * i, 0, 0), new(), new(1, 1, 1)));
                    PPSIC.Add(cat.transform);
                    MTransforms_Dictionary.TryAdd(players[i], cat.MTransform);
                }
                PPSIC.IndexUpdate_AfterChange();
            }
            else throw new BaseOutException("Too many instances");
        }

        //G
        //BLOCK

        public static string language = "default";

        //Preparation
        //SUBBLOCK

        public PrepareCard PrepareCard_Prefab;

        [Serializable]
        public class PreparationInfomation
        {
            public Transform PTransform { get; set; }
            public List<PrepareCard> PreparationSlot { get; set; }
        }

        public Dictionary<Player, PreparationInfomation> PreparationInfomations_Dictionary = new();
        public Dictionary<Player, Transform> MTransforms_Dictionary = new();
        public Transform Current_PreparationSlot_Parent => PreparationInfomations_Dictionary[Current_Player].PTransform;
        public Transform Current_PreparationSlot_Parent_M => MTransforms_Dictionary[Current_Player];
        public List<PrepareCard> Current_PreparationSlot => PreparationInfomations_Dictionary[Current_Player].PreparationSlot;

        public bool UpdatePreparationSlot_TestCardCount()
        {
            return Current_PreparationSlot.Count < 7;
        }
        public void UpdatePreparationSlot_AddNewCard()
        {
            var cat = PrepareCardPool.TryObtainPrepareCard(Current_Player.characters[UnityEngine.Random.Range(0, Current_Player.characters.Count)]);
            cat.transform.localPosition =
                (Current_PreparationSlot.Count > 0) ?
                Current_PreparationSlot[^1].transform.localPosition :
                Current_PreparationSlot_Parent_M.transform.localPosition
                + new Vector3(cat.GetComponent<RectTransform>().rect.x, 0);
            Current_PreparationSlot.Add(cat);
        }
        public void UpdatePreparationSlot_OperateCard(PrepareCard card)
        {
            int catch_index = Current_PreparationSlot.FindIndex(T => { return T == card; });
            Character target_character = card._m_character;
            int level = 1;
            if (catch_index == -1)
            {
                Debug.LogWarning("error UpdatePreparationSlot_OperateCard");
                return;
            }
            if (catch_index > 0 && Current_PreparationSlot[catch_index - 1]._m_character == target_character)
            {
                level++;
                PrepareCardPool.Back(Current_PreparationSlot[catch_index - 1]);
                Current_PreparationSlot.RemoveAt(catch_index - 1);
            }
            if (catch_index < Current_PreparationSlot.Count && Current_PreparationSlot[catch_index + 1]._m_character == target_character)
            {
                level++;
                PrepareCardPool.Back(Current_PreparationSlot[catch_index + 1]);
                Current_PreparationSlot.RemoveAt(catch_index + 1);
            }
            PrepareCardPool.Back(card);
            PreparationalAddCardToReady(level,target_character);
        }
        public void PreparationalAddCardToReady(int level,Character character)
        {
            character.InitCard(level);
            Current_ReadyCharacters.Add(character);
        }

        //Ready
        //SUBBLOCK

        public Dictionary<Player, List<Character>> ReadyCharacters_Dictionary = new();
        public List<Character> Current_ReadyCharacters => ReadyCharacters_Dictionary[players[_m_current_player]];



        //Game
        //SUBBLOCK

        public RootBlock _m_rootblock = null;

        public void GenerateMap(int x, int y)
        {
            _m_rootblock = new(x, y);
        }

        public List<PU.Card.Character> characters = new List<PU.Card.Character>();

        public void InitCharacterList(List<PU.Card.Character> from_characters)
        {
            characters = from_characters;
        }

        public int _m_round_score { get { return 2; } }
        public int _m_start_cost { get { return 10; } }
        public int _m_max_cost { get { return 100; } }
        public int _m_max_player = 2;
        public static int _s_max_playere = 2;

        //Default
        public void InitGameInfomation()
        {
            _m_max_player = 2;
            players = new List<Player>(_m_max_player);
            for (int i = 0; i < players.Count; i++)
            {
                players[i].cost = _m_start_cost;
                players[i].id = i;
                players[i].name += i.ToString();
            }
            _m_current_round = 0;
            _m_current_player = 0;
        }

        public void RoundUpdate()
        {
            _m_current_round++;
            _m_current_player = 0;
        }

        //Handle the current match
        public void CurrentPlayerUpdate()
        {
            PPSIC.ChangeIndex(_m_current_player);
            Current_Player.round++;
            Current_Player.cost++;
            Current_Player.round_cost = _m_round_score;
        }

        //Handle the current round
        public Player CurrentPlayerRoundUpdate(Player current)
        {
            current.round_cost--;
            return current;
        }
        public Player CurrentPlayerRoundUpdate(int i)
        {
            return CurrentPlayerRoundUpdate(players[i]);
        }

        public int _m_current_round { get; set; }
        public int _m_current_player { get; set; }
        public List<Player> players = null;
        public Player Current_Player { get { return players[_m_current_player]; }set { players[_m_current_player] = value; } }

        public void CharacterAttack(Character A, Block targetBlock)
        {
            Character B = targetBlock.character;
            if (B != null)
            {
                var attackInformation = B.Calculating_AttackInformation(A.LaunchAttack(targetBlock));
                A.AttackFeedback(attackInformation);
                B.InjuryFeedback(attackInformation);
            }
        }
    }

    public class Player
    {
        public int id = 0;
        public string name = "player";
        public string description = "default";
        public int cost = 0;
        public int round = 0;
        public int round_cost = 0;
        public List<Character> characters = new();
    }
}
