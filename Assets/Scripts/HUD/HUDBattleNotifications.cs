using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// Script in charge of keeping track of battle notifications.
/// </summary>
public class HUDBattleNotifications : MonoBehaviour
{
    private HUDBattleNotification[] notifications;

    public void Awake()
    {
        notifications = GetComponentsInChildren<HUDBattleNotification>();
    }
    /// <summary>
    /// Add a notification to the latest in the list.
    /// </summary>
    /// <param name="text"></param>
    public void AddNotifaction(string text)
    {
        for(int i = 0; i < notifications.Length; i++)
        {
            if (notifications[i].gameObject.activeInHierarchy)
            {
                continue;
            }
            notifications[i].SetText(text);
            notifications[i].gameObject.SetActive(true);
            return;
        }
    }
    /// <summary>
    /// Set the top most notification to be important.
    /// </summary>
    /// <param name="text"></param>
    public void AddNotifcationPriority(string text)
    {
        for (int i = notifications.Length - 1; i >= 0; i++)
        {
            //If active, push it down the line.
            if (notifications[i].gameObject.activeInHierarchy)
            {
                //If the last in the list, clear it off
                if (i == (notifications.Length - 1))
                {
                    notifications[i].ClearText();
                    continue;
                }
                notifications[i + 1] = notifications[i];
            }
        }
        notifications[0].SetText(text);
    }
    /// <summary>
    /// Turn off a specific notification.
    /// </summary>
    /// <param name="notification"></param>
    public void TurnOffNotification(HUDBattleNotification notification)
    {
        for(int i = 0; i < notifications.Length; i++)
        {
            if (notifications[i] == notification)
            {
                notifications[i].ClearText();
                //if (i < (notifications.Length - 1))
                //{
                //}
            }
        }
    }
    /// <summary>
    /// Turn off the top Notification if it says "Block Standing Low."
    /// </summary>
    public void TurnOffTopNotification()
    {
        if (notifications[0].StatesBlockStandingLow())
        {
            notifications[0].ClearText();
        }
    }
    /// <summary>
    /// Turn off all the notications.
    /// </summary>
    public void TurnOffAll()
    {
        foreach(HUDBattleNotification notification in notifications)
        {
            notification.ClearText();
            notification.gameObject.SetActive(false);
        }
    }
}
