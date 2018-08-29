﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Used to supply water to fountains and mud baths
 */
public class Reservoir : MonoBehaviour {
    public Sprite filledSprite;
    public Sprite filled1N;
    public Sprite filled1S;
    public Sprite filled1W;
    public Sprite filled1E;
    public Sprite filled2NS;
    public Sprite filled2NW;
    public Sprite filled2NE;
    public Sprite filled2SW;
    public Sprite filled2SE;
    public Sprite filled2WE;
    public Sprite filled3NSE;
    public Sprite filled3NSW;
    public Sprite filled3NWE;
    public Sprite filled3SWE;
    public Sprite filled4;
    public Sprite emptySprite;
    public Sprite empty1N;
    public Sprite empty1S;
    public Sprite empty1W;
    public Sprite empty1E;
    public Sprite empty2NS;
    public Sprite empty2NW;
    public Sprite empty2NE;
    public Sprite empty2SW;
    public Sprite empty2SE;
    public Sprite empty2WE;
    public Sprite empty3NSE;
    public Sprite empty3NSW;
    public Sprite empty3NWE;
    public Sprite empty3SWE;
    public Sprite empty4;

    private GameObject world;
    private World myWorld;
    //private GameObject[,] structureArr;
    private GameObject[,] terrainArr;
    private int width;
    private int height;
    private Vector2 reservoirPosition;
    private bool nextToWater;
    private List<GameObject> fillSources;//other reservoirs/aqueducts that help fill this one

    private void Awake()
    {
        world = GameObject.Find("WorldInformation");
        myWorld = world.GetComponent<World>();
        //structureArr = myWorld.constructNetwork.getConstructArr();
        terrainArr = myWorld.terrainNetwork.getTerrainArr();
        width = (int) gameObject.GetComponent<BoxCollider2D>().size.x;
        height = (int) gameObject.GetComponent<BoxCollider2D>().size.y;
        reservoirPosition = gameObject.transform.position;
        nextToWater = false;
        fillSources = new List<GameObject>();
    }

    /**
     * Sets the initial sprite for the reservoir
     */
    void Start () {
        for (int i = 0; i < width; i++)
        {
            //Mathf.CeilToInt(width / 2.0f - 1) finds the middle square and then that is subtracted from x to get to the edge to start checking the structure array
            //Left side
            if ((int)reservoirPosition.x - Mathf.CeilToInt(width / 2.0f - 1) - 1 > 0
                && terrainArr[(int)reservoirPosition.x - Mathf.CeilToInt(width / 2.0f - 1) - 1, (int)reservoirPosition.y - Mathf.CeilToInt(height / 2.0f - 1) + i] != null
                && (terrainArr[(int)reservoirPosition.x - Mathf.CeilToInt(width / 2.0f - 1) - 1, (int)reservoirPosition.y - Mathf.CeilToInt(height / 2.0f - 1) + i].tag == "Water"))
            {
                nextToWater = true;
            }
            //Right side
            else if ((int)reservoirPosition.x - Mathf.CeilToInt(width / 2.0f - 1) + width < myWorld.mapSize - 1
                && terrainArr[(int)reservoirPosition.x - Mathf.CeilToInt(width / 2.0f - 1) + width, (int)reservoirPosition.y - Mathf.CeilToInt(height / 2.0f - 1) + i] != null
                && (terrainArr[(int)reservoirPosition.x - Mathf.CeilToInt(width / 2.0f - 1) + width, (int)reservoirPosition.y - Mathf.CeilToInt(height / 2.0f - 1) + i].tag == "Water"))
            {
                nextToWater = true;
            }
            //Top side
            else if ((int)reservoirPosition.y - Mathf.CeilToInt(height / 2.0f - 1) + height < myWorld.mapSize - 1
                && terrainArr[(int)reservoirPosition.x - Mathf.CeilToInt(width / 2.0f - 1) + i, (int)reservoirPosition.y - Mathf.CeilToInt(height / 2.0f - 1) + height] != null
                && (terrainArr[(int)reservoirPosition.x - Mathf.CeilToInt(width / 2.0f - 1) + i, (int)reservoirPosition.y - Mathf.CeilToInt(height / 2.0f - 1) + height].tag == "Water"))
            {
                nextToWater = true;
            }
            //Bottom side
            else if ((int)reservoirPosition.y - Mathf.CeilToInt(height / 2.0f - 1) - 1 > 0
                && terrainArr[(int)reservoirPosition.x - Mathf.CeilToInt(width / 2.0f - 1) + i, (int)reservoirPosition.y - Mathf.CeilToInt(height / 2.0f - 1) - 1] != null
                && (terrainArr[(int)reservoirPosition.x - Mathf.CeilToInt(width / 2.0f - 1) + i, (int)reservoirPosition.y - Mathf.CeilToInt(height / 2.0f - 1) - 1].tag == "Water"))
            {
                nextToWater = true;
            }
        }
        if (!nextToWater)
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = emptySprite;
        }
    }
	
	/**
     * Updates the appearance of the reservoir and determines if it is filled
     */
	void Update () {
		//TODO: upon being filled, this should also supply water to all nearby terrain tiles
        //TODO: upon being emptied, this should stop supplying water to all nearby terrain tiles
        //TODO: have public methods for dealing with aqueducts.  use enums for determining which sprite should be displayed?
	}

    /**
     * Adds a reservoir to the list of reservoirs helping fill this reservoir with water
     * @param the reservoir that is helping fill this reservoir
     */
    public void addFillSources(GameObject reservoir)
    {
        fillSources.Add(reservoir);
    }

    /**
     * Removes a reservoir from the list of reservoirs helping fill this reservoir with water
     * @param the reservoir that is no longer helping to fill this reservoir
     */
    public void removeFillSources(GameObject reservoir)
    {
        fillSources.Remove(reservoir);
    }

    /**
     * Gets whether the reservoir is next to water and can fill other reservoirs/aqueducts
     * @return whether the reservoir is next to water
     */
    public bool getNextToWater()
    {
        return nextToWater;
    }

    /**
     * Gets whether the reservoir is filled with water for the purposes of filling fountains/reservoirs/aqueducts
     * @return whether the reservoir is filled
     */
    public bool getFilled()
    {
        return fillSources.Count > 0;
    }
}
