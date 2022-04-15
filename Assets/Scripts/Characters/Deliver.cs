using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Sends a delivery orc from its current location to the goal where it will deliver goods
/// before returning to its original location.
/// </summary>
public class Deliver : Animated
{
    private Dictionary<string, int> resources;
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
    private World myWorld;
    private GameObject[,] structureArr;
    private GameObject[,] terrainArr;
    private bool hasGoods;
    public float stepSize;
    public int searchRadius;
    private Animator animator;

    /// <summary>
    /// Initializes the deliver class
    /// </summary>
    void Awake()
    {
        resources = new Dictionary<string, int>();
        originalLocation = new Vector2();
        reachedGoal = false;
        changePath = false;
        runningAStar = false;
        headingHome = false;
        myWorld = GameObject.Find(World.WORLD_INFORMATION).GetComponent<World>();
        structureArr = myWorld.constructNetwork.getConstructArr();
        terrainArr = myWorld.terrainNetwork.getTerrainArr();
        hasGoods = true;
        animator = gameObject.GetComponent<Animator>();
        gameObject.GetComponent<SpriteRenderer>().sortingOrder = gameObject.GetHashCode();
    }

    /// <summary>
    /// Attempts to find the first place to go to
    /// </summary>
    void Start()
    {
        animator.SetBool(Animated.MOVING_DOWN, false);
        animator.SetBool(Animated.MOVING_UP, false);
        animator.SetBool(Animated.MOVING_SIDEWAYS, false);
        animator.SetBool(Animated.DOWN_OBJECT, false);
        animator.SetBool(Animated.UP_OBJECT, false);
        animator.SetBool(Animated.SIDEWAYS_OBJECT, false);
        if (resources.Count == 0)
        {
            animator.SetBool(Animated.IDLE, true);
            animator.SetBool(Animated.IDLE_OBJECT, false);
        }
        else
        {
            animator.SetBool(Animated.IDLE_OBJECT, true);
            animator.SetBool(Animated.IDLE, false);
        }
        currentCharacterAnimation = characterAnimation.Idle;

        StartCoroutine(runDeliver());
    }

