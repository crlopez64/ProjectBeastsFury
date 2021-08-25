using UnityEngine;

/// <summary>
/// Script specific to Player Movement Input only.
/// </summary>
[RequireComponent(typeof(PlayerMotor))]
public class PlayerController : MonoBehaviour
{
    private Animator animator;
    //TODO: Change Player Physical Collision to a seperate layer than "Player"
    private PlayerWeaponControls playerWeaponControls;
    private PlayerMotor playerMotor;
    private Vector2 directionalInput;
    private Vector3 rotation;
    private Vector3 velocity;
    private Vector2 mouseRotation;
    private Vector3 rightMovement;
    private Vector3 cameraRotation;
    private Vector3 wallCheckNormal;
    private Vector3 forwardMovement;
    private Vector3 rightMovementRef;
    private Vector3 forwardMovementRef;
    private Vector3 commandMoveVelocity;
    private bool walled;
    private bool canMove;
    private bool grounded;
    private bool acrobatic; //Augment
    private bool stimulant; //Augment
    private bool canSprint;
    private bool doubleTime; //Augment
    private bool isCrouching;
    private bool isSprinting;
    private bool longJumping;
    private int wallJumps;
    private float speedUse;
    private float stunTimer;
    private float unitVertical;
    /// <summary>
    /// If greater than zero, the valid time in which the Unit can long jump.
    /// </summary>
    private float canLongJumpTimer;
    private float bunnyHopTimer;
    private float wallJumpTimer;
    private float unitHorizontal;
    private float cameraRotationX;
    private float velocityRefInputX;
    private float velocityRefInputZ;
    private float bunnyHopMultiplier = 1f;
    private float wallJumpMovePenalty;
    private float longJumpPenaltyTimer;
    private float slopeForce = 7f;
    private float slopeForceRayLength = 3f;

    public GameObject physicalCollider; //MAY HAVE TO WAIT UNTIL LATER
    //NOTE: Not in use ATM
    public LayerMask whatIsGround;
    public Transform groundCheck;
    [Range(5, 20)]
    public float speed; //Should be sprinting speed
    public float lookSensitivity = 3f;

