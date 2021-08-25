using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script to keep track of all User Input.
/// </summary>
public class PlayerInput : MonoBehaviour
{
    private PlayerWeaponControls playerWeaponControls;
    private PlayerController playerController;

    //TODO: Place all User Input here.
    private void Awake()
    {
        playerWeaponControls = GetComponent<PlayerWeaponControls>();
        playerController = GetComponent<PlayerController>();
    }
    private void Start()
    {
        
    }
    private void Update()
    {
        //Clicking for Shooting
        //Clicking for ADS
        //Pressing F for Interact
        //Also include auto interact?
        //Pressing Reload
        //Switching to abilities
        if (playerWeaponControls.HoldingGun() && (!playerController.Sprinting()))
        {
            //Firing
            switch(playerWeaponControls.FiringMode())
            {
                case 1: //Semi-Auto
                    if (Input.GetButtonDown("Fire1"))
                    {
                        playerWeaponControls.FireGun();
                    }
                    break;
                case 2: //Auto Fire
                    if (Input.GetButton("Fire1"))
                    {
                        //if (playerWeaponControls.CanFire())
                        //{
                        //    playerWeaponControls.FireGun();
                        //}
                        playerWeaponControls.FireGun();
                    }
                    break;
                case 3: //Burst Fire
                    if (Input.GetButtonDown("Fire1") /*&& currentGun.CanFire() && currentGun.GetCanShootBurst()*/)
                    {
                        playerWeaponControls.FireGun();
                    }
                    break;
                default:
                    break;
            }
            //Reload
            if (Input.GetKeyDown(KeyCode.R))
            {
                playerWeaponControls.Reload();
            }
            //ADS
            if (Input.GetMouseButton(1))
            {
                playerWeaponControls.AimDownSight(true);
            }
            //Letting go of ADS
            if (Input.GetMouseButtonUp(1))
            {
                playerWeaponControls.AimDownSight(false);
            }
        }
        //Melee
        if (playerWeaponControls.HoldingMelee())
        {
            if (Input.GetMouseButtonDown(0))
            {
                playerWeaponControls.FireMelee(true);
            }
            if (Input.GetMouseButtonDown(1))
            {
                playerWeaponControls.FireMelee(false);
            }
        }
        //Ability Use
        if (playerWeaponControls.HoldingAbility())
        {
            if (playerWeaponControls.AbilityRequiresCook())
            {
                if (Input.GetMouseButton(0))
                {
                    playerWeaponControls.UseAbilityPrep(true);
                }
                if (Input.GetMouseButtonUp(0))
                {
                    playerWeaponControls.UseAbilityPrep(false);
                }
            }
            else
            {
                if (Input.GetMouseButtonDown(0))
                {
                    playerWeaponControls.UseAbilityClick();
                }
            }
        }
        //Ability Switching
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            playerWeaponControls.SwitchAbility(0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            playerWeaponControls.SwitchAbility(1);
        }
        //Weapon switching, via mouse scroll
        if (Input.GetAxis("Mouse ScrollWheel") > 0f) //WEAPON SCROLL UP
        {
            playerWeaponControls.SelectWeaponIncrement();
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0f) //WEAPON SCROLL DOWN
        {
            playerWeaponControls.SelectWeaponDecrement();
        }
        //Crouching
        if (Input.GetKey(KeyCode.C))
        {
            playerController.SetCrouch(true);
        }
        if (Input.GetKeyUp(KeyCode.C))
        {
            playerController.SetCrouch(false);
        }
    }
    private void FixedUpdate()
    {
        


    }
}
