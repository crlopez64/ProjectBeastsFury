using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script in charge of Attacking.
/// </summary>
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(UnitMove))]
[RequireComponent(typeof(UnitLayers))]
public class UnitAttack : MonoBehaviour
{
    private StringBuilder builder = new StringBuilder();
    private Animator animator;
    private UnitMove unitMove;
    private UnitStats unitStats;
    private UnitLayers unitLayers;
    private UnitHitboxHolder unitHitboxHolder;
    private Queue<int> attacksBuffered;  //The next attacks buffered in.
    private Attack rootAttack;
    private Attack attackToBuffer;  //The current attack for the attack buffer.
    private Attack attackToAnimate;
    private bool autoBlock;
    private bool blockCancel; //If true, cannot input any more attacks if in the middle of a String traversal.
    private byte blocking; //0: Not blocking, 1: Blocking High Standing, 2: Blocking Low Standing, 3: Blocking Low Crouching
    private byte hitsAllowed;
    private byte currentlyHit; //Used for movements if hit, Hit or Block. 0: Not stunned, 1: Hit Stunned, 2: Block Stunned
    private byte hitGroundAnim; //Used for getting hit animations
    /// <summary>
    /// The estimated frame that an attack is in. On starting an attack animation, this resets to zero.
    /// </summary>
    private byte attackFrameIndex;
    private byte currentAttackFrameType; //0(None), 1(Startup), 2(Active), 3(Recovery)
    private int isHitFrames;
    /// <summary>
    /// THe countdown timer before incrementing the attack frame index.
    /// </summary>
    private float frameTimer;
    private float comboScaling; //P(-0.04f), K(-0.07f), S(-0.1f)
    /// <summary>
    /// The duration a Unit will be getting hit or block animation.
    /// </summary>
    private float isHitAnimTimer;
    /// <summary>
    /// The estimated time for one frame in 60FPS time.
    /// </summary>
    public const float OneFrameTime = 1 / 60f;

    [Range(0, 3)]
    public byte alwaysBlocking; //0: None, 1: Block High, 2: Block Low Standing, 3: Block Low Crouching
    public TextAsset moveList;
    public GameObject particleHit;
    public Transform wallStaggerCheckHigh;
    public Transform wallStaggerCheckMid;
    public LayerMask whatIsWall;

    public void Awake()
    {
        unitHitboxHolder = GetComponentInChildren<UnitHitboxHolder>();
        animator = GetComponent<Animator>();
        unitMove = GetComponent<UnitMove>();
        unitStats = GetComponent<UnitStats>();
        unitLayers = GetComponent<UnitLayers>();
        UnitHelperCreatMoveList moveListHelper = new UnitHelperCreatMoveList();
        rootAttack = moveListHelper.CreateMoveList(moveList);
        attackToBuffer = rootAttack;
        attackToAnimate = rootAttack;
    }
    public void Start()
    {
        attacksBuffered = new Queue<int>(7);
        currentlyHit = 0;
        hitsAllowed = 0;
        attackFrameIndex = 0;
        frameTimer = 0.0f;
        comboScaling = 1.0f;
        isHitAnimTimer = 0.0f;
    }
    public void Update()
    {
        //Always blocking things.
        if (alwaysBlocking > 0)
        {
            autoBlock = true;
            unitLayers.SetHitLayer();
            blocking = alwaysBlocking;
        }
        else
        {
            if (autoBlock)
            {
                autoBlock = false;
                blocking = 0;
                unitLayers.SetMovementLayer();
            }
        }

        //Frames and Timers
        if (CurrentlyAttacking())
            {
            if (frameTimer > 0f)
            {
                frameTimer -= Time.deltaTime;
            }
            else
            {
                attackFrameIndex++;
                frameTimer = OneFrameTime;
            }
        }
        if (isHitAnimTimer > 0f)
        {
            isHitAnimTimer -= Time.deltaTime;
        }
        isHitFrames = (int)(isHitAnimTimer / OneFrameTime);

        //Animator
        animator.SetBool("Attacking", CurrentlyAttacking());
        animator.SetInteger("Blocking", blocking);
        animator.SetInteger("HitFrames", isHitFrames);
        animator.SetInteger("HitGroundAnim", hitGroundAnim);
        animator.SetInteger("AttackAnim", attackToAnimate.AttackAnimation());
    }

