using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tough : Augment
{
    private Mercenary mercenary;

    private void Start()
    {
        name = "Tough";
        description = "Decrease the delay time before Health Regen by 66%.";
    }

    public override void TurnOnAugment()
    {
        base.TurnOnAugment();
        mercenary = augmentHolder.GetComponentInParent<Mercenary>();
        mercenary.GetComponent<PlayerHealth>().SetTough(true);
    }
}
