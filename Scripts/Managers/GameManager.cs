using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private CameraFollow cameraFollow;

    public UnitStats[] fighters;

    public void Awake()
    {
        cameraFollow = FindObjectOfType<CameraFollow>();
    }
    public void Start()
    {
        //Time.timeScale = 0.3f;
        SetHealthBars();
        SetComboCounters();
        SetNotifications();
        SetOpponentReference();
    }

    public void SetHealthBars()
    {
        HUDHealthBar[] healthBars = FindObjectsOfType<HUDHealthBar>();
        for (int i = 0; i < healthBars.Length; i++)
        {
            if (healthBars[i].name.Contains("1P") || healthBars[i].name.Contains("P1"))
            {
                healthBars[i].SetForPlayerOne();
                fighters[0].SetHealth(1000, 2, healthBars[i]);
            }
            else
            {
                healthBars[i].SetForPlayerTwo();
                fighters[1].SetHealth(1000, 2, healthBars[i]);
            }
        }
    }
    public void SetComboCounters()
    {
        HUDComboCounter[] comboCounters = FindObjectsOfType<HUDComboCounter>();
        for(int i = 0; i < comboCounters.Length; i++)
        {
            if (comboCounters[i].name.Contains("1P") || comboCounters[i].name.Contains("P1"))
            {
                //set to player 1
                fighters[0].SetComboCounter(comboCounters[i]);
            }
            else
            {
                //set to player 2
                fighters[1].SetComboCounter(comboCounters[i]);
            }
        }
    }
    public void SetNotifications()
    {
        HUDBattleNotifications[] notifications = FindObjectsOfType<HUDBattleNotifications>();
        for(int i = 0; i < notifications.Length; i++)
        {
            if (notifications[i].name.Contains("1P") || notifications[i].name.Contains("P1"))
            {
                //set to player 1
                fighters[0].SetNotifications(notifications[i]);
            }
            else
            {
                //set to player 2
                fighters[1].SetNotifications(notifications[i]);
            }
        }
    }
    public void SetOpponentReference()
    {
        fighters[0].SetOpponent(fighters[1]);
        fighters[1].SetOpponent(fighters[0]);
    }
    /// <summary>
    /// Have the other unit drop the ledge.
    /// </summary>
    /// <param name="unitMove"></param>
    public void OtherPlayerDropLedge(UnitMove unitMove)
    {
        for (int i = 0; i < fighters.Length; i++)
        {
            if (fighters[i].GetComponent<UnitMove>() == unitMove)
            {
                if (i == 0)
                {
                    fighters[1].GetComponent<UnitMove>().SetDropToBelow(fighters[0].transform);
                }
                else
                {
                    fighters[0].GetComponent<UnitMove>().SetDropToBelow(fighters[1].transform);
                }
            }
        }
    }
    public void OtherPlayerPrepareDropLedge(UnitMove unitMove)
    {
        for (int i = 0; i < fighters.Length; i++)
        {
            if (fighters[i].GetComponent<UnitMove>() == unitMove)
            {
                if (i == 0)
                {
                    fighters[1].GetComponent<UnitMove>().SetEnemyFellFromLedge();
                }
                else
                {
                    fighters[0].GetComponent<UnitMove>().SetEnemyFellFromLedge();
                }
            }
        }
    }
}
