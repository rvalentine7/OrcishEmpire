using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Logic for a trader
/// </summary>
public class Trade : Animated
{
    public float stepSize;

    private TradeCityObject myTradeCity;

    private GameObject[,] network;
    private Vector2 exitLocation;
    private List<Vector2> path;
    private Vector2 navigationGoal;
    private GameObject tradingPostGoal;
    private bool waitingOnTradingPost;
    private bool finishedTrading;
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
        waitingOnTradingPost = false;
        finishedTrading = false;
        changePath = false;
        runningAStar = false;
        myWorld = GameObject.Find(World.WORLD_INFORMATION).GetComponent<World>();
        structureArr = myWorld.constructNetwork.getConstructArr();
        terrainArr = myWorld.terrainNetwork.getTerrainArr();
        animator = gameObject.GetComponent<Animator>();
        gameObject.GetComponent<SpriteRenderer>().sortingOrder = gameObject.GetHashCode();

        exitLocation = myWorld.exitLocation;
        navigationGoal = exitLocation;
        network = new GameObject[myWorld.mapSize, myWorld.mapSize];
        //Build the navigation network
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
        
        //Get the trading post to visit and the closest road to it
        GameObject potentialTradingPostToVisit = null;
        for (int i = 0; i < myWorld.mapSize; i++)
        {
            for (int j = 0; j < myWorld.mapSize; j++)
            {
                if (structureArr[i, j] != null && structureArr[i, j].tag.Equals(World.BUILDING)
                    && structureArr[i, j].GetComponent<TradingPost>() != null)
                {
                    TradingPost tradingPost = structureArr[i, j].GetComponent<TradingPost>();
                    //If we don't have another trading post right now or if it has less traders at it than the other option
                    if (structureArr[i, j].GetComponent<Employment>().getNumHealthyWorkers() > 0 && (potentialTradingPostToVisit == null
                        || tradingPost.getNumTraders() < potentialTradingPostToVisit.GetComponent<TradingPost>().getNumTraders()))
                    {
                        if (i - 1 > 0 && structureArr[i - 1, j] != null && structureArr[i - 1, j].tag.Equals(World.ROAD))
                        {
                            navigationGoal = new Vector2(i - 1, j);
                            potentialTradingPostToVisit = tradingPost.gameObject;
                        }
                        else if (i + 1 < structureArr.GetLength(0) && structureArr[i + 1, j] != null
                            && structureArr[i + 1, j].tag.Equals(World.ROAD))
                        {
                            navigationGoal = new Vector2(i + 1, j);
                            potentialTradingPostToVisit = tradingPost.gameObject;
                        }
                        else if (j - 1 > 0 && structureArr[i, j - 1] != null && structureArr[i, j - 1].tag.Equals(World.ROAD))
                        {
                            navigationGoal = new Vector2(i, j - 1);
                            potentialTradingPostToVisit = tradingPost.gameObject;
                        }
                        else if (j + 1 < structureArr.GetLength(1) && structureArr[i, j + 1] != null
                            && structureArr[i, j + 1].tag.Equals(World.ROAD))
                        {
                            navigationGoal = new Vector2(i, j + 1);
                            potentialTradingPostToVisit = tradingPost.gameObject;
                        }
                    }
                }
            }
        }
        tradingPostGoal = potentialTradingPostToVisit;
        if (tradingPostGoal != null)
        {
            tradingPostGoal.GetComponent<TradingPost>().receiveTrader(this);
        }
    }

    /// <summary>
    /// Updates the navigation of the trader
    /// </summary>
    void Update()
    {
        if (!runningAStar && !waitingOnTradingPost)
        {
            StartCoroutine(navigate());
        }
        /*else if (waitingOnTradingPost)
        {
            //TODO: if I still notice the bug where one trader stays idle forever (waitingOnTradingPost?),
            // update this to check for if it's waiting and try to re-insert it in the traders for the trading post
        }*/
    }

    /// <summary>
    /// Moves the trader around the map
    /// </summary>
    /// <returns>A time delay to allow this method to update over multiple frames</returns>
    private IEnumerator navigate()
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
            changePath = false;
            yield return StartCoroutine(aStarSearch.aStar(aStarPath =>
            {
                if (aStarPath != null)
                {
                    path = aStarPath;
                    runningAStar = false;
                    if (path.Count == 0)
                    {
                        Debug.Log("Trader had no path to exit.  It has been destroyed.");
                        destroyTrader();
                    }
                }
            }, start, network[Mathf.RoundToInt(navigationGoal.x), Mathf.RoundToInt(navigationGoal.y)], network));
        }
        else
        {
            if (path[0].x == gameObject.transform.position.x && path[0].y == gameObject.transform.position.y)
            {
                path.RemoveAt(0);
            }
            if (!myWorld.walkableTerrain.Contains(network[Mathf.RoundToInt(path[0].x), Mathf.RoundToInt(path[0].y)].tag))
            {
                changePath = true;
            }
            if (tradingPostGoal == null && navigationGoal != exitLocation)
            {
                navigationGoal = exitLocation;
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
                //If going to the trading post and there's a line, make sure I join in the right spot in the line
                if (navigationGoal != exitLocation)
                {
                    int traderWaitPosition = tradingPostGoal.GetComponent<TradingPost>().getTraderWaitPosition(this);
                    int indexToRemoveAt = Mathf.Max(0, path.Count - traderWaitPosition);
                    if (indexToRemoveAt == 0)
                    {
                        path = new List<Vector2>();
                    }
                }
                //the agent has arrived at the goal location
                if (path.Count == 0)
                {
                    if (navigationGoal.Equals(exitLocation))
                    {
                        //Reached the exit on the map. Disappear from existance
                        destroyTrader();
                    }
                    else
                    {
                        //Let the trading post know I've arrived
                        tradingPostGoal.GetComponent<TradingPost>().traderArrived(this);
                        waitingOnTradingPost = true;

                        animator.SetBool(Animated.MOVING_DOWN, false);
                        animator.SetBool(Animated.MOVING_UP, false);
                        animator.SetBool(Animated.MOVING_SIDEWAYS, false);
                        animator.SetBool(Animated.IDLE, true);
                        currentCharacterAnimation = characterAnimation.Idle;
                    }
                }
            }
        }
        yield return null;
    }

    /// <summary>
    /// Destroys the trader
    /// </summary>
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

    /// <summary>
    /// Notifies the trader its trading post has been destroyed and it can leave the city
    /// </summary>
    public void receiveTradingPostDestruction()
    {
        waitingOnTradingPost = false;
        if (!waitingOnTradingPost && navigationGoal != exitLocation)
        {
            changePath = true;
        }
        navigationGoal = exitLocation;
    }

    /// <summary>
    /// Informs the trader it can leave the city
    /// </summary>
    /// <param name="finished">Whether the trader is finished trading</param>
    public void setFinishedTrading(bool finished)
    {
        finishedTrading = finished;
        waitingOnTradingPost = false;
        navigationGoal = exitLocation;
    }

    /// <summary>
    /// If the orc is waiting on the trading post, it will attempt to move up in the line
    /// </summary>
    public void moveUpInWaitLine()
    {
        waitingOnTradingPost = false;
    }
}
