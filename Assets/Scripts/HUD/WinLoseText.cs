using UnityEngine;
using UnityEngine.UI;

public class WinLoseText : MonoBehaviour
{
    private Text fountainPen;

    private void Awake()
    {
        fountainPen = GetComponentInChildren<Text>();
    }
    void Start()
    {
        gameObject.SetActive(false);
	}

    public void SetText(string text)
    {
        fountainPen.text = text;
    }
}
