using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the trade in the game
/// </summary>
public class TradeManager
{
    private World myWorld;
    private Dictionary<string, TradeCityObject> tradeCities;
    private List<TradeCityObject> activeTradeRoutes;
    public enum TradeStatus
    {
        notTrading,
        importing,
        exporting
    }
    private Dictionary<string, ResourceTrading> tradingPerResource;
    private GameObject trader;
    private float lastTraderSpawnTime;
    
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="myWorld">The world this TradeManager is tied to</param>
    /// <param name="tradingPerResource">The trading for each resource</param>
    /// <param name="trader">The trader prefab</param>
    public TradeManager(World myWorld, Dictionary<string, ResourceTrading> tradingPerResource, GameObject trader)
    {
        this.myWorld = myWorld;
        tradeCities = new Dictionary<string, TradeCityObject>();
        activeTradeRoutes = new List<TradeCityObject>();
        this.tradingPerResource = tradingPerResource;
        this.trader = trader;
        lastTraderSpawnTime = 0.0f;
    }

    /// <summary>
    /// Manages the trade routes in the world
    /// </summary>
    public void manageTradeRoutes()
    {
        //TODO: is there a smarter/more efficient way of doing this?
        //Goes through the trade routes to determine if a trader should spawn
        foreach (TradeCityObject tradeCityObject in activeTradeRoutes)
        {
            //5.0f is an arbitrary time I chose to spread out the spawn times for traders
            if (!tradeCityObject.getTraderInPlayerCity() && myWorld.getGameTime() > tradeCityObject.getTraderArrivalTime() && Time.time - lastTraderSpawnTime > 5.0f)
            {
                GameObject newTrader = Object.Instantiate(trader, myWorld.spawnLocation, Quaternion.identity);
                Trade newTraderTrade = newTrader.GetComponent<Trade>();
                newTraderTrade.setMyTradeCityObject(tradeCityObject);
                tradeCityObject.setTraderInPlayerCity(true);
                lastTraderSpawnTime = Time.time;
            }
        }
    }
    
    /// <summary>
    /// Adds a potential city to trade with
    /// </summary>
    /// <param name="tradeCityObject">The potential city to trade with</param>
    public void addTradeCity(TradeCityObject tradeCityObject)
    {
        tradeCities.Add(tradeCityObject.getCityName(), tradeCityObject);
    }

    /// <summary>
    /// Gets a information on a city from its name
    /// </summary>
    /// <param name="tradeCityName">The name of the city to get information on</param>
    /// <returns>Information on a city from its name</returns>
    public TradeCityObject getTradeCityObject(string tradeCityName)
    {
        return tradeCities[tradeCityName];
    }

    /// <summary>
    /// Adds a city that the player has opened a trade route with
    /// </summary>
    /// <param name="tradeCityObject">The city the player has opened a trade route with</param>
    public void addActiveTradeRoute(TradeCityObject tradeCityObject)
    {
        if (!activeTradeRoutes.Contains(tradeCityObject))
        {
            tradeCityObject.setTraderTimeTravelingStarted(myWorld.getGameTime());
            activeTradeRoutes.Add(tradeCityObject);
        }
    }

    /// <summary>
    /// Sets the max amount of goods a trader can trade per visit
    /// </summary>
    /// <param name="resourceName">The resource a trade limit is being imposed on</param>
    /// <param name="maxAmount">The maximum amount of the resource that can be traded</param>
    public void setMaxPerTrader(string resourceName, int maxAmount)
    {
        tradingPerResource[resourceName].setTradeAmount(maxAmount);
    }

    /// <summary>
    /// Sets the trade status of a resource
    /// </summary>
    /// <param name="resourceName">The resource we're updating the trade status of</param>
    /// <param name="tradeStatus">The trade status for the resource</param>
    public void setTradeStatus(string resourceName, TradeStatus tradeStatus)
    {
        tradingPerResource[resourceName].setTradeStatus(tradeStatus);
    }

    /// <summary>
    /// Gets the tradingPerResource
    /// </summary>
    /// <returns>The tradingPerResource</returns>
    public Dictionary<string, ResourceTrading> getTradingPerResource()
    {
        return tradingPerResource;
    }
}
