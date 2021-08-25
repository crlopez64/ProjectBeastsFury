using UnityEngine;

public class TestFighter : Mercenary
{
    protected override void Awake()
    {
        base.Awake();
    }
    protected override void Start()
    {
        name = "Test Fighter 01";
        isEngy = false;
        healthMax = 120;
        engineerSpeed = 1f;
        classType = 1;
        base.Start();
	}
}
