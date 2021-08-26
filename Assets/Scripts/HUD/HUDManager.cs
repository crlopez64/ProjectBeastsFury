using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    public Canvas healthBars;
    public Canvas buttonInput;
    public Canvas notifications;
    public Canvas pauseMenu;

    private void Start()
    {
        healthBars.gameObject.SetActive(true);
        buttonInput.gameObject.SetActive(true);
        notifications.gameObject.SetActive(true);
        pauseMenu.gameObject.SetActive(false);
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            healthBars.gameObject.SetActive(!healthBars.gameObject.activeInHierarchy);
            buttonInput.gameObject.SetActive(!buttonInput.gameObject.activeInHierarchy);
            pauseMenu.gameObject.SetActive(!pauseMenu.gameObject.activeInHierarchy);
            if(pauseMenu.gameObject.activeInHierarchy)
            {
                pauseMenu.GetComponentInChildren<HUDDebuggerMoveList>().SetMoveList(FindObjectOfType<UnitAttack>().RootAttack());
            }
        }
    }
}
