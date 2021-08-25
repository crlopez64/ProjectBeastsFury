using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Script in charge of keeping track of the combo numbers.
/// </summary>
public class HUDComboCounter : MonoBehaviour
{
    private HUDComboWord hudComboWord;
    private TextMeshProUGUI counter;
    private int count;

    public void Awake()
    {
        counter = GetComponentInChildren<TextMeshProUGUI>();
        hudComboWord = GetComponentInChildren<HUDComboWord>();
    }
    public void Start()
    {
        counter.text = count.ToString();
        counter.text = "";
        hudComboWord.gameObject.SetActive(false);
    }

    /// <summary>
    /// Add a hit to the counter.
    /// </summary>
    public void AddHit()
    {
        count++;
        if (count > 1)
        {
            counter.text = count.ToString();
            hudComboWord.gameObject.SetActive(true);
        }
    }
    /// <summary>
    /// Reset the counter and turn it off.
    /// </summary>
    public void ResetCounter()
    {
        count = 0;
        counter.text = "";
        hudComboWord.gameObject.SetActive(false);
    }
    /// <summary>
    /// Set the counter with a value.
    /// </summary>
    /// <param name="count"></param>
    public void SetCounter(int count)
    {
        this.count = count;
        if (count > 1)
        {
            counter.text = count.ToString();
            hudComboWord.gameObject.SetActive(true);
        }
    }
}
