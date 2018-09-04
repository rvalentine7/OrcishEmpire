using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadInformation : MonoBehaviour {
    private GameObject aqueduct;

    private void Awake()
    {
        aqueduct = null;
    }

    // Use this for initialization
    void Start () {
        
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    /**
     * Sets the aqueduct that is over this road object
     * @param the aqueduct to go over this road
     */
    public void setAqueduct(GameObject aqueduct)
    {
        this.aqueduct = aqueduct;
    }

    /**
     * Gets the aqueduct over this road object
     * @return the aqueduct over this road object if there is one, null otherwise
     */
    public GameObject getAqueduct()
    {
        return this.aqueduct;
    }

    /**
     * Destroys the aqueduct if there is one on top of the road, otherwise destroy the road
     */
    public void destroyRoad()
    {
        if (this.aqueduct != null)
        {
            aqueduct.GetComponent<Employment>().destroyEmployment();
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
