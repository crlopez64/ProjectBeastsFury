using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Script in charge of the visual history of the Player.
/// </summary>
public class HUDInputHistory : MonoBehaviour
{
    private TextMeshProUGUI fountainPen;
    private StringBuilder stringBuilder;
    private List<string> inputs;
    private float timer;

    private void Awake()
    {
        fountainPen = GetComponent<TextMeshProUGUI>();
        stringBuilder = new StringBuilder();
        inputs = new List<string>();
    }
    private void Start()
    {
        fountainPen.text = "";
    }
    private void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
        }
    }

    /// <summary>
    /// Show the inputs placed that the player has done in that particular tick.
    /// </summary>
    /// <param name="movement"></param>
    /// <param name="attack"></param>
    public void SetInput(byte movement, byte attack)
    {
        if (timer <= 0)
        {
            inputs.Clear();
        }
        timer = 5f;
        string currentInput = "";
        if ((movement > 0) && (movement != 5) && (movement < 10))
        {
            //Set movement sprite
            if (movement < 5)
            {
                currentInput += "<sprite=" + (movement - 1) + ">";
            }
            else
            {
                currentInput += "<sprite=" + (movement - 2) + ">";
            }
        }
        //Set for next Attacks
        if ((attack & 0x1) == 0x1)
        {
            currentInput += "<sprite=8>";
        }
        if (((attack & (0x1 << 1)) >> 1) == 0x1)
        {
            currentInput += "<sprite=9>";
        }
        if (((attack & (0x1 << 2)) >> 2) == 0x1)
        {
            currentInput += "<sprite=10>";
        }
        if (((attack & (0x1 << 3)) >> 3) == 0x1)
        {
            currentInput += "<sprite=11>";
        }
        if (((attack & (0x1 << 4)) >> 4) == 0x1)
        {
            currentInput += "<sprite=12>";
        }
        inputs.Add(currentInput);
        if (inputs.Count > 12)
        {
            inputs.RemoveAt(0);
        }
        SetText();
    }

    /// <summary>
    /// Set the Input History text.
    /// </summary>
    private void SetText()
    {
        stringBuilder.Clear();
        foreach(string s in inputs)
        {
            stringBuilder.Append(s);
            stringBuilder.Append("\n");
        }
        fountainPen.text = stringBuilder.ToString();
    }
}
