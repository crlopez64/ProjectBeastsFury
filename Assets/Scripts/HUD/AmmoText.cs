using UnityEngine;
using UnityEngine.UI;

public class AmmoText : MonoBehaviour
{
    private PlayerWeaponControls player;
    private Text fountainPen;

    public bool isCurrentClip;

    private void Awake()
    {
        player = FindObjectOfType<PlayerWeaponControls>();
        fountainPen = GetComponent<Text>();
    }
	void Update()
    {
        if (player.HoldingGun())
        {
            //Gun temp = (Gun)player.GetCurrentWeapon();
            if (isCurrentClip)
            {
                fountainPen.text = player.GetGun().GetCurrentClip().ToString();
            }
            else
            {
                fountainPen.text = player.GetGun().GetCurrentCarry().ToString();
            }
        }
        else
        {
            fountainPen.text = "---";
        }
        
    }
}
