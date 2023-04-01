using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Base.Animation;
using System;
using Base.Information;
using System.Linq;
using PU.Card.Buff;
using PU.Card.Infomation;

namespace PU.Card
{
    [Serializable]
    public class BasicAttribute
    {
        //The name of the multiple language
        public Dictionary<string, string> _m_name = new();
        public int[] _m_maxHP = new int[3] { 1, 1, 1 };
        public int[] _m_damage = new int[3] { 1, 1, 1 };
        public int[] _m_scope=new int[3] { 1, 2, 3 };
        //A hit curve at which the operation actually takes damage
        public FlexibleInt _m_evalue_hitedCurve = new(new List<AnimationInt>() { new AnimationInt() { time = new() { start = 0, end = 100 } } });
        public FlexibleInt _m_evCurve => _m_evalue_hitedCurve;
        //A label slot
        //Used for various signs similar to race
        public List<string> _m_labels = new();
        //How artificial skills are constructed
        public List<int> _m_technique_structure = new();

        public string _m_characterDescription = "dafault";
        public string _m_techniqueDescription = "dafault";

        public BasicAttribute(Dictionary<string, string> name,
                              int[] maxHP,
                              int[] damage,
                              int[] scope,
                              FlexibleInt evalue_hitedCurve,
                              List<string> labels,
                              List<int> technique_structure)
        {
            _m_name = name;
            _m_maxHP = maxHP;
            _m_damage = damage;
            _m_scope = scope;
            _m_evalue_hitedCurve = evalue_hitedCurve;
            _m_labels = labels;
            _m_technique_structure = technique_structure;
        }

        public BasicAttribute()
        {
        }
    }

    [Serializable]
    public class CharacterAttribute
    {
        public BasicAttribute _m_attribute = new();

        public CharacterAttribute()
        {
        }

        public CharacterAttribute(BasicAttribute attribute)
        {
            _m_attribute = attribute;
        }

    }

    [Serializable]
    public class CharacterResource
    {
        public string _m_illustration = "default";
        public string _m_song = "default";
        public string _m_material = "default";
        public string _m_animation="default";

        public CharacterResource()
        {
        }

        public CharacterResource(string illustration, string song, string material, string animation)
        {
            _m_illustration = illustration;
            _m_song = song;
            _m_material = material;
            _m_animation = animation;
        }
    }

    [Serializable]
    public class CharacterLabel
    {
        public List<string> _m_labels = new();

        public CharacterLabel()
        {
        }

        public CharacterLabel(List<string> labels)
        {
            _m_labels = labels;
        }
    }

    [Serializable]
    public class CardSlot
    {
        public Dictionary<string, PU.Card.Buff.Buff> _m_buff = new();
        public Dictionary<string, PU.Card.Buff.Buff> _m_debuff = new();
        public List<Action> _m_delayTable = new();
        public Dictionary<int,Action> _m_timeSeriesTable = new();

        public List<PU.Card.Buff.Buff> _m_conditions = new();

        public CardSlot()
        {
        }

        public CardSlot(Dictionary<string, Buff.Buff> buff, Dictionary<string, Buff.Buff> debuff, List<Action> delayTable, Dictionary<int, Action> timeSeriesTable)
        {
            _m_buff = buff;
            _m_debuff = debuff;
            _m_delayTable = delayTable;
            _m_timeSeriesTable = timeSeriesTable;
        }
    }

