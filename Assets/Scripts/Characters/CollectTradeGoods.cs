using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//When a trader tries to trade, make sure to reference tradingPerResource to see the max amount they can buy/sell (if there is available supply/storage space)
public class CollectTradeGoods : Animated
{
    private List<string> goodsToSell;//Goods the player can sell
    private List<string> goodsToBuy;//Goods the player can buy

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
    private Dictionary<string, ResourceTrading> tradingPerResource;
    private GameObject[,] structureArr;
    private GameObject[,] terrainArr;
    public float stepSize;
    public int searchRadius;
    private Animator animator;
    private bool collectedGoods;
    private List<GameObject> visitedWarehouses;

    /// <summary>
    /// Initializes the collector class
    /// </summary>
    void Awake()
    {
        gameObject.GetComponent<SpriteRenderer>().enabled = false;
        originalLocation = new Vector2(gameObject.transform.position.x, gameObject.transform.position.y);
        reachedGoal = false;
        changePath = false;
        headingHome = false;
        myWorld = GameObject.Find(World.WORLD_INFORMATION).GetComponent<World>();
        structureArr = myWorld.constructNetwork.getConstructArr();
        terrainArr = myWorld.terrainNetwork.getTerrainArr();
        animator = gameObject.GetComponent<Animator>();
        gameObject.GetComponent<SpriteRenderer>().sortingOrder = gameObject.GetHashCode();
        collectedGoods = false;
        visitedWarehouses = new List<GameObject>();

        //TODO: need to track how many resources have been imported/exported
        //ResourceTrading exampleResourceTrading = tradeManager.getTradingPerResource("example");
        //TradeManager.TradeStatus exmpleResourceStatus = exampleResourceTrading.getTradeStatus();
        //int exampleMaxAmount = exampleResourceTrading.getTradeAmount();
        goodsToSell = new List<string>();
        goodsToBuy = new List<string>();
    }

    // Update is called once per frame
    void Update()
    {
        //TODO: when a trade collector picks up goods from a warehouse, subtract it from the city's gold. if the city runs out of gold, the trade collector can return to the trading post
        if (runningAStar == false)
        {
            StartCoroutine(runCollect());
        }
    }

