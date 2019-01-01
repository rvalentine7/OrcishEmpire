using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MarketPopup : MonoBehaviour {
    public GameObject marketplace;
    public Text status;
    public Text employeeNum;
    public Text storageCapacity;
    public Text meatNum;
    public Text wheatNum;
    public Text eggsNum;
    public Text fishNum;
    public Text furnitureNum;
    public Text weaponsNum;
    public Button activateButton;
    public Sprite activateSprite;
    public Sprite deactivateSprite;

    // Use this for initialization
    void Start () {
		
	}
	
	/**
     * Updates the status and number of goods for the marketplace.
     */
	void Update () {
        if (Input.GetKey(KeyCode.Escape))
        {
            Destroy(gameObject);
        }
        Storage storage = marketplace.GetComponent<Storage>();
        storageCapacity.text = "" + storage.getCurrentAmountStored() + "/" + storage.getStorageMax();
        Employment employment = marketplace.GetComponent<Employment>();
        bool activated = employment.getActivated();
        if (!activated)
        {
            activateButton.image.sprite = activateSprite;
        }
        else
        {
            activateButton.image.sprite = deactivateSprite;
        }
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
        wheatNum.text = "" + storage.getWheatCount();
        eggsNum.text = "" + storage.getEggCount();
    }

    /**
     * Toggles the building on/off
     */
    public void toggleActivate()
    {
        bool activated = marketplace.GetComponent<Employment>().toggleActivated();
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
     * Sets the marketplace object this popup is displaying information on.
     */
    public void setMarketplace(GameObject marketplace)
    {
        this.marketplace = marketplace;
    }
}