    /// <summary>
    /// Activate 1 frame's worth of Hitboxes.
    /// </summary>
    public void HitboxActive()
    {
        unitHitboxHolder.ActivateHitboxes();
    }
    /// <summary>
    /// Determine if the Unit is trying to block or not. Completely ignore if attacking, grounded, in blockstunned, or otherwise.
    /// </summary>
    /// <param name="tryingToBlock"></param>
    /// <param name="directionalInput"></param>
    public void MakeBlock(bool tryingToBlock, Vector2 directionalInput)
    {
        //TODO: Constantly calling method. Maybe call it off whenever it's not needed?

        //BASE CASE: If not on the ground, currently attacking, or hitstunned, call off blocking and stop the function.
        if ((!unitMove.Grounded()) || CurrentlyAttacking() || Hitstunned() || unitMove.LedgeThings())
        {
            blocking = 0;
            return;
        }
        //BASE CASE: If blockstunned, stop the function, blocking may still occur.
        if (Blockstunned())
        {
            return;
        }
        //Check if blocking.
        if (tryingToBlock)
        {
            //If trying to jump, ignore blocking
            
            if (directionalInput.y > 0f)
            {
                unitLayers.SetMovementLayer();
                blocking = 0;
            }
            else if (directionalInput.y < 0f)
            {
                unitLayers.SetHitLayer();
                blocking = Blockstunned() ? (byte)2 : (byte)3;
            }
            else
            {
                //Standing block
                unitLayers.SetHitLayer();
                blocking = 1;
            }
        }
        else
        {
            if (!Blockstunned())
            {
                unitLayers.SetMovementLayer();
                blocking = 0;
            }
        }
    }
    /// <summary>
    /// Make the Unit attack.
    /// </summary>
    /// <param name="movementInput"></param>
    /// <param name="attackInput"></param>
    public void MakeAttack(List<byte> movementInput, byte attackInput)
    {
        //String check
        //Debug.Log("Check to see if attack is valid in the move list.");
        //builder.Clear();
        //foreach (byte input in movementInput)
        //{
        //    builder.Append(input);
        //}
        //builder.Append("-" + attackInput);
        //Debug.Log("Input made: " + builder.ToString());

        //BASE CASE: If blocking, Ledge Falling, or Ledge Get Up, ignore the input.
        if (Blocking() || unitMove.LedgeGetUp() || unitMove.FallingFromLedge())
        {
            return;
        }
        //BASE CASE: If attack is already at ender, ignore the input
        if (!attackToBuffer.HasNextInString())
        {
            Debug.Log("Attack is ender; Current Attack to Buffer: " + attackToBuffer.GetAttackInputVersionDebugger() +
                ", NextInString: " + attackToBuffer.GetNextInStringCount());
            return;
        }
        //BASE CASE: If pressed blocked, cannot Make any more attacks until end of animation
        if (blockCancel)
        {
            Debug.Log("Decided to cancel string with Block. Wait until animation finish.");
            return;
        }
        //BASE CASE: If ledge grabbing, make a valid attack when getting up.
        //Check if attack is valid
        if (!MakeAttackHelper(movementInput, attackInput))
        {
            //If attack was not valid, remove to the last movement input, then try again
            //Debug.Log("Did not find attack. Time to reduce movement count");
            if (movementInput.Count > 1)
            {
                byte lastInput = movementInput[movementInput.Count - 1];
                movementInput = new List<byte>(1) { lastInput };
            }
            else
            {
                movementInput = null;
            }
            if (!MakeAttackHelper(movementInput, attackInput))
            {
                //If still not valid, and had a movement, only consider the last input
                if (movementInput != null)
                {
                    //Debug.Log("Still did not find attack. Time to reduce movement count");
                    if (!MakeAttackHelper(null, attackInput))
                    {
                        Debug.Log("Did not find attack to enqueue. Is normal.");
                    }
                }
            }
        }
    }
    /// <summary>
    /// Check to see if an Attack is valid.
    /// </summary>
    private bool MakeAttackHelper(List<byte> movementInput, byte attackInput)
    {
        List<Attack> nextInString = attackToBuffer.GetNextInString();
        for(int i = attackToBuffer.GetNextInStringCount() - 1; i >= 0; i--)
        {
            if (nextInString[i].InputMatch(movementInput, attackInput, unitMove.CurrentHeight()))
            {
                if (CurrentlyAttacking())
                {
                    //Debug.Log("Correct attack: Enqueuing. Attack Input: " + nextInString[i].GetAttackInputVersionDebugger()
                    //    + "; Attack Anim: " + nextInString[i].AttackAnimation());
                    attacksBuffered.Enqueue(i);
                    attackToBuffer = attackToBuffer.GetNextAttack(i);
                }
                else
                {
                    //Debug.Log("Correct attack: Initial attack. Attack Input: " + nextInString[i].GetAttackInputVersionDebugger()
                    //    + "; Attack Anim: " + nextInString[i].AttackAnimation());
                    attackToBuffer = attackToBuffer.GetNextAttack(i);
                    attackToAnimate = attackToAnimate.GetNextAttack(i);
                    hitsAllowed = attackToAnimate.HitsAllowed();
                    unitLayers.SetAttackLayer();
                    AnimAttackStartup();
                    animator.SetTrigger("NextAttack");
                }
                return true;
            }
        }
        return false;
    }
    /// <summary>
    /// Reset everything.
    /// </summary>
    public void ResetAll()
    {
        unitLayers.SetMovementLayer();
        attacksBuffered.Clear();
        attackToAnimate = rootAttack;
        attackToBuffer = rootAttack;
        currentlyHit = 0;
        blockCancel = false;
        comboScaling = 1f;
        currentAttackFrameType = 0;
    }
    /// <summary>
    /// Reset all buffering and all Attack placements. Set IsHit duration as well. If true, IsHit anim plays; else, Blocking anim plays.
    /// </summary>
    public void HasBeenHit(Attack incomingAttack, byte attackFrameIndex, float incomingAttackAnimationLength, bool hitOrBlock)
    {
        currentlyHit = hitOrBlock ? (byte)1 : (byte)2;
        blockCancel = false;
        hitGroundAnim = incomingAttack.EnemyHitAnimation();
        attacksBuffered.Clear();
        attackToAnimate = rootAttack;
        attackToBuffer = rootAttack;
        hitsAllowed = 0;
        currentAttackFrameType = 0;

        //Is Hit duration
        isHitAnimTimer = incomingAttackAnimationLength - (attackFrameIndex * OneFrameTime) + ((-incomingAttack.PlusMinusOnHit()) * OneFrameTime);
        if (isHitAnimTimer < OneFrameTime)
        {
            isHitAnimTimer = OneFrameTime;
        }
        //Animations
        if (hitOrBlock)
        {
            currentlyHit = 1;
            unitStats.DamageUnit(incomingAttack.DamageOnHit(), true);
            animator.SetTrigger("IsHit");
        }
        else
        {
            currentlyHit = 2;
            unitStats.DamageUnit(incomingAttack.DamageOnBlock(), false);
            animator.SetTrigger("IsHitBlocking");
        }
        unitLayers.SetHitLayer();
    }
    public void TurnOffHitLayer()
    {
        hitGroundAnim = 0;
        currentlyHit = 0;
        unitMove.ResetWallStaggerCount();
        unitMove.StopMoving();
        unitStats.ResetOpponentComboCounter();
        //Set move to true
        if (!Blocking())
        {
            unitLayers.SetMovementLayer();
        }
    }
    /// <summary>
    /// Rest if animator reaches an Ender or Player does a Block Cancel.
    /// </summary>
    public void ResetEnderOrBlockCancel()
    {
        hitsAllowed = 0;
        currentAttackFrameType = 0;
        unitLayers.SetMovementLayer();
        attackToBuffer = rootAttack;
        attackToAnimate = rootAttack;
        attacksBuffered.Clear();
        blockCancel = false;
    }
    /// <summary>
    /// Make the Unit move on move.
    /// </summary>
    public void AttackMove()
    {
        unitMove.AttackMove();
    }
    /// <summary>
    /// Play the next attack.
    /// </summary>
    public void PlayNextAttack()
    {
        //if (!Stunned())
        if (!CurrentlyHit())
        {
            if (attacksBuffered.Count > 0)
            {
                //Debug.Log("Play next attack for player");
                attackToAnimate = attackToAnimate.GetNextAttack(attacksBuffered.Dequeue());
                hitsAllowed = attackToAnimate.HitsAllowed();
                AnimAttackStartup();
                animator.SetTrigger("NextAttack");
            }
            else
            {
                //Debug.Log("Reset attack things.");
                ResetEnderOrBlockCancel();
            }
        }
    }
    /// <summary>
    /// Set up any pre-attack things for animation.
    /// </summary>
    public void AnimAttackStartup()
    {
        currentAttackFrameType = 1;
        attackFrameIndex = 0;
        frameTimer = OneFrameTime;

    }
    /// <summary>
    /// If the attack lands, do things.
    /// </summary>
    public void AnimAttackActive()
    { 
        currentAttackFrameType = 2;
        unitHitboxHolder.ActivateHitboxes();
    }
    /// <summary>
    /// Set up any after-attack things for animation.
    /// </summary>
    public void AnimAttackRecovery()
    {
        currentAttackFrameType = 3;
    }
    /// <summary>
    /// Remove 1 hit from the available hits to use.
    /// </summary>
    public void UsedHit()
    {
        if (hitsAllowed > 0)
        {
            hitsAllowed--;
        }
    }
    /// <summary>
    /// Is the Unit blocking? 0: Not blocking, 1: Blocking High Standing, 2: Blocking Low Standing, 3: Blocking Low Crouching
    /// </summary>
    /// <returns></returns>
    public bool Blocking()
    {
        return blocking > 0;
    }
    /// <summary>
    /// Is the animation state having the Unit knockback?
    /// </summary>
    /// <returns></returns>
    public bool HitAnimKnockback()
    {
        return hitGroundAnim >= 3;
    }
    /// <summary>
    /// If there is an incoming attack, does the block succeed? Does not consider evading attacks.
    /// </summary>
    /// <param name="incomingAttackHeight"></param>
    /// <returns></returns>
    public bool BlockSucceeded(int incomingAttackHeight)
    {
        if (!Blocking())
        {
            return false;
        }
        if (blocking == 1) //Blocking High
        {
            switch(incomingAttackHeight)
            {
                case 1: //Low: Get hit
                    return false;
                case 2: //Mid: Blocked
                    return true;
                case 3: //High: Blocked
                    return true;
                case 4: //Overhead: Blocked
                    return true;
                default:
                    return false;
            }
        }
        else if (blocking == 2) //Blocking Low Standing
        {
            switch (incomingAttackHeight)
            {
                case 1: //Low: Blocked
                    return true;
                case 2: //Mid: Blocked
                    return true;
                case 3: //High: Get hit
                    return false;
                case 4: //Overhead: Get hit
                    return false;
                default:
                    return false;
            }
        }
        else if (blocking == 3) //Blocking Low Crouching
        {
            switch (incomingAttackHeight)
            {
                case 1: //Low: Blocked
                    return true;
                case 2: //Mid: Blocked
                    return true;
                case 3: //High: Evaded
                    return false;
                case 4: //Overhead: Get hit
                    return false;
                default:
                    return false;
            }
        }
        else
        {
            return false;
        }
    }
    /// <summary>
    /// Does this Unit completely evade high attacks? Only works if Unit is blocking low and crouching.
    /// </summary>
    /// <param name="incomingAttackHeight"></param>
    /// <returns></returns>
    public bool AttackHighEvaded(int incomingAttackHeight)
    {
        return (blocking == 3) && (incomingAttackHeight == 3);
    }
    /// <summary>
    /// Is this Unit currently attacking?
    /// </summary>
    /// <returns></returns>
    public bool CurrentlyAttacking()
    {
        return attackToAnimate != rootAttack;
    }
    /// <summary>
    /// Is the Unit hit?
    /// </summary>
    /// <returns></returns>
    public bool CurrentlyHit()
    {
        return currentlyHit > 0;
    }
    /// <summary>
    /// Is the Unit stunned from getting hit?
    /// </summary>
    /// <returns></returns>
    public bool Hitstunned()
    {
        return currentlyHit == 1;
    }
    /// <summary>
    /// Is the Unit stunned from blocking?
    /// </summary>
    /// <returns></returns>
    public bool Blockstunned()
    {
        return currentlyHit == 2;
    }
    /// <summary>
    /// Can the Unit still attack with the current active frame?
    /// </summary>
    /// <returns></returns>
    public bool CanStillHit()
    {
        return hitsAllowed > 0;
    }
    /// <summary>
    /// Return the current estimated Attack Frame index.
    /// </summary>
    /// <returns></returns>
    public byte AttackFrameIndex()
    {
        return attackFrameIndex;
    }
    /// <summary>
    /// Return what type of Frame the attack is currently at (None, Startup, Active, or Recovery).
    /// </summary>
    /// <returns></returns>
    public byte CurrentAttackFrameType()
    {
        return currentAttackFrameType;
    }
    /// <summary>
    /// Return the attack animation duration.
    /// </summary>
    /// <returns></returns>
    public float CurrentAttackAnimationDuration()
    {
        AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(animator.layerCount - 1);
        return clipInfo[0].clip.length;
    }
    /// <summary>
    /// Returns the Unit's null Root Attack.
    /// </summary>
    /// <returns></returns>
    public Attack RootAttack()
    {
        return rootAttack;
    }
    /// <summary>
    /// The current attack animating.
    /// </summary>
    /// <returns></returns>
    public Attack AttackToAnimate()
    {
        return attackToAnimate;
    }
    
}
