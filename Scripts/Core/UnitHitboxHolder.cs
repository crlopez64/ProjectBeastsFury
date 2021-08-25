using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitHitboxHolder : MonoBehaviour
{
    private UnitHitbox[] hitboxes;

    private void Awake()
    {
        hitboxes = GetComponentsInChildren<UnitHitbox>();
    }

    /// <summary>
    /// Activate all the Hitboxes in the group.
    /// </summary>
    public void ActivateHitboxes()
    {
        foreach(UnitHitbox box in hitboxes)
        {
            box.ActiveHitbox();
        }
    }
}