    //TODO: Add controller support eventually
    private void Awake()
    {
        animator = GetComponent<Animator>();
        playerWeaponControls = GetComponent<PlayerWeaponControls>();
        playerMotor = GetComponent<PlayerMotor>();
    }
    private void Start()
    {
        canMove = true;
        speedUse = speed;
        wallJumps = 2;
	}
	private void Update()
    {
        //Number setting
        directionalInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        mouseRotation = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
        if (grounded)
        {
            wallJumpTimer = 0;
            if (longJumping)
            {
                longJumping = false;
                if (!ExhaustedWallJumps())
                {
                    longJumpPenaltyTimer = 1f;
                }
            }
            wallJumps = 2;
        }
        if (!Stunned())
        {
            if (!longJumping && !LongJumpPenalty())
            {
                if (isSprinting && !isCrouching)
                {
                    //Sprinting Speed
                    speedUse = speed * 1f;
                }
                else if (!isSprinting && isCrouching)
                {
                    //Crouching Speed
                    speedUse = speed * 0.625f;
                }
                else if (isSprinting && isCrouching)
                {
                    //Crouching Speed
                    speedUse = speed * 0.625f;
                }
                else
                {
                    //Actual "Normal" Speed
                    speedUse = speed * 0.66f;
                }
                if (directionalInput.y < 0)
                {
                    //Moving backwards
                    speedUse = speed * 0.594f;
                }
            }
            else if (longJumping && !LongJumpPenalty())
            {
                //Long Jump Speed
                speedUse = speed * 2.35f;
            }
            else if (!longJumping && LongJumpPenalty())
            {
                //Long Jump Penalty Speed
                speedUse = speed * 0.6f;
            }
        }
        else
        {
            speedUse = speed * 0.25f;
        }
        if (canLongJumpTimer > 0f)
        {
            canLongJumpTimer -= Time.deltaTime;
        }
        if (wallJumpMovePenalty > 0f)
        {
            wallJumpMovePenalty -= Time.deltaTime;
        }
        if (longJumpPenaltyTimer > 0f)
        {
            longJumpPenaltyTimer -= Time.deltaTime;
        }
        if (wallJumpTimer > 0f)
        {
            wallJumpTimer -= Time.deltaTime;
        }
        if (BunnyHopIntent())
        {
            bunnyHopTimer -= Time.deltaTime;
        }
        else
        {
            if (bunnyHopMultiplier <= 1f)
            {
                bunnyHopMultiplier += (Time.deltaTime * 0.4f);
                if (bunnyHopMultiplier > 1f)
                {
                    bunnyHopMultiplier = 1f;
                }
            }
            
        }
        //----------//
        // CONTROLS //
        //----------//
        //TODO: Actually implement crouching
        //if (Input.GetKey(KeyCode.C))
        //{
        //    isCrouching = true;
        //    longJumpTimer = 0.15f;
        //}
        //if (Input.GetKeyUp(KeyCode.C))
        //{
        //    isCrouching = false;
        //    longJumpTimer = 0f;
        //}
        //SPRINTING
        //if (Input.GetKey(KeyCode.LeftShift) && grounded && (directionalInput.x == 0) && (directionalInput.y > 0))
        if (Input.GetKey(KeyCode.LeftShift) && grounded && (directionalInput.y > 0))
        {
            isSprinting = true;
            if (playerWeaponControls.CurrentlyReloading() && !doubleTime)
            {
                playerWeaponControls.StopReloading();
            }
        }
        //TRYING TO SPRINT WHILE MOVING ELSEWHERE
        //if (Input.GetKeyUp(KeyCode.LeftShift) || (!grounded) || (directionalInput.x != 0) || (directionalInput.y < 0))
        if (Input.GetKeyUp(KeyCode.LeftShift) || (!grounded) || (directionalInput.y <= 0) || (directionalInput == Vector2.zero))
        {
            isSprinting = false;
            //set the weapon ready after sprinting
            playerWeaponControls.StopReloading();
        }
        //JUMPING
        if (Input.GetKeyDown(KeyCode.Space) && grounded)
        {
            grounded = false;
            //BUNNY HOPPING INFLUENCE
            if (BunnyHopIntent())
            {
                bunnyHopMultiplier -= 0.07f;
                if (bunnyHopMultiplier < 0.7f)
                {
                    bunnyHopMultiplier = 0.7f;
                }
            }
            bunnyHopTimer = 0.8f;
            //LONG JUMPING
            if ((CanLongJump() && isSprinting && (directionalInput.y > 0)) || (acrobatic && CanLongJump()))
            {
                longJumping = true;
            }
            playerMotor.Jump(bunnyHopMultiplier);
        }
        else if (Input.GetKeyDown(KeyCode.Space) && !grounded && walled && (wallJumps > 0))
        {
            //WALL JUMPING
            wallJumps--;
            wallJumpTimer = 0.25f;
            wallJumpMovePenalty = 0.25f;
            Vector3 movingVector = new Vector3(unitHorizontal, 0, unitVertical).normalized;
            Vector3 correctDirection = (movingVector == Vector3.zero) ?
                wallCheckNormal : transform.rotation * new Vector3(unitHorizontal, 0, unitVertical).normalized;
            float angle = Vector3.Angle(correctDirection, wallCheckNormal);
            if ((angle < 178) && (angle > 90))
            {
                correctDirection = Vector3.Reflect(correctDirection, wallCheckNormal);
            }
            Vector3 direction = (wallCheckNormal + correctDirection).normalized;
            //PREDICTING VECTOR
            Debug.DrawRay(transform.position, movingVector, Color.magenta, 1.3f);
            Debug.DrawRay(transform.position, correctDirection, Color.cyan, 1.3f);
            //RESULTING VECTOR
            //Debug.DrawRay(transform.position, (transform.forward * unitVertical) + (transform.right * unitHorizontal), Color.red, 1.3f);
            //Debug.DrawRay(transform.position, new Vector3(unitHorizontal, 0, unitVertical), Color.magenta, 2f);
            GetComponent<Rigidbody>().AddForce(correctDirection * 20, ForceMode.VelocityChange);
            playerMotor.Jump(0.85f);
            walled = false;
        }
        //Make the Player Motor move
        unitVertical = grounded ? directionalInput.y : Mathf.SmoothDamp(unitVertical, directionalInput.y, ref velocityRefInputZ, 0.2f);
        unitHorizontal = grounded ? directionalInput.x : Mathf.SmoothDamp(unitHorizontal, directionalInput.x, ref velocityRefInputX, 0.2f);
        forwardMovement = WallJumping() ? Vector3.SmoothDamp(forwardMovement, new Vector3(0, 0, unitVertical), ref forwardMovementRef, 0.3f) :
            Vector3.SmoothDamp(forwardMovement, transform.forward * unitVertical, ref forwardMovementRef, 0.08f);
        rightMovement = WallJumping() ? Vector3.SmoothDamp(rightMovement, new Vector3(0, 0, unitHorizontal), ref rightMovementRef, 0.3f) :
            Vector3.SmoothDamp(rightMovement, transform.right * unitHorizontal, ref rightMovementRef, 0.08f);
        rotation = new Vector3(0f, mouseRotation.x, 0f) * lookSensitivity;
        cameraRotationX = mouseRotation.y * lookSensitivity;
        velocity = (forwardMovement + rightMovement) * speedUse;
        if (canMove)
        {
            //TODO: Add additional gravity when on slope. Jumping should not be affected when on these slopes
            playerMotor.Move(velocity + (Vector3.down * 5f));
        }
        else
        {
            playerMotor.Move(Vector3.zero);
        }
        playerMotor.Rotate(rotation);
        playerMotor.RotateCamera(cameraRotationX);

        //Animator
        //animator.SetBool("Sprinting", isSprinting);
	}
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 10)
        {
            Vector3 normal = collision.contacts[0].normal;
            if (normal.y > 0.3f)
            {
                grounded = true;
            }
        }
    }
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.layer == 10)
        {
            Vector3 normal = collision.contacts[0].normal;
            Debug.DrawRay(transform.position, normal, Color.red, 1.3f);
            if (normal.y < 0.35f)
            {
                walled = true;
                wallCheckNormal = normal;
            }
            if (normal.y >= 0.35f)
            {
                grounded = true;
                walled = false;
            }
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.layer == 10)
        {
            grounded = false;
        }
    }
    
    /// <summary>
    /// Set if the Unit is crouching.
    /// </summary>
    /// <param name="tOrF"></param>
    public void SetCrouch(bool tOrF)
    {
        isCrouching = tOrF;
        canLongJumpTimer = tOrF ? 0.15f : 0f;
    }
    public void Jump()
    {
        if (grounded)
        {
            //Normal jump or long jump
            grounded = false;
            if (BunnyHopIntent())
            {
                bunnyHopMultiplier -= 0.07f;
                bunnyHopMultiplier = (bunnyHopMultiplier < 0.7f) ? 0.7f : bunnyHopMultiplier;
            }
            bunnyHopTimer = 0.8f;
            if (CanLongJump01())
            {
                //Long Jump
                //Long Jumping should not increase height of jump, only move forward a greater distance

            }
        }
        else
        {
            //Wall jump
        }
    }
    public void Stun(float timer)
    {
        stunTimer = stimulant ? (timer * 0.7f) : timer;
    }
    public void SetCanMove(bool tOrF)
    {
        canMove = tOrF;
    }
    public void SetAcrobat(bool tOrF)
    {
        acrobatic = tOrF;
    }
    public void SetStimulant(bool tOrF)
    {
        stimulant = tOrF;
    }
    public void SetDoubleTime(bool tOrF)
    {
        doubleTime = tOrF;
    }
    /// <summary>
    /// Can the Unit wall jump?
    /// </summary>
    /// <returns></returns>
    public bool CanWallJump()
    {
        return (!grounded) && walled && (wallJumps > 0);
    }
    /// <summary>
    /// Can the Unit long jump?
    /// </summary>
    /// <returns></returns>
    public bool CanLongJump01()
    {
        //return ((!Stunned()) && (canLongJumpTimer > 0f) && isSprinting && (directionalInput.y > 0))
        //    || (acrobatic && (!Stunned()) && (canLongJumpTimer > 0f));
        return ((!Stunned()) && (canLongJumpTimer > 0f) && (directionalInput.y > 0))
            || (acrobatic && (!Stunned()) && (canLongJumpTimer > 0f));
    }
    /// <summary>
    /// Is the Unit stunned? If yes, then Unit cannot long jump, wall jump, or sprint; and have reduced run speed.
    /// </summary>
    /// <returns></returns>
    public bool Stunned()
    {
        return stunTimer > 0f;
    }
    public bool GetGrounded()
    {
        return grounded;
    }
    public bool BunnyHopIntent()
    {
        return bunnyHopTimer > 0f;
    }
    public bool Sprinting()
    {
        return isSprinting;
    }
    private bool CanLongJump()
    {
        return (!Stunned()) && (canLongJumpTimer > 0f);
    }
    private bool WallJumping()
    {
        return wallJumpTimer > 0f;
    }
    private bool LongJumpPenalty()
    { return longJumpPenaltyTimer > 0f;
    }
    private bool ExhaustedWallJumps()
    {
        return wallJumps <= 0f;
    }
}
