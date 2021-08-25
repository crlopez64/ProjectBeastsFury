using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// HUD Script that will show current progress of objective.
/// </summary>
public class CurrentObjectiveText : MonoBehaviour
{
    private LevelManager levelManager;
    private Text fountainPen;
    private byte currentLevel = 0;

    public PlayerInteract player;
    public Slider primarySlider;
    public Slider secondarySlider;

    private void Awake()
    {
        levelManager = FindObjectOfType<LevelManager>();
        fountainPen = GetComponent<Text>();
    }
    private void Start()
    {
        fountainPen.text = "OBJ 0: TESTER";
        primarySlider.minValue = 0;
        primarySlider.maxValue = 1;
        secondarySlider.minValue = 0;
        secondarySlider.maxValue = 1;
        secondarySlider.gameObject.SetActive(false);
	}
    private void Update()
    {
        if (levelManager.objectives.Length > 0)
        {
            if (currentLevel != levelManager.CurrentObjective())
            {
                currentLevel = levelManager.CurrentObjective();
                ProceedObjective(currentLevel,
                    (player.OnTeamAttackers() ? levelManager.GetObjective(true) : levelManager.GetObjective(false)));
            }
        }
        else
        {
            currentLevel = 0;
            ProceedObjective(currentLevel, "EXPLORE AND DEBUG");
        }
    }

    public void ProceedObjective(int listing, string objective)
    {
        fountainPen.text = "OBJ " + listing + ", " + objective;
    }
    public void ResetBothSliders()
    {
        primarySlider.maxValue = 1;
        primarySlider.value = 0;
        secondarySlider.maxValue = 1;
        secondarySlider.value = 0;
        secondarySlider.gameObject.SetActive(false);
    }
    public void ResetSecondarySlider()
    {
        secondarySlider.maxValue = 1;
        secondarySlider.value = 0;
        secondarySlider.gameObject.SetActive(false);
    }
    public void SetPrimarySliderValue(float currentValue, float maxValue)
    {
        primarySlider.maxValue = maxValue;
        primarySlider.value = currentValue;
    }
    public void SetSecondarySlider(float currentValue, float maxValue)
    {
        secondarySlider.gameObject.SetActive(true);
        secondarySlider.maxValue = maxValue;
        secondarySlider.value = currentValue;
    }
}
