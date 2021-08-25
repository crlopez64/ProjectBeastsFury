using UnityEngine;
using UnityEngine.UI;

public class ObjectiveText : MonoBehaviour
{
    private Text fountainPen;

    private void Awake()
    {
        fountainPen = GetComponent<Text>();
    }
    void Start ()
    {
		
	}

    public void TextHacking()     { fountainPen.text = "HACKING";   }
    public void TextRepairing()   { fountainPen.text = "REPAIRING"; }
    public void TextPlantingC4()  { fountainPen.text = "PLANTING";  }
    public void TextDisarmingC4() { fountainPen.text = "DISARMING"; }
	
}
