using UnityEngine;

/// <summary>
/// A perk meant to aid any Merc in gameplay.
/// </summary>
public class Augment : MonoBehaviour
{
    protected AugmentHolder augmentHolder;
    /// <summary>
    /// The name of the Augment.
    /// </summary>
    protected new string name;
    /// <summary>
    /// The small description of the Augment to display.
    /// </summary>
    protected string description;

    public Sprite sprite;
    
    public virtual void TurnOnAugment()
    {
        Debug.Log("Using Augment.");
	}
    public virtual void TurnOffAugment()
    {
        Debug.Log("Turning off Augment.");
    }

	public virtual void SetAugmentHolder(AugmentHolder augmentHolder)
    {
        this.augmentHolder = augmentHolder;
    }
    public AugmentHolder GetAugmentHolder()
    {
        return augmentHolder;
    }
}