    /// <summary>
    /// Plans out the movement and collection of resources for the collector orc
    /// </summary>
    /// <returns></returns>
    private IEnumerator runCollect()
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
            if (!collectedGoods)
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
                if (path != null && path.Count == 0)
                {
                    if (distanceBetweenPoints > 0.5f)
                    {
                        yield return StartCoroutine(findPathHome(returnPath =>
                        {
                            path = returnPath;
                        }));
                        //if the place of employment is destroyed, this gameobject should be as well
                        if (!placeOfEmployment || path.Count == 0)
                        {
                            Destroy(gameObject);
                        }
                        headingHome = true;
                    }
                    else
                    {
                        placeOfEmployment.GetComponent<TradingPost>().returnTradeCollector();
                        Destroy(gameObject);
                    }
                }
            }
            else if (reachedGoal && runningAStar == false)
            {
                yield return StartCoroutine(findPathHome(returnPath =>
                {
                    path = returnPath;
                }));
                //if the place of employment is destroyed, this gameobject should be as well
                if (!placeOfEmployment || path.Count == 0)
                {
                    Destroy(gameObject);
                }
            }
            if (runningAStar == false)
            {
                changePath = false;
            }
        }
        else if (path != null && path.Count > 0 && runningAStar == false)
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
                && (network[(int)nextLocation.x, (int)nextLocation.y].tag != World.BUILDING
                || network[(int)nextLocation.x, (int)nextLocation.y] == goalObject)))
            {
                //take a step towards the nextLocation
                Vector2 vector = new Vector2(nextLocation.x - currentLocation.x, nextLocation.y - currentLocation.y);
                float magnitude = Mathf.Sqrt(vector.x * vector.x + vector.y * vector.y);
                Vector2 unitVector = new Vector2(vector.x / magnitude, vector.y / magnitude);
                Vector2 newLocation = new Vector2(currentLocation.x + unitVector.x * stepSize * Time.deltaTime, currentLocation.y
                    + unitVector.y * stepSize * Time.deltaTime);
                gameObject.transform.position = newLocation;

                //animation
                if (unitVector.x > 0 && Mathf.Abs(vector.x) > Mathf.Abs(vector.y))
                {
                    if (flipped)
                    {
                        flipSprite();
                    }

                    if (!collectedGoods && currentCharacterAnimation != characterAnimation.Right)
                    {
                        currentCharacterAnimation = characterAnimation.Right;
                        animator.SetBool(Animated.MOVING_SIDEWAYS, true);
                        animator.SetBool(Animated.SIDEWAYS_OBJECT, false);
                    }
                    else if (collectedGoods && currentCharacterAnimation != characterAnimation.RightObject)
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

                    if (!collectedGoods && currentCharacterAnimation != characterAnimation.Left)
                    {
                        currentCharacterAnimation = characterAnimation.Left;
                        animator.SetBool(Animated.MOVING_SIDEWAYS, true);
                        animator.SetBool(Animated.SIDEWAYS_OBJECT, false);
                    }
                    else if (collectedGoods && currentCharacterAnimation != characterAnimation.LeftObject)
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

                    if (!collectedGoods && currentCharacterAnimation != characterAnimation.Up)
                    {
                        currentCharacterAnimation = characterAnimation.Up;
                        animator.SetBool(Animated.MOVING_UP, true);
                        animator.SetBool(Animated.UP_OBJECT, false);
                    }
                    else if (collectedGoods && currentCharacterAnimation != characterAnimation.UpObject)
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

                    if (!collectedGoods && currentCharacterAnimation != characterAnimation.Down)
                    {
                        currentCharacterAnimation = characterAnimation.Down;
                        animator.SetBool(Animated.MOVING_DOWN, true);
                        animator.SetBool(Animated.DOWN_OBJECT, false);
                    }
                    else if (collectedGoods && currentCharacterAnimation != characterAnimation.DownObject)
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
                float distanceBetweenPoints = Mathf.Sqrt((nextLocation.x - gameObject.transform.position.x)
                    * (nextLocation.x - gameObject.transform.position.x) + (nextLocation.y - gameObject.transform.position.y)
                    * (nextLocation.y - gameObject.transform.position.y));
                bool nextIsGoal = false;
                if (nextLocation == goal || (network[(int)nextLocation.x, (int)nextLocation.y] != null
                    && network[(int)nextLocation.x, (int)nextLocation.y] == goalObject))
                {
                    nextIsGoal = true;
                }
                if (distanceBetweenPoints < 0.05f)
                {
                    path.RemoveAt(0);
                }
                if (path.Count == 0 || (network[(int)nextLocation.x, (int)nextLocation.y] != null
                    && network[(int)nextLocation.x, (int)nextLocation.y] == goalObject))
                {
                    //if the orc is at the storage goal, deliver resources
                    if (nextIsGoal)
                    {
                        visitedWarehouses.Add(goalObject);
                        //at the goal, trade
                        Storage storage = goalObject.GetComponent<Storage>();
                        //Checking to see if the warehouse can sell goods to the trader
                        int k = 0;
                        while (k < goodsToSell.Count)
                        {
                            string goodToSell = goodsToSell[k];
                            ResourceTrading goodResourceTrading = tradingPerResource[goodToSell];
                            TradeManager.TradeStatus goodResourceStatus = goodResourceTrading.getTradeStatus();
                            if (goodResourceStatus.Equals(TradeManager.TradeStatus.exporting) && goodResourceTrading.getTradeAmount() > 0)
                            {
                                //Sell as many as the storage has that the trader will take
                                int amountToSell = 0;
                                if (storage.getResourceCount(goodToSell) > goodResourceTrading.getTradeAmount())
                                {
                                    amountToSell = goodResourceTrading.getTradeAmount();
                                }
                                else
                                {
                                    amountToSell = storage.getResourceCount(goodToSell);
                                }
                                //Update the collector with how many more of the good can be sold
                                goodResourceTrading.setTradeAmount(goodResourceTrading.getTradeAmount() - amountToSell);
                                //Removing sold good from storage
                                storage.removeResource(goodToSell, amountToSell);
                                //Increasing city's gold from the sale
                                myWorld.updateCurrency(goodResourceTrading.getCostPerGood() * amountToSell);
                            }
                            k++;
                        }
                        //Check to see if the warehouse can buy goods from the trader
                        int storageSpace = storage.getStorageMax() - storage.getCurrentAmountStored();
                        if (storageSpace > 0)
                        {
                            k = 0;
                            while (k < goodsToBuy.Count)
                            {
                                string goodToBuy = goodsToBuy[k];
                                ResourceTrading goodResourceTrading = tradingPerResource[goodToBuy];
                                TradeManager.TradeStatus goodResourceStatus = goodResourceTrading.getTradeStatus();
                                if (goodResourceStatus.Equals(TradeManager.TradeStatus.importing)
                                    && goodResourceTrading.getCostPerGood() <= myWorld.getCurrency()
                                    && goodResourceTrading.getTradeAmount() > 0)
                                {
                                    //The amount the trader wants to buy
                                    int amountToBuy = goodResourceTrading.getTradeAmount();
                                    //The amount the storage can take based purely on space requirements
                                    if (amountToBuy > storageSpace)
                                    {
                                        amountToBuy = storageSpace;
                                    }
                                    //The cost purely to buy as much as possible without knowing how much money the player has
                                    int goodsCost = amountToBuy * goodResourceTrading.getCostPerGood();
                                    //Reducing the amount if the player doesn't have the currency to buy as much as space allows
                                    if (goodsCost > myWorld.getCurrency())
                                    {
                                        amountToBuy = Mathf.FloorToInt(myWorld.getCurrency() / goodResourceTrading.getCostPerGood());
                                        goodsCost = goodResourceTrading.getCostPerGood() * amountToBuy;
                                    }
                                    //Update the collector with how many more of the good can be bought
                                    goodResourceTrading.setTradeAmount(goodResourceTrading.getTradeAmount() - amountToBuy);
                                    //Adding bought good to storage
                                    storage.addResource(goodToBuy, amountToBuy);
                                    //Decreasing city's gold from the purchase
                                    myWorld.updateCurrency(-goodsCost);
                                }
                                k++;
                            }
                        }
                        
                        //Look into doing more trading
                        yield return StartCoroutine(findPathToStorage(returnPath =>
                        {
                            path = returnPath;
                        }));
                        //If there's no more trading, return home
                        if (path.Count == 0)
                        {
                            reachedGoal = true;
                            yield return StartCoroutine(findPathHome(returnPath =>
                            {
                                path = returnPath;
                            }));
                        }
                        //if the place of employment is destroyed, this gameobject should be as well
                        if (!placeOfEmployment || path.Count == 0)
                        {
                            Destroy(gameObject);
                        }
                    }
                    //if the orc has arrived back at its employment from collecting resources, let the employment know
                    // and update the employment's resource count
                    else if (reachedGoal)
                    {
                        placeOfEmployment.GetComponent<TradingPost>().returnTradeCollector();
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
            if (runningAStar == false && goalObject == null && !reachedGoal && !headingHome)
            {
                changePath = true;
            }
        }
        yield return null;
    }

    /// <summary>
    /// Finds a place for the collector orc to go to and creates a path to it.
    /// </summary>
    /// <param name="returnPath">a callback returning the path to the closest storage facility</param>
    /// <returns>A time delay to break method execution over multiple frames</returns>
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
                else if (!structureArr[i, j].tag.Equals(World.HOUSE))
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
                        (int)originalLocation.y - searchRadius + j].tag.Equals(World.BUILDING)
                        && !visitedWarehouses.Contains(structureArr[(int)originalLocation.x - searchRadius + i,
                        (int)originalLocation.y - searchRadius + j])
                        && !discoveredDeliveryLocs.Contains(structureArr[(int)originalLocation.x - searchRadius + i,
                        (int)originalLocation.y - searchRadius + j])
                        && structureArr[(int)originalLocation.x - searchRadius + i,
                        (int)originalLocation.y - searchRadius + j].GetComponent<Employment>().getNumWorkers() > 0
                        && structureArr[(int)originalLocation.x - searchRadius + i,
                        (int)originalLocation.y - searchRadius + j].GetComponent<Storage>() != null
                        && structureArr[(int)originalLocation.x - searchRadius + i,
                        (int)originalLocation.y - searchRadius + j].GetComponent<Storage>().storageType.Equals(World.WAREHOUSE))
                {
                    bool canTrade = false;
                    Storage warehouseStorage = structureArr[(int)originalLocation.x - searchRadius + i,
                        (int)originalLocation.y - searchRadius + j].GetComponent<Storage>();
                    //Checking to see if the warehouse can sell goods to the trader
                    int k = 0;
                    while (k < goodsToSell.Count && !canTrade)
                    {
                        string goodToSell = goodsToSell[k];
                        ResourceTrading goodResourceTrading = tradingPerResource[goodToSell];
                        TradeManager.TradeStatus goodResourceStatus = goodResourceTrading.getTradeStatus();
                        if (goodResourceStatus.Equals(TradeManager.TradeStatus.exporting) && goodResourceTrading.getTradeAmount() > 0)
                        {
                            if (warehouseStorage.getResourceCount(goodToSell) > 0)
                            {
                                canTrade = true;
                            }
                        }
                        k++;
                    }
                    //Check to see if the warehouse can buy goods from the trader
                    if (!canTrade && warehouseStorage.getStorageMax() - warehouseStorage.getCurrentAmountStored() > 0)
                    {
                        k = 0;
                        while (k < goodsToBuy.Count && !canTrade)
                        {
                            string goodToBuy = goodsToBuy[k];
                            ResourceTrading goodResourceTrading = tradingPerResource[goodToBuy];
                            TradeManager.TradeStatus goodResourceStatus = goodResourceTrading.getTradeStatus();
                            if (goodResourceStatus.Equals(TradeManager.TradeStatus.importing)
                                && goodResourceTrading.getCostPerGood() <= myWorld.getCurrency()
                                && goodResourceTrading.getTradeAmount() > 0)
                            {
                                canTrade = true;
                            }
                            k++;
                        }
                    }

                    if (canTrade)
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
        //check for other storage-type buildings if there are no available warehouses
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

    /// <summary>
    /// Finds a way back to the building that spawned the delivery orc.
    /// </summary>
    /// <param name="returnPath">a callback returning the path back to the original location</param>
    /// <returns>A time delay to break method execution over multiple frames</returns>
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
        Vector2 homeLocation = findRoadNextToEmployment();
        runningAStar = true;
        yield return StartCoroutine(aStarSearch.aStar(tempPath =>
        {
            runningAStar = false;
            returnPath(tempPath);
        }, new Vector2(Mathf.RoundToInt(location.x), Mathf.RoundToInt(location.y)),
            network[(int)homeLocation.x, (int)homeLocation.y], network));
        if (runningAStar == false)
        {
            yield break;
        }
        yield return new WaitForSeconds(0.05f);
    }

    /// <summary>
    /// TODO: this should really be used in a bunch of places and should therefore be in a class that many other classes can access
    /// </summary>
    /// <returns></returns>
    public Vector2 findRoadNextToEmployment()
    {
        World myWorld = GameObject.Find(World.WORLD_INFORMATION).GetComponent<World>();
        GameObject[,] structArr = myWorld.constructNetwork.getConstructArr();
        int width = (int)placeOfEmployment.GetComponent<BoxCollider2D>().size.x;
        int height = (int)placeOfEmployment.GetComponent<BoxCollider2D>().size.y;
        //checking areas around the farm to place an orc on a road
        Vector2 employmentPos = placeOfEmployment.transform.position;
        bool foundSpawn = false;
        Vector2 spawnPosition = new Vector2();
        int i = 0;
        while (!foundSpawn && i < width)
        {
            //checking the row below the gameObject
            if (!foundSpawn && structArr[(Mathf.FloorToInt(employmentPos.x) - Mathf.CeilToInt(width / 2.0f - 1) + i),
                (Mathf.FloorToInt(employmentPos.y) - Mathf.CeilToInt(height / 2.0f - 1) - 1)] != null
                && structArr[(Mathf.FloorToInt(employmentPos.x) - Mathf.CeilToInt(width / 2.0f - 1) + i),
                (Mathf.FloorToInt(employmentPos.y) - Mathf.CeilToInt(height / 2.0f - 1) - 1)].tag.Equals(World.ROAD))
            {
                spawnPosition = new Vector2((Mathf.FloorToInt(employmentPos.x) - Mathf.CeilToInt(width / 2.0f - 1) + i),
                    (Mathf.FloorToInt(employmentPos.y) - Mathf.CeilToInt(height / 2.0f - 1) - 1));
                foundSpawn = true;
            }
            //checking the row above the gameObject
            else if (!foundSpawn && structArr[(Mathf.FloorToInt(employmentPos.x) - Mathf.CeilToInt(width / 2.0f - 1) + i),
                (Mathf.CeilToInt(employmentPos.y) + Mathf.CeilToInt(height / 2.0f - 1) + 1)] != null
                && structArr[(Mathf.FloorToInt(employmentPos.x) - Mathf.CeilToInt(width / 2.0f - 1) + i),
                (Mathf.CeilToInt(employmentPos.y) + Mathf.CeilToInt(height / 2.0f - 1) + 1)].tag.Equals(World.ROAD))
            {
                spawnPosition = new Vector2((Mathf.FloorToInt(employmentPos.x) - Mathf.CeilToInt(width / 2.0f - 1) + i),
                    (Mathf.CeilToInt(employmentPos.y) + Mathf.CeilToInt(height / 2.0f - 1) + 1));
                foundSpawn = true;
            }
            i++;
        }
        int j = 0;
        while (!foundSpawn && j < height)
        {
            //checking the column to the left of the gameObject
            if (!foundSpawn && structArr[(Mathf.FloorToInt(employmentPos.x) - Mathf.CeilToInt(width / 2.0f - 1) - 1),
                (Mathf.FloorToInt(employmentPos.y) - Mathf.CeilToInt(height / 2.0f - 1) + j)] != null
                && structArr[(Mathf.FloorToInt(employmentPos.x) - Mathf.CeilToInt(width / 2.0f - 1) - 1),
                (Mathf.FloorToInt(employmentPos.y) - Mathf.CeilToInt(height / 2.0f - 1) + j)].tag.Equals(World.ROAD))
            {
                spawnPosition = new Vector2((Mathf.FloorToInt(employmentPos.x) - Mathf.CeilToInt(width / 2.0f - 1) - 1),
                    (Mathf.FloorToInt(employmentPos.y) - Mathf.CeilToInt(height / 2.0f - 1) + j));
                foundSpawn = true;
            }
            //checking the column to the right of the gameObject
            else if (!foundSpawn && structArr[(Mathf.FloorToInt(employmentPos.x) + Mathf.CeilToInt(width / 2.0f - 0.5f) + 1),
                (Mathf.FloorToInt(employmentPos.y) - Mathf.CeilToInt(height / 2.0f - 1) + j)] != null
                && structArr[(Mathf.FloorToInt(employmentPos.x) + Mathf.CeilToInt(width / 2.0f - 0.5f) + 1),
                (Mathf.FloorToInt(employmentPos.y) - Mathf.CeilToInt(height / 2.0f - 1) + j)].tag.Equals(World.ROAD))
            {
                spawnPosition = new Vector2((Mathf.FloorToInt(employmentPos.x) + Mathf.CeilToInt(width / 2.0f - 0.5f) + 1),
                    (Mathf.FloorToInt(employmentPos.y) - Mathf.CeilToInt(height / 2.0f - 1) + j));
                foundSpawn = true;
            }
            j++;
        }
        return spawnPosition;
    }

    /// <summary>
    /// Sets the place of employment this delivery orc works for.
    /// </summary>
    /// <param name="employment">the place of employment</param>
    public void setOrcEmployment(GameObject employment)
    {
        placeOfEmployment = employment;
    }

    public void setImportsAndExports(List<string> imports, List<string> exports)
    {
        this.goodsToSell = imports;
        this.goodsToBuy = exports;
    }

    public void setTradingPerResource(Dictionary<string, ResourceTrading> tradingPerResource)
    {
        this.tradingPerResource = new Dictionary<string, ResourceTrading>();
        foreach (string key in tradingPerResource.Keys)
        {
            this.tradingPerResource.Add(key, tradingPerResource[key].deepCopy());
        }
    }
}
