using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class for a basic Attack to be inputted. Will keep record of (no Input)Attack, Forward-Attack, and QCF-Attack.
/// </summary>
public class Attack
{
    private bool addNewAttacks;
    private readonly bool isUniqueFinalAttack;
    private readonly List<List<byte>> movementInputList; //1-9 Input
    private readonly byte damageOnHit;
    private readonly byte damageOnBlock;
    private readonly byte attackInput;    //0000: Guard, Strike, Kick, Punch
    private readonly byte hitsAllowed;    //Can be used to determine multi hit attacks
    private readonly byte attackHeight;   //01: Low, 10: Mid, 11: High, 100: Overhead
    private readonly byte requiredHeight; //00: Lying On Ground, 01: Crouching, 10: Standing, 11: Airborne, 
    private readonly byte requiredFacing; //01: Front-Facing, 10: Back-Facing
    private readonly byte enemyCounterHitAnim;
    private readonly int attackAnimation;
    private readonly int plusMinusOnHit;
    private readonly int plusMinusOnBlock;
    private Vector2 playerMove;
    private Vector2 enemyKnockback;
    private Vector2 knockbackCounter;
    private Vector2 knockbackStunned;
    private readonly string attackName;
    private readonly List<Attack> nextInString;

    /// <summary>
    /// Null constuctor for an Attack. Should be used for Root Attacks.
    /// </summary>
    public Attack()
    {
        addNewAttacks = true;
        isUniqueFinalAttack = false;
        movementInputList = null;
        damageOnHit = 0;
        hitsAllowed = 0;
        damageOnBlock = 0;
        attackInput = 0;
        attackHeight = 0;
        //enemyHitAnim = 0;
        requiredFacing = 0;
        requiredHeight = 0;
        attackAnimation = 0;
        enemyCounterHitAnim = 0;
        playerMove = Vector2.zero;
        enemyKnockback = Vector2.zero;
        knockbackCounter = Vector2.zero;
        attackName = "";
        nextInString = new List<Attack>();
    }
    /// <summary>
    /// Normal constuctor for an Attack.
    /// </summary>
    /// <param name="attackName"></param>
    /// <param name="isUniqueFinalAttack"></param>
    /// <param name="movementInput"></param>
    /// <param name="attackInput"></param>
    /// <param name="attackHeight"></param>
    /// <param name="requiredHeight"></param>
    /// <param name="requiredFacing"></param>
    /// <param name="damageOnHit"></param>
    /// <param name="damageOnBlock"></param>
    /// <param name="enemyHitAnim"></param>
    /// <param name="enemyCounterHitAnim"></param>
    /// <param name="attackAnimation"></param>
    /// <param name="hitsAllowed"></param>
    /// <param name="playerMove"></param>
    /// <param name="enemyKnockback"></param>
    /// <param name="knockbackCounter"></param>
    public Attack(string attackName, bool isUniqueFinalAttack,
        List<byte> movementInput, byte attackInput, byte attackHeight, byte requiredHeight,
        byte requiredFacing, byte damageOnHit, byte damageOnBlock, byte enemyHitAnim, byte enemyCounterHitAnim, int attackAnimation, byte hitsAllowed,
        Vector2 playerMove, Vector2 enemyKnockback, Vector2 knockbackCounter)
    {
        this.attackName = attackName;
        addNewAttacks = true;
        this.isUniqueFinalAttack = isUniqueFinalAttack;
        movementInputList = new List<List<byte>>(1);
        List<byte> mainMovementInput = new List<byte>(movementInput.Count);
        foreach(byte b in movementInput)
        {
            mainMovementInput.Add(b);
        }
        movementInputList.Add(mainMovementInput);
        this.attackInput = attackInput;
        this.attackHeight = attackHeight;
        this.requiredHeight = requiredHeight;
        this.requiredFacing = requiredFacing;
        this.damageOnHit = damageOnHit;
        this.damageOnBlock = damageOnBlock;
        this.playerMove = playerMove;
        this.hitsAllowed = hitsAllowed;
        //this.enemyHitAnim = enemyHitAnim;
        plusMinusOnHit = 4;
        plusMinusOnBlock = 10;
        this.enemyCounterHitAnim = enemyCounterHitAnim;
        this.attackAnimation = attackAnimation;
        this.enemyKnockback = enemyKnockback;
        this.knockbackCounter = knockbackCounter;
        nextInString = new List<Attack>();
        FindShortcutMovement(movementInput);
    }

