using UnityEngine;

/// <summary>
/// Script that is the driving force of the Player's movement and camera rotations.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class PlayerMotor : MonoBehaviour
{
    private new Rigidbody rigidbody;
    private new Camera camera;
    private Vector3 velocity = Vector3.zero;
    private Vector3 rotation = Vector3.zero;
    private bool useGravity;
    private float jumpForce = 240f;
    private float cameraRotation = 0f; //NOT CURRENTLY IN USE
    private float cameraRotationX = 0f;
    private float cameraRotationLimit = 90f;
    private float currentCameraRotationX = 0f;

    public LayerMask whatIsGround;

    private void Awake()
    {
        camera = GetComponentInChildren<Camera>();
        rigidbody = GetComponent<Rigidbody>();
        useGravity = true;
        rigidbody.useGravity = false;
    }
	void FixedUpdate()
    {
        PerformMovement();
        PerformRotation();
        if (useGravity)
        {
            rigidbody.AddForce(Physics.gravity * (rigidbody.mass * rigidbody.mass));
        }
	}

    //----------------//
    // PUBLIC METHODS //
    //----------------//
    /// <summary>
    /// Move the Player.
    /// </summary>
    /// <param name="velocity"></param>
    public void Move(Vector3 velocity)
    {
        this.velocity = velocity;
    }
    /// <summary>
    /// Rotate the Player via the X part of Mouse Input.
    /// </summary>
    /// <param name="rotation"></param>
    public void Rotate(Vector3 rotation)
    {
        this.rotation = rotation;
    }
    /// <summary>
    /// Rotate the Camera via the Y part of Mouse Input.
    /// </summary>
    /// <param name="cameraRotationX"></param>
    public void RotateCamera(float cameraRotationX)
    {
        this.cameraRotationX = cameraRotationX;
    }
    public void Jump(float multiplier)
    {
        if (multiplier > 1.5f)
        {
            multiplier = 0.5f;
        }
        rigidbody.AddForce(Vector3.up * jumpForce * multiplier, ForceMode.Impulse);
    }
    //TODO: Remove the following method.
    public void WallJump(Vector3 direction)
    {
        rigidbody.AddForce(direction, ForceMode.Impulse);
    }

    //-----------------//
    // PRIVATE METHODS //
    //-----------------//
    private void PerformMovement()
    {
        if (velocity != Vector3.zero)
        {
            RaycastHit hit;
            Physics.Raycast(transform.position, Vector3.down, out hit, 1f);
            rigidbody.MovePosition(transform.position + velocity * Time.deltaTime);
        }
    }
    private void PerformRotation()
    {
        rigidbody.MoveRotation(transform.rotation * Quaternion.Euler(rotation));
        if (camera != null)
        {
            currentCameraRotationX -= cameraRotationX;
            currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -cameraRotationLimit, cameraRotationLimit);
            camera.transform.localEulerAngles = new Vector3(currentCameraRotationX, 0, 0);
        }
    }
}
