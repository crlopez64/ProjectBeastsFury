using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script in charge of keeping track of the Fighter's stats.
/// </summary>
public class UnitStats : MonoBehaviour
{
    private UnitStats opponent;
    private UnitAttack unitAttack;
    private HUDHealthBar hudHealthBar;
    private HUDComboCounter hudComboCounter;
    private HUDBattleNotifications hudBattleNotifications;
    /// <summary>
    /// The maximum health of this Unit.
    /// </summary>
    private int maxHealth;
    /// <summary>
    /// The current total hits in a combo.
    /// </summary>
    private int comboHits;
    /// <summary>
    /// The total damage in a combo.
    /// </summary>
    private int comboDamage;
    /// <summary>
    /// The unit's current health.
    /// </summary>
    private int currentHealth;
    /// <summary>
    /// The highest damage done in a combo in a match.
    /// </summary>
    private int highestComboDamage;
    /// <summary>
    /// The damage scaling with respect to the health bar.
    /// </summary>
    private float healthMultiplier;

    public void Awake()
    {
        unitAttack = GetComponent<UnitAttack>();
    }
    
    /// <summary>
    /// Damage the Unit.
    /// </summary>
    /// <param name="damage"></param>
    public void DamageUnit(int damage, bool blocking)
    {
        if (!blocking)
        {
            if (unitAttack.CurrentlyHit() && unitAttack.Hitstunned())
            {
                comboDamage += damage;
                if (comboDamage > highestComboDamage)
                {
                    highestComboDamage = comboDamage;
                }
            }
            else
            {
                comboDamage = damage;
            }
        }
        currentHealth -= (int)(damage * healthMultiplier);
        if (currentHealth < 0)
        {
            currentHealth = 0;
        }
        SetHealthMultiplier();
        hudHealthBar.SetValue(currentHealth);
    }
    /// <summary>
    /// Damage the Unit if they ledge drop. Does not scale.
    /// </summary>
    public void DamageUnitLedgeDrop()
    {
        if (unitAttack.CurrentlyHit() && unitAttack.Hitstunned())
        {
            comboDamage += (int)(maxHealth * 0.16f);
            if (comboDamage > highestComboDamage)
            {
                highestComboDamage = comboDamage;
            }
        }
        else
        {
            comboDamage = (int)(maxHealth * 0.16f);
        }
        currentHealth -= (int)(maxHealth * 0.16f);
        if (currentHealth < 0)
        {
            currentHealth = 0;
        }
        hudHealthBar.SetValue(currentHealth);
    }
    /// <summary>
    /// Record a hit to the visual combo marker.
    /// </summary>
    public void AddToCombo()
    {
        comboHits++;
        //hudComboCounter.AddHit();
        hudComboCounter.SetCounter(comboHits);
    }
    /// <summary>
    /// Reset the visual combo marker.
    /// </summary>
    public void ResetCombo()
    {
        comboHits = 0;
        hudComboCounter.ResetCounter();
    }
    /// <summary>
    /// Refill health to max health.
    /// </summary>
    public void RefillHealth()
    {
        currentHealth = maxHealth;
    }
    /// <summary>
    /// Set the maximum health for the Unit. This will also fixate which Health bar to use for this unit.
    /// </summary>
    /// <param name="maxHealth"></param>
    public void SetHealth(int maxHealth, byte maxRounds, HUDHealthBar hudHealthBar)
    {
        if (this.hudHealthBar != null)
        {
            return;
        }
        this.hudHealthBar = hudHealthBar;
        this.maxHealth = maxHealth;
        currentHealth = maxHealth;
        this.hudHealthBar.SetMaxHealth(maxHealth, maxRounds);
        this.hudHealthBar.SetValue(maxHealth);
        SetHealthMultiplier();
    }
    /// <summary>
    /// Set the proper notifications for this Unit.
    /// </summary>
    /// <param name="notifications"></param>
    public void SetNotifications(HUDBattleNotifications notifications)
    {
        hudBattleNotifications = notifications;
    }
    /// <summary>
    /// Set the proper counter for this Unit.
    /// </summary>
    /// <param name="counter"></param>
    public void SetComboCounter(HUDComboCounter counter)
    {
        hudComboCounter = counter;
    }
    /// <summary>
    /// Set the opponent reference to this Unit.
    /// </summary>
    /// <param name="opponent"></param>
    public void SetOpponent(UnitStats opponent)
    {
        this.opponent = opponent;
    }
    /// <summary>
    /// Add a notification about a counter hit.
    /// </summary>
    public void NotifyCounter()
    {
        hudBattleNotifications.AddNotifaction("COUNTER HIT");
    }
    /// <summary>
    /// Add a notification about a punish hit.
    /// </summary>
    public void NotifyPunish()
    {
        hudBattleNotifications.AddNotifaction("PUNISH HIT");
    }
    /// <summary>
    /// Add a notification that the Unit is blocking low while standing.
    /// </summary>
    public void NotifyBlockStandingLow()
    {
        hudBattleNotifications.AddNotifcationPriority("BLOCK STANDING LOW");
    }
    /// <summary>
    /// Add a notification that the Unit has hit a wall.
    /// </summary>
    public void NotifyWallStagger()
    {
        hudBattleNotifications.AddNotifaction("WALL STAGGER");
    }
    /// <summary>
    /// Reset the opponent's combo counter.
    /// </summary>
    public void ResetOpponentComboCounter()
    {
        Debug.Log("Resetting combo");
        opponent.ResetCombo();
    }
    /// <summary>
    /// Is this the first hit in the combo?
    /// </summary>
    /// <returns></returns>
    public bool FirstHitInCombo()
    {
        return comboHits == 0;
    }
    /// <summary>
    /// Is this Unit's health drained to zero?
    /// </summary>
    /// <returns></returns>
    public bool EmptyHealth()
    {
        return currentHealth <= 0;
    }
    /// <summary>
    /// This Unit's current Health.
    /// </summary>
    /// <returns></returns>
    public int CurrentHealth()
    {
        return currentHealth;
    }
    /// <summary>
    /// This Unit's max health.
    /// </summary>
    /// <returns></returns>
    public int MaxHealth()
    {
        return maxHealth;
    }
    /// <summary>
    /// Return the number of successive hits done to the Opponent.
    /// </summary>
    /// <returns></returns>
    public int ComboHits()
    {
        return comboHits;
    }
    /// <summary>
    /// Current damage within a combo.
    /// </summary>
    /// <returns></returns>
    public int CurrentComboDamage()
    {
        return comboDamage;
    }
    /// <summary>
    /// The highest damage recorded within the match.
    /// </summary>
    /// <returns></returns>
    public int HighestComboDamage()
    {
        return highestComboDamage;
    }

    /// <summary>
    /// Set up the Health multiplier.
    /// </summary>
    private void SetHealthMultiplier()
    {
        healthMultiplier = Mathf.Lerp(0.7f, 1.0f, (float)currentHealth / maxHealth);
    }
}