    [Serializable]
    public class CardException : Base.BaseOutException
    {
        public CardException() { }
        public CardException(string message) : base(message + "_In_Throw_" + Time.time) { }
        public CardException(string message, Exception inner) : base(message + "_In_Throw_" + Time.time, inner) { }
        protected CardException(
          global::System.Runtime.Serialization.SerializationInfo info,
          global::System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

    [Serializable]
    public class Character
    {
        public CharacterAttribute _m_characterAttribute = new();
        public CharacterResource _m_characterResource = new();
        public CharacterLabel _m_characterLabel = new();
        public CardSlot _m_cardSlot = new();

        public int _m_playerindex = 0;

        public Sprite _m_illustration = null;

        public bool TryUpdataIllustration()
        {
            _m_illustration = Resources.Load<Sprite>(_m_characterResource._m_illustration);
            if (_m_illustration == null) return false;
            else return true;
        }

        public int _m_phase = 0;
        public float _m_remainingBloodVolume = 1;
        public int MAXHP => _m_characterAttribute._m_attribute._m_maxHP[_m_phase];
        public int HP = 0;
        public int Damage = 0;
        public int Scope = 0;
        //A hit curve at which the operation actually takes damage
        public FlexibleInt EHitedCurve => _m_characterAttribute._m_attribute._m_evCurve;
        //A label slot
        //Used for various signs similar to race
        public List<string> Labels => _m_characterAttribute._m_attribute._m_labels;
        //How artificial skills are constructed
        public List<int> Technique_structure => _m_characterAttribute._m_attribute._m_technique_structure;

        public string _m_characterDescription => _m_characterAttribute._m_attribute._m_characterDescription;
        public string _m_techniqueDescription => _m_characterAttribute._m_attribute._m_techniqueDescription;

        public void InitCard(int _t_phase)
        {
            _m_phase = _t_phase;
            HP = (int)(_m_characterAttribute._m_attribute._m_maxHP[_m_phase] * _m_remainingBloodVolume) + 1;
            Damage = _m_characterAttribute._m_attribute._m_damage[_m_phase];
            Scope = _m_characterAttribute._m_attribute._m_scope[_m_phase];
        }
        public void QuitCard()
        {
            _m_remainingBloodVolume = HP / (float)_m_characterAttribute._m_attribute._m_maxHP[_m_phase];
        }

        //Should be called by the master core in the first step
        //The location information is then passed to the block for final calculation
        public PU.Card.Infomation.AttackInformation Generate_AttackInformation(bool IsTargetFriend)
        {
            PU.Card.Infomation.AttackInformation attackInformation = new(_m_characterAttribute._m_attribute._m_damage[_m_phase]);
            _attackInfomation_updata_buff(attackInformation, _m_cardSlot._m_buff);
            _attackInfomation_updata_buff(attackInformation, _m_cardSlot._m_debuff);
            _attackInfomation_updata_label_buff(attackInformation, _m_cardSlot._m_buff);
            _attackInfomation_updata_label_buff(attackInformation, _m_cardSlot._m_debuff);
            foreach (var buff in _m_cardSlot._m_conditions)
            {
                if (!(buff.IsTargetFriend() & IsTargetFriend)) continue;
                if (buff.IsImmediatelyCalculate()) attackInformation.AttackAmount = buff.Operation(this, attackInformation.AttackAmount);
                else if (buff.IsAttachment()) attackInformation.Buffs.Add(buff);
            }
            return attackInformation;

            void _attackInfomation_updata_buff(Infomation.AttackInformation attackInformation, Dictionary<string, PU.Card.Buff.Buff> buffs)
            {
                foreach (var buff in from item in buffs
                                     let buff = item.Value
                                     select buff)
                {
                    if (!(buff.IsTargetFriend() & IsTargetFriend)) continue;
                    if (buff.IsImmediatelyCalculate())
                        attackInformation.AttackAmount = buff.Operation(this, attackInformation.AttackAmount);
                    else
                    {
                        if (buff.IsAttachment()) attackInformation.Buffs.Add(buff);
                        if (buff.IsAttachLabel()) attackInformation.LabelBuffs.Add(buff);
                    }
                }
                /*
                foreach (var item in buffs)
                {
                    var buff = item.Value;
                    if (!(buff.IsTargetFriend() & IsTargetFriend)) continue;
                    if (buff.IsImmediatelyCalculate())
                        attackInformation.AttackAmount = buff.Operation(this, attackInformation.AttackAmount);
                    else
                    {
                        if (buff.IsAttachment()) attackInformation.Buffs.Add(buff);
                        if (buff.IsAttachLabel()) attackInformation.LabelBuffs.Add(buff);
                    }
                }
                 */
            }

            void _attackInfomation_updata_label_buff(AttackInformation attackInformation, Dictionary<string, PU.Card.Buff.Buff> buffs)
            {
                attackInformation.Buffs.AddRange(from item in buffs
                                                 let buff = item.Value
                                                 where buff.IsAttachLabel() && (buff.IsTargetFriend() & IsTargetFriend)
                                                 from label in _m_characterLabel._m_labels
                                                 let cat = buff.Operation(this, label)
                                                 where cat != null
                                                 select cat);
                /*
                foreach (var item in buffs)
                {
                    var buff = item.Value;
                    if (buff.IsAttachLabel() && (buff.IsTargetFriend() & IsTargetFriend))
                    {
                        foreach (var label in _m_characterLabel._m_labels)
                        {
                            var cat = buff.Operation(this, label);
                            if (cat != null)
                            {
                                attackInformation.Buffs.Add(cat);
                            }
                        }
                    }
                }
                 */
            }
        }

        PU.Card.Infomation.AttackInformation _tmp_attackInformation_when_Calculating_AttackInformation = null;
        public PU.Card.Infomation.AttackInformation CurrentAttackInformation => _tmp_attackInformation_when_Calculating_AttackInformation;

        //Calculations after an attack
        public PU.Card.Infomation.AttackInformation Calculating_AttackInformation(PU.Card.Infomation.AttackInformation attackInformation)
        {
            //catch
            _tmp_attackInformation_when_Calculating_AttackInformation = attackInformation;
            //Handle the buff executed before the attack
            foreach (var buff in from buff in attackInformation.Buffs
                                 where !buff.IsDelay()
                                 select buff)
            {
                if (buff.IsDirect()) buff.Operation(this);
                else CheakLabel_Buff_WithSubbuff(attackInformation, buff);
            }

            float EAttackAmount = attackInformation.AttackAmount * EHitedCurve.GetValue(attackInformation.AttackAmount);
            int ActualAttackAmount = (int)Mathf.Clamp(EAttackAmount, 0, MAXHP / 2.0f);
            HP -= ActualAttackAmount;
            attackInformation.ActualAttackAmount = ActualAttackAmount;

            //Handle the buff executed after the attack
            foreach (var buff in from buff in attackInformation.Buffs
                                 where buff.IsDelay()
                                 select buff)
            {
                if (buff.IsDirect()) buff.Operation(this);
                else CheakLabel_Buff_WithSubbuff(attackInformation, buff);
            }
            //TODO
            //clear
            _tmp_attackInformation_when_Calculating_AttackInformation = null;
            return attackInformation;

            void CheakLabel_Buff_WithSubbuff(AttackInformation attackInformation, Buff.Buff buff)
            {
                foreach (var label in Labels)
                {
                    var subuff = buff.Operation(this, label);
                    if (subuff == null) continue;
                    if (subuff.IsDirect()) subuff.Operation(this);
                    else attackInformation.AttackAmount = subuff.Operation(this, attackInformation.AttackAmount);
                }
            }
        }
        public PU.Card.Infomation.AttackInformation LaunchAttack(PU.Map.Block targetBlock)
        {
            //Cannot be empty
            if (targetBlock.character == null)
                throw new CardException("null character(LaunchAttack target)");
            //count caculate
            PU.System.CoreSystem.instance.CurrentPlayerRoundUpdate(_m_playerindex);
            if (PU.System.CoreSystem.instance.players[_m_playerindex].characters.Contains(targetBlock.character))
                return LaunchAttack_friend(targetBlock, targetBlock.character);
            else
                return LaunchAttack_enemy(targetBlock, targetBlock.character);
        }

        PU.Card.Infomation.AttackInformation LaunchAttack_friend(PU.Map.Block targetBlock, Character friend) => LaunchAttack_Type(targetBlock, friend, true);
        PU.Card.Infomation.AttackInformation LaunchAttack_enemy(PU.Map.Block targetBlock, Character enemy) => LaunchAttack_Type(targetBlock, enemy, false);
        PU.Card.Infomation.AttackInformation LaunchAttack_Type(PU.Map.Block targetBlock,
                                                                Character target,
                                                                bool IsFirend)
        {
            var attackInformation = Generate_AttackInformation(IsFirend);
            attackInformation.Buffs.AddRange(from buff in targetBlock._m_buffs
                                             where buff.IsTargetFriend() & IsFirend
                                             select buff);
            return attackInformation;
        }

        //The attacker uses this function
        public void AttackFeedback(PU.Card.Infomation.AttackInformation attackInformation)
        {

        }
        //The affected actor uses this function
        public void InjuryFeedback(PU.Card.Infomation.AttackInformation attackInformation)
        {

        }

        //Crisis mode
        //when HP=2
        public void TryCrisisMode()
        {

        }

        protected Character()
        {
        }

        public Character(CharacterAttribute characterAttribute, CharacterResource characterResource, CharacterLabel characterLabel, CardSlot cardSlot)
        {
            _m_characterAttribute = characterAttribute;
            _m_characterResource = characterResource;
            _m_characterLabel = characterLabel;
            _m_cardSlot = cardSlot;
        }

        static public T Make<T>() where T : Character ,new()
        {
            return new T();
        }
    }

}