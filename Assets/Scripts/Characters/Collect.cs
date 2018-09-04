﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Collect sends the orc to take a resource from another building and return it to the building the orc
 *  originated from.
 */
public class Collect : MonoBehaviour {
    private Dictionary<string, int> resources;
    private string resourcesToCollect;
    //private int carryingCapacity;
    private int remainingCapacity;
    private GameObject[,] network;
    private Vector2 originalLocation;
    private GameObject placeOfEmployment;
    private List<Vector2> path;
    private Vector2 goal;
    private GameObject goalObject;
    private bool reachedGoal;
    private bool changePath;
    private bool runningAStar;
    private bool headingHome;
    private GameObject world;
    private World myWorld;
    private GameObject[,] structureArr;
    private GameObject[,] terrainArr;
    public float stepSize;
    public int searchRadius;

    /**
     * Initializes the collector class
     */
    void Awake()
    {
        gameObject.GetComponent<SpriteRenderer>().enabled = false;
        resources = new Dictionary<string, int>();
        resourcesToCollect = "";
        //carryingCapacity = 0;
        remainingCapacity = 0;
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
    void Start()
    {
        StartCoroutine(findPathToStorage(returnPath =>
        {
            path = returnPath;
        }));
    }

    /**
     * Runs the collector orc
     */
    void Update () {
        if (runningAStar == false)
        {
            StartCoroutine(runCollect());
        }
    }

    /**
     * Plans out the movement and collection of resources for the collector orc
     */
    private IEnumerator runCollect()
    {
        //if the place of employment is destroyed, this gameobject should be as well
        if (!placeOfEmployment)
        {
            Destroy(gameObject);
        }
        if (path == null || path.Count == 0 || changePath == true)
        {
            if (!reachedGoal && runningAStar == false)
            {
                yield return StartCoroutine(findPathToStorage(returnPath =>
                {
                    path = returnPath;
                }));
                //if there are no storage locations, and the orc isn't at its place of employment,
                // send it back to its place of employment
                float distanceBetweenPoints = Mathf.Sqrt((originalLocation.x - gameObject.transform.position.x)
                    * (originalLocation.x - gameObject.transform.position.x) + (originalLocation.y - gameObject.transform.position.y)
                    * (originalLocation.y - gameObject.transform.position.y));
                if (path != null && path.Count == 0 && distanceBetweenPoints > 0.5f)
                {
                    yield return StartCoroutine(findPathHome(returnPath =>
                    {
                        path = returnPath;
                    }));
                    //if the place of employment is destroyed, this gameobject should be as well
                    if (!placeOfEmployment)
                    {
                        Destroy(gameObject);
                    }
                    headingHome = true;
                }
            }
            else if (reachedGoal && runningAStar == false)
            {
                yield return StartCoroutine(findPathHome(returnPath =>
                {
                    path = returnPath;
                }));
                //if the place of employment is destroyed, this gameobject should be as well
                if (!placeOfEmployment)
                {
                    Destroy(gameObject);
                }
            }
            if (runningAStar == false)
            {
                changePath = false;
            }
        }
        else if (path != null && runningAStar == false)
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
                        //at the goal, collect any available goods
                        Storage storage = goalObject.GetComponent<Storage>();
                        if (resourcesToCollect.Equals("Food") && storage.getFoodCount() > 0)
                        {
                            if (storage.getMeatCount() > 0)
                            {
                                int meatCount = storage.getMeatCount();
                                if (meatCount > remainingCapacity)
                                {
                                    resources.Add("Meat", remainingCapacity);
                                    storage.removeResource("Meat", remainingCapacity);
                                    remainingCapacity = 0;
                                }
                                else
                                {
                                    resources.Add("Meat", meatCount);
                                    storage.removeResource("Meat", meatCount);
                                    remainingCapacity -= meatCount;
                                }
                            }
                            if (storage.getWheatCount() > 0)
                            {
                                int wheatCount = storage.getWheatCount();
                                if (wheatCount > remainingCapacity)
                                {
                                    resources.Add("Wheat", remainingCapacity);
                                    storage.removeResource("Wheat", remainingCapacity);
                                    remainingCapacity = 0;
                                }
                                else
                                {
                                    resources.Add("Wheat", wheatCount);
                                    storage.removeResource("Wheat", wheatCount);
                                    remainingCapacity -= wheatCount;
                                }
                            }
                            //TODO: when more types of food are added, account for those here
                        }
                        if (resourcesToCollect.Equals("Iron") && storage.getIronCount() > 0)
                        {
                            int ironCount = storage.getIronCount();
                            if (ironCount > remainingCapacity)
                            {
                                resources.Add("Iron", remainingCapacity);
                                storage.removeResource("Iron", remainingCapacity);
                                remainingCapacity = 0;
                            }
                            else
                            {
                                resources.Add("Iron", ironCount);
                                storage.removeResource("Iron", ironCount);
                                remainingCapacity -= ironCount;
                            }
                        }
                        if (resourcesToCollect.Equals("Weapon") && storage.getWeaponCount() > 0)
                        {
                            int weaponCount = storage.getWeaponCount();
                            if (weaponCount > remainingCapacity)
                            {
                                resources.Add("Weapon", remainingCapacity);
                                storage.removeResource("Weapon", remainingCapacity);
                                remainingCapacity = 0;
                            }
                            else
                            {
                                resources.Add("Weapon", weaponCount);
                                storage.removeResource("Weapon", weaponCount);
                                remainingCapacity -= weaponCount;
                            }
                        }
                        if (resourcesToCollect.Equals("Lumber") && storage.getLumberCount() > 0)
                        {
                            int lumberCount = storage.getLumberCount();
                            if (lumberCount > remainingCapacity)
                            {
                                resources.Add("Lumber", remainingCapacity);
                                storage.removeResource("Lumber", remainingCapacity);
                                remainingCapacity = 0;
                            }
                            else
                            {
                                resources.Add("Lumber", lumberCount);
                                storage.removeResource("Lumber", lumberCount);
                                remainingCapacity -= lumberCount;
                            }
                        }
                        if (resourcesToCollect.Equals("Furniture") && storage.getFurnitureCount() > 0)
                        {
                            int furnitureCount = storage.getFurnitureCount();
                            if (furnitureCount > remainingCapacity)
                            {
                                resources.Add("Furniture", remainingCapacity);
                                storage.removeResource("Furniture", remainingCapacity);
                                remainingCapacity = 0;
                            }
                            else
                            {
                                resources.Add("Furniture", furnitureCount);
                                storage.removeResource("Furniture", furnitureCount);
                                remainingCapacity -= furnitureCount;
                            }
                        }
                        if (resourcesToCollect.Equals("Hops") && storage.getHopsCount() > 0)
                        {
                            int hopsCount = storage.getHopsCount();
                            if (hopsCount > remainingCapacity)
                            {
                                resources.Add("Hops", remainingCapacity);
                                storage.removeResource("Hops", remainingCapacity);
                                remainingCapacity = 0;
                            }
                            else
                            {
                                resources.Add("Hops", hopsCount);
                                storage.removeResource("Hops", hopsCount);
                                remainingCapacity -= hopsCount;
                            }
                        }
                        if (resourcesToCollect.Equals("Beer") && storage.getBeerCount() > 0)
                        {
                            int beerCount = storage.getBeerCount();
                            if (beerCount > remainingCapacity)
                            {
                                resources.Add("Beer", remainingCapacity);
                                storage.removeResource("Beer", remainingCapacity);
                                remainingCapacity = 0;
                            }
                            else
                            {
                                resources.Add("Beer", beerCount);
                                storage.removeResource("Beer", beerCount);
                                remainingCapacity -= beerCount;
                            }
                        }
                        if (resourcesToCollect.Equals("Ochre") && storage.getOchreCount() > 0)
                        {
                            int ochreCount = storage.getOchreCount();
                            if (ochreCount > remainingCapacity)
                            {
                                resources.Add("Ochre", ochreCount);
                                storage.removeResource("Ochre", remainingCapacity);
                                remainingCapacity = 0;
                            }
                            else
                            {
                                resources.Add("Ochre", ochreCount);
                                storage.removeResource("Ochre", ochreCount);
                                remainingCapacity -= ochreCount;
                            }
                        }
                        if (resourcesToCollect.Equals("WarPaint") && storage.getWarPaintCount() > 0)
                        {
                            int warPaintCount = storage.getWarPaintCount();
                            if (warPaintCount > remainingCapacity)
                            {
                                resources.Add("WarPaint", warPaintCount);
                                storage.removeResource("WarPaint", remainingCapacity);
                                remainingCapacity = 0;
                            }
                            else
                            {
                                resources.Add("WarPaint", warPaintCount);
                                storage.removeResource("WarPaint", warPaintCount);
                                remainingCapacity -= warPaintCount;
                            }
                        }
                        //if this unit has acquired resources, it should return to its place of employment
                        if (resources.Count > 0)
                        {
                            reachedGoal = true;
                        }
                        yield return StartCoroutine(findPathHome(returnPath =>
                        {
                            path = returnPath;
                        }));
                        //if the place of employment is destroyed, this gameobject should be as well
                        if (!placeOfEmployment)
                        {
                            Destroy(gameObject);
                        }
                    }
                    //if the orc has arrived back at its employment from collecting resources, let the employment know
                    // and update the employment's resource count
                    else if (reachedGoal)
                    {
                        Employment employment = placeOfEmployment.GetComponent<Employment>();
                        employment.setWorkerDeliveringGoods(false);
                        Storage employmentStorage = placeOfEmployment.GetComponent<Storage>();
                        foreach (KeyValuePair<string, int> kvp in resources)
                        {
                            employmentStorage.addResource(kvp.Key, kvp.Value);
                        }
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
            //TODO?: If I want the orc to check for other storage places as it's going home, I should have a check
            // inside of this if statement to see if any still exist
            if (runningAStar == false && goalObject == null && !reachedGoal && !headingHome)
            {
                changePath = true;
            }
        }
        yield return null;
    }

