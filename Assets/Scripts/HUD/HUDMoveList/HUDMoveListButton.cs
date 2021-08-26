using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

/// <summary>
/// Script that shows the input for this selected move.
/// </summary>
public class HUDMoveListButton : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    public TextMeshProUGUI moveName;
    public TextMeshProUGUI moveInput;

    /// <summary>
    /// Set the Move Button.
    /// </summary>
    /// <param name="moveName"></param>
    /// <param name="moveInput"></param>
    public void SetMove(string moveName, string moveInput)
    {
        this.moveName.text = moveName;
        this.moveInput.text = moveInput;
    }
    /// <summary>
    /// Clear out the Move in this button.
    /// </summary>
    public void ClearMove()
    {
        moveName.text = "";
        moveInput.text = "";
    }

    public void OnSelect(BaseEventData eventData)
    {
        throw new System.NotImplementedException();
    }
    public void OnDeselect(BaseEventData eventData)
    {
        throw new System.NotImplementedException();
    }
}