    /// <summary>
    /// If possible, add a new attack in the string.
    /// </summary>
    /// <param name="nextAttack"></param>
    public void AddNextInString(Attack nextAttack)
    {
        if (addNewAttacks)
        {
            nextInString.Add(nextAttack);
        }
    }
    /// <summary>
    /// Stop adding new attacks to this attack.
    /// </summary>
    public void StopAddingAttacks()
    {
        addNewAttacks = false;
        nextInString.TrimExcess();
    }
    /// <summary>
    /// Does this Attack have the potential to pop the Opponent into the air? Combo potent.
    /// </summary>
    /// <returns></returns>
    public bool KnockbackPopUp()
    {
        return false;
    }
    /// <summary>
    /// Must the Player actively place 1 input to use this move?
    /// </summary>
    /// <param name="movementInputCount"></param>
    /// <returns></returns>
    public bool RestrictInput(int movementInputCount)
    {
        return movementInputList[0].Count == 1;
    }
    /// <summary>
    /// Does this attack branch off into other attacks?
    /// </summary>
    /// <returns></returns>
    public bool HasNextInString()
    {
        if (nextInString != null)
        {
            return nextInString.Count > 0;
        }
        return false;
    }
    /// <summary>
    /// Is this the final unique attack?
    /// </summary>
    /// <returns></returns>
    public bool IsUniqueFinalAttack()
    {
        return isUniqueFinalAttack;
    }
    /// <summary>
    /// Return the plus-minus frames on a getting hit by this attack. This will return the inverse of the advantage given.
    /// If plus, User should have advantage.
    /// </summary>
    /// <returns></returns>
    public int PlusMinusOnHit()
    {
        return -plusMinusOnHit;
    }
    /// <summary>
    /// Return the plus-minus frames on a getting blocked by this attack. If plus, User should have advantage.
    /// </summary>
    /// <returns></returns>
    public int PlusMinusOnBlock()
    {
        return -plusMinusOnBlock;
    }
    /// <summary>
    /// Return if the Attack requires an input of any combination of Punch, Kick, Strike (Horns/Tail/etc.), or Guard.
    /// </summary>
    /// <returns></returns>
    public int AttackInput()
    {
        return attackInput;
    }
    /// <summary>
    /// Return the amount of hits this Attack can land while the Hitboxes are active.
    /// </summary>
    /// <returns></returns>
    public byte HitsAllowed()
    {
        return hitsAllowed;
    }
    /// <summary>
    /// Return if this attack classifies as a Low, Mid, or a High (0x01, 0x11, or 0x10 respectively).
    /// </summary>
    /// <returns></returns>
    public byte AttackHeight()
    {
        return attackHeight;
    }
    /// <summary>
    /// Return if the Player should be standing, crouching, or in the air (0x01, 0x10, or 0x11 respectively).
    /// </summary>
    /// <returns></returns>
    public byte RequiredHeight()
    {
        return requiredHeight;
    }
    /// <summary>
    /// Return if the Player should be facing the Opponent or away the Opponent (0x01 or 0x10 respectively).
    /// </summary>
    /// <returns></returns>
    public byte DirectionFacing()
    {
        return requiredFacing;
    }
    /// <summary>
    /// Return the damage to give the Opponent.
    /// </summary>
    /// <returns></returns>
    public byte DamageOnHit()
    {
        return damageOnHit;
    }
    /// <summary>
    /// Return the damage to give the Opponent if blocking. May return zero.
    /// </summary>
    /// <returns></returns>
    public byte DamageOnBlock()
    {
        return damageOnBlock;
    }
    /// <summary>
    /// The Animation the Enemy should play on hit. Returns the Height of Attack.
    /// </summary>
    /// <returns></returns>
    public byte EnemyHitAnimation()
    {
        //return enemyHitAnim;
        return attackHeight;
    }
    /// <summary>
    /// Get the total number of attacks that can branch from this Attack.
    /// </summary>
    /// <returns></returns>
    public int GetNextInStringCount()
    {
        return nextInString.Count;
    }
    /// <summary>
    /// Return the attack animation to play for this attack.
    /// </summary>
    /// <returns></returns>
    public int AttackAnimation()
    {
        return attackAnimation;
    }
    /// <summary>
    /// Get the attack name for this attack.
    /// </summary>
    /// <returns></returns>
    public string GetAttackName()
    {
        return attackName;
    }
    /// <summary>
    /// Return the input for this attack and ready for button visuals (using TMPro sprites).
    /// </summary>
    /// <returns></returns>
    public string GetAttackInputVersionVisual()
    {
        string input = "";
        if (movementInputList[0].Count > 0)
        {
            foreach (byte movement in movementInputList[0])
            {
                if (movement < 5)
                {
                    input += "<sprite=" + (movement - 1) + ">";
                }
                else if ((movement > 5) && (movement < 10))
                {
                    input += "<sprite=" + (movement - 2) + ">";
                }
            }
        }
        if ((attackInput & 0x1) == 0x1)
        {
            input += "<sprite=8>";
        }
        if (((attackInput & (0x1 << 1)) >> 1) == 0x1)
        {
            input += "<sprite=9>";
        }
        if (((attackInput & (0x1 << 2)) >> 2) == 0x1)
        {
            input += "<sprite=10>";
        }
        if (((attackInput & (0x1 << 3)) >> 3) == 0x1)
        {
            input += "<sprite=11>";
        }
        if (((attackInput & (0x1 << 4)) >> 4) == 0x1)
        {
            input += "<sprite=12>";
        }
        return input;
    }
    /// <summary>
    /// Return the input for this attack as a text.
    /// </summary>
    /// <returns></returns>
    public string GetAttackInputVersionDebugger()
    {
        string input = "";
        if (movementInputList[0].Count > 0)
        {
            foreach (byte movement in movementInputList[0])
            {
                input += movement;
            }
        }
        if ((attackInput & 0x1) == 1)
        {
            input += "P";
        }
        if (((attackInput & (0x1 << 1)) >> 1) == 0x1)
        {
            input += "K";
        }
        if (((attackInput & (0x1 << 2)) >> 2) == 0x1)
        {
            input += "S";
        }
        if (((attackInput & (0x1 << 3)) >> 3) == 0x1)
        {
            input += "G";
        }
        if (((attackInput & (0x1 << 4)) >> 4) == 0x1)
        {
            input += "B";
        }
        input += " ";
        return input;
    }
    /// <summary>
    /// Get the Vector of the Player moving into the attack.
    /// </summary>
    /// <returns></returns>
    public Vector2 PlayerMove()
    {
        return playerMove;
    }
    /// <summary>
    /// Return the Enemy Knockback of the attack.
    /// </summary>
    /// <returns></returns>
    public Vector2 EnemyKnockback()
    {
        return enemyKnockback;
    }
    /// <summary>
    /// Get back a specific attack 
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public Attack GetNextAttack(int index)
    {
        return nextInString[index];
    }
    /// <summary>
    /// Get all the next attacks in the current Attack String.
    /// </summary>
    /// <returns></returns>
    public List<Attack> GetNextInString()
    {
        return nextInString;
    }