    /**
     * Finds a place for the collector orc to go to and creates a path to it.
     * @parameter returnPath a callback returning the path to the closest storage facility
     */
    private IEnumerator findPathToStorage(System.Action<List<Vector2>> returnPath)
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
                //collector workers should not travel over houses during their trip.  as such,
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
                    //final check to make sure this location has the goods the collector orc is looking for
                    bool hasNeededResources = false;
                    if (resourcesToCollect.Equals("Food"))
                    {
                        Storage warehouseStorage = structureArr[(int)originalLocation.x - searchRadius + i,
                        (int)originalLocation.y - searchRadius + j].GetComponent<Storage>();
                        //only use this warehouse as a possibility if it has of food
                        if (warehouseStorage.getFoodCount() > 0)
                        {
                            hasNeededResources = true;
                        }
                    }
                    //Used by the Weaponsmith
                    if (resourcesToCollect.Equals("Iron"))
                    {
                        Storage warehouseStorage = structureArr[(int)originalLocation.x - searchRadius + i,
                        (int)originalLocation.y - searchRadius + j].GetComponent<Storage>();
                        //only use this warehouse as a possibility if it has of food
                        if (warehouseStorage.getIronCount() > 0)
                        {
                            hasNeededResources = true;
                        }
                    }
                    //Used by the FurnitureWorkshop
                    if (resourcesToCollect.Equals("Lumber"))
                    {
                        Storage warehouseStorage = structureArr[(int)originalLocation.x - searchRadius + i,
                        (int)originalLocation.y - searchRadius + j].GetComponent<Storage>();
                        //only use this warehouse as a possibility if it has of food
                        if (warehouseStorage.getLumberCount() > 0)
                        {
                            hasNeededResources = true;
                        }
                    }
                    //Used by the Brewery
                    if (resourcesToCollect.Equals("Hops"))
                    {
                        Storage warehouseStorage = structureArr[(int)originalLocation.x - searchRadius + i,
                        (int)originalLocation.y - searchRadius + j].GetComponent<Storage>();
                        //only use this warehouse as a possibility if it has of food
                        if (warehouseStorage.getHopsCount() > 0)
                        {
                            hasNeededResources = true;
                        }
                    }
                    //Used by the Pub
                    if (resourcesToCollect.Equals("Beer"))
                    {
                        Storage warehouseStorage = structureArr[(int)originalLocation.x - searchRadius + i,
                        (int)originalLocation.y - searchRadius + j].GetComponent<Storage>();
                        //only use this warehouse as a possibility if it has of food
                        if (warehouseStorage.getBeerCount() > 0)
                        {
                            hasNeededResources = true;
                        }
                    }
                    //Used by the War Paint Workshop
                    if (resourcesToCollect.Equals("Ochre"))
                    {
                        Storage warehouseStorage = structureArr[(int)originalLocation.x - searchRadius + i,
                        (int)originalLocation.y - searchRadius + j].GetComponent<Storage>();
                        //only use this warehouse as a possibility if it has of food
                        if (warehouseStorage.getOchreCount() > 0)
                        {
                            hasNeededResources = true;
                        }
                    }

