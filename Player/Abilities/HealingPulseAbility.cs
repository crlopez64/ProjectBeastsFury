using System.Collections.Generic;
using UnityEngine;

public class HealingPulseAbility : Ability
{

    public override void Awake()
    {
        heldItemName = "HEALING PULSE";
        heldItemDescription = "An immediate healing wave that heals the User and Teammates within the radius. " +
            "More Health is restored the longer it is charged.";
        maxUses = 1;
        cooldown = 20f;
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
