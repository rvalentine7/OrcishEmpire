using System.Collections;
using System.Collections.Generic;

public class TradeManager
{
    private Dictionary<string, TradeCityObject> tradeCities;
    private List<TradeCityObject> activeTradeRoutes;

    public TradeManager(World myWorld)
    {
        tradeCities = new Dictionary<string, TradeCityObject>();
        activeTradeRoutes = new List<TradeCityObject>();
    }

    public void manageTradeRoutes()
    {
        //TODO: do management of activeTradeRoutes. call this from World's update method
        /*
         This is where timers would occur for determining if a trader should spawn.  
         Respawn timers for traders only start when a trade route is first opened and whenever the trader for the particular trade city leaves the map (after visiting)
         
         */
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
            activeTradeRoutes.Add(tradeCityObject);
        }
    }
}
