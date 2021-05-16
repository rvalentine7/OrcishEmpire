using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls information specific to each grid square
/// </summary>
public class Tile : MonoBehaviour {
    private GameObject world;
    protected World myWorld;
    private GameObject[,] structureArr;
    private int numWaterPipes;//from reservoirs with the purpose of being used by fountains to supply water (numWaterSources)
    private int numWaterSources;//from wells/fountains
    private int desirability;

    //Sprites this tile can appear as
    public List<Sprite> spriteChoices;

    // Use this for initialization
    public void Start () {
        world = GameObject.Find(World.WORLD_INFORMATION);
        myWorld = world.GetComponent<World>();
        structureArr = myWorld.constructNetwork.getConstructArr();
        numWaterPipes = 0;
        numWaterSources = 0;

        //Adds the tile to the terrain array.
        Vector2 terrainPos = transform.position;
        myWorld.terrainNetwork.setTerrainArr((int)terrainPos.x, (int)terrainPos.y, gameObject);

        //Assigns a random sprite from the spriteChoices
        if (spriteChoices.Count > 0)
        {
            SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
            int groundSpriteNum = Mathf.FloorToInt(Random.value * spriteChoices.Count);
            spriteRenderer.sprite = spriteChoices[groundSpriteNum];
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    /**
     * Used by filled reservoirs to add water sources to tiles.  Buildings use 
     * the water source information to determine if the building has access to water
     * @param waterSource the reservoir supplying water to the tile
     */
    public void addWaterPipes(GameObject waterSource)
    {
        if (numWaterPipes == 0)
        {
            //updates the building this tile is on to have water
            structureArr = myWorld.constructNetwork.getConstructArr();
            if (structureArr[(int) gameObject.transform.position.x, (int) gameObject.transform.position.y] != null)
            {
                if (structureArr[(int)gameObject.transform.position.x, (int)gameObject.transform.position.y].tag == World.BUILDING
                    && (structureArr[(int)gameObject.transform.position.x, (int)gameObject.transform.position.y].GetComponent<Fountain>() != null
                    || structureArr[(int)gameObject.transform.position.x, (int)gameObject.transform.position.y].GetComponent<MudBath>() != null))
                {
                    structureArr[(int)gameObject.transform.position.x, (int)gameObject.transform.position.y].GetComponent<Employment>().addWaterSource();
                }
            }
        }
        numWaterPipes++;
    }

    /**
     * Used by a reservoir to inform a tile that it will no longer be receiving water
     * from the reservoir.
     * @param waterSource the reservoir that will no longer be supplying water to the tile
     */
    public void removeWaterPipes(GameObject waterSource)
    {
        //Debug.Log("removing pipes. num: " + numWaterPipes);
        if (numWaterPipes == 1)
        {
            //updates the building this tile is on to remove a water source
            structureArr = myWorld.constructNetwork.getConstructArr();
            if (structureArr[(int)gameObject.transform.position.x, (int)gameObject.transform.position.y] != null)
            {
                if (structureArr[(int)gameObject.transform.position.x, (int)gameObject.transform.position.y].tag == World.BUILDING
                    && (structureArr[(int)gameObject.transform.position.x, (int)gameObject.transform.position.y].GetComponent<Fountain>() != null
                    || structureArr[(int)gameObject.transform.position.x, (int)gameObject.transform.position.y].GetComponent<MudBath>() != null))
                {
                    structureArr[(int)gameObject.transform.position.x, (int)gameObject.transform.position.y].GetComponent<Employment>().removeWaterSource();
                }
            }
        }
        numWaterPipes--;
    }

    /**
     * Whether the tile has a source of water
     * @return whether the tile has a source of water
     */
    public bool hasPipes()
    {
        return numWaterPipes > 0;
    }

    /**
     * Adds a source of water from a fountain/well
     */
    public void addWaterSource()
    {
        numWaterSources++;
    }
    
    /**
     * Removes a source of water from a fountain/well
     */
    public void removeWaterSource()
    {
        numWaterSources--;
    }

    /**
     * Whether the tile has water from a fountain/well
     * @return whether the tile has water from at least 1 fountain/well
     */
    public bool hasWater()
    {
        return numWaterSources > 0;
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
