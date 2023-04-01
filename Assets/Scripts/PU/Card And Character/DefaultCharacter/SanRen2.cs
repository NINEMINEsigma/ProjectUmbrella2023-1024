using System.Collections.Generic;
using Base.Animation;
using PU.Card;

namespace PU.Characters.Default
{
    public class SanRen2 : Character
    {
        public SanRen2() : base(new SanRen2_CharacterAttribute(), new SanRen2_CharacterResource(), new SanRen2_CharacterLabel(), new SanRen2_CardSlot()) { }
    }

    public class SanRen2_CharacterAttribute : CharacterAttribute
    {
        public SanRen2_CharacterAttribute() : base(new SanRen2_BasicAttribute()) { }

        public class SanRen2_BasicAttribute : BasicAttribute
        {
            public SanRen2_BasicAttribute()
            {
                _m_name.TryAdd("default", "…¢»À2");
                _m_maxHP = new int[3] { 7, 7, 7 };
                _m_damage = new int[3] { 1, 1, 1 };
                _m_scope = new int[3] { 1, 1, 1 };
                _m_evalue_hitedCurve = new FlexibleInt(new List<AnimationInt>() { new AnimationInt() { time = new(0, 7) } });
                _m_labels = new();
                _m_technique_structure = new() { 0 };
            }
        }
    }

    public class SanRen2_CharacterResource : CharacterResource
    {
        public SanRen2_CharacterResource() : base("character/dafault/illustration/SanRen1", "null", "null", "null") { }
    }

    public class SanRen2_CharacterLabel : CharacterLabel
    {
        public SanRen2_CharacterLabel() : base(new()) { }
    }

    public class SanRen2_CardSlot : CardSlot
    {
        public SanRen2_CardSlot() : base(new(), new(), new(), new()) { }
    }
}