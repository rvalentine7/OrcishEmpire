using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Sends a delivery orc from its current location to the goal where it will deliver goods
 * before returning to its original location.
 */
public class Delivery : MonoBehaviour {
    private Dictionary<string, int> resources;
    private GameObject[,] network;
    private Vector2 originalLocation;
    private GameObject placeOfEmployment;
    private List<Vector2> path;
    private Vector2 goal;
    private GameObject goalObject;
    private bool reachedGoal;
    private bool changePath;
    private bool headingHome;
    private GameObject world;
    private World myWorld;
    private GameObject[,] structureArr;
    private GameObject[,] terrainArr;
    public float stepSize;
    public int searchRadius;

    /**
     * Initializes the delivery class
     */
    void Awake ()
    {
        resources = new Dictionary<string, int>();
        originalLocation = new Vector2();
        reachedGoal = false;
        changePath = false;
        headingHome = false;
        world = GameObject.Find("WorldInformation");
        myWorld = world.GetComponent<World>();
        structureArr = myWorld.constructNetwork.getConstructArr();
        terrainArr = myWorld.terrainNetwork.getTerrainArr();
    }

    /**
     * Attempts to find the first place to go to
     */
    void Start () {
        //if there is nowhere to go, wait some time before trying again?
        path = findPathToStorage();
    }
	
	/**
     * Plans out the movement and delivery of resources for the delivery orc
     */
	void Update () {
        //if the place of employment is destroyed, this gameobject should be as well
        if (!placeOfEmployment)
        {
            Destroy(gameObject);
        }
        if (path.Count == 0 || changePath == true)
        {
            if (!reachedGoal)
            {
                path = findPathToStorage();
                //if there are no storage locations, and the orc isn't at its place of employment,
                // send it back to its place of employment
                float distanceBetweenPoints = Mathf.Sqrt((originalLocation.x - gameObject.transform.position.x)
                    * (originalLocation.x - gameObject.transform.position.x) + (originalLocation.y - gameObject.transform.position.y)
                    * (originalLocation.y - gameObject.transform.position.y));
                if (path.Count == 0 && distanceBetweenPoints > 0.5f)
                {
                    path = findPathHome();
                    headingHome = true;
                }
            }
            else
            {
                path = findPathHome();
            }
            changePath = false;
        }
        else
        {
            //use path to go to the next available vector2 in it
            Vector2 currentLocation = gameObject.transform.position;
            if (path[0] == currentLocation)
            {
                path.RemoveAt(0);
            }
            Vector2 nextLocation = path[0];
            //if the orc is heading home or the goalobject exists, take a step; otherwise, change the path
            if ((goalObject != null || headingHome || reachedGoal) && (network[(int)nextLocation.x, (int)nextLocation.y] != null
                && (network[(int)nextLocation.x, (int)nextLocation.y].tag != "Building"
                || network[(int)nextLocation.x, (int)nextLocation.y] == goalObject)))
            {
                //take a step towards the nextLocation
                Vector2 vector = new Vector2(nextLocation.x - currentLocation.x, nextLocation.y - currentLocation.y);
                float magnitude = Mathf.Sqrt(vector.x * vector.x + vector.y * vector.y);
                Vector2 unitVector = new Vector2(vector.x / magnitude, vector.y / magnitude);
                Vector2 newLocation = new Vector2(currentLocation.x + unitVector.x * stepSize, currentLocation.y
                    + unitVector.y * stepSize);
                gameObject.transform.position = newLocation;

                //if the agent gets to the next vector then delete it from the path
                // and go to the next available vector
                float distanceBetweenPoints = Mathf.Sqrt((nextLocation.x - gameObject.transform.position.x)
                    * (nextLocation.x - gameObject.transform.position.x) + (nextLocation.y - gameObject.transform.position.y)
                    * (nextLocation.y - gameObject.transform.position.y));
                bool nextIsGoal = false;
                if (nextLocation == goal)
                {
                    nextIsGoal = true;
                }
                if (distanceBetweenPoints < stepSize)
                {
                    path.RemoveAt(0);
                }
                if (path.Count == 0)
                {
                    //if the orc is at the storage goal, deliver resources
                    if (nextIsGoal)
                    {
                        //at the goal, deliver any available goods and then plan a path back
                        Storage storage = goalObject.GetComponent<Storage>();
                        List<string> removeTheseResources = new List<string>();
                        foreach (KeyValuePair<string, int> kvp in resources)
                        {
                            if (storage.acceptsResource(kvp.Key, kvp.Value))
                            {
                                storage.addResource(kvp.Key, kvp.Value);
                                removeTheseResources.Add(kvp.Key);
                            }
                        }
                        //update resources to remove any resources that were given to a storage building
                        foreach (string resourceToRemove in removeTheseResources)
                        {
                            resources.Remove(resourceToRemove);
                        }
                        //if this unit still has resources, it should try to get rid of them
                        if (resources.Count > 0)
                        {
                            changePath = true;
                        }
                        //if the unit is out of resources, it should head back to its place of employment
                        else
                        {
                            reachedGoal = true;
                        }
                        path = findPathHome();
                    }
                    //if the orc has arrived back at its employment from delivering resources, let the employment know
                    else if (reachedGoal)
                    {
                        Employment employment = placeOfEmployment.GetComponent<Employment>();
                        employment.setWorkerDeliveringGoods(false);
                        Destroy(gameObject);
                    }
                    //if the previous delivery location was destroyed and the orc had to return home, it should
                    // be updated to no longer be considering "heading home" and it should look for new places
                    // to deliver resources to
                    else
                    {
                        headingHome = false;
                        changePath = true;
                    }
                }
            }
            else
            {
                changePath = true;
            }
        }
	}

