using System.Collections.Generic;
using UnityEngine;

public class MedkitAbility : Ability
{
    private GameObject medkitAtHand;
    private Vector3 smallerSize;

    public GameObject medkit;

    public override void Awake()
    {
        heldItemName = "MEDKIT";
        heldItemDescription = "Throw a small Medical Pack that will restore a small amount of Health. " +
            "Throwing to a Player directly heals a bit more.";
        maxUses = 3;
        cooldown = 7f;
        requireCook = false;
        smallerSize = new Vector3(0.5f, 0.4f, 0.6f);
        medkitAtHand = Instantiate(medkit, transform.position, transform.rotation);
        medkitAtHand.GetComponent<InstantInteract>().MakeTriggerBoxSmall(smallerSize);
        medkitAtHand.transform.SetParent(transform);
        medkitAtHand.transform.localPosition = medkitAtHand.GetComponent<Pickup>().hipFirePosition;
        medkitAtHand.GetComponent<Rigidbody>().isKinematic = true;
        medkitAtHand.SetActive(false);
        //ammoPacks = new List<GameObject>();
        base.Awake();
    }

    public override void SwitchToAbility()
    {
        base.SwitchToAbility();
        medkitAtHand.transform.SetParent(transform);
        medkitAtHand.transform.localPosition = medkitAtHand.GetComponent<Pickup>().hipFirePosition;
        medkitAtHand.GetComponent<InstantInteract>().MakeTriggerBoxSmall(smallerSize);
        medkitAtHand.GetComponent<Rigidbody>().isKinematic = true;
        medkitAtHand.SetActive(true);
    }
    public override void PutAwayAbility()
    {
        base.PutAwayAbility();
        medkitAtHand.SetActive(false);
    }
    public override void UseAbility()
    {
        base.UseAbility();
        medkitAtHand.GetComponent<Rigidbody>().isKinematic = false;
        medkitAtHand.transform.SetParent(null);
        medkitAtHand.GetComponent<Rigidbody>().AddForce((transform.forward * 32), ForceMode.VelocityChange);
        medkitAtHand.GetComponent<InstantInteract>().RevertBackLayer();
        medkitAtHand = null;
        medkitAtHand = Instantiate(medkit, transform.position, transform.rotation);
        medkitAtHand.GetComponent<InstantInteract>().MakeTriggerBoxSmall(smallerSize);
        medkitAtHand.transform.SetParent(transform);
        medkitAtHand.transform.localPosition = medkitAtHand.GetComponent<Pickup>().hipFirePosition;
        medkitAtHand.GetComponent<Rigidbody>().isKinematic = true;
        medkitAtHand.SetActive(true);
        UseAbilityOne();
    }
}