                    if (hasNeededResources)
                    {
                        discoveredDeliveryLocs.Add(structureArr[(int)originalLocation.x - searchRadius + i,
                        (int)originalLocation.y - searchRadius + j]);
                        AstarSearch aStarSearch = new AstarSearch();
                        Vector2 currentLocation = new Vector2(Mathf.RoundToInt(gameObject.transform.position.x),
                            Mathf.RoundToInt(gameObject.transform.position.y));
                        runningAStar = true;
                        yield return StartCoroutine(aStarSearch.aStar(tempPath =>
                        {
                            runningAStar = false;
                            if (tempPath != null && tempPath.Count > 0)
                            {
                                possiblePaths.Add(tempPath);
                            }
                        }, currentLocation, structureArr[(int)originalLocation.x - searchRadius + i,
                            (int)originalLocation.y - searchRadius + j], network));
                    }
                }
            }
        }
        //check for other storage-type buildings if there are no available warehouses (grainery, etc)
        if (possiblePaths.Count == 0)
        {
            returnPath(new List<Vector2>());
            yield break;
        }
        //if there are any possible storage buildings to deliver to, select the one with the shortest path
        if (possiblePaths.Count > 0)
        {
            SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
            if (spriteRenderer.enabled == false)
            {
                spriteRenderer.enabled = true;
            }
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
            gameObject.GetComponent<SpriteRenderer>().enabled = true;
            returnPath(shortestPath);
            yield break;
        }
        yield return null;
    }

    /**
     * Finds a way back to the building that spawned the delivery orc.
     * @parameter returnPath a callback returning the path back to the original location
     */
    private IEnumerator findPathHome(System.Action<List<Vector2>> returnPath)
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
        runningAStar = true;
        yield return StartCoroutine(aStarSearch.aStar(tempPath =>
        {
            runningAStar = false;
            returnPath(tempPath);
        }, new Vector2(Mathf.RoundToInt(location.x), Mathf.RoundToInt(location.y)),
            network[(int)originalLocation.x, (int)originalLocation.y], network));
        if (runningAStar == false)
        {
            yield break;
        }
        yield return new WaitForSeconds(0.05f);
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

    /**
     * Sets which resources the collector orc is to collect.
     * @param resources is the type of resource to collect
     */
    public void setResourcesToCollect(string resources)
    {
        resourcesToCollect = resources;
    }

    /**
     * Sets the amount of resources this collector can carry.
     * @param num is the number of resources the collector can carry
     */
    public void setCarryingCapacity(int num)
    {
        //carryingCapacity = num;
        remainingCapacity = num;
    }
}