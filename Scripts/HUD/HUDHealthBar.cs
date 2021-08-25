using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Script in charge of showing the Health Bar for a Unit.
/// </summary>
public class HUDHealthBar : MonoBehaviour
{
    private Slider slider;
    private byte maxRounds;
    private byte currentRounds;
    private byte determinateForPlayerOne;
    private static Color greenColor = new Color(0.235f, 0.86f, 0.39f);
    private static Color grassYellowColor = new Color(0.88f, 1f, 0.2f);
    private static Color yellowColor = new Color(1f, 1f, 0.31f);
    private static Color orangeColor = new Color(1f, 0.52f, 0.2f);
    private static Color redColor = new Color(1f, 0.31f, 0.156f);

    public Image fill;
    public Image fillDrain;

    public void Awake()
    {
        slider = GetComponent<Slider>();
        slider.wholeNumbers = true;
    }
    public void Update()
    {
        switch(maxRounds)
        {
            case 1:
                fill.color = Color.Lerp(redColor, orangeColor, slider.value / slider.maxValue);
                break;
            case 2:
                switch(currentRounds)
                {
                    case 1:
                        fill.color = Color.Lerp(redColor, yellowColor, slider.value / slider.maxValue);
                        break;
                    case 2:
                        fill.color = Color.Lerp(yellowColor, greenColor, slider.value / slider.maxValue);
                        break;
                    default:
                        fill.color = redColor;
                        break;
                }
                break;
            case 3:
                switch(currentRounds)
                {
                    case 1:
                        fill.color = Color.Lerp(redColor, orangeColor, slider.value / slider.maxValue);
                        break;
                    case 2:
                        fill.color = Color.Lerp(orangeColor, yellowColor, slider.value / slider.maxValue);
                        break;
                    case 3:
                        fill.color = Color.Lerp(yellowColor, greenColor, slider.value / slider.maxValue);
                        break;
                    default:
                        fill.color = redColor;
                        break;
                }
                break;
            case 4:
                switch (currentRounds)
                {
                    case 1:
                        fill.color = Color.Lerp(redColor, orangeColor, slider.value / slider.maxValue);
                        break;
                    case 2:
                        fill.color = Color.Lerp(orangeColor, yellowColor, slider.value / slider.maxValue);
                        break;
                    case 3:
                        fill.color = Color.Lerp(yellowColor, grassYellowColor, slider.value / slider.maxValue);
                        break;
                    case 4:
                        fill.color = Color.Lerp(grassYellowColor, greenColor, slider.value / slider.maxValue);
                        break;
                    default:
                        fill.color = redColor;
                        break;
                }
                break;
            case 5:
                switch (currentRounds)
                {
                    case 1:
                        fill.color = Color.Lerp(redColor, orangeColor, slider.value / slider.maxValue);
                        break;
                    case 2:
                        fill.color = Color.Lerp(orangeColor, yellowColor, slider.value / slider.maxValue);
                        break;
                    case 3:
                        fill.color = Color.Lerp(yellowColor, grassYellowColor, slider.value / slider.maxValue);
                        break;
                    case 4:
                        fill.color = Color.Lerp(grassYellowColor, greenColor, slider.value / slider.maxValue);
                        break;
                    case 5:
                        fill.color = Color.Lerp(greenColor, greenColor, slider.value / slider.maxValue);
                        break;
                    default:
                        fill.color = redColor;
                        break;
                }
                break;
        }
    }

    /// <summary>
    /// Set this health bar for use by Player 1.
    /// </summary>
    public void SetForPlayerOne()
    {
        if (AlreadyDetermined())
        {
            return;
        }
        determinateForPlayerOne |= 0x3;
    }
    /// <summary>
    /// Set this health bar for use by Player 2.
    /// </summary>
    public void SetForPlayerTwo()
    {
        if (AlreadyDetermined())
        {
            return;
        }
        determinateForPlayerOne |= 0x2;
    }
    /// <summary>
    /// Set a new maximum value for max health and set it as that.
    /// </summary>
    /// <param name="maxHealth"></param>
    /// <param name="rounds"></param>
    public void SetMaxHealth(int maxHealth, byte maxRounds)
    {
        slider.maxValue = maxHealth;
        slider.value = maxHealth;
        this.maxRounds = maxRounds;
        currentRounds = maxRounds;
    }
    /// <summary>
    /// Refill the health bar without going down a round.
    /// </summary>
    public void RefillHealthWin()
    {
        slider.value = slider.maxValue;
    }
    /// <summary>
    /// Refill the health bar and change the color scheme.
    /// </summary>
    public void RefillHealthLose()
    {
        currentRounds--;
        slider.value = slider.maxValue;
    }
    /// <summary>
    /// Set the current health for the slider.
    /// </summary>
    /// <param name="currentHealth"></param>
    public void SetValue(int currentHealth)
    {
        slider.value = currentHealth;
        slider.value = Mathf.Clamp(slider.value, slider.minValue, slider.maxValue);
    }

    /// <summary>
    /// Has this Health bar already been determined for player one?
    /// </summary>
    /// <returns></returns>
    private bool AlreadyDetermined()
    {
        return ((determinateForPlayerOne >> 1) & 0x1) == 0x1;
    }
}
