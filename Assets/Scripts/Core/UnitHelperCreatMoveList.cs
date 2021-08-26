using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitHelperCreatMoveList
{
    /// <summary>
    /// Create the movelist for the fighter.
    /// </summary>
    public Attack CreateMoveList(TextAsset textMoveList)
    {
        Debug.Log("Making movelist...");
        Attack rootAttack = new Attack();
        string[] lines = textMoveList.text.Split('\n');
        foreach (string line in lines)
        {
            string[] linePrep = line.Split('=');
            string[] lineBranching = linePrep[0].Split('.');

            if (lineBranching.Length == 1)
            {
                //Branch from the root
                CreateAttackHelper(lineBranching[0], linePrep[1], null, rootAttack);
            }
            else
            {
                //Branch off from the previous attack based on the numbers given.
                List<string> branching = new List<string>(lineBranching.Length);
                for (int i = 0; i < (lineBranching.Length - 1); i++)
                {
                    branching.Add(lineBranching[i]);
                }
                CreateAttackHelper(lineBranching[0], linePrep[1], branching, rootAttack);
            }
        }
        StopAddingAttacks(rootAttack);
        //Debug.Log("Created movelist. Starting moves: " + rootAttack.GetNextInStringCount());
        return rootAttack;
    }
    /// <summary>
    /// Create an attack into the movelist. Branching of zero will branch an attack from the root.
    /// </summary>
    /// <param name="attackName"></param>
    /// <param name="attackLine"></param>
    /// <param name="branching"></param>
    private void CreateAttackHelper(string attackName, string attackLine, List<string> branching, Attack rootAttack)
    {
        //Branch out from the current branching
        Attack currentAttackInString = rootAttack;
        if (branching != null)
        {
            if (branching.Count > 0)
            {
                for (int i = 0; i < branching.Count; i++)
                {
                    if (branching[i].ToCharArray()[0] == '*')
                    {
                        //Branch from the most recent attack
                        currentAttackInString = currentAttackInString.GetNextAttack(currentAttackInString.GetNextInStringCount() - 1);
                    }
                    else if (int.TryParse(branching[i], out int number))
                    {
                        //Branch attack from the specified number.
                        currentAttackInString = currentAttackInString.GetNextAttack(number);
                    }
                }
            }
        }
        //Add attacks from there.
        string[] attackString = attackLine.Split(';');
        for (int i = 0; i < attackString.Length; i++)
        {
            string moveName = (i != attackString.Length - 1) ? attackName + " Partial" : attackName;
            string[] attackData = attackString[i].Split(',');
            bool isFinalUniqueAttack = (i == attackString.Length - 1);
            //For any data still a string, turn it into an actual value

            //Attack and Movement Input
            string[] wholeInput = attackData[0].Split('-');
            List<byte> movementInput = new List<byte>(wholeInput.Length);
            byte attackInput = 0;
            for (int j = 0; j < wholeInput.Length; j++)
            {
                //Try parse to Movement
                //If fail, is an attack
                if (byte.TryParse(wholeInput[j], out byte isMovement))
                {
                    movementInput.Add(isMovement);
                }
                else
                {
                    char[] attacks = wholeInput[j].ToCharArray();
                    foreach (char c in attacks)
                    {
                        switch (c)
                        {
                            case 'P':
                                attackInput |= 0x1;
                                break;
                            case 'K':
                                attackInput |= (0x1 << 1);
                                break;
                            case 'S':
                                attackInput |= (0x1 << 2);
                                break;
                            case 'G':
                                attackInput |= (0x1 << 3);
                                break;
                            case 'B':
                                attackInput |= (0x1 << 3);
                                break;
                            default:
                                Debug.LogError("ERROR: Incorrect attack found.");
                                break;
                        }
                    }
                }
            }
            //Height of Attack
            byte attackHeight = 0;
            switch (attackData[1])
            {
                case "high":
                    attackHeight = 0x3;
                    break;
                case "mid":
                    attackHeight = 0x2;
                    break;
                case "low":
                    attackHeight = 0x1;
                    break;
                case "overhead":
                    attackHeight = 0x4;
                    break;
                default:
                    Debug.LogError("ERROR: Incorrect attack height found.");
                    break;
            }
            //Required Height of Attack
            byte requiredHeight = 0;
            switch (attackData[2])
            {
                case "standing":
                    requiredHeight = 2;
                    break;
                case "crouching":
                    requiredHeight = 1;
                    break;
                case "airborne":
                    requiredHeight = 3;
                    break;
                case "grounded":
                    requiredHeight = 0;
                    break;
                default:
                    Debug.LogError("ERROR: Incorrect required height attack found.");
                    break;
            }
            //Required Facing
            byte requiredFacing = 0;
            switch (attackData[3])
            {
                case "front":
                    requiredFacing = 0x1;
                    break;
                case "back":
                    requiredFacing = 0x3;
                    break;
                default:
                    Debug.LogError("ERROR: Incorrect required facing found.");
                    break;
            }

            //Make the attack
            string[] damageData = attackData[4].Split(':');
            Attack newAttack = new Attack(moveName, isFinalUniqueAttack, movementInput, attackInput,
                attackHeight, requiredHeight, requiredFacing,
                byte.Parse(damageData[0]), byte.Parse(damageData[1]), byte.Parse(attackData[5]), byte.Parse(attackData[6]),
                int.Parse(attackData[7]), byte.Parse(attackData[8]),
                new Vector2(float.Parse(attackData[9]), float.Parse(attackData[10])),
                new Vector2(float.Parse(attackData[11]), float.Parse(attackData[12])),
                new Vector2(float.Parse(attackData[13]), float.Parse(attackData[14])));

            currentAttackInString.AddNextInString(newAttack);
            currentAttackInString = newAttack;
        }
    }
    /// <summary>
    /// Make all attacks unable to add any more attacks.
    /// </summary>
    /// <param name="currentAttack"></param>
    private void StopAddingAttacks(Attack currentAttack)
    {
        if (currentAttack.HasNextInString())
        {
            foreach (Attack attack in currentAttack.GetNextInString())
            {
                StopAddingAttacks(attack);
            }
        }
        currentAttack.StopAddingAttacks();
    }
}
