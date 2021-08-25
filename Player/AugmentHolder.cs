using UnityEngine;
/// <summary>
/// A Script to hold the Augments of the Player. Any positions needed for reference is not to use this script.
/// </summary>
public class AugmentHolder : MonoBehaviour
{
    //Use player canvas when ready
    private Mercenary mercenary; //CURRENTLY NOT IN USE
    public GameObject augment1;
    public GameObject augment2;
    public GameObject augment3;

    private void Awake()
    {
        mercenary = GetComponentInParent<Mercenary>();
    }
    public void SetAugments()
    {
        if (augment1 != null)
        {
            augment1.GetComponent<Augment>().SetAugmentHolder(this);
            augment1.GetComponent<Augment>().TurnOnAugment();
        }
        if (augment2 != null)
        {
            augment2.GetComponent<Augment>().SetAugmentHolder(this);
            augment2.GetComponent<Augment>().TurnOnAugment();
        }
        if (augment3 != null)
        {
            augment3.GetComponent<Augment>().SetAugmentHolder(this);
            augment3.GetComponent<Augment>().TurnOnAugment();
        }
    }
}
