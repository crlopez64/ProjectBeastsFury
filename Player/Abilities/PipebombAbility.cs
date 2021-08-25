using UnityEngine;

public class PipebombAbility : Ability
{
    
    public GameObject pipebomb;

    public override void Awake()
    {
        heldItemName = "Pipe Bomb";
        heldItemDescription = "A lethal makeshift bomb with a considerable Damage output. While crouching, you may roll the Bomb.";
        maxUses = 1;
        cooldown = 20f;
        base.Awake();
    }
}
