using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FurnitureWorkshopPopup : MonoBehaviour {
    public GameObject furnitureWorkshop;
    public Text status;
    public Text employeeNum;
    public Text lumberNum;
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
        ItemProduction thisFurnitureWorkshop = furnitureWorkshop.GetComponent<ItemProduction>();
        progressNum.text = "" + thisFurnitureWorkshop.getProgressNum() + "/100";
        Employment employment = furnitureWorkshop.GetComponent<Employment>();
        employeeNum.text = "" + employment.getNumWorkers() + "/" + employment.getWorkerCap();
        Storage storage = furnitureWorkshop.GetComponent<Storage>();
        lumberNum.text = "" + storage.getLumberCount();
        if (employment.getNumWorkers() == 0)
        {
            status.text = "Without any employees, this workshop cannot produce any furniture.";
        }
        else if (employment.getWorkerCap() > employment.getNumWorkers())
        {
            status.text = "This workshop is producing slowly due to a lack of employees.";
        }
        else
        {
            status.text = "This workshop is producing furniture at peak efficiency.";
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
     * Sets the farm object this popup is displaying information on.
     */
    public void setFurnitureWorkshop(GameObject furnitureWorkshop)
    {
        this.furnitureWorkshop = furnitureWorkshop;
    }
}
