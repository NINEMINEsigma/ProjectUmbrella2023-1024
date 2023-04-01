using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlasticPipe.Server.MonitorStats;

namespace PU.Card.Infomation
{
    public class AttackInformation
    {
        public AttackInformation()
        {
        }

        public AttackInformation(int init_value)
        {
            AttackAmount = init_value;
        }

        public float AttackAmount { get; set; }
        public int ActualAttackAmount { get; set; }
        public List<PU.Card.Buff.Buff> Buffs { get; set; }
        public List<PU.Card.Buff.Buff> LabelBuffs { get; set; }


    }
}
