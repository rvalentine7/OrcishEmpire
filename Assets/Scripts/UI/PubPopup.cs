using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PubPopup : MonoBehaviour {
    private GameObject pub;
    public Text status;
    public Text employeeNum;
    public Text sickEmployeeNum;
    public Text storageCapacity;
    public Text beerNum;
    public Text timeLeftNum;
    public Button activateButton;
    public Sprite activateSprite;
    public Sprite deactivateSprite;
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
        sickEmployeeNum.text = "" + (employment.getNumWorkers() - employment.getNumHealthyWorkers()) + "/" + employment.getNumWorkers();
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
