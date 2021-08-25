using UnityEngine;

/// <summary>
/// Script in charge of what the Player can do with their guns, no so much input that will shoot the gun off.
/// </summary>
public class PlayerWeaponControls : MonoBehaviour
{
    //private PlayerController playerController; //CURRENTLY NOT IN USE
    private PlayerInteract playerInteract;
    private WeaponAtHand weaponAtHand;
    private HeldItem currentHeldItem;

    private bool holdingObjective;
    private bool autoReload;
    private int selectedWeapon;

    public ObjectPooler bulletPooler;
    public GameObject bullet;

    //SET VIA WEAPONATHAND
    //TODO: Once Abilities work, clean up code
    //TODO: Cook grenade, then throw it
    //TODO: Later, ammo pack to throw and give
    private void Awake()
    {
        playerInteract = GetComponent<PlayerInteract>();
        weaponAtHand = GetComponentInChildren<WeaponAtHand>();
    }
    private void Start()
    {
        autoReload = true;
        bulletPooler.SetObjectPooler(30, bullet);
	}
    
	private void Update()
    {
        //These two could be called elsewhere
        int previousWeapon = selectedWeapon;
        
        //----------//
        // CONTROLS //
        //----------//
        if (previousWeapon != selectedWeapon)
        {
            weaponAtHand.SelectHeldItem(selectedWeapon);
        }
    }
    public void SelectWeaponIncrement()
    {
        if (!playerInteract.HoldingObjective())
        {
            selectedWeapon++;
            if (selectedWeapon > 2)
            {
                selectedWeapon = 0;
            }
            weaponAtHand.SelectHeldItem(selectedWeapon);
            Debug.Log("Selected Weapon Number: " + selectedWeapon);
        }
        else
        {
            DropObjective();
        }
    }
    public void SelectWeaponDecrement()
    {
        if (!playerInteract.HoldingObjective())
        {
            selectedWeapon--;
            if (selectedWeapon < 0)
            {
                selectedWeapon = 2;
            }
            weaponAtHand.SelectHeldItem(selectedWeapon);
            Debug.Log("Selected Weapon Number: " + selectedWeapon);
        }
        else
        {
            DropObjective();
        }
    }
    public void SelectWeapon(int index)
    {
        if (!playerInteract.HoldingObjective())
        {
            selectedWeapon = index;
            Debug.Log("Selected Weapon Number: " + selectedWeapon);
        }
        else
        {
            DropObjective();
        }
    }
    /// <summary>
    /// If Unit is holding an objective, drop it.
    /// </summary>
    private void DropObjective()
    {
        selectedWeapon = 0;
        if (currentHeldItem.GetComponent<Ability>() != null)
        {
            currentHeldItem.GetComponent<Ability>().PutAwayAbility();
        }
        playerInteract.SetHoldingObjective(false);
        weaponAtHand.DropObjectiveCanister();
        weaponAtHand.SelectHeldItem(selectedWeapon);
    }
    /// <summary>
    /// Switch to a desired ability.
    /// </summary>
    /// <param name="index"></param>
    public void SwitchAbility(byte index)
    {
        if (index >= weaponAtHand.abilities.Length)
        {
            return;
        }
        if (weaponAtHand.abilities[index].GetComponent<Ability>().UsesRemaining())
        {
            Debug.Log("Switching to Ability " + (index + 1) + ", " + weaponAtHand.abilities[index].GetComponent<Ability>().ItemName());
            if (currentHeldItem.GetComponent<Ability>() != null)
            {
                currentHeldItem.GetComponent<Ability>().PutAwayAbility();
            }
            currentHeldItem = weaponAtHand.abilities[index].GetComponent<Ability>();
            weaponAtHand.abilities[index].GetComponent<Ability>().SwitchToAbility();
            selectedWeapon = 3 + index; //Selected Weapons 0-2 are Main guns. 3-5 are abilities
            weaponAtHand.SelectHeldItem(selectedWeapon);
        }
        else
        {
            Debug.Log("NEED TO WAIT UNTIL COOLDOWN IS READY!!");
        }
    }
    /// <summary>
    /// Use a Mercenary's ability that does not require a cooking time.
    /// If Ability runs dry after use, revert to primary weapon. Does not do anything if Unit does not have any uses remaining.
    /// </summary>
    public void UseAbilityClick()
    {
        //if (currentAbility == null)
        if (currentHeldItem.GetComponent<Ability>() == null)
        {
            return;
        }
        Ability currentAbility = currentHeldItem.GetComponent<Ability>();
        if (currentAbility.UsesRemaining())
        {
            currentAbility.UseAbility();
            if (!currentAbility.UsesRemaining())
            {
                selectedWeapon = 0;
                currentAbility = null;
            }
        }
    }
    /// <summary>
    /// Use a Mercenary's ability that requires a cooking time.
    /// If Ability runs dry after use, revert to primary weapon. Does not do anything if Unit does not have any uses remaining.
    /// </summary>
    public void UseAbilityPrep(bool holding)
    {
        //if (currentAbility == null)
        if (currentHeldItem.GetComponent<Ability>() == null)
        {
            return;
        }
        Ability currentAbility = currentHeldItem.GetComponent<Ability>();
        if (currentAbility.UsesRemaining())
        {
            if (holding)
            {
                currentAbility.PrepareAbility();
            }
            else
            {
                currentAbility.UseAbility();
                if (!currentAbility.UsesRemaining())
                {
                    selectedWeapon = 0;
                    currentAbility = null;
                }
            }
        }
    }
    /// <summary>
    /// If holding a gun, aim down its sight.
    /// </summary>
    /// <param name="tOrF"></param>
    public void AimDownSight(bool tOrF)
    {
        if (currentHeldItem.GetComponent<Gun>() != null)
        {
            Gun currentGun = currentHeldItem.GetComponent<Gun>();
            if (tOrF)
            {
                currentGun.transform.localPosition = Vector3.Slerp(currentGun.transform.localPosition,
                    currentGun.aimingDownPosition, currentGun.GetAimSpeed() * Time.deltaTime);
            }
            else
            {
                currentGun.transform.localPosition = currentGun.hipFirePosition;
            }
            currentGun.AimingDownSight(tOrF);
        }
    }
    /// <summary>
    /// Fire the held gun. If no gun, cancel the method.
    /// </summary>
    public void FireGun()
    {
        //If no gun, cancel method
        if (currentHeldItem.GetComponent<Gun>() == null)
        {
            return;
        }
        Gun currentGun = currentHeldItem.GetComponent<Gun>();
        if (currentGun.CanFire() && currentGun.CanUse())
        {
            //If burst fire, check if not in the middle of a burst shot
            if (!currentGun.GetCanShootBurst())
            {
                return;
            }
            //If Prep Action, check if not in the middle of a prep action
            //Bow And Arrow: seperate method?
            currentGun.Fire();
        }
        //If the option is on, automatically reload if ammo runs dry.
        //if (autoReload && (!currentGun.HaveAmmoRemaining()))
        //{
        //    Reload();
        //}
    }
    /// <summary>
    /// Strike with melee weapon. Does not do anything if the Unit is not holding anything else.
    /// </summary>
    public void FireMelee(bool weakAttack)
    {
        if (currentHeldItem.GetComponent<Melee>() != null)
        {
            if (weakAttack)
            {
                Debug.Log("Light melee attack");
            }
            else
            {
                Debug.Log("STRONG melee attack");
            }
        }
    }

