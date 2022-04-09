using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Logic for a trading post
/// </summary>
public class TradingPost : Building
{
    public GameObject tradeGoodsCollector;
    private List<Trade> traders;
    private List<Trade> waitingTraders;
    private bool collectingTradeGoods;
    //private float testTime = 0.0f;

    /// <summary>
    /// Initialization
    /// </summary>
    void Start()
    {
        traders = new List<Trade>();
        waitingTraders = new List<Trade>();
        collectingTradeGoods = false;
    }

    /// <summary>
    /// Manages the logic of the trading post
    /// </summary>
    void Update()
    {
        if (!collectingTradeGoods && waitingTraders.Count > 0)
        {
            collectingTradeGoods = true;
            createTradeCollector();
        }
    }

    /// <summary>
    /// Gets the number of traders at or going to this trading post
    /// </summary>
    /// <returns>The number of traders at or going to this trading post</returns>
    public int getNumTraders()
    {
        return traders.Count;
    }

    /// <summary>
    /// Gets a trader that is coming to this trading post
    /// </summary>
    /// <param name="trade">The logic for the trader being received</param>
    public void receiveTrader(Trade trade)
    {
        if (!traders.Contains(trade))
        {
            traders.Add(trade);
        }
    }

    /// <summary>
    /// Gets a trader that has arrived at this trading post
    /// </summary>
    /// <param name="trade">The logic for the trader that has arrived at this trading post</param>
    public void traderArrived(Trade trade)
    {
        if (!waitingTraders.Contains(trade))
        {
            waitingTraders.Add(trade);
        }
    }

    /// <summary>
    /// Gets the position the trader is waiting in line
    /// </summary>
    /// <param name="trade">The trader we're checking the position of</param>
    /// <returns>The trader's position in line</returns>
    public int getTraderWaitPosition(Trade trade)
    {
        return waitingTraders.Contains(trade) ? waitingTraders.IndexOf(trade) : waitingTraders.Count;
    }

    /// <summary>
    /// Creates an orc to retrieve resources from warehouses for the TradingPost.
    /// This building favors placing an orc at the first available road segment
    /// it finds in the order of: bottom, top, left, right
    /// </summary>
    private void createTradeCollector()
    {
        World myWorld = GameObject.Find(World.WORLD_INFORMATION).GetComponent<World>();
        GameObject[,] structArr = myWorld.constructNetwork.getConstructArr();
        int width = (int)gameObject.GetComponent<BoxCollider2D>().size.x;
        int height = (int)gameObject.GetComponent<BoxCollider2D>().size.y;
        //checking areas around the farm to place an orc on a road
        Vector2 employmentPos = gameObject.transform.position;
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

        GameObject newTradeGoodsCollector = Object.Instantiate(tradeGoodsCollector, new Vector2(spawnPosition.x, spawnPosition.y + 0.4f), Quaternion.identity);
        CollectTradeGoods collectTradeGoods = newTradeGoodsCollector.GetComponent<CollectTradeGoods>();
        TradeCityObject tradeCity = waitingTraders[0].GetComponent<Trade>().getTraderCityObject();
        collectTradeGoods.setSellingAndBuying(tradeCity.getImports(), tradeCity.getExports());
        collectTradeGoods.setTradingPerResource(myWorld.getTradeManager().getTradingPerResource());
        collectTradeGoods.setOrcEmployment(gameObject);
    }

    /// <summary>
    /// Notifies this trading post that its trade collector has returned
    /// </summary>
    public void returnTradeCollector()
    {
        collectingTradeGoods = false;

        waitingTraders[0].setFinishedTrading(true);
        waitingTraders.RemoveAt(0);
        traders.RemoveAt(0);

        foreach (Trade trade in waitingTraders)
        {
            trade.moveUpInWaitLine();
        }
    }

    /// <summary>
    /// Lets the traders know the trading post has been destroyed
    /// </summary>
    private new void OnDestroy()
    {
        foreach (Trade trade in traders)
        {
            trade.receiveTradingPostDestruction();
        }

        base.OnDestroy();
    }

    /// <summary>
    /// Returns whether the trading post is currently helping a trader buy/sell
    /// </summary>
    /// <returns>Whether the trading post is currently helping a trader buy/sell</returns>
    public bool getCollectingTradeGoods()
    {
        return collectingTradeGoods;
    }
}
