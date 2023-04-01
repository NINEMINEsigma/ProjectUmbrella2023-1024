using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PU.Card.Buff
{
    public abstract class Buff
    {
        public Buff() { }

        public string _m_name = "default";
        public string _m_description = "default";

        public List<string> _m_label = new();

        //Whether to settle at the time of injury
        public abstract bool IsTriggerByInjury();
        //Whether it is settled after the injury (hited).
        public abstract bool IsDelay();
        //Whether it is an effect with the attack
        public abstract bool IsAttachment();
        //Whether to calculate attack power immediately when attacking
        public abstract bool IsImmediatelyCalculate();
        //Whether to attach a label
        public abstract bool IsAttachLabel();

        //Whether to ignore any judgment and take effect directly
        //And True is Use Operation(PU.Card.Character target)
        //if not direct Operation will Use Operation(PU.Card.Character target,string label);
        public abstract bool IsDirect();
        //Whether the target is an friend force
        public abstract bool IsTargetFriend();
        //Whether the target is an enemy force
        public abstract bool IsTargetEnemy();


        //Perform the action

        //Direct manipulation objects, mostly feedback callbacks
        public abstract void Operation(PU.Card.Character target);
        //Operational numbers, most of which are operational damage values
        public abstract float Operation(PU.Card.Character target,float from);
        //Detect tags to generate new buffs
        //The subbuff if not direct Operation will Use Operation(PU.Card.Character target,float from)
        public abstract Buff Operation(PU.Card.Character target,string label);

        //Returns the operation sign
        public abstract string GetOperationSign();
    }

    public abstract class FriendlyBuff : Buff
    {
        public override abstract string GetOperationSign();

        public override abstract bool IsAttachLabel();

        public override abstract bool IsAttachment();

        public override abstract bool IsDelay();    

        public override abstract bool IsDirect();

        public override abstract bool IsImmediatelyCalculate();

        public override bool IsTargetEnemy()
        {
            return false;
        }

        public override bool IsTargetFriend()
        {
            return true;
        }

        public override abstract bool IsTriggerByInjury();

        public override abstract void Operation(Character target);

        public override abstract float Operation(Character target, float from);

        public override abstract Buff Operation(Character target, string label);
    }

    public abstract class HostilelyBuff : Buff
    {
        public override abstract string GetOperationSign();

        public override abstract bool IsAttachLabel();

        public override abstract bool IsAttachment();

        public override abstract bool IsDelay();

        public override abstract bool IsDirect();

        public override abstract bool IsImmediatelyCalculate();

        public override bool IsTargetEnemy()
        {
            return true;
        }

        public override bool IsTargetFriend()
        {
            return false;
        }

        public override abstract bool IsTriggerByInjury();

        public override abstract void Operation(Character target);

        public override abstract float Operation(Character target, float from);

        public override abstract Buff Operation(Character target, string label);
    }
}
