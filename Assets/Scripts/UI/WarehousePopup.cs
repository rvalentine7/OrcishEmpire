using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class WarehousePopup : MonoBehaviour {
    public GameObject warehouse;
    public Text status;
    public Text employeeNum;
    public Text storageCapacity;
    public Text meatNum;
    public Text wheatNum;
    public Text eggsNum;
    public Text fishNum;
    public Text furnitureNum;
    public Text weaponsNum;
    public Text ironNum;
    public Text lumberNum;
    public Text hopsNum;
    public Text beerNum;
    public Text ochreNum;
    public Text warPaintNum;
    public Text treasureNum;
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

    /**
     * Updates the status and number of goods for the warehouse.
     */
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
        Storage storage = warehouse.GetComponent<Storage>();
        storageCapacity.text = "" + storage.getCurrentAmountStored() + "/" + storage.getStorageMax();
        Employment employment = warehouse.GetComponent<Employment>();
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
            status.text = "Without any employees, this warehouse cannot distribute goods.";
        }
        else if (employment.getWorkerCap() > employment.getNumWorkers())
        {
            status.text = "This warehouse is distributing goods slowly due to a lack of workers.";
        }
        else
        {
            status.text = "This warehouse is efficiently distributing goods.";
        }
        employeeNum.text = "" + employment.getNumWorkers() + "/" + employment.getWorkerCap();
        meatNum.text = "" + storage.getMeatCount();
        wheatNum.text = "" + storage.getWheatCount();
        eggsNum.text = "" + storage.getEggCount();
        ironNum.text = "" + storage.getIronCount();
        lumberNum.text = "" + storage.getLumberCount();
        weaponsNum.text = "" + storage.getWeaponCount();
        furnitureNum.text = "" + storage.getFurnitureCount();
        hopsNum.text = "" + storage.getHopsCount();
        beerNum.text = "" + storage.getBeerCount();
        ochreNum.text = "" + storage.getOchreCount();
        warPaintNum.text = "" + storage.getWarPaintCount();
        treasureNum.text = "0";
    }

    /**
     * Toggles the building on/off
     */
    public void toggleActivate()
    {
        bool activated = warehouse.GetComponent<Employment>().toggleActivated();
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
     * Sets the warehouse object this popup is displaying information on.
     */
    public void setWarehouse(GameObject warehouse)
    {
        this.warehouse = warehouse;
    }
}
