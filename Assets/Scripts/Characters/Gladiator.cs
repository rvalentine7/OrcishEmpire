using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * A Gladiator is used to move gladiators from a GladiatorPit to an Arena in order to perform
 * fights for entertainment.
 */
public class Gladiator : MonoBehaviour {
    private int numGladiators;
    private bool returnHome;
    private GameObject[,] network;
    private Vector2 originalLocation;
    private GameObject placeOfEmployment;
    private List<Vector2> path;
    private Vector2 goalLocation;
    private GameObject goalObject;
    private bool changePath;
    private bool runningAStar;
    private bool headingHome;
    private GameObject world;
    private World myWorld;
    private GameObject[,] structureArr;
    private GameObject[,] terrainArr;
    public float stepSize;

    /**
     * Initializes the deliver class
     */
    void Awake()
    {
        numGladiators = 0;
        returnHome = false;
        originalLocation = new Vector2();
        changePath = false;
        runningAStar = false;
        headingHome = false;
        world = GameObject.Find("WorldInformation");
        myWorld = world.GetComponent<World>();
        structureArr = myWorld.constructNetwork.getConstructArr();
        terrainArr = myWorld.terrainNetwork.getTerrainArr();
    }

    /**
     * Attempts to find a path to the arena
     */
    void Start()
    {
        //TODO: if there is nowhere to go, wait some time before trying again?
        StartCoroutine(findPathToArena(returnPath =>
        {
            path = returnPath;
        }));
    }

    // Update is called once per frame
    void Update () {
        //if the place of employment is destroyed, this gameobject should be as well
        if (!placeOfEmployment)
        {
            //Need to let the arena these gladiators are no longer coming
            goalObject.GetComponent<Arena>().removeIncomingGladiators(numGladiators);
            Destroy(gameObject);
        }
        if (runningAStar == false)
        {
            StartCoroutine(runGoToArena());
        }
    }

    /**
     * Plans out the movement of the gladiator
     */
    private IEnumerator runGoToArena()
    {
        //if the place of employment is destroyed, this gameobject should be as well
        if (!placeOfEmployment)
        {
            //Need to let the arena these gladiators are no longer coming
            goalObject.GetComponent<Arena>().removeIncomingGladiators(numGladiators);
            Destroy(gameObject);
        }
        //If the arena is destroyed or runs out of employees, send the gladiator back to the pit
        if ((goalObject == null || goalObject.GetComponent<Employment>().getNumWorkers() == 0) && !returnHome)
        {
            changePath = true;
            returnHome = true;
        }
        if (path == null || path.Count == 0 || changePath == true)
        {
            //Go to to the arena
            if (!returnHome && runningAStar == false)
            {
                yield return StartCoroutine(findPathToArena(returnPath =>
                {
                    path = returnPath;
                }));
                //if the place of employment is destroyed, this gameobject should be as well
                if (!placeOfEmployment)
                {
                    //Need to let the arena these gladiators are no longer coming
                    goalObject.GetComponent<Arena>().removeIncomingGladiators(numGladiators);
                    Destroy(gameObject);
                }
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
                        //Need to let the arena these gladiators are no longer coming
                        goalObject.GetComponent<Arena>().removeIncomingGladiators(numGladiators);
                        Destroy(gameObject);
                    }
                    headingHome = true;
                }
            }
            //Go to the home location
            else if (returnHome && runningAStar == false)
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
            if ((goalObject != null || headingHome || returnHome) && (network[(int)nextLocation.x, (int)nextLocation.y] != null
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
                //path[1] is valid as next being the goal because arenas are 3x3.  If I add a larger entertainment building that uses gladiators, this will need an update
                if (nextLocation == goalLocation || (network[(int) nextLocation.x, (int) nextLocation.y] != null
                    && network[(int)nextLocation.x, (int)nextLocation.y] == goalObject))
                {
                    nextIsGoal = true;
                }
                if (distanceBetweenPoints < stepSize)
                {
                    path.RemoveAt(0);
                }
                if (path.Count == 0 || (network[(int)nextLocation.x, (int)nextLocation.y] != null
                    && network[(int)nextLocation.x, (int)nextLocation.y] == goalObject))
                {
                    //if the orc is at the arena, update the arena and gladiator pit and destroy this object
                    if (nextIsGoal)
                    {
                        placeOfEmployment.GetComponent<GladiatorPit>().reduceNumReadyGladiators(numGladiators);
                        goalObject.GetComponent<Arena>().gladiatorArrived(placeOfEmployment, numGladiators);
                        Destroy(gameObject);
                    }
                    //if the orc has arrived back at its employment due to the arena no longer functioning, let the gladiator pit know
                    else if (returnHome)
                    {
                        placeOfEmployment.GetComponent<GladiatorPit>().gladiatorReturn(numGladiators);
                        Destroy(gameObject);
                    }
                }
            }
            else
            {
                changePath = true;
            }
        }
        yield return null;
    }

    /**
     * Creates a path to the arena.  TODO: clean up the method (can probably be just like findPathHome and just use one method instead of 2)
     * @parameter returnPath a callback returning the path to the arena
     */
    private IEnumerator findPathToArena(System.Action<List<Vector2>> returnPath)
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
                //gladiators should not travel over houses during their trip.  as such,
                // houses are not included in the network
                else if (structureArr[i, j].tag != "House")
                {
                    network[i, j] = structureArr[i, j];
                }
            }
        }
        List<Vector2> pathToArena = new List<Vector2>();
        AstarSearch aStarSearch = new AstarSearch();
        Vector2 currentLocation = new Vector2(Mathf.RoundToInt(gameObject.transform.position.x),
            Mathf.RoundToInt(gameObject.transform.position.y));
        runningAStar = true;
        yield return StartCoroutine(aStarSearch.aStar(tempPath =>
        {
            runningAStar = false;
            if (tempPath != null && tempPath.Count > 0)
            {
                pathToArena = tempPath;
            }
        }, currentLocation, goalObject, network));
        //if the place of employment is destroyed, this gameobject should be as well
        if (!placeOfEmployment)
        {
            //Need to let the arena these gladiators are no longer coming
            goalObject.GetComponent<Arena>().removeIncomingGladiators(numGladiators);
            Destroy(gameObject);
        }
        
        //check for other storage-type buildings if there are no available warehouses (market, grainery, etc)
        if (pathToArena.Count == 0)
        {
            returnPath(new List<Vector2>());
            yield break;
        }
        //if there are any possible storage buildings to deliver to, select the one with the shortest path
        if (pathToArena.Count > 0)
        {
            goalLocation = pathToArena[pathToArena.Count - 1];
            returnPath(pathToArena);
            yield break;
        }
        yield return null;
    }

    /**
     * Finds a way back to the building that spawned the gladiator.
     * @parameter returnPath a callback returning the path to the original location
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
     * Sets the arena the gladiator should be going to
     * @param arena the arena this gladiator should be going to
     * @param numGladiators the number of gladiators in this gameObject going to the arena
     */
    public void setArena(GameObject arena, int numGladiators)
    {
        this.numGladiators = numGladiators;
        goalObject = arena;
    }

    /**
     * Sets the place the deliver orc starts at.
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
