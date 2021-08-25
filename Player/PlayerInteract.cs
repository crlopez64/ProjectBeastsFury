using UnityEngine;

/// <summary>
/// Script made for Player's Interaction with the world (sans damage).
/// </summary>
public class PlayerInteract : MonoBehaviour
{
    private PlayerCanvas playerCanvas;
    private PlayerWeaponControls playerWeaponControls;
    private ObjectiveProgress importantObjective;
    private PlayerController playerController;
    private Interactable interactable;
    private Mercenary mercenary;
    private bool teamAttackers; //Can merge into one byte
    private bool teamDefenders;
    private bool toggleHoldAction;
    private bool holdingObjective;
    private bool interactingInteractable;
    private bool interactingImportantObjective;
    private float plastiqueArmingProgress;
    private float engineerRateAugment = 1f; //If Engy, this is 4; otherwise, cannot go above 3
    
    public Transform plastiquePlantPosition;
    
    private void Awake()
    {
        playerCanvas = FindObjectOfType<PlayerCanvas>();
        playerWeaponControls = GetComponent<PlayerWeaponControls>();
        playerController = GetComponent<PlayerController>();
        mercenary = GetComponent<Mercenary>();
        teamAttackers = true;
        LevelManager.Instance.SetPlayer(this);
    }
    private void Start()
    {
        toggleHoldAction = true;
        playerCanvas.objectiveSlider.gameObject.SetActive(false);
        plastiqueArmingProgress = 0f;
        TeamPlacementCheck();
	}
	private void Update()
    {
        if (interactable != null)
        {
            if (!interactable.IsInstant())
            {
                if (teamDefenders && interactable.DefendersHoldAction())
                {
                    if (toggleHoldAction && Input.GetKey(KeyCode.F))
                    {
                        interactingInteractable = true;
                        interactable.Interact(this);
                    }
                    else if (!toggleHoldAction && Input.GetKeyDown(KeyCode.F))
                    {
                        //Debug.Log("Continuous Regen Interacting...");
                    }
                    if (toggleHoldAction && Input.GetKeyUp(KeyCode.F))
                    {
                        interactingInteractable = false;
                        interactable.InteractStop(this);
                    }
                }
                else if (Input.GetKeyDown(KeyCode.F))
                {
                    playerCanvas.actionText.TurnOffText();
                    interactable.Interact(this);
                }
            }
        }
        //Important Objective interaction
        if (importantObjective != null)
        {
            if (toggleHoldAction && Input.GetKey(KeyCode.F))
            {
                interactingImportantObjective = true;
                importantObjective.Interact(this);
            }
            else if (!toggleHoldAction && Input.GetKeyDown(KeyCode.F))
            {
                //Debug.Log("Continuous Regen Interacting...");
            }
            if (toggleHoldAction && Input.GetKeyUp(KeyCode.F))
            {
                interactingImportantObjective = false;
                importantObjective.InteractStop(this);
            }
        }
	}
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Interactable"))
        {
            //Interact with Instant Interactiables
            if (other.GetComponent<InstantInteract>())
            {
                other.GetComponent<InstantInteract>().Interact(this);
            }
            //Interactables with Action
            if (other.GetComponent<Interactable>().NeedsAction())
            {
                //Is Action crucial to game or not
                if (other.GetComponent<Interactable>().IsImportant())
                {
                    //If Important Objective matches the corresponding team, ask to do Action
                    importantObjective = other.GetComponent<ObjectiveProgress>();
                    if (importantObjective.IsCurrentObjective())
                    {
                        ShowObjectiveText(other.GetComponent<Interactable>());
                    }
                }
                else
                {
                    //If Normal Object, do you need to be in a certain team to interact
                    if (other.GetComponent<Interactable>().NeedTeamToInteract())
                    {
                        if (other.GetComponent<PlastiquePickup>() != null)
                        {
                            if (teamDefenders && other.GetComponent<PlastiquePickup>().IsArmed())
                            {
                                interactable = other.GetComponent<Interactable>();
                                ShowObjectiveText(other.GetComponent<Interactable>());
                            }
                            else if (teamAttackers && !other.GetComponent<PlastiquePickup>().IsArmed())
                            {
                                interactable = other.GetComponent<Interactable>();
                                ShowObjectiveText(other.GetComponent<Interactable>());
                            }
                        }
                        else
                        {
                            if (other.GetComponent<FixObstacleOfInterest>() != null)
                            {
                                if (!other.GetComponent<FixObstacleOfInterest>().OnRepairCooldown())
                                {
                                    interactable = other.GetComponent<Interactable>();
                                    ShowObjectiveText(other.GetComponent<Interactable>());
                                }
                                else
                                {
                                    ShowObjectiveText(other.GetComponent<Interactable>());
                                }
                            }
                            else
                            {
                                interactable = other.GetComponent<Interactable>();
                                ShowObjectiveText(other.GetComponent<Interactable>());
                            }
                        }
                    }
                    else
                    {
                        interactable = other.GetComponent<Interactable>();
                        ShowObjectiveText(other.GetComponent<Interactable>());
                    }
                }
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Interactable"))
        {
            if (other.GetComponent<Interactable>().NeedsAction())
            {
                if (interactable != null)
                {
                    interactable.InteractStop(this);
                    playerCanvas.actionText.TurnOffText();
                    playerCanvas.objectiveSlider.StopSlider();
                }
                interactable = null;
            }
            if (other.GetComponent<Interactable>().IsImportant())
            {
                other.GetComponent<Interactable>().InteractStop(this);
                interactingImportantObjective = false;
                importantObjective = null;
                interactable = null;
            }
        }
    }

    public void SetHoldingPlastique(bool tOrF)
    {
        if (tOrF)
        {
            playerCanvas.holdingPlastiqueText.ShowText();
        }
        else
        {
            playerCanvas.holdingPlastiqueText.TurnOffText();
        }
    }
    public void ArmingPlastique(float standardArmingRate)
    {
        playerController.SetCanMove(false);
        plastiqueArmingProgress += (standardArmingRate * engineerRateAugment);
        playerCanvas.objectiveSlider.SetText("ARMING C4");
        playerCanvas.objectiveSlider.SetCurrentValue(plastiqueArmingProgress);
    }
    public void ResetPlastiqueArming()
    {
        playerController.SetCanMove(true);
        plastiqueArmingProgress = 0f;
        playerCanvas.objectiveSlider.StopSlider();
    }
    public void PlaceOnTeamAttackers()
    {
        teamAttackers = true;
        teamDefenders = false;
    }
    public void PlaceOnTeamDefenders()
    {
        teamAttackers = false;
        teamDefenders = true;
    }
    public void RemindPlayerOfObjective(string text)
    {
        playerCanvas.mainText.SetText(text);
    }
    public void RemindPlayerOfObjectiveTwice(string text1, string text2)
    {
        playerCanvas.mainText.SetTextTwice(text1, text2);
    }
    public void CompleteObjectivePlacement()
    {
        playerWeaponControls.CompleteObjectivePlacement();
    }
    public void SetHoldingObjective(bool tOrF)
    {
        holdingObjective = tOrF;
    }
    public Mercenary GetMercenary()
    {
        return mercenary;
    }
    public PlayerCanvas GetPlayerCanvas()
    {
        return playerCanvas;
    }
    public ObjectiveCanisterPickup GetCanister()
    {
        return playerWeaponControls.GetPickup();
    }
    public PlayerWeaponControls GetPlayerWeaponControls()
    {
        return playerWeaponControls;
    }
    public bool HoldingObjective()
    {
        return holdingObjective;
    }
    public bool OnTeamAttackers()
    {
        return teamAttackers;
    }
    public bool OnTeamDefenders()
    {
        return teamDefenders;
    }
    public bool PlastiqueArmed()
    {
        return plastiqueArmingProgress >= 1f;
    }
    public bool Interacting()
    {
        return interactingImportantObjective || interactingInteractable;
    }
    public float GetPlastiqueArmingProgress()
    {
        return plastiqueArmingProgress;
    }
    public float GetEngineerRateAugment()
    {
        return engineerRateAugment;
    }
    public byte GetObjectiveKey()
    {
        if (HoldingObjective())
        {
            return interactable.objectiveKey;
        }
        else
        {
            return 0;
        }
    }

    private void TeamPlacementCheck()
    {
        if (!teamAttackers && !teamDefenders)
        {
            Debug.LogWarning("Player not in either team!");
        }
        if (teamAttackers && teamDefenders)
        {
            teamAttackers = false;
        }
    }
    private void ShowObjectiveText(Interactable interactable)
    {
        if (interactable.InteractAttackers() && teamAttackers)
        {
            if (interactable.NeedsObjective())
            {
                if (holdingObjective)
                {
                    //TODO: Haven't implemented C4 object as a carryable object WITHOUT it dropping
                    if (interactable.GetComponent<ObjectiveProgress>().NeedPlastiqueObjectText())
                    {
                        playerCanvas.actionText.ShowTextObjective(interactable.ActionTextCallAttackers(), "C4");
                    }
                    else
                    {
                        playerCanvas.actionText.ShowTextObjective(interactable.ActionTextCallAttackers(),
                        GetComponentInChildren<WeaponAtHand>().GetPickup().interactableName);
                    }
                }
                else
                {
                    playerCanvas.actionText.ShowTextRequireItem();
                }
            }
            else
            {
                playerCanvas.actionText.ShowTextObjective(interactable.ActionTextCallAttackers(), interactable.interactableName);
            }
        }
        if (interactable.InteractDefenders() && teamDefenders)
        {
            if (interactable.GetComponent<PlastiquePickup>() != null)
            {
                if (interactable.GetComponent<PlastiquePickup>().IsArmed())
                {
                    playerCanvas.actionText.ShowTextObjective(interactable.ActionTextCallDefenders(), interactable.interactableName);
                }
            }
            else
            {
                if (interactable.GetComponent<FixObstacleOfInterest>() != null)
                {
                    if (interactable.GetComponent<FixObstacleOfInterest>().OnRepairCooldown())
                    {
                        playerCanvas.actionText.ShowTextFixOnCooldown();
                    }
                    else
                    {
                        playerCanvas.actionText.ShowTextObjective(interactable.ActionTextCallDefenders(), interactable.interactableName);
                    }
                }
                else
                {
                    playerCanvas.actionText.ShowTextObjective(interactable.ActionTextCallDefenders(), interactable.interactableName);
                }
            }
        }
        if (interactable.CanPickUp() && teamAttackers)
        {
            playerCanvas.actionText.ShowTextObjective(interactable.ActionTextCallAttackers(), interactable.interactableName);
        }
    }
}
