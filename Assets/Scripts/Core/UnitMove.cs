using UnityEngine;

/// <summary>
/// Script in charge of Player movement.
/// </summary>
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(UnitStats))]
[RequireComponent(typeof(UnitAttack))]
[RequireComponent(typeof(UnitLayers))]
[RequireComponent(typeof(Rigidbody2D))]
public class UnitMove : MonoBehaviour
{
    //TODO: If walking back on a ledge, grab onto the ledge
    //TODO: If moving toward a ledge, turn around and grab onto the ledge
    //TODO: If attacked while ledge grabbing, fall down.
    //TODO: If an opponent is falling, prevent other player to move and preset fall to next stage.
    private Rigidbody2D rb2D;
    private new Camera camera;
    private BoxCollider2D box;
    private Animator animator;
    private UnitStats unitStats;
    private UnitLayers unitLayers;
    private UnitAttack unitAttack;
    private Collider2D currentGround;
    private UnitHurtboxHolder unitHurtboxHolder;
    private WallStaggerCheck[] wallStaggerChecks;
    private Vector2 velocity;
    private Vector2 moveSpeed; //X = Walk Speed, Y = Dash Speed
    private bool downed;
    private bool grounded;
    private bool poppedUp; //If true, do not use enemy's velocity.x when being attacked.
    private bool crouching;
    private bool wallStaggerMid;
    private bool wallStaggerHigh;
    private bool ledgeDropCameraFocus;
    private byte ledgeState; //0: Not on a ledge, 1: Ledge Grabbing, 2: Get Up, 3: Falling from Ledge (Fail), 4: Tripping, 5: Falling From Ledge (success)
    private byte currentHeight; //0: lying on ground, 1: crouching, 2: standing, 3: airborne
    private byte wallStaggerCount;
    private byte enemyFellFromLedge;
    private sbyte directionFacing;
    private float velocityRef;
    private float downedTimer;
    private float moveLockTimer;
    private float ledgeGrabTimer;
    private float wallStaggerTimer;
    private float gravityScaleNormal;
    private float gravityScaleJuggle;

    public Transform waistPosition;
    public Transform groundedPosition;
    public LayerMask whatIsGround;
    public Color colorWire;
    public Color colorFill;

