﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Uses A* to form a path which the orc emigrant will then use to leave the city.
 */
public class Emigrate : MonoBehaviour {
    //TODO: If a part of its path is destroyed at any point in time, it should find a new path.
    private GameObject[,] network;
    private Vector2 exitLocation;
    private List<Vector2> path;
    public float stepSize;
    private bool changePath;
    private bool runningAStar;

    /**
     * Instantiates the information necessary for the orc to find its way to its new house.
     */
    void Start()
    {
        path = new List<Vector2>();
        changePath = false;
        runningAStar = false;
        GameObject world = GameObject.Find("WorldInformation");
        World myWorld = world.GetComponent<World>();
        exitLocation = myWorld.exitLocation;
        GameObject[,] structureArr = myWorld.constructNetwork.getConstructArr();
        GameObject[,] terrainArr = myWorld.terrainNetwork.getTerrainArr();
        network = new GameObject[myWorld.mapSize, myWorld.mapSize];
        for (int i = 0; i < network.GetLength(0); i++)
        {
            for (int j = 0; j < network.GetLength(1); j++)
            {
                if (structureArr[i, j] == null && terrainArr[i, j].tag != "Water"
                    && terrainArr[i, j].tag != "Rocks" && terrainArr[i, j].tag != "Trees")
                {
                    network[i, j] = terrainArr[i, j];
                }
                else if (i == Mathf.RoundToInt(gameObject.transform.position.x)
                    && j == Mathf.RoundToInt(gameObject.transform.position.y))
                {
                    //Don't want the house being destroyed in just a moment to be saved to the network array
                    network[i, j] = terrainArr[i, j];
                }
                else
                {
                    network[i, j] = structureArr[i, j];
                }
            }
        }
    }

    // Update is called once per frame
    void Update () {
        if (runningAStar == false)
        {
            if (path == null || path.Count == 0 || changePath == true)
            {
                AstarSearch aStarSearch = new AstarSearch();
                //y in the following line of code is floored to an int in case I decide to bump up the position by 0.5 when the agent
                // spawns so that it is walking in the middle of the block (so that it doesn't appear to walk just below the road)
                Vector2 start = new Vector2(Mathf.CeilToInt(gameObject.transform.position.x), Mathf.FloorToInt(gameObject.transform.position.y));
                runningAStar = true;
                StartCoroutine(aStarSearch.aStar(aStarPath => {
                    if (aStarPath != null)
                    {
                        path = aStarPath;
                        runningAStar = false;
                        if (path.Count == 0)
                        {
                            Debug.Log("Spawned orc immigrant had no path to exit.  It has been destroyed.");
                            //TODO: fix the destroy so this script doesn't continue afterwards (Ex. should be destroyed and run into no problems
                            // when a house is created in an impossible location, but it does not.  Might want to run A* check on a house when
                            // it is created and if no path, try again in 10-15s... if no path again, delete the house.)
                            Destroy(gameObject);
                        }
                    }
                }, start, network[Mathf.RoundToInt(exitLocation.x), Mathf.RoundToInt(exitLocation.y)], network));
                changePath = false;
            }
            else
            {
                if (path[0].x == gameObject.transform.position.x && path[0].y == gameObject.transform.position.y)
                {
                    path.RemoveAt(0);
                }
                if (network[Mathf.RoundToInt(path[0].x), Mathf.RoundToInt(path[0].y)].tag != "Ground"
                    && network[Mathf.RoundToInt(path[0].x), Mathf.RoundToInt(path[0].y)].tag != "Road"
                    && network[Mathf.RoundToInt(path[0].x), Mathf.RoundToInt(path[0].y)].tag != "House")
                {
                    changePath = true;
                }
                if (!changePath)
                {
                    //use path to go to the next available vector2 in it
                    Vector2 nextLocation = path[0];
                    Vector2 currentLocation = gameObject.transform.position;
                    //take a step towards the nextLocation
                    Vector2 vector = new Vector2(nextLocation.x - currentLocation.x, nextLocation.y - currentLocation.y);
                    float length = Mathf.Sqrt(vector.x * vector.x + vector.y * vector.y);
                    Vector2 unitVector = new Vector2(vector.x / length, vector.y / length);
                    Vector2 newLocation = new Vector2(currentLocation.x + unitVector.x * stepSize, currentLocation.y
                        + unitVector.y * stepSize);
                    gameObject.transform.position = newLocation;

                    //if the agent gets to the next vector2 then delete it from the path
                    // and go to the next available vector2
                    float distanceBetweenPoints = Mathf.Sqrt((nextLocation.x - gameObject.transform.position.x)
                        * (nextLocation.x - gameObject.transform.position.x) + (nextLocation.y - gameObject.transform.position.y)
                        * (nextLocation.y - gameObject.transform.position.y));
                    if (distanceBetweenPoints < stepSize)
                    {
                        path.RemoveAt(0);
                    }
                    //the agent has arrived at the goal location
                    if (path.Count == 0)
                    {
                        Destroy(gameObject);
                    }
                }
            }
        }
	}
}