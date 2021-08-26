using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class in charge of keeping track all of the Hurtboxes in the Unit.
/// </summary>
public class UnitHurtboxHolder : MonoBehaviour
{
    private UnitHurtbox[] hurtboxes;
    private bool beenHit;
    private byte player;

    private void Awake()
    {
        hurtboxes = GetComponentsInChildren<UnitHurtbox>();
    }
    private void Start()
    {
        beenHit = false;
    }

    /// <summary>
    /// If this Unit hasn't been hit yet, attack them.
    /// </summary>
    public void BeenHit()
    {
        if (!beenHit)
        {
            beenHit = true;
        }
    }
    /// <summary>
    /// Set if this Unit can be hit again.
    /// </summary>
    public void CanHitAgain()
    {
        beenHit = false;
    }
    /// <summary>
    /// Has this Unit been hit?
    /// </summary>
    /// <returns></returns>
    public bool HasBeenHit()
    {
        return beenHit;
    }
    /// <summary>
    /// Return the number of the Player.
    /// </summary>
    /// <returns></returns>
    public byte GetPlayer()
    {
        return player;
    }
}
