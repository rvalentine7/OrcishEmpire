using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trade : Animated
{
    public float stepSize;

    private TradeCityObject myTradeCity;

    private GameObject[,] network;
    private Vector2 exitLocation;
    private List<Vector2> path;
    private Vector2 goal;
    //private GameObject tradingPost;
    //private bool finishedTrading;
    private bool changePath;
    private bool runningAStar;
    private World myWorld;
    private GameObject[,] structureArr;
    private GameObject[,] terrainArr;
    private Animator animator;

    /// <summary>
    /// Initialization
    /// </summary>
    private void Awake()
    {
        //finishedTrading = false;
        changePath = false;
        runningAStar = false;
        myWorld = GameObject.Find(World.WORLD_INFORMATION).GetComponent<World>();
        structureArr = myWorld.constructNetwork.getConstructArr();
        terrainArr = myWorld.terrainNetwork.getTerrainArr();
        animator = gameObject.GetComponent<Animator>();
        gameObject.GetComponent<SpriteRenderer>().sortingOrder = gameObject.GetHashCode();

        exitLocation = myWorld.exitLocation;
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

        /**
         * Get a list of all squares in the worker's travel radius that have houses in a proximity of two
         * Key = House, Value = Closest Road
         */
        /*for (int i = 0; i <= myWorld.mapSize; i++)
        {
            for (int j = 0; j <= myWorld.mapSize; j++)
            {
                if (i > 0 && i < structureArr.GetLength(0) && j > 0 && j < structureArr.GetLength(1))
                {
                    if (structureArr[i, j] != null && structureArr[i, j].tag.Equals(World.HOUSE))
                    {
                        HouseInformation houseInfo = structureArr[i, j].GetComponent<HouseInformation>();
                        if (houseInfo.getHouseholdCurrency() > 1)//if the house has any money that can be collected. > 1 because it won't take money if only at 1 gold
                        {
                            if (i - 1 > 0 && structureArr[i - 1, j] != null && structureArr[i - 1, j].tag.Equals(World.ROAD))
                            {
                                locationsToVisit.Add(structureArr[i, j], structureArr[i - 1, j]);
                            }
                            else if (i + 1 < structureArr.GetLength(0) && structureArr[i + 1, j] != null
                                && structureArr[i + 1, j].tag.Equals(World.ROAD))
                            {
                                locationsToVisit.Add(structureArr[i, j], structureArr[i + 1, j]);
                            }
                            else if (j - 1 > 0 && structureArr[i, j - 1] != null && structureArr[i, j - 1].tag.Equals(World.ROAD))
                            {
                                locationsToVisit.Add(structureArr[i, j], structureArr[i, j - 1]);
                            }
                            else if (j + 1 < structureArr.GetLength(1) && structureArr[i, j + 1] != null
                                && structureArr[i, j + 1].tag.Equals(World.ROAD))
                            {
                                locationsToVisit.Add(structureArr[i, j], structureArr[i, j + 1]);
                            }
                            else if (i - 2 > 0 && structureArr[i - 2, j] != null && structureArr[i - 2, j].tag.Equals(World.ROAD))
                            {
                                locationsToVisit.Add(structureArr[i, j], structureArr[i - 2, j]);
                            }
                            else if (i + 2 < structureArr.GetLength(0) && structureArr[i + 2, j] != null
                                && structureArr[i + 2, j].tag.Equals(World.ROAD))
                            {
                                locationsToVisit.Add(structureArr[i, j], structureArr[i + 2, j]);
                            }
                            else if (j - 2 > 0 && structureArr[i, j - 2] != null && structureArr[i, j - 2].tag.Equals(World.ROAD))
                            {
                                locationsToVisit.Add(structureArr[i, j], structureArr[i, j - 2]);
                            }
                            else if (j + 2 < structureArr.GetLength(1) && structureArr[i, j + 2] != null
                                && structureArr[i, j + 2].tag.Equals(World.ROAD))
                            {
                                locationsToVisit.Add(structureArr[i, j], structureArr[i, j + 2]);
                            }
                        }
                    }
                }
            }
        }*/
    }

    /// <summary>
    /// Updates the animation of the orc
    /// </summary>
    void Update()
    {
        if (runningAStar == false)
        {
            if (path == null || path.Count == 0 || changePath == true)
            {
                animator.SetBool(Animated.MOVING_DOWN, false);
                animator.SetBool(Animated.MOVING_UP, false);
                animator.SetBool(Animated.MOVING_SIDEWAYS, false);
                animator.SetBool(Animated.IDLE, true);
                currentCharacterAnimation = characterAnimation.Idle;

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
                            Debug.Log("Trader had no path to exit.  It has been destroyed.");
                            //TODO: fix the destroy so this script doesn't continue afterwards (Ex. should be destroyed and run into no problems
                            // when a house is created in an impossible location, but it does not.  Might want to run A* check on a house when
                            // it is created and if no path, try again in 10-15s... if no path again, delete the house.)
                            destroyTrader();
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
                if (!myWorld.walkableTerrain.Contains(network[Mathf.RoundToInt(path[0].x), Mathf.RoundToInt(path[0].y)].tag)
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
                    Vector2 newLocation = new Vector2(currentLocation.x + unitVector.x * stepSize * Time.deltaTime, currentLocation.y
                        + unitVector.y * stepSize * Time.deltaTime);
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
                    if (distanceBetweenPoints < 0.05f)
                    {
                        path.RemoveAt(0);
                    }
                    //the agent has arrived at the goal location
                    if (path.Count == 0)
                    {
                        destroyTrader();
                    }
                }
            }
        }
    }

    private void destroyTrader()
    {
        //When the trader finishes its thing in the player city, update traderTimeTravelingStarted to be when the trader exited and setTraderInPlayerCity(false);
        myTradeCity.setTraderInPlayerCity(false);
        myTradeCity.setTraderTimeTravelingStarted(Time.time);
        Destroy(gameObject);
    }

    /// <summary>
    /// Sets the trade city object this trader is acting on the behalf of
    /// </summary>
    /// <param name="tradeCityObject">The trade city object for this trader</param>
    public void setMyTradeCityObject(TradeCityObject tradeCityObject)
    {
        myTradeCity = tradeCityObject;
    }
}
