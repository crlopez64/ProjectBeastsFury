using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script used to check if a Transform has found a wall close to its vision.
/// </summary>
public class WallStaggerCheck : MonoBehaviour
{
    private bool highCheck;


    /// <summary>
    /// Set if this Wall Stagger is the high check.
    /// </summary>
    /// <param name="tOrF"></param>
    public void SetHighCheck(bool tOrF)
    {
        highCheck = tOrF;
    }
    /// <summary>
    /// Return if this Stagger Check is the higher check instead of the lower check.
    /// </summary>
    /// <returns></returns>
    public bool HighCheck()
    {
        return highCheck;
    }
    /// <summary>
    /// Returns if a Wall is found nearby.
    /// </summary>
    /// <param name="whatIsWall"></param>
    /// <returns></returns>
    public bool FoundWall(BoxCollider2D physicalCollider, LayerMask whatIsWall, bool toTheRight)
    {
        return Physics2D.Raycast(transform.position, toTheRight ? Vector2.right : Vector2.left,
                    (physicalCollider.size.x / 2) + 0.05f, whatIsWall);
    }
}