    /// <summary>
    /// Switch the current weapon based on index.
    /// </summary>
    /// <param name="number"></param>
    public void SetCurrentWeapon(int number)
    {
        switch(number)
        {
            case 0:
                currentHeldItem = weaponAtHand.RetrieveWeapon(0).GetComponent<Gun>();
                break;
            case 1:
                currentHeldItem = weaponAtHand.RetrieveWeapon(1).GetComponent<Gun>();
                break;
            case 2:
                currentHeldItem = weaponAtHand.RetrieveWeapon(2).GetComponent<Melee>();
                break;
            default:
                break;
        }
    }
    /// <summary>
    /// Switch the current weapon to an arbitray weapon.
    /// </summary>
    /// <param name="weapon"></param>
    public void SetCurrentWeapon(Weapon weapon)
    {
        playerInteract.SetHoldingObjective(true);
        currentHeldItem = weapon;
    }
    public void SwitchFromAbility()
    {
        currentHeldItem = weaponAtHand.RetrieveWeapon(0).GetComponent<Gun>();
    }
    /// <summary>
    /// Reload the current gun. Does not do anything if ability or melee weapon.
    /// </summary>
    public void Reload()
    {
        if (currentHeldItem.GetComponent<Gun>() == null)
        {
            return;
        }
        Gun currentGun = currentHeldItem.GetComponent<Gun>();
        if (currentGun.CanReload() && currentGun.CanUse())
        {
            currentGun.Reload();
        }
    }
    public void StopReloading()
    {
        if (currentHeldItem.GetComponent<Gun>() != null)
        {
            currentHeldItem.GetComponent<Gun>().WeaponReady();
        }
    }
    public void CompleteObjectivePlacement()
    {
        weaponAtHand.CompleteObjectivePlacement();
    }
    public bool AllAmmoMaxed()
    {
        return (weaponAtHand.RetrieveWeapon(0).GetComponent<Gun>().ReservedMaxedOut() &&
            weaponAtHand.RetrieveWeapon(1).GetComponent<Gun>().ReservedMaxedOut());
    }
    /// <summary>
    /// Is the Unit currently holding a gun?
    /// </summary>
    /// <returns></returns>
    public bool HoldingGun()
    {
        return currentHeldItem.GetComponent<Gun>() != null;
    }
    /// <summary>
    /// Is the Unit currently holding a melee weapon?
    /// </summary>
    /// <returns></returns>
    public bool HoldingMelee()
    {
        return currentHeldItem.GetComponent<Melee>() != null;
    }
    /// <summary>
    /// Is the Unit currently holding out their ability?
    /// </summary>
    /// <returns></returns>
    public bool HoldingAbility()
    {
        return currentHeldItem.GetComponent<Ability>() != null;
    }
    /// <summary>
    /// Can the current gun be fired if there's any ammo remaining?
    /// </summary>
    /// <returns></returns>
    public bool CanFire()
    {
        if (currentHeldItem.GetComponent<Gun>() != null)
        {
            return currentHeldItem.GetComponent<Gun>().HaveAmmoRemaining();
        }
        return false;
    }
    public bool CurrentlyReloading()
    {
        if (currentHeldItem.GetComponent<Gun>() != null)
        {
            return currentHeldItem.GetComponent<Gun>().CanUse();
        }
        return false;
    }
    /// <summary>
    /// Does the current ability require a charging time?
    /// </summary>
    /// <returns></returns>
    public bool AbilityRequiresCook()
    {
        if (currentHeldItem.GetComponent<Ability>() != null)
        {
            return currentHeldItem.GetComponent<Ability>().RequiresCook();
        }
        return false;
    }
    /// <summary>
    /// Return the current weapon's Firing Mode. Returns zero if ability or melee weapon.
    /// </summary>
    /// <returns></returns>
    public int FiringMode()
    {
        if (currentHeldItem.GetComponent<Gun>() == null)
        {
            return -1;
        }
        return currentHeldItem.GetComponent<Gun>().FiringMode();
    }
    public Gun GetPrimaryGun()
    {
        return weaponAtHand.RetrieveWeapon(0).GetComponent<Gun>();
    }
    public Gun GetSecondaryGun()
    {
        return weaponAtHand.RetrieveWeapon(1).GetComponent<Gun>();
    }
    public Gun GetGun()
    {
        return currentHeldItem.GetComponent<Gun>();
    }
    public Melee GetMeleeWeapon()
    {
        return weaponAtHand.RetrieveWeapon(2).GetComponent<Melee>();
    }
    public Weapon GetCurrentWeapon()
    {
        if (currentHeldItem.GetComponent<Gun>() != null)
        {
            return currentHeldItem.GetComponent<Gun>();
        }
        return null;
    }
    public ObjectiveCanisterPickup GetPickup()
    {
        return weaponAtHand.GetPickup();
    }
}
