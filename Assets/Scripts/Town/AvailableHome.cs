using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Lets orcs know this location has available spaces for to live in.
/// </summary>
public class AvailableHome : MonoBehaviour {
    //when desirability is in, this will need to check the desirability of house.
    //  if it is high enough, a new orc will spawn
    public GameObject immigrant;

    private World myWorld;
    private HouseInformation houseInfo;
    private bool spawningImmigrant;

    /// <summary>
    /// Initializes the AvailableHome
    /// </summary>
    void Start()
    {
        myWorld = GameObject.Find(World.WORLD_INFORMATION).GetComponent<World>();
        houseInfo = gameObject.GetComponent<HouseInformation>();
        spawningImmigrant = false;
    }

    /// <summary>
    /// Lets World know this house is trying to spawn an immigrant if the desirability is high enough and there is available room
    /// </summary>
    void Update () {
        if (!spawningImmigrant)
        {
            //if desirability is high enough, create an orc immigrant at the edge of the map and tell it
            // that this location is available for moving in
            int desirability = houseInfo.getDesirability();
            int houseSize = houseInfo.getHouseSize();
            int numInhabitants = houseInfo.getNumInhabitants();
            int numIncomingOrcs = houseInfo.getNumOrcsMovingIn();
            if (desirability > 75 && (numInhabitants + numIncomingOrcs) < houseSize)
            {
                spawningImmigrant = true;

                myWorld.addHomeToMoveInTo(gameObject);
            }
        }
    }

    /// <summary>
    /// Spawns an immigrant to move in to the house
    /// </summary>
    public void spawnImmigrant()
    {
        spawningImmigrant = false;

        int houseSize = houseInfo.getHouseSize();
        int numInhabitants = houseInfo.getNumInhabitants();
        int numIncomingOrcs = houseInfo.getNumOrcsMovingIn();
        if (immigrant == null)
        {
            immigrant = gameObject.GetComponent<ImmigrantPrefab>().getImmigrant();
        }
        GameObject newImmigrant = Instantiate(immigrant, myWorld.spawnLocation, Quaternion.identity);
        Immigrate immigrate = newImmigrant.GetComponent<Immigrate>();
        immigrate.goalObject = gameObject;
        OrcInformation orcInfo = newImmigrant.GetComponent<OrcInformation>();

        houseInfo.orcsMovingIn(houseSize - (numInhabitants + numIncomingOrcs));
        orcInfo.setOrcCount(houseSize - (numInhabitants + numIncomingOrcs));
    }
}
