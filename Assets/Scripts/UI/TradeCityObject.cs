using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Information on a city that can be traded with
/// </summary>
public class TradeCityObject : WorldPopupButton
{
    public int tradeRouteCost;
    public List<string> imports;
    public List<string> exports;
    public List<GameObject> tradeRouteMarkers;
    private bool tradeRouteOpened;
    private bool traderInPlayerCity;
    private float traderTimeTravelingStarted;
    private float traderTravelTime;

    /// <summary>
    /// Initialization
    /// </summary>
    private void Start()
    {
        TradeManager myTradeManager = GameObject.Find(World.WORLD_INFORMATION).GetComponent<World>().getTradeManager();
        myTradeManager.addTradeCity(this);
        tradeRouteOpened = false;
        traderInPlayerCity = false;
        traderTimeTravelingStarted = 0.0f;
        traderTravelTime = 10.0f;//TODO: I probably want this at 3min or longer once I confirm it's working
    }

    /// <summary>
    /// Gets whether a trade route has been opened with this city
    /// </summary>
    /// <returns>Whether a trade route is open with this city</returns>
    public bool getTradeRouteOpened()
    {
        return tradeRouteOpened;
    }

    /// <summary>
    /// Sets whether a trade route is open with this city
    /// </summary>
    /// <param name="opened">Whether the trade route is open</param>
    public void setTradeRouteOpened(bool opened)
    {
        tradeRouteOpened = opened;
    }

    /// <summary>
    /// Gets the name of the city
    /// </summary>
    /// <returns>The name of the city</returns>
    public string getCityName()
    {
        return this.worldItemName;
    }

    /// <summary>
    /// Sets whether a trader from this city is in the player's city
    /// </summary>
    /// <param name="inCity">Whether a trader from this city is in the player's city</param>
    public void setTraderInPlayerCity(bool inCity)
    {
        traderInPlayerCity = inCity;
    }

    /// <summary>
    /// Gets whether a trader from this city is in the player's city
    /// </summary>
    /// <returns>Whether a trader from this city is in the player's city</returns>
    public bool getTraderInPlayerCity()
    {
        return traderInPlayerCity;
    }

    /// <summary>
    /// Sets the earliest time a trader from this city will visit the player's city
    /// </summary>
    /// <param name="time"></param>
    public void setTraderTimeTravelingStarted(float time)
    {
        traderTimeTravelingStarted = time;
    }

    /// <summary>
    /// Gets the goods this city will import
    /// </summary>
    /// <returns>The goods this city will import</returns>
    public List<string> getImports()
    {
        return imports;
    }

    /// <summary>
    /// Gets the goods this city will export
    /// </summary>
    /// <returns>The goods this city will export</returns>
    public List<string> getExports()
    {
        return exports;
    }

    /// <summary>
    /// Gets the time the trader should arrive at the player's city
    /// </summary>
    /// <returns>The time the trader should arrive at the player's city</returns>
    public float getTraderArrivalTime()
    {
        return traderTimeTravelingStarted + traderTravelTime;
    }
}
