using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MercenaryMenu : MonoBehaviour
{
    public Mercenary mercenary;
    public Gun[] primaryWeaponsAllow;
    public Gun[] secondaryWeaponsAllow;
    public Melee[] meleeWeaponsAllow;
    public Augment[] mercSpecificAllow; //Any perks disabled by default turn on
    public Augment[] mercSpecificDeny;  //Any perks enabled by default turn off
    
}
