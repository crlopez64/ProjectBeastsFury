using UnityEngine;
using UnityEngine.UI;

public class ActionText : MonoBehaviour
{
    private Text fountainPen;
    private bool makeTimer;
    private float timerTillFade;

    private void Awake()
    {
        fountainPen = GetComponent<Text>();
    }
    private void Start()
    {
        fountainPen.enabled = false;
	}
	
    public void ShowTextObjective(int objective, string item)
    {
        fountainPen.enabled = true;
        switch(objective)
        {
            case 0:
                fountainPen.text = "Need the Objective to continue!!";
                break;
            case 1:
                fountainPen.text = "Press [F] to pick up the " + item;
                break;
            case 2:
                fountainPen.text = "Press [F] to plant the " + item;
                break;
            case 3:
                fountainPen.text = "Press [F] to place the " + item;
                break;
            case 4:
                fountainPen.text = "Press [F] to return the " + item;
                break;
            case 5:
                fountainPen.text = "Press [F] to repair the " + item;
                break;
            case 6:
                fountainPen.text = "Press [F] to hack the " + item;
                break;
            case 7:
                fountainPen.text = "Press [F] to disarm the " + item;
                return;
            default:
                fountainPen.text = "BUGGED OUT!!";
                break;
        }
    }
    public void ShowTextRequireItem()
    {
        fountainPen.enabled = true;
        fountainPen.text = "Need the Objective to continue!!";
    }
    public void ShowTextInventoryFull()
    {
        fountainPen.enabled = true;
        fountainPen.text = "Already holding an Objective!!";
    }
    public void ShowTextAlreadyReturned()
    {
        fountainPen.enabled = true;
        fountainPen.text = "Already back at Starting Position!!";
    }
    public void ShowTextFixOnCooldown()
    {
        fountainPen.enabled = true;
        fountainPen.text = "Repair on Cooldown!!";
    }
    public void ShowTextBetterHaxxor()
    {
        fountainPen.enabled = true;
        fountainPen.text = "Someone better is hacking! Protect them!!";
    }
    public void ShowTextArmorMaxed()
    {
        fountainPen.enabled = true;
        fountainPen.text = "Armor is already fully kept!!";
    }
    public void TurnOffText()
    {
        fountainPen.enabled = false;
    }
}
