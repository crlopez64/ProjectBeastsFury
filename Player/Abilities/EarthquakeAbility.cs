using System.Collections.Generic;
using UnityEngine;

public class EarthquakeAbility : Ability
{

    public override void Awake()
    {
        heldItemName = "EARTHQUAKE SIMULATOR";
        heldItemDescription = "A deployable that can shake up the ground within its radius, stunning any grounded Enemies nearby.";
        maxUses = 1;
        cooldown = 20f;
        requireCook = false;
        base.Awake();
    }

    //Bring out donut
    public override void SwitchToAbility()
    {
        base.SwitchToAbility();
    }
    //Put away donut
    public override void PutAwayAbility()
    {
        base.PutAwayAbility();
    }
    //Throw it out
    public override void UseAbility()
    {
        base.UseAbility();
    }
}
