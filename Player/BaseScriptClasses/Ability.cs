using UnityEngine;

/// <summary>
/// A Merc's unique ability that cannot be changed between Mercs.
/// </summary>
public class Ability : HeldItem
{
    protected AbilityText abilityText;
    /// <summary>
    /// Does this ability need to be cooked? If not, PressButtonDown() is only needed; otherwise, PressButton() and PressButtonUp() needed.
    /// </summary>
    protected bool requireCook;
    /// <summary>
    /// If true, the cooldown of this ability should be applied to the other ability of the Merc.
    /// </summary>
    protected bool choiceAbility;
    /// <summary>
    /// May not need any more
    /// </summary>
    protected bool purposefulCooldown;
    /// <summary>
    /// The maximum times you can use an Ability at any given moment.
    /// </summary>
    protected int maxUses;
    /// <summary>
    /// The current spam uses of this Ability.
    /// </summary>
    protected int currentUses;
    /// <summary>
    /// The cooldown to use for this item.
    /// </summary>
    protected float cooldown;
    /// <summary>
    /// Current cooldown.
    /// </summary>
    protected float currentCooldown;

    public virtual void Awake()
    {
        currentUses = maxUses;
    }
    public virtual void SetAbility() { }
    public virtual void SetAbility(WeaponAtHand weaponAtHand) { }
    /// <summary>
    /// Method to use when switching to an Ability.
    /// </summary>
    public virtual void SwitchToAbility()
    {
        Debug.Log("Switching to ability...");
    }
    /// <summary>
    /// Method to use when switching from an Ability.
    /// </summary>
    public virtual void PutAwayAbility()
    {
        Debug.Log("Putting away ability...");
    }
    /// <summary>
    /// Method to use when using an Ability. For Throwables, this is the action to use for Throwing.
    /// </summary>
    public virtual void UseAbility()
    {
        Debug.Log("Using ability...");
    }
    /// <summary>
    /// Method to use when preparing an Ability. For Throwables, this is the action to use when Cooking.
    /// </summary>
    public virtual void PrepareAbility()
    {
        Debug.Log("Preparing ability...");
    }
    /// <summary>
    /// Set cooldown without using the Ability.
    /// </summary>
    public void SetCooldown()
    {
        purposefulCooldown = true;
        currentCooldown = cooldown;
    }
    /// <summary>
    /// Set cooldown by a given value. Should be used for Choice Mercs.
    /// </summary>
    /// <param name="value"></param>
    public void SetCooldown(float value)
    {
        currentCooldown = value;
    }
    /// <summary>
    /// Set Ability Text so that it's the individual Ability's responsibility to update the HUD on Abilities.
    /// </summary>
    /// <param name="abilityText"></param>
    public void SetAbilityText(AbilityText abilityText)
    {
        this.abilityText = abilityText;
    }
    /// <summary>
    /// Use up the Ability and give it a cooldown to replace it.
    /// Returns a value if the Ability is a Choice Ability and should be applied to the other Ability.
    /// </summary>
    public float UseAbilityOne()
    {
        if (currentUses == maxUses)
        {
            currentCooldown = cooldown;
        }
        abilityText.SetCooldownCircle(currentCooldown);
        currentUses--;
        if (UsesRemaining()) 
        {
            abilityText.usesLeft.text = currentUses + " / " + maxUses;
        }
        else
        {
            abilityText.usesLeft.text = "ON COOLDOWN";
        }
        return choiceAbility ? currentCooldown : 0;
    }
    /// <summary>
    /// Set up Ability on the HUD.
    /// </summary>
    public void SetUpAbilityOnHUD()
    {
        if (abilityText != null)
        {
            abilityText.abilityText.text = heldItemName;
            abilityText.usesLeft.text = currentUses + " / " + maxUses;
            abilityText.SetAbilityCooldown(cooldown);
            abilityText.cooldownCircle.fillAmount = 0;
        }
    }
    /// <summary>
    /// Can the Player still spam this Ability.
    /// </summary>
    /// <returns></returns>
    public bool UsesRemaining()
    {
        return (currentUses > 0);
    }
    /// <summary>
    /// Does the Ability require a "Cook" for full use of the Ability?
    /// </summary>
    /// <returns></returns>
    public bool RequiresCook()
    {
        return requireCook;
    }
    /// <summary>
    /// To be placed in the Update method.
    /// </summary>
    public void CooldownCheck()
    {
        if (OnCooldown())
        {
            if (!HaveMaxUses())
            {
                currentCooldown -= Time.deltaTime;
                abilityText.SetCooldownCircle(currentCooldown);
            }
        }
        else
        {
            if (purposefulCooldown)
            {
                purposefulCooldown = false;
                if (!HaveMaxUses())
                {
                    currentCooldown = cooldown;
                }
            }
            else
            {
                if (!HaveMaxUses())
                {
                    AddOne();
                }
            }
        }
    }
    protected bool HaveMaxUses()
    {
        return currentUses >= maxUses;
    }
    protected bool CanUse()
    {
        if (UsesRemaining())
        {
            return true; 
        }
        else
        {
            return (CooldownReady());
        }
    }
    private void AddOne()
    {
        if (CooldownReady())
        {
            currentUses++;
            currentCooldown = cooldown;
            abilityText.usesLeft.text = currentUses + " / " + maxUses;
        }
    }
    private bool CooldownReady()
    {
        return currentCooldown <= 0;
    }
    private bool OnCooldown()
    { 
        return currentCooldown > 0;
    }
}
