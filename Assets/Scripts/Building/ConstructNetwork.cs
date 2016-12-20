using UnityEngine;
using System.Collections;

/**
 * BuildingNetwork is used for storing and accessing the roads and buildings contained in the game.
 * Later on I will want the level generator to pass in the map size to the constructor
 */
public class ConstructionNetwork {
    //0, 0 is the bottom left corner
    private GameObject[,] constructArr;
    private int mapSize = 42;

    public ConstructionNetwork()
    {
        constructArr = new GameObject[mapSize, mapSize];
    }

    /**
     * getRoadArr gets the road array
     * @return roadArr is the array containing all of the roads
     */
    public GameObject[,] getConstructArr()
    {
        return constructArr;
    }

    //have an actual setBuildArr (not adding objects to it like the one below)

    /**
     * Set road array inserts a new road GameObject into the roadArr
     * @param y is the y coordinate of the road object
     * @param x is the x coordinate of the road object
     * @param road is the road GameObject being inserted into the array
     */
    public void setConstructArr(int y, int x, GameObject road)
    {
        constructArr[y, x] = road;
    }

    /**
     * Remove a building from the building array
     * @param y is the y coordinate of the road object
     * @param x is the x coordinate of the road object
     */
    public void removeFromBuildArr(int y, int x)
    {
        constructArr[y, x] = null;
    }
}
