using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Debug_AttackFrame : MonoBehaviour
{
    private TextMeshProUGUI fountainPen;

    public UnitAttack player;

    public void Awake()
    {
        fountainPen = GetComponent<TextMeshProUGUI>();
    }

    public void Update()
    {
        string temp = "";
        switch(player.CurrentAttackFrameType())
        {
            case 1:
                temp = "Start Up";
                break;
            case 2:
                temp = "ACTIVE";
                break;
            case 3:
                temp = "Recovery";
                break;
            default:
                temp = "Inactive";
                break;
        }
        fountainPen.text = "";
        fountainPen.text = temp;
    }
}
