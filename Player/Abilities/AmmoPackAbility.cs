using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script that enables the Ammo Pack ability: throwing an ammo pack 
/// </summary>
public class AmmoPackAbility : Ability
{
    //private List<GameObject> ammoPacks; //Use later for Object Pooling
    private GameObject ammoPackAtHand;
    private Vector3 smallerSize;

    public GameObject ammoPack;

    public override void Awake()
    {
        heldItemName = "AMMO PACK";
        heldItemDescription = "Throw an Ammo Pack that will max out the Ammo you can carry.";
        maxUses = 4;
        cooldown = 5f;
        requireCook = false;
        smallerSize = new Vector3(0.5f, 0.4f, 0.6f);
        ammoPackAtHand = Instantiate(ammoPack, transform.position, transform.rotation);
        ammoPackAtHand.GetComponent<InstantInteract>().MakeTriggerBoxSmall(smallerSize);
        ammoPackAtHand.transform.SetParent(transform);
        ammoPackAtHand.transform.localPosition = ammoPackAtHand.GetComponent<Pickup>().hipFirePosition;
        ammoPackAtHand.GetComponent<Rigidbody>().isKinematic = true;
        ammoPackAtHand.SetActive(false);
        //ammoPacks = new List<GameObject>();
        base.Awake();
    }
    public override void SwitchToAbility()
    {
        base.SwitchToAbility();
        ammoPackAtHand.transform.SetParent(transform);
        ammoPackAtHand.transform.localPosition = ammoPackAtHand.GetComponent<Pickup>().hipFirePosition;
        ammoPackAtHand.GetComponent<InstantInteract>().MakeTriggerBoxSmall(smallerSize);
        ammoPackAtHand.GetComponent<Rigidbody>().isKinematic = true;
        ammoPackAtHand.SetActive(true);
    }
    public override void PutAwayAbility()
    {
        base.PutAwayAbility();
        ammoPackAtHand.SetActive(false);
    }
    public override void UseAbility()
    {
        base.UseAbility();
        ammoPackAtHand.GetComponent<Rigidbody>().isKinematic = false;
        ammoPackAtHand.transform.SetParent(null);
        ammoPackAtHand.GetComponent<Rigidbody>().AddForce((transform.forward * 32), ForceMode.VelocityChange);
        ammoPackAtHand.GetComponent<InstantInteract>().RevertBackLayer();
        ammoPackAtHand = null;
        ammoPackAtHand = Instantiate(ammoPack, transform.position, transform.rotation);
        ammoPackAtHand.GetComponent<InstantInteract>().MakeTriggerBoxSmall(smallerSize);
        ammoPackAtHand.transform.SetParent(transform);
        ammoPackAtHand.transform.localPosition = ammoPackAtHand.GetComponent<Pickup>().hipFirePosition;
        ammoPackAtHand.GetComponent<Rigidbody>().isKinematic = true;
        ammoPackAtHand.SetActive(true);
        UseAbilityOne();
    }
}
