using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Uses A* to form a path which the orc immigrant will then use to move in to a house.
/// </summary>
public class Immigrate : Animated {
    //TODO: If a part of its path is destroyed at any point in time, it should find a new path.
    private GameObject[,] network;
    private GameObject[,] structureArr;
    private World myWorld;
    public GameObject goalObject;
    private List<Vector2> path;
    public float stepSize;
    private bool changePath;
    private bool runningAStar;
    private Animator animator;
    private bool exitingMap;

    /// <summary>
    /// Instantiates the information necessary for the orc to find its way to its new house.
    /// </summary>
    void Start () {
        path = new List<Vector2>();
        changePath = false;
        runningAStar = false;
        animator = gameObject.GetComponent<Animator>();
        exitingMap = false;
        myWorld = GameObject.Find(World.WORLD_INFORMATION).GetComponent<World>();
        structureArr = myWorld.constructNetwork.getConstructArr();
        GameObject[,] terrainArr = myWorld.terrainNetwork.getTerrainArr();
        network = new GameObject[myWorld.mapSize, myWorld.mapSize];
        for (int i = 0; i < network.GetLength(0); i++)
        {
            for (int j = 0; j < network.GetLength(1); j++)
            {
                if (structureArr[i, j] == null && myWorld.walkableTerrain.Contains(terrainArr[i, j].tag))
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

    /// <summary>
    /// Update has the orc immigrant seek out its house destination.
    /// </summary>
    void Update () {
        if (goalObject != null && runningAStar == false)
        {
            Vector2 goal = goalObject.transform.position;
            if (path == null || path.Count == 0 || changePath == true)
            {
                animator.SetBool(Animated.MOVING_DOWN, false);
                animator.SetBool(Animated.MOVING_UP, false);
                animator.SetBool(Animated.MOVING_SIDEWAYS, false);
                animator.SetBool(Animated.IDLE, true);
                currentCharacterAnimation = characterAnimation.Idle;

                //What if I had a class attached to this gameobject that A* could call? It would get the path and be able to tell this class when A* is done.
                // In this case, I would need to have another variable here that says A* is running so I don't keep calling it.  When the special class
                // used to get data from A* is updated, that class tells the variable A* is done running and updates the "path" variable in this class.
                // All of update would need to check if A* is running (add a new if statement encasing everything and also add checks in this embedded
                // section)
                AstarSearch aStarSearch = new AstarSearch();
                //y in the following line of code is floored to an int in case I decide to bump up the position by 0.5 when the agent
                // spawns so that it is walking in the middle of the block (so that it doesn't appear to walk just below the road)
                Vector2 start = new Vector2(Mathf.CeilToInt(gameObject.transform.position.x), Mathf.FloorToInt(gameObject.transform.position.y));
                //path = aStarSearch.aStar(start, goalObject, network);
                //Debug.Log("doing the coroutine");
                runningAStar = true;
                StartCoroutine(aStarSearch.aStar(aStarPath => {
                    if(aStarPath != null) {
                        path = aStarPath;
                        runningAStar = false;
                        if (path.Count == 0)
                        {
                            Debug.Log("Spawned orc immigrant had no path to house.  It has been destroyed.");
                            //TODO: fix the destroy so this script doesn't continue afterwards (Ex. should be destroyed and run into no problems
                            // when a house is created in an impossible location, but it does not.  Might want to run A* check on a house when
                            // it is created and if no path, try again in 10-15s... if no path again, delete the house.)
                            Destroy(gameObject);
                        }
                        SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
                        if (spriteRenderer.enabled == false)
                        {
                            spriteRenderer.enabled = true;
                        }
                    }
                }, start, goalObject, network));
                //if (path.Count == 0)
                //{
                //    Debug.Log("Spawned orc immigrant had no path to house.  It has been destroyed.");
                    //TODO: fix the destroy so this script doesn't continue afterwards (Ex. should be destroyed and run into no problems
                    // when a house is created in an impossible location, but it does not.  Might want to run A* check on a house when
                    // it is created and if no path, try again in 10-15s... if no path again, delete the house.)
                //    Destroy(gameObject);
                //}
                changePath = false;
            }
            else
            {
                if ((network[Mathf.RoundToInt(path[0].x), Mathf.RoundToInt(path[0].y)] == null)
                    || (!myWorld.walkableTerrain.Contains(network[Mathf.RoundToInt(path[0].x), Mathf.RoundToInt(path[0].y)].tag)
                    && !network[Mathf.RoundToInt(path[0].x), Mathf.RoundToInt(path[0].y)].tag.Equals(World.HOUSE)))
                {
                    changePath = true;
                }
                if (changePath && structureArr[Mathf.RoundToInt(path[0].x), Mathf.RoundToInt(path[0].y)] != null
                    && structureArr[Mathf.RoundToInt(path[0].x), Mathf.RoundToInt(path[0].y)].tag.Equals(World.ROAD))
                {
                    changePath = false;
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
                    Vector2 newLocation = new Vector2(currentLocation.x + unitVector.x * stepSize * Time.timeScale, currentLocation.y
                        + unitVector.y * stepSize * Time.timeScale);
                    gameObject.transform.position = newLocation;
                    
                    //animation
                    if (unitVector.x > 0 && Mathf.Abs(vector.x) > Mathf.Abs(vector.y) && currentCharacterAnimation != characterAnimation.Right)
                    {
                        if (flipped)
                        {
                            flipSprite();
                        }
                        animator.SetBool(Animated.IDLE, false);
                        animator.SetBool(Animated.MOVING_DOWN, false);
                        animator.SetBool(Animated.MOVING_UP, false);
                        animator.SetBool(Animated.MOVING_SIDEWAYS, true);
                        currentCharacterAnimation = characterAnimation.Right;
                    }
                    else if (unitVector.x < 0 && Mathf.Abs(vector.x) > Mathf.Abs(vector.y) && currentCharacterAnimation != characterAnimation.Left)
                    {
                        //left. needs to flip sprite because it reuses the sprite for moving right
                        if (!flipped)
                        {
                            flipSprite();
                        }
                        animator.SetBool(Animated.IDLE, false);
                        animator.SetBool(Animated.MOVING_DOWN, false);
                        animator.SetBool(Animated.MOVING_UP, false);
                        animator.SetBool(Animated.MOVING_SIDEWAYS, true);
                        currentCharacterAnimation = characterAnimation.Left;
                    }
                    else if (unitVector.y > 0 && Mathf.Abs(vector.y) > Mathf.Abs(vector.x) && currentCharacterAnimation != characterAnimation.Up)
                    {
                        if (flipped)
                        {
                            flipSprite();
                        }
                        animator.SetBool(Animated.IDLE, false);
                        animator.SetBool(Animated.MOVING_DOWN, false);
                        animator.SetBool(Animated.MOVING_SIDEWAYS, false);
                        animator.SetBool(Animated.MOVING_UP, true);
                        currentCharacterAnimation = characterAnimation.Up;
                    }
                    else if (unitVector.y < 0 && Mathf.Abs(vector.y) > Mathf.Abs(vector.x) && currentCharacterAnimation != characterAnimation.Down)
                    {
                        if (flipped)
                        {
                            flipSprite();
                        }
                        animator.SetBool(Animated.IDLE, false);
                        animator.SetBool(Animated.MOVING_SIDEWAYS, false);
                        animator.SetBool(Animated.MOVING_UP, false);
                        animator.SetBool(Animated.MOVING_DOWN, true);
                        currentCharacterAnimation = characterAnimation.Down;
                    }

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
                        if (goalObject.tag.Equals(World.BUILDING) || goalObject.tag.Equals(World.HOUSE))
                        {
                            HouseInformation houseInfo = goalObject.GetComponent<HouseInformation>();
                            OrcInformation orcInfo = gameObject.GetComponent<OrcInformation>();
                            houseInfo.addInhabitants(orcInfo.getOrcCount());
                            houseInfo.orcsMovingIn(houseInfo.getNumOrcsMovingIn() - orcInfo.getOrcCount());
                        }
                        Destroy(gameObject);
                    }
                }
            }
        }
        else if (goalObject == null)//house was deleted, find a new place to go to
        {
            exitingMap = true;
            Vector2 goal = myWorld.exitLocation;
            GameObject[,] terrainArr = myWorld.terrainNetwork.getTerrainArr();
            goalObject = terrainArr[(int) goal.x, (int) goal.y];
            changePath = true;
            runningAStar = false;
        }
    }

    /// <summary>
    /// Gets whether the immigrant is exiting the map
    /// </summary>
    /// <returns>Whether the immigrant is exiting the map</returns>
    public bool getExitingMap()
    {
        return exitingMap;
    }
}