    /**
     * Finds a place for the delivery orc to go to and creates a path to it.
     */
    private List<Vector2> findPathToStorage()
    {
        network = new GameObject[myWorld.mapSize, myWorld.mapSize];
        for (int i = 0; i < network.GetLength(0); i++)
        {
            for (int j = 0; j < network.GetLength(1); j++)
            {
                if (structureArr[i, j] == null)
                {
                    network[i, j] = terrainArr[i, j];
                }
                //delivery workers should not travel over houses during their trip.  as such,
                // houses are not included in the network
                else if (structureArr[i, j].tag != "House")
                {
                    network[i, j] = structureArr[i, j];
                }
            }
        }
        List<List<Vector2>> possiblePaths = new List<List<Vector2>>();
        //checks for the closest warehouse
        List<GameObject> discoveredDeliveryLocs = new List<GameObject>();
        for (int i = 0; i <= searchRadius * 2; i++)
        {
            for (int j = 0; j <= searchRadius * 2; j++)
            {
                if (originalLocation.x - searchRadius + i >= 0 && originalLocation.y - searchRadius + j >= 0
                        && originalLocation.x - searchRadius + i <= 39 && originalLocation.y - searchRadius + j <= 39
                        && structureArr[(int)originalLocation.x - searchRadius + i,
                        (int)originalLocation.y - searchRadius + j] != null
                        && structureArr[(int)originalLocation.x - searchRadius + i,
                        (int)originalLocation.y - searchRadius + j].tag == "Building"
                        && !discoveredDeliveryLocs.Contains(structureArr[(int)originalLocation.x - searchRadius + i,
                        (int)originalLocation.y - searchRadius + j])
                        && structureArr[(int)originalLocation.x - searchRadius + i,
                        (int)originalLocation.y - searchRadius + j].GetComponent<Employment>().getNumWorkers() > 0
                        && structureArr[(int)originalLocation.x - searchRadius + i,
                        (int)originalLocation.y - searchRadius + j].GetComponent<Storage>() != null
                        && structureArr[(int)originalLocation.x - searchRadius + i,
                        (int)originalLocation.y - searchRadius + j].GetComponent<Storage>().storageType.Equals("Warehouse"))
                {
                    //final check to make sure this location will take the delivery orc's goods
                    bool acceptsMyResources = false;
                    foreach (KeyValuePair<string, int> resource in resources)
                    {
                        if (structureArr[(int)originalLocation.x - searchRadius + i,
                        (int)originalLocation.y - searchRadius + j].GetComponent<Storage>().acceptsResource(resource.Key, resource.Value))
                        {
                            acceptsMyResources = true;
                        }
                    }
                    if (acceptsMyResources)
                    {
                        discoveredDeliveryLocs.Add(structureArr[(int)originalLocation.x - searchRadius + i,
                        (int)originalLocation.y - searchRadius + j]);
                        AstarSearch aStarSearch = new AstarSearch();
                        Vector2 currentLocation = new Vector2(Mathf.RoundToInt(gameObject.transform.position.x),
                            Mathf.RoundToInt(gameObject.transform.position.y));
                        List<Vector2> tempPath = aStarSearch.aStar(currentLocation, structureArr[(int)originalLocation.x - searchRadius + i,
                            (int)originalLocation.y - searchRadius + j], network);
                        if (tempPath.Count > 0)
                        {
                            possiblePaths.Add(tempPath);
                        }
                    }
                }
            }
        }
        //check for other storage-type buildings if there are no available warehouses (market, grainery, etc)
        if (possiblePaths.Count == 0)
        {

        }
        //if there are any possible storage buildings to deliver to, select the one with the shortest path
        if (possiblePaths.Count > 0)
        {
            List<Vector2> shortestPath = new List<Vector2>();
            int shortestPathCount = int.MaxValue;
            foreach (List<Vector2> possiblePath in possiblePaths)
            {
                if (possiblePath.Count < shortestPathCount)
                {
                    shortestPath = possiblePath;
                    shortestPathCount = possiblePath.Count;
                }
            }
            goal = shortestPath[shortestPathCount - 1];
            goalObject = network[(int)goal.x, Mathf.RoundToInt(goal.y)];
            return shortestPath;
        }
        return new List<Vector2>();
    }

