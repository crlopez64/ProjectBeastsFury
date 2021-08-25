using UnityEngine;
using UnityEngine.UI;

public class ObjectiveSlider : MonoBehaviour
{
    private Slider slider;
    private Text fountainPen;

    private void Awake()
    {
        slider = GetComponent<Slider>();
        fountainPen = GetComponentInChildren<Text>();
    }
    private void Start()
    {
        slider.minValue = 0;
        slider.maxValue = 1;
        slider.value = slider.minValue;
	}

    public void StopSlider()
    {
        slider.value = 0;
        gameObject.SetActive(false);
    }
    public void SetText(string text)
    {
        fountainPen.text = text;
    }
    public void SetCurrentValue(float value)
    {
        gameObject.SetActive(true);
        slider.value = value;
    }
    public void SetSlider()
    {
        slider.minValue = 0;
        slider.value = 0;
    }
}
