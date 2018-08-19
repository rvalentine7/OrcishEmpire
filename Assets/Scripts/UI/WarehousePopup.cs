using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WarehousePopup : MonoBehaviour {
    public GameObject warehouse;
    public Text status;
    public Text employeeNum;
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

    // Use this for initialization
    void Start () {
		
	}

    /**
     * Updates the status and number of goods for the warehouse.
     */
    void Update () {
        if (Input.GetKey(KeyCode.Escape))
        {
            Destroy(gameObject);
        }
        Storage storage = warehouse.GetComponent<Storage>();
        Employment employment = warehouse.GetComponent<Employment>();
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
        ironNum.text = "" + storage.getIronCount();
        lumberNum.text = "" + storage.getLumberCount();
        weaponsNum.text = "" + storage.getWeaponCount();
        furnitureNum.text = "" + storage.getFurnitureCount();
        hopsNum.text = "" + storage.getHopsCount();
        beerNum.text = "" + storage.getBeerCount();
        ochreNum.text = "" + storage.getOchreCount();
        warPaintNum.text = "" + storage.getWarPaintCount();
        treasureNum.text = "";
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
