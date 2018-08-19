using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LumberMillPopup : MonoBehaviour {
    public GameObject lumberMill;
    public Text status;
    public Text employeeNum;
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
        Production lumberMillProduction = lumberMill.GetComponent<Production>();
        progressNum.text = "" + lumberMillProduction.getProgressNum() + "/100";
        Employment employment = lumberMill.GetComponent<Employment>();
        employeeNum.text = "" + employment.getNumWorkers() + "/" + employment.getWorkerCap();
        if (employment.getNumWorkers() == 0)
        {
            status.text = "Without any employees, this mill cannot produce any lumber.";
        }
        else if (employment.getWorkerCap() > employment.getNumWorkers())
        {
            status.text = "This mill is producing slowly due to a lack of employees.";
        }
        else
        {
            status.text = "This mill is harvesting lumber at peak efficiency.";
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
     * Sets the lumber mill object this popup is displaying information on.
     */
    public void setLumberMill(GameObject lumberMill)
    {
        this.lumberMill = lumberMill;
    }
}
