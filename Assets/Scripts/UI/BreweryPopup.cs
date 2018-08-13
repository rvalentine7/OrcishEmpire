﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BreweryPopup : MonoBehaviour {
    public GameObject brewery;
    public Text status;
    public Text employeeNum;
    public Text hopsNum;
    public Text progressNum;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKey(KeyCode.Escape))
        {
            Destroy(gameObject);
        }
        ItemProduction thisBrewery = brewery.GetComponent<ItemProduction>();
        progressNum.text = "" + thisBrewery.getProgressNum() + "/100";
        Employment employment = brewery.GetComponent<Employment>();
        employeeNum.text = "" + employment.getNumWorkers() + "/" + employment.getWorkerCap();
        Storage storage = brewery.GetComponent<Storage>();
        hopsNum.text = "" + storage.getHopsCount();
        if (employment.getNumWorkers() == 0)
        {
            status.text = "Without any employees, this brewery cannot produce any beer.";
        }
        else if (employment.getWorkerCap() > employment.getNumWorkers())
        {
            status.text = "This brewery is producing slowly due to a lack of employees.";
        }
        else
        {
            status.text = "This brewery is producing beer at peak efficiency.";
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
     * Sets the brewery object this popup is displaying information on.
     * @param brewery the brewery the popup is displaying information on
     */
    public void setBrewery(GameObject brewery)
    {
        this.brewery = brewery;
    }
}
