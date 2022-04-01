using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private void Start()
    {
        TradeManager myTradeManager = GameObject.Find(World.WORLD_INFORMATION).GetComponent<World>().getTradeManager();
        myTradeManager.addTradeCity(this);
        tradeRouteOpened = false;
        traderInPlayerCity = false;
        traderTimeTravelingStarted = 0.0f;
        traderTravelTime = 10.0f;//TODO: I probably want this at 3min or longer once I confirm it's working
    }

    public bool getTradeRouteOpened()
    {
        return tradeRouteOpened;
    }

    public void setTradeRouteOpened(bool opened)
    {
        tradeRouteOpened = opened;
    }

    public string getCityName()
    {
        return this.worldItemName;
    }

    public void setTraderInPlayerCity(bool inCity)
    {
        traderInPlayerCity = inCity;
    }

    public bool getTraderInPlayerCity()
    {
        return traderInPlayerCity;
    }

    public void setTraderTimeTravelingStarted(float time)
    {
        traderTimeTravelingStarted = time;
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
