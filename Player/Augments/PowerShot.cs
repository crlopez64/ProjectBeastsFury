using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerShot : Augment
{
    private void Start()
    {
        name = "Power Shot";
        description = "Increases the damage output from bullets by 8%.";
    }

    public override void TurnOnAugment()
    {
        base.TurnOnAugment();

    }
}
