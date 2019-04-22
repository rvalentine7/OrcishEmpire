﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class OchrePitPopup : MonoBehaviour {
    public GameObject ochrePit;
    public Text status;
    public Text employeeNum;
    public Text sickEmployeeNum;
    public Text progressNum;
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
        Production ochrePitProduction = ochrePit.GetComponent<Production>();
        progressNum.text = "" + ochrePitProduction.getProgressNum() + "/100";
        Employment employment = ochrePit.GetComponent<Employment>();
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
        sickEmployeeNum.text = "" + (employment.getNumWorkers() - employment.getNumHealthyWorkers()) + "/" + employment.getNumWorkers();
        if (employment.getNumWorkers() == 0)
        {
            status.text = "Without any employees, this pit cannot mine any ochre.";
        }
        else if (employment.getWorkerCap() > employment.getNumWorkers())
        {
            status.text = "This pit is producing slowly due to a lack of employees.";
        }
        else
        {
            status.text = "This pit is mining ochre at peak efficiency.";
        }
    }

    /**
     * Toggles the building on/off
     */
    public void toggleActivate()
    {
        bool activated = ochrePit.GetComponent<Employment>().toggleActivated();
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
     * Sets the ochre pit object this popup is displaying information on.
     */
    public void setOchrePit(GameObject ochrePit)
    {
        this.ochrePit = ochrePit;
    }
}