    public void Awake()
    {
        unitHurtboxHolder = GetComponentInChildren<UnitHurtboxHolder>();
        wallStaggerChecks = GetComponentsInChildren<WallStaggerCheck>();
        rb2D = GetComponent<Rigidbody2D>();
        box = GetComponent<BoxCollider2D>();
        camera = FindObjectOfType<Camera>();
        animator = GetComponent<Animator>();
        unitStats = GetComponent<UnitStats>();
        unitAttack = GetComponent<UnitAttack>();
        unitLayers = GetComponent<UnitLayers>();
    }
    public void Start()
    {
        moveSpeed = new Vector2(3, 7);
        ledgeState = 0;
        wallStaggerCount = 0;
        gravityScaleNormal = 6;
        gravityScaleJuggle = 6;
        currentGround = GetGround();
        rb2D.gravityScale = gravityScaleNormal;
        SetWallStaggerChecks();
    }
    public void Update()
    {
        grounded = (rb2D.velocity.y <= 0f) &&
            Physics2D.OverlapBox(groundedPosition.position, new Vector2(box.size.x - 0.02f, 0.1f), 0f, whatIsGround);
        currentHeight = grounded ? (byte)(Crouching() ? 1 : 2) : (byte)3;
        directionFacing = (sbyte)Mathf.Sign(transform.localScale.x);

        if (Input.GetKeyDown(KeyCode.H))
        {
            if (LedgeGrabbing())
            {
                LedgeGrabAnimate(2);
            }
        }

        //Auto blocking things
        if (unitAttack.alwaysBlocking == 3)
        {
            crouching = true;
        }
        else
        {
            crouching = false;
        }
        //Timers
        if (wallStaggerTimer > 0f)
        {
            wallStaggerTimer -= Time.deltaTime;
        }
        if (moveLockTimer > 0f)
        {
            moveLockTimer -= Time.deltaTime;
        }
        if (ledgeGrabTimer > 0f)
        {
            ledgeGrabTimer -= Time.deltaTime;
        }
        else
        {
            if (ledgeState == 1)
            {
                LedgeDrop();
            }
        }
        //Being downed
        if (!unitStats.EmptyHealth())
        {
            if (downedTimer > 0f)
            {
                downedTimer -= Time.deltaTime;
                if (downedTimer <= 1.55f)
                {
                    if (ledgeDropCameraFocus)
                    {
                        //change to fallen fighter
                        ledgeDropCameraFocus = false;
                        camera.GetComponent<CameraFollow>().SetFocusByFighter(this);
                    }
                }
                if (downedTimer > 1.2f && downedTimer < 1.5f)
                {
                    FindObjectOfType<GameManager>().OtherPlayerDropLedge(this);
                }
            }
            else
            {
                downedTimer = 0f;
                if (downed)
                {
                    downed = false;
                    animator.SetTrigger("DownedGetUp");
                }
            }
        }
        //Landing from moment after enemy dropping
        if (grounded && (enemyFellFromLedge == 1))
        {
            enemyFellFromLedge = 0;
            camera.GetComponent<CameraFollow>().SetFocusBothFighters();
            FlipSpriteForceProper();
        }
        //Ledge checking
        if (!Physics2D.Raycast(waistPosition.position, Vector2.down, (enemyFellFromLedge > 0) ? 30f : 15f, whatIsGround))
        {
            if (ledgeState == 0)
            {
                bool rightSide = transform.position.x > currentGround.transform.position.x;
                if (grounded)
                {
                    if (rightSide)
                    {
                        if (directionFacing == 1)
                        {
                            LedgeTrip(rightSide);

                        }
                        else
                        {
                            LedgeGrab(rightSide);
                        }
                    }
                    else
                    {
                        if (directionFacing == 1)
                        {
                            LedgeTrip(rightSide);
                        }
                        else
                        {
                            LedgeGrab(rightSide);
                        }
                    }
                }
                else
                {
                    LedgeGrab(rightSide);
                }
            }
        }
        //Wall Staggers
        if (unitAttack.HitAnimKnockback() && (!grounded))
        {
            //Wall Checking
            wallStaggerMid = wallStaggerChecks[0].FoundWall(box, whatIsGround, velocity.x >= 0);
            wallStaggerHigh = wallStaggerChecks[1].FoundWall(box, whatIsGround, velocity.x >= 0);
            if (wallStaggerMid)
            {
                //Found a wall
                if (wallStaggerHigh)
                {
                    RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, Mathf.Infinity, whatIsGround);
                    if (hit.distance >= 0.3f)
                    {
                        animator.SetTrigger("IsHitWallStagger");
                        transform.position = hit.point;
                    }
                }
                else
                {
                    Debug.Log("Found low obstacle. Topple over it.");
                    RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, Mathf.Infinity, whatIsGround);
                    if (hit.distance >= 0.3f)
                    {
                        Debug.Log("Actually topple over");
                        transform.position = hit.point;
                    }
                }
            }
        }

        animator.SetBool("Grounded", grounded);
        animator.SetBool("Crouching", Crouching());
        animator.SetInteger("LedgeState", ledgeState);
        animator.SetFloat("XVelocity", Mathf.Abs(rb2D.velocity.x));
    }

    
    public void FixedUpdate()
    {
        if (grounded)
        {
            rb2D.gravityScale = gravityScaleNormal;
        }
        velocity.y = rb2D.velocity.y;
        velocity.x = Mathf.Clamp(velocity.x, -25f, 25f);
        velocity.y = Mathf.Clamp(velocity.y, -30f, 30f);
        if ((unitAttack.CurrentlyHit() || unitAttack.CurrentlyAttacking()) && grounded)
        {
            velocity = new Vector2(Mathf.SmoothDamp(rb2D.velocity.x, 0f, ref velocityRef, 0.05f), rb2D.velocity.y);
        }
        rb2D.velocity = velocity;
    }
    public void OnDrawGizmos()
    {
        if (box != null)
        {
            if (box != null)
            {
                Gizmos.color = colorFill;
                Gizmos.DrawCube((Vector2)transform.position + box.offset, box.size);
                Gizmos.color = colorWire;
                Gizmos.DrawWireCube((Vector2)transform.position + box.offset, box.size);
            }
        }
    }

    /// <summary>
    /// Move while not attacking.
    /// </summary>
    /// <param name="directionalInput"></param>
    public void Move(Vector2 directionalInput, byte directionalByte)
    {
        if (MoveLocked())
        {
            return;
        }
        //if (ledgeGrabbing)
        if (LedgeGrabbing())
        {
            Debug.Log("Ledge grabbing!!");
            //If directional input toward stage, get into the stage.
            if (directionFacing >= 1)
            {
                if (directionalByte == 6)
                {
                    LedgeGrabAnimate(2);
                    return;
                }
            }
            if (directionFacing <= -1)
            {
                if (directionalByte == 4)
                {
                    LedgeGrabAnimate(2);
                    return;
                }
            }
            return;
        }
        if ((!unitAttack.CurrentlyAttacking()) || (!unitAttack.CurrentlyHit())
            || (!unitAttack.CurrentlyHit()) || (!unitAttack.Blocking()))
        {
            //Can move
            if (grounded)
            {
                if (directionalByte >= 7)
                {
                    crouching = false;
                    Jump(directionalInput.x);
                    return;
                }
                if (directionalByte <= 3)
                {
                    crouching = true;
                    return;
                }
                //Move normally
                crouching = false;
                velocity = new Vector2(moveSpeed.x * directionalInput.x, 0);
            }
        }
    }
    /// <summary>
    /// Make this Unit jump.
    /// </summary>
    /// <param name="directionalInput"></param>
    public void Jump(float directionalInput)
    {
        grounded = false;
        velocity = new Vector2(10 * directionalInput, 20);
        rb2D.velocity = velocity;
    }
    /// <summary>
    /// Make this Unit stop moving.
    /// </summary>
    public void StopMoving()
    {
        velocity = Vector2.zero;
        rb2D.velocity = velocity;
    }
    /// <summary>
    /// Have the player move for a bit
    /// </summary>
    /// <param name="incomingAttack"></param>
    public void AttackMove()
    {
        Vector2 playerMove = unitAttack.AttackToAnimate().PlayerMove();
        velocity = new Vector2(playerMove.x * transform.localScale.x, playerMove.y);
        rb2D.velocity = velocity;
    }
    /// <summary>
    /// Add knockback from an incoming attack.
    /// </summary>
    /// <param name="attackingPositionX"></param>
    /// <param name="incomingAttack"></param>
    public void Knockback(float attackingPositionX, Attack incomingAttack)
    {
        sbyte direction = (attackingPositionX <= transform.position.x) ? (sbyte)1 : (sbyte)-1;
        velocity = incomingAttack.EnemyKnockback() * direction;
        //If grounded, start the initial pop up; else, small air pumps
        if (grounded)
        {
            //Teleport body
            if (velocity.y > 0f)
            {
                transform.position = waistPosition.position;
            }
            if (velocity.y > 0f)
            {
                grounded = false;
                rb2D.gravityScale = gravityScaleJuggle;
            }
        }
        else
        {
            rb2D.gravityScale += 0.5f;
            rb2D.gravityScale = Mathf.Clamp(rb2D.gravityScale, gravityScaleJuggle, gravityScaleNormal);
            velocity.y = 10f;
        }
        rb2D.velocity = velocity;
    }
    /// <summary>
    /// Flip the character sprite to the opposing side.
    /// </summary>
    public void FlipSprite()
    {
        Vector3 temp = transform.localScale;
        transform.localScale = new Vector3(-temp.x, temp.y, temp.z);
    }
    /// <summary>
    /// Flip the character sprite with respect to the opponent.
    /// </summary>
    public void FlipSpriteForceProper()
    {
        UnitMove[] players = FindObjectsOfType<UnitMove>();
        foreach(UnitMove enemy in players)
        {
            if (enemy == this)
            {
                continue;
            }
            if (transform.position.x < enemy.transform.position.x)
            {
                if (directionFacing == -1)
                {
                    FlipSprite();
                }
            }
            else
            {
                if (directionFacing == 1)
                {
                    FlipSprite();
                }
            }
        }
    }
    /// <summary>
    /// Add a wall stagger count when attacked.
    /// </summary>
    public void AnimAddWallStaggerCount()
    {
        Debug.Log("Method called");
        if (CanWallStagger())
        {
            wallStaggerCount++;
            wallStaggerTimer = 0.2f;
        }
    }
    /// <summary>
    /// Animator method to be checking for ledges again.
    /// </summary>
    public void AnimLedgeCheckAgain()
    {
        if (ledgeState >= 3)
        {
            ledgeDropCameraFocus = true;
            unitStats.DamageUnitLedgeDrop();
            FindObjectOfType<GameManager>().OtherPlayerPrepareDropLedge(this);
        }
        downedTimer = 3f;
        ledgeState = 0;
        downed = true;
        currentGround = GetGround();
    }
    public void GetUpFromGround()
    {
        downed = false;
        downedTimer = 0f;
    }
    /// <summary>
    /// Reset the wall stagger count.
    /// </summary>
    public void ResetWallStaggerCount()
    {
        wallStaggerCount = 0;
    }
    /// <summary>
    /// Animator method to push into the stage.
    /// </summary>
    public void LedgeGrabGetUpFinish()
    {
        ledgeState = 0;
        float halfLength = box.size.x;
        transform.position += new Vector3(halfLength * directionFacing, 0, 0);
        rb2D.gravityScale = gravityScaleNormal;
        rb2D.isKinematic = false;
    }
    /// <summary>
    /// Teleport unit from the air to next to the opponent.
    /// </summary>
    /// <param name="opponent"></param>
    public void TeleportToNextFloor(Transform opponent)
    {
        float distance = (transform.position.x < opponent.transform.position.x) ? 7 : -7;
        transform.position = new Vector3(opponent.position.x + distance, transform.position.y, 0);
        ledgeState = 0;
        currentGround = GetGround();
    }
    /// <summary>
    /// Ledge drop this Unit. If over ground, this does nothing.
    /// </summary>
    public void LedgeDrop()
    {
        LedgeGrabAnimate(3);
        bool rightSide = transform.position.x > currentGround.transform.position.x;
        if (rightSide)
        {
            camera.GetComponent<CameraFollow>().SetFocusSpecificPosition(
                new Vector3(currentGround.bounds.max.x, currentGround.transform.localPosition.y - 7f, 0));
        }
        else
        {
            camera.GetComponent<CameraFollow>().SetFocusSpecificPosition(
                new Vector3(currentGround.bounds.min.x, currentGround.transform.localPosition.y - 7f, 0));
        }

    }
    /// <summary>
    /// Make this Unit trip from the ledge. If over ground, this does nothing.
    /// </summary>
    /// <param name="rightSide"></param>
    public void LedgeTrip(bool rightSide)
    {
        //Debug.Log("Trip");
        ledgeState = 4;
        unitLayers.SetHitLayer();
        animator.SetTrigger("LedgeAction");
        if (rightSide)
        {
            camera.GetComponent<CameraFollow>().SetFocusSpecificPosition(
                new Vector3(currentGround.bounds.max.x, currentGround.transform.localPosition.y - 7f, 0));
        }
        else
        {
            camera.GetComponent<CameraFollow>().SetFocusSpecificPosition(
                new Vector3(currentGround.bounds.min.x, currentGround.transform.localPosition.y - 7f, 0));
        }
    }
    /// <summary>
    /// Move the Unit to the specified position in front of the enemy.
    /// </summary>
    /// <param name="enemy"></param>
    public void SetDropToBelow(Transform enemy)
    {
        if (enemyFellFromLedge > 1)
        {
            enemyFellFromLedge = 1;
            if (enemy.position.x < transform.position.x)
            {
                transform.position = new Vector3(enemy.position.x - 5f, transform.position.y, transform.position.z);
            }
            else
            {
                transform.position = new Vector3(enemy.position.x + 5f, transform.position.y, transform.position.z);
            }
        }
    }
    /// <summary>
    /// Set if the opponent has dropped the ledge.
    /// </summary>
    public void SetEnemyFellFromLedge()
    {
        enemyFellFromLedge = 2;
    }
    /// <summary>
    /// Does this Unit completely evade high attacks? Only works if Unit is blocking low and crouching.
    /// </summary>
    /// <param name="incomingAttackHeight"></param>
    /// <returns></returns>
    public bool AttackHighEvaded(int incomingAttackHeight)
    {
        return (crouching || ((ledgeState < 3) && (ledgeState > 0))) && (incomingAttackHeight == 3);
    }
    /// <summary>
    /// Has the Unit been staggered to the wall at least twice? If so, no more attacks should happen.
    /// </summary>
    /// <returns></returns>
    public bool WallStaggeredMaxReached()
    {
        return wallStaggerCount >= 2;
    }
    /// <summary>
    /// Can this Unit be staggered.
    /// </summary>
    /// <returns></returns>
    public bool CanWallStagger()
    {
        return wallStaggerTimer <= 0f;
    }
    /// <summary>
    /// Is the Unit on the ground?
    /// </summary>
    /// <returns></returns>
    public bool Grounded()
    {
        return grounded;
    }
    /// <summary>
    /// Is the Unit crouching?
    /// </summary>
    /// <returns></returns>
    public bool Crouching()
    {
        return crouching && grounded;
    }
    /// <summary>
    /// Is the Unit ledge grabbing?
    /// </summary>
    /// <returns></returns>
    public bool LedgeGrabbing()
    {
        return ledgeState == 1;
    }
    /// <summary>
    /// Is the Unit getting up from the ledge?
    /// </summary>
    /// <returns></returns>
    public bool LedgeGetUp()
    {
        return ledgeState == 2;
    }
    /// <summary>
    /// Is the Unit falling from the ledge?
    /// </summary>
    /// <returns></returns>
    public bool FallingFromLedge()
    {
        return ledgeState == 3;
    }
    /// <summary>
    /// Is the Unit doing anything on the ledge? Can be any state.
    /// </summary>
    /// <returns></returns>
    public bool LedgeThings()
    {
        return ledgeState > 0;
    }
    /// <summary>
    /// Return the Unit's current height.
    /// </summary>
    /// <returns></returns>
    public byte CurrentHeight()
    {
        return currentHeight;
    }

    private Collider2D GetGround()
    {
        Collider2D hit = Physics2D.Raycast(waistPosition.position, Vector2.down, Mathf.Infinity, whatIsGround).collider;
        return hit;
    }
    /// <summary>
    /// Have the unit hug the ledge. Flip sprite if in air.
    /// </summary>
    /// <param name="rightSide"></param>
    private void LedgeGrab(bool rightSide)
    {
        Vector2 ledgePosition = rightSide ? new Vector2(currentGround.bounds.max.x, currentGround.bounds.max.y) :
            new Vector2(currentGround.bounds.min.x, currentGround.bounds.max.y);
        transform.position = ledgePosition;
        if (rightSide)
        {
            if (directionFacing == 1)
            {
                FlipSprite();
            }
        }
        else
        {
            if (directionFacing == 1)
            {
                FlipSprite();
            }
        }
        velocity = Vector2.zero;
        rb2D.velocity = velocity;
        LedgeGrabAnimate(1);
    }
    /// <summary>
    /// Animate a ledge grab.
    /// </summary>
    /// <param name="state"></param>
    private void LedgeGrabAnimate(byte state)
    {
        ledgeState = state;
        if ((state < 3) && (state >= 1))
        {
            unitLayers.SetMovementLayer();
            if (state == 1)
            {
                rb2D.isKinematic = true;
                rb2D.gravityScale = 0;
                moveLockTimer = 0.2f;
                ledgeGrabTimer = 3f;
            }
            else if (state == 2)
            {
                rb2D.gravityScale = gravityScaleNormal;
            }
        }
        else
        {
            unitLayers.SetHitLayer();
            rb2D.gravityScale = gravityScaleNormal;
            rb2D.isKinematic = false;
            float halfLength = box.size.x;
            transform.position -= new Vector3(halfLength * Mathf.Sign(transform.localScale.x), 0, 0);
        }
        animator.SetTrigger("LedgeAction");
    }
    /// <summary>
    /// Set the wall checking for staggering.
    /// </summary>
    private void SetWallStaggerChecks()
    {
        if (wallStaggerChecks == null)
        {
            Debug.LogError("ERROR: Did not find any wall stagger checks.");
            return;
        }
        for(int i = 0; i < wallStaggerChecks.Length; i++)
        {
            wallStaggerChecks[i].SetHighCheck(i == 1);
            wallStaggerChecks[i].transform.localPosition = new Vector3(0, (i == 1) ? 3f : 1.5f, 0);
        }
    }
    /// <summary>
    /// Is the Unit locked from being able to move?
    /// </summary>
    /// <returns></returns>
    private bool MoveLocked()
    {
        return moveLockTimer > 0f;
    }
}
