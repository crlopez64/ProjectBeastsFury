using UnityEngine;
using UnityEngine.UI;

public class HealthBarSlider : MonoBehaviour
{
    private Slider slider;
    private Text fountainPen;
    private PlayerHealth playerHealth;

    private void Awake()
    {
        slider = GetComponent<Slider>();
        fountainPen = GetComponentInChildren<Text>();
    }
    private void Start()
    {
        slider.minValue = 0;
        slider.maxValue = 100;
        slider.value = slider.maxValue;
        fountainPen.text = slider.value.ToString();
	}
	private void Update()
    {
        slider.value = playerHealth.GetCurrentHealth();
        fountainPen.text = slider.value.ToString();
	}

    public void SetHealthSlider(PlayerHealth playerHealth)
    {
        this.playerHealth = playerHealth;
        slider.minValue = 0;
        slider.maxValue = playerHealth.GetMaxHealth();
        slider.value = playerHealth.GetMaxHealth();
        fountainPen.text = slider.value.ToString();
    }
}
