using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base script in charge of holding a Unit's stats and current weapons.
/// </summary>
public class Mercenary : MonoBehaviour
{
    protected WeaponAtHand weaponAtHand;
    protected AugmentHolder augmentHolder;
    protected Augment augment1;
    protected Augment augment2;
    protected Augment augment3;
    protected Ability ability1;
    protected Ability ability2;
    protected Ability ability3;
    protected new string name;
    protected byte classType; //0: No class, 1: Ammo Support, 2: Medic, 3: Assault, 4: Engineer, 5: Recon
    public bool isEngy;
    public bool choiceMerc; //If On and Merc has 2 Abilities, Cooldown both when either were used.
    public int healthMax;
    public float maxSpeed;
    public float engineerSpeed;
    public string primaryGunFileName;
    public string secondaryGunFileName;
    public string meleeWeaponFileName;
    public string augment1FileName;
    public string augment2FileName;
    public string augment3FileName;
    public string ability1FileName;
    public string ability2FileName;
    public string ability3FileName;
    
    protected virtual void Awake()
    {
        weaponAtHand = GetComponentInChildren<WeaponAtHand>();
        augmentHolder = GetComponentInChildren<AugmentHolder>();
    }
    protected virtual void Start()
    {
        SetWeapons();
        SetAugments();
        SetAbilities();
    }
    /// <summary>
    /// Check the Engineering speed when activated.
    /// </summary>
    protected void CheckEngineerRate()
    {
        engineerSpeed = isEngy ? 2 : Mathf.Clamp(engineerSpeed, 1f, 1.3f);
    }
    /// <summary>
    /// Grabs the Mercenary's weapons and places it to its appropriate spot.
    /// It is then followed by another method by the same name in a different script.
    /// </summary>
    private void SetWeapons()
    {
        weaponAtHand.SetWeapons(primaryGunFileName, secondaryGunFileName, meleeWeaponFileName);
    }
    /// <summary>
    /// Grabs the Mercenary's Abilities and places it to its appropriate spot.
    /// It is them followed by another method by the same name in a different script.
    /// </summary>
    private void SetAbilities()
    {
        List<string> list = new List<string>(3);
        if (!string.IsNullOrEmpty(ability1FileName))
        {
            list.Add(ability1FileName);
        }
        if (!string.IsNullOrEmpty(ability2FileName))
        {
            list.Add(ability2FileName);
        }
        if (!string.IsNullOrEmpty(ability3FileName))
        {
            list.Add(ability3FileName);
        }
        weaponAtHand.SetAbilities(list.ToArray());
    }
    /// <summary>
    /// Grabs the Mercenary's Augments and places it to its appropriate spot.
    /// It is then followed by another method by the same name in a different script.
    /// </summary>
    private void SetAugments()
    {
        if (!string.IsNullOrEmpty(augment1FileName))
        {
            augmentHolder.augment1 = Instantiate(Resources.Load<GameObject>("Augments/" + augment1FileName),
            augmentHolder.transform.position, augmentHolder.transform.rotation);
            augmentHolder.augment1.gameObject.transform.SetParent(augmentHolder.transform);
        }
        if (!string.IsNullOrEmpty(augment2FileName))
        {
            augmentHolder.augment2 = Instantiate(Resources.Load<GameObject>("Augments/" + augment2FileName),
            augmentHolder.transform.position, augmentHolder.transform.rotation);
            augmentHolder.augment2.gameObject.transform.SetParent(augmentHolder.transform);
        }
        if (!string.IsNullOrEmpty(augment3FileName))
        {
            augmentHolder.augment3 = Instantiate(Resources.Load<GameObject>("Augments/" + augment3FileName),
            augmentHolder.transform.position, augmentHolder.transform.rotation);
            augmentHolder.augment3.gameObject.transform.SetParent(augmentHolder.transform);
        }
        GetComponentInChildren<AugmentHolder>().SetAugments();
    }
    protected void TurnOffAugments()
    {
        Debug.Log("Turning off augments...");
        if (augment1 != null) { augment1.GetComponent<Augment>().TurnOffAugment(); }
        if (augment2 != null) { augment2.GetComponent<Augment>().TurnOffAugment(); }
        if (augment3 != null) { augment3.GetComponent<Augment>().TurnOffAugment(); }
    }
}
