﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class FarmPopup : MonoBehaviour {
    public GameObject farm;
    public Text status;
    public Text employeeNum;
    public Text sickEmployeeNum;
    public Text progressNum;
    private bool initialClick;

    // Use this for initialization
    void Start () {
        GameObject panel = GameObject.FindWithTag(World.BUILD_PANEL);
        if (panel != null)
        {
            panel.SetActive(false);
        }
        initialClick = true;
	}
	
	// Update is called once per frame
	void Update () {
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
        Production thisFarm = farm.GetComponent<Production>();
        progressNum.text = "" + thisFarm.getProgressNum() + "/100";
        Employment employment = farm.GetComponent<Employment>();
        employeeNum.text = "" + employment.getNumWorkers() + "/" + employment.getWorkerCap();
        if (employment.getNumWorkers() == 0)
        {
            status.text = "Without any employees, this farm cannot produce food.";
        }
        else if (employment.getWorkerCap() > employment.getNumWorkers())
        {
            status.text = "This farm is producing slowly due to a lack of employees.";
        }
        else
        {
            status.text = "This farm is producing food at peak efficiency.";
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
     * Sets the farm object this popup is displaying information on.
     */
    public void setFarm(GameObject farm)
    {
        this.farm = farm;
    }
}
