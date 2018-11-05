using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TaxCollectorPopup : MonoBehaviour {
    public GameObject taxCollector;
    public Text status;
    public Text employeeNum;
    public Text taxesCollectedNum;

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

        Employment employment = taxCollector.GetComponent<Employment>();
        employeeNum.text = "" + employment.getNumWorkers();
        TaxCollector taxCollectorScript = taxCollector.GetComponent<TaxCollector>();
        if (employment.getNumWorkers() == 0)
        {
            status.text = "This building is unable to collect taxes with no workers";
        }
        else if (taxCollectorScript.getCollectorStatus() == true)
        {
            status.text = "An orc is currently collecting taxes from nearby houses";
        }
        else
        {
            status.text = "An orc is getting ready to collect taxes from nearby houses";
        }
        taxesCollectedNum.text = "" + taxCollectorScript.getTaxesCollected();
    }

    /**
     * Removes the game object from the game.
     */
    public void destroyObject()
    {
        Destroy(gameObject);
    }

    /**
     * Sets the reservoir object this popup is displaying information on.
     * @param reservoir the reservoir the popup is displaying information on
     */
    public void setTaxCollector(GameObject taxCollector)
    {
        this.taxCollector = taxCollector;
    }
}
