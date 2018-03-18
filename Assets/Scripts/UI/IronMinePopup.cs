using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IronMinePopup : MonoBehaviour {
    public GameObject ironMine;
    public Text status;
    public Text employeeNum;
    public Text progressNum;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKey(KeyCode.Escape))
        {
            Destroy(gameObject);
        }
        Production thisIronMine = ironMine.GetComponent<Production>();
        progressNum.text = "" + thisIronMine.getProgressNum() + "/100";
        Employment employment = ironMine.GetComponent<Employment>();
        employeeNum.text = "" + employment.getNumWorkers() + "/" + employment.getWorkerCap();
        if (employment.getNumWorkers() == 0)
        {
            status.text = "Without any employees, this mine cannot produce any iron.";
        }
        else if (employment.getWorkerCap() > employment.getNumWorkers())
        {
            status.text = "This mine is producing slowly due to a lack of employees.";
        }
        else
        {
            status.text = "This mine is mining iron at peak efficiency.";
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
    public void setIronMine(GameObject ironMine)
    {
        this.ironMine = ironMine;
    }
}
