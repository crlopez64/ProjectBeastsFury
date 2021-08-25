using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SleightOfHand : Augment
{
    private Mercenary mercenary;

    private void Start()
    {
        name = "Sleight of Hand";
        description = "Reduces the time it takes to Reload by 20%";
    }

    public override void TurnOnAugment()
    {
        base.TurnOnAugment();
        mercenary = augmentHolder.GetComponentInParent<Mercenary>();
        if (mercenary.GetComponent<PlayerWeaponControls>().GetPrimaryGun() != null)
        {
            mercenary.GetComponent<PlayerWeaponControls>().GetPrimaryGun().SetSleightOfHand(true);
            mercenary.GetComponent<PlayerWeaponControls>().GetSecondaryGun().SetSleightOfHand(true);
        }
    }
    public override void TurnOffAugment()
    {
        base.TurnOffAugment();
        if (mercenary.GetComponent<PlayerWeaponControls>().GetPrimaryGun() != null)
        {
            mercenary.GetComponent<PlayerWeaponControls>().GetPrimaryGun().SetSleightOfHand(false);
            mercenary.GetComponent<PlayerWeaponControls>().GetSecondaryGun().SetSleightOfHand(false);
        }
    }
}
