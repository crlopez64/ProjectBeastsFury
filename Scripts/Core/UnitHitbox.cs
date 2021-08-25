using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Script in charge of attacking.
/// </summary>
public class UnitHitbox : MonoBehaviour
{
    private UnitAttack unitAttack;
    private Collider2D[] hits;

    public Color colorWire;
    public Color colorFill;
    public LayerMask whatToHit;
    public Vector2 hitboxSize;

    public void Awake()
    {
        unitAttack = GetComponentInParent<UnitAttack>();
    }
    public void OnDrawGizmos()
    {
        if (hitboxSize != Vector2.zero)
        {
            Gizmos.color = colorFill;
            Gizmos.DrawCube(transform.position, hitboxSize);
            Gizmos.color = colorWire;
            Gizmos.DrawWireCube(transform.position, hitboxSize);
        }
    }
    /// <summary>
    /// Activate the hitbox.
    /// </summary>
    public void ActiveHitbox()
    {
        //BASE CASE: If any of the dimensions are near zero, cancel the hitbox call.
        if ((hitboxSize.x < 0.1f) || (hitboxSize.y < 0.1f))
        {
            return;
        }
        hits = Physics2D.OverlapBoxAll(transform.position, hitboxSize, 0, whatToHit);
        //BASE CASE: If nothing found, cancel the hitbox call.
        if (hits.Length == 0)
        {
            return;
        }
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].GetComponent<UnitHurtbox>() != null)
            {
                //Do attack
                if (unitAttack.CanStillHit())
                {
                    unitAttack.UsedHit();
                    UnitAttack enemyAttack = hits[i].GetComponentInParent<UnitAttack>();
                    //BASE CASE: If High vs. Low, disregard the attack
                    if (enemyAttack.GetComponent<UnitMove>().AttackHighEvaded(unitAttack.AttackToAnimate().AttackHeight()))
                    {
                        Debug.Log("A hitbox trigger was made but High vs. Blocking Crouching Low. Disregard.");
                        return;
                    }
                    //BASE CASE: If wall staggered at least twice, do not make another attack land
                    if (enemyAttack.GetComponent<UnitMove>().WallStaggeredMaxReached())
                    {
                        Debug.Log("Wall Stagger limit reached.");
                        return;
                    }
                    //BASE CASE: If opponent is ledge grabbing, make them drop
                    if (enemyAttack.GetComponent<UnitMove>().LedgeGrabbing())
                    {
                        if (enemyAttack.GetComponent<UnitMove>().AttackHighEvaded(unitAttack.AttackToAnimate().AttackHeight()))
                        {
                            Debug.Log("High vs. Ledge Grab. Disregard.");
                            return;
                        }
                        else
                        {
                            //Debug.Log("Drop enemy!!");
                            enemyAttack.GetComponent<UnitMove>().LedgeDrop();
                            Instantiate(GetComponentInParent<UnitAttack>().particleHit, transform.position, transform.rotation);
                        }
                        return;
                    }

                    if (enemyAttack.BlockSucceeded(unitAttack.AttackToAnimate().AttackHeight()) && 
                        (!enemyAttack.Hitstunned()))
                    {
                        //Debug.Log("Attack blocked!!");
                        enemyAttack.HasBeenHit(GetComponentInParent<UnitAttack>().AttackToAnimate(),
                            GetComponentInParent<UnitAttack>().AttackFrameIndex(),
                            GetComponentInParent<UnitAttack>().CurrentAttackAnimationDuration(),
                            false);
                        hits[i].GetComponentInParent<UnitMove>().Knockback(GetComponentInParent<UnitMove>().transform.position.x,
                            GetComponentInParent<UnitAttack>().AttackToAnimate());
                        Instantiate(GetComponentInParent<UnitAttack>().particleHit, transform.position, transform.rotation);
                    }
                    else
                    {
                        //Debug.Log("Attack either hit or failed to be blocked!!");
                        if (enemyAttack.CurrentAttackFrameType() == 1) //Attacking while opponent is on start up
                        {
                            GetComponentInParent<UnitStats>().NotifyCounter();
                            //Apply counter knockback and damage (1.5f)
                        }
                        if (enemyAttack.CurrentAttackFrameType() == 3)
                        {
                            GetComponentInParent<UnitStats>().NotifyPunish();
                            //Apply counter knockback and damage (1.15f)
                        }
                        enemyAttack.HasBeenHit(GetComponentInParent<UnitAttack>().AttackToAnimate(),
                            GetComponentInParent<UnitAttack>().AttackFrameIndex(),
                            GetComponentInParent<UnitAttack>().CurrentAttackAnimationDuration(),
                            true);
                        hits[i].GetComponentInParent<UnitMove>().Knockback(GetComponentInParent<UnitMove>().transform.position.x,
                            GetComponentInParent<UnitAttack>().AttackToAnimate());
                        GetComponentInParent<UnitStats>().AddToCombo();
                        Instantiate(GetComponentInParent<UnitAttack>().particleHit, transform.position, transform.rotation);
                    }
                }
            }
        }
    }
}
