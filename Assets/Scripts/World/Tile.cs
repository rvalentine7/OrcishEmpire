using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour {
    private List<GameObject> waterSources;
    private int desirability;

	// Use this for initialization
	void Start () {
        waterSources = new List<GameObject>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    /**
     * Used by filled reservoirs to add water sources to tiles.  Buildings use 
     * the water source information to determine if the building has access to water
     * @param waterSource the reservoir supplying water to the tile
     */
    public void addWaterSource(GameObject waterSource)
    {
        waterSources.Add(waterSource);
    }

    /**
     * Used by a reservoir to inform a tile that it will no longer be receiving water
     * from the reservoir.
     * @param waterSource the reservoir that will no longer be supplying water to the tile
     */
    public void removeWaterSource(GameObject waterSource)
    {
        waterSources.Remove(waterSource);
    }

    /**
     * Whether the tile has a source of water
     * @return whether the tile has a source of water
     */
    public bool hasWater()
    {
        return waterSources.Count > 0;
    }

    /**
     * Gets the desirability level of a particular tile in the game world
     * @return the desirability level
     */
    public int getDesirability()
    {
        return desirability;
    }

    /**
     * Updates the desirability level of the particular terrain tile to help
     * determine if a building should upgrade or if immigrants want to move
     * into the city
     */
    public void updateDesirability(int value)
    {
        desirability += value;
    }
}
