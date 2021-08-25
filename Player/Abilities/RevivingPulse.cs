using System.Collections.Generic;
using UnityEngine;

public class RevivingPulse : Ability
{

    public override void Awake()
    {
        heldItemName = "REVIVING PULSE";
        heldItemDescription = "An immediate revival wave that either Revives downed Teammates or stuns and hurts Enemies. " +
            "More Health (and Damage) is given the longer it is charged.";
        maxUses = 1;
        cooldown = 7f;
        requireCook = false;
        base.Awake();
    }

    //No weapons
    public override void SwitchToAbility()
    {
        base.SwitchToAbility();
    }
    //Charge healing pulse. Longer charge, longer cooldown
    public override void PrepareAbility()
    {
        base.PrepareAbility();
    }
    //Have no weapons to put away anyway
    public override void PutAwayAbility()
    {
        base.PutAwayAbility();
    }
    //Healing Pulse to use
    public override void UseAbility()
    {
        base.UseAbility();
    }
    //What the hell was this for again
    public override void SetAbility()
    {
        base.SetAbility();
    }
}
