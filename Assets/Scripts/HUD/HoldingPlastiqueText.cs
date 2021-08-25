using UnityEngine;
using UnityEngine.UI;

public class HoldingPlastiqueText : MonoBehaviour
{
    private Text fountainPen;

    private void Awake()
    {
        fountainPen = GetComponent<Text>();
    }
    private void Start()
    {
        fountainPen.enabled = false;
	}

    public void ShowText()    { fountainPen.enabled = true;  }
    public void TurnOffText() { fountainPen.enabled = false; }
}
