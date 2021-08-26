using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDDebuggerMoveList : MonoBehaviour
{
    private TextMeshProUGUI fountainPen;

    private void Awake()
    {
        fountainPen = GetComponentInChildren<TextMeshProUGUI>();
    }
    private void Start()
    {
        
    }

    /// <summary>
    /// Set the Move List for the Attacks given.
    /// </summary>
    /// <param name="rootAttack"></param>
    public void SetMoveList(Attack rootAttack)
    {
        fountainPen.text = "";
        foreach(Attack attack in rootAttack.GetNextInString())
        {
            SetMoveListHelper(attack, new List<Attack>());
        }
    }

    private void SetMoveListHelper(Attack currentAttack, List<Attack> stacked)
    {
        //Record the last stack to prevent over-referencing
        List<Attack> newStack = new List<Attack>(5);
        if (stacked.Count > 0)
        {
            foreach (Attack attack in stacked)
            {
                newStack.Add(attack);
            }
        }
        //Add the next attack
        newStack.Add(currentAttack);
        if (currentAttack.HasNextInString())
        {
            foreach (Attack nextAttack in currentAttack.GetNextInString())
            {
                //if (currentAttack.IsUniqueFinalAttack())
                //{
                //buttons[moveListIndex].SetMove(newStack);
                //buttons[moveListIndex].gameObject.SetActive(true);
                //moveListIndex++;
                //}
                SetMoveListHelper(nextAttack, newStack);
            }
        }
        else
        {
            if (currentAttack.IsUniqueFinalAttack())
            {
                //buttons[moveListIndex].SetMove(newStack);
                //buttons[moveListIndex].gameObject.SetActive(true);
                //moveListIndex++;
                foreach (Attack attack in newStack)
                {
                    fountainPen.text += attack.GetAttackInputVersionVisual();
                }
                fountainPen.text += "\n";
            }
        }
    }
}
