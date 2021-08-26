using UnityEngine;

/// <summary>
/// Script in charge of moving the Unit's animation layers.
/// </summary>
public class UnitLayers : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    /// <summary>
    /// Turn on the movement layer.
    /// </summary>
    public void SetMovementLayer()
    {
        animator.SetLayerWeight(0, 1);
        animator.SetLayerWeight(1, 0);
        animator.SetLayerWeight(2, 0);
    }
    /// <summary>
    /// Turn on the Hit layer.
    /// </summary>
    public void SetHitLayer()
    {
        animator.SetLayerWeight(0, 0);
        animator.SetLayerWeight(1, 1);
        animator.SetLayerWeight(2, 0);
    }
    /// <summary>
    /// Turn on the Attack layer.
    /// </summary>
    public void SetAttackLayer()
    {
        animator.SetLayerWeight(0, 0);
        animator.SetLayerWeight(1, 0);
        animator.SetLayerWeight(2, 1);
    }
}
