using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BreweryPopup : MonoBehaviour {
    public GameObject brewery;
    public Text status;
    public Text employeeNum;
    public Text storageCapacity;
    public Text hopsNum;
    public Text progressNum;
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
        ItemProduction thisBrewery = brewery.GetComponent<ItemProduction>();
        progressNum.text = "" + thisBrewery.getProgressNum() + "/100";
        Employment employment = brewery.GetComponent<Employment>();
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
        Storage storage = brewery.GetComponent<Storage>();
        storageCapacity.text = "" + storage.getCurrentAmountStored() + "/" + storage.getStorageMax();
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
     * Toggles the building on/off
     */
    public void toggleActivate()
    {
        bool activated = brewery.GetComponent<Employment>().toggleActivated();
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
     * Sets the brewery object this popup is displaying information on.
     * @param brewery the brewery the popup is displaying information on
     */
    public void setBrewery(GameObject brewery)
    {
        this.brewery = brewery;
    }
}
