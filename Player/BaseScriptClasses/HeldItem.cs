using UnityEngine;

/// <summary>
/// Script used as a base class for Weaponry, Abilities, and Objectives.
/// </summary>
public class HeldItem : MonoBehaviour
{
    protected Animator animator;
    /// <summary>
    /// Who is the owner of this held item?
    /// </summary>
    protected PlayerController heldItemOwner;
    /// <summary>
    /// Can the Unit use this Held Item? If an ability, this will not track if any Uses remaining.
    /// </summary>
    protected bool canUse;
    /// <summary>
    /// The type of item. 0 = Null, 1 = Primary Weapon, 2 = Secondary, 3 = Melee, 4 = Ability, 5 = Objective.
    /// </summary>
    protected byte heldItemType;
    /// <summary>
    /// The name of this item (e.g AK-47, Ammo Pack, Grenade, etc).
    /// </summary>
    protected string heldItemName;
    /// <summary>
    /// The description of the held item. Weaponry: Explain firing type and gist of gameplay.
    /// </summary>
    protected string heldItemDescription;

    public virtual void Awake()
    {
        //Animator
        animator = GetComponent<Animator>();
    }

    public virtual void Update()
    {
        if (animator != null)
        {
            animator.SetFloat("xVelocity", Input.GetAxis("Horizontal"));
            animator.SetFloat("yVelocity", Input.GetAxis("Vertical"));
            animator.SetBool("Sprinting", heldItemOwner.Sprinting());

        }
    }

    /// <summary>
    /// Set the owner of this held item.
    /// </summary>
    /// <param name="playerController"></param>
    public void SetOwnerOfHeldItem(PlayerController playerController)
    {
        heldItemOwner = playerController;
    }
    public void CannotUseHeldItem()
    {
        canUse = false;
    }
    public void CanUseHeldItem()
    {
        canUse = false;
    }
    /// <summary>
    /// Can the Unit use this Held Item? If an ability, this will not track if any Uses remaining.
    /// </summary>
    /// <returns></returns>
    public bool CanUse()
    {
        return canUse;
    }
    /// <summary>
    /// Is this Item a Weapon?
    /// </summary>
    /// <returns></returns>
    public bool IsWeapon()
    {
        return heldItemType < 3;
    }
    /// <summary>
    /// Is this Item a Gun? It can be a Primary or a Secondary.
    /// </summary>
    /// <returns></returns>
    public bool IsGun()
    {
        return heldItemType < 2;
    }
    /// <summary>
    /// Is this Item an Ability?
    /// </summary>
    /// <returns></returns>
    public bool IsAbility()
    {
        return heldItemType == 4;
    }
    /// <summary>
    /// Is this Item an Objective?
    /// </summary>
    /// <returns></returns>
    public bool IsObjective()
    {
        return heldItemType == 5;
    }
    /// <summary>
    /// Is this Item a Primary Gun?
    /// </summary>
    /// <returns></returns>
    public bool IsPrimaryWeapon()
    {
        return heldItemType == 1;
    }
    /// <summary>
    /// Is this Item a Melee Weapon?
    /// </summary>
    /// <returns></returns>
    public bool IsMeleeWeapon()
    {
        return heldItemType == 3;
    }
    /// <summary>
    /// The type of item. 0 = Null, 1 = Primary Weapon, 2 = Secondary, 3 = Melee, 4 = Ability, 5 = Objective.
    /// </summary>
    /// <returns></returns>
    public byte ItemType()
    {
        return heldItemType;
    }
    /// <summary>
    /// The given name to this Item.
    /// </summary>
    /// <returns></returns>
    public string ItemName()
    {
        return heldItemName;
    }
    /// <summary>
    /// The given description to this Item.
    /// </summary>
    /// <returns></returns>
    public string ItemDescription()
    {
        return heldItemDescription;
    }
    /// <summary>
    /// Returns the Owner of this Held Item.
    /// </summary>
    /// <returns></returns>
    public PlayerController OwnerOfHeldItem()
    {
        return heldItemOwner;
    }
}