    /**
     * Finds a way back to the building that spawned the delivery orc.
     * @return the path back to the original location
     */
    private List<Vector2> findPathHome()
    {
        network = new GameObject[myWorld.mapSize, myWorld.mapSize];
        for (int i = 0; i < network.GetLength(0); i++)
        {
            for (int j = 0; j < network.GetLength(1); j++)
            {
                if (structureArr[i, j] == null)
                {
                    network[i, j] = terrainArr[i, j];
                }
                else if (structureArr[i, j].tag != "House")
                {
                    network[i, j] = structureArr[i, j];
                }
            }
        }
        AstarSearch aStarSearch = new AstarSearch();
        //going from the goal destination back to the original location
        Vector2 location = gameObject.transform.position;
        return aStarSearch.aStar(new Vector2(Mathf.RoundToInt(location.x), Mathf.RoundToInt(location.y)),
            network[(int)originalLocation.x, (int)originalLocation.y], network);
    }

    /**
     * Adds to the resources the delivery orc is carrying.
     * @param resourceName is the name of the resource
     * @param num is the number of a type of resource being carried
     */
    public void addResources(string resourceName, int num)
    {
        if (resources.ContainsKey(resourceName))
        {
            resources[resourceName] += num;
        }
        else
        {
            resources.Add(resourceName, num);
        }
    }

    /**
     * Removes a resource from the delivery orc.
     * @param resourceName the resource to be removed
     * @param num the number of the specific resource to remove
     */
    public void removeResources(string resourceName, int num)
    {
        if (resources.ContainsKey(resourceName))
        {
            resources.Remove(resourceName);
        }
        else
        {
            Debug.Log("There has been a call to remove a nonexistant resource from a delivery orc.");
        }
    }

    /**
     * Sets the place the delivery orc starts at.
     * @param position is the position the delivery orc spawned in at
     */
    public void setOriginalLocation(Vector2 position)
    {
        originalLocation = position;
    }

    /**
     * Sets the place of employment this delivery orc works for.
     * @param employment the place of employment
     */
    public void setOrcEmployment(GameObject employment)
    {
        placeOfEmployment = employment;
    }
}
