using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Defibrillator : Ability
{
    public override void Awake()
    {
        heldItemName = "DEFIBRILLATOR";
        heldItemDescription = "Traditional, and portable, paddles that either Revives downed Teammates or damages Enemies. " +
            "More Health (and Damage) is given the longer it is charged.";
        maxUses = 1;
        cooldown = 7f;
        requireCook = false;
        base.Awake();
    }

    //Bring out paddles
    public override void SwitchToAbility()
    {
        base.SwitchToAbility();
    }
    //Charge paddles. Longer charge, longer cooldown
    public override void PrepareAbility()
    {
        base.PrepareAbility();
    }
    //Put away paddles
    public override void PutAwayAbility()
    {
        base.PutAwayAbility();
    }
    //Revive to use
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
