using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scavenger : Augment
{
    private void Start()
    {
        name = "Scavenger";
        description = "Make enemies drop a pickup item when killed, depending on their given class (Cooldown activated).";
    }
}
