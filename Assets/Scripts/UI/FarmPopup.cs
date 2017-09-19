using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FarmPopup : MonoBehaviour {
    public GameObject farm;
    public Text status;
    public Text employeeNum;
    public Text progressNum;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        Production pigFarm = farm.GetComponent<Production>();//would need to be made more general to apply for more farms
        //can do this by making the pigfarm class a more general "farm" type and set any special fields (such as produce type)
        //through public variables on the prefab
        progressNum.text = "" + pigFarm.getProgressNum() + "/100";
        Employment employment = farm.GetComponent<Employment>();
        employeeNum.text = "" + employment.getNumWorkers() + "/" + employment.getWorkerCap();
        if (employment.getNumWorkers() == 0)
        {
            status.text = "Without any employees, this farm cannot produce food.";
        }
        else if (employment.getWorkerCap() > employment.getNumWorkers())
        {
            status.text = "This farm is producing slowly due to a lack of employees.";
        }
        else
        {
            status.text = "This farm is producing food at peak efficiency.";
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
    public void setFarm(GameObject farm)
    {
        this.farm = farm;
    }
}
