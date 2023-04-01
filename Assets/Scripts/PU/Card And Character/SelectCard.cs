using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Base;
using Item.UI;
using Item.Control.AD;
using PU.System;

namespace PU.Card.Instance
{
    public class SelectCard : UnderlyingObject
    {
        public PU.Card.Character _m_character = null;

        [SerializeField] UIButton _m_button;
        [SerializeField] ADTC phase1blood, phase2blood, phase3blood;
        [SerializeField] ADTC phase1damage, phase2damage, phase3damage;
        [SerializeField] bool isCharSelect = true;
        [SerializeField] bool isGameSelect = false;

        public void Init(PU.Card.Character character)
        {
            _m_character = character;
            name = (_m_character._m_characterAttribute._m_attribute._m_name.ContainsKey(CoreSystem.language)) 
                ? _m_character._m_characterAttribute._m_attribute._m_name[CoreSystem.language] : "default";
            if (_m_character.TryUpdataIllustration())
                _m_button.SC.Sprite_Update0(_m_character._m_illustration);
            phase1blood.SetText(_m_character._m_characterAttribute._m_attribute._m_maxHP[0].ToString());
            phase2blood.SetText(_m_character._m_characterAttribute._m_attribute._m_maxHP[1].ToString());
            phase3blood.SetText(_m_character._m_characterAttribute._m_attribute._m_maxHP[2].ToString());
            phase1damage.SetText(_m_character._m_characterAttribute._m_attribute._m_damage[0].ToString());
            phase2damage.SetText(_m_character._m_characterAttribute._m_attribute._m_damage[1].ToString());
            phase3damage.SetText(_m_character._m_characterAttribute._m_attribute._m_damage[2].ToString());
        }

        public void InitLocalPos(int index)
        {
            transform.localPosition += new Vector3(50 * index, 0, 0);
        }

        public void OnClick()
        {
            if(isCharSelect) SelectManager.instance.ChooseSelectCard(this);
        }

    }
}
