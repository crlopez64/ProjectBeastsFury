using UnityEngine;

/// <summary>
/// Script that follows both fighters if still in the ring.
/// </summary>
public class CameraFollow : MonoBehaviour
{
    /*
     * NOTE: May not need focus?
     * Can possibly move around the camera via focus
     * 
     * TODO: Make camera shake for Knockdown
     * 
     */
    private new Camera camera;
    /// <summary>
    /// The default camera offset.
    /// </summary>
    private Vector3 offset;
    /// <summary>
    /// If used, the specified position to move the camera to.
    /// </summary>
    private Vector3 toldPosition;
    /// <summary>
    /// The actual offset to be using.
    /// </summary>
    private Vector3 currentOffset;
    private Vector3 vectorVelocityRef;
    private bool smoothZoom; //Two bools for the same function?
    /// <summary>
    /// Should the camera size be a told size instead of interpolated between a minimum and a maximum size?
    /// </summary>
    private bool overrideSize;
    /// <summary>
    /// Should there be camera smoothing across locations?
    /// </summary>
    private bool smoothCamera;
    /// <summary>
    /// Are the fighters defined?
    /// </summary>
    private bool fightersSet = false;
    /// <summary>
    /// Type of focus.
    /// </summary>
    private byte focus;
    private byte startMatchWith; //Determines how camera goes into the match
    private float cameraSize;
    private float velocityRef;
    /// <summary>
    /// The given camera size to be in place of an interpreted size.
    /// </summary>
    private float toldCameraSize;
    private float distanceBetweenFighters;
    private readonly float cameraSizeMin = 4.5f;
    private readonly float cameraSizeMax = 8;
    private readonly float distanceBetweenFightersMax = 22;

    public GameObject[] fighters;

    public void Awake()
    {
        camera = GetComponent<Camera>();
    }
    public void Start()
    {
        offset = new Vector3(0, 3.2f, 0);
        currentOffset = offset;
        camera.transform.position = camera.transform.position + offset;
        smoothZoom = true;
        smoothCamera = true;
    }
    public void Update()
    {
        distanceBetweenFighters = Vector3.Distance(fighters[0].transform.position, fighters[1].transform.position);

        cameraSize = overrideSize ? toldCameraSize :
            Mathf.Lerp(cameraSizeMin, cameraSizeMax, (distanceBetweenFighters - 4) / distanceBetweenFightersMax);
    }
    public void LateUpdate()
    {
        Vector3 centerPosition = GetTargetPoint();

        camera.transform.position = smoothCamera ?
            Vector3.SmoothDamp(camera.transform.position, centerPosition + currentOffset, ref vectorVelocityRef, 0.2f) :
            camera.transform.position = centerPosition + currentOffset;

        camera.orthographicSize = smoothZoom ? Mathf.SmoothDamp(camera.orthographicSize, cameraSize, ref velocityRef, 0.3f) :
            cameraSize;
    }

    public void AnimTurnOffAnimator()
    {
        smoothCamera = false;
        smoothZoom = false;
        camera.orthographicSize = 3;
        offset = new Vector3(0, 2.25f, 0);
        camera.transform.position = offset;
        smoothCamera = true;
        smoothZoom = true;
    }
    /// <summary>
    /// Set the Fighters for Camera follow. Can only be set once; should be set on Awake() via Level Manager.
    /// </summary>
    /// <param name="player1"></param>
    /// <param name="player2"></param>
    public void SetFighters(GameObject player1, GameObject player2)
    {
        if (!fightersSet)
        {
            fightersSet = true;
            fighters = new GameObject[2];
            fighters[0] = player1;
            fighters[1] = player2;
            focus = 0;
            camera.transform.position = GetTargetPoint() + currentOffset;
            smoothCamera = true;
            smoothZoom = true;
        }
    }
    /// <summary>
    /// Set Smooth Zoom.
    /// </summary>
    /// <param name="tOrF"></param>
    public void SetCameraZoom(bool tOrF)
    {
        smoothZoom = tOrF;
    }
    /// <summary>
    /// Make the camera focus on both players.
    /// </summary>
    public void SetFocusBothFighters()
    {
        focus = 0;
    }
    /// <summary>
    /// Make the camera focus to player one.
    /// </summary>
    public void SetFocusFighterOne()
    {
        focus = 1;
    }
    /// <summary>
    /// Make the camera focus to player two.
    /// </summary>
    public void SetFocusFighterTwo()
    {
        focus = 2;
    }
    /// <summary>
    /// Make the camera stay on its previously mentioned position.
    /// </summary>
    public void SetFocusFree()
    {
        focus = 3;
    }
    /// <summary>
    /// Make the camera focus to the specified Player.
    /// </summary>
    /// <param name="unitMove"></param>
    public void SetFocusByFighter(UnitMove unitMove)
    {
        for(int i = 0; i < fighters.Length; i++)
        {
            if (fighters[i].GetComponent<UnitMove>() == unitMove)
            {
                if (i == 0)
                {
                    SetFocusFighterOne();
                }
                else
                {
                    SetFocusFighterTwo();
                }
            }
        }
    }
    /// <summary>
    /// Make the camera focus on a specified positon.
    /// </summary>
    /// <param name="toldPosition"></param>
    public void SetFocusSpecificPosition(Vector3 toldPosition)
    {
        Debug.Log("Setting focus");
        focus = 4;
        this.toldPosition = toldPosition;
    }
    /// <summary>
    /// Is the camera following both fighters?
    /// </summary>
    /// <returns></returns>
    public bool CameraFocusingBothFighters()
    {
        return focus == 0;
    }
    private Vector3 GetTargetPoint()
    {
        smoothZoom = true;
        overrideSize = true;
        currentOffset = offset;
        if (focus == 1) //Only Fighter 1
        {
            toldCameraSize = 4;
            Vector3 temp = fighters[0].transform.position;
            return new Vector3(temp.x - 1.5f, temp.y, -100);
        }
        else if (focus == 2) //Only Fighter 2
        {
            toldCameraSize = 4;
            Vector3 temp = fighters[1].transform.position;
            return new Vector3(temp.x - 1.5f, temp.y, -100);
        }
        else if (focus == 3) //No Focus in Particular
        {
            toldCameraSize = 4;
            return transform.position;
        }
        else if (focus == 4) //Via a Told Position
        {
            smoothZoom = false;
            toldCameraSize = 7;
            currentOffset = new Vector3(0, 0, -100);
            return toldPosition;
        }
        //Else, follow both players
        overrideSize = false;
        Bounds bounds = new Bounds(fighters[0].transform.position, Vector3.zero);
        for (int i = 0; i < fighters.Length; i++)
        {
            bounds.Encapsulate(fighters[i].transform.position);
        }
        return new Vector3(bounds.center.x, bounds.center.y, -100);
    }
}
