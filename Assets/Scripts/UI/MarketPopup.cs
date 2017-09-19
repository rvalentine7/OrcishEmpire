using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MarketPopup : MonoBehaviour {
    public GameObject marketplace;
    public Text status;
    public Text employeeNum;
    public Text meatNum;
    public Text wheatNum;
    public Text fishNum;
    public Text furnitureNum;
    public Text weaponsNum;

    // Use this for initialization
    void Start () {
		
	}
	
	/**
     * Updates the status and number of goods for the marketplace.
     */
	void Update () {
        Storage storage = marketplace.GetComponent<Storage>();
        Employment employment = marketplace.GetComponent<Employment>();
        if (employment.getNumWorkers() == 0)
        {
            status.text = "Without any employees, this marketplace cannot distribute goods.";
        }
        else if (employment.getWorkerCap() > employment.getNumWorkers())
        {
            status.text = "This marketplace is distributing goods slowly due to a lack of workers.";
        }
        else
        {
            status.text = "This marketplace is efficiently distributing goods.";
        }
        employeeNum.text = "" + employment.getNumWorkers() + "/" + employment.getWorkerCap();
        meatNum.text = "" + storage.getMeatCount();
    }

    /**
     * Removes the game object from the game.
     */
    public void destroyObject()
    {
        Destroy(gameObject);
    }

    /**
     * Sets the marketplace object this popup is displaying information on.
     */
    public void setMarketplace(GameObject marketplace)
    {
        this.marketplace = marketplace;
    }
}
