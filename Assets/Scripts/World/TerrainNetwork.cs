using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * TerrainNetwork stores terrain segments.
 */
public class TerrainNetwork {
    private GameObject[,] terrainArr;
    private int mapSize = 42;

    public TerrainNetwork()
    {
        terrainArr = new GameObject[mapSize, mapSize];
    }

    /**
     * getTerrainArr gets the terrain multidimensional array
     * @return terrainArr is the array containing all of the terrain elements
     */
    public GameObject[,] getTerrainArr()
    {
        return terrainArr;
    }

    /**
     * Set terrain array inserts a new terrain GameObject into the terrainArr
     * @param x is the x coordinate of the terrain object
     * @param y is the y coordinate of the terrain object
     * @param terrain is the terrain GameObject being inserted into the array
     */
    public void setTerrainArr(int x, int y, GameObject terrain)
    {
        terrainArr[x, y] = terrain;
    }

    /**
     * Remove a terrain element from the terrain array
     * @param x is the x coordinate of the terrain object
     * @param y is the y coordinate of the terrain object
     */
    public void removeFromTerrainArr(int x, int y)
    {
        terrainArr[x, y] = null;
    }
}
