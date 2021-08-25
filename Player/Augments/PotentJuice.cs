
public class PotentJuice : Augment
{
    private Mercenary mercenary;

    private void Start()
    {
        name = "Potent Juice";
        description = "Increase the Health Increase per tick by normal Health Regen from 1 to 2.";
    }

    public override void TurnOnAugment()
    {
        base.TurnOnAugment();
        mercenary = augmentHolder.GetComponentInParent<Mercenary>();
        mercenary.GetComponent<PlayerHealth>().SetPotentJuice(true);
    }
}
