using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Script in charge of Input by the player.
/// </summary>
[RequireComponent(typeof(UnitMove))]
[RequireComponent(typeof(UnitAttack))]
public class PlayerInput : MonoBehaviour
{
    private List<byte> directionalInputHistory;
    private List<byte> directionalInputHistorySend;
    private Vector2 directionalInput;
    private UnitMove unitMove;
    private UnitAttack unitAttack;
    private bool tryingToBlock;
    private byte attackInput;
    private byte previousInput;
    private byte attackInputSend;
    private byte directionalByte;
    private float timerAttackStick; //If 0 >, if Press a button, join with another button.
    private float timerToResetInputHistory;

    public HUDInputHistory inputHistory;
    
    public void Awake()
    {
        unitAttack = GetComponent<UnitAttack>();
        unitMove = GetComponent<UnitMove>();
    }
    public void Start()
    {
        tryingToBlock = false;
        directionalInputHistory = new List<byte>(7);
        directionalInputHistorySend = new List<byte>(7);
    }
    public void Update()
    {
        //Take direction movement
        directionalInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        //Timer things
        if (timerAttackStick > 0f)
        {
            timerAttackStick -= Time.deltaTime;
        }
        if (timerToResetInputHistory > 0f)
        {
            timerToResetInputHistory -= Time.deltaTime;
        }
        else
        {
            if (directionalInputHistory.Count > 0)
            {
                timerToResetInputHistory = 0f;
                directionalInputHistory.Clear();
            }
        }

        //If 7-9: Jump
        //If 1-3: Crouch and not move
        directionalByte = GetDirectionalByte();
        unitMove.Move(directionalInput, directionalByte);

        //Get Byte Inputs
        if (previousInput != directionalByte)
        {
            
            previousInput = directionalByte;
            if (directionalByte != 5)
            {
                timerToResetInputHistory = 0.12f;
                directionalInputHistory.Add(directionalByte);
                if (directionalInputHistory.Count > 7)
                {
                    directionalInputHistory.RemoveAt(0);
                }
            }
        }
        //Get Attack Inputs
        if (!unitAttack.CurrentlyHit())
        {
            //Punch
            if (Input.GetKeyDown(KeyCode.J))
            {
                if (!ButtonAlreadyPressed(0))
                {
                    attackInput |= 0x1;
                    if (timerAttackStick <= 0f)
                    {
                        timerAttackStick = 0.05f;
                    }
                }
            }
            //Kick
            if (Input.GetKeyDown(KeyCode.K))
            {
                if (!ButtonAlreadyPressed(1))
                {
                    attackInput |= 0x2;
                    if (timerAttackStick <= 0f)
                    {
                        timerAttackStick = 0.05f;
                    }
                }
            }
            //Strike
            if (Input.GetKeyDown(KeyCode.L))
            {
                if (!ButtonAlreadyPressed(2))
                {
                    attackInput |= 0x4;
                    if (timerAttackStick <= 0f)
                    {
                        timerAttackStick = 0.05f;
                    }
                }
            }
            //Grab
            if (Input.GetKeyDown(KeyCode.Semicolon))
            {
                if (!ButtonAlreadyPressed(3))
                {
                    attackInput |= 0x8;
                    if (timerAttackStick <= 0f)
                    {
                        timerAttackStick = 0.05f;
                    }
                }
            }
            //Block
            if (Input.GetKeyDown(KeyCode.I))
            {
                if (!ButtonAlreadyPressed(4))
                {
                    tryingToBlock = true;
                    attackInput |= 0x10;
                    if (timerAttackStick <= 0f)
                    {
                        timerAttackStick = 0.05f;
                    }
                }
            }
            if (Input.GetKeyUp(KeyCode.I))
            {
                tryingToBlock = false;
            }
        }

        //unitAttack.MakeBlock(tryingToBlock, directionalInput);
        if ((timerAttackStick <= 0f) && (attackInput > 0))
        {
            timerAttackStick = 0f;
            RecordInputs();
            unitAttack.MakeAttack(directionalInputHistorySend, attackInputSend);
        }
    }

    /// <summary>
    /// Return if an attack was already pressed.
    /// </summary>
    /// <param name="buttonPressed"></param>
    /// <returns></returns>
    private bool ButtonAlreadyPressed(byte buttonPressed)
    {
        return (attackInput & (0x1 << buttonPressed)) >> buttonPressed == 0x1;
    }

    /// <summary>
    /// Return the correct byte regardless where the player is facing (e.g. if facing Left, holding left will result in 6)
    /// </summary>
    /// <returns></returns>
    private byte GetFlippedByte(byte directionByte)
    {
        //BASE CASE 1: If byte greater than 9, return neutral
        if (directionByte > 9)
        {
            return 5;
        }
        //BASE CASE 1: If already facing right, return the original
        if (Mathf.Sign(transform.localScale.x) >= 0)
        {
            return directionByte;
        }
        else
        {
            //If up, down, or neutral; return the original
            if ((directionByte == 2) || (directionByte == 5) || (directionByte == 8))
            {
                return directionByte;
            }
            else
            {
                //If divisible by 3, subtract 2; otherwise, add 2
                if (directionByte % 3 == 0)
                {
                    return (directionByte -= 2);
                }
                else
                {
                    return directionByte += 2;
                }
            }
        }
    }
    /// <summary>
    /// Return a single numbe based on the directional input.
    /// </summary>
    /// <returns></returns>
    private byte GetDirectionalByte()
    {
        if (directionalInput.x < 0)
        {
            if (directionalInput.y > 0)
            {
                return 7;
            }
            else if (directionalInput.y == 0)
            {
                return 4;
            }
            else
            {
                return 1;
            }
        }
        else if (directionalInput.x == 0)
        {
            if (directionalInput.y > 0)
            {
                return 8;
            }
            else if (directionalInput.y == 0)
            {
                return 5;
            }
            else
            {
                return 2;
            }
        }
        else
        {
            if (directionalInput.y > 0)
            {
                return 9;
            }
            else if (directionalInput.y == 0)
            {
                return 6;
            }
            else
            {
                return 3;
            }
        }
    }
    private void RecordInputs()
    {
        directionalInputHistorySend.Clear();
        for (int i = 0; i < directionalInputHistory.Count; i++)
        {
            //directionalInputHistorySend[i] = directionalInputHistory[i];
            directionalInputHistorySend.Add(directionalInputHistory[i]);
        }
        attackInputSend = attackInput;
        directionalInputHistory.Clear();
        attackInput = 0;
    }
}
