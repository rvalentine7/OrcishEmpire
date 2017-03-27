using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Marketplaces distribute food to the houses around it.
 * TODO: Sends out a worker to houses in circular area surrounding the marketplace.  Every time the worker
 * is sent out, it has a list of houses to travel to.  Upon reaching the house, it checks if they need food and then supplies them with
 * some before moving on to the next house.  The worker uses the supplies of the marketplace to give food to the houses.  If the marketplace
 * runs out of food, the worker returns.
 */
public class Marketplace : MonoBehaviour {
    Storage storage;

	/**
     * Initializes the marketplace.
     */
	void Start () {
        storage = gameObject.GetComponent<Storage>();
    }
	
	/**
     *
     */
	void Update () {
		//spawn a worker to travel to nearby houses if supplies > 0
        //might look at retrieving supplies from other storage locations
	}

    /**
     * Returns information on what is stored at the marketplace.
     */
    void OnMouseDown()
    {
        List<int> meatInfo = storage.getMeatInfo();
        Debug.Log("Meat: " + meatInfo[0] + "/" + meatInfo[1]);
    }
}