    /// <summary>
    /// Plans out the movement and deliver of resources for the delivery orc
    /// </summary>
    /// <returns>A delay before resuming method execution</returns>
    private IEnumerator runDeliver()
    {
        while (true)
        {
            //if the place of employment is destroyed, this gameobject should be as well
            if (!placeOfEmployment)
            {
                Destroy(gameObject);
            }
            if (path == null || path.Count == 0 || changePath == true)
            {
                animator.SetBool(Animated.MOVING_DOWN, false);
                animator.SetBool(Animated.MOVING_UP, false);
                animator.SetBool(Animated.MOVING_SIDEWAYS, false);
                animator.SetBool(Animated.DOWN_OBJECT, false);
                animator.SetBool(Animated.UP_OBJECT, false);
                animator.SetBool(Animated.SIDEWAYS_OBJECT, false);
                if (resources.Count == 0)
                {
                    animator.SetBool(Animated.IDLE, true);
                    animator.SetBool(Animated.IDLE_OBJECT, false);
                }
                else
                {
                    animator.SetBool(Animated.IDLE_OBJECT, true);
                    animator.SetBool(Animated.IDLE, false);
                }
                currentCharacterAnimation = characterAnimation.Idle;

                //Go to to a storage location
                if (!reachedGoal && runningAStar == false)
                {
                    yield return StartCoroutine(findPathToStorage(returnPath =>
                    {
                        path = returnPath;
                    }));
                    //if the place of employment is destroyed, this gameobject should be as well
                    if (!placeOfEmployment)
                    {
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
                            Destroy(gameObject);
                        }
                        headingHome = true;
                    }
                }
                //Go to the home location
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
                if (path.Count == 0 && getHeadingHome())
                {
                    Employment employment = placeOfEmployment.GetComponent<Employment>();
                    employment.setWorkerDeliveringGoods(false);
                    Destroy(gameObject);
                }
                else
                {
                    Vector2 nextLocation = path[0];
                    //if the orc is heading home or the goalobject exists, take a step; otherwise, change the path
                    if ((goalObject != null || headingHome || reachedGoal) && (network[(int)nextLocation.x, (int)nextLocation.y] != null
                        && (!network[(int)nextLocation.x, (int)nextLocation.y].tag.Equals(World.BUILDING)
                        || network[(int)nextLocation.x, (int)nextLocation.y] == goalObject)))
                    {
                        Vector2 vector = new Vector2(nextLocation.x - currentLocation.x, nextLocation.y - currentLocation.y);
                        float magnitude = Mathf.Sqrt(vector.x * vector.x + vector.y * vector.y);
                        Vector2 unitVector = new Vector2(vector.x / magnitude, vector.y / magnitude);

                        bool nextIsGoal = false;
                        if (nextLocation == goal || (network[(int)nextLocation.x, (int)nextLocation.y] != null
                            && network[(int)nextLocation.x, (int)nextLocation.y] == goalObject))
                        {
                            nextIsGoal = true;
                        }
                        else
                        {
                            //take a step towards the nextLocation
                            Vector2 newLocation = new Vector2(currentLocation.x + unitVector.x * stepSize * Time.deltaTime, currentLocation.y
                                + unitVector.y * stepSize * Time.deltaTime);
                            gameObject.transform.position = newLocation;
                        }

                        //animation
                        if (unitVector.x > 0 && Mathf.Abs(vector.x) > Mathf.Abs(vector.y))
                        {
                            if (flipped)
                            {
                                flipSprite();
                            }

                            if (resources.Count == 0 && currentCharacterAnimation != characterAnimation.Right)
                            {
                                currentCharacterAnimation = characterAnimation.Right;
                                animator.SetBool(Animated.MOVING_SIDEWAYS, true);
                                animator.SetBool(Animated.SIDEWAYS_OBJECT, false);
                            }
                            else if (resources.Count > 0 && currentCharacterAnimation != characterAnimation.RightObject)
                            {
                                currentCharacterAnimation = characterAnimation.RightObject;
                                animator.SetBool(Animated.SIDEWAYS_OBJECT, true);
                                animator.SetBool(Animated.MOVING_SIDEWAYS, false);
                            }

                            animator.SetBool(Animated.MOVING_DOWN, false);
                            animator.SetBool(Animated.MOVING_UP, false);
                            animator.SetBool(Animated.DOWN_OBJECT, false);
                            animator.SetBool(Animated.UP_OBJECT, false);
                            animator.SetBool(Animated.IDLE, false);
                            animator.SetBool(Animated.IDLE_OBJECT, false);
                        }
                        else if (unitVector.x < 0 && Mathf.Abs(vector.x) > Mathf.Abs(vector.y))
                        {
                            //left. needs to flip sprite because it reuses the sprite for moving right
                            if (!flipped)
                            {
                                flipSprite();
                            }

                            if (resources.Count == 0 && currentCharacterAnimation != characterAnimation.Left)
                            {
                                currentCharacterAnimation = characterAnimation.Left;
                                animator.SetBool(Animated.MOVING_SIDEWAYS, true);
                                animator.SetBool(Animated.SIDEWAYS_OBJECT, false);
                            }
                            else if (resources.Count > 0 && currentCharacterAnimation != characterAnimation.LeftObject)
                            {
                                currentCharacterAnimation = characterAnimation.LeftObject;
                                animator.SetBool(Animated.SIDEWAYS_OBJECT, true);
                                animator.SetBool(Animated.MOVING_SIDEWAYS, false);
                            }

                            animator.SetBool(Animated.MOVING_DOWN, false);
                            animator.SetBool(Animated.MOVING_UP, false);
                            animator.SetBool(Animated.DOWN_OBJECT, false);
                            animator.SetBool(Animated.UP_OBJECT, false);
                            animator.SetBool(Animated.IDLE, false);
                            animator.SetBool(Animated.IDLE_OBJECT, false);
                        }
                        else if (unitVector.y > 0 && Mathf.Abs(vector.y) > Mathf.Abs(vector.x))
                        {
                            if (flipped)
                            {
                                flipSprite();
                            }

                            if (resources.Count == 0 && currentCharacterAnimation != characterAnimation.Up)
                            {
                                currentCharacterAnimation = characterAnimation.Up;
                                animator.SetBool(Animated.MOVING_UP, true);
                                animator.SetBool(Animated.UP_OBJECT, false);
                            }
                            else if (resources.Count > 0 && currentCharacterAnimation != characterAnimation.UpObject)
                            {
                                currentCharacterAnimation = characterAnimation.UpObject;
                                animator.SetBool(Animated.UP_OBJECT, true);
                                animator.SetBool(Animated.MOVING_UP, false);
                            }

                            animator.SetBool(Animated.MOVING_DOWN, false);
                            animator.SetBool(Animated.MOVING_SIDEWAYS, false);
                            animator.SetBool(Animated.DOWN_OBJECT, false);
                            animator.SetBool(Animated.SIDEWAYS_OBJECT, false);
                            animator.SetBool(Animated.IDLE, false);
                            animator.SetBool(Animated.IDLE_OBJECT, false);
                        }
                        else if (unitVector.y < 0 && Mathf.Abs(vector.y) > Mathf.Abs(vector.x))
                        {
                            if (flipped)
                            {
                                flipSprite();
                            }

                            if (resources.Count == 0 && currentCharacterAnimation != characterAnimation.Down)
                            {
                                currentCharacterAnimation = characterAnimation.Down;
                                animator.SetBool(Animated.MOVING_DOWN, true);
                                animator.SetBool(Animated.DOWN_OBJECT, false);
                            }
                            else if (resources.Count > 0 && currentCharacterAnimation != characterAnimation.DownObject)
                            {
                                currentCharacterAnimation = characterAnimation.DownObject;
                                animator.SetBool(Animated.DOWN_OBJECT, true);
                                animator.SetBool(Animated.MOVING_DOWN, false);
                            }

                            animator.SetBool(Animated.MOVING_UP, false);
                            animator.SetBool(Animated.MOVING_SIDEWAYS, false);
                            animator.SetBool(Animated.UP_OBJECT, false);
                            animator.SetBool(Animated.SIDEWAYS_OBJECT, false);
                            animator.SetBool(Animated.IDLE, false);
                            animator.SetBool(Animated.IDLE_OBJECT, false);
                        }

                        //if the agent gets to the next vector then delete it from the path
                        // and go to the next available vector
                        float distanceBetweenPoints = myWorld.getDistanceBetweenPoints(gameObject.transform.position, nextLocation);
                        if (distanceBetweenPoints < World.CLOSE_ENOUGH_DIST)
                        {
                            path.RemoveAt(0);
                        }
                        if (path.Count == 0 || (network[(int)nextLocation.x, (int)nextLocation.y] != null
                            && network[(int)nextLocation.x, (int)nextLocation.y] == goalObject))
                        {
                            //if the orc is at the storage goal, deliver resources
                            if (nextIsGoal)
                            {
                                //at the goal, deliver any available goods and then plan a path back
                                Storage storage = goalObject.GetComponent<Storage>();
                                if (storage != null)
                                {
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
                                    if (resources.Count == 0 && hasGoods)
                                    {
                                        hasGoods = false;
                                    }
                                }
                                //path = findPathHome();
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
                            //if the orc has arrived back at its employment from delivering resources, let the employment know
                            else if (reachedGoal)
                            {
                                Employment employment = placeOfEmployment.GetComponent<Employment>();
                                employment.setWorkerDeliveringGoods(false);
                                Destroy(gameObject);
                            }
                            //if the previous deliver location was destroyed and the orc had to return home, it should
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
            }
            yield return null;
        }
    }

    /// <summary>
    /// Finds a place for the delivery orc to go to and creates a path to it.
    /// </summary>
    /// <param name="returnPath">a callback returning the path to the closest storage facility</param>
    /// <returns>A delay before resuming method execution</returns>
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
                //delivery workers should not travel over houses during their trip.  as such,
                // houses are not included in the network
                else if (!structureArr[i, j].tag.Equals(World.HOUSE))
                {
                    network[i, j] = structureArr[i, j];
                }
            }
        }
        List<List<Vector2>> possiblePaths = new List<List<Vector2>>();
        //checks for the closest warehouse
        List<GameObject> discoveredDeliverLocs = new List<GameObject>();
        for (int i = 0; i <= searchRadius * 2; i++)
        {
            for (int j = 0; j <= searchRadius * 2; j++)
            {
                if (originalLocation.x - searchRadius + i >= 0 && originalLocation.y - searchRadius + j >= 0
                        && originalLocation.x - searchRadius + i <= 39 && originalLocation.y - searchRadius + j <= 39
                        && structureArr[(int)originalLocation.x - searchRadius + i,
                        (int)originalLocation.y - searchRadius + j] != null
                        && structureArr[(int)originalLocation.x - searchRadius + i,
                        (int)originalLocation.y - searchRadius + j].tag.Equals(World.BUILDING)
                        && !discoveredDeliverLocs.Contains(structureArr[(int)originalLocation.x - searchRadius + i,
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
                        discoveredDeliverLocs.Add(structureArr[(int)originalLocation.x - searchRadius + i,
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
                        //if the place of employment is destroyed, this gameobject should be as well
                        if (!placeOfEmployment)
                        {
                            Destroy(gameObject);
                        }
                    }
                }
            }
        }
        //check for other storage-type buildings if there are no available warehouses (market, grainery, etc)
        if (possiblePaths.Count == 0)
        {
            returnPath(new List<Vector2>());
            yield break;
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
            returnPath(shortestPath);
            yield break;
        }
        yield return null;
    }

    /// <summary>
    /// Finds a way back to the building that spawned the delivery orc.
    /// </summary>
    /// <param name="returnPath">a callback returning the path to the original location</param>
    /// <returns>A delay before resuming method execution</returns>
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
                else if (!structureArr[i, j].tag.Equals(World.HOUSE))
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

    /// <summary>
    /// Adds to the resources the delivery orc is carrying.
    /// </summary>
    /// <param name="resourceName">the name of the resource</param>
    /// <param name="num">the number of a type of resource being carried</param>
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

    /// <summary>
    /// Removes a resource from the delivery orc.
    /// </summary>
    /// <param name="resourceName">the resource to be removed</param>
    /// <param name="num">the number of the specific resource to remove</param>
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

    /// <summary>
    /// Sets the place the deliver orc starts at.
    /// </summary>
    /// <param name="position">the position the delivery orc spawned in at</param>
    public void setOriginalLocation(Vector2 position)
    {
        originalLocation = position;
    }

    /// <summary>
    /// Sets the place of employment this delivery orc works for.
    /// </summary>
    /// <param name="employment">the place of employment</param>
    public void setOrcEmployment(GameObject employment)
    {
        placeOfEmployment = employment;
    }

    /// <summary>
    /// Gets whether the character is heading back to its place of employment
    /// </summary>
    /// <returns>Whether the character is heading back to its place of employment</returns>
    public bool getHeadingHome()
    {
        return headingHome || reachedGoal;
    }
}
