using UnityEngine;

/// <summary>
/// Script that enables the Grenade Ability: Letting the Unit throw one fragmentation grenade.
/// </summary>
public class GrenadeAbility : Ability
{
    private Grenade grenadeScript;
    private GameObject grenadeReuse;

    //TODO: Remove this reference and place Grenade prefab into Resources
    public GameObject grenade;

    public override void Awake()
    {
        heldItemName = "FRAG GRENADE";
        heldItemDescription = "Throw a lethal grenade with a rather short cook time. Effective against Enemies.";
        maxUses = 1;
        cooldown = 16f;
        requireCook = true;
        grenadeReuse = Instantiate(grenade, transform.position, transform.rotation);
        grenadeScript = grenadeReuse.GetComponent<Grenade>();
        grenadeScript.SetParentAbility(this);
        grenadeReuse.transform.SetParent(transform);
        grenadeReuse.transform.localPosition = grenadeReuse.GetComponent<Grenade>().hipFirePosition;
        grenadeReuse.GetComponent<Rigidbody>().isKinematic = true;
        grenadeReuse.SetActive(false);
        base.Awake();
    }
    //TODO: Possibly remove SetAbility()
    public override void SetAbility()
    {
        grenadeReuse.GetComponent<Grenade>().ResetThrowable();
        grenadeReuse.transform.SetParent(transform);
        grenadeReuse.transform.localPosition = grenadeReuse.GetComponent<Grenade>().hipFirePosition;
        grenadeReuse.GetComponent<Rigidbody>().isKinematic = true;
        grenadeReuse.SetActive(true);
    }
    public override void SwitchToAbility()
    {
        base.SwitchToAbility();
        grenadeReuse.GetComponent<Grenade>().ResetThrowable();
        grenadeReuse.transform.SetParent(transform);
        grenadeReuse.transform.localPosition = grenadeReuse.GetComponent<Grenade>().hipFirePosition;
        grenadeReuse.GetComponent<Rigidbody>().isKinematic = true;
        grenadeReuse.SetActive(true);
    }
    public override void PutAwayAbility()
    {
        base.PutAwayAbility();
        grenadeReuse.SetActive(false);
    }
    public override void PrepareAbility()
    {
        base.PrepareAbility();
        grenadeReuse.GetComponent<Grenade>().Cook();
    }
    public override void UseAbility()
    {
        base.UseAbility();
        grenadeReuse.GetComponent<Grenade>().Throw();
        UseAbilityOne();
    }
}
