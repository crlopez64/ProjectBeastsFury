using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtraPadding : Augment
{
    Mercenary mercenary;

    private void Start()
    {
        name = "Extra Padding";
        description = "Decreases the bullet damage taken by 8%.";
    }
    public override void TurnOnAugment()
    {
        base.TurnOnAugment();
        mercenary = augmentHolder.GetComponentInParent<Mercenary>();
        mercenary.GetComponent<PlayerHealth>().SetExtraPadding(true);
    }
}
