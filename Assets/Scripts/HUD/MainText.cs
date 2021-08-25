using UnityEngine;
using UnityEngine.UI;

public class MainText : MonoBehaviour
{
    private PlayerCanvas playerCanvas;
    private Image panel;
    private Text fountainPen;
    private bool twoText;
    private bool winText;
    private float timerTillFade;
    private string storedTwoText;

    private void Awake()
    {
        playerCanvas = GetComponentInParent<PlayerCanvas>();
        panel = GetComponentInParent<Image>();
        fountainPen = GetComponent<Text>();
    }
	private void Update ()
    {
		if (timerTillFade > 0)
        {
            timerTillFade -= Time.deltaTime;
        }
        else
        {
            if (winText)
            {
                winText = false;
                fountainPen.enabled = false;
                panel.enabled = false;
                playerCanvas.winLoseText.gameObject.SetActive(true);
            }
            else
            {
                if (twoText)
                {
                    twoText = false;
                    SetText(storedTwoText);
                }
                else
                {
                    fountainPen.enabled = false;
                    panel.enabled = false;
                }
            }
        }
	}
    public void SetFinalText(string text)
    {
        winText = true;
        panel.enabled = true;
        fountainPen.enabled = true;
        fountainPen.text = text;
        timerTillFade = 5f;
    }
    public void SetText(string text)
    {
        panel.enabled = true;
        fountainPen.enabled = true;
        fountainPen.text = text;
        timerTillFade = 5f;
    }
    public void SetTextTwice(string text1, string text2)
    {
        twoText = true;
        panel.enabled = true;
        fountainPen.enabled = true;
        fountainPen.text = text1;
        storedTwoText = text2;
        timerTillFade = 2f;
    }
}
