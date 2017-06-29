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
    public Text fishNum;
    public Text furnitureNum;
    public Text weaponsNum;

    // Use this for initialization
    void Start () {
		
	}

    /**
     * Updates the status and number of goods for the warehouse.
     */
    void Update () {
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
        meatNum.text = "" + storage.getMeatInfo();
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
