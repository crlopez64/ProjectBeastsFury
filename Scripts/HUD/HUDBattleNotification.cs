using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// Script in charge of a single battle notification.
/// </summary>
public class HUDBattleNotification : MonoBehaviour
{
    private HUDBattleNotifications parent;
    private TextMeshProUGUI fountainPen;
    private float timer;

    public void Awake()
    {
        parent = GetComponentInParent<HUDBattleNotifications>();
        fountainPen = GetComponent<TextMeshProUGUI>();
    }
    public void Update()
    {
        if (timer > 0f)
        {
            timer -= Time.deltaTime;
        }
        else
        {
            ClearText();
            gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Set the text of the battle notification.
    /// </summary>
    /// <param name="text"></param>
    public void SetText(string text)
    {
        fountainPen.text = text;
    }
    /// <summary>
    /// Clear off the battle notification.
    /// </summary>
    public void ClearText()
    {
        fountainPen.text = "";
    }
    /// <summary>
    /// Does the notification state that the Unit is blocking standing low?
    /// </summary>
    /// <returns></returns>
    public bool StatesBlockStandingLow()
    {
        return fountainPen.text == "BLOCK STANDING LOW";
    }
}
