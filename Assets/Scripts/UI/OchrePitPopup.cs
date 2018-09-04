﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OchrePitPopup : MonoBehaviour {
    public GameObject ochrePit;
    public Text status;
    public Text employeeNum;
    public Text progressNum;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            Destroy(gameObject);
        }
        Production ochrePitProduction = ochrePit.GetComponent<Production>();
        progressNum.text = "" + ochrePitProduction.getProgressNum() + "/100";
        Employment employment = ochrePit.GetComponent<Employment>();
        employeeNum.text = "" + employment.getNumWorkers() + "/" + employment.getWorkerCap();
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