using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WarPaintWorkshopPopup : MonoBehaviour {
    public GameObject warPaintWorkshop;
    public Text status;
    public Text employeeNum;
    public Text ochreNum;
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
        ItemProduction warPaintProduction = warPaintWorkshop.GetComponent<ItemProduction>();
        progressNum.text = "" + warPaintProduction.getProgressNum() + "/100";
        Employment employment = warPaintWorkshop.GetComponent<Employment>();
        employeeNum.text = "" + employment.getNumWorkers() + "/" + employment.getWorkerCap();
        Storage storage = warPaintWorkshop.GetComponent<Storage>();
        ochreNum.text = "" + storage.getOchreCount();
        if (employment.getNumWorkers() == 0)
        {
            status.text = "Without any employees, this workshop cannot produce any war paint.";
        }
        else if (employment.getWorkerCap() > employment.getNumWorkers())
        {
            status.text = "This workshop is producing slowly due to a lack of employees.";
        }
        else
        {
            status.text = "This workshop is producing war paint at peak efficiency.";
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
     * Sets the war paint workshop object this popup is displaying information on.
     */
    public void setWarpaintWorkshop(GameObject warPaintWorkshop)
    {
        this.warPaintWorkshop = warPaintWorkshop;
    }
}
