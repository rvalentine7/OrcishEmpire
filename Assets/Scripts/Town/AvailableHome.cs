using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Lets Orcs know this location has available spaces for orcs to live in.
 */
public class AvailableHome : MonoBehaviour {
    //when desirability is in, this will need to check the desirability of the ground
    // underneath it.  if it is high enough, a new orc will spawn
    public GameObject immigrant;

    /**
     * Instantiates a new orc immigrant.
     */
    void Start () {
        //when desirability is added, this will need to be moved to update so that Orcs only come when the
        // desirability is high enough
        GameObject world = GameObject.Find("WorldInformation");
        World myWorld = world.GetComponent<World>();
        GameObject newImmigrant = Instantiate(immigrant, myWorld.spawnLocation, Quaternion.identity);
        Immigrate immigrate = newImmigrant.GetComponent<Immigrate>();
        immigrate.goalObject = gameObject;
    }
	
	// Update is called once per frame
	void Update () {
        //if desirability is high enough, create an orc immigrant at the edge of the map and tell it
        // that this location is available for moving in
        
    }
}
