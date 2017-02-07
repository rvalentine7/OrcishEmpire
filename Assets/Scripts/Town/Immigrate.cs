using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * See note at the beginning of Update() in BuildingPlacement.cs
 */
public class Immigrate : MonoBehaviour {
    //If the home is destroyed, it will need to find a nearest exit off the map.
    //If a part of it's path is destroyed at any point in time, it should find a new path.
    private GameObject[,] network;
    public GameObject goalObject;
    private List<Vector2> path;
    public float stepSize;

    /**
     * Instantiates the information necessary for the orc to find its way to its new house.
     */
    void Start () {
        path = new List<Vector2>();
        GameObject world = GameObject.Find("WorldInformation");
        World myWorld = world.GetComponent<World>();
        GameObject[,] structureArr = myWorld.constructNetwork.getConstructArr();
        GameObject[,] terrainArr = myWorld.terrainNetwork.getTerrainArr();
        network = new GameObject[myWorld.mapSize, myWorld.mapSize];
        for (int i = 0; i < network.GetLength(0); i++)
        {
            for (int j = 0; j < network.GetLength(1); j++)
            {
                if (network[i, j] == null)
                {
                    network[i, j] = terrainArr[i, j];
                }
                else
                {
                    network[i, j] = structureArr[i, j];
                }
            }
        }
    }

    /**
     * Update has the orc immigrant seek out its house destination.
     */
    void Update () {
        if (goalObject != null)
        {
            Vector2 goal = goalObject.transform.position;
            if (path.Count == 0)
            {
                AstarSearch aStarSearch = new AstarSearch();
                //y in the following line of code is floored to an int in case I decide to bump up the position by 0.5 when the agent
                // spawns so that it is walking in the middle of the block (so that it doesn't appear to walk just below the road)
                Vector2 start = new Vector2(gameObject.transform.position.x, Mathf.FloorToInt(gameObject.transform.position.y));
                path = aStarSearch.aStar(start, goal, network);
                if (path.Count == 0)
                {
                    Debug.Log("Spawned orc immigrant had no path to house.  It has been destroyed.");
                    Destroy(gameObject);
                }
            }
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
                HouseInformation houseInfo = goalObject.GetComponent<HouseInformation>();
                houseInfo.addInhabitant();
                Destroy(gameObject);
            }
        }
    }
}
