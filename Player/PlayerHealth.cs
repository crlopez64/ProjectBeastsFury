using UnityEngine;

/// <summary>
/// Script handling the player's health.
/// </summary>
public class PlayerHealth : MonoBehaviour
{
    private PlayerCanvas playerCanvas;
    private PlayerInteract playerInteract;
    private Mercenary mercenary;
    private bool dead;
    private bool downed;
    private bool toughOn;
    private bool mountainOn;
    private bool potentJuiceOn;
    private bool isHealthRegen; //For Large Health Packs and Health Station
    private bool extraPaddingOn;
    private int maxHealth;
    private int currentHealth;
    private int normalRegenTick = 1;
    private int potentJuiceTick = 2;
    private int regenTickUse;
    private float normalRegenWait = 7f;
    private float toughRegenWait = 1f;
    private float regenWaitUse;
    private float currentRegenWait;
    private float waitOneSecond;

    //TODO: Downed status and dead status
    private void Awake()
    {
        playerCanvas = FindObjectOfType<PlayerCanvas>();
        playerInteract = GetComponent<PlayerInteract>();
        mercenary = GetComponent<Mercenary>();
    }
    private void Start()
    {
        maxHealth = mercenary.healthMax;
        currentHealth = maxHealth;
        regenWaitUse = normalRegenWait;
        regenTickUse = normalRegenTick;
        playerCanvas.healthBarSlider.SetHealthSlider(this);
	}
	private void Update ()
    {
        regenWaitUse = (toughOn ? toughRegenWait : normalRegenWait);
        regenTickUse = (potentJuiceOn ? potentJuiceTick : normalRegenTick);
		if (currentHealth < maxHealth)
        {
            if (currentRegenWait <= regenWaitUse)
            {
                currentRegenWait += Time.deltaTime;
            }
            else
            {
                waitOneSecond += Time.deltaTime;
                if (waitOneSecond >= 1f)
                {
                    waitOneSecond = 0f;
                    currentHealth += regenTickUse;
                    if(currentHealth >= maxHealth) { currentHealth = maxHealth; }
                }
            }
        }
        else
        {
            currentRegenWait = 0f;
            waitOneSecond = 0f;
        }
        if (currentHealth <= 0)
        {
            downed = true;
            GetComponent<PlayerController>().SetCanMove(false);
        }
        else
        {
            GetComponent<PlayerController>().SetCanMove(true);
        }
	}

    public void SetMountain(bool tOrF)     { mountainOn = tOrF;     }
    public void SetExtraPadding(bool tOrF) { extraPaddingOn = tOrF; }
    public void SetTough(bool tOrF)        { toughOn = tOrF;        }
    public void SetPotentJuice(bool tOrF)  { potentJuiceOn = tOrF;  }
    public void AddHealth(int amount)
    {
        currentHealth += amount;
        if (currentHealth >= maxHealth) { currentHealth = maxHealth; }
    }
    public void HealthRegen()
    {
        currentHealth += regenTickUse;
        if (currentHealth >= maxHealth) { currentHealth = maxHealth; }
    }
    /// <summary>
    /// Player to get damaged by bullet. Shooter's Team key: 1) Attackers, 2) Defenders
    /// </summary>
    /// <param name="incomingDamage"></param>
    /// <param name="hadPowerShot"></param>
    /// <param name="isFriendlyFireOn"></param>
    /// <param name="shootersTeam"></param>
    public void DamagePlayerBullet(float incomingDamage, bool hadPowerShot, bool isFriendlyFireOn, byte shootersTeam)
    {
        if (isFriendlyFireOn)
        {
            if ((shootersTeam == 1) && playerInteract.OnTeamAttackers())      { BulletDamage(incomingDamage, hadPowerShot); }
            else if ((shootersTeam == 2) && playerInteract.OnTeamDefenders()) { BulletDamage(incomingDamage, hadPowerShot); }
        }
        else
        {
            if (!((shootersTeam == 1) && playerInteract.OnTeamAttackers()))      { BulletDamage(incomingDamage, hadPowerShot); }
            else if (!((shootersTeam == 2) && playerInteract.OnTeamDefenders())) { BulletDamage(incomingDamage, hadPowerShot); }
        }
    }
    /// <summary>
    /// Player to get damaged by explosion. Shooter's Team key: 1) Attackers, 2) Defenders
    /// </summary>
    /// <param name="incomingDamage"></param>
    /// <param name="isFriendlyFireOn"></param>
    /// <param name="shootersTeam"></param>
    public void DamagePlayerExplosion(float incomingDamage, bool isFriendlyFireOn, byte shootersTeam)
    {
        if (isFriendlyFireOn)
        {
            if ((shootersTeam == 1) && playerInteract.OnTeamAttackers())      { ExplosionDamage(incomingDamage); }
            else if ((shootersTeam == 2) && playerInteract.OnTeamDefenders()) { ExplosionDamage(incomingDamage); }
        }
        else
        {
            if (!((shootersTeam == 1) && playerInteract.OnTeamAttackers()))      { ExplosionDamage(incomingDamage); }
            else if (!((shootersTeam == 2) && playerInteract.OnTeamDefenders())) { ExplosionDamage(incomingDamage); }
        }
    }
    /// <summary>
    /// Player to get damaged by melee. Shooter's Team key: 1) Attackers, 2) Defenders
    /// </summary>
    /// <param name="incomingDamage"></param>
    /// <param name="isChoppterOn"></param>
    /// <param name="isFriendlyFireOn"></param>
    /// <param name="shootersTeam"></param>
    public void DamagePlayerMelee(float incomingDamage, bool hadChopperOn, bool isFriendlyFireOn, byte shootersTeam)
    {
        if (isFriendlyFireOn)
        {
            if ((shootersTeam == 1) && playerInteract.OnTeamAttackers())      { MeleeDamage(incomingDamage, hadChopperOn); }
            else if ((shootersTeam == 2) && playerInteract.OnTeamDefenders()) { MeleeDamage(incomingDamage, hadChopperOn); }
        }
        else
        {
            if (!((shootersTeam == 1) && playerInteract.OnTeamAttackers()))      { MeleeDamage(incomingDamage, hadChopperOn); }
            else if (!((shootersTeam == 2) && playerInteract.OnTeamDefenders())) { MeleeDamage(incomingDamage, hadChopperOn); }
        }
    }
    public int GetMaxHealth()     { return maxHealth;     }
    public int GetCurrentHealth() { return currentHealth; }

    private void BulletDamage(float incomingDamage, bool hadPowerShot)
    {
        if (extraPaddingOn)
        {
            if (!hadPowerShot) { incomingDamage *= 0.92f; }
            //Else, they both cancel out
        }
        else
        {
            if (hadPowerShot) { incomingDamage *= 1.08f; }
        }
        currentHealth -= (int)incomingDamage;
        currentRegenWait = 0f;
        waitOneSecond = 0f;
    }
    private void ExplosionDamage(float incomingDamage)
    {
        if (mountainOn) { incomingDamage *= 0.8f; }
        currentHealth -= (int)incomingDamage;
        currentRegenWait = 0f;
        waitOneSecond = 0f;
    }
    private void MeleeDamage(float incomingDamage, bool hadChopperOn)
    {
        if (hadChopperOn) { incomingDamage += (incomingDamage * 0.2f); }
        currentHealth -= (int)incomingDamage;
        currentRegenWait = 0f;
        waitOneSecond = 0f;
    }
}
