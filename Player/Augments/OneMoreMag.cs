
public class OneMoreMag : Augment
{
    private Mercenary mercenary;

    private void Start()
    {
        name = "One More Mag";
        description = "Increase the number of magazines to carry and starting magazines by 1.";
    }

    public override void TurnOnAugment()
    {
        base.TurnOnAugment();
        mercenary = augmentHolder.GetComponentInParent<Mercenary>();
        if (mercenary.GetComponent<PlayerWeaponControls>().GetPrimaryGun() != null)
        {
            mercenary.GetComponent<PlayerWeaponControls>().GetPrimaryGun().OneMoreMagOn();
            mercenary.GetComponent<PlayerWeaponControls>().GetSecondaryGun().OneMoreMagOn();
        }
    }
    public override void TurnOffAugment()
    {
        base.TurnOffAugment();
        if (mercenary.GetComponent<PlayerWeaponControls>().GetPrimaryGun() != null)
        {
            mercenary.GetComponent<PlayerWeaponControls>().GetPrimaryGun().OneMoreMagOff();
            mercenary.GetComponent<PlayerWeaponControls>().GetSecondaryGun().OneMoreMagOff();
        }
    }
}
