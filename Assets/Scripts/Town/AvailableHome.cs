using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Lets orcs know this location has available spaces for to live in.
 */
public class AvailableHome : MonoBehaviour {
    //when desirability is in, this will need to check the desirability of house.
    //  if it is high enough, a new orc will spawn
    public GameObject immigrant;
	
	/**
     * Spawns orc immigrants if the house has high enough desirability.
     */
	void Update () {
        //if desirability is high enough, create an orc immigrant at the edge of the map and tell it
        // that this location is available for moving in
        HouseInformation houseInfo = gameObject.GetComponent<HouseInformation>();
        int desirability = houseInfo.getDesirability();
        int houseSize = houseInfo.getHouseSize();
        int numInhabitants = houseInfo.getNumInhabitants();
        int numIncomingOrcs = houseInfo.getNumOrcsMovingIn();
        if (desirability > 75 && (numInhabitants + numIncomingOrcs) < houseSize)
        {
            GameObject world = GameObject.Find("WorldInformation");
            World myWorld = world.GetComponent<World>();
            GameObject newImmigrant = Instantiate(immigrant, myWorld.spawnLocation, Quaternion.identity);
            Immigrate immigrate = newImmigrant.GetComponent<Immigrate>();
            immigrate.goalObject = gameObject;
            OrcInformation orcInfo = newImmigrant.GetComponent<OrcInformation>();

            if (houseSize - (numInhabitants + numIncomingOrcs) >= 3)
            {
                houseInfo.orcsMovingIn(3);
                orcInfo.setOrcCount(3);
            } else if (houseSize - (numInhabitants + numIncomingOrcs) == 2)
            {
                houseInfo.orcsMovingIn(2);
                orcInfo.setOrcCount(2);
            } else
            {
                houseInfo.orcsMovingIn(1);
                orcInfo.setOrcCount(1);
            }
        }
        
    }
}
