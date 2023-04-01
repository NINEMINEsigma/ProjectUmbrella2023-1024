using System.Collections.Generic;
using Base.Animation;
using PU.Card;

namespace PU.Characters.Default
{
    public class SanRen1: Character
    {
        public SanRen1() : base(new SanRen1_CharacterAttribute(), new SanRen1_CharacterResource(), new SanRen1_CharacterLabel(), new SanRen1_CardSlot()) { }
    }

    public class SanRen1_CharacterAttribute : CharacterAttribute
    {
        public SanRen1_CharacterAttribute() : base(new SanRen1_BasicAttribute()) { }

        public class SanRen1_BasicAttribute : BasicAttribute
        {
            public SanRen1_BasicAttribute()
            {
                _m_name.TryAdd("default", "…¢»À");
                _m_maxHP = new int[3] { 7, 7, 7 };
                _m_damage = new int[3] { 1, 1, 1 };
                _m_scope = new int[3] { 1, 1, 1 };
                _m_evalue_hitedCurve = new FlexibleInt(new List<AnimationInt>() { new AnimationInt() { time = new(0, 7) } });
                _m_labels = new();
                _m_technique_structure = new() { 0 };
            }
        }
    }

    public class SanRen1_CharacterResource : CharacterResource
    {
        public SanRen1_CharacterResource() : base("character/dafault/illustration/SanRen1", "null", "null", "null") { }
    }

    public class SanRen1_CharacterLabel : CharacterLabel
    {
        public SanRen1_CharacterLabel() : base(new()) { }
    }

    public class SanRen1_CardSlot : CardSlot
    {
        public SanRen1_CardSlot() : base(new(), new(), new(), new()) { }
    }
}