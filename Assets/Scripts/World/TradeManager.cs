using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    

    public TradeManager(World myWorld, Dictionary<string, ResourceTrading> tradingPerResource, GameObject trader)
    {
        this.myWorld = myWorld;
        tradeCities = new Dictionary<string, TradeCityObject>();
        activeTradeRoutes = new List<TradeCityObject>();
        this.tradingPerResource = tradingPerResource;
        this.trader = trader;
        lastTraderSpawnTime = 0.0f;
    }

    public void manageTradeRoutes()
    {
        /*
         This is where timers would occur for determining if a trader should spawn.  
         Respawn timers for traders only start when a trade route is first opened and whenever the trader for the particular trade city leaves the map (after visiting)
         
        When a trader tries to trade, make sure to reference tradingPerResource to see the max amount they can buy/sell (if there is available supply/storage space)
         */
        //TODO: is there a smarter/more efficient way of doing this?
        foreach(TradeCityObject tradeCityObject in activeTradeRoutes)
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
    
    public void addTradeCity(TradeCityObject tradeCityObject)
    {
        tradeCities.Add(tradeCityObject.getCityName(), tradeCityObject);
    }

    public TradeCityObject getTradeCityObject(string tradeCityName)
    {
        return tradeCities[tradeCityName];
    }

    public void addActiveTradeRoute(TradeCityObject tradeCityObject)
    {
        if (!activeTradeRoutes.Contains(tradeCityObject))
        {
            tradeCityObject.setTraderTimeTravelingStarted(myWorld.getGameTime());
            activeTradeRoutes.Add(tradeCityObject);
        }
    }

    public void setMaxPerTrader(string resourceName, int maxAmount)
    {
        tradingPerResource[resourceName].setTradeAmount(maxAmount);
    }

    public void setTradeStatus(string resourceName, TradeStatus tradeStatus)
    {
        tradingPerResource[resourceName].setTradeStatus(tradeStatus);
    }
}
