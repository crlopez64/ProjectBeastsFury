using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mountain : Augment
{
    private Mercenary mercenary;

    private void Start()
    {
        name = "Mountain";
        description = "Decreases explosive damage taken by 20%.";
    }
    public override void TurnOnAugment()
    {
        base.TurnOnAugment();
        mercenary = augmentHolder.GetComponentInParent<Mercenary>();
        mercenary.GetComponent<PlayerHealth>().SetMountain(true);
    }
}
