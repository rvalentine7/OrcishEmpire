using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PubPopup : MonoBehaviour {
    private GameObject pub;
    public Text status;
    public Text employeeNum;
    public Text storageCapacity;
    public Text beerNum;
    public Text timeLeftNum;
    public Button activateButton;
    public Sprite activateSprite;
    public Sprite deactivateSprite;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKey(KeyCode.Escape))
        {
            Destroy(gameObject);
        }
        Employment employment = pub.GetComponent<Employment>();
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
        Storage storage = pub.GetComponent<Storage>();
        storageCapacity.text = "" + storage.getCurrentAmountStored() + "/" + storage.getStorageMax();
        if (employment.getNumWorkers() == 0)
        {
            status.text = "There is no drinking going on at the pub right now.";
        }
        else if (employment.getWorkerCap() > employment.getNumWorkers())
        {
            status.text = "With few employees, entertainment will not be provided as often.";
        }
        else if (storage.getBeerCount() > 0)
        {
            status.text = "This pub is providing entertainment to nearby houses.";
        }
        else
        {
            status.text = "This pub is waiting on a supply of beer to provide entertainment.";
        }
        beerNum.text = "" + storage.getBeerCount();
        Pub pubScript = pub.GetComponent<Pub>();
        int timeLeft = pubScript.getTimeLeftOnCurrentDrinks();
        if (timeLeft <= 0)
        {
            timeLeftNum.text = "0s";
        }
        else
        {
            timeLeftNum.text = "" + timeLeft + "s";
        }
    }

    /**
     * Toggles the building on/off
     */
    public void toggleActivate()
    {
        bool activated = pub.GetComponent<Employment>().toggleActivated();
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
     * Sets the pub object this popup is displaying information on.
     */
    public void setPub(GameObject pub)
    {
        this.pub = pub;
    }
}
