using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ArenaPopup : MonoBehaviour {
    public GameObject arena;
    public Text status;
    public Text employeeNum;
    public Text gladiatorsNum;
    public Text progressNum;
    public Text fightDurationNum;
    public Button activateButton;
    public Sprite activateSprite;
    public Sprite deactivateSprite;
    private bool initialClick;

    // Use this for initialization
    void Start()
    {
        GameObject panel = GameObject.FindWithTag(World.BUILD_PANEL);
        if (panel != null)
        {
            panel.SetActive(false);
        }
        initialClick = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (initialClick && (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1)))
        {
            initialClick = false;
        }
        if (Input.GetKey(KeyCode.Escape) || (!initialClick
            && !EventSystem.current.IsPointerOverGameObject()
            && (Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(0))))
        {
            Destroy(gameObject);
        }
        Employment employment = arena.GetComponent<Employment>();
        bool activated = employment.getActivated();
        if (!activated)
        {
            activateButton.image.sprite = activateSprite;
        }
        else
        {
            activateButton.image.sprite = deactivateSprite;
        }
        employeeNum.text = "" + employment.getNumWorkers() + "/" + employment.getWorkerCap();
        if (employment.getNumWorkers() == 0)
        {
            status.text = "There are no fights going on at the arena right now.";
        }
        else if (employment.getWorkerCap() > employment.getNumWorkers())
        {
            status.text = "With few employees, fights will rarely occur.";
        }
        else
        {
            status.text = "This arena has fights occuring on a regular basis.";
        }
        Arena arenaScript = arena.GetComponent<Arena>();
        gladiatorsNum.text = arenaScript.getNumGladiators() + "/" + arenaScript.getNumGladiatorsRequired();
        string setupProgress = "" + arenaScript.getSetupProgress() + "/100";
        if (arenaScript.getSetupProgress() >= 100
            && arenaScript.getNumGladiators() < arenaScript.getNumGladiatorsRequired())
        {
            setupProgress = "Need gladiators";
        }
        else if (arenaScript.getSetupProgress() >= 100
            && arenaScript.getNumGladiators() == arenaScript.getNumGladiatorsRequired())
        {
            setupProgress = "Fight in progress";
        }
        progressNum.text = setupProgress;
        string fightDurationTime = "Waiting on next fight";
        if (arenaScript.getRemainingFightTime() > 0)
        {
            fightDurationTime = "" + Mathf.RoundToInt(arenaScript.getRemainingFightTime()) + "s";
        }
        fightDurationNum.text = fightDurationTime;
    }

    /**
     * Toggles the building on/off
     */
    public void toggleActivate()
    {
        bool activated = arena.GetComponent<Employment>().toggleActivated();
        if (!activated)
        {
            activateButton.image.sprite = activateSprite;
        }
        else
        {
            activateButton.image.sprite = deactivateSprite;
        }
    }

    /**
     * Removes the game object from the game.
     */
    public void destroyObject()
    {
        Destroy(gameObject);
    }

    /**
     * Sets the arena object this popup is displaying information on.
     */
    public void setArena(GameObject arena)
    {
        this.arena = arena;
    }
}
