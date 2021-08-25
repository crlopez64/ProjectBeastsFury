using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Script to visually show one Ability a Merc has. Will show visual cooldown and amount of uses left.
/// </summary>
public class AbilityText : MonoBehaviour
{
    private float abilityCooldown;

    /// <summary>
    /// The name of the Ability.
    /// </summary>
    public Text abilityText;
    /// <summary>
    /// The spam amount you can do on any given cooldown. If no uses remaining, replace with "COOLDOWN".
    /// </summary>
    public Text usesLeft;
    /// <summary>
    /// The graphic for the Ability.
    /// </summary>
    public Image sprite;
    /// <summary>
    /// The red circle to show cooldown
    /// </summary>
    public Image cooldownCircle;

    public void SetAbilityCooldown(float cooldown)
    {
        abilityCooldown = cooldown;
    }
    public void SetCooldownCircle(float cooldown)
    {
        cooldownCircle.fillAmount = (cooldown / abilityCooldown);
    }
}
