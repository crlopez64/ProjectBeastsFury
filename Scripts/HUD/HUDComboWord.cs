using UnityEngine;
using TMPro;

/// <summary>
/// Script that is placed with the word "Combo" in the HUD.
/// </summary>
public class HUDComboWord : MonoBehaviour
{
    public void Awake()
    {
        GetComponent<TextMeshProUGUI>().text = "COMBO";
    }
}