    /// <summary>
    /// Check if the input is valid for this attack.
    /// </summary>
    /// <param name="movementInput"></param>
    /// <param name="attackInput"></param>
    /// <returns></returns>
    public bool InputMatch(List<byte> movementInput, byte attackInput, byte currentHeight)
    {
        //BASE CASE: If there is no attack input, return false
        if (attackInput == 0)
        {
            Debug.Log("Attack input equals zero. Return false");
            return false;
        }
        //Check if required height is true
        if (currentHeight != requiredHeight)
        {
            Debug.Log("User Height does not match required height. User Height: " + currentHeight + ", Required Height: " + requiredHeight);
            return false;
        }
        //If there is a neutral movement input, or doesn't exist, then only consider attack if movement is not required
        if ((movementInput == null) || (movementInput.Count == 0))
        {
            if ((movementInputList[0][0] == 5))
            {
                return HasAttackInput(attackInput);
            }
            return false;
        }
        //If there is only attack input required, then only consider attack
        if ((movementInputList[0][0] == 5))
        {
            return HasAttackInput(attackInput);
        }
        //Otherwise, check movement and attack input
        return HasMovementInput(movementInput) && HasAttackInput(attackInput);
    }

    /// <summary>
    /// Check if the Movement input is valid.
    /// </summary>
    /// <param name="movementInput"></param>
    /// <returns></returns>
    private bool HasMovementInput(List<byte> movementInput)
    {
        foreach(List<byte> requiredInput in movementInputList)
        {
            int index = 0;
            for(int i = 0; i < movementInput.Count; i++)
            {
                if (index >= requiredInput.Count)
                {
                    return true;
                }
                if (movementInput[i] == requiredInput[index])
                {
                    index++;
                    if (index >= requiredInput.Count)
                    {
                        return true;
                    }
                }
                else
                {
                    index = 0;
                }
            }
        }
        return false;
    }
    /// <summary>
    /// Check if an attack input matches this Attack's attack input.
    /// </summary>
    /// <param name="attackInput"></param>
    /// <returns></returns>
    private bool HasAttackInput(byte attackInput)
    {
        //Check if the input is exactly the same
        if (this.attackInput == attackInput)
        {
            return true;
        }
        //Else, if most of the input is valid, check true
        if ((attackInput & 0x1) == 0x1)
        {
            if ((this.attackInput & 0x1) != (attackInput & 0x1))
            {
                return false;
            }
        }
        if (((attackInput & (0x1 << 1)) >> 1) == 0x1)
        {
            if (((this.attackInput & (0x1 << 1)) >> 1) != ((attackInput & (0x1 << 1)) >> 1))
            {
                return false;
            }
        }
        if (((attackInput & (0x1 << 2)) >> 2) == 0x1)
        {
            if (((this.attackInput & (0x1 << 2)) >> 2) != ((attackInput & (0x1 << 2)) >> 2))
            {
                return false;
            }
        }
        if (((attackInput & (0x1 << 3)) >> 3) == 0x1)
        {
            if (((this.attackInput & (0x1 << 3)) >> 3) != ((attackInput & (0x1 << 3)) >> 3))
            {
                return false;
            }
        }
        if (((attackInput & (0x1 << 4)) >> 4) == 0x1)
        {
            if (((this.attackInput & (0x1 << 4)) >> 4) != ((attackInput & (0x1 << 4)) >> 4))
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// Create shortcuts for the Attack if needed.
    /// </summary>
    /// <param name="movementInput"></param>
    private void FindShortcutMovement(List<byte> movementInput)
    {
        //If directional input is 2 inputs or less, ignore
        if (movementInput.Count < 3)
        {
            return;
        }
        if (movementInput.Count == 3)
        {
            //QCF Shortcuts
            if ((movementInput[0] == 2) && (movementInput[1] == 3) && (movementInput[2] == 6))
            {
                List<byte> shortcut0 = new List<byte>(3) { 2, 3, 9 };
                List<byte> shortcut1 = new List<byte>(2) { 2, 6 };
                movementInputList.Add(shortcut0);
                movementInputList.Add(shortcut1);
            }
            //QCB Shortcuts
            if ((movementInput[0] == 2) && (movementInput[1] == 1) && (movementInput[2] == 4))
            {
                List<byte> shortcut0 = new List<byte>(3) { 2, 1, 7 };
                List<byte> shortcut1 = new List<byte>(2) { 2, 4 };
                movementInputList.Add(shortcut0);
                movementInputList.Add(shortcut1);
            }
            //DP Shortcuts
            if ((movementInput[0] == 6) && (movementInput[1] == 2) && (movementInput[2] == 3))
            {
                List<byte> shortcut0 = new List<byte>(3) { 6, 2, 6 };
                List<byte> shortcut1 = new List<byte>(5) { 6, 3, 2, 3, 6 };
                movementInputList.Add(shortcut0);
                movementInputList.Add(shortcut1);
            }
            //BDP Shortcuts
            if ((movementInput[0] == 4) && (movementInput[1] == 2) && (movementInput[2] == 1))
            {
                List<byte> shortcut0 = new List<byte>(3) { 4, 1, 4 };
                List<byte> shortcut1 = new List<byte>(5) { 4, 1, 2, 1, 4 };
                movementInputList.Add(shortcut0);
                movementInputList.Add(shortcut1);
            }
            //QCFD Shortcuts
            if ((movementInput[0] == 6) && (movementInput[1] == 3) && (movementInput[2] == 2))
            {
                List<byte> shortcut0 = new List<byte>(2) { 6, 2 };
                movementInputList.Add(shortcut0);
            }
            //QCBD Shortcuts
            if ((movementInput[0] == 4) && (movementInput[1] == 1) && (movementInput[2] == 2))
            {
                List<byte> shortcut0 = new List<byte>(2) { 4, 2 };
                movementInputList.Add(shortcut0);
            }
        }
        else if (movementInput.Count == 5)
        {
            //HCF Shortcuts
            if ((movementInput[0] == 4) && (movementInput[1] == 1) && (movementInput[2] == 2)
                && (movementInput[2] == 3) && (movementInput[2] == 6))
            {
                List<byte> shortcut0 = new List<byte>(3) { 4, 2, 6 };
                List<byte> shortcut1 = new List<byte>(3) { 4, 2, 9 };
                movementInputList.Add(shortcut0);
                movementInputList.Add(shortcut1);
            }
            //HCB Shortcuts
            if ((movementInput[0] == 6) && (movementInput[1] == 3) && (movementInput[2] == 2)
                && (movementInput[2] == 1) && (movementInput[2] == 4))
            {
                List<byte> shortcut0 = new List<byte>(3) { 6, 2, 4 };
                List<byte> shortcut1 = new List<byte>(3) { 6, 2, 7 };
                movementInputList.Add(shortcut0);
                movementInputList.Add(shortcut1);
            }
        }
        else if (movementInput.Count == 7)
        {
            //QCF,HCB Shortcuts
            if ((movementInput[0] == 2) && (movementInput[1] == 3) && (movementInput[2] == 6)
                && (movementInput[2] == 3) && (movementInput[2] == 2)
                && (movementInput[2] == 1) && (movementInput[2] == 4))
            {
                List<byte> shortcut0 = new List<byte>(4) { 2, 6, 2, 4 };
                List<byte> shortcut1 = new List<byte>(4) { 2, 6, 2, 7 };
                movementInputList.Add(shortcut0);
                movementInputList.Add(shortcut1);
            }
            //QCB,HCF Shortcuts
            if ((movementInput[0] == 2) && (movementInput[1] == 1) && (movementInput[2] == 4)
                && (movementInput[2] == 1) && (movementInput[2] == 2)
                && (movementInput[2] == 3) && (movementInput[2] == 6))
            {
                List<byte> shortcut0 = new List<byte>(4) { 2, 4, 2, 6 };
                List<byte> shortcut1 = new List<byte>(4) { 2, 4, 2, 9 };
                movementInputList.Add(shortcut0);
                movementInputList.Add(shortcut1);
            }
        }
    }
}
